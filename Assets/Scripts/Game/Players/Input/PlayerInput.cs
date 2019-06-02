using System;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class PlayerInput : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        public Actor Owner => _owner;

        protected IPlayer Player => (IPlayer)Owner;

        [SerializeField]
        private PlayerInputData _data;

        public PlayerInputData PlayerInputData => _data;

        [SerializeField]
        private float _mouseSensitivity = 0.5f;

        protected float MouseSensitivity => _mouseSensitivity;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastControllerMove;

        public Vector3 LastControllerMove
        {
            get => _lastControllerMove;
            protected set => _lastControllerMove = value;
        }

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastControllerLook;

        public Vector3 LastControllerLook
        {
            get => _lastControllerLook;
            protected set => _lastControllerLook = value;
        }

        protected virtual bool InputEnabled => !PartyParrotManager.Instance.IsPaused && Player.IsLocalActor;

        protected bool EnableMouseLook { get; private set; } = !Application.isEditor;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsTrue(Owner is IPlayer);
            Assert.IsNotNull(_data);

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        protected virtual void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }

            DestroyDebugMenu();
        }

        protected virtual void OnEnable()
        {
            EnableControls(true);
        }

        protected virtual void OnDisable()
        {
            EnableControls(false);
        }

        protected virtual void Update()
        {
            if(!Player.IsLocalActor) {
                return;
            }

            if(!InputEnabled) {
                LastControllerMove = Vector3.zero;
            }
        }
#endregion

        public virtual void Initialize()
        {
            if(!Player.IsLocalActor) {
                return;
            }

            InitDebugMenu();
        }

        protected virtual bool IsOurDevice(InputAction.CallbackContext ctx)
        {
            // no input unless we have focus
            if(!Application.isFocused) {
                return false;
            }

            // ignore keyboard/mouse while the debug menu is open
            if(DebugMenuManager.Instance.Enabled && (ctx.control.device == Keyboard.current || ctx.control.device == Mouse.current)) {
                return false;
            }

            return true;
        }

        protected virtual void EnableControls(bool enable)
        {
        }

#region Common Actions
        public void OnPause(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Pause: {context.action.phase}");
            }

            if(context.performed) {
                PartyParrotManager.Instance.TogglePause();
            }
        }

        public virtual void OnMove(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            LastControllerMove = new Vector3(axes.x, axes.y, 0.0f);
        }

        public virtual void OnLook(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            LastControllerLook = new Vector3(axes.x, axes.y, 0.0f);
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(PartyParrotManager.Instance.IsPaused) {
                if(GameStateManager.Instance.PlayerManager.DebugInput) {
                    Debug.Log("Disabling player controls");
                }
                EnableControls(false);
            } else {
                if(GameStateManager.Instance.PlayerManager.DebugInput) {
                    Debug.Log("Enabling player controls");
                }
                EnableControls(true);
            }
        }
#endregion

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => $"Game.Player {Player.Id} Input");
            _debugMenuNode.RenderContentsAction = () => {
                /*GUILayout.BeginHorizontal();
                    GUILayout.Label("Mouse Sensitivity:");
                    _mouseSensitivity = GUIUtils.FloatField(_mouseSensitivity);
                GUILayout.EndHorizontal();*/

                if(Application.isEditor) {
                    EnableMouseLook = GUILayout.Toggle(EnableMouseLook, "Enable Mouse Look");
                }
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
