<%@ Page Language="C#" MasterPageFile="~/MasterPage/PrincipalFull.Master" AutoEventWireup="true" CodeBehind="tabMenu.aspx.cs" Inherits="Sam.Web.Tabelas.tabMenu" Title="SAM - Sistema de Administração de Materiais" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" runat="server">
<script src="../JScript/jquery.abas.js" type="text/javascript"></script>

    <div id="content"> 
		 
			<div id="container_abas_tabelas">
			
				<!-- Inicio Aba 1 - Estrutura Organizacional -->
				<div id="aba1" class="aba">
					<h1>Módulo Tabelas - Estrutura Organizacional</h1>				
<%--	         	<img src="../imagens/menu_estrutura_organizacional.gif" alt="Menu Estrutura Organizacional" usemap="#MapEstOrg" />
	            <map  id="MapEstOrg">
	                <area shape="rect" coords="360,8,546,47" href="cadastroOrgao.aspx" alt="Órgão"/>
	                <area shape="rect" coords="11,388,152,427" href="cadastroResponsavel.aspx" alt="Responsável" />
	                <area shape="rect" coords="202,95,388,135" href="cadastroUO.aspx" alt="UO" />
	                <area shape="rect" coords="542,388,683,429" href="cadastroAlmoxarifado.aspx" alt="Almoxarifado" />
	                <area shape="rect" coords="10,291,151,332" href="cadastroCentroCusto.aspx" alt="Centrp de Custo" />
	                <area shape="rect" coords="207,388,392,427" href="cadastroDivisao.aspx" alt="Divisão" />
	                <area shape="rect" coords="499,92,684,131" href="cadastroGestor.aspx" alt="Gestor" />
	                <area shape="rect" coords="434,290,575,331" href="cadastroUnidade.aspx" alt="Unidade" />
	                <area shape="rect" coords="203,291,387,331" href="cadastroUA.aspx" alt="UA" />
	                <area shape="rect" coords="202,195,388,235" href="cadastroUGE.aspx" alt="UGE" />
	            </map>--%>

                <div id="MenuFluxoSO" runat="server"></div>
	        	</div>
			  	<!-- Fim Aba 1 - Estrutura Organizacional -->								
						
				<!-- Inicio Aba 2 - Catálogo-->
				<div id="aba2" class="aba">
					<h1>Módulo Tabelas - Catálogo</h1>
<%--	            <img src="../imagens/menu_catalogo.gif" alt="Menu Catálogo" usemap="#MapCatalogo"/>--%>
<%--	            <map name="MapCatalogo" id="Map2">
	                <area shape="rect" coords="236,7,421,47" href="cadastroGrupo.aspx" alt="Grupo" />
	                <area shape="rect" coords="236,411,421,451" href="cadastroSubItemMaterial.aspx" alt="Subitem de Material" />
	                <area shape="rect" coords="236,184,421,224" href="cadastroMaterial.aspx" alt="Material" />


	                <area shape="rect" coords="506,501,691,541" href="cadastroContaAuxiliar.aspx" alt="Conta Auxiliar" />
	                <area shape="rect" coords="236,94,421,134" href="cadastroClasse.aspx" alt="Classe" />
	                <area shape="rect" coords="236,275,421,315" href="cadastroItemMaterial.aspx" alt="Item de Material" />
	                <area shape="rect" coords="506,411,690,451" href="cadastroUnidadeFornecimento.aspx" alt="Unidade de Fornecimento" />
	                <area shape="rect" coords="506,313,691,353" href="cadastroNaturezaDespesa.aspx" alt="Natureza de Despesa" />
	                <area shape="rect" coords="7,344,192,384" href="cadastroRelacaoItemSubItem.aspx" alt="Definir Relação ItemxSubitem" />
	            </map>   --%>
                     <div id="MenuFluxoCatalogo" runat="server"></div>
	        	</div>
			  	<!-- Fim Aba 2 - Catálogo -->								
						
				<!-- Inicio Aba 3 - Outros -->
				<div id="aba3" class="aba">
					<h1>Módulo Tabelas - Outras</h1>
<%--	            <img src="../imagens/menu_outros.gif" alt="Menu Outros" usemap="#MapOutros" />--%>
<%--	            <map  id="MapOutros">
	                <area shape="rect" coords="473,308,658,348" href="cadastroMotivoBaixa.aspx" alt="Tipo de Baixa" />
	                <area shape="rect" coords="33,223,218,263" href="cadastroPTRes.aspx" alt="PT_RES" />
	                <area shape="rect" coords="33,93,218,133" href="cadastroFontesRecurso.aspx" alt="Fonte de Recurso" />

	                <area shape="rect" coords="473,14,658,54" href="cadastroSigla.aspx" alt="Sigla" />
	                <area shape="rect" coords="33,8,218,48" href="cadastroFornecedor.aspx" alt="Fornecedor" />
	                <area shape="rect" coords="473,212,658,252" href="cadastroTipoIncorp.aspx" alt="Tipo de Incorporação" />
	                <area shape="rect" coords="473,112,658,152" href="cadastroTerceiro.aspx" alt="Terceiro" />
	            </map>	 --%>
                <div id="MenuFluxoOutros" runat="server"></div>
	        	</div>			  
				<!-- Fim Aba 3 - Outros -->
					
			</div>
			<!-- Fim do container das Abas -->
		  
		
		</div> 
		<!-- fim id content -->
  
</asp:Content>

