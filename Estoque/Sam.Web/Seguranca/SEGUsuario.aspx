<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SEGUsuario.aspx.cs" Inherits="Sam.Web.Seguranca.SEGUsuario" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <div id="content">
        <h1>Módulo Segurança - Usuário - Cadastro de Usuários</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>
                <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                    <fieldset class="fieldset">
                        <p style="display: none">
                            <asp:Label CssClass="labelFormulario" Text="Sistema:" Width="120px" runat="server" />
                            <asp:DropDownList ID="ddlSistema" DataTextField="Descricao" DataValueField="SistemaId"
                                Width="80%" runat="server" />
                        </p>
                        <p>
                            <asp:Label ID="Label1" CssClass="labelFormulario" Text="Órgão:" Width="120px" runat="server" />
                            <asp:DropDownList ID="ddlOrgao" DataTextField="CodigoDescricao" DataValueField="Id" Width="80%"
                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged" />
                        </p>
                        <p>
                            <asp:Label ID="Label3" CssClass="labelFormulario" Text="Gestor:" Width="120px" runat="server" />
                            <asp:DropDownList ID="ddlGestor" DataTextField="CodigoDescricao" DataValueField="Id" Width="80%"
                                runat="server" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged1" AutoPostBack="True" />
                        </p>

                        <p>
                            <asp:Label ID="Label4" CssClass="labelFormulario" Text="UGE:" Width="120px" runat="server" />
                            <asp:DropDownList ID="cboUge" DataTextField="Descricao" DataValueField="Id" Width="80%"
                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="cboUge_SelectedIndexChanged" />
                        </p>

                         <p>
                            <asp:Label ID="Label5" CssClass="labelFormulario" Text="Almoxarifado:" Width="120px" runat="server" />
                            <asp:DropDownList ID="ddlAlmoxarifado" DataTextField="TB_ALMOXARIFADO_DESCRICAO" DataValueField="TB_ALMOXARIFADO_ID" Width="80%"
                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlAlmoxarifado_SelectedIndexChanged" />
                        </p>

                        <p>
                            <asp:Label ID="Label19" CssClass="labelFormulario" Text="Perfil:" Width="120px" runat="server" />
                            <asp:DropDownList ID="ddlPerfil" DataTextField="Descricao" DataValueField="IdPerfil"
                                Width="80%" runat="server" OnSelectedIndexChanged="ddlPerfil_SelectedIndexChanged"
                                AutoPostBack="True" AppendDataBoundItems="True" />
                        </p>
                        <p>
                            <table border="0" width="100%">
                                <tr>
                                    <td width="80%">
                                        <asp:Label ID="Label20" style="font-size: 13px" CssClass="labelFormulario" Text="Pesquisar:" Width="120px" runat="server" />
                                        <asp:TextBox  style=" display:inline;  width: 100px !important; position: absolute; left: 95px; top: 0px!important" runat="server" ID="txtPesquisar" Text="" Width="20%"></asp:TextBox>
                                        <asp:Button runat="server" Text="Pesquisar" SkinID="Btn120" ID="btnPesquisar"
                                            OnClick="btnPesquisar_Click" /> <asp:Label ID="Label7" style="font-size: 9px" runat="server" Text="Pesquisa por: Nome, Cpf, E-mail, ou ' '."></asp:Label>  
                                    </td>
                                </tr>
                            </table>

                        </p>

                    </fieldset>
                    <asp:GridView ID="gridUsuario" runat="server" AutoGenerateColumns="False" AllowPaging="True"
                        OnSelectedIndexChanged="gridUsuario_SelectedIndexChanged" OnRowCommand="gridUsuario_RowCommand"
                        OnPageIndexChanging="gridUsuario_PageIndexChanging">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                        CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                    <asp:Label ID="lblId" Text='<%# Bind("Id") %>' runat="server" />
                                    <asp:Label ID="lblCpf" Text='<%# Bind("Cpf") %>' runat="server" />
                                    <asp:Label ID="lblNome" Text='<%# Bind("NomeUsuario") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CPF" Visible="true" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:Label ID="ItemMaterialCodigo" Text='<%# Bind("Cpf") %>' runat="server" Style="padding-left: 2px; padding-right: 2px" />
                                </ItemTemplate>
                                <ItemStyle Width="80px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Usuário" Visible="true" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubItemMaterialCodigo" Text='<%# Eval("NomeUsuario").ToString().ToUpper() %>' runat="server" Style="padding-left: 3px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Telefone" Visible="true" ItemStyle-HorizontalAlign="right">
                                <ItemTemplate>
                                    <asp:Label ID="lblTelefone" Text='<%# Bind("Fone") %>' runat="server" CssClass="telefone" Style="padding-right: 3px" />
                                </ItemTemplate>
                                <ItemStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="e-Mail" Visible="true" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmail" Text='<%# Bind("Email") %>' runat="server" Style="padding-left: 3px" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Perfil" Visible="true" ItemStyle-HorizontalAlign="left">
                                <ItemTemplate>
                                    <asp:Label ID="lblDescPerfil" Text='<%# DataBinder.Eval(Container.DataItem, "Login.Perfil.DescricaoComCodigoEstrutura") %>' runat="server" Style="padding-left: 3px" />
                                </ItemTemplate>
                                <ItemStyle Width="270px" />
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="Dt. Últ. Acesso" Visible="true" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblDataUltimoAcesso" Text='<%# DataBinder.Eval(Container.DataItem, "Login.DataUltimoAcesso", "{0:dd/MM/yy HH:mm}")  %>' runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="90px" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ativo" Visible="true" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblAtivo" Text='<%# (bool)Eval("UsuarioAtivo") ? "Sim" : "Não" %>' runat="server" />
                                </ItemTemplate>
                                <ItemStyle Width="50px" />
                            </asp:TemplateField>--%>
                            <asp:ButtonField ButtonType="Image" CommandName="perfil" HeaderText="Perfil" HeaderStyle-VerticalAlign="Middle"
                                ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center" ImageUrl="~/Imagens/perfil.gif"
                                ItemStyle-Width="50px">
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
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>


                </div>
                <asp:ObjectDataSource ID="sourceListaGridUsuario" runat="server" EnablePaging="True"
                    StartRowIndexParameterName="startRowIndexParameterName" MaximumRowsParameterName="maximumRowsParameterName"
                    SelectCountMethod="TotalRegistros" SelectMethod="ListarUsuarios" TypeName="Sam.Presenter.UsuarioPresenter">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlSistema" Name="_sistemaId" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <div class="DivButton">
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" Text="Imprimir" AccessKey="I"
                            OnClick="btnImprimir_Click" Visible="False" />
                        <asp:Button ID="btnAjuda" runat="server" Text="Ajuda" CssClass="" AccessKey="A" OnClientClick="OpenModal();" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Seguranca/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="100px" Text="CPF*:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtCPF" runat="server" MaxLength="11" size="11" CssClass="cpf" Width="120px" />
                                </p>
                                <p>
                                    <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="100px" Text="Nome*:"
                                        Font-Bold="True" />
                                    <asp:TextBox ID="txtNome" runat="server" MaxLength="200" Width="80%" />
                                </p>
                                <p>
                                    <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="100px" Text="Senha*:"
                                        Font-Bold="True" />
                                    <asp:TextBox ID="txtSenha" runat="server" MaxLength="20" Width="120px" />
                                    <asp:Button ID="btnGerarSenha" runat="server" Text="Gerar Senha" OnClientClick="OpenModalSenhaWs()"
                                        OnClick="btnGerarSenha_Click" />
                                </p>
                                <p>
                                    <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="100px" Text="Email:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" Width="80%" />
                                    <asp:RegularExpressionValidator ID="regExEmail" runat="server" Display="Dynamic"
                                        ErrorMessage="Email inválido." ControlToValidate="txtEmail" ValidationExpression='^(([^<>()[\]\\.,;:\s@""]+(\.[^<>()[\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$'>
                                    </asp:RegularExpressionValidator>
                                </p>

                                <p>
                                    <asp:Label ID="Label17" runat="server" CssClass="labelFormulario" Width="100px" Text="Telefone:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtTelefone" CssClass="telefone" Width="200px" size="10" runat="server" />
                                </p>

                                <%--     <p>
                                    <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="100px" Text="RG:"
                                        Font-Bold="true" />
                                    <asp:TextBox ID="txtRG" runat="server" MaxLength="20" Width="120px" />
                                </p>
                                --%>
                                <%--<p class="sideduplo">
                                        <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="100px" Text="Órgão Emissor:"
                                            Font-Bold="true" />
                                        <asp:TextBox ID="txtOrgaoEmissor" runat="server" MaxLength="5" Width="120px" />
                                    </p>
                                    <p class="sideduplo">
                                        <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="100px" Text="UF Emissor:"
                                            Font-Bold="true" />
                                        <asp:DropDownList ID="ddlUFEmissor" DataTextField="Sigla" DataValueField="Sigla"
                                            runat="server" Width="80px" />
                                    </p--%>

                                <p>
                                    <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Width="100px" Text="Usuário Ativo:"
                                        Font-Bold="true" />
                                    <asp:DropDownList ID="ddlAtivo" Width="60px" runat="server">
                                        <asp:ListItem Text="Não" Value="0" />
                                        <asp:ListItem Text="Sim" Value="1" Selected="True" />
                                    </asp:DropDownList>
                                </p>



                                <%--             --%>
                                <%-- <fieldset class="fieldset">--%>
                                <%--          <legend>Endereço</legend>
                                        <p>
                                            <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="100px" Text="Logradouro*:"
                                                Font-Bold="True" />
                                            <asp:TextBox ID="txtLogradouro" Width="80%" MaxLength="200" runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="100px" Text="Número*:"
                                                Font-Bold="True" />
                                            <asp:TextBox ID="txtNumero" Width="100px" MaxLength="5" onkeypress='return SomenteNumero(event)'
                                                runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="100px" Text="Complemento:"
                                                Font-Bold="true" />
                                            <asp:TextBox ID="txtComplemento" Width="80%" MaxLength="30" runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="100px" Text="Bairro*:"
                                                Font-Bold="True" />
                                            <asp:TextBox ID="txtBairro" Width="80%" MaxLength="50" runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="100px" Text="Município*:"
                                                Font-Bold="True" />
                                            <asp:TextBox ID="txtMunicipio" Width="80%" MaxLength="50" runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="100px" Text="UF*:"
                                                Font-Bold="True" />
                                            <asp:DropDownList ID="ddlUF" Width="100px" DataTextField="Sigla" DataValueField="Sigla"
                                                runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="100px" Text="CEP*:"
                                                Font-Bold="True" />
                                            <asp:TextBox ID="txtCEP" Width="100px" MaxLength="8" CssClass="cep" runat="server" />
                                        </p>
                                        <p>
                                            <asp:Label ID="Label17" runat="server" CssClass="labelFormulario" Width="100px" Text="Telefone:"
                                                Font-Bold="true" />
                                            <asp:TextBox ID="txtTelefone" CssClass="telefone" Width="200px" size="10" runat="server" />
                                        </p>--%>


                                <%--          </fieldset>--%>
                            </fieldset>
                        </div>
                        <div>
                            <p>
                                <small>Os campos marcados com (*) são obrigatórios. </small>
                            </p>
                        </div>
                    </div>
                    <br />
                    <br />
                    <!-- fim id interno -->
                    <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                    <div id="DivBotoes" class="DivButton">
                        <p class="botaoLeft">
                            <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" AccessKey="G"
                                OnClick="btnGravar_Click" Enabled="False" />
                            <asp:Button ID="btnCancelar" CssClass="" runat="server" Text="Cancelar" AccessKey="C"
                                OnClick="btnCancelar_Click" Enabled="False" />
                        </p>
                    </div>

                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="relativeButton">
            <p>
                <asp:Button runat="server" Text="Gerar Excel" ID="btnExportarExcel"
                    OnClick="btnExportarExcel_Click" style=" display:inline;  width: 100px !important; position: absolute; left: 460px; top: 0px!important" />
            </p>
        </div>
    </div>

    <div id="dialogSenhaWS2" title="Senha" style="visibility: hidden">
        <asp:UpdatePanel ID="updMsgSenha" runat="server" Visible="false">
            <ContentTemplate>
                <asp:Label ID="lblMsgSenha" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
