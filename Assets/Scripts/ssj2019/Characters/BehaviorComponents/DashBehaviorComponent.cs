using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class DashBehaviorComponent : GameCharacterBehaviorComponent
    {
#region Actions
        public class DashAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public override string ToString()
            {
                return $"DashAction(Axes: {Axes})";
            }
        }
#endregion

        [SerializeField]
        [ReadOnly]
        private Brawler _brawler;

        public Brawler Brawler { get; set; }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is DashAction dashAction)) {
                return false;
            }

            Brawler.BrawlerBehavior.Dash(dashAction);

            return true;
        }
    }
}
