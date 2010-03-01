using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.IO;



namespace OpenVisualization
{
    public class OMSReader : ChartDataReaderBase
    {
        public OMSReader() : base()
        {
        }

        private string[] _sensors;

        public string[] Sensors
        {
            get
            {
                if (_sensors == null)
                {
                    throw (new Exception("Sensors must be set"));
                }
                else
                    return _sensors;
            }
            set
            {
                _sensors = value;
            }
        }

        private string _startDate;

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

        public override XmlDocument getXML()
        {
            XmlDocument xReturn = new XmlDocument();

            DateTime startDate = new DateTime();
            startDate = DateTime.Parse(StartDate);

            DateTime endDate = new DateTime();
            endDate = DateTime.Parse(EndDate);

            OpenVisualization.Data.DataContext.OmsDataContext db = new OpenVisualization.Data.DataContext.OmsDataContext();

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