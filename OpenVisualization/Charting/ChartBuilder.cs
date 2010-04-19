using System.Collections;
using System.Web.UI.DataVisualization.Charting;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System;
using System.Reflection;
using OpenVisualization.Configuration;
using System.Web.UI;

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
        private readonly ChartConfigProvider currConfig;

        /// <summary>
        /// The chart object used to provide the chart
        /// </summary>
        private readonly Chart chartToBuild;

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
        /// <param name="ChartConfig">The chart config.</param>
        /// <param name="ThisPage">The this page.</param>
        /// <param name="imageMap">if set to <c>true</c> [image map].</param>
        public ChartBuilder(ChartConfigProvider ChartConfig, Page ThisPage, bool imageMap)
        {
            try
            {
                currConfig = ChartConfig;
                chartToBuild = new Chart();
                chartToBuild.ChartAreas.Add(new ChartArea());
                chartToBuild.Page = ThisPage;

                XmlDocument xmlData = GetXmlData();

                BuildXAxisLabels(xmlData);

                FillSeriesData(xmlData);
                SetObjectParameters(chartToBuild, currConfig.ChartParams);
                SetObjectParameters(chartToBuild.ChartAreas[0], currConfig.ChartAreaParams);
                SetObjectParameters(chartToBuild.ChartAreas[0].AxisX, currConfig.ChartAxisXParams);
                SetObjectParameters(chartToBuild.ChartAreas[0].AxisY, currConfig.ChartAxisYParams);
                SetObjectParameters(chartToBuild.ChartAreas[0].AxisX2, currConfig.ChartAxisX2Params);
                SetObjectParameters(chartToBuild.ChartAreas[0].AxisY2, currConfig.ChartAxisY2Params);              

                if (currConfig.ChartLegendParams.Count > 0)
                {
                    Legend l = new Legend();
                    SetObjectParameters(l, currConfig.ChartLegendParams);
                    chartToBuild.Legends.Add(l);
                }

                SetRenderMethod(imageMap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Will set the chart rendering method to imagemap if imagemap is true, otherwise the chart rendering method will be set as imagetag.
        /// </summary>
        /// <param name="imageMap"></param>
        private void SetRenderMethod(bool imageMap)
        {
            if (imageMap)
            {
                chartToBuild.ImageLocation = "~/ChartPic_#UID";
                chartToBuild.RenderType = RenderType.ImageTag;
                chartToBuild.ImageType = ChartImageType.Png;
                chartToBuild.ImageStorageMode = ImageStorageMode.UseImageLocation;
                //chartToBuild.Series[0].ToolTip = "X Value \t= #VALX{f}\nY Value \t= #VALY{n}";
                
            }
            else
            {
                chartToBuild.ImageLocation = "~/ChartPic_#UID";
                chartToBuild.RenderType = RenderType.ImageTag;
                chartToBuild.ImageType = ChartImageType.Png;
                chartToBuild.ImageStorageMode = ImageStorageMode.UseImageLocation;
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
            SetObjectParameters(chartToBuild.ChartAreas[0].AxisX2, currConfig.ChartAxisX2Params);
            SetObjectParameters(chartToBuild.ChartAreas[0].AxisY2, currConfig.ChartAxisY2Params);

            if (currConfig.ChartLegendParams.Count > 0)
            {
                Legend l = new Legend();
                SetObjectParameters(l, currConfig.ChartLegendParams);
                chartToBuild.Legends.Add(l);
            }

            // Process special params
            if (currConfig.ChartAxisYParams.Contains("GridLines"))
            {
                bool val = Convert.ToBoolean(currConfig.ChartAxisYParams["GridLines"]);
                chartToBuild.ChartAreas[0].AxisY.MajorGrid.Enabled = val;
            }
            if (currConfig.ChartAxisY2Params.Contains("GridLines"))
            {
                bool val = Convert.ToBoolean(currConfig.ChartAxisY2Params["GridLines"]);
                chartToBuild.ChartAreas[0].AxisY2.MajorGrid.Enabled = val;
            }
            if (currConfig.ChartAxisXParams.Contains("GridLines"))
            {
                bool val = Convert.ToBoolean(currConfig.ChartAxisXParams["GridLines"]);
                chartToBuild.ChartAreas[0].AxisX.MajorGrid.Enabled = val;
            }
            if (currConfig.ChartAxisX2Params.Contains("GridLines"))
            {
                bool val = Convert.ToBoolean(currConfig.ChartAxisX2Params["GridLines"]);
                chartToBuild.ChartAreas[0].AxisX2.MajorGrid.Enabled = val;
            }
        }

        /// <summary>
        /// Parse the xAxis values from the XML
        /// </summary>
        /// <param name="xmlData">XmlDocument containing data in the standardized format</param>
        private void BuildXAxisLabels(XmlNode xmlData)
        {
            xAxisLabelValues = new ArrayList();

            //Find the name of the series with the xaxis label
            //Get the xAxisLabel values
            foreach (ChartConfigSeries ccSeries in currConfig.Series)
            {
                if (ccSeries.IsXAxisLabel)
                {
                    xAxisLabelSeriesName = ccSeries.Name;

                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    if (data != null)
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
        private void FillSeriesData(XmlNode xmlData)
        {
            foreach (ChartConfigSeries ccSeries in currConfig.Series)
            {
                //Ignore the xAxisLabelSeries when populating y axis data
                if (ccSeries.Name != xAxisLabelSeriesName)
                {
                    
                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    int xAxisIndex = 0;
                    if (data != null)
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

                    chartToBuild.Series.Add(ccSeries);
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

        /// <summary>
        /// Gets the chart HTML.
        /// </summary>
        /// <returns></returns>
        public string GetChartHtml()
        {

            StringBuilder htmlString = new StringBuilder(); // this will hold the string

            using (StringWriter stringWriter = new StringWriter(htmlString))
            {

                HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

                chartToBuild.RenderControl(htmlWriter);

                string html = htmlString.ToString();

                // For future use if we ever get to cross-domain ajax scripting
                //string srchStr = "src=\"/";

                //int srcIndex = html.IndexOf(srchStr)+srchStr.Length;

                //string start = html.Substring(0, srcIndex-1);

                //string end = html.Substring(start.Length,html.Length-(start.Length));

                //string hostName = ConfigurationManager.AppSettings["ExternalHostName"];

                //html = string.Format("{0}{1}{2}", start, hostName, end);

                return html;
            }
        }

        /// <summary>
        /// Gets the HTML image map.
        /// </summary>
        /// <returns></returns>
        public string GetHtmlImageMap()
        {
            return chartToBuild.GetHtmlImageMap("ImageMap");
        }


        private static void SetObjectParameters(Object chartObject, IDictionary ht)
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