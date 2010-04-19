using System;
using System.Xml;
using OpenVisualization.Data;

namespace OpenVisualization.Services
{
    /// <summary>
    /// Gets a price history result set from Yahoo's stock quote webservice (http://ichart.finance.yahoo.com/)
    /// </summary>
    public partial class GetPriceHistory : System.Web.UI.Page
    {
        YahooReader yahooRdr = new YahooReader();

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string strTicker, strStartDate, strEndDate;

            if (Request.Params["ticker"] != null)
                strTicker = Request.Params["ticker"];
            else
                strTicker = "MSFT";

            if (Request.Params["startdate"] != null)
                strStartDate = Request.Params["startdate"];
            else
                strStartDate = "12-1-2009";

            if (Request.Params["enddate"] != null)
                strEndDate = Request.Params["enddate"];
            else
                strEndDate = "1-26-2010";

            yahooRdr.Ticker = strTicker;
            yahooRdr.StartDate = strStartDate;
            yahooRdr.EndDate = strEndDate;

            XmlDocument xReturn = yahooRdr.getXML();

            Response.ContentType = "text/xml";
            Response.Write(xReturn.OuterXml);
        }
    }
}
