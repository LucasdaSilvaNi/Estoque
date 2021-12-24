<%@ Page Title="Módulo SAC :: Atendimento :: Abertura Chamado" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="AberturaChamado.aspx.cs" Inherits="Sam.Web.SAC.AberturaChamado" ValidateRequest="false" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>
<%@ Register Src="~/Controles/ComboboxesHierarquiaPadrao.ascx" TagName="CombosHierarquiaPadrao" TagPrefix="cmbHierarquiaPadrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>Módulo SAC - Atendimento - Abertura Chamado</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <fieldset class="fieldset">
                    <cmbHierarquiaPadrao:CombosHierarquiaPadrao ID="combosEstruturaOrganizacionalPadrao" runat="server" EnableViewState="true" ShowStatus="false" ShowNumeroRequisicao="false" />
                </fieldset>

                <asp:GridView ID="gdvAtendimentoChamados" runat="server" AllowPaging="false" OnSelectedIndexChanged="gdvAtendimentoChamados_SelectedIndexChanged" OnRowDataBound="gdvAtendimentoChamados_RowDataBound" OnRowCommand="gdvAtendimentoChamados_RowCommand" OnPageIndexChanging="gdvAtendimentoChamados_PageIndexChanging" PageSize="20" OnRowCreated="gdvAtendimentoChamados_RowCreated">
                    <Columns>
                        <asp:TemplateField ShowHeader="True" Visible="false" HeaderText="Lote" ItemStyle-Width="1%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkAtualizarStatusLote" Text="" Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Chamado" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkChamadoID" runat="server" Font-Bold="true" CausesValidation="False" Visible="false" />
                                <asp:Label runat="server" ID="lblNumeroChamado" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Cliente" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblNomeReduzidoCliente" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Abertura" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataAberturaChamado" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Última Edição" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataHoraUltimaEdicao" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="false" HeaderText="Usuário" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIdentificacaoUsuario" Text="..." Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Usuário" ItemStyle-Width="8%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCpfUsuarioSolicitante" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Funcionalidade" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblFuncionalidadeSistema" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo Chamado" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblTipoChamado" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Status (Usuário)" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblStatusChamadoAtendimentoUsuario" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Status (Prodesp)" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblStatusChamadoAtendimentoProdesp" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Usuário (SuporteSAM)" ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblResponsavelUsuarioSAM" Text="..." Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Fechamento" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDataFechamentoChamado"  Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Ação" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgEdicao" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" CausesValidation="False" CommandName="Select" />
                                <asp:ImageButton ID="imgHistorico" runat="server" Font-Bold="true" ImageUrl="~/Imagens/indicativo_historico.png" CausesValidation="False" CommandName="Select" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo"></HeaderStyle>
                </asp:GridView>

                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" CssClass="" runat="server" Text="Novo Chamado" AccessKey="N" OnClick="btnNovo_Click" style="width: 120px !important" Visible="true" />
                        <asp:Button ID="btnEditarStatusLote" runat="server" Text="Atualizar Status Em Lote" CssClass="" style="width: 180px !important; font-weight: bold;" OnClick="btnEditarStatusLote_Click" />
                    </p>
                    <p class="botaoRight">
<%--                        <asp:Button ID="Button1" runat="server" Text="Imprimir" />--%>
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();"
                            Text="Ajuda" />
                        <asp:Button ID="btnSair" runat="server" Text="Voltar" CssClass="" PostBackUrl="~/Tabelas/TABMenu.aspx" />
                    </p>
                </div>

                <asp:Panel runat="server" ID="pnlEditar">
                    <br />
                    <div id="interno">
                        <fieldset class="fieldset">
                            <div style="width:50%; float:left">
                                <p>
                                    <asp:Label ID="lblStatusAtendimentoProdesp" runat="server" CssClass="labelFormulario" Width="120px" Text="Status (Prodesp):" />
                                    <asp:DropDownList runat="server" ID="ddlStatusChamadoAtendimentoProdesp" Width="430px">
                                        <asp:ListItem Value="1">Aberto</asp:ListItem>
                                        <asp:ListItem Value="2" Selected="True">Em Atendimento</asp:ListItem>
                                        <asp:ListItem Value="3">Aguardando Usuário</asp:ListItem>
                                        <asp:ListItem Value="5">Finalizado</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblNumeroChamadoInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Chamado:" />
                                    <asp:TextBox ID="txtNumeroChamado" runat="server" Width="430px" CssClass="inputFromNumero" Enabled="false" />
                                </p>
                                <p>
                                    <asp:Label ID="lblNomeReduzidoClienteInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Cliente:" />
                                    <asp:TextBox ID="txtNomeReduzidoCliente" runat="server" Width="430px" Enabled="false" />
                                </p>
                                <div style="width:40%; position:relative; float:left">
                                    <p>
                                        <asp:Label ID="lblDataAberturaChamadoInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Data Abertura:" />
                                        <asp:TextBox ID="txtDataAberturaChamado" runat="server" Width="100px" Enabled="false" />
                                    </p>
                                </div>
                                <div style="width:60%; position:relative; float:right">
                                    <p>
                                        <asp:Label ID="lblDataFechamentoChamado" runat="server" CssClass="labelFormulario" Width="120px" Text="Data Fechamento:" />
                                        <asp:TextBox ID="txtDataFechamentoChamado" runat="server" Width="100px" Enabled="false" />
                                    </p>
                                </div>
                                <div style="width:40%; position:relative; float:left">
                                    <p>
                                        <asp:Label ID="lblCpfUsuarioInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="CPF Usuário:" />
                                        <asp:TextBox ID="txtCpfUsuario" runat="server" Width="100px" Enabled="false"  />
                                    </p>
                                </div>
                                <div style="width:60%; position:relative; float:right">
                                    <p>
                                        <asp:Label ID="lblNomeUsuarioInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Nome Usuário:" />
                                        <asp:TextBox ID="txtNomeUsuario" runat="server" Width="185px" Enabled="false" />                                
                                    </p>
                                </div>
                                <p>
                                    <asp:Label ID="lblDescricaoPerfilUsuario" runat="server" CssClass="labelFormulario" Width="120px" Text="Perfil Usuário:" />
                                    <asp:TextBox ID="txtDescricaoPerfilUsuario" runat="server" Width="430px" Enabled="false" />
                                    <asp:TextBox ID="perfilUsuarioAberturaChamado" Width="20px" Enabled="false" Visible="false" runat="server"></asp:TextBox>
                                </p>
                                <p>
                                    <asp:Label ID="lblEMailUsuarioInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="E-Mail*:" />
                                    <asp:TextBox ID="txtEMailUsuario" runat="server" Width="430px" />
                                </p>
                                <p>
                                    <asp:Label ID="lblSistemaModuloInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Sistema/Módulo:*" />
                                    <asp:DropDownList runat="server" ID="ddlSistemaModulo" Enabled="false" Width="430px">
                                        <asp:ListItem Value="0" Selected="True">SAM / Estoque</asp:ListItem>
                                        <asp:ListItem Value="1">SAM / Patrimônio</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblAmbienteSistemaInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Ambiente:*" />
                                    <asp:DropDownList runat="server" ID="ddlAmbienteSistema" Enabled="true" Width="430px">
                                        <asp:ListItem Value="0" Selected="True">Produção</asp:ListItem>
                                        <asp:ListItem Value="1">Homologação</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblStatusChamadoAtendimentoUsuario" runat="server" CssClass="labelFormulario" Width="120px" Text="Status (Usuário)*:" />
                                    <asp:DropDownList runat="server" ID="ddlStatusChamadoAtendimentoUsuario" Width="430px">
                                        <asp:ListItem Value="1" Selected="True">Aberto</asp:ListItem>
                                        <asp:ListItem Value="6">Reaberto</asp:ListItem>
                                        <asp:ListItem Value="4">Concluído</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblNomeAtendenteSuporteInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Responsável*:" />
                                    <asp:TextBox ID="txtNomeAtendenteSuporte" runat="server" Enabled="true" Width="430px" Text="SuporteSAM" />
                                </p>
                                <p>
                                    <asp:Label ID="lblFuncionalidadeSistemaInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Funcionalidade:*" />
                                    <asp:DropDownList runat="server" ID="ddlFuncionalidadeSistema" Width="430px" ViewStateMode="Enabled">
                                        <asp:ListItem Value="-1" Selected="True">- Selecione - </asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblTipoChamadoInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Tipo Chamado:*" />
                                    <asp:DropDownList runat="server" ID="ddlTipoChamado" Width="430px">
                                        <asp:ListItem Value="0" Selected="True">- Selecione - </asp:ListItem>
                                        <asp:ListItem Value="1">Erro</asp:ListItem>
                                        <asp:ListItem Value="2">Dúvida</asp:ListItem>
                                        <asp:ListItem Value="3">Melhoria</asp:ListItem>
                                        <asp:ListItem Value="4">Solicitação</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="lblObservacoesChamadoInfo" runat="server" CssClass="labelFormulario" Width="120px" Text="Observações*:" />
                                    <asp:TextBox ID="txtObservacoesChamado" TextMode="MultiLine" MaxLength="77" Rows="10" Width="430px" Style="height: 90%" SkinID="MultiLine" runat="server" />
                                </p>
                                <p>
                                    <asp:Label ID="lblUpload" runat="server" CssClass="labelFormulario" Width="120px" Text="Anexar Arquivo:" Font-Bold="true" Visible="true" />
                                    <asp:UpdatePanel ID="updFileUp" runat="server" Visible="true">
                                        <ContentTemplate>
                                            <div style="text-align: left">
                                                <asp:FileUpload ID="fuplAnexo" runat="server" Width="430px" Style="height: 90%"/>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:PostBackTrigger ControlID="btnImportarArquivo" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                    <p>
                                    </p>
                                    <p>
                                        <asp:Label ID="lblListaArquivosAnexosInfo" runat="server" 
                                            CssClass="labelFormulario" Text="Arquivos Anexos:" Visible="true" 
                                            Width="120px" />
                                        <asp:ListBox ID="lstListaArquivosAnexos" runat="server" Rows="4" 
                                            SkinID="MultiLine" Style="height: 15%" TextMode="MultiLine" Visible="true" 
                                            Width="430px" />
                                    </p>
                                    <p style="text-align: center;padding-left: 80px;">
                                        <asp:Button ID="btnImportarArquivo" runat="server" 
                                            OnClick="btnImportarArquivo_Click" SkinID="Btn120" Text="Anexar Arquivo" 
                                            Visible="true" />
                                        <asp:Button ID="btnDownloadAnexos" runat="server" 
                                            OnClick="btnDownloadAnexos_Click" SkinID="Btn100" Text="Baixar Anexos" 
                                            Visible="true" />
                                        <asp:Button ID="btnRemoverArquivo" runat="server" 
                                            OnClick="btnRemoverArquivo_Click" SkinID="Btn100" Text="Excluir Anexo" 
                                            Visible="true" Width="50px" />
                                    </p>
                                </p>
                            </div>

                            <div style="float: right;width:50%">
                                <p style="position: relative; left: auto; vertical-align: top">
                                    <asp:Label ID="lblInfoLogHistoricoAtendimento" runat="server" CssClass="labelFormulario" Width="150px" Text="Histórico Atendimento" />
                                    <div id="logHistorico" runat="server"></div>
                                </p>
                            </div>
                        </fieldset>
                        <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <br />
                    <!-- fim id interno -->
                    <div class="Divbotao">
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnGravar" runat="server" Text="Gravar" CssClass="button" OnClick="btnGravar_Click" />
                                <asp:Button ID="btnExcluir" runat="server" Text="Excluir" CssClass="button" OnClick="btnExcluir_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="button" OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="pnlStatusAtualizar">
                    <br />
                    <div id="painelAtualizarStatus">
                        <fieldset class="fieldset">
                            <div style="width:50%; float:left">
                                <p>
                                    <asp:Label ID="Label1" runat="server" CssClass="labelFormulario" Width="120px" Text="Status (Prodesp):" />
                                    <asp:DropDownList runat="server" ID="ddlStatusAtualizar" Width="430px">
                                        <asp:ListItem Value="2" Selected="true">Em Atendimento</asp:ListItem>
                                        <asp:ListItem Value="3">Aguardando Usuário</asp:ListItem>
                                        <asp:ListItem Value="5">Finalizado</asp:ListItem>
                                    </asp:DropDownList>
                                </p>
                                <p>
                                    <asp:Label ID="Label13" runat="server" CssClass="labelFormulario" Width="120px" Text="Responsável*:" />
                                    <asp:TextBox ID="txtResponsavelStatusAtualizar" runat="server" Enabled="true" Width="430px" Text="" />
                                </p>
                                <p>
                                    <asp:Label ID="Label16" runat="server" CssClass="labelFormulario" Width="120px" Text="Observações*:" />
                                    <asp:TextBox ID="txtObservacoesStatusAtualizar" TextMode="MultiLine" MaxLength="77" Rows="10" Width="430px" Style="height: 90%" SkinID="MultiLine" runat="server" Text="" />
                                </p>
                            </div>
                        </fieldset>
                        <uc1:ListInconsistencias ID="ListInconsistencias_1" runat="server" EnableViewState="False" />
                        <p>
                            <small>Os campos marcados com (*) são obrigatórios. </small>
                        </p>
                    </div>
                    <br />
                    <!-- fim id interno -->
                    <div class="Divbotao">
                        <div class="DivButton">
                            <p class="botaoLeft">
                                <asp:Button ID="btnStatusAtualizar" runat="server" Text="Gravar" CssClass="button" OnClick="btnStatusAtualizar_Click" />
                                <asp:Button ID="btnCancelarStatusAtualizar" runat="server" Text="Cancelar" CssClass="button" OnClick="btnCancelarStatusAtualizar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>

            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
