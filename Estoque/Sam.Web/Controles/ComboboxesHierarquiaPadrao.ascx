<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxesHierarquiaPadrao.ascx.cs"
    Inherits="Sam.Web.Controles.ComboboxesHierarquiaPadrao" %>
<fieldset class="fieldset" id="search1">
    <asp:Panel ID="Div3" runat="server">
        <p>
            <asp:Label ID="lblOrgao" runat="server" CssClass="labelFormulario" Text="Orgão*:" /> 
            <asp:DropDownList runat="server" ID="ddlOrgao" AutoPostBack="True" DataTextField="CodigoDescricao" CssClas="comboboxHierarquia" 
                ClientIDMode="Static" DataValueField="Id" OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged"
                OnDataBound="ddlOrgao_DataBound" AppendDataBoundItems="True">
            </asp:DropDownList>
        </p>
        <p>
            <asp:Label ID="lblUO" runat="server" CssClass="labelFormulario" Text="UO*:" />
            <asp:DropDownList runat="server" ID="ddlUO" AutoPostBack="True" DataTextField="CodigoDescricao" CssClas="comboboxHierarquia" 
                ClientIDMode="Static" DataValueField="Id" OnSelectedIndexChanged="ddlUO_SelectedIndexChanged"
                OnDataBound="ddlUO_DataBound">
            </asp:DropDownList>
        </p>
        <p>
            <asp:Label ID="lblUge" runat="server" CssClass="labelFormulario" Text="UGE*:" />
            <asp:DropDownList runat="server" ID="ddlUGE" AutoPostBack="True" DataTextField="CodigoDescricao" CssClas="comboboxHierarquia" 
                ClientIDMode="Static" DataValueField="Id" OnSelectedIndexChanged="ddlUGE_SelectedIndexChanged"
                OnDataBound="ddlUGE_DataBound">
            </asp:DropDownList>
        </p>
        <p>
            <asp:Panel ID="panelComboAlmoxarifados" runat="server" ClientIDMode="Static" />
            <p>
                <asp:Label ID="lblUA" runat="server" CssClass="labelFormulario" Text="UA*:" />
                <asp:DropDownList ID="ddlUA" runat="server" AutoPostBack="True" ClientIDMode="Static" DataTextField="CodigoDescricao" DataValueField="Id" CssClas="comboboxHierarquia" 
                    OnDataBound="ddlUA_DataBound" OnSelectedIndexChanged="ddlUA_SelectedIndexChanged">
                </asp:DropDownList>
            </p>
            <p>
                <asp:Label ID="lblDivisao" runat="server" CssClass="labelFormulario" Text="Divisão*:" />
                <asp:DropDownList ID="ddlDivisao" runat="server" AutoPostBack="True" ClientIDMode="Static" DataTextField="CodigoDescricao" CssClas="comboboxHierarquia" DataValueField="Id" OnDataBound="ddlDivisao_DataBound" OnSelectedIndexChanged="ddlDivisao_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsDivisao" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="PopularDadosDivisao" TypeName="Sam.Presenter.DivisaoPresenter">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter DefaultValue="" Name="maximumRowsParameterName" Type="Int32" />
                        <asp:Parameter DefaultValue="18" Name="_orgaoId" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlUA" DefaultValue="" Name="_uaId" PropertyName="SelectedValue" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </p>
            <p>
                <asp:Label ID="lblPTRES" runat="server" CssClass="labelFormulario" Text="PTRES:" />
                <asp:DropDownList ID="ddlPTRES" runat="server" AutoPostBack="True" DataTextField="Descricao" DataValueField="Id" CssClas="comboboxHierarquia" OnDataBound="ddlPTRES_DataBound" OnSelectedIndexChanged="ddlPTRES_SelectedIndexChanged">
                </asp:DropDownList>
            </p>
            <p>
                <asp:Label ID="lblStatus" runat="server" ClientIDMode="Static" CssClass="labelFormulario" Text="Status:" />
                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="True" DataTextField="Descricao" DataValueField="Id" CssClas="comboboxHierarquia" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                </asp:DropDownList>
            </p>

            <asp:Panel ID="painelStatusProdesp" runat="server" ClientIDMode="Static" CssClass="esconderControle">
                <p>
                    <asp:Label ID="lblStatusAtendimentoUsuarioInfo" runat="server" CssClass="labelFormulario" style="width: 120px;font-size: 13px;" Text="Status (Prodesp):" />
                    <asp:DropDownList runat="server" ID="ddlStatusProdesp" AutoPostBack="True" CssClas="comboboxHierarquia" ClientIDMode="Static" />
                </p>
            </asp:Panel>


            <%-- <p>
            <asp:Label ID="lblNumeroRequisicao" runat="server" CssClass="labelFormulario" Text="Nr. Requisição:" ClientIDMode ="Static" />
            <asp:TextBox ID="txtNumRequisicao" runat="server" ClientIDMode="Static" />
            <asp:RegularExpressionValidator ID="RVNumRequisicao" runat="server" ErrorMessage="*" ClientIDMode="Static" ControlToValidate="txtNumRequisicao" Display="Dynamic" Text="Insira um nÃºmero de requisição válido!" ValidationExpression="^([0-9]{4}\\{1}[0-9]+)|([0-9]+)$"></asp:RegularExpressionValidator>            
        </p>        --%>
            <asp:Panel ID="panelSearch" runat="server" ClientIDMode="Static">
            </asp:Panel>
            <%-- <p id="searchField">
            <table border="0" width="100%">
                <tr>
                    <td width="80%">
                        <asp:Label ID="Label20" CssClass="labelFormulario" Text="Pesquisar:" Width="120px" runat="server" />
                        <asp:TextBox runat="server" ID="txtPesquisar" Text="" Width="20%"></asp:TextBox>
                        <asp:Button runat="server" Text="Pesquisar" SkinID="Btn120" ID="btnPesquisar" OnClick="btnPesquisar_Click" />
                    </td>
                </tr>
            </table>
        </p>--%>
        </p>
    </asp:Panel>
</fieldset>

<br /><br />

<asp:Panel ID="search2" runat="server">
    <fieldset class="fieldset" id="searchNumeroRequisicao">
        <asp:Panel ID="searchRequestNumber" runat="server">
            <p>
                <asp:Label ID="lblNumeroRequisicao" runat="server" CssClass="labelFormulario" Text="Nr. Requisição:"
                    ClientIDMode="Static" />
                <asp:TextBox ID="txtNumRequisicao" runat="server" ClientIDMode="Static" />
                <asp:RegularExpressionValidator ID="RVNumRequisicao" runat="server" ErrorMessage="*"
                    ClientIDMode="Static" ControlToValidate="txtNumRequisicao" Display="Dynamic"
                    Text="Insira um número de requisição válido!" ValidationExpression="^([0-9]{4}\\{1}[0-9]+)|([0-9]+)$"></asp:RegularExpressionValidator>
            </p>
        </asp:Panel>
    </fieldset>
</asp:Panel>

