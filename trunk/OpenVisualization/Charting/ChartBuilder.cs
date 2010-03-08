﻿using System.Collections;
using System.Collections.Generic;
using System.Web.UI.DataVisualization.Charting;
using System.Xml.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System;
using System.Reflection;

using OpenVisualization.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace OpenVisualization.Charting
{
    /// <summary>
    /// ChartBuilder interacts with the data source and the chart configuration to provide
    /// a Chart object.
    /// </summary>
    public class ChartBuilder
    {
        /// <summary>
        /// The current working configuration for the chart
        /// </summary>
        private ChartConfigProvider currConfig;

        /// <summary>
        /// The chart object used to provide the chart
        /// </summary>
        private Chart chartToBuild;

        /// <summary>
        /// Label for the xAxis seris
        /// </summary>
        string xAxisLabelSeriesName = string.Empty;

        /// <summary>
        /// Array of xAxis values
        /// </summary>
        ArrayList xAxisLabelValues;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ChartBuilder()
        {
        }


        /// <summary>
        /// This overloaded constructor creates a chart from the provided ChartConfigProvider
        /// </summary>
        /// <param name="ChartConfig"></param>
        public ChartBuilder(ChartConfigProvider ChartConfig, System.Web.UI.Page ThisPage)
        {
            try
            {
                currConfig = ChartConfig;
                chartToBuild = new Chart();
                chartToBuild.Series.Add(new Series());
                chartToBuild.ChartAreas.Add(new ChartArea());
                chartToBuild.ImageLocation = "~/ChartPic_#UID";
                chartToBuild.Page = ThisPage;
                chartToBuild.RenderType = RenderType.ImageTag;
                chartToBuild.ImageType = ChartImageType.Png;
                chartToBuild.ImageStorageMode = ImageStorageMode.UseImageLocation;

                XmlDocument xmlData = GetXmlData();

                BuildXAxisLabels(xmlData);

                FillSeriesData(xmlData);
                SetObjectParameters(chartToBuild, currConfig.ChartParams);
                SetObjectParameters(chartToBuild.ChartAreas[0], currConfig.ChartAreaParams);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ChartConfig">A populated ChartConfig object</param>
        /// <param name="ChartToBuild">The chart object used to provide the chart</param>
        public ChartBuilder(ChartConfigProvider ChartConfig, Chart ChartToBuild)
        {
            currConfig = ChartConfig;
            chartToBuild = ChartToBuild;

            XmlDocument xmlData = GetXmlData();

            BuildXAxisLabels(xmlData);

            FillSeriesData(xmlData);
            SetObjectParameters(chartToBuild, currConfig.ChartParams);
            SetObjectParameters(chartToBuild.ChartAreas[0], currConfig.ChartAreaParams);
            SetObjectParameters(chartToBuild.ChartAreas[0].AxisX, currConfig.ChartAxisXParams);
            SetObjectParameters(chartToBuild.ChartAreas[0].AxisY, currConfig.ChartAxisYParams);

            if (currConfig.ChartLegendParams.Count > 0)
            {
                Legend l = new Legend();
                SetObjectParameters(l, currConfig.ChartLegendParams);
                chartToBuild.Legends.Add(l);
            }
        }

        /// <summary>
        /// Parse the xAxis values from the XML
        /// </summary>
        /// <param name="xmlData">XmlDocument containing data in the standardized format</param>
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

        /// <summary>
        /// Populates the Chart Series with data values from the XML
        /// </summary>
        /// <param name="xmlData">XmlDocument containing data in the standardized format</param>
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

        /// <summary>
        /// Gets data from the URI provided in the ChartConfigProvider passed in the constructor
        /// </summary>
        /// <returns></returns>
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

        public string GetChartHtml()
        {

            StringBuilder htmlString = new StringBuilder(); // this will hold the string

            using (StringWriter stringWriter = new StringWriter(htmlString))
            {

                HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

                chartToBuild.RenderControl(htmlWriter);

                return htmlString.ToString();
            }
        }


        private void SetObjectParameters(Object chartObject, Hashtable ht)
        {
            foreach (string name in ht.Keys)
            {
                string val = ht[name].ToString();

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
                    else if (pi.PropertyType.FullName == "System.Web.UI.WebControls.Unit")
                    {
                        o = new System.Web.UI.WebControls.Unit(val);
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
    }
}