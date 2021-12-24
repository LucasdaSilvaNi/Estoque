using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.View;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Entity;
using Sam.Web.MasterPage;

namespace Sam.Web.Seguranca
{ 
    

    public partial class perfilAlmoxarifado : PageBase, IAlmoxarifadoView
    {
        public int AlmoxarifadoId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);

                if (almoxarifado.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
                    codigo.Text = almoxarifado.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.ToString();
                almoxarifado.Load();

            }

            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }
                
        #region IEntidadesAuxiliaresView Members

        public bool MostrarPainelEdicao
        {
            set
            {
                this.pnlEditar.Visible = value;
            }
        }

        public void PopularListaOrgao()
        {
            ddlOrgao.DataSourceID = "sourceListaOrgao";
        }
        
        public string OrgaoId
        {
            set
            {
                ListItem item = ddlOrgao.Items.FindByValue(value);
                if (item != null)
                {
                    ddlOrgao.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return Convert.ToString(ddlOrgao.SelectedIndex); }
        }

        public void PopularListaGestor(int _orgaoId)
        {
            ddlGestor.DataSourceID = "sourceListaGestor";
        }

        public string GestorId
        {
            set
            {
                ListItem item = ddlGestor.Items.FindByValue(value);
                if (item != null)
                {
                    ddlGestor.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlGestor.SelectedValue; }
        }
               
        public string EnderecoLogradouro
        {
            get { return txtLogradouro.Text; }
            set { txtLogradouro.Text = value; }
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

        public string Descricao
        {
            get { return txtNome.Text; }
            set { txtNome.Text = value; }
        }

        public string EnderecoNumero
        {
            get { return txtNumero.Text; }
            set { txtNumero.Text = value; }
        }

        public string EnderecoComplemento
        {
            get { return txtComplemento.Text; }
            set { txtComplemento.Text = value; }
        }

        public string EnderecoBairro
        {
            get { return txtBairro.Text; }
            set { txtBairro.Text = value; }
        }

        public string EnderecoMunicipio
        {
            get { return txtMunicipio.Text; }
            set { txtMunicipio.Text = value; }
        }

        public void PopularListaUf()
        {
            this.ddlUf.DataSourceID = "sourceListaUf";
            this.ddlUf.DataBind();
            this.ddlUf.Items.Insert(0, " - Selecione - ");
        }

        public string UfId
        {
            set
            {
                ListItem item = ddlUf.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUf.ClearSelection();
                    item.Selected = true;
                }
            }
            get { return ddlUf.SelectedValue; }
        }

        public string EnderecoCep
        {
            get { return txtCep.Text; }
            set { txtCep.Text = value; }
        }

        public string EnderecoTelefone
        {
            get { return txtTelefone.Text; }
            set { txtTelefone.Text = value; }
        }

        public string EnderecoFax
        {
            get { return txtFax.Text; }
            set { txtFax.Text = value; }
        }

        public string Responsavel
        {
            set
            {
                txtResponsavel.Text = value;
            }

            get { return txtResponsavel.Text; }
        }

        public void PopularListaUge()
        {
            this.ddlUGE.DataSourceID = "sourceListaUge";
            this.ddlUGE.DataBind();
            this.ddlUGE.Items.Insert(0, " - Selecione - ");
            if (this.ddlUGE.Items.Count <= 2)
               this.ddlUGE.Items[this.ddlUGE.Items.Count-1].Selected=true;
        }

        public void PopularListaAlmoxarifados()
        {
            throw new NotImplementedException();
        }

        public bool IsSubAlmoxarifado { get { return false; } set { } }
        public int? AlmoxarifadoVinculadoId { get { return 0; } set { } }
        public string CCCentroCusto { get; set; }

        public string UgeId
        {
            set
            {
                ListItem item = ddlUGE.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;

                }
            }

            get { return ddlUGE.SelectedValue; }
        }

        public string IndicadorAtividadeId
        {
            set
            {
                ListItem item = ddlIndicadorAtividade.Items.FindByValue(value);
                if (item != null)
                {
                    ddlIndicadorAtividade.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlIndicadorAtividade.SelectedValue;
            }
        }

        public bool PermiteIgnorarCalendarioSiafemParaReabertura { get; set; }

        public void PopularListaIndicadorAtividade()
        {
        }

        public string RefInicial
        {
            get { return txtRefInicial.Text; }
            set { txtRefInicial.Text = value; }
        }

        public string RefFaturamento
        {
            get { return txtRefFaturamento.Text; }
            set { txtRefFaturamento.Text = value; }
        }

        public string TipoAlmoxarifado
        {
            set
            {
                ListItem item = ddlTipoAlmoxarifado.Items.FindByValue(value);
                if (item != null)
                {
                    ddlTipoAlmoxarifado.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlTipoAlmoxarifado.SelectedValue;
            }
        }

        public void PopularGrid()
        {
            this.gridAlmoxarifado.DataSourceID = "sourceGridAlmoxarifado";
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
                
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                
            }
        }

        public bool BloqueiaExcluir
        {
            set
            {
                
            }
        }

        public bool BloqueiaCancelar
        {
            set
            {
               
            }
        }
                
        public bool BloqueiaCodigo
        {
            set
            {
               
            }
        }

        public bool BloqueiaDescricao
        {
            set{ } 
        }
               
        public bool BloqueiaEnderecoLogradouro
        {
            set {  }
        }

        public bool BloqueiaEnderecoNumero
        {
            set {  }
        }

        public bool BloqueiaEnderecoComplemento
        {
            set {  }
        }

        public bool BloqueiaEnderecoTelefone
        {
            set {}
        }

        public bool BloqueiaEnderecoMunicipio
        {
            set { }
        }

        public bool BloqueiaEnderecoBairro
        {
            set {  }
        }

        public bool BloqueiaRefInicial
        {
            set { }
        }

        public bool BloqueiaEnderecoFax
        {
            set {  }
        }

        public bool BloqueiaEnderecoCep
        {
            set {  }
        }
                
        
        public bool BloqueiaListaUge
        {
            set {  }
        }
        
        public bool BloqueiaListaIndicadorAtividade
        {
            set { }
        }

        public bool BloqueiaResponsavel
        {
            set { }
        }

        public bool BloqueiaListaUf
        {
            set {  }
        }

        
        public void  ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "confirm('" + _mensagem + "');", true);
        }


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
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
        
        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();
            this.PopularListaGestor((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }
        
        protected void ddlGestor_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();
            gridAlmoxarifado.PageIndex = 0;
            codigo.Text = almoxarifado.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.ToString();
            PopularGrid();
            this.PopularListaUge();

        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Cancelar();
        }       

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            almoxarifado.Imprimir();
        }

        protected void gridAlmoxarifado_SelectedIndexChanged(object sender, EventArgs e)
        {
            //AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
            //var acesso = GetAcesso;
            //acesso.Transacoes.Perfis[0].AlmoxarifadoLogado = almoxarifado.SelecionarAlmoxarifadoPorGestor(Convert.ToInt32(gridAlmoxarifado.SelectedValue)) as AlmoxarifadoEntity;
            //Response.Redirect("SelecionaAlmoxarifado.aspx");
            // Response.Write("alert('Almoxarifado e Gestor foram alterados com sucesso!');");
        }


        protected void gridAlmoxarifado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cmdSelecionar")
            {
                int almox = (Convert.ToInt32(e.CommandArgument));
                AlmoxarifadoPresenter almoxarifado = new AlmoxarifadoPresenter(this);
                GestorPresenter gestor = new GestorPresenter();
                OrgaoPresenter orgao = new OrgaoPresenter();
                var acesso = GetAcesso;
                
                acesso.Transacoes.Perfis[0].GestorPadrao = gestor.SelecionarRegistro(int.Parse(ddlGestor.SelectedValue));
                acesso.Transacoes.Perfis[0].OrgaoPadrao = orgao.LerRegistro(acesso.Transacoes.Perfis[0].GestorPadrao.Orgao.Id ?? 0);
                acesso.Transacoes.Perfis[0].AlmoxarifadoLogado = almoxarifado.SelecionarAlmoxarifadoPorGestor(almox) as AlmoxarifadoEntity;
                Response.Redirect("SelecionaAlmoxarifado.aspx", false);
            }
        }

        
    }
}
