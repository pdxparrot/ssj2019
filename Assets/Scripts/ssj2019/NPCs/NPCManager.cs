using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Game.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCManager : NPCManager<NPCManager>
    {
        private DebugMenuNode _debugMenuNode;

#region Debug
        [SerializeField]
        private bool _dumbBrawlers;

        public bool DumbBrawlers => _dumbBrawlers;
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }
#endregion

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.NPCManager");
            _debugMenuNode.RenderContentsAction = () => {
                _dumbBrawlers = GUILayout.Toggle(_dumbBrawlers, "Dumb Brawlers");
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
