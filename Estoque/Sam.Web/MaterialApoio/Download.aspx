<%@ Page Title="SAM - Sistema de Administração de Materiais" Language="C#" AutoEventWireup="true"
    CodeBehind="Download.aspx.cs" Inherits="Sam.Web.Download" MasterPageFile="~/MasterPage/PrincipalFull.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <div id="content">
        <h1>
            Sistema SAM - Material de Apoio</h1>
        <fieldset class="fieldset">
            <p>
                O Sistema de Administração de Materiais da PRODESP foi desenvolvido especialmente para os diversos órgãos da 
                administração pública do Estado de São Paulo e é integrado aos sistemas da administração financeira, utilizando a 
                estrutura organizacional do SIAFEM, a codificação de materiais do SIAFISICO e BEC, informações de empenhos e planos de trabalho. </p>
            <p>
                O escopo do SAM é a gestão dos materiais consumíveis e dos permanentes, proporcionando apoio as rotinas operacionais, 
                automatização de tarefas e o fornecimento de informações operacionais e gerenciais.
            </p>
        </fieldset>
        <div id="tabelas">
            <h2>Apostila</h2>
            <br />
            <p>As apostilas desenvolvidas pela equipe de desenvolvimento e suporte do sistema SAM, possuem o objetivo de familiarizar o usuário a operar o sistema.</p>
            <br />
            <p> A apostila visa demonstrar o sistema como um todo, para que o usuário possa tirar quaisquer dúvidas especifícas sobre uma dada tela em especial.</p>
            <br />
            <asp:Panel runat="server" ID="pnlDocumentos">
                <asp:GridView  SkinID="GridNovo" ID="grdDocumentos" runat="server" AllowPaging="True" AutoGenerateColumns="False" 
                DataKeyNames="Id" CssClass="tabela">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle CssClass="" HorizontalAlign="Left" />
                    <AlternatingRowStyle CssClass="odd" />
                    <Columns>
                        <asp:TemplateField HeaderText="Download" ShowHeader="False" Visible="true" ItemStyle-Width="7%"  ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                            <asp:ImageButton ID="imgDownloadDocumento" runat="server" Font-Bold="false" ImageUrl='<%# RetornaPathFileIconeTipoRecurso((string)Eval("TipoRecurso"))%>' CommandName="Select" CommandArgument='<%# Eval("Id")%>' Text='<%# Eval("NomeArquivo")%>' ToolTip='<%# Eval("Descricao")%>' OnCommand="linkCodigo_Command"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" ItemStyle-Width="7%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "Codigo")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Material" ItemStyle-Width="16%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "Descricao")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição Material" ItemStyle-Width="70%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "DescricaoDetalhada")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo" />
                </asp:GridView>
                <asp:ObjectDataSource ID="sourceGridDocumentos" runat="server" 
                    SelectMethod="ListarRecursosPorPerfil"
                    EnablePaging="True" 
                    MaximumRowsParameterName="maximumRowsParameterName" 
                    SelectCountMethod="TotalRegistros"
                    StartRowIndexParameterName="startRowIndexParameterName" 
                    TypeName="Sam.Presenter.MaterialApoioPresenter"
                    OldValuesParameterFormatString="original_{0}">
                <SelectParameters>
                    <asp:Parameter Name="Perfil_ID" Type="Int32" />
                </SelectParameters>
                </asp:ObjectDataSource>
            </asp:Panel>
            <br />
        </div>
        <div id="tabelas">
            <h2>Vídeo</h2>
            <br />
            <p>O material audiovisual desenvolvido, possui a finalidade de acelerar a velocidade de aprendizado dos módulos do sistema SAM.</p>
            <p>Possui a função de passar os fluxo de trabalho do sistema de modo simples e didático as funções mais comuns e utilizadas no dia-a-dia de um almoxarifado.</p>
            <br />
            <asp:Panel runat="server" ID="pnlVideos">
                <asp:GridView  SkinID="GridNovo" ID="grdVideos" runat="server" AllowPaging="True" AutoGenerateColumns="False" DataKeyNames="Id" CssClass="tabela">
                    <PagerStyle HorizontalAlign="Center" />
                    <RowStyle CssClass="" HorizontalAlign="Left" />
                    <AlternatingRowStyle CssClass="odd" />
                    <Columns>
                        <asp:TemplateField HeaderText="Download" ShowHeader="False" Visible="true" ItemStyle-Width="7%"  ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                            <asp:ImageButton ID="imgDownloadDocumento" runat="server" Font-Bold="false" ImageUrl='<%# RetornaPathFileIconeTipoRecurso((string)Eval("TipoRecurso"))%>' CommandName="Select" CommandArgument='<%# Eval("Id")%>' Text='<%# Eval("NomeArquivo")%>' ToolTip='<%# Eval("Descricao")%>' OnCommand="linkCodigo_Command"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "Codigo")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Material" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "Descricao")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição Material" ItemStyle-Width="70%" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>                             
                                <%#DataBinder.Eval(Container.DataItem, "DescricaoDetalhada")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="corpo" />
                </asp:GridView>
                <asp:ObjectDataSource ID="sourceGridVideos" runat="server" 
                    SelectMethod="ListarRecursosPorPerfil" EnablePaging="True" 
                    MaximumRowsParameterName="maximumRowsParameterName" 
                    SelectCountMethod="TotalRegistros" 
                    StartRowIndexParameterName="startRowIndexParameterName" 
                    TypeName="Sam.Presenter.MaterialApoioPresenter" 
                    OldValuesParameterFormatString="original_{0}">
                <SelectParameters>
                    <asp:Parameter Name="Perfil_ID" Type="Int32" />
                </SelectParameters>
                </asp:ObjectDataSource>
            </asp:Panel>
            <br />
        </div>
        <br />
        <br />
        </div>        
        
            
</asp:Content>
