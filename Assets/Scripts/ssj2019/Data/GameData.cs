using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Camera;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="pdxpartyparrot/ssj2019/Data/Game Data")]
    [Serializable]
    public sealed class GameData : Game.Data.GameData
    {
#region Viewers
        [Header("Viewer")]

        [SerializeField]
        private GameViewer _viewerPrefab;

        public GameViewer ViewerPrefab => _viewerPrefab;

        [SerializeField]
        private Vector3 _viewerPosition;

        public Vector3 ViewerPosition => _viewerPosition;

        [SerializeField]
        private Vector3 _viewerRotation;

        public Vector3 ViewerRotation => _viewerRotation;
#endregion

        [Space(10)]

#region Characters
        [Header("Player Characters")]

        [SerializeField]
        [ReorderableList]
        private PlayerCharacterData.ReorderableList _playerCharacterData = new PlayerCharacterData.ReorderableList();

        public IReadOnlyCollection<PlayerCharacterData> PlayerCharacterData => _playerCharacterData.Items;
#endregion
    }
}
