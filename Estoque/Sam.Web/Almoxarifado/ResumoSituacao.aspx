<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="ResumoSituacao.aspx.cs" EnableEventValidation="true" Inherits="Sam.Web.Almoxarifado.ResumoSituacao" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    
    <link href="../CSS/estiloFull.css" rel="stylesheet" type="text/css" />   
    <div id="content">
        <asp:UpdatePanel ID="updForn" runat="server" UpdateMode="Conditional">
            <ContentTemplate>


                <h1>SAM - Resumo da Situação</h1>
                <fieldset class="fieldset" id="search1">

                    <asp:Panel ID="Div3" runat="server">
                        <p>
                            <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="60px" Text="Órgão:" />
                            <asp:DropDownList runat="server" ID="ddlOrgao" Width="46.3%" AutoPostBack="True"
                                DataTextField="CodigoDescricao" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosTodosCod"
                                TypeName="Sam.Presenter.OrgaoPresenter"></asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:UpdatePanel runat="server" ID="pnlCombosEscolha" Style="text-align: left; margin: 10px">
                                <ContentTemplate>
                                    <asp:Label runat="server" ID="lblSelecionaAlmoxarifado" Font-Bold="true" Text="Almoxarifado:" />
                                    <asp:DropDownList ID="ddlAlmoxarifado" runat="server" Width="45%"
                                        AutoPostBack="True" OnSelectedIndexChanged="ddlTipoExportacao_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <p>
                            </p>
                            <div class="sidebar">
                                <%-- Pesquisa Requisição --%>
                                <asp:Label ID="lblRequisicoesPendentes" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="grdRequisicao" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="false" CssClass="tabela" DataKeyNames="Id" OnPageIndexChanging="grdRequisicao_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <RowStyle CssClass="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Nº Requisição" ShowHeader="False" SortExpression="NumeroDocumento" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblNrDocumento" runat="server" Text='<%#  Bind("NumeroDocumento") %>' Visible="true"></asp:Label>
                                                <%-- <asp:LinkButton ID="linkCodigo" runat="server" Font-Bold="true" CausesValidation="False"
                                                            OnClientClick='RetornaCodigoRequisicao(this)' value-obs='<%# Eval("Observacoes")%>' value-obs2='<%# Eval("DataMovimento")%>'
                                                            CommandName="Select" Text='<%# Eval("NumeroDocumento")%>' ToolTip='<%# Eval("Id")%>' value-obs3='<%# Eval("GeradorDescricao")%>'></asp:LinkButton>--%>
                                                <asp:Label ID="lblId" runat="server" Font-Bold="True" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle ForeColor="White" Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="DataMovimento" FooterStyle-Wrap="false" HeaderStyle-ForeColor="White" HeaderStyle-Width="200px" HeaderStyle-Wrap="false" HeaderText="Data Movimento" SortExpression="DataMovimento">
                                            <FooterStyle Wrap="False" />
                                            <HeaderStyle Width="200px" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Width="100px" HeaderStyle-Wrap="false" HeaderText="Descrição" SortExpression="GeradorDescricao">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%#  Bind("GeradorDescricao") %>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Width="60%" />
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="corpo" />
                                </asp:GridView>
                            </div>
                            <div class="sidebar">
                                <%--Grid: Pesquisa Estoque Mínimo--%>
                                <asp:Label ID="lblSubItemEstoqueMinimo" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gridSubItemEstoqueMinimo" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="false" CssClass="tabela" DataKeyNames="Id" OnPageIndexChanging="gridSubItemEstoqueMinimo_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <RowStyle CssClass="Left" />
                                    <Columns>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="Codigo" DataFormatString="{0:D12}" HeaderText="Cód." ItemStyle-Width="50px" ItemStyle-Wrap="true">
                                            <ItemStyle Width="50px" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Descricao" HeaderStyle-HorizontalAlign="Left" HeaderText="Descrição" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="40%" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CodigoNaturezaDesp" HeaderStyle-HorizontalAlign="Left" HeaderText="Natureza" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="90" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="90" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="CodigoUnidadeFornec" HeaderText="UN" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20px" ItemStyle-Wrap="true">
                                            <ItemStyle Width="50px" Wrap="True" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="sidebar">
                                <%--Grid: Estoque Máximo--%>
                                <asp:Label ID="lblSubItemEstoqueMaximo" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gridSubItemEstoqueMaximo" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="tabela" DataKeyNames="Id" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" OnPageIndexChanging="gridSubItemEstoqueMaximo_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <RowStyle CssClass="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="Codigo" DataFormatString="{0:D12}" HeaderText="Cód." ItemStyle-Width="50px" ItemStyle-Wrap="true">
                                            <ItemStyle Width="50px" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Descricao" HeaderStyle-HorizontalAlign="Left" HeaderText="Descrição" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="40%" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CodigoNaturezaDesp" HeaderStyle-HorizontalAlign="Left" HeaderText="Natureza" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="90" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="90" Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="CodigoUnidadeFornec" HeaderText="UN" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20px" ItemStyle-Wrap="true">
                                            <ItemStyle Width="50px" Wrap="True" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="corpo" />
                                </asp:GridView>
                            </div>
                            <div class="sidebar">
                                <%--Grid: Pesquisa SIAFEM--%>
                                <asp:Label ID="lblNotaLancamentoPendenteSIAFEM" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gdvNotaLancamentoSIAFEM" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="false" CssClass="tabela" DataKeyNames="Id" OnPageIndexChanging="gdvNotaLancamentoSIAFEM_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Documento" ShowHeader="False" SortExpression="DocumentoSAM" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%#Bind("DocumentoSAM") %>' Visible="true"></asp:Label>
                                                <asp:Label ID="lblId" runat="server" Font-Bold="True" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Tipo Nota" SortExpression="TipoNotaSiafem">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%#Bind("TipoNotaSIAF") %>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Erro SIAFEM" SortExpression="ErroProcessamentoMsgWS">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%#Bind("ErroProcessamentoMsgWS") %>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="sidebar">
                                <%--Grid: Almoxarifado com Pendências--%>
                                <asp:Label ID="lblAlmoxarifadoPendencia" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gridAlmoxarifadoPendenciaFechamento" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="tabela" DataKeyNames="Id"
                                    HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" OnPageIndexChanging="gridAlmoxarifadoPendenciaFechamento_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <RowStyle CssClass="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="Codigo" HeaderText="Cód." ItemStyle-Wrap="true">
                                            <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Descricao" HeaderStyle-HorizontalAlign="Left" HeaderText="Descrição" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Último Mês-Referência Fechamento" SortExpression="MesRef">
                                            <ItemTemplate>
                                                <asp:Label ID="lblMesReferencia" runat="server" Text='<%# (string)(Eval("MesRef")) !=""? (string)(Eval("MesRef")).ToString().Substring(4,2) + "/" + (string)(Eval("MesRef")).ToString().Substring(0,4):""%>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--MesRef--%>
                                    </Columns>
                                    <HeaderStyle CssClass="corpo" />
                                </asp:GridView>
                            </div>
                            <div class="sidebar">
                                <%--UGE's implantadas--%>
                                <asp:Label ID="lblUgeImplantada" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gridUgeImplantada" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="tabela" DataKeyNames="Id"
                                    HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" OnPageIndexChanging="gridUgeImplantada_PageIndexChanging" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <RowStyle CssClass="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <%-- <asp:BoundField ApplyFormatInEditMode="False" DataField="Codigo" HeaderText="Código" ItemStyle-Wrap="true">
                                        <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Descricao" HeaderStyle-HorizontalAlign="Left" HeaderText="Descrição" ItemStyle-Wrap="true">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Wrap="True" />
                                        </asp:BoundField>   

                                      <%--  string.Format("{0:N2}", saldo) SaldoValor--%>
                                        <%--  <asp:BoundField DataField="SaldoValor" HeaderStyle-HorizontalAlign="Left" HeaderText="Saldo" ItemStyle-Wrap="true">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Wrap="True" />
                                        </asp:BoundField>--%>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Código" ShowHeader="False" SortExpression="Codigo" Visible="true">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescricao" runat="server" Text='<%#Bind("Codigo") %>' Visible="true"></asp:Label>
                                                <%-- <asp:Label ID="lblId" runat="server" Font-Bold="True" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Descrição" SortExpression="Descricao">
                                            <ItemTemplate>
                                                <asp:Label ID="Label1" runat="server" Text='<%#Bind("Descricao") %>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false" HeaderText="Saldo" SortExpression="SaldoValor">
                                            <ItemTemplate>
                                                <%-- Eval("SaldoValor").ToString()=="" ?"0,0":  --%>
                                                <asp:Label ID="Label3" runat="server" Text='<%# string.IsNullOrEmpty(Convert.ToString(Eval("SaldoValor")))==true? "0,00" :string.Format("{0:N2}", Eval("SaldoValor"))%>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                    <HeaderStyle CssClass="corpo" />
                                </asp:GridView>
                                <%----%>
                            </div>

                            <div class="sidebar">
                                <%--Grid: Usuários logados--%>
                                <asp:Label ID="lblUsuarioLogadoAlmoxarifado" runat="server" Font-Bold="True" Font-Size="Medium"></asp:Label>
                                <asp:GridView ID="gridUsuarioLogadoAlmoxarifado" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CssClass="tabela" DataKeyNames="Id" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" OnPageIndexChanging="gridUsuarioLogadoAlmoxarifado_PageIndexChanging" OnRowCommand="gridUsuarioLogadoAlmoxarifado_RowCommand" Width="575px">
                                    <SortedAscendingHeaderStyle CssClass="sortasc" />
                                    <SortedDescendingHeaderStyle CssClass="sortdesc" />
                                    <PagerStyle HorizontalAlign="Center" />
                                    <RowStyle CssClass="" HorizontalAlign="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <RowStyle CssClass="Left" />
                                    <AlternatingRowStyle CssClass="odd" />
                                    <Columns>
                                        <asp:BoundField ApplyFormatInEditMode="False" DataField="Usuario" HeaderText="Usuário" ItemStyle-Wrap="true">
                                            <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="UsuarioNome" HeaderStyle-HorizontalAlign="Left" HeaderText="Nome do Usuário" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="DataHoraLogado" HeaderStyle-HorizontalAlign="Left" HeaderText="Logado" ItemStyle-Wrap="true">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle Wrap="True" />
                                        </asp:BoundField>
                                    </Columns>
                                    <HeaderStyle CssClass="corpo" />
                                </asp:GridView>



                                <%----%>
                            </div>

                          

                    </asp:Panel>
                </fieldset>

                
                
                <div id="">

                    <div class="DivButton">
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlAlmoxarifado" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="relativeButton">
            <asp:UpdatePanel ID="Paispanel" runat="server">
                <ContentTemplate>
                    <p>
                    <asp:Label ID="Label11" CssClass="labelFormulario" runat="server" Style="font-size: 14px; color: White !important" Text="Mês/ano:" />
                    <asp:DropDownList ID="ddlAnoMes" AutoPostBack="true" Width="25%"
                        AppendDataBoundItems="true" runat="server">
                    </asp:DropDownList>
                        </p>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlAnoMes" />
                </Triggers>
            </asp:UpdatePanel>
            </div>
        <div class="relativeButton">
         <%--  --%>
              <p>
            <asp:Button ID="btnGerarRelatorioConsolidado" style=" display:inline;  width: 223px !important; position: absolute; left: 201px; top: -23px!important" runat="server" Text="Gerar Relatório Contábil Consolidado" OnClick="btnGerarRelatorioConsolidado_Click" />
                  </p>
        </div>
        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
    </div>

</asp:Content>

