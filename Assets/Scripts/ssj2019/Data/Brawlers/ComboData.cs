using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data.Brawlers
{
    [CreateAssetMenu(fileName="ComboData", menuName="pdxpartyparrot/ssj2019/Data/Brawlers/Combo Data")]
    [Serializable]
    public sealed class ComboData : ScriptableObject
    {
        [SerializeField]
        private string _name;

        public string Name => _name;

        [SerializeField]
        [ReorderableList]
        private ComboMove.ReorderableList _moves = new ComboMove.ReorderableList();

        public IReadOnlyCollection<ComboMove> Moves => _moves.Items;
    }
}
