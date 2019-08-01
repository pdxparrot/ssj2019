using pdxpartyparrot.Core.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.UI
{
    [RequireComponent(typeof(UIObject))]
    public sealed class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private GameObject _characterPanelContainer;
    }
}
