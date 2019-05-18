using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Collections;

using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    public abstract class Interactables : MonoBehaviour
    {
#region Events
        public event EventHandler<InteractableEventArgs> InteractableAddedEvent;
        public event EventHandler<InteractableEventArgs> InteractableRemovedEvent;
#endregion

        private readonly Dictionary<Type, HashSet<IInteractable>> _interactables = new Dictionary<Type, HashSet<IInteractable>>();

        public bool RemoveInteractable(IInteractable interactable)
        {
            var interactables = _interactables.GetOrAdd(interactable.GetType());
            return interactables.Remove(interactable);
        }

        public IReadOnlyCollection<IInteractable> GetInteractables<T>() where T: IInteractable
        {
            return _interactables.GetOrAdd(typeof(T));
        }

        protected void AddInteractable(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if(null == interactable || !interactable.CanInteract) {
                return;
            }

            var interactables = _interactables.GetOrAdd(interactable.GetType());
            if(interactables.Add(interactable)) {
                InteractableAddedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }

        protected void RemoveInteractable(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if(null == interactable) {
                return;
            }

            if(RemoveInteractable(interactable)) {
                InteractableRemovedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }
    }
}
