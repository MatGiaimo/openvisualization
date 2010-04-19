using System;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using OpenVisualization.Configuration;
using OpenVisualization.Charting;


namespace OpenVisualization.Web
{
    /// <summary>
    /// Public partial class Default
    /// </summary>
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set chart image storage mode
            Chart1.ImageStorageMode = ImageStorageMode.UseImageLocation;
            Chart2.ImageStorageMode = ImageStorageMode.UseImageLocation;
            Chart3.ImageStorageMode = ImageStorageMode.UseImageLocation;

            double[] yValues = { 20, 10, 24, 23 };
            string[] xValues = { "England", "Scotland", "Ireland", "Wales" };
            Series mySeries = Chart1.Series[0];
            mySeries.Points.DataBindXY(xValues, yValues);
            mySeries.ChartType = SeriesChartType.Pie;
        }

        /// <summary>
        /// Handles the Click event of the btnShowPieChart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnShowPieChart_Click(object sender, EventArgs e)
        {
            pnlPieChart.Visible = true;
            pnlTimeSeries.Visible = false;
            pnlTelemetryData.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the btnShowTimeSeries control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnShowTimeSeries_Click(object sender, EventArgs e)
        {
            pnlPieChart.Visible = false;
            pnlTimeSeries.Visible = true;
            pnlTelemetryData.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the btnShowTelemetryData control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnShowTelemetryData_Click(object sender, EventArgs e)
        {
            pnlPieChart.Visible = false;
            pnlTimeSeries.Visible = false;
            pnlTelemetryData.Visible = true;
        }

        /// <summary>
        /// Handles the Click event of the btnShowRESTImageSubmit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnShowRESTImageSubmit_Click(object sender, EventArgs e)
        {
            pnlPieChart.Visible = false;
            pnlTimeSeries.Visible = false;
            pnlTelemetryData.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the Button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Button1_Click(object sender, EventArgs e)
        {
            // Variable declarations
            StringBuilder dataURI = new StringBuilder();
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("~/Configuration/Charts/PriceHistory1.xml"),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);

            // I'm hard coding to read in the chart called 'Price History 1'. In a
            // 'real' environment my config would contain multiple charts, and I'd
            // pass the desired chart (along with any parameters) in the request
            // string. But for simplicity I've kept this hard coded.
            XmlNodeList lst =
              xmlChartConfig.SelectNodes("/root/Chart[@Name='PriceHistory1']/Uri/*");

            // The first child contains the root URI
            if (lst != null)
            {
                dataURI.Append(lst.Item(0).InnerText);

                // If the textboxes are full use the values else use the config file
                if (!string.IsNullOrEmpty(ticker.Text) && startdate.SelectedDate != null && enddate.SelectedDate != null)
                {
                    dataURI.Append("?");
                    dataURI.Append(ticker.ID);
                    dataURI.Append("=");
                    dataURI.Append(ticker.Text);
                    dataURI.Append("&");
                    dataURI.Append(startdate.ID);
                    dataURI.Append("=");
                    dataURI.Append(string.Format("{0:MM-dd-yyyy}",startdate.SelectedDate));
                    dataURI.Append("&");
                    dataURI.Append(enddate.ID);
                    dataURI.Append("=");
                    dataURI.Append(string.Format("{0:MM-dd-yyyy}", enddate.SelectedDate));
                }
                else
                {
                    // The rest of the children of this node contain the parameters
                    // the first parameter is prefixed with ?, the rest with &
                    // i.e. http://url?firstparam=firstval&secondparam=secondval etc
                    for (int lp = 1; lp < lst.Count; lp++)
                    {
                        if (lp == 1)
                            dataURI.Append("?");
                        else
                            dataURI.Append("&");

                        // In this case the desired parameters are hard coded into the XML.
                        // in a 'real' server you'd likely accept them as params to this page
                        dataURI.Append(lst.Item(lp).Attributes.Item(0).Value);
                        dataURI.Append("=");
                        dataURI.Append(lst.Item(lp).InnerText);
                    }
                }
            }

            // Now that we have the URI, we can call it and get the XML
            uri = new Uri(dataURI.ToString());
            Stream phData = webClient.OpenRead(uri);
            xmlText = new XmlTextReader(phData);
            xmlData.Load(xmlText);

            // This simple example is hard coded for a particular chart
            // ('PriceHistory1') and assumes only 1 series
            lst = xmlChartConfig.SelectNodes(
              "/root/Chart[@Name='PriceHistory1']/Data/SeriesDefinitions/Series/Data");

            // I'm taking the first series, because I only have 1
            // A 'real' server would iterate through all the matching nodes on the
            // XPath
            if (lst != null)
            {
                string xPath = lst.Item(0).InnerText;

                // I've read the XPath that determines the data location, so I can
                // create a nodelist from that
                XmlNodeList data = xmlData.SelectNodes(xPath);
                Series series = new Series();

                // I'm hard coding for 'Line' here -- the 'real' server should
                // read the chart type from the config
                series.ChartType = SeriesChartType.Line;
                double nCurrent = 0.0;

                // I can now iterate through all the values of the node list, and
                if (data != null)
                    foreach (XmlNode nd in data)
                    {
                        // .. create a DataPoint from them, which is added to the Series
                        DataPoint d = new DataPoint(nCurrent, Convert.ToDouble(nd.
                                                                                   InnerText));
                        series.Points.Add(d);
                        nCurrent++;
                    }

                // Finally I add the series to my chart
                Chart2.Series.Add(series);
            }
        }

        /// <summary>
        /// Handles the Click event of the Button2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Button2_Click(object sender, EventArgs e)
        {
            // Variable declarations
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("~/Configuration/Charts/PriceHistory1.xml"),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);
            configData.Close();

            ChartConfigProvider chartConfig = new ChartConfigProvider(xmlChartConfig);

            ChartBuilder cb = new ChartBuilder(chartConfig, Chart2);
        }

        /// <summary>
        /// Handles the Click event of the Button3 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Button3_Click(object sender, EventArgs e)
        {
            // Variable declarations
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("~/Configuration/Charts/TelemetryData1.xml"),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);
            configData.Close();

            ChartConfigProvider chartConfig = new ChartConfigProvider(xmlChartConfig);

            ChartBuilder cb = new ChartBuilder(chartConfig, Chart3);
        }
    }
}
