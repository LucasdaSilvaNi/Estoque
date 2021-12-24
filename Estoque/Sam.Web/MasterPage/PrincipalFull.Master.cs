using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Entity;
using System.Threading;
using System.Globalization;
using Sam.Presenter;
using Sam.Common;
using Sam.Common.Util;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Web.MasterPage
{
    public partial class PrincipalFull : System.Web.UI.MasterPage
    {
        private Acesso acesso = null;
        protected int timeoutSistema = 60;

        public static string Ticket
        {
            get
            {
                try
                {
                    return ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
        }

        protected void linksair_Click(object sender, EventArgs e)
        {
            sairDoSistema();

        }

        private void UsuarioLogadoRemove(Int32 id)
        {
            UsuarioPresenter usuarioPresenter = new UsuarioPresenter();

            usuarioPresenter.RemoveUsuarioLogadoId(id);

        }

        public void sairDoSistema()
        {
            var IdLoginLogado = Session["IdLoginLogado"] != null ? Convert.ToInt32(Session["IdLoginLogado"].ToString()) : 0;

            if (Session["usuarioCahe"] != null)
            {
                HttpContext.Current.Cache.Remove(Session["usuarioCahe"].ToString());
                RemoverCache(Session["usuarioCahe"].ToString());
            }

            HttpContext.Current.Request.Cookies.Clear();
            HttpContext.Current.Response.Cookies.Clear();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Cache.Remove(string.Format("Perfil_{0}", PrincipalFull.Ticket));
            Session["usuarioCahe"] = null;
            FormsAuthentication.SignOut();
            Session.Abandon();

            UsuarioLogadoEntity usuario = new UsuarioLogadoEntity();
            UsuarioLogadoRemove(IdLoginLogado);

            Response.Redirect(FormsAuthentication.LoginUrl, true);
        }

        private void RemoverCache(string nome)
        {
            Cache.Remove(nome);
            Cache.Remove(string.Format("Perfil_{0}", nome));
            Application.Remove(nome);
            Session.Clear();
            HttpContext.Current.Cache.Remove(nome);
        }

        private void CarregarDados()
        {
            lblMsg.Text = "";

            if (Ticket == null)
                return;

            acesso = HttpContext.Current.Cache[Ticket] as Acesso;

            if (acesso != null)
            {
                CarregarDadosAcesso();
                if (acesso == null)
                {
                    lblMsg.Text = "Houve uma falha no perfil do usuário logado!";
                    return;
                }
                CarregarMenu();
            }
        }

        private void CarregarDadosAcesso()
        {
            string UsuarioLogado = string.Empty; ;
            if (!String.IsNullOrEmpty(acesso.Cpf))
            {
                // lblUserName.Text = acesso.Cpf;
                UsuarioLogado = acesso.Transacoes.Perfis.Count > 0 ? acesso.Transacoes.Perfis[0].Descricao + " " : "";

                //if (acesso.NomeUsuario.Length > 30)
                //    UsuarioLogado = UsuarioLogado + acesso.NomeUsuario.Substring(0, 27) + "...";
                //else
                //    UsuarioLogado = UsuarioLogado + acesso.NomeUsuario;

                UsuarioLogado = UsuarioLogado + acesso.Cpf;
                lblUserLogado.Text = UsuarioLogado;
            }
            lblData.Text = DateTime.Now.ToShortDateString();
            string periodo = string.Empty;

            if (acesso.Transacoes.Perfis.Count() > 0)
            {
                var almoxarifado = acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

                //Atualizar descrição almoxarifado
                if (almoxarifado != null)
                {
                    if (!(acesso.Transacoes.Perfis[0].IdPerfil == (int)TipoPerfil.Requisitante || acesso.Transacoes.Perfis[0].IdPerfil == (int)TipoPerfil.RequisitanteGeral))
                    {
                        MovimentoPresenter mov = new MovimentoPresenter();
                        decimal saldo = mov.ConsultarSaldoTotalAlmoxarifado((int)almoxarifado.Id);
                        decimal saldo33 = mov.ConsultarSaldoTotalAlmoxarifado33((int)almoxarifado.Id);
                        decimal saldo44 = mov.ConsultarSaldoTotalAlmoxarifado44((int)almoxarifado.Id);
                        lblSaldo.Text = string.Format("{0:N2}", saldo);
                        lblSaldo33.Text = string.Format("{0:N2}", saldo33);
                        lblSaldo44.Text = string.Format("{0:N2}", saldo44);
                        pSaldo.Visible = true;
                        pSaldo1.Visible = true;
                        pSaldo2.Visible = true;
                        p1.Visible = true;
                        p2.Visible = true;
                        p3.Visible = true;
                    }
                    else
                        pSaldo.Visible = false;
                        pSaldo1.Visible = true;
                        pSaldo2.Visible = true;
                        p1.Visible = true;
                        p2.Visible = true;
                        p3.Visible = true;

                    if (!string.IsNullOrEmpty(almoxarifado.Descricao))
                        lblAmoxarifado.Text = almoxarifado.Descricao;



                    //Atualizar mês referencia do almoxarifado
                    if (!string.IsNullOrEmpty(almoxarifado.MesRef))
                    {
                        periodo = almoxarifado.MesRef;
                        lblMesAnoRef.Text = string.Format("{0}/{1}", periodo.Substring(4, 2), periodo.Substring(0, 4));
                    }

                    //Atualizar gestor do almoxarifado
                    if (almoxarifado.Gestor != null)
                    {
                        if (!string.IsNullOrEmpty(almoxarifado.Gestor.NomeReduzido))
                            lblGestor.Text = almoxarifado.Gestor.NomeReduzido;

                        Session["OrgaoId"] = almoxarifado.Gestor.Id;
                    }
                    //Codigo da UGE
                    if(almoxarifado.Uge != null)
                    {
                         var codigoUge = almoxarifado.Uge.Codigo;
                         lblUge.Text = codigoUge.Value.ToString();                        
                   } 
                }
                else
                {
                    acesso = null;
                }
            }
        }

        private void CarregarMenu()
        {
            mnSam.Items.Clear();

            string pathApplication = HttpContext.Current.Request.ApplicationPath; //System.Configuration.ConfigurationManager.AppSettings["pathApplication"].ToString();
            pathApplication = pathApplication == "/" ? "" : pathApplication;

            if (acesso.Transacoes.Perfis.Count() > 0)
            {
                foreach (var modulo in acesso.Transacoes.Perfis[0].Modulos)
                {
                    if (modulo.ModuloPaiId == 0 || modulo.ModuloPaiId == null)
                    {
                        int count = 0;
                        MenuItem menuPrincipal = new MenuItem(modulo.Descricao, string.Empty, string.Empty, pathApplication + modulo.Caminho, string.Empty);
                        //Verificar se existe transações para o módulo Pai                                                                                                                                                                                                                                                                                                                                                                          
                        foreach (var subModulo in acesso.Transacoes.Perfis[0].Modulos)
                        {
                            if (subModulo.ModuloPaiId == modulo.ModuloId)
                            {
                                MenuItem subMenu = new MenuItem(subModulo.Descricao);

                                foreach (var trans in subModulo.Transacoes)
                                {

                                    subMenu.ChildItems.Add(new MenuItem(trans.Sigla, string.Empty, string.Empty, pathApplication + trans.Caminho, string.Empty));
                                }

                                menuPrincipal.ChildItems.Add(subMenu);
                                count++;
                            }
                        }

                        if (count == 0)
                        {
                            foreach (var trans in modulo.Transacoes)
                            {
                                if (!trans.Sigla.Contains("."))
                                    menuPrincipal.ChildItems.Add(new MenuItem(trans.Sigla, "", "", pathApplication + trans.Caminho, ""));
                            }
                        }

                        mnSam.Items.Add(menuPrincipal);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // verificarCachUsuario();
            timeoutSistema = Session.Timeout * 60;

            if (Session["usuarioCahe"] != null)
            {
                // Adicionei mais 20 sengundos para o timeout afim de evitar conflito com os outros controles de logoff automático
                Response.AppendHeader("Refresh", String.Concat((Session.Timeout * 60), ";URL=", ResolveClientUrl("~/LogoffAutomatico.aspx"), "?lf=1&si=", Session["usuarioCahe"], "&li=", Session["IdLoginLogado"]));
                ScriptManager.RegisterStartupScript(this, GetType(), "startCountdown", "startCountdown();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "incrementTabCounter", "incrementTabCounter();", true);
                ScriptManager.RegisterStartupScript(this, GetType(), "verificarAbaDuplicada", "verificarAbaDuplicada();", true);
            }

            CarregarDados();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "LogoffUsuario", "javascript:setLogoff(true);", true);
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ResetCronometro", "javascript:resetCronometro(" + timeoutSistema.ToString() + ");", true);
        }
        private void verificarCachUsuario()
        {
            if (Session["usuarioCahe"] != null)
            {
                if (HttpContext.Current.Cache[Session["usuarioCahe"].ToString()] == null)
                {
                    Response.Redirect(ResolveClientUrl("~/login.aspx"));
                }
            }
        }

    }
}
