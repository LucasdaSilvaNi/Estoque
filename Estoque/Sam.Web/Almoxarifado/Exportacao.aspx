<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Exportacao.aspx.cs"
    EnableEventValidation="true" Inherits="Sam.Web.Almoxarifado.Exportacao" MasterPageFile="~/MasterPage/PrincipalFull.Master" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaFornecedor.ascx" TagName="ListFornecedor"
    TagPrefix="uc2" %>
<%@ Register Src="~/Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova"
    TagPrefix="uc3" %>
<%@ Register Src="../Controles/Loading.ascx" TagName="Loading" TagPrefix="uc4" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="WSSenha" TagPrefix="uc5" %>
<%@ Register Src="../Controles/PesquisaDocumentoNova.ascx" TagName="PesquisaDocumento"
    TagPrefix="uc6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <noscript>
        <p>
            O javascript de seu navegador está desabilitado. Favor habilitá-lo para acessar
            as funcionalidades deste site.</p>
    </noscript>
    <!-- Habilitar JavaScript -->
    <div id="content">
        <center>
            <h1>
                Exportação</h1>
        </center>
        <asp:UpdatePanel ID="updPanel" runat="server">
            <ContentTemplate>
                <div class="formulario">
                    <asp:GridView ID="grid" runat="server" Visible="false" />
                    <fieldset class="fieldset">
                        <asp:UpdatePanel runat="server" ID="pnlCombosEscolha" Style="text-align: left; margin: 10px">
                            <ContentTemplate>
                                <asp:Label runat="server" ID="lblSelecionaTipoExportacao" Text="Exportação:" />
                                <asp:DropDownList ID="ddlTipoExportacao" runat="server" OnSelectedIndexChanged="ddlTipoExportacao_SelectedIndexChanged"
                                    AutoPostBack="True">
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <br />
                        <asp:UpdatePanel runat="server" ID="pnlExportacaoAnalitica" Visible="false" Style="text-align: left;
                            margin: 10px">
                            <ContentTemplate>
                                <div id="divPeriodoExportacaoAnalitica" runat="server">
                                    <asp:Label ID="lblPeriodoDeExpAnalitica" runat="server" Text="Período de:" />
                                    <asp:DropDownList ID="ddlPeriodoDeExpAnalitica" runat="server" Width="120px" AutoPostBack="True"
                                        OnSelectedIndexChanged="ddlPeriodoDeExpAnalitica_SelectedIndexChanged">
                                    </asp:DropDownList>
                                    <asp:Label ID="lblPeriodoAteExpAnalitica" runat="server" Text=" até: "></asp:Label>
                                    <asp:DropDownList ID="ddlPeriodoAteExpAnalitica" Width="120px" runat="server">
                                    </asp:DropDownList>
                                </div>
                                <br />
                                <br />
                                <div runat="server" id="divCamposExportacaoAnalitica">
                                    <h5>
                                        Selecione os campos que irão para a planilha</h5>
                                    <table>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaTodosAnalitica" runat="server" Font-Overline="False"
                                                    Font-Strikeout="False" Text="Selecionar todos os campos" AutoPostBack="True"
                                                    OnCheckedChanged="chkExportaTodosAnalitica_CheckedChanged" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNLConsumoAnalitica" runat="server" Text="NL Consumo" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNLLiquidacaoAnalitica" runat="server" Text="NL Liquidação" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNLLiquidacaoEstornoAnalitica" runat="server" Text="NL Liquidação Estorno" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUgeCodigoAnalitica" runat="server" Text="UGE Código" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUgeDescricaoAnalitica" runat="server" Text="UGE Descrição" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaDataMovimentoAnalitica" runat="server" Text="Data Movimento" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNaturezaDespesaCodigoAnalitica" runat="server" Text="Nat. Despesa Código" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNaturezaDespesaDescricaoAnalitica" runat="server" Text="Nat. Despesa Descrição" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUnidFornecimentoAnalitica" runat="server" Text="Unid. Fornecimento" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExpotaSubItemCodigoAnalitica" runat="server" Text="Subitem Código" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaSubItemDescricaoAnalitica" runat="server" Text="Subitem Descrição" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaTipoMovimentoDescricaoAnalitica" runat="server" Text="Tipo Movimento Descrição" />
                                            </td>
                                            <asp:CheckBox ID="chkExportaSituacaoDescricaoAnalitica" Visible="false" runat="server"
                                                Text="Situação" />
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaPeriodoAnalitica" runat="server" Font-Overline="False"
                                                    Font-Strikeout="False" Text="Período" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaFornecedorDescricaoAnalitica" runat="server" Text="Fornecedor" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaNumeroDocumentoAnalitica" runat="server" Text="Número do Documento" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaLoteAnalitica" runat="server" Text="Lote Descrição" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaVencimentoDocAnalitica" runat="server" Text="Vencimento Subitem (Lote)" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaQtdeDocAnalitica" runat="server" Text="Quantidade Documento" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaPrecoUnitarioEMPAnalitica" runat="server" Text="Preço Unitário Empenho" />
                                            </td>

                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaValorUnitarioAnalitica" runat="server" Text="Preço Médio" />
                                            </td>

                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaDesdobroAnalitica" runat="server" Text="Desdobro" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaTotalDocAnalitica" runat="server" Text="Total Documento" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaQtdeSaldoAnalitica" runat="server" Text="Saldo Quantidade" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaValorSaldoAnalitica" runat="server" Text="Saldo Valor" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUaCodigoRequisicao" runat="server" Text="UA Código (Requisição)" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUaDescricaoRequisicao" runat="server" Text="UA Descrição (Requisição)" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaDivisao" runat="server" Text="Divisão" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUGECodigoDestino" runat="server" Text="UGE Código (Destino)" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaUGEDescricaoDestino" runat="server" Text="UGE Descrição (Destino)" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkExportaObservacoesMovimentoAnalitica" runat="server" Text="Inf. Complementares" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="btnFiltrosAnalitica" runat="server" Visible="false" Text="Filtros"
                                                    OnClick="btnFiltrosAnalitica_Click" CssClass="button" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divCamposFiltrosAnalitica" visible="true">
                                    <h5 style="margin-top: 10px">
                                        Selecione os filtros da exportação</h5>
                                    <table>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroSelecionaTodosAnalitica" runat="server" Font-Overline="False"
                                                    Font-Strikeout="False" Text="Selecionar todos os filtros" AutoPostBack="True"
                                                    OnCheckedChanged="chkFiltroSelecionaTodosAnalitica_CheckedChanged" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:CheckBox ID="chkFiltroNLConsumoAnalitica" runat="server" Text="NL Consumo" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkFiltroNLLiquidacao" runat="server" Text="NL Liquidação" />
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkFiltroNLLiquidacaoEstorno" runat="server" Text="NL Liquidação Estorno" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroFornecedorAnalitica" runat="server" Text="Fornecedor" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroLoteAnalitica" runat="server" Text="Lote Descrição" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroNaturezaDespesaAnalitica" runat="server" Text="Natureza de Despesa" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroNumeroDocumentoAnalitica" runat="server" Text="Número do Documento" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroSaldoQuantidadeAnalitica" runat="server" Text="Saldo Quantidade" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroSaldoValorAnalitica" runat="server" Text="Saldo Valor" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroUGEAnalitica" runat="server" Text="UGE" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroSubitemAnalitica" runat="server" Text="Subitem" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroTipoMovimentoAnalitica" runat="server" Text="Tipo Movimento" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroUaCodigoDestino" runat="server" Text="UA Código (Destino)" />
                                            </td>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroUGECodigoDestino" runat="server" Text="UGE Código (Destino)" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="text-align: left;">
                                                <asp:CheckBox ID="chkFiltroSituacaoAnalitica" Visible="false" runat="server" Text="Situação" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="padding-top: 10px">
                                                <asp:Button ID="btnGerarFiltrosAnalitica" runat="server" Text="Gerar Filtros" OnClick="btnGerarFiltros_Click"
                                                    CssClass="button" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <asp:TextBox ID="txtCodFornecedor" runat="server" Visible="false" Width="70%" />
                                <div runat="server" id="divValorFiltroFornecedorAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Text="Fornecedor:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroFornecedorAnalitica" runat="server" Width="70%" Enabled="false"></asp:TextBox>
                                                <asp:ImageButton ID="ImgLupaFornecedor" runat="server" CommandName="Select" CssClass="basic"
                                                    ImageUrl="../Imagens/lupa.png" ClientIDMode="Predictable" OnClientClick="OpenModal();"
                                                    ToolTip="Pesquisar Fornecedor" OnClick="ImgLupaFornecedor_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroNLConsumo" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label29" runat="server" CssClass="labelFormulario" Text="NL Consumo:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroNLConsumoAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroNLLiquidacao" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label30" runat="server" CssClass="labelFormulario" Text="NL Liquidação:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroNLLiquidacaoAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroNLLiquidacaoEstorno" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label31" runat="server" CssClass="labelFormulario" Text="NL Liquidação Estorno:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroNLLiquidacaoEstornoAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroloteAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Text="Nro. Lote:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroLoteDescricaoAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Text="Vencimento Lote:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroLoteDataAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroNaturezaDespesaAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label3" CssClass="labelFormulario" runat="server" Text="Natureza de Despesa:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroNaturezaDespesaAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroNumeroDocumentoAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label4" CssClass="labelFormulario" runat="server" Text="Nro. Documento:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroNumeroDocumentoAnalitica" runat="server" Width="80%" />
                                                <asp:ImageButton ID="imgLupaDocumento" runat="server" CommandName="Select" ImageUrl="../Imagens/lupa.png"
                                                    CssClass="basic" ClientIDMode="Predictable" OnClientClick="OpenModalDoc();" ToolTip="Pesquisar"
                                                    Visible="False" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroSaldoQuantidadeAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label5" CssClass="labelFormulario" runat="server" Text="Saldo Qtde.:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroSaldoQuantidadeAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroSaldoValorAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label6" CssClass="labelFormulario" runat="server" Text="Saldo Valor:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:TextBox ID="txtValorFiltroSaldoValorAnalitica" runat="server" Width="80%" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroSituacaoAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label7" CssClass="labelFormulario" runat="server" Text="Situação:" />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:DropDownList ID="ddlValorFiltroSituacaoAnalitica" runat="server" Width="80%">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroTipoMovimentoAnalitica" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label9" CssClass="labelFormulario" runat="server" Text="Tipo Movimento:" />
                                            </td>
                                            <td style="width: 400px">
                                                <p>
                                                    <asp:CheckBox ID="chkValorFiltroTMEntrada" Text="Entrada" runat="server" AutoPostBack="True"
                                                        OnCheckedChanged="chkValorFiltroTMEntrada_CheckedChanged" /></p>
                                                <div runat="server" id="divValorFiltroTMTodasEntradas" visible="false" style="margin-left: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMEEmpenho" Text="Entrada por Empenho" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMEAvulsa" Text="Entrada Avulsa" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMETransferencia" Text="Entrada por Transferência"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMEDoacao" Text="Entrada por Doação" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMEDoacaoOrgaoImplantado" Text="Doação por Orgão Implantado" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMEDevolucao" Text="Entrada por Devolução" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMETransformado" Text="Material Transformado" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <p>
                                                    <asp:CheckBox ID="chkValorFiltroTMSaida" Text="Saída" runat="server" AutoPostBack="True"
                                                        OnCheckedChanged="chkValorFiltroTMSaida_CheckedChanged" />
                                                </p>
                                                <div runat="server" id="divValorFiltroTMTodasSaidas" visible="false" style="margin-left: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMSAprovada" Text="Requisição Aprovada" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMSTransferencia" Text="Saída por Transferência"
                                                                    runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMSDoacao" Text="Saída por Doação" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMSOutros" Text="Outras Saídas" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <p>
                                                    <asp:CheckBox ID="chkValorFiltroTMRequisicao" Text="Requisição" runat="server" AutoPostBack="True"
                                                        OnCheckedChanged="chkValorFiltroTMRequisicao_CheckedChanged" /></p>
                                                <div runat="server" id="divValorFiltroTMTodasRequisicao" visible="false" style="margin-left: 10px;">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMRPendente" Text="Requisição Pendente" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMRFinalizada" Text="Requisição Finalizada" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:CheckBox ID="chkValorFiltroTMRCancelada" Text="Requisição Cancelada" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel runat="server" ID="pnlExportacaoSintetico" Visible="false">
                            <ContentTemplate>
                                <div runat="server" id="divCamposExportacaoSintetico">
                                    <h4 style="margin: 10px; text-align: left;">
                                        Selecione os campos que irão para a planilha</h4>
                                    <table style="width: 100%">
                                        <tr class="table-Exportacao-row">
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkTodosExportaSintetico" runat="server" AutoPostBack="True" OnCheckedChanged="chkTodosExportaSintetico_CheckedChanged"
                                                    CssClass="check-Exportacao" />
                                                <asp:Label ID="Label13" runat="server" Text="Selecionar todos os campos" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr class="table-Exportacao-row">
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkUgeCodigoExportaSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label14" runat="server" Text="UGE Código" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkUgeDescricaoExportaSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label15" runat="server" Text="UGE Descrição" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkGrupoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label16" runat="server" Text="Grupo Material" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr class="table-Exportacao-row">
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkSubitemCodigoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label17" runat="server" Text="Subitem Código" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkSubitemDescricaoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label18" runat="server" Text="Subitem Descrição" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkUnidadeFornecExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label19" runat="server" Text="Unidade Fornecimento" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr class="table-Exportacao-row">
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkSaldoQtdeExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label20" runat="server" Text="Saldo Quantidade" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkSaldoValorExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label21" runat="server" Text="Saldo Valor" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkPrecoMedioExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label22" runat="server" Text="Preço Médio" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkLoteDescricaoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label25" runat="server" Text="Lote Descrição" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkLoteQtdeExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label26" runat="server" Text="Lote Quantidade" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkLoteDataVencExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label27" runat="server" Text="Lote Data Vencimento" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr class="table-Exportacao-row">
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkNDCodigoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label23" runat="server" Text="Nat. Despesa Código" CssClass="check-Exportacao" />
                                            </td>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkNDDescricaoExportacaoSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="Label24" runat="server" Text="Nat. Despesa Descrição" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divFiltrosExportacaoSintetico" visible="true">
                                    <h4 style="margin: 10px; text-align: left;">
                                        Selecione os filtros da exportação</h4>
                                    <table style="width: 100%">
                                        <tr>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkUgeFiltroExportaSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="lblchkUgeFiltroExportaSintetico" runat="server" Text="UGE" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkGrupoFiltroExportaSintetico" runat="server" CssClass="check-Exportacao" />
                                                <asp:Label ID="lblchkGrupoFiltroExportaSintetico" runat="server" Text="Grupo Material"
                                                    CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="table-Exportacao-td">
                                                <asp:CheckBox ID="chkComSemSaldo" runat="server" CssClass="check-Exportacao" Checked="true" />
                                                <asp:Label ID="Label28" runat="server" Text="Exibir subitens sem saldo" CssClass="check-Exportacao" />
                                            </td>
                                        </tr>
                                        <tr class="table-Exportacao-row" style="padding-top: 15px">
                                            <td class="table-Exportacao-td">
                                                <asp:Button ID="btnExibirFiltrosSintetico" runat="server" Text="Gerar Filtros" OnClick="btnGerarFiltros_Click"
                                                    CssClass="button" Width="150px" />
                                        </tr>
                                    </table>
                                </div>
                                <div runat="server" id="divValorFiltroGrupoExportaSintetico" visible="false" style="margin-left: 10px;">
                                    <table>
                                        <tr>
                                            <td style="width: 140px">
                                                <asp:Label ID="Label12" CssClass="labelFormulario" runat="server" Text="Grupo: " />
                                            </td>
                                            <td style="width: 400px">
                                                <asp:DropDownList ID="ddlGrupoFiltroExportaSintetico" runat="server" Width="80%">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="divfiltrosComuns">
                            <div runat="server" id="divValorFiltroSubitem" visible="false" style="margin-left: 10px;">
                                <table>
                                    <tr>
                                        <td style="width: 140px">
                                            <asp:Label ID="Label8" CssClass="labelFormulario" runat="server" Text="Subitem:" />
                                        </td>
                                        <td style="width: 400px">
                                            <asp:TextBox ID="txtSubItem" class="txtSubItem" runat="server" Width="70%" onblur="return preencheZeros(this,'12')"
                                                onkeypress="return SomenteNumero(event)" />
                                            <asp:ImageButton ID="imgSubItemMaterial" ClientIDMode="Static" ImageUrl="../Imagens/lupa.png"
                                                OnClientClick="OpenModalItem();" CssClass="basic" ToolTip="Pesquisar" runat="server"
                                                OnClick="imgSubItemMaterial_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div runat="server" id="divValorFiltroUGE" visible="false" style="margin-left: 10px;">
                                <table>
                                    <tr>
                                        <td colspan="2" rowspan="2" style="width: 600px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="LabelUgeDisponivel" runat="server" Text="UGEs Disponíveis"></asp:Label>
                                                        <asp:ListBox ID="lstUGEAnalitica" runat="server" Height="200px" Width="450px"></asp:ListBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnSelecionar" Text=">" runat="server" OnClick="btnSelecionar_Click" />
                                                        <asp:Button ID="btnSelecionarTodasUges" Text=">>" runat="server" Visible="false" />
                                                        <asp:Button ID="btnExcluir" Text="<" runat="server" OnClick="btnExcluir_Click" />
                                                        <asp:Button ID="btnExcluirTodos" Text="<<" runat="server" Visible="false" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label11" runat="server" Text="UGEs Selecionadas "></asp:Label>
                                                        <asp:ListBox ID="lstValorFiltroUGEAnalitica" runat="server" Height="200px" Width="450px">
                                                        </asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtValorFiltroUGEAnalitica" runat="server" Width="50%" Enabled="false"
                                                Visible="False" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                         <div runat="server" id="divValorFiltroUACodigo" visible="false" style="margin-left: 10px;">

                                <asp:UpdatePanel runat="server" ID="UpdatePanelComboUGEDestino" Style="text-align: left; margin: 10px">
                                    <ContentTemplate>
                                        <asp:Label runat="server" ID="Label32" Text="Filtrar pela UGE:" />
                                        <asp:DropDownList ID="ddlUGEDestino" runat="server" OnSelectedIndexChanged="ddlUGEDestino_SelectedIndexChanged"
                                            AutoPostBack="True">
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <table>
                                    <tr>
                                        <td colspan="2" rowspan="2" style="width: 600px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="LabelUACodigoDestinoDisponivel" runat="server" Text="UAs Disponíveis"></asp:Label>
                                                        <asp:ListBox ID="lstUACodigoDestinoDisponivel" runat="server" Height="200px" Width="450px"></asp:ListBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnSelecionarUA" Text=">" runat="server" OnClick="btnSelecionarUA_Click" />
                                                        <asp:Button ID="btnExcluirUA" Text="<" runat="server" OnClick="btnExcluirUA_Click" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LabelUACodigoDestinoSelecionados" runat="server" Text="UAs Selecionadas "></asp:Label>
                                                        <asp:ListBox ID="lstUACodigoDestinoSelecionados" runat="server" Height="200px" Width="450px"></asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtValorFiltroUADestino" runat="server" Width="50%" Enabled="false"
                                                Visible="False" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        
                             <div runat="server" id="divValorFiltroUGEDestino" visible="false" style="margin-left: 10px;">
                                <table>
                                    <tr>
                                        <td colspan="2" rowspan="2" style="width: 600px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="LabelUgeCodigoDestinoDisponivel" runat="server" Text="UGEs Destino Disponíveis"></asp:Label>
                                                        <asp:ListBox ID="lstUGECodigoDestinoDisponivel" runat="server" Height="200px" Width="450px"></asp:ListBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnUgeDestinoSelecionar" Text=">" runat="server" OnClick="btnSelecionarUgeDestino_Click" />
                                                        <asp:Button ID="btnUgeDestinoExcluir" Text="<" runat="server" OnClick="btnExcluirUgeDestino_Click" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LabelUgeCodigoDestinoSelecionados" runat="server" Text="UGEs Destino Selecionadas "></asp:Label>
                                                        <asp:ListBox ID="lstUGECodigoDestinoSelecionados" runat="server" Height="200px" Width="450px"></asp:ListBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtValorFiltroUGEDestino" runat="server" Width="50%" Enabled="false"
                                                Visible="False" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <asp:UpdatePanel runat="server" ID="pnlExportacaoConsumoMedio" Visible="false">
                            <ContentTemplate>
                                Consumo Medio
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div style="float: right">
                           
                        </div>
                    </fieldset>
                    <div id="container_abas_consultas">
                        <div class="DivButton">
                            <p class="botaoRight">
                                <%--<asp:Button ID--%>
                                <asp:Button ID="btnAjuda" CssClass="button" runat="server" Text="Ajuda" AccessKey="A" />
                                <asp:Button ID="btnvoltar" CssClass="button" runat="server" Text="Voltar" PostBackUrl="~/Seguranca/TABMenu.aspx"
                                    AccessKey="V" />
                                 <asp:Button ID="btnExportar" runat="server" Width="250px" Text="Exportar" CssClass="button"
                                OnClick="btnExportar_Click" />
                            </p>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="dialog" title="Pesquisar Fornecedor">
            <uc2:ListFornecedor runat="server" ID="uc2ListFornecedor" />
        </div>
        <div id="dialogItem" title="Pesquisar Subitem">
            <uc3:PesquisaSubitemNova runat="server" ID="uc3SubItem" />
        </div>
    </div>
</asp:Content>
