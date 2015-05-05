using System;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace SimplePlugin
{
    public class HelloWorld : IServerPlugin
    {

        public string Name
        {
            get { return "Hello world!"; }
        }

        public string Category
        {
            get { return "Tutorial"; }
        }

        protected IServerPluginContext Context { get; private set; }

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
            //MessageBox.Show("Hello world - I have just been initialized");
            context.Controller.SelectedNodeChanged += Controller_SelectedNodeChanged;
        }

        void Controller_SelectedNodeChanged(IServerNode node)
        {
            Console.WriteLine(node.NodeText + " has been selected ");
        }

        public void Execute()
        {
            if (Context.ServerWrapper == null || Context.Controller.SelectedNode == null)
            {
                MessageBox.Show("No server or no node selected");
            }
            var r = Context.ServerWrapper.ShowRange(Context.ServerUtils.OOP, Context.Controller.SelectedNode);

            MessageBox.Show("Current OOP range has " + r.TotalWeights() + " total combos");
        }
    }

}
