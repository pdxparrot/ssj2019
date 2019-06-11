using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIObject : MonoBehaviour
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            GameUIManager.Instance.RegisterUIObject(this);
        }

        protected virtual void OnDestroy()
        {
            if(GameUIManager.HasInstance) {
                GameUIManager.Instance.UnregisterUIObject(this);
            }
        }
#endregion
    }
}
