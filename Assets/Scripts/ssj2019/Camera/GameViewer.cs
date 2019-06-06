using Cinemachine;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Camera
{
    [RequireComponent(typeof(CinemachineFramingTransposer))]
    [RequireComponent(typeof(CinemachinePOV))]
    //[RequireComponent(typeof(CinemachineConfiner))]
    public sealed class GameViewer : CinemachineViewer, IPlayerViewer
    {
        [Space(10)]

        [Header("Target")]

        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        public Viewer Viewer => this;

        private CinemachineFramingTransposer _transposer;
        //private CinemachineConfiner _confiner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _transposer = GetCinemachineComponent<CinemachineFramingTransposer>();
            //_confiner = GetComponent<CinemachineConfiner>();
        }
#endregion

        public void Initialize(GameData gameData)
        {
            switch(gameData.SelectedViewerMode)
            {
            case GameData.ViewerMode.Mode2D:
                Viewer.Set2D(gameData.ViewportSize);
                break;
            case GameData.ViewerMode.Mode3D:
                Viewer.Set3D();
                break;
            }

            _transposer.m_GroupFramingMode = CinemachineFramingTransposer.FramingMode.HorizontalAndVertical;
            _transposer.m_MinimumOrthoSize = gameData.ViewportSize;
            _transposer.m_MaximumOrthoSize = gameData.ViewportSize * 2.0f;
        }

        /*public void SetBounds(Collider2D bounds)
        {
            Debug.Log("Setting viewer bounds");

            _confiner.m_BoundingShape2D = bounds;
        }*/

        public void AddTarget(Actor actor, float weight=1.0f)
        {
            Debug.Log($"Adding viewer target {actor.Id}");

            _targetGroup.AddMember(actor.transform, weight, actor.Radius);
        }

        public void RemoveTarget(Actor actor)
        {
            Debug.Log($"Removing viewer target {actor.Id}");

            _targetGroup.RemoveMember(actor.transform);
        }
    }
}
