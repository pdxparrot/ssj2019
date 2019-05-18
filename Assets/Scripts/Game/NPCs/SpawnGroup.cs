using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    [Serializable]
    internal class SpawnGroup
    {
        private readonly SpawnGroupData _spawnGroupData;

        [SerializeField]
        [ReadOnly]
        private ITimer _spawnTimer;

        private string PoolTag => $"spawnGroup_{_spawnGroupData.Tag}";

        private GameObject _poolContainer;

        private readonly  WaveSpawner _owner;

        public SpawnGroup(SpawnGroupData spawnGroupData, WaveSpawner owner)
        {
            _spawnGroupData = spawnGroupData;
            _owner = owner;
        }

        public void Initialize(float waveDuration)
        {
            PooledObject pooledObject = _spawnGroupData.ActorPrefab.GetComponent<PooledObject>();
            if(null != pooledObject) {
                _poolContainer = new GameObject(PoolTag);
                _poolContainer.transform.SetParent(_owner.transform);

                int count = Mathf.Max(_spawnGroupData.PoolSize, 1);
                if(ObjectPoolManager.Instance.HasPool(PoolTag)) {
                    ObjectPoolManager.Instance.EnsurePoolSize(PoolTag, count);
                } else {
                    ObjectPoolManager.Instance.InitializePoolAsync(PoolTag, pooledObject, count);
                }
            }

            _spawnTimer = TimeManager.Instance.AddTimer();
            _spawnTimer.TimesUpEvent += SpawnTimerTimesUpEventHandler;
        }

        public void Shutdown()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_spawnTimer);
            }
            _spawnTimer = null;

            if(null != _poolContainer) {
                if(ObjectPoolManager.HasInstance) {
                    ObjectPoolManager.Instance.DestroyPool(PoolTag);
                }

                UnityEngine.Object.Destroy(_poolContainer);
                _poolContainer = null;
            }
        }

        public void Start()
        {
            _spawnTimer.Start(_spawnGroupData.Delay);
        }

        public void Stop()
        {
            _spawnTimer.Stop();
        }

        private void Spawn()
        {
            int amount = _spawnGroupData.Count.GetRandomValue();
            for(int i=0; i<amount; ++i) {
                var spawnPoint = SpawnManager.Instance.GetSpawnPoint(_spawnGroupData.Tag);
                if(null == spawnPoint) {
                    //Debug.LogWarning($"No spawnpoints for {_spawnGroupData.Tag}!");
                } else {
                    Actor actor = null;
                    if(null != _poolContainer) {
                        actor = ObjectPoolManager.Instance.GetPooledObject<Actor>(PoolTag, _poolContainer.transform);
                        if(null == actor) {
                            Debug.LogWarning($"Actor for pool {PoolTag} missing its PooledObject!");
                            continue;
                        }

                        if(!spawnPoint.Spawn(actor, Guid.NewGuid(), _spawnGroupData.NPCBehaviorData)) {
                            actor.GetComponent<PooledObject>().Recycle();
                            Debug.LogWarning($"Failed to spawn actor for {_spawnGroupData.Tag}");
                            continue;
                        }
                    } else {
                        actor = spawnPoint.SpawnFromPrefab(_spawnGroupData.ActorPrefab, Guid.NewGuid(), _spawnGroupData.NPCBehaviorData, _poolContainer.transform);
                        if(null == actor) {
                            continue;
                        }
                    }
                }

                if(!_spawnGroupData.Once) {
                    _spawnTimer.Start(_spawnGroupData.Delay);
                }
            }
        }

#region Event Handlers
        private void SpawnTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            Spawn();
        }
#endregion
    }
}
