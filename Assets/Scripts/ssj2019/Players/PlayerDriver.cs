using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Input;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerDriver : Game.Players.SideScollerPlayerDriver<PlayerControls>, PlayerControls.IPlayerActions
    {
        protected override bool CanDrive => base.CanDrive && !GameManager.Instance.IsGameOver;

        private Player GamePlayer => (Player)Player;

        public GamepadListener GamepadListener { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Player is Player);
            Assert.IsNull(GetComponent<GamepadListener>());
        }

        protected override void OnDestroy()
        {
            if(null != GamepadListener) {
                Destroy(GamepadListener);
            }
            GamepadListener = null;

            Actions.Player.Disable();
            Actions.Player.SetCallbacks(null);

            base.OnDestroy();
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            if(!Player.IsLocalActor) {
                return;
            }

            GamepadListener = gameObject.AddComponent<GamepadListener>();

            Actions.Player.SetCallbacks(this);
            Actions.Player.Enable();
        }

        protected override bool IsOurDevice(InputAction.CallbackContext ctx)
        {
            if(!base.IsOurDevice(ctx)) {
                return false;
            }

            return GamepadListener.IsOurGamepad(ctx)
#if UNITY_EDITOR
                // allow keyboard / mouse when running with a single player in editor
                || (ActorManager.Instance.ActorCount<Player>() == 1 && (Keyboard.current == ctx.control.device || Mouse.current == ctx.control.device))
#endif
            ;
        }

#region IPlayerActions
        public void OnJump(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context) || !CanDrive) {
                return;
            }

            if(PlayerManager.Instance.DebugInput) {
                Debug.Log($"Jump: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.GamePlayerBehavior.ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
            }
        }
#endregion
    }
}
