using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
#region Event Handlers
        public override void OnStart()
        {
            Debug.Log("TOOD: Start()");

            //Owner.PushPanel(_characterSelectPanel);
//            GameStateManager.Instance.StartLocal(GameManager.Instance.GameGameData.MainGameStatePrefab);
        }
#endregion
    }
}
