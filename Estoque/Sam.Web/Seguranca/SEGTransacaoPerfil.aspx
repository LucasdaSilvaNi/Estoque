<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SEGTransacaoPerfil.aspx.cs" Inherits="Sam.Web.Seguranca.SEGTransacaoPerfil" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <div id="content">
        <h1>
            Módulo Segurança - Transação Perfil - Cadastro de Transações Perfil</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>
                <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="Label2" CssClass="labelFormulario" Text="Módulo:" Width="100px" runat="server" />
                            <asp:DropDownList ID="ddlModulo" DataTextField="Descricao" DataValueField="Id" Width="80%"
                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlModulo_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                        <p>
                                    <asp:Label ID="Label1" runat="server" class="labelFormulario" Width="100px" Text="Perfil:"></asp:Label>
                                    <asp:DropDownList ID="ddlPerfil" DataSourceID="oPerfil" runat="server" DataTextField="TB_PERFIL_DESCRICAO" AutoPostBack="True"
                                        Width="80%" DataValueField="TB_PERFIL_ID">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="oPerfil" runat="server" SelectMethod="SelectAll"                                        
                                        TypeName="Sam.Presenter.PerfilPresenter"
                                        OldValuesParameterFormatString="original_{0}">
                                        <SelectParameters>                                            
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </p>
                        <p>
                        <asp:Label ID="Label4" runat="server" class="labelFormulario" Width="100px" Text="Ativo:"></asp:Label>
                                    <asp:DropDownList ID="ddlFiltroAtivo" runat="server" DataTextField="Descricao" Width="155px" AutoPostBack="True"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>

                    </fieldset>
                    <asp:GridView SkinID="GridNovo" ID="grdTransacaoPerfil" runat="server" AllowPaging="True"
                        AutoGenerateColumns="false" DataKeyNames="TB_TRANSACAO_PERFIL_ID" OnSelectedIndexChanged="grdTransacaoPerfil_SelectedIndexChanged">
                        <Columns>                            
                            <asp:BoundField DataField="TB_TRANSACAO_PERFIL_ID" HeaderText="Id" ItemStyle-Width="20px" Visible="false"
                                DataFormatString="{0:D4}">
                                <ItemStyle Width="50px"></ItemStyle>
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Transação">
                            <ItemTemplate>
                            <%#DataBinder.Eval(Container.DataItem, "TB_TRANSACAO.TB_TRANSACAO_DESCRICAO")%>
                            </ItemTemplate>
                            </asp:TemplateField>                            
                            <asp:TemplateField HeaderText="Perfil">
                            <ItemTemplate>
                            <%#DataBinder.Eval(Container.DataItem, "TB_PERFIL.TB_PERFIL_DESCRICAO")%>
                            </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TB_TRANSACAO_ATIVO" HeaderText="Ativo" ItemStyle-Width="20px">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_TRANSACAO_PERFIL_EDITA" HeaderText="Edita" ItemStyle-Width="20px">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_TRANSACAO_FILTRA_COMBO" HeaderText="Filtra C." ItemStyle-Width="20px">
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="20px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("TB_TRANSACAO_PERFIL_ID") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="sourceGridAlmoxarifado" runat="server" SelectMethod="ListarTransacaoPerfil"
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.TransacaoPerfilPresenter"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="ddlModulo" Name="moduloId" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="ddlPerfil" Name="perfilId" PropertyName="SelectedValue" />
                            <asp:ControlParameter ControlID="ddlFiltroAtivo" Name="ativo" PropertyName="SelectedValue" />
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
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" />
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
                                    <asp:ObjectDataSource ID="odsPerfil" runat="server" SelectMethod="SelectAll"                                        
                                        TypeName="Sam.Presenter.PerfilPresenter"
                                        OldValuesParameterFormatString="original_{0}">
                                        <SelectParameters>                                            
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </p>                                
                                <p>
                                    <asp:Label ID="Label8" runat="server" class="labelFormulario" Width="150px" Text="Transação:*"></asp:Label>
                                    <asp:DropDownList ID="ddlTransacao" DataSourceID="odsTransacao" runat="server" DataTextField="TB_TRANSACAO_DESCRICAO"
                                        Width="500px" DataValueField="TB_TRANSACAO_ID">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsTransacao" runat="server" SelectMethod="SelectAll"
                                        TypeName="Sam.Presenter.TransacaoPresenter"
                                        OldValuesParameterFormatString="original_{0}">
                                    </asp:ObjectDataSource>
                                </p>
                                <p>
                                    <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="150px" Text="Ind. de Atividade:*"></asp:Label>
                                    <asp:DropDownList ID="ddlAtividade" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label10" runat="server" class="labelFormulario" Width="150px" Text="Edita:*"></asp:Label>
                                    <asp:DropDownList ID="ddlEdita" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Sim</asp:ListItem>
                                        <asp:ListItem Value="False">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label11" runat="server" class="labelFormulario" Width="150px" Text="Filtra Combo:*"></asp:Label>
                                    <asp:DropDownList ID="ddlFiltraCombo" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Sim</asp:ListItem>
                                        <asp:ListItem Value="False">Não</asp:ListItem>
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
    </div>
</asp:Content>
