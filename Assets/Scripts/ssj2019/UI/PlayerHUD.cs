using pdxpartyparrot.Core.UI;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.UI
{
    [RequireComponent(typeof(UIObject))]
    public sealed class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private CharacterSelector[] _characterPanels;

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_characterPanels.Length == GameManager.Instance.GameGameData.MaxLocalPlayers);

            foreach(CharacterSelector characterPanel in _characterPanels) {
                characterPanel.gameObject.SetActive(false);
            }
        }
#endregion

        public void EnableCharacterPanel(short controllerId, PlayerCharacterData characterData)
        {
            if(controllerId < 0 || controllerId >= _characterPanels.Length) {
                return;
            }

            CharacterSelector characterPanel = _characterPanels[controllerId];

            characterPanel.ShowCharacterDisplay();
            characterPanel.SetCharacterData(characterData, controllerId);
            characterPanel.SetCharacterPortrait(Instantiate(characterData.CharacterPortraitPrefab));
            characterPanel.gameObject.SetActive(true);
        }
    }
}
