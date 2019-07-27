using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Core.Scenes;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Game.State
{
    public abstract class GameState : MonoBehaviour
    {
        public string Name => name;

        [SerializeField]
        [FormerlySerializedAs("_sceneName")]
        private string _initialSceneName;

        [SerializeField]
        [ReadOnly]
        private string _currentSceneName;

        public string CurrentSceneName
        {
            get => _currentSceneName;
            protected set => _currentSceneName = value;
        }

        public bool HasScene => !string.IsNullOrWhiteSpace(CurrentSceneName);

        [SerializeField]
        [FormerlySerializedAs("_makeSceneActive")]
        private bool _makeInitialSceneActive;

        public bool MakeInitialSceneActive => _makeInitialSceneActive;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _currentSceneName = _initialSceneName;
        }
#endregion

        public IEnumerator<float> LoadSceneRoutine()
        {
            if(!HasScene) {
                yield break;
            }

            IEnumerator<float> runner = SceneManager.Instance.LoadSceneRoutine(CurrentSceneName, MakeInitialSceneActive);
            while(runner.MoveNext()) {
                yield return runner.Current;
            }
        }

        public IEnumerator<float> UnloadSceneRoutine()
        {
            if(!HasScene) {
                yield break;
            }

            if(SceneManager.HasInstance) {
                IEnumerator<float> runner = SceneManager.Instance.UnloadSceneRoutine(CurrentSceneName);
                while(runner.MoveNext()) {
                    yield return runner.Current;
                }
            }
        }

        public void ChangeSceneAsync(string sceneName, Action onComplete)
        {
            StartCoroutine(ChangeSceneRoutine(sceneName, onComplete));
        }

        private IEnumerator ChangeSceneRoutine(string sceneName, Action onComplete)
        {
            IEnumerator<float> runner = UnloadSceneRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            CurrentSceneName = sceneName;

            runner = LoadSceneRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            onComplete?.Invoke();
        }

        public virtual IEnumerator<LoadStatus> OnEnterRoutine()
        {
            Debug.Log($"Enter State: {Name}");

            yield break;
        }

        public virtual IEnumerator<LoadStatus> OnExitRoutine()
        {
            Debug.Log($"Exit State: {Name}");

            yield break;
        }

        public virtual void OnResume()
        {
            Debug.Log($"Resume State: {Name}");
        }

        public virtual void OnPause()
        {
            Debug.Log($"Pause State: {Name}");
        }

        public virtual void OnUpdate(float dt)
        {
        }
    }
}
