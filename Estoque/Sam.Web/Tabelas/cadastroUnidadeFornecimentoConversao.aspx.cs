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
using Sam.Entity;



namespace Sam.Web.Seguranca
{
    public partial class cadastroUnidadeFornecimentoConversao : PageBase, IUnidadeFornecimentoConversaoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
                objPresenter.Load();

                MostrarPainelEdicao = true;
                PopularGrid();
            }

            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

            //MostrarPainelEdicao = true;
            //BloqueiaDescricao = false;
            //BloqueiaGravar = false;
            btnNovo.Enabled = true;
            txtDescricao.Enabled = true;
            gridUnidadeFornecimentoConversao.Visible = true;
            
        }
        
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }
        public void PopularGrid()
        {
            //gridUnidadeFornecimentoConversao.DataSourceID = "sourcegridUnidadeFornecimentoConversao";
            UnidadeFornecimentoConversaoPresenter     objPresenter = new UnidadeFornecimentoConversaoPresenter();
            IList<UnidadeFornecimentoConversaoEntity> lstRetorno = objPresenter.ListarUnidadeFornecimentodeConversao(this.GestorId);

            if (!lstRetorno.IsNull() && lstRetorno.Count > 0)
            {
                this.gridUnidadeFornecimentoConversao.DataSource = lstRetorno;
                this.gridUnidadeFornecimentoConversao.DataMember  = "Id";
                this.gridUnidadeFornecimentoConversao.DataBind();
            }
            else
            {
                
            }

            this.gridUnidadeFornecimentoConversao.Visible = true;
        }

        public string Codigo
        {
            get
            {
                return txtCodigo.Text;
            }
            set
            {
                txtCodigo.Text = value;
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

        public int OrgaoId
        {
            get
            {
                var almoxarifadoLogado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                
                int _orgaoId = -1;

                if (!almoxarifadoLogado.Orgao.IsNull())
                    _orgaoId = almoxarifadoLogado.Orgao.Id.Value;
                else if (!almoxarifadoLogado.Gestor.Orgao.IsNull())
                    _orgaoId = almoxarifadoLogado.Gestor.Orgao.Id.Value;
                else
                    _orgaoId = GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id.Value;

                return _orgaoId;
            }
        }

        public int GestorId
        {
            get
            {
                int _gestorId = -1;
                var almoxarifadoLogado = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                
                if (!almoxarifadoLogado.Gestor.IsNull())
                    _gestorId = almoxarifadoLogado.Gestor.Id.Value;

                return _gestorId;
            }
        }

        public string Descricao
        {
            get
            {
                return txtDescricao.Text;
            }
            set
            {
                txtDescricao.Text = value;
            }
        }

        public int UnidadeFornecimentoSistemaSamId
        {
            get
            {
                return TratamentoDados.TryParseInt32(ddlUnidadeFornecimentoSistemaSam.SelectedValue).Value;
            }
            set
            {
                ddlUnidadeFornecimentoSistemaSam.SelectedValue = value.ToString();
            }
        }

        public int UnidadeFornecimentoSistemaSiafisicoId
        {
            get
            {
                return TratamentoDados.TryParseInt32(ddlUnidadeFornecimentoSistemaSiafisico.SelectedValue).Value;
            }
            set
            {
                ddlUnidadeFornecimentoSistemaSiafisico.SelectedValue = value.ToString();
            }
        }

        public decimal FatorUnitario
        {
            get
            {
                return TratamentoDados.TryParseDecimal(txtFatorUnitario.Text).Value;
            }
            set
            {
                txtFatorUnitario.Text = value.ToString();
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

        public bool BloqueiaCodigo
        {
            set
            {
                this.txtCodigo.Enabled = value;
            }
        }

        public bool BloqueiaDescricao
        {
            set
            {
                this.txtDescricao.Enabled = value;
            }
        }

        public SortedList ParametrosRelatorio { get { return new SortedList(); } }
        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region IEstruturaOrganizacionalView Members
        
        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }
        
        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public void PopularListaUnidadeFornecimentoSistemaSAM(int orgaoId, int gestorId)
        {
            UnidadeFornecimentoConversaoPresenter lObjPresenter = new UnidadeFornecimentoConversaoPresenter();
            List<UnidadeFornecimentoEntity>       lstRetorno    = new List<UnidadeFornecimentoEntity>();

            lstRetorno = lObjPresenter.ListarUnidadeFornecimentoSistemaSAM(this.OrgaoId).ToList();

            if (!lstRetorno.IsNull() && lstRetorno.Count > 0)
            {
                this.ddlUnidadeFornecimentoSistemaSam.DataSource = lstRetorno;
                this.ddlUnidadeFornecimentoSistemaSam.DataValueField = "Id";
                this.ddlUnidadeFornecimentoSistemaSam.DataTextField = "Descricao";
                this.ddlUnidadeFornecimentoSistemaSam.DataBind();
            }
            else
            {
                this.ddlUnidadeFornecimentoSistemaSam.Items.Add(new ListItem(""));
            }
        }

        public void PopularListaUnidadeFornecimentoSistemaSiafisico()
        {
            UnidadeFornecimentoConversaoPresenter lObjPresenter = new UnidadeFornecimentoConversaoPresenter();
            List<UnidadeFornecimentoSiafEntity>   lstRetorno    = new List<UnidadeFornecimentoSiafEntity>();

            lstRetorno = lObjPresenter.ListarUnidadeFornecimentoSistemaSIAFISICO().ToList();

            if (!lstRetorno.IsNull() && lstRetorno.Count > 0)
            {
                this.ddlUnidadeFornecimentoSistemaSiafisico.DataSource = lstRetorno;
                this.ddlUnidadeFornecimentoSistemaSiafisico.DataValueField = "Codigo";
                this.ddlUnidadeFornecimentoSistemaSiafisico.DataTextField = "Descricao";
                this.ddlUnidadeFornecimentoSistemaSiafisico.DataBind();
            }
            else
            {
                this.ddlUnidadeFornecimentoSistemaSiafisico.Items.Add(new ListItem(""));
            }
        }

        #endregion
                
        protected void gridUnidadeFornecimentoConversao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rowGrid = gridUnidadeFornecimentoConversao.Rows[gridUnidadeFornecimentoConversao.SelectedIndex];

            this.Id                               = gridUnidadeFornecimentoConversao.DataKeys[gridUnidadeFornecimentoConversao.SelectedIndex].Value.ToString();
            this.Codigo                           = Server.HtmlDecode(rowGrid.Cells[1].Text);
            this.Descricao                        = Server.HtmlDecode(rowGrid.Cells[2].Text);
            this.FatorUnitario                    = Decimal.Parse(Server.HtmlDecode(rowGrid.Cells[5].Text));

            if (!String.IsNullOrWhiteSpace(rowGrid.Cells[7].Text))
            {
                ListItem lstItem = null;

                //this.UnidadeFornecimentoSistemaSamId = Int32.Parse(rowGrid.Cells[7].Text);
                //this.ddlUnidadeFornecimentoSistemaSam.SelectedValue = this.UnidadeFornecimentoSistemaSamId.ToString();
                lstItem = this.ddlUnidadeFornecimentoSistemaSam.Items.FindByValue(rowGrid.Cells[7].Text);

                if (!lstItem.IsNull())
                    this.ddlUnidadeFornecimentoSistemaSam.SelectedValue = lstItem.Value;
                else
                {
                    this.ListaErros = new List<string>( new string[] {"Unidade de Fornecimento associada, não cadastrada para gestor/orgão do almoxarifado!"} );
                }
            }

            if (!String.IsNullOrWhiteSpace(rowGrid.Cells[6].Text))
            {
                ListItem lstItem = null;

                //this.UnidadeFornecimentoSistemaSiafisicoId = Int32.Parse(rowGrid.Cells[6].Text);
                //this.ddlUnidadeFornecimentoSistemaSiafisico.SelectedValue = this.UnidadeFornecimentoSistemaSiafisicoId.ToString();
                lstItem = this.ddlUnidadeFornecimentoSistemaSiafisico.Items.FindByValue(rowGrid.Cells[6].Text);

                if (!lstItem.IsNull())
                    this.ddlUnidadeFornecimentoSistemaSiafisico.SelectedValue = lstItem.Value;
                else
                {
                    this.ListaErros = new List<string>(new string[] { "Unidade de Fornecimento associada, não cadastrada para gestor/orgão associado ao almoxarifado!" });
                }
            }

            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.RegistroSelecionado();
        }

        protected void btnAjuda_Click(object sender, EventArgs e)
        {
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.Novo();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.Cancelar();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            UnidadeFornecimentoConversaoPresenter objPresenter = new UnidadeFornecimentoConversaoPresenter(this);
            objPresenter.Imprimir();
            
        }       
    }
}
