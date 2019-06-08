using System;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

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
        private bool _moveReceived;

        private CircularBuffer<Vector3> _moveBuffer;

        public Vector3 LastMove => _moveBuffer.Tail;

        [SerializeField]
        [ReadOnly]
        private bool _lookReceived;

        private CircularBuffer<Vector3> _lookBuffer;

        public Vector3 LastLook => _moveBuffer.Tail;

        [SerializeField]
        [ReadOnly]
        private bool _actionReceived;

        private CircularBuffer<CharacterBehaviorComponent.CharacterBehaviorAction> _actionBuffer;

        public CharacterBehaviorComponent.CharacterBehaviorAction LastAction => _actionBuffer.Tail;

        protected virtual bool InputEnabled => !PartyParrotManager.Instance.IsPaused && Player.IsLocalActor;

        protected bool EnableMouseLook { get; private set; } = !Application.isEditor;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsTrue(Owner is IPlayer);
            Assert.IsNotNull(PlayerInputData);
            Assert.IsTrue(PlayerInputData.InputBufferSize > 0);

            _moveBuffer = new CircularBuffer<Vector3>(PlayerInputData.InputBufferSize);
            _lookBuffer = new CircularBuffer<Vector3>(PlayerInputData.InputBufferSize);
            _actionBuffer = new CircularBuffer<CharacterBehaviorComponent.CharacterBehaviorAction>(PlayerInputData.InputBufferSize);

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

            if(!_moveReceived) {
                _moveBuffer.RemoveOldest();
            }
            _moveReceived = false;

            if(!_lookReceived) {
                _lookBuffer.RemoveOldest();
            }
            _lookReceived = false;

            if(!_actionReceived) {
                _actionBuffer.RemoveOldest();
            }
            _actionReceived = false;
        }
#endregion

        public virtual void Initialize()
        {
            if(!Player.IsLocalActor) {
                return;
            }

            InitDebugMenu();
        }

        protected virtual void EnableControls(bool enable)
        {
        }

#region Common Actions
        public void OnPause()
        {
            PartyParrotManager.Instance.TogglePause();
        }

        public void OnMove(Vector3 axes)
        {
            _moveBuffer.Add(axes);
            _moveReceived = true;
        }

        public void OnLook(Vector3 axes)
        {
            _lookBuffer.Add(axes);
            _lookReceived = true;
        }

        public void OnAction(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            _actionBuffer.Add(action);
            _actionReceived = true;
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

                GUILayout.BeginVertical("Move Buffer", GUI.skin.box);
                    foreach(var move in _moveBuffer) {
                        GUILayout.Label(move.ToString());
                    }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("Look Buffer", GUI.skin.box);
                    foreach(var look in _lookBuffer) {
                        GUILayout.Label(look.ToString());
                    }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("Action Buffer", GUI.skin.box);
                    foreach(var action in _actionBuffer) {
                        GUILayout.Label(action.ToString());
                    }
                GUILayout.EndVertical();
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
