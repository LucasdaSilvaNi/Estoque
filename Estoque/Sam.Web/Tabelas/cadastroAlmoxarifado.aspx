<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Almoxarifado" Language="C#"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroAlmoxarifado.aspx.cs"
    Inherits="Sam.Web.Seguranca.cadastroAlmoxarifado" ValidateRequest="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Estrutura Organizacional - Almoxarifados</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <div id="Div3">
                        <p>
                            <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="100px" Text="Órgão*:"
                                Font-Bold="true"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True" DataTextField="CodigoDescricao"
                                DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged" OnDataBound="ddlOrgao_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularListaOrgaoTodosCod" TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="100px" Text="Gestor*:"
                                Font-Bold="true"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True" DataTextField="CodigoDescricao"
                                DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged" OnDataBound="ddlGestor_DataBound">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularListaGestorTodosCod" TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                    </div>
                    <p>
                        <asp:Label ID="Label17" runat="server" CssClass="labelFormulario" Width="100px" Text="Código Almoxarifado:"
                            Font-Bold="true"></asp:Label>
                        <asp:TextBox ID="txtAlmoxarifadoMulti" CssClass="txtAlmoxarifadoMulti" runat="server"
                            Columns="10" Rows="10" TextMode="MultiLine" Width="60%" onkeypress="return NumeroVirgula( this , event ) ;"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" Text="Buscar" />
                    </p>
                </fieldset>
                <br />
                <asp:GridView ID="gridAlmoxarifado" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridAlmoxarifado_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                    CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                <asp:Label runat="server" ID="lblLogradouro" Text='<%# Bind("EnderecoLogradouro") %>'
                                    Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblNumero" Text='<%# Bind("EnderecoNumero") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblCompl" Text='<%# Bind("EnderecoCompl") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblBairro" Text='<%# Bind("EnderecoBairro") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblMunicipio" Text='<%# Bind("EnderecoMunicipio") %>'
                                    Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblUfId" Text='<%# Bind("Uf.Id") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblCep" Text='<%# Bind("EnderecoCep") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblTelefone" Text='<%# Bind("EnderecoTelefone") %>'
                                    Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblFax" Text='<%# Bind("EnderecoFax") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblResponsavel" Text='<%# Bind("Responsavel") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblIdUge" Text='<%# Bind("Uge.Id") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblrefInicial" Text='<%# Bind("RefInicial") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblrefFaturamento" Text='<%# Bind("RefFaturamento") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblTipoAlmoxarifado" Text='<%# Bind("TipoAlmoxarifado") %>' Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblIndicadorAtividadeId" Text='<%# Bind("IndicadorAtividade.Id") %>'
                                    Visible="false"></asp:Label>
                                <asp:Label runat="server" ID="lblIgnoraCalendarioSiafemParaReabertura" Text='<%# Bind("IgnoraCalendarioSiafemParaReabertura") %>'
                                    Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D3}">
                            <ItemStyle Width="50px"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição"></asp:BoundField>
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
                <asp:ObjectDataSource ID="sourceGridAlmoxarifado" runat="server" SelectMethod="PopularDadosAlmoxarifado"
                    EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                    StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.AlmoxarifadoPresenter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue"
                            Type="Int32" />
                        <asp:ControlParameter ControlID="txtAlmoxarifadoMulti" Name="_almoxarifadoCodigo"
                            PropertyName="Text" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();"
                            Text="Ajuda" CssClass="" AccessKey="A" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar">
                    <br />
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="200px" Text="Código*:"></asp:Label>
                                    <asp:TextBox ID="txtCodigo" MaxLength="3" runat="server" size="3" CssClass="inputFromNumero"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="200px" Text="Descrição*:"></asp:Label>
                                    <asp:TextBox ID="txtNome" runat="server" MaxLength="120" size="120" Width="400px"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="200px" Text="Endereço*:"></asp:Label>
                                    <asp:TextBox ID="txtLogradouro" runat="server" MaxLength="120" Width="400px"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="200px" Text="Número*:"></asp:Label>
                                    <asp:TextBox ID="txtNumero" runat="server" MaxLength="10" size="10"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="200px" Text="Complemento:"></asp:Label>
                                    <asp:TextBox ID="txtComplemento" runat="server" MaxLength="10" size="30"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="200px" Text="Bairro*:"></asp:Label>
                                    <asp:TextBox ID="txtBairro" runat="server" MaxLength="45" size="45"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="200px" Text="Município*:"></asp:Label>
                                    <asp:TextBox ID="txtMunicipio" runat="server" MaxLength="45" size="45"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="200px" Text="UF*:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlUf" Width="150px" AutoPostBack="True" DataTextField="Descricao"
                                        DataValueField="Id">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="sourceListaUf" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="PopularListaUf" TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                                    </asp:ObjectDataSource>
                                </p>
                                <p>
                                    <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="200px" Text="CEP*:"></asp:Label>
                                    <asp:TextBox ID="txtCep" runat="server" MaxLength="10" size="10" CssClass="cep"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="200px" Text="Telefone:"></asp:Label>
                                    <asp:TextBox ID="txtTelefone" runat="server" MaxLength="30" size="30" CssClass="telefone"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="200px" Text="Fax:"></asp:Label>
                                    <asp:TextBox ID="txtFax" runat="server" MaxLength="30" size="30" CssClass="telefone"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="200px" Text="Responsável*:"></asp:Label>
                                    <asp:TextBox ID="txtResponsavel" runat="server" MaxLength="50" size="50" Width="400px"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="200px" Text="UGE*:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlUGE" Width="400px" AutoPostBack="True" DataTextField="CodigoDescricao"
                                        DataValueField="Id" OnDataBound="ddlUGE_DataBound">
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="200px" Text="Mês Ref. Inicial*:"></asp:Label>
                                    <asp:TextBox ID="txtRefInicial" runat="server" MaxLength="30" size="10" CssClass="mesAno"></asp:TextBox>(AAAA/MM)
                                </p>
                                <p>
                                    <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Width="200px" Text="Mês Ref. Fatumento:"></asp:Label>
                                    <asp:TextBox ID="txtRefFaturamento" runat="server" MaxLength="30" size="10" CssClass="mesAno"></asp:TextBox>(AAAA/MM)
                                </p>
                                <p>
                                    <asp:Label ID="Label19" runat="server" CssClass="labelFormulario" Width="200px" Text="Tipo de Almoxarifado:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlTipoAlmoxarifado" Width="150px">
                                        <asp:ListItem Text="Selecione" Value="" />
                                        <asp:ListItem Text="Principal" Value="Principal" />
                                        <asp:ListItem Text="Secundário" Value="Complementar" />
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="200px" Text="Ind. de Atividade*:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlIndicadorAtividade" Width="100px" AutoPostBack="True">
                                        <asp:ListItem Text="Ativo" Value="1" />
                                        <asp:ListItem Text="Inativo" Value="0" />
                                    </asp:DropDownList>
                                </p>
                                <p id="campoIgnoraCalendarioSiafemParaReabertura" runat="server">
                                    <asp:Label ID="lblIgnoraCalendarioSiafemParaReabertura" runat="server" CssClass="labelFormulario"
                                        Width="200px" Text="Ignorar Calendário SIAFEM*:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlIgnoraCalendarioSiafemParaReabertura" Width="100px"
                                        AutoPostBack="True">
                                        <asp:ListItem Text="Sim" Value="1" />
                                        <asp:ListItem Text="Não" Value="0" Selected="True" />
                                    </asp:DropDownList>
                                </p>
                            </fieldset>
                        </div>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <br />
                    <!-- fim id interno -->
                    <div class="Divbotao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" runat="server" Text="Gravar" CssClass="button" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnExcluir" runat="server" Text="Excluir" CssClass="button" OnClick="btnExcluir_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
    <script language="javascript">
        function NumeroVirgula(obj, e) {
            var tecla = (window.event) ? e.keyCode : e.which;
            if (tecla == 8 || tecla == 0)
                return true;
            if (tecla != 44 && tecla < 48 || tecla > 57)
                return false;
        }

    </script>
</asp:Content>
