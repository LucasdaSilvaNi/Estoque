<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true"
    CodeBehind="cadastroOrgao.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroOrgao"
    Title="Módulo Tabelas :: Estrutura Organizacional :: Órgão" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>Módulo Tabelas - Estrutura Organizacional - Órgão</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <asp:GridView ID="gridOrgao" runat="server" AllowPaging="true" OnSelectedIndexChanged="gridOrgao_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                    CommandName="Select" Text='<%# Bind("ID") %>' Width="50px"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D2}" ApplyFormatInEditMode="False">
                            <ItemStyle Width="50px"></ItemStyle>
                        </asp:BoundField>

                        <%--  <asp:BoundField DataField="lblAtivo" Visible="false" HeaderText="" />--%>
                        <asp:TemplateField Visible="false" HeaderText="">
                            <ItemTemplate>
                                <asp:Label ID="lblAtivo" Visible="false" runat="server" Text='<%#Eval("Ativo")%>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricaoOrgao" runat="server" Text='<%# Convert.ToString(Eval("Descricao")) + (Convert.ToBoolean(Eval("Ativo"))==true ?  "" : " (Inativo)") %>'>                        
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Implantado">
                            <ItemTemplate>
                                <asp:Label ID="lblImplantado" Width="50px" runat="server" Text='<%# (Convert.ToBoolean(Eval("Implantado"))==true ?  "Sim" : "Não") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>


                         <asp:TemplateField HeaderText="IntegracaoSIAFEM">
                            <ItemTemplate>
                                <asp:Label ID="ddlIntegracaoSIAFEM" Width="50px" runat="server" Text='<%# (Convert.ToBoolean(Eval("IntegracaoSIAFEM"))==true ?  "Sim" : "Não") %>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

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
                <asp:ObjectDataSource ID="sourceDados" runat="server" EnablePaging="True" MaximumRowsParameterName="maximumRowsParameterName"
                    StartRowIndexParameterName="startRowIndexParameterName" SelectCountMethod="TotalRegistros"
                    SelectMethod="PopularDados"
                    TypeName="Sam.Presenter.OrgaoPresenter"
                    OldValuesParameterFormatString="original_{0}">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <div id="Div1" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" OnClick="btnAjuda_Click" />
                        <asp:Button ID="btnVoltar" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" Visible="False">
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="145px" Text="Código*:" />
                                    <asp:TextBox ID="txtCodigo" CssClass="inputFromNumero" MaxLength="5"
                                        runat="server" size="2"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label2" runat="server" CssClass="labelFormulario" Width="145px" Text="Descrição*:" />
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="120" size="90"></asp:TextBox>
                                </p>

                                <p>
                                    <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="145px" Text="Status*:"></asp:Label>
                                    <asp:DropDownList ID="ddlAtivo" runat="server" DataTextField="ddlAtivo" Width="100px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label3" runat="server" CssClass="labelFormulario" Width="145px" Text="Implantado*:"></asp:Label>
                                    <asp:DropDownList ID="ddlImplantado" runat="server" DataTextField="Implantado" Width="100px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Sim</asp:ListItem>
                                        <asp:ListItem Value="False">Não</asp:ListItem>
                                    </asp:DropDownList>

                                </p>

                                <p>
                                       <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="145px" Text="Integração SIAFEM*:"></asp:Label>
                                    <asp:DropDownList
                                        runat="server" ID="ddlIntegracaoSIAFEM" Width="100px" AutoPostBack="True"
                                        DataTextField="IntegracaoSIAFEM" DataValueField="Id">
                                        <asp:ListItem Text="Sim" Value="true" />
                                        <asp:ListItem Text="Não" Value="false" />
                                    </asp:DropDownList>
                               
                                </p>

                            </fieldset>
                        </div>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div>
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
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
