<%@ Control Language="C#" CodeBehind="PesquisaSubitem.ascx.cs" Inherits="Sam.Web.Controles.PesquisaSubitem"%>
    <asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
        <ProgressTemplate>
            <div id="progressBackgroundFilter"></div>
            <div id="processMessage"><center>Carregando...</center><br />
                    <center><img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel runat="server" ID="updModal">
    <ContentTemplate>
        <div id="Div3">
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="120px" Text="Palavra-chave: " />
                <asp:DropDownList ID="ddlTipoFiltro" runat="server" Width="100px" 
                    Visible="False">
                    <asp:ListItem Text="Código" Value="1" />
                    <asp:ListItem Text="Descrição" Value="2" Selected="True" />
                </asp:DropDownList>
                <asp:TextBox ID="txtChave" runat="server" MaxLength="120" Width="350px" EnableViewState="false"/>
                <asp:Button ID="btnProcurar" runat="server" Text="Procurar" OnClick="btnProcurar_Click" />
            </p>
        </div>
        <br />
        <asp:GridView ID="gridItemMaterial" runat="server" 
            HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass" EnableViewState="true"
            DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanged="gridItemMaterial_PageIndexChanged"
            OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged" 
            CssClass="tabela" ShowFooter="True" ShowHeaderWhenEmpty="True" 
            EnableSortingAndPagingCallbacks="True" 
            onpageindexchanging="gridItemMaterial_PageIndexChanging">
            <PagerStyle HorizontalAlign="Center" />
            <RowStyle CssClass="" HorizontalAlign="Left" />
            <AlternatingRowStyle CssClass="odd" />
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="true" HeaderText="Código">
                    <ItemTemplate>
                        <asp:LinkButton id = "linkCodigo" runat="server" Font-Bold="true" CausesValidation="False" OnClientClick='RetornaCodigo(this)'
                        CommandName="Select" Text='<%# String.Format("{0:D12}", Eval("Codigo"))%>' ToolTip='<%# Eval("Id") + " - " + Eval("Descricao") + " - " + Eval("UnidadeFornecimento.Descricao")  + " - " + String.Format("{0:D9}", Eval("ItemMaterial.Codigo"))%>'></asp:LinkButton>
                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="Descricao" HeaderText="Descrição"  HeaderStyle-Width="70%"/>
                <asp:TemplateField HeaderText="Unid." HeaderStyle-Width="20%">
                <ItemTemplate>
                    <asp:Label runat="server" ID="Label5" Text='<%# Bind("UnidadeFornecimento.Descricao") %>' Visible="true"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridItemMaterial" runat="server" EnablePaging="True"
            MaximumRowsParameterName="maximumRowsParameterName" StartRowIndexParameterName="startRowIndexParameterName"
            SelectCountMethod="TotalRegistros" SelectMethod="ListarSubItemAlmoxPorPalavraChave"
            TypeName="Sam.Presenter.SubItemMaterialPresenter" 
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" DefaultValue="" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                <asp:ControlParameter ControlID="ddlTipoFiltro" Name="opcao" PropertyName="SelectedValue"
                    Type="String" />
                <asp:ControlParameter ControlID="txtChave" Name="valor" PropertyName="Text" 
                    Type="String" DefaultValue="" />
                <asp:Parameter DefaultValue="" Name="comSaldo" Type="Boolean" />
            </SelectParameters>
        </asp:ObjectDataSource>   
    </ContentTemplate>
    </asp:UpdatePanel>
