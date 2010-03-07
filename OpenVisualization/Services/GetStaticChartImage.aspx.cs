using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Xml;
using System.IO;
using OpenVisualization.Configuration;
using OpenVisualization.Charting;

namespace OpenVisualization.Services
{
    public partial class GetStaticChartImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Variable declarations
            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            XmlDocument xmlData = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath("/Configuration/Charts/TelemetryData1.xml"),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);
            configData.Close();

            ChartConfigProvider ccp = new ChartConfigProvider(xmlChartConfig);

            ChartBuilder cb = new ChartBuilder(ccp,this.Page);

            Response.ContentType = "text/html";
            Response.Write(cb.GetChartHtml());
        }
    }
}
