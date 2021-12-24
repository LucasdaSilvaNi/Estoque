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
using tipoPerfil = Sam.Common.Util.GeralEnum.TipoPerfil;


namespace Sam.Web.Almoxarifado
{
    public partial class RelatoriosMensais : PageBase, IRelatoriosMensaisView
    {

        private MovimentoEntity movimento = new MovimentoEntity();
        private const string sessaoMov = "movimento";

        public void RaisePostBackEvent(string eventArgument) { }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopularListaRelatorios(null);
                PopularListaAlmoxLogado();
                PopularPeriodo();
                PopularAnoReferencia();
            }
        }

        private void HabilitarConsulta(bool valorAcao)
        {
            ddlPeriodoDe.Enabled = valorAcao;
            ddlPeriodoAte.Enabled = valorAcao;
            btnImprimir.Enabled = valorAcao;
        }

        private void HabilitarPeriodo(int IdAlmoxarifado)
        {
            switch (IdAlmoxarifado)
            {
                case (int)RelatorioEnum.BalanceteAnual:
                    lblData.Visible = false;
                    lblDataAnual.Visible = true;

                    ddlAnoRef.Visible = true;
                    ddlAnoRef.Enabled = true;

                    ddlPeriodoDe.Visible = false;
                    ddlPeriodoDe.Enabled = false;
                    break;
                default:
                    lblData.Visible = true;
                    lblDataAnual.Visible = false;

                    ddlAnoRef.Visible = false;
                    ddlAnoRef.Enabled = false;

                    ddlPeriodoDe.Visible = true;
                    ddlPeriodoDe.Enabled = true;
                    break;
            }
        }

        #region Bloquear Controles

        #endregion

        #region Popular Combos
        public void PopularListaAlmoxLogado()
        {
            var AlmoxarifadoLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
            List<AlmoxarifadoEntity> lstAlmoxarifados = new List<AlmoxarifadoEntity>();
            AlmoxarifadoPresenter almox = new AlmoxarifadoPresenter();
            lstAlmoxarifados.Add(almox.SelecionarAlmoxarifadoPorGestor(AlmoxarifadoLogado.Id.Value));
            ddlLstAlmox.Items.Clear();
            ddlLstAlmox.DataTextField = "Descricao";
            ddlLstAlmox.DataValueField = "Id";
            //ddlLstAlmox.DataSource = almox.AlmoxarifadoTodosCod(1,GetAcesso().Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            ddlLstAlmox.DataSource = lstAlmoxarifados;
            ddlLstAlmox.DataBind();
        }

        public void PopularListaAlmoxPorOrgaoLogado()
        {
            int orgaoId = (int)(new PageBase().GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id);
            List<AlmoxarifadoEntity> lstAlmoxarifados = new List<AlmoxarifadoEntity>();
            AlmoxarifadoPresenter almox = new AlmoxarifadoPresenter();
            lstAlmoxarifados.Add(new AlmoxarifadoEntity() { Id = 0, Descricao = "TODOS" });
            lstAlmoxarifados.AddRange(almox.ListarAlmoxarifadosPorOrgao(orgaoId));
            ddlLstAlmox.Items.Clear();
            ddlLstAlmox.DataTextField = "Descricao";
            ddlLstAlmox.DataValueField = "Id";
            //ddlLstAlmox.DataSource = almox.AlmoxarifadoTodosCod(1,GetAcesso().Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            ddlLstAlmox.DataSource = lstAlmoxarifados;
            ddlLstAlmox.DataBind();
        }

        public void PopularListaRelatorios(bool? newMethod)
        {
            ddlRelatorios.Items.Clear();

            ddlRelatorios.Items.Add(new ListItem("Balancete Mensal", "40", true));
            ddlRelatorios.Items.Add(new ListItem("Balancete Anual", "56", true));
            ddlRelatorios.Items.Add(new ListItem("Inventário", "47", true));
            ddlRelatorios.Items.Add(new ListItem("Analítico", "43", true));


            int PerfilId = new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            switch (PerfilId)
            {
                case (int)tipoPerfil.AdministradorGeral:
                case (int)tipoPerfil.AdministradorGestor:
                case (int)tipoPerfil.AdministradorOrgao:
                case (int)tipoPerfil.ConsultaRelatorio:
                    ddlRelatorios.Items.Add(new ListItem("Exportação Custos", "55", true));
                    ddlRelatorios.Items.Add(new ListItem("Exportação Custos de Consumo Imediato", "63", true));
                    break;
                default:
                    break;
            }


            //desativados
            ddlRelatorios.Items.Add(new ListItem("Balancete (Consumo)", "41", false));
            ddlRelatorios.Items.Add(new ListItem("Balancete (Patrimônio)", "42", false));
            ddlRelatorios.Items.Add(new ListItem("Analítico (Consumo)", "44", false));
            ddlRelatorios.Items.Add(new ListItem("Analítico (Patrimônio)", "45", false));
            ddlRelatorios.Items.Add(new ListItem("Analítico por Grupo/Classe/Material", "46", false));
        }

        public void PopularListaRelatorios()
        {
            ddlRelatorios.Items.Clear();

            ddlRelatorios.Items.Add("Balancete");
            ddlRelatorios.Items[0].Value = "40";
            //ddlRelatorios.Items.Add("Balancete (Consumo)");
            //ddlRelatorios.Items[1].Value = "41";
            //ddlRelatorios.Items.Add("Balancete (Patrimônio)");
            //ddlRelatorios.Items[2].Value = "42";
            ddlRelatorios.Items.Add("Analítico");
            ddlRelatorios.Items[1].Value = "43";
            //ddlRelatorios.Items.Add("Analítico (Consumo)");
            //ddlRelatorios.Items[4].Value = "44";
            //ddlRelatorios.Items.Add("Analítico (Patrimônio)");
            //ddlRelatorios.Items[5].Value = "45";
            //ddlRelatorios.Items.Add("Analítico por Grupo/Classe/Material"); //O relatório foi desativado devido ao relacionamento Item_SubItem_Material que pode duplicar itens
            //ddlRelatorios.Items[2].Value = "46";
            ddlRelatorios.Items.Add("Inventário");
            ddlRelatorios.Items[2].Value = "47";
        }

        public void PopularPeriodo()
        {
            int almoxId = Int32.Parse(ddlLstAlmox.SelectedValue);
            ListItem itemLista = null;

            ddlPeriodoAte.Items.Clear();
            ddlPeriodoDe.Items.Clear();

            AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();
            FechamentoMensalPresenter fechamentoPresenter = new FechamentoMensalPresenter();

            AlmoxarifadoEntity almox = objPresenter.ObterAlmoxarifado(almoxId);
            IList<string> mesesFechados = fechamentoPresenter.ListarFechamentosEfetuados(almoxId);
            string mesAberto = "";

            if (mesesFechados.HasElements())
            {
                foreach (var mesFechado in mesesFechados)
                {
                    itemLista = new ListItem(mesFechado, String.Format("{0}{1}", mesFechado.Substring(3, 4), mesFechado.Substring(0, 2)));

                    ddlPeriodoDe.Items.Add(itemLista);
                    ddlPeriodoAte.Items.Add(itemLista);
                }
                if (!String.IsNullOrEmpty(ddlRelatorios.SelectedValue) && (Convert.ToInt32(ddlRelatorios.SelectedValue)) == (int)RelatorioEnum.ExportacaoCustos)
                {
                    mesAberto = Convert.ToDateTime("01/" + itemLista.Text).AddMonths(1).ToString().Substring(3, 7);
                    ddlPeriodoDe.Items.Add(new ListItem(mesAberto, Convert.ToString((mesAberto.Substring(3, 4)) + (mesAberto.Substring(0, 2)))));
                }
                //HabilitarConsulta(true);
            }
            else
            {
                itemLista = new ListItem("ALMOXARIFADO NÃO POSSUI FECHAMENTOS MENSAIS EEFTUADOS", null);
                ddlPeriodoDe.Items.Add(itemLista);
                ddlPeriodoAte.Items.Add(itemLista);

                //HabilitarConsulta(false);
            }
        }

        public void PopularAnoReferencia()
        {
            int almoxId = Int32.Parse(ddlLstAlmox.SelectedValue);
            ListItem itemLista = null;

            ddlAnoRef.Items.Clear();

            AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();
            FechamentoMensalPresenter fechamentoPresenter = new FechamentoMensalPresenter();

            IList<string> mesesFechados = fechamentoPresenter.ListarFechamentosEfetuados(almoxId);
            string mesAberto = "";

            if (mesesFechados.HasElements())
            {
                foreach (var mesFechado in mesesFechados)
                {
                    if (mesFechado.Substring(0, 2) == "12")
                    {
                        itemLista = new ListItem(mesFechado.Substring(3, 4), mesFechado.Substring(3, 4));

                        ddlAnoRef.Items.Add(itemLista);
                    }
                }
            }
            else
            {
                itemLista = new ListItem("Para gerar o relatório de BALANCETE ANUAL é necessário fechar todos os meses de pelo menos 1 ano", null);
                ddlAnoRef.Items.Add(itemLista);
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
                string anoMesRef = ddlPeriodoDe.SelectedItem.Value;

                int? almoxId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                int? intGestorId = new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id;
                int? orgaoId = new PageBase().GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id;

                FechamentoMensalPresenter fecha = new FechamentoMensalPresenter();
                fecha.VerificarTransferenciasPendentes((int)almoxId, Convert.ToInt32(anoMesRef));
                fecha.VerificarSubitensInativos((int)almoxId);

                SortedList paramList = new SortedList();
                DateTime dataInicial = new DateTime(Convert.ToInt32(ddlPeriodoDe.SelectedItem.Value.Substring(0, 4)), Convert.ToInt32(ddlPeriodoDe.SelectedItem.Value.Substring(4, 2)), 1);
                DateTime dataFinal = new DateTime(Convert.ToInt32(ddlPeriodoAte.SelectedItem.Value.Substring(0, 4)), Convert.ToInt32(ddlPeriodoAte.SelectedItem.Value.Substring(4, 2)), 1);

                dataFinal.AddMonths(1);

                paramList.Add("AlmoxId", this.Almoxarifado.Value);
                paramList.Add("DataInicial", dataInicial);
                paramList.Add("DataFinal", dataFinal);
                paramList.Add("TituloRelatorio", "Balancete - mês: ");
                paramList.Add("TransacaoPendentes", fecha.GetPendenciasTransferencia().ToString().ToUpper());
                paramList.Add("SubitensInativos", fecha.SubitensInativosFechamento);

                Page.Session.Add("chkSaldoMaiorZero", this.chkSaldoMaiorZero.Checked);

                switch (Convert.ToInt32(ddlRelatorios.SelectedValue))
                {
                    case (int)RelatorioEnum.ExportacaoCustos:
                        paramList.Add("AnoMesRef", anoMesRef);
                        paramList.Add("GestorId", intGestorId);
                        break;
                    case (int)RelatorioEnum.ExportacaoCustosConsumoImediato:
                        paramList.Add("AnoMesRef", anoMesRef);
                        paramList.Add("GestorId", intGestorId);
                        paramList.Add("OrgaoId", orgaoId);
                        break;
                    case (int)RelatorioEnum.BalanceteAnual:
                        int mesExtracaoInicial;
                        int mesExtracaoFinal;
                        int mesAnterior;

                        mesExtracaoInicial = Convert.ToInt32(ddlAnoRef.SelectedItem.Value + "01");
                        mesExtracaoFinal = Convert.ToInt32(ddlAnoRef.SelectedItem.Value + "12");

                        mesAnterior = Convert.ToInt32((Convert.ToInt32(ddlAnoRef.SelectedItem.Value) - 1).ToString() + "12");

                        paramList.Add("MesExtracaoInicial", mesExtracaoInicial);
                        paramList.Add("MesExtracaoFinal", mesExtracaoFinal);
                        paramList.Add("almoxDescricao", ddlLstAlmox.SelectedItem);
                        paramList.Add("mesAnterior", mesAnterior);

                        break;
                    default:
                        break;
                }


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

        public IList ListaErros { set { } }

        #region Bloquear Controles Movimento

        public bool BloqueiaNovo { set { } }
        public bool BloqueiaGravar { set { } }
        public bool BloqueiaExcluir { set { } }
        public bool BloqueiaCancelar { set { } }
        public bool BloqueiaImprimir { set { } }
        public bool BloqueiaAjuda { set { } }
        public bool BloqueiaCodigo { set { } }

        #endregion

        public bool MostrarPainelEdicao { set { } }

        public int OrgaoId { get; set; }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            RelatoriosMensaisPresenter rel = new RelatoriosMensaisPresenter(this);
            rel.Imprimir();
        }

        public int? Almoxarifado
        {
            set
            {
                ListItem item = ddlLstAlmox.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlLstAlmox.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlLstAlmox.ClearSelection();
            }
            get
            {
                return TratamentoDados.TryParseInt32(ddlLstAlmox.SelectedValue);
            }
        }

        public string NomeRelatorio
        {
            set
            {
                ListItem item = ddlRelatorios.Items.FindByValue(value);
                if (item != null)
                {
                    ddlRelatorios.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlRelatorios.ClearSelection();
            }
            get
            {
                return ddlRelatorios.SelectedValue;
            }
        }


        public bool BloqueiaDescricao
        {
            set { throw new NotImplementedException(); }
        }

        protected void ddlLstAlmox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLstAlmox.SelectedIndex > -1)
                PopularPeriodo();
        }

        protected void ddlRelatorios_SelectedIndexChanged(object sender, EventArgs e)
        {
            //HabilitarConsulta(true);
            HabilitarPeriodo(Convert.ToInt32(ddlRelatorios.SelectedValue));
            PopularPeriodo();

            int idPerfil = new PageBase().GetAcesso.Transacoes.Perfis[0].IdPerfil;

            if (int.Parse(ddlRelatorios.SelectedValue) == (int)RelatorioEnum.ExportacaoCustos || int.Parse(ddlRelatorios.SelectedValue) == (int)RelatorioEnum.ExportacaoCustosConsumoImediato)
            {
                if (idPerfil == (int)GeralEnum.TipoPerfil.AdministradorGeral || idPerfil == (int)GeralEnum.TipoPerfil.AdministradorOrgao)
                {
                    PopularListaAlmoxPorOrgaoLogado();
                }
            }
            else {
                PopularListaAlmoxLogado();
            }
        }

        public IEnumerable<object> anosFechados { get; set; }
    }
}
