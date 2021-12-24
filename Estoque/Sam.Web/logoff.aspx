<%@ Page Language="C#" 
    AutoEventWireup="true" 
    CodeBehind="logoff.aspx.cs"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" 
    Title="Logoff usuário logado" 
    EnableViewState="true"
    ValidateRequest="false" 
    EnableEventValidation="false" 
    Inherits="Sam.Web.logoff" %>
    <%@ Register Src="Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
   
        <asp:UpdatePanel ID="updPendente" runat="server">
            <ContentTemplate>
                <div class="content">
                    <h1>
                     Logoff
                    </h1>
                     <fieldset class="fieldset">
                                <div id="Div1">
                                    <br />
                                    <p id="paragrafo3">
                                        <label id="labelSubItem" runat="server" class ="labelFormulario"><b>Usuário:</b></label>
                                        <asp:TextBox ID="txtUsuario" runat="server" MaxLength="11" CssClass="inputFromNumero"></asp:TextBox>
                                    </p>
                                   
                                    <p>
                                        <asp:Button ID="btnLogoff" runat="server" Text="Logoff" Width="200px" 
                                            onclick="btnLogoff_Click" />
                                    </p>
                                </div>
                            </fieldset>
                </div>           
                 <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="false" /> 
                <br />
                <br />
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>

   
</asp:Content>
