<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"
    AutoEventWireup="true" ValidateRequest="false" CodeBehind="SEGMensagemUsuario.aspx.cs"
    Inherits="Sam.Web.Seguranca.SEGMensagemUsuario" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <div id="content">
        <h1>
            Módulo Segurança - Mensagem Usuário</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>
                <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                    <fieldset class="fieldset">
                        <br />
                        <p>
                            <asp:Label ID="Label4" CssClass="labelFormulario" Text="Ativo:" Width="150px" runat="server" />
                            <asp:DropDownList ID="ddlAtivoConsulta" runat="server" Width="150px" 
                                AutoPostBack="True">
                                <asp:ListItem Selected="True" Text="Sim" Value="True"></asp:ListItem>
                                <asp:ListItem Selected="False" Text="Não" Value="False"></asp:ListItem>
                            </asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="150px" Text="Perfil:"></asp:Label>
                            <asp:DropDownList ID="ddlPerfilConsulta" DataSourceID="odsPerfil" 
                                runat="server" DataTextField="TB_PERFIL_DESCRICAO"
                                Width="500px" DataValueField="TB_PERFIL_ID" AutoPostBack="True">
                            </asp:DropDownList>
                        </p>
                    </fieldset>
                    <asp:GridView SkinID="GridNovo" ID="grdNotificacao" runat="server" AllowPaging="True"
                        AutoGenerateColumns="false" DataKeyNames="TB_NOTIFICACAO_ID" OnSelectedIndexChanged="grdNotificacao_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false"></asp:TemplateField>
                            <asp:BoundField DataField="TB_NOTIFICACAO_ID" HeaderText="Id" ItemStyle-Width="50px"
                                DataFormatString="{0:D4}">
                                <ItemStyle Width="50px"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_PERFIL_DESCRICAO" HeaderText="Perfil" ItemStyle-Width="15%">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_NOTIFICACAO_TITULO" HeaderText="Titulo" ItemStyle-Width="20%">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_NOTIFICACAO_MENSAGEM" HeaderText="Mensagem" ItemStyle-Width="50%">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_NOTIFICACAO_DATA" HeaderText="DATA" ItemStyle-Width="10%">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_NOTIFICACAO_IND_ATIVO" HeaderText="Ativo" ItemStyle-Width="3%">
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("TB_NOTIFICACAO_ID") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="sourceGridPerfil" runat="server" SelectMethod="ListarNotificacao"
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.NotificacaoPresenter"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="ddlAtivoConsulta" Name="ativo" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="ddlPerfilConsulta" Name="perfilId" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Visible="false" Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();"
                            Text="Ajuda" CssClass="" AccessKey="A" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Seguranca/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="Label9" runat="server" class="labelFormulario" Width="150px" Text="Perfil:*"></asp:Label>
                                    <asp:DropDownList ID="ddlPerfilEdit" DataSourceID="odsPerfil" runat="server" DataTextField="TB_PERFIL_DESCRICAO"
                                        Width="500px" DataValueField="TB_PERFIL_ID">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsPerfil" runat="server" SelectMethod="SelectAllComTodos"
                                        TypeName="Sam.Presenter.PerfilPresenter" OldValuesParameterFormatString="original_{0}">
                                        <SelectParameters>
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </p>
                                <p>
                                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="150px" Text="Titulo Mensagem:*"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtTituloMensagem" size="20" Width="80%"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="150px" Text="Mensagem:*"></asp:Label>
                                    <asp:TextBox CausesValidation="false" SkinID="MultiLine" Height="300px" TextMode="MultiLine"
                                        Rows="100" runat="server" ID="txtMensagem" Width="80%"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label1" CssClass="labelFormulario" runat="server" Width="150px" Text="Ativo:" />
                                    <asp:DropDownList ID="ddlAtivo" runat="server" Width="150px">
                                        <asp:ListItem Text="Sim" Value="True"></asp:ListItem>
                                        <asp:ListItem Text="Não" Value="False"></asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                            </fieldset>
                        </div>
                        <div>
                            <p>
                                <small>Os campos marcados com (*) são obrigatórios. </small>
                            </p>
                        </div>
                    </div>
                    <div class="Divbotao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                    OnClick="btnExcluir_Click" Visible="true" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
