<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroTipoIncorp.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroTipoIncorp" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <table align="center" width="100%">
        <tr>
            <td colspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="2" style="background-color: Black; color: White;" width="100%">
                Módulo Tabelas :: Outras :: Fontes de Recurso</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="upnGridDados" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gridTipoIncorp" runat="server" 
                            HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" AllowPaging="True" 
                            AutoGenerateColumns="False" CssClass="Grid" DataKeyNames="Id" 
                            onselectedindexchanged="gridTipoIncorp_SelectedIndexChanged" 
                            onpageindexchanged="gridTipoIncorp_PageIndexChanged" PageSize="3">
                            <RowStyle CssClass="corsim" />
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="True" CausesValidation="False" 
                                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                        <asp:Label ID="lblCodigoTransacao" runat="server" 
                                            Text='<%# Bind("CodigoTransacao") %>' Visible="False"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Codigo" HeaderText="Cod." />
                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                            </Columns>
                            <HeaderStyle CssClass="corpo" />
                            <AlternatingRowStyle CssClass="cornao" />
                        </asp:GridView>
                        <asp:ObjectDataSource ID="sourceGrid" runat="server" EnablePaging="True" 
                            MaximumRowsParameterName="maximumRowsParameterName" 
                            SelectCountMethod="TotalRegistros" SelectMethod="PopularGrid" 
                            StartRowIndexParameterName="startRowIndexParameterName" 
                            TypeName="Sam.Presenter.TipoIncorpPresenter">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                        <br />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                * Codigo:</td>
            <td style="text-align: left;">
                <asp:UpdatePanel ID="upnCodigo" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCodigo" runat="server" 
                            Width="100px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                * Descrição:</td>
            <td style="text-align: left;">
                <asp:UpdatePanel ID="upnDescricao" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDescricao" runat="server" 
                            Width="300px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="text-align: right">
                Código Transação: </td>
            <td style="text-align: left;">
                <asp:UpdatePanel ID="upnCodigotransacao" runat="server" 
                    UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCodigoTransacao" runat="server" 
    Width="100px" MaxLength="3" EnableViewState="False"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;</td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="upnIncosistencias" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="left" width="50%">
                <asp:UpdatePanel ID="upnBotoes" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button ID="btnNovo" runat="server" Text="Novo" CssClass="button" 
                            onclick="btnNovo_Click" />
                        <asp:Button ID="btnGravar" runat="server" Text="Gravar" CssClass="button" 
                            onclick="btnGravar_Click" />
                        <asp:Button ID="btnExcluir" runat="server" CssClass="button" Text="Excluir" 
                            onclick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" runat="server" CssClass="button" Text="Cancelar" 
                            onclick="btnCancelar_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
          <td width="50%">
                <asp:UpdatePanel ID="upnBotoesSecundarios" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div align="right">
                            <asp:Button ID="btnImprimir"  runat="server" Text="Imprimir" CssClass="button" onclick="btnImprimir_Click" />
                            <asp:Button ID="btnAjuda" runat="server" Text="Ajuda" CssClass="button" OnClientClick="OpenModal();" />
                            <asp:Button ID="btnSair" runat="server" Text="Sair" PostBackUrl="~/Tabelas/TABMenu.aspx" CssClass="button" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        </table>
</asp:Content>
