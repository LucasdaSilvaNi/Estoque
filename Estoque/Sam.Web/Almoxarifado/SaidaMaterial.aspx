<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SaidaMaterial.aspx.cs" Inherits="Sam.Web.Almoxarifado.SaidaMaterial"
    Title="Módulo Tabelas :: Almoxarifado :: Saida de Materiais" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="~/Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova"
    TagPrefix="uc2" %>
<%@ Register Src="../Controles/PesquisaItem.ascx" TagName="ListItemMaterial" TagPrefix="uc3" %>
<%@ Register Src="../Controles/PesquisaRequisicao.ascx" TagName="PesquisaRequisicao"
    TagPrefix="uc4" %>
<%@ Register Src="../Controles/WSSenha.ascx" TagName="WSSenha" TagPrefix="uc5" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);
        function EndRequestHandler(sender, args) {
            if (args.get_error() != undefined) {
                args.set_errorHandled(true);
            }
        }
    </script>
    <script type="text/javascript">
          
         DesabilitarDuploClick();

        function toFixedSupportEnable() { return (2489.8237).toFixed; }
        function possuiParteFracionaria() { return (2489.8237).toFixed; }
        function formatarEntradaNumerica() {
            var padraoSiafemAtivo = (<%=NumeroCasasDecimais_QtdeItem()%> != 0);

            var numCasasDecimais_ValorUnitario = <%=NumeroCasasDecimais_ValorUnitario()%>;
            var numCasasDecimais_QtdeItem = <%=NumeroCasasDecimais_QtdeItem()%>;
        
            var QtdeMov = document.getElementById("<%=txtQtdeMov.ClientID%>").value;
            var QtdeSolicitada = document.getElementById("<%=txtQtdSolicitada.ClientID%>").value;
            var QtdeSaldo = document.getElementById("<%=txtSaldo.ClientID%>").value;

            if (toFixedSupportEnable) {
                if (QtdeMov != null && QtdeMov != "") {
                    QtdeMov = QtdeMov.replace(".", "");
                    QtdeMov = QtdeMov.replace(",", ".");

                    if(padraoSiafemAtivo != null && padraoSiafemAtivo != 0)
                      QtdeMov = (Number(QtdeMov)).toFixed(numCasasDecimais_QtdeItem);

                    QtdeMov = QtdeMov.toString();
                }
                else {
                    QtdeMov = 0.000.toFixed(numCasasDecimais_QtdeItem).toString();
                }

                if (QtdeSolicitada != null && QtdeSolicitada != "") {
                    QtdeSolicitada = QtdeSolicitada.replace(".", "");
                    QtdeSolicitada = QtdeSolicitada.replace(",", ".");
                    QtdeSolicitada = (Number(QtdeSolicitada)).toFixed(numCasasDecimais_QtdeItem);
                    QtdeSolicitada = QtdeSolicitada.toString();
                }
                else {
                    QtdeSolicitada = 0.000.toFixed(numCasasDecimais_QtdeItem).toString();
                }

                if (QtdeSaldo != null && QtdeSaldo != "") {
                    QtdeSaldo = QtdeSaldo.replace(".", "");
                    QtdeSaldo = QtdeSaldo.replace(",", ".");
                    QtdeSaldo = (Number(QtdeSaldo)).toFixed(numCasasDecimais_QtdeItem);
                    QtdeSaldo = QtdeSaldo.toString();
                }
                else {
                    QtdeSaldo = 0.000.toFixed(numCasasDecimais_QtdeItem).toString();
                }

                 if (parseFloat(QtdeMov) >= parseFloat(QtdeSaldo)) 
                    QtdeMov = QtdeSaldo;

                document.getElementById("<%=txtQtdeMov.ClientID%>").value = QtdeMov.replace(".", ",");
                document.getElementById("<%=txtQtdSolicitada.ClientID%>").value = QtdeSolicitada.replace(".", ",");
                document.getElementById("<%=txtSaldo.ClientID%>").value = QtdeSaldo.replace(".", ",");               
            }
        }
    </script>

    <style type="text/css">
        .radioButtonList { list-style:none; margin: 0; padding: 0;}
        .radioButtonList.horizontal li { display: inline;}

        .radioButtonList label{
            display:inline;
        }
</style>
    <div id="content">
        <h1>
            Módulo Almoxarifado - Saída de Material</h1>
        <div id="" title="Pesquisar Requisições">
            <asp:UpdatePanel runat="server" ID="udpPanel">
                <ContentTemplate>
                    <div>
                        <div class="formulario" style="height: 320px !important">
                            <div>
                                <div class="sideLeftSaida">
                                    <fieldset class="sideLeftSaida1">
                                        <asp:Label runat="server" ID="lblTipo" Text="Tipo de Saída:" CssClass="labelFormulario"
                                            Font-Bold="True"></asp:Label>
                                        <asp:RadioButtonList ID="rblTipoMovimento" runat="server" AutoPostBack="true" DataTextField="Descricao"
                                            DataValueField="Id" Height="16px" OnSelectedIndexChanged="rblTipoMovimento_SelectedIndexChanged"
                                            Width="100%" OnDataBound="rblTipoMovimento_DataBound">
                                        </asp:RadioButtonList>
                                        <asp:ObjectDataSource ID="sourceListaTipoMovimento" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularListaTipoMovimentoSaida" TypeName="Sam.Presenter.TipoMovimentoPresenter">
                                        </asp:ObjectDataSource>
                                    </fieldset>
                                    <fieldset class="sideLeftSaida2">
                                        <asp:RadioButtonList runat="server" ID="rblTipoOperacao" AutoPostBack="true" OnSelectedIndexChanged="rblTipoOperacao_SelectedIndexChanged"
                                            Width="200px" OnDataBound="rblTipoOperacao_DataBound">
                                            <asp:ListItem Text="Nova" Value="1" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Estornar" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Nota Fornecimento" Value="3"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </fieldset>
                                </div>
                                <asp:Panel ID="pnlMovimentacoesSaida" CssClass="painelRightSaida" runat="server"
                                    BorderWidth="0" BorderStyle="Dashed" Visible="true" Height="205px">
                                    <fieldset class="sideRightEntrada">
                                        <table id="tblPainelMovimentacoesSaidas" runat="server">
                                            <tr id="trCamposInfoSiafemCE" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblCampoInfoSiafemCE" runat="server" CssClass="labelFormulario" Width="100px"
                                                        Text="Inscrição (CE)*:" Style="display: inline-block; width: 120px; font-size: 12px;
                                                        float: left;" />
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="txtInfoSiafemCE" runat="server" MaxLength="22" Style="float: left !important;" />
                                                </td>
                                            </tr>
                                            <tr id="trCampoRequisicao_DivisaoRequisicao" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Text="Divisão:"
                                                        Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="padding-right: 10px">
                                                    <asp:DropDownList ID="ddlDivisao" runat="server" AutoPostBack="True" DataTextField="Descricao"
                                                        DataValueField="Id" OnDataBound="ddlDivisao_DataBound" Style="float: left !important;" />
                                                    <asp:ObjectDataSource ID="odsDivisao" runat="server" OldValuesParameterFormatString="original_{0}"
                                                        SelectMethod="ListarPorAlmoxTodosCod" TypeName="Sam.Presenter.DivisaoPresenter">
                                                        <SelectParameters>
                                                            <asp:Parameter Name="almoxarifadoId" Type="Int32" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </td>
                                            </tr>
                                            <tr id="trCampoDataMovimento" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblDataMovimento" runat="server" CssClass="labelFormulario" Text="Data Movimento*:"
                                                        Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="text-align: left; padding-right: 10px">
                                                    <asp:TextBox ID="txtDataMovimento" runat="server" size="7" class="dataFormat" OnTextChanged="txtDataMovimento_TextChanged" />
                                                </td>
                                            </tr>
                                            <tr id="trCampoRequisicao_NumeroDocumentoRequisicao" runat="server" style="line-height: 30px;">

                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblRequisicao" runat="server" CssClass="labelFormulario" Text="Documento Nº*:"
                                                        Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="text-align: left; padding-right: 10px">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox runat="server" ID="txtRequisicao" Width="200px" Enabled="false" Style="float: left !important;" />
                                                            </td>
                                                            <td>
                                                                <asp:ImageButton ID="imgLupaPesquisaDocumento" runat="server" CommandName="Select"
                                                                    CssClass="basic" ImageUrl="../Imagens/lupa.png" OnClick="imgLupaPesquisaDocumento_Click"
                                                                    ToolTip="Pesquisar" Style="float: left !important;" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <asp:HiddenField runat="server" ID="hdfMovimentoId" />
                                                </td>
                                            </tr>
                                             <tr id="idSubTipo" visible="false" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblSubTipoMovimento" runat="server" CssClass="labelFormulario" Width="140px"
                                                        Text="SubTipo:" Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" /></td>
                                                <td style="padding-right: 10px">
                                                    <asp:DropDownList ID="ddlSubTipoMovimento" AutoPostBack="true" runat="server" Width="500px" Style="float: left !important;" OnSelectedIndexChanged="ddlSubTipoMovimento_SelectedIndexChanged" />
                                                   <asp:HiddenField runat="server" ID="hdfNaturaSelecionada" />
                                                </td>
                                            </tr>
                                          
                                            <tr id="trRadioOpcoes" runat="server" style="line-height: 30px;" visible="false">
                                                <td style="padding-right: 15px">
                                                    <asp:RadioButtonList ID="rblOpcoes" CssClass="radioButtonList" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rdl_Opcoes_SelectedIndexChanged">
                                                        <asp:ListItem Text="Órgão/Almoxarifado" Value="orgao_almoxarifado" />
                                                        <asp:ListItem Text="CPF/CNPJ" Value="cpf_cnpj" /> 
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>

                                             <tr id="trCampo_CPF_CNPJ" runat="server" style="line-height: 30px;" visible="false">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="100px"
                                                        Text="CPF/CNPJ:" Style="display: inline-block; width: 120px; font-size: 12px;
                                                        float: left;"/>
                                                </td>
                                                <td style="text-align: left;">
                                                    <asp:TextBox ID="txtCPF_CNPJ" runat="server" MaxLength="22" Style="float: left !important;" onkeypress='return SomenteNumeroDecimal(event);' onkeyDown='return bloqueiaCopiarEColar();' />
                                                </td>
                                            </tr>

                                            <tr id="trCampoTransferenciaDoacao_ComboOrgao" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblInfoCampo_TransferenciaDoacao" runat="server" CssClass="labelFormulario"
                                                        Text="Orgão:" Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="padding-right: 10px">
                                                    <asp:DropDownList ID="ddlOrgao_TransferenciaDoacao" runat="server" Width="500px"
                                                        AutoPostBack="true" OnSelectedIndexChanged="ddlOrgao_TransferenciaDoacao_SelectedIndexChanged"
                                                        Style="float: left !important;" />
                                                </td>
                                            </tr>
                                            <tr id="trCampoTransferenciaDoacao_ComboAlmoxarifado" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblAlmoxTransfer" runat="server" CssClass="labelFormulario" Text="Almoxarifado:"
                                                        Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="padding-right: 10px">
                                                    <asp:DropDownList ID="ddlAmoxarifado_TransferenciaDoacao" runat="server" AutoPostBack="True"
                                                        DataTextField="Descricao" DataValueField="Id" Width="500px" Style="float: left !important;"
                                                        OnSelectedIndexChanged="ddlAmoxarifado_TransferenciaDoacao_SelectedIndexChanged1" />
                                                </td>
                                            </tr>
                                            <tr id="trtxtAvulso" runat="server" visible="false" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblTextoAvulso" runat="server" CssClass="labelFormulario" Text="UGE/CPF/CNPJ:"
                                                        Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="padding-right: 10px">
                                                    <asp:TextBox ID="txtDescricaoAvulsa" MaxLength="14" onkeypress='return SomenteNumero(event)' runat="server" Width="500px" Style="float: left !important;"></asp:TextBox>
                                                </td>
                                            </tr>                                            
                                            <tr id="trCampoValorTotal" runat="server" style="line-height: 30px; visibility: hidden">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblValorTotal" runat="server" CssClass="labelFormulario" Text="Valor Total:"
                                                        Font-Bold="True" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="text-align: left; padding-right: 10px;">
                                                    <asp:TextBox ID="txtValorTotal" runat="server" size="7" Enabled="false" />
                                                </td>
                                            </tr>
                                            <tr id="trCampoObservacoes" runat="server" style="line-height: 30px;">
                                                <td style="padding-right: 15px">
                                                    <asp:Label ID="lblObservacoes" runat="server" CssClass="labelFormulario" Text="Observações*:"
                                                        Font-Bold="true" Style="display: inline-block; width: 120px; font-size: 12px;" />
                                                </td>
                                                <td style="padding-right: 10px">
                                                    <asp:TextBox ID="txtObservacoes" runat="server" Width="500px" Rows="4" TextMode="MultiLine"
                                                        SkinID="MultiLine" Style="float: left !important;" MaxLength="154" onkeyup='limitarTamanhoTexto(this,154)' />
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </asp:Panel>
                            </div>
                        </div>
                    </div>
                    <div style="padding-top: 30px">
                        <asp:Repeater runat="server" ID="rptSubItem" OnItemDataBound="rptSubItem_ItemDataBound"
                            OnItemCommand="rptSubItem_ItemCommand">
                            <HeaderTemplate>
                                <table width="100%" border="1" cellpadding="0" cellspacing="0" class="tabela">
                                    <tr runat="server" visible="false" style="background-color: Red" id="hdrAlerta">
                                        <th style="background-color: Red; font-size: 15px" runat="server" id="hdrTDAlerta"
                                            colspan="12">
                                        </th>
                                    </tr>
                                </table>
                                <table width="100%" border="1" cellpadding="0" cellspacing="0" class="tabela">
                                    <tr id="hdrSubItem" runat="server" class="corpo">
                                        <th rowspan="2" align="center" width="2%">
                                            #
                                        </th>
                                        <th width="10%">
                                            Subitem
                                        </th>
                                        <th width="5%">
                                            Unid.
                                        </th>
                                        <th width="15%">
                                            Lote
                                        </th>
                                        <th width="7%" id="hdrPTRes" runat="server">
                                            PTRes
                                        </th>
                                        <th width="7%" id="hdrPTResAcao" runat="server">
                                            A&#231;&#227;o
                                        </th>
                                        <th width="10%" id="hdrSaltoTotalDt" runat="server">
                                            Saldo Total
                                            <asp:Label ID="lblGridDtaMov" runat="server" Text=""></asp:Label>
                                        </th>
                                        <th width="7%" id="hdrSaltoTotal" runat="server">
                                            Saldo Total
                                        </th>
                                        <th width="7%" style="display: none">
                                            Saldo
                                        </th>
                                        <th width="7%" id="hdrMedia" runat="server">
                                            Média Ultimos 3 meses
                                        </th>
                                        <th width="7%">
                                            Qtd. Solic.
                                        </th>
                                        <th width="7%">
                                            Qtd. Fornec.
                                        </th>
                                        <th rowspan="2" width="5%" id="hdrEditar" runat="server">
                                            Editar
                                        </th>
                                    </tr>
                                    <tr>
                                        <th colspan="11" align="center" id="hdrDescricao" runat="server">
                                            Descrição
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr runat="server" id="celSubItem">
                                    <td rowspan="2" align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "Posicao")%>&nbsp;
                                        <asp:Label runat="server" Visible="false" ID="movimentoItemId" Text='<%# Bind("Id") %>'></asp:Label>
                                        <asp:HiddenField ID="hdfidSubItem" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Id")%>' />
                                        <asp:HiddenField ID="idSubItem" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Id")%>' />
                                    </td>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%>&nbsp;
                                    </td>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>&nbsp;
                                    </td>
                                    <%-- <td align="center">
                                    <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoLote.UGE.Descricao")%>&nbsp;
                                </td>--%>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "CodigoDescricao")%>&nbsp;
                                        <asp:HiddenField ID="hdfSaldoQtde" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoLote.SaldoQtde")%>' />
                                    </td>
                                    <td align="center" id="celPTRes" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:Label runat="server" Visible="true" ID="lblPTRes" Text='<%# Bind("PTRes.Codigo") %>'></asp:Label>
                                    </td>
                                    <td align="center" id="celPTResAcao" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:Label runat="server" Visible="true" ID="lblPTResAcao" Text='<%# Bind("PTRes.ProgramaTrabalho.ProjetoAtividade") %>'></asp:Label>
                                    </td>
                                    <td align="center" id="celSaldoQtdeDt" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoTotalDataMovimento")%>&nbsp;
                                    </td>
                                    <td align="center" id="celSaldoQtde" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoTotal")%>&nbsp;
                                    </td>
                                    <td align="center" id="celMediaQtde" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeMedia")%>&nbsp;
                                    </td>
                                    <td align="center" id="celQtdeLiq" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>&nbsp;
                                    </td>
                                    <td align="center" id="celQtdeMov" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>&nbsp;
                                    </td>
                                    <td rowspan="2" align="center" id="celEditar" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                            CausesValidation="False" Visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'
                                            CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                    </td>
                                </tr>
                                <tr runat="server">
                                    <td colspan="10" id="celDescricao" align="left" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Descricao")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr runat="server" class="odd" id="celSubItem">
                                    <td rowspan="2" align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "Posicao")%>&nbsp;
                                        <asp:Label runat="server" Visible="false" ID="movimentoItemId" Text='<%# Bind("Id") %>'></asp:Label>
                                        <asp:HiddenField ID="hdfidSubItem" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Id")%>' />
                                        <asp:HiddenField ID="idSubItem" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Id")%>' />
                                    </td>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.CodigoFormatado")%>&nbsp;
                                    </td>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.UnidadeFornecimento.Codigo")%>&nbsp;
                                    </td>
                                    <%--  <td align="center">
                                    <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoLote.UGE.Descricao")%>&nbsp;
                                </td>--%>
                                    <td align="center" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "CodigoDescricao")%>&nbsp;
                                        <asp:HiddenField ID="hdfSaldoQtde" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoLote.SaldoQtde")%>' />
                                    </td>
                                    <td align="center" id="celPTRes" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:Label runat="server" Visible="true" ID="lblPTRes" Text='<%# Bind("PTRes.Codigo") %>'></asp:Label>
                                    </td>
                                    <td align="center" id="celPTResAcao" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:Label runat="server" Visible="true" ID="lblPTResAcao" Text='<%# Bind("PTRes.ProgramaTrabalho.ProjetoAtividade") %>'></asp:Label>
                                    </td>
                                    <td align="center" id="celSaldoQtdeDt" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoTotalDataMovimento")%>&nbsp;
                                    </td>
                                    <td align="center" id="celSaldoQtdeTotal" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'
                                        style="display: none">
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoTotal")%>&nbsp;
                                    </td>
                                    <td align="center" id="celSaldoQtde" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.SomaSaldoTotal")%>&nbsp;
                                    </td>
                                    <td align="center" id="celMediaQtde" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeMedia")%>&nbsp;
                                    </td>
                                    <td align="center" id="celQtdeLiq" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeLiq")%>&nbsp;
                                    </td>
                                    <td align="center" id="celQtdeMov" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "QtdeMov")%>&nbsp;
                                    </td>
                                    <td rowspan="2" align="center" id="celEditar" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                            CausesValidation="False" Visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'
                                            CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                    </td>
                                </tr>
                                <tr class="odd">
                                    <td colspan="10" id="celDescricao" align="left" runat="server" visible='<%# DataBinder.Eval(Container.DataItem, "Visualizar") != null ? DataBinder.Eval(Container.DataItem, "Visualizar") : true %>'>
                                        <%#DataBinder.Eval(Container.DataItem, "SubItemMaterial.Descricao")%>
                                    </td>
                                </tr>
                            </AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <asp:ObjectDataSource ID="sourceListaGridSaidaMaterial" runat="server" EnablePaging="True"
                            StartRowIndexParameterName="startRowIndexParameterName" MaximumRowsParameterName="maximumRowsParameterName"
                            SelectMethod="ListarMovimentoItens" TypeName="Sam.Web.Almoxarifado.SaidaMaterial"
                            OldValuesParameterFormatString="original_{0}">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                <asp:ControlParameter ControlID="txtRequisicao" Name="_documento" PropertyName="Text"
                                    Type="String" />
                                <asp:ControlParameter ControlID="txtDataMovimento" Name="dataMovimento" PropertyName="Text"
                                    Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <asp:ObjectDataSource ID="sourceListaGridSaidaMaterialRascunho" runat="server" EnablePaging="True"
                            StartRowIndexParameterName="startRowIndexParameterName" MaximumRowsParameterName="maximumRowsParameterName"
                            SelectMethod="CarregarRascunho" TypeName="Sam.Web.Almoxarifado.SaidaMaterial"
                            OldValuesParameterFormatString="original_{0}">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <caption>
                            <p>
                                <div id="Div1">
                                    <p class="botaoLeftPadding">
                                        <asp:Button ID="btnNovo" runat="server" AccessKey="N" CssClass="" OnClick="btnNovo_Click"
                                            SkinID="Btn100" Text="Adicionar Item" Width="100px" />
                                    </p>
                                </div>
                                <p>
                                </p>
                                <asp:Panel ID="pnlEditar" runat="server">
                                    <div id="interno">
                                        <div>
                                            <fieldset class="fieldset">
                                                <p>
                                                    <table border="0" cellpadding="0" cellspacing="0" style="font-size:14px!important;">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                                 Text="Subitem:*" Width="100px" />
                                                            </td>
                                                            <td>
                                                                <asp:Panel ID="pnlDefaulButton" runat="server" DefaultButton="btnCarregarSubItem"
                                                                    Style="text-align: left; border: 0">
                                                                    <asp:TextBox ID="txtSubItem" runat="server" CssClass="inputFromNumero" Enabled="true"
                                                                        MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress="return SomenteNumero(event)"
                                                                        size="12"></asp:TextBox>
                                                                    <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                                                                        ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalItem();" ToolTip="Pesquisar"
                                                                        Width="24px" />
                                                                    <asp:HiddenField ID="idSubItem" runat="server" />
                                                                    <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                                                                    <asp:Button ID="btnCarregarSubItem" runat="server" OnClick="btnCarregarSubItem_Click"
                                                                        Style="visibility: hidden" />
                                                                </asp:Panel>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                        Text="Descrição:" Width="100px" />
                                                    <asp:TextBox ID="txtDescricao" runat="server" Enabled="False" MaxLength="60" size="60"
                                                        Width="80%" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                        Text="Unidade:" Width="100px" />
                                                    <asp:TextBox ID="txtUnidadeFornecimento" runat="server" Enabled="False" MaxLength="60"
                                                        size="60" Width="80%" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="UGE:" Width="100px" />
                                                    <asp:HiddenField ID="hdfUge" runat="server" />
                                                    <asp:DropDownList ID="ddlUGE" runat="server" AutoPostBack="True" DataTextField="Descricao"
                                                        DataValueField="Id" OnDataBound="ddlUGE_DataBound" OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged"
                                                        Width="80%">
                                                    </asp:DropDownList>
                                                    <asp:ObjectDataSource ID="odsUGE" runat="server" SelectMethod="ListarUGESaldoTodosCod"
                                                        TypeName="Sam.Presenter.UGEPresenter">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="idSubItem" Name="subItemId" PropertyName="Value"
                                                                Type="Int32" />
                                                            <asp:Parameter DefaultValue="0" Name="almoxarifadoId" Type="Int32" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Lote:" Width="100px" />
                                                    <asp:DropDownList ID="ddlLote" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao"
                                                        DataValueField="IdLote" Enabled="false" OnDataBound="ddlLote_DataBound" OnSelectedIndexChanged="ddlLote_SelectedIndexChanged"
                                                        Width="80%">
                                                    </asp:DropDownList>
                                                    <table>
                                                        <asp:Repeater ID="rptLote" runat="server">
                                                            <ItemTemplate>
                                                                <tr>
                                                                    <td>
                                                                        <asp:HiddenField ID="idSaldoLote" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "IdLote") %>' />
                                                                    </td>
                                                                    <td>
                                                                        <asp:HiddenField ID="hdFSaldo" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SaldoQtde")%>' />
                                                                    </td>
                                                                    <td style="float: left">
                                                                        <asp:Label ID="lblDescricaoLote" runat="server" Text=' <%#DataBinder.Eval(Container.DataItem, "CodigoDescricao")%>'></asp:Label>
                                                                    </td>
                                                                    <td style="float: right">
                                                                        <asp:TextBox ID="txtQtdeLote" runat="server" ClientIDMode="Static" CssClass="txtQtdeLote"
                                                                            data-saldo='<%#DataBinder.Eval(Container.DataItem, "SaldoQtde")%>' onblur="CompararInserirSaldoFornecido()"
                                                                            onkeypress="return SomenteNumeroDecimal(event)" onPaste="return false;"></asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </table>
                                                    <%--<asp:ObjectDataSource ID="odsLote" runat="server" SelectMethod="ListarSaldoSubItemPorLote"
                                                        TypeName="Sam.Presenter.MovimentoItemPresenter">
                                                        <SelectParameters>
                                                            <asp:ControlParameter ControlID="idSubItem" Name="subItemId" PropertyName="Value"
                                                                Type="Int32" />
                                                            <asp:Parameter Name="almoxId" Type="Int32" />
                                                            <asp:ControlParameter ControlID="ddlUGE" Name="ugeId" PropertyName="SelectedValue"
                                                                Type="Int32" />
                                                        </SelectParameters>
                                                    </asp:ObjectDataSource>--%>
                                                </p>
                                                <br />
                                                <p>
                                                    <asp:Label ID="lblComboPTResAcao" runat="server" CssClass="labelFormulario" Style="width: 100px !important"
                                                        Text="Ação (PTRes)*:" />
                                                    <asp:DropDownList ID="ddlPTResAcao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPTResAcao_SelectedIndexChanged"
                                                        Style="width: 160px !important; float: left; height: 20px; line-height: 22px;" />
                                                    <asp:Label ID="lblIndicadorPTRes" runat="server" CssClass="labelFormulario" Style="width: 50px !important"
                                                        Text="PTRes:" />
                                                    <asp:DropDownList ID="ddlPTResItemMaterial" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPTResItemMaterial_SelectedIndexChanged"
                                                        Style="width: 120px !important; float: left; height: 20px; line-height: 22px;" />
                                                    <asp:Label ID="lblIndicadorDescricaoPTRes" runat="server" Font-Bold="true" Style="font-weight: bold;
                                                        font-size: 0.9em; width: 300px !important; margin-left: 10px" Text="Descrição:"
                                                        Visible="true" />
                                                    <asp:TextBox ID="txtDescricaoPTRes" runat="server" Enabled="false" Style="height: 20px;
                                                        line-height: 22px;" Visible="true" Width="575px" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Saldo:" Width="100px" />
                                                    <asp:TextBox ID="txtSaldo" runat="server" Enabled="False" MaxLength="10" onkeypress="return SomenteNumero(event)"
                                                        Width="150px" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                                        Text="Qtd. Solic.:" Width="100px" />
                                                    <asp:TextBox ID="txtQtdSolicitada" runat="server" Enabled="False" MaxLength="10"
                                                        onkeypress="return SomenteNumero(event)" Width="150px" />
                                                </p>
                                                <p>
                                                    <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                                        Text="Qtd. Fornecer:*" Width="100px" />
                                                    <asp:TextBox ID="txtQtdeMov" runat="server" MaxLength="20" onblur="CompararSaldoFornecido()"
                                                        onkeypress="return SomenteNumeroDecimal(event)" onPaste="return false;" Width="150px" />
                                                </p>
                                            </fieldset>
                                        </div>
                                        <p>
                                            <small>Os campos marcados com (*) são obrigatórios. </small>
                                        </p>
                                    </div>
                                    <!-- fim id interno -->
                                    <div class="botao">
                                        <!-- simula clique link editar/excluir -->
                                        <div class="DivButton">
                                            <p class="botaoLeft">
                                                <asp:Button ID="btnAdd" runat="server" CssClass="" OnClick="btnAdd_Click" Text="Adicionar" />
                                                <asp:Button ID="btnExcluir" runat="server" AccessKey="E" CssClass="" OnClick="btnExcluir_Click"
                                                    Text="Excluir" />
                                                <asp:Button ID="btnCancelar" runat="server" AccessKey="L" CausesValidation="False"
                                                    CssClass="" OnClick="btnCancelar_Click" Text="Cancelar" /></p>

                                            <%--  <asp:Label runat="server"  style="COLOR: white"
                                                  Text="<b>ATENÇÃO:</b> Não é possível realizar Requisição de Material quando o almoxarifado não possuir nenhuma Fechamento Mensal realizado!" />
                                                --%>
                                        </div>
                                    </div>
                                </asp:Panel>
                                <br />
                                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                                <div id="DivBotoes" class="DivButton">
                                    <p class="botaoLeft">
                                        <asp:Button ID="btnGravar" runat="server" AccessKey="G" CssClass="" OnClick="btnGravar_Click"
                                            Text="Gravar" />

                                        <asp:Button ID="btnEstornar" runat="server" AccessKey="G" CssClass="" OnClick="btnEstornar_Click"
                                            Text="Estornar" />
                                        <asp:CheckBox ID="cbxRascunho" runat="server" CssClass="rascunho" Text="Rascunho" />
                                    </p>
                                    <p class="botaoRight">
                                        <asp:Button ID="btnImprimir" runat="server" AccessKey="I" OnClick="btnImprimir_Click"
                                            SkinID="Btn120" Text="Imprimir Nota" Width="110px" />
                                        <asp:Button ID="btnAjuda" runat="server" AccessKey="A" CssClass="" Text="Ajuda" />
                                        <asp:Button ID="btnvoltar" runat="server" AccessKey="V" CssClass="" PostBackUrl="~/Almoxarifado/ConsultarSaidaMaterial.aspx"
                                            Text="Voltar" />
                                    </p>
                                </div>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                                <p>
                                </p>
                            </p>
                        </caption>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnGravar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <div id="dialog" title="Pesquisar Requisição">
                <uc4:PesquisaRequisicao ID="PesquisaRequisicao1" runat="server" />
            </div>
            <div id="dialogItem" title="Pesquisar SubItem Material">
                <uc2:PesquisaSubitemNova ID="PesquisaSubitem1" runat="server" />
            </div>
            <div id="dialogSenhaWS" title="Senha de Acesso Webservice">
                <uc5:WSSenha runat="server" ID="ucAcessoSIAFEM" />
            </div>
        </div>
    </div>
</asp:Content>
