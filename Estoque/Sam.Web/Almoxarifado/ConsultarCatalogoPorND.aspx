<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="ConsultarCatalogoPorND.aspx.cs" Inherits="Sam.Web.ConsultarCatalogoPorND"
    Title="Módulo Tabelas :: Catálogo :: Consulta Subitem Material" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaItem.ascx" TagName="ListItemMaterial" TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>
            Módulo Almoxarifado - Consultar Catálogo Almoxarifado (Natureza de Despesa) </h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <fieldset class="fieldset">            
                    <p>
                        <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="140px" Text="Natureza de Despesa:" Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlNatureza" Width="50%" DataTextField="Descricao" DataValueField="Id" AutoPostBack="True" OnSelectedIndexChanged="ddlNatureza_SelectedIndexChanged" />
                    </p>            
                </fieldset>
                <div style="margin-bottom: 20px; margin-top: 20px;">
                    <asp:GridView ID="gridSubItemMaterial" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" DataKeyNames="Id" OnPageIndexChanging="gridItemMaterial_PageIndexChanging" AutoGenerateColumns="False" AllowPaging="false" PageSize="99999" CssClass="tabela">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="True" HeaderText="Nat. Despesa"  ItemStyle-Width="40px">
                                <ItemTemplate>
                                    <asp:Label ID="lblNatureza" runat="server" Text='<%# Bind("NaturezaDespesa.Codigo") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" HeaderText="Código Item" ItemStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:Label ID="lblItemMaterial" runat="server" Text='<%# Bind("ItemMaterial.CodigoFormatado","{0:D9}") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Código Subitem" ItemStyle-Width="20px" DataFormatString="{0:D12}" />
                            <asp:TemplateField ShowHeader="True" HeaderText="Unidade" ItemStyle-Width="5px" >
                                <ItemTemplate>
                                    <asp:Label ID="lblUnidForn" runat="server" Text='<%# Bind("UnidadeFornecimento.Codigo") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-Width="625px" ItemStyle-HorizontalAlign="Left" />
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="odsGridSubItemMaterial" runat="server" EnablePaging="True" MaximumRowsParameterName="maximumRows" StartRowIndexParameterName="startRowIndex"
                        SelectCountMethod="TotalRegistrosConsultaSubitemPorND" SelectMethod="ListarSubitemMaterialAlmoxarifadoPorNaturezaDespesa" TypeName="Sam.Presenter.SubItemMaterialPresenter" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndex" Type="Int32" />
                            <asp:Parameter Name="maximumRows" DefaultValue="50" Type="Int32" />
                            <asp:ControlParameter ControlID="ddlNatureza" Name="iNaturezaDespesa_ID" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server"  EnableViewState="true" />
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGerenciaCatalogo" CssClass="" runat="server" 
                            style="{width: 150px;}" Text="Gerência do Catálogo" AccessKey="C" 
                            PostBackUrl="~/Almoxarifado/gerenciaCatalogo.aspx" 
                            onclick="btnGerenciaCatalogo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" OnClick="btnImprimir_Click" Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" AccessKey="A" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                    <!-- fim id interno -->
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>    
    </div>

        <script type="text/javascript" language="javascript">

            function limparidSubItem() {
                $("#ctl00_cphBody_itemMaterialId").attr("value", "");
            }
</script>    
</asp:Content>
