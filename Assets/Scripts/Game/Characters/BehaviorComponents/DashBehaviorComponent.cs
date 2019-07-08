using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    public class DashBehaviorComponent : CharacterBehaviorComponent
    {
#region Actions
        public class DashAction : CharacterBehaviorAction
        {
            public static DashAction Default = new DashAction();
        }
#endregion

        [SerializeField]
        private DashBehaviorComponentData _data;

        public DashBehaviorComponentData DashBehaviorComponentData
        {
            get => _data;
            set => _data = value;  
        }

#region Effects
        [Header("Effects")]

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _dashEffect;
#endregion

        private ITimer _dashTimer;

        public bool IsDashing => _dashTimer.IsRunning;

        private ITimer _cooldownTimer;

        public bool IsDashCooldown => _cooldownTimer.IsRunning;

        public bool CanDash => !IsDashing && !IsDashCooldown;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _dashTimer = TimeManager.Instance.AddTimer();
            _dashTimer.TimesUpEvent += DashTimesUpEventHandler;

            _cooldownTimer = TimeManager.Instance.AddTimer();
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_dashTimer);
                TimeManager.Instance.RemoveTimer(_cooldownTimer);
            }

            _dashTimer = null;
            _cooldownTimer = null;

            base.OnDestroy();
        }
#endregion

        public override bool OnPhysicsUpdate(float dt)
        {
            if(!IsDashing) {
                return false;
            }

// TODO: base movement but faster

            return false;
        }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is DashAction)) {
                return false;
            }

            _dashTimer.Start(DashBehaviorComponentData.DashTimeSeconds);

            if(null != _dashEffect) {
                _dashEffect.Trigger();
            }

            return true;
        }

#region Event Handlers
        private void DashTimesUpEventHandler(object sender, EventArgs args)
        {
            _cooldownTimer.Start(DashBehaviorComponentData.DashCooldownSeconds);
        }
#endregion
    }
}
