using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.KungFuCircle
{
    public sealed class StageManager : SingletonBehavior<StageManager>
    {
        [SerializeField]
        [ReadOnly]
        private int GridCount;

        private void Start()
        {
            GridCount = 0;
        }

        public void Register<T>(T grid) where T : KungFuGrid
        {
            GridCount++;
        }

        public void Unregister<T>(T grid) where T : KungFuGrid
        {
            GridCount--;
        }
    }
}
