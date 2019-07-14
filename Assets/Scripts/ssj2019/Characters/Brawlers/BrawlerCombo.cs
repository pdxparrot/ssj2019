using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    public sealed class BrawlerCombo
    {
        public interface IComboEntry
        {
            ComboMove Move { get; }

            IReadOnlyDictionary<string, IComboEntry> ComboEntries { get; }

            [CanBeNull]
            IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action);
        }

        private class ComboEntry : IComboEntry
        {
            public ComboMove Move { get; set; }

            public readonly Dictionary<string, IComboEntry> comboEntries = new Dictionary<string, IComboEntry>();

            public IReadOnlyDictionary<string, IComboEntry> ComboEntries => comboEntries;

            [CanBeNull]
            public IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action)
            {
                if(action is DashBehaviorComponent.DashAction) {
                    foreach(var kvp in comboEntries) {
                        if(ComboMove.ComboMoveType.Dash == kvp.Value.Move.Type) {
                            return kvp.Value;
                        }
                    }
                } else if(action is AttackBehaviorComponent.AttackAction) {
                    foreach(var kvp in comboEntries) {
                        // TODO: handle the other attack parameters
                        if(ComboMove.ComboMoveType.Attack == kvp.Value.Move.Type) {
                            return kvp.Value;
                        }
                    }
                }

                return null;
            }
        }

        private ComboEntry _rootComboEntry = new ComboEntry();

        public IComboEntry RootComboEntry => _rootComboEntry;

        public void Initialize(IReadOnlyCollection<ComboData> combos)
        {
            _rootComboEntry.comboEntries.Clear();

            foreach(ComboData comboData in combos) {
                AddComboData(_rootComboEntry, comboData, 0);
            }
        }

        private void AddComboData(ComboEntry comboEntry, ComboData comboData, int depth)
        {
            if(comboData.Moves.Count <= depth) {
                return;
            }

            ComboMove move = comboData.Moves.ElementAt(depth);

            ComboEntry nextComboEntry;
            if(comboEntry.comboEntries.ContainsKey(move.Id)) {
                nextComboEntry = (ComboEntry)comboEntry.comboEntries[move.Id];
            } else {
                nextComboEntry = new ComboEntry{
                    Move = move,
                };
                comboEntry.comboEntries.Add(move.Id, nextComboEntry);
            }

            AddComboData(nextComboEntry, comboData, depth + 1);
        }
    }
}
