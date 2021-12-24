using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.Infrastructure;
using Sam.Common.Util;
using System.Data;

namespace Sam.Web.Controles
{
    public partial class PesquisaSubitemNova : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblFiltraGestor.Style.Add("display", "none");
            lblFiltrarAlmox.Style.Add("display", "none");
            lblUsaSaldo.Style.Add("display", "none");
            lblDivisaoId.Style.Add("display", "none");
            lblFiltrarNaturezasDespesaConsumoImediato.Style.Add("display", "none");
        }

        private bool _usaSaldo = false;
        public bool UsaSaldo
        {
            get
            {
                return _usaSaldo;
            }
            set
            {
                _usaSaldo = value;

                if (lblUsaSaldo != null)
                    lblUsaSaldo.Text = value.ToString();
            }
        }

        private bool _filtrarAlmox = false;
        public bool FiltrarAlmox
        {
            get
            {
                return _filtrarAlmox;
            }
            set
            {
                _filtrarAlmox = value;

                if (lblFiltrarAlmox != null)
                    lblFiltrarAlmox.Text = value.ToString();
            }
        }

        private int? _divisaoId = null;
        public int? DivisaoId
        {
            get
            {
                return _divisaoId;
            }
            set
            {
                _divisaoId = value;

                if (lblDivisaoId != null)
                    lblDivisaoId.Text = value.ToString();
            }
        }

        private bool _FiltraGestor = false; // Padrao False;
        public bool FiltraGestor
        {
            get
            {
                return _FiltraGestor;
            }
            set
            {
                _FiltraGestor = value;

                if (lblFiltraGestor != null)
                    lblFiltraGestor.Text = value.ToString();
            }
        }

        private bool _FiltrarNaturezasDespesaConsumoImediato = false;
        public bool FiltrarNaturezasDespesaConsumoImediato
        {
            get
            {
                return _FiltrarNaturezasDespesaConsumoImediato;
            }
            set
            {
                _FiltrarNaturezasDespesaConsumoImediato = value;

                if (lblFiltrarNaturezasDespesaConsumoImediato.IsNotNull())
                    lblFiltrarNaturezasDespesaConsumoImediato.Text = value.ToString();
            }
        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridItemMaterial.PageIndex = e.NewPageIndex;
            gridItemMaterial.DataSourceID = "sourceGridItemMaterial";
        }


        //public IList<TB_SUBITEM_MATERIAL> BuscarSubItemMaterial(int maximumRowsParameterName, int startRowIndexParameterName, string valor, string _FiltrarAlmox, string _UsaSaldo, string _FiltraGestor, string _DivisaoId)
        public IList<TB_SUBITEM_MATERIAL> BuscarSubItemMaterial(int maximumRowsParameterName, int startRowIndexParameterName, string valor, string _FiltrarAlmox, string _UsaSaldo, string _FiltraGestor, string _DivisaoId, string _FiltrarNaturezasDespesaConsumoImediato)
        {
            bool pesquisaRequisicaoSaida = false; // Indica se a tela que chamou a pesquisa é a tela de requisição e/ou saída
            bool dispRequisicao = false; // Filtro para pegar apenas subitens disponíveis para requisição
            string url = HttpContext.Current.Request.Url.AbsolutePath;

            if (!String.IsNullOrEmpty(_FiltrarAlmox))
                //FiltrarAlmox = Convert.ToBoolean(_FiltrarAlmox);

                if (!String.IsNullOrEmpty(_UsaSaldo))
                    UsaSaldo = Convert.ToBoolean(_UsaSaldo);

            //if (!String.IsNullOrEmpty(_FiltraGestor))
            //   FiltraGestor = Convert.ToBoolean(_FiltraGestor);

            if (!String.IsNullOrEmpty(_DivisaoId))
                DivisaoId = Convert.ToInt32(_DivisaoId);

            if (!String.IsNullOrEmpty(_FiltrarNaturezasDespesaConsumoImediato))
                FiltrarNaturezasDespesaConsumoImediato = Convert.ToBoolean(_FiltrarNaturezasDespesaConsumoImediato);

            if (url.Contains("RequisicaoMaterial.aspx") || url.Contains("SaidaMaterial.aspx"))
                pesquisaRequisicaoSaida = true;
            if (url.Contains("RequisicaoMaterial.aspx"))
                dispRequisicao = true;
            if (url.Contains("RequisicaoMaterial.aspx") || url.Contains("Consultas.aspx") || url.Contains("gerenciaCatalogo.aspx") || url.Contains("SaidaMaterial.aspx"))
                FiltrarAlmox = true;
            //Para requisição de material saída e consulta não filtrar pelo gestor
            if (url.Contains("cadastroSubItemMaterial.aspx") || url.Contains("EntradaMaterial.aspx"))// || url.Contains("gerenciaCatalogo.aspx")) // 
            { FiltraGestor = true;
            FiltrarAlmox = true; }// possiveil solucao aqui - verificar como a gerencia de catalogo chama e seta true ou false para variavel filtrar gestor
            else
             FiltraGestor = false; 

            var presenter = new SubItemMaterialPresenter();           
            var result = presenter.BuscarSubItemMaterial(startRowIndexParameterName, valor, FiltrarAlmox, UsaSaldo, FiltraGestor, DivisaoId, pesquisaRequisicaoSaida, dispRequisicao, FiltrarNaturezasDespesaConsumoImediato);
            this.TotalRegistrosGrid = presenter.TotalRegistrosGrid;
            return result;
        }

        int TotalRegistrosGrid = 0;
        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, string valor, string _FiltrarAlmox, string _UsaSaldo, string _FiltraGestor, string _DivisaoId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int maximumRowsParameterName, int startRowIndexParameterName, string valor, string _FiltrarAlmox, string _UsaSaldo, string _FiltraGestor, string _DivisaoId, string _FiltrarNaturezasDespesaConsumoImediato)
        {
            return this.TotalRegistrosGrid;
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {
            gridItemMaterial.DataSourceID = "sourceGridItemMaterial";
           

        }
     
        public void SetFocusOnLoad()
        {
            txtChave.Focus();
        }

        protected void gridItemMaterial_SelectedIndexChanging(object sender, GridViewSelectEventArgs e)
        {
            gridItemMaterial.DataSourceID = "sourceGridItemMaterial";
        }

    }
}
