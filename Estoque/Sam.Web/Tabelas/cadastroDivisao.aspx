<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Divisão" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroDivisao.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroDivisao"
 ValidateRequest="false" EnableViewState="true" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">   
        <div id="content">
        <h1>Módulo Tabelas - Estrutura Organizacional - Divisão</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <fieldset class="fieldset">            
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="120px" Text="Órgão*:" Font-Bold="true" />
                    <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                        DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                    </asp:DropDownList>
            </p>
            <p>
            <asp:Label ID="lblUo" runat="server" CssClass="labelFormulario" Font-Bold="True" Text="UO:*" Width="120px" Visible="true" />
                <asp:DropDownList ID="ddlUo" runat="server" AutoPostBack="True" DataTextField="CodigoDescricao" DataValueField="Id" 
                onselectedindexchanged="ddlUo_SelectedIndexChanged" Width="80%" Visible="true" />
            </p>
            <p>
                <asp:Label ID="lblUge" runat="server" CssClass="labelFormulario" Font-Bold="True" Text="UGE*:" Width="120px" Visible="true" />
                <asp:DropDownList ID="ddlUge" runat="server" AutoPostBack="True" 
                    DataTextField="CodigoDescricao" DataValueField="Id" onselectedindexchanged="ddlUge_SelectedIndexChanged" Width="80%" Visible="true" />
            </p>
            <p>
                <asp:Label ID="Label32" runat="server" CssClass="labelFormulario" Width="120px" Text="UA*:" Font-Bold="true" />
                <asp:DropDownList runat="server" ID="ddlUA" Width="80%" AutoPostBack="True"
                    DataTextField="CodigoDescricao" DataValueField="Id" 
                    OnSelectedIndexChanged="ddlUA_SelectedIndexChanged" 
                    OnDataBound="ddlUA_DataBound">
                </asp:DropDownList>
                <%--<asp:ObjectDataSource ID="sourceListaUA" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="PopularDadosTodosCod" 
                    TypeName="Sam.Presenter.UaPresenter">
                        <SelectParameters>
                        <asp:ControlParameter ControlID="ddlOrgao" DefaultValue="SelectedValue" 
                            Name="_orgaoId" PropertyName="SelectedValue" />
                    </SelectParameters>
                </asp:ObjectDataSource>--%>
            </p>
        </fieldset>        
        <asp:GridView ID="gridDivisao" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridDivisao_SelectedIndexChanged" PagerStyle-HorizontalAlign="Center">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                            <asp:Label runat="server" ID="lblOrgId" Text='<%# Bind("Orgao.Id") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblUAId" Text='<%# Bind("UA.Id") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblLogradouro" Text='<%# Bind("EnderecoLogradouro") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblNumero" Text='<%# Bind("EnderecoNumero") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblCompl" Text='<%# Bind("EnderecoCompl") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblBairro" Text='<%# Bind("EnderecoBairro") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblMunicipio" Text='<%# Bind("EnderecoMunicipio") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblUfId" Text='<%# Bind("Uf.Id") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblCep" Text='<%# Bind("EnderecoCep") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblTelefone" Text='<%# Bind("EnderecoTelefone") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblFax" Text='<%# Bind("EnderecoFax") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblResponsavelId" Text='<%# Bind("Responsavel.Id") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblAlmoxarifadoId" Text='<%# Bind("Almoxarifado.Id") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblArea" Text='<%# Bind("Area") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblNumeroFuncionarios" Text='<%# Bind("NumeroFuncionarios") %>' Visible="false"></asp:Label>
                            <asp:Label runat="server" ID="lblIndicadorAtividadeId" Text='<%# Bind("IndicadorAtividade") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D3}">
                    <ItemStyle Width="50px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Nome" />
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
        <asp:ObjectDataSource ID="sourceGridDivisao" runat="server"
            SelectMethod="PopularDadosDivisao"
            MaximumRowsParameterName="maximumRowsParameterName" 
            SelectCountMethod="TotalRegistros" 
            StartRowIndexParameterName="startRowIndexParameterName" 
            TypeName="Sam.Presenter.DivisaoPresenter" 
            OldValuesParameterFormatString="original_{0}" EnablePaging="True">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                    Type="Int32" />
                    <asp:ControlParameter ControlID="ddlUA" Name="_uaId" PropertyName="SelectedValue"
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
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/consultaDivisao.aspx"
                    AccessKey="V" />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">            
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="120px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" MaxLength="3" onblur="preencheZeros(this,'3')" CssClass="inputFromNumero"
                                runat="server" size="3"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="120px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtNome" runat="server" MaxLength="120" size="120" Width="80%"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="120px" Text="Responsável:"></asp:Label>
                            <asp:DropDownList 
                                runat="server" ID="ddlResponsavel" Width="80%" 
                                DataTextField="Descricao" DataValueField="Id">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaResponsavel" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="PopularDadosResponsavelPorOrgaoGestor" 
                                TypeName="Sam.Presenter.ResponsavelPresenter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlOrgao" 
                                    Name="_orgaoId" PropertyName="SelectedValue" Type="Int32" />
                                <asp:ControlParameter ControlID="txtGestor" Name="_gestorId" 
                                    PropertyName="Text" Type="Int32" />
                            </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="120px" Text="Almoxarifado*:"></asp:Label>
                            <asp:DropDownList 
                                runat="server" ID="ddlAlmoxarifado" Width="80%" 
                                DataTextField="Descricao" DataValueField="Id">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaAlmoxarifado" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="PopularListaAlmoxarifado" 
                                TypeName="Sam.Presenter.DivisaoPresenter">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="ddlOrgao" 
                                    Name="OrgaoId" PropertyName="SelectedValue" Type="Int32" />
                                <asp:ControlParameter ControlID="txtGestor" Name="GestorId" PropertyName="Text" 
                                    Type="Int32" />
                            </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="120px" Text="Endereço*:"></asp:Label>
                            <asp:TextBox ID="txtLogradouro" runat="server" MaxLength="120" size="120" 
                                Width="80%"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="120px" Text="Número*:"></asp:Label>
                            <asp:TextBox ID="txtNumero" runat="server" MaxLength="10" size="10"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="120px" Text="Complemento:"></asp:Label>
                            <asp:TextBox ID="txtComplemento" runat="server" MaxLength="40" size="45" 
                                Width="400px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="120px" Text="Bairro*:"></asp:Label>
                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="20" size="20" 
                                Width="250px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="120px" Text="Município*:"></asp:Label>
                            <asp:TextBox ID="txtMunicipio" runat="server" MaxLength="45" size="45" 
                                Width="400px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="120px" Text="UF*:"></asp:Label>
                            <asp:DropDownList 
                                runat="server" ID="ddlUf" Width="269px" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUf" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="PopularListaUf" 
                                TypeName="Sam.Presenter.DivisaoPresenter">
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="120px" Text="CEP*:"></asp:Label>
                            <asp:TextBox ID="txtCep" runat="server" MaxLength="20" size="20" CssClass="cep"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="120px" Text="Telefone:"></asp:Label>
                            <asp:TextBox ID="txtTelefone" CssClass="telefone" runat="server" MaxLength="20" size="20"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="120px" Text="Fax:"></asp:Label>
                            <asp:TextBox ID="txtFax" runat="server" MaxLength="20" CssClass="telefone" size="20"></asp:TextBox>
                        </p>
                        <p class="esconderControle">
                            <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="120px" Text="Área:"></asp:Label>
                            <asp:TextBox ID="txtArea" runat="server" MaxLength="5" size="5" onkeypress='return SomenteNumero(event)' />
                        </p>

                        <p>
                            <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="120px" Text="Nº Funcionários:"></asp:Label>
                            <asp:TextBox ID="txtNumeroFuncionarios" runat="server" MaxLength="5" size="5" />
                        </p>

                        <p>
                            <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="120px" Text="Ind. de Atividade*:"></asp:Label>
                            <asp:DropDownList ID="ddlIndicadorAtividade" runat="server" DataTextField="Descricao" Width="155px"
                                DataValueField="Id">
                                <asp:ListItem Value="True">Ativo</asp:ListItem>
                                <asp:ListItem Value="False">Inativo</asp:ListItem>
                            </asp:DropDownList>
                            <asp:TextBox ID="txtGestor" runat="server" MaxLength="5" size="5" 
                                Visible="False" Width="0px" />
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
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
</asp:Content>
