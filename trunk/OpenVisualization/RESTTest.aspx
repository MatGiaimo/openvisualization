<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RESTTest.aspx.cs" Inherits="OpenVisualization.Services.RESTTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Panel ID="pnlRESTImageSubmit" runat="server" Visible="true">
        <div>
            <asp:RadioButtonList ID="rblConfigs" runat="server">
                <asp:ListItem Text="PriceHistory1.xml" Value="~/Configuration/Charts/PriceHistory1.xml"></asp:ListItem>
                <asp:ListItem Text="TelemetryData1.xml" Value="~/Configuration/Charts/TelemetryData1.xml"></asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div>
            <asp:Button ID="btnSubmitImageXML" runat="server" Text="Submit XML" OnClick="btnSubmitImageXML_Click" />
        </div>
    </asp:Panel>
    </div>
    </form>
</body>
</html>
