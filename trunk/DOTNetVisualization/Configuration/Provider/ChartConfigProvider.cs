using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Web.UI.DataVisualization.Charting;

namespace ChartConfig
{
    public enum DataSourceType
    {
        Uri
    }

    /// <summary>
    /// Not used yet.  Inteded to expand the series class to include an XPath for the Data Interface to use
    /// </summary>
    public class ChartConfigSeries : System.Web.UI.DataVisualization.Charting.Series
    {
        #region Private members
        private string xPath;
        #endregion

        #region Properties
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
        #endregion
    }

    public class ChartConfigProvider
    {
        #region Private members
        private XmlDocument xmlChartDefinition;
        private ArrayList series;
        private ArrayList seriesXPath;
        private DataSourceType dataSource;
        private string uri;
        #endregion

        #region Properties
        public ArrayList Series
        {
            get
            {
                return series;
            }
        }

        public ArrayList SeriesXPath
        {
            get
            {
                return seriesXPath;
            }
        }

        public string URI
        {
            get
            {
                return uri;
            }
        }

        public DataSourceType DataSource
        {
            get 
            { 
                return dataSource; 
            }
        }
        #endregion

        #region Constructors
        public ChartConfigProvider()
        {

        }

        public ChartConfigProvider(string chartDefinition)
        {
            XmlTextReader xmlText = new XmlTextReader(chartDefinition);
            xmlChartDefinition.Load(xmlText);
            seriesXPath = new ArrayList();
            series = new ArrayList();


            ParseCharts();
        }

        public ChartConfigProvider(XmlDocument chartDefinition)
        {
            xmlChartDefinition = chartDefinition;
            seriesXPath = new ArrayList();
            series = new ArrayList();

            ParseCharts();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method reads series definitions from the xml config and builds a SeriesCollection which can then be used to build a chart.
        /// </summary>
        private void BuildSeries()
        {
            //foreach(SeriesType series in xmlChartDefinition.Chart.Data.Items[0])
            //{

            //}
        }

        private void ParseCharts()
        {
            XmlNodeList charts = xmlChartDefinition.SelectNodes("/root/Chart");

            foreach (XmlNode chartNode in charts)
            {
                // We're starting with Uri DataSourceTypes so lets check for that first
                // We only support one uri config (for now) so use SelectSingleNode
                StringBuilder dataURI = new StringBuilder();
                XmlNode uriNode = chartNode.SelectSingleNode("/root/Chart/Uri");

                if (uriNode != null)
                    dataSource = DataSourceType.Uri;

                //Check for Path
                string uriPath = uriNode["Path"].InnerText;

                if (string.IsNullOrEmpty(uriPath))
                    throw new Exception("Uri path not found");

                dataURI.Append(uriPath);

                XmlNodeList uriParams = uriNode.SelectNodes("/root/Chart/Uri/Param");

                // Get params
                for (int lp = 1; lp < uriParams.Count; lp++)
                {
                    if (lp == 1)
                        dataURI.Append("?");
                    else
                        dataURI.Append("&");

                    // In this case the desired parameters are hard coded into the XML.
                    // in a 'real' server you'd likely accept them as params to this page
                    dataURI.Append(uriParams.Item(lp).Attributes.Item(0).Value.ToString());
                    dataURI.Append("=");
                    dataURI.Append(uriParams.Item(lp).InnerText);
                }

                uri = dataURI.ToString();
     
                //Get the series definitions
                XmlNodeList seriesList = chartNode.SelectNodes("/root/Chart/Data/SeriesDefinitions/Series");

                foreach (XmlNode seriesNode in seriesList)
                {
                    Series currSeries = new Series();

                    // Set the Name (id in current xml definition)
                    currSeries.Name = seriesNode.Attributes["id"].Value.ToString();

                    XmlNode xPathNode = seriesNode.SelectSingleNode(string.Format("/root/Chart/Data/SeriesDefinitions/Series[@id='{0}']/Data",currSeries.Name));

                    seriesXPath.Add(xPathNode.InnerText);

                    XmlNode chartTypeNode = seriesNode.SelectSingleNode(string.Format("/root/Chart/Data/SeriesDefinitions/Series[@id='{0}']/Type",currSeries.Name));

                    try
                    {
                        SeriesChartType seriesType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType),chartTypeNode.InnerText);

                        currSeries.ChartType = seriesType;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unrecognized chart type", ex);
                    }

                    series.Add(currSeries);
                }
            }
        }

        #endregion



    }
}