using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Configuration;

namespace OpenVisualization.Services
{
    /// <summary>
    /// Public partial class RESTTest
    /// </summary>
    public partial class RESTTest : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the Click event of the btnSubmitImageXML control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSubmitImageXML_Click(object sender, EventArgs e)
        {

            //string baseUrl = Context.Request.Url.GetLeftPart(UriPartial.Authority);
            string baseUrl = ConfigurationManager.AppSettings["AppBaseUrl"];

            string webPath = baseUrl + "/Services/GetChartImageMap.aspx";

            string xmlToUse = rblConfigs.SelectedValue;

            WebClient webClient = new WebClient();
            XmlDocument xmlChartConfig = new XmlDocument();
            // Get the chart config
            Uri uri = new Uri(Server.MapPath(xmlToUse),
              UriKind.RelativeOrAbsolute);
            Stream configData = webClient.OpenRead(uri);
            XmlTextReader xmlText = new XmlTextReader(configData);
            xmlChartConfig.Load(xmlText);
            configData.Close();

            Response.Write(PostXml(webPath, xmlChartConfig.OuterXml));
        }

        /// <summary>
        /// Posts the XML.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public string PostXml(string url, string xml)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = bytes.Length;
            request.ContentType = "text/xml";
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string message = String.Format("POST failed. Received HTTP {0}",
                response.StatusCode);
                throw new ApplicationException(message);
            }
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);
            return responseReader.ReadToEnd();
        } 
    }
}
