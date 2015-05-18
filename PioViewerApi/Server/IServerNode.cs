using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Util;

namespace PioViewerApi.Server
{
    /// <summary>
    /// Representation of a PioSolver tree node.
    /// </summary>
    public interface IServerNode
    {
        /// <summary>
        /// Unique ID of a node as assigned by PioSolver
        /// </summary>
        string NodeId { get; }
        /// <summary>
        /// UPI-specified string denoting the type of this node. (e.g. DECISION_NODE, SPLIT_NODE)
        /// </summary>
        string NodeType { get; }

        /// <summary>
        /// Array representing the board at current node.
        /// </summary>
        Card[] Board { get; }

        /// <summary>
        /// OOP invested, IP invested, Dead
        /// </summary>
        int[] Pot { get; }

        /// <summary>
        /// Link to the parent node. Null for root node.s
        /// </summary>
        IServerNode Parent { get; }

        /// <summary>
        /// Returns name of the player to act if action node, otherwise null;
        /// </summary>
        IServerPlayer ActionPlayer { get; }

        /// <summary>
        /// Human readable description of the node. E.g. RAISE 300, 8h
        /// </summary>
        string NodeText { get; }

        /// <summary>
        /// Set of flags for the node. Might change to the enum in the future
        /// </summary>
        string Flags { get; }

        /// <summary>
        /// Set only for raise and bet nodes. The smallest bet/raise has this value 0, the biggest bet has this value 1.
        /// When there are 4 bet sizes then 4 betting nodes have this index 0, 0.33, 0.66 and 1 respectively in order from the smallest to biggest.
        /// </summary>
        float BettingIndex { get; }
    }
}
