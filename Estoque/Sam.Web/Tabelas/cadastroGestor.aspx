<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Gestor" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroGestor.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroGestor"
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Estrutura Organizacional - Gestor</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <fieldset class="fieldset">
            <div id="Div3">
                <p>
                    <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="100px" Text="Órgão*:" Font-Bold="true" />
                        <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                            DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="PopularListaOrgaoTodosCod" 
                        TypeName="Sam.Presenter.GestorPresenter">
                        </asp:ObjectDataSource>
                </p>
            </div>
        </fieldset>
        <br />
        <asp:GridView ID="gridGestor" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridGestor_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                        <asp:Label runat="server" ID="lblIdUo" Text='<%# Bind("Uo.Id") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblTipoId" Text='<%# Bind("TipoId") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblIdUge" Text='<%# Bind("Uge.Id") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblOrgId" Text='<%# Bind("Orgao.Id") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblNomeReduzido" Text='<%# Bind("NomeReduzido") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblLogradouro" Text='<%# Bind("EnderecoLogradouro") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblCompl" Text='<%# Bind("EnderecoCompl") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblNumero" Text='<%# Bind("EnderecoNumero") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblTelefone" Text='<%# Bind("EnderecoTelefone") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblCodigoGestao" Text='<%# Bind("CodigoGestao") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Nome" HeaderText="Nome" />
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
        <asp:ObjectDataSource ID="sourceGridGestor" runat="server"
            SelectMethod="PopularDadosGestor" EnablePaging="True" 
            MaximumRowsParameterName="maximumRowsParameterName" 
            SelectCountMethod="TotalRegistros" 
            StartRowIndexParameterName="startRowIndexParameterName" 
            TypeName="Sam.Presenter.GestorPresenter" 
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                    Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                    Text="Imprimir" AccessKey="I" />
                <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                    AccessKey="A" />
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
                            <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="108px" 
                                Text="Nome*:" />
                            <asp:TextBox ID="txtNome" runat="server" MaxLength="120" size="90" Width="80%" />
                        </p>
                        <p>
                            <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Nome Reduzido*:" />
                            <asp:TextBox ID="txtNomeReduzido" runat="server" MaxLength="25" size="25" Width="200px" />
                        </p>
                        <p>
                            <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Logradouro*:" />
                            <asp:TextBox ID="txtLogradouro" runat="server" MaxLength="120" size="90" Width="80%" />
                        </p>
                        <p>
                            <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Número*:" />
                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="10" size="10" />
                        </p>
                        <p>
                            <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Complemento:" />
                            <asp:TextBox ID="txtComplemento" runat="server" MaxLength="10" size="30" 
                                Width="200px" />
                        </p>
                            <p>
                            <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Telefone*:" />
                            <asp:TextBox ID="txtTelefone" runat="server" MaxLength="20" size="20" CssClass="telefone" Width="150px" />
                        </p>
                            <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Código Gestão*:" />                                

                            <asp:TextBox ID="txtCodigoGestao" runat="server"  onblur="preencheZeros(this,'5')"  MaxLength="5" size="5" CssClass="inputFromNumero"
                                Width="57px"></asp:TextBox>
                        </p>
                            <p>
                            <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="Tipo*:"></asp:Label>
                            <asp:RadioButtonList ID="rblTipo" runat="server" AutoPostBack="True" 
                                onselectedindexchanged="rblTipo_SelectedIndexChanged" 
                                RepeatDirection="Horizontal" Width="200px" Height="17px">
                                <asp:ListItem Value="0">Órgao</asp:ListItem>
                                <asp:ListItem Value="1">UO</asp:ListItem>
                                <asp:ListItem Value="2">UGE</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="lblTipo" runat="server" CssClass="labelFormulario" Width="110px" 
                                Text="" />
                            <asp:DropDownList ID="ddlUo" runat="server" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id" Width="80%">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUo" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="PopularDadosTodosCod" 
                                TypeName="Sam.Presenter.UOPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>

                            </asp:ObjectDataSource>
                            <asp:DropDownList ID="ddlUGE" runat="server" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id" Width="80%">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUGE" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="PopularDadosTodosCod" 
                                TypeName="Sam.Presenter.UGEPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                            </p>
                            <br>
                            <p>
                            <asp:Label ID="Label10" runat="server" class="labelFormulario" Text="Logotipo:" 
                                Width="110px"></asp:Label>
                            
                            <asp:Image ID="imgGestor" runat="server" EnableTheming="True" Width="100px" 
                                ImageUrl="~/Imagens/imgNaoCadastrada.gif" />
                            </p>
                            <p>
                            <asp:Label ID="Label11" runat="server" class="labelFormulario" Text="Carregar Imagem:" 
                                Width="110px"></asp:Label>
                            <asp:UpdatePanel ID="updFileUp" runat="server" style="text-align:left">
                                <ContentTemplate>
                                    <asp:FileUpload ID="fileUploadGestor" runat="server" Width="80%" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnGravar" />
                                </Triggers>
                            </asp:UpdatePanel>
                         </p>
                    </fieldset>
                </div>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <p>
                    <small>Os campos marcados com (*) são obrigatórios. </small>
                </p>
            </div>
            <!-- fim id interno -->
            <div class="Divbotao">
                <!-- simula clique link editar/excluir -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                        <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                            OnClick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                            OnClick="btnCancelar_Click" />
                    </p>
                </div>
            </div>
        </asp:Panel>
        <br />
    </ContentTemplate>
    </asp:UpdatePanel>

    </div>
</asp:Content>
