using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    /// <summary>
    /// Immutable representation of a range. Functionally an immutable Dictionary<Hand, double>
    /// </summary>
    public interface IRange : IEnumerable<Hand>
    {
        /// <summary>
        /// Get the weights of a given hand in range by using the following syntax:
        /// var range = server.ShowRange(OOP, node);
        /// range[new Hand("AcAd")] -> Get's the weight for the AcAd hand
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        double this[Hand key] { get; }

        Dictionary<Hand, double> Weights
        {
            get;
        }

        /// <summary>
        /// Create the copy of a range where the most frequent hand in rescaled to one
        /// </summary>
        /// <returns></returns>
        IRange RescaledToOne();

        /// <summary>
        /// Returns the copy of a range where all weight are multiplied by a constant
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        IRange MultilpiedBy(double multiplier);

        /// <summary>
        /// Teturn the copy of a range where all weights are multiplied by a corresponding weight in other range
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        IRange MultilpiedBy(IRange multiplier);

        /// <summary>
        /// Return a copy of a range with hands that overlap with the board removed.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        IRange FilterBy(Card[] board);

        /// <summary>
        /// Returns a human readable format of the range.
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Sorted in descending orders values. All numbers are shown (even zero and negative) no hands are grouped together
        /// </summary>
        /// <returns></returns>
        string ToNumericalString(string splitString);

        /// <summary>
        /// Writes a string in parsable single-line format
        /// </summary>
        /// <returns></returns>
        string ToSingleLineString();

        /// <summary>
        /// Range representation in a format understandable by solver
        /// </summary>
        /// <param name="handsOrder"></param>
        /// <returns></returns>
        string To1326String(Hand[] handsOrder);

        double TotalWeights();
        double TotalWeights(IEnumerable<Card> DeadCards);
    }

}
