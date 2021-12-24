using Sam.Entity;
using Sam.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common.Enums;
using Sam.Common;

namespace Sam.Web.Seguranca
{
    public partial class segUsuarioLogado : System.Web.UI.Page
    {
        private UsuarioPresenter presenter = null;
        private Int32 TotalRegistrosGrid;

        public bool BloqueioConsulta
        {
            set { btnConsultar.Enabled = !value; }
        }

        private void SetDataSourceGrid()
        {
            grdLogados.DataSourceID = "logados";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var _perfilPermitido = new List<EnumPerfil> { EnumPerfil.ADMINISTRADOR_GERAL, EnumPerfil.ADMINISTRADOR_ORGAO };
            var _perfilUsuario = (EnumPerfil)int.Parse(Session["usuarioPerfil"].ToString());

            if (!_perfilPermitido.Contains(_perfilUsuario))
            {
                BloqueioConsulta = true;
                ExibirMensagem("Funcionalidade disponível apenas ao perfil Adminstrador Geral e/ou Administrador do Órgão (Nível I)");
                return;
            }

            if (!IsPostBack)
                PopularListaHierarquica();
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            PopularDados();
        }


        protected void grdLogados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Logoff"))
            {
                Int32 loginId = Int32.Parse(e.CommandArgument.ToString());
                presenter = new UsuarioPresenter();

                try
                {
                    presenter.RemoveUsuarioLogadoId(loginId);
                    var nome = this.txtCpf.Text;
                    grdLogados.DataSourceID = "logados";
                    ExibirMensagem("Usuário liberado com sucesso!");
                    Response.Redirect(ResolveUrl("~/Seguranca/SEGUsuarioLogado.aspx"));
                }
                catch (Exception ex)
                {
                    ExibirMensagem(ex.Message);
                }
            }
        }

        public int TotalRegistros(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf)
        {
            return this.TotalRegistrosGrid;
        }

        public IList<UsuarioLogadoEntity> ConsultarLogados(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, int gestorId)
        {
            presenter = new UsuarioPresenter();

            var lista = presenter.ListarTodosUsuariosLogadoPorGestor(gestorId);
            this.TotalRegistrosGrid = presenter.TotalRegistro;

            return (System.Collections.Generic.IList<Sam.Entity.UsuarioLogadoEntity>)lista;
        }

        public IList<UsuarioLogadoEntity> ConsultarLogados(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf, int perfilId = default(int))
        {
            presenter = new UsuarioPresenter();

            var lista = presenter.UsuarioLogado(maximumRowsParameterName, StartRowIndexParameterName, cpf, perfilId);
            this.TotalRegistrosGrid = presenter.TotalRegistro;

            return lista;
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        protected void drpPaginas_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDataSourceGrid();
        }

        protected void grdLogados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var _usuarioLogado = (UsuarioLogadoEntity)e.Row.DataItem;
                ImageButton button = (ImageButton)e.Row.FindControl("imageLogoff");

                if (_usuarioLogado.LoginId != (int)Session["IdLoginLogado"])
                    button.OnClientClick = "return confirm('Tem certeza em liberar o usuário para realizar login no SAM?');";
                else
                    button.Visible = false;
            }
        }

        private void PopularDados()
        {
            var _perfilUsuario = (EnumPerfil)int.Parse(Session["usuarioPerfil"].ToString());
            var _usuarioCache = Session["usuarioCahe"];
            var _acesso = (Acesso)Cache[_usuarioCache.ToString()];

            LimparGrid();
            grdLogados.DataSource = ConsultarLogados(grdLogados.PageSize, 0, txtCpf.Text, _perfilUsuario == EnumPerfil.ADMINISTRADOR_GERAL ? 0 : _acesso.Transacoes.Perfis.FirstOrDefault().GestorPadrao.Id.Value);
            grdLogados.DataBind();
        }

        private void LimparGrid()
        {
            grdLogados.DataSource = null;
            grdLogados.DataBind();
        }

        private void PopularListaHierarquica()
        {
            var _perfilUsuario = (EnumPerfil)int.Parse(Session["usuarioPerfil"].ToString());
            var _usuarioCache = Session["usuarioCahe"];
            var _acesso = (Acesso)Cache[_usuarioCache.ToString()];

            PopularListaHierarquica(new UsuarioPresenter().ListarTodosUsuariosLogadoPorGestor(_perfilUsuario == EnumPerfil.ADMINISTRADOR_GERAL ? 0 : _acesso.Transacoes.Perfis.FirstOrDefault().GestorPadrao.Id.Value));
        }

        private void PopularListaHierarquica(IList<UsuarioLogadoPorGestorEntity> listaUsuarioLogado)
        {
            TreeNode parentNode = null;
            trvUsuarioLogado.Nodes.Clear();

            foreach (var _item in listaUsuarioLogado)
            {
                if (_item == null)
                    return;

                if (parentNode == null || _item.GestorId.Value.ToString() != parentNode.Value)
                {
                    parentNode = new TreeNode(string.Format("{0} ({1})", _item.GestorDescricao, _item.UsuarioLogado.Count), _item.GestorId.Value.ToString());
                    parentNode.Target = "parent";
                }

                TreeNode childNode = null;
                int qtdPerfil = 1;
                foreach (var _usuario in _item.UsuarioLogado.OrderBy(a => a.Login.Perfil.Descricao))
                {
                    if (childNode == null || childNode.Value != _usuario.Login.Perfil.Id.ToString())
                    {
                        if (childNode != null)
                            parentNode.ChildNodes.Add(childNode);

                        qtdPerfil = 1;
                        childNode = new TreeNode(string.Format("{0} ({1})", _usuario.Login.Perfil.Descricao, qtdPerfil), _usuario.Login.Perfil.Id.ToString());
                        childNode.Target = "child";
                    }
                    else
                    {
                        childNode.Text = string.Format("{0} ({1})", _usuario.Login.Perfil.Descricao, qtdPerfil);
                    }

                    qtdPerfil++;
                }

                if (!parentNode.ChildNodes.Contains(childNode))
                    parentNode.ChildNodes.Add(childNode);

                trvUsuarioLogado.Nodes.Add(parentNode);
            }
        }

        protected void trvUsuarioLogado_SelectedNodeChanged(object sender, EventArgs e)
        {
            grdLogados.PageIndex = 0;
            popularGrid();
        }

        protected void grdLogados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grdLogados.PageIndex = e.NewPageIndex;
            popularGrid();
        }

        private void popularGrid()
        {
            presenter = new UsuarioPresenter();
            int _gestorId = int.Parse(trvUsuarioLogado.SelectedNode.Value);
            int _perfilId = 0;

            IList<UsuarioLogadoEntity> _listaUsuarioLogado = null;

            if (trvUsuarioLogado.SelectedNode.Target == "parent")
            {
                arvoreItemSelecionado.InnerText = trvUsuarioLogado.SelectedNode.Text.Substring(0, trvUsuarioLogado.SelectedNode.Text.LastIndexOf('('));
                _listaUsuarioLogado = presenter.ListarTodosUsuariosLogadoPorGestor(_gestorId, 0);
            }
            else
            {
                arvoreItemSelecionado.InnerText = string.Format("{0} => {1}",
                                                                trvUsuarioLogado.SelectedNode.Parent.Text.Substring(0, trvUsuarioLogado.SelectedNode.Parent.Text.Length - 4),
                                                                trvUsuarioLogado.SelectedNode.Text.Substring(0, trvUsuarioLogado.SelectedNode.Text.Length - 4));
                _gestorId = int.Parse(trvUsuarioLogado.SelectedNode.Parent.Value);
                _perfilId = int.Parse(trvUsuarioLogado.SelectedNode.Value);
            }

            _listaUsuarioLogado = presenter.ListarTodosUsuariosLogadoPorGestor(_gestorId, _perfilId);

            grdLogados.DataSource = _listaUsuarioLogado;
            grdLogados.DataBind();
        }
    }
}