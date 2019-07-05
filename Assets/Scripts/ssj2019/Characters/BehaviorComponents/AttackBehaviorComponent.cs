using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class AttackBehaviorComponent : GameCharacterBehaviorComponent
    {
#region Actions
        public class AttackAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public override string ToString()
            {
                return $"AttackAction(Axes: {Axes})";
            }
        }
#endregion

        [SerializeField]
        [ReadOnly]
        private Brawler _brawler;

        public Brawler Brawler { get; set; }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is AttackAction)) {
                return false;
            }

            Brawler.BrawlerBehavior.Attack();

            return true;
        }
    }
}
