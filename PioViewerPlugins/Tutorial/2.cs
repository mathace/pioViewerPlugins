using System;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace SimplePlugin
{
    /// <summary>
    /// This is a very simple plugin showing how to execute code 
    /// when the node has changed and how to get information 
    /// about currently selected node from PioViewer
    /// </summary>
    class HelloWorldWithEventHandling : IServerPlugin
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
            //Context is a container for many useful objects and operations including the server access,
            //cotrolling the viewer and many others.
            this.Context = context;

            // In this plugin we are interested in functionality of a PioViewer browser controller.
            IBrowserController controller = context.Controller;

            // This method is the C# way of registering to the event
            // In this case we register to an event, that is triggered on node selection change in the viewer
            controller.SelectedNodeChanged += Controller_SelectedNodeChanged;
        }

        /// <summary>
        /// Method that will be executed when the node is selected.
        /// </summary>
        /// <param name="node">Selected node</param>
        void Controller_SelectedNodeChanged(IServerNode node)
        {
            //This will open a new window with information about what node is currently selected.
            //node.NodeText is a way of getting the label of a node e.g. RAISE 350
            MessageBox.Show(node.NodeText + " has been selected ");
        }

        public void Execute(IPluginProgressProvider progress)
        {
            //If the user selects plugin from the menu we can ask controller about currently selected node.
            IServerNode currentlySelectedNode = Context.Controller.SelectedNode;
            //If there is no node selected - the property will be null.
            if (currentlySelectedNode == null)
            {
                MessageBox.Show("There is no node selected at the moment");
            }
            else
            {
                MessageBox.Show(currentlySelectedNode.NodeText + " is currently selected");
            }
        }
    }

}
