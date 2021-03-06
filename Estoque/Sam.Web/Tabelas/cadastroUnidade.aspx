<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Unidade" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroUnidade.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroUnidade"
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
<div id="content">
        <h1>
            Módulo Tabelas - Estrutura Organizacional - Unidade</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <fieldset class="fieldset">
            <div id="Div3">
                <p>
                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="100px" Text="Órgão*:"
                        Font-Bold="true"></asp:Label>
                    <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                        DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="PopularListaOrgaoTodosCod" 
                        TypeName="Sam.Presenter.UnidadePresenter">
                    </asp:ObjectDataSource>
                </p>
                <p>
                    <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="100px" Text="Gestor*:"
                        Font-Bold="true"></asp:Label>
                    <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True"
                        DataTextField="CodigoDescricao" DataValueField="Id" 
                        OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged" 
                        ondatabound="ddlGestor_DataBound">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="sourceListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="PopularListaGestorTodosCod"
                        TypeName="Sam.Presenter.UnidadePresenter">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlOrgao" 
                                Name="_orgaoId" PropertyName="SelectedValue" Type="Int32" />                    </SelectParameters>
                    </asp:ObjectDataSource>
                </p>
            </div>
        </fieldset>
        <br />
        <asp:GridView ID="gridUnidade" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridUnidade_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D2}">
                    <ItemStyle Width="50px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
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
        <asp:ObjectDataSource ID="sourceGridUnidade" runat="server"
            SelectMethod="PopularDadosUnidade" 
            TypeName="Sam.Presenter.UnidadePresenter" EnablePaging="True" 
            MaximumRowsParameterName="maximumRowsParameterName" 
            SelectCountMethod="TotalRegistros" 
            StartRowIndexParameterName="startRowIndexParameterName">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue"
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
                            <asp:Label ID="codigo" runat="server" class="labelFormulario" Width="100px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" MaxLength="3"
                                runat="server" size="12" Width="100px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" class="labelFormulario" Width="100px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="60" size="60"></asp:TextBox>
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
