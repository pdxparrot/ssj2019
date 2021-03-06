using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Actors;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data.Players;
using pdxpartyparrot.ssj2019.State;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="pdxpartyparrot/ssj2019/Data/Game Data")]
    [Serializable]
    public sealed class GameData : Game.Data.GameData
    {
#region Game States
        [Header("Game States")]

        [SerializeField]
        private MainGameState _mainGameStatePrefab;

        public MainGameState MainGameStatePrefab => _mainGameStatePrefab;

        [SerializeField]
        private TrainingGameState _trainingGameStatePrefab;

        public TrainingGameState TrainingGameStatePrefab => _trainingGameStatePrefab;
#endregion

        [Space(10)]

#region Viewers
        [Header("Viewer")]

        [SerializeField]
        private GameViewer _viewerPrefab;

        public GameViewer ViewerPrefab => _viewerPrefab;
#endregion

        [Space(10)]

#region Characters
        [Header("Player Characters")]

        [SerializeField]
        [ReorderableList]
        private PlayerCharacterData.ReorderableList _playerCharacterData = new PlayerCharacterData.ReorderableList();

        public IReadOnlyCollection<PlayerCharacterData> PlayerCharacterData => _playerCharacterData.Items;
#endregion

        [Space(10)]

#region Floating Text
        [Header("Floating text")]

        [SerializeField]
        private FloatingText _floatingTextPrefab;

        public FloatingText FloatingTextPrefab => _floatingTextPrefab;

        [SerializeField]
        private int _floatingTextPoolSize = 10;

        public int FloatingTextPoolSize => _floatingTextPoolSize;
#endregion

        [Space(10)]

        // TODO: this should move to the TrainingLevel prefab
        [SerializeField]
        private TrainingDummy _trainingDummyPrefab;

        public TrainingDummy TrainingDummyPrefab => _trainingDummyPrefab;
    }
}
