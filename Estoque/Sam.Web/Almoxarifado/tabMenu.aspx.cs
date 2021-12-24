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

namespace Sam.Web.Almoxarifado
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
            get
            {
                return 5;
            }
        }

        private void MontarMenuPerfil(List<Sam.Entity.Transacao> Transacoes)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<img src=\"../imagens/menu_operacoes.gif\" alt=\"Menu Almoxarifado\" usemap=\"#MapAlmox\" />");
            sb.AppendLine("<map name=\"MapAlmox\" id=\"MapAlmox\">");

            foreach (var trans in Transacoes)
            {
                if (trans.Caminho.Contains("LiquidacaoEmpenho.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"378,8,562,47\" href=\"LiquidacaoEmpenho.aspx\" alt=\"Liquidação de Empenho\" />");

                if (trans.Caminho.Contains("EntradaMaterial.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"144,7,327,46\" href=\"EntradaMaterial.aspx\" alt=\"Entrada de Material\" />");

                if (trans.Caminho.Contains("selecionaAlmoxarifado.aspx"))
                sb.AppendLine("<area shape=\"circle\" coords=\"351,179,71\" href=\"selecionaAlmoxarifado.aspx\" alt=\"Selecionar Almoxarifado\" />");

                if (trans.Caminho.Contains("ReservaMaterial.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"11,205,197,245\" href=\"ReservaMaterial.aspx\" alt=\"Reserva de Material\" />");

                if (trans.Caminho.Contains("FechamentoMensal.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"262,350,446,390\" href=\"FechamentoMensal.aspx\" alt=\"Fechamento Mensal\" />");

                if (trans.Caminho.Contains("SaidaMaterial.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"501,160,686,199\" href=\"SaidaMaterial.aspx\" alt=\"Saída de Material\" />");

                if (trans.Caminho.Contains("GerenciaCatalogo.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"33,351,219,390\" href=\"GerenciaCatalogo.aspx\" alt=\"Gerência do Catálogo\" />");

                if (trans.Caminho.Contains("Consultas.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"501,351,686,392\" href=\"Consultas.aspx\" alt=\"Consultas\" />");

                if (trans.Caminho.Contains("RelatoriosMensais.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"263,450,448,489\" href=\"RelatoriosMensais.aspx\" alt=\"Relatórios Mensais\" />");

                if (trans.Caminho.Contains("ConsultarRequisicaoMaterial.aspx"))
                sb.AppendLine("<area shape=\"rect\" coords=\"11,123,197,163\" href=\"RequisicaoMaterial.aspx\" alt=\"Requisição de Material\" />");

            }
            sb.AppendLine("</map>");
            MenuFluxo.InnerHtml = sb.ToString();

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
