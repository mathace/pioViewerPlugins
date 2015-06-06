using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    /// <summary>
    /// EV is always the result of calculation Wins/Matchups, except for the case when Matchups is 0.
    /// If Matchups is 0, then Wins are also 0. 
    /// Matchups represent frequency. For example Equity of 1 hand against a range consisting of 1 
    /// hand on the river results in 1 Matchup.
    /// </summary>
    public class EVHolder
    {
        public double EV { get; set; }
        public double Wins { get; set; }
        public double Matchups { get; set; }

        public override string ToString()
        {
            return string.Format("EV: {0:0.00} = {1:0.00} / {2:0.00}", EV, Wins, Matchups);
        }
    }

    /// <summary>
    /// A wrapper for a Dictionary<Hand, EVHolder>. Stores EV object for each hand
    /// </summary>
    public class EVRange : IEnumerable<Hand>
    {
        private Dictionary<Hand, EVHolder> _eVHolders { get; set; }

        public EVRange()
        {
            _eVHolders = new Dictionary<Hand, EVHolder>();
        }

        public EVHolder this[Hand key]
        {
            get
            {
                if (!_eVHolders.ContainsKey(key))
                {
                    _eVHolders[key] = new EVHolder();
                }
                return _eVHolders[key];
            }
            set
            {
                _eVHolders[key] = value;
            }
        }

        public double TotalEV()
        {
            double wins = 0, matchups = 0;
            foreach (var kvp in _eVHolders)
            {
                if (kvp.Value.Matchups > 0)
                {
                    wins += kvp.Value.Wins;
                    matchups += kvp.Value.Matchups;
                }
            }
            return wins / matchups;
        }

        public double TotalEV(IEnumerable<Hand> hands)
        {
            double wins = 0, matchups = 0;
            foreach (var hand in hands)
            {
                if (_eVHolders.ContainsKey(hand))
                {
                    var ev = _eVHolders[hand];
                    if (ev.Matchups > 0)
                    {
                        wins += ev.Wins;
                        matchups += ev.Matchups;
                    }
                }
            }
            return wins / matchups;
        }


        #region sugar

        public double TotalWins()
        {
            double sum = 0.0;
            foreach (var hand in _eVHolders.Keys)
            {
                sum += double.IsNaN(_eVHolders[hand].Wins) ? 0 : _eVHolders[hand].Wins;
            }
            return sum;
        }

        public double TotalMatchups()
        {
            double sum = 0.0;
            foreach (var hand in _eVHolders.Keys)
            {
                sum += double.IsNaN(_eVHolders[hand].Matchups) ? 0 : _eVHolders[hand].Matchups;
            }
            return sum;
        }

        public Dictionary<Hand, double> GetEV()
        {
            var result = new Dictionary<Hand, double>();
            foreach (var hand in _eVHolders.Keys)
            {
                result[hand] = _eVHolders[hand].Wins / _eVHolders[hand].Matchups;
            }
            return result;
        }
        #endregion

        #region Ienumerable
        public IEnumerator<Hand> GetEnumerator()
        {
            foreach (var hand in _eVHolders.Keys)
            {
                yield return hand;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var hand in _eVHolders.Keys)
            {
                yield return hand;
            }
        }
        #endregion
    }
}
