using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
#region Event Handlers
        public override void OnStart()
        {
            //Owner.PushPanel(_characterSelectPanel);
//            GameStateManager.Instance.StartLocal(GameManager.Instance.GameGameData.MainGameStatePrefab);
        }
#endregion
    }
}
