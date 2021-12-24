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
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PesquisaFornecedor.ascx.cs" Inherits="Sam.Web.Controles.PesquisaFornecedor" %>
<asp:UpdateProgress ID="updateProgress" runat="server" DisplayAfter="1">
    <ProgressTemplate>
        <div id="progressBackgroundFilter"></div>
        <div id="processMessage"><center>Carregando...</center><br />
                <center><img src='<%=ResolveClientUrl("~/Imagens/loading.gif")%>' /></center>
        </div>
    </ProgressTemplate>
</asp:UpdateProgress>
<asp:UpdatePanel ID="updForn" runat="server">
    <ContentTemplate>

        <fieldset class="fieldset">
            <div id="Div3">
                <p>
                    <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="100px" Text="Palavra-chave:" Font-Bold="true" />
                    <asp:TextBox ID="txtFornecedor" runat="server" MaxLength="120" Width="300px" />
                    <asp:Button ID="btnProcurar" runat="server" Text="Procurar" 
                        onclick="btnProcurar_Click" />
<%--                    <asp:ObjectDataSource ID="sourceListaFornecedor" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="PopularFornecedorPorPalavraChave" 
                    TypeName="Sam.Presenter.FornecedorPresenter">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                        <asp:ControlParameter ControlID="txtFornecedor" Name="_chave" 
                            PropertyName="Text" Type="String" />
                    </SelectParameters>
                    </asp:ObjectDataSource>
--%>                </p>
            </div>
        </fieldset>
        <br />

        <asp:GridView ID="gridFornecedor" runat="server" AllowPaging="true" 
            AutoGenerateColumns="False" 
            onselectedindexchanged="gridFornecedor_SelectedIndexChanged" 
            onpageindexchanging="gridFornecedor_PageIndexChanging">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkId" runat="server" Font-Bold="true" CausesValidation="true"
                            CommandName="fornec" Text='<%# Bind("ID") %>' OnClientClick="CloseModal2('dialog');" Width="50px"></asp:LinkButton>
                        <asp:Label ID="lblCodFornecedor" Visible="false" runat="server" Text='<%# Bind("Id") %>'></asp:Label>
                        <asp:Label ID="lblCPFCNPJ" Visible="false" runat="server" Text='<%# Bind("CpfCnpj") %>'></asp:Label>
                        <asp:TextBox ID="txtNome_" Visible="false" runat="server" Text='<%# Bind("Nome") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="CPF/CNPJ" Visible="true">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkCnpj" runat="server" Font-Bold="false"
                            CommandName="Select" Text='<%# Bind("CpfCnpj") %>' OnClientClick="CloseModal();" Width="150px"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Nome" HeaderText="Nome" />
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
