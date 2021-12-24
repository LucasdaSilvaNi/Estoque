<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="cadastroItemMaterial.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroItemMaterial"
    Title="Módulo Tabelas :: Catálogo :: Item Material" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Módulo Tabelas - Catálogo - Itens de Material</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <div style="margin-bottom: 20px; margin-top: 20px">
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="lblGrupo" runat="server" class="labelFormulario" Width="100px" Text="Grupo*:"
                                Font-Bold="true"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlGrupo" Width="80%" AutoPostBack="True"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlGrupo_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaGrupo" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosGrupoTodosCod" 
                                TypeName="Sam.Presenter.GrupoPresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="lblClasse" runat="server" class="labelFormulario" Width="100px" Text="Classe*:"
                                Font-Bold="true"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlClasse" Width="80%" AutoPostBack="True"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlClasse_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaClasse" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosClasseComCod" 
                                TypeName="Sam.Presenter.ClassePresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlGrupo" Name="_grupoId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="lblmaterial" runat="server" class="labelFormulario" Width="100px"
                                Text="Material*:" Font-Bold="true"></asp:Label>
                            <asp:DropDownList runat="server" ID="ddlMaterial" Width="80%" AutoPostBack="True"
                                DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaMaterial" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosMaterialComCod" 
                                TypeName="Sam.Presenter.MaterialPresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                    <asp:ControlParameter ControlID="ddlClasse" Name="_classeId" PropertyName="SelectedValue"
                                        Type="Int32" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                    </fieldset>
                </div>
                <asp:GridView ID="gridItemMaterial" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                    DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanged="gridItemMaterial_PageIndexChanged"
                    OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged" CssClass="tabela">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle CssClass="" HorizontalAlign="Left" />
                    <AlternatingRowStyle CssClass="odd" />
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                    CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                <asp:Label ID="lblId" runat="server" Text='<%# Bind("Atividade") %>' Visible="false"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." DataFormatString="{0:D9}" ItemStyle-Width="70px" />
                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                        <asp:BoundField DataField="StatusFormatado" HeaderText="Status" />
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
                <asp:ObjectDataSource ID="sourceGridItemMaterial" runat="server" EnablePaging="True"
                    MaximumRowsParameterName="maximumRowsParameterName" StartRowIndexParameterName="startRowIndexParameterName"
                    SelectCountMethod="TotalRegistros" SelectMethod="PopularDadosClasse" TypeName="Sam.Presenter.ItemMaterialPresenter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlMaterial" Name="_materialId" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <div id="Div1"  class="DivButton">
                    <p class="botaoLeft">
<%--                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />--%>
                        <asp:Button ID="btnSiafisico" CssClass="" style="{width: 150px;}" runat="server" Width="150px" 
                            Text="Importar Siafisico" AccessKey="S" onclick="btnSiafisico_Click" ToolTip="Importa dados de um item do Siafisico para o sistema" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" />
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
                                    <asp:Label ID="codigo" runat="server" class="labelFormulario" Width="121px" 
                                        Text="Código*:"></asp:Label>
                                    <asp:TextBox ID="txtCodigo" CssClass="inputFromNumero"
                                        onblur="preencheZeros(this,'9')" MaxLength="9"
                                        runat="server" size="9" Width="68px"></asp:TextBox> &nbsp;
                                    <asp:Button ID="btnConsultarSiafisico" runat="server" CssClass="" 
                                        Text="Consultar" onclick="btnConsultarSiafisico_Click" />
                                </p>

                                <asp:Panel ID="pnlSiafisico" runat="server">
                                    <p>
                                        <asp:Label ID="Label4" runat="server" class="labelFormulario" Width="121px" 
                                            Text="Grupo:"></asp:Label>
                                        <asp:TextBox ID="txtSiafCodGrupo" runat="server" MaxLength="2" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafDescGrupo" runat="server" MaxLength="120" Width="70%" Enabled="false" />
                                    </p>

                                    <p>
                                        <asp:Label ID="Label5" runat="server" class="labelFormulario" Width="121px" 
                                            Text="Classe:"></asp:Label>
                                        <asp:TextBox ID="txtSiafCodClasse" runat="server" MaxLength="2" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafDescClasse" runat="server" MaxLength="120" Width="70%" Enabled="false" />
                                    </p>

                                    <p>
                                        <asp:Label ID="Label6" runat="server" class="labelFormulario" Width="121px" 
                                            Text="Material:"></asp:Label>
                                        <asp:TextBox ID="txtSiafCodMaterial" runat="server" MaxLength="2" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafDescMaterial" runat="server" MaxLength="120" Width="70%" Enabled="false" />
                                    </p>

                                    <p>
                                        <asp:Label ID="Label7" runat="server" class="labelFormulario" Width="121px" 
                                            Text="Item de Material:"></asp:Label>
                                        <asp:TextBox ID="txtSiafCodItemMaterial" runat="server" MaxLength="2" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafDescItemMaterial" runat="server" MaxLength="120" Width="70%" Enabled="false" />
                                    </p>

                                    <p>
                                        <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="121px" 
                                            Text="Natureza Desp.:"></asp:Label>
                                        <asp:TextBox ID="txtSiafItemNat1" runat="server" MaxLength="8" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafItemNat2" runat="server" MaxLength="8" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafItemNat3" runat="server" MaxLength="8" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafItemNat4" runat="server" MaxLength="8" Width="10%" Enabled="false" />
                                        <asp:TextBox ID="txtSiafItemNat5" runat="server" MaxLength="8" Width="10%" Enabled="false" />
                                    </p>

                                </asp:Panel>

                                <p>
                                    <asp:Label ID="lblDescricao" runat="server" class="labelFormulario" Width="121px" 
                                        Text="Descrição*:"></asp:Label>
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="100" size="90" 
                                        Width="80%"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="lblAtividade" runat="server" class="labelFormulario" Width="121px" 
                                        Text="Ind. de Atividade:*"></asp:Label>
                                    <asp:DropDownList ID="ddlAtividade" runat="server" DataTextField="Descricao" Width="155px"
                                        DataValueField="Id" AutoPostBack="True">
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
                                <asp:Button ID="btnGravarSiafisico" runat="server" Text="Salvar" ToolTip="Salvar os dados Siafisico para o SAM"
                                    onclick="btnGravarSiafisico_Click" />
                                <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                    OnClick="btnExcluir_Click" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>

                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
