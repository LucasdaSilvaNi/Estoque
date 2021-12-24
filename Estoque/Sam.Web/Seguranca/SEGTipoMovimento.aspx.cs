using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Web.Seguranca
{
    public partial class SEGTipoMovimento : PageBase, ITipoMovimentoView
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            var presenter = new TipoMovimentoPresenter(this);
            if (!IsPostBack) 
            {
                presenter.LoadData();
                PopularGrid();
                PopularListaTipoMovimentoAgrupamento();
            }
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnCancelar.Attributes.Add("OnClick", "return confirm('Pressione OK para cancelar.');");
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            int tipoMovimentoId = (int)grdTipoMovimento.SelectedDataKey.Value;

            var presenter = new TipoMovimentoPresenter(this);
            var tipoMovimento = presenter.Recupera(tipoMovimentoId);
            tipoMovimento.Ativo = ddlAtivo.SelectedValue.ToString().ToLower() == "true" ? true : false;

            presenter.Gravar(tipoMovimento);
            grdTipoMovimento.DataBind();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnvoltar_Click(object sender, EventArgs e)
        {

        }

        protected void ddlTipoMovimentoAgrupamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter(this);
            presenter.EstadoCancelar();
        }

        protected void grdTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.pnlEditar.Visible = true;
            int tipoMovimentoId = (int)grdTipoMovimento.SelectedDataKey.Value;

            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter(this);
            var tipoMovimentoSelecionado = presenter.Recupera(tipoMovimentoId);

            ddlAtivo.SelectedValue = tipoMovimentoSelecionado.Ativo == true ? "True" : "False";

            presenter.EstadoEdicao();

            ddlAtivo.Focus();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Propriedades

        public void PopularGrid()
        {
            this.grdTipoMovimento.DataSourceID = "sourceGridTipoMovimento";
        }

        public void PopularListaTipoMovimentoAgrupamento()
        {
            ddlTipoMovimentoAgrupamento.DataSource = new List<Infrastructure.TB_TIPO_MOVIMENTO_AGRUPAMENTO>()
            {
                new Infrastructure.TB_TIPO_MOVIMENTO_AGRUPAMENTO() {
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada,
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO = "Entrada"

                },
                new Infrastructure.TB_TIPO_MOVIMENTO_AGRUPAMENTO() {
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida,
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO = "Saída"
                },
                new Infrastructure.TB_TIPO_MOVIMENTO_AGRUPAMENTO() {
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID = (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato,
                    TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO = "Consumo Imediato"
                }
            };
            ddlTipoMovimentoAgrupamento.DataTextField = "TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO";
            ddlTipoMovimentoAgrupamento.DataValueField = "TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID";

            ddlTipoMovimentoAgrupamento.DataBind();
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

        //public bool BloqueiaNovo
        //{
        //    set { btnNovo.Enabled = !value; }
        //}

        public bool BloqueiaGravar
        {
            set { btnGravar.Enabled = !value; }
        }

        public bool BloqueiaExcluir
        {
            set {  }
        }

        public bool BloqueiaCancelar
        {
            set { btnCancelar.Enabled = !value; }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaDescricao
        {
            set {  }
        }

        public bool MostrarPainelEdicao
        {
            set 
            { 
                if(value == true)
                    pnlEditar.CssClass = "mostrarControle";
                else
                    pnlEditar.CssClass = "esconderControle";
            }
        }

        public string Id
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Codigo
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Descricao
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool BloqueiaNovo
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
