using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Data.Characters;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters
{
    public abstract class CharacterBehavior : ActorBehavior
    {
        private struct ActionBufferEntry
        {
            public long TimestampMs { get; }

            public CharacterBehaviorComponent.CharacterBehaviorAction Action { get; }

            public ActionBufferEntry(CharacterBehaviorComponent.CharacterBehaviorAction action)
            {
                TimestampMs = TimeManager.Instance.CurrentUnixMs;
                Action = action;
            }

            public bool Expired(int timeout)
            {
                return TimeManager.Instance.CurrentUnixMs - TimestampMs > timeout;
            }
        }

        public ICharacterMovement CharacterMovement => (ICharacterMovement)Movement;

        [CanBeNull]
        public CharacterBehaviorData CharacterBehaviorData => (CharacterBehaviorData)BehaviorData;

        [Header("Components")]

        [SerializeField]
        [ReorderableList]
        private CharacterBehaviorComponent.ReorderableList _components = new CharacterBehaviorComponent.ReorderableList();

        [Space(10)]

#region Physics
        [Header("Character Physics")]

        [SerializeField]
        [ReadOnly]
        private bool _isGrounded;

        public bool IsGrounded
        {
            get => _isGrounded;
            set => _isGrounded = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _isSliding;

        public bool IsSliding
        {
            get => _isSliding;
            set => _isSliding = value;
        }

        public bool IsFalling => Movement.UseGravity && (!IsGrounded && !IsSliding && Movement.Velocity.y < 0.0f);
#endregion

        public override bool CanMove => base.CanMove && !GameStateManager.Instance.GameManager.IsGameOver;

#region Action Buffer
        [CanBeNull]
        private CircularBuffer<ActionBufferEntry> _actionBuffer;

        public CharacterBehaviorComponent.CharacterBehaviorAction NextAction => null == _actionBuffer || _actionBuffer.Count < 1 ? null : _actionBuffer.Head.Action;
#endregion

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Movement is CharacterMovement3D);

            base.Awake();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();

            while(null != _actionBuffer && _actionBuffer.Count > 0 && _actionBuffer.Head.Expired(CharacterBehaviorData.ActionBufferTimeoutMs)) {
                _actionBuffer.RemoveOldest();
            }

            if(null != Animator) {
                Animator.SetBool(CharacterBehaviorData.FallingParam, IsFalling);
            }
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is CharacterBehaviorData);

            base.Initialize(behaviorData);

            _actionBuffer = new CircularBuffer<ActionBufferEntry>(CharacterBehaviorData.ActionBufferSize);

            foreach(CharacterBehaviorComponent component in _components.Items) {
                component.Initialize(this);
            }

            InitDebugMenu();
        }

#region Components
        [CanBeNull]
        public T GetBehaviorComponent<T>() where T: CharacterBehaviorComponent
        {
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    return tc;
                }
            }
            return null;
        }

        public void GetBehaviorComponents<T>(ICollection<T> components) where T: CharacterBehaviorComponent
        {
            components.Clear();
            foreach(var component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    components.Add(tc);
                }
            }
        }

        public void RunOnComponents(Func<CharacterBehaviorComponent, bool> f)
        {
            foreach(CharacterBehaviorComponent component in _components.Items) {
                if(f(component)) {
                    return;
                }
            }
        }
#endregion

#region Action Buffer
        public void BufferAction(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            _actionBuffer?.Add(new ActionBufferEntry(action));
        }

        public void PopNextAction()
        {
            _actionBuffer?.RemoveOldest();
        }

        public void ClearActionBuffer()
        {
            _actionBuffer?.Clear();
        }
#endregion

#region Actions
        public virtual void ActionStarted(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnStarted(action));
        }

        public virtual void ActionPerformed(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnPerformed(action));
        }

        public virtual void ActionCancelled(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            RunOnComponents(c => c.OnCancelled(action));
        }
#endregion

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            RunOnComponents(c => c.OnAnimationUpdate(dt));
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            RunOnComponents(c => c.OnPhysicsUpdate(dt));
        }

        protected void AlignToMovement(Vector3 forward)
        {
            if(!IsMoving) {
                return;
            }

            SetFacing(forward);
        }

#region Debug Menu
        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => $"Game.CharacterBehavior {Owner.Id}");
            _debugMenuNode.RenderContentsAction = () => {
                if(null != _actionBuffer) {
                    GUILayout.BeginVertical("Action Buffer", GUI.skin.box);
                        foreach(var action in _actionBuffer) {
                            GUILayout.Label(action.ToString());
                        }
                    GUILayout.EndVertical();
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
#endregion
    }
}
