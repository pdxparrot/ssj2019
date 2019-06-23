﻿using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(Interactables))]
    [RequireComponent(typeof(BoxCollider))]
    public abstract class ActionVolume : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Actor _owner;

        protected Actor Owner => _owner;

        protected BoxCollider _collider;

        [SerializeField]
        [ReadOnly]
        private bool _enabled;

        protected bool Enabled
        {
            get => _enabled;
            private set => _enabled = value;
        }

        public bool CanInteract => Enabled;

        public Vector3 Offset
        {
            get => _collider.center;
            protected set => _collider.center = value;
        }

        public Vector3 Size
        {
            get => _collider.size;
            protected set => _collider.size = value;
        }

        protected Interactables Interactables { get; private set; }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;

            Interactables = GetComponent<Interactables>();
        }

        protected virtual void OnDrawGizmos()
        {
            if(!Application.isPlaying || !Enabled) {
                return;
            }
            Gizmos.DrawCube(transform.position + _collider.center, _collider.size);
        }
#endregion

        public virtual void EnableVolume(bool enable)
        {
            Enabled = enable;
        }
    }
}
