using Sam.Presenter;
using Sam.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text.RegularExpressions;
using Sam.Common.Util;

namespace Sam.Web.Seguranca
{
    public partial class SEGESP : PageBase, IESPView
    {
        #region [ Implementar métodos Interface ]

        #region [ Bloqueio ]

        #region [ BloqueiaCancelar ]
        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }
        #endregion

        #region [ BloqueiaCodigo ]
        public bool BloqueiaCodigo
        {
            set { }
        }
        #endregion

        #region [ BloqueiaDescricao ]
        public bool BloqueiaDescricao
        {
            set { }
        }
        #endregion

        #region [ BloqueiaExcluir ]
        public bool BloqueiaExcluir
        {
            set { btnExcluir.Enabled = !value; }
        }
        #endregion

        #region [ BloqueiaGravar ]
        public bool BloqueiaGravar
        {
            set { btnGravar.Enabled = !value; }
        }
        #endregion

        #region [ BloqueiaNovo ]
        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }
        #endregion

        #endregion

        #region [ Atributos ]

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string Id { get; set; }

        public int ID
        {
            get { return Int32.Parse(hdnEspID.Value); }
            set { hdnEspID.Value = value.ToString(); }
        }

        public int EspCodigo
        {
            get { return TratamentoDados.ParseIntNull(txtNumESP.Text); }
            set { txtNumESP.Text = value.ToString(); }
        }

        public string EspSistema
        {
            get { return ddlModulo.SelectedValue; }
            set
            {
                if (ddlModulo.Items.FindByValue(value) != null)
                    ddlModulo.SelectedValue = value;
                else
                    ddlModulo.SelectedIndex = 0;
            }
        }

        public int GestorId
        {
            get { return Int32.Parse(ddlGestor.SelectedValue); }
            set
            {
                if (ddlGestor.Items.FindByValue(value.ToString()) != null)
                    ddlGestor.SelectedValue = value.ToString();
                else
                    ddlGestor.SelectedIndex = 0;
            }
        }

        public DateTime? DataInicioVigencia
        {
            get { return TratamentoDados.TryParseDateTime(tbIniVigencia.Text); }
            set { tbIniVigencia.Text = value.HasValue ? Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro) : null; }
        }

        public DateTime? DataFimVigencia
        {
            get { return TratamentoDados.TryParseDateTime(tbFimVigencia.Text); }
            set { tbFimVigencia.Text = value.HasValue ? Convert.ToDateTime(value.ToString()).ToString(base.fmtDataFormatoBrasileiro) : null; }
        }

        public int QtdeRepositorioPrincipal
        {
            get { return TratamentoDados.ParseIntNull(txtQtdeRepoPrinc.Text); }
            set { txtQtdeRepoPrinc.Text = value.ToString(); }
        }

        public int QtdeRepositorioComplementar
        {
            get { return TratamentoDados.ParseIntNull(txtQtdeRepoCompl.Text); }
            set { txtQtdeRepoCompl.Text = value.ToString(); }
        }

        public int QtdeUsuarioNivelI
        {
            get { return TratamentoDados.ParseIntNull(txtQtdeUserI.Text); }
            set { txtQtdeUserI.Text = value.ToString(); }
        }

        public int QtdeUsuarioNivelII
        {
            get { return TratamentoDados.ParseIntNull(txtQtdeUserII.Text); }
            set { txtQtdeUserII.Text = value.ToString(); }
        }

        public byte TermoId
        {
            get { return TratamentoDados.ParseByteNull(txtNumTermo.Text); }
            set { txtNumTermo.Text = value.ToString(); }
        }

        public enTipoOperacao OPERACAO
        {
            get { return (enTipoOperacao)Enum.Parse(typeof(enTipoOperacao), hdnTipoOperacao.Value, true); }
            set { hdnTipoOperacao.Value = value.ToString(); }
        }
        #endregion

        #region [ ListaErros ]
        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }
        #endregion

        #region [ MostrarPainelEdicao ]
        public bool MostrarPainelEdicao
        {
            set
            {
                pnlEditar.CssClass = (value == true) ? "mostrarControle" : "esconderControle";
            }
        }
        #endregion

        #region [ ExibirMensagem ]
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }
        #endregion

        #region [ PopularGrid ]
        public void PopularGrid()
        {
            // this.grdESP.DataSourceID = "sourceGridESP";
            var presenter = new ESPPresenter(this);
            grdESP.DataSource = presenter.ListarESP(0, 20);
            grdESP.DataBind();
        }
        #endregion

        #region [ PopularGestor ]
        public void PopularGestor()
        {
            GestorPresenter gestor = new GestorPresenter();
            ddlGestor.Items.Clear();

            ddlGestor.DataSource = gestor.PopularDadosGestorTodos();
            ddlGestor.DataBind();
        }
        #endregion

        #endregion

        #region [ Page Load ]
        protected void Page_Load(object sender, EventArgs e)
        {
            ESPPresenter presenter = new ESPPresenter(this);
            ScriptManager.RegisterStartupScript(this.tbIniVigencia, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.tbFimVigencia, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);

            if (!IsPostBack)
            {
                presenter.Load();
                PopularGrid();
                this.BloqueiaNovo = false;
            }

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnCancelar.Attributes.Add("OnClick", "return confirm('Pressione OK para cancelar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para excluir.');");
        }
        #endregion

        #region [ Botões de Ação ]

        #region [ btnGravar_Click ]
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ESPPresenter esp = new ESPPresenter(this, OPERACAO);
            esp.Gravar();
        }
        #endregion

        #region [ btnNovo_Click ]
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ESPPresenter esp = new ESPPresenter(this);
            esp.Novo();
            PopularGestor();
            hdnTipoOperacao.Value = enTipoOperacao.Insert.ToString();
        }
        #endregion

        #region [ btnCancelar_Click ]
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ESPPresenter esp = new ESPPresenter(this);
            esp.Cancelar();
        }
        #endregion

        #region [ btnExcluir_Click ]
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ESPPresenter esp = new ESPPresenter(this);
            esp.Excluir();
        }
        #endregion

        #region [ btnImprimir_Click ]
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ESPPresenter esp = new ESPPresenter(this);
            //perfil.Imprimir();
        }
        #endregion

        #region [ btnAjuda_Click ]
        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region [ btnvoltar_Click ]
        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion

        protected void grdESP_SelectedIndexChanged(object sender, EventArgs e)
        {
            int _linha = grdESP.SelectedIndex;

            this.Id = obterTextoLabel(this.obterCampoLinhaSelecionada(_linha, "lblId"));

            hdnTipoOperacao.Value = enTipoOperacao.Update.ToString();

            PopularGestor();
            ESPPresenter _espPresenter = new ESPPresenter(this);
            _espPresenter.RegistroSelecionado();
        }

        private Label obterCampoLinhaSelecionada( int linha, string campo)
        {
            return (Label)grdESP.Rows[linha].FindControl(campo);
        }

        private string obterTextoLabel(Label label)
        {
            return label.Text;
        }

        protected void PopularDados()
        {
            var presenter = new ESPPresenter(this);
            grdESP.DataSource = presenter.ListarESP(0, 20);
            grdESP.DataBind();
        }
    }
}