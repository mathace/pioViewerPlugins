using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    /// <summary>
    /// Set of two cards
    /// </summary>
    public class Hand : IComparable<Hand>
    {
        private Card[] _cards;
        public Card[] Cards
        {
            set
            {
                this._cards = value.OrderByDescending((c) => c.Rank*13 + c.Suit).ToArray();
            }
            get
            {
                return _cards;
            }
        }

        public Hand(Card c1, Card c2)
        {
            if (c1 == null || c2 == null)
            {
                throw new ArgumentException("Cards cannot be null");
            }
            if (c1.Equals(c2))
            {
                throw new ArgumentException("Two cards must be different");
            }

            this.Cards = new Card[] { c1, c2 };
        }

        public Hand(string text)
        {
            var cards = CardStructures.ParseCards(text);
            if (cards.Length != 2)
            {
                throw new ArgumentException("Cannot parse hand " + text);
            }
            this.Cards = new Card[] { cards[0], cards[1] };
        }

        public override string ToString()
        {
            string result = "";
            foreach (var card in Cards.OrderByDescending((c) => c.Rank))
            {
                result += card.ToString();
            }
            return result;
        }

        public string ToUnicodeString()
        {
            string result = "";
            foreach (var card in Cards.OrderByDescending((c) => c.Rank))
            {
                result += card.ToUnicodeString();
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Hand;
            if (other == null)
            {
                return false;
            }
            return this.GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var card in Cards)
            {
                result = result * 52 + card.GetHashCode();
            }
            return result;
        }

        public int CompareTo(Hand other)
        {
            return this.GetHashCode().CompareTo(other.GetHashCode());
        }

        public bool ContainsOneOf(IEnumerable<Card> cards)
        {
            if (cards == null)
            {
                return false;
            }
            foreach (var card in cards)
            {
                if (_cards[0] == card || _cards[1] == card)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
