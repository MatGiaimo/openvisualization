using System;
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

                GetStaticChartImage cp = new GetStaticChartImage();

                Type type = BuildManager.GetCompiledType("~/Services/GetStaticChartImage.aspx");
                GetStaticChartImage pageView = (GetStaticChartImage)Activator.CreateInstance(type);

                StringWriter textWriter = new StringWriter();
                HttpContext.Current.Server.Execute((IHttpHandler)pageView, textWriter, false);
                return textWriter.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
