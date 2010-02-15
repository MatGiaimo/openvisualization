using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace DOTNetVisualization
{
    using System.Web.UI.DataVisualization.Charting;
    using ChartConfig;
    using System.Xml;
    using System.Text;
    using System.Net;
    using System.IO;

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Chart1.ImageStorageMode = ImageStorageMode.UseImageLocation;
            Chart2.ImageStorageMode = ImageStorageMode.UseImageLocation;

            double[] yValues = { 20, 10, 24, 23 };
            string[] xValues = { "England", "Scotland", "Ireland", "Wales" };
            Series mySeries = Chart1.Series[0];
            mySeries.Points.DataBindXY(xValues, yValues);
            mySeries.ChartType = SeriesChartType.Pie;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // Variable declarations
            StringBuilder dataURI = new StringBuilder();
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("/Configuration/Charts/PriceHistory1.xml"),
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
            dataURI.Append(lst.Item(0).InnerText.ToString());

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
                    dataURI.Append(lst.Item(lp).Attributes.Item(0).Value.ToString());
                    dataURI.Append("=");
                    dataURI.Append(lst.Item(lp).InnerText);
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

        protected void Button2_Click(object sender, EventArgs e)
        {
            // Variable declarations
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("/Configuration/Charts/PriceHistory1.xml"),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);

            ChartConfigProvider chartConfig = new ChartConfigProvider(xmlChartConfig);

            // Now that we have the URI, we can call it and get the XML
            uri = new Uri(chartConfig.URI);
            Stream phData = webClient.OpenRead(uri);
            xmlText = new XmlTextReader(phData);
            xmlData.Load(xmlText);

            string xAxisLabelSeriesName = string.Empty;
            ArrayList xAxisValues = new ArrayList();
            //Find the name of the series with the xaxis label
            //Get the xAxisLabel values
            foreach (ChartConfigSeries ccSeries in chartConfig.Series)
            {
                if (ccSeries.IsXAxisLabel == true)
                {
                    xAxisLabelSeriesName = ccSeries.Name;

                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    foreach (XmlNode nd in data)
                    {
                        xAxisValues.Add(DateTime.Parse(nd.InnerText));
                    }
                }
            }
           
            foreach (ChartConfigSeries ccSeries in chartConfig.Series)
            {
                //Ignore the xAxisLabelSeries when populating y axis data
                if (ccSeries.Name != xAxisLabelSeriesName)
                {
                    XmlNodeList data = xmlData.SelectNodes(ccSeries.XPath);

                    int xAxisIndex = 0;
                    foreach (XmlNode nd in data)
                    {
                        ccSeries.Points.AddXY(xAxisValues[xAxisIndex],nd.InnerText);
                        xAxisIndex++;
                    }
                    xAxisIndex = 0;
                    Chart2.Series.Add((Series)ccSeries);
                }
            }
            configData.Close();
        }

    }
}
