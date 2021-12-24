using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Infrastructure;

namespace Sam.Web.Controles
{
    public partial class PesquisaDocumentoNova : System.Web.UI.UserControl
    {
        //private MovimentoEntity movimento;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblTipoMovimento.Style.Add("display", "none");
            lbltipoOperacao.Style.Add("display", "none");

            if (IsPostBack)
            {
                this.grdDocumento.Visible = false;
            }

        }


        private string tipoOperacao = "";
        public string TipoOperacao
        {
            get
            {
                return tipoOperacao;
            }
            set
            {
                tipoOperacao = value;

                if (tipoOperacao != null)
                    lbltipoOperacao.Text = value.ToString();
            }
        }

        private string tipoMovimento = "";
        public string TipoMovimento
        {
            get
            {
                return tipoMovimento;
            }
            set
            {
                tipoMovimento = value;

                if (tipoMovimento != null)
                    lblTipoMovimento.Text = value.ToString();
            }
        }

        public IList<TB_MOVIMENTO> PesquisaDocumentos(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave, int tipoMovimento, int tipoOperacao)
        {
            var presenter = new MovimentoPresenter();
            

            TB_MOVIMENTO movimento = new TB_MOVIMENTO();
            movimento.TB_TIPO_MOVIMENTO_ID = tipoMovimento;
            if (tipoOperacao == (int)Common.Util.GeralEnum.OperacaoEntrada.Nova && ((tipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia) || (tipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao)))
            {
                var result = presenter.PesquisaDocumentosMesReferencia(maximumRowsParameterName, startRowIndexParameterName, palavraChave, movimento, tipoOperacao);
                this.TotalRegistrosGrid = presenter.TotalRegistrosGrid;
                return result;
            }
            else
            {
                var result = presenter.PesquisaDocumentos(maximumRowsParameterName, startRowIndexParameterName, palavraChave, movimento, tipoOperacao);
                this.TotalRegistrosGrid = presenter.TotalRegistrosGrid;
                return result;
            }
        }

        int TotalRegistrosGrid = 0;
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, string palavraChave, int tipoMovimento, int tipoOperacao)
        {
            return this.TotalRegistrosGrid;
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {
            grdDocumento.DataSourceID = "sourceDocumentos";
            grdDocumento.Visible = true;
        }

        protected void grdDocumento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            grdDocumento.PageIndex = e.NewPageIndex;
            btnProcurar_Click(sender,e);

        //    ParameterCollection parametro = sourceDocumentos.SelectParameters;

        //    int maximumRowsParameterName = Convert.ToInt32(parametro["maximumRowsParameterName"].DefaultValue);
        //    int startRowIndexParameterName=Convert.ToInt32(parametro["startRowIndexParameterName"].DefaultValue);
        //    string palavraChave=parametro["palavraChave"].DefaultValue;
        //    int tipoMovimento=Convert.ToInt32(parametro["tipoMovimento"].DefaultValue);
        //    int tipoOperacao= Convert.ToInt32(parametro["tipoOperacao"].DefaultValue);

        //    PesquisaDocumentos(maximumRowsParameterName,  startRowIndexParameterName, palavraChave, tipoMovimento, tipoOperacao);

        //    //PesquisaDocumentos(null, e.NewPageIndex);
        }

       

    }
}
