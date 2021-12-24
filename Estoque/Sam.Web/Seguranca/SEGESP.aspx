<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" 
    EnableEventValidation="true" CodeBehind="SEGESP.aspx.cs" ValidateRequest="false" Inherits="Sam.Web.Seguranca.SEGESP" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/Calendario.ascx" TagName="Calendario" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>

    <div id="content">
        <h1>Módulo Segurança - Controle de Acesso - Cadastro de ESP</h1>
    </div>

    <asp:UpdatePanel runat="server" ID="udpPanel">
        <ContentTemplate>
            <asp:HiddenField ID="hdnCalendarioEscolhido" runat="server" />

            <div id="loader" class="loader" style="display: none;">
                <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
            </div>
            <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align: text-top">
                <asp:GridView SkinID="GridNovo" ID="grdESP" runat="server" AllowPaging="True"
                    AutoGenerateColumns="false" DataKeyNames="ID" OnSelectedIndexChanged="grdESP_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblId" Text='<%# Bind("ID") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ID" HeaderText="ID" ItemStyle-Width="50px" DataFormatString="{0:D4}" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="EspCodigo" HeaderText="ESP" ItemStyle-Width="50px" DataFormatString="{0:D4}" ItemStyle-HorizontalAlign="Center">
                            <ItemStyle Width="50px"></ItemStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="TermoId" HeaderText="Termo" ItemStyle-Width="50px" DataFormatString="{0:D3}" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="ESPSistemaDescricao" HeaderText="Módulo" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:TemplateField HeaderText="Gestor">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Eval("Gestor.GestorDescricao") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="DataInicioVigencia" HeaderText="Início Vigência" ItemStyle-Width="110px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="DataFimVigencia" HeaderText="Final Vigência" ItemStyle-Width="110px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="QtdeRepositorioPrincipal" HeaderText="Qtde. Almox. (Princ)" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="QtdeRepositorioComplementar" HeaderText="Qtde. Almox. (Compl.)" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="QtdeUsuarioNivelI" HeaderText="Qtde. Usuário Almox." ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:BoundField DataField="QtdeUsuarioNivelII" HeaderText="Qtde. Usuário Requis." ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
                        <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                            <ItemTemplate>
                                <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                    CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("ID") %>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo"></HeaderStyle>
                </asp:GridView>
            </div>
            <div id="DivBotoes" class="DivButton">
                <p class="botaoLeft">
                    <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                </p>
                <p class="botaoRight">
                    <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                        Visible="false" Text="Imprimir" AccessKey="I" />
                    <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                        AccessKey="A" />
                    <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Seguranca/TABMenu.aspx"
                        AccessKey="V" />
                </p>
            </div>
            <asp:Panel runat="server" ID="pnlEditar" ViewStateMode="Enabled">
                <div id="interno">
                    <div>
                        <fieldset class="fieldset">
                            <asp:HiddenField ID="hdnEspID" runat="server" />
                            <p>
                                <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="175px" Text="SAM Módulo:*" AssociatedControlID="ddlModulo"></asp:Label>
                                <asp:DropDownList ID="ddlModulo" runat="server" DataTextField="SAM Módulo" Width="50%">
                                    <asp:ListItem Value="EST">Estoque</asp:ListItem>
                                    <asp:ListItem Value="PAT">Patrimônio</asp:ListItem>
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label ID="Label8" CssClass="labelFormulario" Text="Gestor:*" Width="175px" runat="server" AssociatedControlID="ddlGestor" />
                                <asp:DropDownList ID="ddlGestor" DataTextField="Nome" DataValueField="Id" Width="50%"
                                    runat="server" />
                            </p>
                            <p>
                                <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="175px" Text="Nº ESP*:" AssociatedControlID="txtNumESP" />
                                <asp:TextBox runat="server" ID="txtNumESP" Width="90px" MaxLength="8" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="175px" Text="Termo (Ativo):" AssociatedControlID="txtNumTermo" />
                                <asp:TextBox runat="server" ID="txtNumTermo" Width="90px" MaxLength="4" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="175px" Text="Início Vigência*:" AssociatedControlID="tbIniVigencia" />
                                <asp:TextBox ID="tbIniVigencia" runat="server" CssClass="dataFormat" MaxLength="10" Width="90px"></asp:TextBox>
                                <asp:ImageButton ID="calendar_Link" runat="server" CommandName="Select" ImageUrl="../Imagens/Calendar_scheduleHS.png" OnClientClick="OpenCalendar('tbIniVigencia');" CssClass="basic" ToolTip="Calendário" />
                            </p>
                            <p>
                                <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="175px" Text="Fim Vigência*:" AssociatedControlID="tbFimVigencia" />
                                <asp:TextBox ID="tbFimVigencia" runat="server" CssClass="dataFormat" MaxLength="10" Width="90px"></asp:TextBox>
                                <asp:ImageButton ID="calendar_Link1" runat="server" CommandName="Select" ImageUrl="../Imagens/Calendar_scheduleHS.png" OnClientClick="OpenCalendar('tbFimVigencia');" CssClass="basic" ToolTip="Calendário" />
                            </p>
                            <p>
                                <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="175px" Text="Qtde. Almox. Princ.:" AssociatedControlID="txtQtdeRepoPrinc" />
                                <asp:TextBox runat="server" ID="txtQtdeRepoPrinc" Width="90px" MaxLength="4" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="175px" Text="Qtde. Almoxarifado Complem.:" AssociatedControlID="txtQtdeRepoCompl" />
                                <asp:TextBox runat="server" ID="txtQtdeRepoCompl" Width="90px" MaxLength="4" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="175px" Text="Qtde. Operador:" AssociatedControlID="txtQtdeUserI" />
                                <asp:TextBox runat="server" ID="txtQtdeUserI" Width="90px" MaxLength="5" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="175px" Text="Qtde. Requisitante:" AssociatedControlID="txtQtdeUserII" />
                                <asp:TextBox runat="server" ID="txtQtdeUserII" Width="90px" MaxLength="5" CssClass="textBox" onkeypress="return SomenteNumeroApenas(event)"
                                    TextMode="SingleLine"></asp:TextBox>
                            </p>
                        </fieldset>
                    </div>
                    <div>
                        <p>
                            <asp:HiddenField ID="hdnTipoOperacao" runat="server" />
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                </div>

                <div class="Divbotao">
                    <!-- simula clique link editar/excluir -->
                    <div class="DivButton">
                        <p class="botaoLeft">
                            <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                            <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                OnClick="btnExcluir_Click" Visible="true" />
                            <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                OnClick="btnCancelar_Click" />
                        </p>
                    </div>
                </div>
            </asp:Panel>
            <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div id="dialogCalendar" title="Calendário" class="esconderControle">
        <asp:UpdatePanel ID="updCalendario" runat="server">
            <ContentTemplate>
                <uc2:Calendario runat="server" ID="uc2Calendar" EnableViewState="True" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

