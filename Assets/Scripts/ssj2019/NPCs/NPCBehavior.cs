using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using Spine;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    // TODO: split this into behavior components
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior
    {
        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

        [Header("Animations")]

#region Attack Animations
        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _attackAnimationEffectTriggerComponent;
#endregion

#region Block Animations
        [SerializeField]
        private EffectTrigger _blockBeginEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockBeginAnimationEffectTriggerComponent;

        [SerializeField]
        private EffectTrigger _blockEndEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockEndAnimationEffectTriggerComponent;
#endregion

#region Hit Animations
        [SerializeField]
        private EffectTrigger _hitEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _hitAnimationEffectTriggerComponent;
#endregion

        [Space(10)]

#region Action Volumes
        [Header("Action Volumes")]

        [SerializeField]
        private AttackVolume _attackVolume;

        [SerializeField]
        private BlockVolume _blockVolume;
#endregion

        private bool CanJump => !IsBlocking;

        private bool CanAttack => !IsBlocking;

        private bool CanBlock => IsGrounded;

        [SerializeField]
        [ReadOnly]
        private bool _blocking;

        public bool IsBlocking => _blocking;

        [SerializeField]
        [ReadOnly]
        private bool _parry;

        public bool IsParry => _parry;

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune => _immune;

        public override bool CanMove => base.CanMove && !IsBlocking;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _attackVolume.gameObject.SetActive(false);
            _blockVolume.gameObject.SetActive(false);

            _attackAnimationEffectTriggerComponent.StartEvent += AttackAnimationStartHandler;
            _attackAnimationEffectTriggerComponent.CompleteEvent += AttackAnimationCompleteHandler;

            _blockBeginAnimationEffectTriggerComponent.StartEvent += BlockBeginAnimationStartHandler;
            _blockBeginAnimationEffectTriggerComponent.CompleteEvent += BlockBeginAnimationCompleteHandler;

            _blockEndAnimationEffectTriggerComponent.StartEvent += BlockEndAnimationStartHandler;
            _blockEndAnimationEffectTriggerComponent.CompleteEvent += BlockEndAnimationCompleteHandler;

            _hitAnimationEffectTriggerComponent.StartEvent += HitAnimationStartHandler;
            _hitAnimationEffectTriggerComponent.CompleteEvent += HitAnimationCompleteHandler;
        }

        // TODO: probably safest if we release the events in OnDestroy

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

        private void ResetIdle()
        {
            SpineAnimationHelper.SetAnimation(GameNPCBehaviorData.IdleAnimationName, false);
        }

        private void DoAttack()
        {
            _attackEffectTrigger.Trigger(() => ResetIdle());
        }

        private void EnableAttackVolume(bool enable)
        {
            _attackVolume.gameObject.SetActive(enable);
        }

        private void EnableBlockVolume(bool enable)
        {
            _blockVolume.gameObject.SetActive(enable);
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
            if(_blocking) {
                _blocking = false;
                _blockEndEffectTrigger.Trigger(() => ResetIdle());
                return;
            }

            if(!CanBlock) {
                return;
            }

            ClearActionBuffer();

            _blocking = true;
            _blockBeginEffectTrigger.Trigger();
        }
#endregion

#region Events
        public void OnDamage(Actor source, string type, int amount)
        {
            Debug.Log($"NPC {Owner.Id} damaged by {source.Id}");
        }
#endregion

#region Event Handlers
        private void AttackAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += AttackAnimationEvent;
        }

        private void AttackAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= AttackAnimationEvent;
        }

        private void AttackAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(GameNPCBehaviorData.AttackVolumeSpawnEvent == evt.Data.Name) {
                EnableAttackVolume(true);
            } else if(GameNPCBehaviorData.AttackVolumeDeSpawnEvent == evt.Data.Name) {
                EnableAttackVolume(false);
            } else {
                Debug.LogWarning($"Unhandled attack event: {evt.Data.Name}");
            }
        }

        private void BlockBeginAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += BlockBeginAnimationEvent;
        }

        private void BlockBeginAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= BlockBeginAnimationEvent;
        }

        private void BlockBeginAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(GameNPCBehaviorData.BlockVolumeSpawnEvent == evt.Data.Name) {
                EnableBlockVolume(true);
            } else if(GameNPCBehaviorData.ParryWindowOpenEvent == evt.Data.Name) {
                _parry = true;
            } else if(GameNPCBehaviorData.ParryWindowCloseEvent == evt.Data.Name) {
                _parry = false;
            } else {
                Debug.Log($"Unhandled block begin event: {evt.Data.Name}");
            }
        }

        private void BlockEndAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += BlockEndAnimationEvent;
        }

        private void BlockEndAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= BlockEndAnimationEvent;
        }

        private void BlockEndAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(GameNPCBehaviorData.BlockVolumeDeSpawnEvent == evt.Data.Name) {
                EnableBlockVolume(false);
            } else {
                Debug.Log($"Unhandled block end event: {evt.Data.Name}");
            }
        }

        private void HitAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += HitAnimationEvent;
        }

        private void HitAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= HitAnimationEvent;
        }

        private void HitAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(GameNPCBehaviorData.HitImpactEvent == evt.Data.Name) {
                // TODO: damage
            } else {
                Debug.Log($"Unhandled hit end event: {evt.Data.Name}");
            }
        }
#endregion
    }
}
