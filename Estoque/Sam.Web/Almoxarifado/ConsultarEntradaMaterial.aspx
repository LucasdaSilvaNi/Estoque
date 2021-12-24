<%@ Page Title="Módulo Almoxarifado :: Consultar Entrada Material" Language="C#"
    MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="ConsultarEntradaMaterial.aspx.cs"
    Inherits="Sam.Web.Almoxarifado.ConsultarEntradaMaterial" EnableViewState="true"
    ValidateRequest="false" EnableEventValidation="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias"
    TagPrefix="uc1" %>
<%@ Register Src="../Controles/PesquisaSubitem.ascx" TagName="PesquisaSubitem" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">    
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <asp:UpdatePanel runat="server" ID="ajax1">
        <ContentTemplate>
            <div id="content">
                <h1>
                    Módulo Almoxarifado - Consultar Entrada de Material</h1>
                <fieldset class="fieldset">
                    <div id="Div3">
                        <p>
                        <table width="100%">
                        <tr>
                            <td width="20%">
                                <asp:Label ID="Label32" runat="server" CssClass="labelFormulario" Width="120px" Text="Documento:"
                                Font-Bold="true" />
                                <asp:TextBox runat="server" ID="txtDocumento" onkeypress='return SomenteNumeroDecimal(event)' MaxLength="12" ></asp:TextBox>                            
                            </td>
                            <td width="80%">
                                <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="120px" Text="Empenho:"
                                Font-Bold="true" />
                                <asp:TextBox runat="server" ID="txtEmpenho"  onkeypress='return SomenteNumeroDecimal(event)' MaxLength="11"></asp:TextBox>                            
                            </td>
                        </tr>                        
                        </table>                            
                        </p>
                        <p class="esconderControle">
                            <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="120px" Text="Uge:"
                                Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlUge" Width="80%" AutoPostBack="True" DataTextField="Descricao" DataValueField="Id"></asp:DropDownList>                            
                            <asp:ObjectDataSource ID="odsUge" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosUge" TypeName="Sam.Presenter.UgePresenter">
                                <SelectParameters>
                                    <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                                    <asp:Parameter Name="maximumRowsParameterName" Type="Int32" DefaultValue="" />
                                    <asp:Parameter DefaultValue="18" Name="_orgaoId" Type="Int32" />                                    
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                        <p>
                            <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="120px" Text="Tipo:"
                                Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlTipo" Width="80%" AutoPostBack="True"
                                DataTextField="Descricao" DataValueField="Id" >
                            </asp:DropDownList>                            
                        </p>
                        <p>
                        <table>
                            <tr style="display:none">
                                <td>
                                    <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="120px" Text="Data Emissão:" Font-Bold="true" />
                                    <asp:TextBox runat="server" ID="txtDataEmissao" CssClass="dataFormat" MaxLength="10"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="Label7" runat="server" CssClass="labelFormulario" Width="120px" Text="Até:" Font-Bold="true" />
                                    <asp:TextBox runat="server" ID="txtDataEmissaoFinal" CssClass="dataFormat" MaxLength="10"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        </p>                        
                        <p>
                        <table>
                            <tr style="display:none">
                                <td>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="120px" Text="Data Receb.:" Font-Bold="true" />
                                    <asp:TextBox runat="server" ID="txtDataRecebimentoInicial" CssClass="dataFormat" MaxLength="10"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="120px" Text="Até:" Font-Bold="true" />
                                    <asp:TextBox runat="server" ID="txtDataRecebimentoFinal" CssClass="dataFormat" MaxLength="10"></asp:TextBox>
                                </td>                                
                            </tr>
                        </table>                                                        
                        </p>

                        <p>
                            <asp:Label ID="Label8" runat="server" CssClass="labelFormulario" Width="120px" Text="Estornados:" Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlEstornados"  Width="165px">                                    
                                <asp:ListItem Selected="True"  Text="Não" Value="0">
                                </asp:ListItem>
                                <asp:ListItem Selected="False" Text="Sim" Value="1">
                                </asp:ListItem>
                            </asp:DropDownList>
                        </p>
                        <p>
                        <table width="100%">
                            <tr>
                                <td width="80%">
                            </td>                                
                                <td width="20%">
                                    <asp:Button ID="btnPesquisar" CssClass="" SkinID="Btn120" runat="server" Text="Pesquisar" AccessKey="P" Width="200px" OnClick="btnPesquisar_Click"/>
                                </td>
                       </table>
                        </p>
                    </div>
                </fieldset>                
                <asp:Panel runat="server" ID="pnlEntrada">
                    <asp:GridView ID="grdDocumentos"  SkinID="GridNovo" runat="server" AllowPaging="True" AutoGenerateColumns="False" OnRowCreated="grdDocumentos_RowCreated"
                        CssClass="tabela" DataKeyNames="TB_MOVIMENTO_ID" OnRowCommand="grdDocumentos_RowCommand" OnRowDataBound="grdDocumentos_RowDataBound">
                        <PagerStyle HorizontalAlign="Center" />
                        <RowStyle CssClass="" HorizontalAlign="Left" />
                        <AlternatingRowStyle CssClass="odd" />
                        <Columns>
                            <asp:BoundField DataField="TB_MOVIMENTO_DATA_MOVIMENTO" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                HeaderText="Data Doc.">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MOVIMENTO_NUMERO_DOCUMENTO" HeaderStyle-Wrap="false" HeaderStyle-Width="8%"
                                HeaderText="Nº Doc.">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MOVIMENTO_EMPENHO" HeaderStyle-Wrap="false" HeaderStyle-Width="8%"
                                HeaderText="Nº Empenho">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>   
                            <asp:BoundField DataField="TB_MOVIMENTO_GERADOR_DESCRICAO" HeaderStyle-Wrap="false" HeaderStyle-Width="15%"
                                HeaderText="Gerador Descrição">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_USUARIO_NOME" HeaderStyle-Wrap="false" HeaderStyle-Width="12%"
                                HeaderText="Usuário">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MOVIMENTO_DATA_OPERACAO" DataFormatString="{0:dd/MM/yyyy hh:MM:ss}" HeaderStyle-Wrap="false" HeaderStyle-Width="12%"
                                HeaderText="Data Criação">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_USUARIO_NOME_ESTORNO" HeaderStyle-Wrap="false" HeaderStyle-Width="12%"
                                HeaderText="Usuário Estorno">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:BoundField DataField="TB_MOVIMENTO_DATA_ESTORNO" DataFormatString="{0:dd/MM/yyyy hh:MM:ss}" HeaderStyle-Wrap="false" HeaderStyle-Width="12%"
                                HeaderText="Data Estorno">
                                <FooterStyle Wrap="False" />
                                <HeaderStyle />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="Fornecedor">
                            <ItemTemplate>
                                <%#DataBinder.Eval(Container.DataItem, "TB_FORNECEDOR.TB_FORNECEDOR_NOME")%>
                            </ItemTemplate>
                            <HeaderStyle Width="20%" />
                            </asp:TemplateField>                            
                            <asp:TemplateField HeaderText="Valor">
                            <ItemTemplate>                            
                                <asp:Label ID="lblValorGrid" runat="server" Text='<%# String.Format("{0:0.00}", DataBinder.Eval(Container.DataItem, "TB_MOVIMENTO_VALOR_DOCUMENTO"))%>' ></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="9%" />
                            </asp:TemplateField>                  
                            <asp:TemplateField HeaderStyle-Width="3%" HeaderText="Nota" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                <asp:ImageButton runat="server" id="imgImprimirNota" Width="25px" ToolTip="Imprimir nota de Fornecimento" CausesValidation="false" CommandName="Imprimir" ImageUrl="~/Imagens/pdf_icon.png" CommandArgument='<%# Bind("TB_MOVIMENTO_ID") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="3%" HeaderText="Editar" ItemStyle-HorizontalAlign="Center" Visible="false">
                                <ItemTemplate>
                                <asp:ImageButton runat="server" id="imgEditar" Width="25px" ToolTip="Editar Documento" CausesValidation="false" CommandName="Editar" ImageUrl="~/Imagens/Text-Edit-icon.png" CommandArgument='<%# Bind("TB_MOVIMENTO_ID") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderStyle-Width="3%" HeaderText="Estornar" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                <asp:ImageButton runat="server" id="imgEstornar" Width="25px" ToolTip="Estornar Documento" CausesValidation="false" CommandName="Estornar" ImageUrl="~/Imagens/Delete2.png" CommandArgument='<%# Bind("TB_MOVIMENTO_ID") %>'/>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle CssClass="corpo" />
                    </asp:GridView>
                                    
                </asp:Panel>
                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />                
                <div class="DivButton">
                <p class="botaoLeft">
                <asp:Button ID="btnNovo" CssClass="" SkinID="Btn120" runat="server" Text="Nova Entrada"
                        AccessKey="N" Width="200px" OnClick="btnNovo_Click" />
                </p>
                    <p class="botaoRight">                        
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" OnClick="btnSair_Click" />
                    </p>
                </div>                
            </div>
            
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
