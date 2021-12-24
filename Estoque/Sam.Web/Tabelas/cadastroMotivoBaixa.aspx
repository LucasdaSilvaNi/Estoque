<%@ Page Title="Módulo Tabelas :: Outras :: Motivos de Baixa" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroMotivoBaixa.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroMotivoBaixa"
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/Css.css" rel="stylesheet" type="text/css" />
    
    <table class="table">
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" style="background-color: Black; color: White;">
                Módulo Tabelas :: Outras :: Motivos de Baixa
            </td>
        </tr>
        
        
        <tr>
        <td colspan="2">
                <asp:UpdatePanel ID="upnGridDados" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gridMotivoBaixa" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                            DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" 
                            CssClass="Grid" onselectedindexchanged="gridMotivoBaixa_SelectedIndexChanged">
                            <RowStyle CssClass="corsim" />
                            <AlternatingRowStyle CssClass="cornao" />
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                            CommandName="Select" Text='<%# Bind("Ordem") %>'></asp:LinkButton>
                                        <asp:Label ID="lblCodigoTransacao" runat="server" Text='<%# Bind("CodigoTransacao") %>' 
                                            Visible="False"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle Width="20px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Codigo" HeaderText="Cód." >
                                <ItemStyle Width="30px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                            </Columns>
                            <HeaderStyle CssClass="corpo"></HeaderStyle>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="sourceGridMotivoBaixa" runat="server"
                            SelectMethod="PopularDadosMotivoBaixa" 
                            TypeName="Sam.Presenter.MotivoBaixaPresenter" EnablePaging="True" 
                            MaximumRowsParameterName="maximumRowsParameterName" 
                            SelectCountMethod="TotalRegistros" 
                            StartRowIndexParameterName="startRowIndexParameterName" 
                            OldValuesParameterFormatString="original_{0}">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />                            
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
            <td style="text-align: right" >
                C&oacute;digo:
            </td>
            <td style="text-align: left;">
                <asp:UpdatePanel ID="upnCodigo" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCodigo" CssClass="inputFromNumero" runat="server" EnableViewState="False" Width="81px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td style="text-align:right">
                Descrição
            </td>
            <td class="corpo">
                <asp:UpdatePanel ID="upnDescricao" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDescricao" runat="server" EnableViewState="False" 
                            Width="338px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
            <td style="text-align:right">
                Código de Transação
            </td>
            <td class="corpo">
                <asp:UpdatePanel ID="upnCodigoTransacao" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCodigoTransacao" MaxLength="3" runat="server" EnableViewState="False" 
                            Width="81px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="updInconsistencia" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <uc1:ListInconsistencias ID="ListInconsistencias" EnableViewState="false" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td width="50%" align=left>
                <asp:UpdatePanel ID="upnBotoes" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:Button ID="btnNovo" runat="server" Text="Novo" OnClick="btnNovo_Click" CssClass="button" />
                        <asp:Button ID="btnGravar" runat="server" Text="Gravar" OnClick="btnGravar_Click" CssClass="button" />
                        <asp:Button ID="btnExcluir" runat="server" Text="Excluir" OnClick="btnExcluir_Click" CssClass="button" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" CssClass="button" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
           <td width="50%">
                <asp:UpdatePanel ID="upnBotoesSecundarios" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div align="right">
                            <asp:Button ID="btnImprimir"  runat="server" Text="Imprimir" CssClass="button" onclick="btnImprimir_Click" />
                           <asp:Button ID="btnAjuda" runat="server" Text="Ajuda" CssClass="" AccessKey="A"  OnClientClick="OpenModal();" />
                            <asp:Button ID="btnSair" runat="server" Text="Sair" PostBackUrl="~/Tabelas/TABMenu.aspx" CssClass="button" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>

        </tr>
        
    </table>
</asp:Content>
