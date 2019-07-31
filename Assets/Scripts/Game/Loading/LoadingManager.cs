﻿using System.Collections.Generic;

using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Game.Cinematics;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Game.Loading
{
    public abstract class LoadingManager<T> : Core.Loading.LoadingManager<LoadingManager<T>> where T: LoadingManager<T>
    {
        [Space(10)]

#region Manager Prefabs
        [Header("Game Manager Prefabs")]

        [SerializeField]
        private GameStateManager _gameStateManagerPrefab;

        [SerializeField]
        [FormerlySerializedAs("_uiManagerPrefab")]
        private GameUIManager _gameUIManagerPrefab;

        [SerializeField]
        private DialogueManager _dialogManagerPrefab;

        [SerializeField]
        private CinematicsManager _cinematicsManagerPrefab;
#endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameStateManager.CreateFromPrefab(_gameStateManagerPrefab, ManagersContainer);
            GameUIManager.CreateFromPrefab(_gameUIManagerPrefab, ManagersContainer);
            CinematicsManager.CreateFromPrefab(_cinematicsManagerPrefab, ManagersContainer);
            DialogueManager.CreateFromPrefab(_dialogManagerPrefab, ManagersContainer);
            HighScoreManager.Create(ManagersContainer);
        }

        protected override IEnumerator<LoadStatus> OnLoadRoutine()
        {
            IEnumerator<LoadStatus> runner = base.OnLoadRoutine();
            while(runner.MoveNext()) {
                yield return runner.Current;
            }

            GameStateManager.Instance.TransitionToInitialStateAsync();
        }
    }
}
