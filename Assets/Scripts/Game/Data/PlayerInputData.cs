using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [Serializable]
    public abstract class PlayerInputData : ScriptableObject
    {
        [SerializeField]
        private float _movementLerpSpeed = 5.0f;

        public float MovementLerpSpeed => _movementLerpSpeed;

        [SerializeField]
        private float _lookLerpSpeed = 10.0f;

        public float LookLerpSpeed => _lookLerpSpeed;

        [Space(10)]

#region Input Buffering
        [Header("Input Buffering")]

        [SerializeField]
        private int _inputBufferSize = 1;

        public int InputBufferSize => _inputBufferSize;
#endregion
    }
}
