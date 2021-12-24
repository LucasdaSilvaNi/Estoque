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

namespace Sam.Web.Seguranca
{
    public partial class cadastroDivisao : PageBase,  IDivisaoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DivisaoPresenter divisao = new DivisaoPresenter(this);
                divisao.Load();
                pnlEditar.CssClass = "esconderControle";

                PopularOrgao();
            }

            ScriptManager.RegisterStartupScript(this.txtTelefone, GetType(), "telefone", "$('.telefone').mask('(99)9999-9999');", true);
            ScriptManager.RegisterStartupScript(this.txtCep, GetType(), "cep", "$('.cep').mask('99999-999');", true);
            ScriptManager.RegisterStartupScript(this.txtCodigo, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
//            ScriptManager.RegisterStartupScript(this.txtArea, GetType(), "area", "$('.area').priceFormat({prefix: '',centsSeparator: ',',  thousandsSeparator: '.' }); ", true);

            
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");

          
        }
                
        #region IEntidadesAuxiliaresView Members

        public void PopularListaOrgao()
        {
            //ddlOrgao.DataSourceID = "sourceListaOrgao";
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
            get { return ddlOrgao.SelectedValue; }
        }

        public void PopularListaUA(int _orgaoId)
        {
            //ddlUA.DataSourceID = "sourceListaUA";
        }

        public string UAId
        {
            set
            {
                ListItem item = ddlUA.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUA.ClearSelection();
                    item.Selected = true;

                }
            }
            get { return ddlUA.SelectedValue; }
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

        public string NumeroFuncionarios
        {
            get { return txtNumeroFuncionarios.Text; }
            set { txtNumeroFuncionarios.Text = value; }
        }

        public string Area
        {
            get { return txtArea.Text; }
            set { txtArea.Text = value; }
        }

        public string AlmoxarifadoId
        {
            set
            {
                ListItem item = ddlAlmoxarifado.Items.FindByValue(value);
                if (item != null)
                {
                    ddlAlmoxarifado.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlAlmoxarifado.SelectedValue;
            }
        }

        public string ResponsavelId
        {
            set
            {
                ListItem item = ddlResponsavel.Items.FindByValue(value);
                if (item != null)
                {
                    ddlResponsavel.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                return ddlResponsavel.SelectedValue;
            }
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

        public void PopularGrid()
        {
            this.gridDivisao.DataSourceID = "sourceGridDivisao";
        }

        public void PopularListaUF()
        {
            //this.ddlUf.Items.Clear();
            this.ddlUf.DataSourceID = "sourceListaUF";

        }

        public void PopularListaUA()
        {
            this.ddlUA.DataSourceID = "sourceListaUA";
        }

        public void PopularListaResponsavel()
        {
            this.ddlResponsavel.Items.Clear();
            this.ddlResponsavel.Items.Insert(0, " - Selecione - ");
            this.ddlResponsavel.AppendDataBoundItems = true;
            int gestorId = 0;
            // armazena o gestor por UA
            if (ddlUA.SelectedIndex >= 0) 
            {
                UAPresenter uaPr = new UAPresenter();
                gestorId = uaPr.PopularDadosTodas().Where(a => a.Id == 
                    TratamentoDados.TryParseInt32(ddlUA.SelectedValue))
                    .Select(a => a.Gestor.Id.Value).FirstOrDefault();
            }

            if (gestorId != 0)
                txtGestor.Text = gestorId.ToString();

            this.ddlResponsavel.DataSourceID = "sourceListaResponsavel";
        }

        public void PopularListaAlmoxarifado()
        {
            this.ddlAlmoxarifado.Items.Clear();
            this.ddlAlmoxarifado.Items.Insert(0, " - Selecione - ");
            this.ddlAlmoxarifado.AppendDataBoundItems = true;
            
            // procurar o gestor da UA
            //txtGestor.Text = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.ToString();
            //ResponsavelPresenter resp = new ResponsavelPresenter();
            //resp.PopularDadosResponsavelPorOrgaoGestor
            //txtGestor.Text = ua.PopularDadosTodosCod(this.OrgaoId);

            this.ddlAlmoxarifado.DataSourceID = "sourceListaAlmoxarifado";
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

        public bool BloqueiaArea
        {
            set { this.txtArea.Enabled = value; }
        }

        public bool BloqueiaNumeroFuncionarios
        {
            set { this.txtNumeroFuncionarios.Enabled = value; }
        }

        public bool BloqueiaListaUA
        {
            set { this.ddlUA.Enabled = value; }
        }

        public bool BloqueiaListaUF
        {
            set { this.ddlUf.Enabled = value; }
        }

        public bool BloqueiaListaResponsavel
        {
            set { this.ddlResponsavel.Enabled = value; }
        }

        public bool BloqueiaListaAlmoxarifado
        {
            set { this.ddlAlmoxarifado.Enabled = value; }
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
            set{ this.txtNome.Enabled = value;} 
        }
               
        public bool BloqueiaEnderecoLogradouro
        {
            set { this.txtLogradouro.Enabled = value; }
        }

        public bool BloqueiaEnderecoNumero
        {
            set { this.txtNumero.Enabled = value; }
        }

        public bool BloqueiaEnderecoComplemento
        {
            set { this.txtComplemento.Enabled = value; }
        }

        public bool BloqueiaEnderecoTelefone
        {
            set { this.txtTelefone.Enabled = value; }
        }

        public bool BloqueiaEnderecoMunicipio
        {
            set { this.txtMunicipio.Enabled = value; }
        }

        public bool BloqueiaEnderecoBairro
        {
            set { this.txtBairro.Enabled = value; }
        }
                
        public bool BloqueiaEnderecoFax
        {
            set { this.txtFax.Enabled = value; }
        }

        public bool BloqueiaEnderecoCep
        {
            set { this.txtCep.Enabled = value; }
        }
        
        public bool BloqueiaListaIndicadorAtividade
        {
            set { this.ddlIndicadorAtividade.Enabled = value; }
        }
               
        public bool BloqueiaListaUf
        {
            set { this.ddlUf.Enabled = value; }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                if(value == false)
                    this.pnlEditar.CssClass = "esconderControle";
                else
                    this.pnlEditar.CssClass = "mostrarControle";
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
        
        public SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                paramList.Add("CodigoUA", ddlUA.SelectedValue.ToString());
                paramList.Add("DescricaoUA", this.ddlUA.SelectedItem.Text);
                
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
        
        #region Controles ASPX

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {

            PopularUo();

            //DivisaoPresenter divisao = new DivisaoPresenter(this);
            //divisao.Cancelar();
            //this.PopularListaUA((Int32)TratamentoDados.TryParseInt32(this.OrgaoId));
        }
        
        protected void ddlUA_SelectedIndexChanged(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Cancelar();
            PopularListaResponsavel();
            PopularListaAlmoxarifado();
            PopularGrid();
            if (ddlUA.Items.Count > 0)
            {
                btnNovo.Enabled = true;
                btnImprimir.Enabled = true;
            }
            else
            {
                btnNovo.Enabled = false;
                btnImprimir.Enabled = false;
            }
        }

        protected void ddlUA_DataBound(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Cancelar();
            PopularListaResponsavel();
            PopularListaAlmoxarifado();
            PopularGrid();
            if (ddlUA.Items.Count > 0)
            {
                btnNovo.Enabled = true;
                btnImprimir.Enabled = true;
            }
            else
            {
                btnNovo.Enabled = false;
                btnImprimir.Enabled = false;
            }
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            PopularListaResponsavel();
            PopularListaAlmoxarifado();
            divisao.Novo();
            txtCodigo.Focus();
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Gravar();
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Excluir();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Cancelar();
        }

        protected void gridDivisao_SelectedIndexChanged(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            this.Id = gridDivisao.DataKeys[gridDivisao.SelectedIndex].Value.ToString();
            divisao.divisaoId = int.Parse(this.Id);
            divisao.RegistroSelecionado();
            txtCodigo.Focus();
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.Imprimir();
        }


        public void PopularListaIndicadorAtividade()
        {
        }

        protected void inicializarCombos(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("- Selecione -");
            ddl.Items[0].Value = "0";
            ddl.AppendDataBoundItems = true;
        }

        public void PopularOrgao()
        {
            OrgaoPresenter org = new OrgaoPresenter();
            inicializarCombos(ddlOrgao);
            ddlOrgao.DataSource = org.PopularDadosTodosCod();
            ddlOrgao.DataBind();
            // limpar UO, UGE, UA, DIVISÃƒO
            inicializarCombos(ddlUo);
            inicializarCombos(ddlUge);
            inicializarCombos(ddlUA);            
        }

        public void PopularUo()
        {
            UOPresenter uo = new UOPresenter();
            inicializarCombos(ddlUo);
            ddlUo.DataSource = uo.PopularDadosUO(Convert.ToInt32(ddlOrgao.SelectedValue));
            ddlUo.DataBind();
            // limpar UGE, UA, DIVISÃƒO
            inicializarCombos(ddlUge);
            inicializarCombos(ddlUA);
            inicializarCombos(ddlAlmoxarifado);
        }

        public void PopularUge()
        {
            UGEPresenter uge = new UGEPresenter();
            inicializarCombos(ddlUge);
            ddlUge.DataSource = uge.PopularDadosTodosCodPorUo(Convert.ToInt32(ddlUo.SelectedValue));
            ddlUge.DataBind();
            // limpar UA, DIVISÃƒO
            inicializarCombos(ddlUA);
        }

        public void PopularUa()
        {
            UAPresenter ua = new UAPresenter();
            inicializarCombos(ddlUA);

            foreach (var _item in ua.PopularDadosTodosCodPorUge((Convert.ToInt32(ddlUge.SelectedValue))))
            {
                ListItem _novoItem = new ListItem(_item.CodigoDescricao, _item.Id.GetValueOrDefault().ToString());

                if (!_item.IndicadorAtividade)
                    _novoItem.Text = _novoItem.Text + " (Inativa)";

                ddlUA.Items.Add(_novoItem);
            }

            ddlUA.DataBind();
        }


        protected void ddlUo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUge();
        }

        protected void ddlUge_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopularUa();
        }
        
        public string UOId
        {
            get;
            set;
        }

        public string UGEId
        {
            get;
            set;
        }
    }      
}
