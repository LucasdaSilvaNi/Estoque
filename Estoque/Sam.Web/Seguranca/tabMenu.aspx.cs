using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using System.Text;

namespace Sam.Web.Seguranca
{
    public partial class tabMenu : System.Web.UI.Page, IMenuTabelasView
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                base.OnInit(e);

                Presenter.MenuTabelasPresenter presenter = new Presenter.MenuTabelasPresenter(this);
                presenter.RetornaTransacoesModulo();
            }
            catch (Exception ex)
            {
                
            }
        }

        public int Modulo
        {
            get
            {
                return 1;
            }
        }

        public bool EstruturaOrganizacional
        {
            set { throw new NotImplementedException(); }
        }

        public bool Catalogo
        {
            set { throw new NotImplementedException(); }
        }

        public bool Outros
        {
            set { throw new NotImplementedException(); }
        }


        public List<Sam.Entity.Transacao> Transacoes
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

        private void MontarMenuPerfil(List<Sam.Entity.Transacao> Transacoes)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<img src=\"../Imagens/menu_seguranca.gif\" alt=\"Menu Segurança\" usemap=\"#MapEstOrg\" />");
            sb.AppendLine("<map name=\"MapEstOrg\" id=\"MapEstOrg\">");

            foreach (var trans in Transacoes)
            {
                if (trans.Caminho.Contains("SEGOrgao.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"282,550,467,590\" href=\"SEGOrgao.aspx\" alt=\"Órgão\" />");

                if (trans.Caminho.Contains("SEGUsuario.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"165,359,350,399\" href=\"SEGUsuario.aspx\" alt=\"Usuário\" />");

                if (trans.Caminho.Contains("SEGCargo.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"49,459,234,499\" href=\"SEGCargo.aspx\" alt=\"Cargo\" />");

                if (trans.Caminho.Contains("SEGGestor.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"280,459,465,499\" href=\"SEGGestor.aspx\" alt=\"Gestor\" />");

                if (trans.Caminho.Contains("SEGPerfilTransacao.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"496,195,681,235\" href=\"SEGPerfilTransacao.aspx\" alt=\"Definir Relação Perfil x Transação\" />");

                if (trans.Caminho.Contains("SEGUsuarioPerfil.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"10,275,195,315\" href=\"SEGUsuarioPerfil.aspx\" alt=\"Definir Relação Usuário x Perfil\" />");

                if (trans.Caminho.Contains("SEGPerfil.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"165,196,350,236\" href=\"SEGPerfil.aspx\" alt=\"Perfil\" />");

                if (trans.Caminho.Contains("SEGTransacao.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"332,97,517,137\" href=\"SEGTransacao.aspx\" alt=\"Transação\" />");

                if (trans.Caminho.Contains("SEGSistema.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"10,97,195,137\" href=\"SEGSistema.aspx\" alt=\"Sistema\" />");

                if (trans.Caminho.Contains("SEGModulo.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"165,10,350,50\" href=\"SEGModulo.aspx\" alt=\"Módulo\" />");

                if (trans.Caminho.Contains("SEGConvenio.aspx"))
                    sb.AppendLine("<area shape=\"rect\" coords=\"496,459,681,499\" href=\"SEGConvenio.aspx\" alt=\"Convênio\" />");

            }
            sb.AppendLine("</map>");
            MenuFluxo.InnerHtml = sb.ToString();
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

        public System.Collections.IList ListaErros
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
