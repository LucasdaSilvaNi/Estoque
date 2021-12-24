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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucConversaoUnidadesEmpenho.ascx.cs" Inherits="Sam.Web.Controles.ucConversaoUnidadesEmpenho" %>
<asp:Panel runat="server" ID="pnl" DefaultButton="btnOK">
    <asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
        <ProgressTemplate>
            <div id="progressBackgroundFilter"></div>
            <div id="processMessage"><center>Carregando...</center><br />
                    <center><img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="upd" runat="server">
    <ContentTemplate>
        <div>
        <p>
            <asp:Label ID="lblPrompt" runat="server" Text="Entre com os valores de conversão de Unidades de Fornecimento (SIAFEM x SAM)" />
        </p>
        <p>
            <center>
                <table>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblUnidadeFornecimentoSIAFEM" Font-Size="12px" CssClass="labelFormulario" Width="160px" runat="server" Text="Unidade Fornecimento SIAFEM:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtUnidadeFornecimentoSIAFEM" runat="server" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblUnidadeFornecimentoSAM" Font-Size="12px" CssClass="labelFormulario" Width="160px" runat="server" Text="Unidade Fornecimento SAM:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtUnidadeFornecimentoSAM" runat="server" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <asp:Label ID="lblInformativoFatorConversao" Font-Size="12px" CssClass="labelFormulario" Width="160px" runat="server" Text="Fator de Conversão:" />
                        </td>
                        <td align="left">
                            <asp:TextBox ID="txtValorFatorConversao" runat="server" Width="100px" />
                        </td>
                    </tr>
                </table>
            </center>
            <p>
            </p>
            <p style="text-align:center">
                <asp:Button ID="btnOK" runat="server" onclick="btnOK_Click" 
                    Text="OK" />
                <asp:Button ID="btnCancelar" runat="server" 
                    onclick="btnCancelar_Click" OnClientClick="CloseModalConversaoUnidadeFornecimento();" 
                    Text="Cancelar" />
            </p>
        </p>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
</asp:Panel>
