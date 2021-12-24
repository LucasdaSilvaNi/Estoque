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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PesquisaDocumento.ascx.cs" Inherits="Sam.Web.Controles.PesquisaDocumento" %>
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
        <div id="Div3">
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="100px" 
                    Text="Palavra-chave: " Visible="False"/>
                <asp:DropDownList ID="ddlTipoFiltro" runat="server" Width="100px" Visible="false">
                    <asp:ListItem Text="Código" Value="1"  Enabled="false"/>
                    <asp:ListItem Text="Descrição" Value="2" />
                </asp:DropDownList>
                <asp:TextBox ID="txtChave" runat="server" MaxLength="120" Width="264px" 
                    Visible="False" />
                <asp:Button ID="btnProcurar" runat="server" Text="Procurar" 
                    OnClick="btnProcurar_Click" Visible="False" />
            </p>
        </div>
        <br />
        <asp:GridView ID="grdRequisicao" runat="server"
            DataKeyNames="Id" AutoGenerateColumns="False" OnPageIndexChanged="gridItemMaterial_PageIndexChanged"
            OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged" 
            CssClass="tabela" onpageindexchanging="gridItemMaterial_PageIndexChanging" 
            AllowPaging="True">
            <PagerStyle HorizontalAlign="Center" />
            <RowStyle CssClass="" HorizontalAlign="Left" />
            <AlternatingRowStyle CssClass="odd" />
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="true" HeaderText="Nº Documento">
                    <ItemTemplate>
                        <asp:LinkButton id = "linkCodigo" runat="server" Font-Bold="true" CausesValidation="False" OnClientClick='RetornaCodigoDocumento(this)' 
                        CommandName="Select" Text='<%# Eval("NumeroDocumento")%>' ToolTip='<%# Eval("Id")%>' ></asp:LinkButton>
                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("Id") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="DataDocumento" HeaderText="Data Documento" HeaderStyle-Width="200px" FooterStyle-Wrap="false"/>
                <asp:TemplateField HeaderText="Nome" HeaderStyle-Width="200px">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDescricao" Text='<%# Bind("CodigoFormatado") %>'
                                    Visible="true"></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="60%" />
                        </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
     </ContentTemplate>
</asp:UpdatePanel>
