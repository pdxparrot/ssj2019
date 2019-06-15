using pdxpartyparrot.ssj2019.Data;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior
    {
        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

#region Spawn
        public override void OnDeSpawn()
        {
            GameManager.Instance.LevelHelper.WaveSpawner.CurrentWave.OnWaveSpawnMemberDone();

            base.OnDeSpawn();
        }
#endregion
    }
}
