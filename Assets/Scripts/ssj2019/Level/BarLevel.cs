using System;

using pdxpartyparrot.Game.NPCs;
using pdxpartyparrot.ssj2019.Data.NPCs;
using pdxpartyparrot.ssj2019.NPCs;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Level
{
    public sealed class BarLevel : BaseLevel
    {
        [SerializeField]
        private WaveSpawner _waveSpawnerPrefab;

        public WaveSpawner WaveSpawner { get; private set; }

        [SerializeField]
        private NPCBartenderBehaviorData _bartenderBehaviorData;

        [SerializeField]
        private NPCBartender _bartender;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // disable the bartender so its components don't start going crazy
            _bartender.gameObject.SetActive(false);
        }

        protected override void OnDestroy()
        {
            ShutdownWaveSpawner();

            base.OnDestroy();
        }
#endregion

        private void InitializeWaveSpawner()
        {
            if(null == _waveSpawnerPrefab) {
                return;
            }

            WaveSpawner = Instantiate(_waveSpawnerPrefab);
            WaveSpawner.Initialize();

            WaveSpawner.WaveStartEvent += WaveStartEventHandler;
            WaveSpawner.WaveCompleteEvent += WaveCompleteEventHandler;
        }

        private void ShutdownWaveSpawner()
        {
            if(null == WaveSpawner) {
                return;
            }

            Destroy(WaveSpawner);
            WaveSpawner = null;
        }

#region Event Handlers
        protected override void GameStartServerEventHandler(object sender, EventArgs args)
        {
            base.GameStartServerEventHandler(sender, args);

            InitializeWaveSpawner();
        }

        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            PlayerManager.Instance.GamePlayerUI.HUD.SetWave(1);
            PlayerManager.Instance.GamePlayerUI.HUD.SetScore(0);
        }

        protected override void GameReadyEventHandler(object sender, EventArgs args)
        {
            base.GameReadyEventHandler(sender, args);

            // initialize the bartender now that it's safe
            _bartender.gameObject.SetActive(true);
            _bartender.Initialize(Guid.NewGuid(), _bartenderBehaviorData);

            // TODO: this should wait until after all of the players are ready
            if(null != WaveSpawner) {
                WaveSpawner.StartSpawner();
            }
        }

        protected override void GameOverEventHandler(object sender, EventArgs args)
        {
            base.GameOverEventHandler(sender, args);

            if(null != WaveSpawner) {
                WaveSpawner.StopSpawner();
            }
        }

        private void WaveStartEventHandler(object sender, SpawnWaveEventArgs args)
        {
            PlayerManager.Instance.GamePlayerUI.HUD.SetWave(args.WaveIndex + 1);
        }

        private void WaveCompleteEventHandler(object sender, SpawnWaveEventArgs args)
        {
            if(!args.IsFinalWave) {
                return;
            }

            TransitionLevel();
        }
#endregion
    }
}
