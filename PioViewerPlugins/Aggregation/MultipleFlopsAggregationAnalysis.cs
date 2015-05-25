using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace PioViewerPlugins.Aggregation
{
    public class MultipleFlopsAggregationAnalysis : IServerPlugin
    {
        #region Identification
        public string Name
        {
            get { return "Multiple Files runouts aggregated frequencies analysis"; }
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
            progress.UpdateProgress("Started");

            AggregationRunner result = new AggregationRunner(node, progress, this.Context);
            result.CFRFilePaths = new List<string>();

            if (Context.Controller.CurrentFileName == null)
            {
                throw new InvalidOperationException("Unknown location of current tree.");
            }
            var dirName = Path.GetDirectoryName(Context.Controller.CurrentFileName);

            var files = Directory.GetFiles(dirName, "*.cfr");

            var msg = "Perform analysis over " + files.Length + " files in " + dirName + "?" +Environment.NewLine + "Please not that it only makes sense if all trees are identical except for the board.";
            var dialogResult = MessageBox.Show(msg, "Confirm aggregation?", MessageBoxButtons.YesNoCancel);

            if (dialogResult == DialogResult.Yes)
            {
                result.CFRFilePaths.AddRange(files);
                result.RunReport();

                var fileName = Context.Controller.CurrentFileName;
                progress.UpdateProgress("Generating intro");
            }
        }
    }
}
