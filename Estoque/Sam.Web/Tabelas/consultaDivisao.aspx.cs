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
    public partial class consultaDivisao : PageBase,  IDivisaoView
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DivisaoPresenter divisao = new DivisaoPresenter(this);
                //divisao.LoadConsulta();
                //PopularGrid();
                PopularUo();
            }
        }
                
        #region IEntidadesAuxiliaresView Members

        public void PopularListaOrgao()
        {
            //ddlOrgao.DataSourceID = "sourceListaOrgao";
        }

        public string OrgaoId
        {
            get;
            set;
        }

        public void PopularListaUA(int _orgaoId)
        {
            //ddlUA.DataSourceID = "sourceListaUA";
        }

        public string UAId
        {
            set;
            get;
        }

        public string UGEId
        {
            set
            {
                ListItem item = ddlUge.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUge.ClearSelection();
                    item.Selected = true;

                }
            }
            get
            {
                if (ddlUge.Items.Count > 0)
                    if (ddlUge.SelectedValue != "s")
                    {
                        return ddlUge.SelectedValue;
                    }
                    else
                    {
                        return "0";
                    }
                else
                {
                    return "0";
                }
            }
        }

        public string UOId
        {
            set
            {
                ListItem item = ddlUo.Items.FindByValue(value);
                if (item != null)
                {
                    ddlUo.ClearSelection();
                    item.Selected = true;

                }
            }
            get
            {
                if (ddlUo.Items.Count > 0)
                    if (ddlUo.SelectedValue != "s")
                    {
                        return ddlUo.SelectedValue;
                    }
                    else
                    {
                        return "0";
                    }
                else
                {
                    return "0";
                }
            }
        }

        public string EnderecoLogradouro
        {
            get;
            set;
        }

        public string Codigo
        {
            get;
            set;
        }

        public string Descricao
        {
            get;
            set;
        }

        public string EnderecoNumero
        {
            get;
            set;
        }

        public string EnderecoComplemento
        {
            get;
            set;
        }

        public string EnderecoBairro
        {
            get;
            set;
        }

        public string EnderecoMunicipio
        {
            get;
            set;
        }
        
        public string UfId
        {
            get;
            set;
        }

        public string EnderecoCep
        {
            get;
            set;
        }

        public string EnderecoTelefone
        {
            get;
            set;
        }

        public string EnderecoFax
        {
            get;
            set;
        }

        public string NumeroFuncionarios
        {
            get;
            set;
        }

        public string Area
        {
            get;
            set;
        }

        public string AlmoxarifadoId
        {
            get;
            set;
        }

        public string ResponsavelId
        {
            get;
            set;
        }
               
        public string IndicadorAtividadeId
        {
            get;
            set;
        }

        public void PopularGrid()
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            var result = divisao.PopularDadosDivisao(gridDivisao.PageIndex, 10, int.Parse(UOId), Int64.Parse(UGEId));
            //var result = divisao.ListarDivisaoByGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value);

            //if (result.Count() > 0)
            //    gridDivisao.PageSize = result.Count();

            gridDivisao.DataSource = result;
            gridDivisao.DataBind();
        }

        public void PopularListaUF()
        {
        }

        public void PopularListaUA()
        {
        }

        public void PopularListaResponsavel()
        {
        }

        public void PopularListaAlmoxarifado()
        {
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
        
        public bool BloqueiaNovo
        {
            set
            {
                this.btnNovo.Enabled = value;
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
                paramList.Add("UOId", this.UOId);
                paramList.Add("UGEId", this.UGEId);
                paramList.Add("NomeSort", ((this.UGEId != null && this.UGEId != "0") && (this.UOId != null && this.UOId != "0")) ? "Ordenado por UO e UGE" : (this.UOId != null && this.UOId != "0") ? "Ordenado por UO" : "");
                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion
        
        #region Controles ASPX


        protected void btnNovo_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            PopularListaResponsavel();
            PopularListaAlmoxarifado();
            divisao.Novo();
        }


        protected void gridDivisao_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #endregion

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            DivisaoPresenter divisao = new DivisaoPresenter(this);
            divisao.ImprimirConsulta();
        }

        public void PopularListaIndicadorAtividade()
        {
        }

        protected void inicializarCombos(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add("- Selecione -");
            ddl.Items[0].Value = "s";

            ddl.Items.Add("- Todas -");
            ddl.Items[1].Value = "0";


            ddl.AppendDataBoundItems = true;
        }

        public void PopularOrgao()
        {
        }

        public void PopularUo()
        {
            UOPresenter uo = new UOPresenter();
            inicializarCombos(ddlUo);
            ddlUo.DataSource = uo.PopularDadosTodosCod(new PageBase().GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Id.Value);
            ddlUo.DataBind();
            // limpar UGE, UA, DIVISÃƒO
            inicializarCombos(ddlUge);
        }

        public void PopularUge()
        {
            UGEPresenter uge = new UGEPresenter();
            inicializarCombos(ddlUge);
            ddlUge.DataSource = uge.PopularDadosTodosCodPorUo(Convert.ToInt32(UOId));
            ddlUge.DataBind();

            // limpar UA, DIVISÃƒO
        }

        public IList ListaErros
        {
            set;
            get;
        }


        public bool BloqueiaEnderecoLogradouro
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoNumero
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoComplemento
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoBairro
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoMunicipio
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoCep
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoTelefone
        {
            set;
            get;
        }

        public bool BloqueiaEnderecoFax
        {
            set;
            get;
        }

        public bool BloqueiaListaAlmoxarifado
        {
            set;
            get;
        }

        public bool BloqueiaListaResponsavel
        {
            set;
            get;
        }

        public bool BloqueiaListaUA
        {
            set;
            get;
        }

        public bool BloqueiaListaUF
        {
            set;
            get;
        }

        public bool BloqueiaNumeroFuncionarios
        {
            set;
            get;
        }

        public bool BloqueiaArea
        {
            set;
            get;
        }

        public bool BloqueiaListaIndicadorAtividade
        {
            set;
            get;
        }

        public bool BloqueiaGravar
        {
            set;
            get;
        }

        public bool BloqueiaExcluir
        {
            set;
            get;
        }

        public bool BloqueiaCancelar
        {
            set;
            get;
        }

        public bool BloqueiaCodigo
        {
            set;
            get;
        }

        public bool BloqueiaDescricao
        {
            set;
            get;
        }

        public bool MostrarPainelEdicao
        {
            set;
            get;
        }

        protected void ddlUo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridDivisao.PageIndex = 0;
            PopularUge();
        }

        protected void ddlUge_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridDivisao.PageIndex = 0;
            PopularGrid();
        }

        protected void ddlUge_DataBound(object sender, EventArgs e)
        {
            PopularGrid();
        }

        protected void gridDivisao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridDivisao.PageIndex = e.NewPageIndex;
            PopularGrid();
        }
    }      
}
