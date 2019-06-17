using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockEffectTrigger;

#region Unity Lifecycle
        protected override void Update()
        {
            base.Update();

            if(_attackEffectTrigger.IsRunning || _blockEffectTrigger.IsRunning) {
                return;
            }

            // process actions here rather than Think() so that they're instantaneous
            if(LastAction is AttackBehaviorComponent.AttackAction) {
                DoAttack();
            } else if(LastAction is BlockBehaviorComponent.BlockAction) {
                DoBlock();
            }
        }
#endregion

        private void DoAttack()
        {
            _attackEffectTrigger.Trigger(() => {
                ClearActionBuffer();
            });
        }

        private void DoBlock()
        {
            _blockEffectTrigger.Trigger(() => {
                ClearActionBuffer();
            });
        }

#region Events
        public void OnJump()
        {
            ClearActionBuffer();

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void OnAttack(Vector3 lastMove)
        {
            BufferAction(new AttackBehaviorComponent.AttackAction{
                Axes = lastMove,
            });
        }

        // TODO: we might want the entire move buffer
        public void OnBlock(Vector3 lastMove)
        {
            BufferAction(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
            });
        }
#endregion
    }
}
