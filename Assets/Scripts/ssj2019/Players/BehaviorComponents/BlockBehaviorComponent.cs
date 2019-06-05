namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class BlockBehaviorComponent : GamePlayerBehaviorComponent
    {
#region Actions
        public class BlockAction : CharacterBehaviorAction
        {
            public static BlockAction Default = new BlockAction();
        }
#endregion

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is BlockAction)) {
                return false;
            }

// TODO

            return true;
        }
    }
}
