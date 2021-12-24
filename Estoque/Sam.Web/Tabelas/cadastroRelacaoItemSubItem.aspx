<%@ Page Title="Módulo Tabelas :: Catálogo :: Relação Subitem x Item de Material"
    Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="cadastroRelacaoItemSubItem.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroRelacaoItemSubItem" %>
    <%@ Register Src="../Controles/PesquisaSubitemNova.ascx" TagName="PesquisaItemNova" TagPrefix="uc3" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
<link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
    <h1>Módulo Tabelas - Catálogo - Definir Relações Subitem x Itens de Material</h1>
    <asp:UpdatePanel runat="server" ID= "UpdAjax">
    <ContentTemplate>    
    <div>
        <fieldset class="fieldset">
        <p>
                <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="100px" Text="SubItem:"></asp:Label>
                <asp:TextBox ID="txtItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                    CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                    ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalItem();" 
                    ToolTip="Pesquisar" onclick="imgLupaSubItem_Click" />
                <asp:HiddenField ID="idSubItem" runat="server" />
                <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
            </p>
            <p>
                <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="100px" Text="Orgão*:"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                    DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged"
                    OnDataBound="ddlOrgao_DataBound">
                    <asp:ListItem Text="Selecione" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularListaOrgaoTodosCod" TypeName="Sam.Presenter.RelacaoMaterialItemSubItemPresenter">
                </asp:ObjectDataSource>
            </p>
            <p>
                <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="100px" Text="Gestor*:"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True"
                    DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged"
                    OnDataBound="ddlGestor_DataBound">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularListaGestorTodosCod" TypeName="Sam.Presenter.RelacaoMaterialItemSubItemPresenter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </p>
            <p>
                <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="100px" Text="Grupo*:"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlGrupo" Width="80%" AutoPostBack="True"
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
                <asp:DropDownList runat="server" ID="ddlClasse" Width="80%" AutoPostBack="True"
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
                <asp:DropDownList runat="server" ID="ddlMaterial" Width="80%" AutoPostBack="True"
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
                <asp:DropDownList runat="server" ID="ddlItem" Width="80%" AutoPostBack="True" DataTextField="Descricao"
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
            <p>
                <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="100px" Text="SubItem*:"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlSubitem" Width="80%" AutoPostBack="True"
                    DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlSubItem_SelectedIndexChanged"
                    OnDataBound="ddlSubitem_DataBound">
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
                    DataFormatString="{0:D9}" />
                <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-HorizontalAlign="Left"
                    HeaderStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                            CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField Visible="false"><ItemTemplate><asp:Label runat="server" ID="MaterialId" Text='<%# Eval("MaterialId") %>' Visible="false"></asp:Label></ItemTemplate></asp:TemplateField>                
                <asp:TemplateField Visible="false"><ItemTemplate><asp:Label runat="server" ID="ClasseId" Text='<%# Eval("ClasseId") %>' Visible="false"></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:TemplateField Visible="false"><ItemTemplate><asp:Label runat="server" ID="GrupoId" Text='<%# Eval("GrupoId") %>' Visible="false"></asp:Label></ItemTemplate></asp:TemplateField>
                <asp:TemplateField Visible="false"><ItemTemplate><asp:Label runat="server" ID="ItemSubItemMaterialId" Text='<%# Eval("ItemSubItemMaterialId") %>' Visible="false"></asp:Label></ItemTemplate></asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridSubItemMaterial" runat="server" EnablePaging="True"
            MaximumRowsParameterName="maximumRowsParameterName" StartRowIndexParameterName="startRowIndexParameterName"
            SelectCountMethod="TotalRegistros" SelectMethod="PopularDadosItemMaterialBySubItem"
            TypeName="Sam.Presenter.ItemMaterialPresenter" OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlSubitem" Name="_materialId" PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue" Type="Int32" />
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
            <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                AccessKey="V" />
        </p>
    </div>
    <asp:Panel runat="server" ID="pnlEditar">        
        <div id="interno">
            <div>
                <fieldset class="fieldset">
                    <p>
                        <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="100px" Text="Grupo*:"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlGrupoEdit" Width="80%" AutoPostBack="True"
                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlGrupoEdit_SelectedIndexChanged"
                            OnDataBound="ddlGrupoEdit_DataBound">
                        </asp:DropDownList>
                    </p>
                    <p>
                        <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="100px" Text="Classe*:"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlClasseEdit" Width="80%" AutoPostBack="True"
                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlClasseEdit_SelectedIndexChanged"
                            OnDataBound="ddlClasseEdit_DataBound">
                        </asp:DropDownList>
                    </p>
                    <p>
                        <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="100px" Text="Material*:"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlMaterialEdit" Width="80%" AutoPostBack="True"
                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlMaterialEdit_SelectedIndexChanged"
                            OnDataBound="ddlMaterialEdit_DataBound">
                        </asp:DropDownList>
                    </p>
                    <p>
                        <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="100px" Text="Item*:"></asp:Label>
                        <asp:DropDownList runat="server" ID="ddlItemEdit" Width="80%" AutoPostBack="True"
                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlItemEdit_SelectedIndexChanged"
                            OnDataBound="ddlItemEdit_DataBound">
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
                        OnClick="btnExcluir_Click" />
                    <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                        OnClick="btnCancelar_Click" />
                </p>
                <br />
            </div>
        </div>
    </asp:Panel>
</ContentTemplate>
    </asp:UpdatePanel>
            <div id="dialogItem" title="Pesquisar Item Material">
    <uc3:PesquisaItemNova ID="PesquisaSubitem1" runat="server" />    
    </div>
  </div>
</asp:Content>
