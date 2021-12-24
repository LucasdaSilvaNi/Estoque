<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" EnableEventValidation="true" CodeBehind="ConsultarLiquidacaoEmpenho.aspx.cs" Inherits="Sam.Web.Almoxarifado.ConsultarLiquidacaoEmpenho"    Title="Módulo Tabelas :: Almoxarifado :: Consultar Empenhos a Liquidar" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/Loading.ascx" TagName="Loading" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>Módulo Tabelas - Almoxarifado - Liquidação de Empenho</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div style="margin-bottom: 20px; margin-top: 20px">
                    <asp:GridView ID="gvRelacaoEmpenhos" runat="server" AutoGenerateColumns="False" AllowPaging="false" PageSize="99999" 
                        OnRowDataBound="gvRelacaoEmpenhos_RowDataBound"
                        OnRowCommand="gvRelacaoEmpenhos_RowCommand" >
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" HeaderText="MovimentoID" Visible="False">
                                <ItemTemplate>
                                    <asp:Label ID="lblMovimentoId" Text='<%# Bind("Id") %>' runat="server"  Visible="False" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Empenho" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <%--<asp:LinkButton ID="lnkCommand" runat="server" Font-Bold="true" CausesValidation="False" Text='<%# Bind("Empenho") %>' CommandName="cmdSelecionarEmpenho" CommandArgument='<%# "AlmoxID:" +  Eval("Almoxarifado.Id") + "|CodigoEmpenho:" + Eval("Empenho")+ "|MesRefEmpenho:" + Eval("AnoMesReferencia") + "|MovID:" + Eval("Id")%>' ToolTip='<%# "AlmoxID:" +  Eval("Almoxarifado.Id") + "|CodigoEmpenho:" + Eval("Empenho")+ "|MesRefEmpenho:" + Eval("AnoMesReferencia") + "|MovID:" + Eval("Id")%>' />--%>
                                    <asp:LinkButton ID="lnkCommand" runat="server" Font-Bold="true" CausesValidation="False" Text='<%# Bind("Empenho") %>' CommandName="cmdSelecionarEmpenho" CommandArgument='<%# "AlmoxID:" +  Eval("Almoxarifado.Id") + "|CodigoEmpenho:" + Eval("Empenho")+ "|MesRefEmpenho:" + Eval("AnoMesReferencia") + "|MovID:" + Eval("Id")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tipo Empenho" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescricaoTipoEmpenho" Text="###" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Documento SAM" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumeroDocumento" Text='<%# Bind("NumeroDocumento") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Natureza Despesa" Visible="true" ItemStyle-HorizontalAlign="center" HeaderStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Label ID="lblNaturezaDespesa" Text='<%# Eval("NaturezaDespesaEmpenho") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
<%--                            <asp:TemplateField HeaderText="Data Movimento" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataMovimento" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DataMovimento")) %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Valor Movimento" Visible="true" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Label ID="lblValorDocumento" Text='<%# string.Format("R$ {0:0,0.00}",Eval("ValorDocumento")) %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Liquidação" Visible="true" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="150px" >
                                <ItemTemplate>
                                    <asp:Label ID="lblNLSiafisico" Text='2015NL00000' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Incorporação /<br>Reclassificação" Visible="true" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Label ID="lblNLSiafem" Text='2015NL00000' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                </div>
                
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoRight">
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/default.aspx" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
