using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Data.NPCs;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    public sealed class NPCBartenderBehavior : NPCBehavior
    {
        private enum State
        {
            Idle
        }

        public NPCBartenderBehaviorData NPCBartenderBehaviorData => (NPCBartenderBehaviorData)NPCBehaviorData;

        public NPCBartender NPCBartender => (NPCBartender)Owner;

        public override Vector3 MoveDirection => Vector3.zero;

        [SerializeField]
        [ReadOnly]
        private State _state = State.Idle;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is NPCBartender);
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBartenderBehaviorData);

            base.Initialize(behaviorData);
        }

        public override void Think(float dt)
        {
            switch(_state)
            {
            case State.Idle:
                HandleIdle();
                break;
            }
        }

#region NPC State
        private void SetState(State state)
        {
            if(NPCManager.Instance.DebugBehavior) {
                Debug.Log($"NPCBartender {Owner.Id} set state {state}");
            }

            _state = state;
            switch(_state)
            {
            case State.Idle:
                NPCOwner.ResetPath();
                break;
            }
        }

        private void HandleIdle()
        {
        }
#endregion

#region Spawn
        public override void OnSpawn(SpawnPoint spawnpoint)
        {
            base.OnSpawn(spawnpoint);

            SetState(State.Idle);
        }

        public override void OnReSpawn(SpawnPoint spawnpoint)
        {
            base.OnReSpawn(spawnpoint);

            SetState(State.Idle);
        }
#endregion
    }
}
