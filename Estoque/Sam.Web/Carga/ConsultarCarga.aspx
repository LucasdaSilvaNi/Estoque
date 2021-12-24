<%@ Page Title="Módulo Carga :: Carga Inicial :: Almoxarifado" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"
    AutoEventWireup="true" CodeBehind="ConsultarCarga.aspx.cs" Inherits="Sam.Web.Carga.ConsultarCarga"
    ValidateRequest="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Carga - Consultar Carga</h1>        
        <asp:GridView SkinID="GridNovo" ID="grdPendentes" runat="server">
            <Columns>
                <asp:BoundField DataField="NomeArquivo" HeaderText="Arquivo Pendente" ItemStyle-Width="100%">
                </asp:BoundField>
                <asp:TemplateField HeaderText="Importar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDImp" runat="server" Font-Bold="true" ImageUrl="~/Imagens/Add.gif" Width="25px"
                            CausesValidation="true" CommandName="Importar" CommandArgument='<%# Bind("NomeArquivo") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Excluir" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDEx" runat="server" Font-Bold="true" ImageUrl="~/Imagens/Delete.gif" Width="25px"
                            CausesValidation="true" CommandName="Excluir" CommandArgument='<%# Bind("NomeArquivo") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <div>
        <fieldset class="fieldset">
            <div id="Div3">
                <p>
                    <asp:Label ID="lblUpload" runat="server" CssClass="labelFormulario" Width="100px"
                        Text="Arquivo Excel:" Font-Bold="true"></asp:Label>
                    <asp:FileUpload ID="fulExcel" runat="server" Width="400px" />
                    <asp:Button runat="server" SkinID="Btn120" ID="btnImportar" Text="Subir Arquivo" OnClick="btnImportar_Click" />
                </p>
            </div>
            
        </fieldset>
        </div>
    </div>
</asp:Content>
