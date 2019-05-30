using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Game.Data
{
    [Serializable]
    public abstract class GameData : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("_viewerLayer")]
        private LayerMask _viewerLayerMask;

        public LayerMask ViewerLayerMask => _viewerLayerMask;

        [SerializeField]
        [FormerlySerializedAs("_worldLayer")]
        private LayerMask _worldLayerMask;

        public LayerMask WorldLayerMask => _worldLayerMask;

        [Space(10)]

        [Header("Viewport")]

        // TODO: this probably isn't the best way to handle this or the best place to put it
        [SerializeField]
        [Tooltip("The orthographic size of the 2D camera, which is also the height of the viewport.")]
        private float _viewportSize = 10;

        public float ViewportSize => _viewportSize;

        [Space(10)]

        [Header("Players")]

        [SerializeField]
        private int _maxLocalPlayers = 1;

        public int MaxLocalPlayers => _maxLocalPlayers;

        [SerializeField]
        private bool _gamepadsArePlayers;

        public bool GamepadsArePlayers => _gamepadsArePlayers;
    }
}
