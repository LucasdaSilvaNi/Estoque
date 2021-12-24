<%@ Page Title="Módulo Tabelas :: Estrutura Organizacional :: UGE" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroUGE.aspx.cs" Inherits="Sam.Web.Seguranca.cadastroUGE"
    ValidateRequest="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">

    <div id="content">
        <h1>Módulo Tabelas - Estrutura Organizacional - UGE</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <div id="Div3">
                        <p>
                            <asp:Label ID="Label2" runat="server" class="labelFormulario" Width="100px" Text="Órgão*:" Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlOrgao" Width="80%" AutoPostBack="True"
                                DataTextField="CodigoDescricao" DataValueField="Id"
                                OnSelectedIndexChanged="ddlOrgao_SelectedIndexChanged" />
                            <asp:ObjectDataSource ID="sourceListaOrgao" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosTodosCod" TypeName="Sam.Presenter.OrgaoPresenter" />
                        </p>
                        <p>
                            <asp:Label ID="Label3" runat="server" class="labelFormulario" Width="100px" Text="UO*:" Font-Bold="true" />
                            <asp:DropDownList runat="server" ID="ddlUo" Width="80%" AutoPostBack="True"
                                DataTextField="CodigoDescricao" DataValueField="Id"
                                OnSelectedIndexChanged="ddlUo_SelectedIndexChanged"
                                OnDataBound="ddlUo_DataBound" />
                            <asp:ObjectDataSource ID="sourceListaUo" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="PopularDadosTodosCod" TypeName="Sam.Presenter.UOPresenter">
                                <SelectParameters>
                                    <asp:ControlParameter ControlID="ddlOrgao" DefaultValue="SelectedValue"
                                        Name="_orgaoId" PropertyName="SelectedValue" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </p>
                    </div>
                </fieldset>
                <br />
                <asp:GridView ID="gridUGE" runat="server" AllowPaging="True" OnSelectedIndexChanged="gridUGE_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton1" runat="server" Font-Bold="true" CausesValidation="False"
                                    CommandName="Select" Text='<%# Bind("Id") %>' />
                                <asp:Label ID="lblTipoUge" runat="server" Text='<%# Bind("TipoUge") %>' Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Codigo" HeaderText="Cód." ItemStyle-Width="50px" DataFormatString="{0:D6}">
                            <ItemStyle Width="50px"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:Label ID="lblDescricaoUge" runat="server" Text='<%#                                                                                                            
                                  Convert.ToString(Eval("Descricao") + ((bool)Eval("Ativo") ? "" : " (Inativa)"))%>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblAtivo" Visible="false" runat="server" Text='<%#Eval("Ativo")%>'>
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Integração SIAFEM">
                            <ItemTemplate>
                                <asp:Label ID="lblIntegracaoSIAFEM" runat="server" Text='<%# (Convert.ToBoolean(Eval("IntegracaoSIAFEM"))==true ?  "Sim" : " Não") %>'>                                                          
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Implantado">
                            <ItemTemplate>
                                <asp:Label ID="lblImplantado" runat="server" Text='<%# (Convert.ToBoolean(Eval("Implantado"))==true ?  "Sim" : " Não") %>'>                                                          
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
                <asp:ObjectDataSource ID="sourceGridUGE" runat="server"
                    SelectMethod="PopularDadosUGE"
                    TypeName="Sam.Presenter.UGEPresenter" EnablePaging="True"
                    MaximumRowsParameterName="maximumRowsParameterName"
                    SelectCountMethod="TotalRegistros"
                    StartRowIndexParameterName="startRowIndexParameterName">
                    <SelectParameters>
                        <asp:Parameter Name="startRowIndexParameterName" Type="Int32" />
                        <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                        <asp:ControlParameter ControlID="ddlUo" Name="_uoId" PropertyName="SelectedValue"
                            Type="Int32" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnImprimir" runat="server" CssClass="" OnClick="btnImprimir_Click"
                            Text="Imprimir" AccessKey="I" />
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" CssClass=""
                            AccessKey="A" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx"
                            AccessKey="V" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" Visible="False">
                    <br />
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <p>
                                    <asp:Label ID="codigo" runat="server" CssClass="labelFormulario" Width="145px" Text="Código*:" />
                                    <asp:TextBox ID="txtCodigo" MaxLength="6" CssClass="inputFromNumero" runat="server" size="6" />
                                </p>
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="145px" Text="Descrição*:" />
                                    <asp:TextBox ID="txtDescricao" runat="server" MaxLength="120" size="90"
                                        Width="80%"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="Label4" runat="server" CssClass="labelFormulario" Width="145px" Text="Tipo*:"></asp:Label>
                                    <asp:DropDownList
                                        runat="server" ID="ddlTipo" Width="100px" AutoPostBack="True"
                                        DataTextField="Descricao" DataValueField="Id">
                                        <asp:ListItem Text="Normal" Value="0" />
                                        <asp:ListItem Text="Contábil" Value="1" />
                                    </asp:DropDownList>
                                </p>

                                <p>
                                    <asp:Label ID="Label10" runat="server" CssClass="labelFormulario" Width="145px" Text="Status*:"></asp:Label>
                                    <asp:DropDownList ID="ddlAtivo" runat="server" DataTextField="Status" Width="100px"
                                        DataValueField="Id">
                                        <asp:ListItem Value="True">Ativo</asp:ListItem>
                                        <asp:ListItem Value="False">Inativo</asp:ListItem>
                                    </asp:DropDownList>
                                </p>

                                <p>
                                    <asp:Label ID="Label6" runat="server" CssClass="labelFormulario" Width="145px" Text="Integração SIAFEM*:"></asp:Label>
                                    <asp:DropDownList
                                        runat="server" ID="ddlIntegracaoSIAFEM" Width="100px" AutoPostBack="True"
                                        DataTextField="UgeIntegracaoSIAFEM" DataValueField="Id">
                                        <asp:ListItem Text="Sim" Value="true" />
                                        <asp:ListItem Text="Não" Value="false" />
                                    </asp:DropDownList>
                                </p>

                                <p>
                                    <asp:Label ID="Label5" runat="server" CssClass="labelFormulario" Width="145px" Text="Implantado*:"></asp:Label>
                                    <asp:DropDownList
                                        runat="server" ID="ddlUgeImplantado" Width="100px" AutoPostBack="True"
                                        DataTextField="UgeImplantado" DataValueField="Id">
                                        <asp:ListItem Text="Sim" Value="true" />
                                        <asp:ListItem Text="Não" Value="false" />
                                    </asp:DropDownList>
                                </p>

                                </p>
                            </fieldset>
                        </div>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <!-- fim id interno -->
                    <div class="Divbotao">
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
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
