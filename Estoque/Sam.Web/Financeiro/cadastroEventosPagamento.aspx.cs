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
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;


namespace Sam.Web.Financeiro
{
    /// <summary>
    /// Página criada para controle de eventos que serão vinculados a cada tipo de movimentação, no SAM, para maior integração contábil/financeira com o SIAFEM.
    /// </summary>
    public partial class cadastroEventosPagamento : PageBase, IEventosPagamentoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
                objPresenter.Load();
            }

            ScriptManager.RegisterStartupScript(this.txtPrimeiroCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            ScriptManager.RegisterStartupScript(this.txtPrimeiroCodigoEstorno, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            ScriptManager.RegisterStartupScript(this.txtSegundoCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            ScriptManager.RegisterStartupScript(this.txtSegundoCodigoEstorno, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set { this.pnlEditar.Visible = value; }
        }

        public void PopularGrid()
        {
            IList<EventosPagamentoEntity> lstRetorno = null;

            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();
            lstRetorno = objPresenter.CarregarListaEventos(ddlAnoFechamento.SelectedItem.Text);

            gdvEventosPagamento.DataSource = lstRetorno;
            gdvEventosPagamento.DataBind();
        }

        public void PopularDdlAno()
        {
            IList<string> lstRetorno = null;

            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();
            lstRetorno = objPresenter.CarregarAnoListaEventos();

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
        public int PrimeiroCodigo
        {
            get
            {
                int _codigoEvento = 0;

                Int32.TryParse(txtPrimeiroCodigo.Text, out _codigoEvento);

                return _codigoEvento;
            }
            set
            {
                txtPrimeiroCodigo.Text = value.ToString("D6");
            }
        }
        public int? PrimeiroCodigoEstorno
        {
            get
            {
                int _codigoEstornoEvento = 0;
                int? _numRetorno = null;

                if (Int32.TryParse(txtPrimeiroCodigoEstorno.Text, out _codigoEstornoEvento) && _codigoEstornoEvento > 0)
                    _numRetorno = _codigoEstornoEvento;

                return _numRetorno;
            }
            set
            {
                if (value.HasValue && value.Value > 0)
                    txtPrimeiroCodigoEstorno.Text = value.Value.ToString("D6");
                else
                    txtPrimeiroCodigoEstorno.Text = null;
            }
        }
        public string PrimeiraInscricao
        {
            get { return ddlPrimeiraInscricao.SelectedItem.Value; }
            set
            {
                ListItem itemLista = ddlPrimeiraInscricao.Items.FindByValue(value.ToUpperInvariant());

                if (itemLista.IsNotNull())
                {
                    ddlPrimeiraInscricao.ClearSelection();
                    itemLista.Selected = true;
                }
            }
        }
        public string PrimeiraClassificacao
        {
            get { return txtPrimeiraClassificacao.Text; }
            set { txtPrimeiraClassificacao.Text = value; }
        }
        public int SegundoCodigo
        {
            get
            {
                int _codigoEvento = 0;

                Int32.TryParse(txtSegundoCodigo.Text, out _codigoEvento);

                return _codigoEvento;
            }
            set
            {
                txtSegundoCodigo.Text = value.ToString("D6");
            }
        }
        public int? SegundoCodigoEstorno
        {
            get
            {
                int _codigoEstornoEvento = 0;
                int? _numRetorno = null;

                if (Int32.TryParse(txtSegundoCodigoEstorno.Text, out _codigoEstornoEvento) && _codigoEstornoEvento > 0)
                    _numRetorno = _codigoEstornoEvento;

                return _numRetorno;
            }
            set
            {
                if (value.HasValue && value.Value > 0)
                    txtSegundoCodigoEstorno.Text = value.Value.ToString("D6");
                else
                    txtSegundoCodigoEstorno.Text = null;
            }
        }
        public string SegundaClassificacao
        {
            get { return txtSegundaClassificacao.Text; }
            set { txtSegundaClassificacao.Text = value; }
        }
        public string SegundaInscricao
        {
            //get { return txtSegundaInscricao.Text; }
            //set { txtSegundaInscricao.Text = value; }
            get { return ddlSegundaInscricao.SelectedItem.Value; }
            set 
            {
                ListItem itemLista = ddlSegundaInscricao.Items.FindByValue(value.ToUpperInvariant());

                if (itemLista.IsNotNull())
                {
                    ddlSegundaInscricao.ClearSelection();
                    itemLista.Selected = true;
                }
            }
        }

        public string AnoBase 
        {
            get { return txtAnoBase.Text; }
            set { txtAnoBase.Text = value; }
        }
        public TipoMovimentoEntity TipoMovimentoAssociado
        {
            get
            {
                TipoMovimentoEntity objEntidade = null;

                if(ddlTipoMovimentoAssociado.SelectedIndex > 0)
                    objEntidade = new TipoMovimentoEntity() { Id = Int32.Parse(ddlTipoMovimentoAssociado.SelectedItem.Value), Descricao = ddlTipoMovimentoAssociado.SelectedItem.Text };

                return objEntidade;
            }

            set 
            {
                ListItem itemLista = null;
                var opcoesComboBox = ddlTipoMovimentoAssociado.Items.Cast<ListItem>().ToList();

                if (opcoesComboBox.HasElements())
                    itemLista = opcoesComboBox.Where(_item => _item.Value == (value.IsNotNull() ? value.Id.ToString() : "-1")).FirstOrDefault();

                if (itemLista.IsNotNull())
                    itemLista.Selected = true;
            }
        }
        public int TipoMaterialAssociado
        {
            get
            {
                int valorRetorno = -1;
                TipoMaterial valorEnum = default(TipoMaterial);

                if (ddlTipoMaterial.SelectedIndex >= 0)
                    valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMaterial>(ddlTipoMaterial.SelectedItem.Text);
                //    valorRetorno = ddlTipoMaterial.SelectedIndex;

                return (int)valorEnum;

                //return valorRetorno;
            }

            set
            {
                ListItem itemLista = null;
                var opcoesComboBox = ddlTipoMaterial.Items.Cast<ListItem>().ToList();

                if (opcoesComboBox.HasElements())
                    itemLista = opcoesComboBox.Where(_item => _item.Value == ((value == 2 || value == 3) ? value.ToString() : "-1")).FirstOrDefault();

                if (itemLista.IsNotNull())
                    itemLista.Selected = true;
            }
        }
        public bool Ativo
        {
            get
            {
                bool blnStatus = false;

                blnStatus = (ddlStatusAtivo.SelectedValue == "1");

                return blnStatus;
            }
            set
            {
                if (value == true)
                    ddlStatusAtivo.SelectedValue = "1";
                else
                    ddlStatusAtivo.SelectedValue = "0";
            }
        }
        public bool UGFavorecida
        {
            get
            {
                bool blnStatus = false;

                blnStatus = (ddlUGFavorecida.SelectedValue == "1");

                return blnStatus;
            }
            set
            {
                if (value == true)
                    ddlUGFavorecida.SelectedValue = "1";
                else
                    ddlUGFavorecida.SelectedValue = "0";
            }
        }

        string ICrudView.Id { get; set; }
        string ICrudView.Codigo { get; set; }
        string ICrudView.Descricao { get; set; }
        bool ICrudView.BloqueiaDescricao { set { } }

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
            set { }
        }
        public bool BloqueiaCancelar
        {
            set { this.btnCancelar.Enabled = value; }
        }
        public bool BloqueiaCodigo
        {
            set { this.txtPrimeiroCodigo.Enabled = this.txtSegundoCodigo.Enabled = value; }
        }

        public SortedList ParametrosRelatorio { get { throw new NotImplementedException("Funcionalidade não-implementada."); } }
        public RelatorioEntity DadosRelatorio { get; set; }

        private IList<TipoMovimentoEntity> obterRelacaoTiposMovimento()
        {
            IList<TipoMovimentoEntity> lstRetorno = null;
            TipoMovimentoPresenter presenterTipoMovimentacao = null;

            presenterTipoMovimentacao = new TipoMovimentoPresenter();
            lstRetorno = presenterTipoMovimentacao.Listar();

            return lstRetorno;
        }
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
                
        protected void gdvEventosPagamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indiceRow = -1;
            int _numCodigo, _numCodigoEstorno = -1;
            string _valor01, _valor02;
            bool _registroAtivo;


            Label lblValorCampo = null;
            string nomeControleCampo = null;



            indiceRow = gdvEventosPagamento.SelectedIndex;
            //ID
            nomeControleCampo = "lnkID";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _tmpControle = new LinkButton();
                var _numID = -1;
                _tmpControle = (LinkButton)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((_tmpControle.IsNotNull() && !String.IsNullOrWhiteSpace(_tmpControle.Text)) && Int32.TryParse(_tmpControle.Text, out _numID))
                {
                    this.Id = _numID;
                    ((ICrudView)this).Id = _numID.ToString();
                }
            }

            //PrimeiroCodigo/PrimeiroCodigoEstorno
            nomeControleCampo = "lblPrimeiroCodigo_PrimeiroCodigoEstorno";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)))
                {
                    Int32.TryParse(lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0], out _numCodigo);
                    Int32.TryParse(lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[1], out _numCodigoEstorno);

                    this.PrimeiroCodigo = _numCodigo;
                    this.PrimeiroCodigoEstorno = _numCodigoEstorno;
                }
            }

            //PrimeiraInscricao/PrimeiraClassificacao
            nomeControleCampo = "lblPrimeiraInscricao_PrimeiraClassificacao";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)))
                {
                    this.PrimeiraInscricao = lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    this.PrimeiraClassificacao = lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }

            //SegundoCodigo/SegundoCodigoEstorno
            nomeControleCampo = "lblSegundoCodigo_SegundoCodigoEstorno";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)))
                {
                    Int32.TryParse(lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0], out _numCodigo);
                    Int32.TryParse(lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[1], out _numCodigoEstorno);

                    this.SegundoCodigo = _numCodigo;
                    this.SegundoCodigoEstorno = _numCodigoEstorno;
                }
            }

            //SegundaInscricao/SegundaClassificacao
            nomeControleCampo = "lblSegundaInscricao_SegundaClassificacao";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {

                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)))
                {
                    this.SegundaInscricao = lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    this.SegundaClassificacao = lblValorCampo.Text.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }

            //AnoBase
            nomeControleCampo = "lblAnoBase";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    this.AnoBase = lblValorCampo.Text;
            }

            //TipoMovimentoAssociado
            PopularListaTiposMovimento();
            nomeControleCampo = "lblTipoMovimentoAssociado";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                TipoMovimento _valorEnum = default(TipoMovimento);
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    _valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMovimento>(lblValorCampo.Text);

                TipoMovimentoAssociado = new TipoMovimentoEntity() { Id = (int)_valorEnum, Descricao = lblValorCampo.Text };
            }


            //TipoMaterial (Consumo/Permanente)
            nomeControleCampo = "lblTipoMaterial";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                TipoMaterial _valorEnum = default(TipoMaterial);
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text) && Int32.TryParse(lblValorCampo.Text, out _numCodigo))
                    _valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMaterial>(lblValorCampo.Text);

                    TipoMaterialAssociado = (int)_valorEnum;
            }

            //Status
            nomeControleCampo = "lblUGFavorecida";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text) && (lblValorCampo.Text.ToLowerInvariant() == "Sim".ToLowerInvariant() || lblValorCampo.Text.ToLowerInvariant() == "Não".ToLowerInvariant()))
                    UGFavorecida = (lblValorCampo.Text.ToLowerInvariant() == "Sim".ToLowerInvariant() ? true : false);
            }

            //UG Favorecida
            nomeControleCampo = "lblAtivo";
            if (gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEventosPagamento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text) && (lblValorCampo.Text.ToLowerInvariant() == "Sim".ToLowerInvariant() || lblValorCampo.Text.ToLowerInvariant() == "Não".ToLowerInvariant()))
                    Ativo = (lblValorCampo.Text.ToLowerInvariant() == "Sim".ToLowerInvariant() ? true : false);

            }

            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.RegistroSelecionado();
        }

        protected string RetornaDescricaoTipoMaterial(int tipoMaterialID)
        {
            return GeralEnum.GetEnumDescription((TipoMaterial)tipoMaterialID);
        }

        protected string RetornaVerdadeiroOuFalso(bool valorBooleano)
        {
            return ((valorBooleano) ? "Sim" : "Não");
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {
        }
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.Novo();
        }
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.Gravar();
        }
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.Excluir();
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.Cancelar();
        }
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter(this);
            objPresenter.Imprimir();
            
        }

        public void PopularListaTiposMovimento()
        {
            ddlTipoMovimentoAssociado.DataSource = obterRelacaoTiposMovimento();
            ddlTipoMovimentoAssociado.DataValueField = "Id";
            ddlTipoMovimentoAssociado.DataTextField = "Descricao";
            ddlTipoMovimentoAssociado.DataBind();


            ddlTipoMovimentoAssociado.InserirElementoSelecione();
        }

        protected void ddlAnoFechamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }
    }
}
