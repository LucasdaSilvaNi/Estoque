<style type="text/css">
    .labelModel
    {
        font-family: Arial;
        display: block;
        float: left;
        font-size: 1.0em;
        font-weight: bold;
        margin-top: 3px;
        margin-right: 2px;
        text-align: right;
    }
</style>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PesquisaDocumentoNova.ascx.cs" Inherits="Sam.Web.Controles.PesquisaDocumentoNova" %>
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
        <div>
            <p>
                <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="150px" Text="Número Documento: " Visible="true"/>
                <asp:TextBox ID="txtChave" runat="server" MaxLength="120" Width="400px" Visible="true" />
                <asp:Button ID="btnProcurar" runat="server" Text="Procurar" OnClick="btnProcurar_Click" Visible="true" />
            </p>
        </div>
        <br />
        <asp:GridView ID="grdDocumento" runat="server" SkinID="GridModal" DataKeyNames="TB_MOVIMENTO_ID" 
                AutoGenerateColumns="False"
            CssClass="tabela" AllowPaging="True" OnPageIndexChanging="grdDocumento_PageIndexChanging" >
            <PagerStyle HorizontalAlign="Center" />
            <RowStyle CssClass="" HorizontalAlign="Left"  />
            <AlternatingRowStyle CssClass="odd" />
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="true" HeaderText="Documento" ControlStyle-Width="100px" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:LinkButton id = "linkCodigo" runat="server" Font-Bold="true" CausesValidation="False" OnClientClick='RetornaCodigoDocumento(this)' 
                            CommandName="Select" Text='<%# Eval("TB_MOVIMENTO_NUMERO_DOCUMENTO")%>' value-gerdesc='<%# Eval("TB_MOVIMENTO_GERADOR_DESCRICAO")%>'
                            ToolTip='<%# Eval("TB_MOVIMENTO_ID") + ";;" + Eval("TB_ALMOXARIFADO_CODIGO_DESCRICAO") + ";;" + Eval("TB_ALMOXARIFADO_ID")%>' ></asp:LinkButton>
                        <asp:Label ID="lblId" runat="server" Text='<%# Bind("TB_MOVIMENTO_ID") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>                
                <asp:BoundField DataField="TB_MOVIMENTO_DATA_MOVIMENTO" HeaderText="Data Movimento" HeaderStyle-Width="80px" dataformatstring="{0:dd/MM/yyyy}" FooterStyle-Wrap="false"/>
                <asp:BoundField DataField="TB_MOVIMENTO_GERADOR_DESCRICAO" HeaderText="Descrição" HeaderStyle-Width="250px" FooterStyle-Wrap="false"/>
                <asp:BoundField DataField="TB_ALMOXARIFADO_DESCRICAO" HeaderText="Almoxarifado Origem" HeaderStyle-Width="250px" FooterStyle-Wrap="false"/>                
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceDocumentos" runat="server" SelectMethod="PesquisaDocumentos" 
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Web.Controles.PesquisaDocumentoNova"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="txtChave" Name="palavraChave" PropertyName="Text" Type="String" DefaultValue="" />
                            <asp:ControlParameter ControlID="lblTipoMovimento" Name="tipoMovimento" Type="Int32" DefaultValue="" />
                            <asp:ControlParameter ControlID="lbltipoOperacao" Name="tipoOperacao" Type="Int32" DefaultValue="" />
                        </SelectParameters>
        </asp:ObjectDataSource>
        <asp:Label runat="server" ID="lblTipoMovimento" /> 
        <asp:Label runat="server" ID="lbltipoOperacao" /> 
     </ContentTemplate>
 
</asp:UpdatePanel>
