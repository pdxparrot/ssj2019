using UnityEngine;

namespace pdxpartyparrot.ssj2019.Level
{
    public sealed class LevelHelper : MonoBehaviour
    {
        /*[SerializeField]
        private Collider2D _cameraBounds;

        public Collider2D CameraBounds => _cameraBounds;*/

#region Unity Lifecycle
        private void Awake()
        {
            //GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }
#endregion
    }
}
