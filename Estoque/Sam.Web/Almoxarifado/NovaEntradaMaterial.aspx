<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="NovaEntradaMaterial.aspx.cs" Inherits="Sam.Web.Almoxarifado.NovaEntradaMaterial"
    EnableEventValidation="true" Title="Módulo Tabelas :: Almoxarifado :: Entrada de Materiais" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaFornecedor.ascx" TagName="ListFornecedor"
    TagPrefix="uc2" %>
<%@ Register Src="../Controles/PesquisaSubitem.ascx" TagName="ListSubItemMaterial"
    TagPrefix="uc3" %>
<%@ Register Src="../Controles/Loading.ascx" TagName="Loading" TagPrefix="uc4" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="WSSenha" TagPrefix="uc5" %>
<%@ Register Src="../Controles/PesquisaDocumento.ascx" TagName="PesquisaDocumento"
    TagPrefix="uc6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <script type="text/javascript">

        function calcularPrecoUnitario() {

            var qtd = document.getElementById("<%=txtQtdeMov.ClientID%>").value;
            var precoUnit = document.getElementById("<%=txtPrecoUnit.ClientID%>").value;
            var valTotal = document.getElementById("<%=txtValorMovItem.ClientID%>").value;            
            var nomeee = "";

            var RB1 = document.getElementById("<%=rdoTipoMovimento.ClientID%>");
            var radio = RB1.getElementsByTagName("input");
            var isChecked = false;
            for (var i = 0; i < radio.length; i++) {
                if (radio[i].checked) {
                    nomeee = radio[i].value;
                    break;
                }
            }

            if (valTotal != null) {
                valTotal = valTotal.replace(",", ".");
                if (qtd == "")
                    qtd = 0.0;
                if (nomeee != "2") {
                    precoUnit = (Number(valTotal) / Number(qtd)).toString().replace(".", ",");
                    if (qtd == 0.0)
                        precoUnit = 0.00;
                    $('#<%= txtPrecoUnit.ClientID %>').val(precoUnit);
                }
                else {
                    precoUnit = precoUnit.replace(",", ".");
                    valTotal = (Number(precoUnit) * Number(qtd)).toString().replace(".", ",");
                    document.getElementById('<%= txtValorMovItem.ClientID %>').value = valTotal;
                }
            }
        }
    </script>
    <div id="content">
        <h1>
            Módulo - Almoxarifado - Entrada de Materiais</h1>
       <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
            <h3>Selecione o Tipo do Movimento:</h3>
            <div class="DivTipoMovimento">
                    <asp:RadioButtonList ID="rdoTipoMovimento" runat="server" AutoPostBack="true" DataTextField="Descricao"
                        DataValueField="Id" Height="16px" OnSelectedIndexChanged="rdoTipoMovimento_SelectedIndexChanged" 
                        Width="100%" RepeatDirection="Horizontal">
                    </asp:RadioButtonList>                    
             </div>
            <asp:Panel ID="pnlSideRight" runat="server" BorderWidth="0" BorderStyle="Dotted" Enabled="false"  Visible="false">
                        <fieldset class="fieldset">
                            <p>
                                <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="120px" Text="UGE*:"
                                    Font-Bold="true" />
                                <asp:DropDownList runat="server" ID="ddlUGE" Width="80%" AutoPostBack="True" DataTextField="Descricao"
                                    DataValueField="Id" />
                                <asp:TextBox ID="txtCodFornecedor" runat="server" Visible="false" Width="80%" />
                            </p>
                            <asp:HiddenField runat="server" ID="hdfMovimentoId" />
                            <asp:HiddenField runat="server" ID="hdfMovimentoIdTransf" />
                            <p id="idEmpenho" runat="server">
                                <asp:Label ID="lblEmpenho" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Empenho:" Font-Bold="True" />
                                <asp:TextBox ID="txtEmpenho" runat="server" MaxLength="11" Width="135px" />
                                <asp:DropDownList ID="ddlEmpenho" runat="server" Width="150px" CssClass="esconderControle"
                                    AutoPostBack="True" OnSelectedIndexChanged="ddlEmpenho_SelectedIndexChanged" />
                                <asp:Button ID="btnListarEmpenhos" runat="server" Text="Carregar" Width="80px" CssClass=""
                                    OnClick="btnCarregarEmpenho_Click"  />
                            </p>
                            <p id="idCodigoEventoEmpenho" runat="server">
                                <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="120px" Text="Licitação:"
                                    Font-Bold="True" />
                                <asp:DropDownList ID="ddlEmpenhoEvento" runat="server" Width="80%" DataTextField="CodigoDescricao"
                                    DataValueField="Id" Enabled="false" />
                            </p>
                            <p id="idFornecedor" runat="server">
                                <asp:Label ID="lblFornecedor" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Fornecedor:" Font-Bold="True" />
                                <asp:TextBox ID="txtGeradorDescricao" runat="server" Width="79%" Enabled="false"></asp:TextBox>
                                <asp:ImageButton ID="imgLupaFornecedor" runat="server" CommandName="Select" CssClass="basic"
                                    ImageUrl="../Imagens/lupa.png" ClientIDMode="Predictable" OnClick="imgLupaFornecedor_Click"
                                    OnClientClick="OpenModal();" ToolTip="Pesquisar Fornecedor" />
                            </p>
                            <p id="idTransfer" runat="server" class="esconderControle">
                                <asp:Label ID="lblAlmoxTransfer" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Almoxarifado:" Font-Bold="True" />
                                <asp:DropDownList ID="ddlAlmoxarifadoTransfer" runat="server" Width="80%" DataTextField="Descricao"
                                    DataValueField="Id" CssClass="esconderControle" AutoPostBack="true" />
                            </p>
                            <p id="idDevolucao" runat="server" class="esconderControle">
                                <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Divisão:" Font-Bold="True" />
                                <asp:DropDownList ID="ddlDivisao" runat="server" Width="80%" DataTextField="Descricao"
                                    DataValueField="Id" Visible="false" />
                            </p>
                            <p id="idDocumento">
                                <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="120px" Text="Documento:"
                                    Font-Bold="true" />
                                <asp:Label ID="lblDocumentoAvulsoAnoMov" runat="server" Text="" />
                                <asp:TextBox ID="txtDocumentoAvulso" runat="server" MaxLength="8" CssClass="inputFromNumero"
                                    Width="95px" />
                                <asp:ImageButton ID="imgLupaRequisicao" runat="server" CommandName="Select" ImageUrl="../Imagens/lupa.png"
                                    CssClass="basic" ClientIDMode="Predictable" OnClick="imgLuparequisicao_Click"
                                    OnClientClick="OpenModalDoc();" ToolTip="Pesquisar" />
                            </p>
                            <p>
                                <asp:Label ID="lblDataEmissao" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Data Emissão:" Font-Bold="true" />
                                <asp:TextBox ID="txtDataEmissao" runat="server" CssClass="dataFormat" MaxLength="10"
                                    Width="133px" />
                            </p>
                            <p>
                                <asp:Label ID="lblDataReceb" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Data Receb.:" Font-Bold="true" />
                                <asp:TextBox ID="txtDataReceb" runat="server" CssClass="dataFormat" MaxLength="10"
                                    Width="133px" />
                            </p>                            
                            <p>
                                <asp:Label ID="lblObservacoes" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Observações:" Font-Bold="true" />
                                <asp:TextBox ID="txtObservacoes" runat="server" Width="80%" Rows="4" TextMode="MultiLine"
                                    SkinID="MultiLine" />
                            </p>
                            <p>
                                <asp:Label ID="lblValorTotal" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Valor Total:" Font-Bold="True" />
                                    <asp:Label runat="server" ID="lblValor"  Width="133px" ForeColor="Red"></asp:Label>
                                <asp:TextBox ID="txtValorTotal" runat="server" Enabled="false" CssClass="numerico" Visible="false"
                                    onkeypress='return SomenteNumeroDecimal(event)' MaxLength="10" Width="133px" />
                            </p>
                        </fieldset>
                    </asp:Panel>
            <asp:Panel ID="pnlGrid" runat="server">
            <br />
                    <asp:Repeater runat="server" ID="gridSubItemMaterial" OnItemDataBound="gridSubItemMaterial_ItemDataBound"
                        OnItemCommand="gridSubItemMaterial_ItemCommand">
                        <HeaderTemplate>
                            <table width="100%" border="1" cellpadding="0" cellspacing="0" class="tabela">
                                <tr class="corpo">
                                    <th rowspan="4" align="center" width="1%" id="hdrPosicao" runat="server">
                                        #
                                    </th>
                                    <th rowspan="2" width="8%">
                                        Item
                                    </th>
                                    <th rowspan="2" width="8%">
                                        Subitem
                                    </th>
                                    <th rowspan="2" width="4%" id="celHdrUnidSiaf" runat="server">
                                        Unid. Siafisico
                                    </th>
                                    <th rowspan="2" width="4%" id="celHdrUnidSub" runat="server">
                                        Unid. Subitem
                                    </th>
                                    <th colspan="2" width="8%" id="celHdrQtdeLiq" runat="server">
                                        Qtd. Liquidar
                                    </th>
                                    <th rowspan="2" width="6%">
                                        Qtd. Rec. Subitem
                                    </th>
                                    <th rowspan="2" width="7%">
                                        Preço Unit.
                                    </th>
                                    <th rowspan="2" width="7%" id="hdrPrecoTotal" runat="server">
                                        Vl. Total
                                    </th>
                                    <th rowspan="4" width="5%" id="hdrEdit" runat="server">
                                        Editar
                                    </th>
                                </tr>
                                <tr class="corpo">
                                    <th width="4%" id="hdr2QtdLiq1" runat="server">
                                        SIAFISICO
                                    </th>
                                    <th width="4%" id="hdr2QtdLiq2" runat="server">
                                        Entrada
                                    </th>
                                </tr>
                                <tr>
                                    <th colspan="9" align="center" id="hdrLote" runat="server">
                                        Lote
                                    </th>
                                </tr>
                                <tr>
                                    <th colspan="9" align="center" id="hdrDescricao" runat="server">
                                        Descrição
                                    </th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td rowspan="3" align="center" id="celPosicao" runat="server">
                                    <asp:Label ID="lblPosicao" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Posicao")%>' />
                                    &nbsp;
                                    <asp:Label runat="server" Visible="false" ID="movimentoItemId" Text='<%# Bind("Id") %>'></asp:Label>
                                    <div class="esconderControle">
                                        <asp:Label ID="lblMovimentoId" Text='<%# Bind("Movimento.Id") %>' runat="server" />
                                        <asp:Label ID="lblMovimentoItemId" Text='<%# Bind("Id") %>' runat="server" />
                                        <asp:Label ID="lblItemMaterialId" Text='<%# Bind("SubItemMaterial.ItemMaterial.Id") %>'
                                            runat="server" />
                                        <asp:Label ID="lblNaturezaDespesaEmpenho" Text='<%# Bind("Movimento.NaturezaDespesaEmpenho") %>'
                                            runat="server" />
                                        <asp:Label ID="lblSubItemMaterialId" Text='<%# Bind("SubItemMaterial.Id") %>' runat="server" />
                                        <asp:Label ID="lblUnidadeFornecimentoId" Text='<%# Bind("SubItemMaterial.UnidadeFornecimento.Id") %>'
                                            runat="server" />
                                        <asp:Label ID="lblQtdeLiqSiafisico" Text='<%# Bind("QtdeLiqSiafisico") %>' runat="server" />
                                    </div>
                                </td>
                                <td align="center">
                                    <asp:Label ID="lblItemMatGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.CodigoFormatado")%>' />&nbsp;
                                </td>
                                <td align="center">
                                    <asp:Label ID="lblSubItem" runat="server"><%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%></asp:Label>
                                    &nbsp;
                                    <asp:DropDownList ID="ddlSubItemList" AutoPostBack="true" OnSelectedIndexChanged="ddlSubItemList_SelectedIndexChanged"
                                        DataTextField="CodigoFormatado" DataValueField="Id" runat="server" />
                                </td>
                                <td align="center" id="celUnidSiaf" runat="server">
                                    <asp:Label ID="lblUnidFornecimentoGridSiafisico" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UnidadeFornecimentoSiafisico")%>' />&nbsp;
                                </td>
                                <td align="center" id="celUnidSub" runat="server">
                                    <asp:Label ID="lblUnidFornecimentoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>' />&nbsp;
                                </td>
                                <td align="center" id="celQtdeLiq" runat="server">
                                    <asp:Label ID="lblQtdeLiqGridSiafisico" runat="server" Width="40px" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiqSiafisico")%>' /><br />
                                </td>
                                <td align="center" id="celQtdeLiq2" runat="server">
                                    <asp:TextBox ID="txtQtdeLiqGrid" runat="server" Width="40px" onkeypress='return SomenteNumero(event)'
                                        MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                </td>
                                <td align="center" id="celQtdeMov" runat="server">
                                    <asp:TextBox ID="txtQtdeMovGrid" BackColor="#F9F3B2" runat="server" Width="60px"
                                        MaxLength="20" onkeypress='return SomenteNumero(event)' Text=' <%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                </td>
                                <td align="center" id="celPrecoUnit" runat="server">
                                    <asp:Label ID="lblPrecoUnitGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "PrecoUnit"))%>' />
                                </td>
                                <td align="center" id="celPrecoTotal" runat="server">
                                    <asp:Label ID="lblValorMovItemGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "ValorMov"))%>' />
                                </td>
                                <td rowspan="3" align="center" id="cellEdit" runat="server">
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                </td>
                            </tr>
                            <tr id="tblLote" runat="server">
                                <td colspan="9" align="left" id="rowLote" runat="server">
                                    <table width="100%" style="font-size: 10px" cellpadding="0" border="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" Text="Fabric.: " />
                                                <asp:TextBox ID="txtFabLote" Width="80" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FabricanteLote")%>' />&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="Label8" runat="server" Text="Ident.: " />
                                                <asp:TextBox ID="txtIdLote" Width="80" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>' />&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="Label18" runat="server" Text="Vcto.: " />
                                                <asp:TextBox ID="txtVctoLote" Width="80" runat="server" CssClass="dataFormat" Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>' />&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="9" align="left" id="rowDescricao" runat="server">
                                    <asp:Label ID="lblDescricaoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Descricao")%>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="odd">
                                <td rowspan="3" align="center" id="celPosicao" runat="server">
                                    <asp:Label ID="lblPosicao" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Posicao")%>' />
                                    &nbsp;
                                    <asp:Label runat="server" Visible="false" ID="movimentoItemId" Text='<%# Bind("Id") %>'></asp:Label>
                                    <div class="esconderControle">
                                        <asp:Label ID="lblMovimentoId" Text='<%# Bind("Movimento.Id") %>' runat="server" />
                                        <asp:Label ID="lblMovimentoItemId" Text='<%# Bind("Id") %>' runat="server" />
                                        <asp:Label ID="lblItemMaterialId" Text='<%# Bind("SubItemMaterial.ItemMaterial.Id") %>'
                                            runat="server" />
                                        <asp:Label ID="lblNaturezaDespesaEmpenho" Text='<%# Bind("Movimento.NaturezaDespesaEmpenho") %>'
                                            runat="server" />
                                        <asp:Label ID="lblSubItemMaterialId" Text='<%# Bind("SubItemMaterial.Id") %>' runat="server" />
                                        <asp:Label ID="lblUnidadeFornecimentoId" Text='<%# Bind("SubItemMaterial.UnidadeFornecimento.Id") %>'
                                            runat="server" />
                                        <asp:Label ID="lblQtdeLiqSiafisico" Text='<%# Bind("QtdeLiqSiafisico") %>' runat="server" />
                                    </div>
                                </td>
                                <td align="center">
                                    <asp:Label ID="lblItemMatGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.CodigoFormatado")%>' />&nbsp;
                                </td>
                                <td align="center">
                                    <asp:Label ID="lblSubItem" runat="server"><%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%></asp:Label>
                                    &nbsp;
                                    <asp:DropDownList ID="ddlSubItemList" DataTextField="CodigoFormatado" DataValueField="Id"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlSubItemList_SelectedIndexChanged"
                                        runat="server" />
                                </td>
                                <td align="center" id="celUnidSiaf" runat="server">
                                    <asp:Label ID="lblUnidFornecimentoGridSiafisico" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UnidadeFornecimentoSiafisico")%>' />&nbsp;<br />
                                </td>
                                <td align="center" id="celUnidSub" runat="server">
                                    <asp:Label ID="lblUnidFornecimentoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>' />&nbsp;
                                </td>
                                <td align="center" id="celQtdeLiq" runat="server">
                                    <asp:Label ID="lblQtdeLiqGridSiafisico" runat="server" Width="40px" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiqSiafisico")%>' /><br />
                                </td>
                                <td align="center" id="celQtdeLiq2" runat="server">
                                    <asp:TextBox ID="txtQtdeLiqGrid" runat="server" Width="40px" onkeypress='return SomenteNumero(event)'
                                        MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                </td>
                                <td align="center" id="celQtdeMov" runat="server">
                                    <asp:TextBox ID="txtQtdeMovGrid" runat="server" BackColor="#F9F3B2" Width="60px"
                                        MaxLength="20" onkeypress='return SomenteNumero(event)' Text='<%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                </td>
                                <td align="center" id="celPrecoUnit" runat="server">
                                    <asp:Label ID="lblPrecoUnitGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "PrecoUnit"))%>' />
                                </td>
                                <td align="center" id="celPrecoTotal" runat="server">
                                    <asp:Label ID="lblValorMovItemGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "ValorMov"))%>' />
                                </td>
                                <td rowspan="3" align="center" id="cellEdit" runat="server">
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                </td>
                            </tr>
                            <tr class="odd" id="tblLote" runat="server">
                                <td colspan="9" align="left" id="rowLote" runat="server">
                                    <table width="100%" style="font-size: 10px" cellpadding="0" border="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="Label6" runat="server" Text="Fabric.: " />
                                                <asp:TextBox ID="txtFabLote" Width="80" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FabricanteLote")%>' />&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="Label8" runat="server" Text="Ident.: " />
                                                <asp:TextBox ID="txtIdLote" Width="80" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>' />&nbsp;
                                            </td>
                                            <td>
                                                <asp:Label ID="Label18" runat="server" Text="Vcto.: " />
                                                <asp:TextBox ID="txtVctoLote" Width="80" runat="server" CssClass="dataFormat" Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>' />&nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr class="odd">
                                <td colspan="9" align="left" id="rowDescricao" runat="server">
                                    <asp:Label ID="lblDescricaoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Descricao")%>' />
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            <p class="botaoRight" id="divVlTotalEmpenho" runat="server" visible="false">
                    <asp:Label ID="lblValorTotalMovimento" Text="Valor Total (R$): " CssClass="labelFormulario"
                        runat="server" />
                    <asp:TextBox ID="txtValorTotalMovimento" runat="server" />
                </p>
            <p>
                 <div id="divBotaoNovo" class="DivButton" runat="server">
                <p class="botaoLeft">
                    <asp:Button ID="btnNovo" runat="server" Text="Adicionar Item" SkinID="Btn100" Width="100px"
                        OnClick="btnNovo_Click" Visible="False" />
                </p>
                </div>
                </p>
            <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                    <br />
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="Label17" runat="server" CssClass="labelFormulario" Width="120px" Text="Item:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtItemMaterial" runat="server" MaxLength="9" size="9" onkeypress='return SomenteNumero(event)'
                                        onblur="return preencheZeros(this,'9')" OnTextChanged="txtItemMaterial_TextChanged"
                                        AutoPostBack="True" Enabled="False" />
                                </p>
                                <p id="ItemDescricao" runat="server" visible="false">
                                    <asp:Label ID="Label19" runat="server" CssClass="labelFormulario" Width="120px" Text="Item Descrição:"
                                        Font-Bold="true" />
                                    <asp:Label ID="lblItemMaterialDescricao" Width="490px" runat="server" Text="" />
                                </p>
                                <p>
                                    <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="120px" Text="Subitem:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtSubItem" runat="server" MaxLength="12" size="12" onkeypress='return SomenteNumero(event)'
                                        onblur="return preencheZeros(this,'12')" Enabled="False" />
                                    <asp:ImageButton ID="imgSubItemMaterial" runat="server" CommandName="Select" ImageUrl="../Imagens/search_02.png"
                                        OnClientClick="OpenModalItem();" CssClass="basic" ToolTip="Pesquisar" />
                                    <asp:HiddenField ID="idSubItem" runat="server" />
                                </p>
                                <p>
                                    <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="120px" Text="Classif.:"
                                        Font-Bold="True" />
                                    <asp:DropDownList runat="server" ID="ddlSubItemClassif" Width="150px" AutoPostBack="True"
                                        DataTextField="Codigo" DataValueField="Id" />
                                </p>
                                <p>
                                    <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="120px" Text="Descrição:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" size="60" Width="80%"
                                        Enabled="False" />
                                </p>
                                <p id="divQtdLiq" runat="server">
                                    <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="120px" Text="Qtd. a Liquid.:"
                                        Font-Bold="True" />
                                    <asp:TextBox ID="txtQtdeLiq" runat="server" MaxLength="10" onkeypress='return SomenteNumero(event)'
                                        Width="150px" Enabled="False" />
                                </p>
                                <p>
                                    <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="120px" Text="Qtd. a Entrar:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtQtdeMov" runat="server" MaxLength="20" onkeypress='return SomenteNumero(event)'
                                        Width="150px" />
                                </p>                                
                                <p>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="120px" Text="Valor Total:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtValorMovItem" runat="server" MaxLength="20" CssClass="numerico"
                                        onkeypress='return SomenteNumeroDecimal(event)' Width="150px" />
                                </p>
                                <p>
                                    <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="120px" Text="Preço Unit.:"
                                        Font-Bold="true"/>
                                        <asp:Literal runat="server"  ID="lblPrecoUnitario"></asp:Literal>                                        
                                    <asp:TextBox ID="txtPrecoUnit" runat="server" MaxLength="20" CssClass="numerico" Enabled="false"
                                        onkeypress='return SomenteNumeroDecimal(event)' Width="150px" BackColor="#CCCCCC" />
                                </p>
                                <p>
                                    <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="120px" Text="Unidade:"
                                        Font-Bold="true" />
                                    <asp:DropDownList runat="server" ID="ddlUnidade" Width="150px" AutoPostBack="True"
                                        DataTextField="Codigo" DataValueField="Id" Enabled="False" />
                                </p>
                                <asp:Panel ID="pnlLote" runat="server" CssClass="esconderControle">
                                    <div class="formulario">
                                        <fieldset class="sideRight">
                                            <h1>
                                                Lote</h1>
                                            <p>
                                                <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="120px" Text="Fabricante:"
                                                    Font-Bold="True" />
                                                <asp:TextBox ID="txtFabricLoteItem" runat="server" MaxLength="60" Width="300px" />
                                            </p>
                                            <p>
                                                <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="120px" Text="Vencimento:"
                                                    Font-Bold="true" />
                                                <asp:TextBox ID="txtVencimentoLote" CssClass="dataFormat" runat="server" MaxLength="10"
                                                    Width="100px" />
                                            </p>
                                            <p>
                                                <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="120px" Text="Identificação:"
                                                    Font-Bold="True" />
                                                <asp:TextBox ID="txtIdentLoteItem" runat="server" MaxLength="60" Width="300px" />
                                            </p>
                                        </fieldset>
                                        <asp:HiddenField ID="hidtxtSubItemMaterialId" runat="server" />
                                        <asp:HiddenField ID="hidtxtMovimentoItemId" runat="server" />
                                        <asp:HiddenField ID="hidtxtItemMaterialId" runat="server" />
                                        <asp:HiddenField ID="hidAlmoxId" runat="server" />
                                    </div>
                                </asp:Panel>
                            </fieldset>
                        </div>
                    </div>
                    <div>
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div class="botao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravarItem" CssClass="" runat="server" Text="Adicionar" OnClick="btnGravarItem_Click" />
                                <asp:Button ID="btnExcluirItem" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                    OnClick="btnExcluirItem_Click" />
                                <asp:Button ID="btnCancelarItem" CssClass="" AccessKey="L" runat="server" Text="Cancelar"
                                    OnClick="btnCancelarItem_Click" />
                            </p>
                        </div>
                </asp:Panel>            
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Gravar" OnClick="btnGravar_Click"
                            Enabled="False" Visible="false" />
                        <asp:Button ID="btnEstornar" CssClass="" runat="server" Text="Estornar" OnClick="btnEstornar_Click"
                            Enabled="False" Visible="false" />
                        <asp:Button ID="btnCancelar" CssClass="" runat="server" Text="Cancelar" OnClick="btnCancelar_Click"
                            Enabled="False" Visible="false" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" Style="width: 150px;" Text="Nota de Recebimento"
                            OnClick="btnImprimir_Click" />
                        <asp:Button ID="btnAjuda" runat="server" Text="Ajuda" CssClass="" AccessKey="A" Enabled="true" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Almoxarifado/ConsultarEntradaMaterial.aspx"
                            AccessKey="V" />
                    </p>
                </div>
            </ContentTemplate>            
        </asp:UpdatePanel>
        <div id="dialog" title="Pesquisar Fornecedor">
            <uc2:ListFornecedor runat="server" ID="uc2ListFornecedor" />
        </div>
        <div id="dialogItem" title="Pesquisar Subitem">
            <uc3:ListSubItemMaterial runat="server" ID="uc3SubItem" />
        </div>
        <div id="dialogDoc" title="Pesquisar Documento">
            <uc6:PesquisaDocumento runat="server" ID="PesquisaDocumento" />
        </div>
        <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
            <uc5:WSSenha runat="server" ID="uc5Senha" />
        </div>
    </div>
</asp:Content>
