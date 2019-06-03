using Cinemachine;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineViewer : Viewer
    {
        private CinemachineVirtualCamera _cinemachineCamera;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsNotNull(Camera.GetComponent<CinemachineBrain>());

            _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        }
#endregion
    }
}