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
using System.Web.UI.HtmlControls;

namespace Sam.Web.Almoxarifado
{
    public partial class ConsultarCatalogo : PageBase, ISubItemMaterialView, ICombosPadroesView
    {

        #region Propriedades

        int PerfilId;
        int LoginId;

        public string ItemId
        {
            get { return ""; }
        }

        public int UnidadeFornecimentoId
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public string GestorId
        {
            get { return ""; }
        }

        public string NaturezaDespesaId
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string ContaAuxiliarId
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public bool IndicadorAtividadeId
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public string CodigoBarras
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public bool ControlaLote
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool ExpandeDecimos
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool PermiteFracionamento
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public SortedList ParametrosRelatorio
        {
            get { return new SortedList(); }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public bool IndicadorAtividadeAlmox
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public int IndicadorDisponivelId
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }


        public decimal EstoqueMaximo
        {
            get { return 0.0m; }
            set { }
        }

        public decimal EstoqueMinimo
        {
            get { return 0.0m; }
            set { }
        }

        public bool BloqueiaCodBarras
        {
            set { }
        }

        public bool BloqueiaNaturezaDespesa
        {
            set { }
        }

        public bool BloqueiaContaAuxiliar
        {
            set { }
        }

        public bool BloqueiaControlaLote
        {
            set { }
        }

        public bool BloqueiaExpandeDecimais
        {
            set { }
        }

        public bool BloqueiaPermiteFacionamento
        {
            set { }
        }

        public bool BloqueiaAtividade
        {
            set { }
        }

        public bool BloqueiaUnidadeFornecimento
        {
            set { }
        }

        public void LimparPesquisaItem()
        {

        }

        public string Id
        {
            get { return ""; }
        }

        public string Codigo
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string Descricao
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public int ID
        {
            get
            {
                int retorno = 0;
                if (Session["ID_Cat"] != null)
                    retorno = Convert.ToInt32(Session["ID_Cat"].ToString());
                return retorno;
            }
            set
            {
                Session["ID_Cat"] = value;
            }
        }

        public IList ListaErros
        {
            set { }
        }

        public bool BloqueiaNovo
        {
            set { }
        }

        public bool BloqueiaGravar
        {
            set { }
        }

        public bool BloqueiaExcluir
        {
            set { }
        }

        public bool BloqueiaCancelar
        {
            set { }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaDescricao
        {
            set { }
        }

        public bool MostrarPainelEdicao
        {
            set { }
        }

        string ICrudView.Id
        {
            get
            {
                return "";
            }
            set
            {
            }
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




        #endregion


        #region Controles ASPX

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.FindControl("pnlBarraGestor").Visible = false;
            AddSearchField(this.CombosHierarquiaPadrao1);                      
            this.CombosHierarquiaPadrao1.View = this;

            if (!IsPostBack)
            {
                SubItemMaterialPresenter requisicao = new SubItemMaterialPresenter(this);
                requisicao.Load();
            }
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SubItemMaterialPresenter requisicao = new SubItemMaterialPresenter(this);
            requisicao.Imprimir();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {

        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void ddlDivisao_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlDivisao_DataBound(object sender, EventArgs e)
        {            
        }

        protected void ddlUA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlUA_DataBound(object sender, EventArgs e)
        {
            SubItemMaterialPresenter requisicao = new SubItemMaterialPresenter(this);
            requisicao.Cancelar();
        }

        #endregion


        #region Métodos
        
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

        }

        public void PopularListaContaAuxiliar()
        {

        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
        }

        public void PopularListaUnidadeFornecimento()
        {

        }

        public void PopularGrid()
        {
            if (this.CombosHierarquiaPadrao1.DdlDivisao.Items.Count > 0)
            {
                int divisao = Convert.ToInt32(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue);
                TextBox txtPesquisar = (TextBox)this.CombosHierarquiaPadrao1.FindControl("txtPesquisar");                
                string pesquisa = txtPesquisar.Text.Trim();

                IList<Infrastructure.TB_SUBITEM_MATERIAL_ALMOX> lstItensAlmoxarifado = null;
                var objPresenter = new SubItemMaterialPresenter(this);
                lstItensAlmoxarifado = objPresenter.ListarCatalogoAlmox(divisao, pesquisa, false);

                if (!lstItensAlmoxarifado.IsNull() && lstItensAlmoxarifado.Count() > 0)
                    grdDocumentos.PageSize = lstItensAlmoxarifado.Count();

                grdDocumentos.DataSource = lstItensAlmoxarifado;
                grdDocumentos.DataBind();
            }
            else
            {
                this.ExibirMensagem("Favor selecionar a divisão.");
            }
        }

        public void ExibirMensagem(string _mensagem)
        {
        }

        private void AddSearchField(UserControl usercontrol)
        {
            #region InstanciaComponentes

            HtmlGenericControl divSearch = new HtmlGenericControl();
            divSearch.ID = "pSearch";
            divSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            divSearch.TagName = "p";

            HtmlGenericControl tableSearch = new HtmlGenericControl();
            tableSearch.ID = "tableSearch";
            tableSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            tableSearch.TagName = "table";
            tableSearch.Attributes["border"] = "0";
            tableSearch.Attributes["width"] = "100%";

            HtmlGenericControl trSearch1 = new HtmlGenericControl();
            trSearch1.TagName = "tr";

            HtmlGenericControl tdSearch1 = new HtmlGenericControl();
            tdSearch1.TagName = "td";
            tdSearch1.Attributes["width"] = "80%";
            tdSearch1.Attributes["style"] = "text-align:left;";

            Label labelSearch1 = new Label();
            labelSearch1.ID = "labelPesquisar";
            labelSearch1.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            labelSearch1.Attributes["class"] = "labelFormulario";            
            labelSearch1.Text = "Pesquisar:";

            Label literalSearch1 = new Label();
            literalSearch1.Text = "&nbsp";

            TextBox textboxSearch1 = new TextBox();
            textboxSearch1.ID = "txtPesquisar";
            textboxSearch1.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            textboxSearch1.Attributes["type"] = "text";
            textboxSearch1.Attributes["style"] = "height:20px; line-height:22px;";
            textboxSearch1.Attributes["style"] = "width:20%;";

            Button buttonSearch2 = new Button();
            buttonSearch2.ID = "btnPesquisar";
            buttonSearch2.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            buttonSearch2.Attributes["type"] = "submit";
            buttonSearch2.Attributes["style"] = "width:120px";
            buttonSearch2.Attributes["class"] = "button";
            buttonSearch2.Text = "Pesquisar";
            buttonSearch2.Click += btnPesquisar_Click;

            #endregion

            #region AdicionaComponentes

            tdSearch1.Controls.Add(labelSearch1);            
            tdSearch1.Controls.Add(textboxSearch1);
            tdSearch1.Controls.Add(literalSearch1);
            tdSearch1.Controls.Add(buttonSearch2);

            trSearch1.Controls.Add(tdSearch1);

            tableSearch.Controls.Add(trSearch1);

            Panel pElement = (Panel)usercontrol.Controls[1].FindControl("panelSearch");
            pElement.Attributes["class"] = "panelSearch";
            pElement.Controls.Add(tableSearch);
            
            //usercontrol.Controls.AddAt(26, tableSearch);

            #endregion
        }
        

        #endregion


        #region UserControlImplements

        public void PrepararVisaoDeCombosPorPerfil(int perfilId)
        {
            int idPerfil = perfilId;
            if (idPerfil != (int)Sam.Common.Perfil.REQUISITANTE_GERAL && idPerfil != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            {
                this.BloqueiaListaOrgao = true;
                this.BloqueiaListaUO = true;
                this.BloqueiaListaUGE = true;
            }

            else if (idPerfil == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
                this.BloqueiaListaOrgao = true;
            else
                this.BloqueiaListaOrgao = false;
        }

        public void PopularListaOrgao()
        {

        }

        public bool BloqueiaListaOrgao
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaOrgao = value; }
        }

        public bool BloqueiaListaUO
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUO = value; }
        }

        public bool BloqueiaListaUGE
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUGE = value; }
        }
        public bool BloqueiaListaUA
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaUA = value; }
        }

        public bool BloqueiaListaDivisao
        {
            set { this.CombosHierarquiaPadrao1.BloqueiaDivisao = value; }
        }

        public int OrgaoId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlOrgao.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlOrgao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                this.CombosHierarquiaPadrao1.DdlOrgao.SelectedValue = value.ToString();
            }
        }

        public int UOId
        {
            get
            {   
                if (this.CombosHierarquiaPadrao1.DdlUO.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUO.SelectedValue);
                else
                    return 0;
            }
            set
            {
                this.CombosHierarquiaPadrao1.DdlUO.SelectedValue = value.ToString();
            }
        }

        public int UGEId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlUGE.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUGE.SelectedValue);
                else
                    return 0;
            }
            set
            {
                this.CombosHierarquiaPadrao1.DdlUGE.SelectedValue = value.ToString();
            }
        }

        public int UAId
        {
            get
            {
                if (this.CombosHierarquiaPadrao1.DdlUA.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlUA.SelectedValue);
                else
                    return 0;
            }
            set
            {
                this.CombosHierarquiaPadrao1.DdlUA.SelectedValue = value.ToString();
            }

        }

        public int DivisaoId
        {
            get
            {   
                if (!String.IsNullOrEmpty(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue))
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                this.CombosHierarquiaPadrao1.DdlDivisao.SelectedValue = value.ToString();
            }
        }

        private bool _cascatearDDLOrgao = true;
        public bool CascatearDDLOrgao
        {
            get
            {
                return _cascatearDDLOrgao;
            }
            set
            {
                _cascatearDDLOrgao = value;
            }
        }

        private bool _cascatearDDLUO = true;
        public bool CascatearDDLUO
        {
            get
            {
                return _cascatearDDLUO;
            }
            set
            {
                _cascatearDDLUO = value;
            }
        }

        private bool _cascatearDDLUGE = true;
        public bool CascatearDDLUGE
        {
            get
            {
                return _cascatearDDLUGE;
            }
            set
            {
                _cascatearDDLUGE = value;
            }
        }

        private bool _cascatearDDLUA = true;
        public bool CascatearDDLUA
        {
            get
            {
                return _cascatearDDLUA;
            }
            set
            {
                _cascatearDDLUA = value;
            }
        }

        private bool _cascatearDDLAlmoxarifado = true;
        public bool CascatearDDLAlmoxarifado
        {
            get { return _cascatearDDLAlmoxarifado; }
            set { _cascatearDDLAlmoxarifado = value; }
        }

        public bool PreservarComboboxValues
        {
            get
            {
                return this.CombosHierarquiaPadrao1.PreservarComboboxValues;
            }
            set
            {
                this.CombosHierarquiaPadrao1.PreservarComboboxValues = value;
            }
        }
    
        #endregion
       
    }

}
