using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    /// <summary>
    /// Representation of a card. e.g. Ace of spades.
    /// 
    /// Properties Rank and Suit can be used to acces information about well, rank and suit of a card.
    /// ToString() and ToUnicodeString() are the methods to convert this card into human readable string.
    /// 
    /// </summary>
    public class Card : IComparable<Card>
    {
        public static readonly string Ranks = "23456789TJQKA";
        public static readonly string Suits = "cdhs";
        public static readonly string SuitsUpper = "CDHS";

        private static String[] UnicodeSuits = new String[] { "\u2663", "\u2666", "\u2665", "\u2660" };

        public int Rank { get; set; }
        public int Suit { get; set; }

        public override string ToString()
        {
            return Ranks[Rank] + "" + Suits[Suit];
        }

        public string ToUnicodeString()
        {
            return Ranks[Rank] + "" + UnicodeSuits[Suit];
        }

        public override bool Equals(object obj)
        {
            var other = obj as Card;
            if (other == null)
            {
                return false;
            }
            return other.Rank == this.Rank && other.Suit == this.Suit;
        }

        public override int GetHashCode()
        {
            return this.Rank * 4 + this.Suit;
        }

        public int CompareTo(Card other)
        {
            return -this.GetHashCode().CompareTo(other.GetHashCode());
        }
    }

}
