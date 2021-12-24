<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: UA" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"  AutoEventWireup="true" CodeBehind="cadastroUA.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroUA" ValidateRequest="False"  EnableEventValidation="False" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Estrutura Organizacional - UA</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>   
        <fieldset class="fieldset">
            <div id="Div3">
                <p>
                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="120px" Text="Órgão*:" Font-Bold="true" />
                        <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                            DataTextField="CodigoDescricao" DataValueField="Id" 
                        OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged" 
                        ondatabound="ddlOrgao_DataBound">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="PopularDadosTodosCod" TypeName="Sam.Presenter.OrgaoPresenter">
                        </asp:ObjectDataSource>
                </p>
                <p>
                    <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="120px" Text="UO*:" Font-Bold="true" />
                    <asp:DropDownList runat="server" ID="ddlUO" Width="80%" AutoPostBack="True"
                        DataTextField="CodigoDescricao" DataValueField="Id" 
                        OnSelectedIndexChanged="ddlUO_SelectedIndexChanged" 
                        ondatabound="ddlUO_DataBound">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="sourceListaUo" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="PopularDadosTodosCod" TypeName="Sam.Presenter.UOPresenter">
                                          
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlOrgao" DefaultValue="SelectedValue" 
                                Name="_orgaoId" PropertyName="SelectedValue" />
                        </SelectParameters>
                                          
                    </asp:ObjectDataSource>
                </p>
                <p>
                    <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="120px" Text="UGE*:" Font-Bold="true" />
                    <asp:DropDownList runat="server" ID="ddlUGE" Width="80%" AutoPostBack="True"
                        DataTextField="CodigoDescricao" DataValueField="Id" 
                        OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged" 
                        ondatabound="ddlUGE_DataBound">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="sourceListaUGE" runat="server" 
                        SelectMethod="PopularDadosTodosCodPorUo" 
                        TypeName="Sam.Presenter.UGEPresenter" 
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="ddlUo" DefaultValue="0" 
                                Name="_uoId" PropertyName="SelectedValue" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </p>
            </div>
        </fieldset>
        <br /><br />
        <asp:Panel ID="search2" runat="server">
            <fieldset class="fieldset" id="searchUA">
                <asp:Panel ID="searchUAID" runat="server">
                    <p>
                        <asp:Label ID="lblUAID" runat="server" CssClass="labelFormulario" Text="Código UA:"
                            ClientIDMode="Static" />

                        <asp:TextBox ID="txtNumUA" runat="server" ClientIDMode="Static" />
                        <asp:RegularExpressionValidator ID="RVNumUA" runat="server" ErrorMessage="*"
                            ClientIDMode="Static" ControlToValidate="txtNumUA" Display="Dynamic"
                            Text="Insira Código de UA válido!" ValidationExpression="^([0-9]{4}\\{1}[0-9]+)|([0-9]+)$"></asp:RegularExpressionValidator>
                        <asp:Button runat="server" ID="btnPesquisar" Text="Pesquisar" CssClass="button" ClientIDMode="Static" Width="120px" OnClick="btnPesquisar_Click" />
                    </p>
                </asp:Panel>
            </fieldset>
        </asp:Panel>
        <br />
        <asp:GridView ID="gridUA" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridUA_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                            <asp:Label ID="lblUnidadeID" runat="server" Text='<%# Eval("Unidade.Id") %>' 
                                Visible="False"></asp:Label>
                            <asp:Label ID="lblUAVinculada" runat="server" Text='<%# Eval("UaVinculada") %>' 
                                Visible="False"></asp:Label>
                            <asp:Label ID="lblCentroCustoId" runat="server" 
                                Text='<%# Eval("CentroCusto.Id") %>' Visible="False"></asp:Label>
                            <asp:Label ID="lblIndicadorAtividadeId" runat="server" 
                                Text='<%# Eval("IndicadorAtividade") %>' Visible="False"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D7}">
                    <ItemStyle Width="50px">
                    </ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="Descrição">
                    <ItemTemplate>
                        <asp:Label ID="lblDescricaoUa" runat="server" Text='<%# Eval("Descricao") + ((bool)Eval("IndicadorAtividade") ? "" : " (Inativa)") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                            CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridUA" runat="server" EnablePaging="True" 
            MaximumRowsParameterName="maximumRowsParameterName" 
            OldValuesParameterFormatString="original_{0}" 
            SelectCountMethod="TotalRegistros" SelectMethod="PopularDados" 
            StartRowIndexParameterName="startRowIndexParameterName" 
            TypeName="Sam.Presenter.UAPresenter">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlUGE" Name="_ugeId" 
                    PropertyName="SelectedValue" Type="Int32" />
                <asp:ControlParameter ControlID="txtNumUA" Name="_uaCodigo" 
                    PropertyName="Text" Type="String" />
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
                    AccessKey="A" />
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                    AccessKey="V" />
            </p>
            <p>
            <br />
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <br />
            <div id="interno">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="codigo" runat="server" class="labelFormulario" Width="120px" Text="Código*:"></asp:Label>
                            <asp:TextBox ID="txtCodigo" MaxLength="7" CssClass="inputFromNumero" 
                                runat="server" size="10" />
                        </p>
                        <p>
                            <asp:Label ID="Label6" runat="server" class="labelFormulario" Width="120px" Text="Unidade:"></asp:Label>
                            <asp:DropDownList ID="ddlUnidade" runat="server" 
                                Width="80%" AutoPostBack="True" DataTextField="Descricao" 
                                DataValueField="Id">
                            </asp:DropDownList>
                            <br />
<%--                            <asp:ObjectDataSource ID="sourceListaUnidade" runat="server" 
                                OldValuesParameterFormatString="original_{0}" 
                                SelectMethod="PopularDadosTodosCod" 
                                TypeName="Sam.Presenter.UnidadePresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" 
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
--%>                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" class="labelFormulario" Width="120px" Text="Descrição*:"></asp:Label>
                            <asp:TextBox ID="txtDescricao" runat="server" MaxLength="120" Width="80%"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label7" runat="server" class="labelFormulario" Width="120px" Text="UA Vinculada:"></asp:Label>
                            <asp:TextBox ID="txtUaVinculada" MaxLength="5" runat="server" size="5"></asp:TextBox>
                        </p>
                        <p>
                            <asp:Label ID="Label8" runat="server" class="labelFormulario" Width="120px" Text="Centro de Custo:"></asp:Label>
                            <asp:DropDownList ID="ddlCentroCusto" runat="server" 
                                Width="80%" AutoPostBack="True" DataTextField="Descricao" 
                                DataValueField="Id">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaCentroCusto" runat="server" 
                                OldValuesParameterFormatString="original_{0}" 
                                SelectMethod="PopularDadosTodosCod" 
                                TypeName="Sam.Presenter.CentroCustoPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" Name="_orgaoId" 
                                        PropertyName="SelectedValue" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label4" runat="server" class="labelFormulario" Width="120px" Text="Ind. de Atividade*:"></asp:Label>
                            <asp:DropDownList 
                                runat="server" ID="ddlIndicadorAtividade" Width="100px" AutoPostBack="True">
                                <asp:ListItem Text="Ativo" Value="True" />
                                <asp:ListItem Text="Inativo" Value="False" />
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
