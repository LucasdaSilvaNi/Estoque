<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Calendario.ascx.cs" Inherits="Sam.Web.Controles.Calendario" %>

<asp:Calendar ID="calDate" runat="server"
    BackColor="White" BorderColor="#3366CC" CellPadding="1" DayNameFormat="Shortest"
    Font-Names="Verdana" Font-Size="8pt" ForeColor="#003399" Height="200px"
    Width="300px" OnDayRender="calDate_DayRender" ShowDayHeader="True" TitleFormat="MonthYear" OnSelectionChanged="calDate_SelectionChanged">
    <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
    <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
    <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
    <WeekendDayStyle BackColor="#CCCCFF" />
    <OtherMonthDayStyle ForeColor="#999999" />
    <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
    <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
    <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" Font-Bold="True"
        Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
</asp:Calendar>

<asp:DropDownList ID="Ddyear" runat="server" AutoPostBack="True"
    OnSelectedIndexChanged="DdyearSelectedIndexChanged">
</asp:DropDownList>

<asp:DropDownList ID="Ddmonth" runat="server" AutoPostBack="True"
    OnSelectedIndexChanged="DdmonthSelectedIndexChanged">
    <asp:ListItem Value="01">Jan</asp:ListItem>
    <asp:ListItem Value="02">Fev</asp:ListItem>
    <asp:ListItem Value="03">Mar</asp:ListItem>
    <asp:ListItem Value="04">Abr</asp:ListItem>
    <asp:ListItem Value="05">Mai</asp:ListItem>
    <asp:ListItem Value="06">Jun</asp:ListItem>
    <asp:ListItem Value="07">Jul</asp:ListItem>
    <asp:ListItem Value="08">Ago</asp:ListItem>
    <asp:ListItem Value="09">Set</asp:ListItem>
    <asp:ListItem Value="10">Out</asp:ListItem>
    <asp:ListItem Value="11">Nov</asp:ListItem>
    <asp:ListItem Value="12">Dez</asp:ListItem>
</asp:DropDownList>

