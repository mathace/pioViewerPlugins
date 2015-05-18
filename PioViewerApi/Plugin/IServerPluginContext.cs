using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Server;

namespace PioViewerApi.Plugin
{
    /// <summary>
    /// Some usuful objects that can be used by the plugins
    /// </summary>
    public interface IServerPluginContext
    {
        /// <summary>
        /// Wrapped UPI interface talking with the solver process connected to the Viewer. Call IsConnected method to see if there is an active server connection.
        /// </summary>
        IServerWrapper ServerWrapper { get; }

        /// <summary>
        /// The object to control behaviour of a PioViewer browser.
        /// </summary>
        IBrowserController Controller { get; }

        /// <summary>
        /// The utilities provider.
        /// </summary>
        IServerUtils ServerUtils { get; }

        /// <summary>
        /// Drawing utilities provider.
        /// </summary>
        IDrawingUtils DrawingUtils { get; }
    }
}
