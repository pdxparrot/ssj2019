using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class Brawler : MonoBehaviour
    {
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

        [Space(10)]

        [SerializeField]
        private BrawlerBehavior _brawlerBehavior;

        public BrawlerBehavior BrawlerBehavior => _brawlerBehavior;

        [SerializeField]
        [ReadOnly]
        private BrawlerData _brawlerData;

        public BrawlerData BrawlerData => _brawlerData;

        [SerializeField]
        [ReadOnly]
        private BrawlerAction _currentAction;

        public CharacterModel CharacterModel { get; private set; }

        public BrawlerAction CurrentAction
        {
            get => _currentAction;
            set => _currentAction = value;
        }

        public void Initialize(BrawlerData brawlerData)
        {
            _brawlerData = brawlerData;

            _health = _brawlerData.MaxHealth;

            CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
        }

        public void InitializeModel(ActorBehavior behavior, CharacterModel prefab, GameObject parent, int skinIndex)
        {
            CharacterModel = Instantiate(prefab, parent.transform);
            CharacterModel.InitializeBehavior(behavior, skinIndex);

            Billboard billboard = parent.GetComponent<Billboard>();
            if(billboard != null) {
                billboard.Camera = GameManager.Instance.Viewer.Camera;
            }
        }
    }
}
