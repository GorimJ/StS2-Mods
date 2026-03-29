using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace HandSmoother
{
    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Draw), typeof(PlayerChoiceContext), typeof(decimal), typeof(Player), typeof(bool))]
    public static class HandSmootherPatch
    {
        private static FieldInfo _cardsField = typeof(CardPile).GetField("_cards", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Prefix(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw)
        {
            try
            {
                // Only trigger on the initial starting hand draw.
                if (!fromHandDraw || player.Creature?.CombatState == null || player.Creature.CombatState.RoundNumber != 1) return;

                CardPile drawPile = PileType.Draw.GetPile(player);
                CardPile handPile = PileType.Hand.GetPile(player);
                if (drawPile == null || handPile == null) return;
                
                // Access the underlying _cards list to sort it in place
                List<CardModel> cardsList = (List<CardModel>)_cardsField.GetValue(drawPile);
                if (cardsList == null || cardsList.Count == 0) return;

                int targetDrawCount = Math.Min((int)count, 10 - handPile.Cards.Count);
                if (targetDrawCount <= 0) return;

                // Innate cards are at the top already. We don't touch them.
                int drawIndexOffset = cardsList.FindLastIndex(c => c.Keywords != null && c.Keywords.Contains(CardKeyword.Innate));
                int startIndex = drawIndexOffset == -1 ? 0 : drawIndexOffset + 1;
                int cardsToSort = targetDrawCount - startIndex; // Calculate how many non-innate cards we still need to draw.

            if (cardsToSort <= 0 || cardsList.Count <= startIndex) return; // Prevent sorting if no extra slots or deck is empty

            // Gather the deck we are actually sorting from (the part below innate cards)
            List<CardModel> availableDeck = cardsList.Skip(startIndex).ToList();
            if (availableDeck.Count == 0) return;

            // Calculate ratios based on the WHOLE combat deck to be accurate
            int totalCards = drawPile.Cards.Count;
            
            Dictionary<CardType, int> countsByType = new Dictionary<CardType, int>();
            foreach (var card in drawPile.Cards)
            {
                if (!countsByType.ContainsKey(card.Type)) countsByType[card.Type] = 0;
                countsByType[card.Type]++;
            }

            // Largest Remainder Method for proportional allocation
            Dictionary<CardType, int> allocatedCounts = new Dictionary<CardType, int>();
            Dictionary<CardType, double> remainders = new Dictionary<CardType, double>();

            int remainingToAllocate = Math.Min(cardsToSort, availableDeck.Count); 
            
            foreach (var kvp in countsByType)
            {
                double exactTarget = (kvp.Value * 1.0 / totalCards) * remainingToAllocate;
                allocatedCounts[kvp.Key] = (int)Math.Floor(exactTarget);
                remainders[kvp.Key] = exactTarget - allocatedCounts[kvp.Key];
            }

            int currentAllocated = allocatedCounts.Values.Sum();
            
            // Assign the remaining slots to highest remainders
            var sortedRemainders = remainders.OrderByDescending(x => x.Value).ToList();
            foreach (var kvp in sortedRemainders)
            {
                if (currentAllocated >= remainingToAllocate) break;
                // Only give if we actually have enough cards of that type!
                if(allocatedCounts[kvp.Key] < countsByType[kvp.Key]) 
                {
                    allocatedCounts[kvp.Key]++;
                    currentAllocated++;
                }
            }

            // Now, select exactly that many cards from availableDeck to push to the top
            List<CardModel> selectedCards = new List<CardModel>();
            List<CardModel> remainingDeck = new List<CardModel>(availableDeck);
            
            foreach(var type in allocatedCounts.Keys)
            {
                int needed = allocatedCounts[type];
                for(int i = 0; i < needed; i++)
                {
                    var card = remainingDeck.FirstOrDefault(c => c.Type == type);
                    if (card != null)
                    {
                        selectedCards.Add(card);
                        remainingDeck.Remove(card);
                    }
                }
            }

            // Reassemble the Deck Internal List
            // 0 -> startIndex (Exclusive)
            var finalDeckList = cardsList.Take(startIndex).ToList();
            
            // Add our perfectly smoothed cards
            finalDeckList.AddRange(selectedCards);
            
            // Add the rest randomly
            finalDeckList.AddRange(remainingDeck);

            // Mutate the list in-place to avoid breaking the readonly reference in CardPile
            cardsList.Clear();
            cardsList.AddRange(finalDeckList);
            }
            catch (Exception ex)
            {
                GD.PrintErr($"HandSmoother Mod Error: {ex}");
            }
        }
    }
}
