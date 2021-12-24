<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="cadastroContaAuxiliar.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroContaAuxiliar"
    Title="Módulo Tabelas :: Catálogo :: Conta Auxiliar" %>

<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Catálogo - Conta Auxiliar</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <asp:GridView ID="gridConta" runat="server" AllowPaging="True"  
            OnSelectedIndexChanged="gridConta_SelectedIndexChanged" OnPageIndexChanged="gridConta_PageIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                        <asp:Label ID="lblContaContabil" runat="server" Text='<%# Bind("ContaContabil") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D4}">
                    <ItemStyle Width="50px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                <asp:BoundField DataField="ContaContabil" HeaderText="Conta Contábil" DataFormatString="{0:D9}" Visible="false" />
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
        <asp:ObjectDataSource ID="sourceGridConta" runat="server" EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName"
            OldValuesParameterFormatString="original_{0}" SelectCountMethod="TotalRegistros"
            SelectMethod="PopularDadosConta" StartRowIndexParameterName="startRowIndexParameterName"
            TypeName="Sam.Presenter.ContaAuxiliarPresenter">
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
                            <asp:Label ID="codigo" runat="server" class="labelFormulario" Width="100px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" MaxLength="4" onkeypress='return SomenteNumero(event)' onblur="preencheZeros(this,'4')"
                                runat="server" size="4"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" class="labelFormulario" Width="100px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" size="90" 
                                Width="80%"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="100px" Text="Conta Contábil*:"></asp:Label>
                            <asp:TextBox ID="txtContaContabil" onkeypress='return SomenteNumero(event)' 
                                onblur="preencheZeros(this,'9')" runat="server" MaxLength="9" size="9" 
                                Width="150px"></asp:TextBox>
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
