using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(PooledObject))]
    public sealed class NPC : NPC3D
    {
        public NPCBehavior GameNPCBehavior => (NPCBehavior)NPCBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private NPCCharacterData _characterData;

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

            Behavior.SpriteAnimationHelper.AddRenderer(model.ModelSprite);
            Behavior.SpriteAnimationHelper.AddRenderer(model.ShadowSprite);
        }
    }
}
