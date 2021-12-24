<%@ Page Title="Módulo Financeiro :: Outras :: Empenho Evento" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroEmpenhoEvento.aspx.cs" Inherits="Sam.Web.Financeiro.cadastroEmpenhoEvento" ValidateRequest="false" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Outras - Empenho Evento</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <asp:GridView ID="gdvEmpenhoEvento" runat="server" AllowPaging="true" OnSelectedIndexChanged="gdvEmpenhoEvento_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkID" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("ID") %>' Width="50px" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Código" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodigoEmpenhoEvento" Text='<%# String.Format("{0:D6}", Eval("Codigo")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Descrição" ItemStyle-Width="525px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescricaoEmpenhoEvento" Text='<%# Bind("Descricao") %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Ano Base" ItemStyle-Width="55px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAnoBaseEmpenhoEvento" Text='<%# Bind("AnoBase") %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Código Estorno" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCodigoEstornoEmpenhoEvento" Text='<%# String.Format("{0:D6}", Eval("CodigoEstorno")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo Mov. Associado" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblTipoMovimentoAssociado" Text='<%# Bind("TipoMovimentoAssociado.Descricao") %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Editar" ItemStyle-Width="15px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo"></HeaderStyle>
                </asp:GridView>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click" Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnVoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx" AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" Visible="false">
                    <br />
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInfoEmpenhoEventoCodigo" runat="server" CssClass="labelFormulario" Width="175px" Text="Código no Siafem*:" Font-Size="12px" Style="float: none !important" />
                                            <asp:TextBox ID="txEmpenhoEventoCodigo" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoEmpenhoEventoCodigoEstorno" runat="server" CssClass="labelFormulario" Width="175px" Text="Código Estorno no Siafem:" Font-Size="12px" Style="float: none !important" />
                                            <asp:TextBox ID="txEmpenhoEventoCodigoEstorno" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoEmpenhoEventoAnoBase" runat="server" CssClass="labelFormulario" Width="150px" Text="Ano-Base*:" Font-Size="12px" Style="float: none !important" />
                                            <asp:TextBox ID="txtEmpenhoEventoAnoBase" runat="server" MaxLength="4" size="10" CssClass="mesAno"></asp:TextBox>(AAAA)
                                        </td>
                                    </tr>
                                </table>
                                <p>
                                    <asp:Label ID="lblInfoEmpenhoEventoDescricao" runat="server" CssClass="labelFormulario" Width="175px" Text="Descrição*:" Font-Size="12px" />
                                    <asp:TextBox ID="txtEmpenhoEventoDescricao" runat="server" MaxLength="100" size="90" />
                                </p>
                                <p>
                                    <asp:Label ID="lblInfoEmpenhoEventoCodigoTipoMovimentoAssociado" runat="server" CssClass="labelFormulario" Width="175px" Text="Tipo Movimento Assoc. *:" Font-Size="12px" />
                                    <asp:DropDownList runat="server" ID="ddlTipoMovimentoAssociado" Width="300px" AutoPostBack="True" />
                                </p>
                                <p>
                                    <asp:Label ID="lblInfoTipoMaterial" runat="server" CssClass="labelFormulario" Width="175px" Text="Tipo Material*:" Font-Size="12px" />
                                    <asp:DropDownList runat="server" ID="ddlTipoMaterial" Width="300px" AutoPostBack="True">
                                        <asp:ListItem Text="Material de Consumo" Value="2" />
                                        <asp:ListItem Text="Material Permanente" Value="3" />
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblInfoEmpenhoEventoStatusAtivo" runat="server" Font-Size="12px" CssClass="labelFormulario" Width="175px" Text="Ind. de Atividade*:" />
                                    <asp:DropDownList runat="server" ID="ddlStatusAtivo" Width="100px" AutoPostBack="True">
                                        <asp:ListItem Text="Ativo" Value="1" />
                                        <asp:ListItem Text="Inativo" Value="0" />
                                    </asp:DropDownList>
                                </p>
                            </fieldset>
                        </div>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div class="Divbotao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
