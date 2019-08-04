// GENERATED AUTOMATICALLY FROM 'Assets/Data/Input/ServerSpectator.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace pdxpartyparrot.Game.Input
{
    public class ServerSpectatorControls : IInputActionCollection
    {
        private InputActionAsset asset;
        public ServerSpectatorControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""ServerSpectator"",
    ""maps"": [
        {
            ""name"": ""ServerSpectator"",
            ""id"": ""ec5f26a9-62cb-4b87-a738-18ecf77fce9d"",
            ""actions"": [
                {
                    ""name"": ""move forward"",
                    ""type"": ""Button"",
                    ""id"": ""5f680737-f97a-44bb-a4a9-42297a5656c6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move backward"",
                    ""type"": ""Button"",
                    ""id"": ""298a78da-e8b2-425d-b32e-99704d97cf30"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move left"",
                    ""type"": ""Button"",
                    ""id"": ""ba3ee732-32c5-4934-9178-f3000ee7738f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move right"",
                    ""type"": ""Button"",
                    ""id"": ""abe9a4c8-45f9-463d-aad9-c020d80d2707"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move up"",
                    ""type"": ""Button"",
                    ""id"": ""e7eae091-7558-45cf-8634-c087fae63693"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""move down"",
                    ""type"": ""Button"",
                    ""id"": ""93ec50f1-df3c-44cd-a1a8-12a64595be61"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""look"",
                    ""type"": ""Value"",
                    ""id"": ""751f7475-d7c4-4878-81e8-c2c8156be6c1"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7e61ac38-6c73-4bf5-b82a-dccb8a7c85e0"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move forward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""02649163-897d-4cac-8ea0-caeb98c335ba"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move backward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""60cb2adc-fb3e-43f2-92fe-d7fa4cd2a02e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb00bded-3835-4c97-9456-bac9eab4aaa3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""688ff567-4f3b-4139-9d89-b2c3dd62d15b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move up"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""881b7fb3-408e-4076-b522-7152574a5ffa"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move down"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0396b14a-618b-46a7-89b2-38521b860245"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // ServerSpectator
            m_ServerSpectator = asset.GetActionMap("ServerSpectator");
            m_ServerSpectator_moveforward = m_ServerSpectator.GetAction("move forward");
            m_ServerSpectator_movebackward = m_ServerSpectator.GetAction("move backward");
            m_ServerSpectator_moveleft = m_ServerSpectator.GetAction("move left");
            m_ServerSpectator_moveright = m_ServerSpectator.GetAction("move right");
            m_ServerSpectator_moveup = m_ServerSpectator.GetAction("move up");
            m_ServerSpectator_movedown = m_ServerSpectator.GetAction("move down");
            m_ServerSpectator_look = m_ServerSpectator.GetAction("look");
        }

        ~ServerSpectatorControls()
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

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

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

        // ServerSpectator
        private readonly InputActionMap m_ServerSpectator;
        private IServerSpectatorActions m_ServerSpectatorActionsCallbackInterface;
        private readonly InputAction m_ServerSpectator_moveforward;
        private readonly InputAction m_ServerSpectator_movebackward;
        private readonly InputAction m_ServerSpectator_moveleft;
        private readonly InputAction m_ServerSpectator_moveright;
        private readonly InputAction m_ServerSpectator_moveup;
        private readonly InputAction m_ServerSpectator_movedown;
        private readonly InputAction m_ServerSpectator_look;
        public struct ServerSpectatorActions
        {
            private ServerSpectatorControls m_Wrapper;
            public ServerSpectatorActions(ServerSpectatorControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @moveforward => m_Wrapper.m_ServerSpectator_moveforward;
            public InputAction @movebackward => m_Wrapper.m_ServerSpectator_movebackward;
            public InputAction @moveleft => m_Wrapper.m_ServerSpectator_moveleft;
            public InputAction @moveright => m_Wrapper.m_ServerSpectator_moveright;
            public InputAction @moveup => m_Wrapper.m_ServerSpectator_moveup;
            public InputAction @movedown => m_Wrapper.m_ServerSpectator_movedown;
            public InputAction @look => m_Wrapper.m_ServerSpectator_look;
            public InputActionMap Get() { return m_Wrapper.m_ServerSpectator; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(ServerSpectatorActions set) { return set.Get(); }
            public void SetCallbacks(IServerSpectatorActions instance)
            {
                if (m_Wrapper.m_ServerSpectatorActionsCallbackInterface != null)
                {
                    moveforward.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveforward;
                    moveforward.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveforward;
                    moveforward.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveforward;
                    movebackward.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovebackward;
                    movebackward.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovebackward;
                    movebackward.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovebackward;
                    moveleft.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveleft;
                    moveleft.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveleft;
                    moveleft.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveleft;
                    moveright.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveright;
                    moveright.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveright;
                    moveright.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveright;
                    moveup.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveup;
                    moveup.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveup;
                    moveup.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMoveup;
                    movedown.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovedown;
                    movedown.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovedown;
                    movedown.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnMovedown;
                    look.started -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnLook;
                    look.performed -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnLook;
                    look.canceled -= m_Wrapper.m_ServerSpectatorActionsCallbackInterface.OnLook;
                }
                m_Wrapper.m_ServerSpectatorActionsCallbackInterface = instance;
                if (instance != null)
                {
                    moveforward.started += instance.OnMoveforward;
                    moveforward.performed += instance.OnMoveforward;
                    moveforward.canceled += instance.OnMoveforward;
                    movebackward.started += instance.OnMovebackward;
                    movebackward.performed += instance.OnMovebackward;
                    movebackward.canceled += instance.OnMovebackward;
                    moveleft.started += instance.OnMoveleft;
                    moveleft.performed += instance.OnMoveleft;
                    moveleft.canceled += instance.OnMoveleft;
                    moveright.started += instance.OnMoveright;
                    moveright.performed += instance.OnMoveright;
                    moveright.canceled += instance.OnMoveright;
                    moveup.started += instance.OnMoveup;
                    moveup.performed += instance.OnMoveup;
                    moveup.canceled += instance.OnMoveup;
                    movedown.started += instance.OnMovedown;
                    movedown.performed += instance.OnMovedown;
                    movedown.canceled += instance.OnMovedown;
                    look.started += instance.OnLook;
                    look.performed += instance.OnLook;
                    look.canceled += instance.OnLook;
                }
            }
        }
        public ServerSpectatorActions @ServerSpectator => new ServerSpectatorActions(this);
        public interface IServerSpectatorActions
        {
            void OnMoveforward(InputAction.CallbackContext context);
            void OnMovebackward(InputAction.CallbackContext context);
            void OnMoveleft(InputAction.CallbackContext context);
            void OnMoveright(InputAction.CallbackContext context);
            void OnMoveup(InputAction.CallbackContext context);
            void OnMovedown(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
        }
    }
}
