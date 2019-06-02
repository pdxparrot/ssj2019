using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerCharacterData", menuName="pdxpartyparrot/ssj2019/Data/Player/PlayerCharacter Data")]
    [Serializable]
    public sealed class PlayerCharacterData : ScriptableObject
    {
        [Serializable]
        public class ReorderableList : ReorderableList<PlayerCharacterData>
        {
        }
    }
}
