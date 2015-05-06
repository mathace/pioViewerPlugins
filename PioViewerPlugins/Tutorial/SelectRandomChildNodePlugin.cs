using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace SimplePlugin
{
    /// <summary>
    /// This is an example plugin that shows how to browse a tree.
    /// When plugin is started it creates a new Form with a button and returns the control to the Browser.
    /// The form is then visible and can interoperate with the browser, while user can keep normally using
    /// the viewer.
    /// 
    /// When the button is clicked the plugin selects randomly one of the children of the node it previously selected.
    /// </summary>
    class SelectRandomChildNodePlugin : IServerPlugin
    {

        public string Name
        {
            get { return "Select random children!"; }
        }

        public string Category
        {
            get { return "Tutorial"; }
        }

        protected IServerPluginContext Context { get; private set; }

        public void Initialize(IServerPluginContext context)
        {
            this.Context = context;
        }

        public void Execute()
        {
            new TreeBrowsingForm(Context).Show();
        }


        class TreeBrowsingForm : Form
        {
            IServerNode CurrentNode { get; set; }

            IServerPluginContext Context { get; set; }

            private void InitializeComponent()
            {
                Button b = new Button();
                b.Text = "Select random child";
                b.Location = new System.Drawing.Point(10, 10);
                b.AutoSize = true;
                b.Click += b_Click;

                this.Size = new System.Drawing.Size(500, 500);
                this.Controls.Add(b);
            }

            public TreeBrowsingForm(IServerPluginContext context)
            {
                InitializeComponent();
                this.Context = context;
                this.CurrentNode = context.ServerWrapper.ShowRootNode();
                this.Text = CurrentNode.NodeId;
            }

            void b_Click(object sender, EventArgs e)
            {
                var children = Context.ServerWrapper.ShowChildren(CurrentNode).ToList();

                if (children.Count == 0)
                {
                    MessageBox.Show("Node " + CurrentNode.NodeId + " has no children");
                }
                else
                {
                    var randomChild = children[new Random().Next(children.Count)];
                    this.CurrentNode = randomChild;
                    this.Context.Controller.SelectedNode = randomChild;
                    this.Text = randomChild.NodeId;
                }
            }
        }
    }

}
