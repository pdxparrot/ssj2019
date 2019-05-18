using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    [Serializable]
    public struct IntRangeConfig
    {
        [SerializeField]
        private int _min;

        public int Min => _min;

        [SerializeField]
        private int _max;

        public int Max => _max;

        public int GetRandomValue()
        {
            return PartyParrotManager.Instance.Random.Next(_min, _max);
        }

        // rounds down
        public int GetPercentValue(float pct)
        {
            pct = Mathf.Clamp01(pct);
            return (int)(_min + (pct * (_max - _min)));
        }
    }

    [Serializable]
    public struct FloatRangeConfig
    {
        [SerializeField]
        private float _min;

        public float Min => _min;

        [SerializeField]
        private float _max;

        public float Max => _max;

        public float GetRandomValue()
        {
            return PartyParrotManager.Instance.Random.NextSingle(_min, _max);
        }

        public float GetPercentValue(float pct)
        {
            pct = Mathf.Clamp01(pct);
            return _min + (pct * (_max - _min));
        }
    }
}
