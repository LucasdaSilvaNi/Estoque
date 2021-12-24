<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Divisão" Language="C#"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="ConsultarCatalogo.aspx.cs"
    Inherits="Sam.Web.Almoxarifado.ConsultarCatalogo" EnableViewState="true" ValidateRequest="false"
    EnableEventValidation="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitem.ascx" TagName="PesquisaSubitem" TagPrefix="uc2" %>
<%@ Register Src="../Controles/ComboboxesHierarquiaPadrao.ascx" TagName="CombosHierarquiaPadrao"
    TagPrefix="uc3" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <asp:UpdatePanel runat="server" ID="ajax1">
        <ContentTemplate>
            <div id="content">
                <h1>
                    Módulo Almoxarifado - Consultar Catálogo</h1>
                <uc3:CombosHierarquiaPadrao ID="CombosHierarquiaPadrao1" runat="server" EnableViewState="true" ShowNumeroRequisicao="false" ShowStatus="false" />
                              
                <asp:Panel runat="server" ID="pnlRequisicoes">
                    <asp:GridView SkinID="GridNovo" ID="grdDocumentos" runat="server" AllowPaging="True"
                        AutoGenerateColumns="False" CssClass="tabela" DataKeyNames="TB_SUBITEM_MATERIAL_ALMOX_ID">
                        <PagerStyle HorizontalAlign="Center" />
                        <RowStyle CssClass="" HorizontalAlign="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField HeaderText="Cód" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SubItem" ItemStyle-Width="30%">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Unid." ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Natureza" ItemStyle-Width="20%">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Disponivel" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Almoxarifado" ItemStyle-Width="30%">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo" />
                    </asp:GridView>
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <div class="DivButton">
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" Visible="False" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();"
                            Text="Ajuda" CssClass="" AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
            </div>
            <script type="text/javascript" language="javascript">

                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
                    if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                        args.set_errorHandled(true);
                    }
                }); 
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
