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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntradaCampoCE.ascx.cs" Inherits="Sam.Web.Controles.EntradaCampoCE" %>
<asp:Panel runat="server" ID="frmPanel" DefaultButton="btnValorOK">
    <asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
        <ProgressTemplate>
            <div id="progressBackgroundFilter"></div>
            <div id="processMessage"><center>Carregando...</center><br />
                    <center><img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="updatePanelValorCE" runat="server">
    <ContentTemplate>
        <div>
        <asp:HiddenField  ID="hdnNumeroDocumento" runat="server" />
        <asp:HiddenField  ID="hdnProcessar" runat="server" />
        <p>
            <asp:Label ID="lblAcaoJanela" runat="server" Text="Digite valor SIAFEM 'CE'." />
        </p>
        <p>
            <center>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblValorCE" Font-Size="12px" CssClass="labelFormulario" Width="80px" runat="server" Text="Valor CE:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtValorCE" runat="server" MaxLength="22" Width="140px" OnTextChanged="txtValorCE_TextChanged" />
                        </td>
                    </tr>
                    <tr class="<%:classExibirControle %>">
                        <td align="left">
                            <asp:Label ID="Label2" Font-Size="12px" Width="80px" CssClass="labelFormulario" runat="server" Text="UGE Destino:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtValorUgeDestino" CssClass="inputFromNumero" runat="server" MaxLength="6" Width="140px" />
                        </td>
                    </tr>
                </table>
            </center>
            <p>
            </p>
            <p style="text-align:center">
                <asp:Button ID="btnValorOK" runat="server" onclick="btnValorOK_Click" Text="OK" />
                <asp:Button ID="btnCancelarEntradaValorCE" runat="server" onclick="btnCancelarEntradaValorCE_Click" OnClientClick="CloseModalCampoSiafemCE();" 
                    Text="Cancelar" />
            </p>
        </p>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
