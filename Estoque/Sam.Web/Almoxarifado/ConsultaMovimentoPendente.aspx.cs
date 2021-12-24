using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.Infrastructure.Infrastructure.Interface;
using Sam.View;
using System.Collections;
using System.Threading;


namespace Sam.Web.Almoxarifado
{
    public partial class ConsultaMovimentoPendente : PageBase, IMovimentoPendente, IMovimentoView
    {
        private MovimentoPresenter presenterConsulta = new MovimentoPresenter();
        private readonly static AlmoxarifadoPresenter presenterAlmoxarifado = new AlmoxarifadoPresenter();
        private readonly static UGEPresenter presenterUge = new UGEPresenter();
        private string path = string.Empty;
        private Thread thredCorrigir = null;
        private String Caminho { get; set; }
        private ConsultaMovimentoPendente pendente;
        private Int32 TotalRegistrosGrid;
        private int IdGestor = (new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
        private void setTotalRegistro()
        {
        
            if (Session["listaMovimentoPendente"] != null)
            {
                var retorno = (IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>)Session["listaMovimentoPendente"];
                this.labelTotalRegistro.Text = this.presenterConsulta.getSubItensCount(retorno).ToString();
                this.spanTotalMovimento.Visible = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //SetTimeoutProcessamentoAssincrono(2147483647);
            ScriptManager.GetCurrent(this).AsyncPostBackTimeout = 90000;
            Session.Timeout = 525600;
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(525600));
           
            if (!IsPostBack)
            {
                ComboDeAlmoxarifado();
                //registrarScript();
                if (Session["listaMovimentoPendente"] != null)
                    Session["listaMovimentoPendente"] = null;
                spanTotalMovimento.Visible = false;
                timerArquivo.Enabled = false;
            }
            ThreadStart StartCorrigir = new ThreadStart(Recalcular);
            thredCorrigir = new Thread(StartCorrigir);
             

        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }
        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (verificarAlmoxarifadoSelecionado())
            {

                Session["listaMovimentoPendente"] = null;
                grdMovimento.DataSourceID = null;
                grdMovimento.Caption = "";
                grdMovimento.DataSourceID = "movimento";
                grdMovimento.Visible = true;
                
               // this.labelTotalRegistro.Text = this.TotalRegistrosGrid.ToString();
            }
        }

        #region atributos publicos
  
        public DateTime? TB_MOVIMENTO_DATA_MOVIMENTO
        {
            get;
            set;

        }

        public DateTime? TB_MOVIMENTO_DATA_DOCUMENTO
        {
            get;
            set;
        }

        public DateTime? TB_MOVIMENTO_DATA_OPERACAO
        {
            get;
            set;
        }

        public string TB_MOVIMENTO_ANO_MES_REFERENCIA
        {
            get;
            set;
        }

        public string TB_TIPO_MOVIMENTO_DESCRICAO
        {
            get;
            set;
        }

        public long TB_SUBITEM_MATERIAL_CODIGO
        {
            get;
            set;
        }


        public decimal? TB_MOVIMENTO_ITEM_QTDE_MOV { get; set;  }
        public decimal? TB_MOVIMENTO_ITEM_QTDE_MOV_CORRIGIDO { get; set; }
        public decimal? TB_MOVIMENTO_ITEM_SALDO_QTDE { get; set; }
        public decimal? TB_MOVIMENTO_ITEM_SALDO_QTDE_CORRIGIDO { get; set; }

        public decimal? TB_MOVIMENTO_ITEM_VALOR_MOV
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_SALDO_VALOR
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_PRECO_UNIT
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_DESD
        {
            get;
            set;
        }

        public int TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
        {
            get;
            set;
        }

        public bool? TB_MOVIMENTO_ATIVO
        {
            get;
            set;
        }

        public int TB_MOVIMENTO_ID
        {
            get;
            set;
        }

        public int TB_MOVIMENTO_ITEM_ID
        {
            get;
            set;
        }

        public int TB_SUBITEM_MATERIAL_ID
        {
            get;
            set;
        }

        public string TB_MOVIMENTO_NUMERO_DOCUMENTO
        {
            get;
            set;
        }

        public int? TB_UGE_ID
        {
            get;
            set;

        }

        public int TB_ALMOXARIFADO_ID
        {
            get;
            set;
        }

        public int TB_ALMOXARIFADO_CODIGO
        {
            get;
            set;
        }

        public int? TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO
        {
            get;
            set;
        }

        public int? TB_LOGIN_ID
        {
            get;
            set;
        }

        public string TB_MOVIMENTO_EMPENHO
        {
            get;
            set;
        }

        public string STATUS
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_VALOR_MOV_CORRIGIDO
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_SALDO_VALOR_CORRIGIDO
        {
            get;
            set;
        }

        public decimal? TB_MOVIMENTO_ITEM_PRECO_UNIT_CORRIGIDO
        {
            get;
            set;
        }
        public decimal? TB_MOVIMENTO_ITEM_DESD_CORRIGIDO
        {
            get;
            set;
        }

        #region movimento
        public int OrgaoId
        {
            get { throw new NotImplementedException(); }
        }

        public int UgeId
        {
            get;
            set;
        }

        public int UnidadeId
        {
            get;
            set;
        }

        public int? FornecedorId
        {
            get;
            set;
        }

        public int TipoMovimento
        {
            get;
            set;
        }

        public string NumeroDocumento
        {
            get;
            set;
        }

        public string AnoMesReferencia
        {
            get;
            set;
        }

        public DateTime? DataDocumento
        {
            get;
            set;
        }

        public DateTime? DataMovimento
        {
            get;
            set;
        }

        public string FonteRecurso
        {
            get;
            set;
        }

        public decimal? ValorDocumento
        {
            get;
            set;
        }

        public string Observacoes
        {
            get;
            set;
        }

        public string Instrucoes
        {
            get;
            set;
        }

        public string Empenho
        {
            get;
            set;
        }

        public string NlLiquidacao
        {
            get;
            set;
        }

        public int? DivisaoId
        {
            get;
            set;
        }

        public int? AlmoxarifadoIdOrigem
        {
            get;
            set;
        }

        public string GeradorDescricao
        {
            get;
            set;
        }

        public void ExibirRelatorio()
        {
            throw new NotImplementedException();
        }

        public System.Collections.SortedList ParametrosRelatorio
        {
            get { throw new NotImplementedException(); }
        }

        public Domain.Entity.RelatorioEntity DadosRelatorio
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string Codigo
        {
            get;
            set;
        }

        public string Descricao
        {
            get;
            set;
        }

        public void PopularGrid()
        {
            throw new NotImplementedException();
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

       

        public bool BloqueiaNovo
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaGravar
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaExcluir
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaCancelar
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaCodigo
        {
            set { throw new NotImplementedException(); }
        }

        public bool BloqueiaDescricao
        {
            set { throw new NotImplementedException(); }
        }

        public bool MostrarPainelEdicao
        {
            set { throw new NotImplementedException(); }
        }
        #endregion

        #endregion

        #region métodos privados
        private void ComboDeAlmoxarifado()
        {
            drpAlmoxarifado.Items.Clear();
            drpAlmoxarifado.DataTextField = "TB_ALMOXARIFADO_DESCRICAO";
            drpAlmoxarifado.DataValueField = "TB_ALMOXARIFADO_ID";
            drpAlmoxarifado.Items.Add("- Selecione -");
            drpAlmoxarifado.Items[0].Value = "0";
            drpAlmoxarifado.AppendDataBoundItems = true;

            //refatorar esse método PopularListaAlmoxarifado, LINQ não deve ser usado na camada de VIEW
            this.drpAlmoxarifado.DataSource = presenterAlmoxarifado.ListarAlmoxarifadoPorGestorMovimentoPendente(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value); //presenterAlmoxarifado.ListarAlmoxarifadoPorGestorMovimentoPendente(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value).Where(a => a.TB_ALMOXARIFADO_ID != new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            this.drpAlmoxarifado.DataBind();
        }

        private Int32 getAlmoxarifadoIdlistaMovimentoPendente()
        {
            if (Session["listaMovimentoPendente"] == null)
            {
                return 0;
            }
            else
            {
                var lista = (IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>)Session["listaMovimentoPendente"];

                if (lista.Count > 0)
                    return lista[0].TB_ALMOXARIFADO_ID;
                else
                    return 0;
            }
        }
        #endregion

        #region métodos publicos
        public IList<IMovimentoPendente> PesquisaMovimentoPendente(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, Int32 almoxarifadoId, Int64 subtItemCodigo)
        {

            presenterConsulta.View = this;

            if (Session["listaMovimentoPendente"] != null)
            {
                presenterConsulta.setListaDeMovimentoPendente((IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>)Session["listaMovimentoPendente"]);
            }
            else
            {
                StartRowIndexParameterName = 0;
                
            }

            Session["listaMovimentoPendente"] = null;
            
            var retorno = presenterConsulta.ListaDeMovimentoPendente(StartRowIndexParameterName, almoxarifadoId, subtItemCodigo);
            this.TotalRegistrosGrid = presenterConsulta.TotalRegistrosGrid;
            if (this.TotalRegistrosGrid > 0)
            {
                var retornoPendente = presenterConsulta.getListaDeMovimentoPendente();
                Session.Add("listaMovimentoPendente", retornoPendente);


                return retorno;
            }
            else
            {
                Session.Add("listaMovimentoPendente", null);
                return null;
            }
            
        }


        public int TotalRegistros(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, Int32 almoxarifadoId, Int64 subtItemCodigo)
        {
            return this.TotalRegistrosGrid;
        }

        #endregion

        protected void grdMovimento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (verificarAlmoxarifadoSelecionado())
            {
                grdMovimento.PageIndex = e.NewPageIndex;
                paginarGrid();
            }
            
        }

        protected void paginarGrid()
        {
            grdMovimento.DataSourceID = "movimento";
        }

        private bool verificarAlmoxarifadoSelecionado()
        {
            if (this.drpAlmoxarifado.SelectedValue.Equals("0"))
            {
                //ExibirMensagem("Favor informar o almoxarifado!");
                List<string> lstErro = new List<string>();
                lstErro.Add("Favor informar o almoxarifado!");
                ListaErros = lstErro;
                return false;
            }
            else
            {
                return true;
            }
        }
     
        protected void grdMovimento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                IMovimentoPendente pendente = (IMovimentoPendente)e.Row.DataItem;

                e.Row.Cells[7].Text = (pendente.TB_MOVIMENTO_ITEM_QTDE_MOV != pendente.TB_MOVIMENTO_ITEM_QTDE_MOV_CORRIGIDO ? "<span style='color:#C00000;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_QTDE_MOV.ToString() + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_QTDE_MOV_CORRIGIDO.ToString() + "</span>" : "<span>" + pendente.TB_MOVIMENTO_ITEM_QTDE_MOV.ToString() + "</span>");
                e.Row.Cells[8].Text = (pendente.TB_MOVIMENTO_ITEM_VALOR_MOV != pendente.TB_MOVIMENTO_ITEM_VALOR_MOV_CORRIGIDO.Value ? "<span style='color:#C00000;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_VALOR_MOV.Value.ToString("n2") + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_VALOR_MOV_CORRIGIDO.Value.ToString("n2") + "</span>" : "<span>" + pendente.TB_MOVIMENTO_ITEM_VALOR_MOV.Value.ToString("n2") + "</span>");
                e.Row.Cells[9].Text = (pendente.TB_MOVIMENTO_ITEM_SALDO_QTDE != pendente.TB_MOVIMENTO_ITEM_SALDO_QTDE_CORRIGIDO ? "<span style='color:#C00000;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_SALDO_QTDE.ToString() + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_SALDO_QTDE_CORRIGIDO.ToString() + "</span>" : "<span>" + pendente.TB_MOVIMENTO_ITEM_SALDO_QTDE.ToString() + "</span>");
                e.Row.Cells[10].Text = (pendente.TB_MOVIMENTO_ITEM_SALDO_VALOR.Value != pendente.TB_MOVIMENTO_ITEM_SALDO_VALOR_CORRIGIDO.Value ? "<span style='color:#C00000;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_SALDO_VALOR.Value.ToString("n2") + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_SALDO_VALOR_CORRIGIDO.Value.ToString("n2") + "</span>" : "<span>" + pendente.TB_MOVIMENTO_ITEM_SALDO_VALOR.Value.ToString("n2") + "</span>");
                e.Row.Cells[11].Text = (pendente.TB_MOVIMENTO_ITEM_PRECO_UNIT.Value != pendente.TB_MOVIMENTO_ITEM_PRECO_UNIT_CORRIGIDO.Value ? "<span style='color:#C00000;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_PRECO_UNIT.Value.ToString("n4") + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_PRECO_UNIT_CORRIGIDO.Value.ToString("n4") + "</span>" : "<span>" + pendente.TB_MOVIMENTO_ITEM_PRECO_UNIT.Value.ToString("n4") + "</span>");
                e.Row.Cells[12].Text = (pendente.TB_MOVIMENTO_ITEM_DESD.Value != pendente.TB_MOVIMENTO_ITEM_DESD_CORRIGIDO.Value ? "<span style='color:#C00000;font-weight:bold'>" + (pendente.TB_MOVIMENTO_ITEM_DESD !=null ? pendente.TB_MOVIMENTO_ITEM_DESD.Value.ToString("n2") : "0.0") + "</span>=><span style='color:#0033CC;font-weight:bold'>" + pendente.TB_MOVIMENTO_ITEM_DESD_CORRIGIDO.Value.ToString("n2") + "</span>" : "<span>" + (pendente.TB_MOVIMENTO_ITEM_DESD != null ? pendente.TB_MOVIMENTO_ITEM_DESD.Value.ToString("n2") : "0.0") + "</span>"); 
                
            
            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                setTotalRegistro();
            }
            
        }

        protected void drpAlmoxarifado_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.grdMovimento.DataSourceID = null;
            this.grdMovimento.Visible = false;
        }

        protected void btnRecalculo_Click(object sender, EventArgs e)
        {

            pendente = new ConsultaMovimentoPendente();

            path = Server.MapPath("") + @"\" + "Recalculo" + drpAlmoxarifado.SelectedValue.ToString() + ".txt";

            if (getArquivoExiste(path))
                deletaArquivo(path);

            Session.Add("NOMERECALCULO", "Recalculo" + drpAlmoxarifado.SelectedValue.ToString());
            Session.Add(Session["NOMERECALCULO"].ToString(), path);

            ScriptManager.RegisterStartupScript(this, this.GetType(), "CorrigirAlmoxarifado", "alert('O Recalculo do almoxarifado:" + this.drpAlmoxarifado.SelectedItem.Text + " vai inicializar, por favor espere a mensagem de conclusão para selecionar outro almoxarifado!');", true);

            pendente.TB_ALMOXARIFADO_ID = Convert.ToInt32(drpAlmoxarifado.SelectedValue);
            pendente.TB_SUBITEM_MATERIAL_CODIGO = (this.txtSubtItemCodigo.Text.Trim().Length > 0 ? Convert.ToInt64(this.txtSubtItemCodigo.Text) : 0);
            pendente.Caminho = path;
            timerArquivo.Enabled = true;
            this.btnRecalculo.Enabled = false;
            var constante = Sam.Common.Util.Constante.CST_ANO_MES_DATA_CORTE_SAP;
            thredCorrigir.Start();

            
        }

        private void Recalcular()
        {
            try
            {
               
 
                presenterConsulta.View = pendente;
                var gestorId = IdGestor;
                presenterConsulta.recalculaSubtItem(pendente.TB_ALMOXARIFADO_ID, pendente.TB_SUBITEM_MATERIAL_CODIGO, IdGestor);


                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pendente.Caminho))
                {

                    string line = "Almoxarifado:" + pendente.TB_ALMOXARIFADO_ID.ToString() + "| Data:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                    file.WriteLine(line);

                    file.Close();

                }
            }
            catch (Exception ex)
            {
                if (!getArquivoExiste(pendente.Caminho))
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter(pendente.Caminho);

                    file.Close();
                    
                }
                //throw new Exception(ex.Message, ex.InnerException);

            }
            finally
            {
                thredCorrigir.Abort();
            }
        
        }

        private Boolean getArquivoExiste(String path)
        {
            string filepath = path;
            System.IO.FileInfo file = new System.IO.FileInfo(filepath);
            if (file.Exists)
            {
                file = null;
                return true;
            }
            file = null;
            return false;

        }

        private void deletaArquivo(String path)
        {
            string filepath = path;
            System.IO.FileInfo file = new System.IO.FileInfo(filepath);

            if (file.Exists)
                file.Delete();

            file = null;
        }

        protected void timerArquivo_Tick(object sender, EventArgs e)
        {
            string filepath = Session[Session["NOMERECALCULO"].ToString()].ToString();
            try
            {
               
                System.IO.FileInfo file = new System.IO.FileInfo(filepath);

                if (file.Exists)
                {

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Recalculo", "alert('Recalculo executado com sucesso!');", true);
                     this.btnRecalculo.Enabled = true;
                    file.Delete();
                
                    timerArquivo.Enabled = false;
                    file = null;
                }
            }
            catch (Exception ex)
            {
                timerArquivo.Enabled = false;
                this.btnRecalculo.Enabled = true;
                System.IO.FileInfo file = new System.IO.FileInfo(filepath);

                if (file.Exists)
                {
                    file.Delete();
                    file = null;
                }
                throw ex;
            }
        }

        public bool ExibirListaEmpenho
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public bool ExibirNumeroEmpenho
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public bool BloqueiaEmpenho
        {
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public int TipoOperacao
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }
    }
}
