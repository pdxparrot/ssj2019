using System;
using System.Collections;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
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
        public bool HasPath => _agent.hasPath;

        public Vector3 NextPosition => _agent.nextPosition;
#endregion

        [CanBeNull]
        private PooledObject _pooledObject;

        private NavMeshAgent _agent;

        private Coroutine _agentStuckCheck;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastStuckCheckPosition;

        [SerializeField]
        [ReadOnly]
        private int _stuckCheckCount;

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
        public bool UpdatePath(Vector3 target)
        {
            if(!_agent.SetDestination(target)) {
                Debug.LogWarning($"Failed to set NPC destination: {target}");
                return false;
            }
            
            // TODO: whenever NPCManager moves to Game,
            // we can do this when DebugBehavior is true
            //Debug.Log($"NPC {Id} updating path from {Behavior.Movement.Position} to {target}");

            if(null == _agentStuckCheck) {
                _agentStuckCheck = StartCoroutine(AgentStuckCheck());
            }

            return true;
        }

        public void ResetPath()
        {
            if(null != _agentStuckCheck) {
                StopCoroutine(_agentStuckCheck);
                _agentStuckCheck = null;
            }

            _agent.ResetPath();

            NPCBehavior.OnIdle();
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
            } else if(null != _agentStuckCheck) {
                StopCoroutine(_agentStuckCheck);
                _agentStuckCheck = null;

                NPCBehavior.OnIdle();
            }
        }

        public void Recycle()
        {
            NPCBehavior.OnRecycle();
            if(null != _pooledObject) {
                _pooledObject.Recycle();
            }
        }

        private IEnumerator AgentStuckCheck()
        {
            _lastStuckCheckPosition = Behavior.Movement.Position;
            _stuckCheckCount = 0;

            // TODO: make this configurable
            WaitForSeconds wait = new WaitForSeconds(0.1f);
            while(_agent.pathPending || _agent.hasPath) {
                // wait until we have a path
                if(_agent.pathPending) {
                    _stuckCheckCount = 0;

                    yield return wait;
                    continue;
                }

                // see if we've moved at least our stopping distance
                Vector3 position = Behavior.Movement.Position;
                if((position - _lastStuckCheckPosition).sqrMagnitude < _agent.stoppingDistance) {
                    _stuckCheckCount += 1;
                } else {
                    _stuckCheckCount = 0;
                }
                _lastStuckCheckPosition = position;

                // are we stuck?
                // TODO: make this configurable
                if(_stuckCheckCount >= 2) {
                    // TODO: whenever NPCManager moves to Game,
                    // we can do this when DebugBehavior is true
                    //Debug.Log($"NPC {Id} is stuck");

                    Stop(true);
                    yield break;
                }

                yield return wait;
            }

            _agentStuckCheck = null;
        }

#region Event Handlers
        private void RecycleEventHandler(object sender, EventArgs args)
        {
            OnDeSpawn();
        }
#endregion
    }
}
