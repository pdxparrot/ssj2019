using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Game;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019
{
    public sealed class GameManager : GameManager<GameManager>
    {
        [SerializeField]
        private MainGameState _mainGameStatePrefab;

        public MainGameState MainGameStatePrefab => _mainGameStatePrefab;

        public GameData GameGameData => (GameData)GameData;

        // only valid on the client
        public GameViewer Viewer { get; private set; }

        private readonly Dictionary<InputDevice, PlayerCharacterData> _characters = new Dictionary<InputDevice, PlayerCharacterData>();

        public override void Shutdown()
        {
            _characters.Clear();

            base.Shutdown();
        }

        //[Client]
        public void InitViewer()
        {
            Viewer = ViewerManager.Instance.AcquireViewer<GameViewer>(gameObject);
            if(null == Viewer) {
                Debug.LogWarning("Unable to acquire game viewer!");
                return;
            }
            Viewer.Initialize(GameGameData);
        }

        //[Client]
        public void AddCharacter(InputDevice device, PlayerCharacterData playerCharacterData)
        {
            _characters[device] = playerCharacterData;
        }

        //[Client]
        [CanBeNull]
        public PlayerCharacterData AcquireCharacter(InputDevice device)
        {
            if(device == null) {
                return null;
            }

            PlayerCharacterData playerCharacterData = _characters.GetOrDefault(device);
            if(null == playerCharacterData) {
                return null;
            }

            _characters.Remove(device);
            return playerCharacterData;
        }

        //[Client]
        [CanBeNull]
        public PlayerCharacterData AcquireFreeCharacter()
        {
            foreach(var kvp in _characters) {
                _characters.Remove(kvp.Key);
                return kvp.Value;
            }

            return null;
        }
    }
}
