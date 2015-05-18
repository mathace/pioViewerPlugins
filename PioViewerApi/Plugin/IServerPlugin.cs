using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Server;

namespace PioViewerApi.Plugin
{
    /// <summary>
    /// Interface to extend if you want to plug some functionality into PioViewer
    /// </summary>
    public interface IServerPlugin
    {
        /// <summary>
        /// The name under which plugin will be visible in the menu
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The plugin will apear in Plugins menu under submenu with this name
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Plugin should return true if this plugin creates or interacts with GUI objects.
        /// GUI plugins don't have access to progress bar and block the UI until the execute method completes.
        /// </summary>
        bool IsGUIPlugin { get; }

        /// <summary>
        /// Called when the pioViewer is starting.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        void Initialize(IServerPluginContext context);

        /// <summary>
        /// Perform the plugin's logic
        /// </summary>
        void Execute(IPluginProgressProvider updatesProvider);
    }

    /// <summary>
    /// Control the progress of a plugin execution
    /// </summary>
    public interface IPluginProgressProvider
    {
        /// <summary>
        /// Send back information to the caller about the progress.
        /// If there are three chunks report UpdateProgress(0,3) at the beginning.
        /// if one third of the progress was done report UpdateProgress(1,3);
        /// </summary>
        /// <param name="done"></param>
        /// <param name="total"></param>
        void UpdateProgress(int done, int total);

        /// <summary>
        /// Update progress by reporting what plugin is currently working on.
        /// </summary>
        /// <param name="message"></param>
        void UpdateProgress(string message);

        /// <summary>
        /// Check this property periodically. If it is true - then the caller requested plugin operation to cancel.
        /// </summary>
        bool CancelRequested { get; }
    }
}
