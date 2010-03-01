using System.Collections;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Xml.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System;

namespace Charting
{
    using ChartConfig;

    /// <summary>
    /// ChartBuilder interacts with the data source and the chart configuration to provide
    /// a Chart object.
    /// </summary>
    public class ChartBuilder
    {
        private ChartConfigProvider currConfig;
        private Chart chartToBuild;

        //XAxis labeling
        string xAxisLabelSeriesName = string.Empty;
        ArrayList xAxisLabelValues;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ChartBuilder()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ChartConfig">A populated ChartConfig object.</param>
        public ChartBuilder(ChartConfigProvider ChartConfig, Chart ChartToBuild)
        {
            currConfig = ChartConfig;
            chartToBuild = ChartToBuild;

            XmlDocument xmlData = GetXmlData();

            BuildXAxisLabels(xmlData);

            FillSeriesData(xmlData);
        }

        private void BuildXAxisLabels(XmlDocument xmlData)
        {
            xAxisLabelValues = new ArrayList();

            //Find the name of the series with the xaxis label
            //Get the xAxisLabel values
            foreach (ChartConfigSeries ccSeries in currConfig.Series)
            {
                if (ccSeries.IsXAxisLabel == true)
                {
                    xAxisLabelSeriesName = ccSeries.Name;

                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    foreach (XmlNode nd in data)
                    {
                        xAxisLabelValues.Add(DateTime.Parse(nd.InnerText));
                    }
                }
            }
        }

        private void FillSeriesData(XmlDocument xmlData)
        {
            foreach (ChartConfigSeries ccSeries in currConfig.Series)
            {
                //Ignore the xAxisLabelSeries when populating y axis data
                if (ccSeries.Name != xAxisLabelSeriesName)
                {
                    
                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    int xAxisIndex = 0;
                    foreach (XmlNode nd in data)
                    {
                        if (xAxisLabelValues.Count > 0)
                        {
                            ccSeries.Points.AddXY(xAxisLabelValues[xAxisIndex], nd.InnerText);

                        }
                        else
                        {
                            ccSeries.Points.AddXY(xAxisIndex, nd.InnerText);
                        }
                        xAxisIndex++;
                    }
                    xAxisIndex = 0;

                    chartToBuild.Series.Add((Series)ccSeries);
                }
            }
        }

        private XmlDocument GetXmlData()
        {
            // Now that we have the URI, we can call it and get the XML
            Uri uri = new Uri(currConfig.URI);
            WebClient webClient = new WebClient();
            Stream phData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(phData);
            XmlDocument xmlData = new XmlDocument();
            xmlData.Load(xmlText);

            return xmlData;
        }
    }
}