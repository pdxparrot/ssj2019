#if ENABLE_SERVER_SPECTATOR
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Network;

namespace pdxpartyparrot.Game.Camera
{
    public sealed class ServerSpectatorViewer : FollowViewer3D
    {
        public void Initialize(ServerSpectator owner)
        {
            Initialize(0);

            FollowCamera3D.SetTarget(owner.FollowTarget);
            SetFocus(owner.transform);
        }
    }
}
#endif
