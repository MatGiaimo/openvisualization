using System;
using System.Xml;
using OpenVisualization.Data;

namespace OpenVisualization.Services
{
    /// <summary>
    /// Retrieves telemetry data from the 'oms' webservice
    /// </summary>
    public partial class GetTelemetryData : System.Web.UI.Page
    {
        OMSReader omsRdr = new OMSReader();

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string[] sensors;
            string strStartDate, strEndDate;

            if (Request.Params["sensors"] != null)
                sensors = Request.Params["sensors"].Split(new[] { ',' });
            else
                sensors = new[] { "091F0022" };

            if (Request.Params["startdate"] != null)
                strStartDate = Request.Params["startdate"];
            else
                strStartDate = "2010-01-12";

            if (Request.Params["enddate"] != null)
                strEndDate = Request.Params["enddate"];
            else
                strEndDate = "2010-01-13";

            omsRdr.Sensors = sensors;
            omsRdr.StartDate = strStartDate;
            omsRdr.EndDate = strEndDate;

            XmlDocument xReturn = omsRdr.getXML();

            Response.ContentType = "text/xml";
            Response.Write(xReturn.OuterXml);
        }
    }
}
