using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Common.Util;

namespace Sam.Web.Controles
{
    public partial class PesquisaDocumento : System.Web.UI.UserControl
    {
        private const string sessionMovimento = "sessionMovimento";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void PopularDadosGridDocumento(MovimentoEntity movimento, int pageIndex)
        {
            int tipoMovimentoID = 0;
            int tipoMovAuxiliarID = 0;
            ObjectDataSource obs = new ObjectDataSource();
            MovimentoEntity movTransf = new MovimentoEntity();
            MovimentoPresenter presenter = new MovimentoPresenter();


            if (movimento != null)
                PageBase.SetSession<MovimentoEntity>(movimento, sessionMovimento);
            else
                movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);


            tipoMovimentoID = movimento.TipoMovimento.Id;
            //if(movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
            if ((tipoMovimentoID == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia) || (tipoMovimentoID == (int)GeralEnum.TipoMovimento.SaidaPorDoacao))
            {
                
                // preparar o filtro para transferências já efetuadas
                //ALT 26/10/2015: transferências e doações são tratadas de modo igual
                if (tipoMovimentoID == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
                    tipoMovAuxiliarID = (int)GeralEnum.TipoMovimento.EntradaPorTransferencia;
                else if (tipoMovimentoID == (int)GeralEnum.TipoMovimento.SaidaPorDoacao)
                    tipoMovAuxiliarID = (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado;


                movTransf.TipoMovimento = new TipoMovimentoEntity(tipoMovAuxiliarID);
                movTransf.UGE = new UGEEntity(movimento.UGE.Id);
                movTransf.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(movimento.Almoxarifado.Id);
                movTransf.Almoxarifado = new AlmoxarifadoEntity(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);

                // não filtrar uge para novas entradas por transferência 
                movimento.UGE.Id = 0;
            }

            IList<MovimentoEntity> list = presenter.ListarDocumentoByAlmoxarifadoUge(10, pageIndex, movimento);
            if (movimento.Almoxarifado.Id == 0)
            {
                grdRequisicao.PageSize = 20;
                grdRequisicao.PageIndex = pageIndex;
                grdRequisicao.AllowPaging = true;
                grdRequisicao.DataSource = null;
                grdRequisicao.DataBind();
                return;
            }

            // procura entradas por transferência já processadas (saída por transferência = para as NOVAS transf. por entrada)
            //if (list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
            if ((list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia) || (list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorDoacao))
            {
                IList<MovimentoEntity> listTransf = presenter.ListarDocumentoByAlmoxarifadoUge(10, pageIndex, movTransf);
                
                // compara o documento e remove as transferências já processadas
                foreach (MovimentoEntity mov in listTransf)
                {
                    // por ser SAÍDA, o número do documento é ÚNICO e por isso pode ser restringido dessa forma
                    list = list.Where(a => a.NumeroDocumento != mov.NumeroDocumento).ToList();
                }
            }

            grdRequisicao.PageSize = 20;
            grdRequisicao.PageIndex = pageIndex;
            grdRequisicao.AllowPaging = true;
            grdRequisicao.DataSource = list;
            grdRequisicao.DataBind();
        }

        public void PopularDadosGridDocumento(MovimentoEntity movimento, int pageIndex, bool docsParaEstorno)
        {
            if (!docsParaEstorno)
            {
                PopularDadosGridDocumento(movimento, pageIndex);
                return;
            }

            MovimentoPresenter presenter = new MovimentoPresenter();

            if (movimento != null)
                PageBase.SetSession<MovimentoEntity>(movimento, sessionMovimento);
            else
                movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);

            ObjectDataSource obs = new ObjectDataSource();

            MovimentoEntity movTransf = new MovimentoEntity();

            IList<MovimentoEntity> list = presenter.ListarDocumentoByAlmoxarifadoUge(10, pageIndex, movimento, docsParaEstorno);
            if (movimento.Almoxarifado.Id == 0)
            {
                grdRequisicao.PageSize = 20;
                grdRequisicao.PageIndex = pageIndex;
                grdRequisicao.AllowPaging = true;
                grdRequisicao.DataSource = null;
                grdRequisicao.DataBind();
                return;
            }

            // procura entradas por transferência já processadas (saída por transferência = para as NOVAS transf. por entrada)
            //if (list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
            if ((list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia) || (list != null && movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.SaidaPorDoacao))
            {
                IList<MovimentoEntity> listTransf = presenter.ListarDocumentoByAlmoxarifadoUge(10, pageIndex, movTransf, docsParaEstorno);

                // compara o documento e remove as transferências já processadas
                foreach (MovimentoEntity mov in listTransf)
                {
                    // por ser SAÍDA, o número do documento é ÚNICO e por isso pode ser restringido dessa forma
                    list = list.Where(a => a.NumeroDocumento != mov.NumeroDocumento).ToList();
                }
            }

            grdRequisicao.PageSize = 20;
            grdRequisicao.PageIndex = pageIndex;
            grdRequisicao.AllowPaging = true;
            grdRequisicao.DataSource = list;
            grdRequisicao.DataBind();
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {

        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PopularDadosGridDocumento(null, e.NewPageIndex);
        }
    }
}
