using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Plugin;
using PioViewerApi.Server;
using PioViewerApi.Util;

namespace PioViewerPlugins.Aggregation
{
    class ReportLine
    {
        public Card[] Board { get; set; }
        public double EVOOPSums { get; set; }
        public double EVOOPMatchups { get; set; }
        public double EVIPSums { get; set; }
        public double EVIPMatchups { get; set; }

        public double EQOOPSums { get; set; }
        public double EQOOPMatchups { get; set; }
        public double EQIPSums { get; set; }
        public double EQIPMatchups { get; set; }

        public List<string> ActionNames { get; set; }
        public Dictionary<string, double> ActionFrequencies { get; set; }

        private ReportLine()
        {
            ActionFrequencies = new Dictionary<string, double>();
            ActionNames = new List<string>();
        }

        /// <summary>
        /// Get the results from one particular node into the structure
        /// </summary>
        public static ReportLine CreateReportLineForNode(IServerNode node, IServerPluginContext context)
        {
            var result = new ReportLine();

            var OOP = context.ServerUtils.OOP;
            var IP = context.ServerUtils.IP;


            //the board
            result.Board = node.Board;

            // Calculate IP and OOP ev and equity in the node.
            // The result for EV calculation is two ranges
            // one range is sums and the other is matchups
            // EV for a given hand can be calculated by dividing
            // sums by matchups.
            // The total EV in node can be obtained by diving
            // total sums in the node by total counters
            var evIP = context.ServerWrapper.CalcEVInNode(IP, node);
            var evOOP = context.ServerWrapper.CalcEVInNode(OOP, node);

            result.EVOOPSums = evOOP.TotalWins();
            result.EVOOPMatchups = evOOP.TotalMatchups();
            result.EVIPSums = evIP.TotalWins();
            result.EVIPMatchups = evIP.TotalMatchups();

            var eqIP = context.ServerWrapper.CalcEquityInNode(IP, node);
            var eqOOP = context.ServerWrapper.CalcEquityInNode(OOP, node);

            result.EQOOPSums = eqOOP.TotalWins();
            result.EQOOPMatchups = eqOOP.TotalMatchups();
            result.EQIPSums = eqIP.TotalWins();
            result.EQIPMatchups = eqIP.TotalMatchups();

            // If the node is action node (e.g. - not a street split node nor final node)
            // then we also compute how often given action is selected
            if (node.ActionPlayer != null)
            {
                // we ask server to list all children of a current node
                var actions = context.ServerWrapper.ShowChildren(node);
                foreach (var action in actions)
                {
                    // for each child we ask for the range of an action player
                    var rangeForAction = context.ServerWrapper.ShowRange(node.ActionPlayer, action);
                    // and store it in the result dictionary
                    // NodeText property denotes the action (e.g. CHECK or BET 100)
                    result.ActionFrequencies[action.NodeText] = rangeForAction.TotalWeights();
                    // Node name is stored separately to preserve the action's order
                    result.ActionNames.Add(action.NodeText);
                }
            }
            return result;
        }

        private void AddBoard(List<String> result, Card[] board)
        {
            if (board.Length >= 3)
            {
                result.Add(board[0] + " " + board[1] + " " + board[2]);
            }
            if (board.Length >= 4)
            {
                result.Add("" + board[3]);
            }
            if (board.Length >= 5)
            {
                result.Add("" + board[4]);
            }
        }

        /// <summary>
        /// Generates a list of calvulated values for the columns in a single line of a report
        /// </summary>
        public List<String> ToCSVLine(double rootMatchups)
        {
            var result = new List<String>();
            AddBoard(result, this.Board);
            result.Add((this.EQOOPMatchups * 100 / rootMatchups).ToString("G6"));
            result.Add((this.EQOOPSums / this.EQOOPMatchups).ToString("G6"));
            result.Add((this.EQIPSums / this.EQIPMatchups).ToString("G6"));
            result.Add((this.EVOOPSums / this.EVOOPMatchups).ToString("G6"));
            result.Add((this.EVIPSums / this.EVIPMatchups).ToString("G6"));
            double[] frequencies = new double[this.ActionNames.Count];
            for (int i = 0; i < frequencies.Length; i++)
            {
                var actionName = this.ActionNames[i];
                frequencies[i] = this.ActionFrequencies[actionName];
            }
            double sum = frequencies.Sum();
            foreach (var f in frequencies)
            {
                result.Add((f * 100 / sum).ToString("G3"));
            }
            return result;
        }

        /// <summary>
        /// Sum the results in a single structure
        /// </summary>
        /// <param name="fullReport"></param>
        /// <returns></returns>
        public static ReportLine ToSummaryCSVLine(List<ReportLine> fullReport)
        {
            var result = new ReportLine();

            if (fullReport.Count > 0)
            {
                result.ActionNames = fullReport[0].ActionNames;
            }

            foreach (var line in fullReport)
            {
                if (result.Board == null)
                {
                    result.Board = new Card[line.Board.Length];
                    for (int i = 0; i < 3; i++)
                    {
                        result.Board[i] = line.Board[i];
                    }
                }
                foreach (var actionName in line.ActionFrequencies.Keys)
                {
                    if (!result.ActionFrequencies.ContainsKey(actionName))
                    {
                        result.ActionFrequencies[actionName] = 0;
                    }
                    result.ActionFrequencies[actionName] += line.ActionFrequencies[actionName];
                }
                result.EVIPMatchups += line.EVIPMatchups;
                result.EVIPSums += line.EVIPSums;
                result.EVOOPMatchups += line.EVOOPMatchups;
                result.EVOOPSums += line.EVOOPSums;

                result.EQIPMatchups += line.EQIPMatchups;
                result.EQIPSums += line.EQIPSums;
                result.EQOOPMatchups += line.EQOOPMatchups;
                result.EQOOPSums += line.EQOOPSums;
            }
            return result;
        }
    }

}
