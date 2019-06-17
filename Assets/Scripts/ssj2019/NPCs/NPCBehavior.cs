using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior
    {
        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

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

        public override void Think(float dt)
        {
            // TODO: here we think but also queue movement and actions
        }

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

#region Spawn
        public override void OnDeSpawn()
        {
            GameManager.Instance.LevelHelper.WaveSpawner.CurrentWave.OnWaveSpawnMemberDone();

            base.OnDeSpawn();
        }
#endregion
    }
}
