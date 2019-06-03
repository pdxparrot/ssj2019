using Cinemachine;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineViewer : Viewer
    {
        private CinemachineVirtualCamera _cinemachineCamera;

        private CinemachineBrain _cinemachineBrain;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();

            _cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(_cinemachineBrain);
        }
#endregion

        public override void Set2D(float size)
        {
            base.Set2D(size);

            _cinemachineCamera.m_Lens.OrthographicSize = size;
        }

        public override void Set3D(float fieldOfView)
        {
            base.Set3D(fieldOfView);

            _cinemachineCamera.m_Lens.FieldOfView = fieldOfView;
        }
    }
}