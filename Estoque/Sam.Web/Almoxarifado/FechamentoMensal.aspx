<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" EnableEventValidation="true" CodeBehind="FechamentoMensal.aspx.cs" Inherits="Sam.Web.Almoxarifado.FechamentoMensal" Title="Módulo Tabelas :: Almoxarifado :: Fechamento Mensal" %>

<%@ Register Src="~/Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="wsSenha" TagPrefix="ucAcessoSIAFEM" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        DesabilitarDuploClick();

        function AlterarTextoLinkPagamentoSiafem(linkButton, texto) {
            document.getElementById(linkButton).innerHTML = texto;
            document.getElementById(linkButton).style.textDecoration = "underline none";
        }
        function PagamentoSiafem(linkButton, texto) {
            document.getElementById(linkButton).innerHTML = texto;
            document.getElementById(linkButton).style.textDecoration = "underline underline";
        }
        function UnCheckedCheckBox(CheckBox) {
            document.getElementById(CheckBox).checked = false;
        }
    </script>
    <!-- Conteúdo -->
    <asp:UpdatePanel runat="server" ID="ajax1" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="content">
                <h1>Módulo Almoxarifado - Efetuar Fechamento Mensal</h1>

                <asp:Panel ID="pnlFechamento" runat="server" Height="500px" ScrollBars="Auto" BorderStyle="None"
                    BorderWidth="0" CssClass="esconderControle">
                    <asp:Repeater runat="server" ID="rptFechamento" OnItemDataBound="rptFechamento_ItemDataBound">
                        <HeaderTemplate>
                            <table width="100%" border="1" cellpadding="0" cellspacing="0" class="tabela">
                                <tr class="corpo">
                                    <th rowspan="2">Código
                                    </th>
                                    <th rowspan="2">Descrição
                                    </th>
                                    <th rowspan="2">UGE
                                    </th>
                                    <th colspan="2">Saldo Anterior
                                    </th>
                                    <th colspan="4">Saldo Atual
                                    </th>
                                    <th colspan="2">Fechamento
                                    </th>
                                </tr>
                                <tr class="corpo">
                                    <th>Qtde.
                                    </th>
                                    <th>Valor
                                    </th>
                                    <th>Qtde. Entrada
                                    </th>
                                    <th>Valor Entrada
                                    </th>
                                    <th>Qtde. Saída
                                    </th>
                                    <th>Valor Saída
                                    </th>
                                    <th>Qtde.
                                    </th>
                                    <th>Valor
                                    </th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblCodigo" runat="server" Text='<%#Eval("CodigoFormatado")%>' />
                                </td>
                                <td align="left" width="8%">
                                    <asp:Label ID="lblSubitemMaterialDescricao" runat="server" Text='<%#Eval("SubItemMaterialDescr")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblUGECodigo" runat="server" Text='<%#Eval("UGECodigo")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblSaldoAnterior" runat="server" Text='<%#Eval("SaldoAnterior")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblSaldoAnteriorValor" runat="server" Text='<%#Eval("SaldoAnteriorValor")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeEntrada" runat="server" Text='<%#Eval("QtdeEntrada")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorEntrada" runat="server" Text='<%#Eval("ValEntrada")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeSaida" runat="server" Text='<%#Eval("QtdeSaida")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorSaida" runat="server" Text='<%#Eval("ValSaida")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeFechamento" runat="server" Text='<%#Eval("QtdeFechamento")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorFechamento" runat="server" Text='<%#Eval("ValFechamento")%>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblCodigo" runat="server" Text='<%#Eval("CodigoFormatado")%>' />
                                </td>
                                <td align="left" width="8%">
                                    <asp:Label ID="lblSubitemMaterialDescricao" runat="server" Text='<%#Eval("SubItemMaterialDescr")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblUGECodigo" runat="server" Text='<%#Eval("UGECodigo")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblSaldoAnterior" runat="server" Text='<%#Eval("SaldoAnterior")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblSaldoAnteriorValor" runat="server" Text='<%#Eval("SaldoAnteriorValor")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeEntrada" runat="server" Text='<%#Eval("QtdeEntrada")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorEntrada" runat="server" Text='<%#Eval("ValEntrada")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeSaida" runat="server" Text='<%#Eval("QtdeSaida")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorSaida" runat="server" Text='<%#Eval("ValSaida")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblQtdeFechamento" runat="server" Text='<%#Eval("QtdeFechamento")%>' />
                                </td>
                                <td align="right" width="8%">
                                    <asp:Label ID="lblValorFechamento" runat="server" Text='<%#Eval("ValFechamento")%>' />
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
                <div>
                    <asp:GridView ID="gridConsumoUAsAlmox" runat="server" AllowPaging="False"
                        CaptionAlign="Left" AutoGenerateColumns="False"
                        OnRowCommand="gridConsumoUAsAlmox_RowCommand"
                        OnRowDataBound="gridConsumoUAsAlmox_RowDataBound"
                        ShowFooter="true"
                        FooterStyle-BorderStyle="None" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="UA" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <asp:Label ID="lblUADescricao" runat="server" Text='<%# Eval("UA.CodigoDescricao") %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="PTRes" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="lblPTRes" runat="server" Text='<%# Eval("PTRes.Codigo") %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Natureza Despesa" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNaturezaDespesa" runat="server" Text='<%# Eval("NaturezaDespesa.Codigo") %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Número Documento" ItemStyle-Width="7%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNumeroDocumentoRelacionado" runat="server" Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Valor (R$)" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="lblValorConsumo" runat="server" Text='<%# String.Format("R$ {0:#,#0.00}", Eval("Valor")) %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="NL Consumo" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblMensagemErroSIAFEM" runat="server" Text='<%# Eval("Obs") %>' Font-Bold="true" ForeColor="Red" Visible="false" />
                                    <asp:Button ID="btnPagarSIAFEM" runat="server" CssClass="simulateLinkButtonLook" Width="200px" Text='Realizar NL de Consumo' CommandName="cmdPagarSIAFEM" CommandArgument='<%# "UA:" + Eval("UA.Id") + "|UGE:" + Eval("UGE.Id") + "|PTRes:" +  Eval("PTRes.Id") + "|ND:" +  Eval("NaturezaDespesa.Id") + "|Valor:" +  Eval("Valor")+ "|AlmoxID:" +  Eval("Almoxarifado.Id")+ "|MovItemIDs:" + Eval("MovimentoItemIDs") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Estorno NL Consumo" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center">
                                <ItemTemplate>
                                    <asp:Label ID="lblMensagemErroEstornoSIAFEM" runat="server" Text='<%# Eval("Obs") %>' Font-Bold="true" ForeColor="Red" Visible="false" />
                                    <asp:CheckBox ID="chkEstornarNL" runat="server" Text="Estornar NL" Visible="false" AutoPostBack="true" OnCheckedChanged="chkEstornoNLConsumo_CheckedChanged" CommandName="cmdEstornarPagamentoSIAFEM" EventArgument='<% "NL:" + Eval(NlLancamento) %>' />
                                    <asp:Label ID="lblNLConsumoEstornada" runat="server" Visible="false"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr id="trInfoMaterialPermanente" runat="server">
                                        <td id="tdInfoMaterialPermanente" runat="server" colspan="6" align="left">
                                            <asp:Label runat="server" ID="lblInformativoMaterialPermanente" Font-Bold="true" ForeColor="Red" Text="* Material Permanente não gera NL de Consumo. Em caso de dúvidas, favor contatar CAU/SEFAZ." />
                                        </td>
                                    </tr>
                                    <tr id="trValorPendentePagamento" runat="server">
                                        <td id="tdValorPendentePagamento01" runat="server" colspan="3">
                                            <asp:Label ID="lblInformativoPendentes" Text="Valor pendente de pagamento" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorPendentePagamento02" runat="server" colspan="1">
                                            <asp:Label ID="lblValorTotalPendentes" Text="[lblValorTotalPendentes]" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorPendentePagamento03" runat="server" colspan="1">
                                            <asp:Button ID="btnPagarConsumoAlmox" runat="server" Style="width: 200px !important" Text="Gerar Todas as NL's Consumo" Visible="true" Enabled="false" CommandName='cmdPagarConsumoTotalUA' />
                                        </td>
                                        <td id="tdValorPendentePagamento04" runat="server" colspan="1">
                                            <asp:Button ID="btnEstornarConsumoAlmox" runat="server" Style="width: 200px !important" Text="Estornar Todas as NL's Consumo" Visible="true" Enabled="false" CommandName='cmdEstornarConsumoTotalUA' />
                                        </td>
                                    </tr>
                                    <tr id="trValorPagoSiafem" runat="server">
                                        <td id="tdValorPagoSiafem01" runat="server" colspan="3">
                                            <asp:Label ID="lblInformativoSiafem" Text="Valor pago ao SIAFEM" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorPagoSiafem02" runat="server" colspan="1">
                                            <asp:Label ID="lblValorTotalPagoSiafem" Text="[lblValorTotalPagoSiafem]" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorPagoSiafem03" runat="server" colspan="2" />
                                    </tr>
                                    <tr id="trValorTotalConsumo" runat="server">
                                        <td id="tdValorTotalConsumo01" runat="server" colspan="3">
                                            <asp:Label ID="lblInformativoAlmox" Text="Valor Total de Consumo do Almoxarifado" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorTotalConsumo02" runat="server" colspan="1">
                                            <asp:Label ID="lblValorTotalConsumo" Text="[lblValorTotalConsumo]" runat="server" Visible="true" />
                                        </td>
                                        <td id="tdValorTotalConsumo03" runat="server" colspan="2" />
                                    </tr>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo" />
                    </asp:GridView>
                </div>
                <div>
                    <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                </div>
                <!-- fim id interno -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnAnalise" Style="width: 90px !important" runat="server" Text="Análise" OnClick="btnAnalise_Click" />
                        <asp:Button ID="btnSimularFechamento" Style="width: 90px !important" runat="server" AccessKey="S" Text="Simulação" OnClick="btnSimularFechamento_click" />
                        <asp:Button ID="btnFechamento" Style="width: 90px !important" runat="server" AccessKey="F" Text="Fechamento" OnClick="btnFechamento_Click" />
                        <asp:Button ID="btnReabertura" Style="width: 90px !important" runat="server" AccessKey="R" Text="Reabertura" OnClick="btnReabertura_Click" />
                        <asp:Button ID="btnNLConsumo" runat="server" Style="width: 90px !important" AccessKey="R" Text="NL Consumo" OnClick="btnNLConsumo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnAjuda" runat="server" CssClass="button" AccessKey="A" Text="Ajuda" />
                        <asp:Button ID="btnSair" runat="server" CssClass="button" AccessKey="V"
                            Text="Voltar" OnClick="btnSair_Click" />
                    </p>
                    <!-- fim id content -->
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
        <ucAcessoSIAFEM:wsSenha runat="server" ID="ucAcessoSIAFEM" />
    </div>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
        || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                args.set_errorHandled(true);
            }
        });
    </script>
</asp:Content>
