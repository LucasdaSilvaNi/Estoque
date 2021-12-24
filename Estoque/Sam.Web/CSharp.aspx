<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSharp.aspx.cs" Inherits="Sam.Web.CSharp" %>
<%@ Register Assembly="MSCaptcha" Namespace="MSCaptcha" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:CaptchaControl ID="Captcha1" runat="server" 
        CaptchaBackgroundNoise="Low" CaptchaLength="5" 
        CaptchaHeight="60" CaptchaWidth="200" 
        CaptchaLineNoise="None" CaptchaMinTimeout="5" 
        CaptchaMaxTimeout="240" FontColor = "#529E00" />
     
    </div>
        <asp:TextBox ID="txtCaptcha" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="btnVerify" runat="server" Text="Verify" OnClick="btnVerify_Click" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*Required" ControlToValidate = "txtCaptcha"></asp:RequiredFieldValidator>
        <br />
        <br />
        <asp:Label ID="lblMessage" runat="server" Font-Names = "Arial" Text=""></asp:Label>
    </form>
</body>
</html>
