<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SEGModulo.aspx.cs" Inherits="Sam.Web.Seguranca.SEGModulo" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <div id="content">
        <h1>
            Módulo Segurança - Módulo - Cadastro de Módulos</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>
                <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                    <asp:GridView SkinID="GridNovo" ID="grdModulo" runat="server" AllowPaging="True"
                        AutoGenerateColumns="false" DataKeyNames="TB_MODULO_ID" OnSelectedIndexChanged="grdModulo_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false"></asp:TemplateField>
                            <asp:TemplateField HeaderText="Sistema" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "TB_SISTEMA.TB_SISTEMA_SIGLA")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="TB_MODULO_DESCRICAO" HeaderText="Descrição" ItemStyle-Width="200px">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MODULO_CAMINHO" HeaderText="Caminho" ItemStyle-Width="300px">
                                <ItemStyle Width="300px"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MODULO_SIGLA" HeaderText="Sigla." ItemStyle-Width="100px">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MODULO_ID_PAI" HeaderText="Pai" ItemStyle-Width="50px">
                                <ItemStyle Width="50px"></ItemStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MODULO_ORDEM" HeaderText="Ordem" ItemStyle-Width="50px">
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MODULO_IND_ATIVO" HeaderText="Ativo" ItemStyle-Width="20px">
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("TB_MODULO_ID") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="sourceGridModulo" runat="server" SelectMethod="ListarModulo"
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.ModuloPresenter"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
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
                                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="150px" Text="Sistema:*"></asp:Label>
                                    <asp:DropDownList ID="ddlSistema" DataSourceID="odsSistema" runat="server" DataTextField="TB_SISTEMA_DESCRICAO"
                                        Width="500px" DataValueField="TB_SISTEMA_ID">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsSistema" runat="server" SelectMethod="SelectAll" TypeName="Sam.Presenter.SistemaPresenter"
                                        OldValuesParameterFormatString="original_{0}"></asp:ObjectDataSource>
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
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="150px" Text="Sigla*:"
                                        Font-Bold="True" />
                                    <asp:TextBox runat="server" ID="txtSigla" size="30" MaxLength="30"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="150px" Text="Descrição*:"
                                        Font-Bold="True" />
                                    <asp:TextBox runat="server" ID="txtDescricao" Width="510px" SkinID="MultiLine" onkeyup='limitarTamanhoTexto(this,256)'
                                        MaxLength="100" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="150px" Text="Caminho*:"
                                        Font-Bold="True" />
                                    <asp:TextBox runat="server" ID="txtCaminho" Width="500px" MaxLength="255"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="150px" Text="Ordem:"
                                        Font-Bold="True" />
                                    <asp:TextBox runat="server" ID="txtOrdem" size="4" MaxLength="4" CssClass="inputFromNumero"
                                        onkeypress='return SomenteNumero(event)'></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="150px" Text="Pai:"
                                        Font-Bold="True" />
                                    <asp:TextBox runat="server" ID="txtPai" size="4" MaxLength="4" CssClass="inputFromNumero"
                                        onkeypress='return SomenteNumero(event)'></asp:TextBox>
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
