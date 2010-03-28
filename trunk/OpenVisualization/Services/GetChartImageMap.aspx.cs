﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using OpenVisualization.Configuration;
using OpenVisualization.Charting;

namespace OpenVisualization.Services
{
    public partial class GetChartImageMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XmlDocument xmlInput = new XmlDocument();
            xmlInput.Load(Request.InputStream);

            ChartConfigProvider ccp = new ChartConfigProvider(xmlInput);

            ChartBuilder cb = new ChartBuilder(ccp, this.Page, true);

            Response.ContentType = "text/html";
            Response.Write(cb.GetChartHtml()+cb.GetHtmlImageMap());
        }
    }
}