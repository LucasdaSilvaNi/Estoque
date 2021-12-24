<%@ Page Title="Módulo Financeiro :: Pendências :: Notas de Lançamentos SIAFEM" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="NotaLancamentoPendenteSIAFEM.aspx.cs" Inherits="Sam.Web.Financeiro.NotaLancamentoPendenteSIAFEM" ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="wsSenha" TagPrefix="ucAcessoSIAFEM" %>
<%@ Register Src="../Controles/EntradaCampoCE.ascx" TagName="EntradaCampoCE" TagPrefix="ucEntradaCampoCE" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
<div id="content">
        <h1>
            Módulo Tabelas - Pendências - Notas de Lançamento SIAFEM</h1>
        <asp:UpdatePanel runat="server" ID="updPanelRegistros">
        <ContentTemplate>
        <asp:GridView ID="gdvNotaLancamentoPendenteSIAFEM" runat="server" 
                AllowPaging="false" 
                OnRowDataBound="gdvNotaLancamentoPendenteSIAFEM_RowDataBound" 
                onpageindexchanging="gdvNotaLancamentoPendenteSIAFEM_PageIndexChanging" OnRowCommand="gdvNotaLancamentoPendenteSIAFEM_RowCommand" >
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>                 
                        <asp:LinkButton ID="lnkID" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' Width="2px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Documento" ItemStyle-Width="10px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNLP_DocumentoSAM" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo Nota" ItemStyle-Width="10px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNLP_TipoNotaSIAFEM" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Erro SIAFEM" ItemStyle-Width="275px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNLP_DescricaoErroSIAFEM" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="CE Informado" ItemStyle-Width="7px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNLP_CampoCE" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Data Envio SIAFEM" ItemStyle-Width="7px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblNLP_DataEnvioMsgWS" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Reenvio" ItemStyle-Width="15px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkPagarSIAFEM" runat="server" Text="Reenviar agora" CommandName="cmdEnviarComandoPagamentoNotaSIAFEM" Width="90px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="NL(s) Movimentação" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Right" >
                    <ItemTemplate>
                        <table>
                            <tr>
                                <tr align="right">
                                    <td id="tdLabelNotaLancamento" style="border-color: transparent !important"">
                                        <asp:Label runat="server" ID="lblNLP_AlteracaoNotaLancamentoMovimentacao" Visible="true" Width="130px" Style="text-align:right" />
                                    </td>
                                    <td id="tdNumeroNotaLancamento" style="border-color: transparent !important">
                                        <asp:TextBox runat="server" AutoPostBack="false" ID="txtAlteracaoNotaLancamentoMovimentacao" Visible="true" CssClass="inputFromNumero" MaxLength="5" Width="50px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="border-color: transparent !important">
                                        <asp:LinkButton ID="lnkAlteracaoNotaLancamentoMovimentacao" runat="server" Text="Inclusão Manual de NL" CommandName="cmdAlteracaoManualNotaLancamentoMovimentacao" Visible="true" />
                                    </td>
                                </tr>
                            </tr>
                            <tr>
                                <tr>
                                    <td id="tdLabelNotaReclassificacao" style="border-color: transparent !important">
                                        <asp:Label runat="server" ID="lblNLP_AlteracaoNotaReclassificacaoMovimentacao" Visible="false"  Width="130px" Style="text-align:right" />
                                    </td>
                                    <td id="tdNumeroNotaReclassificacao" style="border-color: transparent !important">
                                        <asp:TextBox runat="server" AutoPostBack="false" ID="txtAlteracaoNotaReclassificacaoMovimentacao" Visible="false" CssClass="inputFromNumero" MaxLength="5" Width="50px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" style="border-color: transparent !important">
                                        <asp:LinkButton ID="lnkAlteracaoNotaReclassificacaoMovimentacao" runat="server" Text="Inclusão Manual de NL" CommandName="cmdAlteracaoManualNotaReclassificacaoMovimentacao" Visible="false" />
                                    </td>
                                </tr>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
        </ContentTemplate>
        </asp:UpdatePanel>
        <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
            <ucAcessoSIAFEM:WSSenha runat="server" ID="ucAcessoSIAFEM" />
        </div>
        <div id="dialogEntradaCampoCE" title="Valor Campo CE (SIAFEM)">
            <ucEntradaCampoCE:EntradaCampoCE runat="server" ID="ucEntradaCampoCE" />
        </div>
    </div>
</asp:Content>
