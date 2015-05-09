using System;
using System.Windows.Forms;
using PioViewerApi.Plugin;
using PioViewerApi.Server;

namespace SimplePlugin
{
    /// <summary>
    /// The very first plugin. Make this class public to enable this plugin in PioViewer
    /// </summary>
    class HelloWorld : IServerPlugin
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
            MessageBox.Show("Hello world plugin has just been initialized");
        }

        public void Execute(IPluginProgressProvider progress)
        {
            MessageBox.Show("Hello world!");
        }
    }

}
