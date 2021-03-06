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

<%@ Control Language="C#" CodeBehind="PesquisaItemNova.ascx.cs" Inherits="Sam.Web.Controles.PesquisaItemNova" %>
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
                    <asp:Label ID="Label2" runat="server" CssClass="labelModel" Width="100px" Text="Palavra-chave: " />
                    <asp:TextBox ID="txtChave" runat="server" MaxLength="255" Width="264px" />
                    <asp:Button ID="btnProcurar" runat="server" Text="Procurar" OnClick="btnProcurar_Click" />
                </p>
            </div>
            <br />
            <asp:GridView ID="gridItemMaterial" runat="server" DataKeyNames="TB_ITEM_MATERIAL_ID" AutoGenerateColumns="False" SkinID="GridModal"
                OnSelectedIndexChanged="gridItemMaterial_SelectedIndexChanged" CssClass="tabela" onpageindexchanging="gridItemMaterial_PageIndexChanging">
                <PagerStyle HorizontalAlign="Center" />
                <RowStyle CssClass="" HorizontalAlign="Left" />
                <AlternatingRowStyle CssClass="odd" />
                <Columns>
                    <asp:TemplateField ShowHeader="False" Visible="true" HeaderText="Código">
                        <ItemTemplate>
                            <asp:LinkButton id = "linkCodigo" runat="server" Font-Bold="true" CausesValidation="False" OnClientClick='RetornaCodigoItem(this)'
                            CommandName="Select" Text='<%# String.Format("{0:D9}", Eval("TB_ITEM_MATERIAL_CODIGO"))%>' ToolTip='<%# String.Format("{0:D9}", Eval("TB_ITEM_MATERIAL_ID")) + ";;" + Eval("TB_ITEM_MATERIAL_DESCRICAO") %>'></asp:LinkButton>
                            <asp:Label ID="lblId" runat="server" Text='<%# Bind("TB_ITEM_MATERIAL_ID") %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>                
                    <asp:BoundField DataField="TB_ITEM_MATERIAL_DESCRICAO" HeaderText="Descrição" />
                </Columns>
                <HeaderStyle CssClass="corpo"></HeaderStyle>
            </asp:GridView>
              <asp:ObjectDataSource ID="sourceGridItemMaterial" runat="server" SelectMethod="BuscarSubItemMaterial"
                        EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName" SelectCountMethod="TotalRegistros"
                        StartRowIndexParameterName="startRowIndexParameterName" TypeName="Sam.Web.Controles.PesquisaItemNova"
                        OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                            <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                            <asp:ControlParameter ControlID="txtChave" Name="palavraChave" PropertyName="Text" Type="String" DefaultValue="" />
                        </SelectParameters>
        </asp:ObjectDataSource>
         </ContentTemplate>
    </asp:UpdatePanel>
