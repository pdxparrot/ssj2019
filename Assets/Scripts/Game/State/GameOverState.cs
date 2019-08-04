using System;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Game.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public abstract class GameOverState : SubGameState
    {
        [SerializeField]
        private Menu.Menu _menuPrefab;

        private Menu.Menu _menu;

        [SerializeField]
        private float _completeWaitTimeSeconds = 5.0f;

        private ITimer _completeTimer;

        [SerializeField]
        private AudioClip _endGameMusic;

#region Unity Lifecycle
        private void OnDestroy()
        {
            // have to remove the time here because OnExit() is tied to the timer completing
            // which mucks up the TimeManager internal state
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_completeTimer);
            }
            _completeTimer = null;
        }
#endregion

        public override void OnEnter()
        {
            base.OnEnter();

            AudioManager.Instance.StopAllMusic();
            AudioManager.Instance.PlayMusic(_endGameMusic);

            if(null == _menuPrefab) {
                _completeTimer = TimeManager.Instance.AddTimer();
                _completeTimer.TimesUpEvent += CompleteTimerTimesUpEventHandler;
                _completeTimer.Start(_completeWaitTimeSeconds);
            } else {
                InputManager.Instance.EventSystem.UIModule.EnableAllActions();

                _menu = GameUIManager.Instance.InstantiateUIPrefab(_menuPrefab);
                _menu.Initialize();
            }
        }

        public override void OnExit()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.DisableAllActions();
            }

            if(null != _menu) {
                Destroy(_menu.gameObject);
                _menu = null;
            }

            AudioManager.Instance.StopAllMusic();

            base.OnExit();
        }

        public virtual void Initialize()
        {
        }

#region Event Handlers
        private void CompleteTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            GameStateManager.Instance.TransitionToInitialStateAsync();
        }
#endregion
    }
}
