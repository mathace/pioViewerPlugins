using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Server
{
    /// <summary>
    /// Simple object to wrap UPI interface command.
    /// </summary>
    public interface IServerCommand
    {
        string Command { get; set; }
        string Arguments { get; set; }
    }
}
