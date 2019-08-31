using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Volumes;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class BlockBehaviorComponent : GameCharacterBehaviorComponent
    {
#region Actions
        public class BlockAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public bool Cancel { get; set; }

            public override string ToString()
            {
                return $"BlockAction(Axes: {Axes})";
            }
        }
#endregion

        [SerializeField]
        private BrawlerBehavior _brawlerBehavior;

        [SerializeField]
        private BlockVolume _blockVolume;

        [SerializeField]
        private EffectTrigger _blockBeginEffectTrigger;

        [SerializeField]
        private EffectTrigger _blockEndEffectTrigger;

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is BlockAction blockAction)) {
                return false;
            }

            return blockAction.Cancel ? EndBlock() : StartBlock();
        }

        private bool StartBlock()
        {
            if(!_brawlerBehavior.CanBlock) {
                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                _brawlerBehavior.DisplayDebugText("Begin Block", Color.cyan);
            }

            _brawlerBehavior.CancelActions(false);

            // TODO: calling Initialize() here is dumb, but we can't do it in our own Initialize()
            // because the models haven't been initialized yet (and that NEEDS to get changed cuz this is dumb)
            _blockVolume.Initialize(_brawlerBehavior.Brawler.Model.SpineModel);
            _blockVolume.SetBlock(_brawlerBehavior.Brawler.BrawlerData.BlockVolumeOffset, _brawlerBehavior.Brawler.BrawlerData.BlockVolumeSize, Behavior.Owner.FacingDirection, _brawlerBehavior.Brawler.BrawlerData.BlockBoneName);

            _brawlerBehavior.Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Block);
            _blockBeginEffectTrigger.Trigger();

            return true;
        }

        private bool EndBlock()
        {
            if(!_brawlerBehavior.Brawler.CurrentAction.IsBlocking) {
                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                _brawlerBehavior.DisplayDebugText("End Block", Color.cyan);
            }

            _blockEndEffectTrigger.Trigger(() => {
                _brawlerBehavior.Idle();
            });

            return true;
        }
    }
}
