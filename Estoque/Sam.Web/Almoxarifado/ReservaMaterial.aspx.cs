using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Almoxarifado
{
    public partial class ReservaMaterial : PageBase, View.IReservaMaterialView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PesquisaSubitem1.UsaSaldo = true;
            PesquisaSubitem1.FiltrarAlmox = true;

            if (!IsPostBack)
            {
                ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
                reservaMaterial.Cancelar();
                reservaMaterial.Load();
            }

            PopularUGE_List();
        }
        
        public bool MostrarPainelEdicao
        {
            set
            {                
                this.pnlEditar.Visible = value;                
            }
        }

        //public void PopularListaUge()
        //{
        //    ReservaMaterialPresenter reservaMaterialPresenter = new ReservaMaterialPresenter();
        //    ddlUGE.DataSource = reservaMaterialPresenter.PopularListaUge(Convert.ToInt32(this.ddlSubItemMaterial.SelectedItem.Value));
        //    ddlUGE.DataBind();
        //}

        public void PopularListaSubItemMaterial(int? ItemId)
        {
        }
        
        public void PopularListaIndicadorAtividade()
        {
        }

        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                //paramList.Add("NomeUsuario", Session["UserLogged"].ToString());
                //paramList.Add("NomeGestor", Session["NameGestor"].ToString());

                paramList.Add("DescricaoUGE", this.ddlUGE.SelectedItem.Text);
                
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }
         
        public string UgeId
        {
            get
            {
                return ddlUGE.SelectedValue;
            }
            set
            {
                ddlUGE.SelectedValue = value;
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

     
        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
            }
        }
                
        public bool BloqueiaNovo
        {
            set
            {
                this.btnNovo.Enabled = value;
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                this.btnGravar.Enabled = value;
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                this.btnExcluir.Enabled = value;
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                this.btnCancelar.Enabled = value;
            }
        }
      
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }
   
        protected void btnNovo_Click(object sender, EventArgs e)
        {
            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.Novo();
            ddlUGE.Items.Clear();
            //ddlSubItemMaterial.Items.Clear();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.Gravar();
            PopularGrid();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.Imprimir();
        }  

        public string SubItemMaterialId
        {
            get
            {
                return idSubItem.Value;
            }
            set
            {

                
            }
        }

        public string AlmoxarifadoId
        {
            get
            {
                string retorno="";
                if (Session["Almoxarifado"] != null)
                    retorno = Session["Almoxarifado"].ToString();
                return retorno;
            }
            set
            {
                Session["Almoxarifado"] = value;
            }
        }

        public string Quantidade
        {
            get
            {
                return txtQuantidade.Text;
            }
            set
            {
                txtQuantidade.Text = value.ToString();
            }
        }

        public string Data
        {
            get
            {
                return txtObs.Text;
            }
            set
            {
                txtObs.Text = value;
            }
        }

        public string Obs
        {
            get
            {
                return txtObs.Text;
            }
            set
            {
                txtObs.Text = value;
            }
        }

        public string ItemMaterialId
        {
            get
            {
                return txtSubItem.Text;
            }
            set
            {
                txtSubItem.Text = value;
            }
        }

        public void PopularListaReservaMaterial()
        {
            throw new NotImplementedException();
        }

        public void PopularListaReservaMaterial(int UgeId, int AlmoxarifadoId)
        {
            throw new NotImplementedException();
        }

        protected void gridReservaMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
        
            //Id
            this.Id = gridReservaMaterial.DataKeys[gridReservaMaterial.SelectedIndex].Value.ToString();

            //Item
            if (gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblItem") != null)
            {
                Label lbl = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblItem");
                this.ItemMaterialId = Server.HtmlDecode(lbl.Text);
            }
            //Sub Item
            if (gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblSubItem") != null)
            {
                Label lbl = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblSubItem");
                Label lblId = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblSubItemId");
                this.SubItemMaterialId = Server.HtmlDecode(lblId.Text);
                //this.ddlSubItemMaterial.Items.Clear();
                //this.ddlSubItemMaterial.Items.Add(new ListItem(Server.HtmlDecode(lbl.ToolTip.ToString()), Server.HtmlDecode(lblId.Text)));
            }

            //Uge
            if (gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblUge") != null)
            {
                Label lbl = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblUge");
                this.UgeId = Server.HtmlDecode(lbl.Text);
                this.ddlUGE.Items.Clear();
                this.ddlUGE.Items.Add(new ListItem(Server.HtmlDecode(lbl.ToolTip.ToString()), Server.HtmlDecode(lbl.Text)));
            }

            //Quantidade
            if (gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblQuantidade") != null)
            {
                Label lbl = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblQuantidade");
                this.Quantidade = Server.HtmlDecode(lbl.Text);
            }

            //Observação
            if (gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblObs") != null)
            {
                Label lbl = (Label)gridReservaMaterial.Rows[gridReservaMaterial.SelectedIndex].FindControl("lblObs");
                this.Obs = Server.HtmlDecode(lbl.Text);
            }

            ReservaMaterialPresenter reservaMaterial = new ReservaMaterialPresenter(this);
            reservaMaterial.RegistroSelecionado();
            txtSubItem.Focus();
        }


        string IReservaMaterialView.UgeId
        {

            get
            {
                return ddlUGE.SelectedValue;
            }
            set
            {

                ListItem item = ddlUGE.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;
                }
            }
        }


        public void PopularGrid()
        {
            gridReservaMaterial.DataSourceID = "sourceGridReservaMaterial";
        }

        public void PopularUge()
        {
            Domain.Entity.UGEEntity uge = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoPadrao.Uge;
            if (uge == null)
            {
                List<string> listaErros = new List<string>();
                listaErros.Add("O almoxarifado que você está logado não possui UGE!");
                ListInconsistencias.ExibirLista(listaErros);
                ListInconsistencias.DataBind();
            }
            ddlUGE.Items.Add(new ListItem(uge.Descricao, uge.Id.Value.ToString()));
        }

        public string Codigo
        {
            set;
            get;
        }

        public string Descricao
        {
            set;get;
        }



        public bool BloqueiaCodigo
        {
            set; get;
        }

        public bool BloqueiaDescricao
        {
            set; get;
        }

        public void PopularListaReservaMaterial(int AlmoxarifadoId)
        {
            throw new NotImplementedException();
        }

        protected void imgLupaItemMaterial_Click(object sender, ImageClickEventArgs e)
        {
            SetSession(hidtxtItemMaterialId, "itemMaterialId");
            //SetSession(txtItem, "itemMaterialCodigo");
            //SetSession(ddlSubItemMaterial, "lstSubItemMaterial");
            //SetSession<int>(this.UgeId, "ugeId");
            //SetSession<int>(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, "almoxId");            
        }


        protected void ddlSubItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUGE_List();
        }

        private void PopularUGE_List()
        {
            if (! string.IsNullOrEmpty(SubItemMaterialId))
            {
                int subItem = Convert.ToInt32(SubItemMaterialId);
                ReservaMaterialPresenter reservaMaterialPresenter = new ReservaMaterialPresenter();
                var lista = reservaMaterialPresenter.PopularListaUge(subItem);
                ddlUGE.DataValueField = "Id";
                ddlUGE.DataTextField = "Descricao";
                ddlUGE.DataSource = lista;
                ddlUGE.DataBind();
                if (lista.Count() == 1)
                {
                    List<string> listaUge = ddlUGE.SelectedItem.Text.Split(':').ToList();
                    if (listaUge.Count() == 2)
                        txtQuantidade.Text = listaUge[1].ToString().Trim();
                }
            }
            else
            {
                //ddlSubItemMaterial.SelectedIndex = 0;
            }

        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> listaUge = ddlUGE.SelectedItem.Text.Split(':').ToList();
            if (listaUge.Count() == 2)
                txtQuantidade.Text = listaUge[1].ToString().Trim();
        }

        protected void ddlSubItemMaterial_DataBound(object sender, EventArgs e)
        {
            PopularUGE_List();
        }

     }
}
