using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019
{
    public sealed class GameManager : GameManager<GameManager>
    {
        [SerializeField]
        private MainGameState _mainGameStatePrefab;

        public MainGameState MainGameStatePrefab => _mainGameStatePrefab;

        public GameData GameGameData => (GameData)GameData;

        // only valid on the client
        public GameViewer Viewer { get; private set; }

        //[Client]
        public void InitViewer()
        {
            Viewer = ViewerManager.Instance.AcquireViewer<GameViewer>(gameObject);
            if(null == Viewer) {
                Debug.LogWarning("Unable to acquire game viewer!");
                return;
            }

            switch(GameGameData.SelectedViewerMode)
            {
            case Data.GameData.ViewerMode.Mode2D:
                Viewer.Set2D(GameGameData.ViewportSize);
                break;
            case Data.GameData.ViewerMode.Mode3D:
                Viewer.Set3D();
                break;
            }

            Transform viewerTransform = Viewer.transform;
            viewerTransform.position = GameGameData.ViewerPosition;
            viewerTransform.eulerAngles = GameGameData.ViewerRotation;
        }
    }
}
