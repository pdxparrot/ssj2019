using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class BlockBehaviorComponent : GameCharacterBehaviorComponent
    {
#region Actions
        public class BlockAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public override string ToString()
            {
                return $"BlockAction(Axes: {Axes})";
            }
        }
#endregion

        [SerializeField]
        [ReadOnly]
        private Brawler _brawler;

        public Brawler Brawler { get; set; }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is BlockAction)) {
                return false;
            }

            if(Brawler.CurrentAction.IsBlocking) {
                Brawler.BrawlerBehavior.ToggleBlock();
                return true;
            }

            if(!Brawler.BrawlerBehavior.CanBlock) {
                return false;
            }

            Brawler.BrawlerBehavior.ToggleBlock();

            return true;
        }
    }
}
