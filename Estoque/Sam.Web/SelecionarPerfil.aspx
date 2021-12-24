<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" 
    CodeBehind="SelecionarPerfil.aspx.cs" Inherits="Sam.Web.SelecionarPerfil" %>
    <%@ Register Src="Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
    <div class="barraAzul">
    </div>
        <h1>
            Selecione o perfil</h1>
        <p>
            <asp:UpdatePanel runat="server" ID="udpPanel">
                <ContentTemplate>
                    <asp:GridView ID="gridUsuarioPerfil" runat="server" AutoGenerateColumns="False" 
                        AllowPaging="True" DataKeyNames="PerfilLoginId"
                        OnRowCommand="gridUsuarioPerfil_RowCommand" 
                        onselectedindexchanged="gridUsuarioPerfil_SelectedIndexChanged">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False"
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
                            <asp:TemplateField HeaderText="Selecione" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/perfil.gif" CausesValidation ="true"
                                    CommandName="Select" CommandArgument='<%# Bind("PerfilLoginId") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </p>
        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
    </div>
</asp:Content>
