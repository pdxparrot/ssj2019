using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Core.Actors
{
    public class ActorAnimator : MonoBehaviour
    {
        [Serializable]
        protected struct AnimationState
        {
            public bool IsAnimating;

            public float AnimationSeconds;
            public float AnimationSecondsRemaining;

            public float PercentComplete => 1.0f - (AnimationSecondsRemaining / AnimationSeconds);

            public bool IsFinished => AnimationSecondsRemaining <= 0.0f;

            public Vector3 StartPosition;
            public Vector3 EndPosition;

            public Quaternion StartRotation;
            public Quaternion EndRotation;

            public bool IsKinematic;

            public Action OnComplete;
        }

        [SerializeField]
        private ActorBehavior _behavior;

        protected ActorBehavior Behavior => _behavior;

        [SerializeField]
        [ReadOnly]
        protected AnimationState _animationState;

        public virtual bool IsAnimating => _animationState.IsAnimating;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsNotNull(Behavior);
        }

        protected void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            UpdateAnimation(dt);
        }
#endregion

        public virtual void StartAnimation(Vector3 targetPosition, Quaternion targetRotation, float timeSeconds, Action onComplete=null)
        {
            if(IsAnimating) {
                return;
            }

            if(ActorManager.Instance.EnableDebug) {
                Debug.Log($"Starting manual animation from {Behavior.Movement.Position}:{Behavior.Movement.Rotation} to {targetPosition}:{targetRotation} over {timeSeconds} seconds");
            }

            _animationState.IsAnimating = true;

            _animationState.StartPosition = Behavior.Movement.Position;
            _animationState.EndPosition = targetPosition;

            _animationState.StartRotation = Behavior.Movement.Rotation;
            _animationState.EndRotation = targetRotation;

            _animationState.AnimationSeconds = timeSeconds;
            _animationState.AnimationSecondsRemaining = timeSeconds;

            _animationState.IsKinematic = Behavior.Movement.IsKinematic;
            Behavior.Movement.IsKinematic = true;

            _animationState.OnComplete = onComplete;
        }

        protected virtual void UpdateAnimation(float dt)
        {
            if(!IsAnimating || PartyParrotManager.Instance.IsPaused) {
                return;
            }

            Profiler.BeginSample("ActorAnimator.UpdateAnimation");
            try {
                if(_animationState.IsFinished) {
                    if(ActorManager.Instance.EnableDebug) {
                        Debug.Log("Actor animation complete!");
                    }

                    _animationState.IsAnimating = false;

                    Behavior.Movement.Position = _animationState.EndPosition;
                    Behavior.Movement.Rotation = _animationState.EndRotation;
                    Behavior.Movement.IsKinematic = _animationState.IsKinematic;

                    _animationState.OnComplete?.Invoke();
                    _animationState.OnComplete = null;

                    return;
                }

                _animationState.AnimationSecondsRemaining -= dt;
                if(_animationState.AnimationSecondsRemaining < 0.0f) {
                    _animationState.AnimationSecondsRemaining = 0.0f;
                }

                Behavior.Movement.Position = Vector3.Slerp(_animationState.StartPosition, _animationState.EndPosition, _animationState.PercentComplete);
                Behavior.Movement.Rotation = Quaternion.Slerp(_animationState.StartRotation, _animationState.EndRotation, _animationState.PercentComplete);
            } finally {
                Profiler.EndSample();
            }
        }
    }
}
