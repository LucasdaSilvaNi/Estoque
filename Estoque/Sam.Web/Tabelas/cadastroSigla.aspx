<%@ Page Title="Módulo Tabelas :: Outras :: Siglas" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroSigla.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroSigla"
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">

    
    <table class="table">
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" style="background-color: Black; color: White;">
                Módulo Tabelas :: Outras :: Siglas
            </td>
        </tr>
        
         <tr>
            <td colspan="2" align="left">
                <table>
                    <tr>
                        <td style="width: 86px; font-weight: bold;">
                            Selecione
                        </td>
                        <tr>
                            <td style="width: 86px">
                                * Órgão
                            </td>
                            <td>
                                <asp:UpdatePanel ID="upnDropOrgao" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularListaOrgao" 
                                            TypeName="Sam.Presenter.SiglaPresenter">
                                        </asp:ObjectDataSource>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        
                        <td style="width: 86px">
                                * Gestor
                        </td>
                        <td>
                            <asp:UpdatePanel ID="upnDropGestor" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True"
                                            DataTextField="Nome" DataValueField="Id" 
                                            OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="sourceListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularListaGestor" 
                                            TypeName="Sam.Presenter.SiglaPresenter">
                                          
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="ddlOrgao" DefaultValue="SelectedValue" 
                                                    Name="_orgaoId" PropertyName="SelectedValue" />
                                            </SelectParameters>
                                          
                                        </asp:ObjectDataSource>
                                        
                                      
                                        
                                        
                                    </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
        </tr>
        
        <tr>
        <td colspan="2">
                <asp:UpdatePanel ID="upnGridDados" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gridSigla" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                            DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" 
                            CssClass="Grid" onselectedindexchanged="gridSigla_SelectedIndexChanged" 
                            onpageindexchanged="gridSigla_PageIndexChanged">
                            <RowStyle CssClass="corsim" />
                            <AlternatingRowStyle CssClass="cornao" />
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                            CommandName="Select" Text='<%# Bind("Ordem") %>'></asp:LinkButton>
                                        <asp:Label ID="lblIndicadorBemProprioId" Visible="false" runat="server" Text='<%# Bind("IndicadorBemProprio.Id") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Codigo" HeaderText="Cód." />
                                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                            </Columns>
                            <HeaderStyle CssClass="corpo"></HeaderStyle>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="sourceGridSigla" runat="server"
                            SelectMethod="PopularDadosSigla" 
                            TypeName="Sam.Presenter.SiglaPresenter" EnablePaging="True" 
                            MaximumRowsParameterName="maximumRowsParameterName" 
                            SelectCountMethod="TotalRegistros" 
                            StartRowIndexParameterName="startRowIndexParameterName">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ddlGestor" Name="_gestorId" PropertyName="SelectedValue"
                                    Type="Int32" />                            
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
                        <asp:TextBox ID="txtCodigo" runat="server" EnableViewState="False"> </asp:TextBox>
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
                            Width="269px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        
        <tr>
            <td style="text-align: right" >
                Bens Próprios?
            </td>
            <td style="text-align: left;">
                <asp:UpdatePanel ID="upnDropIndicadorBemProprio" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlIndicadorBemProprio" runat="server" AutoPostBack="True" 
                             DataTextField="Descricao" DataValueField="Id">   
                              </asp:DropDownList>
                        <asp:ObjectDataSource ID="sourceListaIndicadorBemProprio" runat="server" 
                            OldValuesParameterFormatString="original_{0}" SelectMethod="PopularListaIndicadorBemProprio" 
                            TypeName="Sam.Presenter.SiglaPresenter">
                           
                        </asp:ObjectDataSource>
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
