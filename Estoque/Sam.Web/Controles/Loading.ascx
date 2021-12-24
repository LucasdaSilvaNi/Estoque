<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Loading.ascx.cs" Inherits="Sam.Web.Controles.Loading" %>
    <link href="../CSS/loading.css" rel="stylesheet" type="text/css" />
        <asp:UpdateProgress ID="prgLoading" runat="server" DynamicLayout="true" DisplayAfter="0" AssociatedUpdatePanelID="udpPanel">
            <ProgressTemplate>
                <div class="updateModal">
                    <div style="text-align:center" class="updateModalBox">
                        <img src="../Imagens/loading.gif" />
                    </div>
                </div>            
            </ProgressTemplate>
        </asp:UpdateProgress>
