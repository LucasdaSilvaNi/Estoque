using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Entity;
using System.IO;

namespace Sam.Web
{
    public partial class _default : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                VerificaMensagemUsuario();

                if (!User.Identity.IsAuthenticated)
                    Response.Redirect("login.aspx", false);

                string ticket = ((System.Web.Security.FormsIdentity)(User.Identity)).Ticket.UserData;

                var acesso = Cache[ticket] as Acesso;

                if (Convert.ToString(Session["RESET_TABCOUNTER"]) == "1" && acesso != null)
                {
                    Session["RESET_TABCOUNTER"] = "2";
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "initTabCounter", "javascript:initTabCounter();", true);
                }

                if (acesso.Transacoes.TrocarSenha == true)
                {
                    Session["RESET_TABCOUNTER"] = "1";
                    Response.Redirect("~/Seguranca/SEGAlterSenha.aspx", false);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("login.aspx", false);
            }

        }

        private void VerificaMensagemUsuario()
        {
            CarregarNotificacaoUsuario();
        }

        private void CarregarNotificacaoUsuario()
        {
            Presenter.NotificacaoPresenter presenter = new Presenter.NotificacaoPresenter();
            var notificacoes = presenter.CarregarNotificacoesUsuario();

            //Por padrão todas as mensagems estao vazias e desativadas
            pnlMensagemTodos.Visible = false;
            lblTituloMensagemTodos.Text = "";
            lblMensagemTodos.Text = "";

            lblTituloMensagemPerfil.Text = "";
            lblMensagemPerfil.Text = "";
            pnlMensagemPerfil.Visible = false;

            foreach (var notificacao in notificacoes)
            {
                if (notificacao.TB_PERFIL_ID == null) // Todos os perfils
                {
                    lblTituloMensagemTodos.Text = String.Format("{0} - {1}", notificacao.TB_NOTIFICACAO_DATA, notificacao.TB_NOTIFICACAO_TITULO);
                    lblMensagemTodos.Text = notificacao.TB_NOTIFICACAO_MENSAGEM;
                    pnlMensagemTodos.Visible = true;
                }
                if (notificacao.TB_PERFIL_ID != null)
                {
                    lblTituloMensagemPerfil.Text = String.Format("{0} - {1}", notificacao.TB_NOTIFICACAO_DATA, notificacao.TB_NOTIFICACAO_TITULO);
                    lblMensagemPerfil.Text = notificacao.TB_NOTIFICACAO_MENSAGEM;
                    pnlMensagemPerfil.Visible = true;
                }
            }
        }

        //private void CarregarArquivoTxt()
        //{
        //    string appPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;        
        //    string caminhoArquivo = appPath + "mensagemUsuario.txt";

        //    try
        //    {
        //        if(File.Exists(caminhoArquivo))
        //        {
        //            using (System.IO.StreamReader file = new System.IO.StreamReader(caminhoArquivo))
        //            {
        //                string texto = file.ReadToEnd();

        //                if (!String.IsNullOrEmpty(texto))
        //                {
        //                    string[] array = texto.Split(';');

        //                    lblTituloMensagem.Text = String.Format("{0} - {1}", array.GetValue(0).ToString(), array.GetValue(3).ToString());
        //                    lblMensagem.Text = array.GetValue(1).ToString();
        //                    pnlMensagem.Visible = array.GetValue(2).ToString() == "1" ? true : false;
        //                }
        //                else
        //                {
        //                    pnlMensagem.Visible = false;
        //                    lblTituloMensagem.Text = "";
        //                    lblMensagem.Text = "";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //}

    }
}
