<%@ Page Title="Módulo Financeiro :: Cadastros :: Datas Fechamento SIAFEM" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="CalendarioFechamentoMensal.aspx.cs" Inherits="Sam.Web.Financeiro.CalendarioFechamentoMensal" ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
<div id="content">
        <h1>
            Módulo Tabelas - Cadastro - Datas Fechamento Mensal SIAFEM</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>

        <fieldset class="fieldset botaoLeft">
            <p>
                <asp:Label ID="lblAnoFechamento" runat="server" CssClass="labelFormulario" Text="Ano Referência:"></asp:Label>
                <asp:DropDownList ID="ddlAnoFechamento" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlAnoFechamento_SelectedIndexChanged" Width="90px"></asp:DropDownList>
            </p>
        </fieldset>

        <asp:GridView ID="gdvCalendarioFechamentoMensal" runat="server" AllowPaging="False" OnSelectedIndexChanged="gdvCalendarioFechamentoMensal_SelectedIndexChanged" OnRowDataBound="gdvCalendarioFechamentoMensal_RowDataBound">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkID" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("ID") %>' Width="50px" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Ano Referência" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCFM_AnoReferencia" Visible="true" CssClass="centralizar" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Mês Referencia" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCFM_MesReferencia" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Fechamento" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCFM_DataFechamento" Visible="true" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
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
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <br />
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblCFM_AnoReferenciaInfo" runat="server" CssClass="labelFormulario" Width="175px" Text="Ano Referência*:" Font-Size="12px" Style="float: none !important" />
                                            <asp:TextBox ID="txtCFM_AnoReferencia" CssClass="inputFromNumero" onblur="preencheZeros(this,'4')" MaxLength="4" runat="server" size="4" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblCFM_MesReferenciaInfo" runat="server" CssClass="labelFormulario" Width="175px" Font-Size="12px" Text="Mês Referência*:" />
                                            <asp:DropDownList runat="server" ID="ddlCFM_MesReferencia" Width="120px" AutoPostBack="True">
                                                <asp:ListItem Value="-1" Selected="True">- Selecione - </asp:ListItem>
                                                <asp:ListItem Text="Janeiro" Value="1" />
                                                <asp:ListItem Text="Fevereiro" Value="2" />
                                                <asp:ListItem Text="Março" Value="3" />
                                                <asp:ListItem Text="Abril" Value="4" />
                                                <asp:ListItem Text="Maio"  Value="5" />
                                                <asp:ListItem Text="Junho" Value="6" />
                                                <asp:ListItem Text="Julho" Value="7" />
                                                <asp:ListItem Text="Agosto" Value="8" />
                                                <asp:ListItem Text="Setembro" Value="9" />
                                                <asp:ListItem Text="Outubro" Value="10" />
                                                <asp:ListItem Text="Novembro" Value="11" />
                                                <asp:ListItem Text="Dezembro" Value="12" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblCFM_DataFechamentoInfo" runat="server" CssClass="labelFormulario" Width="150px" Text="Fechamento*:" Font-Size="12px" Style="float: none !important" />
                                            <asp:TextBox ID="txtCFM_DataFechamento" runat="server" CssClass="dataFormat" MaxLength="10" Width="100px" />
                                        </td>
                                    </tr>
                                </table>
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
                        <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir" OnClick="btnExcluir_Click" />
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
