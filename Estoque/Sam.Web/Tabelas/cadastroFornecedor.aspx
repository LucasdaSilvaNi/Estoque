<%@ Page Title="Módulo Tabelas :: Outras :: Fornecedores" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroFornecedor.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroFornecedor"
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">

    <div id="content">
        <h1>
            Módulo Tabelas - Outras - Fornecedores</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>
        <asp:GridView ID="gridFornecedor" runat="server" AllowPaging="true" 
            OnSelectedIndexChanged="gridFornecedor_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("ID") %>' Width="50px"></asp:LinkButton>
                        <asp:Label ID="lblCPFCNPJ" Visible="false" runat="server" Text='<%# Bind("CpfCnpj") %>'></asp:Label>
                        <asp:Label ID="lblLogradouro" Visible="false" runat="server" Text='<%# Bind("Logradouro") %>'></asp:Label>
                        <asp:Label ID="lblNumero" Visible="false" runat="server" Text='<%# Bind("Numero") %>'></asp:Label>
                        <asp:Label ID="lblComplemento" Visible="false" runat="server" Text='<%# Bind("Complemento") %>'></asp:Label>
                        <asp:Label ID="lblBairro" Visible="false" runat="server" Text='<%# Bind("Bairro") %>'></asp:Label>
                        <asp:Label ID="lblCep" Visible="false" runat="server" Text='<%# Bind("Cep") %>'></asp:Label>
                        <asp:Label ID="lblCidade" Visible="false" runat="server" Text='<%# Bind("Cidade") %>'></asp:Label>
                        <asp:Label ID="lblUfId" Visible="false" runat="server" Text='<%# Bind("Uf.Id") %>'></asp:Label>
                        <asp:Label ID="lblTelefone" Visible="false" runat="server" Text='<%# Bind("Telefone") %>'></asp:Label>
                        <asp:Label ID="lblFax" Visible="false" runat="server" Text='<%# Bind("Fax") %>'></asp:Label>
                        <asp:Label ID="lblEmail" Visible="false" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                        <asp:Label ID="lblInfoComplementares" Visible="false" runat="server" Text='<%# Bind("InformacoesComplementares") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CPFCNPJ"  HeaderText="CNPJ/CPF" ItemStyle-Width="60px"  ApplyFormatInEditMode="False" >
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>
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
        <asp:ObjectDataSource ID="sourceGridFornecedor" runat="server"
            SelectMethod="PopularDadosFornecedor" 
            TypeName="Sam.Presenter.FornecedorPresenter" EnablePaging="True" 
            MaximumRowsParameterName="maximumRowsParameterName" 
            SelectCountMethod="TotalRegistros" 
            StartRowIndexParameterName="startRowIndexParameterName" 
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
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
                    AccessKey="A" onclick="btnAjuda_Click" />
                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                    AccessKey="V" />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <br />
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                    <p>
                            <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="115px" Text="Tipo Pessoa*:" />
                            <asp:RadioButton ID="rdoCNPJ" runat="server" Text="Jurídica"  Checked="true" AutoPostBack="true"
                                GroupName="CNPJCPF" oncheckedchanged="rdoCPF_CheckedChanged" />
                            <asp:RadioButton ID="rdoCPF" runat="server" Text="Física" GroupName="CNPJCPF" 
                                AutoPostBack="True" oncheckedchanged="rdoCNPJ_CheckedChanged" />                            
                        <p>
                        <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Text="CNPJ/CPF*:"  Width="115px" />
                                    <asp:TextBox ID="txtCpfCnpj" runat="server" CssClass="cnpjcpf" MaxLength="14" 
                                            onkeypress="return SomenteNumero(event)" Width="120px"></asp:TextBox>
                        </p>                         	
                        <p>
                            <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="115px" Text="Nome*:" />
                            <asp:TextBox ID="txtNome" MaxLength="60"
                                runat="server" size="60" Width="400px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="Logradouro:" />
                            <asp:TextBox ID="txtLogradouro" MaxLength="120" runat="server" size="120" 
                                Width="400px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="Número:" />
                            <asp:TextBox ID="txtNumero" MaxLength="10" runat="server" size="10" Width="100px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="115px" Text="Complemento:" />
                            <asp:TextBox ID="txtComplemento" MaxLength="10" runat="server" size="10" Width="200px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="Bairro:" />
                            <asp:TextBox ID="txtBairro" runat="server" MaxLength="45" size="45" 
                                Width="200px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="115px" Text="CEP:" />
                            <asp:TextBox ID="txtCep" runat="server" CssClass="cep" MaxLength="8" size="8" Width="100px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="Cidade:" />
                            <asp:TextBox ID="txtCidade" runat="server" MaxLength="45" size="45" 
                                Width="300px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="UF:" />
                             <asp:DropDownList 
                                runat="server" ID="ddlUf" Width="269px" AutoPostBack="True" 
                                DataTextField="Descricao" DataValueField="Id">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUf" runat="server" OldValuesParameterFormatString="original_{0}"
                                                SelectMethod="PopularListaUf" 
                                TypeName="Sam.Presenter.FornecedorPresenter">
                            </asp:ObjectDataSource>
                        </p>

                        <p>
                            <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="115px" Text="Telefone:" />
                            <asp:TextBox ID="txtTelefone" runat="server" MaxLength="20" size="20" CssClass="telefone" Width="110px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="115px" Text="Fax:" />
                            <asp:TextBox ID="txtFax" CssClass="fax" runat="server" MaxLength="20" size="20" Width="110px"></asp:TextBox>
                        </p>

                        <p>
                            <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="115px" Text="E-mail:" />
                            <asp:TextBox ID="txtEmail" runat="server" MaxLength="50" size="50" 
                                Width="400px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="115px" 
                                Text="Informações Complementares:" />
                            <asp:TextBox ID="txtInfoComplementares" runat="server" MaxLength="255"  onkeyup='limitarTamanhoTexto(this,255)' onkeydown='limitarTamanhoTexto(this,255)' 
                                TextMode="MultiLine" Width="80%" SkinID="MultiLine"></asp:TextBox>
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
        <br />
        </div>

</asp:Content>
