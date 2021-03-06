﻿using UnityEngine;

namespace pdxpartyparrot.ssj2019.Volumes
{
    public sealed class BlockVolume : ActionVolume
    {
#region Unity Lifecycle
        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            base.OnDrawGizmos();
        }
#endregion

        public void SetBlock(Vector3 offset, Vector3 size, Vector3 direction, string boneName)
        {
            offset.x *= Mathf.Sign(direction.x);

            Offset = offset;
            Size = size;

            BoneFollower.SetBone(boneName);
        }
    }
}
