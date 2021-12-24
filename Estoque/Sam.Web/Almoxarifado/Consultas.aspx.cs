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
using Sam.Domain.Entity;
using Sam.Web.MasterPage;






namespace Sam.Web.Almoxarifado
{
    public partial class Consultas : PageBase, IConsultasView
    {
        private MovimentoEntity movimento = new MovimentoEntity();
        private const string sessaoMov = "movimento";

        public void RaisePostBackEvent(string eventArgument) { }

        protected void Page_Load(object sender, EventArgs e)
        {
            PesquisaSubitem1.UsaSaldo = false;
            PesquisaSubitem1.FiltraGestor = true;

            if (!IsPostBack)
            {

                ddlConsulta.Items.Add("- Selecione -");
                ddlConsulta.Items[0].Value = "0";
                ddlConsulta.Items.Add("Analítica");
                ddlConsulta.Items[1].Value = "1";
                ddlConsulta.Items.Add("Sintética");
                ddlConsulta.Items[2].Value = "2";
                ddlConsulta.Items.Add("Ficha de Prateleira");
                ddlConsulta.Items[3].Value = "3";
                sintetica.Visible = false;
                analitica.Visible = false;
                InicializarCombos();
                idConsumo.Visible = false;
                idConsumoSubItem.Visible = false;
                MultiView1.ActiveViewIndex = 0;
            }
        }

        public void PopularComboConsumo()
        {
            ddlConsultaConsumo.Items.Add("Perfil de Consumo - Almoxarifado");
            ddlConsultaConsumo.Items[0].Value = "1";
            ddlConsultaConsumo.Items.Add("Perfil de Consumo - Requisitante");
            ddlConsultaConsumo.Items[1].Value = "2";
            if (GetAcesso.Transacoes.Perfis[0].IdPerfil == (int)GeralEnum.TipoPerfil.AdministradorGeral
                || GetAcesso.Transacoes.Perfis[0].IdPerfil == (int)GeralEnum.TipoPerfil.AdministradorOrgao)
            {
                ddlConsultaConsumo.Items.Add("Perfil de Consumo - SubItem");
                ddlConsultaConsumo.Items[2].Value = "3";
            }
        }

        protected void Menu1_MenuItemClick(Object sender, MenuEventArgs e)
        {
            MultiView1.ActiveViewIndex = Int32.Parse(e.Item.Value);
        }

        #region Bloquear Controles

        #endregion

        #region Popular Combos

        public void PopularDadosUGETodosCod()
        {
            bool isConsultaAnaliticaOuFichaPrateleira;
            UGEPresenter uge = new UGEPresenter();
            //IList<UGEEntity> lstUge = uge.PopularDadosComSaldo(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value,
            //    new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            //IList<UGEEntity> lstUge = uge.PopularDadosTodosCodPorGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            IList<UGEEntity> lstUGEsComSaldoParaSubItem = null;
            if (!String.IsNullOrWhiteSpace(txtSubItem.Text))
            {
    
                lstUGEsComSaldoParaSubItem = uge.PopularUGEsComSaldoParaSubItem(Convert.ToInt64(txtSubItem.Text), new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            }
                

            ddlUgeAnalitico.Items.Clear();

            isConsultaAnaliticaOuFichaPrateleira = (this.ddlConsulta.SelectedItem.Text.ToLowerInvariant() == "analítica" || this.ddlConsulta.SelectedItem.Text.ToLowerInvariant() == "ficha de prateleira");
            if (isConsultaAnaliticaOuFichaPrateleira && lstUGEsComSaldoParaSubItem != null && lstUGEsComSaldoParaSubItem.Count > 0)
            {
                ddlUgeAnalitico.DataTextField = "Descricao";
                ddlUgeAnalitico.DataValueField = "Id";
                ddlUgeAnalitico.DataSource = lstUGEsComSaldoParaSubItem; // órgão do perfil
                ddlUgeAnalitico.DataBind();
                ddlUgeAnalitico.Enabled = true;
                btnImprimir.Enabled = true;
            }
            else if (isConsultaAnaliticaOuFichaPrateleira && String.IsNullOrWhiteSpace(this.txtSubItem.Text))
            {
                ddlUgeAnalitico.Items.Add(new ListItem("NÃO HÁ SUBITEM SELECIONADO."));
                ddlUgeAnalitico.Enabled = false;
                btnImprimir.Enabled = false;
            }
            else if (isConsultaAnaliticaOuFichaPrateleira && !String.IsNullOrWhiteSpace(this.txtSubItem.Text) && (lstUGEsComSaldoParaSubItem != null && lstUGEsComSaldoParaSubItem.Count == 0))
            {
                ddlUgeAnalitico.Items.Add(new ListItem("NÃO HÁ UGEs COM SALDO PARA ESTE SUBITEM."));
                ddlUgeAnalitico.Enabled = false;
                btnImprimir.Enabled = false;
            }


            if (!isConsultaAnaliticaOuFichaPrateleira) btnImprimir.Enabled = true;
            ddlUgeSintetico.Items.Clear();
            ddlUgeSintetico.DataTextField = "Descricao";
            ddlUgeSintetico.DataValueField = "Id";
            // ddlUgeSintetico.Items.Add("- Todas -");
            //  ddlUgeSintetico.Items[0].Value = "0";
            // ddlUgeSintetico.AppendDataBoundItems = true;
            // ddlUgeSintetico.DataSource = lstUge; // órgão do perfil
            ddlUgeSintetico.Items.Add(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Descricao);
            ddlUgeSintetico.Items[0].Value = Convert.ToString(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id);
            ddlUgeSintetico.DataBind();
        }

        public void PopularDadosGrupoTodosCod()
        {
            GrupoPresenter grp = new GrupoPresenter();
            ddlGrupo.Items.Clear();
            ddlGrupo.DataTextField = "Descricao";
            ddlGrupo.DataValueField = "Id";
            ddlGrupo.Items.Add("- Todos -");
            ddlGrupo.Items[0].Value = "0";
            ddlGrupo.AppendDataBoundItems = true;
            ddlGrupo.DataSource = grp.PopularDadosGrupoTodosCod(0, 0); // órgão do perfil
            ddlGrupo.DataBind();

        }

        public void PopularDadosFornecedorTodosCod()
        {
            FornecedorPresenter forn = new FornecedorPresenter();
            ddlFornecedorMov.Items.Clear();
            ddlFornecedorMov.DataTextField = "Nome";
            ddlFornecedorMov.DataValueField = "Id";
            ddlFornecedorMov.Items.Add("- Selecione -");
            ddlFornecedorMov.Items[0].Value = "0";
            ddlFornecedorMov.AppendDataBoundItems = true;
            ddlFornecedorMov.DataSource = forn.PopularDadosFornecedorTodosCod(); // órgão do perfil
            ddlFornecedorMov.DataBind();
        }

        public void PopularDadosDivisaoTodosCod()
        {
            DivisaoPresenter divisao = new DivisaoPresenter();
            ddlDivisaoMov.Items.Clear();
            ddlDivisaoMov.DataTextField = "Descricao";
            ddlDivisaoMov.DataValueField = "Id";
            ddlDivisaoMov.Items.Add("- Selecione -");
            ddlDivisaoMov.Items[0].Value = "0";
            ddlDivisaoMov.AppendDataBoundItems = true;
            ddlDivisaoMov.DataSource = divisao.PopularListaDivisao(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            ddlDivisaoMov.DataBind();

            ddlDivisaoConsumo.Items.Clear();
            ddlDivisaoConsumo.DataTextField = "Descricao";
            ddlDivisaoConsumo.DataValueField = "Id";
            ddlDivisaoConsumo.Items.Add("- Selecione -");
            ddlDivisaoConsumo.Items[0].Value = "0";
            ddlDivisaoConsumo.AppendDataBoundItems = true;
            ddlDivisaoConsumo.DataSource = divisao.PopularListaDivisao(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            ddlDivisaoConsumo.DataBind();

        }

        public void PopularDadosAlmoxTodosCod()
        {
            AlmoxarifadoPresenter almox = new AlmoxarifadoPresenter();
            ddlAlmoxConsumo.Items.Clear();
            ddlAlmoxConsumo.DataTextField = "Descricao";
            ddlAlmoxConsumo.DataValueField = "Id";
            ddlAlmoxConsumo.Items.Add("- Selecione -");
            ddlAlmoxConsumo.Items[0].Value = "0";
            ddlAlmoxConsumo.AppendDataBoundItems = true;
            ddlAlmoxConsumo.DataSource = almox.PopularListaAlmoxarifado(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value); //  almox.PopularListaAlmoxarifado(7).Where(a => a.Id != 1).ToList() ---trocar o perfil
            ddlAlmoxConsumo.DataBind();
        }

        public void PopularPeriodo()
        {
            DateTime data = DateTime.Now;

            ddlPeriodoAte.Items.Clear();
            ddlPeriodoDeMov.Items.Clear();
            ddlPeriodoDeConsumo.Items.Clear();

            ddlPeriodoDe.Items.Add("- Selecione -");
            ddlPeriodoDe.Items[0].Value = "0";

            ddlPeriodoDeMov.Items.Add("- Selecione -");
            ddlPeriodoDeMov.Items[0].Value = "0";

            ddlPeriodoDeConsumo.Items.Add("- Selecione -");
            ddlPeriodoDeConsumo.Items[0].Value = "0";

            for (DateTime dat = new DateTime(Convert.ToInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.RefInicial.Substring(0, 4)),
                Convert.ToInt32(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.RefInicial.Substring(4, 2)), 1); dat < DateTime.Now; dat = dat.AddMonths(1))
            {
                ddlPeriodoDe.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoDe.Items[ddlPeriodoDe.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                //ddlPeriodoAte.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                //ddlPeriodoAte.Items[ddlPeriodoAte.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                ddlPeriodoDeMov.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoDeMov.Items[ddlPeriodoDeMov.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                //ddlPeriodoAteMov.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                //ddlPeriodoAteMov.Items[ddlPeriodoAteMov.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                ddlPeriodoDeConsumo.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoDeConsumo.Items[ddlPeriodoDeConsumo.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                //ddlPeriodoAteConsumo.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                //ddlPeriodoAteConsumo.Items[ddlPeriodoAteConsumo.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
            }

        }

        public void PopularPeriodo(string MesAno)
        {

            ddlPeriodoAte.Items.Clear();
            ddlPeriodoAteMov.Items.Clear();
            ddlPeriodoAteConsumo.Items.Clear();
           

            DateTime DiaMesAno = Convert.ToDateTime(MesAno).AddMonths(12);

            DiaMesAno = DiaMesAno > DateTime.Now ? DateTime.Now : DiaMesAno;

            for (DateTime dat = Convert.ToDateTime(MesAno); dat < DiaMesAno; dat = dat.AddMonths(1))
            {

                ddlPeriodoAte.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoAte.Items[ddlPeriodoAte.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                ddlPeriodoAteMov.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoAteMov.Items[ddlPeriodoAteMov.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');

                ddlPeriodoAteConsumo.Items.Add(string.Format("{0}/{1}", dat.Month.ToString().PadLeft(2, '0'), dat.Year.ToString().PadLeft(4, '0')));
                ddlPeriodoAteConsumo.Items[ddlPeriodoAteConsumo.Items.Count - 1].Value = dat.Year.ToString().PadLeft(4, '0') + dat.Month.ToString().PadLeft(2, '0');
            }

        }

        #endregion

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                if (ddlConsulta.SelectedValue == "1") // analítico
                {
                    DateTime dataInicial = new DateTime(Convert.ToInt32(ddlPeriodoDe.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDe.SelectedValue.Substring(4, 2)), 1);
                    DateTime dataFinal = new DateTime(Convert.ToInt32(ddlPeriodoAte.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAte.SelectedValue.Substring(4, 2)), 1);
                    dataFinal = dataFinal.AddMonths(1);

                    paramList.Add("UgeId", this.ddlUgeAnalitico.SelectedValue);
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("SubItemMaterialId", SubItemId);

                    if (!String.IsNullOrWhiteSpace(this.ddlUgeAnalitico.SelectedItem.Text))
                        paramList.Add("NomeUGE", this.ddlUgeAnalitico.SelectedItem.Text);

                    paramList.Add("DataInicial", dataInicial);
                    paramList.Add("DataFinal", dataFinal);

                    ReservaMaterialPresenter reserva = new ReservaMaterialPresenter();
                    var saldoReserva = reserva.CalcularSaldoReservaPorAlmoxSubMaterial(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, Convert.ToInt64(this.txtSubItem.Text), dataInicial, dataFinal);
                    paramList.Add("ReservaMatSaldo", saldoReserva);
                }
                if (ddlConsulta.SelectedValue == "2") // sintético
                {
                    paramList.Add("UgeId", this.ddlUgeSintetico.SelectedValue);
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("GrupoId", this.ddlGrupo.SelectedValue);
                    paramList.Add("NomeUGE", this.ddlUgeSintetico.SelectedItem.Text);
                    paramList.Add("ComSemSaldo", this.ddlSelSintetico.SelectedValue);
                    paramList.Add("Ordenacao", this.ddlOrdenar.SelectedValue);
                    paramList.Add("NomeGrupo", this.ddlGrupo.SelectedItem.Text);
                    paramList.Add("AgrupadoPorND", Convert.ToInt32(this.ddlGrupoND.SelectedValue));
                    paramList.Add("ExibirDetalhesLote", Convert.ToInt32(this.ddlDetalhesLote.SelectedValue));
                }
                if (ddlConsulta.SelectedValue == "3") // sintético por subitem
                {
                    DateTime dataInicial = new DateTime(Convert.ToInt32(ddlPeriodoDe.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDe.SelectedValue.Substring(4, 2)), 1);
                    DateTime dataFinal = new DateTime(Convert.ToInt32(ddlPeriodoAte.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAte.SelectedValue.Substring(4, 2)), 1);
                    dataFinal = dataFinal.AddMonths(1);

                    paramList.Add("UgeId", this.ddlUgeAnalitico.SelectedValue);
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("SubItemMaterialId", this.idSubItem.Value);
                    paramList.Add("NomeUGE", this.ddlUgeAnalitico.SelectedItem.Text);
                    paramList.Add("DataInicial", dataInicial);
                    paramList.Add("DataFinal", dataFinal);

                    ReservaMaterialPresenter reserva = new ReservaMaterialPresenter();
                    var saldoReserva = reserva.CalcularSaldoReservaPorAlmoxSubMaterial(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, Convert.ToInt64(this.txtSubItem.Text), dataInicial, dataFinal);
                    paramList.Add("ReservaMatSaldo", saldoReserva);
                }
                return paramList;
            }
        }

        public System.Collections.SortedList ParametrosRelatorioMovimentacao
        {
            get
            {
                DateTime dataInicial = new DateTime(Convert.ToInt32(ddlPeriodoDeMov.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDeMov.SelectedValue.Substring(4, 2)), 1);
                DateTime dataFinal = new DateTime(Convert.ToInt32(ddlPeriodoAteMov.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAteMov.SelectedValue.Substring(4, 2)), 1);
                DateTime dataInicialReport = new DateTime(Convert.ToInt32(ddlPeriodoDeMov.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDeMov.SelectedValue.Substring(4, 2)), 1);
                DateTime dataFinalReport = new DateTime(Convert.ToInt32(ddlPeriodoAteMov.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAteMov.SelectedValue.Substring(4, 2)), 1);
                dataFinal = dataFinal.AddMonths(1);

                SortedList paramList = new SortedList();
                if (ddlConsultaMov.SelectedValue == "1") // documentos de entrada
                {
                    ddlDivisaoMov.SelectedIndex = 0;
                    paramList.Add("TipoMovAgrupamentoId", (int)GeralEnum.TipoMovimentoAgrupamento.Entrada);
                }
                if (ddlConsultaMov.SelectedValue == "2") // documentos de saída
                {
                    ddlFornecedorMov.SelectedIndex = 0;
                    paramList.Add("TipoMovAgrupamentoId", (int)GeralEnum.TipoMovimentoAgrupamento.Saida);
                }
                if (ddlConsultaMov.SelectedValue == "3") // documentos de transferencia (entradas e saídas)
                {
                    paramList.Add("TipoMovAgrupamentoId", null);
                    paramList.Add("AlmoxarifadoLogado", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao);
                }

                paramList.Add("TipoMovimentoId", 0);
                paramList.Add("GestorId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id);
                paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                paramList.Add("FornecedorId", ddlFornecedorMov.SelectedValue);
                paramList.Add("DivisaoId", ddlDivisaoMov.SelectedValue);
                paramList.Add("DataInicial", dataInicial);
                paramList.Add("DataFinal", dataFinal);
                return paramList;
            }
        }

        public System.Collections.SortedList ParametrosRelatorioConsumo
        {
            get
            {
                DateTime dataInicial = new DateTime(Convert.ToInt32(ddlPeriodoDeConsumo.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDeConsumo.SelectedValue.Substring(4, 2)), 1);
                DateTime dataFinal = new DateTime(Convert.ToInt32(ddlPeriodoAteConsumo.SelectedValue.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAteConsumo.SelectedValue.Substring(4, 2)), 1);
                dataFinal = dataFinal.AddMonths(1);

                SortedList paramList = new SortedList();
                if (ddlConsultaConsumo.SelectedValue == "1") // consumo por almoxarifado
                {
                    ddlDivisaoConsumo.SelectedIndex = 0;
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("AlmoxNome", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao);
                }
                if (ddlConsultaConsumo.SelectedValue == "2") // consumo por requisitante/divisão
                {
                    ddlAlmoxConsumo.SelectedIndex = 0;
                    paramList.Add("DivisaoId", ddlDivisaoConsumo.SelectedValue);
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("AlmoxNome", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao);
                    paramList.Add("RequisitanteNome", ddlDivisaoConsumo.SelectedItem.Text);
                }
                if (ddlConsultaConsumo.SelectedValue == "3") // Consumo por Subitem x Almox
                {
                    int iDifMeses = Convert.ToDateTime(dataInicial).MonthDiff(Convert.ToDateTime(dataFinal));

                    ddlDivisaoConsumo.SelectedIndex = 0;
                    paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
                    paramList.Add("AlmoxNome", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao);
                    paramList.Add("SubitemCodigo", txtSubItemC.Text.Trim());
                    paramList.Add("DifMeses", iDifMeses);
                }

                paramList.Add("GestorId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id);
                paramList.Add("DataInicial", dataInicial);
                paramList.Add("DataFinal", dataFinal);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

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

        public string TipoMovimentacao
        {
            set
            {
                ListItem item = ddlConsultaMov.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlConsultaMov.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlConsultaMov.ClearSelection();
            }
            get
            {
                return ddlConsultaMov.SelectedValue;
            }
        }

        public string TipoConsumo
        {
            set
            {
                ListItem item = ddlConsultaConsumo.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlConsultaConsumo.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlConsultaConsumo.ClearSelection();
            }
            get
            {
                return ddlConsultaConsumo.SelectedValue;
            }
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public void PopularGrid()
        {
            throw new NotImplementedException();
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

        #region Bloquear Controles Movimento

        public bool BloqueiaNovo { set { bool a = value; } }
        public bool BloqueiaGravar { set { bool a = value; } }
        public bool BloqueiaExcluir { set { bool a = value; } }
        public bool BloqueiaCancelar { set { bool a = value; } }
        public bool BloqueiaImprimir { set { bool a = value; } }
        public bool BloqueiaAjuda { set { bool a = value; } }
        public bool BloqueiaCodigo { set { bool a = value; } }

        #endregion

        public bool MostrarPainelEdicao { set { bool a = value; } }

        public string SubItemMaterialAnalitico
        {
            get { return txtSubItem.Text; }
            set { txtSubItem.Text = value; }
        }

        public int OrgaoId { get; set; }

        public int? SubItemId
        {
            get
            {
                return Common.Util.TratamentoDados.TryParseInt32(idSubItem.Value);
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
        }


        protected void btnCancelar_Click(object sender, EventArgs e)
        {
        }

        protected void btnCancelarMov_Click(object sender, EventArgs e)
        {

        }

        private bool ValidarDatas()
        {
            bool isValid = true;

            if (ddlPeriodoDeConsumo.Visible && ddlPeriodoDeConsumo.SelectedIndex == -1)
            {
                this.ExibirMensagem("A data Periodo De Consumo está vazia");
                isValid = false;
            }
            else
            {
                if (ddlPeriodoAteConsumo.Visible && ddlPeriodoAteConsumo.SelectedIndex == -1)
                {
                    this.ExibirMensagem("A data Periodo Até Consumo está vazia");
                    isValid = false;
                }
            }

            return isValid;
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            if (ValidarDatas())
            {
                ConsultasPresenter consulta = new ConsultasPresenter(this);

                switch (ddlConsulta.SelectedValue)
                {
                    case "1": consulta.ImprmirEstoqueAnalitico();
                        break;
                    case "2": consulta.Imprimir(); //ConsultaEstoqueSintetico
                        break;
                    case "3": consulta.ImprmirFichaPrateleira();
                        break;

                    case "":
                    case null:
                        RetornarMsgConsultaInvalida();
                        break;
                }
            }
        }

        private void RetornarMsgConsultaInvalida()
        {
            List<string> lstErro = new List<string>();
            lstErro.Add("Selecione o tipo de consulta!");
            ListaErros = lstErro;
        }

        public bool BloqueiaDescricao
        {
            set { throw new NotImplementedException(); }
        }

        protected void ddlConsultaMov_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (ddlConsultaMov.SelectedValue)
            {
                case "1":
                    lblSelecionarMov.Visible = true;
                    lblSelecionarMov.Text = "Fornecedor";
                    ddlFornecedorMov.CssClass = "mostrarControle";
                    ddlDivisaoMov.CssClass = "esconderControle";
                    break;

                case "2":
                    lblSelecionarMov.Visible = true;
                    lblSelecionarMov.Text = "Divisão";
                    ddlFornecedorMov.CssClass = "esconderControle";
                    ddlDivisaoMov.CssClass = "mostrarControle";
                    break;

                case "3":
                    ddlDivisaoMov.CssClass = "esconderControle";
                    ddlFornecedorMov.CssClass = "esconderControle";
                    lblSelecionarMov.Visible = false;
                    break;
                case "4":
                    break;
                case "5":
                    break;
                default:
                    lblSelecionarMov.Visible = false;
                    ddlFornecedorMov.CssClass = "esconderControle";
                    ddlDivisaoMov.CssClass = "esconderControle";
                    break;
            }
        }


        public void ExibirRelatorioEstoqueAnalitico()
        {
            throw new NotImplementedException();
        }

        protected void ddlConsulta_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlConsulta.SelectedValue)
            {
                case "1":
                    PesquisaSubitem1.FiltrarAlmox = false;
                    sintetica.Visible = false;
                    analitica.Visible = true;
                    break;
                case "3":
                    sintetica.Visible = false;
                    analitica.Visible = true;
                    PesquisaSubitem1.FiltrarAlmox = true;
                    break;
                case "2":
                    sintetica.Visible = true;
                    analitica.Visible = false;
                    PesquisaSubitem1.FiltrarAlmox = true;
                    break;
            }

            btnImprimir.Enabled = true;
            InicializarCombos();
        }

        private void InicializarCombos()
        {
            ZerarCombos();
            PopularDadosUGETodosCod();
            PopularDadosFornecedorTodosCod();
            PopularDadosDivisaoTodosCod();
            PopularDadosGrupoTodosCod();
            PopularDadosAlmoxTodosCod();
            PopularPeriodo();
            PopularComboConsumo();
        }

        private void ZerarCombos()
        {
            this.txtSubItem.Text = string.Empty;
            this.txtSubItemC.Text = string.Empty;
            this.ddlUgeAnalitico.Items.Clear();
            this.ddlUgeSintetico.Items.Clear();
            this.ddlFornecedorMov.Items.Clear();
            this.ddlDivisaoConsumo.Items.Clear();
            this.ddlDivisaoMov.Items.Clear();
            this.ddlGrupo.Items.Clear();
            this.ddlOrdenar.SelectedIndex = 0;
            this.ddlSelSintetico.SelectedIndex = 0;

            this.ddlPeriodoAte.Items.Clear();
            this.ddlPeriodoAteConsumo.Items.Clear();
            this.ddlPeriodoAteMov.Items.Clear();
            this.ddlPeriodoDe.Items.Clear();
            this.ddlPeriodoDeConsumo.Items.Clear();
            this.ddlPeriodoDeMov.Items.Clear();

            this.ddlAlmoxConsumo.Items.Clear();

            this.ddlConsultaConsumo.Items.Clear();
        }

        protected void btnImprimirMovimentacao_Click(object sender, EventArgs e)
        {
            ConsultasPresenter consulta = new ConsultasPresenter(this);
            if (ddlConsultaMov.SelectedValue != "0")
            {
                consulta.ImprmirMovimento();
            }
            else
            {
                List<string> lstErro = new List<string>();
                lstErro.Add("Selecione o tipo de movimentação!");
                ListaErros = lstErro;
            }
        }

        protected void ddlConsultaConsumo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlConsultaConsumo.SelectedValue == "1")
            {
                ddlAlmoxConsumo.CssClass = "mostrarControle";
                ddlDivisaoConsumo.CssClass = "esconderControle";
                idConsumo.Visible = false;
                idConsumoSubItem.Visible = false;
                lblConsumoTipo.Text = "Almoxarifado:";
            }
            else
            {
                if (ddlConsultaConsumo.SelectedValue == "2")
                {
                    ddlAlmoxConsumo.CssClass = "esconderControle";
                    ddlDivisaoConsumo.CssClass = "mostrarControle";
                    idConsumo.Visible = true;
                    idConsumoSubItem.Visible = false;
                    lblConsumoTipo.Text = "Requisitante:";
                }
                else if (ddlConsultaConsumo.SelectedValue == "3")
                {
                    ddlAlmoxConsumo.CssClass = "esconderControle";
                    ddlDivisaoConsumo.CssClass = "esconderControle";
                    idConsumo.Visible = false;
                    idConsumoSubItem.Visible = true;
                    lblConsumoTipo.CssClass = "esconderControle";
                    lblConsumoTipo.Text = "";
                }               
                else
                {
                    List<string> lstErro = new List<string>();
                    lstErro.Add("Selecione o tipo de movimentação!");
                    ListaErros = lstErro;
                }
            }

        }

        protected void btnImprimirConsumo_Click(object sender, EventArgs e)
        {
            ConsultasPresenter consulta = new ConsultasPresenter(this);
            consulta.ImprmirConsumo();
        }

        protected void txtSubItem_TextChanged(object sender, EventArgs e)
        {
            this.PopularDadosUGETodosCod();
        }


        public bool AgrupadoPor
        {
            get
            {
                if (ddlNaturezaDespesa.SelectedValue == "1")
                    return true;
                else
                    return false;
            }
        }

        protected void ddlPeriodoDe_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularPeriodo(ddlPeriodoDe.SelectedItem.Text);
        }

        protected void ddlPeriodoDeMov_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularPeriodo(ddlPeriodoDeMov.SelectedItem.Text);
        }

        protected void ddlPeriodoDeConsumo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularPeriodo(ddlPeriodoDeConsumo.SelectedItem.Text);
        }

       
    }
}
