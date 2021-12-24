<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="RelatoriosMensais.aspx.cs" Inherits="Sam.Web.Almoxarifado.RelatoriosMensais"
    Title="Módulo Tabelas :: Almoxarifado :: Relatórios Mensais" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <!-- Habilitar JavaScript -->
    <noscript>
        <p>
            O javascript de seu navegador está desabilitado. Favor habilitá-lo para acessar
            as funcionalidades deste site.</p>
    </noscript>
    <asp:UpdatePanel ID="updPnl" runat="server">
        <ContentTemplate>
            <!-- Conteúdo -->
            <div id="content">
                <div id="interno" class="formulario">
                    <h1>
                        Módulo Almoxarifado - Relatórios Mensais</h1>
                    <fieldset class="fieldset">
                        <p>
                        </p>
                        <p>
                            <asp:Label CssClass="labelFormulario" Text="Almoxarifado:" Style="text-align: left"
                                    Font-Bold="true" Width="100px" runat="server" />
                                <asp:DropDownList ID="ddlLstAlmox" runat="server" AutoPostBack="True" Width="80%"
                                    OnSelectedIndexChanged="ddlLstAlmox_SelectedIndexChanged" />
                        </p>
                        <p>
                            <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Style="text-align: left"
                                Width="100px" Text="Relatório:" />
                            <asp:DropDownList ID="ddlRelatorios" Width="80%" runat="server" OnSelectedIndexChanged="ddlRelatorios_SelectedIndexChanged"
                                AutoPostBack="True" />
                        </p>
                        <div class="formulario">
                            <p class="sideLeft" id="periodoDeAte">
                                <asp:Label ID="lblData" CssClass="labelFormulario" Style="text-align: left" Width="100px"
                                    runat="server" Text="Data:" />
                                <asp:DropDownList ID="ddlPeriodoDe" runat="server" Width="10%" />
                                &nbsp;<asp:DropDownList ID="ddlPeriodoAte" runat="server" Width="10%" Visible="false" />
                            </p>
                            <p class="sideLeft" id="periodoAnual">
                                <asp:Label ID="lblDataAnual" CssClass="labelFormulario" Style="text-align: left"
                                    Width="100px" runat="server" Text="Ano de referência:" Visible="false" />
                                <asp:DropDownList ID="ddlAnoRef" runat="server" Width="10%" Enabled="false" Visible="false" />
                            </p>
                        </div>
                            <p class="sideLeft">
                                <asp:Label  CssClass="labelFormulario" Text="Mostrar saldo zero:" Style="text-align: left"  Font-Bold="true" Width="100px" runat="server" />
                                <asp:CheckBox ID="chkSaldoMaiorZero" runat="server" AutoPostBack="True"  Style="text-align: left"  />
                            </p>
                    </fieldset>
                </div>
                <!-- fim id interno -->
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnCancelar" CssClass="" AccessKey="L" runat="server" Text="Cancelar"
                            CausesValidation="False" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" CssClass="" runat="server" Text="Relatório" AccessKey="R"
                            OnClick="btnImprimir_Click" />
                        <asp:Button ID="btnAjuda" CssClass="" runat="server" Text="Ajuda" AccessKey="A" />
                        <asp:Button ID="btnvoltar" CssClass="" runat="server" Text="Voltar" PostBackUrl="~/Seguranca/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
            </div>
            <!-- fim id content -->
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
