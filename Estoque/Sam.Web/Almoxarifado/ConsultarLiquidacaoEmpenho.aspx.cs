using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using Sam.Domain.Business;
using logErro = Sam.Domain.Business.LogErro;
using Sam.Domain.Business.SIAFEM;


namespace Sam.Web.Almoxarifado
{
    public partial class ConsultarLiquidacaoEmpenho : PageBase, IPostBackEventHandler
    {
        public void RaisePostBackEvent(string eventArgument) { }
        private string chaveEmpenhoParaLiquidar = "empenhoParaLiquidar";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { }

            ListarRelacaoEmpenhos();
        }

        #region Popular Combos

        public void PopularCombo(ListControl listaSelecao, IList listaDados, bool ZerarAoPopularCombo)
        {
            listaSelecao.InserirElementoSelecione(ZerarAoPopularCombo);
            listaSelecao.AppendDataBoundItems = true;
            listaSelecao.DataSource = listaDados;
            listaSelecao.DataBind();
        }

        #endregion

        public void ExibirRelatorio() { throw new NotImplementedException();   }

        public System.Collections.SortedList ParametrosRelatorio { get { throw new NotImplementedException(); } }

        public string Id
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public void PopularGrid() { throw new NotImplementedException(); }

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

        #region Bloquear Controles Movimento

        public bool BloqueiaCancelar
        {
            //set { btnCancelar.Enabled = !value; }
            set { }
        }

        public bool BloqueiaImprimir
        {
            //set { btnImprimir.Enabled = !value; }
            set { }
        }

        public bool BloqueiaAjuda
        {
            //set { btnAjuda.Enabled = !value; }
            set { }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        #endregion

        public bool MostrarPainelEdicao
        {
            //set {  pnlEditar.Visible = value; }
            get;
            set;
        }

        public int OrgaoId { get; set; }

        public string UgeCodigoDescricao
        {
            get;
            set;
        }

        public int? FornecedorId
        {
            get; set;
        }

        public string Empenho 
        {
            get;
            set;
        }


        private void ListarRelacaoEmpenhos(bool empenhosNaoLiquidados = true)
        {
            gvRelacaoEmpenhos.DataSource = null;
            gvRelacaoEmpenhos.DataBind();

            //if (ddlEmpenho.SelectedIndex > 0)
            {
                #region Variaveis
                MovimentoPresenter objPresenter = null;
                IList<MovimentoEntity> lstMovimentacoes = null;
                int almoxID = -1;
                int fornecedorID = -1;
                string anoMesRef = string.Empty;
                string codigoEmpenho = string.Empty;
                #endregion Variaveis

                var almoxLogado = (new PageBase()).GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                objPresenter = new MovimentoPresenter();

                almoxID = almoxLogado.Id.Value;
                anoMesRef = almoxLogado.MesRef;

                //TODO: [DOUGLAS BATISTA]: Carregar lista de movimentos, para o empenho selecionado, que não foram liquidados - OK.
                lstMovimentacoes = objPresenter.ListarMovimentacoesEmpenhoAgrupadas(almoxID, fornecedorID, anoMesRef, codigoEmpenho, empenhosNaoLiquidados);

                gvRelacaoEmpenhos.AllowPaging = false;
                gvRelacaoEmpenhos.DataSource = lstMovimentacoes;
                gvRelacaoEmpenhos.DataBind();
            }
        }

        protected void gvRelacaoEmpenhos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Label lblMovimentoId = null;
            Label lblDescricaoTipoEmpenho = null;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LiquidacaoEmpenhoPresenter objPresenter = new LiquidacaoEmpenhoPresenter();

                MovimentoEntity movEmpenho = null;

                lblMovimentoId = (Label)e.Row.FindControl("lblMovimentoId");
                lblDescricaoTipoEmpenho = (Label)e.Row.FindControl("lblDescricaoTipoEmpenho");
                var _movimentoID = Int32.Parse(lblMovimentoId.Text);

                movEmpenho = objPresenter.ObterMovimento(_movimentoID);
                lblDescricaoTipoEmpenho.Text = movEmpenho.RetornarDescricaoEmpenho();
            }
        }

        protected void gvRelacaoEmpenhos_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            #region Variaveis

            GridViewCommandEventArgs gvceArgs = e;

            #endregion Variaveis

            #region Init

            if (gvceArgs.IsNotNull())
                SetSession<GridViewCommandEventArgs>(e, chaveEmpenhoParaLiquidar);
            else
                gvceArgs = GetSession<GridViewCommandEventArgs>(chaveEmpenhoParaLiquidar);

            #endregion Init

            if (gvceArgs.CommandName == "cmdSelecionarEmpenho")
                Response.Redirect("LiquidacaoEmpenho.aspx", false);

        }
    }
}
