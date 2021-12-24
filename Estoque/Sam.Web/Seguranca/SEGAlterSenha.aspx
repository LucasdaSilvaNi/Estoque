<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="SEGAlterSenha.aspx.cs" Inherits="Sam.Web.Seguranca.SEGAlterSenha" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register TagPrefix="anthem" Namespace="System.Windows.Forms" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>  
    <link href="../CSS/loading.css" rel="stylesheet" type="text/css" />

<script language="javascript" type="text/javascript">
    function mensagem() {
        if (window.confirm("Confirma a Alteração de Senha ?")) {
            window.alert("Após a alteração da senha. A Página será redirecionada para a página de Login.");            
        }
    }     
</script> 

    <div id="content">
        <h1>Módulo Segurança - Usuário - Alterar Senha</h1>

        <asp:UpdatePanel runat="server" ID="udpPanel">
        <ContentTemplate>            

        <div id="loader" class="loader" style="display:none;">
	        <img id="img-loader" src="../Imagens/loading.gif" alt="Loading"/>
	    </div>

        <div class="formulario" style="margin-bottom: 20px; margin-top: 20px; vertical-align:text-top">
        		
            <div ID="content2" align="left">
                <asp:UpdatePanel ID="upnUsuario" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <fieldset class="fieldset">
                            <p>
                                <asp:Label ID="Label19" runat="server" CssClass="labelFormulariolft" 
                                    EnableViewState="False" Font-Bold="True" Text="Identificação*:" 
                                    Width="170px"></asp:Label>
                                <asp:TextBox ID="txtUsuario" runat="server" MaxLength="11" Width="179px"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label20" runat="server" CssClass="labelFormulariolft" 
                                    EnableViewState="False" Font-Bold="True" Text="Senha Atual*:" 
                                    Width="170px"></asp:Label>
                                <asp:TextBox ID="txtSenhaAtual" runat="server" TextMode="Password" 
                                    Width="179px" ></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label21" runat="server" CssClass="labelFormulariolft" 
                                    EnableViewState="False" Font-Bold="True" Text="Nova Senha*:" Width="170px"></asp:Label>
                                <asp:TextBox ID="txtNovaSenha" runat="server" TextMode="Password" Width="179px"></asp:TextBox>
                            </p>
                            <p>
                                <asp:Label ID="Label22" runat="server" CssClass="labelFormulariolft" 
                                    EnableViewState="False" Font-Bold="True" Text="Confirme Nova Senha*:" 
                                    Width="170px"></asp:Label>
                                <asp:TextBox ID="txtConfirmaSenha" runat="server" TextMode="Password" 
                                    Width="179px"></asp:TextBox>
                                    <p> <asp:Label ID="Label23" runat="server" CssClass="labelFormulario" 
                                    EnableViewState="False" Font-Bold="True" Width="170px" Height="16px"></asp:Label>                                                                         
                                    <asp:Button ID="btnAlterarSenha" runat="server" CssClass="button" 
                    Font-Bold="True" Height="25px" onclick="btnAlterarSenha_Click" Text="Alterar" 
                    Width="169px" onclientclick="mensagem() " /><asp:Button ID="btnVoltar" CssClass="button" runat="server" 
                                Text="Voltar" AutoUpdateAfterCallBack="True" 
                                 Width="152px" Font-Bold="True" Height="25px" onclick="btnVoltar_Click"/></p>
                                

                                    <div class="btFinalPaginaInterna">
              
                                <p>
                                 <asp:Label ID="Label24" runat="server" CssClass="labelFormulariolft" 
                                        EnableViewState="False" Font-Bold="False" Width="490px">&#8226;	Os campos de senha devem ter de 8 à 20 caracteres;</asp:Label>
                                    <br />
                                                                         
                                    <asp:Label ID="Label25" runat="server" CssClass="labelFormulariolft" 
                                        EnableViewState="False" Font-Bold="False" Width="531px">&#8226;	 Letras maiúsculas e minúsculas fazem diferença;</asp:Label>
                                    <br />
                                    <asp:Label ID="Label26" runat="server" CssClass="labelFormulariolft" 
                                        EnableViewState="False" Font-Bold="False" Width="445px">&#8226;	Use somente letras e/ou  números;</asp:Label>
                                    </div>                             
                                    
                                
                        </fieldset>
                        
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" 
                            EnableViewState="false" />
                        <br />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>       

        </ContentTemplate>
        </asp:UpdatePanel>        
    </div>

    <div id="dialog" title="Mensagem">
        <asp:UpdatePanel ID="updMsgSenha" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblMsgSenha" runat="server" />
                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

</asp:Content>
