using UnityEngine;

namespace pdxpartyparrot.Core.Actors
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class Actor2D : Actor
    {
#region Collider
        public Collider2D Collider { get; private set; }
#endregion

        public override float Height => Collider.bounds.size.y;

        public override float Radius => Collider.bounds.size.x / 2.0f;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Collider = GetComponent<Collider2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Behavior.CollisionEnter(collision.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            Behavior.CollisionStay(collision.gameObject);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            Behavior.CollisionExit(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Behavior.TriggerEnter(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            Behavior.TriggerStay(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Behavior.TriggerExit(other.gameObject);
        }
#endregion
    }
}
