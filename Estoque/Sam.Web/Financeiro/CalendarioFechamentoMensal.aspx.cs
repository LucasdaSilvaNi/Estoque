using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Web.Financeiro;
using Sam.Common.Enums;
using Sam.Common.Enums.CalendarioFechamentoMensalEnums;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;



namespace Sam.Web.Financeiro
{
    /// <summary>
    /// Página criada para visualização das datas de fechamento do SIAFEM, pelos usuários do SAM.
    /// </summary>
    public partial class CalendarioFechamentoMensal : PageBase, ICalendarioFechamentoMensalView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
                objPresenter.Load();
            }

            RegistraJavascript();
        }

        private void RegistraJavascript()
        {
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

            ScriptManager.RegisterStartupScript(this.txtCFM_DataFechamento, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }
        public void PopularGrid()
        {
            IList<CalendarioFechamentoMensalEntity> lstRetorno = null;

            if (ddlAnoFechamento != null && !ddlAnoFechamento.SelectedItem.Text.IsNullOrEmpty())
            {
                CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter();
                lstRetorno = objPresenter.ListarDatasFechamento(int.Parse(ddlAnoFechamento.SelectedItem.Text));
            }

            gdvCalendarioFechamentoMensal.DataSource = lstRetorno;
            gdvCalendarioFechamentoMensal.DataBind();
        }

        public void PopularDdlAno()
        {
            IList<int> lstRetorno = null;

            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter();
            lstRetorno = objPresenter.ListarAnoFechamento();

            ddlAnoFechamento.DataSource = lstRetorno;
            ddlAnoFechamento.DataBind();
        }

        public int? Id
        {
            get
            {
                int? _numValor = null;
                if (Session["rowTabelaID"] != null)
                    _numValor = (Session["rowTabelaID"] as int?);

                return _numValor;
            }
            set
            {
                Session["rowTabelaID"] = value;
            }
        }
        public int AnoReferencia
        {
            get
            {
                int _numRetorno = -1;
                int.TryParse(txtCFM_AnoReferencia.Text, out _numRetorno);

                return _numRetorno;
            }

            set 
            {
                txtCFM_AnoReferencia.Text = value.ToString();
            }
        }
        public byte MesReferencia 
        {
            get
            {
                byte _numRetorno = 0;
                byte.TryParse(ddlCFM_MesReferencia.SelectedValue, out _numRetorno);


                return _numRetorno;
            }

            set { ddlCFM_MesReferencia.SelectedIndex = value; }
        }

        public DateTime DataFechamentoDespesa
        {
            get
            {
                DateTime _dataHora;
                string strDataFechamento = null;

                strDataFechamento = txtCFM_DataFechamento.Text;
                if (!DateTime.TryParse(strDataFechamento, out _dataHora))
                    _dataHora = DateTime.MinValue;

                return _dataHora;
            }

            set
            {
                DateTime _dataHora;
                string strDataFechamento = null;

                strDataFechamento = value.ToString("dd/MM/yyyy");
                if (DateTime.TryParse(strDataFechamento, out _dataHora))
                    txtCFM_DataFechamento.Text = _dataHora.ToString("dd/MM/yyyy");
                else
                    txtCFM_DataFechamento.Text = "Data inválida";
            }
        }

        string ICrudView.Id 
        { 
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        string ICrudView.Descricao
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        string ICrudView.Codigo
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaCodigo
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }
        bool ICrudView.BloqueiaDescricao
        {
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }

        public SortedList ParametrosRelatorio
        {
            get { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
            set { throw new NotImplementedException("Campo/Propriedade não utilizado(a) por esta tela."); }
        }

        public IList ListaErros
        {
            set { this.ListInconsistencias.ExibirLista(value); }
        }

        public bool BloqueiaNovo
        {
            set { this.btnNovo.Enabled = value; }
        }
        public bool BloqueiaGravar
        {
            set { this.btnGravar.Enabled = value; }
        }
        public bool BloqueiaExcluir
        {
            set { this.btnExcluir.Enabled = value; }
        }
        public bool BloqueiaCancelar
        {
            set { this.btnCancelar.Enabled = value; }
        }
        
        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region IEstruturaOrganizacionalView Members
        
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }
        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        #endregion

        protected void gdvCalendarioFechamentoMensal_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indiceRow            = -1;
            Label lblValorCampo      = null;
            string nomeControleCampo = null;
            CalendarioFechamentoMensalPresenter objPresenter = null;

            indiceRow = gdvCalendarioFechamentoMensal.SelectedIndex;

            //ID
            nomeControleCampo = "lnkID";
            if (gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _tmpControle = new LinkButton();
                var _numID = -1;
                _tmpControle = (LinkButton)gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((_tmpControle.IsNotNull() && !String.IsNullOrWhiteSpace(_tmpControle.Text)) && Int32.TryParse(_tmpControle.Text, out _numID))
                    this.Id = _numID;
            }

            //AnoReferencia
            nomeControleCampo = "lblCFM_AnoReferencia";
            if (gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _numAnoReferencia = -1;
                lblValorCampo = (Label)gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)) && Int32.TryParse(lblValorCampo.Text, out _numAnoReferencia))
                    this.AnoReferencia = _numAnoReferencia;
            }

            //AnoReferencia
            nomeControleCampo = "lblCFM_MesReferencia";
            if (gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                int _valorNum;
                lblValorCampo = (Label)gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo);

                if (Int32.TryParse(lblValorCampo.Text, out _valorNum))
                    AnoReferencia = _valorNum;
            }

            //MesReferencia
            nomeControleCampo = "lblCFM_MesReferencia";
            if (gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                MesPorExtenso _valorEnum;// = default(MesPorExtenso);
                lblValorCampo = (Label)gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo);

                _valorEnum = EnumUtils.ParseDescriptionToEnum<MesPorExtenso>(lblValorCampo.Text);
                MesReferencia = (byte)_valorEnum;
            }

            //DataFechamento
            nomeControleCampo = "lblCFM_DataFechamento";
            if (gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _dtDataFechamento = DateTime.MinValue;
                lblValorCampo = (Label)gdvCalendarioFechamentoMensal.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)) && DateTime.TryParse(lblValorCampo.Text, out _dtDataFechamento))
                    this.DataFechamentoDespesa = _dtDataFechamento;
            }

            objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.RegistroSelecionado();
        }

        protected void gdvCalendarioFechamentoMensal_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var gridRow = e.Row;

            if (gridRow.DataItem.IsNotNull() && gridRow.RowType == DataControlRowType.DataRow)
            {
                CalendarioFechamentoMensalEntity datasFechamento = null;

                Label lblCFM_AnoReferencia;
                Label lblCFM_MesReferencia;
                Label lblCFM_DataFechamento;
                LinkButton lnkID;


                datasFechamento              = (CalendarioFechamentoMensalEntity)gridRow.DataItem;
                lnkID                        = (LinkButton)gridRow.FindControl("lnkID");
                lblCFM_AnoReferencia         = (Label)gridRow.FindControl("lblCFM_AnoReferencia");
                lblCFM_MesReferencia         = (Label)gridRow.FindControl("lblCFM_MesReferencia");
                lblCFM_DataFechamento		 = (Label)gridRow.FindControl("lblCFM_DataFechamento");


                int numMes = datasFechamento.MesReferencia;

                if (lnkID.IsNotNull())
                    lnkID.Text = datasFechamento.Id.ToString();

                if (lblCFM_AnoReferencia.IsNotNull())
                    lblCFM_AnoReferencia.Text = datasFechamento.AnoReferencia.ToString();

                if (lblCFM_MesReferencia.IsNotNull() && numMes > 0)
                    lblCFM_MesReferencia.Text = GeralEnum.GetEnumDescription((MesPorExtenso)numMes);

                if (lblCFM_DataFechamento.IsNotNull())
                    lblCFM_DataFechamento.Text = datasFechamento.DataFechamentoDespesa.ToString("dd/MM/yyyy");
            }
        }
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.Novo();
        }
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.Gravar();
        }
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.Excluir();
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.Cancelar();
        }
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            CalendarioFechamentoMensalPresenter objPresenter = new CalendarioFechamentoMensalPresenter(this);
            objPresenter.Imprimir();
            
        }

        protected void ddlAnoFechamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopularGrid();
        }
    }
}
