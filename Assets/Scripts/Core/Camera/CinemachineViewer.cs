using Cinemachine;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Camera
{
    // TODO: instead of having the virtual camera here (or maybe in addition to?)
    // we should be able to place virtual cameras throughout the level
    // the (eventual) CinematicManager should then be able to manage the "active"
    // virtual camera for each viewer that we have available
    // TODO: it might also be worth looking into integrating Unity Timeline for this

    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineViewer : Viewer
    {
        private CinemachineVirtualCamera _cinemachineCamera;

        private CinemachineBrain _cinemachineBrain;

        [CanBeNull]
        private CinemachineImpulseListener _impulseListener;

        [CanBeNull]
        protected CinemachineImpulseListener ImpulseListener => _impulseListener;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
            _impulseListener = GetComponent<CinemachineImpulseListener>();

            _cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(_cinemachineBrain);
        }
#endregion

        public override void Set2D(float size)
        {
            base.Set2D(size);

            _cinemachineCamera.m_Lens.OrthographicSize = size;

            if(null != _impulseListener) {
                _impulseListener.m_Use2DDistance = true;
            }
        }

        public override void Set3D(float fieldOfView)
        {
            base.Set3D(fieldOfView);

            _cinemachineCamera.m_Lens.FieldOfView = fieldOfView;

            if(null != _impulseListener) {
                _impulseListener.m_Use2DDistance = false;
            }
        }

        public T GetCinemachineComponent<T>() where T: CinemachineComponentBase
        {
            return _cinemachineCamera.GetCinemachineComponent<T>();
        }

        public void Follow(Transform target)
        {
            _cinemachineCamera.Follow = target;
        }

        public void LookAt(Transform target)
        {
            _cinemachineCamera.LookAt = target;
        }
    }
}