using pdxpartyparrot.Core.Math;

using UnityEngine;

namespace pdxpartyparrot.Core.Animation
{
    public class SpriteAnimationHelper : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Abs(direction.x) < MathUtil.Epsilon) {
                return;
            }

            Transform rt = _renderer.transform;

            Vector3 localScale = rt.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (direction.x < 0 ? -1.0f : 1.0f);

            rt.localScale = localScale;
        }
    }
}
