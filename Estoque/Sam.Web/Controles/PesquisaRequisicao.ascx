<style type="text/css">
    .labelModel
    {
        font-family: Arial;
        display: block;
        float: left;
        font-size: 1.1em;
        font-weight: bold;
        margin-top: 3px;
        margin-right: 5px;
        text-align: right;
    }
</style>
<%@ Control Language="C#" CodeBehind="PesquisaRequisicao.ascx.cs" Inherits="Sam.Web.Controles.PesquisaRequisicao" %>
<asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="300">
    <ProgressTemplate>
        <div style="position: absolute; left: 48%; top: 48%">
            <img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="updForn" runat="server">
    <ContentTemplate>
        <div id="Div3">
            <p>
                <asp:Label ID="lblTipoMovimento" runat="server" Text="" Visible="false" />
                <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="100px" Text="Número Documento: "
                    Visible="true" />
                <asp:DropDownList ID="ddlTipoFiltro" runat="server" Width="100px" Visible="false">
                    <asp:ListItem Text="Código" Value="1" Enabled="false" />
                    <asp:ListItem Text="Descrição" Value="2" />
                </asp:DropDownList>
                <asp:TextBox ID="txtChave" runat="server" MaxLength="120" Width="264px" Visible="true" />
                <asp:Button ID="btnProcurar" runat="server" Text="Procurar" OnClick="btnProcurar_Click"
                    Visible="true" />
            </p>
        </div>
        <br />
        <asp:GridView ID="grdRequisicao" runat="server" DataKeyNames="Id" AutoGenerateColumns="false"
            OnPageIndexChanged="gridItemMaterial_PageIndexChanged" OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged"
            CssClass="tabela" OnPageIndexChanging="gridItemMaterial_PageIndexChanging"
            AllowSorting="True" AllowPaging="True" OnSorting="grdRequisicao_Sorting" 
             OnRowDataBound="grdRequisicao_RowDataBound">

            <SortedAscendingHeaderStyle CssClass="sortasc" />
            <SortedDescendingHeaderStyle CssClass="sortdesc" />
            <PagerStyle HorizontalAlign="Center" />
            <RowStyle CssClass="" HorizontalAlign="Left" />
            <AlternatingRowStyle CssClass="odd" />
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="true" HeaderStyle-Wrap="false" HeaderText="Nº Requisição" SortExpression="NumeroDocumento"
                    HeaderStyle-ForeColor="White" >
                    <ItemTemplate>
                        <asp:LinkButton ID="linkCodigo" runat="server"  Font-Bold="true" CausesValidation="False"
                            OnClientClick='RetornaCodigoRequisicao(this)' value-obs='<%# Eval("Observacoes")%>' value-obs2='<%# Eval("DataMovimento")%>'
                            CommandName="Select" Text='<%# Eval("NumeroDocumento")%>' ToolTip='<%# Eval("Id")%>' value-obs3='<%# Eval("GeradorDescricao")%>'></asp:LinkButton>
                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="DataMovimento" HeaderText="Data Movimento" HeaderStyle-Width="200px"
                    FooterStyle-Wrap="false" SortExpression="DataMovimento" HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false">
                    <FooterStyle Wrap="False" />
                    <HeaderStyle Width="200px" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Descrição" HeaderStyle-Width="100px" SortExpression="GeradorDescricao" HeaderStyle-ForeColor="White" HeaderStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDescricao" Text='<%# Bind("GeradorDescricao") %>'
                            Visible="true" ></asp:Label>
                        <%--   <asp:Label runat="server" ID="lblDescricao" Text='<%# Bind("GeradorDescricao") %>'
                            Visible="true"></asp:Label>--%>
                    </ItemTemplate>
                    <HeaderStyle Width="60%" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
