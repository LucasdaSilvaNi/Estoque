<%@ Page Title="Módulo Tabelas :: Outras :: Terceiros" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroTerceiro.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroTerceiro"
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
                Módulo Tabelas :: Outras :: Terceiros
            </td>
        </tr>
        
         <tr>
            <td colspan="2" align="left">
                <table>
                    <tr>
                        <td style="width: 86px; font-weight : bold ">
                            Selecione:
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
                                            TypeName="Sam.Presenter.TerceiroPresenter">
                                        </asp:ObjectDataSource>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        
                        <tr>
                            <td style="width: 86px">
                                * Gestor
                            </td>
                            <td>
                                <asp:UpdatePanel ID="upnDropGestor" UpdateMode="Conditional" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="True"
                                            DataTextField="Nome" DataValueField="Id" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="sourceListaGestor" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularListaGestor" 
                                            TypeName="Sam.Presenter.TerceiroPresenter">
                                            
                                             <SelectParameters>
                                                <asp:ControlParameter ControlID="ddlOrgao" DefaultValue="SelectedValue" 
                                                    Name="_orgaoId" PropertyName="SelectedValue" />
                                            </SelectParameters>
                                            
                                        </asp:ObjectDataSource>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                       
                    </tr>
                </table>
        </tr>
        
        <tr>
        <td colspan="2">
                <asp:UpdatePanel ID="upnGridDados" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="gridTerceiro" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                            DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" 
                             CssClass="Grid" onselectedindexchanged="gridTerceiro_SelectedIndexChanged" 
                            >
                            <RowStyle CssClass="corsim" />
                            <AlternatingRowStyle CssClass="cornao" />
                            <Columns>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lbId" runat="server" Font-Bold="true" CausesValidation="False"
                                            CommandName="Select" Text='<%# Bind("Ordem") %>'></asp:LinkButton>
                                        <asp:Label runat="server" ID="lblOrgId" Text='<%# Bind("Orgao.Id") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblGestorId" Text='<%# Bind("Gestor.Id") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblLogradouro" Text='<%# Bind("Logradouro") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblNumero" Text='<%# Bind("Numero") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblCompl" Text='<%# Bind("Complemento") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblBairro" Text='<%# Bind("Bairro") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblCidade" Text='<%# Bind("Cidade") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblUfId" Text='<%# Bind("Uf.Id") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblCep" Text='<%# Bind("Cep") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblTelefone" Text='<%# Bind("Telefone") %>' Visible="false"></asp:Label>
                                        <asp:Label runat="server" ID="lblFax" Text='<%# Bind("Fax") %>' Visible="false"></asp:Label>
                                                                            
                                    </ItemTemplate>
                                    <ItemStyle Width="30px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Cnpj" HeaderText="Código" 
                                    DataFormatString="{0:000\.000\.000\-00}" >
                                <ItemStyle Width="60px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Nome" HeaderText="Descrição" >
                                 <HeaderStyle HorizontalAlign="Left" />                              
                                </asp:BoundField>
                               
                            </Columns>
                            <HeaderStyle CssClass="corpo"></HeaderStyle>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="sourceGridTerceiro" runat="server"
                            SelectMethod="PopularDadosTerceiro" EnablePaging="True" 
                            MaximumRowsParameterName="maximumRowsParameterName" 
                            SelectCountMethod="TotalRegistros" 
                            StartRowIndexParameterName="startRowIndexParameterName" 
                            TypeName="Sam.Presenter.TerceiroPresenter" 
                            OldValuesParameterFormatString="original_{0}">
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
            <td style="text-align:right">
                CNPJ
            </td>
            <td class="corpo">
                <asp:UpdatePanel ID="upnCnpj" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCnpj" CssClass="cnpj" runat="server" EnableViewState="False" 
                            Width="215px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
       
       
       
        <tr>
            <td style="text-align:right">
                Nome</td>
            <td class="corpo">
                <asp:UpdatePanel ID="upnNome" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtNome" runat="server" EnableViewState="False" 
                            Width="320px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
                       
        <tr>  <td style="width: 86px; font-weight: bold">
                            Endereço:
                        </td>
        
        <tr>
           <td style="text-align:right">
                Logradouro
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnLogradouro" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtLogradouro" runat="server" EnableViewState="False" 
                            Width="269px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                Número
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnNumero" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtNumero" runat="server" EnableViewState="False" 
                            Width="69px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                Complemento
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnComplemento" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtComplemento" runat="server" EnableViewState="False" 
                            Width="269px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                Bairro
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnBairro" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBairro" runat="server" EnableViewState="False" 
                            Width="200px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                Município
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnMunicipio" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCidade" runat="server" EnableViewState="False" 
                            Width="320px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                UF
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnDropUF" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                         <asp:DropDownList 
                            runat="server" ID="ddlUf" Width="269px" AutoPostBack="True" 
                            DataTextField="Descricao" DataValueField="Id" >
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="sourceListaUF" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularListaUF" 
                            TypeName="Sam.Presenter.TerceiroPresenter">
                        </asp:ObjectDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                CEP
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnCep" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCep" CssClass="cep" runat="server" EnableViewState="False" 
                            Width="100px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        
        <tr>
           <td style="text-align:right">
                Telefone
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnTelefone" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtTelefone" CssClass="telefone" runat="server" EnableViewState="False" 
                            Width="100px"></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
         
        <tr>
           <td style="text-align:right">
                Fax
            </td>
            <td class="corpo" >
                <asp:UpdatePanel ID="upnFax" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <asp:TextBox ID="txtFax" CssClass="telefone" runat="server" EnableViewState="False" 
                          Width="100px"  ></asp:TextBox>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
                       
        </tr>
                
        <tr>
            <td colspan="2">
                <asp:UpdatePanel ID="upnInconsistencia" UpdateMode="Conditional" runat="server">
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
                        <asp:Button ID="btnNovo" runat="server" Text="Novo"  CssClass="button" 
                            onclick="btnNovo_Click" />
                        <asp:Button ID="btnGravar" runat="server" Text="Gravar"  CssClass="button" 
                            onclick="btnGravar_Click" />
                        <asp:Button ID="btnExcluir" runat="server" Text="Excluir"  CssClass="button" 
                            onclick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar"  CssClass="button" 
                            onclick="btnCancelar_Click" />
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
