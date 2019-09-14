using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    public sealed class Brawler : MonoBehaviour
    {
        public Actor Actor { get; private set; }

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

        [SerializeField]
        [ReadOnly]
        private BrawlerModel _model;

        public BrawlerModel Model
        {
            get => _model;
            private set => _model = value;
        }

        public BrawlerAction CurrentAction
        {
            get => _currentAction;
            set
            {
                if(GameManager.Instance.DebugBrawlers) {
                    _brawlerBehavior.DisplayDebugText($"Action: {value}", Color.magenta);
                }
                _currentAction = value;
            }
        }

        public void Initialize(Actor actor, BrawlerData brawlerData)
        {
            Actor = actor;

            _brawlerData = brawlerData;
            BrawlerCombo.Initialize(BrawlerData.Combos);

            _brawlerBehavior.Initialize(this);
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

        public void OnDeSpawn()
        {
            if(null != Model) {
                Destroy(Model.gameObject);
                Model = null;
            }
        }
#endregion
    }
}
