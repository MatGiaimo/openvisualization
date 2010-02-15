using System;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Xml.Linq;

namespace ChartConfig
{
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

    public class ChartConfigProvider
    {
        #region Private members
        private XDocument xDocChartDefinition;
        private ArrayList series;
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

            xDocChartDefinition = XDocument.Load(xmlText);

            series = new ArrayList();


            ParseChartsXDoc();
        }

        public ChartConfigProvider(XmlDocument chartDefinition)
        {
            xDocChartDefinition = ChartConfigProvider.DocumentToXDocumentReader(chartDefinition);
            series = new ArrayList();

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
                
                //Not sure to do when these properties are not found so
                //just swallow exceptions for now
                // Type (Line, Column, Bar, Etc)
                try
                {
                    SeriesChartType seriesType = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), xSeries.Element("Type").Value.ToString());
                    currSeries.ChartType = seriesType;
                }
                catch (Exception ex)
                {
                    //throw new Exception("Unrecognized chart type", ex);
                }

                // Color
                try
                {
                    currSeries.Color = System.Drawing.Color.FromName(xSeries.Element("Color").Value.ToString());
                }
                catch (Exception ex)
                {
                    
                }

                // Axis Label
                try
                {
                    currSeries.AxisLabel = xSeries.Element("AxisLabel").Value.ToString();
                }
                catch (Exception ex)
                {
                    
                }

                //XValueType
                try
                {
                    currSeries.XValueType = (ChartValueType)Enum.Parse(typeof(ChartValueType), xSeries.Element("XValueType").Value.ToString());
                }
                catch (Exception ex)
                {
                    
                }

                //IsXAxisLabel
                try
                {
                    currSeries.IsXAxisLabel = bool.Parse(xSeries.Element("IsXAxisLabel").Value.ToString());
                }
                catch (Exception ex)
                {
                }

                series.Add(currSeries);
            }
        }

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
            }
        }

        #endregion

        #region Static Helper Methods
        private static XDocument DocumentToXDocumentReader(XmlDocument doc)
        {
            return XDocument.Load(new XmlNodeReader(doc));
        }
        #endregion
    }
}