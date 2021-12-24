<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="GerenciaCatalogo.aspx.cs" Inherits="Sam.Web.GerenciaCatalogo" Title="Módulo Tabelas :: Catálogo :: Subitem Material" %>

<%@ Register Src="../Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova"
    TagPrefix="uc2" %>
<%@ Register Src="../Controles/PesquisaItemNova.ascx" TagName="PesquisaItemNova"
    TagPrefix="uc3" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>Gerência de Catálogo do Almoxarifado</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <div style="margin-bottom: 20px; margin-top: 20px">
                    <table>
                        <tr>
                            <td style="width: 10px">
                                <asp:CheckBox ID="chbNatureza" runat="server" AutoPostBack="true" OnCheckedChanged="chbNatureza_CheckedChanged" />
                            </td>
                            <td style="width: 1500px">
                                <fieldset class="fieldset">
                                    <p>
                                        <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="140px" Text="Natureza de Despesa:"
                                            Font-Bold="true" />
                                        <asp:DropDownList runat="server" ID="ddlNatureza" Width="80%" DataTextField="CodigoDescricao"
                                            DataValueField="Id" AutoPostBack="True" OnSelectedIndexChanged="ddlNatureza_SelectedIndexChanged" />
                                    </p>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px">
                                <asp:CheckBox ID="chbSubItem" runat="server" AutoPostBack="true" OnCheckedChanged="chbSubItem_CheckedChanged" />
                            </td>
                            <td style="width: 1500px">
                                <fieldset class="fieldset">
                                    <p>
                                        <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="100px" Text="SubItem:"></asp:Label>
                                        <asp:TextBox ID="txtSubItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                                            CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                                        <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                                            ImageUrl="../Imagens/lupa.png" ToolTip="Pesquisar" OnClick="imgLupaSubItem_Click" />
                                        <asp:HiddenField ID="idSubItem" runat="server" />
                                        <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                                    </p>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px">
                                <asp:CheckBox ID="chbItem" runat="server" AutoPostBack="true" OnCheckedChanged="chbItem_CheckedChanged" />
                            </td>
                            <td style="width: 1500px">
                                <fieldset class="fieldset">
                                    <p>
                                        <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="100px" Text="Item:"></asp:Label>
                                        <asp:TextBox ID="txtItem" MaxLength="9" onblur="return preencheZeros(this,'9')" onkeypress='return SomenteNumero(event)'
                                            CssClass="inputFromNumero" runat="server" size="9" Enabled="false"></asp:TextBox>
                                        <asp:ImageButton ID="imgLupaItem" runat="server" CommandName="Select" CssClass="basic"
                                            ImageUrl="../Imagens/lupa.png" ToolTip="Pesquisar" OnClick="imgLupaItem_Click" />
                                        <asp:HiddenField ID="itemMaterialId" runat="server" />
                                    </p>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px">
                                <asp:CheckBox ID="chbGrupo" runat="server" AutoPostBack="true" OnCheckedChanged="chbGrupo_CheckedChanged" />
                            </td>
                            <td style="width: 1500px">
                                <fieldset class="fieldset">
                                    <%--  <p>
                                        <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="100px" Text="SubItem:"></asp:Label>
                                        <asp:TextBox ID="txtSubItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                                            CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                                        <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                                            ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalItem();" ToolTip="Pesquisar" />
                                        <asp:HiddenField ID="idSubItem" runat="server" />
                                        <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                                    </p>--%>
                                    <p>
                                        <asp:Label ID="Label11" runat="server" CssClass="labelFormulario" Width="100px" Text="Grupo*:"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlGrupo" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlGrupo_SelectedIndexChanged"
                                            OnDataBound="ddlGrupo_DataBound">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="odsListaGrupo" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularDadosGrupoByAlmoxarifadoTodosCod" TypeName="Sam.Presenter.GrupoPresenter">
                                            <SelectParameters>
                                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </p>
                                    <p>
                                        <asp:Label ID="Label12" runat="server" CssClass="labelFormulario" Width="100px" Text="Classe*:"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlClasse" Width="80%" AutoPostBack="True" onchange="limparidSubItem();"
                                            DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlClasse_SelectedIndexChanged"
                                            OnDataBound="ddlClasse_DataBound">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="odsListaClasse" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularDadosClasseComCod" TypeName="Sam.Presenter.ClassePresenter">
                                            <SelectParameters>
                                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                                <asp:ControlParameter ControlID="ddlGrupo" DefaultValue="" Name="_grupoId" PropertyName="SelectedValue"
                                                    Type="Int32" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </p>
                                    <p>
                                        <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="100px" Text="Material*:"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlMaterial" Width="80%" AutoPostBack="True"
                                            onchange="limparidSubItem();" DataTextField="Descricao" DataValueField="Id" OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged"
                                            OnDataBound="ddlMaterial_DataBound">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="odsListaMaterial" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="PopularDadosMaterialComCod" TypeName="Sam.Presenter.MaterialPresenter">
                                            <SelectParameters>
                                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                                <asp:ControlParameter ControlID="ddlClasse" Name="_classeId" PropertyName="SelectedValue"
                                                    Type="Int32" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </p>
                                    <p>
                                        <asp:Label ID="Label14" runat="server" CssClass="labelFormulario" Width="100px" Text="Item*:"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlItem" Width="80%" AutoPostBack="True" DataTextField="Descricao"
                                            onchange="limparidSubItem();" DataValueField="Id" OnSelectedIndexChanged="ddlItem_SelectedIndexChanged"
                                            OnDataBound="ddlItem_DataBound">
                                        </asp:DropDownList>
                                        <asp:ObjectDataSource ID="odsListaItem" runat="server" OldValuesParameterFormatString="original_{0}"
                                            SelectMethod="ListarItemAlmox" TypeName="Sam.Presenter.ItemMaterialPresenter">
                                            <SelectParameters>
                                                <asp:ControlParameter ControlID="ddlMaterial" Name="_materialId" PropertyName="SelectedValue"
                                                    Type="Int32" />
                                                <asp:Parameter DefaultValue="7" Name="_gestorId" Type="Int32" />
                                                <asp:Parameter DefaultValue="1" Name="_almoxarifadoId" Type="Int32" />
                                            </SelectParameters>
                                        </asp:ObjectDataSource>
                                    </p>
                                </fieldset>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10px"></td>
                            <td style="width: 1500px">
                                <fieldset class="fieldset">
                                    <p>
                                        <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="140px" Text="Indicador:"
                                            Font-Bold="true" />
                                        <asp:DropDownList ID="ddlIndicador" EnableViewState="true" runat="server" AutoPostBack="true" DataTextField="Descricao" Width="100px"
                                            DataValueField="Id"
                                            OnSelectedIndexChanged="ddlIndicador_SelectedIndexChanged">
                                            <asp:ListItem Value="-1">Todos</asp:ListItem>
                                            <asp:ListItem Value="1">Ativo</asp:ListItem>
                                            <asp:ListItem Value="0">Inativo</asp:ListItem>
                                        </asp:DropDownList>
                                    </p>
                                    <p>
                                        <asp:Label ID="Label9" runat="server" CssClass="labelFormulario" Width="140px" Text="Saldo:"
                                            Font-Bold="true" />
                                        <asp:DropDownList ID="ddlSaldo" EnableViewState="true" runat="server" AutoPostBack="true" DataTextField="Descricao" Width="100px"
                                            DataValueField="Id"
                                            OnSelectedIndexChanged="ddlSaldo_SelectedIndexChanged">
                                            <asp:ListItem Value="-1">Todos</asp:ListItem>
                                            <asp:ListItem Value="1">Com Saldo</asp:ListItem>
                                            <asp:ListItem Value="0">Sem Saldo</asp:ListItem>
                                        </asp:DropDownList>

                                    </p>
                                </fieldset>

                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <table>
                                    <tr>
                                        <td style="width: 400px">
                                            <asp:Button ID="btnAtivar" runat="server" Text="Subitem Saldo Zerado Disponível para Requisição" OnClick="btnAtivar_Click" Style="width: 300px; height: 30px" /></td>
                                        <td></td>
                                        <td style="width: 400px">
                                            <asp:Button ID="btnDesativar" runat="server" Text="Subitem Saldo Zerado Indisponível para Requisição" OnClick="btnDesativar_Click" Style="width: 300px; height: 30px" /></td>
                                        <td></td>
                                    </tr>
                                </table>

                            </td>
                            <td></td>
                        </tr>
                    </table>
                    <%--<table>
                        <tr>
                            <td>Indicador</td>
                            <td>Cód</td>
                            <td>Descrição</td>
                            <td>Natureza</td>
                            <td>UN</td>
                            <td>Disp. p/ Requisição
                                <input type="checkbox" id="chkAll" onclick="toggleCheckBoxes(this)" /></td>
                            <td>Estoque Mínimo</td>
                            <td>Estoque Máximo</td>
                            <td></td>
                            <td></td>
                        </tr>
                    </table>--%>
                    <caption>

                        <asp:GridView ID="gridSubItemMaterial" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                            CssClass="tabela" DataKeyNames="Id" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                            OnPageIndexChanged="gridSubItemMaterial_PageIndexChanged" OnRowDataBound="gridSubItemMaterial_RowDataBound"
                            OnSelectedIndexChanged="gridSubItemMaterial_SelectedIndexChanged" OnRowCommand="gridSubItemMaterial_RowCommand">
                            <RowStyle CssClass="Left" />
                            <AlternatingRowStyle CssClass="odd" />
                            <Columns>
                                <asp:TemplateField HeaderText="Indicador">
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkIndicador" Checked='<%# Bind("IndicadorAtividadeAlmox") %>'
                                                runat="server" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--     <asp:CheckBoxField HeaderText="Indicador" DataField="IndicadorAtividadeAlmox" ReadOnly="true" />--%>
                                <asp:TemplateField ShowHeader="False" Visible="false">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select"
                                            Font-Bold="true" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField ApplyFormatInEditMode="False" DataField="Codigo" DataFormatString="{0:D12}"
                                    HeaderText="Cód." ItemStyle-Width="50px" ItemStyle-Wrap="true">
                                    <ItemStyle Width="50px" Wrap="True" />
                                </asp:BoundField>
                                <asp:BoundField DataField="Descricao" HeaderStyle-HorizontalAlign="Left" HeaderText="Descrição"
                                    ItemStyle-HorizontalAlign="Left" ItemStyle-Width="40%" ItemStyle-Wrap="true">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="40%" Wrap="True" />
                                </asp:BoundField>
                                <asp:BoundField DataField="CodigoNaturezaDesp" HeaderStyle-HorizontalAlign="Left" HeaderText="Natureza"
                                    ItemStyle-HorizontalAlign="Left" ItemStyle-Width="90" ItemStyle-Wrap="true">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="90" Wrap="True" />
                                </asp:BoundField>
                                <asp:BoundField ApplyFormatInEditMode="False" DataField="CodigoUnidadeFornec"
                                    HeaderText="UN" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="true">
                                    <ItemStyle Width="50px" Wrap="True" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Disp. p/ Requisição">
                                    <%--   <HeaderTemplate>
                         
                                        <asp:CheckBox ID="chkb1" runat="server" Text="Disp. p/ Requisição" OnCheckedChanged="sellectAll" AutoPostBack="true" />

                                    </HeaderTemplate>--%>
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkDisp" Checked='<%# Bind("IndicadorDisponivel") %>' runat="server" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Subitem Saldo Zerado Disponível para Requisição">
                                    <%--   <HeaderTemplate>
                         
                                        <asp:CheckBox ID="chkb1" runat="server" Text="Disp. p/ Requisição" OnCheckedChanged="sellectAll" AutoPostBack="true" />

                                    </HeaderTemplate>--%>
                                    <ItemTemplate>
                                        <center>
                                            <asp:CheckBox ID="chkDispZerado" Checked='<%# Bind("IndicadorDisponivelZerado") %>' runat="server" />
                                        </center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--   <asp:BoundField DataField="IndicadorDisponivelDescricao" HeaderStyle-HorizontalAlign="Left"
                                    HeaderText="Disp. p/ Requisição" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="15%"
                                    ItemStyle-Wrap="true">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="15%" Wrap="True" />
                                </asp:BoundField>--%>

                                <asp:TemplateField HeaderText="Saldo Unitário do SubItem">
                                    <ItemTemplate>
                                        <center>
                                            <asp:TextBox ID="txtsaldoitemunit" CssClass="txtsaldoitemunit" Text='<%#Eval("SaldoSubItemUnit","{0:#,#0.000}")%>'
                                                ReadOnly="true" Width="90" runat="server" onkeypress="return SomenteNumeroDecimal(event)" /></center>
                                    </ItemTemplate>
                                </asp:TemplateField>




                                <asp:TemplateField HeaderText="Estoque Mínimo">
                                    <ItemTemplate>
                                        <center>
                                            <asp:TextBox ID="txtEstoqueMinimo" CssClass="txtEstoqueMaximo" Text='<%#Eval("EstoqueMinimo","{0:#,#0.000}")%>'
                                                Width="90" runat="server" onkeypress="return SomenteNumeroDecimal(event)" /></center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--  <asp:BoundField DataField="EstoqueMinimo" HeaderStyle-HorizontalAlign="Left" HeaderText="Estoque Mínimo"
                                    ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" ItemStyle-Wrap="true">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="True" />
                                </asp:BoundField>--%>
                                <asp:TemplateField HeaderText="Estoque. Máximo">
                                    <ItemTemplate>
                                        <center>
                                            <asp:TextBox ID="txtEstoqueMaximo" CssClass="txtEstoqueMaximo" Text='<%#Eval("EstoqueMaximo","{0:#,#0.000}")%>'
                                                Width="90" runat="server" onkeypress="return SomenteNumeroDecimal(event)" /></center>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--     <asp:BoundField DataField="EstoqueMaximo" HeaderStyle-HorizontalAlign="Left" HeaderText="Estoque. Máximo"
                                    ItemStyle-HorizontalAlign="Left" ItemStyle-Width="10%" ItemStyle-Wrap="true">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" Width="10%" Wrap="True" />
                                </asp:BoundField>--%>
                                <%--<asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="linkID" runat="server" CausesValidation="False" CommandArgument='<%# Bind("Id") %>'
                                            CommandName="Select" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>--%>
                                <asp:TemplateField ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <asp:LinkButton CssClass="button3" ID="lnbSalvar" CommandName="Select" CommandArgument='<%# Bind("Id") %>'
                                            runat="server" OnClick="lnbsalvar"> Salvar</asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="50px" Visible="false">
                                    <ItemTemplate>
                                        <asp:LinkButton CssClass="button3" ID="lnbExcluir" Visible="false" CommandName="Excluir" CommandArgument='<%# Bind("Id") %>'
                                            runat="server">Excluir</asp:LinkButton>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="corpo" />
                        </asp:GridView>

                        <asp:ObjectDataSource ID="sourceGridSubItemMaterial" runat="server" EnablePaging="True"
                            MaximumRowsParameterName="maximumRowsParameterName" OldValuesParameterFormatString="original_{0}"
                            SelectCountMethod="TotalRegistros" SelectMethod="PopularDadosAlmoxPorMaterial"
                            StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Presenter.SubItemMaterialPresenter">
                            <SelectParameters>
                                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                <asp:ControlParameter ControlID="ddlGrupo" Name="_grupoId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ddlClasse" Name="_classeId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ddlMaterial" Name="_materialId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ddlItem" Name="_itemId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:Parameter DefaultValue="7" Name="_gestorID" Type="Int32" />
                                <asp:Parameter DefaultValue="" Name="_almoxarifadoId" Type="Int32" />
                                <%--<asp:Parameter DefaultValue="" Name="_SubItemcodigo" Type="Int64" />--%>
                                <asp:ControlParameter ControlID="ddlNatureza" Name="iNaturezaDespesa_ID" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="ddlIndicador" Name="_indicadorId" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="txtSubItem" DefaultValue="" Name="_SubItemcodigo"
                                    Type="Int64" />
                                <asp:ControlParameter ControlID="txtItem" DefaultValue="" Name="_Itemcodigo" Type="Int64" />
                                <asp:ControlParameter ControlID="ddlSaldo" Name="_saldoId" PropertyName="SelectedValue"
                                    Type="Int32" />

                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </caption>
                    <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                </div>
                <div class="DivButton">
                    <caption>
                        <p class="botaoLeft">
                            <asp:Button ID="btnNovo" runat="server" AccessKey="N" CssClass="" OnClick="btnNovo_Click"
                                Text="Novo" />
                        </p>
                        <p class="botaoRight">
                            <asp:Button ID="btnImprimir" Visible="false" runat="server" AccessKey="I" CssClass=""
                                OnClick="btnImprimir_Click" Text="Imprimir" />
                            <asp:Button ID="btnAjuda" runat="server" AccessKey="A" CssClass="" OnClientClick="OpenModal();"
                                Text="Ajuda" Visible="true" />
                            <asp:Button ID="btnvoltar" runat="server" AccessKey="V" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                                Text="Voltar" />
                        </p>
                    </caption>
                </div>
                <asp:Panel runat="server" ID="pnlEditar">
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlEditar2" Visible="false">
                    <div id="interno">

                        <fieldset class="fieldset">
                            <p>
                                <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="150px" Text="Código*:"></asp:Label>
                                <asp:Label ID="lblCod" runat="server" CssClass="" Width="300px"></asp:Label>
                            </p>
                            <p>
                                <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="150px" Text="Descrição*:"></asp:Label>
                                <asp:Label ID="lblDescricao" runat="server" CssClass="" Width="500px"></asp:Label>
                            </p>
                            <p>
                                <asp:Label ID="Label8" runat="server" class="labelFormulario" Width="150px" Text="Disp. p/ requisição*:"></asp:Label>
                                <asp:DropDownList ID="ddlDisponivel" runat="server" AutoPostBack="True" DataTextField="Descricao"
                                    DataValueField="Id" OnSelectedIndexChanged="ddlUnidadeFornecimento_SelectedIndexChanged"
                                    Width="273px">
                                    <asp:ListItem Selected="True" Value="0">Não</asp:ListItem>
                                    <%--   <asp:ListItem Value="1">Sim. Até o limite</asp:ListItem>--%>
                                    <asp:ListItem Value="2">Sim</asp:ListItem>
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="150px" Text="Indicador:*"></asp:Label>
                                <asp:DropDownList ID="ddlAtividade" runat="server" DataTextField="Descricao" Width="155px"
                                    DataValueField="Id">
                                    <asp:ListItem Value="1">Ativo</asp:ListItem>
                                    <asp:ListItem Value="0">Inativo</asp:ListItem>
                                </asp:DropDownList>
                            </p>
                            <p>
                                <asp:Label ID="Label15" runat="server" CssClass="labelFormulario" Width="150px" Text="Estoque Mínimo:*"></asp:Label>
                                <asp:TextBox ID="txtEstoqueMinimo" runat="server" EnableViewState="False" onkeypress='return SomenteNumero(event)'
                                    MaxLength="9" size="20"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="150px" onkeypress='return SomenteNumero(event)'
                                    Text="Estoque Máximo:*"></asp:Label>
                                <asp:TextBox ID="txtEstoqueMaximo" runat="server" EnableViewState="False" size="20"
                                    MaxLength="9"></asp:TextBox>
                            </p>
                        </fieldset>
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div class="botao">
                        <!-- simula clique link editar/excluir -->
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                                    OnClick="btnExcluir_Click" Visible="False" />
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                                    OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                    <br />
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="dialogSubItem" title="Pesquisar SubItem Material">
            <uc2:PesquisaSubitemNova ID="PesquisaSubitemNova" runat="server" />
        </div>
        <div id="dialogItem" title="Pesquisar Item Material">
            <uc3:PesquisaItemNova ID="PesquisaSubitem1" runat="server" />
        </div>
    </div>
    <script type="text/javascript" language="javascript">


        function limparidSubItem() {
            $("#ctl00_cphBody_idSubItem").attr("value", "");
        }


        $(document).ready(function () {
            $('[id$=chkHeader]').click(function () {
                $("[id$='chkDisp']").attr('checked', this.checked);
            });
        });

        //function toggleCheckBoxes(elem) {

        //    var div = document.getElementById('#ctl00_cphBody_chkDisp');

        //    var chk = div.getElementsByTagName('input');
        //    var len = chk.length;

        //    for (var i = 0; i < len; i++) {
        //        if (chk[i].type === 'checkbox') {
        //            chk[i].checked = elem.checked;
        //        }
        //    }
        //}
        //function AtualizaIndSaldo() {
        //    indicador = $("#ctl00_cphBody_ddlIndicador").val();
        //    saldo = $("#ctl00_cphBody_ddlSaldo").val();
        //    $("#ctl00_cphBody_hdfInfSaldo").val(indicador + "," + saldo);
        //}



        //var itemIndicador = e.options[indicador.selectedIndex].text;
        //var itemSaldo = e.options[saldo.selectedIndex].text;

      

    </script>
</asp:Content>
