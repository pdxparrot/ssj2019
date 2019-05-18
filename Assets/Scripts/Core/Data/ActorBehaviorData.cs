using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [Serializable]
    public abstract class ActorBehaviorData : ScriptableObject
    {
        [SerializeField]
        private LayerMask _actorLayer;

        public LayerMask ActorLayer => _actorLayer;

#region Physics
        [Header("Actor Physics")]

        [SerializeField]
        [Tooltip("Mass in Kg")]
        private float _mass = 45.0f;

        public float Mass => _mass;

        [SerializeField]
        [Tooltip("Drag coefficient")]
        private float _drag = 0.0f;

        public float Drag => _drag;

        [SerializeField]
        [Tooltip("Angular drag coefficient")]
        private float _angularDrag = 0.0f;

        public float AngularDrag => _angularDrag;

        [SerializeField]
        private bool _isKinematic = false;

        public bool IsKinematic => _isKinematic;
#endregion
    }
}
