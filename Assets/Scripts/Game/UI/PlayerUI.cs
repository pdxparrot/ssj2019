using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    // TODO: this should probably be called "Game UI"
    // since it's not player-specific
    public abstract class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        public virtual void Initialize(UnityEngine.Camera uiCamera)
        {
            _canvas.worldCamera = uiCamera;
        }
    }
}
