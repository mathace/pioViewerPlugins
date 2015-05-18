using System;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace SimplePlugin
{
    /// <summary>
    /// This is a very simple plugin demonstrating one of the ways to browse the tree from plugins.
    /// When executing this plugin the parent node of currently selected node will be selected.
    /// </summary>
    class SelectParentNodePlugin : IServerPlugin
    {
        public string Name
        {
            get { return "Select Parent Node"; }
        }

        public string Category
        {
            get { return "Tutorial"; }
        }

        public bool IsGUIPlugin
        {
            get { return true; }
        }

        protected IServerPluginContext Context { get; private set; }

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
        }

        public void Execute(IPluginProgressProvider progress)
        {
            //If the user selects plugin from the menu we can ask controller about currently selected node.
            IServerNode currentlySelectedNode = Context.Controller.SelectedNode;
            if (currentlySelectedNode == null)
            {
                MessageBox.Show("There is no node selected at the moment");
                return;
            }
            if (currentlySelectedNode.Parent == null)
            {
                MessageBox.Show("Selected node has no parent");
                return;
            }
            this.Context.Controller.SelectedNode = currentlySelectedNode.Parent;
        }
    }

}
