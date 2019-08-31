using System;

using pdxpartyparrot.ssj2019.Characters.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.BehaviorComponents
{
    public sealed class DashBehaviorComponent : Game.Characters.BehaviorComponents.DashBehaviorComponent
    {
        [SerializeField]
        private BrawlerBehavior _brawlerBehavior;

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is DashAction)) {
                return false;
            }

            if(!_brawlerBehavior.AdvanceCombo(action)) {
                _brawlerBehavior.ComboFail();
                return true;
            }

            if(GameManager.Instance.DebugBrawlers) {
                _brawlerBehavior.DisplayDebugText("Dash", Color.cyan);
            }

            _brawlerBehavior.Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Dash);

            base.OnPerformed(action);

            return true;
        }

        // TODO: these are temporary until we have a dash animation
#region Event Handlers
        protected override void DashStopEventHandler(object sender, EventArgs args)
        {
            base.DashStopEventHandler(sender, args);

            if(!_brawlerBehavior.DashAnimationCompleteHandler()) {
                _brawlerBehavior.Idle();
            }
        }

        protected override void DashTimesUpEventHandler(object sender, EventArgs args)
        {
            base.DashTimesUpEventHandler(sender, args);

            if(!_brawlerBehavior.DashAnimationCompleteHandler()) {
                _brawlerBehavior.Idle();
            }
        }
#endregion
    }
}
