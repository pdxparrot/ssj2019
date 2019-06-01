using System;

using pdxpartyparrot.Core.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    // TODO: this should be capable of moving an actor (2D or 3D)
    // without relying on Unity's physics system (eg. no Rigidbody)
    public class ActorMovement : MonoBehaviour
    {
        [SerializeField]
        private ActorBehavior _behavior;

        public ActorBehavior Behavior => _behavior;

        //[Space(10)]

#region Physics
        //[Header("Physics")]

        private Transform _transform;

        public virtual Vector3 Position
        {
            get => _transform.position;
            set
            {
                if(ActorManager.Instance.EnableDebug) {
                    Debug.Log($"Teleporting actor {Behavior.Owner.Id} to {value}");
                }
                _transform.position = value;
            }
        }

        public virtual Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        public virtual Vector3 Velocity
        {
            get => Vector3.zero;
            set {}
        }

        public virtual float Mass
        {
            get => 1.0f;
            set {}
        }

        public virtual float LinearDrag
        {
            get => 0.0f;
            set {}
        }

        public virtual float AngularDrag
        {
            get => 0.0f;
            set {}
        }

        public virtual bool IsKinematic
        {
            get => true;
            set {}
        }

        public virtual bool UseGravity
        {
            get => false;
            set {}
        }
#endregion

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsNotNull(Behavior);

            _transform = Behavior.Owner.GetComponent<Transform>();

            // always start out kinematic so that we don't
            // fall while we're loading
            IsKinematic = true;

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        protected virtual void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }
        }
#endregion

        public virtual void Initialize(ActorBehaviorData behaviorDat)
        {
            Rotation = Quaternion.identity;
            Velocity = Vector3.zero;
        }

        protected virtual void ResetFromData(ActorBehaviorData behaviorData)
        {
            Mass = behaviorData.Mass;
            LinearDrag = behaviorData.Drag;
            AngularDrag = behaviorData.AngularDrag;
            IsKinematic = behaviorData.IsKinematic;
            UseGravity = !behaviorData.IsKinematic;
        }

        public virtual void Teleport(Vector3 position)
        {
            if(ActorManager.Instance.EnableDebug) {
                Debug.Log($"Teleporting actor {Behavior.Owner.Id} to {position}");
            }
            _transform.position = position;
        }

        public virtual void MoveTowards(Vector3 position, float speed, float dt)
        {
            Vector3 newPosition = Vector3.MoveTowards(Position, position, speed * dt);
            _transform.position = newPosition;
        }

        public virtual void MoveRotation(Quaternion rot)
        {
            _transform.rotation = rot;
        }

#region Event Handlers
        protected virtual void PauseEventHandler(object sender, EventArgs args)
        {
        }
#endregion
    }
}
