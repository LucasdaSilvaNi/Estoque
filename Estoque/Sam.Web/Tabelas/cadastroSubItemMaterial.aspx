<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="cadastroSubItemMaterial.aspx.cs" Inherits="Sam.Web.cadastroSubItemMaterial"
    Title="Módulo Tabelas :: Catálogo :: Subitem Material" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="~/Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova" TagPrefix="uc2" %>

<%@ Register Src="../Controles/PesquisaItemNova.ascx" TagName="PesquisaItemNova" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>

    <div id="content">
        <h1>Módulo Tabelas - Catálogo - Subitens de Material</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div style="margin-bottom: 20px; margin-top: 20px">
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="100px" Text="Item:"></asp:Label>
                            <asp:TextBox ID="txtItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                                CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                            <asp:ImageButton ID="imgLupaItem" runat="server" CommandName="Select" CssClass="basic"
                                ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalItem();" ToolTip="Pesquisar" />
                            <asp:HiddenField ID="itemMaterialId" Value="" runat="server" />
                            <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                        </p>

                        <p>

                            <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Font-Bold="true"
                                Text="Subitem:" Width="100px" />

                            <asp:Panel ID="pnlDefaulButton" runat="server" DefaultButton="btnCarregarSubItem"
                                Style="text-align: left; border: 0">
                                <asp:TextBox ID="txtSubItem" runat="server" CssClass="inputFromNumero" Enabled="false"
                                    MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress="return SomenteNumero(event)"
                                    size="12"></asp:TextBox>
                                <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                                    ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalSubItem();" ToolTip="Pesquisar"
                                    Width="24px" />
                                <asp:HiddenField ID="idSubItem" runat="server" />
                                <asp:HiddenField ID="HiddenField1" runat="server" />
                                <asp:Button ID="btnCarregarSubItem" runat="server" OnClick="btnCarregarSubItem_Click"
                                    Style="visibility: hidden" />
                            </asp:Panel>

                        </p>
                        <p>
                            <asp:Label ID="Label9" runat="server" class="labelFormulario" Width="100px" Text="Orgão*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged"
                                OnDataBound="ddlOrgao_DataBound">
                                <asp:ListItem Text="Selecione" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularListaOrgaoTodosCod" TypeName="Sam.Presenter.SubItemMaterialPresenter"></asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="100px" Text="Gestor*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged"
                                OnDataBound="ddlGestor_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularListaGestorTodosCod" TypeName="Sam.Presenter.SubItemMaterialPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="100px" Text="Grupo*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlGrupo" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlGrupo_SelectedIndexChanged"
                                OnDataBound="ddlGrupo_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaGrupo" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosGrupoTodosCod" TypeName="Sam.Presenter.GrupoPresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="100px" Text="Classe*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlClasse" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlClasse_SelectedIndexChanged"
                                OnDataBound="ddlClasse_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaClasse" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosClasseComCod" TypeName="Sam.Presenter.ClassePresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlGrupo" Name="_grupoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="100px" Text="Material*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlMaterial" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged"
                                OnDataBound="ddlMaterial_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaMaterial" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosMaterialComCod" TypeName="Sam.Presenter.MaterialPresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlClasse" Name="_classeId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="100px" Text="Item*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlItem" Width="80%" AutoPostBack="True" DataTextField="Descricao" onchange="limparidSubItem();"
                                DataValueField="Id" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged" OnDataBound="ddlItem_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsListaItem" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosItemMaterialComCod" TypeName="Sam.Presenter.ItemMaterialPresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlMaterial" Name="_materialId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <!--
                             <p>
                <asp:Label ID="Label5" Visible="false" runat="server" CssClass="labelFormulario" Width="100px" Text="SubItem*:"></asp:Label>
                <asp:DropDownList  Visible="false" runat="server" ID="ddlSubitem" Width="80%" AutoPostBack="True"
                    DataTextField="Descricao" DataValueField="Id" 
                   >
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsListaSubitem" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularDadosSubItemComCod" 
                    TypeName="Sam.Presenter.SubItemMaterialPresenter">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" DefaultValue="10000" />
                        <asp:ControlParameter ControlID="ddlItem" Name="_itemId" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlGestor" Name="_gestorID" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </p>
-->
                    </fieldset>
                    <asp:GridView ID="gridSubItemMaterial" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                        DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanged="gridItemMaterial_PageIndexChanged"
                        OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged" CssClass="tabela">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                        CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" ApplyFormatInEditMode="False"
                                DataFormatString="{0:D12}" />
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-HorizontalAlign="Left"
                                HeaderStyle-HorizontalAlign="Left" />

                            <%--  <asp:BoundField DataField="IndicadorAtividade" HeaderText="Status" ItemStyle-HorizontalAlign="Left"
                                HeaderStyle-HorizontalAlign="Left" />--%>


                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="IndicadorAtividade" runat="server" Text='<%# (Convert.ToBoolean(Eval("IndicadorAtividade"))==true ?  "Ativo" : "Inativo") %>'>                                                          
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />

                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="sourceGridSubItemMaterial" runat="server" EnablePaging="True"
                        MaximumRowsParameterName="maximumRowsParameterName" StartRowIndexParameterName="startRowIndexParameterName"
                        SelectCountMethod="TotalRegistros" SelectMethod="PopularDados" TypeName="Sam.Presenter.SubItemMaterialPresenter"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="ddlItem" Name="_itemId" PropertyName="SelectedValue"
                                Type="Int32" />
                            <asp:ControlParameter ControlID="ddlGestor" Name="_gestorID" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/consultaSubItemMaterial.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar">
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset" runat="server" id="fildSetEdit">
                                <p>
                                    <asp:HiddenField ID="hfdSubItemNatureza" runat="server" />
                                    <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="150px" Text="Código*:"></asp:Label>
                                    <asp:TextBox ID="txtCodigo" runat="server" EnableViewState="False" CssClass="inputFromNumero"
                                        MaxLength="12" size="12"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="150px" Text="Descrição*:"></asp:Label>
                                    <asp:TextBox ID="txtDescricao" runat="server" EnableViewState="False" MaxLength="100"
                                        size="73"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label15" runat="server" Visible="false" CssClass="labelFormulario" Width="150px" Text="Cód. Barras:"></asp:Label>
                                    <asp:TextBox ID="txtCodBarras" runat="server" Visible="false" EnableViewState="False" MaxLength="25"
                                        size="73"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="150px" Text="Natureza*:"></asp:Label>
                                    <asp:DropDownList ID="ddlNatureza" runat="server" DataTextField="CodigoDescricao" DataValueField="Id"
                                        Width="480px">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsListaNatDespesa" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="PopularNaturezaDespesaTodosCodPorItemMaterial" TypeName="Sam.Presenter.ItemMaterialPresenter">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="ddlItem" Name="_itemMaterialId" PropertyName="SelectedValue"
                                                Type="Int32" />

                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:DropDownList ID="ddlNaturezaEdit" runat="server" Visible="false" DataTextField="CodigoDescricao"
                                        DataValueField="Id" Width="480px">
                                    </asp:DropDownList>
                                </p>
                                <%--     <p>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="150px" Text="Conta Auxiliar*:"></asp:Label>
                                    <asp:DropDownList ID="ddlConta" runat="server" DataTextField="Descricao" DataValueField="Id"
                                        Width="480px">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsListaConta" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="PopularDadosContaTodosCod" TypeName="Sam.Presenter.ContaAuxiliarPresenter">
                                    </asp:ObjectDataSource>
                                </p>--%>
                                <%--  <p>
                                    <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="150px" Text="Controla Lote*:"></asp:Label>
                                    <asp:DropDownList ID="ddlControlaLote" runat="server" Width="155px" Enabled="false">
                                        <asp:ListItem Text="SIM" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="NÃO" Value="False" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </p>--%>
                                <p runat="server" visible="false">
                                    <asp:Label ID="Label16" runat="server" class="labelFormulario" Width="150px" Text="Ex. Decimos*:"></asp:Label>
                                    <asp:DropDownList ID="ddlExpandeDecimos" runat="server" Width="155px">
                                        <asp:ListItem Text="SIM" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="NÂO" Value="False"></asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p runat="server" visible="false">
                                    <asp:Label ID="Label6" runat="server" class="labelFormulario" Width="150px" Text="Permite Frac.*:"></asp:Label>
                                    <asp:DropDownList ID="ddlPermiteFracionamento" runat="server" Width="155px">
                                        <asp:ListItem Text="SIM" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="NÂO" Value="False"></asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label7" runat="server" class="labelFormulario" Width="150px" Text="Unid. Fornecimento*:"></asp:Label>
                                    <asp:DropDownList ID="ddlUnidadeFornecimento" runat="server" AutoPostBack="True"
                                        DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlUnidadeFornecimento_SelectedIndexChanged"
                                        Width="273px" OnDataBound="ddlUnidadeFornecimento_DataBound">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="sourceListaUnidadeFornecimento" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="PopularDadosUnidadeFornecimentoTodosCod" TypeName="Sam.Presenter.UnidadeFornecimentoPresenter">
                                        <SelectParameters>
                                            <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                                Type="Int32" />
                                            <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue"
                                                Type="Int32" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </p>
                                <p>
                                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="150px" Text="Ind. de Atividade:*"></asp:Label>
                                    <asp:DropDownList ID="ddlAtividade" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                            </fieldset>
                        </div>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div class="Divbotao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                    OnClick="btnExcluir_Click" Visible="false" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
                <br />
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div id="dialogItem" title="Pesquisar Item Material">
        <uc3:PesquisaItemNova ID="PesquisaItem" runat="server" />
    </div>

    <div id="dialogSubItem" title="Pesquisar SubItem Material">
        <uc2:PesquisaSubitemNova ID="PesquisaSubitem" runat="server" />
    </div>
    <script type="text/javascript" language="javascript">

        function limparidSubItem() {
            $("#ctl00_cphBody_itemMaterialId").attr("value", "");

        }

        function limparSub() {
            $("#ctl00_cphBody_idSubItem").attr("value", "");
        }

    </script>
</asp:Content>
