using System;
using System.Data;
using System.Linq;
using System.Xml;
using System.IO;



namespace OpenVisualization.Data
{
    /// <summary>
    /// A ChartDataReader that reads data from a SQL Telemetry database 'oms'
    /// </summary>
    public class OMSReader : ChartDataReaderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OMSReader"/> class.
        /// </summary>
        public OMSReader() : base()
        {
        }

        private string[] _sensors;

        /// <summary>
        /// A string array of sensors to query
        /// </summary>
        public string[] Sensors
        {
            get
            {
                if (_sensors == null)
                {
                    throw (new Exception("Sensors must be set"));
                }
                return _sensors;
            }
            set
            {
                _sensors = value;
            }
        }

        private string _startDate;

        /// <summary>
        /// The starting date/time for the query
        /// </summary>
        public string StartDate
        {
            get
            {
                if (_startDate == null)
                {
                    throw (new Exception("StartDate must be set"));
                }
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        private string _endDate;

        /// <summary>
        /// The ending date/time for the query
        /// </summary>
        public string EndDate
        {
            get
            {
                if (_endDate == null)
                {
                    throw (new Exception("EndDate must be set"));
                }
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        /// <summary>
        /// Reads data from the 'oms' database
        /// </summary>
        /// <returns>Standard XML representation of a SQL result from querying the 'oms' table with the parameters provided through the properties</returns>
        public override XmlDocument getXML()
        {
            XmlDocument xReturn = new XmlDocument();

            DateTime startDate;
            startDate = DateTime.Parse(StartDate);

            DateTime endDate;
            endDate = DateTime.Parse(EndDate);

            DataContext.OmsDataContext db = new DataContext.OmsDataContext();

            var results = from q in db.Queries
                          where Sensors.Contains(q.sensor_id)
                          where q.read_date > startDate
                          where q.read_date < endDate
                          select q;

            DataSet ds = new DataSet("Telemetry");
            DataTable dt = LINQToDataTable(results);

            dt.TableName = "Data";

            ds.Tables.Add(dt);

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextwriter = new XmlTextWriter(stringWriter);

            ds.WriteXml(xmlTextwriter, XmlWriteMode.IgnoreSchema);

            xReturn.LoadXml(stringWriter.ToString());

            return xReturn;
        }
    }
}