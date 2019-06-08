using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
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

        public GamepadListener GamepadListener { get; private set; }

        private ITimer _inputQueueTimeout;

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
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_inputQueueTimeout);
            }
            _inputQueueTimeout = null;

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

            _inputQueueTimeout = TimeManager.Instance.AddTimer();
            _inputQueueTimeout.TimesUpEvent += InputQueueTimeoutTimesUpEventHandler;
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
                GamePlayer.GamePlayerBehavior.ClearActionQueue();
                _inputQueueTimeout.Stop();

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
                GamePlayer.GamePlayerBehavior.QueueAction(new AttackBehaviorComponent.AttackAction{
                    Axes = LastMove,
                });

                _inputQueueTimeout.ReStartMillis(GamePlayerInputData.InputQueueTimeout);
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
                GamePlayer.GamePlayerBehavior.ClearActionQueue();
                _inputQueueTimeout.Stop();

                GamePlayer.GamePlayerBehavior.ActionPerformed(BlockBehaviorComponent.BlockAction.Default);
            }
        }
#endregion

#region EventHandlers
        private void InputQueueTimeoutTimesUpEventHandler(object sender, EventArgs args)
        {
            if(PlayerManager.Instance.DebugInput) {
                Debug.Log($"Clearing action queue");
            }

            GamePlayer.GamePlayerBehavior.ClearActionQueue();
        }
#endregion
    }
}
