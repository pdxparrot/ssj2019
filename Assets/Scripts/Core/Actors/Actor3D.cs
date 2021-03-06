﻿using UnityEngine;

namespace pdxpartyparrot.Core.Actors
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class Actor3D : Actor
    {
#region Collider
        public Collider Collider { get; private set; }
#endregion

        public override float Height => Collider.bounds.size.y;

        public override float Radius => Collider.bounds.size.x / 2.0f;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Behavior.CollisionEnter(collision.gameObject);
        }

        private void OnCollisionStay(Collision collision)
        {
            Behavior.CollisionStay(collision.gameObject);
        }

        private void OnCollisionExit(Collision collision)
        {
            Behavior.CollisionExit(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            Behavior.TriggerEnter(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            Behavior.TriggerStay(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            Behavior.TriggerExit(other.gameObject);
        }
#endregion
    }
}
