<%@ Page Title="Módulo Requisição :: Requisição Material" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="RequisicaoMaterial.aspx.cs" Inherits="Sam.Web.Almoxarifado.RequisicaoMaterial" EnableViewState="true" ValidateRequest="false" EnableEventValidation="false" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova" TagPrefix="uc2" %>
<%@ Register Src="~/Controles/ComboboxesHierarquiaPadrao.ascx" TagName="CombosHierarquiaPadrao" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    
    <script type="text/javascript">

        DesabilitarDuploClick();

        function toFixedSupportEnable() { return (2489.8237).toFixed; }
    </script>
    <asp:UpdatePanel runat="server" ID="ajax1">
        <ContentTemplate>
            <div id="content">
                <h1>Módulo Requisição - Requisição de Material</h1>
                <uc3:CombosHierarquiaPadrao ID="CombosHierarquiaPadrao1" runat="server" EnableViewState="true" ShowStatus="false" ShowNumeroRequisicao="false" />
                                
                <asp:GridView ID="gridRequisicao" runat="server" AllowPaging="false" OnSelectedIndexChanged="gridRequisicao_SelectedIndexChanged"
                    EnableViewState="false" AutoGenerateColumns="False" AllowSorting="true" PageSize="9999"  OnPageIndexChanging="gridRequisicao_PageIndexChanging"
                    DataKeyNames="Id" OnRowDataBound="gridRequisicao_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Subitem" HeaderStyle-Width="200px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="Codigo" Text='<%# String.Format("{0:D12}", Eval("SubItemMaterial.Codigo"))%>'
                                    Visible="true"></asp:Label>
                                <asp:Label runat="server" ID="Id" Visible="false" Text='<%# String.Format("{0:D12}", Eval("SubItemMaterial.Id"))%>'></asp:Label>
                                <asp:Label runat="server" ID="IdAleatorio" Visible="false" Text='<%# String.Format("{0:D12}", Eval("Id"))%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição" HeaderStyle-Width="400px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescricao" Text='<%# Bind("SubItemMaterial.Descricao") %>'
                                    Visible="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantidade" HeaderStyle-Width="100px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblQtd" Text='<%# Bind("QtdeLiq") %>' Visible="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Un. Fornecimento" HeaderStyle-Width="140px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblUnidadeFornecimento" Text='<%# Bind("SubItemMaterial.UnidadeFornecimento.Descricao") %>' Visible="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PTRes" HeaderStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPTRes" Text='<%# Eval("PTRes.Codigo") %>' Visible="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ação" HeaderStyle-Width="60px" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPTResAcao" Text='<%# Eval("PTRes.ProgramaTrabalho.ProjetoAtividade") %>' Visible="true"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                    CommandName="Select" CommandArgument='<%# Bind("Id") %>' EnableViewState="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo"></HeaderStyle>
                </asp:GridView>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar">
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:HiddenField ID="idSubItem" runat="server" />
                                    <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="120px" Text="SubItem*:"></asp:Label>
                                    <asp:TextBox ID="txtSubItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'  CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                                    <asp:ImageButton ID="dialog_link" runat="server" CommandName="Select" ImageUrl="../Imagens/lupa.png"   OnClientClick="OpenModal();" CssClass="basic" ToolTip="Pesquisar" OnClick="dialog_link_Click" />
                                </p>
                                <p>
                                    <asp:Label ID="lblComboPTResAcao" runat="server" CssClass="labelFormulario" style="width: 120px !important" Text="Ação (PTRes)*:" />
                                    <asp:DropDownList ID="ddlPTResAcao" runat="server" style="width: 120px !important;float: left;" OnSelectedIndexChanged="ddlPTResAcao_SelectedIndexChanged" AutoPostBack="true" />

                                    <asp:Label ID="lblIndicadorPTRes" runat="server" CssClass="labelFormulario" style="width: 50px !important" Text="PTRes:" />
                                    <asp:DropDownList ID="ddlPTResItemMaterial" runat="server" style="width: 120px !important;float:left" OnSelectedIndexChanged="ddlPTResItemMaterial_SelectedIndexChanged" AutoPostBack="true" />

                                    <asp:Label ID="lblIndicadorDescricaoPTRes" runat="server" style="font-weight:bold;font-size: 0.9em;width: 300px !important;margin-left:10px" Font-Bold="true" Text="Descrição:" Visible="true" />
                                    <asp:TextBox ID="txtDescricaoPTRes" runat="server" Width="611px" style="height:20px;line-height: 22px;" Enabled="false" Visible="true" />

<%--                                    <asp:Label ID="lblIndicadorAcaoPTRes" runat="server" style="font-size: 0.9em;width: 400px !important" Font-Bold="true" Text="Ação:" Visible="false" />
                                    <asp:TextBox ID="txtAcaoPTRes" runat="server" MaxLength="4" size="6" style="width: 150px !important" Enabled="false" Visible="false" />
--%>                                </p>
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="120px" Text="Descrição*:"></asp:Label>
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="255" size="255" style="height:20px;width:985px;line-height: 22px;"  Enabled="false" TextMode="MultiLine" SkinID="MultiLine" Rows="2"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="lblUnidadeFornecimento" runat="server" CssClass="labelFormulario" Width="120px" Text="Unid. Fornec.*:"></asp:Label>
                                    <asp:TextBox ID="txtUnidadeFornecimento" runat="server" MaxLength="255" style="height:20px;width: 990px;line-height: 22px;" Enabled="false" />
                                </p>
                                <p>
                                    <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Width="120px" Text="Quantidade*:"></asp:Label>
                                    <asp:TextBox ID="txtQuantidade" runat="server" MaxLength="9" CssClass="QtdeEmpenho"
                                        onkeypress='return SomenteNumero(event)' size="12"></asp:TextBox>
                                </p>
                            </fieldset>
                        </div>
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <p>
                        <div class="botao">
                            <div class="DivButton">
                                <p class="botaoLeft">
                                    <asp:Button ID="btnAdicionar" CssClass="" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" />
                                    <asp:Button ID="btnEditar" runat="server" CssClass="" OnClick="btnEditar_Click" Text="Editar" />
                                    <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                        OnClick="btnExcluir_Click" />
                                    <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                        OnClick="btnCancelar_Click" />
                                </p>
                            </div>
                        </div>
                    </p>
                </asp:Panel>
                <fieldset class="fieldset">
                    <p>
                        <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="120px" Text="Observações:"></asp:Label>
                        <asp:TextBox ID="txtObservacoes" SkinID="MultiLine" runat="server" MaxLength="154"
                            size="255" Rows="4" onkeyup='limitarTamanhoTexto(this,154)' TextMode="MultiLine"
                            Width="80%"></asp:TextBox>
                    </p>
                </fieldset>
                <p>
                    <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                </p>
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                        <asp:Button ID="btnCancelarRequisicao" runat="server" Text="Cancelar Requisição" Enabled='false'    
                            SkinID="Btn140" CssClass="button" OnClick="btnCancelarRequisicao_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" Visible="False" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" AccessKey="V" OnClick="btnSair_Click" />
                    </p>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="dialog" title="Pesquisar SubItem Material">
        <uc2:PesquisaSubitemNova ID="PesquisaSubitem1" runat="server" />
    </div>
    <script type="text/javascript">
        function ShowConfirm(yourDropDown) {
            //PostBack manually
            if (confirm("Ao mudar a seleção a requisição atual será vinculada a divisão selecionada. \nTem certeza que deseja continuar?")) {
                __doPostBack('ddlYesNo', '');
                return true;
            }
            //Revert the selection and don't PostBack
            else {
                yourDropDown.selectedIndex = (yourDropDown.value == "No") ? 0 : 1;
                return false;
            }
        }

        $(document).ready(function () {
                 
            $("#ddlUA").attr("disabled", "disabled")
            $("#ddlDivisao").attr("disabled", "disabled")
            $("#ddlUGE").attr("disabled", "disabled")
            $("#ddlUO").attr("disabled", "disabled")
            
        });

    </script>
</asp:Content>
