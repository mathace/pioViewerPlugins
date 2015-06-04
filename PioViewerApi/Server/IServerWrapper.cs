using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Util;

namespace PioViewerApi.Server
{
    /// <summary>
    /// Wrapper for the UPI interface
    /// </summary>
    public interface IServerWrapper
    {
        /// <summary>
        /// Load the tree at given path into memory
        /// </summary>
        /// <param name="path"></param>
        void LoadTree(string path);

        /// <summary>
        /// Dump current in-memory tree to the given path
        /// </summary>
        /// <param name="path"></param>
        void DumpTree(string path);

        /// <summary>
        /// Returns the root node of currently active tree
        /// </summary>
        /// <returns></returns>
        IServerNode ShowRootNode();

        /// <summary>
        /// Refreshes the given node with the new tree state.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IServerNode RefreshNode(IServerNode node);

        /// <summary>
        /// Lists the children of specified node object
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        IEnumerable<IServerNode> ShowChildren(IServerNode node);

        /// <summary>
        /// returns object representing the range of specified player in specified node
        /// </summary>
        /// <param name="player"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        IRange ShowRange(IServerPlayer player, IServerNode node);

        /// <summary>
        /// Returns the list of Ranges, representing strategies in a given node. For each hand in a range of the player in action the strategies sum to 1.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        List<IRange> ShowStrategy(IServerNode node);

        /// <summary>
        /// Calculates the equity of a given player in a given node.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        EVRange CalcEquityInNode(IServerPlayer player, IServerNode node);

        /// <summary>
        /// Calcuate equity for given ranges and board
        /// </summary>
        /// <param name="rangeHero"></param>
        /// <param name="rangeVillain"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        EVRange CalcEquity(IRange rangeHero, IRange rangeVillain, Card[] board);

        /// <summary>
        /// Calculates the expected value of a given player in a given node.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        EVRange CalcEVInNode(IServerPlayer player, IServerNode node);

        /// <summary>
        /// Calculates the expected value of a given player in a given node and all similar nodes (nodes with the same action, but different board cards).
        /// </summary>
        /// <param name="player"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        EVRange CalcEVInLine(IServerPlayer player, IServerNode node);

        /// <summary>
        /// Shows the hands order used by the UPI interface. Always returns the same order.
        /// </summary>
        /// <returns></returns>
        List<Hand> ShowHandsOrder();

        /// <summary>
        /// Run calculations for a specified number of seconds. Set 0 to run indefinitely.
        /// </summary>
        /// <param name="seconds"></param>
        void Go(int seconds);

        /// <summary>
        /// Set the maximum number of threads for the calculation.
        /// </summary>
        /// <param name="threads"></param>
        void SetThreads(int threads);

        /// <summary>
        /// Stop the calculation.
        /// </summary>
        void Stop();

        /// <summary>
        /// Set recalculation accuracy for incomplete trees/
        /// </summary>
        /// <param name="flop"></param>
        /// <param name="turn"></param>
        /// <param name="river"></param>
        void SetRecalcAccuracy(float flop, float turn, float river);

        /// <summary>
        /// Calculate the results. MES and EV for both players and total exploitability.
        /// </summary>
        /// <returns></returns>
        string CalcResults();

        /// <summary>
        /// Estimate the size of a tree.
        /// </summary>
        /// <returns></returns>
        string EstimateTree();

        /// <summary>
        /// Show total physical memory available to the solver.
        /// </summary>
        /// <returns></returns>
        string ShowAvaiablePhysicalMemory();

        /// <summary>
        /// Change strategy in a given node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="childId"></param>
        /// <param name="strategy"></param>
        void SetStrategy(IServerNode node, int childId, IRange strategy);

        /// <summary>
        /// Lock strategy in specified node.
        /// </summary>
        /// <param name="node"></param>
        void LockNode(IServerNode node);

        /// <summary>
        /// Unlock strategy in specified node.
        /// </summary>
        /// <param name="node"></param>
        void UnlockNode(IServerNode node);

        /// <summary>
        /// Execute this command. Has to be single-line UPI command. WARNING: USE WITH CAUTION. Calling multiline command with this method will cause viewer to hang.
        /// </summary>
        /// <param name="commands"></param>
        void ExecuteSingleLineCommands(List<IServerCommand> commands);

        /// <summary>
        /// Execute this command. Returns the result of the command. WARNING: USE WITH CAUTION. Calling multiline command with this method will cause viewer to hang.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string ExecuteSingleLineCommand(IServerCommand command);

        /// <summary>
        /// Execute this command. Returns the result of the command as a multiline string. WARNING: USE WITH CAUTION. Calling single line command with this method will cause viewer to hang.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        string ExecuteMultiLineCommand(IServerCommand command);

        /// <summary>
        /// Calculates the range analysis data for a given board.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        RangeAnalysisData ShowRangeAnalysisData(Card[] board);

        /// <summary>
        /// True if the solver is connected
        /// </summary>
        bool IsConnected { get; }
    }
}
