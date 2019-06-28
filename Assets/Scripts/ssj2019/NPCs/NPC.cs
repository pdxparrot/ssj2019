using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(PooledObject))]
    [RequireComponent(typeof(Brawler))]
    public sealed class NPC : NPC3D, IDamagable, IInteractable
    {
        public NPCBehavior GameNPCBehavior => (NPCBehavior)NPCBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private NPCCharacterData _characterData;

        public NPCCharacterData NPCCharacterData => _characterData;

        public bool IsDead => Brawler.Health < 1;

        public bool CanInteract => !IsDead;

        public Brawler Brawler { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();
            
            Assert.IsTrue(NPCBehavior is NPCBehavior);

            Brawler = GetComponent<Brawler>();
        }
#endregion

        public override void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBehaviorData);

            base.Initialize(id, behaviorData);

            _characterData = GameNPCBehavior.GameNPCBehaviorData.CharacterOptions.GetRandomEntry();

            InitializeModel();

            Billboard billboard = Model.GetComponent<Billboard>();
            if(billboard != null) {
                billboard.Camera = GameManager.Instance.Viewer.Camera;
            }
        }

        private void InitializeModel()
        {
            if(null == Model) {
                return;
            }

            CharacterModel model = Instantiate(_characterData.CharacterModelPrefab, Model.transform);

            if(null != model.ModelSprite) {
                Behavior.SpriteAnimationHelper.AddRenderer(model.ModelSprite);
            }

            if(null != model.SpineModel) {
                Behavior.SpineAnimationHelper.SkeletonAnimation = model.SpineModel;

                Behavior.SpineSkinHelper.SkeletonAnimation = model.SpineModel;
                if(_characterData.SkinIndex < 0) {
                    Behavior.SpineSkinHelper.SetRandomSkin();
                } else {
                    Behavior.SpineSkinHelper.SetSkin(_characterData.SkinIndex);
                }
            }

            Behavior.SpriteAnimationHelper.AddRenderer(model.ShadowSprite);
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.Register(this);

            Brawler.Initialize(NPCCharacterData.BrawlerData);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            NPCManager.Instance.Register(this);

            Brawler.Initialize(NPCCharacterData.BrawlerData);

            return true;
        }

        public override void OnDeSpawn()
        {
            NPCManager.Instance.Unregister(this);

            base.OnDeSpawn();
        }
#endregion

        public bool Damage(Actor source, string type, int amount, Bounds attackBounds, Vector3 force)
        {
            return GameNPCBehavior.OnDamage(source, type, amount, attackBounds, force);
        }
    }
}
