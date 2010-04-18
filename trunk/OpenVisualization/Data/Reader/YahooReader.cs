using System;
using System.Xml;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Data.OleDb;


namespace OpenVisualization.Data
{
    /// <summary>
    /// A ChartDataReader that reads stock quote data from Yahoo's stock quote webservice (http://ichart.finance.yahoo.com/)
    /// </summary>
    public class YahooReader : ChartDataReaderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YahooReader"/> class.
        /// </summary>
        public YahooReader() : base()
        {
        }

        private string _ticker;

        /// <summary>
        /// Stock ticker symbol
        /// </summary>
        public string Ticker
        {
            get
            {
                if (_ticker == null)
                {
                    throw (new Exception("Ticker must be set"));
                }
                else
                    return _ticker;
            }
            set
            {
                _ticker = value;
            }
        }

        private string _startDate;

        /// <summary>
        /// Start date for the stock quote resultset
        /// </summary>
        public string StartDate
        {
            get
            {
                if (_startDate == null)
                {
                    throw (new Exception("StartDate must be set"));
                }
                else
                    return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        private string _endDate;

        /// <summary>
        /// Ending date for the stock quote resultset
        /// </summary>
        public string EndDate
        {
            get
            {
                if (_endDate == null)
                {
                    throw (new Exception("EndDate must be set"));
                }
                else
                    return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        /// <summary>
        /// Builds the URI used to call Yahoo's stock ticker service
        /// </summary>
        /// <param name="strTicker">Stock ticker symbol</param>
        /// <param name="strStartDate">Start date</param>
        /// <param name="strEndDate">End date</param>
        /// <returns></returns>
        private string BuildYahooURI(string strTicker, string strStartDate, string strEndDate)
        {
            string strReturn = "";

            DateTime dStart = Convert.ToDateTime(strStartDate);
            DateTime dEnd = Convert.ToDateTime(strEndDate);
            string sStartDay = dStart.Day.ToString();
            string sStartMonth = (dStart.Month - 1).ToString();
            string sStartYear = dStart.Year.ToString();
            string sEndDay = dEnd.Day.ToString();
            string sEndMonth = (dEnd.Month - 1).ToString();
            string sEndYear = dEnd.Year.ToString();
            StringBuilder sYahooURI =
              new StringBuilder("http://ichart.finance.yahoo.com/table.csv?s=");
            sYahooURI.Append(strTicker);
            sYahooURI.Append("&a=");
            sYahooURI.Append(sStartMonth);
            sYahooURI.Append("&b=");
            sYahooURI.Append(sStartDay);
            sYahooURI.Append("&c=");
            sYahooURI.Append(sStartYear);
            sYahooURI.Append("&d=");
            sYahooURI.Append(sEndMonth);
            sYahooURI.Append("&e=");
            sYahooURI.Append(sEndDay);
            sYahooURI.Append("&f=");
            sYahooURI.Append(sEndYear);
            sYahooURI.Append("&g=d");
            sYahooURI.Append("&ignore=.csv");
            strReturn = sYahooURI.ToString();

            return strReturn;
        }


        /// <summary>
        /// Returns an XML representation of Yahoo's stock qoute service
        /// </summary>
        /// <returns>Standard XML representation of a csv result from the Yahoo stock webservice with the parameters provided through the properties</returns>
        public override XmlDocument getXML()
        {
            XmlDocument xReturn = new XmlDocument();
            DataSet result = new DataSet();

            string sYahooURI = BuildYahooURI(Ticker, StartDate, EndDate);

            WebClient wc = new WebClient();
            Stream yData = wc.OpenRead(sYahooURI);

            result = GenerateDataSet(yData);

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextwriter = new XmlTextWriter(stringWriter);

            result.WriteXml(xmlTextwriter, XmlWriteMode.IgnoreSchema);

            XmlNode xRoot = xReturn.CreateElement("root");
            xReturn.AppendChild(xRoot);
            xReturn.LoadXml(stringWriter.ToString());

            return xReturn;
        }
    }

}

