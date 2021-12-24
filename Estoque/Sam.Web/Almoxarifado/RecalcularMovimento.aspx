<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: Divisão" Language="C#"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="RecalcularMovimento.aspx.cs"
    Inherits="Sam.Web.Almoxarifado.RecalcularMovimento" EnableViewState="true"
    ValidateRequest="false" EnableEventValidation="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitem.ascx" TagName="PesquisaSubitem" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">    
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <asp:UpdatePanel runat="server" ID="ajax1">
        <ContentTemplate>
            <div id="content">
                <h1>
                    Recalcular os saldos dos movimentos</h1>

                      <div class="Divbotao">
                <!-- simula clique link editar/excluir -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button runat="server" SkinID="Btn140" ID="btnProcessar"  Text="Recalcular Saldo" OnClick="btnProcessar_Click" />
                    </p>
                </div>
            </div>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />                
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <script type="text/javascript">
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException' 
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                args.set_errorHandled(true);
            }
        }); 
    </script>
</asp:Content>
