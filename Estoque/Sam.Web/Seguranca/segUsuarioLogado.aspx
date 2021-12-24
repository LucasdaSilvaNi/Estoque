<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="segUsuarioLogado.aspx.cs" Inherits="Sam.Web.Seguranca.segUsuarioLogado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <br />
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <div id="Div3">
                        <p>
                            <label for="txtCpf">CPF do Usuário</label>&nbsp;<asp:TextBox ID="txtCpf" runat="server" MaxLength="50" CssClass="textBox"></asp:TextBox>
                            &nbsp;<asp:Button ID="btnConsultar" runat="server" Text="Consultar" CssClass="button" OnClick="btnConsultar_Click" />
                        </p>
                    </div>

                    <div class="divUsuarioLogado">
                        <div id="divTreeView" class="divTreeViewUsuarioLogado">
                            <asp:TreeView ID="trvUsuarioLogado" runat="server" ShowLines="True" OnSelectedNodeChanged="trvUsuarioLogado_SelectedNodeChanged" SelectedNodeStyle-BackColor="#FFFF80" SelectedNodeStyle-BorderColor="Blue" SelectedNodeStyle-BorderStyle="Solid" SelectedNodeStyle-BorderWidth="1px" SelectedNodeStyle-HorizontalPadding="5px" SelectedNodeStyle-VerticalPadding="2px"></asp:TreeView>
                        </div>

                        <div id="divGridView" class="divGridViewUsuarioLogado">
                            <label id="arvoreItemSelecionado" style="align-content: center;" runat="server"></label>

                            <asp:GridView ID="grdLogados" SkinID="GridModal" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                                CssClass="tabela" DataKeyNames="LoginId" OnRowCommand="grdLogados_RowCommand" PageSize="20" OnRowDataBound="grdLogados_RowDataBound" OnPageIndexChanging="grdLogados_PageIndexChanging">
                                <PagerStyle HorizontalAlign="Center" />
                                <RowStyle CssClass="" HorizontalAlign="Left" />
                                <AlternatingRowStyle CssClass="odd" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Usuário">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" style="text-wrap: none;" Text='<%# Bind("Usuario") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle Wrap="False" />
                                        <HeaderStyle Width="80px" Wrap="False" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Nome do Usuário">
                                        <ItemTemplate>
                                            <asp:Label ID="Label2" runat="server" class="textoMaisuculo" Text='<%# Bind("UsuarioNome") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle Wrap="False" />
                                        <HeaderStyle Wrap="False" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Logado">
                                        <ItemTemplate>
                                            <asp:Label ID="Label3" runat="server" class="gridViewTextNoWrap" Text='<%# Bind("DataHoraLogado", "{0:dd/MM/yyyy HH:mm:ss}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle Wrap="False" />
                                        <HeaderStyle Width="110px" Wrap="False" />
                                    </asp:TemplateField>

                                    <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="imageLogoff" runat="server" CausesValidation="false" CommandName="Logoff" CommandArgument='<%# Bind("LoginId") %>' ImageUrl="~/Imagens/delete.png" Text="Logoff" Width="18" Height="18" />
                                        </ItemTemplate>
                                        <HeaderStyle Width="30px" Wrap="False" />
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="corpo"></HeaderStyle>
                            </asp:GridView>
                        </div>
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
