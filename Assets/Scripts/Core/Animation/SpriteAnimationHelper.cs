﻿using pdxpartyparrot.Core.Math;

using UnityEngine;

namespace pdxpartyparrot.Core.Animation
{
    public class SpriteAnimationHelper : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer[] _renderers;

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Abs(direction.x) < MathUtil.Epsilon) {
                return;
            }

            foreach(SpriteRenderer r in _renderers) {
                r.flipX = direction.x < 0;
            }
        }
    }
}
