using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace PioViewerPlugins.Aggregation
{
    public class AggregationRunner
    {
        public IServerNode Node { get; private set; }
        public IServerPluginContext Context { get; private set; }
        public IPluginProgressProvider ProgressProvider { get; private set; }

        public String ActionLine { get; private set; }
        public StringBuilder ReportBuilder { get; private set; }

        private int BoardLen { get; set; }

        /// <summary>
        /// The report goes through all the CFR files listed and aggregates report for all of them.
        /// </summary>
        public List<string> CFRFilePaths { get; set; }

        public AggregationRunner(IServerNode node, IPluginProgressProvider progress, IServerPluginContext context)
        {
            this.Node = node;
            this.Context = context;
            this.ProgressProvider = progress;
            this.ActionLine = GenerateLineString(node);
            this.ReportBuilder = new StringBuilder();
            this.BoardLen = node.Board.Length;
        }

        public void RunReport()
        {
            GenerateReportIntro();

            GenerateReport();
            if (ProgressProvider.CancelRequested)
            {
                return;
            }
            ProgressProvider.UpdateProgress("Writing the report");
            WriteReportToFile();
        }

        /// <summary>
        /// Looks for all the nodes with the same action line, but different board cards.
        /// </summary>
        public List<IServerNode> FindSimilarNodesOnOtherBoards(IServerNode node)
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
                    ProgressProvider.UpdateProgress("listing nodes. Path (" + progressReportingPathIndex + "/" + progressReportingPathTotal + ") " + progressReportingNodeIndex++ + " nodes");
                    if (ProgressProvider.CancelRequested)
                    {
                        return new List<IServerNode>();
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

        /// <summary>
        /// Generate the full report
        /// </summary>
        private void GenerateReport()
        {
            ReportBuilder.AppendLine("");

            ProgressProvider.UpdateProgress("list nodes");
            

            if (CFRFilePaths == null)
            {
                CFRFilePaths = new List<string>();
                CFRFilePaths.Add(null);
            }

            int loop = 0;
            int loops = CFRFilePaths.Count;
            foreach (var path in CFRFilePaths)
            {
                loop++;

                if (path != null)
                {
                    ProgressProvider.UpdateProgress("Load file " + loop + " out of " + loops);
                    Context.ServerWrapper.LoadTree(path);
                    Node = Context.ServerWrapper.RefreshNode(Node);
                }

                var similarNodes = FindSimilarNodesOnOtherBoards(Node);
                int total = similarNodes.Count;
                int progressNodeCount = 0;

                var root = Context.ServerWrapper.ShowRootNode();
                var rootMatchups = Context.ServerWrapper.CalcEquityInNode(Context.ServerUtils.IP, root).TotalMatchups();

                var results = new List<ReportLine>();

                foreach (var singleNode in similarNodes)
                {
                    progressNodeCount++;
                    if (ProgressProvider.CancelRequested)
                    {
                        return;
                    }
                    if (loops > 1)
                    {
                        ProgressProvider.UpdateProgress("file " + loop + " out of " + loops + ". Analyse " + string.Join("", singleNode.Board.ToList()) + " " + progressNodeCount + " out of " + total);
                    }
                    else
                    {
                        ProgressProvider.UpdateProgress("Analyze " + string.Join("", singleNode.Board.ToList()) + " " + progressNodeCount + " out of " + total);
                    }
                    results.Add(ReportLine.CreateReportLineForNode(singleNode, this.Context));
                }
                var summaryResult = ReportLine.ToSummaryCSVLine(results);

                if (loop == 1)
                {
                    ReportBuilder.AppendLine(string.Join(",", GenerateHeaderNames(summaryResult.ActionNames)));
                }

                if (results.Count > 1)
                {
                    ReportBuilder.AppendLine(string.Join(",", summaryResult.ToCSVLine(rootMatchups)));
                }

                foreach (var resultLine in results)
                {
                    ReportBuilder.AppendLine(string.Join(",", resultLine.ToCSVLine(rootMatchups)));
                }
            }
        }

        private List<String> GenerateHeaderNames(List<string> actionNames)
        {
            var result = new List<String>();
            var streets = new String[] { "Flop", "Turn", "River" };
            for (int i = 3; i <= BoardLen; i++)
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

        private void WriteReportToFile()
        {
            if (!Directory.Exists("Reports"))
            {
                Directory.CreateDirectory("Reports");
            }
            var path = Path.Combine("Reports", "AggregatedReport_" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".csv");
            File.WriteAllText(path, ReportBuilder.ToString());
        }

        private void GenerateReportIntro()
        {
            var fileName = Context.Controller.CurrentFileName;
            ReportBuilder.AppendLine("Aggregation analysis");
            ReportBuilder.AppendLine("File, " + fileName);
            ReportBuilder.AppendLine("Action Line," + ActionLine);
        }

        private string GenerateLineString(IServerNode node)
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