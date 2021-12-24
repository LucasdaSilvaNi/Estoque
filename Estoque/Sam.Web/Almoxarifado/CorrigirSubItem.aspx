<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="CorrigirSubItem.aspx.cs" Inherits="Sam.Web.Almoxarifado.CorrigirSubItem" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <script src="../JScript/Utilitarios.js" type="text/javascript"></script>
    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                args.set_errorHandled(true);
            }
        });
    </script>
    <asp:UpdatePanel runat="server" ID="udpPanel">
        <ContentTemplate>
            <div id="content">
                <asp:Timer ID="timerArquivo" runat="server" Enabled="false" Interval="20000" OnTick="timerArquivo_Tick">
                </asp:Timer>
                <h1>
                    Módulo Almoxarifado - Corrigir SubItem</h1>
                <div>
                    <div id="container_abas_consultas">
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                    </div>
                    <br />
                    <br />
                    <br />
                    <fieldset class="fieldset">
                        <div id="Div3">
                            <p>
                                <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="120px" Text="Órgão*:"
                                    Font-Bold="true" />
                                <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="true" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                                </asp:DropDownList>
                            </p>
                            <p style="visibility: hidden; display: none;">
                                <asp:Label ID="Label4" runat="server" class="labelFormulario" Width="120px" Text="UO*:"
                                    Font-Bold="true" />
                                <asp:DropDownList runat="server" ID="ddlUO" Width="80%" AutoPostBack="True" OnSelectedIndexChanged="ddlUO_SelectedIndexChanged">
                                </asp:DropDownList>
                            </p>
                            <p style="visibility: hidden; display: none;">
                                <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="120px" Text="UGE*:"
                                    Font-Bold="true" />
                                <asp:DropDownList runat="server" ID="ddlUGE" Width="80%" AutoPostBack="True" OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged">
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label ID="Label6" runat="server" class="labelFormulario" Width="120px" Text="Gestor*:"
                                    Font-Bold="true" />
                                <asp:DropDownList runat="server" ID="ddlGestor" Width="80%" AutoPostBack="true" OnSelectedIndexChanged="ddlGestor_SelectedIndexChanged">
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label ID="labelAlmoxa" runat="server" CssClass="labelFormulario" Width="120px"
                                    Text="Almoxarifado:"></asp:Label>
                                <asp:DropDownList ID="drpAlmoxarifado" CssClass="selecioneMaior" runat="server" Width="80%">
                                </asp:DropDownList>
                            </p>
                        </div>
                    </fieldset>
                    <br />
                    <br />

                    <fieldset class="fieldset">
                    <div id="Div2">
                        <p id="p3">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Label ID="label13" runat="server" CssClass="labelFormulario" Width="140px" Text="Id Movimento:" />
                                        <asp:TextBox ID="txtMovimentoId" runat="server" onkeypress="return SomenteNumero(event)"
                                            CssClass="textBox"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="label14" runat="server" CssClass="labelFormulario" Width="140px" Text="Bloquear:" />
                                        <asp:DropDownList ID="ddlBloquear" CssClass="selecioneMaior" runat="server" Width="40%">
                                            <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Sim"></asp:ListItem>
                                            <asp:ListItem Value="0" Text="Não"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </p>
                        <p class="botaoRight">
                            <asp:Button ID="btnSalvarAlm" runat="server" Text="Salvar" OnClick="btnSalvarAlm_Click" />
                        </p>
                    </div>
                </fieldset>
                    <br />
                    <br />
                    <fieldset class="fieldset" style="visibility:hidden">
                        <div id="Div1">
                            <p id="p1">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="label9" runat="server" CssClass="labelFormulario" Width="140px" Text="Id PtresMensal:" />
                                            <asp:TextBox ID="txtIdPtresMensal" runat="server" onkeypress="return SomenteNumero(event)"
                                                CssClass="textBox"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="label12" runat="server" CssClass="labelFormulario" Width="140px" Text="Id MovimentoItem:" />
                                            <asp:TextBox ID="txtMovimentoItemNL" runat="server" onkeypress="return SomenteNumero(event)"
                                                CssClass="textBox"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </p>
                            <p id="P2" runat="server">
                                <asp:Label ID="label11" runat="server" CssClass="labelFormulario" Width="140px" Text="NL" />
                                <asp:TextBox ID="txtNL" runat="server" CssClass="textBox"></asp:TextBox>
                            </p>
                            <p class="botaoRight">
                                <asp:Button ID="Button1" runat="server" Text="Salvar" OnClick="btnSalvarPTR_Click" />
                            </p>
                        </div>
                    </fieldset>
                    <br />
                    <br />
                    <fieldset class="fieldset" style="visibility:hidden">
                        <asp:Panel ID="analitica" runat="server">
                            <br />
                            <p id="paragrafo3">
                                <asp:Label ID="labelDataMovimento" runat="server" CssClass="labelFormulario" Width="140px"
                                    Text="Mês de Refêrencia:" />
                                <asp:TextBox ID="txtMesReferencia" runat="server" CssClass="textBox" MaxLength="6"></asp:TextBox>
                            </p>
                            <p id="paragrafo4">
                                <asp:Label ID="labelSubItem" runat="server" CssClass="labelFormulario" Width="140px"
                                    Text="Cod.SubItem:" />
                                <asp:TextBox ID="txtSubtItemCodigo" runat="server" CssClass="textBox"></asp:TextBox>
                            </p>
                            <p class="botaoRight">
                                <asp:Button ID="btnCorrigir" runat="server" Text="Corrigir" OnClick="btnCorrigir_Click"
                                    AccessKey="R" />
                            </p>
                        </asp:Panel>
                        <asp:Button ID="btnDownload" runat="server" Text="Download" OnClick="btnDownload_Click" />
                        <asp:Label ID="lblmessage" runat="server"></asp:Label>
                    </fieldset>
                </div>
                <br />
                <br />
                
                <br />
                <br />
                <div>
                    <fieldset class="fieldset" style="visibility:hidden">
                        <p id="p5">
                            <asp:Label ID="label7" runat="server" CssClass="labelFormulario" Width="140px" Text="Id MovimentoItem:" />
                            <asp:TextBox ID="txtMovimentoItem" runat="server" onkeypress="return SomenteNumero(event)"
                                CssClass="textBox"></asp:TextBox>
                        </p>
                        <p id="p6">
                            <asp:Label ID="label8" runat="server" CssClass="labelFormulario" Width="140px" Text="Ação:" />
                            <asp:DropDownList ID="ddlAcao" CssClass="selecioneMaior" runat="server" Width="20%"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlAcao_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
                                <asp:ListItem Value="Ativar" Text="Ativar MovimentoItem"></asp:ListItem>
                                <asp:ListItem Value="Alterar" Text="Alterar Qtde MovimentoItem"></asp:ListItem>
                                <asp:ListItem Value="AlterarValor" Text="Alterar Valor MovimentoItem"></asp:ListItem>
                            </asp:DropDownList>
                        </p>
                        <div>
                            <p>
                            </p>
                            <p id="idAtivacao" runat="server" visible="false">
                                <asp:Label ID="label1" runat="server" CssClass="labelFormulario" Width="140px" Text="Ativar Mov" />
                                <asp:DropDownList ID="ddlAtivar" CssClass="selecioneMaior" runat="server" Width="10%">
                                    <asp:ListItem Value="" Text="Selecione"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Ativar"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="Inativar"></asp:ListItem>
                                </asp:DropDownList>
                            </p>
                            <p>
                            </p>
                            <p id="idQtde" runat="server" visible="false">
                                <asp:Label ID="label10" runat="server" CssClass="labelFormulario" Width="140px" Text="Qtde Mov" />
                                <asp:TextBox ID="txtQtdeSubItem" onkeypress="return SomenteNumeroDecimal(event)"
                                    runat="server" CssClass="textBox"></asp:TextBox>
                            </p>
                            <p id="idValor" runat="server" visible="false">
                                <asp:Label ID="label2" runat="server" CssClass="labelFormulario" Width="140px" Text="Valor Mov" />
                                <asp:TextBox ID="txtValorSubItem" onkeypress="return SomenteNumeroDecimal(event)"
                                    runat="server" CssClass="textBox"></asp:TextBox>
                            </p>
                        </div>
                        <p class="botaoRight">
                            <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="btnSalvar_Click" />
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                        <p>
                        </p>
                    &nbsp;</p></div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownload" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
