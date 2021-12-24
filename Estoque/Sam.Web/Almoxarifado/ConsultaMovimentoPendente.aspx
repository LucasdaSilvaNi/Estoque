<%@ Page Language="C#" AutoEventWireup="true" 
                       CodeBehind="ConsultaMovimentoPendente.aspx.cs" 
                       MasterPageFile="~/MasterPage/PrincipalFull.Master" 
                       Title="Consulta Movimentos Pendentes de Correção"  
                       Inherits="Sam.Web.Almoxarifado.ConsultaMovimentoPendente" 
                       EnableViewState="true"
                       ValidateRequest="false" 
                       EnableEventValidation="false" %>

<%@ Register Src="../Controles/ListInconsistencias.ascx" TagName="ListInconsistencias" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
    <script type="text/javascript" src="<%=ResolveClientUrl("~/JScript/jquery-floatnumber.js")%>"></script>
    <link href="../CSS/cupertino/jquery-ui-1.7.3.custom.css" rel="stylesheet" type="text/css" />
    <script src="../JScript/jquery-ui-1.7.3.custom.min.js" type="text/javascript"></script>
    <script src="../JScript/Modal.js" type="text/javascript"></script>
   <%-- <script language="javascript" type="text/javascript">
     $('#labelTotalRegistro').text('Total price: ');
    </script>--%>
        <asp:UpdatePanel ID="updPendente" runat="server">
        <ContentTemplate>
            <div id="content">
            <asp:Timer ID="timerArquivo" runat="server" Enabled="false" Interval="20000" 
                    ontick="timerArquivo_Tick">
        </asp:Timer>
                <h1>
                    Módulo Almoxarifado - Consultar Movimentos Pendentes</h1>
                       
                            <fieldset class="fieldset">
                                <div id="Div1">
                                    <br />
                                    <p id="paragrafo1" class="paragrafo">
                                        <label id="labelAlmoxarifado" runat="server" class ="labelFormulario"><b>Almoxarifado:</b></label>
                                        <asp:DropDownList ID="drpAlmoxarifado" runat="server" Width="70%" AutoPostBack="true"
                                            onselectedindexchanged="drpAlmoxarifado_SelectedIndexChanged"></asp:DropDownList>
                                    </p>
                                    <p id="paragrafo3">
                                        <label id="labelSubItem" runat="server" class ="labelFormulario"><b>Cod.SubItem:</b></label>
                                        <asp:TextBox ID="txtSubtItemCodigo" runat="server" CssClass="textBox"></asp:TextBox>
                                    </p>
                                   
                                    <p class="botaoRight">
                                        <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" Width="200px" 
                                            onclick="btnPesquisar_Click" />
                                        <asp:Button ID="btnRecalculo" runat="server" Text ="Recalculo" 
                                            onclick="btnRecalculo_Click" />
                                    </p>
                                </div>
                            </fieldset>
                            <asp:GridView ID="grdMovimento"  SkinID="GridModal" runat="server" 
                                            AllowPaging="True" AutoGenerateColumns="False"
                                            CssClass="tabela" DataKeyNames="TB_MOVIMENTO_ID" 
                                            onpageindexchanging="grdMovimento_PageIndexChanging" Visible="false" 
                                            onrowdatabound="grdMovimento_RowDataBound">
                                            <PagerStyle HorizontalAlign="Center" />
                                            <RowStyle CssClass="" HorizontalAlign="Left" />
                                            <AlternatingRowStyle CssClass="odd" />
                                            <Columns>
                                                <asp:BoundField DataField="TB_MOVIMENTO_DATA_MOVIMENTO" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Data Mov.">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_DATA_DOCUMENTO" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Wrap="false" HeaderStyle-Width="7%" 
                                                    HeaderText="Data Doc.">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_DATA_OPERACAO" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Data Ope.">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>   
                                                <asp:BoundField DataField="TB_MOVIMENTO_ANO_MES_REFERENCIA" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Mês de Ref.">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_TIPO_MOVIMENTO_DESCRICAO" HeaderStyle-Wrap="false" HeaderStyle-Width="10%"
                                                    HeaderText="Mov. Descrição">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_SUBITEM_MATERIAL_CODIGO" DataFormatString="{0:0}" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Cod. SubItem">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_NUMERO_DOCUMENTO" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="N. Documento">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                
                                                <asp:BoundField DataField="TB_MOVIMENTO_ITEM_QTDE_MOV" HeaderStyle-Wrap="false" HeaderStyle-Width="5%"
                                                    HeaderText="Qtde Mov." ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_ITEM_VALOR_MOV" ReadOnly="true" DataFormatString="{0:N2}" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Vlr Mov." ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_ITEM_SALDO_QTDE" ReadOnly="true" DataFormatString="{0:0}" HeaderStyle-Wrap="false" HeaderStyle-Width="5%"
                                                    HeaderText="Qtde Saldo" ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_ITEM_SALDO_VALOR" ReadOnly="true" DataFormatString="{0:N2}" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Vlr Saldo" ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="TB_MOVIMENTO_ITEM_PRECO_UNIT" ReadOnly="true" DataFormatString="{0:N2}" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Preço Médio" ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                                 <asp:BoundField DataField="TB_MOVIMENTO_ITEM_DESD" ReadOnly="true" DataFormatString="{0:N2}" HeaderStyle-Wrap="false" HeaderStyle-Width="7%"
                                                    HeaderText="Desdobro" ItemStyle-HorizontalAlign="Right">
                                                    <FooterStyle Wrap="False" />
                                                    <HeaderStyle />
                                                </asp:BoundField>
                                            </Columns>
                                            <HeaderStyle CssClass="corpo" />
                        </asp:GridView>
                        <asp:ObjectDataSource ID="movimento" runat="server" 
                                              SelectMethod="PesquisaMovimentoPendente" 
                                              TypeName="Sam.Web.Almoxarifado.ConsultaMovimentoPendente" 
                                              SelectCountMethod ="TotalRegistros" 
                                              EnablePaging="true" 
                                              MaximumRowsParameterName="maximumRowsParameterName" 
                                              
                    StartRowIndexParameterName="StartRowIndexParameterName">
                            <SelectParameters>
                                <asp:Parameter Name="maximumRowsParameterName" Type="Int32" />
                                <asp:Parameter Name="StartRowIndexParameterName" Type="Int32" />
                                <asp:ControlParameter ControlID="drpAlmoxarifado" Name="almoxarifadoId" 
                                    PropertyName="SelectedValue" Type="Int32" />
                                <asp:ControlParameter ControlID="txtSubtItemCodigo" Name="subtItemCodigo" 
                                    PropertyName="Text" Type="Int64" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                            <div id="totalRegistro" style="float:right;padding-right:10px;">
                                <span id="spanTotalMovimento" class="labelFormulario" runat="server">Total de Sub Itens(s) :</span>
                                <asp:label ID="labelTotalRegistro" runat="server"  CssClass="labelFormulario"></asp:label>
                            </div>
                            <div id="container_abas_consultas">
                                <uc1:ListInconsistencias ID="ListInconsistencias" runat="server" EnableViewState="False" />
		                    </div>
                       </div>
            
             
        </ContentTemplate>
    </asp:UpdatePanel>

   
</asp:Content>

