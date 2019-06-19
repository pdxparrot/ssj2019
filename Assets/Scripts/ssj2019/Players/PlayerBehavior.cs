using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockBeginEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockEndEffectTrigger;

        private bool IsAnimating => _attackEffectTrigger.IsRunning || _blockBeginEffectTrigger.IsRunning || _blockEndEffectTrigger.IsRunning;

        private bool CanJump => !IsBlocking;

        private bool CanAttack => !IsBlocking;

        private bool CanBlock => !IsAnimating;

        [SerializeField]
        [ReadOnly]
        private bool _blocking;

        public bool IsBlocking => _blocking;

#region Unity Lifecycle
        protected override void Update()
        {
            base.Update();

            if(IsAnimating) {
                return;
            }

            // process actions here rather than Think() so that they're instantaneous
            if(LastAction is AttackBehaviorComponent.AttackAction) {
                DoAttack();
            }
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);
        }

        private void DoAttack()
        {
            _attackEffectTrigger.Trigger(() => ResetIdle());
        }

        private void ResetIdle()
        {
            SpineAnimationHelper.SetAnimation(GamePlayerBehaviorData.IdleAnimationName, false);
        }

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            ClearActionBuffer();

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove)
        {
            if(!CanAttack) {
                return;
            }

            BufferAction(new AttackBehaviorComponent.AttackAction{
                Axes = lastMove,
            });
        }
        
        public void ToggleBlock()
        {
            if(!CanBlock) {
                return;
            }

            ClearActionBuffer();

            _blocking = !_blocking;
            if(_blocking) {
                _blockBeginEffectTrigger.Trigger();
            } else {
                _blockEndEffectTrigger.Trigger(() => ResetIdle());
            }
        }
#endregion
    }
}
