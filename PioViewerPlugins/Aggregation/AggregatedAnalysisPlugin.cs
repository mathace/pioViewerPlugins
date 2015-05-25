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

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
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
            result.RunReport();
            
            var fileName = Context.Controller.CurrentFileName;
            progress.UpdateProgress("Generating intro");
        }
    }
}
