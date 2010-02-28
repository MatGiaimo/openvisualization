using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace DOTNetVisualization.Services
{
    public partial class GetTelemetryData : System.Web.UI.Page
    {
        OMSReader omsRdr = new OMSReader();

        protected void Page_Load(object sender, EventArgs e)
        {
            string[] sensors;
            string strStartDate, strEndDate;

            if (Request.Params["sensors"] != null)
                sensors = Request.Params["sensors"].ToString().Split(new char[] { ',' });
            else
                sensors = new string[] { "091F0022" };

            if (Request.Params["startdate"] != null)
                strStartDate = Request.Params["startdate"].ToString();
            else
                strStartDate = "2010-01-12";

            if (Request.Params["enddate"] != null)
                strEndDate = Request.Params["enddate"].ToString();
            else
                strEndDate = "2010-01-13";

            XmlDocument xReturn = omsRdr.getXML(sensors, strStartDate, strEndDate);

            Response.ContentType = "text/xml";
            Response.Write(xReturn.OuterXml);
        }
    }
}
