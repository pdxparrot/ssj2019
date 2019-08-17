using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Level
{
    public sealed class TrainingLevel : BaseLevel
    {
        private DebugMenuNode _debugMenuNode;

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

        private void SpawnTrainingDummy()
        {
            Debug.Log("Spawning training dummy...");

            Instantiate(GameManager.Instance.GameGameData.TrainingDummyPrefab, transform);
        }

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.Level");
            _debugMenuNode.RenderContentsAction = () => {
                if(GUIUtils.LayoutButton("Spawn Training Dummy")) {
                    SpawnTrainingDummy();
                }
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
