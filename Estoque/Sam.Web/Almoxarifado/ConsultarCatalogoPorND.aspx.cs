using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Presenter;
using Sam.View;
using Sam.Common.Util;
using Sam.Domain.Entity;




namespace Sam.Web
{
    public partial class ConsultarCatalogoPorND : PageBase, ISubItemMaterialView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
                classe.LoadConsulta();
                PopularListaNaturezaDespeza();
            }

            HabilitarBotoes();
        }

        #region IEntidadesAuxiliaresView Members

        public void PopularGrid()
        {
            gridSubItemMaterial.DataSourceID = "odsGridSubItemMaterial";
        }

        public void PopularListaOrgao()
        {

        }

        public void PopularListaGestor()
        {
        }

        public void PopularListaGrupo()
        {
        }

        public void PopularListaClasse()
        {
        }

        public void PopularListaMaterial()
        {
        }

        public void PopularListaItem()
        {
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        public void PopularListaNaturezaDespeza()
        {
            SubItemMaterialPresenter lObjPresenter = new SubItemMaterialPresenter(this);
            IList<NaturezaDespesaEntity> lstNaturezaDespesa = lObjPresenter.PopularNaturezaDespesaComSubItem(Int32.Parse(GestorId));

            ddlNatureza.Items.Clear();

            ListItem todos = new ListItem();
            todos.Value = "0";
            todos.Text = "- Todas -";

            ddlNatureza.Items.Add("- Selecione -");
            ddlNatureza.Items.Add(todos);
            ddlNatureza.AppendDataBoundItems = true;
            ddlNatureza.DataTextField = "CodigoDescricao";
            ddlNatureza.DataValueField = "Id";

            ddlNatureza.DataSource = lstNaturezaDespesa;
            ddlNatureza.DataBind();
        }

        public void PopularListaContaAuxiliar()
        {
        }

        public void PopularListaUnidadeFornecimento()
        {
        }

        public string Codigo
        {
            get;
            set;
        }

        public string ItemId
        {
            get;
            set;
        }

        public string GestorId
        {
            get
            {
                int iGestor_ID = 0;

                var perfilLogado = new PageBase().GetAcesso.Transacoes.Perfis[0];
                var gestorAlmoxarifadoLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;

                if (gestorAlmoxarifadoLogado.HasValue)
                    iGestor_ID = gestorAlmoxarifadoLogado.Value;
                else
                    iGestor_ID = perfilLogado.GestorPadrao.Id.Value;

                return iGestor_ID.ToString();
            }

            set { }
        }

        public string Id
        {
            get;
            set;
        }

        public string Descricao
        {
            get;
            set;
        }

        public string GrupoId
        {
            get;
            set;
        }

        public string CodigoBarras
        {
            get;
            set;
        }

        public IList ListaErros
        {
            set
            {
                if (value != null)
                {
                    this.ListInconsistencias.ExibirLista(value);
                    this.ListInconsistencias.DataBind();
                }
            }
        }

        public bool BloqueiaNovo
        {
            get;
            set;
        }

        public bool BloqueiaGravar
        {
            get;
            set;
        }

        public bool BloqueiaExcluir
        {
            get;
            set;
        }

        public bool BloqueiaCancelar
        {
            get;
            set;
        }

        public bool BloqueiaCodigo
        {
            get;
            set;
        }

        public bool BloqueiaDescricao
        {
            get;
            set;
        }

        public bool BloqueiaCodBarras
        {
            get;
            set;
        }

        public bool BloqueiaNaturezaDespesa
        {
            get;
            set;
        }

        public bool BloqueiaContaAuxiliar
        {
            get;
            set;
        }

        public bool BloqueiaControlaLote
        {
            get;
            set;
        }

        public bool BloqueiaExpandeDecimais
        {
            get;
            set;
        }

        public bool BloqueiaPermiteFacionamento
        {
            get;
            set;
        }

        public bool BloqueiaAtividade
        {
            get;
            set;
        }

        public bool BloqueiaUnidadeFornecimento
        {
            get;
            set;
        }

        public bool MostrarPainelEdicao
        {
            get;
            set;
        }

        #endregion

        #region Eventos

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridSubItemMaterial.DataSourceID = "odsGridSubItemMaterial";
        }

        #region ISubItemMaterialView Members

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                var descAlmoxarifado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Descricao;
                var iAlmoxarifado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;

                SortedList paramList = new SortedList();
                paramList.Add("NaturezaDespesa_ID", ddlNatureza.SelectedIndex == 1 ? "0" : ddlNatureza.SelectedValue.ToString());
                paramList.Add("DescNatDespesa", ddlNatureza.SelectedIndex == 1 ? "Todas as Naturezas de Despesa" : String.Format("Natureza de Despesa: {0}", ddlNatureza.SelectedItem));
                paramList.Add("DescAlmoxarifado", descAlmoxarifado);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            if (ddlNatureza.SelectedIndex == 0)
            {
                List<string> lstErros = new List<string>(new string[] { "Favor selecionar uma Natureza de Despesa da lista, ou a opção 'Todas'" });
                this.ListaErros = lstErros;
                this.ListInconsistencias.Visible = true;
            }
            else
            {
                SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
                //subitensMaterial.ImprimirConsultaSubitensPorND();
                subitensMaterial.ImprimirConsulta();
            }
        }

        #endregion

        #region Propriedades

        public string NaturezaDespesaId
        {
            get { return ddlNatureza.SelectedItem.Value; }
            set { this.ddlNatureza.SelectedIndex = Int32.Parse(value); }
        }

        public string ContaAuxiliarId
        {
            get;
            set;
        }

        public int UnidadeFornecimentoId
        {
            get;
            set;
        }

        public bool IndicadorAtividadeId
        {
            get;
            set;
        }

        public bool ControlaLote
        {
            get;
            set;
        }

        public bool ExpandeDecimos
        {
            get;
            set;
        }

        public bool PermiteFracionamento
        {
            get;
            set;
        }

        #endregion

        #region Metodos

        private void Cancelar()
        {
            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.Cancelar();
        }

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        private void HabilitarBotoes()
        {
        }


        #endregion

        public int IndicadorDisponivelId
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

        public decimal EstoqueMinimo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public decimal EstoqueMaximo
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IndicadorAtividadeAlmox
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

        public void PopularListaIndicardorDisponivel()
        {
            throw new NotImplementedException();
        }

        protected void ddlUnidadeFornecimento_DataBound(object sender, EventArgs e)
        {

        }

        void ISubItemMaterialView.LimparPesquisaItem()
        {
            this.ItemPesquisado = new ItemMaterialEntity();
        }

        protected void ddlNatureza_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ListaErros = null;
            this.ListInconsistencias.Visible = false;

            PopularGrid();
        }

        protected void btnGerenciaCatalogo_Click(object sender, EventArgs e)
        {
            string[] strCaminhoPrincipal = HttpContext.Current.Request.Url.AbsoluteUri.BreakLine("/".ToCharArray());
            Response.Redirect(String.Format("{0}//{1}/{2}/{3}", strCaminhoPrincipal[0], strCaminhoPrincipal[1], strCaminhoPrincipal[2], "GerenciaCatalogo.aspx"), true);
        }

        public bool FiltraAlmoxarifado
        {
            get { return false; }
        }

        public bool FiltraGestor
        {
            get { return false; }
        }

        public bool ComSaldo
        {
            get { return false; }
        }


        public bool BloqueiaListaOrgao
        {
            set { }
        }

        public bool BloqueiaListaUO
        {
            set { }

        }

        public bool BloqueiaListaUGE
        {
            set { }
        }
    }
}
