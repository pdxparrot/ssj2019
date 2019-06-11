using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Players.Input;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Input;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerInput : SideScollerPlayerInput<PlayerControls>, PlayerControls.IPlayerActions
    {
        protected override bool InputEnabled => base.InputEnabled && !GameManager.Instance.IsGameOver;

        private PlayerInputData GamePlayerInputData => (PlayerInputData)PlayerInputData;

        private Player GamePlayer => (Player)Player;

// TODO: it's cool we have a gamepad listener, but we need to tell it what our Gamepad is if we have one
// rather than it immediately trying to acquire one from the pool (or maybe there's a better way to do this?)
// like maybe when a player spawns they get the character associated with their gamepad?
// in which case we have to deal with "our gamepad is gone" and "we don't have a gamepad"

        public GamepadListener GamepadListener { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInputData is PlayerInputData);
            Assert.IsTrue(Player is Player);
            Assert.IsNull(GetComponent<GamepadListener>());
        }

        protected override void OnDestroy()
        {
            Actions.Player.Disable();
            Actions.Player.SetCallbacks(null);

            if(null != GamepadListener) {
                Destroy(GamepadListener);
            }
            GamepadListener = null;

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
            if(!InputEnabled || !IsOurDevice(context)) {
                return;
            }

            if(PlayerManager.Instance.DebugInput) {
                Debug.Log($"Jump: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.GamePlayerBehavior.ClearActionBuffer();

                GamePlayer.GamePlayerBehavior.ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(!InputEnabled || !IsOurDevice(context)) {
                return;
            }

            if(PlayerManager.Instance.DebugInput) {
                Debug.Log($"Attack: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.GamePlayerBehavior.BufferAction(new AttackBehaviorComponent.AttackAction{
                    Axes = LastMove,
                });
            }
        }

        public void OnBlock(InputAction.CallbackContext context)
        {
            if(!InputEnabled || !IsOurDevice(context)) {
                return;
            }

            if(PlayerManager.Instance.DebugInput) {
                Debug.Log($"Block: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.GamePlayerBehavior.BufferAction(new BlockBehaviorComponent.BlockAction{
                    Axes = LastMove,
                });
            }
        }
#endregion
    }
}
