using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Common;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class SEGUsuarioPerfil : PageBase, IUsuarioPerfilView
    {
        private UsuarioEntity usuarioEditar = new UsuarioEntity();

        private const string ID_PERFIL_LOGIN = "ID_PERFIL_LOGIN";
        private const string ID_PERFIL = "ID_PERFIL";
        private const string ID_LOGIN = "ID_LOGIN";

        #region Métodos

        private void RecuperarUsuarioSessao()
        {
            usuarioEditar = GetSession<UsuarioEntity>("usuarioEditado");
        }

        public void ApagarSessaoIDs()
        {
            RemoveSession(ID_PERFIL_LOGIN);
            RemoveSession(ID_PERFIL);
            RemoveSession(ID_LOGIN);

        }

        public List<string> ConsistirPerfil()
        {
            List<string> lst = new List<string>();

            if (this.ddlPerfil.Visible == true)
            {
                if (ddlPerfil.SelectedIndex <= 0)
                    lst.Add("Informar o perfil.");
            }

            if (this.ddlOrgao.Visible == true)
            {
                if (ddlOrgao.SelectedIndex <= 0)
                    lst.Add("Informar o órgão.");
            }

            if (this.ddlGestor.Visible == true)
            {
                if (ddlGestor.SelectedIndex <= 0)
                    lst.Add("Informar o gestor.");
            }

            if (this.ddlAlmoxarifado.Visible == true)
            {
                if (ddlAlmoxarifado.SelectedIndex <= 0)
                    lst.Add("Informar o almoxarifado.");
            }

            if (this.ddlDivisao.Visible == true)
            {
                if (ddlDivisao.SelectedIndex <= 0)
                    lst.Add("Informar a divisão.");
            }

            return lst;
        }

        public void EscondeDdl(bool Sn)
        {
            this.lblOrgao.Visible = Sn;
            this.ddlOrgao.Visible = Sn;
            this.lblGestor.Visible = Sn;
            this.ddlGestor.Visible = Sn;
            this.lblUo.Visible = Sn;
            this.ddlUo.Visible = Sn;
            this.lblUge.Visible = Sn;
            this.ddlUge.Visible = Sn;
            this.lblUa.Visible = Sn;
            this.ddlUa.Visible = Sn;
            this.lblAlmoxarifado.Visible = Sn;
            this.ddlAlmoxarifado.Visible = Sn;
            this.lblDivisao.Visible = Sn;
            this.ddlDivisao.Visible = Sn;
        }

        private void CarregarInformacoesUsuarioEditado()
        {
            if (usuarioEditar != null && usuarioEditar.Id != null)
            {
                this.lblNomeUsuario.Text = String.Format("Nome: {0} - CPF: {1}", usuarioEditar.Nome, usuarioEditar.Cpf);
            }
        }

        public void PopularGrid()
        {
            LoginPresenter login = new LoginPresenter();
            Sam.Entity.Login Login = new Sam.Entity.Login();
            if (UserId != null)
            {
                Login = login.PopularDadosLoginPorUserId((int)UserId);
            }
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);

            var list = usu.PopularDadosUsuarioPerfilGrid(Login.ID);
            gridUsuarioPerfil.DataSource = list;
            gridUsuarioPerfil.PageSize = (list.Count() == 0) ? 1 : list.Count();
            gridUsuarioPerfil.DataBind();


        }

        protected void inicializarCombos(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("- Selecione -");
            ddl.Items[0].Value = "0";
            ddl.AppendDataBoundItems = true;
        }

        private bool FiltrarPorAlmoxarifadoLogado()
        {
            if (ddlPerfil.SelectedValue == ((int)Sam.Common.Perfil.REQUISITANTE).ToString())
            {

                if (new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL ||
                    new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void PopularOrgao()
        {
            OrgaoPresenter org = new OrgaoPresenter();
            inicializarCombos(ddlOrgao);
            ddlOrgao.DataSource = org.PopularDadosTodosCod();
            ddlOrgao.DataBind();
            // limpar UO, UGE, UA, DIVISÃO
            inicializarCombos(ddlUo);
            inicializarCombos(ddlUge);
            inicializarCombos(ddlUa);
            inicializarCombos(ddlDivisao);
        }

        public void PopularUo()
        {
            UOPresenter uo = new UOPresenter();
            inicializarCombos(ddlUo);
            ddlUo.DataSource = uo.PopularDadosTodosCod(OrgaoId.Value, FiltrarPorAlmoxarifadoLogado());
            ddlUo.DataBind();
            // limpar UGE, UA, DIVISÃO
            inicializarCombos(ddlUge);
            inicializarCombos(ddlUa);
            inicializarCombos(ddlDivisao);
        }

        public void PopularUge()
        {
            UGEPresenter uge = new UGEPresenter();
            inicializarCombos(ddlUge);
            inicializarCombos(ddlDivisao);
            ddlUge.DataSource = uge.PopularDadosTodosCodPorUo(UoId.Value, FiltrarPorAlmoxarifadoLogado());
            ddlUge.DataBind();
            // limpar UA, DIVISÃO
            inicializarCombos(ddlUa);
            inicializarCombos(ddlDivisao);

        }

        public void PopularUa()
        {
            UAPresenter ua = new UAPresenter();
            inicializarCombos(ddlUa);
            inicializarCombos(ddlDivisao);
            ddlUa.DataSource = ua.PopularDadosTodosCodPorUge(UgeId, FiltrarPorAlmoxarifadoLogado());
            ddlUa.DataBind();
        }

        public void PopularDivisao()
        {
            DivisaoPresenter div = new DivisaoPresenter();
            inicializarCombos(ddlDivisao);
            ddlDivisao.DataSource = div.ListarDivisaoByUALogin((int)UaId, (int)GestorId).ToList();
            ddlDivisao.DataBind();
        }

        public void PopularAlmoxarifado()
        {
            inicializarCombos(ddlAlmoxarifado);

            string nivelAcessoDesc = "ALMOXARIFADO";//Campo Identity no banco
            var nivelAcesso = new Sam.Facade.FacadePerfil().ListarPerfilLoginNivelAcesso(new PageBase().GetAcesso.Transacoes.Perfis[0].IdLogin, nivelAcessoDesc);
            var almoxarifados = new AlmoxarifadoPresenter().AlmoxarifadoTodosCod();
            var idPerfil = new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            if (idPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL || idPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_ORGAO)
            {
                //Gestor ID
                ddlAlmoxarifado.DataSource = almoxarifados.Where(a => a.Gestor.Id == GestorId.Value).ToList();
                ddlAlmoxarifado.DataBind();
            }
            else
            {
                //Pega do nivel de acesso
                List<AlmoxarifadoEntity> almoxList = new List<AlmoxarifadoEntity>();
                foreach (var almox in almoxarifados)
                {
                    if (nivelAcesso.Where(a => a.AlmoxId == almox.Id).ToList().Count > 0)
                        almoxList.Add(almox);
                }

                ddlAlmoxarifado.DataSource = almoxList;
                ddlAlmoxarifado.DataBind();
            }
        }

        private void PopularUaByUgeId(string UgeId)
        {
            int indiceItemsDdlUge = 1;
            do
            {
                this.UgeId = ddlUge.Items[indiceItemsDdlUge].Value == "0" ? 0 : Convert.ToInt32(ddlUge.Items[indiceItemsDdlUge].Value);
                //PopularUa();
                indiceItemsDdlUge++;
            }
            while (this.UgeId == null || this.UgeId == 0);
        }

        private void PopularDivisaoByUaId(string UaId)
        {
            int indiceItemsDdlUa = 1;
            int quantidadeDeUAs = ddlUa.Items.Count - 1;
            do
            {
                this.UaId = ddlUa.Items[indiceItemsDdlUa].Value == "0" ? 0 : Convert.ToInt32(ddlUa.Items[indiceItemsDdlUa].Value);

                if (ddlDivisao.Items.Count > 1)
                    this.DivisaoId = ddlDivisao.Items[1].Value == "0" ? 0 : Convert.ToInt32(ddlDivisao.Items[1].Value);
                indiceItemsDdlUa++;

            } while ((ddlDivisao.Items.Count <= 1 && indiceItemsDdlUa <= quantidadeDeUAs)
                            && (this.DivisaoId == null || this.DivisaoId == 0));

        }
                
        private void PopularCombosRequisitanteGeral()
        {
            List<String> listaErros = new List<String>() {"É necessário configurar Divisões antes de atribuir chave Requisitante Geral."};

            if (ddlUge.Items.Count <= 1)
            {
                this.ExibirMensagem("Inconsistências encontradas.");
                this.ListaErros = listaErros;
                return;
            }
            PopularUaByUgeId(ddlUge.Items[1].Value);

            if (ddlUa.Items.Count <= 1)
            {
                this.ExibirMensagem("Inconsistências encontradas.");
                this.ListaErros = listaErros;
                return;
            }
            ddlUge.Enabled = false;
            PopularDivisaoByUaId(ddlUa.Items[1].Value);

            if (ddlDivisao.Items.Count <= 1)
            {
                this.ExibirMensagem("Inconsistências encontradas.");
                this.ListaErros = listaErros;
                return;
            }
            ddlUa.Enabled = false;
            ddlDivisao.Enabled = false;
        }

        private void BloquearCombosRequisitanteGeral(bool enableDisable)
        {
            ddlUge.Enabled = enableDisable;
            ddlUa.Enabled = enableDisable;
            ddlDivisao.Enabled = enableDisable;
        }

        // Alteração solicitada que bloqueia o perfil, caso o usuario já tenho um perfil associado.        
        private void BloquearDDLPerfil()
        {
            if (gridUsuarioPerfil.Rows.Count > 0)
            {
                string perfilId = ((Label)gridUsuarioPerfil.SelectedRow.FindControl("lblId")).Text;

                if (!string.IsNullOrEmpty(perfilId))
                {
                    ddlPerfil.SelectedValue = perfilId;
                    ddlPerfil.Enabled = false;
                }
                else
                {
                    ddlPerfil.Enabled = true;
                }

            }
        }

        public void PopularPerfil()
        {
            PerfilPresenter perfil = new PerfilPresenter();
            inicializarCombos(ddlPerfil);

            ddlPerfil.DataSource = perfil.PopularDadosPerfil((int)new PageBase().GetAcesso.Transacoes.Perfis[0].Peso);
            ddlPerfil.DataBind();
        }

        public void PopularGestor()
        {
            GestorPresenter gestor = new GestorPresenter();
            inicializarCombos(ddlGestor);
            if (OrgaoId > 0)
            {
                ddlGestor.DataSource = gestor.PopularDadosGestorTodosCod(OrgaoId.Value);
                ddlGestor.DataBind();
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
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }


        private void PrepararCamposPerfilNivel()
        {
            IList<PerfilNivel> nv = new List<PerfilNivel>();
            nv = new UsuarioPerfilPresenter().PopularPerfilNivel(Convert.ToInt32(this.ddlPerfil.SelectedItem.Value));

            foreach (var perfilNivel in nv)
            {
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.GESTOR)
                {
                    this.lblGestor.Visible = true;
                    this.ddlGestor.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.Orgao)
                {
                    this.lblOrgao.Visible = true;
                    this.ddlOrgao.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.UO)
                {
                    this.lblUo.Visible = true;
                    this.ddlUo.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.UGE)
                {
                    this.lblUge.Visible = true;
                    this.ddlUge.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.UA || perfilNivel.NivelId == (int)NivelAcessoEnum.UA_GESTOR)
                {
                    this.lblUa.Visible = true;
                    this.ddlUa.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.DIVISAO || perfilNivel.NivelId == (int)NivelAcessoEnum.DIVISAO_UA)
                {
                    this.lblDivisao.Visible = true;
                    this.ddlDivisao.Visible = true;
                }
                if (perfilNivel.NivelId == (int)NivelAcessoEnum.ALMOXARIFADO)
                {
                    this.lblAlmoxarifado.Visible = true;
                    this.ddlAlmoxarifado.Visible = true;
                }
            }
        }

        private void ClearSelectionDDLs()
        {
            ddlOrgao.ClearSelection();
            ddlGestor.ClearSelection();
            ddlUo.ClearSelection();
            ddlUge.ClearSelection();
            ddlUa.ClearSelection();
            ddlDivisao.ClearSelection();
            ddlAlmoxarifado.ClearSelection();
        }

        #endregion

        #region Propriedades

        //Id do usuário editado, buscado na sessao
        public int? UserId
        {
            get
            {
                if (usuarioEditar != null)
                    return usuarioEditar.Id;
                else
                    return null;
            }
        }

        // usará TB_PERFIL_LOGIN_ID
        public string Id
        {
            get
            {
                string retorno = null;
                if (Session[ID_PERFIL_LOGIN] != null)
                    retorno = Session[ID_PERFIL_LOGIN].ToString();
                return retorno;
            }
            set
            {
                Session[ID_PERFIL_LOGIN] = value;
            }
        }

        public int? PerfilLoginId
        {
            get
            {
                string retorno = null;
                if (Session[ID_PERFIL_LOGIN] != null)
                    retorno = Session[ID_PERFIL_LOGIN].ToString();
                return Convert.ToInt32(retorno);
            }
            set
            {
                Session[ID_PERFIL_LOGIN] = value;
            }
        }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public bool IsPerfilPadrao
        {
            get
            {
                return chPadrao.Checked;
            }
            set
            {
            }
        }

        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }

        public bool BloqueiaGravar
        {
            set { btnGravar.Enabled = !value; }
        }

        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }

        public bool BloqueiaExcluir { set { } }

        public bool BloqueiaCodigo { set { } }

        public bool BloqueiaDescricao { set { } }

        public bool MostrarPainelEdicao
        {
            set
            {
                pnlEditar.Visible = value;
            }
        }

        public int? OrgaoId
        {
            get { return TratamentoDados.TryParseInt32(ddlOrgao.SelectedValue); }
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlOrgao.ClearSelection();
                    item.Selected = true;
                    PopularUo();
                    PopularGestor();
                }
            }
        }

        public int? UoId
        {
            get { return TratamentoDados.TryParseInt32(ddlUo.SelectedValue); }
            set
            {
                ListItem item = ddlUo.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUo.ClearSelection();
                    item.Selected = true;
                    PopularUge();                    
                    if (this.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                        PopularCombosRequisitanteGeral();
                }
            }
        }

        public int? UgeId
        {
            get { return TratamentoDados.TryParseInt32(ddlUge.SelectedValue); }
            set
            {
                ListItem item = ddlUge.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUge.ClearSelection();
                    item.Selected = true;
                    PopularUa();
                }
            }
        }

        public int? UaId
        {
            get { return TratamentoDados.TryParseInt32(ddlUa.SelectedValue); }
            set
            {
                ListItem item = ddlUa.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUa.ClearSelection();
                    item.Selected = true;
                    PopularDivisao();
                }
            }
        }

        public int? GestorId
        {
            get { return TratamentoDados.TryParseInt32(ddlGestor.SelectedValue); }
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlGestor.ClearSelection();
                    item.Selected = true;
                    PopularDivisao();
                    PopularAlmoxarifado();
                }
            }
        }

        public int? AlmoxId
        {
            get
            {

                if (ddlAlmoxarifado.Items.Count > 0)
                {
                    return TratamentoDados.TryParseInt32(ddlAlmoxarifado.SelectedValue);
                }
                else
                    return 0;

            }
            set
            {
                ListItem item = ddlAlmoxarifado.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAlmoxarifado.ClearSelection();
                    item.Selected = true;
                    PopularDivisao();
                }
            }
        }

        public int? DivisaoId
        {
            get { return TratamentoDados.TryParseInt32(ddlDivisao.SelectedValue); }
            set
            {
                ListItem item = ddlDivisao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlDivisao.ClearSelection();
                    item.Selected = true;

                }
            }
        }

        public int? PerfilId
        {
            get { return TratamentoDados.TryParseInt32(ddlPerfil.SelectedValue); }
            set
            {
                ListItem item = ddlPerfil.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlPerfil.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public int? LoginId
        {
            get
            {
                string retorno = null;
                if (Session[ID_LOGIN] != null)
                    retorno = Session[ID_LOGIN].ToString();
                return Convert.ToInt32(retorno);
            }
            set
            {
                Session[ID_LOGIN] = value;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);

            //Recupera a sessao do usuário Editado
            if (usuarioEditar.Id == null)
                RecuperarUsuarioSessao();

            CarregarInformacoesUsuarioEditado();

            if (!IsPostBack)
            {
                if (usuarioEditar != null && usuarioEditar.Id != null)
                {
                    PopularGrid();
                }

                usu.Load();
                PopularOrgao();
                PopularPerfil();
            }

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usuarioPresenter = new UsuarioPerfilPresenter(this);
            LoginPresenter loginPresenter = new LoginPresenter();

            //Retorna os dados do Login atualizado
            usuarioPresenter.Login = loginPresenter.PopularDadosLoginPorUserId((int)UserId);

            if (usuarioPresenter.Login.Id != null)
            {
                LoginId = usuarioPresenter.Login.Id; //Atualiza a view LoginId

                if (ddlPerfil.SelectedValue != "0" && Convert.ToInt32(ddlPerfil.SelectedValue) == Sam.Common.Perfil.REQUISITANTE_GERAL  && !usuarioPresenter.PermitidoGravarRequisitanteGeral((int)UserId, (int)this.UoId))
                    return;
                
                usuarioPresenter.GravarPerfilLogin();
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
            usu.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
            usu.Cancelar();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
            usu.Novo();
            ClearSelectionDDLs();

            ddlPerfil.SelectedIndex = 0;
            ddlPerfil_SelectedIndexChanged(sender, e);
            ddlPerfil.Enabled = true;
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
            usu.Imprimir();
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUo();
            PopularGestor();
        }

        protected void ddlUo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUge();

            if (this.PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL) 
                PopularCombosRequisitanteGeral();
        }

        protected void ddlUge_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUa();
        }

        protected void ddlUa_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularDivisao();
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

            UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
            usu.LerRegistro(this.LoginId, Convert.ToInt32(this.Id), Convert.ToInt32(this.PerfilId));

            chPadrao.Checked = Convert.ToBoolean(((Label)gridUsuarioPerfil.SelectedRow.FindControl("lblIsAlmoxarifadoPadrao")).Text);

            //ddlPerfil_SelectedIndexChanged(sender, e);
            EscondeDdl(false);
            PrepararCamposPerfilNivel();            
            BloquearCombosRequisitanteGeral(true);

            BloquearDDLPerfil();
        }

        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularAlmoxarifado();
            PopularDivisao();
        }

        protected void ddlAlmoxarifado_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularDivisao();
        }

        protected void gridUsuarioPerfil_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                int index = int.Parse((string)e.CommandArgument);
                GridViewRow row = gridUsuarioPerfil.Rows[index];
                Label lblId = (Label)gridUsuarioPerfil.Rows[index].FindControl("lblId");
                Label lblIdPerfilLogin = (Label)gridUsuarioPerfil.Rows[index].FindControl("lblIdPerfilLogin");

                //Captura o PerfilLoginID Selecionado no grid
                if (!String.IsNullOrEmpty(lblIdPerfilLogin.Text))
                    PerfilLoginId = Convert.ToInt32(lblIdPerfilLogin.Text);
                else
                    PerfilLoginId = null;

                //Captura o PerfilId Selecionado no grid
                if (!String.IsNullOrEmpty(lblId.Text))
                    PerfilId = Convert.ToInt32(lblId.Text);
                else
                    PerfilId = null;
            }

            if (e.CommandName == "excluir")
            {
                int _perfilLoginId = int.Parse((string)e.CommandArgument);
                UsuarioPerfilPresenter usu = new UsuarioPerfilPresenter(this);
                LoginPresenter loginPresenter = new LoginPresenter();


                if (UserId != null)
                {
                    int userId = Convert.ToInt32(UserId);
                    usu.Login = loginPresenter.PopularDadosLoginPorUserId(userId);
                }

                usu.Perfil.PerfilLoginId = _perfilLoginId;
                usu.Excluir();
            }

        }

        protected void gridUsuarioPerfil_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton l = (LinkButton)e.Row.FindControl("lnkExcluir");
                l.Attributes.Add("onclick", "javascript:return " +
                "confirm('Confirma a desassociação?')");
            }
        }

        protected void ddlPerfil_SelectedIndexChanged(object sender, EventArgs e)
        {
            EscondeDdl(false);
            PrepararCamposPerfilNivel();            
            //Chamadas para Desbloquear e Limpar seleÃƒÂ§ão.
            BloquearCombosRequisitanteGeral(true);
            PopularOrgao();

        }

        protected void ddlDivisao_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
