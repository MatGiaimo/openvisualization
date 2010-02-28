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



namespace DOTNetVisualization
{
    public class OMSReader
    {
        public OMSReader()
        {
        }

        public XmlDocument getXML(string[] sensors, string strStartDateTime, string strEndDateTime)
        {
            XmlDocument xReturn = new XmlDocument();

            DateTime startDate = new DateTime();
            startDate = DateTime.Parse(strStartDateTime);

            DateTime endDate = new DateTime();
            endDate = DateTime.Parse(strEndDateTime);

            OmsDataContext db = new OmsDataContext();

            var results = from q in db.Queries
                          where sensors.Contains(q.sensor_id)
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


        private DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
    }
}