﻿using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(PooledObject))]
    public sealed class NPC : NPC3D, IDamagable
    {
        public NPCBehavior GameNPCBehavior => (NPCBehavior)NPCBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private NPCCharacterData _characterData;

        public NPCCharacterData NPCCharacterData => _characterData;

        [Space(10)]

#region Stats
        [Header("Stats")]

        [SerializeField]
        [ReadOnly]
        private int _health;

        public int Health
        {
            get => _health;
            set => _health = value;
        }
#endregion

        public bool IsDead => Health < 1;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();
            
            Assert.IsTrue(NPCBehavior is NPCBehavior);
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

        private void InitializeStats()
        {
            Health = _characterData.MaxHealth;
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            InitializeStats();

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            InitializeStats();

            return true;
        }
#endregion

        public void Damage(Actor source, string type, int amount)
        {
            GameNPCBehavior.OnDamage(source, type, amount);
        }
    }
}
