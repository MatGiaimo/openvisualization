﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.IO;
using OpenVisualization.Configuration;
using OpenVisualization.Charting;
using System.Xml;
using System.Web.UI;
using System.Web.Compilation;
using System.Web.UI.DataVisualization.Charting;
using System.Text;

namespace OpenVisualization.Services
{
    /// <summary>
    /// Summary description for GetChart
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GetChart : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string GetStaticImage(string xmlChartConfig)
        {
            try
            {
                //Type type = BuildManager.GetCompiledType("~/Services/GetStaticChartImage.aspx");
                //GetStaticChartImage pageView = (GetStaticChartImage)Activator.CreateInstance(type);

                //StringWriter textWriter = new StringWriter();
                //HttpContext.Current.Server.Execute((IHttpHandler)pageView, textWriter, false);
                //return textWriter.ToString();

                string baseUrl = Context.Request.Url.GetLeftPart(UriPartial.Authority);

                string webPath = baseUrl + "/Services/GetStaticChartImage.aspx";

                return PostXml(webPath, xmlChartConfig);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
