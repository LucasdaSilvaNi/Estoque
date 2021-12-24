using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Web.MasterPage;
using Sam.Presenter;
using Sam.View;
using System.Collections;

namespace Sam.Web
{
    public partial class SelecionarPerfil : System.Web.UI.Page, IUsuarioPerfilView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((Panel)Master.FindControl("pnlBarraGestor")).Visible = false;
                ((Panel)Master.FindControl("pnlUsuario")).Visible = true;
                ((Menu)Master.FindControl("mnSam")).Visible = false;
                ((Panel)Master.FindControl("pnlUsuario")).FindControl("liUsuario").Visible = false;
                ((Panel)Master.FindControl("pnlUsuario")).FindControl("liUData").Visible = false;
                ((Panel)Master.FindControl("pnlUsuario")).FindControl("lisair").Visible = true;

                VerificaAcesso();
            }
        }

        private void VerificaAcesso()
        {
            Sam.Entity.Login _Login = this.Cache[PrincipalFull.Ticket] as Sam.Entity.Login;

            PopularGrid(_Login.Usuario.Id);
        }

        public void PopularGrid(int? UserId)
        {
            LoginPresenter login = new LoginPresenter();
            Sam.Entity.Login Login = new Sam.Entity.Login();
            if (UserId != null)
            {
                Login = login.PopularDadosLoginPorUserId((int)UserId);
            }
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter();

            var list = usu.PopularDadosUsuarioPerfilGrid(Login.ID);
            gridUsuarioPerfil.DataSource = list;
            gridUsuarioPerfil.PageSize = (list.Count() == 0) ? 1 : list.Count();
            gridUsuarioPerfil.DataBind();

        }

        protected void gridUsuarioPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {

            Label lblIdPerfil = (Label)gridUsuarioPerfil.Rows[gridUsuarioPerfil.SelectedIndex].FindControl("lblId");
            Label lblIdLogin = (Label)gridUsuarioPerfil.Rows[gridUsuarioPerfil.SelectedIndex].FindControl("lblIdLogin");
            Label lblIdPerfilLogin = (Label)gridUsuarioPerfil.Rows[gridUsuarioPerfil.SelectedIndex].FindControl("lblIdPerfilLogin");

            if (lblIdPerfilLogin != null)
            {
                this.Id = lblIdPerfilLogin.Text;
                PerfilLoginId = Convert.ToInt32(lblIdPerfilLogin.Text);
            }

            if (lblIdPerfil != null)
                this.PerfilId = Convert.ToInt32(lblIdPerfil.Text);

            if (lblIdLogin != null)
                this.LoginId = Convert.ToInt32(lblIdLogin.Text);

                UsuarioPerfilPresenter presenter = new UsuarioPerfilPresenter(this);

                var LoginNivelAcessoList = new Sam.Facade.FacadePerfil().ListarPerfilLoginNivelAcesso((int)PerfilLoginId);

                if (LoginNivelAcessoList.Count > 0)
                {
                    //Preenche Almox
                    var Almox = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.ALMOXARIFADO);
                    if (Almox.Count() > 0)
                        this.AlmoxId = Almox.FirstOrDefault().Valor;

                    //Preenche Divisao
                    var Divisao = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.DIVISAO);
                    if (Divisao.Count() > 0)
                        this.DivisaoId = Divisao.FirstOrDefault().Valor;

                    //Preenche Gestor
                    var Gestor = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.GESTOR);
                    if (Gestor.Count() > 0)
                        this.GestorId = Gestor.FirstOrDefault().Valor;

                    //Preenche Orgao
                    var Orgao = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.Orgao);
                    if (Orgao.Count() > 0)
                        this.OrgaoId = Orgao.FirstOrDefault().Valor;

                    //Preenche UA
                    var Ua = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.UA);
                    if (Ua.Count() > 0)
                        presenter.View.UaId = Ua.FirstOrDefault().Valor;

                    //Preenche UGE
                    var UGE = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.UGE);
                    if (UGE.Count() > 0)
                        presenter.View.UgeId = UGE.FirstOrDefault().Valor;

                    //Preenche UO
                    var UO = LoginNivelAcessoList.Where(a => a.NivelAcesso.Id == (Byte)Sam.Common.NivelAcessoEnum.UO);
                    if (UO.Count() > 0)
                        presenter.View.UoId = UO.FirstOrDefault().Valor;

                    //Nesse caso sempre será True (Padrão)
                    presenter.View.IsPerfilPadrao = true;

                    presenter.GravarPerfilLogin();

                    VerificaAcesso();

                    Response.Redirect("login.aspx", false);
                }
        }

        protected void gridUsuarioPerfil_RowCommand(object sender, GridViewCommandEventArgs e)
        {


        }

        int? userId = 0;
        public int? UserId
        {
            get { return userId; }

            set
            {
                userId = value;
            }
        }

        int? perfilId = 0;
        public int? PerfilId
        {
            get
            {
                return perfilId;
            }
            set
            {
                perfilId = value;
            }
        }

        int? loginId = 0;
        public int? LoginId
        {
            get
            {
                return loginId;
            }
            set
            {
                loginId = value;
            }
        }

        int? orgaoId = 0;
        public int? OrgaoId
        {
            get
            {
                return orgaoId;
            }
            set
            {
                orgaoId = value;
            }
        }

        int? uoId = 0;
        public int? UoId
        {
            get
            {
                return uoId;
            }
            set
            {
                uoId = value;
            }
        }

        int? ugeId = 0;
        public int? UgeId
        {
            get
            {
                return ugeId;
            }
            set
            {
                ugeId = value;
            }
        }

        int? uaId = 0;
        public int? UaId
        {
            get
            {
                return uaId;
            }
            set
            {
                uaId = value;
            }
        }

        int? divisaoId = 0;
        public int? DivisaoId
        {

            get
            {
                return divisaoId;
            }
            set
            {
                divisaoId = value;
            }
        }

        int? almoxId = 0;
        public int? AlmoxId
        {
            get
            {
                return almoxId;
            }
            set
            {
                almoxId = value;
            }
        }

        int? gestorId = 0;
        public int? GestorId
        {
            get
            {
                return gestorId;
            }
            set
            {
                gestorId = value;
            }
        }

        int? perfilLoginId = 0;
        public int? PerfilLoginId
        {
            get
            {
                return perfilLoginId;
            }
            set
            {
                perfilLoginId = value;
            }
        }

        public bool IsPerfilPadrao
        {
            get
            {
                return true;
            }
            set
            {

            }
        }

        public void ApagarSessaoIDs()
        {

        }

        public List<string> ConsistirPerfil()
        {
            return new List<string>();
        }

        public string Id
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string Codigo
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string Descricao
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public void PopularGrid()
        {
           
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        public bool BloqueiaNovo
        { set { } }

        public bool BloqueiaGravar
        { set { } }

        public bool BloqueiaExcluir
        { set { } }

        public bool BloqueiaCancelar
        { set { } }

        public bool BloqueiaCodigo
        { set { } }

        public bool BloqueiaDescricao
        { set { } }

        public bool MostrarPainelEdicao
        { set { } }
    }
}
