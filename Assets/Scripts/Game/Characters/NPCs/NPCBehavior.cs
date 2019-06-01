using pdxpartyparrot.Game.Data.Characters;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    public abstract class NPCBehavior : CharacterBehavior
    {
        public NPCBehaviorData NPCBehaviorData => (NPCBehaviorData)BehaviorData;

        public INPC NPC => (INPC)Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is INPC);
        }
#endregion

#region Events
        public virtual void OnRecycle()
        {
        }
#endregion
    }
}
