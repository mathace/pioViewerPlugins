using System;
using System.IO;
using System.Text;
using PioViewerApi.Plugin;
using PioViewerApi.Server;
using PioViewerApi.Util;
using System.Collections.Generic;
using System.Linq;

namespace PioViewerPlugins.Aggregation
{
    public class AggregatedAnalysis : IServerPlugin
    {
        #region Identification
        public string Name
        {
            get { return "Runouts aggregated frequencies analysis"; }
        }

        public string Category
        {
            get { return "Aggregation"; }
        }

        public bool IsGUIPlugin
        {
            get { return false; }
        }
        #endregion

        #region plugin initialization
        protected IServerPluginContext Context { get; private set; }

        protected IServerPlayer OOP, IP;

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
            this.OOP = context.ServerUtils.OOP;
            this.IP = context.ServerUtils.IP;
        }
        #endregion

        public void Execute(IPluginProgressProvider progress)
        {
            var node = Context.Controller.SelectedNode;
            if (node == null)
            {
                throw new InvalidOperationException("The aggregated analysis can only be run when action node is selected");
            }
            var report = new StringBuilder();

            progress.UpdateProgress("Started");

            var root = Context.ServerWrapper.ShowRootNode();
            var rootMatchups = Context.ServerWrapper.CalcEquityInNode(IP, root).TotalMatchups();

            string line = GenerateLineString(node);
            var fileName = Context.Controller.CurrentFileName;
            progress.UpdateProgress("Generating intro");
            GenerateReportIntro(node, report, line, fileName);

            GenerateReport(node, report, rootMatchups, progress);
            if (progress.CancelRequested)
            {
                return;
            }
            progress.UpdateProgress("Writing the report");
            WriteReportToFile(node, report, line);
        }

        /// <summary>
        /// Looks for all the nodes with the same action line, but different board cards.
        /// </summary>
        private List<IServerNode> FindSimilarNodesOnOtherBoards(IServerNode node, IPluginProgressProvider progress)
        {
            Stack<IServerNode> fullPath = new Stack<IServerNode>();
            for (IServerNode temp = node; temp.Parent != null; temp = temp.Parent)
            {
                fullPath.Push(temp);
            }

            List<IServerNode> allSimilar = new List<IServerNode>();
            allSimilar.Add(fullPath.Pop());
            List<IServerNode> tempList = new List<IServerNode>();

            int progressReportingPathIndex = 0;
            int progressReportingPathTotal = fullPath.Count;

            while (fullPath.Count > 0)
            {
                var pattern = fullPath.Pop();
                
                int progressReportingNodeIndex = 1;
                progressReportingPathIndex++;

                foreach (var item in allSimilar)
                {
                    progress.UpdateProgress("listing nodes. Path (" + progressReportingPathIndex + "/" + progressReportingPathTotal + ") " + progressReportingNodeIndex++ + " nodes");
                    if (progress.CancelRequested)
                    {
                        return allSimilar;
                    }

                    var itemChildren = Context.ServerWrapper.ShowChildren(item);
                    foreach (var child in itemChildren)
                    {
                        if (child.NodeText == pattern.NodeText || item.NodeType == "SPLIT_NODE")
                        {
                            tempList.Add(child);
                        }
                    }
                }
                allSimilar = tempList;
                tempList = new List<IServerNode>();
            }
            return allSimilar;
        }

        private class ReportLine
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

            public ReportLine()
            {
                ActionFrequencies = new Dictionary<string, double>();
                ActionNames = new List<string>();
            }
        }

        /// <summary>
        /// Get the results from one particular node into the structure
        /// </summary>
        private ReportLine CreateReportLineForNode(IServerNode node)
        {
            var result = new ReportLine();

            //the board
            result.Board = node.Board;

            // Calculate IP and OOP ev and equity in the node.
            // The result for EV calculation is two ranges
            // one range is sums and the other is matchups
            // EV for a given hand can be calculated by dividing
            // sums by matchups.
            // The total EV in node can be obtained by diving
            // total sums in the node by total counters
            var evIP = Context.ServerWrapper.CalcEV(IP, node);
            var evOOP = Context.ServerWrapper.CalcEV(OOP, node);

            result.EVOOPSums = evOOP.TotalWins();
            result.EVOOPMatchups = evOOP.TotalMatchups();
            result.EVIPSums = evIP.TotalWins();
            result.EVIPMatchups = evIP.TotalMatchups();

            var eqIP = Context.ServerWrapper.CalcEquityInNode(IP, node);
            var eqOOP = Context.ServerWrapper.CalcEquityInNode(OOP, node);

            result.EQOOPSums = eqOOP.TotalWins();
            result.EQOOPMatchups = eqOOP.TotalMatchups();
            result.EQIPSums = eqIP.TotalWins();
            result.EQIPMatchups = eqIP.TotalMatchups();

            // If the node is action node (e.g. - not a street split node nor final node)
            // then we also compute how often given action is selected
            if (node.ActionPlayer != null)
            {
                // we ask server to list all children of a current node
                var actions = Context.ServerWrapper.ShowChildren(node);
                foreach (var action in actions)
                {
                    // for each child we ask for the range of an action player
                    var rangeForAction = Context.ServerWrapper.ShowRange(node.ActionPlayer, action);
                    // and store it in the result dictionary
                    // NodeText property denotes the action (e.g. CHECK or BET 100)
                    result.ActionFrequencies[action.NodeText] = rangeForAction.TotalWeights();
                    // Node name is stored separately to preserve the action's order
                    result.ActionNames.Add(action.NodeText);
                }
            }
            return result;
        }

        /// <summary>
        /// Sum the results in a single structure
        /// </summary>
        /// <param name="fullReport"></param>
        /// <returns></returns>
        private ReportLine GenerateSummaryReportLine(List<ReportLine> fullReport)
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


        private List<String> GenerateHeaderNames(List<string> actionNames, int boardLen)
        {
            var result = new List<String>();
            var streets = new String[] { "Flop", "Turn", "River" };
            for (int i = 3; i <= boardLen; i++)
            {
                result.Add(streets[i - 3]);
            }

            result.Add("Global %");
            result.Add("OOP Equity");
            result.Add("IP Equity");
            result.Add("OOP EV");
            result.Add("IP EV");
            foreach (var actionName in actionNames)
            {
                result.Add(actionName);
            }
            return result;
        }

        /// <summary>
        /// Generates a list of calvulated values for the columns in a single line of a report
        /// </summary>
        private List<String> GenerateReportLine(ReportLine reportLine, double rootMatchups)
        {
            var result = new List<String>();

            AddBoard(result, reportLine.Board);
            result.Add((reportLine.EQOOPMatchups * 100 / rootMatchups).ToString("G6"));
            result.Add((reportLine.EQOOPSums / reportLine.EQOOPMatchups).ToString("G6"));
            result.Add((reportLine.EQIPSums / reportLine.EQIPMatchups).ToString("G6"));
            result.Add((reportLine.EVOOPSums / reportLine.EVOOPMatchups).ToString("G6"));
            result.Add((reportLine.EVIPSums / reportLine.EVIPMatchups).ToString("G6"));
            double[] frequencies = new double[reportLine.ActionNames.Count];
            for (int i = 0; i < frequencies.Length; i++)
            {
                var actionName = reportLine.ActionNames[i];
                frequencies[i] = reportLine.ActionFrequencies[actionName];
            }
            double sum = frequencies.Sum();
            foreach (var f in frequencies)
            {
                result.Add((f * 100 / sum).ToString("G3"));
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
        /// Generate the full report
        /// </summary>
        private void GenerateReport(IServerNode nodeForReport, StringBuilder report, double rootMatchups, IPluginProgressProvider progress)
        {
            report.AppendLine("");

            progress.UpdateProgress("list nodes");
            var similarNodes = FindSimilarNodesOnOtherBoards(nodeForReport, progress);
            var results = new List<ReportLine>();

            int total = similarNodes.Count;
            int progressNodeCount = 0;
            foreach (var singleNode in similarNodes)
            {
                progressNodeCount++;
                if (progress.CancelRequested)
                {
                    return;
                }
                progress.UpdateProgress("Analyze " + string.Join("", singleNode.Board.ToList()) + " " + progressNodeCount + " out of " + total);
                results.Add(CreateReportLineForNode(singleNode));
            }
            var summaryResult = GenerateSummaryReportLine(results);
            report.AppendLine(string.Join(",", GenerateHeaderNames(summaryResult.ActionNames, nodeForReport.Board.Length)));
            report.AppendLine(string.Join(",", GenerateReportLine(summaryResult, rootMatchups)));
            foreach (var resultLine in results)
            {
                report.AppendLine(string.Join(",", GenerateReportLine(resultLine, rootMatchups)));
            }
        }

        private static void WriteReportToFile(IServerNode node, StringBuilder report, string line)
        {
            if (!Directory.Exists("Reports"))
            {
                Directory.CreateDirectory("Reports");
            }
            var path = Path.Combine("Reports", "AggregatedReport_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".csv");
            File.WriteAllText(path, report.ToString());
        }

        private static void GenerateReportIntro(IServerNode node, StringBuilder report, string line, string path)
        {
            report.AppendLine("Aggregation analysis");
            report.AppendLine("File, " + path);
            report.AppendLine("Action Line," + line);
        }

        private static string GenerateLineString(IServerNode node)
        {
            string line = node.NodeText;
            var parent = node.Parent;
            while (parent.Parent != null)
            {
                line = parent.NodeText + " " + line;
                parent = parent.Parent;
            }
            return line;
        }
    }
}
