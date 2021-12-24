<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    EnableEventValidation="true" CodeBehind="EntradaMaterial.aspx.cs" ValidateRequest="false" Inherits="Sam.Web.Almoxarifado.EntradaMaterial"
    Title="Módulo Tabelas :: Almoxarifado :: Entrada de Materiais" %>

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
    <link href="../CSS/estiloFull.css" rel="stylesheet" />
    <script type="text/javascript">
   
      DesabilitarDuploClick();   

        function toFixedSupportEnable() { return (2489.8237).toFixed; }
        function toFixed(num, fixed) {
                                        fixed = fixed || 0;
                                        fixed = Math.pow(10, fixed);
                                        return Math.floor(num * fixed) / fixed;
        }
        function calcularPrecoUnitario() {
            var numCasasDecimais_ValorUnitario = <%=NumeroCasasDecimais_ValorUnitario()%>;
            var numCasasDecimais_QtdeItem = <%=NumeroCasasDecimais_QtdeItem()%>;

            var qtd = document.getElementById("<%=txtQtdeMov.ClientID%>").value;
            var precoUnit = document.getElementById("<%=txtPrecoUnit.ClientID%>").value;
            var valTotal = document.getElementById("<%=txtValorMovItem.ClientID%>").value;
            var nomeee = "";
            var RB1 = document.getElementById("<%=rblTipoMovimento.ClientID%>");
            var radio = RB1.getElementsByTagName("input");
            var isChecked = false;
            for (var i = 0; i < radio.length; i++) {
                if (radio[i].checked) {
                    nomeee = radio[i].value;
                    break;
                }
            }

            if (precoUnit != null && precoUnit == "")
                precoUnit = toFixed(0, numCasasDecimais_ValorUnitario);

            if (valTotal != null) 
            {
                valTotal = valTotal.replace(",", ".");
                qtd = qtd.replace(",", ".");

                if (qtd == "")
                    qtd = toFixed(0,numCasasDecimais_QtdeItem);
                if (nomeee != "2") {
                    precoUnit = (Number(valTotal) / Number(qtd));
                    if (qtd == toFixed(0,numCasasDecimais_QtdeItem) || valTotal == toFixed(0,2))
                        precoUnit = toFixed(0,numCasasDecimais_ValorUnitario);
                    document.getElementById('<%= txtPrecoUnit.ClientID %>').value = toFixed(precoUnit,numCasasDecimais_ValorUnitario);
                }
                else {
                    precoUnit = precoUnit.replace(",", ".");
                    valTotal = (Number(precoUnit) * Number(qtd)).toString().replace(".", ",");
                    document.getElementById('<%= txtValorMovItem.ClientID %>').value = toFixed(valTotal, 2);
                }

                if (toFixedSupportEnable) {
                    if (valTotal != null && valTotal == "") valTotal = toFixed(0,5);

                    precoUnit = (Number(precoUnit)).toFixed(10);
                    valTotal = (Number(valTotal)).toFixed(2);
                    qtd = (Number(qtd)).toFixed(numCasasDecimais_QtdeItem);

                    if (precoUnit == toFixed(0,numCasasDecimais_ValorUnitario) && valTotal != toFixed(0,numCasasDecimais_ValorUnitario) && qtd != toFixed(0,numCasasDecimais_ValorUnitario)) precoUnit = (Number(0.0001)).toFixed(4);

                    document.getElementById("<%=txtQtdeMov.ClientID%>").value = qtd.replace(".",",");
                    document.getElementById("<%=txtPrecoUnit.ClientID%>").value = precoUnit.replace(".", ",");
                    document.getElementById("<%=txtValorMovItem.ClientID%>").value = valTotal.replace(".", ",");
                 } 
            }
        }

        function ShowInformation() {
            var nomeee = "";
            var RB1 = document.getElementById("<%=rblTipoMovimento.ClientID%>");
            var radio = RB1.getElementsByTagName("input");
            var isChecked = false;
            for (var i = 0; i < radio.length; i++) {
                if (radio[i].checked) {
                    nomeee = radio[i].value;
                    break;
                }
            }
            if (nomeee == "5") {
                OpenModalNotification();
            }
        }

        function FormatTextBox(obj) {
            //This is just a dummy function that adds $ as the first character.
            var value = obj.value

            if (value.length > 0 )
                value =  value;

            obj.value = value; // This line causes the autopostback not to fire. This only happens if the value of the input 
            // is set in the keyup or keydown event and only in IE.
        }


        function FormatTextBoxDirty(obj) {


            obj.dirty = false;  // This prevents firing onchange twice in the event that we don't modify the value
            if (obj.value != null || obj.value != undefined) {
                if (obj.value.length > 0 ) {
                    obj.value =  obj.value;
                    $(".txtSubItem").attr("disabled", " disabled");
                    $("#imgSubItemMaterial").attr("visible", " false");
                    obj.dirty = true;
                } else {
                    $("#txtSubItem").removeAttr();
                    $("#imgSubItemMaterial").removeAttr();

                }
            }
        }

    </script>
    <div id="content">
        <!--Alteração-->
        <asp:UpdatePanel runat="server" ID="updAjax">
            <ContentTemplate>
                <h1>
                    Módulo - Almoxarifado - Entrada de Materiais</h1>
                <div class="formulario">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 25%" valign="top">
                                <fieldset class="sideLeftEntrada1">
                                    <p>
                                        <asp:Label ID="Label20" CssClass="labelFormulario" Text="Tipo:" runat="server" />
                                    </p>
                                    <p>
                                        <asp:RadioButtonList ID="rblTipoMovimento" runat="server" AutoPostBack="true" DataTextField="Descricao"
                                            DataValueField="Id" Height="16px" OnSelectedIndexChanged="rblTipoMovimento_SelectedIndexChanged"
                                            Width="300px" onclick="ShowInformation();">
                                        </asp:RadioButtonList>
                                    </p>
                                </fieldset>
                                <fieldset class="sideLeftEntrada2" id="fldSetOperacaoEntrada" runat="server">
                                    <asp:RadioButtonList runat="server" ID="rblTipoOperacao" AutoPostBack="true" OnSelectedIndexChanged="rblTipoOperacao_SelectedIndexChanged">
                                        <asp:ListItem Text="Nova" Value="1" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Estornar" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="Nota de Recebimento" Value="3"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </fieldset>
                            </td>
                            <td style="width: 3px">
                            </td>
                            <td style="width: 74%" valign="top">
                                <asp:Panel ID="pnlSideRight" runat="server" Enabled="false">
                                    <fieldset class="sideRightEntrada">
                                        <p id="inscCE" runat ="server">
                                            <table>
                                                <tr>
                                                    <td style="padding-right: 15px">
                                                        <asp:Label ID="lblCampoInfoSiafemCE" runat="server" CssClass="labelFormulario" Width="100px"
                                                            Text="Inscrição (CE):" Style="display: inline-block; width: 100px; font-size: 12px;" />
                                                    </td>
                                                    <td style="padding-right: 10px">
                                                        <asp:TextBox ID="txtInfoSiafemCE"  runat="server" MaxLength="22" Style="float: left !important;
                                                            font-size: 12px"  />   
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="hdfInfoSiafemCEOld" runat="server" Text=""></asp:Label>
                                                        <%--<asp:HiddenField ID="hdfInfoSiafemCEOld" runat="server" />--%>
                                                </tr>
												</td>
                                            </table>
                                        </p>
                                        <p />
                                        <p runat="server" id ="divUge">
                                            <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="100px" Text="UGE*:"
                                                Font-Bold="true" />
                                            <asp:DropDownList runat="server" ID="ddlUGE" Width="70%" AutoPostBack="True" DataTextField="CodigoDescricao"
                                                DataValueField="Id"  OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged" />
                                            <asp:TextBox ID="txtCodFornecedor" runat="server" Visible="false" Width="70%" />
                                        </p>
                                        <asp:HiddenField runat="server" ID="hdfMovimentoId" />
                                        <asp:HiddenField runat="server" ID="hdfAlmoxTransId" />
                                        <asp:HiddenField runat="server" ID="hdfMovimentoIdTransf" />
                                        <p id="idEmpenho" runat="server">
                                            <asp:Label ID="lblEmpenho" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Empenho:" Font-Bold="True" />
                                            <asp:TextBox ID="txtEmpenho" runat="server" MaxLength="11" Width="135px" />
                                            <asp:DropDownList ID="ddlEmpenho" runat="server" Width="150px" CssClass="esconderControle"
                                                AutoPostBack="True" OnSelectedIndexChanged="ddlEmpenho_SelectedIndexChanged"
                                                Enabled="false" />
                                            <asp:Button ID="btnListarEmpenhos" runat="server" Text="Listar Empenhos" CssClass=""
                                                OnClick="btnListarEmpenhos_Click" Style="position: static; margin-top: -22px;
                                                width: 120px !important" />
                                        </p>
                                         <p id="idSubTipo" visible="false" runat="server" >
					                        <asp:Label ID="lblSubTipoMovimento" runat="server" CssClass="labelFormulario" Width="100px"
					                            Text="SubTipo:" Font-Bold="True" />
					                        <asp:DropDownList ID="ddlSubTipoMovimento" runat="server"  Width="70%" AutoPostBack="true"  OnSelectedIndexChanged="ddlSubTipoMovimento_SelectedIndexChanged" />
                                             <asp:HiddenField runat="server" ID="hdfNaturaSelecionada" />
					                    </p>
					                    <p id="idOrgaoTransf" visible="false" runat="server" style="padding-right: 10px">
					                        <asp:Label ID="lblOrgao" runat="server" CssClass="labelFormulario" Width="100px" Text="Orgão:"
					                            Font-Bold="True" />
					                        <asp:TextBox ID="txtOrgao_Transferencia" Width="500px" runat="server" MaxLength="14" onkeypress='return SomenteNumero(event)'></asp:TextBox>
					                        <%-- <asp:DropDownList ID="ddlOrgao_Transferencia" runat="server" Width="500px" AutoPostBack="true"
					                                                Style="float: left !important;" />--%>
					                    </p>
                                        <p id="idFornecedor" runat="server">
                                            <asp:Label ID="lblFornecedor" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Fornecedor:" Font-Bold="True" />
                                            <asp:TextBox ID="txtGeradorDescricao" runat="server" Width="70%" Enabled="false"></asp:TextBox>
                                            <asp:ImageButton ID="imgLupaFornecedor" runat="server" CommandName="Select" CssClass="basic"
                                                ImageUrl="../Imagens/lupa.png" ClientIDMode="Predictable" OnClick="imgLupaFornecedor_Click"
                                                OnClientClick="OpenModal();" ToolTip="Pesquisar Fornecedor" />
                                        </p>
                                        <p id="idDevolucao" runat="server" class="esconderControle">
                                            <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Divisão:" Font-Bold="True" />
                                            <asp:DropDownList ID="ddlDivisao" runat="server" Width="70%" DataTextField="Descricao"
                                                DataValueField="Id" Visible="false" />
                                        </p>
                                        <p id="idDocumento">
                                            <asp:Label ID="lblDocumentoInfoCampo" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Documento:" Font-Bold="true" />
                                            <asp:Label ID="lblDocumentoAvulsoAnoMov" runat="server" CssClass="" />
                                            <asp:TextBox ID="txtDocumentoAvulso" runat="server" MaxLength="8" CssClass="inputFromNumero"
                                                Width="120px" Style="display: inline-block; font-weight: bold; width: 500px;
                                                height: 20px" />
                                            <asp:ImageButton ID="imgLupaRequisicao" runat="server" CommandName="Select" ImageUrl="../Imagens/lupa.png"
                                                CssClass="basic" ClientIDMode="Predictable" OnClick="imgLupaRequisicao_Click"
                                                OnClientClick="OpenModalDoc();" ToolTip="Pesquisar" />
                                        </p>
                                        <p id="idTransfer" runat="server" class="esconderControle">
                                            <asp:Label ID="lblAlmoxTransfer" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Almoxarifado:" Font-Bold="True" />
                                            <asp:DropDownList ID="ddlAlmoxarifadoTransfer" Visible="false" runat="server" Width="70%"
                                                DataTextField="Descricao" DataValueField="Id" CssClass="mostrarControle" AutoPostBack="true" />
                                            <asp:TextBox ID="txtAlmoxarifadoTransf" runat="server" Width="70%" Enabled="false"></asp:TextBox>
                                        </p>
                                        <p>
                                            <asp:Label ID="lblDataEmissao" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Data Emissão:" Font-Bold="true" />
                                            <asp:TextBox ID="txtDataEmissao" runat="server" CssClass="dataFormat" MaxLength="10"
                                                Width="100px" />
                                        </p>
                                        <p>
                                            <asp:Label ID="lblDataReceb" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Data Receb.:" Font-Bold="true" />
                                            <asp:TextBox ID="txtDataReceb" runat="server" CssClass="dataFormat" MaxLength="10"
                                                Width="100px" />
                                        </p>
                                        <p>
                                            <asp:Label ID="lblValorTotal" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Valor Total:" Font-Bold="True" />
                                            <asp:TextBox ID="txtValorTotal" runat="server" Enabled="false" CssClass="numerico"
                                                onkeypress='return SomenteNumeroDecimal(event)' MaxLength="10" Width="100px" />
                                        </p>
                                        <p>
                                            <asp:Label ID="lblObservacoes" runat="server" CssClass="labelFormulario" Width="100px"
                                                Text="Observações:" Font-Bold="true" />
                                            <asp:TextBox ID="txtObservacoes" runat="server" Width="70%" Rows="4" TextMode="MultiLine"
                                                SkinID="MultiLine" MaxLength="154" onkeyup='limitarTamanhoTexto(this,154)' />
                                        </p>
                                        <p>
                                            <asp:Label ID="lblTipoEmpenho" runat="server" CssClass="labelFormulario" Visible="false"
                                                Style="text-align: right" Font-Bold="true" />
                                        </p>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:Panel ID="pnlGrid" runat="server">
                        <asp:Repeater runat="server" ID="gridSubItemMaterial" OnItemDataBound="gridSubItemMaterial_ItemDataBound"
                            OnItemCommand="gridSubItemMaterial_ItemCommand">
                            <HeaderTemplate>
                                <table width="100%" border="1" cellpadding="0" cellspacing="0" class="tabela">
                                    <tr id="celHeaderEmpenho" runat="server" style="display: none">
                                        <th colspan="6" id="HeaderDadosEmpenho" runat="server">
                                            DADOS DO EMPENHO
                                        </th>
                                        <th colspan="2">
                                            DADOS DO SAM
                                        </th>
                                        <th colspan="4">
                                            DADOS DA ENTRADA
                                        </th>
                                    </tr>
                                    <tr class="corpo">
                                        <th rowspan="2" align="center" width="1%" id="hdrLiquidar" runat="server" style="display: none">
                                            Liquidar
                                        </th>
                                        <th rowspan="2" align="center" width="1%" id="hdrPosicao" runat="server">
                                            #
                                        </th>
                                        <th rowspan="1" width="8%">
                                            <asp:Label ID="lblItem" runat="server" Text="Item"></asp:Label>
                                        </th>
                                        <th rowspan="1" width="4%" id="celHdrUnidSiaf" runat="server">
                                            <asp:Label ID="lblUnidSiaf" runat="server" Text="Unid. Siafisico"></asp:Label>
                                        </th>
                                        <th rowspan="1" width="4%" id="hdr2QtdLiq1" runat="server">
                                            <asp:Label ID="lblSIAFEM" runat="server" Text="SIAFEM"></asp:Label>
                                        </th>
                                        <th rowspan="1" width="4%" id="celHdrQuant" runat="server" style="display: none">
                                            Valor Unit.
                                        </th>
                                        
                                        <th rowspan="1" width="8%">
                                            Subitem
                                        </th>
                                       

                                        <th rowspan="1" width="4%" id="celHdrUnidSub" runat="server">
                                            <asp:Label ID="lblUnidSubitem" runat="server" Text="Unid. Subitem"></asp:Label>
                                        </th>
                                        
                                        <th rowspan="1" width="8%" id="celHdrQtdeLiq" runat="server">
                                            <asp:Label ID="lblUnidLiquidar" runat="server" Text="Qtd. Liquidar"></asp:Label>
                                        </th>
                                        <th rowspan="1" width="6%" id="hdrQtdRecSubitem" runat="server">
                                            <asp:Label ID="lblQtdRecSubitem" runat="server" Text="Qtd. Rec. Subitem"></asp:Label>
                                        </th>
                                        <th rowspan="1" width="7%">
                                            <asp:Label ID="lblPrecoUnit" runat="server" Text="Preço Unit."></asp:Label>
                                        </th>
                                        <th rowspan="1" width="7%" id="hdrPrecoTotal" runat="server">
                                            Vl. Total
                                        </th>
                                        <th rowspan="1" width="7%" id="hdrPrecoUnitSAM" style="display: none;" runat="server">
                                            Preço Unitário SAM
                                        </th>
                                         <th rowspan="1" width="7%" id="hdrSaldoLiquidar" runat="server">
                                             <asp:Label ID="Label1" runat="server" Text="Valor a Liquidar"></asp:Label>
                                        </th>
                                        <th rowspan="2" width="5%" id="hdrEdit" runat="server">
                                            Editar
                                        </th>
                                    </tr>
                                    <tr class="corpo">
                                        <th width="4%" id="hdr2QtdLiq1x" runat="server" style="display: none;">
                                            SIAFEM
                                        </th>
                                        <th width="4%" id="hdr2QtdLiq2" runat="server">
                                            Entrada
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td rowspan="3" align="center" id="celLiquidar" runat="server" style="display: none">
                                        <asp:CheckBox ID="chkLiquidarEmpenhoBEC" runat="server" AutoPostBack="True" OnCheckedChanged="chkLiquidarEmpenhoBEC_CheckedChanged" />
                                        <asp:Label ID="lblLiquidado" runat="server" Text="Item Liquidado" Visible="false" />
                                    </td>
                                    <td rowspan="3" align="center" style="border-bottom: none !important;" id="celPosicao"
                                        runat="server">
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
                                        <asp:Label ID="lblItemMatGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.Codigo")%>' />&nbsp;                                    
                                        <asp:Label visible="false" ID="lblItemMatGridDescricao" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.Descricao")%>' />&nbsp;
                                    </td>
                                    <td align="center" id="celUnidSiaf" runat="server">
                                        <asp:Label ID="lblUnidFornecimentoGridSiafisico" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UnidadeFornecimentoSiafisico.Descricao") %>' />&nbsp;
                                    </td>
                                    <td align="center" id="celQtdeLiq" runat="server">
                                        <asp:Label ID="lblQtdeLiqGridSiafisico" runat="server" Width="60px" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiqSiafisico")%>' /><br />
                                    </td>
                                    <td align="center" id="celQuant" runat="server" style="display: none">
                                        <asp:Label ID="lblValorUnit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblSubItem" runat="server"><%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%></asp:Label>
                                        &nbsp;
                                        <asp:DropDownList ID="ddlSubItemList" AutoPostBack="true" OnSelectedIndexChanged="ddlSubItemList_SelectedIndexChanged"
                                            DataTextField="CodigoFormatado" DataValueField="Id" runat="server" />
                                         <asp:DropDownList ID="ddlSubItemListDoacaoImplantado" DataTextField="CodigoDescricao" DataValueField="Id" runat="server" onchange="GetCountryDetails()" />
                                    </td>
                                    <td align="center" id="celUnidSub" runat="server">
                                        <asp:Label ID="lblUnidFornecimentoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>' />&nbsp;
                                    </td>

                                   
                                    <td align="center" id="celQtdeLiq2" runat="server">
                                        <asp:TextBox ID="txtQtdeLiqGrid" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                            onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true" OnTextChanged="txtQtdeLiqGridRepeater_OnTextChanged"
                                            MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                        <asp:HiddenField ID="hdfQtdeLiqGrid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                      
                                    </td>
                                    <td align="center" id="celQtdeMov" runat="server">
                                        <asp:TextBox ID="txtQtdeMovGrid" BackColor="#F9F3B2" CssClass="QtdeEmpenho" runat="server"
                                            Width="60px" MaxLength="20" onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true"
                                            OnTextChanged="txtQtdeSubitemGridRepeater_OnTextChanged" Text=' <%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                        <asp:HiddenField ID="hdfQtdeMovGrid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                    </td>
                                    <td align="center" id="celPrecoUnit" runat="server">
                                        <asp:Label ID="lblPrecoUnitGrid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                        <asp:Label ID="lblPrecoUnitGridCalc" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                    </td>
                                    <td align="center" id="celPrecoTotal" runat="server">
                                        <asp:Label ID="lblValorMovItemGrid" runat="server" Text='<%#  DataBinder.Eval(Container.DataItem, "ValorMov")%>' />
                                    </td>
                                    <td align="center" id="celPrecoUnitSAM" runat="server">
                                        <asp:Label ID="lblPrecoUnitSAMGrid" runat="server" Text="" />
                                    </td>
                                     <td align="center" id="celSaldoLiq" runat="server">
                                       
                                        <asp:Label ID="lblSaldoLiq" runat="server" Text='<%#  String.Format("{0:0.00}",DataBinder.Eval(Container.DataItem, "SaldoLiq") )%>'></asp:Label>
                                    </td>
                                    <td rowspan="3" style="border-bottom: none !important;" align="center" id="cellEdit"
                                        runat="server">
                                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                            CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                    </td>
                                </tr>
                                <tr class="odd" id="tblLote" runat="server" style="background-color: White !important">
                                    <td colspan="4" id="celTdEmpenho" runat="server" style="display: none">
                                    </td>
                                    <td colspan="2">
                                        <table runat="server" id="tbEmpenhoLoteNovo" visible="false">
                                            <tr>
                                                <td colspan="2">
                                                    Descrição
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    (1)
                                                </td>
                                                <td>
                                                    Quant. a Liquidar - EMPENHO
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    (2)
                                                </td>
                                                <td>
                                                    Quant. a Liquidar c/ Conversão p/ o SAM
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="celTdEmpenho2" runat="server" colspan="3">
                                        <table style="float: left" cellpadding="0" border="0" cellspacing="0">
                                            <tr runat="server" id="trEmpenhoLote" visible="false">
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td style="width: 100px">
                                                                Lote:
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Label ID="lblLote" runat="server" Text='<%#  DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>'></asp:Label>
                                                            </td>
                                                            <td style="width: 150px">
                                                                Vcto Lote:
                                                            </td>
                                                            <td style="width: 150px">
                                                                <asp:Label ID="lblVctoLote" runat="server" Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trEmpenhoLoteNovo" visible="false">
                                                <td colspan="6">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                EMPENHO<sup style="font-size: smaller">(1)</sup>
                                                            </td>
                                                            <td>
                                                                SAM<sup style="font-size: smaller">(2)</sup>
                                                            </td>
                                                            <td>
                                                                Nº do Lote
                                                            </td>
                                                            <td>
                                                                Vcto
                                                            </td>
                                                        </tr>
                                                        <asp:Repeater ID="rptItemLote" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:HiddenField ID="hfdIndex" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "index")%>' />
                                                                        <asp:Label ID="lblLiqSiafisico" runat="server" Width="80px" CssClass="QtdeEmpenho"
                                                                            Text='<%#DataBinder.Eval(Container.DataItem,"liqSiafisico") %>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblMovEstoque" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                                                            MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem,"movEstoque") %>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblLoteItem" Width="100px" runat="server" MaxLength="12" Text='<%#DataBinder.Eval(Container.DataItem, "loteItem")%>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblVctoLoteItem" Width="80px " runat="server" CssClass="dataFormat"
                                                                            Text='<%#Eval("vctoLoteItem", "{0:dd/MM/yyyy}")%>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnDeletarLoteItem" runat="server" Width="100px" Style="width: 100px !important"
                                                                            CommandName='<%#DataBinder.Eval(Container.DataItem, "indexMovItem")%>' CommandArgument='<%#DataBinder.Eval(Container.DataItem, "index")%>'
                                                                            OnClick="btnDeletarLoteItem_Click" Text="Deletar Lote" />
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="txtLiqSiafisico" runat="server" Width="80px" CssClass="QtdeEmpenho"
                                                                    onkeypress='return SomenteNumeroDecimal(event)' OnTextChanged="txtLiqSiafisico_OnTextChanged" AutoPostBack="true" MaxLength="20"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMovEstoque" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                                                    onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true" MaxLength="20"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtLoteItem" Width="100px" AutoCompleteType="Disabled" runat="server"
                                                                    AutoPostBack="true" OnTextChanged="txtLoteItemRepeater_OnTextChanged" MaxLength="12"
                                                                    Text="" />&nbsp;
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtVctoLoteItem" Width="80px" runat="server" Enabled="false" CssClass="dataFormat"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnInserirLote" runat="server" Width="100px" Style="width: 100px !important"
                                                                    CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Posicao")%>' Text="Adicionar Lote"
                                                                    OnClick="btnInserirLote_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <%--  <tr>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:Label ID="lblLote" runat="server" Text="Nº do Lote: " />
                                                </td>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:TextBox ID="txtIdLote" Width="130" AutoCompleteType="Disabled" runat="server"
                                                        AutoPostBack="true" OnTextChanged="txtIdLoteRepeater_OnTextChanged" MaxLength="12"
                                                        Text='<%#DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>' />&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:Label ID="lblVctoLote" runat="server" Text="Vcto.: " />
                                                </td>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:TextBox ID="txtVctoLote" Width="80" runat="server" Enabled="false" CssClass="dataFormat"
                                                        Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>' />&nbsp;
                                                </td>
                                            </tr>--%>
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
                                    <td rowspan="3" align="center" id="celLiquidar" runat="server" style="display: none">
                                        <asp:CheckBox ID="chkLiquidarEmpenhoBEC" runat="server" AutoPostBack="True"  OnCheckedChanged="chkLiquidarEmpenhoBEC_CheckedChanged" />
                                        <asp:Label ID="lblLiquidado" runat="server" Text="Item Liquidado" Visible="false" />
                                    </td>
                                    <td rowspan="3" align="center" style="border-bottom: none !important;" id="celPosicao"
                                        runat="server">
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
                                        <asp:Label ID="lblItemMatGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.Codigo")%>' />&nbsp;                                   
                                        <asp:Label visible="false" ID="lblItemMatGridDescricao" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.ItemMaterial.Descricao")%>' />&nbsp;
                                    </td>
                                    <td align="center" id="celUnidSiaf" runat="server">
                                       <asp:Label ID="lblUnidFornecimentoGridSiafisico" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UnidadeFornecimentoSiafisico")%>' />&nbsp;<br />
                                    </td>
                                    <td align="center" id="celQtdeLiq" runat="server">
                                        <asp:Label ID="lblQtdeLiqGridSiafisico" runat="server" Width="60px" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiqSiafisico")%>' /><br />
                                    </td>
                                    <td align="center" id="celQuant" runat="server" style="display: none">
                                        <asp:Label ID="lblValorUnit" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                    </td>
                                    <td align="center">
                                        <asp:Label ID="lblSubItem" runat="server"><%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%></asp:Label>
                                        &nbsp;
                                        <asp:DropDownList ID="ddlSubItemList" DataTextField="CodigoFormatado" DataValueField="Id"
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlSubItemList_SelectedIndexChanged"
                                            runat="server" />
                                         <asp:DropDownList ID="ddlSubItemListDoacaoImplantado" DataTextField="CodigoDescricao" DataValueField="Id" runat="server" onchange="GetCountryDetails()" />
                                    </td>
                                    <td align="center" id="celUnidSub" runat="server">
                                        <asp:Label ID="lblUnidFornecimentoGrid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>' />&nbsp;
                                    </td>
                                    
                                    <td align="center" id="celQtdeLiq2" runat="server">
                                        <asp:TextBox ID="txtQtdeLiqGrid" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                            onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true" OnTextChanged="txtQtdeLiqGridRepeater_OnTextChanged"
                                            MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                        <asp:HiddenField ID="hdfQtdeLiqGrid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>' />
                                    </td>
                                    <td align="center" id="celQtdeMov" runat="server">
                                        <asp:TextBox ID="txtQtdeMovGrid" runat="server" CssClass="QtdeEmpenho" BackColor="#F9F3B2"
                                            Width="60px" MaxLength="20" onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true"
                                            OnTextChanged="txtQtdeSubitemGridRepeater_OnTextChanged" Text='<%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                        <asp:HiddenField ID="hdfQtdeMovGrid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>' />
                                    </td>
                                    <td align="center" id="celPrecoUnit" runat="server">
                                        <asp:Label ID="lblPrecoUnitGrid" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                        <asp:Label ID="lblPrecoUnitGridCalc" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "PrecoUnit")%>' />
                                    </td>
                                    <td align="center" id="celPrecoTotal" runat="server">
                                        <asp:Label ID="lblValorMovItemGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "ValorMov"))%>' />
                                    </td>
                                    <td align="center" id="celPrecoUnitSAM" runat="server">
                                        <asp:Label ID="lblPrecoUnitSAMGrid" runat="server" Text="" />
                                    </td>
                                    <td align="center" id="celSaldoLiq" runat="server">
                                       
                                        <asp:Label ID="lblSaldoLiq" runat="server" Text='<%#  String.Format("{0:0.00}",DataBinder.Eval(Container.DataItem, "SaldoLiq")) %>'></asp:Label>
                                    </td>
                                    <td rowspan="3" style="border-bottom: none !important;" align="center" id="cellEdit"
                                        runat="server">
                                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                            CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                    </td>
                                </tr>
                                <tr class="odd" id="tblLote" runat="server">
                                    <td colspan="4" id="celTdEmpenho" runat="server" style="display: none">
                                    </td>
                                    <td colspan="2">
                                        <table runat="server" id="tbEmpenhoLoteNovo" visible="false">
                                            <tr>
                                                <td colspan="2">
                                                    Descrição
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    (1)
                                                </td>
                                                <td>
                                                    Quant. a Liquidar - EMPENHO
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    (2)
                                                </td>
                                                <td>
                                                    Quant. a Liquidar c/ Conversão p/ o SAM
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td id="celTdEmpenho2" runat="server" colspan="3">
                                        <table style="float: left" cellpadding="0" border="0" cellspacing="0">
                                            <tr runat="server" id="trEmpenhoLote" visible="false">
                                                <td colspan="2">
                                                    <table>
                                                        <tr>
                                                            <td style="width: 100px">
                                                                Lote:
                                                            </td>
                                                            <td style="width: 100px">
                                                                <asp:Label ID="lblLote" runat="server" Text='<%#  DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>'></asp:Label>
                                                            </td>
                                                            <td style="width: 150px">
                                                                Vcto Lote:
                                                            </td>
                                                            <td style="width: 150px">
                                                                <asp:Label ID="lblVctoLote" runat="server" Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>'></asp:Label>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trEmpenhoLoteNovo" visible="false">
                                                <td colspan="6">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                EMPENHO<sup style="font-size: smaller">(1)</sup>
                                                            </td>
                                                            <td>
                                                                SAM<sup style="font-size: smaller">(2)</sup>
                                                            </td>
                                                            <td>
                                                                Nº do Lote
                                                            </td>
                                                            <td>
                                                                Vcto
                                                            </td>
                                                        </tr>
                                                        <asp:Repeater ID="rptItemLote" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:HiddenField ID="hfdIndex" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "index")%>' />
                                                                        <asp:Label ID="lblLiqSiafisico" runat="server" Width="80px" CssClass="QtdeEmpenho"
                                                                            Text='<%#DataBinder.Eval(Container.DataItem,"liqSiafisico") %>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblMovEstoque" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                                                            MaxLength="20" Text='<%#DataBinder.Eval(Container.DataItem,"movEstoque") %>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblLoteItem" Width="100px" runat="server" MaxLength="12" Text='<%#DataBinder.Eval(Container.DataItem, "loteItem")%>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblVctoLoteItem" Width="80px" runat="server" CssClass="dataFormat"
                                                                            Text='<%#Eval("vctoLoteItem", "{0:dd/MM/yyyy}")%>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnDeletarLoteItem" runat="server" Width="100px" Style="width: 100px !important"
                                                                            CommandName='<%#DataBinder.Eval(Container.DataItem, "indexMovItem")%>' CommandArgument='<%#DataBinder.Eval(Container.DataItem, "index")%>'
                                                                            OnClick="btnDeletarLoteItem_Click" Text="Deletar Lote" />
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="txtLiqSiafisico" runat="server" Width="80px" CssClass="QtdeEmpenho"
                                                                    onkeypress='return SomenteNumeroDecimal(event)' OnTextChanged="txtLiqSiafisico_OnTextChanged" AutoPostBack="true" MaxLength="20"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMovEstoque" runat="server" Width="60px" CssClass="QtdeEmpenho"
                                                                    onkeypress='return SomenteNumeroDecimal(event)' AutoPostBack="true" MaxLength="20"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtLoteItem" Width="100px" AutoCompleteType="Disabled" runat="server"
                                                                    AutoPostBack="true" OnTextChanged="txtLoteItemRepeater_OnTextChanged" MaxLength="12"
                                                                    Text="" />&nbsp;
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtVctoLoteItem" Width="80px" runat="server" Enabled="false" CssClass="dataFormat"
                                                                    Text="" />
                                                            </td>
                                                            <td>
                                                                <asp:Button ID="btnInserirLote" runat="server" Width="100px" Style="width: 100px !important"
                                                                    Text="Adicionar Lote" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Posicao")%>'
                                                                    OnClick="btnInserirLote_Click" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <%--  <tr>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:Label ID="lblLote" runat="server" Text="Nº do Lote: " />
                                                </td>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:TextBox ID="txtIdLote" Width="130" AutoCompleteType="Disabled" runat="server"
                                                        AutoPostBack="true" OnTextChanged="txtIdLoteRepeater_OnTextChanged" MaxLength="12"
                                                        Text='<%#DataBinder.Eval(Container.DataItem, "IdentificacaoLote")%>' />&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:Label ID="lblVctoLote" runat="server" Text="Vcto.: " />
                                                </td>
                                                <td style="border: 0 !important; float: left">
                                                    <asp:TextBox ID="txtVctoLote" Width="80" runat="server" Enabled="false" CssClass="dataFormat"
                                                        Text='<%#Eval("DataVencimentoLote", "{0:dd/MM/yyyy}")%>' />&nbsp;
                                                </td>
                                            </tr>--%>
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
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" runat="server" Text="Adicionar Item" SkinID="Btn100" Width="100px"
                            OnClick="btnNovo_Click" Visible="False" />
                    </p>
                    <br />
                    <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                        <br />
                        <div id="interno">
                            <div>
                                <fieldset class="fieldset">
                                    <br />
                                    <p>
                                        <asp:Label ID="Label23" runat="server" CssClass="labelFormulario" Width="100px" Text="Lote:"
                                            Font-Bold="true" />
                                        <asp:RadioButtonList ID="rblLote" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblLote_SelectedIndexChanged">
                                            <asp:ListItem Value="1" Text="Sim"></asp:ListItem>
                                            <asp:ListItem Value="0" Selected="True" Text="Não"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </p>
                                    <p>
                                        <asp:Label ID="Label17" runat="server" CssClass="labelFormulario" Width="100px" Text="Item:"
                                            Font-Bold="true" />
                                        <asp:TextBox ID="txtItemMaterial" runat="server" MaxLength="9" size="9" onkeypress='return SomenteNumero(event)'
                                            onblur="return preencheZeros(this,'9')" OnTextChanged="txtItemMaterial_TextChanged"
                                            AutoPostBack="True" Enabled="False" />
                                    </p>
                                    <p id="ItemDescricao" runat="server" visible="false">
                                        <asp:Label ID="Label19" runat="server" CssClass="labelFormulario" Width="100px" Text="Item Descrição:"
                                            Font-Bold="true" />
                                        <asp:Label ID="lblItemMaterialDescricao" Width="490px" runat="server" Text="" />
                                    </p>
                                    <p>
                                        <table border="0" cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="100px" Text="Subitem:*"
                                                        Font-Bold="true" />
                                                </td>
                                                <td>
                                                    <asp:Panel runat="server" ID="pnlDefaulButton" DefaultButton="btnCarregarSubItem"
                                                        Style="text-align: left; border: 0">
                                                        <asp:TextBox ID="txtSubItem" class="txtSubItem" runat="server" MaxLength="12" size="12"
                                                            onblur="return preencheZeros(this,'12')" onkeypress="return SomenteNumero(event)" />
                                                        <asp:ImageButton ID="imgSubItemMaterial" ClientIDMode="Static" ImageUrl="../Imagens/lupa.png"
                                                            OnClientClick="OpenModalItem();" CssClass="basic" ToolTip="Pesquisar" runat="server" />
                                                        <asp:HiddenField ID="idSubItem" runat="server" />
                                                        <asp:HiddenField ID="idSubItemOld" runat="server" />
                                                        <asp:Button runat="server" ID="btnCarregarSubItem" Style="visibility: hidden" OnClick="btnCarregarSubItem_Click" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </p>
                                    <p style="display: none">
                                        <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                            Text="Classif.:" Width="100px" />
                                        <asp:DropDownList ID="ddlSubItemClassif" runat="server" AutoPostBack="True" DataTextField="Codigo"
                                            DataValueField="Id" Width="150px" />
                                    </p>
                                    <p>
                                        <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                            Text="Descrição:" Width="100px" />
                                        <asp:TextBox ID="txtDescricao" runat="server" Enabled="False" MaxLength="60" size="60"
                                            Width="80%" />
                                    </p>
                                    <p id="divQtdLiq" runat="server">
                                        <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                            Text="Qtd. a Liquid.:" Width="100px" />
                                        <asp:TextBox ID="txtQtdeLiq" runat="server" Enabled="False" MaxLength="10" onkeypress="return SomenteNumero(event)"
                                            Width="150px" />
                                    </p>
                                    <p>
                                        <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                            Text="Qtd. a Entrar:*" Width="100px" />
                                        <asp:TextBox ID="txtQtdeMov" runat="server" MaxLength="20" onkeypress="return SomenteNumero(event)"
                                            Width="150px" />
                                    </p>
                                    <p>
                                        <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                            Text="Preço Unit.:" Width="100px" />
                                        <asp:TextBox ID="txtPrecoUnit" runat="server" BackColor="#CCCCCC" CssClass="numerico"
                                            MaxLength="20" onkeypress="return SomenteNumeroDecimal(event)" Width="150px" />
                                    </p>
                                    <p>
                                        <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                            Text="Valor Total:*" Width="100px" />
                                        <asp:TextBox ID="txtValorMovItem" runat="server" CssClass="numerico" MaxLength="20"
                                            onkeypress="return SomenteNumeroDecimal(event)" Width="150px" />
                                    </p>
                                    <p>
                                        <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                            Text="Unidade:" Width="100px" />
                                        <asp:TextBox ID="txtUnidadeForn" runat="server" Enabled="False" MaxLength="60" Width="150px" />
                                    </p>
                                    <div id="DivLote" visible="false" runat="server">
                                        <asp:UpdatePanel ID="updLote" runat="server">
                                            <ContentTemplate>
                                                <p>
                                                    <asp:Label ID="Label22" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Nº do Lote:*" Width="100px" />
                                                    <asp:TextBox ID="txtIdentLoteItem" runat="server" MaxLength="12" Width="100px" OnTextChanged="txtNLote_TextChanged"
                                                        onkeyup="FormatTextBoxDirty(this)" onblur="if (this.dirty){this.onchange();}"
                                                        AutoPostBack="true" AutoCompleteType="Disabled" />
                                                </p>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <p>
                                            <asp:Label ID="Label21" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                Text="Data Venc:" Width="100px" />
                                            <asp:TextBox ID="txtVencimentoLote" runat="server" CssClass="dataFormat" MaxLength="10"
                                                Width="100px" Enabled="false" />
                                            <asp:DropDownList ID="ddlVencimentoLote" Width="150px" DataValueField="Key" DataTextField="Key"
                                                runat="server" Visible="false">
                                            </asp:DropDownList>
                                        </p>
                                    </div>
                                    <asp:Panel ID="pnlLote" runat="server" CssClass="esconderControle">
                                        <div class="formulario">
                                            <fieldset class="sideRight">
                                                <h1>
                                                    Lote</h1>
                                                <p>
                                                    <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Fabricante:" Width="100px" />
                                                    <asp:TextBox ID="txtFabricLoteItem" runat="server" MaxLength="60" Width="300px" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                        Text="Vencimento:" Width="100px" />
                                                    <asp:TextBox ID="txtVencimentoLote2" runat="server" CssClass="dataFormat" MaxLength="10"
                                                        Width="100px" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Identificação:" Width="100px" />
                                                    <asp:TextBox ID="txtIdentLoteItem2" runat="server" MaxLength="60" Width="300px" />
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
                            <div>
                                <p>
                                    <small>Os campos marcados com (*) são obrigatórios. </small>
                                </p>
                            </div>
                            <div class="botao">
                                <div class="DivButton">
                                    <p class="botaoLeft">
                                        <asp:Button ID="btnGravarItem" CssClass="" runat="server" Text="Adicionar" OnClick="btnGravarItem_Click" />
                                        <asp:Button ID="btnExcluirItem" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                            OnClick="btnExcluirItem_Click" />
                                        <asp:Button ID="btnCancelarItem" CssClass="" AccessKey="L" runat="server" Text="Cancelar"
                                            OnClick="btnCancelarItem_Click" />
                                    </p>
                                </div>
                            </div>
                    </asp:Panel>
                    <br />
                    <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                    <br />
                    <div id="DivBotoes" class="DivButton">
                        <p class="botaoLeft">
                            <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Gravar" OnClick="btnGravar_Click"
                                Enabled="False" Visible="false" />
                            <asp:Button ID="btnEstornar" CssClass="" runat="server" Text="Estornar" OnClick="btnEstornar_Click"
                                Enabled="False" Visible="false" />
                            <asp:Button ID="btnCancelar" CssClass="" runat="server" Text="Cancelar" OnClick="btnCancelar_Click"
                                Enabled="False" Visible="false" />
                        </p>
                        <p class="relativeEntrada">
                            <asp:Button ID="btnImprimir" runat="server" CssClass="" Style="width:150px;" Text="Nota de Recebimento"
                                OnClick="btnImprimir_Click" />
                           <%-- <asp:Button ID="btnAjuda" runat="server" Text="Ajuda" CssClass="" AccessKey="A" Enabled="true" />--%>
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
            <uc3:PesquisaSubitemNova runat="server" ID="uc3SubItem" />
        </div>
        <div id="dialogDoc" title="Pesquisar Documento">
            <uc6:PesquisaDocumento runat="server" ID="PesquisaDocumento" />
        </div>
        <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
            <uc5:WSSenha runat="server" ID="ucAcessoSIAFEM" />
        </div>
        <div id="dialogInfo" title="Atenção!">
            <img alt="Warning" src="../Imagens/Warning2.png" class="imgPopUpInfo" />
            <p class="infoMsg">
                Selecione corretamente o subitem para esta movimentação.
                <br />
                Preste atenção na natureza de despesa do mesmo.</p>
        </div>
        <div id="log">
        </div>
    </div>
    <script type="text/javascript">
        function FormatTextBox(obj) {
            //This is just a dummy function that adds $ as the first character.
            var value = obj.value

            if (value.length > 0)
                value = value;

            obj.value = value; // This line causes the autopostback not to fire. This only happens if the value of the input 
            // is set in the keyup or keydown event and only in IE.

        }

        function FormatTextBoxDirty(obj) {

            obj.dirty = false;  // This prevents firing onchange twice in the event that we don't modify the value
            if (obj.value.length > 0) {
                obj.value = obj.value;
                obj.dirty = true;
            }

            if (obj.value.length > 0) {
                $(".txtSubItem").attr("disabled", " disabled");
                $("#imgSubItemMaterial").attr("visible", " false");
            } else {
                $("#txtSubItem").removeAttr();
                $("#imgSubItemMaterial").removeAttr();

            }
        }

    </script>
      <script type="text/javascript">
        function GetCountryDetails() {
          
            var parm = document.getElementById('ctl00_cphBody_gridSubItemMaterial_ctl01_ddlSubItemListDoacaoImplantado');            
             var strCountry = parm.options[parm.selectedIndex];             
             //document.getElementById('ctl00_cphBody_gridSubItemMaterial_ctl01_lbltxt').innerHTML = strCountry.text;
             //document.getElementById('ctl00_cphBody_gridSubItemMaterial_ctl01_lblid').innerHTML = parm.options[parm.selectedIndex].value;            
         }
    </script>
    <%--<script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                args.set_errorHandled(true);
            }
        }); 
    </script>--%>
</asp:Content>
