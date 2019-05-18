using pdxpartyparrot.Game.Data.Characters;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    public interface INPCBehavior
    {
        NPCBehaviorData NPCBehaviorData { get; }

        INPC NPC { get; }

#region Events
        void OnRecycle();
#endregion
    }
}
