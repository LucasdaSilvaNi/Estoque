<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Divisão" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="consultaDivisao.aspx.cs" Inherits="Sam.Web.Seguranca.consultaDivisao"
 ValidateRequest="false" EnableViewState="true" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">   
        <div id="content">
        <h1>Módulo Tabelas - Estrutura Organizacional - Consulta de Divisões</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>
        <fieldset class="fieldset">            
            <p>
            <asp:Label ID="lblUo" runat="server" CssClass="labelFormulario" Font-Bold="True" Text="UO:*" Width="120px" Visible="true" />
                <asp:DropDownList ID="ddlUo" runat="server" AutoPostBack="True" 
                    DataTextField="CodigoDescricao" DataValueField="Id" 
                Width="80%" Visible="true" 
                    onselectedindexchanged="ddlUo_SelectedIndexChanged" />
            </p>
            <p>
                <asp:Label ID="lblUge" runat="server" CssClass="labelFormulario" Font-Bold="True" Text="UGE*:" Width="120px" Visible="true" />
                <asp:DropDownList ID="ddlUge" runat="server" AutoPostBack="True" 
                    DataTextField="CodigoDescricao" DataValueField="Id" Width="80%" Visible="true" 
                    OnSelectedIndexChanged="ddlUge_SelectedIndexChanged" 
                    ondatabound="ddlUge_DataBound" />
            </p>
        </fieldset>
        <asp:GridView ID="gridDivisao" runat="server" AllowPaging="True" OnPageIndexChanging="gridDivisao_PageIndexChanging" PageSize="50">
            <Columns>
                <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D3}">
                    <ItemStyle Width="50px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Nome" />
                <asp:TemplateField HeaderText="UO">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblUO" Text='<%# Bind("Ua.Uge.Uo.CodigoDescricao")%>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="UGE">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblUGE" Text='<%# Bind("Ua.Uge.CodigoDescricao")%>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="UA">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblUA" Text='<%# Bind("Ua.CodigoDescricao")%>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" style="{width: 150px;}" runat="server" Text="Cadastro de Divisão" AccessKey="N" PostBackUrl="~/Tabelas/cadastroDivisao.aspx" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click" Text="Imprimir" AccessKey="I" />
                <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass="" AccessKey="A" />
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx" AccessKey="V" />
            </p>
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
</asp:Content>
