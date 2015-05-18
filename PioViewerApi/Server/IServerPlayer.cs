using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Server
{
    /// <summary>
    /// Server player object
    /// </summary>
    public interface IServerPlayer
    {
        /// <summary>
        /// 0 for OOP player, 1 for IP player
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Name as recognized by the pioSOLVER engine.
        /// OOP for OOP player
        /// IP for IP player
        /// </summary>
        string Name { get; }
    }
}
