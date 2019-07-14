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
                Debug.Log($"Brawler {Behavior.Owner.Id} starting dash");
            }

            _brawlerBehavior.Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Dash);

            base.OnPerformed(action);

            return true;
        }

#region Event Handlers
        protected override void DashStopEventHandler(object sender, EventArgs args)
        {
            base.DashStopEventHandler(sender, args);

            _brawlerBehavior.Idle();
        }

        protected override void DashTimesUpEventHandler(object sender, EventArgs args)
        {
            base.DashTimesUpEventHandler(sender, args);

            _brawlerBehavior.Idle();
        }
#endregion
    }
}
