<style type="text/css">
    .labelModel
    {
        font-family: Arial;
        display: block;
        float: left;
        font-size: 1.1em;
        font-weight: bold;
        margin-top: 3px;
        margin-right: 5px;
        text-align: right;
    }
</style>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModalSenha.ascx.cs" Inherits="Sam.Web.Controles.ModalSenha" %>
<asp:Panel runat="server" ID="pnl" DefaultButton="btnOKSenha">
    <asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
        <ProgressTemplate>
            <div id="progressBackgroundFilter"></div>
            <div id="processMessage"><center>Carregando...</center><br />
                    <center><img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="updSenha" runat="server">
    <ContentTemplate>
        <div>
        <p>
            <asp:Label ID="lblSenhaPrompt" runat="server" Text="Digite sua senha para acesso ao SIAFEM" />
        </p>
        <p>
            <center>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblUserName" Font-Size="12px" CssClass="labelFormulario" Width="60px" runat="server" Text="Login:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtUserName" runat="server" MaxLength="11" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblSenha" Font-Size="12px" CssClass="labelFormulario" Width="60px" runat="server" Text="Senha:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtSenha" TextMode="Password" runat="server" Width="100px" />
                        </td>
                    </tr>
                </table>
            </center>
        </p>
        <p style="text-align:center">
            <asp:Button ID="btnOKSenha" runat="server" Text="OK" 
                onclick="btnOKSenha_Click" />
            <asp:Button ID="btnCancelarSenha" runat="server" 
                OnClientClick="CloseModalSenhaWs();" Text="Cancelar" />
        </p>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
