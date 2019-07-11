using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Actors;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="pdxpartyparrot/ssj2019/Data/Game Data")]
    [Serializable]
    public sealed class GameData : Game.Data.GameData
    {
        public enum ViewerMode
        {
            Mode2D,
            Mode3D
        }

#region Viewers
        [Header("Viewer")]

        [SerializeField]
        private ViewerMode _viewerMode = ViewerMode.Mode2D;

        public ViewerMode SelectedViewerMode => _viewerMode;

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

        [SerializeField]
        private FloatingText _floatingTextPrefab;

        public FloatingText FloatingTextPrefab => _floatingTextPrefab;

        [SerializeField]
        private int _floatingTextPoolSize = 10;

        public int FloatingTextPoolSize => _floatingTextPoolSize;

        [Space(10)]

        [SerializeField]
        private TrainingDummy _trainingDummyPrefab;

        public TrainingDummy TrainingDummyPrefab => _trainingDummyPrefab;
    }
}
