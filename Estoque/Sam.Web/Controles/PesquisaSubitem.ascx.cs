using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.Presenter;

namespace Sam.Web.Controles
{
    public partial class PesquisaSubitem : System.Web.UI.UserControl
    {
        private readonly string sessionSubItem = "sessionItem";
        //string listaSubItens = "subItens";
        string listaSubItens = "subItensParaFiltro";

        protected void Page_Load(object sender, EventArgs e)
        {
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
            }
        }



        public void PopularDadosGridItemMaterial()
        {
            //Presenter.SubItemMaterialPresenter presenter = new Presenter.SubItemMaterialPresenter();

           
            
            //var result = presenter.ListarSubItemAlmoxPorPalavraChave(0, 0, ddlTipoFiltro.SelectedValue, txtChave.Text.Trim(), FiltrarAlmox, DivisaoId, FiltraGestor);

            ////Filtrar somente os itens que tem saldo - Padrão UsaSaldo = false (Não usa saldo)
            //if (UsaSaldo && result != null)
            //{
            //    SubItemMaterialPresenter itemMaterialPresenter = new SubItemMaterialPresenter();
            //    IList<SubItemMaterialEntity> subItemSaldoByAlmox = itemMaterialPresenter.ListarSubItemSaldoByAlmox();
            //    result = result.Intersect(subItemSaldoByAlmox, new BaseEntityIEqualityComparer()).Cast<SubItemMaterialEntity>().ToList();
            //}

            //result = FiltrarListaPreExistente(result);

            //gridItemMaterial.PageSize = 20;
            //gridItemMaterial.DataSource = result;
            //gridItemMaterial.DataBind();
            //gridItemMaterial.PageIndex = 0;
            //PageBase.SetSession(result, sessionSubItem);
        }

        private IList<SubItemMaterialEntity> FiltrarListaPreExistente(IList<SubItemMaterialEntity> listaOriginal)
        {
            if (listaOriginal != null && listaOriginal.Count > 0)
            {
                string cstLoginAcesso  = "loginAcesso";
                string strLoginUsuario = string.Empty;

                IList<SubItemMaterialEntity> copiaListaOriginal = null;
                SubItemMaterialEntity[]      arrListaSombra     = null;

                copiaListaOriginal = listaOriginal;
                arrListaSombra     = new SubItemMaterialEntity[copiaListaOriginal.Count];
                strLoginUsuario    = PageBase.GetSession<string>(cstLoginAcesso);
                //listaSubItens      = String.Format("{0}{1}", listaSubItens, strLoginUsuario);

                List<MovimentoItemEntity> listaItensMovimento = PageBase.GetSession<List<MovimentoItemEntity>>(listaSubItens);

                if (listaItensMovimento != null && listaItensMovimento.Count > 0)
                {
                    listaOriginal.CopyTo(arrListaSombra, 0);
                    foreach (var SubItemMaterial in copiaListaOriginal.ToList())
                    {
                        foreach (var ItemMovimento in listaItensMovimento)
                        {
                            if (ItemMovimento.SubItemMaterial.Id == SubItemMaterial.Id)
                            {
                                listaOriginal.Remove(SubItemMaterial);
                                break;
                            }
                        }
                    }
                }

                PageBase.RemoveSession(listaSubItens);
            }

            return listaOriginal;
        }

        public void CarregarListasPorBusca()
        {
            ItemMaterialPresenter p = new ItemMaterialPresenter();
        }

        protected void btnProcurar_Click(object sender, EventArgs e)
        {
            PopularDadosGridItemMaterial();
        }

        protected void gridItemMaterial_PageIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridItemMaterial.DataSource = null;
            gridItemMaterial.DataBind();
            gridItemMaterial.EmptyDataText = "";
            txtChave.Text = "";
        }

        protected void gridItemMaterial_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridItemMaterial.PageIndex = e.NewPageIndex;            
            if (Session[sessionSubItem] != null)
            {
                gridItemMaterial.DataSource = PageBase.GetSession<List<SubItemMaterialEntity>>(sessionSubItem);
                gridItemMaterial.DataBind();
            }
        }

        public void SetFocusOnLoad()
        {
            txtChave.Focus();
        }

    }
}
