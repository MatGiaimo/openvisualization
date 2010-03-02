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

        protected void Page_Load(object sender, EventArgs e)
        {
            string strTicker, strStartDate, strEndDate;

            if (Request.Params["ticker"] != null)
                strTicker = Request.Params["ticker"].ToString();
            else
                strTicker = "MSFT";

            if (Request.Params["startdate"] != null)
                strStartDate = Request.Params["startdate"].ToString();
            else
                strStartDate = "12-1-2009";

            if (Request.Params["enddate"] != null)
                strEndDate = Request.Params["enddate"].ToString();
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
