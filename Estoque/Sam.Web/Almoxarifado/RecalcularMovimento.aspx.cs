using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Almoxarifado
{
    public partial class RecalcularMovimento : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this);
            scriptManager.AsyncPostBackTimeout = 36000;

            btnProcessar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        protected void btnProcessar_Click(object sender, EventArgs e)
        {
            MovimentoPresenter presenter = new MovimentoPresenter();

            try
            {
                int contador = presenter.CorrigirSaldo();
                ExibirMensagem(String.Format("Saldo corrigido com sucesso. {0} registro(s) alterado(s).", contador));
            }
            catch(Exception ex)
            {
                var list = new List<String>();
                list.Add(ex.Message);
                
                ListaErros = list;
            }
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public IList ListaErros
        {
            set
            {
                ListInconsistencias.ExibirLista(value);
            }
        }
    }
}
