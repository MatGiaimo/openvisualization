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
using System.Data.OleDb;

namespace OpenVisualization.Data
{

    /// <summary>
    /// Base class for a ChartDataReader
    /// </summary>
    public class ChartDataReaderBase : IChartDataReader
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartDataReaderBase()
        {

        }

        /// <summary>
        /// Get the XML representation of the data
        /// </summary>
        /// <returns></returns>
        public virtual XmlDocument getXML()
        {
            return new XmlDocument();
        }

        /// <summary>
        /// Used to copy one Stream to another
        /// </summary>
        /// <param name="source">Source stream</param>
        /// <param name="target">Target stream</param>
        protected void Copy(Stream source, Stream target)
        {
            byte[] buffer = new byte[0x10000];
            int bytes;
            try
            {
                while ((bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                {
                    target.Write(buffer, 0, bytes);
                }
            }
            finally
            {
                target.Flush();
                target.Close();
            }
        }

        /// <summary>
        /// Generates a DataSet object from a stream of data.  Currently tailored for csv datasets.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected DataSet GenerateDataSet(Stream data)
        {
            //filename = SaveStreamToTempFile(data);

            string tempFilePath = @"C:\projects\EEC626\openvisualization\TempData.csv";

            FileStream fs = new FileStream(tempFilePath, FileMode.OpenOrCreate);

            Copy(data, fs);

            fs.Close();

            String conn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\;Extended Properties=""Text;HDR=No;FMT=Delimited""";

            OleDbConnection cn = new OleDbConnection(conn);
            OleDbCommand cmd = new OleDbCommand(string.Format(@"SELECT * FROM {0}", tempFilePath), cn);
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);

            cn.Open();

            DataSet ds = new DataSet("YahooFinance");


            da.Fill(ds);
            ds.Tables[0].TableName = "TimeSeries";

            File.Delete(tempFilePath);

            return ds;

            //DataTable dt = ds.Tables[0]; 
        }

        /// <summary>
        /// Returns a DataTable representation of the supplied LINQ result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varlist"></param>
        /// <returns></returns>
        protected DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
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