<%@ Page Title="Módulo Almoxarifado :: Reserva de Material" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master"  AutoEventWireup="true" CodeBehind="ReservaMaterial.aspx.cs" Inherits="Sam.Web.Almoxarifado.ReservaMaterial" 
 ValidateRequest="False" %>
<%@ Register src="../Controles/ListInconsistencias.ascx" tagname="ListInconsistencias" tagprefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitem.ascx" TagName="PesquisaSubitem" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>  
    <div id="content">
        <h1>
            Módulo Almoxarifado - Reserva de Material</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
        <ContentTemplate>    
          
        <asp:GridView ID="gridReservaMaterial" DataSourceID ="sourceGridReservaMaterial" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridReservaMaterial_SelectedIndexChanged">
            <Columns>
                <asp:TemplateField ShowHeader="False" Visible="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="Id" runat="server" Font-Bold="true" CausesValidation="False"
                            CommandName="Select" Text='<%# Bind("Id") %>'></asp:LinkButton>                            
                        <asp:Label runat="server" ID="lblItem" Text='<%# Bind("ItemMaterial.Codigo") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblSubItem" ToolTip='<%# Bind("SubItemMaterial.Descricao") %>' Text='<%# Bind("SubItemMaterial.Codigo") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblSubItemId" Text='<%# Bind("SubItemMaterial.Id") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblUge" ToolTip='<%# Bind("Uge.Descricao") %>' Text='<%# Bind("Uge.Id") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblQuantidade" Text='<%# Bind("Quantidade") %>' Visible="false"></asp:Label>
                        <asp:Label runat="server" ID="lblObs" Text='<%# Bind("Obs") %>' Visible="false"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Item" ShowHeader="False" Visible="True">
                    <ItemTemplate >
                        <asp:Label runat="server" ID="lblItemMaterial" ToolTip='<%# Bind("ItemMaterial.Descricao") %>' Text='<%# String.Format("{0:D9}", Eval("ItemMaterial.Codigo")) %>' Visible="True" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="SubItem" ShowHeader="False" Visible="True">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblSubItemMaterial" ToolTip='<%# Bind("SubItemMaterial.Descricao") %>' Text='<%# String.Format("{0:D12}",Eval("SubItemMaterial.Codigo")) %>' Visible="True" ItemStyle-HorizontalAlign="Right"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Uge" ShowHeader="False" Visible="True">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Uge" ToolTip='<%# Bind("Uge.Descricao") %>' Text='<%# String.Format("{0:D6}",Eval("Uge.Codigo")) %>' Visible="True" ItemStyle-HorizontalAlign="Right"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Unidade" ShowHeader="False" Visible="True" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblUnidade" Text='<%# Bind("UnidadeFornecimento.Descricao") %>' Visible="True"  ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="Quantidade" HeaderText="Quantidade" ItemStyle-HorizontalAlign="Right" />
                <asp:BoundField DataField="Obs" HeaderText="Observação" />
                <asp:TemplateField HeaderText="Editar" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif"
                            CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <HeaderStyle CssClass="corpo"></HeaderStyle>
        </asp:GridView>
        <asp:ObjectDataSource ID="sourceGridReservaMaterial" runat="server" EnablePaging="True" 
            MaximumRowsParameterName="maximumRowsParameterName" 
            OldValuesParameterFormatString="original_{0}" 
            SelectCountMethod="TotalRegistros" SelectMethod="PopularDadosGrid" 
            StartRowIndexParameterName="startRowIndexParameterName" 
            TypeName="Sam.Presenter.ReservaMaterialPresenter">
            <SelectParameters>
                <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
            </SelectParameters>            
        </asp:ObjectDataSource>
        <div id="DivButton" class="DivButton" >
            <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" 
                    OnClick="btnNovo_Click" Width="75px" />
            </p>
            <p class="botaoRight">
                <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click" Visible="false"
                    Text="Imprimir" AccessKey="I" />
                <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                    AccessKey="A" />
                <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                    AccessKey="V" />
            </p>
            <p>
            
            </p>
        </div>
        <asp:Panel runat="server" ID="pnlEditar">
            <asp:HiddenField ID="hidtxtItemMaterialId" runat="server" />
            <div id="interno2">
                <div>
                    <fieldset class="fieldset">
                        <p>
                            <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="120px" Text="SubItem*:"></asp:Label>
                        <asp:TextBox ID="txtSubItem" MaxLength="12" onblur="preencheZeros(this,'12')" onkeypress='return SomenteNumero(event)'
                            CssClass="inputFromNumero" runat="server" size="12" Enabled="false"></asp:TextBox>

                            <asp:ImageButton ID="imgLupaSubItem" runat="server" CommandName="Select" CssClass="basic"
                                ImageUrl="../Imagens/lupa.png"  OnClientClick="OpenModalItem();" ToolTip="Pesquisar" />                                    
                            <asp:HiddenField ID="idSubItem" runat="server" />
                            <asp:HiddenField ID="hfdMovimentoItemId" runat="server" />
                        </p>
                        <p>
                            <asp:Label ID="Label9" runat="server" class="labelFormulario" Width="120px" Text="UGE*:"></asp:Label>
                            <asp:DropDownList ID="ddlUGE" runat="server" 
                                Width="80%" AutoPostBack="True" DataTextField="Descricao" 
                                DataValueField="Id" onselectedindexchanged="ddlUGE_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="sourceListaUGE" runat="server" 
                                OldValuesParameterFormatString="original_{0}" 
                                SelectMethod="PopularListaUge" 
                                TypeName="Sam.Presenter.ReservaMaterialPresenter">
                            </asp:ObjectDataSource>
                        </p>

                        <p>
                            <asp:Label ID="Label10" runat="server" class="labelFormulario" Width="120px" Text="Quantidade*:"></asp:Label>
                            <asp:TextBox ID="txtQuantidade" MaxLength="5" CssClass="inputFromNumero" 
                                runat="server" size="5" Width="132px" />
                        </p>
                        <p>
                            <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="120px" 
                                Text="Obs:" />
                            <asp:TextBox ID="txtObs" runat="server" MaxLength="256"  
                                onkeyup='limitarTamanhoTexto(this,256)' onkeydown='limitarTamanhoTexto(this,256)' 
                                TextMode="MultiLine" Width="80%" Height="36px" SkinID="MultiLine"></asp:TextBox>
                        </p>
                    </fieldset>
                </div>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                <p>
                    <small>Os campos marcados com (*) são obrigatórios. </small>
                </p>
            </div>
            <!-- fim id interno -->
            <div class="botao">
                <!-- simula clique link editar/excluir -->
                <div class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnGravar" CssClass="" runat="server" Text="Salvar" OnClick="btnGravar_Click" />
                        <asp:Button ID="btnExcluir" AccessKey="E" CssClass="" runat="server" Text="Excluir"
                            OnClick="btnExcluir_Click" />
                        <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar"
                            OnClick="btnCancelar_Click" />
                    </p>
                </div>
            </div>
        </asp:Panel>
        <br />
        </ContentTemplate>
        </asp:UpdatePanel>        
            <div id="dialogItem" title="Pesquisar SubItem Material">
            <uc2:PesquisaSubitem ID="PesquisaSubitem1" runat="server" />
            </div> 
        </div>    
    </div>
</asp:Content>
