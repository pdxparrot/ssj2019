﻿using System;
using System.Collections;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    public class GroundCheckBehaviorComponent3D : CharacterBehaviorComponent3D
    {
#region Events
        public event EventHandler<EventArgs> SlopeLimitEvent;
#endregion

        [SerializeField]
        private GroundCheckBehaviorComponentData _data;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _groundedEffect;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private RaycastHit[] _groundCheckHits = new RaycastHit[4];

        [SerializeField]
        [ReadOnly]
        private bool _didGroundCheckCollide;

        public bool DidGroundCheckCollide => _didGroundCheckCollide;

        [SerializeField]
        [ReadOnly]
        private Vector3 _groundCheckNormal;

        [SerializeField]
        [ReadOnly]
        private float _groundCheckMinDistance;

        [SerializeField]
        [ReadOnly]
        private float _groundSlope;

        private float GroundCheckRadius => Behavior.Owner.Height - 0.1f;

        private Vector3 GroundCheckCenter => Behavior.Movement3D.Position + (GroundCheckRadius * Vector3.up);

        private Coroutine _raycastCoroutine;

#region Unity Lifecycle
        private void OnEnable()
        {
            StartRoutine();
        }

        private void OnDisable()
        {
            if(null != _raycastCoroutine) {
                StopCoroutine(_raycastCoroutine);
            }
            _raycastCoroutine = null;
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

            Gizmos.color = Behavior.IsGrounded ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(GroundCheckCenter + (_data.GroundedEpsilon * Vector3.down), GroundCheckRadius);

            Gizmos.color = _didGroundCheckCollide ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(GroundCheckCenter + (_data.GroundCheckLength * Vector3.down), GroundCheckRadius);
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            StartRoutine();
        }

        public override bool OnPhysicsUpdate(float dt)
        {
            if(!Behavior.IsGrounded || _groundSlope < _data.SlopeLimit) {
                return false;
            }

            float dp = Vector3.Dot(Behavior.Owner.transform.forward, _groundCheckNormal);
            if(dp >= 0.0f) {
                return false;
            }

            SlopeLimitEvent?.Invoke(this, EventArgs.Empty);

            return false;
        }

        private void StartRoutine()
        {
            if(null != _raycastCoroutine) {
                return;
            }

            if(Behavior.IsInitialized && !Behavior.CharacterBehaviorData.IsKinematic) {
                _raycastCoroutine = StartCoroutine(RaycastRoutine());
            }
        }

        private IEnumerator RaycastRoutine()
        {
            Debug.Log($"Starting ground check raycast routine for {Behavior.Owner.Id}");

            WaitForSeconds wait = new WaitForSeconds(_data.RaycastRoutineRate);
            while(true) {
                UpdateIsGrounded();

                yield return wait;
            }
        }

        private void UpdateIsGrounded()
        {
            Profiler.BeginSample("CharacterBehaviorGroundedChecker.UpdateIsGrounded");
            try {
                bool wasGrounded = Behavior.IsGrounded;

                _didGroundCheckCollide = CheckIsGrounded(out _groundCheckMinDistance);

                if(Behavior.Movement3D.IsKinematic) {
                    // something else is handling this case?
                } else {
                    Behavior.IsGrounded = _didGroundCheckCollide && _groundCheckMinDistance < _data.GroundedEpsilon;
                }

                // if we're on a slope, we're sliding down it
                Behavior.IsSliding = _groundSlope >= _data.SlopeLimit;

                if(!wasGrounded && Behavior.IsGrounded) {
                    if(null != _groundedEffect) {
                        _groundedEffect.Trigger();
                    }
                }
            } finally {
                Profiler.EndSample();
            }
        }

        private bool CheckIsGrounded(out float minDistance)
        {
            minDistance = float.MaxValue;

            Vector3 origin = GroundCheckCenter;

            int hitCount = Physics.SphereCastNonAlloc(origin, GroundCheckRadius, Vector3.down, _groundCheckHits, _data.GroundCheckLength, Behavior.CharacterBehaviorData.CollisionCheckLayerMask, QueryTriggerInteraction.Ignore);
            if(hitCount < 1) {
                // no slope if not grounded
                _groundSlope = 0;
                return false;
            }

            // figure out the slope of whatever we hit
            _groundCheckNormal = Vector3.zero;
            for(int i=0; i<hitCount; ++i) {
                _groundCheckNormal += _groundCheckHits[i].normal;
                minDistance = Mathf.Min(minDistance, _groundCheckHits[i].distance);
            }
            _groundCheckNormal /= hitCount;

            _groundSlope = Vector3.Angle(Vector3.up, _groundCheckNormal);

            return true;
        }
    }
}
