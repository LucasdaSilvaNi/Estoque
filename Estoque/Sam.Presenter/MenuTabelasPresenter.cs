using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;

namespace Sam.Presenter
{
    public class MenuTabelasPresenter : CrudPresenter<IMenuTabelasView>
    {
        IMenuTabelasView view;
        public MenuTabelasPresenter(IMenuTabelasView _view)
        {
            view = _view;
        }

        public void RetornaTransacoesModulo()
        {
            List<Sam.Entity.Transacao> Transacoes = new List<Sam.Entity.Transacao>();

            foreach (var modulo in Acesso.Transacoes.Perfis[0].Modulos)
            {
                if(modulo.Transacoes != null)
                Transacoes.AddRange(modulo.Transacoes);
            }

            view.Transacoes = Transacoes;
        }

        public void Load()
        {
            view.Catalogo = false;
            view.EstruturaOrganizacional = true;
            view.Outros = false;
        }

        public void CarregarCatalogo()
        {
            view.Catalogo = true;
            view.EstruturaOrganizacional = false;
            view.Outros = false;
        }

        public void CarregarEstruturaOrganizacional()
        {
            view.Catalogo = false;
            view.EstruturaOrganizacional = true;
            view.Outros = false;
        }

        public void CarregarOutros()
        {
            view.Catalogo = false;
            view.EstruturaOrganizacional = false;
            view.Outros = true;
        }
    }
}
