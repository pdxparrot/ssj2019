using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.KungFuCircle;

using UnityEngine;

namespace pdxpartyparrot.Core.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StageManager.Instance.Register(this);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDestroy()
        {
            StageManager.Instance.Unregister(this);
        }
    }
}
