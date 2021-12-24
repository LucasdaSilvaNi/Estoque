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
    public partial class cadastroEmpenhoEvento : PageBase, IEmpenhoEventoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
                objPresenter.Load();
            }

            ScriptManager.RegisterStartupScript(this.txEmpenhoEventoCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            ScriptManager.RegisterStartupScript(this.txEmpenhoEventoCodigoEstorno, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set { this.pnlEditar.Visible = value; }
        }

        public void PopularGrid()
        {
            IList<EmpenhoEventoEntity> lstRetorno = null;

            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter();
            lstRetorno = objPresenter.CarregarListaEventos();

            gdvEmpenhoEvento.DataSource = lstRetorno;
            gdvEmpenhoEvento.DataBind();
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
        public int Codigo
        {
            get
            {
                int _codigoEvento = 0;

                Int32.TryParse(txEmpenhoEventoCodigo.Text, out _codigoEvento);

                return _codigoEvento;
            }
            set
            {
                txEmpenhoEventoCodigo.Text = value.ToString("D6");
            }
        }
        public int? CodigoEstorno
        {
            get
            {
                int _codigoEstornoEvento = 0;
                int? _numRetorno = null;

                if (Int32.TryParse(txEmpenhoEventoCodigoEstorno.Text, out _codigoEstornoEvento) && _codigoEstornoEvento > 0)
                    _numRetorno = _codigoEstornoEvento;

                return _numRetorno;
            }
            set
            {
                if (value.HasValue && value.Value > 0)
                    txEmpenhoEventoCodigoEstorno.Text = value.Value.ToString("D6");
                else
                    txEmpenhoEventoCodigoEstorno.Text = null;
            }
        }
        public string Descricao
        {
            get { return txtEmpenhoEventoDescricao.Text; }
            set { txtEmpenhoEventoDescricao.Text = value; }
        }
        public string AnoBase 
        {
            get { return txtEmpenhoEventoAnoBase.Text; }
            set { txtEmpenhoEventoAnoBase.Text = value; }
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
                //TipoMaterial valorEnum = default(TipoMaterial);

                if (ddlTipoMaterial.SelectedIndex >= 0)
                //    valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMaterial>(ddlTipoMaterial.SelectedItem.Text);
                    valorRetorno = ddlTipoMaterial.SelectedIndex;

                //return (int)valorEnum;

                return valorRetorno;
            }

            set
            {
                ListItem itemLista = null;
                var opcoesComboBox = ddlTipoMaterial.Items.Cast<ListItem>().ToList();

                if (opcoesComboBox.HasElements())
                    itemLista = opcoesComboBox.Where(_item => _item.Value == ((value == 0 || value == 1) ? value.ToString() : "-1")).FirstOrDefault();

                if (itemLista.IsNotNull())
                    itemLista.Selected = true;
            }
        }
        public bool Ativo
        {
            get
            {
                bool blnStatus = false;

                //Boolean.TryParse(ddlStatusAtivo.SelectedValue, out blnStatus);
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

        string ICrudView.Id { get; set; }
        string ICrudView.Codigo
        {
            get { return this.Codigo.ToString("D6"); }

            set
            {
                int _numCodigo = -1;

                if (Int32.TryParse(value, out _numCodigo))
                    this.Codigo = _numCodigo;
            }
        }

        public IList ListaErros
        {

            set
            {
                this.ListInconsistencias.ExibirLista(value);
            }
        }

        public bool BloqueiaNovo
        {
            set
            {
                this.btnNovo.Enabled = value;
            }
        }
        public bool BloqueiaGravar
        {
            set
            {
                this.btnGravar.Enabled = value;
            }
        }
        public bool BloqueiaExcluir
        {
            set { }
        }
        public bool BloqueiaCancelar
        {
            set
            {
                this.btnCancelar.Enabled = value;
            }
        }
        public bool BloqueiaCodigo
        {
            set
            {
                this.txEmpenhoEventoCodigo.Enabled = value;
            }
        }
        public bool BloqueiaDescricao
        {
            set
            {
                this.txtEmpenhoEventoDescricao.Enabled = value;
            }
        }

        public SortedList ParametrosRelatorio { get { throw new NotImplementedException("Funcionalidade não-implementada."); } }
        public RelatorioEntity DadosRelatorio { get; set; }

        private IList<TipoMovimentoEntity> obterRelacaoTiposMovimento()
        {
            IList<TipoMovimentoEntity> lstRetorno = null;
            //IList<TipoMovimento> lstTiposMovimentos = new List<TipoMovimento>() { TipoMovimento.AquisicaoCompraEmpenho, TipoMovimento.AquisicaoAvulsa, TipoMovimento.EntradaPorTransferencia, TipoMovimento.EntradaPorDoacao, TipoMovimento.EntradaPorDevolucao, TipoMovimento.MaterialTransformado, TipoMovimento.SaidaPorTransferencia, TipoMovimento.SaidaPorDoação };
            IList<TipoMovimento> lstTiposMovimentos = new List<TipoMovimento>() { TipoMovimento.EntradaPorEmpenho, TipoMovimento.EntradaAvulsa, TipoMovimento.EntradaPorTransferencia, TipoMovimento.EntradaPorDoacaoImplantado, TipoMovimento.EntradaPorDevolucao, TipoMovimento.EntradaPorMaterialTransformado, TipoMovimento.SaidaPorTransferencia, TipoMovimento.SaidaPorDoacao };

            lstRetorno = new List<TipoMovimentoEntity>();
            foreach (var tipoMovimento in lstTiposMovimentos)
                lstRetorno.Add(new TipoMovimentoEntity() { Id = (int)tipoMovimento, Descricao = GeralEnum.GetEnumDescription(tipoMovimento) });
            lstRetorno.Add(new TipoMovimentoEntity(){ Id = 11, Descricao = "Saída Por Requisição"});

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
                
        protected void gdvEmpenhoEvento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indiceRow               = -1;
            Label lblValorCampo = null;
            string nomeControleCampo    = null;


            indiceRow = gdvEmpenhoEvento.SelectedIndex;

            //ID
            nomeControleCampo = "lnkID";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _tmpControle = new LinkButton();
                var _numID = -1;
                _tmpControle = (LinkButton)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((_tmpControle.IsNotNull() && !String.IsNullOrWhiteSpace(_tmpControle.Text)) && Int32.TryParse(_tmpControle.Text, out _numID))
                {
                    this.Id = _numID;
                    ((ICrudView)this).Id = _numID.ToString();
                }

            }

            //Codigo
            nomeControleCampo = "lblCodigoEmpenhoEvento";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _numCodigo = -1;
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)) && Int32.TryParse(lblValorCampo.Text, out _numCodigo))
                    this.Codigo = _numCodigo;
            }

            //Descricao
            nomeControleCampo = "lblDescricaoEmpenhoEvento";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if(lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    this.Descricao = lblValorCampo.Text;
            }

            //AnoBase
            nomeControleCampo = "lblAnoBaseEmpenhoEvento";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if(lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    this.AnoBase = lblValorCampo.Text;
            }

            //CodigoEstorno
            nomeControleCampo = "lblCodigoEstornoEmpenhoEvento";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                var _numCodigoEstorno = -1;
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if ((lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text)) && Int32.TryParse(lblValorCampo.Text, out _numCodigoEstorno))
                    this.CodigoEstorno = _numCodigoEstorno;
            }

            //TipoMovimentoAssociado
            PopularListaTiposMovimento();
            nomeControleCampo = "lblTipoMovimentoAssociado";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                TipoMovimento _valorEnum = default(TipoMovimento);
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    _valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMovimento>(lblValorCampo.Text);

                TipoMovimentoAssociado = new TipoMovimentoEntity() { Id = (int)_valorEnum, Descricao = lblValorCampo.Text };
            }

            //TipoMaterial (Consumo/Permanente)
            nomeControleCampo = "lblTipoMaterial";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                TipoMaterial _valorEnum = default(TipoMaterial);
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    _valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMaterial>(lblValorCampo.Text);

                TipoMaterialAssociado = (int)_valorEnum;
            }

            //Status
            nomeControleCampo = "lblTipoMaterial";
            if (gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo) != null)
            {
                TipoMaterial _valorEnum = default(TipoMaterial);
                lblValorCampo = (Label)gdvEmpenhoEvento.Rows[indiceRow].FindControl(nomeControleCampo);

                if (lblValorCampo.IsNotNull() && !String.IsNullOrWhiteSpace(lblValorCampo.Text))
                    _valorEnum = EnumUtils.ParseDescriptionToEnum<TipoMaterial>(lblValorCampo.Text);

                TipoMaterialAssociado = (int)_valorEnum;
            }
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
            objPresenter.RegistroSelecionado();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {
        }
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
            objPresenter.Novo();
        }
        protected void btnGravar_Click(object sender, EventArgs e)
        {
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
            objPresenter.Gravar();
        }
        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
            objPresenter.Excluir();
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
            objPresenter.Cancelar();
        }
        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            EmpenhoEventoPresenter objPresenter = new EmpenhoEventoPresenter(this);
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
    }
}
