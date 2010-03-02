using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace OpenVisualization.Data
{
    /// <summary>
    /// Interface for ChartDataReader
    /// </summary>
    interface IChartDataReader
    {
        /// <summary>
        /// Used to get XML from a data source and return it in the standard format
        /// </summary>
        /// <returns></returns>
        XmlDocument getXML();

    }
}
