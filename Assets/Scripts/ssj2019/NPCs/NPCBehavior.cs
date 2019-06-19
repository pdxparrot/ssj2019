using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior
    {
        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockBeginEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockEndEffectTrigger;

        private bool CanJump => !IsBlocking;

        private bool CanAttack => !IsBlocking;

        private bool CanBlock => true;

        [SerializeField]
        [ReadOnly]
        private bool _blocking;

        public bool IsBlocking => _blocking;

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune => _immune;

        public override bool CanMove => base.CanMove && !IsBlocking;

#region Unity Lifecycle
        protected override void Update()
        {
            base.Update();

            // process actions here rather than Think() so that they're instantaneous
            if(LastAction is AttackBehaviorComponent.AttackAction && !_attackEffectTrigger.IsRunning) {
                DoAttack();
            }
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBehaviorData);

            base.Initialize(behaviorData);
        }

        public override void Think(float dt)
        {
            // TODO: here we think but also queue movement and actions
        }

        private void DoAttack()
        {
            _attackEffectTrigger.Trigger(() => ResetIdle());
        }

        private void ResetIdle()
        {
            SpineAnimationHelper.SetAnimation(GameNPCBehaviorData.IdleAnimationName, false);
        }

#region Spawn
        public override void OnDeSpawn()
        {
            GameManager.Instance.LevelHelper.WaveSpawner.CurrentWave.OnWaveSpawnMemberDone();

            base.OnDeSpawn();
        }
#endregion

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            //ClearActionBuffer();

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
