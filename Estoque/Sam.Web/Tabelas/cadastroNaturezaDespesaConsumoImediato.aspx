<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroNaturezaDespesaConsumoImediato.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroNaturezaDespesaConsumoImediato" Title="Módulo Tabelas :: Catálogo :: Natureza Despesa (Consumo Imediato)" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Catálogo - Naturezas de Despesa (Consumo Imediato)</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>
        <asp:GridView ID="gridNaturezaDespesaConsumoImediato" runat="server" AllowPaging="true" OnPageIndexChanged="gridNaturezaDespesaConsumoImediato_PageIndexChanged"
            OnPageIndexChanging="gridNaturezaDespesaConsumoImediato_PageIndexChanging"
            OnSelectedIndexChanged="gridNaturezaDespesaConsumoImediato_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("Codigo") %>' Width="50px"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo"  HeaderText="Código" ItemStyle-Width="50px"  DataFormatString="{0:D8}" ApplyFormatInEditMode="False" >
                    <ItemStyle Width="50px" />
                </asp:BoundField>
                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Codigo") %>' />
                        <asp:Label ID="lblCodigoNaturezaDespesaConsumoImediato" runat="server" Text='<%# Bind("Codigo") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridNaturezaDespesaConsumoImediato" runat="server" EnablePaging="True"
            MaximumRowsParameterName="maximumRowsParameterName" OldValuesParameterFormatString="original_{0}"
            SelectCountMethod="TotalRegistros" SelectMethod="PopularDadosNaturezasDepesaConsumoImediato" StartRowIndexParameterName="startRowIndexParameterName"
            TypeName="Sam.Presenter.NaturezaDespesaConsumoImediatoPresenter">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                    Text="Imprimir" AccessKey="I" />
                <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                    AccessKey="A" onclick="btnAjuda_Click" />
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                    AccessKey="V" />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <br />
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="100px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" CssClass="inputFromNumero" MaxLength="8"
                                runat="server" size="8"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="100px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="50" size="90"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="100px" Text="Status:*"></asp:Label>
                            <asp:DropDownList ID="ddlStatus" runat="server" DataTextField="Descricao" Width="155px" DataValueField="Id">
                                <asp:ListItem Value="True">Ativo</asp:ListItem>
                                <asp:ListItem Value="False">Inativo</asp:ListItem>
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
                        <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                            OnClick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                            OnClick="btnCancelar_Click" />
                    </p>
                </div>
            </div>
        </asp:Panel>
        </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
