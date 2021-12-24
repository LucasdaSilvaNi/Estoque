using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;
using Sam.Infrastructure;

namespace Sam.Web.Almoxarifado
{
    public partial class ConsultarEntradaMaterial : PageBase, IConsultarEntradaView
    {
        private MovimentoEntity movimento = new MovimentoEntity();
        private string sessionListMovimento = "listMovimento";
        private string idDocumentoEdit = "idDocumentoEdit";        
        private int novoDocumento = 0;
        
        #region Metodos

        private void RegistraJavaScript()
        {
            ScriptManager.RegisterStartupScript(this.txtDataEmissao, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtDataEmissaoFinal, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtDataRecebimentoFinal, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtDataRecebimentoInicial, GetType(), "numerico", "$('.numerico').floatnumber(',',2);", true);
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        private void EstornarEntrada(int controleId)
        {
            ConsultarEntradaMaterialPresenter presenter = new ConsultarEntradaMaterialPresenter(this);
            MovimentoEntity movimento = new MovimentoEntity();

            //Seta o Id do movimento selecionado no grid
            movimento.Id = controleId;

            //Executa o Estorno do movimento
            presenter.Estornar(movimento);
        }

        private void ImprimirNotaFornecimento(int controleId)
        {
            ConsultarEntradaMaterialPresenter presenter = new ConsultarEntradaMaterialPresenter(this);
            SortedList paramList = new SortedList();

            paramList.Add("MovimentoId", controleId);

            ParametrosRelatorio = paramList;

            presenter.Imprimir(ParametrosRelatorio);
            ExibirRelatorio();

        }

        public SortedList paramList;
        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                return paramList;
            }

            set
            {
                paramList = value;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public void PopularDadosUGETodosCod()
        {
            // carregar apenas as uges do perfil.
            ddlUge.Items.Clear();
            ddlUge.Items.Add("- Selecione -");
            ddlUge.Items[0].Value = "0";
            ddlUge.AppendDataBoundItems = true;

            UGEPresenter uge = new UGEPresenter();
            ddlUge.DataSource = uge.PopularDadosTodosCodPorGestor(1);            
            ddlUge.DataBind();
        }

        public void PopularListaTipoMovimentoEntrada() 
        { 
            // carregar apenas as uges do perfil.
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add("- Selecione -");
            ddlTipo.Items[0].Value = "0";
            ddlTipo.AppendDataBoundItems = true;

            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter();
            ddlTipo.DataSource = presenter.PopularListaTipoMovimentoEntrada();
            ddlTipo.DataBind();
        }

        #endregion

        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            RegistraJavaScript();

            RemoveSession(idDocumentoEdit);
            RemoveSession(sessionListMovimento);

            if (!IsPostBack)
            {
                PopularDadosUGETodosCod();
                PopularListaTipoMovimentoEntrada();
            }
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            this.PopularGrid();
        }

        protected void grdDocumentos_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                ((ImageButton)e.Row.Cells[5].FindControl("imgImprimirNota")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
                ((ImageButton)e.Row.Cells[6].FindControl("imgEditar")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
                ((ImageButton)e.Row.Cells[7].FindControl("imgEstornar")).Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            }
        }

        protected void grdDocumentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Imprimir")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.ImprimirNotaFornecimento(controleId);
            }
            if (e.CommandName == "Editar")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.EditarEntrada(controleId);
                return;
            }
            if (e.CommandName == "Estornar")
            {
                int controleId = Convert.ToInt32(e.CommandArgument.ToString());
                this.EstornarEntrada(controleId);
                this.PopularGrid();
            }
        }

        private void EditarEntrada(int controleId)
        {
            RemoveSession(sessionListMovimento);
            SetSession<int>(controleId, idDocumentoEdit);
            Response.Redirect(String.Format("EntradaMaterial.aspx"), false);
        }

        protected void grdDocumentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                var objMesRef = DataBinder.Eval(e.Row.DataItem, "TB_MOVIMENTO_ANO_MES_REFERENCIA");
                if (objMesRef.ToString() != new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef)
                {
                    ((ImageButton)e.Row.Cells[7].FindControl("imgEstornar")).ImageUrl = "~/Imagens/Delete2_Desab.png";
                    ((ImageButton)e.Row.Cells[7].FindControl("imgEstornar")).Enabled = false;
                }

                var objAtivo = DataBinder.Eval(e.Row.DataItem, "TB_MOVIMENTO_ATIVO");
                if (objAtivo != null)
                {
                    bool ativo = (bool)objAtivo;

                    if (ativo == false)
                    {
                        //Foi Estornado                        
                        ((ImageButton)e.Row.Cells[6].FindControl("imgEditar")).Enabled = false;
                        ((ImageButton)e.Row.Cells[7].FindControl("imgEstornar")).Enabled = false;
                    }
                }
            }

        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            RemoveSession(sessionListMovimento);
            SetSession<int>(novoDocumento, idDocumentoEdit);
            Response.Redirect(String.Format("EntradaMaterial.aspx"), false);
        }

        protected void btnSair_Click(object sender, EventArgs e)
        {

        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }       

        #endregion

        #region Propriedades

        public new int ID
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

        public int ALMOXARIFADO_ID
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

        public int TIPO_MOVIMENTO_ID
        {
            get
            {
                return Convert.ToInt32(ddlTipo.SelectedValue);
            }
        }

        public string MOVIMENTO_NUMERO_DOCUMENTO
        {
            get
            {
                return txtDocumento.Text.Trim();                
            }
        }

        public int MOVIMENTO_ANO_MES_REFERENCIA
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

        public DateTime? MOVIMENTO_DATA_DOCUMENTO
        {
            get
            {
                var date = Common.Util.TratamentoDados.TryParseDateTime(txtDataEmissao.Text);

                if (date == null)
                    return DateTime.MinValue;
                else
                    return date;
            }
        }

        public DateTime? MOVIMENTO_DATA_MOVIMENTO
        {
            get
            {
                var date = Common.Util.TratamentoDados.TryParseDateTime(txtDataRecebimentoInicial.Text);

                if (date == null)
                    return DateTime.MinValue;
                else
                    return date;
            }
        }

        public DateTime? MOVIMENTO_DATA_DOCUMENTO_ATE
        {
            get
            {
                var date = Common.Util.TratamentoDados.TryParseDateTime(txtDataEmissaoFinal.Text);

                if (date == null)
                    return DateTime.MaxValue;
                else
                    return date;
            }
        }

        public DateTime? MOVIMENTO_DATA_MOVIMENTO_ATE
        {
            get
            {
                var date = Common.Util.TratamentoDados.TryParseDateTime(txtDataRecebimentoInicial.Text);

                if (date == null)
                    return DateTime.MaxValue;
                else
                    return date;
            }
        }

        public string EMPENHO_COD
        {
            get
            {
                return txtEmpenho.Text.Trim();
            }
        }

        public int? UGE_ID
        {
            get
            {
                return Common.Util.TratamentoDados.TryParseInt32(ddlUge.SelectedValue);
            }

        }

        public string Id
        {
            get
            {
                string retorno = null;
                if (Session["ID"] != null)
                    retorno = Session["ID"].ToString();
                return retorno;
            }
            set
            {
                Session["ID"] = value;
            }

        }

        public string Codigo
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        public string Descricao
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        public void PopularGrid()
        {
            ConsultarEntradaMaterialPresenter presenter = new ConsultarEntradaMaterialPresenter(this);
            var result = presenter.ConsultarMovimentos(grdDocumentos.PageIndex, 10);

            grdDocumentos.DataSource = result;
            grdDocumentos.DataBind();
            //grdDocumentos.PageSize = presenter.TotalRegistrosGrid;

        }

        public bool Estornado
        {
            get
            {
                if(ddlEstornados.SelectedValue == "1")
                    return true;
                else
                    return false;
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

        public bool BloqueiaNovo
        {
            set { btnNovo.Enabled = !value; }
        }

        public bool BloqueiaGravar
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaCodigo
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaDescricao
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaEditar
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaEstornar
        {
            set
            {
                //Não Utilizado
            }
        }

        public bool BloqueiaNotaFornecimento
        {
            set
            {
                //Não Utilizado
            }
        }

        #endregion
    }
}
