using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Util;

namespace PioViewerApi.Server
{
    /// <summary>
    /// Delegator for useful server objects and methods.
    /// </summary>
    public interface IServerUtils
    {
        /// <summary>
        /// Server object for OOP player
        /// </summary>
        IServerPlayer OOP { get; }
        /// <summary>
        /// Server object for IP player
        /// </summary>
        IServerPlayer IP { get; }

        /// <summary>
        /// UPI command
        /// </summary>
        /// <returns></returns>
        IServerCommand CreateCommand(string command, params string[] arguments);

        /// <summary>
        /// Create range by parsing given expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IRange CreateRange(string expression);

        /// <summary>
        /// Create a range using the provided dictionary
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        IRange CreateRange(Dictionary<Hand, double> weights);
    }
}
