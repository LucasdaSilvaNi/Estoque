<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Sam.Web.UC.Menu" %>

<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>
<p>
    &nbsp;</p>

<p>
&nbsp;&nbsp;&nbsp;
</p>
<asp:Menu ID="Menu2" runat="server" Style="width: 740px;" Orientation="Horizontal" BackColor="#A2ABB0" 
Font-Names="Tahoma" Font-Size="8pt" ForeColor="White">

    <Items>
        <asp:MenuItem Text="New Item" Value="New Item">
            <asp:MenuItem Text="New Item" Value="New Item">
                <asp:MenuItem Text="New Item" Value="New Item">
                    <asp:MenuItem Text="New Item" Value="New Item"></asp:MenuItem>
                </asp:MenuItem>
            </asp:MenuItem>
        </asp:MenuItem>
        <asp:MenuItem Text="New Item" Value="New Item"></asp:MenuItem>
        <asp:MenuItem Text="New Item" Value="New Item"></asp:MenuItem>
        <asp:MenuItem Text="New Item" Value="New Item"></asp:MenuItem>
        <asp:MenuItem Text="New Item" Value="New Item">
            <asp:MenuItem Text="New Item" Value="New Item">
                <asp:MenuItem Text="New Item" Value="New Item"></asp:MenuItem>
            </asp:MenuItem>
        </asp:MenuItem>
    </Items>

<StaticHoverStyle CssClass="SubMenuItemSelected" />

<DynamicMenuStyle BackColor="#A2ABB0" />

<DynamicMenuItemStyle CssClass="SubMenuItem" HorizontalPadding="14px" VerticalPadding="2px" />

<DynamicSelectedStyle CssClass="SubMenuItemSelected" ForeColor="#996600" Font-Bold="True"/>

<DynamicHoverStyle CssClass="SubMenuItemSelected" />

</asp:Menu>
