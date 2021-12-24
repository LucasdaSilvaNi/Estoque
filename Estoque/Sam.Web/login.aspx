<%@ Page Title="SAM - Sistema de Administração de Materiais" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"
    AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Sam.Web.login" %>

<%@ Register Src="Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript">
        var _btnAcessar = "<%:btnAcessar.ClientID%>";

        function desabilitaDuploEnter(habilitar)
        {
            $("#"+_btnAcessar).attr('disabled', habilitar);
        }
    </script>
    <div id="content">
        <asp:UpdatePanel ID="upnUsuario" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <p>
                        <asp:Label ID="Label1" runat="server" Text="Identificação*:" EnableViewState="False"
                            CssClass="labelFormulario" Width="100px" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="11" CssClass="inputFromNumero"></asp:TextBox>
                    </p>
                    <p>
                        <asp:Label ID="Label2" runat="server" Text="Senha*:" EnableViewState="False" CssClass="labelFormulario"
                            Width="100px" Font-Bold="True"></asp:Label>
                        <asp:TextBox ID="txtSenha" runat="server" TextMode="Password" onkeydown=""></asp:TextBox>
                    </p>
                    <p>
                        <asp:Label ID="Label4" runat="server" Text="Trocar Perfil:" EnableViewState="False"
                            CssClass="labelFormulario" Width="100px" Font-Bold="True"></asp:Label>
                        <asp:RadioButtonList ID="rdoPerfil" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Selected="False" Text="Sim" Value="1"></asp:ListItem>
                            <asp:ListItem Selected="True" Text="Não" Value="0"></asp:ListItem>
                        </asp:RadioButtonList>
                    </p>
                    <p>
                        <asp:Label ID="Label3" runat="server" EnableViewState="False" CssClass="labelFormulario"
                            Width="100px" Text="&nbsp;" Font-Bold="True"></asp:Label>
                        <asp:Button ID="btnAcessar" runat="server" OnClick="btnAcessar_Click" Text="Acessar"
                            CssClass="button" OnClientClick="javascript:desabilitaDuploEnter(true);" UseSubmitBehavior="False" />
                    </p>
                    <p>
                    </p>
                </fieldset>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="false" />
                <br />
                <br />
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
