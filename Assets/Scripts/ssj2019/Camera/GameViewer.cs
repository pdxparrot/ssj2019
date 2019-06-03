using Cinemachine;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Camera
{
    public sealed class GameViewer : CinemachineViewer, IPlayerViewer
    {
        [Space(10)]

        [Header("Target")]

        [SerializeField]
        private CinemachineTargetGroup _targetGroup;

        public Viewer Viewer => this;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Follow(_targetGroup.transform);
            LookAt(_targetGroup.transform);
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

            Transform viewerTransform = Viewer.transform;
            viewerTransform.position = gameData.ViewerPosition;
            viewerTransform.eulerAngles = gameData.ViewerRotation;
        }

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
