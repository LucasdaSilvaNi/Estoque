<%@ Control Language="C#" CodeBehind="PesquisaSubitemNova.ascx.cs" Inherits="Sam.Web.Controles.PesquisaSubitemNova" %>
<asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
    <ProgressTemplate>
        <div id="progressBackgroundFilter">
        </div>
        <div id="processMessage">
            <center>
                Carregando...</center>
            <br />
            <center>
                <img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel runat="server" ID="updModal">
    <ContentTemplate>
        <div id="Div3">
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="120px" Text="Palavra-chave: " />
                <asp:DropDownList ID="ddlTipoFiltro" runat="server" Width="100px" Visible="False">
                    <asp:ListItem Text="Código" Value="1" />
                    <asp:ListItem Text="Descrição" Value="2" Selected="True" />
                </asp:DropDownList>
                <asp:TextBox ID="txtChave" onkeydown="return (event.keyCode!=13);" runat="server" MaxLength="120" Width="350px" />
                <asp:Button ID="btnProcurar" runat="server" Text="Procurar" OnClick="btnProcurar_Click" />
            </p>
        </div>
        <br />
        <asp:GridView ID="gridItemMaterial" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
            EnableViewState="true" SkinID="GridModal" DataKeyNames="TB_SUBITEM_MATERIAL_ID"
            AutoGenerateColumns="False" AllowPaging="True" CssClass="tabela" ShowFooter="True"
            ShowHeaderWhenEmpty="True" EnableSortingAndPagingCallbacks="True" OnSelectedIndexChanging="gridItemMaterial_SelectedIndexChanging"
            OnPageIndexChanging="gridItemMaterial_PageIndexChanging">
            <PagerStyle HorizontalAlign="Center" />
            <RowStyle CssClass="" HorizontalAlign="Left" />
            <AlternatingRowStyle CssClass="odd" />
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="true" HeaderText="Código">
                    <ItemTemplate>
                        <asp:LinkButton ID="linkCodigo" runat="server" Font-Bold="true" CausesValidation="False"
                            OnClientClick='RetornaCodigo(this)' ommandName="Select" Text='<%# String.Format("{0:D12}", Eval("TB_SUBITEM_MATERIAL_CODIGO"))%>'
                            ToolTip='<%# Eval("TB_SUBITEM_MATERIAL_ID") 
                    + ";;" + Eval("TB_SUBITEM_MATERIAL_DESCRICAO") + ";;" + Eval("TB_UNIDADE_FORNECIMENTO_CODIGO_DESCRICAO")  +
                     ";;" + String.Format("{0:D9}", Eval("TB_ITEM_MATERIAL_CODIGO"))%>'>
                        </asp:LinkButton>
                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("TB_SUBITEM_MATERIAL_ID") %>'
                            Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TB_SUBITEM_MATERIAL_DESCRICAO" HeaderText="Descrição" 
                    HeaderStyle-Width="55%" />               
                <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                 <asp:Label ID="lblDescricao" runat="server" Text='<%# (Convert.ToBoolean(Eval("TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE"))==true ?  "Ativo" : "Inativo") %>'>                                                          
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                <asp:TemplateField HeaderText="Unid">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Natureza Despesa">
                    <ItemTemplate>
                        <%#DataBinder.Eval(Container.DataItem, "TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridItemMaterial" runat="server" SelectMethod="BuscarSubItemMaterial"
            EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
            StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Web.Controles.PesquisaSubitemNova"
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="txtChave" Name="valor" PropertyName="Text" Type="String"
                    DefaultValue="" />
                <asp:ControlParameter ControlID="lblFiltrarAlmox" Name="_FiltrarAlmox" PropertyName="Text"
                    Type="String" DefaultValue="" />
                <asp:ControlParameter ControlID="lblUsaSaldo" Name="_UsaSaldo" PropertyName="Text"
                    Type="String" DefaultValue="" />
                <asp:ControlParameter ControlID="lblFiltraGestor" Name="_FiltraGestor" PropertyName="Text"
                    Type="String" DefaultValue="" />
                <asp:ControlParameter ControlID="lblDivisaoId" Name="_DivisaoId" PropertyName="Text"
                    Type="String" DefaultValue="" />
                <asp:ControlParameter ControlID="lblFiltrarNaturezasDespesaConsumoImediato" Name="_FiltrarNaturezasDespesaConsumoImediato" PropertyName="Text" Type="String" DefaultValue="" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:Label runat="server" ID="lblFiltrarAlmox" />
        <asp:Label runat="server" ID="lblUsaSaldo" />
        <asp:Label runat="server" ID="lblFiltraGestor" />
        <asp:Label runat="server" ID="lblDivisaoId" />
        <asp:Label runat="server" ID="lblFiltrarNaturezasDespesaConsumoImediato" />
    </ContentTemplate>
</asp:UpdatePanel>
<script type="text/javascript">
    var nav = window.Event ? true : false;
    if (nav) {
        window.captureEvents(Event.KEYDOWN);
        window.onkeydown = NetscapeEventHandler_KeyDown;
    } else {
        document.onkeydown = MicrosoftEventHandler_KeyDown;
    }

    function NetscapeEventHandler_KeyDown(e) {
        if (e.which == 13 && e.target.type != 'textarea' && e.target.type != 'submit') {
            return false;
        }
        return true;
    }

    function MicrosoftEventHandler_KeyDown() {
        if (event.keyCode == 13 && event.srcElement.type != 'textarea' &&
        event.srcElement.type != 'submit')
            return false;
        return true;
    }
</script>
