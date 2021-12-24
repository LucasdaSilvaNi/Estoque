using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Sam.Common;
using Sam.Domain.Entity;
using Sam.Entity;
using System.Security.Principal;
using Sam.Web.MasterPage;
using System.Threading;
using System.Globalization;
using System.Net;
using System.Web.UI.WebControls;
using Sam.Common.Util;


namespace Sam.Web
{
    public class PageBase : System.Web.UI.Page
    {
        public static decimal _valorZero = 0.0000m; 

        private Acesso _acesso = null;
        private Int32 _idPerfil = default(Int32);
        private string _urlCompleta = string.Empty;
        private Dictionary<string, bool> _listaTransacoes = new Dictionary<string, bool>();
        ScriptManager _scriptManager = null;

        protected override void OnInit(EventArgs e)
        {
            try
            {

                base.OnInit(e);

                if (DesignMode)
                    return;

                ValidaTickeUsuario();
                AttributesOnClickPressOK();
                ValidaControlesAcesso();
            }
            catch (Exception ex)
            {
                new Presenter.LogErroPresenter().GravarLogErro(ex);
            }
        }

        public PageBase()
        {
            setAsyncProc();
        }

        internal bool setAsyncProc()
        {
            //ScriptManager _scriptManager = ScriptManager.GetCurrent(this);
            _scriptManager = ScriptManager.GetCurrent(this);

            if (_scriptManager.IsNotNull())
                _scriptManager.AsyncPostBackTimeout = 540;

            return (_scriptManager.IsNotNull());
        }

        #region Propriedades

        private string usuario;
        public string Usuario
        {
            get
            {
                string usuarioLogado = string.Empty;
                try
                {
                    usuarioLogado = ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.UserData;
                }
                catch (InvalidCastException excDisparada)
                {
                    throw new Exception("USUARIO NAO LOGADO", excDisparada.InnerException);
    			}

                return usuarioLogado;
            }
			set { usuario = value; }
        }
        public Acesso GetAcesso
        {
            get
            {
                try
                {
                    return (Acesso)HttpContext.Current.Cache[Usuario];
                }
                catch (Exception ex)
                {
                    throw new Exception("USUARIO NAO LOGADO", ex.InnerException);
                }
			}
        }

        public string ChaveImpressaoUsuario
        {
            get
            {
                return String.Format("impressao{0}", ((Acesso)HttpContext.Current.Cache[Usuario]).LoginBase);
            }
        }
        #endregion

        #region Metodos

        public Transacao RetornaTransacao(Acesso acesso)
        {
            string urlTransacao = _urlCompleta.Substring(_urlCompleta.LastIndexOf("/") + 1).ToLower();
            return acesso.Transacoes.Perfis[0].Transacoes.Where(a => a.Caminho.ToUpper().Contains(urlTransacao.ToUpper())).FirstOrDefault();
        }

 
        protected override void InitializeCulture()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
            base.InitializeCulture();
        }

        private void ValidaTickeUsuario()
        {
            //Se não tiver ticket válido
            if (PrincipalFull.Ticket == null)
                Response.Redirect(FormsAuthentication.LoginUrl, false);

            //Tratar erro se não encontrar cache
            try
            {
                _acesso = this.Cache[PrincipalFull.Ticket] as Acesso;
            }
            catch (Exception)
            {
                Response.Redirect(FormsAuthentication.LoginUrl, false);
            }

            //Se acesso não for válido
            if (_acesso == null)
            {
                Response.Redirect(FormsAuthentication.LoginUrl, false);
            }
            else
            {
                //Verifica se o usuário já está logado
                if (Application[_acesso.LoginBase] == null)
                    Response.Redirect(FormsAuthentication.LoginUrl, false);
            }

        }

        private void ValidaControlesAcesso()
        {
            //Autorizar transação
            switch (this.AutorizaTransacao())
            {
                case Enuns.AcessoTransacao.Negado:
                    Response.Redirect(FormsAuthentication.GetRedirectUrl(_acesso.NomeUsuario, true));
                    break;
                case Enuns.AcessoTransacao.Consulta:
                    MostrarControleAcesso(false, "btnNovo");
                    MostrarControleAcesso(false, "btnGravar");
                    MostrarControleAcesso(false, "btnExcluir");
                    Session["acessoBotao"] = false;
                    break;
                case Enuns.AcessoTransacao.Edita:
                    MostrarControleAcesso(true, "btnNovo");
                    MostrarControleAcesso(true, "btnGravar");
                    MostrarControleAcesso(true, "btnExcluir");
                    Session["acessoBotao"] = true;
                    break;
            }

            MostrarControleAcesso(false, "btnAjuda");//desativado para todos
        }

        private void AttributesOnClickPressOK()
        {
            if (!IsPostBack)
            {
                Button btnGravar = ((Button)this.Page.FindControl(string.Format("{0}{1}", "ctl00$cphBody$", "btnGravar")));
                Button btnCancelar = ((Button)this.Page.FindControl(string.Format("{0}{1}", "ctl00$cphBody$", "btnCancelar")));
                Button btnExcluir = ((Button)this.Page.FindControl(string.Format("{0}{1}", "ctl00$cphBody$", "btnExcluir")));

                if (btnGravar != null)
                    btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para salvar.');");

                if (btnCancelar != null)
                    btnCancelar.Attributes.Add("OnClick", "return confirm('Pressione OK para cancelar.');");

                if (btnExcluir != null)
                    btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para excluir.');");
            }
        }

        private void MostrarControleAcesso(bool visivel, string nomeControle)
        {
            try
            {
                Control controle = ((Control)this.Page.FindControl(string.Format("{0}{1}", "ctl00$cphBody$", nomeControle)));
                if (controle != null)
                    controle.Visible = visivel;
            }
            catch (Exception e)
            {
                throw new Exception("Erro validar o acesso ao controle");
            }
        }

        public Enuns.AcessoTransacao AutorizaTransacao()
        {
            Enuns.AcessoTransacao resposta = Enuns.AcessoTransacao.Negado;

            try
            {
                if (String.IsNullOrEmpty(PrincipalFull.Ticket))
                    Response.Redirect("/login.aspx", true);

                _idPerfil = Convert.ToInt32(this.Cache[string.Format("Perfil_{0}", PrincipalFull.Ticket)]);
                _urlCompleta = string.Format("{0}://{1}:{2}{3}"
                                , Request.ServerVariables["HTTPS"] != null
                                            ? Request.ServerVariables["SERVER_PROTOCOL"].ToString().ToLower().Substring(0, 4)
                                            : Request.ServerVariables["SERVER_PROTOCOL"].ToString().ToLower().Substring(0, 5)
                                , Request.ServerVariables["SERVER_NAME"].ToString()
                                , Request.ServerVariables["SERVER_PORT"].ToString()
                                , Request.ServerVariables["SCRIPT_NAME"].ToString());

                _acesso = this.Cache[PrincipalFull.Ticket] as Acesso;

                if (_acesso != null)
                {
                    List<Transacao> transacoes = null;
                    if (_acesso.Transacoes.Perfis.Where(perf => perf.IdPerfil == _idPerfil) != null)
                        transacoes = _acesso.Transacoes.Perfis.Where(perf => perf.IdPerfil == _idPerfil).FirstOrDefault().Transacoes;

                    foreach (var t in transacoes)
                    {
                        t.Caminho = t.Caminho.Substring(t.Caminho.LastIndexOf("/") + 1).ToLower();
                    }

                    string chaveDicionario = _urlCompleta.Substring(_urlCompleta.LastIndexOf("/") + 1).ToLower();
                    var contemTransacao = transacoes.Where(url => url.Caminho == chaveDicionario).ToList();

                    if (contemTransacao.Count().Equals(0))
                        resposta = Enuns.AcessoTransacao.Negado;
                    else if (contemTransacao.FirstOrDefault().Edita)
                        resposta = Enuns.AcessoTransacao.Edita;
                    else
                        resposta = Enuns.AcessoTransacao.Consulta;
                }

                //Se for administrador, não precisa autorizar as páginas
                if (_idPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
                    resposta = Enuns.AcessoTransacao.Edita;

            }
            catch (Exception ex)
            {
                new Presenter.LogErroPresenter().GravarLogErro(ex);
            }

            return resposta;

        }

        private static int sessionCount = 0;
      
        private static int GetSessionCount()
        {
            return sessionCount;
        }

        private static void SetSessionCount()
        {
            sessionCount += 1;
        }

        public static void SetSession<T>(T obj, string sessionName)
        {
            HttpContext.Current.Session.Add(sessionName, obj);
        }

        public static T GetSession<T>(string sessionName)
        {
            if (HttpContext.Current.Session[sessionName] != null)
                return (T)HttpContext.Current.Session[sessionName];
            else
                return default(T);
        }

        public static void RemoveSession(string sessionName)
        {
            HttpContext.Current.Session[sessionName] = null;
            HttpContext.Current.Session.Remove(sessionName);
        }

        public List<OrgaoEntity> ObterOrgaos()
        {
            _acesso = this.Cache[PrincipalFull.Ticket] as Acesso;

            return _acesso.Estruturas.Orgao.ToList();
        }

        public List<UOEntity> ObterUo(int orgaoId)
        {
            _acesso = this.Cache[PrincipalFull.Ticket] as Acesso;

            return _acesso.Estruturas.Uo.Where(t => t.Orgao.Id.Equals(orgaoId)).ToList<UOEntity>();
        }

        public void SetTimeoutProcessamentoAssincrono(int timeoutEmSegundos)
        {
            if(setAsyncProc() && timeoutEmSegundos > 180)
                _scriptManager.AsyncPostBackTimeout = timeoutEmSegundos;
        }

        /// <summary>
        /// Metodo criado para determinar tipo de perfil logado no modulo em uso do SAM
        /// </summary>
        /// <param name="disparaExcecaoSeNulo">Comportamento default, de disparar exception se nao for possivel determinar tipo de perfil.
        /// Para utilizacao sem exception, informar false.</param>
        /// <returns></returns>
        internal int obterPerfilID(bool disparaExcecaoSeNulo = true)
        {
            int perfilID = 0;

            var objAcesso = this.GetAcesso;

            if (objAcesso.IsNotNull())
            {
                var perfilLogado = objAcesso.Transacoes.Perfis[0];
                perfilID = perfilLogado.IdPerfil;
            }
            else if(objAcesso.IsNull() && disparaExcecaoSeNulo)
            {
                throw new Exception("Não foi possível determinar tipo de perfil logado");
            }

            return perfilID;
        }
        #endregion

        #region DualModeView
        public bool FormatoSIAFEM()
        {
            var utilizarFormatoSIAFEM = false;

            try
            {
                if (HttpContext.Current.Cache[Usuario].IsNotNull())
                {
                    var almoxLogado = ((Acesso)HttpContext.Current.Cache[Usuario]).Transacoes.Perfis[0].AlmoxarifadoLogado;
                    utilizarFormatoSIAFEM = (almoxLogado.IsNotNull() && (Int32.Parse(almoxLogado.MesRef) >= Constante.CST_ANO_MES_DATA_CORTE_SAP));
                }
                else
                    utilizarFormatoSIAFEM = true;
            }
            catch (Exception excErroGenerico)
            {
                Sam.Domain.Business.LogErro.GravarMsgInfo("Padrão SIAFEM UI", "Erro ao determinar utilização de padrão SIAFEM para interface gráfica.");
            }
 
            return utilizarFormatoSIAFEM;
        }

        public string fmtValorFinanceiro
        {
            get { return "#,##0.00"; }
        }
        public string fmtFracionarioMaterialQtde
        {
            get
            {
                string strRetorno = null;

                if (this.FormatoSIAFEM())
                    strRetorno = String.Format("#,#0.000");
                else
                    strRetorno = String.Format("#0,0");

                return strRetorno;
            }
        }
        public string fmtFracionarioMaterialValorUnitario
        {
            get
            {
                string strRetorno = null;

                if (this.FormatoSIAFEM())
                    strRetorno = String.Format("#,#0.0000");
                else
                    strRetorno = String.Format("#,#0.00");

                return strRetorno;
            }
        }
        public string fmtDataFormatoBrasileiro
        { get { return "dd/MM/yyyy"; } }
        public string fmtDataHoraFormatoBrasileiro
        { get { return "dd/MM/yyyy HH:mm:ss"; } }
        public int numCasasDecimaisMaterialQtde
        {
            get
            {
                int iRetorno = 0;

                if (this.FormatoSIAFEM())
                    iRetorno = 3;

                return iRetorno;
            }
        }
        public int numCasasDecimaisValorUnitario
        {
            get
            {
                int iRetorno = 2;

                if (this.FormatoSIAFEM())
                    iRetorno = 4;

                return iRetorno;
            }
        }

        #endregion

    }
}
