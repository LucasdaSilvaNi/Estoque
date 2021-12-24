using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using System.Reflection;
using Sam.Entity;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using System.IO;
using System.Collections;

namespace Sam.Web.Almoxarifado
{
    public partial class ResumoSituacao : PageBase
    {

        public int orgaoPadraoId = 0;
        int tipoTipomovimento = 10;
        int divisaoId = 0;
        int tipoOperacao = 10;
        LogUsuarioEntity logUsuario = new LogUsuarioEntity();
        public String pediodo = String.Empty;

        private const string sessionMovimento = "sessionMovimento";
        protected void Page_Load(object sender, EventArgs e)
        {

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            if (!IsPostBack)
            {
                PopularOrgao();
                ddlAnoMes.Enabled = false;
            }

        }
        #region Popula Órgão      
        private void PopularOrgao()
        {
            ResumoSituacaoPresenter resumoSituacaoPresenter = new ResumoSituacaoPresenter();

            var orgaoPadrao = (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null) ? (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id : 0;

            var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
            DesabilitarAlmoxarifadoUsuario();
            // btnGerarRelatorioConsolidado.Enabled = false;

            ddlOrgao.AppendDataBoundItems = true;

            switch (perfil)
            {
                case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    this.ddlOrgao.DataValueField = "Id";
                    this.ddlOrgao.DataTextField = "CodigoDescricao";
                    this.ddlOrgao.DataSource = resumoSituacaoPresenter.PopularListaOrgaoTodosCod();
                    this.ddlOrgao.DataBind();

                    ddlOrgao.SelectedValue = orgaoPadrao.ToString();
                    ddlOrgao.Enabled = false;
                    PopularComboAlmoxarifado();
                    break;

                default:
                    ddlOrgao.Items.Add("- Selecione -");
                    ddlOrgao.Items[0].Value = "0";
                    this.ddlOrgao.DataValueField = "Id";
                    this.ddlOrgao.DataTextField = "CodigoDescricao";
                    this.ddlOrgao.DataSource = resumoSituacaoPresenter.PopularListaOrgaoTodosCod();
                    this.ddlOrgao.DataBind();
                    ddlOrgao.Enabled = true;
                    break;

            }
            AtivarAlmoxarifadoUsuario();
        }
        #endregion

        #region Popula Almoxarifado       
        private void PopularComboAlmoxarifado()
        {
            ResumoSituacaoPresenter presenter = new ResumoSituacaoPresenter();
            IList<AlmoxarifadoEntity> lista = null;
            ddlAlmoxarifado.Enabled = true;
            var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
            this.ddlAlmoxarifado.Items.Clear();
            var idAlmozarifado = Convert.ToString((GetAcesso.Transacoes.Perfis[0].AlmoxarifadoPadrao).Id);
            var idOrgao = int.Parse(ddlOrgao.SelectedValue);
            lista = presenter.ListaAlmoxarifadoPorOgao(idOrgao);
            switch (perfil)
            {
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    ddlAlmoxarifado.AppendDataBoundItems = true;
                    this.ddlAlmoxarifado.DataValueField = "Id";
                    this.ddlAlmoxarifado.DataTextField = "Descricao";
                    this.ddlAlmoxarifado.DataSource = lista;
                    this.ddlAlmoxarifado.DataBind();
                    ddlAlmoxarifado.SelectedValue = idAlmozarifado;
                    ddlAlmoxarifado.Enabled = false;
                    DesabilitarAlmoxarifadoUsuario();
                    ddlTipoExportacao_SelectedIndexChanged(this, EventArgs.Empty);
                    break;
                default:
                    ddlAlmoxarifado.Items.Add("- Selecione -");
                    ddlAlmoxarifado.Items.Add("- Todos -");
                    ddlAlmoxarifado.Items[0].Value = "0";
                    ddlAlmoxarifado.AppendDataBoundItems = true;
                    this.ddlAlmoxarifado.DataValueField = "Id";
                    this.ddlAlmoxarifado.DataTextField = "Descricao";
                    this.ddlAlmoxarifado.DataSource = lista;
                    this.ddlAlmoxarifado.DataBind();
                    break;
            }

            ddlAlmoxarifado.DataSource = null;

        }
        #endregion

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            DesabilitarRequisicaoEstoqueMaxMinNotaSiafen();
            DesabilitarAlmoxarifadoUsuario();

            if (ddlOrgao.SelectedValue == "0")
            {
                ddlAlmoxarifado.SelectedValue = "0";
                gridAlmoxarifadoPendenciaFechamento.DataSource = null;
                ddlAlmoxarifado.Enabled = false;
                return;
            }


            PopularComboAlmoxarifado();
            ddlAnoMes.Items.Clear();
        }

        protected void ddlTipoExportacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
            var almoxarifadoSelecionado = Convert.ToString(ddlAlmoxarifado.SelectedItem);
            btnGerarRelatorioConsolidado.Visible = true;
            if (this.ddlAlmoxarifado.SelectedValue.Equals("0") && almoxarifadoSelecionado != " - Selecione - ")
            {
                DesabilitarAlmoxarifadoUsuario();
                DesabilitarRequisicaoEstoqueMaxMinNotaSiafen();
                return;
            }
            if (almoxarifadoSelecionado == "- Todos -")
            {
                PopulaDadosUsuarioLogado();
                var IdAlmoxarifadoSelecionado = int.Parse(ddlOrgao.SelectedValue);
                PopulaDadosStatusFechamento(0);
                switch (perfil)
                {
                    case (int)GeralEnum.TipoPerfil.AdministradorFinanceiroSEFAZ:
                    case (int)GeralEnum.TipoPerfil.AdministradorGeral:
                    case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                        gridUgeImplantada.Visible = true;
                        lblUgeImplantada.Visible = true;
                        PopularUgeImplandatas();
                        break;
                    default:
                        gridUgeImplantada.Visible = false;
                        lblUgeImplantada.Visible = false;
                        break;
                }

                AtivarAlmoxarifadoUsuario();
                DesabilitarRequisicaoEstoqueMaxMinNotaSiafen();

            }

            else
            {
                DesabilitarAlmoxarifadoUsuario();
                PopularDadosGridRequisicao(null, 0);
                PopularDadosGridSiafem();
                PopularDadosEstoqueMaximo();
                PopularDadosEstoqueMinimo();
                DesabilitarAlmoxarifadoUsuario();
                AtivarRequisicaoEstoqueMaxMinNotaSiafen();
                gridUgeImplantada.Visible = false;
                lblUgeImplantada.Visible = false;
            }
            PopularPeriodo();
          //  ddlAnoMes.SelectedValue = "0";
            ddlAnoMes.Enabled = true;
        }

        #region Dados Requisições pendentes  filtra por almoxarifado   
        public void PopularDadosGridRequisicao(MovimentoEntity movimento, int pageIndex)
        {
            try
            {
                ResumoSituacaoPresenter presenter = new ResumoSituacaoPresenter();
                lblRequisicoesPendentes.Text = "Requisições pendentes";

                if (movimento != null)
                {
                    PageBase.SetSession<MovimentoEntity>(movimento, sessionMovimento);
                }
                else
                {
                    movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);
                }
                var idAlmoxarifado = (int)Convert.ToInt32(this.ddlAlmoxarifado.SelectedValue); //Id almoxarifado                                                                                           
                IList<MovimentoEntity> list = presenter.ListarRequisicaoByPendete(15, pageIndex, idAlmoxarifado, divisaoId, tipoTipomovimento, tipoOperacao);
                grdRequisicao.PageSize = 15;
                grdRequisicao.PageIndex = pageIndex;
                grdRequisicao.AllowPaging = true;
                grdRequisicao.DataSource = list;
                grdRequisicao.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        #endregion

        #region UGE's implantadas
        private void PopularUgeImplandatas()
        {
            ResumoSituacaoPresenter populaUgePresenter = new ResumoSituacaoPresenter();
            lblUgeImplantada.Text = "UGE's Implantadas";
            var IdOrgao = Convert.ToInt32(ddlOrgao.SelectedValue);
            var resultado = populaUgePresenter.ListarUgeImplantada(IdOrgao);
            gridUgeImplantada.PageSize = 30;
            gridUgeImplantada.AllowPaging = true;
            gridUgeImplantada.DataSource = null;
            gridUgeImplantada.DataSource = resultado;
            gridUgeImplantada.DataBind();
        }

        protected void gridUgeImplantada_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridUgeImplantada.PageIndex = e.NewPageIndex;
            PopularUgeImplandatas();
        }
        #endregion

        #region Popula index da página    
        protected void grdRequisicao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PopularDadosGridRequisicao(null, e.NewPageIndex);
            ViewState["pagina"] = e.NewPageIndex;
            if (ViewState["SortExpression"] != null)
                Sort(ViewState["SortExpression"].ToString());

        }
        private void Sort(string SortExpression)
        {

            MovimentoEntity movimento = null;
            movimento = PageBase.GetSession<MovimentoEntity>(sessionMovimento);

            ResumoSituacaoPresenter presenter = new ResumoSituacaoPresenter();

            int pagina = ViewState["pagina"] == null ? 0 : Convert.ToInt32(ViewState["pagina"].ToString());
            string Sortdir = GetSortDirection(SortExpression);
            string SortExp = SortExpression;
            var idAlmoxarifado = (int)Convert.ToInt32(this.ddlAlmoxarifado.SelectedValue); //Id almoxarifado
            var list = presenter.ListarRequisicaoByPendete(15, pagina, idAlmoxarifado, divisaoId, tipoTipomovimento, tipoOperacao).ToList();
            if (Sortdir == "ASC")
            {
                list = Sort<MovimentoEntity>(list, SortExp, SortDirection.Ascending);
            }
            else
            {
                list = Sort<MovimentoEntity>(list, SortExp, SortDirection.Descending);
            }
            this.grdRequisicao.DataSource = list;
            this.grdRequisicao.DataBind();
        }
        public List<MovimentoEntity> Sort<TKey>(List<MovimentoEntity> list, string sortBy, SortDirection direction)
        {
            PropertyInfo property = list.GetType().GetGenericArguments()[0].GetProperty(sortBy);

            if (direction == SortDirection.Ascending)
            {
                return list.OrderBy(e => property.GetValue(e, null)).ToList<MovimentoEntity>();
            }
            else
            {
                return list.OrderByDescending(e => property.GetValue(e, null)).ToList<MovimentoEntity>();
            }


        }
        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;
            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;
            return sortDirection;
        }
        #endregion

        #region Dados SIAFEM  filtra por almoxarifado      
        public void PopularDadosGridSiafem()
        {
            try
            {
                lblNotaLancamentoPendenteSIAFEM.Text = "Notas Siafem Pendentes";
                int almoxID = (int)Convert.ToInt32(this.ddlAlmoxarifado.SelectedValue); //Id almoxarifado
                List<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = null;
                tipoPesquisa tipoPesquisa = tipoPesquisa.Almox;

                ResumoSituacaoPresenter objPresenter = new ResumoSituacaoPresenter();
                lstRetorno = (List<NotaLancamentoPendenteSIAFEMEntity>)objPresenter.ListarNotasLancamentosPendentes(tipoPesquisa, almoxID);

                var retorno = lstRetorno.ConvertAll(new Converter<NotaLancamentoPendenteSIAFEMEntity, ResumoSiafem>(ResumoSiafem.GetResumoSiafem));

                NotaLancamentoPendenteSIAFEMEntity nota = new NotaLancamentoPendenteSIAFEMEntity();
                gdvNotaLancamentoSIAFEM.PageSize = 15;
                gdvNotaLancamentoSIAFEM.AllowPaging = true;
                gdvNotaLancamentoSIAFEM.DataSource = retorno;
                gdvNotaLancamentoSIAFEM.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected void gdvNotaLancamentoSIAFEM_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvNotaLancamentoSIAFEM.PageIndex = e.NewPageIndex;
            PopularDadosGridSiafem();
        }
        #endregion

        #region Itens com estoque máximo filtra por almoxarifado      
        public void PopularDadosEstoqueMaximo()
        {
            try
            {
                lblSubItemEstoqueMaximo.Text = "Itens que estão com o estoque máximo";
                ResumoSituacaoPresenter subPresenter = new ResumoSituacaoPresenter();
                int almoxID = (int)Convert.ToInt32(this.ddlAlmoxarifado.SelectedValue); //Id almoxarifado
                int filtro = 1; // 1: Estoque máximo
                var retorno = subPresenter.ListarItensEstoque(filtro, almoxID);
                gridSubItemEstoqueMaximo.PageSize = 15;
                gridSubItemEstoqueMaximo.AllowPaging = true;
                gridSubItemEstoqueMaximo.DataSource = retorno;
                gridSubItemEstoqueMaximo.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        protected void gridSubItemEstoqueMaximo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridSubItemEstoqueMaximo.PageIndex = e.NewPageIndex;
            PopularDadosEstoqueMaximo();
        }
        #endregion

        #region Itens com estoque máximo filtra por almoxarifado    
        public void PopularDadosEstoqueMinimo()
        {
            try
            {
                lblSubItemEstoqueMinimo.Text = "Itens que estão com o estoque mínimo";
                ResumoSituacaoPresenter subPresenter = new ResumoSituacaoPresenter();
                int almoxID = (int)Convert.ToInt32(this.ddlAlmoxarifado.SelectedValue); //Id almoxarifado
                int filtro = 2; // 1: Estoque Mínimo
                var retorno = subPresenter.ListarItensEstoque(filtro, almoxID);
                gridSubItemEstoqueMinimo.PageSize = 15;
                gridSubItemEstoqueMinimo.AllowPaging = true;
                gridSubItemEstoqueMinimo.DataSource = retorno;
                gridSubItemEstoqueMinimo.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        protected void gridSubItemEstoqueMinimo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridSubItemEstoqueMinimo.PageIndex = e.NewPageIndex;
            PopularDadosEstoqueMinimo();
        }
        #endregion

        #region Status almoxarifado

        private void PopulaDadosStatusFechamento(int IdAlmoxarifado)
        {
            try
            {
                lblAlmoxarifadoPendencia.Text = "Status Fechamento Almoxarifados";
                ResumoSituacaoPresenter presenter = new ResumoSituacaoPresenter();
                int idOrgao = Convert.ToInt32(ddlOrgao.SelectedValue);
                IList<AlmoxarifadoEntity> lista = presenter.ListarAlmoxarifadoStatusFechamento(IdAlmoxarifado, idOrgao);
                gridAlmoxarifadoPendenciaFechamento.PageSize = 30;
                gridAlmoxarifadoPendenciaFechamento.AllowPaging = true;
                gridAlmoxarifadoPendenciaFechamento.DataSource = null;
                gridAlmoxarifadoPendenciaFechamento.DataSource = lista;
                gridAlmoxarifadoPendenciaFechamento.DataBind();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected void gridAlmoxarifadoPendenciaFechamento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridAlmoxarifadoPendenciaFechamento.PageIndex = e.NewPageIndex;
            PopulaDadosStatusFechamento(0);
        }
        #endregion

        protected void btnGerarRelatorioConsolidado_Click(object sender, EventArgs e)
        {
            var Orgao = ddlOrgao.SelectedValue;
            var almoxarifado = ddlAlmoxarifado.SelectedValue;
            var periodo = ddlAnoMes.SelectedValue;
            if (Orgao != "0" && periodo != "0" && ddlAnoMes.SelectedValue!="") { this.GerarExportacaoAlmoxarifado(); }
            else
            {
                if (Orgao == "0")
                {
                   // ExibirMensagem("Selecione o Órgão");
                    return;
                }
                if (almoxarifado=="0")
                {
                   // ExibirMensagem("Selecione o Almoxarifado");
                    return;
                }
                if (periodo == "0")
                {
                   // ExibirMensagem("Selecione o Mês/ano Referência");
                    return;
                }               
            }
        }
        private void GerarExportacaoAlmoxarifado()
        {
            try
            {
                ResumoSituacaoPresenter presenter = new ResumoSituacaoPresenter();
                DataSet dsRetorno = new DataSet();
                string AnoMesRef = string.Empty;
                var mensagemErro = string.Empty;
                var IdOrgao = int.Parse(ddlOrgao.SelectedValue);
                var IdAlmoxarifado = (ddlAlmoxarifado.SelectedValue) == "- Todos -" ? 0 : int.Parse(ddlAlmoxarifado.SelectedValue);
                var Periodo = ddlAnoMes.SelectedValue;
                if (ddlAnoMes.SelectedIndex == -1)
                {
                    //ExibirMensagem("Selecione Mês/ano referência");
                    return;
                }               
                dsRetorno = presenter.GerarExportacaoAlmoxarifadoStatusFechamento(IdOrgao, IdAlmoxarifado, Periodo);
                if (dsRetorno.Tables[0].Rows.Count > 0)
                {
                    ExportarDataSetParaExcel(dsRetorno.Tables[0].DataSet, "EXPORTACAO_ALMOXARIFADO" + DateTime.Now.ToString("ddMMyyyy") + ".xls");
                }
                else
                {
                    ExibirMensagem("Não existe dados para ser exportado");
                    return;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ExportarDataSetParaExcel(DataSet ds, string sNome)
        {
            //Limpar o conteudo
            Response.Clear();
            try
            {
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + sNome);
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
                Response.Charset = "ISO-8859-1";
                EnableViewState = false;
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        GridView gv = new GridView();
                        gv.DataSource = ds.Tables[0];
                        gv.DataBind();
                        gv.RenderControl(htw);
                        Response.Write(sw.ToString());
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.SuppressContent = true;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GerarExcel(DataTable listaExportacao, string nomeDoArquivo)
        {

            try
            {
                var grdExport = new GridView();
                grdExport.DataSource = listaExportacao;
                grdExport.DataBind();

                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + nomeDoArquivo + ".xls");
                Response.Charset = "";

                StringWriter stringWrite = new StringWriter();
                HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

                grdExport.RenderControl(htmlWrite);
                Response.Write(stringWrite.ToString());

                Response.End();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        #region Usuários
        private void PopulaDadosUsuarioLogado()
        {
            try
            {


                ResumoSituacaoPresenter usuarioPresent = new ResumoSituacaoPresenter();
                List<UsuarioLogadoEntity> lista = null;
                var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
                lblUsuarioLogadoAlmoxarifado.Text = "Usuários Logados";
                switch (perfil)
                {
                    case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                    case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                        orgaoPadraoId = (GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id != null) ? (int)GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id : 0;
                        break;
                    default:
                        orgaoPadraoId = int.Parse(ddlOrgao.SelectedValue);
                        break;
                }

                lista = usuarioPresent.ListarUsuariosOnlinePorOrgao(orgaoPadraoId);
                var retorno = (lista.ConvertAll(new Converter<UsuarioLogadoEntity, UsuarioLogado>(UsuarioLogado.GetResumoUsuario)));
                gridUsuarioLogadoAlmoxarifado.PageSize = 35;
                gridUsuarioLogadoAlmoxarifado.AllowPaging = true;
                gridUsuarioLogadoAlmoxarifado.DataSource = retorno;
                gridUsuarioLogadoAlmoxarifado.DataBind();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        protected void gridUsuarioLogadoAlmoxarifado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridUsuarioLogadoAlmoxarifado.PageIndex = e.NewPageIndex;
            PopulaDadosUsuarioLogado();
        }
        protected void gridUsuarioLogadoAlmoxarifado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
        }
        public int IdOrgao
        {
            get
            {
                int vlRetorno = 0;
                if (this.GetAcesso.IsNotNull() &&
                   this.GetAcesso.Transacoes.IsNotNull() &&
                   this.GetAcesso.Transacoes.Perfis.IsNotNullAndNotEmpty() &&
                   (int)(this.GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id) != 0)
                {
                    vlRetorno = (int)this.GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;
                }

                return vlRetorno;

            }
            set { }
        }

        public int CodigoOrgao
        {
            get
            {
                int vlRetorno = 0;
                if (this.GetAcesso.IsNotNull() &&
                   this.GetAcesso.Transacoes.IsNotNull() &&
                   this.GetAcesso.Transacoes.Perfis.IsNotNullAndNotEmpty() &&
                   (int)(this.GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Codigo) != 0)
                {
                    vlRetorno = (int)this.GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Codigo;
                }

                return vlRetorno;

            }
            set { }
        }

        #endregion

        #region Visibilidade     
        private void DesabilitarAlmoxarifadoUsuario()
        {
            gridAlmoxarifadoPendenciaFechamento.Visible = false;
            gridAlmoxarifadoPendenciaFechamento.DataSource = null;
            lblAlmoxarifadoPendencia.Visible = false;
            gridUsuarioLogadoAlmoxarifado.Visible = false;
            gridUsuarioLogadoAlmoxarifado.DataSource = null;
            lblUsuarioLogadoAlmoxarifado.Visible = false;
            // btnGerarRelatorioConsolidado.Enabled = false;
            gridUgeImplantada.Visible = false;
            lblUgeImplantada.Visible = false;
        }
        private void AtivarAlmoxarifadoUsuario()
        {
            gridAlmoxarifadoPendenciaFechamento.Visible = true;
            lblAlmoxarifadoPendencia.Visible = true;
            gridUsuarioLogadoAlmoxarifado.Visible = true;
            lblUsuarioLogadoAlmoxarifado.Visible = true;
            // btnGerarRelatorioConsolidado.Enabled = true;
        }
        private void DesabilitarRequisicaoEstoqueMaxMinNotaSiafen()
        {
            grdRequisicao.Visible = false;
            lblRequisicoesPendentes.Visible = false;
            gridSubItemEstoqueMinimo.Visible = false;
            lblSubItemEstoqueMinimo.Visible = false;
            gridSubItemEstoqueMaximo.Visible = false;
            lblSubItemEstoqueMaximo.Visible = false;
            gdvNotaLancamentoSIAFEM.Visible = false;
            lblNotaLancamentoPendenteSIAFEM.Visible = false;

        }
        private void AtivarRequisicaoEstoqueMaxMinNotaSiafen()
        {
            grdRequisicao.Visible = true;
            lblRequisicoesPendentes.Visible = true;
            gridSubItemEstoqueMinimo.Visible = true;
            lblSubItemEstoqueMinimo.Visible = true;
            gridSubItemEstoqueMaximo.Visible = true;
            lblSubItemEstoqueMaximo.Visible = true;
            gdvNotaLancamentoSIAFEM.Visible = true;
            lblNotaLancamentoPendenteSIAFEM.Visible = true;
        }
        #endregion

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public void PopularPeriodo()
        {
            var presenter = new ResumoSituacaoPresenter();
            DateTime data = DateTime.Now;
            ddlAnoMes.Items.Clear();
            ddlAnoMes.Items.Add("- Selecione -");
            ddlAnoMes.Items[0].Value = "0";
            var idOrgao = int.Parse(ddlOrgao.SelectedValue);
            var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
            switch (perfil)
            {
                case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                    ddlAnoMes.Enabled = false;
                    btnGerarRelatorioConsolidado.Enabled = false;
                    break;
                default:
                    btnGerarRelatorioConsolidado.Enabled = true;
                    ddlAnoMes.Enabled = true;
                    if (idOrgao > 0)
                    {
                        List<AlmoxarifadoEntity> retorno = new List<AlmoxarifadoEntity>(presenter.ListarInicioFechamento(idOrgao));                    
                        var anoMesRef = (string) retorno.Min(x=>x.MesRef);
                        if (!string.IsNullOrEmpty(anoMesRef))
                        {
                            for (DateTime dat = new DateTime(Convert.ToInt32(anoMesRef.Substring(0, 4)),
                                                       Convert.ToInt32(anoMesRef.Substring(4, 2)), 1); dat < DateTime.Now; dat = dat.AddMonths(1))
                            {
                                ddlAnoMes.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                                ddlAnoMes.Items[ddlAnoMes.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
                            }
                        }
                        else
                        {                          
                            return;
                        }                                               
                    }
                    break;
            }

        }


        List<string> listaErro = new List<string>();
        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }
        public IList ListaErros
        {
            set { this.ListInconsistencias.ExibirLista(value); }
        }
    }
}