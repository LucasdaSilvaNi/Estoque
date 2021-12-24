<%@ Page Title="Módulo Financeiro :: Outras :: Eventos Pagamento SIAFEM" Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="cadastroEventosPagamento.aspx.cs" Inherits="Sam.Web.Financeiro.cadastroEventosPagamento" ValidateRequest="false" %>
<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>Módulo Tabelas - Eventos Pagamento SIAFEM</h1>
        <asp:UpdatePanel runat="server" ID="udpGeral">
            <ContentTemplate>
                <fieldset class="fieldset botaoLeft">
                    <p>
                        <asp:Label ID="lblAnoFechamento" runat="server" CssClass="labelFormulario" Text="Ano Referência:"></asp:Label>
                        <asp:DropDownList ID="ddlAnoFechamento" runat="server" AutoPostBack="True" Width="90px" OnSelectedIndexChanged="ddlAnoFechamento_SelectedIndexChanged"></asp:DropDownList>
                    </p>
                </fieldset>

                <asp:GridView ID="gdvEventosPagamento" runat="server" AllowPaging="true" OnSelectedIndexChanged="gdvEventosPagamento_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField ShowHeader="False" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkID" runat="server" Font-Bold="true" CausesValidation="False" CommandName="Select" Text='<%# Bind("ID") %>' Width="50px" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Ano Base" ItemStyle-Width="55px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAnoBase" Text='<%# Bind("AnoBase") %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo Material" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblTipoMaterial" Visible="true" Text='<%# RetornaDescricaoTipoMaterial((int)Eval("TipoMaterialAssociado"))%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="Tipo Mov. Associado" ItemStyle-Width="140px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblTipoMovimentoAssociado" Text='<%# Bind("TipoMovimentoAssociado.Descricao") %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="1.° Código<br>Código Estorno" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPrimeiroCodigo_PrimeiroCodigoEstorno" Text='<%# String.Format("{0:D6}{1}{2:D6}", Eval("PrimeiroCodigo"), "<br>", Eval("PrimeiroCodigoEstorno")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="1.° Inscrição<br>1.° Classificação" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPrimeiraInscricao_PrimeiraClassificacao" Text='<%# String.Format("{0:D6}{1}{2:D6}", Eval("PrimeiraInscricao"), "<br>", Eval("PrimeiraClassificacao")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="2.° Código<br>Código Estorno" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSegundoCodigo_SegundoCodigoEstorno" Text='<%# String.Format("{0:D6}{1}{2:D6}", Eval("SegundoCodigo"), "<br>", Eval("SegundoCodigoEstorno")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="2.° Inscrição<br>2.° Classificação" ItemStyle-Width="85px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSegundaInscricao_SegundaClassificacao" Text='<%# String.Format("{0:D6}{1}{2:D6}", Eval("SegundaInscricao"), "<br>", Eval("SegundaClassificacao")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="true" HeaderText="UG Favorecida" ItemStyle-Width="55px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblUGFavorecida" Text='<%# RetornaVerdadeiroOuFalso((bool)Eval("UGFavorecida")) %>' Visible="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="True" Visible="false" HeaderText="Ativo" ItemStyle-Width="0px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblAtivo" Text='<%# RetornaVerdadeiroOuFalso((bool)Eval("Ativo")) %>' Visible="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Editar" ItemStyle-Width="15px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:ImageButton ID="linkID" runat="server" Font-Bold="true" ImageUrl="~/Imagens/alterar.gif" CausesValidation="False" CommandName="Select" CommandArgument='<%# Bind("Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo"></HeaderStyle>
                </asp:GridView>
                <div id="DivButton" class="DivButton">
                    <p class="botaoLeft">
                        <asp:Button ID="btnNovo" runat="server" Text="Novo" AccessKey="N" OnClick="btnNovo_Click" />
                    </p>
                    <p class="botaoRight">
                        <asp:Button ID="btnAjuda" runat="server" Visible="true" OnClientClick="OpenModal();" Text="Ajuda" AccessKey="A" OnClick="btnAjuda_Click" />
                    </p>
                </div>
                <asp:Panel runat="server" ID="pnlEditar" Visible="false">
                    <br />
                    <div id="interno">
                        <div>
                            <fieldset class="fieldset">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInfoTipoMovimentoAssociado" runat="server" CssClass="labelFormulario" Width="175px" Text="Tipo Movimento Assoc. *:" Font-Size="12px" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlTipoMovimentoAssociado" Width="175px" AutoPostBack="True" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoTipoMaterial" runat="server" CssClass="labelFormulario" Width="175px" Text="Tipo Material*:" Font-Size="12px" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlTipoMaterial" Width="175px" AutoPostBack="True">
                                                <asp:ListItem Text="Material de Consumo" Value="2" />
                                                <asp:ListItem Text="Material Permanente" Value="3" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoAnoBase" runat="server" CssClass="labelFormulario" Width="175px" Text="Ano-Base*:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtAnoBase" runat="server" MaxLength="4" size="10" CssClass="mesAno" Width="110px"></asp:TextBox> (AAAA)
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInfoPrimeiroCodigo" runat="server" CssClass="labelFormulario" Width="175px" Text="1° Código no Siafem*:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtPrimeiroCodigo" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoPrimeiroCodigoEstorno" runat="server" CssClass="labelFormulario" Width="175px" Text="Código Estorno no Siafem:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtPrimeiroCodigoEstorno" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />                                        
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoPrimeiraInscricao" runat="server" CssClass="labelFormulario" Width="175px" Text="Inscrição:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlPrimeiraInscricao" Width="120px" AutoPostBack="True">
                                                <asp:ListItem Text="CE" Value="CE" />
                                                <asp:ListItem Text="Não" Value="Não" />
                                                <asp:ListItem Text="CE PADRÃO" Value="CE PADRÃO" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoPrimeiraClassificacao" runat="server" CssClass="labelFormulario" Width="175px" Text="Classificação:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtPrimeiraClassificacao" MaxLength="9" runat="server" size="9" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInfoSegundoCodigo" runat="server" CssClass="labelFormulario" Width="175px" Text="2º Código no Siafem*:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtSegundoCodigo" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoSegundoCodigoEstorno" runat="server" CssClass="labelFormulario" Width="175px" Text="Código Estorno no Siafem:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtSegundoCodigoEstorno" CssClass="inputFromNumero" onblur="preencheZeros(this,'6')" MaxLength="6" runat="server" size="6" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoSegundaInscricao" runat="server" CssClass="labelFormulario" Width="175px" Text="Inscrição:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlSegundaInscricao" Width="120px" AutoPostBack="True">
                                                <asp:ListItem Text="CE" Value="CE" />
                                                <asp:ListItem Text="Não" Value="Não" />
                                                <asp:ListItem Text="CE PADRÃO" Value="CE PADRÃO" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoSegundaClassificacao" runat="server" CssClass="labelFormulario" Width="175px" Text="Classificação:" Font-Size="12px" Style="float: none !important" />
                                        </td>
                                        <td style="float:left">
                                            <asp:TextBox ID="txtSegundaClassificacao" MaxLength="9" runat="server" size="9" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInfoUGFavorecida" runat="server" Font-Size="12px" CssClass="labelFormulario" Width="175px" Text="UG Favorecida*:" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlUGFavorecida" Width="75px" AutoPostBack="True">
                                                <asp:ListItem Text="Sim" Value="1" />
                                                <asp:ListItem Text="Não" Value="0" />
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInfoStatusAtivo" runat="server" Font-Size="12px" CssClass="labelFormulario" Width="175px" Text="Status*:" />
                                        </td>
                                        <td style="float:left">
                                            <asp:DropDownList runat="server" ID="ddlStatusAtivo" Width="75px" AutoPostBack="True">
                                                <asp:ListItem Text="Ativo" Value="1" />
                                                <asp:ListItem Text="Inativo" Value="0" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
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
                                <asp:Button ID="btnCancelar" CssClass="" AccessKey="C" runat="server" Text="Cancelar" OnClick="btnCancelar_Click" />
                            </p>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <br />
    </div>
</asp:Content>
