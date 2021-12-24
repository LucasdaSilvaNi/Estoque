<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="consultaSubItemMaterial.aspx.cs" Inherits="Sam.Web.consultaSubItemMaterial"
    Title="Módulo Tabelas :: Catálogo :: Consulta Subitem Material" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaItem.ascx" TagName="ListItemMaterial" TagPrefix="uc3" %>
<%@ Register Src="../Controles/PesquisaItemNova.ascx" TagName="PesquisaItemNova" TagPrefix="uc3" %>
<%@ Register Src="~/Controles/PesquisaSubitemNova.ascx" TagName="PesquisaSubitemNova" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <div id="content">
        <h1>
            Módulo Tabelas - Catálogo - Subitens de Material</h1>
        <asp:UpdatePanel runat="server" ID="udpPanel">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <p>
                        <asp:Label ID="lblitem" runat="server" CssClass="labelFormulario" Width="140px" Text="Item:" />
                        <asp:TextBox ID="txtItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                            CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>
                        <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic" ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalItem();" ToolTip="Pesquisar" />
                        <asp:HiddenField ID="itemMaterialId" runat="server" />

                        <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                    </p>

                    <p>                           
                        <asp:Label ID="lblSubItem" runat="server" CssClass="labelFormulario" Width="140px" Text="Subitem:" Font-Bold="true" />                        
                        <asp:TextBox ID="txtSubItem" runat="server" MaxLength="12" size="12" onblur="return preencheZeros(this,'12')" 
                            CssClass="inputFromNumero" onkeypress="return SomenteNumero(event)"  />
                        <asp:ImageButton ID="imgSubItemMaterial" ImageUrl="../Imagens/lupa.png" OnClientClick="OpenModalSubItem();" CssClass="basic" ToolTip="Pesquisar" runat="server" />
                        <asp:HiddenField ID="idSubItem" runat="server" />
                    </p>
                    <p>
                        <asp:Label ID="lblNaturezaDespesa" runat="server" CssClass="labelFormulario" Font-Bold="true" Text="Natureza de Despesa:" Width="140px" />
                        <asp:DropDownList ID="ddlNatureza" runat="server" AutoPostBack="True" DataTextField="Descricao" DataValueField="Id"  
                            OnSelectedIndexChanged="ddlNatureza_SelectedIndexChanged" Width="50%" />
                        <asp:Button ID="btnPesquisar" runat="server" OnClick="btnPesquisar_Click" Text="Pesquisar" />
                        &nbsp
                        <asp:Button ID="btnLimpar" runat="server" Text="Limpar Campos"  CssClass="" runat="server" Style="{width: 130px; }" onclick="btnLimpar_Click" OnClientClick="limparCampos();" />
                        <%--onchange="limparCampos();"--%>
                    </p>                    
                </fieldset>


                <div style="margin-bottom: 20px; margin-top: 20px;">
                    <asp:GridView ID="gridSubItemMaterial" runat="server" HeaderStyle-CssClass="ProdespGridViewHeaderStyleClass"
                        DataKeyNames="Id" AutoGenerateColumns="False" AllowPaging="False" CssClass="tabela">
                        <RowStyle CssClass="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:TemplateField ShowHeader="False" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                        CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" ApplyFormatInEditMode="False"
                                DataFormatString="{0:D12}" />
                            <asp:BoundField DataField="Descricao" HeaderText="Descrição" ItemStyle-HorizontalAlign="Left"
                                HeaderStyle-HorizontalAlign="Left" />
                            <asp:TemplateField ShowHeader="True" HeaderText="Codigo Item">
                                <ItemTemplate>
                                    <asp:Label ID="lblItemMaterial" runat="server" Text='<%# ( String.Format( ((Sam.Domain.Entity.SubItemMaterialEntity)Container.DataItem).ItemMaterial.CodigoFormatado,"{0:D9}") ) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" HeaderText="Unidade Fornecimento">
                                <ItemTemplate>
                                    <asp:Label ID="lblUnidForn" runat="server" Text='<%# (((Sam.Domain.Entity.SubItemMaterialEntity)Container.DataItem).UnidadeFornecimento.CodigoDescricao) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" HeaderText="Nat.Despesa">
                                <ItemTemplate>
                                    <asp:Label ID="lblNatureza" runat="server" Text='<%# (((Sam.Domain.Entity.SubItemMaterialEntity)Container.DataItem).NaturezaDespesa.Codigo) %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="True" HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblAtivo" runat="server" Text='<%# ((((Sam.Domain.Entity.SubItemMaterialEntity)Container.DataItem).IndicadorAtividade) ?  "Ativo" : "Inativo") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo"></HeaderStyle>
                    </asp:GridView>
                </div>

                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="true" />

                <div id="DivBotoes" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Style="{width: 150px; }" Text="Cadastro Subitem"
                            AccessKey="N" PostBackUrl="~/Tabelas/cadastroSubItemMaterial.aspx" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click" Text="Imprimir" AccessKey="I" Visible="false" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass="" AccessKey="A" />
                        <asp:Button ID="btnvoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx" AccessKey="V" />
                    </p>
                </div>
                <!-- fim id interno -->
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>

        <div id="dialogItem" title="Pesquisar Item Material">
            <uc3:pesquisaitemnova ID="Pesquisaitem" runat="server" />            
        </div>

        <div id="dialogSubItem" title="Pesquisar Subitem">
            <uc3:PesquisaSubitemNova ID="PesquisaSubItem" runat="server" />
        </div>

    </div>
    <script type="text/javascript" language="javascript">

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
            if (args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerTimeoutException'
            || args.get_error() && args.get_error().name === 'Sys.WebForms.PageRequestManagerServerErrorException') {
                args.set_errorHandled(true);
            } 
        });

        function limparCampos() {
            $("#ctl00_cphBody_itemMaterialId").attr("value", "");
            $("#ctl00_cphBody_idSubItem").attr("value", "");
        }

    </script>
</asp:Content>
