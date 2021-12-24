<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="LogErro.aspx.cs" Inherits="Sam.Web.LogErro" Title="Log Erros do Sistema" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Log de Erros do Sistema</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <asp:GridView ID="gridLogErro"  runat="server" Width="100%" AllowPaging="True" 
                    OnPageIndexChanging="gridLogErro_PageIndexChanging" >
                    <Columns>
                        <asp:BoundField DataField="Id" ControlStyle-Width="5%" HeaderText="Id" SortExpression="Id" />
                        <asp:BoundField DataField="Message"  ControlStyle-Width="20%" HeaderText="Mensagem" 
                            SortExpression="Mensagem" />
                        <asp:BoundField DataField="StrackTrace"  ControlStyle-Width="60%" HeaderText="StrackTrace" 
                            SortExpression="StrackTrace" />
                        <asp:BoundField DataField="Data"  ControlStyle-Width="15%" HeaderText="Data" SortExpression="Data" />
                    </Columns>
                    <HeaderStyle CssClass="gridview"></HeaderStyle>
                </asp:GridView>
                <asp:ObjectDataSource ID="sourceDados" runat="server" EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName"
                    StartRowIndexParameterName="startRowIndexParameterName" SelectMethod="PopularListaLogErro"
                    SelectCountMethod="TotalRegistros" TypeName="Sam.Web.LogErro">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
