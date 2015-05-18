using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    public static class CardStructures
    {
        /// <summary>
        /// Cards[rank, color]
        /// Cards[12, 1] = "AD"
        /// </summary>
        public static readonly Card[,] Cards;

        /// <summary>
        /// All 1326 hands
        /// </summary>
        public static readonly Hand[] Hands;

        static CardStructures()
        {
            Cards = new Card[13, 4];
            for (int c = 0; c < 4; c++)
            {
                for (int i = 0; i < 13; i++)
                {
                    Cards[i, c] = new Card() { Rank = i, Suit = c };
                }
            }

            var hands = new List<Hand>();
            foreach (var c1 in Cards)
                foreach (var c2 in Cards)
                {
                    if (c1.CompareTo(c2) < 0)
                    {
                        hands.Add(new Hand(c1, c2));
                    }
                }
            Hands = hands.ToArray();
        }

        public static Card[] ParseCards(string cardString)
        {
            List<int> ranks = new List<int>();
            List<int> suits = new List<int>();

            foreach (var ch in cardString.ToUpper().ToCharArray())
            {
                if (Card.SuitsUpper.Contains(ch))
                {
                    suits.Add(Card.SuitsUpper.IndexOf(ch));
                }
                if (Card.Ranks.Contains(ch))
                {
                    ranks.Add(Card.Ranks.IndexOf(ch));
                }
            }


            Card[] cards = new Card[Math.Min(ranks.Count, suits.Count)];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = Cards[ranks[i], suits[i]];
            }
            return cards;
        }
    }
}
