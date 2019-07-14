using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Guid _id;

        public Guid Id => _id;

        public abstract float Height { get; }

        public abstract float Radius { get; }

#region Model
        [CanBeNull]
        [SerializeField]
        private GameObject _model;

        [CanBeNull]
        public GameObject Model
        {
            get => _model;
            protected set => _model = value;
        }
#endregion

#region Behavior
        [SerializeField]
        private ActorBehavior _behavior;

        public ActorBehavior Behavior => _behavior;
#endregion

        [SerializeField]
        [ReadOnly]
        private Vector3 _facingDirection = new Vector3(1.0f, 0.0f, 0.0f);

        public Vector3 FacingDirection
        {
            get => _facingDirection;
            private  set => _facingDirection = value;
        }

#region Network
        public abstract bool IsLocalActor { get; }
#endregion

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsNotNull(Behavior);
        }

        protected virtual void OnDestroy()
        {
            if(ActorManager.HasInstance) {
                ActorManager.Instance.Unregister(this);
            }
        }
#endregion

        public virtual void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            if(ActorManager.Instance.EnableDebug) {
                Debug.Log($"Initializing actor {id}");
            }

            _id = id;
            name = Id.ToString();

            Behavior.Initialize(behaviorData);
        }

        public virtual void SetFacing(Vector3 direction)
        {
            if(direction.sqrMagnitude < MathUtil.Epsilon) {
                return;
            }

            FacingDirection = direction.normalized;

#if USE_SPINE
            if(null != Behavior.SpineAnimationHelper) {
                Behavior.SpineAnimationHelper.SetFacing(FacingDirection);
            }
#endif

            if(null != Behavior.SpriteAnimationHelper) {
                Behavior.SpriteAnimationHelper.SetFacing(FacingDirection);
            }

            if(null != Model && Behavior.BehaviorData.AnimateModel) {
                Model.transform.forward = FacingDirection;
            }
        }

        // TODO: would be better if we id radius (x) and height (y) separately
        public bool Collides(Actor other, float distance=float.Epsilon)
        {
            Vector3 offset = other.Behavior.Movement.Position - Behavior.Movement.Position;
            float r = other.Radius + Radius;
            float d = r * r;
            return offset.sqrMagnitude < d;
        }

#region Events
        public virtual bool OnSpawn(SpawnPoint spawnpoint)
        {
            ActorManager.Instance.Register(this);

            Behavior.OnSpawn(spawnpoint);

            return true;
        }

        public virtual bool OnReSpawn(SpawnPoint spawnpoint)
        {
            ActorManager.Instance.Register(this);

            Behavior.OnReSpawn(spawnpoint);

            return true;
        }

        public virtual void OnDeSpawn()
        {
            Behavior.OnDeSpawn();

            if(ActorManager.HasInstance) {
                ActorManager.Instance.Unregister(this);
            }
        }
#endregion
    }
}
