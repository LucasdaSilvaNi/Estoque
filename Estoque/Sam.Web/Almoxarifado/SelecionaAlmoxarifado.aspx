<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Almoxarifado" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="SelecionaAlmoxarifado.aspx.cs" Inherits="Sam.Web.Seguranca.perfilAlmoxarifado"
 ValidateRequest="false"    %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
   
    <div id="content">
        <h1>
            Módulo Almoxarifado - Seleciona Almoxarifado</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <fieldset Class="fieldset">
          <div id="Div3">
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="100px" Text="Órgão*:"
                    Font-Bold="true"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                    DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularListaOrgaoTodosCod" 
                    TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                </asp:ObjectDataSource>
          </p>
          <p>
                <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="100px" Text="Gestor*:"
                    Font-Bold="true"></asp:Label>
                <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True"
                    DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="sourceListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularListaGestorTodosCod"
                    TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlOrgao" 
                            Name="_orgaoId" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
         </p>
            </div>
        </fieldset>
        
        <asp:ObjectDataSource ID="sourceGridAlmoxarifado" runat="server"
        SelectMethod="PopularListaAmoxarifadoPerfil" EnablePaging="True" 
        SelectCountMethod="TotalRegistros"
        MaximumRowsParameterName="maximumRowsParameterName" 
        StartRowIndexParameterName="startRowIndexParameterName" 
        TypeName="Sam.Presenter.AlmoxarifadoPresenter" 
        OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="codigo" Name="_almoxarifadoId" 
                    PropertyName="Text" Type="Int32" />
            </SelectParameters> 
        </asp:ObjectDataSource>
            <asp:GridView ID="gridAlmoxarifado" runat="server" AllowPaging="True"
                OnSelectedIndexChanged="gridAlmoxarifado_SelectedIndexChanged" 
                CaptionAlign="Left" onrowcommand="gridAlmoxarifado_RowCommand" >
                <Columns>
                <asp:TemplateField ItemStyle-Width="80px" HeaderText="Selecionar">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelecionar" Text="Selecionar" CommandName="cmdSelecionar" runat="server" CommandArgument='<%# Bind("Id") %>' OnClick="gridAlmoxarifado_SelectedIndexChanged"
                            OnClientClick="return alert('Almoxarifado e Gestor foram alterados com sucesso!');" />
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False" Visible="false">
                        <ItemTemplate>

                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                                CommandName="Select" Font-Bold="true" Text='<%# Bind("Id") %>'></asp:LinkButton>
                            <asp:Label ID="lblLogradouro" runat="server" 
                                Text='<%# Bind("EnderecoLogradouro") %>' Visible="false"></asp:Label>
                            <asp:Label ID="lblNumero" runat="server" Text='<%# Bind("EnderecoNumero") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblCompl" runat="server" Text='<%# Bind("EnderecoCompl") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblBairro" runat="server" Text='<%# Bind("EnderecoBairro") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblMunicipio" runat="server" 
                                Text='<%# Bind("EnderecoMunicipio") %>' Visible="false"></asp:Label>
                            <asp:Label ID="lblUfId" runat="server" Text='<%# Bind("Uf.Id") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblCep" runat="server" Text='<%# Bind("EnderecoCep") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblTelefone" runat="server" 
                                Text='<%# Bind("EnderecoTelefone") %>' Visible="false"></asp:Label>
                            <asp:Label ID="lblFax" runat="server" Text='<%# Bind("EnderecoFax") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblResponsavel" runat="server" Text='<%# Bind("Responsavel") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblIdUge" runat="server" Text='<%# Bind("Uge.Id") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblrefInicial" runat="server" Text='<%# Bind("RefInicial") %>' 
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblIndicadorAtividadeId" runat="server" 
                                Text='<%# Bind("IndicadorAtividade.Id") %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Codigo" DataFormatString="{0:D3}" HeaderText="Cód." 
                        ItemStyle-Width="50px">
                    <ItemStyle Width="50px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                    
<%--                    <asp:CommandField AccessibleHeaderText="Selecionar" CancelText="" DeleteText=""  
                        EditText="" InsertText="" NewText="" SelectText="Selecionar"
                        ShowSelectButton="True" UpdateText="Selecionar">
                    <ItemStyle Width="30px" />
                    </asp:CommandField>--%>
                </Columns>
                <HeaderStyle CssClass="corpo" />
            </asp:GridView>
            <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                &nbsp;</p>
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
            
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="150px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" MaxLength="3"
                                runat="server" size="3" CssClass="inputFromNumero"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="150px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtNome" runat="server" MaxLength="60" size="60" Width="400px"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="150px" Text="Endereço*:"></asp:Label>
                            <asp:TextBox ID="txtLogradouro" runat="server" MaxLength="40" size="40"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="150px" Text="Número*:"></asp:Label>
                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="10" size="10"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="150px" Text="Complemento:"></asp:Label>
                            <asp:TextBox ID="txtComplemento" runat="server" MaxLength="30" size="30"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="150px" Text="Bairro*:"></asp:Label>
                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="20" size="20"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="150px" Text="Município*:"></asp:Label>
                            <asp:TextBox ID="txtMunicipio" runat="server" MaxLength="45" size="45"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="150px" Text="UF*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlUf" Width="100px" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id" >
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUf" runat="server" OldValuesParameterFormatString="original_{0}"
                                                  SelectMethod="PopularListaUf" TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                            </asp:ObjectDataSource>
                        </p>

                        <p>
                            <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="150px" Text="CEP*:"></asp:Label>
                            <asp:TextBox ID="txtCep" runat="server" MaxLength="10" size="10" CssClass="cep"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="150px" Text="Telefone:"></asp:Label>
                            <asp:TextBox ID="txtTelefone" runat="server" MaxLength="30" size="30" 
                                CssClass="telefone"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="150px" Text="Fax:"></asp:Label>
                            <asp:TextBox ID="txtFax" runat="server" MaxLength="17" size="17"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="150px" Text="Responsável*:"></asp:Label>
                            <asp:TextBox ID="txtResponsavel" runat="server" MaxLength="50" size="50" 
                                Width="400px"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="150px" Text="UGE*:"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlUGE" Width="400px" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id"></asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUGE" runat="server" 
                                OldValuesParameterFormatString="original_{0}" SelectMethod="PopularListaUGE" 
                                TypeName="Sam.Presenter.AlmoxarifadoPresenter">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="ddlGestor" Name="gestorId" PropertyName="SelectedValue" Type="Int32" />
                                    </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>

                        <p>
                            <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="150px" Text="Mês Ref. Inicial*:"></asp:Label>
                            <asp:TextBox ID="txtRefInicial" runat="server" MaxLength="30" size="10" 
                                CssClass="mesAno"></asp:TextBox>(AAAA/MM)
                        </p>
                        <p>
                                    <asp:Label ID="Label18" runat="server" CssClass="labelFormulario" Width="200px" Text="Mês Ref. Fatumento*:"></asp:Label>
                                    <asp:TextBox ID="txtRefFaturamento" runat="server" MaxLength="30" size="10" CssClass="mesAno"></asp:TextBox>(AAAA/MM)
                                </p>
                                <p>
                                    <asp:Label ID="Label19" runat="server" CssClass="labelFormulario" Width="200px" Text="Tipo de Almoxarifado*:"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlTipoAlmoxarifado" Width="150px">
                                        <asp:ListItem Text="Selecione" Value="0" />
                                        <asp:ListItem Text="Principal" Value="Principal" />
                                        <asp:ListItem Text="Secundário" Value="Secundário" />
                                    </asp:DropDownList>
                                </p>
                        <p>
                            <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="150px" Text="Ind. de Atividade*:"></asp:Label>
                            <asp:DropDownList 
                                runat="server" ID="ddlIndicadorAtividade" Width="100px" AutoPostBack="True">
                                <asp:ListItem Text="Ativo" Value="True" />
                                <asp:ListItem Text="Inativo" Value="False" />
                            </asp:DropDownList>
                        </p>
                    </fieldset>
                </div>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <p>
                    <small>Os campos marcados com (*) são obrigatórios. </small>
                </p>
            </div>
            
            <!-- fim id interno -->
            <div Class="botao">
                <!-- simula clique link editar/excluir -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" runat="server" Text="Gravar"  CssClass="button" onclick="btnGravar_Click" />
                        <asp:Button ID="btnExcluir" runat="server" Text="Excluir"  CssClass="button" onclick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"  CssClass="button" onclick="btnCancelar_Click" />
                    </p>
                </div>
            </div>
        </asp:Panel>
        </ContentTemplate>
        </asp:UpdatePanel>
        
    </div>
</asp:Content>
