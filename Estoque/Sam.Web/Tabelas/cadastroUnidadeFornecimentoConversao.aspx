<%@ Page Title="Módulo Tabelas :: Outras :: Unidade Fornecimento (Conversão)" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" EnableEventValidation="true" CodeBehind="cadastroUnidadeFornecimentoConversao.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroUnidadeFornecimentoConversao" 
 ValidateRequest="false" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1> Módulo Tabelas - Outras - Unidade Fornecimento (Conversão)</h1> 
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>
        <asp:GridView ID="gridUnidadeFornecimentoConversao" runat="server" AllowPaging="true" OnSelectedIndexChanged="gridUnidadeFornecimentoConversao_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkItem" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("ID") %>' Width="50px"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo" HeaderText="Código" ItemStyle-Width="50px"  DataFormatString="{0:D6}" ApplyFormatInEditMode="False" />
                <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                <asp:BoundField DataField="SistemaSiafisicoDescricao" HeaderText="Unidade Sistema SIAFISICO" />
                <asp:BoundField DataField="SistemaSamDescricao" HeaderText="Unidade Sistema SAM" />
                <asp:BoundField DataField="FatorUnitario" HeaderText="Fator Conversão" />
                
                <asp:BoundField DataField="SistemaSiafisicoCodigo" HeaderText="SistemaSiafisicoCodigo" HeaderStyle-CssClass="esconderControle" ItemStyle-CssClass="esconderControle" />
                <asp:BoundField DataField="SistemaSamId" HeaderText="SistemaSamId" HeaderStyle-CssClass="esconderControle" ItemStyle-CssClass="esconderControle" />

                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click" Text="Imprimir" AccessKey="I" />
                <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass="" AccessKey="A" onclick="btnAjuda_Click" />
                <asp:Button ID="btnVoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx" AccessKey="V" />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <br />
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="lblCodigo" runat="server" CssClass="labelFormulario" Width="140px" Text="Código*:" />
                            <asp:TextBox ID="txtCodigo" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6"  Width="275px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblDescricao" runat="server" CssClass="labelFormulario" Width="140px" Text="Descrição*:" />
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" size="60" Width="275px"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="lblUnidadeFornecimentoSistemaSiafisico" runat="server" CssClass="labelFormulario" Width="140px" Text="Unidade SIAFISICO*:" />
                            <asp:DropDownList runat="server" ID="ddlUnidadeFornecimentoSistemaSiafisico" Width="275px" DataTextField="Descricao" DataValueField="Id"></asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblUnidadeFornecimentoSistemaSam" runat="server" CssClass="labelFormulario" Width="140px" Text="Unidade SAM*:" />
                            <asp:DropDownList runat="server" ID="ddlUnidadeFornecimentoSistemaSam" Width="275px" DataTextField="Descricao" DataValueField="Id"></asp:DropDownList>
                        </p>
                        <p>
                            <asp:Label ID="lblFatorUnitario" runat="server" CssClass="labelFormulario" Width="140px" Text="Fator Conversão*:" />
                            <asp:TextBox ID="txtFatorUnitario" runat="server" CssClass="numerico" MaxLength="18" size="18" Width="275px"></asp:TextBox>&nbsp;(Unidade SIAFISICO / Unidade SAM)</p>
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
