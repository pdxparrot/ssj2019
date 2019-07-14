using System.Collections.Generic;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.NPCs
{
    // TODO this needs a Game level parent
    public sealed class NPCManager : SingletonBehavior<NPCManager>
    {
#region Debug
        [SerializeField]
        private bool _npcsImmune;

        public bool NPCsImmune => _npcsImmune;

        [SerializeField]
        private bool _debugBehavior;

        public bool DebugBehavior => _debugBehavior;

        [SerializeField]
        private bool _dumbBrawlers;

        public bool DumbBrawlers => _dumbBrawlers;
#endregion

        private readonly HashSet<INPC> _npcs = new HashSet<INPC>();

        public IReadOnlyCollection<INPC> NPCs => _npcs;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }
#endregion

        public void Register(INPC npc)
        {
            _npcs.Add(npc);
        }

        public void Unregister(INPC npc)
        {
            _npcs.Remove(npc);
        }

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.NPCManager");
            _debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Players", GUI.skin.box);
                    foreach(INPC npc in _npcs) {
                        GUILayout.Label($"{npc.Id} {npc.Behavior.Movement.Position}");
                    }
                GUILayout.EndVertical();

                _npcsImmune = GUILayout.Toggle(_npcsImmune, "NPCs Immune");
                _debugBehavior = GUILayout.Toggle(_debugBehavior, "Debug Behavior");

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
