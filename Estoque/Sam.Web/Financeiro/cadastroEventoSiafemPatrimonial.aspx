<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroEventoSiafemPatrimonial.aspx.cs" Inherits="Sam.Web.Financeiro.cadastroEventoSiafemPatrimonial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">


    <table>
        <tr>
            <td>Tipo de NL</td>
            <td>

                <asp:DropDownList ID="ddlTipoNl" runat="server">
                    <asp:ListItem Value="Selecione" Text="Selecione"></asp:ListItem>
                    <asp:ListItem Value="ENTRADA" Text="Entrada"></asp:ListItem>
                    <asp:ListItem Value="SAIDA" Text="Saida"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoNl" ValidationGroup="salvar" InitialValue="Selecione" ControlToValidate="ddlTipoNl" ErrorMessage="Selecione Tipo de NL" />
            </td>


        </tr>

        <tr>
            <td>Tipo de Movimento (SAM)</td>
            <td>
                <asp:DropDownList ID="ddlTipoMovimento" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoMovimento" ValidationGroup="salvar" InitialValue="0" ControlToValidate="ddlTipoMovimento" ErrorMessage="Selecione Tipo de Movimento" />
            </td>
        </tr>

        <tr>
            <td>Tipo de Movimento (Siafem Net) </td>
            <td>
                <asp:TextBox ID="txtTipoEntrada" runat="server" Width="700px"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoEntrada" ValidationGroup="salvar" ControlToValidate="txtTipoEntrada" ErrorMessage="Preencher Tipo de Movimento (Siafem Net)" />
            </td>
        </tr>


        <%-- <tr>
            <td>SUBTIPO MOVIMENTO</td>
            <td>
                <asp:DropDownList ID="lldSubTipoMovimento" runat="server">
                </asp:DropDownList></td>
            <td>
                <asp:RequiredFieldValidator runat="server" InitialValue="0" ValidationGroup="salvar" ID="reqSubTipoMovimento" ControlToValidate="lldSubTipoMovimento" ErrorMessage="Selecione SUBTIPO MOVIMENTO" />
            </td>
        </tr>--%>
        <tr>
            <td>Tipo de Estoque</td>
            <td>
                <asp:DropDownList ID="ddlTipoEstoque" runat="server">
                    <asp:ListItem Value="Selecione" Text="Selecione"></asp:ListItem>
                    <asp:ListItem Value="CONSUMO" Text="Material Consumo"></asp:ListItem>
                    <asp:ListItem Value="PERMANENTE" Text="Material Permanente"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ValidationGroup="salvar" InitialValue="Selecione" ID="reqTipoMaterial" ControlToValidate="ddlTipoEstoque" ErrorMessage="Selecione Tipo de Estoque" />
            </td>
        </tr>

        <%--<tr>
            <td>EVENTO TIPO ESTOQUE</td>
            <td>
                <asp:TextBox ID="txtTipoEstoque" runat="server" MaxLength="10"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoEstoque" ControlToValidate="txtTipoEstoque" ErrorMessage="Preencher EVENTO TIPO ESTOQUE" />
            </td>
        </tr>--%>
        <%--<tr>
            <td>EVENTO ESTOQUE</td>
            <td>
                <asp:TextBox ID="txtEvento" runat="server" MaxLength="20"></asp:TextBox></td>
        </tr>--%>
        <tr>
            <td>Tipo de Movimentação</td>
            <td>
                <asp:TextBox ID="txtTipoMovimentacao" runat="server" Width="500px"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoMovimentacao" ValidationGroup="salvar" ControlToValidate="txtTipoMovimentacao" ErrorMessage="Preencher Tipo de Movimentação" />
            </td>
        </tr>
          <tr>
            <td>Gerar estimulo</td>
            <td>
                <asp:CheckBox ID="ckbEstimulo" runat="server" />
            </td>
        </tr>
        <%--  <tr>
            <td>EVENTO TIPO MOVIMENTO</td>
            <td>
                <asp:TextBox ID="txtTipoMovimento" runat="server" MaxLength="15"></asp:TextBox>
            </td>
            <td>
                <asp:RequiredFieldValidator runat="server" ID="reqTipoMovimento" ValidationGroup="salvar" ControlToValidate="txtTipoMovimento" ErrorMessage="Preencher EVENTO TIPO MOVIMENTO" />

            </td>
        </tr>--%>
        <tr style="visibility: hidden">
            <td>Detalhe</td>
            <td>
                <asp:CheckBox ID="ckbDetalhe" runat="server" />
            </td>
        </tr>
       
        <tr>
            <td>
                <asp:Button ID="btnGravar" runat="server" Text="Gravar" ValidationGroup="salvar" OnClick="btnGravar_Click" /></td>

        </tr>

    </table>

    <asp:GridView ID="gdvEvento" runat="server" OnSorting="gdvEvento_Sorting" AllowSorting="true" OnRowCommand="gdvEvento_RowCommand" OnRowDeleting="gdvEvento_RowDeleting" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="gdvEvento_PageIndexChanging"
        EnableModelValidation="True" OnRowEditing="gdvEvento_RowEditing" OnRowCancelingEdit="gdvEvento_RowCancelingEdit">
        <%--<Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de NL" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                    <asp:Label runat="server" ID="Label144" Text='<%# Bind("Id") %>' Visible="true" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="Label1444" Text='<%# Bind("Id") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>


        </Columns>--%>

        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de NL" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblTipoMovi" Text='<%# Bind("EventoTipoMovimento") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de Estoque" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblTipoEst" Text='<%# Bind("EventoTipoEstoque") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de Movimento (SAM)" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:HiddenField runat="server" ID="hdfTipoMov" Value='<%# Bind("SubTipoMovimento.TipoMovimento.Id") %>' />
                    <asp:Label runat="server" ID="lblTipoMov" Text='<%# Bind("SubTipoMovimento.TipoMovimento.CodigoFormatado") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>


        <%-- <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de Movimento (Siafem Net)" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblSubTipoMov" Text='<%# Bind("SubTipoMovimento.Descricao") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>--%>

        <%--<Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Evento Tipo Material" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEventotipo" Text='<%# Bind("EventoTipoMaterial") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>--%>
        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de Movimento (Siafem Net)" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                    <asp:TextBox runat="server" ID="txtEventoEntr" CssClass="lineAlterar" TextMode="MultiLine"  Columns="80" Height="50px" Rows="50" Text='<%# Bind("EventoTipoEntradaSaidaReclassificacaoDepreciacao") %>' Visible="true" />
                    <asp:HiddenField  runat="server" ID="hdfEventoEntr" Value='<%# Bind("EventoTipoEntradaSaidaReclassificacaoDepreciacao") %>'  />
                    <asp:HiddenField  runat="server" ID="hdfSubTipo" Value='<%# Bind("SubTipoMovimento.Id") %>'  />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEventoEntr" Text='<%# Bind("EventoTipoEntradaSaidaReclassificacaoDepreciacao") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo de Movimentação" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                    <asp:TextBox runat="server" ID="txtTipoMoviment" CssClass="lineAlterar" TextMode="MultiLine" Columns="50" Height="50px" Rows="50" Text='<%# Bind("EventoTipoMovimentacao") %>' Visible="true" />
                <asp:HiddenField runat="server" ID="hdfTipoMoviment" Value='<%# Bind("EventoTipoMovimentacao") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblTipoMoviment" Text='<%# Bind("EventoTipoMovimentacao") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>


        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Estoque" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblEstoque" Text='<%# Bind("EventoEstoque") %>' Visible="true" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

          <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Gerar Estímulo"  ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                     <asp:CheckBox ID="ckbEstimuloT" runat="server" Enabled="true" Checked='<%# Bind("EstimuloAtivo") %>' />
                    <asp:HiddenField runat="server" ID="hdfEstimuloT" Value='<%# Bind("EstimuloAtivo") %>' />
                    </EditItemTemplate>
                <ItemTemplate>
                     <asp:CheckBox ID="ckbEstimulo" runat="server" Enabled="false" Checked='<%# Bind("EstimuloAtivo") %>' />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Alterar" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                    <asp:ImageButton ID="imgSalvar" runat="server" Font-Bold="true" Width="20px" Height="20px" ImageUrl="~/Imagens/Apply.gif" CausesValidation="False" CommandName="Alterar" CommandArgument='<%# Bind("Id") %>' />
                    <asp:ImageButton ID="imgCancelar" runat="server" Font-Bold="true" Width="18px" Height="18px" ImageUrl="~/Imagens/delete.png" CausesValidation="False" CommandName="Cancel" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="imgAlterar" runat="server" Font-Bold="true" Width="25px" Height="25px" ImageUrl="~/Imagens/text-edit-icon.png" CausesValidation="False" CommandName="Edit" />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>


        <Columns>
            <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Deletar" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" Width="30px" Height="30px" ImageUrl="~/Imagens/lixeira.png" CausesValidation="False" CommandName="Delete" OnClientClick="return confirm('Tem certeza que deseja Deletar?');" CommandArgument='<%# Bind("Id") %>' />
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

    </asp:GridView>



</asp:Content>
