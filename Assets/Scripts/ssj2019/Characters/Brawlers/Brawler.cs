using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
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

        public float HealthPercent => Health / (float)BrawlerData.MaxHealth;
#endregion

        [Space(10)]

        [SerializeField]
        private BrawlerBehavior _brawlerBehavior;

        public BrawlerBehavior BrawlerBehavior => _brawlerBehavior;

        [SerializeField]
        [ReadOnly]
        private BrawlerData _brawlerData;

        public BrawlerData BrawlerData => _brawlerData;

        public BrawlerCombo BrawlerCombo { get; } = new BrawlerCombo();

        [SerializeField]
        [ReadOnly]
        private BrawlerAction _currentAction;

        public BrawlerModel Model { get; private set; }

        public BrawlerAction CurrentAction
        {
            get => _currentAction;
            set => _currentAction = value;
        }

        public void Initialize(BrawlerData brawlerData)
        {
            _brawlerData = brawlerData;
            BrawlerCombo.Initialize(BrawlerData.Combos);

            _brawlerBehavior.Initialize();
        }

        public void InitializeModel(ActorBehavior behavior, BrawlerModel modelPrefab, GameObject parent, int skinIndex)
        {
            Model = Instantiate(modelPrefab, parent.transform);
            Model.InitializeBehavior(behavior, skinIndex);

            Billboard billboard = parent.GetComponent<Billboard>();
            if(billboard != null) {
                billboard.Camera = GameManager.Instance.Viewer.Camera;
            }
        }

#region Spawn
        public void OnSpawn()
        {
            _health = _brawlerData.MaxHealth;

            CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);

            _brawlerBehavior.OnSpawn();
        }

        public void OnReSpawn()
        {
            _health = _brawlerData.MaxHealth;

            CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);

            _brawlerBehavior.OnReSpawn();
        }
#endregion
    }
}
