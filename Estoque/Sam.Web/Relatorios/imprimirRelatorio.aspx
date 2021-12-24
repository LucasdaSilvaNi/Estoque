<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="imprimirRelatorio.aspx.cs" Inherits="Sam.Web.Relatorios.imprimirRelatorio" EnableSessionState="True"  %>

<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title> SAM - Sistema de Administração de Materiais </title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    
    </div>
    <rsweb:ReportViewer ID="rptViewer" runat="server" Width="100%" Height="50%" DocumentMapWidth="100%" SizeToReportContent="True">
    </rsweb:ReportViewer>
  
    </form>
</body>
</html>
