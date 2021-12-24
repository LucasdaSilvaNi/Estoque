using Sam.Domain.Entity;
using Sam.Presenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sam.Web.Financeiro
{
    public partial class cadastroEventoSiafemPatrimonial : System.Web.UI.Page
    {

        string usuario = new PageBase().GetAcesso.Cpf;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ListarTipoMovimento();
                CarregarEvento();


            }

        }



        public void ListarTipoMovimento()
        {
            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter();
            ddlTipoMovimento.Items.Insert(0, new ListItem("Selecione", "0"));
            ddlTipoMovimento.AppendDataBoundItems = true;
            ddlTipoMovimento.DataValueField = "Id";
            ddlTipoMovimento.DataTextField = "CodigoFormatado";
            ddlTipoMovimento.DataSource = presenter.ListarTipoMovimentoAtivoNl().OrderBy(a => a.CodigoFormatado).ToList();

            ddlTipoMovimento.DataBind();



        }

        //public void ListarSubTipoMovimento()
        //{
        //    TipoMovimentoPresenter presenterx = new TipoMovimentoPresenter();
        //    lldSubTipoMovimento.Items.Insert(0, new ListItem("Selecione", "0"));
        //    lldSubTipoMovimento.AppendDataBoundItems = true;
        //    lldSubTipoMovimento.DataValueField = "Id";
        //    lldSubTipoMovimento.DataTextField = "Descricao";
        //    lldSubTipoMovimento.DataSource = presenterx.ListarSubTipoMovimento().OrderBy(a => a.Descricao).ToList();

        //    lldSubTipoMovimento.DataBind();



        //}


        public SubTipoMovimentoEntity ListarInserirSubTipoMovimento(SubTipoMovimentoEntity objSubTipo)
        {
            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter();
            return presenter.ListarInserirSubTipoMovimento(objSubTipo);

        }

        public string EventoTipoMaterial { get; set; }
        public string EventoTipoEntradaSaidaReclassificacaoDepreciacao { get; set; }
        public string EventoTipoEstoque { get; set; }
        public string EventoEstoque { get; set; }
        public string EventoTipoMovimentacao { get; set; }
        public string EventoTipoMovimento { get; set; }
        public bool Ativo { get; set; }
        public int SubTipoMovimentoId { get; set; }
        public SubTipoMovimentoEntity SubTipoMovimento { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }

        public void GravarEvento()
        {
            EventoSiafemEntity itemSiafem = new EventoSiafemEntity();
            SubTipoMovimentoEntity itemSubTipo = new SubTipoMovimentoEntity();
            string msg;






            itemSubTipo.Descricao = txtTipoEntrada.Text;
            itemSubTipo.Ativo = true;
            itemSubTipo.TipoMovimentoId = Convert.ToInt32(ddlTipoMovimento.SelectedValue);
            var retornoSub = ListarInserirSubTipoMovimento(itemSubTipo);

            if (retornoSub != null)
            {
                itemSiafem.Ativo = true;
                itemSiafem.EventoTipoMaterial = ddlTipoEstoque.SelectedValue == "PERMANENTE" ? "MaterialPermanente" : "MaterialConsumo"; ;
                itemSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao = txtTipoEntrada.Text;
                itemSiafem.EventoTipoEstoque = ddlTipoEstoque.SelectedValue;
                itemSiafem.EventoTipoMovimentacao = txtTipoMovimentacao.Text;
                itemSiafem.EventoTipoMovimento = ddlTipoNl.SelectedValue;
                itemSiafem.EventoEstoque = ddlTipoEstoque.SelectedValue == "PERMANENTE" ? "ESTOQUE LONGO PRAZO" : "ESTOQUE CURTO PRAZO";
                itemSiafem.DataInclusao = DateTime.Now;
                itemSiafem.SubTipoMovimentoId = Convert.ToInt32(retornoSub.Id);
                itemSiafem.DetalheAtivo = ckbDetalhe.Checked;
                itemSiafem.EstimuloAtivo = ckbEstimulo.Checked;
                itemSiafem.LoginAtivacao = usuario;

                EventoSiafemEntity itemRetorno = null;
                EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();

                itemRetorno = objPresenter.SalvarEventoSiafem(itemSiafem);

                if (itemRetorno.Id != null)
                {
                    if (itemRetorno.inseriu)
                    {

                        LimparCampo();
                        CarregarEvento();
                        msg = "Evento Gravado com sucesso";
                    }
                    else
                        msg = "Evento já existe, por favor deletar este, para inserir um novo!";
                }
                else
                    msg = "Erro ao gravar! Tente mais tarde.";
            }
            else
                msg = "Erro ao gravar! Tente mais tarde.";


            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + msg + "');", true);
        }


        public void LimparCampo()
        {
            ddlTipoNl.SelectedValue = "Selecione";
            ddlTipoEstoque.SelectedValue = "Selecione";
            ddlTipoMovimento.SelectedValue = "0";
            txtTipoMovimentacao.Text = string.Empty;
            txtTipoEntrada.Text = string.Empty;
            ckbEstimulo.Checked = false;

        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                GravarEvento();
            }
        }

        public void CarregarEvento()
        {
            List<EventoSiafemEntity> lisRetorno = new List<EventoSiafemEntity>();
            EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();

            lisRetorno = objPresenter.CarregarListaEventoPatrimonial().ToList();

            foreach (var item in lisRetorno)
            {
                item.TipoMovSAM = item.SubTipoMovimento.TipoMovimento.Descricao;
            }

            DataTable dt = ConvertToDataTable(lisRetorno);

            if (dt != null)
            {

                //Sort the data.
                dt.DefaultView.Sort = " EventoTipoMovimento, EventoTipoEntradaSaidaReclassificacaoDepreciacao, TipoMovSAM,EventoTipoMaterial  ASC";

                gdvEvento.DataSource = dt;
                gdvEvento.DataBind();
                Session["Eventos"] = dt.AsEnumerable().ToList();

            }

            //ViewState["Eventos"] = lisRetorno.ToList();
            //gdvEvento.DataSource = lisRetorno.ToList();
            //gdvEvento.DataBind();

        }

        protected void gdvEvento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvEvento.PageIndex = e.NewPageIndex;
            CarregarEvento();
        }


        protected void gdvEvento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string msg = string.Empty;
            try
            {
                EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();


                if (e.CommandName == "Delete")
                {
                    int index = Convert.ToInt32(e.CommandArgument);


                    if (objPresenter.InativarItemEventoSiafem(index, usuario))
                    {
                        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                        int RemoveAt = gvr.RowIndex;
                        List<DataRow> dt = new List<DataRow>();
                        dt = (List<DataRow>)Session["Eventos"];
                        dt.RemoveAt(RemoveAt);
                        Session["Eventos"] = dt;

                        msg = "Evento deletado com sucesso";
                    }
                }

                if (e.CommandName == "Alterar")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
                    int indextabela = gvr.RowIndex;


                    //AlterarItemEventoSiafem
                    //Update the values.



                    GridViewRow row = gdvEvento.Rows[indextabela];

                    HiddenField hdfEventoEntr = ((HiddenField)(row.FindControl("hdfEventoEntr")));
                    TextBox txtEventoEntr = ((TextBox)(row.FindControl("txtEventoEntr")));
                    TextBox txtTipoMoviment = ((TextBox)(row.FindControl("txtTipoMoviment")));
                    HiddenField hdfTipoMov = ((HiddenField)(row.FindControl("hdfTipoMov")));
                    HiddenField hdfSubTipo = ((HiddenField)(row.FindControl("hdfSubTipo")));
                    HiddenField hdfTipoMoviment = ((HiddenField)(row.FindControl("hdfTipoMoviment")));
                    CheckBox ckbEstimuloT =  ((CheckBox)(row.FindControl("ckbEstimuloT")));
                    HiddenField hdfEstimuloT = ((HiddenField)(row.FindControl("hdfEstimuloT")));

                    bool check = (ckbEstimuloT.Checked != (Convert.ToBoolean(hdfEstimuloT.Value)));

                    if ((hdfEventoEntr.Value.Trim() != txtEventoEntr.Text.Trim()) || (txtTipoMoviment.Text.Trim() != hdfTipoMoviment.Value.Trim()) || check)
                    {
                        SubTipoMovimentoEntity itemSubTipo = new SubTipoMovimentoEntity();
                        itemSubTipo.Descricao = txtEventoEntr.Text.Trim();
                        itemSubTipo.Ativo = true;
                        itemSubTipo.TipoMovimentoId = Convert.ToInt32(hdfTipoMov.Value);
                        
                        var retornoSub = ListarInserirSubTipoMovimento(itemSubTipo);

                        if (retornoSub != null)
                        {
                            if (objPresenter.AlterarItemEventoSiafem(index, usuario, txtEventoEntr.Text.Trim(), txtTipoMoviment.Text.Trim(), (int)retornoSub.Id, Convert.ToInt32(hdfSubTipo.Value), ckbEstimuloT.Checked))
                            {

                                //Reset the edit index.
                                gdvEvento.EditIndex = -1;

                                CarregarEvento();

                                msg = "Evento Gravado com sucesso";

                            }
                        }
                        else
                            msg = "Erro ao gravar! Tente mais tarde.";

                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            if (!string.IsNullOrEmpty(msg))
                ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + msg + "');", true);
        }


        protected void gdvEvento_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                CarregarEvento();
                //gdvEvento.DataSource = (List<DataRow>)Session["Eventos"];
                //gdvEvento.DataBind();
            }
            catch (Exception ex)
            {

            }
        }


        protected void gdvEvento_Sorting(object sender, GridViewSortEventArgs e)
        {

            //Retrieve the table from the session object.
            List<EventoSiafemEntity> list = ViewState["Eventos"] as List<EventoSiafemEntity>;


            DataTable dt = ConvertToDataTable(list);

            if (dt != null)
            {

                //Sort the data.
                dt.DefaultView.Sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
                gdvEvento.DataSource = dt;
                gdvEvento.DataBind();
            }

        }


        protected void gdvEvento_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gdvEvento.EditIndex = e.NewEditIndex;
            CarregarEvento();
        }


        private string GetSortDirection(string column)
        {

            // By default, set the sort direction to ascending.
            string sortDirection = "ASC";

            // Retrieve the last column that was sorted.
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                // Check if the same column is being sorted.
                // Otherwise, the default value can be returned.
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            // Save new values in ViewState.
            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        //protected void gdvEvento_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{

        //    int index = Convert.ToInt32(e.CommandArgument);

        //    EventosPagamentoPresenter objPresenter = new EventosPagamentoPresenter();
        //    if (objPresenter.InativarItemEventoSiafem(index, usuario))
        //    {
        //        GridViewRow gvr = (GridViewRow)(((ImageButton)e.CommandSource).NamingContainer);
        //        int RemoveAt = gvr.RowIndex;
        //        List<DataRow> dt = new List<DataRow>();
        //        dt = (List<DataRow>)Session["Eventos"];
        //        dt.RemoveAt(RemoveAt);
        //        Session["Eventos"] = dt;
        //    }


        //    //Retrieve the table from the session object.
        //    DataTable dt = (DataTable)Session["TaskTable"];

        //    //Update the values.
        //    GridViewRow row = gdvEvento.Rows[e.RowIndex];
        //    dt.Rows[row.DataItemIndex]["Id"] = ((TextBox)(row.Cells[1].Controls[0])).Text;
        //    dt.Rows[row.DataItemIndex]["Description"] = ((TextBox)(row.Cells[2].Controls[0])).Text;
        //    dt.Rows[row.DataItemIndex]["IsComplete"] = ((CheckBox)(row.Cells[3].Controls[0])).Checked;

        //    //Reset the edit index.
        //    gdvEvento.EditIndex = -1;

        //    //Bind data to the GridView control.
        //    //BindData();


        //}

        protected void gdvEvento_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gdvEvento.EditIndex = -1;
            CarregarEvento();
        }
    }
}