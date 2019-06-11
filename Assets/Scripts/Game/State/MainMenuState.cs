﻿using System.Collections.Generic;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.Game.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public sealed class MainMenuState : GameState
    {
        [SerializeField]
        private Menu.Menu _menuPrefab;

        private Menu.Menu _menu;

        [SerializeField]
        private AudioClip _music;

        public override IEnumerator<LoadStatus> OnEnterRoutine()
        {
            yield return new LoadStatus(0.0f, "Initializing main menu...");

            IEnumerator<LoadStatus> runner = base.OnEnterRoutine();
            while(runner.MoveNext()) {
                yield return runner.Current;
            }

            yield return new LoadStatus(0.5f, "Initializing main menu...");

            InputManager.Instance.EventSystem.UIModule.EnableAllActions();

            AudioManager.Instance.PlayMusic(_music);

            _menu = GameUIManager.Instance.InstantiateUIPrefab(_menuPrefab);

            yield return new LoadStatus(1.0f, "Main menu initialized!");
        }

        public override IEnumerator<LoadStatus> OnExitRoutine()
        {
            yield return new LoadStatus(0.0f, "Shutting down main menu...");

            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopAllMusic();
            }

            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.DisableAllActions();
            }

            Destroy(_menu.gameObject);
            _menu = null;

            yield return new LoadStatus(0.5f, "Shutting down main menu...");

            IEnumerator<LoadStatus> runner = base.OnExitRoutine();
            while(runner.MoveNext()) {
                yield return runner.Current;
            }

            yield return new LoadStatus(1.0f, "Main menu shut down!");
        }

        public override void OnResume()
        {
            base.OnResume();

            _menu.gameObject.SetActive(true);
        }

        public override void OnPause()
        {
            _menu.gameObject.SetActive(false);

            base.OnPause();
        }
    }
}
