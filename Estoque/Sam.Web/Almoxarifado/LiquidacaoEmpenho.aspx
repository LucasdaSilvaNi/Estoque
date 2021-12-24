<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="LiquidacaoEmpenho.aspx.cs" Inherits="Sam.Web.Almoxarifado.LiquidacaoEmpenho" Title="Módulo Tabelas :: Almoxarifado :: Liquidação de Empenho" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/Loading.ascx" TagName="Loading" TagPrefix="uc4" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="wsSenha" TagPrefix="ucAcessoSIAFEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>Módulo Tabelas - Liquidação - Empenhos Pendentes</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div style="margin-bottom: 20px; margin-top: 20px">
                    <div class="formulario">
                        <fieldset class="fieldset">
                            <p>
                                <asp:Label ID="lblUGE" runat="server" class="labelFormulario" Width="40px"  Text="UGE*:" Font-Bold="true" />
                                <asp:Label ID="lblUGEDescricao" runat="server" Width="1000px" />
                            </p>
                            <p>
                                <asp:Label ID="lblCodigoEmpenhoInf" runat="server" CssClass="labelFormulario" Width="120px" Text="Empenho:" Font-Bold="True" />
                                <asp:Label ID="lblCodigoEmpenho" runat="server" Width="1000px" Text="" />
                            </p>
<%--                            <p style="float: left; clear: none; width: 30%;">
                                <asp:Label ID="lblValorDocumentoInf" runat="server" CssClass="labelFormulario" Width="40px" Text="Valor:" Font-Bold="true" />
                                <asp:Label ID="lblValorDocumento" runat="server" Text=" " />
                            </p>--%>
                            <p style="float: left; clear: none; width: 25%;">
                                <asp:Label ID="lblTipoEmpenhoInf" runat="server" CssClass="labelFormulario" Width="120px" Text="Tipo:" Font-Bold="true" />
                                <asp:Label ID="lblTipoEmpenho" runat="server" Text=" " />
                            </p>
                            <p style="float: left; clear: none; width: 45%;">
                                <asp:Label ID="lblNaturezaDespesaInf" runat="server" CssClass="labelFormulario" Width="120px" Text="Natureza Despesa:" Font-Bold="true" />
                                <asp:Label ID="lblNaturezaDespesa" runat="server" Text=" " />
                            </p>
                        </fieldset>
                    </div>
                    <asp:GridView ID="gdvMovimentosEmpenho" runat="server" AutoGenerateColumns="False" AllowPaging="false" PageSize="99999" OnRowDataBound="gdvMovimentosEmpenho_RowDataBound">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <HeaderStyle CssClass="corpo" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkCommand" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                    <asp:Label ID="lblMovimentoId" Text='<%# Bind("Id") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="N.º Agrupamento" Visible="false" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumeroEmpenho" Text='<%# Bind("Empenho") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Documento SAM" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumeroDocumento" Text='<%# Bind("NumeroDocumento") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Data Movimento" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataMovimento" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DataMovimento")) %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Observação<br>(Texto limitado a 77 caracteres)" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="15%" >
                                <ItemTemplate>
                                    <asp:TextBox id="txtObservacoesMovimentosEmpenho" Text='<%# Eval("Observacoes") %>' TextMode="MultiLine" MaxLength="77" Rows="4" Width="250px" style="height:90%" SkinID="MultiLine" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Itens Movimento SAM" Visible="true">
                                <ItemTemplate>
                                    <asp:GridView runat="server" ID="gdvItensMovimentacao" AutoGenerateColumns="False" AllowPaging="false" PageSize="99999">
                                        <RowStyle CssClass="Left" />
                                        <AlternatingRowStyle CssClass="odd" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Item SIAFISICO" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="50%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCodigoDescricaoSubitem" Text='<%# Bind("ItemMaterial.CodigoDescricao") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Qtde. Subitem" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="25%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQtdeSubitemMovimento"  Text='<%# Eval("QtdeLiq") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Valor Subitem" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="25%">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblValorSubitemMovimento"  Text='<%# string.Format("R$ {0:0,0.00}",Eval("ValorMov")) %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Valor Movimentação" Visible="true" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblValorTotalGrupoParaPagar"  Text="[lblValorTotalGrupoParaPagar]" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NL Liquidação" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNL_Liquidacao" Text='2015NL00000' runat="server" />
                                    </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NL Incorporacao / <br>Reclassificação" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Width="15%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNL_Incorporacao" Text='2015NL00000' runat="server" />
                                    </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <p class="botaoRight" id="divVlTotalEmpenho" runat="server">
                        <asp:Label ID="lblValorTotalMovimento" Text="Valor Total (R$): " CssClass="labelFormulario" runat="server" />
                        <asp:TextBox ID="txtValorTotalMovimento" runat="server" Enabled="False" />
                    </p>
                </div>
                
                <div id="DivBotoes" class="DivButton">
                    <p>
                        <asp:Button ID="btnPagarEmpenhoSiafisico" style="width: 275px !important"  runat="server" Text="Liquidar Empenho" OnClick="btnPagarEmpenhoSiafisico_Click" Enabled="false" Visible="false" />
                        <asp:Button ID="btnEstornarPagamentoEmpenhoSiafisico" style="width: 275px !important"  runat="server" Text="Estornar NL Liquidação Empenho" OnClick="btnEstornarPagamentoEmpenhoSiafisico_Click" Enabled="false" Visible="false" />
                    </p>
                    <p>
                        <asp:Button ID="btnPagarEmpenhoSiafem" style="width: 275px!important"  runat="server" Text="Incorporar/Reclassificar" OnClick="btnPagarEmpenhoSiafem_Click" Enabled="false" Visible="false" />
                        <asp:Button ID="btnEstornarPagamentoEmpenhoSiafem" style="width: 275px !important" runat="server" Text="Estornar NL Incorporação/Reclassificação" OnClick="btnEstornarPagamentoEmpenhoSiafem_Click" Enabled="false" Visible="false" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnVoltar" runat="server" PostBackUrl="~/Almoxarifado/ConsultarLiquidacaoEmpenho.aspx" Text="Voltar" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                </asp:Panel>
                
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
            <ucAcessoSIAFEM:WSSenha runat="server" ID="ucAcessoSIAFEM" />
        </div>
        <script type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
                if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                    args.set_errorHandled(true);
                }
            }); 
    </script>
    </div>
</asp:Content>
