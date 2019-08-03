using pdxpartyparrot.Core.UI;
using pdxpartyparrot.ssj2019.Data.Players;

using TMPro;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.UI
{
    [RequireComponent(typeof(UIObject))]
    public sealed class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private CharacterSelector[] _characterPanels;

        [SerializeField]
        private TextMeshProUGUI _waveText;

        [SerializeField]
        private TextMeshProUGUI _scoreText;

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_characterPanels.Length == GameManager.Instance.GameGameData.MaxLocalPlayers);

            foreach(CharacterSelector characterPanel in _characterPanels) {
                characterPanel.HideDisplay();
            }
        }
#endregion

        public void EnableCharacterPanel(int playerNumber, PlayerCharacterData characterData)
        {
            if(playerNumber < 0 || playerNumber >= _characterPanels.Length) {
                return;
            }

            CharacterSelector characterPanel = _characterPanels[playerNumber];

            characterPanel.ShowCharacterDisplay();
            characterPanel.SetCharacterData(characterData, playerNumber);
            characterPanel.SetCharacterPortrait(Instantiate(characterData.CharacterPortraitPrefab));
        }

        public void SetPlayerHealthPercent(int playerNumber, float healthPercent)
        {
            if(playerNumber < 0 || playerNumber >= _characterPanels.Length) {
                return;
            }

            CharacterSelector characterPanel = _characterPanels[playerNumber];
            characterPanel.SetHealthPercent(healthPercent);
        }

        public void SetWave(int wave)
        {
            _waveText.text = $"{wave}";
        }

        public void SetScore(int score)
        {
            _scoreText.text = $"{score}";
        }
    }
}
