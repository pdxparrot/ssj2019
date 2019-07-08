using System;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Data.NPCs;

using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCBartender : NPC3D
    {
        public NPCBartenderBehavior NPCBartenderBehavior => (NPCBartenderBehavior)NPCBehavior;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();
            
            Assert.IsTrue(NPCBehavior is NPCBartenderBehavior);
        }
#endregion

        public override void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBartenderBehaviorData);

            base.Initialize(id, behaviorData);
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.Register(this);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.Register(this);

            return true;
        }

        public override void OnDeSpawn()
        {
            NPCManager.Instance.Unregister(this);

            base.OnDeSpawn();
        }
#endregion
    }
}
