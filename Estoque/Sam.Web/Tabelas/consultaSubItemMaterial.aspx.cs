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
    public partial class consultaSubItemMaterial : PageBase, ISubItemMaterialView
    {
        ItemMaterialEntity ItemPesquisado = new ItemMaterialEntity();
        SubItemMaterialEntity SubItemPesquisado = new SubItemMaterialEntity();

        protected void Page_Load(object sender, EventArgs e)
        {
            Pesquisaitem.UsaSaldo = false;
            Pesquisaitem.FiltrarAlmoxarifadoLogado = false;
            PesquisaSubItem.FiltrarAlmox = true;
            PesquisaSubItem.UsaSaldo = false;

            if (!String.IsNullOrEmpty(itemMaterialId.Value))
            {
                SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
                ItemPesquisado = presenter.GetItemMaterialByItem((int)Common.Util.TratamentoDados.TryParseInt32(itemMaterialId.Value));
            }

            if (!string.IsNullOrEmpty(idSubItem.Value))
            {
                var gestorId = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
                long? codigoSubItem = Common.Util.TratamentoDados.TryParseLong(txtSubItem.Text.Trim());
                SubItemPesquisado = new SubItemMaterialPresenter().CarregarSubItem(codigoSubItem, gestorId);
            }

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
            var gestorId = new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value;            
            var naturezaDespesaCodigo = TratamentoDados.TryParseInt32(ddlNatureza.SelectedValue);
            int item = ItemPesquisado.Codigo;
            long subItem = SubItemPesquisado.Codigo;

            IList<SubItemMaterialEntity> listaDados = null;
            SubItemMaterialPresenter objPresenter = new SubItemMaterialPresenter(this);

            if (ddlNatureza.SelectedIndex == 0 && item == 0 && subItem == 0)
            {
                LimparGridviewSubItemMaterial(listaDados);                
            }
            else
            {
                listaDados = objPresenter.ListarCatalogoGestor(gestorId, naturezaDespesaCodigo, item, subItem);

                if (listaDados.Count() > 0)
                    gridSubItemMaterial.PageSize = listaDados.Count();

                gridSubItemMaterial.DataSource = listaDados;
                gridSubItemMaterial.DataBind();
                gridSubItemMaterial.Visible = true;
            }

            ShowImprimirButton();
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
            SubItemMaterialPresenter sub = new SubItemMaterialPresenter();
            ddlNatureza.Items.Clear();
            ddlNatureza.Items.Add("- Selecione -");
            ddlNatureza.Items.Add("- Todas -");
            ddlNatureza.AppendDataBoundItems = true;
            ddlNatureza.DataTextField = "CodigoDescricao";
            ddlNatureza.DataValueField = "Codigo";
            ddlNatureza.DataSource = sub.PopularNaturezaDespesaComSubItem(new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value);
            //ddlNatureza.DataSource = natureza.PopularDadosNaturezaTodosCod();
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
            get;
            set;
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

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Id = gridSubItemMaterial.DataKeys[gridSubItemMaterial.SelectedIndex].Value.ToString();

            SubItemMaterialPresenter classe = new SubItemMaterialPresenter(this);
            classe.SubItemId = int.Parse(this.Id);
            classe.RegistroSelecionado();
            HabilitarBotoes();
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
                SortedList paramList = new SortedList();
                //paramList.Add("NaturezaDespesa", ddlNatureza.SelectedIndex <= 0 ? "0" : ddlNatureza.SelectedValue.ToString());
                //paramList.Add("DescNatDespesa", ddlNatureza.SelectedIndex <= 0 ? "Todas as Naturezas de Despesa" : "Natureza de Despesa: " + ddlNatureza.SelectedItem);
                paramList.Add("NaturezaDespesa", ddlNatureza.SelectedIndex <= 1 ? "0" : ddlNatureza.SelectedValue.ToString());
                paramList.Add("DescNatDespesa", ddlNatureza.SelectedIndex == 1 ? "Todas as Naturezas de Despesa" : "Natureza de Despesa: " + ddlNatureza.SelectedItem);
                paramList.Add("ItemMaterialCodigo", ItemPesquisado.Codigo);
                paramList.Add("SubitemMaterialCodigo", SubItemPesquisado.Codigo);
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            if (ddlNatureza.SelectedIndex == 0 && ItemPesquisado.Codigo == 0 && SubItemPesquisado.Codigo == 0)
            {
                List<string> lstErros = new List<string>(new string[] { "Favor selecionar uma opção de filtro." });
                this.ListaErros = lstErros;
                this.ListInconsistencias.Visible = true;
            }
            else
            {
                SubItemMaterialPresenter subitensMaterial = new SubItemMaterialPresenter(this);
                subitensMaterial.ImprimirConsulta();
            }
        }



        protected void ddlNatureza_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ListaErros = null;
            this.ListInconsistencias.Visible = false;

            PopularGrid();
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            txtItem.Text = "";
            itemMaterialId.Value = "";

            txtSubItem.Text = "";
            idSubItem.Value = "";

            ddlNatureza.SelectedIndex = 0;

            LimparGridviewSubItemMaterial(new List<SubItemMaterialEntity>());

            ShowImprimirButton();
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            PopularGrid();
        }


        #endregion

        #region Propriedades

        public string NaturezaDespesaId
        {
            get;
            set;
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

        private void ShowImprimirButton()
        {
            btnImprimir.Visible = gridSubItemMaterial.Rows.Count > 0 ? true : false;
        }

        private void LimparGridviewSubItemMaterial(IList<SubItemMaterialEntity> lstDados)
        {
            gridSubItemMaterial.DataSource = lstDados;
            gridSubItemMaterial.DataBind();
            gridSubItemMaterial.Visible = false;
        }


        #endregion


        public int IndicadorDisponivelId
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
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
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }

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
