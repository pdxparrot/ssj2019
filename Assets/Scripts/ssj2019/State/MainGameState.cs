﻿using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.State
{
    public sealed class MainGameState : Game.State.MainGameState
    {
        protected override bool InitializeServer()
        {
            if(!base.InitializeServer()) {
                Debug.LogWarning("Failed to initialize server!");
                return false;
            }

            GameManager.Instance.StartGameServer();

            return true;
        }

        protected override bool InitializeClient()
        {
            // need to init the viewer before we start spawning players
            // so that they have a viewer to attach to
            ViewerManager.Instance.AllocateViewers(1, GameManager.Instance.GameGameData.ViewerPrefab);
            GameManager.Instance.InitViewer();

            // need this before players spawn
            GameUIManager.Instance.InitializePlayerUI(GameManager.Instance.Viewer.UICamera);
            GameUIManager.Instance.GamePlayerUI.HUD.SetScore(0);

            if(!base.InitializeClient()) {
                Debug.LogWarning("Failed to initialize client!");
                return false;
            }

            GameManager.Instance.StartGameClient();

            return true;
        }
    }
}
