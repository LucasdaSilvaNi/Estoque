<%@ Page Language="C#" MasterPageFile="MasterPage/Principal.Master" AutoEventWireup="true"
    CodeBehind="TesteWebservice.aspx.cs" Inherits="Sam.Web.TesteWebservice"
    Title="Teste de Webservices SEFAZ" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
<div id="content">
        <h1>Teste de Acesso Webservice</h1>
        <br />
        <asp:Label ID="lblMsgDesenvolvimento" ForeColor="Red" Text="Webservices SEFAZ (status produção): " CssClass="listInconsistencias" runat="server" />
        <br />
        <br />
        <asp:Label ID="lblMsgHomologacao" ForeColor="Red" Text="Webservices SEFAZ (status homologação): " CssClass="listInconsistencias" runat="server" />
        <br />
        <br />
        <br />
    </div>
</asp:Content>       
