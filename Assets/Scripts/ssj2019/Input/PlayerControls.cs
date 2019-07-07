// GENERATED AUTOMATICALLY FROM 'Assets/Data/Input/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace pdxpartyparrot.ssj2019.Input
{
    public class PlayerControls : IInputActionCollection
    {
        private InputActionAsset asset;
        public PlayerControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""89525c91-3845-4a31-a306-a607d9d2d231"",
            ""actions"": [
                {
                    ""name"": ""pause"",
                    ""id"": ""44b6e5fa-3d7b-440f-9060-13fa7d641325"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""move"",
                    ""id"": ""1caee292-fa5f-4e0c-b724-6e1c7da09e51"",
                    ""expectedControlLayout"": ""Vector2"",
                    ""continuous"": true,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""jump"",
                    ""id"": ""efb3b829-c18a-4071-a255-a6697a3b990e"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""attack"",
                    ""id"": ""de643a9a-fc39-42b6-9600-eaabc22ef9ab"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""block"",
                    ""id"": ""b910815f-1b25-4825-971f-37f5863bd38f"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                },
                {
                    ""name"": ""dash"",
                    ""id"": ""496bddd6-7d31-418c-8359-ef595963fa7c"",
                    ""expectedControlLayout"": ""Button"",
                    ""continuous"": false,
                    ""passThrough"": false,
                    ""initialStateCheck"": false,
                    ""processors"": """",
                    ""interactions"": """",
                    ""bindings"": []
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""0830489b-9c1e-4038-a321-130bc0faee1a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""wasd"",
                    ""id"": ""2c16dd73-c32d-4492-ab3f-2601a47fab91"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1615a6a2-034c-4b17-89e2-0fa57dea391d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6192d0a8-e38a-46be-8042-4f55215789ea"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""left"",
                    ""id"": ""469a061d-9d80-49f4-b766-8803e940891c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b968d6c2-ca21-4be3-baf9-7cafeb743d14"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""arrow keys"",
                    ""id"": ""df464eee-4a68-4cd7-a924-64f2c2eb5337"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""up"",
                    ""id"": ""463585fd-aa37-4add-94a2-28f7c3b3bbe4"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""down"",
                    ""id"": ""3d462a16-a2a1-45f2-b7a0-1a8156f95e4a"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f680bbe-dc65-4e50-b5fa-0853d63ba127"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ff5b2709-5af5-43cd-b149-c357e6b2da0f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""b7abc3d1-76d3-4571-9dbe-2dc681aceac4"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""834f4796-b4a0-494d-8975-71b96e984b12"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""0727335b-51ae-4a78-8a6e-14d8a5a2dc39"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""8dc8d354-adf3-4447-b4b9-9aa694723896"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""a10b599b-64fa-436e-a9ae-c9712a4e7d7b"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""ba1422cd-7cba-4d7e-8b8c-1113c7f5e850"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""83faa88f-c164-433f-89a1-4141c227a9f5"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""015a53f7-2114-4eba-b58b-ed34c5cc5239"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""block"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""f540bebb-a084-4960-9078-ce6264b7326f"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                },
                {
                    ""name"": """",
                    ""id"": ""5959b1c8-44b3-4089-a478-7d103a3d469a"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false,
                    ""modifiers"": """"
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Player
            m_Player = asset.GetActionMap("Player");
            m_Player_pause = m_Player.GetAction("pause");
            m_Player_move = m_Player.GetAction("move");
            m_Player_jump = m_Player.GetAction("jump");
            m_Player_attack = m_Player.GetAction("attack");
            m_Player_block = m_Player.GetAction("block");
            m_Player_dash = m_Player.GetAction("dash");
        }

        ~PlayerControls()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes
        {
            get => asset.controlSchemes;
        }

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Player
        private InputActionMap m_Player;
        private IPlayerActions m_PlayerActionsCallbackInterface;
        private InputAction m_Player_pause;
        private InputAction m_Player_move;
        private InputAction m_Player_jump;
        private InputAction m_Player_attack;
        private InputAction m_Player_block;
        private InputAction m_Player_dash;
        public struct PlayerActions
        {
            private PlayerControls m_Wrapper;
            public PlayerActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @pause { get { return m_Wrapper.m_Player_pause; } }
            public InputAction @move { get { return m_Wrapper.m_Player_move; } }
            public InputAction @jump { get { return m_Wrapper.m_Player_jump; } }
            public InputAction @attack { get { return m_Wrapper.m_Player_attack; } }
            public InputAction @block { get { return m_Wrapper.m_Player_block; } }
            public InputAction @dash { get { return m_Wrapper.m_Player_dash; } }
            public InputActionMap Get() { return m_Wrapper.m_Player; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled { get { return Get().enabled; } }
            public InputActionMap Clone() { return Get().Clone(); }
            public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerActions instance)
            {
                if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
                {
                    pause.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    pause.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    pause.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPause;
                    move.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    move.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    move.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMove;
                    jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                    attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                    block.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBlock;
                    block.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBlock;
                    block.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBlock;
                    dash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                    dash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                    dash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnDash;
                }
                m_Wrapper.m_PlayerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    pause.started += instance.OnPause;
                    pause.performed += instance.OnPause;
                    pause.canceled += instance.OnPause;
                    move.started += instance.OnMove;
                    move.performed += instance.OnMove;
                    move.canceled += instance.OnMove;
                    jump.started += instance.OnJump;
                    jump.performed += instance.OnJump;
                    jump.canceled += instance.OnJump;
                    attack.started += instance.OnAttack;
                    attack.performed += instance.OnAttack;
                    attack.canceled += instance.OnAttack;
                    block.started += instance.OnBlock;
                    block.performed += instance.OnBlock;
                    block.canceled += instance.OnBlock;
                    dash.started += instance.OnDash;
                    dash.performed += instance.OnDash;
                    dash.canceled += instance.OnDash;
                }
            }
        }
        public PlayerActions @Player
        {
            get
            {
                return new PlayerActions(this);
            }
        }
        public interface IPlayerActions
        {
            void OnPause(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnBlock(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
        }
    }
}
