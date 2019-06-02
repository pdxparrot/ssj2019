using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Animation;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public abstract class ActorBehavior : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        public Actor Owner => _owner;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        protected ActorBehaviorData _behaviorData;

        [CanBeNull]
        public ActorBehaviorData BehaviorData => _behaviorData;

        [Space(10)]

#region Movement
        [Header("Movement")]

        [SerializeField]
        private ActorMovement _movement;

        public ActorMovement Movement => _movement;

        public bool IsMoving { get; protected set; }

        public virtual bool CanMove => !PartyParrotManager.Instance.IsPaused && (null == _actorAnimator || !_actorAnimator.IsAnimating);
#endregion

        [Space(10)]

#region Animation
        [Header("Animation")]

#if USE_SPINE
        [SerializeField]
        [CanBeNull]
        private SpineAnimationHelper _spineAnimationHelper;

        [CanBeNull]
        public SpineAnimationHelper SpineAnimationHelper => _spineAnimationHelper;
#endif

        [SerializeField]
        [CanBeNull]
        private SpriteAnimationHelper _spriteAnimationHelper;

        [CanBeNull]
        public SpriteAnimationHelper SpriteAnimationHelper => _spriteAnimationHelper;

        [SerializeField]
        [CanBeNull]
        private Animator _animator;

        [CanBeNull]
        public Animator Animator => _animator;

        [SerializeField]
        [CanBeNull]
        private ActorAnimator _actorAnimator;

        [CanBeNull]
        public ActorAnimator ActorAnimator => _actorAnimator;

        [SerializeField]
        private bool _pauseAnimationOnPause = true;

        protected bool PauseAnimationOnPause => _pauseAnimationOnPause;
#endregion

        [Space(10)]

#region Effects
        [Header("Actor Effects")]

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _spawnEffect;

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _respawnEffect;

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _despawnEffect;
#endregion

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsNotNull(Owner);
            Assert.IsNotNull(Movement);

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        protected virtual void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }

        protected virtual void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            AnimationUpdate(dt);
        }

        protected virtual void FixedUpdate()
        {
            float dt = UnityEngine.Time.fixedDeltaTime;

            PhysicsUpdate(dt);

            // fixes sketchy rigidbody angular momentum shit
            Movement.ResetAngularVelocity();
        }
#endregion

        public virtual void Initialize(ActorBehaviorData behaviorData)
        {
            _behaviorData = behaviorData;

            IsMoving = false;

            Movement.Initialize(behaviorData);
        }

        // called by the ActorManager
        public virtual void Think(float dt)
        {
        }

        // called in Update()
        protected virtual void AnimationUpdate(float dt)
        {
        }

        // called in FixedUpdate()
        protected virtual void PhysicsUpdate(float dt)
        {
        }

        protected void SetFacing(Vector3 direction)
        {
#if USE_SPINE
            if(null != SpineAnimationHelper) {
                SpineAnimationHelper.SetFacing(direction);
            }
#endif

            if(null != SpriteAnimationHelper) {
                SpriteAnimationHelper.SetFacing(direction);
            }

            if(null != Owner.Model && BehaviorData.AnimateModel) {
                Owner.Model.transform.forward = direction;
            }
        }

#region Events
        public virtual void OnSpawn(SpawnPoint spawnpoint)
        {
            if(null != _spawnEffect) {
                _spawnEffect.Trigger(OnSpawnComplete);
            } else {
                OnSpawnComplete();
            }
        }

        protected virtual void OnSpawnComplete()
        {
        }

        public virtual void OnReSpawn(SpawnPoint spawnpoint)
        {
            if(null != _respawnEffect) {
                _respawnEffect.Trigger(OnReSpawnComplete);
            } else {
                OnReSpawnComplete();
            }
        }

        protected virtual void OnReSpawnComplete()
        {
        }

        public virtual void OnDeSpawn()
        {
            if(null != _despawnEffect) {
                _despawnEffect.Trigger(OnDeSpawnComplete);
            } else {
                OnDeSpawnComplete();
            }
        }

        protected virtual void OnDeSpawnComplete()
        {
        }

        public virtual void CollisionEnter(GameObject collideObject)
        {
        }

        public virtual void CollisionStay(GameObject collideObject)
        {
        }

        public virtual void CollisionExit(GameObject collideObject)
        {
        }

        public virtual void TriggerEnter(GameObject triggerObject)
        {
        }

        public virtual void TriggerStay(GameObject triggerObject)
        {
        }

        public virtual void TriggerExit(GameObject triggerObject)
        {
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(!PauseAnimationOnPause) {
                return;
            }

#if USE_SPINE
            if(SpineAnimationHelper != null) {
                SpineAnimationHelper.Pause(PartyParrotManager.Instance.IsPaused);
            }
#endif

            if(Animator != null) {
                Animator.enabled = !PartyParrotManager.Instance.IsPaused;
            }
        }
#endregion
    }
}
