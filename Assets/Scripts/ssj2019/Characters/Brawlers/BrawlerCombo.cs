﻿using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    public sealed class BrawlerCombo
    {
        public interface IComboEntry
        {
            ComboMove Move { get; }

            IReadOnlyCollection<IComboEntry> ComboEntries { get; }

            [CanBeNull]
            IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action, IComboEntry previousMove, BrawlerAction currentAction);
        }

        private class ComboEntry : IComboEntry
        {
            public ComboMove Move { get; set; }

            public readonly List<IComboEntry> comboEntries = new List<IComboEntry>();

            public IReadOnlyCollection<IComboEntry> ComboEntries => comboEntries;

            [CanBeNull]
            public IComboEntry NextEntry(CharacterBehaviorComponent.CharacterBehaviorAction action, IComboEntry previousMove, BrawlerAction currentAction)
            {
                if(action is DashBehaviorComponent.DashAction) {
                    foreach(IComboEntry comboEntry in comboEntries) {
                        if(ComboMove.ComboMoveType.Dash == comboEntry.Move.Type) {
                            return comboEntry;
                        }
                    }
                } else if(action is AttackBehaviorComponent.AttackAction attackAction) {
                    foreach(IComboEntry comboEntry in comboEntries) {
                        if(comboEntry.Move.Equals(attackAction) && (null == previousMove || !Move.RequireHit || !previousMove.Move.IsAttack || currentAction.DidHit)) {
                            return comboEntry;
                        }
                    }

                    // if we failed and this is the opener
                    // we'll be kind and fall back on the directionless attack
                    if(null == previousMove) {
                        if(GameManager.Instance.DebugBrawlers) {
                            Debug.Log($"Fallback on directionless attack");
                        }

                        foreach(IComboEntry comboEntry in comboEntries) {
                            if(comboEntry.Move.IsDirectionlessAttack) {
                                return comboEntry;
                            }
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

            bool hasDirectionlessOpener = false;
            foreach(ComboData comboData in combos) {
                ComboMove openerMove = AddComboData(_rootComboEntry, comboData, 0);
                hasDirectionlessOpener = hasDirectionlessOpener || (null != openerMove && openerMove.IsDirectionlessAttack);
            }
            Assert.IsTrue(hasDirectionlessOpener, "Brawler combo requires a directionless opener attack");
        }

        [CanBeNull]
        private ComboMove AddComboData(ComboEntry comboEntry, ComboData comboData, int depth)
        {
            if(comboData.Moves.Count <= depth) {
                return null;
            }

            ComboMove move = comboData.Moves.ElementAt(depth);

            ComboEntry nextComboEntry = null;
            foreach(IComboEntry entry in comboEntry.comboEntries) {
                if(move.Equals(entry.Move)) {
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

            return move;
        }
    }
}
