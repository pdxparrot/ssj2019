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

            IReadOnlyCollection<IComboEntry> ComboEntries { get; }

            [CanBeNull]
            IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action);
        }

        private class ComboEntry : IComboEntry
        {
            public ComboMove Move { get; set; }

            public readonly List<IComboEntry> comboEntries = new List<IComboEntry>();

            public IReadOnlyCollection<IComboEntry> ComboEntries => comboEntries;

            [CanBeNull]
            public IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action)
            {
                if(action is DashBehaviorComponent.DashAction) {
                    foreach(IComboEntry comboEntry in comboEntries) {
                        if(ComboMove.ComboMoveType.Dash == comboEntry.Move.Type) {
                            return comboEntry;
                        }
                    }
                } else if(action is AttackBehaviorComponent.AttackAction) {
                    foreach(IComboEntry comboEntry in comboEntries) {
                        // TODO: handle the other attack parameters
                        if(ComboMove.ComboMoveType.Attack == comboEntry.Move.Type) {
                            return comboEntry;
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

            ComboEntry nextComboEntry = null;
            foreach(IComboEntry entry in comboEntry.comboEntries) {
                if(comboEntry.Move.Equals(entry.Move)) {
                    nextComboEntry = (ComboEntry)entry;
                    break;
                }
            }

            if(null == nextComboEntry) {
                nextComboEntry = new ComboEntry{
                    Move = move,
                };
                comboEntry.comboEntries.Add(nextComboEntry);
            }

            AddComboData(nextComboEntry, comboData, depth + 1);
        }
    }
}
