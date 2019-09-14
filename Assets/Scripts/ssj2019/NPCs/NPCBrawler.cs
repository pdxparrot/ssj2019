using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.NPCs;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(PooledObject))]
    [RequireComponent(typeof(Brawler))]
    public sealed class NPCBrawler : NPC25D, IDamagable, IInteractable
    {
        public NPCBrawlerBehavior NPCBrawlerBehavior => (NPCBrawlerBehavior)NPCBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private NPCBrawlerData _npcBrawlerData;

        public NPCBrawlerData NPCBrawlerData => _npcBrawlerData;

        public bool IsDead => Brawler.Health < 1;

        public bool CanInteract => !IsDead;

        public Brawler Brawler { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();
            
            Assert.IsTrue(NPCBehavior is NPCBrawlerBehavior);

            Brawler = GetComponent<Brawler>();
        }
#endregion

        public override void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBrawlerBehaviorData);

            base.Initialize(id, behaviorData);

            _npcBrawlerData = NPCBrawlerBehavior.NPCBrawlerBehaviorData.BrawlerCharacterOptions.GetRandomEntry();

            Brawler.Initialize(this, NPCBrawlerData.BrawlerData);

            InitializeModel();
        }

        private void InitializeModel()
        {
            if(null == Model) {
                return;
            }

            Brawler.InitializeModel(Behavior, NPCBrawlerData.BrawlerModelPrefab, Model, NPCBrawlerData.SkinIndex);
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.RegisterNPC(this);

            Brawler.OnSpawn();

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.RegisterNPC(this);

            Brawler.OnReSpawn();

            return true;
        }

        public override void OnDeSpawn()
        {
            NPCManager.Instance.UnregisterNPC(this);

            Brawler.ShutdownModel(Behavior);

            base.OnDeSpawn();
        }
#endregion

        public bool Damage(DamageData damageData)
        {
            return NPCBrawlerBehavior.OnDamage(damageData);
        }
    }
}
