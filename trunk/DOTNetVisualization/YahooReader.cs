using System;
using System.Xml;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Data.OleDb;


public class YahooReader
{
    public YahooReader()
    {
    }

    public string BuildYahooURI(string strTicker,string strStartDate, string strEndDate)
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

    public XmlDocument getXML(string strTicker,string strStartDate, string strEndDate)
    {
        XmlDocument xReturn = new XmlDocument();
        DataSet result = new DataSet();

        string sYahooURI = BuildYahooURI(strTicker, strStartDate, strEndDate);

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

    public DataSet GenerateDataSet(Stream data)
    {
        //filename = SaveStreamToTempFile(data);

        string tempFilePath = @"C:\projects\EEC626\DOTNetVisualization\TempData.csv";

        FileStream fs = new FileStream(tempFilePath,FileMode.OpenOrCreate);

        Copy(data, fs);

        fs.Close();

        String conn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\;Extended Properties=""Text;HDR=No;FMT=Delimited""";

        OleDbConnection cn = new OleDbConnection(conn);
        OleDbCommand cmd = new OleDbCommand(string.Format(@"SELECT * FROM {0}",tempFilePath), cn);
        OleDbDataAdapter da = new OleDbDataAdapter(cmd);

        cn.Open();

        DataSet ds = new DataSet("YahooFinance");


        da.Fill(ds);
        ds.Tables[0].TableName = "TimeSeries";

        File.Delete(tempFilePath);

        return ds;

        //DataTable dt = ds.Tables[0]; 
    }

    public void Copy(Stream source, Stream target)
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

}



