using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class NPC3D : Actor3D, INPC
    {
        public GameObject GameObject => gameObject;

#region Network
        public override bool IsLocalActor => false;
#endregion

#region Behavior
        public NPCBehavior NPCBehavior => (NPCBehavior)Behavior;
#endregion

#region Pathing
        public Vector3 NextPosition => _agent.nextPosition;
#endregion

        [CanBeNull]
        private PooledObject _pooledObject;

        private NavMeshAgent _agent;

#if UNITY_EDITOR
        private LineRenderer _debugPathRenderer;
#endif

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Behavior is NPCBehavior);

            _agent = GetComponent<NavMeshAgent>();

            _pooledObject = GetComponent<PooledObject>();
            if(null != _pooledObject) {
                _pooledObject.RecycleEvent += RecycleEventHandler;
            }

#if UNITY_EDITOR
            _debugPathRenderer = gameObject.AddComponent<LineRenderer>();
            _debugPathRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _debugPathRenderer.receiveShadows = false;
            _debugPathRenderer.allowOcclusionWhenDynamic = false;
            _debugPathRenderer.startWidth = 0.1f;
            _debugPathRenderer.endWidth = 0.1f;
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            DebugRenderPath();
#endif
        }

        private void LateUpdate()
        {
            // TODO: this works great except that
            // the player can abuse the NPC re-accelerating
            // by pausing and unpausing the game
            if(!NPCBehavior.CanMove) {
                Stop(false);
            }
        }
#endregion

        public override void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBehaviorData);

            base.Initialize(id, behaviorData);

            _agent.speed = NPCBehavior.NPCBehaviorData.MoveSpeed;
            _agent.angularSpeed = NPCBehavior.NPCBehaviorData.AngularMoveSpeed;
            _agent.acceleration = NPCBehavior.NPCBehaviorData.MoveAcceleration;
            _agent.stoppingDistance = Radius + NPCBehavior.NPCBehaviorData.StoppingDistance;
            _agent.autoBraking = true;

            _agent.radius = Radius;
            _agent.height = Height;
        }

#region Pathing
        public void UpdatePath(Vector3 target)
        {
            if(!_agent.SetDestination(target)) {
                Debug.LogWarning($"Failed to set NPC destination: {target}");
                return;
            }

            if(_agent.pathPending) {
                // TODO: whenever NPCManager moves to Game,
                // we can do this when DebugBehavior is true
                //Debug.Log($"NPC {Id} updating path from {Behavior.Movement.Position} to {target}");
            }
        }

        public void ResetPath()
        {
            _agent.ResetPath();
        }

#if UNITY_EDITOR
        private void DebugRenderPath()
        {
            if(!_agent.hasPath) {
                _debugPathRenderer.positionCount = 0;
                return;
            }

            _debugPathRenderer.positionCount = _agent.path.corners.Length;
            for(int i=0; i<_agent.path.corners.Length; ++i) {
                _debugPathRenderer.SetPosition(i, _agent.path.corners[i]);
            }
        }
#endif
#endregion

        public void Stop(bool resetPath)
        {
            _agent.velocity = Vector3.zero;

            if(resetPath) {
                ResetPath();
            }
        }

        public void Recycle()
        {
            NPCBehavior.OnRecycle();
            if(null != _pooledObject) {
                _pooledObject.Recycle();
            }
        }

#region Event Handlers
        private void RecycleEventHandler(object sender, EventArgs args)
        {
            OnDeSpawn();
        }
#endregion
    }
}
