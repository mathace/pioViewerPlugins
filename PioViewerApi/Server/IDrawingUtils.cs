using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PioViewerApi.Util;

namespace PioViewerApi.Server
{
    public interface IDrawingUtils
    {
        /// <summary>
        /// Convert EV in server format into viewer using the specified formula. If formula is not specified then formula 
        /// from user's configuration will be used.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="originalEvRange"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        IRange ConvertSolverEVIntoViewerEV(IServerNode node, IServerPlayer player, IRange originalEvRange, String formula = null);

        /// <summary>
        /// Maps Viewer EV range into 0-1 range for color presentation purposes. If formula is not specified then formula 
        /// from user's configuration will be used.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="convertedEvRange"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        IRange ConvertViewerEVIntoColorIndex(IServerNode node, IServerPlayer player, IRange convertedEvRange, String formula = null);

        /// <summary>
        /// The rescaling function that can be used to covert EV as returned by the server to values displayed by a viewer.
        /// By default the formula specified by the user in his configuration is used, but it's possible to specify other formula.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="player"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        Func<double, double> GetEVConvertedSolverToViewer(IServerNode node, IServerPlayer player, string formula = null);

        /// <summary>
        /// Function to convert Viewer EV into a (0,1) range where 0 would represent the smallest possible value, and 1 the highest possible value for the purpose of assigning colors to EV.
        /// By default the formula specified by the user in his configuration is used, but it's possible to specify other formula.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="player"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        Func<double, double> GetEVConvertedViewerToColorIndex(IServerNode node, IServerPlayer player, string formula = null);

        /// <summary>
        /// Maps 0-1 range to Weights color gradient
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        Color GetColorForWeight(double weight);

        /// <summary>
        /// Maps 0-1 range to EV color gradient
        /// </summary>
        /// <param name="convertedEV"></param>
        /// <returns></returns>
        Color GetColorForEV(double convertedEV);

        /// <summary>
        /// Get's the UI color for a given node. Can be used to obtain a color for action performed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        Color GetColorForNode(IServerNode node);
    }
}
