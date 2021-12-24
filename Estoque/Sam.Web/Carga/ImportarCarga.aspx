<%@ Page Title="Módulo Carga :: Carga Inicial :: Almoxarifado" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"
    AutoEventWireup="true" CodeBehind="ImportarCarga.aspx.cs" Inherits="Sam.Web.Carga.ImportarCarga"
    ValidateRequest="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id=" ">
        <h1>
            Módulo Carga - Importar Carga</h1>
        <asp:GridView SkinID="GridNovo" ID="grdPendentes" runat="server" AllowPaging="True"
            OnRowCommand="grdPendentes_RowCommand" 
            OnRowCreated="grdPendentes_RowCreated" onrowupdating="grdPendentes_RowUpdating" 
            onselectedindexchanged="grdPendentes_SelectedIndexChanged" 
            onselectedindexchanging="grdPendentes_SelectedIndexChanging">
            <Columns>
                <%--<asp:BoundField DataField="TB_CONTROLE_NOME_ARQUIVO" HeaderText="Nome Arquivo" ItemStyle-Width="30%">
                </asp:BoundField>--%>
                <asp:BoundField DataField="TB_CONTROLE_DATA_OPERACAO" HeaderText="Data" ItemStyle-Width="20%">
                </asp:BoundField>
                <asp:TemplateField HeaderText="Tipo" ItemStyle-Width="30%">
                    <ItemTemplate>
                        <asp:HiddenField ID="idTipo" runat="server" Value='<%# Bind("TB_TIPO_CONTROLE.TB_TIPO_CONTROLE_ID") %>' />
                        <%#DataBinder.Eval(Container.DataItem, "TB_TIPO_CONTROLE.TB_TIPO_CONTROLE_DESCRICAO")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Situação" ItemStyle-Width="30%">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "TB_CONTROLE_SITUACAO.TB_CONTROLE_SITUACAO_DESCRICAO")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Exportar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDExp" runat="server" Font-Bold="true" ImageUrl="~/Imagens/excel-2010-icon.png"
                            Width="25px" CausesValidation="false" CommandName="Exportar" CommandArgument='<%# Bind("TB_CONTROLE_ID") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Erros" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDErros" runat="server" Font-Bold="true" ImageUrl="~/Imagens/erroDoc.png"
                            Width="25px" CausesValidation="false" CommandName="Erro" CommandArgument='<%# Bind("TB_CONTROLE_ID") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Executar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDImp" runat="server" Font-Bold="true" ImageUrl="~/Imagens/Misc_Upload_Database.png"
                            Width="25px" Visible='<%#Eval("TB_CONTROLE_SITUACAO.TB_CONTROLE_SITUACAO_DESCRICAO").ToString() == "Pendente" ? true : false %>'
                            CausesValidation="false"  CommandName="Importar" CommandArgument='<%# Bind("TB_CONTROLE_ID") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Cancelar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkIDExcluir" runat="server" Font-Bold="true" ImageUrl="~/Imagens/Delete2.png"
                            Width="25px" CausesValidation="false" CommandName="Excluir" CommandArgument='<%# Bind("TB_CONTROLE_ID") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGrid" runat="server" SelectMethod="ListarControleCarga"
            TypeName="Sam.Presenter.CargaPresenter" EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName"
            SelectCountMethod="TotalRegistros" StartRowIndexParameterName="startRowIndexParameterName"
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <div id="DivButton" class="DivButton">
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Seguranca/TABMenu.aspx"
                    AccessKey="V" />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <fieldset class="fieldset">
                <p>
                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="100px" Text="Tipo Arquivo:"
                        Font-Bold="true"></asp:Label>
                    <asp:DropDownList runat="server" ID="ddlTipoControle" DataSourceID="odsTipoControle"
                        DataTextField="TB_TIPO_CONTROLE_DESCRICAO" DataValueField="TB_TIPO_CONTROLE_ID"
                        Width="50%">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsTipoControle" runat="server" SelectMethod="SelectTipoControle"
                        TypeName="Sam.Presenter.CargaPresenter"></asp:ObjectDataSource>
                </p>
                <p>
                    <asp:Label ID="lblUpload" runat="server" CssClass="labelFormulario" Width="100px"
                        Text="Arquivo Excel:" Font-Bold="true"></asp:Label>
                    <asp:UpdatePanel ID="updFileUp" runat="server">
                        <ContentTemplate>
                            <div style="text-align: left">
                                <asp:FileUpload ID="fulExcel" runat="server" Width="550px" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="btnImportar" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <p>
                    </p>
                </p>
            </fieldset>
            <div class="Divbotao">
                <!-- simula clique link editar/excluir -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button runat="server" SkinID="Btn120" ID="btnImportar" Text="Subir Arquivo"
                            OnClick="btnImportar_Click" />
                        <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                            OnClick="btnCancelar_Click" />
                    </p>
                </div>
            </div>
        </asp:Panel>
        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
    </div>
</asp:Content>
