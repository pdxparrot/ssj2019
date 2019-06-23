using pdxpartyparrot.Game.Interactables;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class BlockVolume : ActionVolume
    {
#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Interactables.InteractableAddedEvent += InteractableAddedEventHandler;
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            base.OnDrawGizmos();
        }
#endregion

        public void SetBlock(Vector3 offset, Vector3 size, Vector3 direction)
        {
            offset.x *= Mathf.Sign(direction.x);

            Offset = offset;
            Size = size;
        }

        public override void EnableVolume(bool enable)
        {
            base.EnableVolume(enable);

            if(!Enabled) {
                return;
            }

            foreach(IInteractable interactable in Interactables) {
                BlockInteractable(interactable);
            }
        }

#region Event Handlers
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            if(!Enabled) {
                return;
            }

            BlockInteractable(args.Interactable);
        }
#endregion

        private void BlockInteractable(IInteractable interactable)
        {
            // TODO: block?

            // TODO: parry?
        }
    }
}
