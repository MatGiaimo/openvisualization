using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Xml.Linq;
using System.Reflection;

namespace OpenVisualization.Configuration
{
    /// <summary>
    /// Enumeration of supported data source types
    /// </summary>
    public enum DataSourceType
    {
        Uri
    }

    /// <summary>
    /// ChartConfigSeries builds upon the Series class to add extra properties used in configuration.
    /// </summary>
    public class ChartConfigSeries : System.Web.UI.DataVisualization.Charting.Series
    {
        #region Private members
        private string xPath;
        private bool isXAxisLabel;
        #endregion

        #region Properties

        /// <summary>
        /// An XPath expression used to select data from the XML data
        /// </summary>
        public string XPath
        {
            get
            {
                return xPath;
            }
            set
            {
                xPath = value;
            }
        }

        /// <summary>
        /// True if the Series is used as the xAxis label
        /// </summary>
        public bool IsXAxisLabel
        {
            get
            {
                return isXAxisLabel;
            }
            set
            {
                isXAxisLabel = value;
            }
        }
        #endregion
    }

    /// <summary>
    /// This class reads an XML configuration file that is used to build a chart
    /// </summary>
    public class ChartConfigProvider
    {
        #region Private members
        private XDocument xDocChartDefinition;
        private ArrayList series;
        private DataSourceType dataSource;
        private string uri;
        private Hashtable chartParams;
        private Hashtable chartAreaParams;
        private Hashtable chartAxisXParams;
        private Hashtable chartAxisYParams;
        private Hashtable chartLegendParams;
        
        #endregion

        #region Properties

        /// <summary>
        /// Holds an array of ChartConfigSeries
        /// </summary>
        public ArrayList Series
        {
            get
            {
                return series;
            }
        }

        /// <summary>
        /// The service URI used to retrieve data values
        /// </summary>
        public string URI
        {
            get
            {
                return uri;
            }
        }

        /// <summary>
        /// The type of the data source
        /// </summary>
        public DataSourceType DataSource
        {
            get 
            { 
                return dataSource; 
            }
        }

        public Hashtable ChartParams
        {
            get
            {
                return chartParams;
            }
        }

        public Hashtable ChartAreaParams
        {
            get
            {
                return chartAreaParams;
            }
        }

        public Hashtable ChartAxisXParams
        {
            get
            {
                return chartAxisXParams;
            }
        }

        public Hashtable ChartAxisYParams
        {
            get
            {
                return chartAxisYParams;
            }
        }

        public Hashtable ChartLegendParams
        {
            get
            {
                return chartLegendParams;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartConfigProvider()
        {

        }

        /// <summary>
        /// Overloaded constructor that builds a configuration from an XML string
        /// </summary>
        /// <param name="chartDefinition">XML string</param>
        public ChartConfigProvider(string chartDefinition)
        {
            // Make sure string is uri encoded
            XmlDocument xChartDefinition = new XmlDocument();
            xChartDefinition.LoadXml(chartDefinition);

            xDocChartDefinition = ChartConfigProvider.DocumentToXDocumentReader(xChartDefinition);

            series = new ArrayList();
            chartParams = new Hashtable();
            chartAreaParams = new Hashtable();
            chartAxisXParams = new Hashtable();
            chartAxisYParams = new Hashtable();
            chartLegendParams = new Hashtable();

            ParseChartsXDoc();
        }

        /// <summary>
        /// Overloaded constructor that builds a configuration from an XmlDocument
        /// </summary>
        /// <param name="chartDefinition">XmlDocument configuration</param>
        public ChartConfigProvider(XmlDocument chartDefinition)
        {
            xDocChartDefinition = ChartConfigProvider.DocumentToXDocumentReader(chartDefinition);
            series = new ArrayList();
            chartParams = new Hashtable();
            chartAreaParams = new Hashtable();
            chartAxisXParams = new Hashtable();
            chartAxisYParams = new Hashtable();
            chartLegendParams = new Hashtable();

            ParseChartsXDoc();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method reads series definitions from the xml config and builds a SeriesCollection which can then be used to build a chart.
        /// </summary>
        private void BuildSeries(IEnumerable<XElement> xSeriesList)
        {
            foreach (XElement xSeries in xSeriesList)
            {
                ChartConfigSeries currSeries = new ChartConfigSeries();

                // Set the Name (id in current xml definition)
                currSeries.Name = xSeries.Attribute("id").Value.ToString();

                // Set the xpath to get data
                currSeries.XPath = xSeries.Element("Data").Value.ToString();

                // Loop through all Param values in the nodelist for the series
                Object obj = currSeries;
                FillObjectParameters(ref obj, xSeries.Elements("Param"));

                currSeries = (ChartConfigSeries)obj; // Possible redundant code
                series.Add(currSeries);
            }
        }

        /// <summary>
        /// Parses Chart definitions inside the XML configuration
        /// </summary>
        private void ParseChartsXDoc()
        {
            IEnumerable<XElement> xChartList = xDocChartDefinition.Root.Elements("Chart");

            foreach (XElement el in xChartList)
            {
                // We're starting with Uri DataSourceTypes so lets check for that first
                // We only support one uri config (for now) so use SelectSingleNode
                StringBuilder dataURI = new StringBuilder();

                XElement xUri = el.Element("Uri");

                try
                {
                    dataSource = DataSourceType.Uri;

                    string uriPath = xUri.Element("Path").Value.ToString();

                    dataURI.Append(uriPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("Uri path not found",ex);
                }
                             
                IEnumerable<XElement> xParamList = xUri.Elements("Param");

                int paramCount = 1;
                foreach (XElement xParam in xParamList)
                {
                    if (paramCount == 1)
                        dataURI.Append("?");
                    else
                        dataURI.Append("&");

                    // In this case the desired parameters are hard coded into the XML.
                    // in a 'real' server you'd likely accept them as params to this page
                    dataURI.Append(xParam.Attribute("Name").Value.ToString());
                    dataURI.Append("=");
                    dataURI.Append(xParam.Value.ToString());
                    paramCount++;
                }

                // Set the uri variable for access through property
                uri = dataURI.ToString();

                //Get the series definitions
                BuildSeries(el.Descendants("Data").Descendants("SeriesDefinitions").Descendants("Series"));

                // Process ChartArea Parameters
                XElement xChartArea = el.Element("ChartArea");

                if (xChartArea != null)
                {
                    FillConfigParameters(chartAreaParams, xChartArea.Elements("Param"));

                    // Process Axis Params
                    FillConfigParameters(chartAxisXParams, xChartArea.Element("AxisX").Elements("Param"));
                    FillConfigParameters(chartAxisYParams, xChartArea.Element("AxisY").Elements("Param"));

                }

                // Process Legend Params 
                XElement xLegend = el.Element("Legend");
                if (xLegend != null)
                    FillConfigParameters(chartLegendParams, xLegend.Elements("Param"));

                // Process Chart Params
                FillConfigParameters(chartParams, el.Elements("Param"));
            }
        }

        private void FillConfigParameters(Hashtable ht, IEnumerable<XElement> paramElements)
        {
            // Loop through all Param values in the nodelist for the series
            foreach (XElement xe in paramElements)
            {
                string name = xe.Attribute("name").Value.ToString();
                string val = xe.Value.ToString();

                ht.Add(name, val);
            }
        }

        private void FillObjectParameters(ref Object chartObject, IEnumerable<XElement> paramElements)
        {
            // Loop through all Param values in the nodelist for the series
            foreach (XElement xe in paramElements)
            {
                string name = xe.Attribute("name").Value.ToString();
                string val = xe.Value.ToString();

                PropertyInfo pi = chartObject.GetType().GetProperty(name);

                // Evaluate special cases: Enum, Color, etc. Else do basic conversion
                try
                {
                    Object o;

                    if (pi.PropertyType.BaseType.FullName == "System.Enum")
                    {
                        o = Enum.Parse(pi.PropertyType, val);
                    }
                    else if (pi.PropertyType.FullName == "System.Drawing.Color")
                    {
                        o = System.Drawing.Color.FromName(val);
                    }
                    else
                    {
                        o = Convert.ChangeType(val, pi.PropertyType);
                    }

                    pi.SetValue(chartObject, o, null);
                }
                catch (Exception e)
                // DO NOTHING ... Yet
                {

                }
            }
        }
        #endregion

        #region Static Helper Methods
        /// <summary>
        /// Helper method to convert an XmlDocument to a LINQ XDocument
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static XDocument DocumentToXDocumentReader(XmlDocument doc)
        {
            return XDocument.Load(new XmlNodeReader(doc));
        }
        #endregion
    }
}