<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DOTNetVisualization.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%--Microsoft Charting - http://weblogs.asp.net/scottgu/archive/2008/11/24/new-asp-net-charting-control-lt-asp-chart-runat-quot-server-quot-gt.aspx--%>
<%--http://weblogs.asp.net/melvynharbour/archive/2008/11/25/combining-asp-net-mvc-and-asp-net-charting-controls.aspx--%>
<%--http://blogs.msdn.com/delay/archive/2009/03/19/silverlight-charting-is-faster-and-better-than-ever-silverlight-toolkit-march-09-release-now-available.aspx--%>
<html>
<head>
    <title>GetPriceHistory</title>
    <link rel="stylesheet" type="text/css" href="main.css" />
</head>
<body>
    <form id="form2" runat="server">
    <asp:ScriptManager ID="Scriptmanager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <div>
                            Examples:</div>
                        <div>
                            <a href="Services/GetPriceHistory.aspx">Services/GetPriceHistory.aspx</a>
                        </div>
                        <div>
                            <asp:LinkButton ID="btnShowPieChart" runat="server" OnClick="btnShowPieChart_Click">Simple Pie Chart Example</asp:LinkButton>
                        </div>
                        <div>
                            <asp:LinkButton ID="btnShowTimeSeries" runat="server" OnClick="btnShowTimeSeries_Click">Time Series Example</asp:LinkButton>
                        </div>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
            <asp:Panel ID="pnlPieChart" runat="server" Visible="false">
                <asp:Chart ID="Chart1" runat="server">
                    <Series>
                        <asp:Series Name="Series1">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </asp:Panel>
            <asp:Panel ID="pnlTimeSeries" runat="server" Visible="false">
                <div>
                    <asp:Chart ID="Chart2" runat="server">
                        <Series>
                            <asp:Series Name="Series1">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                </div>
                <div>
                    <asp:Button ID="Button2" runat="server" Text="Build Chart Using ConfigProvider" OnClick="Button2_Click" />
                </div>
                <asp:Panel ID="pnlChartParams" runat="server" Visible="false">
                    <asp:Button ID="Button1" runat="server" Text="Build Chart Using Parameters" OnClick="Button1_Click" />
                    <div>
                        Ticker Symbol:<asp:TextBox ID="ticker" runat="server"></asp:TextBox></div>
                    <div>
                        Start Date:<asp:Calendar ID="startdate" runat="server"></asp:Calendar>
                    </div>
                    <div>
                        End Date:<asp:Calendar ID="enddate" runat="server"></asp:Calendar>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
