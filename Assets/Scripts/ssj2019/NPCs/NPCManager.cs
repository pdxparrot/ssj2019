using System.Collections.Generic;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCManager : SingletonBehavior<NPCManager>
    {
#region Debug
        [SerializeField]
        private bool _npcsImmune;

        public bool NPCsImmune => _npcsImmune;

        [SerializeField]
        private bool _debugBehavior;

        public bool DebugBehavior => _debugBehavior;
#endregion

        private readonly HashSet<NPC> _npcs = new HashSet<NPC>();

        public IReadOnlyCollection<NPC> NPCs => _npcs;

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

        public void Register(NPC npc)
        {
            _npcs.Add(npc);
        }

        public void Unregister(NPC npc)
        {
            _npcs.Remove(npc);
        }

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.NPCManager");
            _debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Players", GUI.skin.box);
                    foreach(NPC npc in _npcs) {
                        GUILayout.Label($"{npc.Id} {npc.Behavior.Movement.Position}");
                    }
                GUILayout.EndVertical();

                _npcsImmune = GUILayout.Toggle(_npcsImmune, "NPCs Immune");
                _debugBehavior = GUILayout.Toggle(_debugBehavior, "Debug Behavior");
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
