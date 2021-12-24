using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Presenter;
using System.Text;
using System.Collections.Generic;

namespace Sam.Web.Tabelas
{
    public partial class tabMenu : System.Web.UI.Page, IMenuTabelasView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MenuTabelasPresenter presenter = new MenuTabelasPresenter(this);
                presenter.Load();
                presenter.RetornaTransacoesModulo();
            }
        }

        private void MontarMenuPerfil(List<Sam.Entity.Transacao> Transacoes)
        {
            StringBuilder sbEO = new StringBuilder();
            StringBuilder sbCatalogo = new StringBuilder();
            StringBuilder sbEOutros = new StringBuilder();

            sbEO.AppendLine("<img src=\"../imagens/menu_estrutura_organizacional.gif\" alt=\"Menu Estrutura Organizacional\" usemap=\"#MapEstOrg\" />");
            sbEO.AppendLine("<map  id=\"MapEstOrg\">");

            sbCatalogo.AppendLine("<img src=\"../imagens/menu_catalogo.gif\" alt=\"Menu Catálogo\" usemap=\"#MapCatalogo\" />");
            sbCatalogo.AppendLine("<map name=\"MapCatalogo\"  id=\"Map2\">");

            sbEOutros.AppendLine("<img src=\"../imagens/menu_outros.gif\" alt=\"Menu Outros\" usemap=\"#MapOutros\" />");
            sbEOutros.AppendLine("<map id=\"MapOutros\">");

            foreach (var trans in Transacoes)
            {
                if (trans.Caminho.Contains("cadastroOrgao.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"360,8,546,47\" href=\"cadastroOrgao.aspx\" alt=\"Órgão\"/>");

                if (trans.Caminho.Contains("cadastroResponsavel.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"11,388,152,427\" href=\"cadastroResponsavel.aspx\" alt=\"Responsável\" />");

                if (trans.Caminho.Contains("cadastroUO.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"202,95,388,135\" href=\"cadastroUO.aspx\" alt=\"UO\" />");

                if (trans.Caminho.Contains("cadastroAlmoxarifado.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"542,388,683,429\" href=\"cadastroAlmoxarifado.aspx\" alt=\"Almoxarifado\" />");

                if (trans.Caminho.Contains("cadastroCentroCusto.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"10,291,151,332\" href=\"cadastroCentroCusto.aspx\" alt=\"Centrp de Custo\" />");

                if (trans.Caminho.Contains("cadastroDivisao.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"207,388,392,427\" href=\"cadastroDivisao.aspx\" alt=\"Divisão\" />");

                if (trans.Caminho.Contains("cadastroGestor.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"499,92,684,131\" href=\"cadastroGestor.aspx\" alt=\"Gestor\" />");

                if (trans.Caminho.Contains("cadastroUnidade.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"434,290,575,331\" href=\"cadastroUnidade.aspx\" alt=\"Unidade\" />");

                if (trans.Caminho.Contains("cadastroUA.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"203,291,387,331\" href=\"cadastroUA.aspx\" alt=\"UA\" />");

                if (trans.Caminho.Contains("cadastroUGE.aspx"))
                sbEO.AppendLine("<area shape=\"rect\" coords=\"202,195,388,235\" href=\"cadastroUGE.aspx\" alt=\"UGE\" />");

                if (trans.Caminho.Contains("cadastroGrupo.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"236,7,421,47\" href=\"cadastroGrupo.aspx\" alt=\"Grupo\"/>");

                if (trans.Caminho.Contains("cadastroSubItemMaterial.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"236,411,421,451\" href=\"cadastroSubItemMaterial.aspx\" alt=\"Subitem de Material\" />");

                if (trans.Caminho.Contains("cadastroMaterial.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"236,184,421,224\" href=\"cadastroMaterial.aspx\" alt=\"Material\" />");

                if (trans.Caminho.Contains("cadastroContaAuxiliar.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"506,501,691,541\" href=\"cadastroContaAuxiliar.aspx\" alt=\"Conta Auxiliar\" />");

                if (trans.Caminho.Contains("cadastroClasse.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"236,94,421,134\" href=\"cadastroClasse.aspx\" alt=\"Classe\" />");

                if (trans.Caminho.Contains("cadastroItemMaterial.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"236,275,421,315\" href=\"cadastroItemMaterial.aspx\" alt=\"Item de Material\" />");

                if (trans.Caminho.Contains("cadastroUnidadeFornecimento.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"506,411,690,451\" href=\"cadastroUnidadeFornecimento.aspx\" alt=\"Unidade de Fornecimento\" />");

                if (trans.Caminho.Contains("cadastroNaturezaDespesa.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"506,313,691,353\" href=\"cadastroNaturezaDespesa.aspx\" alt=\"Natureza de Despesa\" />");

                if (trans.Caminho.Contains("cadastroRelacaoItemSubItem.aspx"))
                sbCatalogo.AppendLine("<area shape=\"rect\" coords=\"7,344,192,384\" href=\"cadastroRelacaoItemSubItem.aspx\" alt=\"Definir Relação ItemxSubitem\" />");

                if (trans.Caminho.Contains("cadastroMotivoBaixa.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"473,308,658,348\" href=\"cadastroMotivoBaixa.aspx\" alt=\"Sigla\"/>");

                if (trans.Caminho.Contains("cadastroPTRes.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"33,223,218,263\" href=\"cadastroPTRes.aspx\" alt=\"PT_RES\" />");

                if (trans.Caminho.Contains("cadastroFontesRecurso.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"33,93,218,133\" href=\"cadastroFontesRecurso.aspx\" alt=\"Fonte de Recurso\" />");

                if (trans.Caminho.Contains("cadastroSigla.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"473,14,658,54\" href=\"cadastroSigla.aspx\" alt=\"Almoxarifado\" />");

                if (trans.Caminho.Contains("cadastroFornecedor.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"33,8,218,48\" href=\"cadastroFornecedor.aspx\" alt=\"Fornecedor\" />");

                if (trans.Caminho.Contains("cadastroTipoIncorp.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"473,212,658,252\" href=\"cadastroTipoIncorp.aspx\" alt=\"Tipo de Incorporação\" />");

                if (trans.Caminho.Contains("cadastroTerceiro.aspx"))
                sbEOutros.AppendLine("<area shape=\"rect\" coords=\"473,112,658,152\" href=\"cadastroTerceiro.aspx\" alt=\"Terceiro\" />");
            }

            sbEO.AppendLine("</map>");
            sbCatalogo.AppendLine("</map>");
            sbEOutros.AppendLine("</map>");

            MenuFluxoSO.InnerHtml = sbEO.ToString();
            MenuFluxoCatalogo.InnerHtml = sbCatalogo.ToString();
            MenuFluxoOutros.InnerHtml = sbEOutros.ToString();
        }

        protected void btnEstruturaOrganizacional_Click(object sender, ImageClickEventArgs e)
        {
            MenuTabelasPresenter presenter = new MenuTabelasPresenter(this);

            presenter.CarregarEstruturaOrganizacional();
        }

        protected void btnCatalogo_Click(object sender, ImageClickEventArgs e)
        {
            MenuTabelasPresenter presenter = new MenuTabelasPresenter(this);

            presenter.CarregarCatalogo();
        }

        protected void btnOutros_Click(object sender, ImageClickEventArgs e)
        {
            MenuTabelasPresenter presenter = new MenuTabelasPresenter(this);

            presenter.CarregarOutros();
        }

        #region IMenuTabelas Members

        public bool EstruturaOrganizacional
        {            
            set
            {
                //this.conteudoEstruturaOrganizacional.Style.Add(HtmlTextWriterStyle.Display,value ? "" : "none");
            }
        }

        public bool Catalogo
        {            
            set
            {
                //this.conteudoCatalogo.Style.Add(HtmlTextWriterStyle.Display, value ? "" : "none");
                
                
            }
        }

        public bool Outros
        {  
            set
            {
                //this.conteudoOutros.Style.Add(HtmlTextWriterStyle.Display, value ? "" : "none");
            }
        }

        #endregion

        public int Modulo
        {
             get { return 5; }
        }

        public System.Collections.Generic.List<Sam.Entity.Transacao> Transacoes
        {
            get
            {
                return null;
            }

            set
            {
                MontarMenuPerfil(value);

            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Codigo
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Descricao
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void PopularGrid()
        {
            throw new NotImplementedException();
        }

        public void ExibirMensagem(string _mensagem)
        {
            throw new NotImplementedException();
        }

        public IList ListaErros
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaNovo
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaGravar
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaExcluir
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaCancelar
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaCodigo
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaDescricao
        {
            set { throw new NotImplementedException(); }
        }

        public bool MostrarPainelEdicao
        {
            set { throw new NotImplementedException(); }
        }
    }
}
