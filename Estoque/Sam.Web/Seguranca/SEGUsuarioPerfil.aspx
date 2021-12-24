<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SEGUsuarioPerfil.aspx.cs" Inherits="Sam.Web.Seguranca.SEGUsuarioPerfil" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>
            Módulo Segurança - Atribuir Perfil Usuário</h1>
        <h5 align="center">
            <asp:Label ID="lblNomeUsuario" runat="server"></asp:Label>
        </h5>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>
                <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                    <asp:GridView ID="gridUsuarioPerfil" runat="server" AutoGenerateColumns="False" AllowPaging="True" 
                        OnSelectedIndexChanged="gridUsuarioPerfil_SelectedIndexChanged" OnRowCommand="gridUsuarioPerfil_RowCommand"
                        OnRowDataBound="gridUsuarioPerfil_RowDataBound">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select"
                                        Font-Bold="true" Text='<%# Bind("PerfilLoginId") %>'></asp:LinkButton>
                                    <asp:Label ID="lblId" runat="server" Text='<%# Bind("IdPerfil") %>' />
                                    <asp:Label ID="lblIdLogin" runat="server" Text='<%# Bind("IdLogin") %>' />
                                    <asp:Label ID="lblIdPerfilLogin" runat="server" Text='<%# Bind("PerfilLoginId") %>' />
                                    <asp:Label ID="lblNome" runat="server" Text='<%# Bind("Descricao") %>' />
                                    <asp:Label ID="lblIsAlmoxarifadoPadrao" runat="server" Text='<%# Bind("IsAlmoxarifadoPadrao") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Perfil" Visible="true">
                                <ItemTemplate>
                                    <asp:Label ID="lblPerfil" Text='<%# Bind("Descricao") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Almoxarifado" Visible="False"></asp:TemplateField>
                            <asp:ButtonField CommandName="padrao" HeaderText="Padrão" ButtonType="Image" ImageUrl="~/Imagens/button_accept.png"
                                Visible="False">
                                <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </asp:ButtonField>
                            <asp:ButtonField ButtonType="Image" CommandName="perfil" Visible="false" HeaderStyle-VerticalAlign="Middle"
                                HeaderText="Acessos" ImageUrl="~/Imagens/perfil.gif" ItemStyle-HorizontalAlign="Center"
                                ItemStyle-VerticalAlign="Middle" ItemStyle-Width="50px">
                                <HeaderStyle VerticalAlign="Middle" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="50px" />
                            </asp:ButtonField>
                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Excluir" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkExcluir" CommandName="excluir" CommandArgument='<%# Eval("PerfilLoginId") %>'
                                        runat="server">
                            <img alt="imagem" src="../Imagens/button_cancel.png" /></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                </div>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" runat="server" AccessKey="N" CssClass="" OnClick="btnNovo_Click"
                            Text="Novo" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                    <div id="interno">
                        <h3>
                            Padrões para o Perfil:</h3>
                        <fieldset class="fieldset">
                            <p>
                                <asp:CheckBox ID="chPadrao" runat="server" CssClass="labelFormulario" Text="Definir como Padrão" /><br />
                                <br />
                            </p>
                            <p>
                                <asp:Label ID="Label8" CssClass="labelFormulario" Text="Perfil*:" Width="100px" runat="server" />
                                <asp:DropDownList ID="ddlPerfil" DataTextField="Descricao" DataValueField="IdPerfil"
                                    Width="80%" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged" />
                            </p>
                            <p id="pOrgao">
                                <asp:Label ID="lblOrgao" runat="server" CssClass="labelFormulario" Width="100px"
                                    Text="Órgão*:" Font-Bold="True" Visible="False" />
                                <asp:DropDownList ID="ddlOrgao" runat="server" Width="80%" DataTextField="CodigoDescricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged" AutoPostBack="True"
                                    Visible="False" />
                            </p>
                            <p id="pGestor">
                                <asp:Label ID="lblGestor" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="Gestor*:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlGestor" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged" Width="80%"
                                    Visible="False" OnDataBound="ddlGestor_DataBound" />
                            </p>
                            <p id="pUo">
                                <asp:Label ID="lblUo" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="UO:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlUo" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlUo_SelectedIndexChanged" Width="80%"
                                    Visible="False" />
                            </p>
                            <p id="pUge">
                                <asp:Label ID="lblUge" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="UGE:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlUge" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlUge_SelectedIndexChanged" Width="80%"
                                    Visible="False" />
                            </p>
                            <p id="pUa">
                                <asp:Label ID="lblUa" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="UA:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlUa" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlUa_SelectedIndexChanged" Width="80%"
                                    Visible="False" />
                            </p>
                            
                            <p id="pDivisao">
                                <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="Divisão*:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlDivisao" runat="server" DataTextField="CodigoDescricao" DataValueField="Id"
                                    Width="80%" Visible="False" OnSelectedIndexChanged="ddlDivisao_SelectedIndexChanged" />
                            </p>
                            <p id="pAlmoxarifado">
                                <asp:Label ID="lblAlmoxarifado" runat="server" CssClass="labelFormulario" Font-Bold="True"
                                    Text="Almoxarifado*:" Width="100px" Visible="False" />
                                <asp:DropDownList ID="ddlAlmoxarifado" runat="server" AutoPostBack="True" DataTextField="Descricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlAlmoxarifado_SelectedIndexChanged"
                                    Width="80%" Visible="False" />
                            </p>
                    </div>
                    <p>
                        <small>Os campos marcados com (*) são obrigatórios. </small>
                    </p>
                    <!-- fim id interno -->
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" runat="server" AccessKey="G" CssClass="" Enabled="False"
                            OnClick="btnGravar_Click" Text="Salvar" />
                        <asp:Button ID="btnCancelar" runat="server" AccessKey="C" CssClass="" Enabled="False"
                            OnClick="btnCancelar_Click" Text="Cancelar" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" AccessKey="I" CssClass="" OnClick="btnImprimir_Click"
                            Text="Relatório" />
                        <asp:Button ID="btnAjuda" runat="server" AccessKey="A" CssClass="" Text="Ajuda" OnClientClick="OpenModal();" />
                        <asp:Button ID="btnvoltar" runat="server" AccessKey="V" CssClass="" PostBackUrl="~/Seguranca/SEGUsuario.aspx"
                            Text="Voltar" />
                    </p>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:HiddenField ID="hidPerfilId" runat="server" />
        <asp:HiddenField ID="hidLoginId" runat="server" />
        <asp:HiddenField ID="hidPerfilLoginId" runat="server" />
    </div>
</asp:Content>
