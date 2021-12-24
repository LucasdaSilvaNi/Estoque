<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="SEGTipoMovimento.aspx.cs" Inherits="Sam.Web.Seguranca.SEGTipoMovimento" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <div id="content">
        <h1>
            Módulo Segurança - Tipo de Movimento - Mudança de Status</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div id="loader" class="loader" style="display: none;">
                    <img id="img-loader" src="../Imagens/loading.gif" alt="Loading" />
                </div>    
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="Label2" CssClass="labelFormulario" Text="Tipo:" Width="100px" runat="server" />
                            <asp:DropDownList ID="ddlTipoMovimentoAgrupamento" DataTextField="Descricao" DataValueField="Id" Width="80%"
                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlTipoMovimentoAgrupamento_SelectedIndexChanged">
                            </asp:DropDownList>
                        </p>
                    </fieldset>            
                    <asp:GridView SkinID="GridNovo" ID="grdTipoMovimento" runat="server" AllowPaging="True"
                        AutoGenerateColumns="false" DataKeyNames="Id" OnSelectedIndexChanged="grdTipoMovimento_SelectedIndexChanged">
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false"></asp:TemplateField>
                            <asp:BoundField DataField="Id" HeaderText="Id" ItemStyle-Width="5%" HeaderStyle-HorizontalAlign="Center"
                                DataFormatString="{0:D4}">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-Width="80%">
                            </asp:BoundField>
                            <asp:BoundField DataField="Ativo" HeaderText="Ativo" ItemStyle-Width="10%">
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Editar" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                                        CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="sourceGridTipoMovimento" runat="server" SelectMethod="PopularListaTipoMovimentoEntradaOuSaida"
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" 
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.TipoMovimentoPresenter"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="ddlTipoMovimentoAgrupamento" Name="tipoMovimentoAgrupamentoId" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>                
                <div id="DivBotoes" class="DivButton">
                  <%--  <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>--%>
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
                                <p>
                                    <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="150px" Text="Ativo:*"></asp:Label>
                                    <asp:DropDownList ID="ddlAtivo" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                            </fieldset>
                        </div>
                    </div>
                    <div class="Divbotao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
