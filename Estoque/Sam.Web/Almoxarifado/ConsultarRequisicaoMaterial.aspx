<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Divisão" Language="C#"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="ConsultarRequisicaoMaterial.aspx.cs"
    Inherits="Sam.Web.Almoxarifado.ConsultarRequisicaoMaterial" EnableViewState="true"
    ValidateRequest="false" EnableEventValidation="false" %>

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
                    Módulo Requisição - Requisição de Material</h1>
                <fieldset class="fieldset" id="search1">
                    <asp:Panel ID="Div3" runat="server">
                        <p>
                            <asp:Label ID="lblOrgao" runat="server" CssClass="labelFormulario" Text="Orgão*:" />
                            <asp:DropDownList ID="ddlOrgao" runat="server" ClientIDMode="Static" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblUO" runat="server" CssClass="labelFormulario" Text="UO*:" />
                            <asp:DropDownList ID="ddlUO" runat="server" ClientIDMode="Static" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlUO_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblUge" runat="server" CssClass="labelFormulario" Text="UGE*:" />
                            <asp:DropDownList ID="ddlUGE" runat="server" ClientIDMode="Static" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblUA" runat="server" CssClass="labelFormulario" Text="UA*:" />
                            <asp:DropDownList ID="ddlUA" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlUA_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Text="Divisão*:" />
                            <asp:DropDownList ID="ddlDivisao" runat="server" ClientIDMode="Static" >
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblStatus" runat="server" CssClass="labelFormulario" Text="Status:" />
                            <asp:DropDownList ID="ddlStatus" runat="server" Width="165px">
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblNumRequisicao" runat="server" CssClass="labelFormulario" Text="Nº Requisição:" />
                            <asp:TextBox ID="txtNumRequisicao" runat="server" Width="150px"></asp:TextBox>
                        </p>
                    </asp:Panel>
                </fieldset>
                <asp:Panel runat="server" ID="pnlRequisicoes">
                    <asp:GridView ID="grdDocumentos" runat="server" AllowPaging="false" AutoGenerateColumns="False"
                        CssClass="tabela" DataKeyNames="Id" OnRowDataBound="grdDocumentos_RowDataBound"
                        OnRowCommand="grdDocumentos_RowCommand" OnPageIndexChanging="grdDocumentos_PageIndexChanging">
                        <PagerStyle HorizontalAlign="Center" />
                        <RowStyle CssClass="" HorizontalAlign="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField HeaderText="Nº Requisição" ShowHeader="False" Visible="true">
                                <ItemTemplate>
                                    <asp:LinkButton ID="linkCodigo" runat="server" Font-Bold="false" CausesValidation="False"
                                        Visible='<%# ElementIfTrue((int)Eval("TipoMovimento.Id"))%>' CommandName="Select"
                                        CommandArgument='<%# Eval("Id")%>' Enabled ='<%# (!(bool)Eval("Bloquear")) %>'  Text='<%# Eval("NumeroDocumento")%>'
                                         ToolTip='<%# Eval("Id")%>'
                                        OnCommand="linkCodigo_Command"></asp:LinkButton>
                                    <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="false" Visible='<%# !ElementIfTrue((int)Eval("TipoMovimento.Id"))%>'
                                        CommandName="Select"  CausesValidation="False"
                                        OnCommand="linkCodigo_Command" Enabled ='<%# (!(bool)Eval("Bloquear")) %>'   Text='<%# Eval("NumeroDocumento")%>' ToolTip='<%# Eval("Id")%>'></asp:LinkButton>
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="DataDocumento" HeaderStyle-Wrap="false" HeaderStyle-Width="600px"
                                HeaderText="Data Documento">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:TemplateField HeaderStyle-Width="55%" HeaderText="Divisão">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescricao0" runat="server" Text='<%# Bind("Divisao.Descricao") %>'
                                        Visible="true"></asp:Label>
                                 </ItemTemplate>
                                <HeaderStyle Width="55%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="350px" HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Image runat="server" ID="imgStatus" Width="16px" ToolTip='<%# RetornaToolTipStatusRequisicao((int)Eval("TipoMovimento.Id"))%>'
                                        ImageUrl='<%# RetornaIconeStatusRequisicao((int)Eval("TipoMovimento.Id"))%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="350px" HeaderText="Requisição" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" ID="imgImprimirNota" Width="25px" ToolTip="Imprimir nota de Requisição"
                                        CausesValidation="false" CommandName="ImprimirRequisicao" ImageUrl="~/Imagens/pdf_icon.png"
                                        CommandArgument='<%# Bind("Id") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="420px" HeaderText="Nota de Fornecimento" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" ID="imgImprimirNotaSaida" Width="25px" ToolTip="Imprimir nota de Fornecimento"
                                        CausesValidation="false" CommandName="ImprimirSaida" ImageUrl="~/Imagens/pdf_icon.png" Visible='<%# MostraIconeNotaSaida((int)Eval("TipoMovimento.Id"))%>'
                                        CommandArgument='<%# string.Concat(Eval("NumeroDocumento"), ";", Eval("Divisao.Id"))%> ' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo" />
                    </asp:GridView>
                    <asp:ObjectDataSource ID="requisicao" runat="server" SelectMethod="ListarRequisicao"
                        TypeName="Sam.Web.Almoxarifado.ConsultarRequisicaoMaterial" SelectCountMethod="TotalRegistros"
                        EnablePaging="true" MaximumRowsParameterName="maximumRowsParameterName" StartRowIndexParameterName="StartRowIndexParameterName">
                        <SelectParameters>
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:Parameter Name="StartRowIndexParameterName" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" SkinID="Btn120" runat="server" Text="Nova Requisição"
                            AccessKey="R" Width="200px" OnClick="btnNovo_Click1" />
                        <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" Visible="False" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();"
                            Text="Ajuda" CssClass="" AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" OnClick="btnSair_Click" />
                    </p>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
