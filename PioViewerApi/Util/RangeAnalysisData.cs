using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioViewerApi.Util
{
    public class RangeAnalysisData
    {
        /// <summary>
        /// Names of the categories
        /// </summary>
        public string[][] CategoriesNames { get; set; }
        /// <summary>
        /// index of the category for a hand in CategoryNames array
        /// </summary>
        public Dictionary<Hand, int>[] CategoriesDistribution { get; set; }
    }
}
