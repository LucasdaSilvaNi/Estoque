<%@ Control Language="C#" CodeBehind="Ajuda.ascx.cs" Inherits="Sam.Web.Controles.Ajuda"%>
<div id="divAjuda">
    <%--<img src="../Imagens/Help-icon.png" style="margin-left:90%; position:absolute" width="45px">--%>
    <h5><asp:Label runat="server" ID="lblTituloAjuda" Text=""></asp:Label></h5>
    <br />
    <div runat="server" id="divCorpo" style="width:100%;height:400px;overflow:auto;"></div>    
    <p>
    <asp:Label runat="server" ID="lblCorpo1" Text=""></asp:Label>
    </p>
    <br />
    <p>
    <asp:Label runat="server" ID="lblCorpo2" Text=""></asp:Label>
    </p>
</div>
            
