using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Camera;

namespace pdxpartyparrot.ssj2019.Camera
{
    public sealed class GameViewer : CinemachineViewer, IPlayerViewer
    {
        public Viewer Viewer => this;
    }
}
