<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="Consultas.aspx.cs" Inherits="Sam.Web.Almoxarifado.Consultas" Title="Módulo Tabelas :: Almoxarifado :: Consultas" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <%--Arquivo UTF-8 com assinatura
Claudio Carvalho - 26/03/2014--%>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <noscript>
        <p>
            O javascript de seu navegador está desabilitado. Favor habilitá-lo para acessar
            as funcionalidades deste site.</p>
    </noscript>
    <!-- Habilitar JavaScript -->
    <div id="content">
        <center>
            <h1>
                Módulo Almoxarifado - Consultas</h1>
        </center>
        <asp:UpdatePanel ID="updPanel" runat="server">
            <ContentTemplate>
                <asp:Menu ID="Menu1" Width="500px" runat="server" Orientation="Horizontal" OnMenuItemClick="Menu1_MenuItemClick">
                    <Items>
                        <asp:MenuItem Text="Estoque" Value="0"></asp:MenuItem>
                        <asp:MenuItem Text="Movimentação" Value="1"></asp:MenuItem>
                        <asp:MenuItem Text="Consumo" Value="2"></asp:MenuItem>
                    </Items>
                    <StaticMenuStyle CssClass="tab" />
                    <StaticHoverStyle CssClass="hov" />
                    <StaticMenuItemStyle CssClass="item" />
                    <StaticSelectedStyle CssClass="selectedTab" />
                </asp:Menu>
                <div class="formulario">
                    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                        <asp:View ID="Tab1" runat="server">
                            <!-- Inicio Aba 1 -->
                            <div id="aba1" class="formulario">
                                <div id="internoAba">
                                    <fieldset class="fieldset">
                                        <p>
                                            <asp:Label ID="Label1" CssClass="labelFormulario" runat="server" Width="120px" Text="Consulta:" />
                                            <asp:DropDownList ID="ddlConsulta" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlConsulta_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </p>
                                        <asp:Panel ID="analitica" runat="server">
                                            <p>
                                                <asp:Label ID="Label2" CssClass="labelFormulario" runat="server" Width="120px" Text="Subitem:" />
                                                <asp:TextBox ID="txtSubItem" size="20" runat="server" Enabled="False" OnTextChanged="txtSubItem_TextChanged" />
                                                <a href="#" id="dialog_link" onclick="OpenModal();">
                                                    <img src="../imagens/lupa.png" alt="Pesquisar" class="basic" /></a>
                                                <asp:HiddenField ID="idSubItem" runat="server" />
                                            </p>
                                            <p>
                                                <asp:Label ID="Label3" CssClass="labelFormulario" runat="server" Width="120px" Text="UGE:" />
                                                <asp:DropDownList ID="ddlUgeAnalitico" CssClass="selecioneMaior" runat="server" />
                                            </p>
                                            <p class="sideduplo">
                                                <asp:Label ID="Label4" CssClass="labelFormulario" runat="server" Width="120px" Text="Período de:" />
                                                <asp:DropDownList ID="ddlPeriodoDe" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriodoDe_SelectedIndexChanged" />
                                            </p>
                                            <p class="sideduplo">
                                                <asp:Label ID="Label5" CssClass="labelFormulario" runat="server" Width="120px" Text="Até:" />
                                                <asp:DropDownList ID="ddlPeriodoAte" runat="server"  />
                                            </p>
                                        </asp:Panel>
                                        <asp:Panel ID="sintetica" runat="server">
                                            <p>
                                                <asp:Label ID="Label6" CssClass="labelFormulario" runat="server" Width="120px" Text="Grupo:" />
                                                <asp:DropDownList ID="ddlGrupo" CssClass="selecioneMaior" runat="server" />
                                            </p>
                                            <p>
                                                <asp:Label ID="Label7" CssClass="labelFormulario" runat="server" Width="120px" Text="UGE:" />
                                                <asp:DropDownList ID="ddlUgeSintetico" CssClass="selecioneMaior" runat="server" />
                                            </p>
                                            <p>
                                                <asp:Label ID="Label8" CssClass="labelFormulario" runat="server" Width="120px" Text="Selecionar:" />
                                                <asp:DropDownList ID="ddlSelSintetico" CssClass="selecioneMaior" runat="server">
                                                    <asp:ListItem Selected="true" Text="Todos" Value="0" />
                                                    <asp:ListItem Text="Com saldo" Value="1" />
                                              <%--      <asp:ListItem Text="Sem saldo" Value="2" />--%>
                                                </asp:DropDownList>
                                            </p>
                                            <p>
                                                <asp:Label ID="Label10" CssClass="labelFormulario" runat="server" Width="120px" Text="Ordenar:" />
                                                <asp:DropDownList ID="ddlOrdenar" CssClass="selecioneMaior" runat="server">
                                                    <asp:ListItem Text="Código" Value="0" Selected="true"/>
                                                    <asp:ListItem Text="Descrição" Value="1" />
                                                    <asp:ListItem Text="Saldo (crescente)" Value="2" />
                                                </asp:DropDownList>
                                            </p>
                                            <p>
                                                <asp:Label ID="Label17" CssClass="labelFormulario" runat="server" Width="120px" Text="Detalhar Lote:" />
                                                <asp:DropDownList runat="server" ID="ddlDetalhesLote" Width="80px">
                                                    <asp:ListItem Text="Sim" Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Não" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                            </p>
                                            <p>
                                                <asp:Label ID="Label18" CssClass="labelFormulario" runat="server" Width="120px" Text="Grupo por ND:" />
                                                <asp:DropDownList runat="server" ID="ddlGrupoND" Width="80px">
                                                    <asp:ListItem Text="Sim" Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Não" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                            </p>
                                            <p>
                                                <asp:Label ID="Label14" CssClass="labelFormulario" runat="server" Width="120px" Text="Agrupado por:"
                                                    Visible="false" />
                                                <asp:DropDownList runat="server" ID="ddlNaturezaDespesa" Visible="false">
                                                    <asp:ListItem Text="Sem Agrupamento" Value="0" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Natureza de Despesa" Value="1"></asp:ListItem>
                                                </asp:DropDownList>
                                            </p>
                                        </asp:Panel>
                                        <p class="botaoRight">
                                            <asp:Button ID="btnImprimir" CssClass="button" runat="server" Text="Relatório" OnClick="btnImprimir_Click"
                                                AccessKey="R" />
                                        </p>
                                    </fieldset>
                                </div>
                                <!-- fim id interno -->
                            </div>
                            <!-- Fim Aba 1  -->
                        </asp:View>
                        <asp:View ID="Tab2" runat="server">
                            <!-- Inicio Aba 2 -->
                            <div id="aba2" class="formulario">
                                <div id="internoAba2">
                                    <fieldset class="fieldset">
                                        <p>
                                            <asp:Label ID="Label9" CssClass="labelFormulario" runat="server" Width="150px" Text="Consulta:" />
                                            <asp:DropDownList ID="ddlConsultaMov" CssClass="selecioneMaior" runat="server" AutoPostBack="True"
                                                OnSelectedIndexChanged="ddlConsultaMov_SelectedIndexChanged">
                                                <asp:ListItem Selected="true" Text="- Selecione -" Value="0" />
                                                <asp:ListItem Text="Documentos de Entrada" Value="1" />
                                                <asp:ListItem Text="Documentos de Saída" Value="2" />
                                                <asp:ListItem Text="Mov. Transferência" Value="3" />
                                                <%--<asp:ListItem Text="Mov. Transferência pendente - (Confirmação de recebimento)" Value="4" />
                                <asp:ListItem Text="Mov. Transferência - (Todos)" Value="5" />--%>
                                            </asp:DropDownList>
                                        </p>
                                        <p>
                                            <asp:Label ID="lblSelecionarMov" CssClass="labelFormulario" runat="server" Width="120px"
                                                Text="Selecione:" />
                                            <asp:DropDownList ID="ddlFornecedorMov" CssClass="selecioneMaior" runat="server"
                                                Width="573px" />
                                            <asp:DropDownList ID="ddlDivisaoMov" CssClass="esconderControle" runat="server" Width="573px" />
                                        </p>
                                        <div class="formulario">
                                            <p class="sideduplo">
                                                <asp:Label ID="Label11" CssClass="labelFormulario" runat="server" Width="120px" Text="Período de:" />
                                                <asp:DropDownList ID="ddlPeriodoDeMov" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriodoDeMov_SelectedIndexChanged"  />
                                            </p>
                                            <p class="sideduplo">
                                                <asp:Label ID="Label12" CssClass="labelFormulario" runat="server" Width="120px" Text="Até:" />
                                                <asp:DropDownList ID="ddlPeriodoAteMov" runat="server"  />
                                            </p>
                                        </div>
                                        <p class="botaoRight">
                                            <asp:Button ID="btnImprimirMovimentacao" CssClass="button" runat="server" Text="Relatório"
                                                OnClick="btnImprimirMovimentacao_Click" />
                                        </p>
                                    </fieldset>
                                </div>
                                <!-- fim id interno -->
                            </div>
                            <!-- Fim Aba 2 -->
                        </asp:View>
                        <asp:View ID="Tab3" runat="server">
                            <!-- Inicio Aba 3 -->
                            <div id="aba3" class="formulario">
                                <div id="internoAba3">
                                    <fieldset class="fieldset">
                                        <p>
                                            <asp:Label ID="Label13" CssClass="labelFormulario" runat="server" Width="120px" Text="Consulta:" />
                                            <asp:DropDownList ID="ddlConsultaConsumo" runat="server" Width="300px" AutoPostBack="True"
                                                OnSelectedIndexChanged="ddlConsultaConsumo_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </p>
                                        <p id="idConsumo" runat="server">
                                            <asp:Label ID="lblConsumoTipo" CssClass="labelFormulario" runat="server" Width="120px"
                                                Text="Almoxarifado:" />
                                            <asp:DropDownList ID="ddlAlmoxConsumo" runat="server" CssClass="mostrarControle"
                                                Width="573px" />
                                            <asp:DropDownList ID="ddlDivisaoConsumo" runat="server" CssClass="esconderControle"
                                                Width="573px" />
                                        </p>
                                         <p id="idConsumoSubItem" runat="server">
                                            <asp:Label ID="Label19" CssClass="labelFormulario" runat="server" Width="120px"
                                                Text="SubItem:" />
                                            <asp:TextBox ID="txtSubItemC" size="20" runat="server" Enabled="False" OnTextChanged="txtSubItem_TextChanged" />
                                                <a href="#" id="A1" onclick="OpenModal();">
                                                    <img src="../imagens/lupa.png" alt="Pesquisar" class="basic" /></a>
                                                <asp:HiddenField ID="idSubItemC" runat="server" />
                                        </p>
                                        <div class="formulario">
                                            <p class="sideduplo">
                                                <asp:Label ID="Label15" CssClass="labelFormulario" runat="server" Width="120px" Text="Período de:" />
                                                <asp:DropDownList ID="ddlPeriodoDeConsumo" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPeriodoDeConsumo_SelectedIndexChanged"   />
                                            </p>
                                            <p class="sideduplo">
                                                <asp:Label ID="Label16" CssClass="labelFormulario" runat="server" Width="120px" Text="Até:" />
                                                <asp:DropDownList ID="ddlPeriodoAteConsumo" runat="server" />
                                            </p>
                                        </div>
                                        <p class="botaoRight">
                                            <asp:Button ID="btnImprimirConsumo" CssClass="button"  runat="server" Text="Relatório"
                                                OnClick="btnImprimirConsumo_Click" />
                                        </p>
                                    </fieldset>
                                </div>
                                <!-- fim id interno -->
                            </div>
                            <!-- Fim Aba 3 -->
                        </asp:View>
                    </asp:MultiView>
                    <div id="container_abas_consultas">
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <div class="DivButton">
                            <p class="botaoRight">
                                <%--<asp:Button ID--%>
                                <asp:Button ID="btnAjuda" CssClass="button" runat="server" Text="Ajuda" AccessKey="A" />
                                <asp:Button ID="btnvoltar" CssClass="button" runat="server" Text="Voltar" PostBackUrl="~/Seguranca/TABMenu.aspx"
                                    AccessKey="V" />
                            </p>
                        </div>
                    </div>
                </div>
                <!-- Conteúdo -->
                <!-- fim id content -->
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="dialog" title="Pesquisar SubItem Material">
            <uc2:PesquisaSubitemNova ID="PesquisaSubitem1" runat="server" />
        </div>
    </div>
</asp:Content>
