using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Sam.View;
using Sam.Common;
using Sam.Common.Util;
using Sam.Presenter;
using Sam.Domain.Entity;

namespace Sam.Web.Almoxarifado
{
    public partial class NovaEntradaMaterial : PageBase, INovaEntradaMaterialView, IPostBackEventHandler
    {
        private MovimentoEntity movimento = new MovimentoEntity();
        private string sessaoMov = "movimento";
        private string idDocumentoEdit = "idDocumentoEdit";
        private int? idDocumento = null;
        private bool perfilEditar;
        private int novoDocumento = 0;

        #region Novo Código

        //Sessão do Movimento e MovimentoItens inseridos do grid
        public void GetSessao()
        {
            if (movimento.MovimentoItem == null)
            {
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    movimento = GetSession<MovimentoEntity>(sessaoMov);
                }
            }
        }

        public void RemoveSessao()
        {
            RemoveSession(sessaoMov);
        }

        //Sessão do Movimento que foi selecionado na tela de consulta para edição
        public void GetSessaoEdicao()
        {
            if (GetSession<int>(idDocumentoEdit) != null)
            {
                idDocumento = GetSession<int>(idDocumentoEdit);
            }
            else
            {
                idDocumento = 0;//zero é nova entrada
            }
        }        

        /// <summary>
        /// Verifica se a tela carregará como Nova entrada ou Edição
        /// </summary>
        public void TratarEntrada()
        {
            if (idDocumento == novoDocumento)
            {
                //Novo Documento
                PrepararNovaEntrada();
            }
            else if (String.IsNullOrEmpty(idDocumentoEdit))
            {
                //Novo Documento   
                PrepararNovaEntrada();
            }
            else
            {
                //Carregar dados da Edição
                PreprarEdicaoEntrada();
            }
        }

        public void PreprarEdicaoEntrada()
        {
            NovaEntradaMaterialPresenter presenter = new NovaEntradaMaterialPresenter(this);
            presenter.PreprarEdicaoEntrada();
            HabilitarAquisicaoAvulsaNovo();
            HabilitarAquisicaoAvulsa(true);            
        }

        public void NotPostBackInicialize()
        {
            lblFornecedor.Visible = false;
            imgLupaFornecedor.Visible = false;
            ExibirGeradorDescricao = false;
            //PerfilConsultar = true;
            hidAlmoxId.Value = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value.ToString();
            lblDocumentoAvulsoAnoMov.Text = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + "/";
            ExibirImprimir = false;
        }

        public void PrepararNovaEntrada()
        {
            NovaEntradaMaterialPresenter presenter = new NovaEntradaMaterialPresenter(this);
            presenter.PrepararNovaEntrada();
        }

        public void PreencherDadosEdicao()
        {
            //Busca o movimento completo atualizado
            var presenter = new NovaEntradaMaterialPresenter(this);

            movimento = presenter.GetMovimento((int)idDocumento);

            if (movimento != null)
            {
                presenter.CarregarMovimentoTela(movimento);

                SelecionaUGELogada();
                BloqueiaNumeroDocumento = true;
                CarregaEmpenhosALiquidar();
                TipoMovimentoEdicao();                
                CarregarGridSubitensSessao();
                SetSession<MovimentoEntity>(movimento, sessaoMov);
            }
        }

        public bool BloqueiaDataRecebimento
        {
            set { txtDataReceb.Enabled = !value; }
        }

        #endregion

        private void RegistraJavaScript()
        {
            btnGravarItem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluirItem.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para concluir a entrada de materiais.');");
            btnEstornar.Attributes.Add("OnClick", "return confirm('Pressione OK para estornar a entrada de materiais.');");
            txtQtdeMov.Attributes.Add("onblur", "return calcularPrecoUnitario();");
            txtValorMovItem.Attributes.Add("onblur", "return calcularPrecoUnitario();");

            ScriptManager.RegisterStartupScript(this.txtDataEmissao, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtDataReceb, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtVencimentoLote, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            ScriptManager.RegisterStartupScript(this.txtValorTotal, GetType(), "numerico", "$('.numerico').floatnumber(',',2);", true);
            ScriptManager.RegisterStartupScript(this.txtPrecoUnit, GetType(), "numerico", "$('.numerico').floatnumber(',',2);", true);
            ScriptManager.RegisterStartupScript(this.txtDocumentoAvulso, GetType(), "inputFromNumero", "$('.inputFromNumero').numeric();", true);
            txtDocumentoAvulso.Attributes.Add("onblur", "preencheZeros(this,'8') ");
        }

        public void CarregarPermissaoAcesso()
        {
            // carregar permissão
            switch (AutorizaTransacao())
            {
                case Enuns.AcessoTransacao.Edita:
                    perfilEditar = true;
                    break;
                default:
                    perfilEditar = false;
                    break;
            }
        }

        private void CarregarGridSubitensSessao()
        {
            //Preenche o campo posição do Grid pela sequencia de registros
            int contador = 1;
            foreach (var movItem in movimento.MovimentoItem)
            {
                movItem.Posicao = contador;
                contador = contador + 1;
            }

            gridSubItemMaterial.DataSource = movimento.MovimentoItem;
            gridSubItemMaterial.DataBind();
        }

        public void ConfiguracaoPagina()
        {
            RegistraJavaScript();

            //Propriedades para o filtro da busca de subitens
            uc3SubItem.FiltrarAlmox = true;
            uc3SubItem.UsaSaldo = false;
            OcultaDescricaoItem();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter presenter = new NovaEntradaMaterialPresenter(this);
            presenter.Load();

            if (!IsPostBack)
            {
                presenter.NotPostBackInicialize();
            }
        }

        #region Popular Listas

        public void PopularListaDivisao()
        {
            int _almoxId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            ddlDivisao.Items.Clear();
            ddlDivisao.Items.Add("-Selecione-");
            ddlDivisao.AppendDataBoundItems = true;
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlDivisao.DataSource = mat.PopularListaDivisao(_almoxId);
            ddlDivisao.DataBind();
        }

        public void PopularListaAlmoxarifado()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlAlmoxarifadoTransfer.Items.Clear();
            ddlAlmoxarifadoTransfer.Items.Add("- Selecione -");
            ddlAlmoxarifadoTransfer.Items[0].Value = "0";
            ddlAlmoxarifadoTransfer.AppendDataBoundItems = true;
            ddlAlmoxarifadoTransfer.DataSource = mat.PopularListaAlmoxarifado(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value).Where(a => a.Id != new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            ddlAlmoxarifadoTransfer.DataBind();
        }

        public void PopularDadosSubItemClassif()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlSubItemClassif.Items.Clear();
            ddlSubItemClassif.Items.Add("- Selecione -");
            ddlSubItemClassif.AppendDataBoundItems = true;
            ddlSubItemClassif.DataSource = mat.PopularDadosSubItemClassif();
            ddlSubItemClassif.DataBind();
        }

        #region Popular Combos

        public IList<MovimentoItemEntity> ListarMovimentoItens(int startRowIndexParameterName,
                int maximumRowsParameterName, string _documento)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            IList<MovimentoItemEntity> itens = mat.ListarMovimentoItens(startRowIndexParameterName, maximumRowsParameterName, _documento);
            gridSubItemMaterial.DataSource = itens;
            gridSubItemMaterial.DataBind();
            return itens;
        }

        public void PopularDadosUGETodosCod()
        {
            // carregar apenas as uges do perfil.
            ddlUGE.Items.Clear();
            ddlUGE.Items.Add("- Selecione -");
            ddlUGE.Items[0].Value = "0";
            ddlUGE.AppendDataBoundItems = true;
            UGEPresenter uge = new UGEPresenter();
            ddlUGE.DataSource = uge.PopularDadosTodosCodPorGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            ddlUGE.DataBind();
            int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            if (ddlUGE.Items.Count > 0)
            {
                ddlUGE.SelectedValue = ugeLogado.ToString();
            }
        }

        protected void carregarListaSubItens(DropDownList drp, string naturezaDespesa, int itemMaterialId)
        {
            drp.DataSource = null;
            drp.Items.Insert(0, "- Selecione -");
            drp.AppendDataBoundItems = true;
            drp.DataSource = new SubItemMaterialPresenter().ListarSubItemByAlmoxItemMaterial(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value, itemMaterialId, naturezaDespesa);
            drp.DataBind();
        }

        public void PopularUnidFornecimentoTodosPorUge(int _ugeId)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlUnidade.DataSource = mat.PopularUnidFornecimentoTodosPorUge(_ugeId);
            ddlUnidade.DataBind();
        }

        public void PopularUnidFornecimentoTodosPorGestor()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlUnidade.Items.Clear();
            ddlUnidade.Items.Add("- Selecione -");
            ddlUnidade.Items[0].Value = "0";
            ddlUnidade.AppendDataBoundItems = true;
            ddlUnidade.DataSource = mat.PopularUnidFornecimentoTodosPorGestor(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            ddlUnidade.DataBind();
        }

        public void PopularGrid() { throw new NotImplementedException(); }
        public void PopularListaFornecedor() { throw new NotImplementedException(); }
        public void PopularListaUGE() { throw new NotImplementedException(); }

        public void PopularListaUnidade()
        {
            ddlUnidade.Items.Clear();
            ddlUnidade.Items.Add("- Selecione -");
            ddlUnidade.AppendDataBoundItems = true;
            ddlUnidade.DataSourceID = "listaUnidadeFornecimento";
        }

        public void PopularDadosSubItemMaterial(string txtSubItemCodigo)
        {
            List<string> msgs = new List<string>();
            BloqueiaGravarItem = false;
            HabilitarLote = false;
            SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
            //SubItemMaterialEntity subItem = presenter.ListarSubItemAlmoxPorPalavraChave(0, 0, "2", txtSubItemCodigo, true, null, false).FirstOrDefault();
            //if (subItem != null)
            //{
            //    if (subItem.IndicadorAtividadeAlmox == false)
            //    {
            //        msgs.Add("Subitem inativo!");
            //        txtSubItem.Text = "";
            //        BloqueiaGravarItem = true;
            //    }

            //    if (msgs.Count > 0)
            //    {
            //        ListaErros = msgs;
            //        return;
            //    }

            //    NaturezaDespesaIdItem = subItem.NaturezaDespesa.Id;
            //    UnidadeId = subItem.UnidadeFornecimento.Id.Value;
            //    ItemMaterialCodigo = subItem.ItemMaterial.Codigo;
            //    lblItemMaterialDescricao.Text = subItem.ItemMaterial.Descricao.ToLower();

            //    OcultaDescricaoItem();

            //    if (subItem.IsLote == true)
            //    {
            //        HabilitarLote = true;
            //    }
            //}
        }

        public void PopularListaTipoMovimentoEntrada() 
        {
            rdoTipoMovimento.DataSource = new TipoMovimentoPresenter().PopularListaTipoMovimentoEntrada();
            rdoTipoMovimento.DataBind();
        }

        public bool PopularEmpenhosALiquidar(int pIntUgeID, int pIntTipoMovimento)
        {
            bool lBlnRetorno = false;

            var ugeLogada = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge;
            var almoxLogado = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

            var ugeAlmoxLogado = almoxLogado.Uge.Codigo.Value;
            var _codigoUGE = ugeLogada.Codigo.Value;

            int almoxarifadoId = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            int gestorId = this.GetAcesso.Transacoes.Perfis[0].GestorPadrao.Id.Value;

            NovaEntradaMaterialPresenter lObjPresenter = new NovaEntradaMaterialPresenter(this);
            WebUtil lObjWebUtil = new WebUtil();
            IList<string> lILstEmpenhos = null;

            string lStrLoginUsuario = string.Empty;
            string lStrSenhaUsuario = string.Empty;
            int lIntContador = 0;

            // carregar a lista de empenhos do mês corrente
            //if (pIntTipoMovimento == ((int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho))
            if (pIntTipoMovimento == ((int)GeralEnum.TipoMovimento.EntradaPorEmpenho))
            {
                lStrLoginUsuario = new PageBase().GetAcesso.Cpf;
                lStrSenhaUsuario = GetSession<string>("senhaWsSiafem");

                lILstEmpenhos = lObjPresenter.ObterListaEmpenhos(almoxarifadoId, gestorId, ugeAlmoxLogado, lStrLoginUsuario, lStrSenhaUsuario);

                // remove a sessão caso haja irregularidade com o usuário.
                if (lILstEmpenhos != null && lILstEmpenhos.Count > 0)
                {
                    ddlEmpenho.Items.Clear();
                    ddlEmpenho.Items.Add("- Selecione -");
                    ddlEmpenho.Items[0].Value = lIntContador.ToString();

                    foreach (string CodigoEmpenho in lILstEmpenhos)
                    {
                        lIntContador++;
                        ddlEmpenho.Items.Add(CodigoEmpenho);
                        ddlEmpenho.Items[lIntContador].Value = lIntContador.ToString();
                    }

                    lBlnRetorno = true;
                }
                else
                {
                    return lBlnRetorno;
                }
            }

            return lBlnRetorno;
        }

        public void PopularListaEmpenhoEvento()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            ddlEmpenhoEvento.Items.Clear();
            ddlEmpenhoEvento.Items.Add("- Selecione -");
            ddlEmpenhoEvento.Items[0].Value = "0";
            ddlEmpenhoEvento.AppendDataBoundItems = true;
            ddlEmpenhoEvento.DataSource = mat.PopularEmpenhoLicitacao();
            ddlEmpenhoEvento.DataBind();
        }

        #endregion

        #region Bloquear Controles

        public bool BloqueiaItemEfetivado
        {
            set
            {
                this.BloqueiaGravarItem = value;
                this.BloqueiaExcluirItem = !value;
                this.BloqueiaItemQtdeMov = !value;
                this.BloqueiaItemPrecoUnit = true;
                this.BloqueiaItemLoteFabric = !value;
                this.BloqueiaItemLoteIdent = !value;
                this.BloqueiaItemLoteDataVenc = !value;
            }
        }

        public bool BloqueiaValorTotal { set { txtValorTotal.Enabled = !value; } }
        public bool BloqueiaTipoMovimento { set { rdoTipoMovimento.Enabled = !value; } }
        public bool BloqueiaGravarItem { set { btnGravarItem.Enabled = !value; } }
        public bool BloqueiaExcluirItem { set { btnExcluirItem.Enabled = !value; } }
        public bool BloqueiaItemQtdeMov { set { txtQtdeMov.Enabled = !value; } }
        public bool BloqueiaItemQtdeLiq { set { txtQtdeLiq.Enabled = !value; } }
        public bool BloqueiaItemPrecoUnit { set { txtPrecoUnit.Enabled = !value; } }
        public bool BloqueiaItemLoteFabric { set { txtFabricLoteItem.Enabled = !value; } }
        public bool BloqueiaItemLoteIdent { set { txtIdentLoteItem.Enabled = !value; } }
        public bool BloqueiaItemLoteDataVenc { set { txtVencimentoLote.Enabled = !value; } }
        public bool BloqueiaGeradorDescricao
        {
            set { txtGeradorDescricao.Enabled = !value; }
        }

        public bool ExibirGeradorDescricao
        {
            set
            {
                if (value == false)
                    txtGeradorDescricao.CssClass = "esconderControle";
                else
                    txtGeradorDescricao.CssClass = "mostrarControle";
            }
        }

        public bool ExibirListaDivisao { set { ddlDivisao.Visible = value; } }

        public bool ExibirListaEmpenho
        {
            set
            {
                if (value == true)
                    ddlEmpenho.CssClass = "mostrarControle";
                else
                    ddlEmpenho.CssClass = "esconderControle";
            }
            get
            {
                return (ddlEmpenho.CssClass == "mostrarControle");
            }
        }

        public bool ExibirNumeroEmpenho
        {
            set { txtEmpenho.Visible = value; }
            get { return txtEmpenho.Visible; }
        }

        public bool BloqueiaListaUGE { set { ddlUGE.Enabled = !value; } }
        public bool BloqueiaDocumento { set { txtDocumentoAvulso.Enabled = !value; } }
        public bool BloqueiaNumeroDocumento { set { txtDocumentoAvulso.Enabled = !value; } }
        public bool ExibirDocumentoAvulso { set { txtDocumentoAvulso.Visible = value; } }
        public bool BloqueiaListaIndicadorAtividade { set { throw new NotImplementedException(); } }

        public bool BloqueiaListaDivisao
        {
            set
            {
                ddlDivisao.Enabled = !value;
                if (value == false)
                    ddlDivisao.CssClass = "mostrarControle";
                else
                    ddlDivisao.CssClass = "esconderControle";
            }
        }

        public bool BloqueiaListaAlmoxarifado
        {
            set
            {
                ddlAlmoxarifadoTransfer.Enabled = !value;
                if (value == false)
                    ddlAlmoxarifadoTransfer.CssClass = "mostrarControle";
                else
                    ddlAlmoxarifadoTransfer.CssClass = "esconderControle";
            }
        }

        protected bool tipoMovimentoSelecionado()
        {
            for (int i = 0; i < rdoTipoMovimento.Items.Count; i++)
            {
                if (rdoTipoMovimento.Items[i].Selected == true)
                    return true;
            }
            return false;
        }

        #endregion

        #region Propriedades

        public string Id
        {
            get
            {
                if (hdfMovimentoId != null)
                    return hdfMovimentoId.Value.ToString();
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value != null)
                    hdfMovimentoId.Value = value.ToString();
                else
                    hdfMovimentoId.Value = string.Empty;
            }
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string NumeroDocumentoCombo { set; get; }
        public string NumeroEmpenhoCombo
        {
            set
            {
                ListItem item = ddlEmpenho.Items.FindByText(value.ToString());
                if (item != null)
                {
                    ddlEmpenho.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlEmpenho.ClearSelection();
            }
            get
            {
                return ddlEmpenho.SelectedItem.Text; ;
            }
        }

        public int? DivisaoId
        {
            set
            {
                ListItem item = ddlDivisao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlDivisao.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlDivisao.ClearSelection();
            }
            get
            {
                return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlDivisao.SelectedValue));
            }
        }

        public int TipoOperacao
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public int TipoMovimento
        {
            set
            {
                ListItem item = rdoTipoMovimento.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    rdoTipoMovimento.ClearSelection();
                    item.Selected = true;
                }
            }
            get
            {
                if(rdoTipoMovimento.SelectedIndex >= 0)
                {
                    return Convert.ToInt32(rdoTipoMovimento.SelectedValue);
                }
                else
                {
                    if (movimento != null)
                        return movimento.TipoMovimento.Id;
                    else
                        return 0;
                }                
            }
        }

        public int? AlmoxarifadoIdOrigem
        {
            set
            {
                ListItem item = ddlAlmoxarifadoTransfer.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlAlmoxarifadoTransfer.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlAlmoxarifadoTransfer.ClearSelection();
            }
            get
            {
                return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlAlmoxarifadoTransfer.SelectedValue));
            }
        }

        public string GeradorDescricao
        {
            get { return txtGeradorDescricao.Text; }
            set { txtGeradorDescricao.Text = value; }
        }

        public string SubItemMaterialDescricao
        {
            get { return txtDescricao.Text; }
            set { txtDescricao.Text = value; }
        }

        public string ItemMaterialDescricao
        {
            get { return lblItemMaterialDescricao.Text; }
            set { lblItemMaterialDescricao.Text = value; }
        }


        public int? EmpenhoEventoId
        {
            get;
            set;
        }

        public int? EmpenhoLicitacaoId
        {
            set
            {
                ListItem item = ddlEmpenhoEvento.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlEmpenhoEvento.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlEmpenhoEvento.ClearSelection();
            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlEmpenhoEvento.SelectedValue)); }
        }

        public int OrgaoId { get; set; }

        public int UgeId
        {
            set
            {
                ListItem item = ddlUGE.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUGE.ClearSelection();
                    item.Selected = true;
                }

            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlUGE.SelectedValue)); }
        }

        public int? MovimentoItemId
        {
            get { return hidtxtMovimentoItemId.Value.ToString() == "" ? 0 : Convert.ToInt32(hidtxtMovimentoItemId.Value.ToString()); }
            set { hidtxtMovimentoItemId.Value = value.ToString(); }
        }

        public int UnidadeId
        {
            set
            {
                ListItem item = ddlUnidade.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlUnidade.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlUnidade.ClearSelection();
            }
            get { return Convert.ToInt32(ddlUnidade.SelectedValue); }
        }

        public int? FornecedorId
        {
            get { return TratamentoDados.TryParseInt32(txtCodFornecedor.Text); }
            set { txtCodFornecedor.Text = value.ToString(); }
        }

        public int? ItemMaterialId
        {
            get { return hidtxtItemMaterialId.Value.ToString() == "" ? 0 : Convert.ToInt32(hidtxtItemMaterialId.Value.ToString()); }
            set { hidtxtItemMaterialId.Value = value.ToString(); }
        }

        public int? SubItemMaterialId
        {
            set { idSubItem.Value = value.ToString(); }
            get { return TratamentoDados.TryParseInt32(idSubItem.Value); }
        }


        public string SubItemMaterialTxt
        {
            set
            {
                hidtxtSubItemMaterialId.Value = value;
            }
            get { return hidtxtSubItemMaterialId.Value.ToString(); }
        }


        public long? SubItemMaterialCodigo
        {
            set { txtSubItem.Text = value.ToString(); }
            get { return TratamentoDados.TryParseLong(txtSubItem.Text); }
        }


        public int? ItemMaterialCodigo
        {
            get { return Convert.ToInt32(txtItemMaterial.Text); }
            set { txtItemMaterial.Text = value != null ? value.ToString() : null; }
        }

        public string NumeroDocumento
        {
            get { return txtDocumentoAvulso.Text; }
            set { txtDocumentoAvulso.Text = value; }
        }

        public string AnoMesReferencia { get; set; }

        public DateTime? DataDocumento
        {
            get
            {
                return TratamentoDados.TryParseDateTime(txtDataEmissao.Text);
            }
            set
            {
                if (value.HasValue)
                {
                    txtDataEmissao.Text = Convert.ToDateTime(value.ToString()).ToString("dd/MM/yyyy");
                }
                else
                {
                    txtDataEmissao.Text = "";
                }
            }
        }

        public DateTime? DataMovimento
        {
            get
            {
                return TratamentoDados.TryParseDateTime(txtDataReceb.Text);
            }
            set
            {
                if (value.HasValue)
                    txtDataReceb.Text = Convert.ToDateTime(value.ToString()).ToString("dd/MM/yyyy");
                else
                    txtDataReceb.Text = "";
            }
        }

        public void LimparGridSubItemMaterial()
        {
            gridSubItemMaterial.DataSourceID = null;
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
        }

        public string FonteRecurso { get; set; }

        public decimal? ValorDocumento
        {
            get 
            {
                string valorDocumento = lblValor.Text.Replace("R$", "");
                return Convert.ToDecimal(TratamentoDados.TryParseDecimal(valorDocumento));
            }
            set
            {
                if (value.HasValue)
                {
                    lblValor.Text = string.Format("{1} {0:#,##0.00}", value, "R$");
                }
                else
                {
                    lblValor.Text = string.Empty;
                }
            }
        }

        public decimal? ValorTotalMovimento
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtValorTotalMovimento.Text)); }
            set { txtValorTotalMovimento.Text = string.Format("{0:#,##0.00}", value); }
        }

        public string Observacoes
        {
            get { return txtObservacoes.Text; }
            set { txtObservacoes.Text = value; }
        }

        public string Instrucoes { get; set; }

        public string Empenho
        {
            get { return txtEmpenho.Text; }
            set { txtEmpenho.Text = value; }
        }

        public string hiddenMovimentoId
        {
            get { return hdfMovimentoId.Value; }
            set { hdfMovimentoId.Value = value; }
        }

        public string NlLiquidacao { get; set; }

        public string IdItem { get; set; }

        public int? SubItemMatItem { get; set; }

        public DateTime? DataVctoLoteItem
        {
            get { return TratamentoDados.TryParseDateTime(txtVencimentoLote.Text); }
            set
            {
                if (value.HasValue)
                    txtVencimentoLote.Text = Convert.ToDateTime(value.ToString()).ToString("dd/MM/yyyy");
                else
                    txtVencimentoLote.Text = "";
            }
        }

        public string DataVctoLoteItemTexto
        {
            get { return txtVencimentoLote.Text; }
            set { txtVencimentoLote.Text = value; }
        }

        public string IdentificacaoLoteItem
        {
            get { return txtIdentLoteItem.Text; }
            set { txtIdentLoteItem.Text = value; }

        }
        public string FabricLoteItem
        {
            get { return txtFabricLoteItem.Text; }
            set { txtFabricLoteItem.Text = value; }
        }

        public decimal QtdeItem
        {
            get { return Convert.ToInt32(TratamentoDados.TryParseDecimal(txtQtdeMov.Text)); }
            set { txtQtdeMov.Text = value.ToString(); }
        }

        public decimal QtdeLiqItem
        {
            get { return Convert.ToInt32(TratamentoDados.TryParseDecimal(txtQtdeLiq.Text)); }
            set { txtQtdeLiq.Text = value.ToString(); }
        }

        public decimal SaldoQtdeItem { get; set; }
        public decimal? SaldoQtdeLoteItem { get; set; }

        public decimal PrecoUnitItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtPrecoUnit.Text)); }
            set
            {
                txtPrecoUnit.Text = string.Format("{0:#,##0.00}", value);
            }
        }

        public decimal? SaldoValorItem { get; set; }

        public decimal ValorMovItem
        {
            get { return Convert.ToDecimal(TratamentoDados.TryParseDecimal(txtValorMovItem.Text)); }
            set { txtValorMovItem.Text = string.Format("{0:#,##0.00}", value); }
        }

        public int? NaturezaDespesaIdItem
        {
            set
            {
                ListItem item = ddlSubItemClassif.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    ddlSubItemClassif.ClearSelection();
                    item.Selected = true;
                }
                else
                    ddlSubItemClassif.ClearSelection();
            }
            get { return Convert.ToInt32(TratamentoDados.TryParseInt32(ddlSubItemClassif.SelectedValue)); }

        }

        public decimal DesdItem { get; set; }
        public bool? AtivoItem { get; set; }

        #endregion

        #endregion

        public void RaisePostBackEvent(string eventArgument) { }

        private void OcultaDescricaoItem()
        {
            if (String.IsNullOrEmpty(lblItemMaterialDescricao.Text))
                ItemDescricao.Visible = false;
            else
                ItemDescricao.Visible = true;
        } 

        #region Mensagens de Erro

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
                this.ListInconsistencias.DataBind();
            }
        }

        #endregion

        #region Bloquear Controles Movimento

        public bool BloqueiaNovo { set { btnNovo.Enabled = !value; } }
        public bool ExibirDocumentoAvulsoAnoMov { set { lblDocumentoAvulsoAnoMov.Visible = value; } }
        public bool BloqueiaGravar { set { btnGravar.Enabled = !value; } }
        public bool BloqueiaExcluir { set { btnExcluirItem.Enabled = !value; } }
        public bool BloqueiaBotaoCarregarEmpenho
        {
            set
            {
                btnListarEmpenhos.Enabled = !value;
                btnListarEmpenhos.Visible = !value;

                idEmpenho.Visible = !value; //Oculpa todo o paragrafo do Empenho
            }
        }
        public bool VisibleNovo
        {
            set
            {
                btnNovo.Visible = value;
                divBotaoNovo.Visible = value;
            }
        }
        public bool BloqueiaCancelar { set { btnCancelar.Enabled = !value; } }
        public bool BloqueiaImprimir { set { btnImprimir.Enabled = !value; } }
        public bool ExibirImprimir { set { btnImprimir.Visible = value; } }
        public bool BloqueiaAjuda { set { btnAjuda.Enabled = !value; } }
        public bool BloqueiaCodigo { get; set; }
        public bool BloqueiaDescricao { set { txtDescricao.Enabled = value; } }
        public bool BloqueiaEmpenho 
        { 
            set 
            {
                lblEmpenho.Visible = !value;
                txtEmpenho.Enabled = !value;
                txtEmpenho.Visible = !value;
            } 
        }
        public bool BloqueiaEmpenhoEvento
        {
            set
            {
                if (value == true)
                {
                    idCodigoEventoEmpenho.Style.Value = "display: none;";

                    if (ddlEmpenhoEvento.SelectedIndex != -1)
                        ddlEmpenhoEvento.SelectedIndex = 0;
                }
                else
                {
                    idCodigoEventoEmpenho.Style.Value = "display: block;";
                }
            }

        }
        public bool BloqueiaNovoItem { set { throw new NotImplementedException(); } }
        public bool BloqueiaEstornar { set { btnEstornar.Enabled = !value; } }
        public bool HabilitaPesquisaFornecedor { set { imgLupaFornecedor.Visible = value; } }
        public bool HabilitarLote
        {
            set
            {
                if (value == true)
                    pnlLote.CssClass = "mostrarControle";
                else
                    pnlLote.CssClass = "esconderControle";
            }
        }
        public bool HabilitaPesquisaItemMaterial { set { imgSubItemMaterial.Visible = value; } }
        public bool HabilitarBotoes
        {
            set
            {
                BloqueiaNovo = !perfilEditar;
                BloqueiaCancelar = !value;
                BloqueiaAjuda = !value;
            }
        }

        // usar controles baseado no perfil de consulta
        public bool HabilitarControles
        {
            set
            {
                txtDataEmissao.Enabled = true;
                txtEmpenho.Enabled = value;
                txtObservacoes.Enabled = value;
                BloqueiaListaUGE = !value;
                ExibirGeradorDescricao = value;
                BloqueiaEmpenho = !value;
                BloqueiaListaDivisao = true;
                BloqueiaListaAlmoxarifado = true;
                // deixar sempre false
                //MostrarPainelEdicao = false;
                HabilitaPesquisaFornecedor = false;
                ExibirGeradorDescricao = value;
                BloqueiaNovo = !perfilEditar;
                BloqueiaDataRecebimento = false;
                ExibirListaEmpenho = false;
            }
        }

        public void HabilitarControlesEdicao(bool Editar)
        {
            // conceder permissões
            //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                BloqueiaNovo = true;
            else
                BloqueiaNovo = !perfilEditar;

            txtValorTotal.Enabled = false;
            txtDataEmissao.Enabled = Editar;
            txtDataReceb.Enabled = Editar;
            txtObservacoes.Enabled = Editar;
            DataMovimento = DateTime.Now;
            BloqueiaCancelar = !Editar;
            BloqueiaItemQtdeLiq = true;
        }
        public bool MostrarPainelEdicao { set { pnlEditar.Visible = value; } }

        #endregion

        #region Compra Direta / BEC / Pregao
        public void HabilitarCompraDireta(bool Editar)
        {
            lblFornecedor.Text = "Fornecedor:";
            HabilitarControles = true;
            HabilitarBotoes = false;
            ExibirGeradorDescricao = true;
            BloqueiaGeradorDescricao = true;
            // conceder permissões (de acordo com o perfil)
            BloqueiaNovo = !perfilEditar;
            txtEmpenho.Visible = false;
            ExibirListaEmpenho = true;
            BloqueiaEmpenhoEvento = false;
            lblValorTotal.Text = "Valor Doc.:";
            btnExcluirItem.Visible = false;
            VisibleNovo = false;
            idCodigoEventoEmpenho.Attributes["class"] = "mostrarControle";
            idDevolucao.Attributes["class"] = "esconderControle";
            idFornecedor.Attributes["class"] = "mostrarControle";
            idTransfer.Attributes["class"] = "esconderControle";
            BloqueiaBotaoCarregarEmpenho = false;
            divQtdLiq.Visible = true;
        }

        public void HabilitarCompraDiretaNovo()
        {
            HabilitarControlesEdicao(true);
            BloqueiaEmpenhoEvento = false;
            BloqueiaItemQtdeLiq = true;
            HabilitaPesquisaItemMaterial = false;
            BloqueiaImprimir = true;
            BloqueiaGravarItem = true; // bloqueado por padrão para não permitir a inclusão de novo material no empenho.
            VisibleNovo = false;
        }

        public void HabilitarCompraDiretaEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
            BloqueiaNovo = !perfilEditar;
            VisibleNovo = false;
        }

        #endregion

        #region Aquisicao Avulsa

        public void HabilitarAquisicaoAvulsa(bool Editar)
        {
            lblFornecedor.Text = "Fornecedor:";
            HabilitarControles = true;
            HabilitaPesquisaFornecedor = true;
            VisibleNovo = true;
            imgLupaRequisicao.Visible = false;
            
            // perfil
            BloqueiaNovo = !perfilEditar;
            idCodigoEventoEmpenho.Attributes["class"] = "mostrarControle";
            idDevolucao.Attributes["class"] = "esconderControle";
            idFornecedor.Attributes["class"] = "mostrarControle";
            idTransfer.Attributes["class"] = "esconderControle";
            BloqueiaBotaoCarregarEmpenho = true;
            divQtdLiq.Visible = false;
        }

        public void HabilitarAquisicaoAvulsaNovo()
        {
            HabilitarControlesEdicao(true);
            BloqueiaEmpenhoEvento = true;
            HabilitaPesquisaFornecedor = true;
        }

        public void criarNovoDocumento()
        {
            MovimentoPresenter mov = new MovimentoPresenter();

            //if (GetSession<MovimentoEntity>("movimento") != null)
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                //movimento = GetSession<MovimentoEntity>("movimento");
                movimento = GetSession<MovimentoEntity>(sessaoMov);
                movimento.Id = null;
            }

            movimento.AnoMesReferencia = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            this.AnoMesReferencia = movimento.AnoMesReferencia;

            movimento = mov.gerarNovoDocumento(movimento);
            txtDocumentoAvulso.Text = movimento.NumeroDocumento;
        }


        public void HabilitarAquisicaoAvulsaEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
        }

        #endregion

        #region Doacao

        public void HabilitarDoacao(bool Editar)
        {
            //lblFornecedor.Text = "Doador:";
            //HabilitarControles = true;
            //BloqueiaEmpenho = true;
            //BloqueiaGeradorDescricao = false;
            //BloqueiaBotaoCarregarEmpenho = true;
            //// conceder permissões (de acordo com o perfil)
            //BloqueiaNovo = !perfilEditar;
            //idCodigoEventoEmpenho.Attributes["class"] = "esconderControle";
            //idDevolucao.Attributes["class"] = "esconderControle";
            //idFornecedor.Attributes["class"] = "mostrarControle";
            //idTransfer.Attributes["class"] = "esconderControle";
            //BloqueiaEmpenhoEvento = true;
            //divQtdLiq.Visible = false;
        }

        public void HabilitarDoacaoNovo()
        {
            HabilitarControlesEdicao(true);
        }

        public void HabilitarDoacaoEdit()
        {
            HabilitarControles = false;
            HabilitarBotoes = true;
        }

        #endregion

        #region Transferencia
        public void HabilitarTransferencia(bool Editar)
        {
            lblFornecedor.Text = "Almoxarifado:";
            HabilitarControles = true;
            BloqueiaListaAlmoxarifado = false;
            BloqueiaEmpenho = true;
            BloqueiaEmpenhoEvento = true;
            BloqueiaGeradorDescricao = true;
            ExibirGeradorDescricao = false;
            BloqueiaBotaoCarregarEmpenho = true;
            BloqueiaNovo = !perfilEditar; // sempre bloqueado. usará apenas o Estornar e Gravar.
            idCodigoEventoEmpenho.Attributes["class"] = "esconderControle";
            idDevolucao.Attributes["class"] = "esconderControle";
            idFornecedor.Attributes["class"] = "esconderControle";
            idTransfer.Attributes["class"] = "mostrarControle";
            VisibleNovo = false;
            divQtdLiq.Visible = false;
            txtDataEmissao.Enabled = false;
        }

        public void HabilitarTransferenciaNovo()
        {
            // verificar o documento se pendente ou efetivado
            HabilitarControlesEdicao(true);
            ExibirGeradorDescricao = false;
            txtDataEmissao.Enabled = false;
            VisibleNovo = false;
        }

        #endregion

        #region Devolucao
        public void HabilitarDevolucao(bool Editar)
        {
            lblFornecedor.Text = "Divisão:";
            HabilitarControles = true;
            BloqueiaEmpenho = true;
            ExibirGeradorDescricao = false;
            BloqueiaListaDivisao = false;
            ExibirListaDivisao = true;
            BloqueiaGeradorDescricao = true;
            BloqueiaEmpenhoEvento = true;
            BloqueiaBotaoCarregarEmpenho = true;
            divQtdLiq.Visible = false;

            // perfil
            BloqueiaNovo = !perfilEditar;
            idCodigoEventoEmpenho.Attributes["class"] = "esconderControle";
            idDevolucao.Attributes["class"] = "mostrarControle";
            idFornecedor.Attributes["class"] = "esconderControle";
            idTransfer.Attributes["class"] = "esconderControle";
        }

        public void HabilitarDevolucaoNovo()
        {
            HabilitarBotoes = true;
            ExibirGeradorDescricao = false;
            BloqueiaListaDivisao = false;
        }

        #endregion

        #region Material Transformado

        public void HabilitarMaterialTransformado(bool Editar)
        {
            lblFornecedor.Text = "Origem:";
            HabilitarControles = true;
            BloqueiaEmpenho = true;
            BloqueiaGeradorDescricao = false;
            BloqueiaNovo = !perfilEditar;
            idCodigoEventoEmpenho.Attributes["class"] = "esconderControle";
            idDevolucao.Attributes["class"] = "esconderControle";
            idFornecedor.Attributes["class"] = "mostrarControle";
            idTransfer.Attributes["class"] = "esconderControle";
            BloqueiaEmpenhoEvento = true;
            BloqueiaBotaoCarregarEmpenho = true;
            divQtdLiq.Visible = false;
        }

        public void HabilitarMaterialTransformadoNovo()
        {
            HabilitarControlesEdicao(true);

        }
        #endregion

        #region Eventos

        private void SelecionaUGELogada()
        {
            //Seleciona a UGE Logada
            int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            if (ddlUGE.Items.Count > 0)
            {
                ddlUGE.SelectedValue = ugeLogado.ToString();
            }
        }

        private void CarregaEmpenhosALiquidar()
        {
            //Carrega a lista de Empenhos a liquidar
            if (ddlUGE.SelectedIndex > 0)
            {
                PopularEmpenhosALiquidar(Convert.ToInt32(ddlUGE.SelectedValue), Convert.ToInt32(rdoTipoMovimento.SelectedValue));
                pnlSideRight.Enabled = true;
                pnlSideRight.Visible = true;
            }
        }

        private void TipoMovimentoEdicao()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);

            if (rdoTipoMovimento.SelectedIndex >= 0)
            {
                //Monta o formumário conforme o tipo de movimento
                mat.HabilitarControles(Convert.ToInt32(rdoTipoMovimento.SelectedValue), perfilEditar);


                //Seta os controles
                lblFornecedor.Visible = true;
                imgLupaFornecedor.Visible = true;
                BloqueiaTipoMovimento = true;
                lblValorTotal.Text = "Valor Total:";
            }
        }

        private void TipoMovimentoSelected()
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            this.RemoveSessao(); // Ao selecionar um novo tipo de movimento, sempre lipar a sessão

            if (rdoTipoMovimento.SelectedIndex >= 0)
            {
                //Monta o formumário conforme o tipo de movimento
                mat.HabilitarControles(Convert.ToInt32(rdoTipoMovimento.SelectedValue), perfilEditar);

                //Limpa o grid
                gridSubItemMaterial.DataSource = null;
                gridSubItemMaterial.DataBind();

                //Seta os controles
                lblFornecedor.Visible = true;
                imgLupaFornecedor.Visible = true;
                lblValorTotal.Text = "Valor Total:";
                mat.LimparMovimento();
                SelecionaUGELogada();
                CarregaEmpenhosALiquidar();
            }
        }
        
        protected void rdoTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            TipoMovimentoSelected();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            BloqueiaListaUGE = true;
            MostrarPainelEdicao = true;
            BloqueiaGravarItem = false;
            imgLupaRequisicao.Visible = false;
        }

        public void ddlSubItemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlSubItemList = (DropDownList)sender;
            RepeaterItem ri = (RepeaterItem)ddlSubItemList.Parent;

            TextBox txtQtdeMovGrid = ri.FindControl("txtQtdeMovGrid") as TextBox;
            Label lblUnidFornecimentoGrid = ri.FindControl("lblUnidFornecimentoGrid") as Label;
            Label lblDescricaoGrid = ri.FindControl("lblDescricaoGrid") as Label;

            // trazer os dados do subitem (para EMPENHO)
            if (ddlSubItemList.SelectedIndex > 0)
            {
                NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
                SubItemMaterialEntity subItem = mat.SelectSubItemMaterialRetorno(Convert.ToInt32(ddlSubItemList.SelectedValue));
                if (subItem != null)
                {
                    // linha substitui a coluna (do lote)
                    System.Web.UI.HtmlControls.HtmlTableRow tbl = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("tblLote");
                    if (!subItem.IsLote.Value)
                        tbl.Style.Add("display", "none"); // Visible = false;
                    else
                        tbl.Style.Add("display", "inherit");  // Visible = true;
                }
                txtQtdeMovGrid.Focus();
            }
        }

        protected void imgLupaFornecedor_Click(object sender, ImageClickEventArgs e)
        {
            SetSession(txtCodFornecedor, "fornecedorId");
            SetSession(txtGeradorDescricao, "fornecedorDados");
        }

        protected void txtQtdeMovGrid_TextChanged(object sender, EventArgs e)
        {
            TextBox txtQtdeMovGrid = (TextBox)sender;

            Label lblUnidFornecimentoGrid = txtQtdeMovGrid.Parent.FindControl("lblUnidFornecimentoGrid") as Label;
            Label lblDescricaoGrid = txtQtdeMovGrid.Parent.FindControl("lblDescricaoGrid") as Label;
            Label lblPrecoUnitGrid = txtQtdeMovGrid.Parent.FindControl("lblPrecoUnitGrid") as Label;
            Label lblValorMovItemGrid = txtQtdeMovGrid.Parent.FindControl("lblValorMovItemGrid") as Label;

            lblValorMovItemGrid.Text = string.Format("{0:0,0.00}", (Convert.ToDecimal(lblPrecoUnitGrid.Text) * Convert.ToInt32(txtQtdeMovGrid.Text)));
        }


        protected void txtItemMaterial_TextChanged(object sender, EventArgs e)
        {
            if (ddlUGE.SelectedIndex <= 0)
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('Informar a UGE');", true);
                return;
            }

            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            if (txtItemMaterial.Text != "")
            {
                mat.ProcurarItemMaterialId(Convert.ToInt32(txtItemMaterial.Text));
            }
        }

        protected void btnGravarItem_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);

            movimento = mat.GravarItem(movimento);

            if (movimento == null)
                return;

            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);
            mat.LimparItem();

            CarregarGridSubitensSessao();
            
            //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }
            else
            {
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }

            pnlGrid.Enabled = true;
            BloqueiaGravar = false;
            MostrarPainelEdicao = false;
        }

        protected void btnCancelarItem_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            mat.CancelarItem();
            this.MostrarPainelEdicao = false;
            pnlGrid.Enabled = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            mat.Cancelar();
            pnlGrid.Enabled = true;
            txtDataEmissao.Enabled = true;
            if (ddlEmpenho.Items.Count > 0)
                ddlEmpenho.SelectedIndex = 0;
            hdfMovimentoId.Value = "";            

            //if (rdoTipoMovimento.SelectedValue == ((int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho).ToString())
            if (rdoTipoMovimento.SelectedValue == ((int)GeralEnum.TipoMovimento.EntradaPorEmpenho).ToString())
            {
                ValorTotalMovimento = null;
                ExibirListaEmpenho = true;
                txtEmpenho.Visible = false;
                LimparListaLicitacao();
            }
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                if (!GravarMovimentoEmpenho())
                    return;
            }

            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                // salva o almoxarifado do perfil
                mov.Almoxarifado = new AlmoxarifadoEntity(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);
                mov.AnoMesReferencia = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                this.AnoMesReferencia = mov.AnoMesReferencia;

                MovimentoEntity movStatus = mat.Gravar(mov);
                if (movStatus != null)
                {
                    RemoveSessao();
                }
                else
                {
                    mov = GetSession<MovimentoEntity>(sessaoMov);
                }
            }

        }

        protected void btnCalcularSomaEmpenho_Click(object sender, EventArgs e)
        {
            CalcularValorEmpenhoRepeater();
        }

        protected void btnHidQtdeMov_Click(object sender, EventArgs e)
        {
            SetFocus(txtValorMovItem);
        }

        protected void lnkCancelarQtdeMovEmpenho_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            movimento = mat.CancelarItemEmpenho(movimento);
            if (movimento == null)
                return;
            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);
            mat.LimparItem();
            gridSubItemMaterial.DataSource = movimento.MovimentoItem;
            gridSubItemMaterial.DataBind();
            //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                this.MostrarPainelEdicao = false;
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }
            else
            {
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
                this.ValorTotalMovimento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }
            BloqueiaGravar = false;
        }

        protected void rdoOperacaoEntrada_SelectedIndexChanged(object sender, EventArgs e)
        {
            // nova entrada
            //if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Nova)
            //    optNovo();
            //if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno)
            //    optEstornar();
            //if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
            //    optNota();
        }

        private void CarregarDadosMovimento()
        {
            NovaEntradaMaterialPresenter presenter = new NovaEntradaMaterialPresenter(this);

            RemoveSession(sessaoMov);
            InstanciaObjetoMovimento();
            movimento.MovimentoItem = null;

            movimento.TipoMovimento.Id = TipoMovimento;
            movimento.UGE = new UGEEntity(UgeId);
            movimento.Almoxarifado = new AlmoxarifadoEntity(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);
            movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(0);

            if (TipoMovimento != 0)
            {
                // procura saÃ­da por transferência
                //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                {
                    movimento.Empenho = this.NumeroEmpenhoCombo;
                }

            }

            PesquisaDocumento.PopularDadosGridDocumento(movimento, 0);
            SetSession<MovimentoEntity>(movimento, sessaoMov);
        }

        protected void imgLuparequisicao_Click(object sender, ImageClickEventArgs e)
        {
            CarregarDadosMovimento();
        }

        protected void btnEstornar_Click(object sender, EventArgs e)
        {
            //NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            //if (GetSession<MovimentoEntity>(sessaoMov) != null)
            //{
            //    MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
            //    mov.Id = Common.Util.TratamentoDados.TryParseInt32(this.Id);
            //    mov = mat.Estornar(mov);
            //    if (mov != null)
            //    {
            //        RemoveSession(sessaoMov);
            //        mat.HabilitarControles(Convert.ToInt32(rdoTipoMovimento.SelectedValue), perfilEditar);                    
            //    }
            //}
        }

        protected void btnExcluirItem_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);

            movimento = mat.ExcluirItem(movimento);

            if (movimento == null)
                return;

            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);

            mat.LimparItem();
            gridSubItemMaterial.DataSource = movimento.MovimentoItem;
            gridSubItemMaterial.DataBind();
            // total do movimento
            if (movimento.MovimentoItem != null && movimento.MovimentoItem.Count > 0)
            {
                this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }
            else
                this.ValorDocumento = 0;
        }

        #endregion

        #region Imprimir

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            mat.Imprimir();
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                paramList.Add("UgeId", UgeId);
                paramList.Add("TipoMovimento", TipoMovimento);
                paramList.Add("FornecedorId", FornecedorId.HasValue ? FornecedorId : 0);
                paramList.Add("DivisaoId", DivisaoId);
                paramList.Add("AlmoxId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);
                paramList.Add("GestorId", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id);

                // usado para impressão
                if (txtDocumentoAvulso.Text != "")
                {
                    NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
                    paramList.Add("NumeroDocumento", new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + txtDocumentoAvulso.Text);
                }

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion

        #region Habilitar Perfis
        //public bool PerfilAlterar
        //{
        //    set
        //    {
        //        // disponÃ­veis
        //        rdoTipoMovimento.Enabled = value;
        //        ddlUGE.Enabled = value;
        //        txtEmpenho.Enabled = value;
        //        txtGeradorDescricao.Enabled = value;
        //        txtDataEmissao.Enabled = value;
        //        txtDataReceb.Enabled = value;
        //        txtObservacoes.Enabled = value;
        //        txtItemMaterial.Enabled = value;
        //        txtQtdeMov.Enabled = value;
        //        txtPrecoUnit.Enabled = value;
        //        txtVencimentoLote.Enabled = value;
        //        txtFabricLoteItem.Enabled = value;
        //        txtIdentLoteItem.Enabled = value;
        //        // indisponÃ­veis
        //        ddlUnidade.Enabled = false;
        //        txtDescricao.Enabled = false;
        //        txtQtdeLiq.Enabled = false;
        //        ddlSubItemClassif.Enabled = false;
        //    }
        //}

        //public bool PerfilConsultar
        //{
        //    set
        //    {
        //        // disponÃ­veis
        //        rdoTipoMovimento.Enabled = value;
        //        ddlUGE.Enabled = value;
        //        txtEmpenho.Enabled = value;
        //        txtGeradorDescricao.Enabled = value;
        //        // indisponÃ­veis
        //        txtDataEmissao.Enabled = false;
        //        txtDataReceb.Enabled = false;
        //        txtObservacoes.Enabled = false;
        //        txtItemMaterial.Enabled = false;
        //        txtQtdeMov.Enabled = false;
        //        txtPrecoUnit.Enabled = false;
        //        txtVencimentoLote.Enabled = false;
        //        txtFabricLoteItem.Enabled = false;
        //        txtIdentLoteItem.Enabled = false;
        //        ddlUnidade.Enabled = false;
        //        txtDescricao.Enabled = false;
        //        txtQtdeLiq.Enabled = false;
        //        ddlSubItemClassif.Enabled = false;
        //    }
        //}
        #endregion

        #region Metodos

        //protected void CarregaGridSubItemMaterial(string Documento)
        //{
        //    NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);

        //    int _ugeId = 0;
        //    int _Id = 0;
        //    int _tipoMovimento = Convert.ToInt32(rdoTipoMovimento.SelectedValue);
        //    if (ddlUGE.SelectedIndex >= 0)
        //    {

        //        // procura o fornecedorID
        //        _ugeId = Convert.ToInt32(ddlUGE.SelectedValue);
        //        _tipoMovimento = Convert.ToInt32(rdoTipoMovimento.SelectedValue);
        //        _Id = Convert.ToInt32(Documento);
                
        //        // transferência - se NOVA, procurar pela SAÃDA
        //        //if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Nova && _tipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
        //        //{
        //        //    _tipoMovimento = (int)GeralEnum.TipoMovimento.SaidaPorTransferencia;
        //        //    _ugeId = 0;
        //        //}
        //        //movimento = mat.ListarMovimentoExistente(_ugeId, _Id, _tipoMovimento);
        //        //// total do movimento
        //        //if (movimento != null)
        //        //{
        //        //    if (movimento.MovimentoItem.Count > 0)
        //        //    {
        //        //        this.ValorDocumento = movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
        //        //    }   
        //        //}
        //    }

        //    if (movimento != null)
        //    {
        //        mat.CarregarMovimentoTela(movimento);
        //        //if (GetSession<MovimentoEntity>("movimento") != null)
        //        if (GetSession<MovimentoEntity>(sessaoMov) != null)
        //        {
        //            //RemoveSession("movimento");
        //            //SetSession(movimento, "movimento");
        //            RemoveSession(sessaoMov);
        //            SetSession(movimento, sessaoMov);
        //        }
        //        else
        //        {
        //            //SetSession(movimento, "movimento");
        //            SetSession(movimento, sessaoMov);
        //        }

        //        gridSubItemMaterial.DataSource = movimento.MovimentoItem;
        //        gridSubItemMaterial.DataBind();

        //        if(movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
        //        {
        //            if(ddlEmpenho.Items.Count > 1)
        //            {
        //                NumeroEmpenhoCombo = movimento.Empenho;
        //            }
        //        }

        //        // habilitar controles
        //        BloqueiaCancelar = false;
        //        if (perfilEditar)
        //        {
        //            // controle especial para transferência. 
        //            // pendente = não contém a movimentação tipo ENTRADA POR TRANSFÃŠRENCIA, só SAÃDA
        //            // efetivada = quando tiver a movimentação tipo SAÃDA POR TRANSFERÃŠNCIA
        //            //{
        //                //rdoOperacaoEntrada.Items[1].Selected = true;
        //                BloqueiaNovo = !perfilEditar;
        //            //}
        //        }
        //    }
        //    BloqueiaListaUGE = true;

        //    // habilitar botão estornar/imprimir nota de recbto caso existam itens no movimento
        //    if (movimento.MovimentoItem.Count > 0)
        //    {
        //        BloqueiaGravar = false;
        //        BloqueiaEstornar = false;
        //        BloqueiaImprimir = false;
        //    }

        //}

        #endregion

        #region Metodos do Empenho

        protected void GravarEmpenho()
        {
            // teste
            foreach (RepeaterItem ri in gridSubItemMaterial.Items)
            {
                Label lblTeste = ri.FindControl("lblValorMovItemGrid") as Label;
            }
        }

        protected bool GravarMovimentoEmpenho()
        {
            bool bGravar = false;
            List<string> msgErro = new List<string>();
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
            {
                MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                // salva o almoxarifado do perfil
                mov.Almoxarifado = new AlmoxarifadoEntity(new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id);
                mov.AnoMesReferencia = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                this.AnoMesReferencia = mov.AnoMesReferencia;

                IList<MovimentoItemEntity> newList = mov.MovimentoItem;

                foreach (RepeaterItem ri in gridSubItemMaterial.Items)
                {
                    // instanciar cada item
                    Label itemMaterialCodigo = ri.FindControl("lblItemMatGrid") as Label;
                    DropDownList drpSubItem = ri.FindControl("ddlSubItemList") as DropDownList;
                    TextBox qtdLiq = ri.FindControl("txtQtdeLiqGrid") as TextBox;
                    Label qtdLiqSiafisico = ri.FindControl("lblQtdeLiqSiafisico") as Label;
                    TextBox qtdMov = ri.FindControl("txtQtdeMovGrid") as TextBox;
                    Label precoUnit = ri.FindControl("lblPrecoUnitGrid") as Label;
                    TextBox txtFabLote = ri.FindControl("txtFabLote") as TextBox;
                    TextBox txtIdLote = ri.FindControl("txtIdLote") as TextBox;
                    TextBox txtVctoLote = ri.FindControl("txtVctoLote") as TextBox;
                    Label lblPosicao = ri.FindControl("lblPosicao") as Label;

                    if (qtdLiq.Text != "")
                    {
                        if (qtdMov.Text != "")
                        {
                            bGravar = true;
                            string subitem = drpSubItem.SelectedValue;
                            MovimentoItemEntity movItem = mov.MovimentoItem.Where(a => a.ItemMaterial.Codigo == Convert.ToInt64(itemMaterialCodigo.Text)).FirstOrDefault();
                            movItem.QtdeLiqSiafisico = TratamentoDados.TryParseInt32(qtdLiqSiafisico.Text);
                            movItem.UGE = new UGEEntity(UgeId);
                            movItem.QtdeLiq = TratamentoDados.TryParseInt32(qtdLiq.Text);
                            movItem.QtdeLiqSiafisico = TratamentoDados.TryParseInt32(qtdLiqSiafisico.Text);
                            movItem.QtdeMov = TratamentoDados.TryParseInt32(qtdMov.Text);
                            movItem.ValorMov = TratamentoDados.TryParseInt32(qtdMov.Text) * TratamentoDados.TryParseDecimal(precoUnit.Text);

                            if (drpSubItem.SelectedIndex <= 0 && (movItem.QtdeMov.HasValue && movItem.QtdeMov > 0))
                            {
                                msgErro.Add("Selecionar um subitem para o item " + lblPosicao.Text + " - " + itemMaterialCodigo.Text + "!");
                            }
                            else
                            {
                                movItem.SubItemMaterial = new SubItemMaterialEntity(TratamentoDados.TryParseInt32(subitem));
                                if (Common.Util.Helper.IsDate(txtVctoLote.Text))
                                    movItem.DataVencimentoLote = Convert.ToDateTime(txtVctoLote.Text);
                                movItem.FabricanteLote = txtFabLote.Text == "" ? null : txtFabLote.Text;
                                movItem.IdentificacaoLote = txtIdLote.Text == "" ? null : txtIdLote.Text;
                                //if (EmpenhoLicitacaoId == (int)GeralEnum.EmpenhoLicitacao.Convite || EmpenhoLicitacaoId == (int)GeralEnum.EmpenhoLicitacao.Dispensa)
                                //{
                                //    // qtde liquidar do SIAFISICO diferente da qtde a liquidar recebida não pode ser aceita
                                //    if (movItem.QtdeLiqSiafisico != 0 && movItem.QtdeLiqSiafisico != movItem.QtdeLiq)
                                //    {
                                //        msgErro.Add("Quantidade mov. do subitem " + drpSubItem.SelectedItem.Text + " deve ser igual Ã  qtde. a liquidar.");
                                //    }
                                //    else
                                //    {
                                //        newList = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
                                //        newList.Add(movItem);
                                //    }
                                //}
                                //else
                                //{
                                    // qtde liquidar do SIAFISICO diferente da qtde a liquidar recebida não pode ser aceita
                                if (movItem.QtdeMov > movItem.QtdeLiq)
                                {
                                    msgErro.Add("Quantidade recebida do subitem " + drpSubItem.SelectedItem.Text + " inválida.");
                                }

                                if (movItem.QtdeMov != 0 && movItem.QtdeLiq > movItem.QtdeLiqSiafisico)
                                {
                                    msgErro.Add("Quantidade a liquidar do subitem " + drpSubItem.SelectedItem.Text + " inválida.");
                                }
                                else
                                {
                                    movItem.SubItemMaterial.ItemMaterial = new ItemMaterialEntity();
                                    movItem.SubItemMaterial.ItemMaterial = movItem.ItemMaterial;
                                    newList = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
                                    newList.Add(movItem);
                                }
                                //}
                                mov.MovimentoItem = newList;
                            }
                        }
                    }
                }

                if (!bGravar)
                {
                    msgErro.Add("Não foi informada nenhuma quantidade no empenho!");
                }

                if (msgErro.Count > 0)
                {
                    ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                    this.ListaErros = msgErro;
                    return false;
                }

                //SetSession(mov, "movimento");
                SetSession(mov, sessaoMov);

                return true;
            }
            return true;
        }

        public void LimparListaEmpenho()  { ddlEmpenho.Items.Clear(); }
        public void LimparListaLicitacao() { ddlEmpenhoEvento.Items.Clear(); }

        private void InstanciaObjetoMovimento()
        {
            if (movimento == null) movimento = new MovimentoEntity();
            movimento.MovimentoItem = null;
            if (movimento.TipoMovimento == null) movimento.TipoMovimento = new TipoMovimentoEntity();
            if (movimento.Almoxarifado == null) movimento.Almoxarifado = new AlmoxarifadoEntity();
            if (movimento.Divisao == null) movimento.Divisao = new DivisaoEntity();
            if (movimento.MovimAlmoxOrigemDestino == null) movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();
            if (movimento.UGE == null) movimento.UGE = new UGEEntity();
        }

        protected void ddlEmpenho_SelectedIndexChanged(object sender, EventArgs e)
        {
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            // Procurar itens por empenho
            gridSubItemMaterial.DataSource = null;
            gridSubItemMaterial.DataBind();
            LimparDadosDocumento();
        }

        public void CalcularValorEmpenhoRepeater()
        {
            decimal valTotal = 0;
            if (gridSubItemMaterial.Items.Count > 1)
            {
                foreach (RepeaterItem ri in gridSubItemMaterial.Items)
                {
                    // calcular o valor total (qtde * preco unitario)
                    TextBox qtdeMov = (TextBox)ri.FindControl("txtQtdeMovGrid");
                    Label precoUnit = (Label)ri.FindControl("lblPrecoUnitGrid");
                    if (qtdeMov.Text != "")
                        valTotal += Convert.ToDecimal(qtdeMov.Text) * Convert.ToDecimal(precoUnit.Text);
                }
                ValorTotalMovimento = valTotal;
            }
        }

        public void SetarUGELogado()
        {
            int ugeLogado = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id.Value;
            if (ddlUGE.Items.Count > 0) ddlUGE.SelectedValue = ugeLogado.ToString();
        }

        override protected void OnInit(EventArgs e) { base.OnInit(e); }

        public void LimparDadosDocumento() 
        {
            ValorDocumento = null;
            DataMovimento = null;
            txtDataEmissao.Text = "";
            GeradorDescricao = null;
            FornecedorId = null;
            NumeroDocumento = null;
            txtValorTotal.Text = "";
            Observacoes = null;
            EmpenhoLicitacaoId = null;
        }

        #endregion

        #region Focar Objeto

        public void FocarFornecedor() { SetFocus(txtCodFornecedor); }
        public void FocarDataDocumento() { SetFocus(txtDataEmissao); }
        public void FocarItemDataVencLote() { SetFocus(txtVencimentoLote); }
        public void FocarDataRecebimento() { SetFocus(txtDataReceb); }
        public void FocarItemQtdeEntrar() { SetFocus(txtQtdeMov); }
        public void FocarValorMovItem() { SetFocus(txtValorMovItem); }
        public void FocarFabricLoteItem() { SetFocus(txtFabricLoteItem); }

        #endregion

        #region Operação Entrada

        //protected void optNota()
        //{
        //    NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
        //    if (!tipoMovimentoSelecionado())
        //    {
        //        ExibirMensagem("Selecionar o tipo de movimento.");
        //        return;
        //    }
        //    LimparGridSubItemMaterial();
        //    imgLupaRequisicao.Visible = true;
        //    btnEstornar.Visible = false;
        //    btnGravar.Visible = false;
        //    btnCancelar.Visible = true;
        //    carrgarPermissaoAcesso();
        //    mat.HabilitarControlesNovo(Convert.ToInt16(rdoTipoMovimento.SelectedValue));
        //    imgLupaFornecedor.Visible = false;
        //    VisibleNovo = false;
        //    if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
        //        BloqueiaBotaoCarregarEmpenho = true;
        //    BloqueiaImprimir = true;
        //    ExibirImprimir = true;
        //    BloqueiaEstornar = true;
        //    BloqueiaGravar = true;
        //}

        //protected void optEstornar()
        //{
        //    NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
        //    if (!tipoMovimentoSelecionado())
        //    {
        //        ExibirMensagem("Selecionar o tipo de movimento.");
        //        return;
        //    }
        //    LimparGridSubItemMaterial();
        //    imgLupaRequisicao.Visible = true;
        //    btnEstornar.Visible = true;
        //    BloqueiaEstornar = false;
        //    btnGravar.Visible = false;
        //    btnCancelar.Visible = true;
        //    carrgarPermissaoAcesso();
        //    mat.HabilitarControlesNovo(Convert.ToInt16(rdoTipoMovimento.SelectedValue));
        //    imgLupaFornecedor.Visible = false;
        //    VisibleNovo = false;
        //    if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
        //        BloqueiaBotaoCarregarEmpenho = true;
        //    BloqueiaImprimir = true;
        //    ExibirImprimir = false;
        //    BloqueiaEstornar = true;
        //}

        //protected void optNovo()
        //{
        //    if (!tipoMovimentoSelecionado())
        //    {
        //        return;
        //    }
        //    LimparGridSubItemMaterial();
        //    VisibleNovo = true;
        //    btnGravar.Visible = true;
        //    BloqueiaGravar = false;
        //    btnEstornar.Visible = false;
        //    BloqueiaEstornar = true;
        //    btnCancelar.Visible = true;
        //    imgLupaRequisicao.Visible = false;
        //    VisibleNovo = true;
        //    List<String> listaErro = new List<string>();
        //    int UgeId = 0;
        //    NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
        //    btnExcluirItem.Visible = true;
        //    if (ddlUGE.SelectedIndex == 0)
        //    {
        //        ExibirMensagem("Selecionar a UGE.");
        //        return;
        //    }
        //    carregarPermissaoAcesso();
        //    UgeId = TratamentoDados.TryParseInt32(ddlUGE.SelectedValue).Value;
        //    // não limpar o grid caso seja aquisição direta
        //    txtValorMovItem.Enabled = true;
        //    if (this.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
        //    {
        //        //RemoveSession("movimento");
        //        RemoveSession(sessaoMov);
        //        mat.LimparMovimento();
        //        BloqueiaBotaoCarregarEmpenho = true;
        //    }
        //    else
        //    {
        //        BloqueiaBotaoCarregarEmpenho = false;
        //        txtValorMovItem.Enabled = false;
        //        //if (GetSession<MovimentoEntity>("movimento") != null)
        //        if (GetSession<MovimentoEntity>(sessaoMov) != null)
        //        {
        //            //movimento = GetSession<MovimentoEntity>("movimento");
        //            movimento = GetSession<MovimentoEntity>(sessaoMov);
        //            movimento.Id = null;
        //        }

        //        if (listaErro.Count > 0)
        //        {
        //            ExibirMensagem("Inconsistências encontradas. Favor verificar.");
        //            this.ListaErros = listaErro;
        //            return;
        //        }
        //        mat.LimparMovimentoCompraDireta();
        //        BloqueiaNovo = true;
        //    }
        //    mat.LimparItem();
        //    this.UgeId = UgeId;
        //    this.BloqueiaItemEfetivado = false;
        //    HabilitaPesquisaItemMaterial = true;
        //    mat.HabilitarControlesNovo(Convert.ToInt16(rdoTipoMovimento.SelectedValue));
        //    if(TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
        //        imgLupaRequisicao.Visible = true;
        //    else
        //        imgLupaRequisicao.Visible = false;
        //    ExibirImprimir = false;
        //    BloqueiaGravar = true;
        //}

        #endregion

        #region Grid

        protected void gridSubItemMaterial_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int subItemId = 0;

            if (e.CommandName == "Select")
            {
                subItemId = Convert.ToInt32(e.CommandArgument);
                // atribuições para empenho
                //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                {
                    if (btnNovo.Enabled)
                    {
                        MostrarPainelEdicao = true;
                        BloqueiaGravarItem = true;
                        BloqueiaExcluirItem = true;
                    }
                    else
                    {
                        btnExcluirItem.Visible = true;
                        BloqueiaGravarItem = false;
                        //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                        if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                        {
                            btnExcluirItem.Visible = false;
                        }
                    }
                }
                Label lblMovimentoId = (Label)e.Item.FindControl("lblMovimentoId");
                Label lblMovimentoItemId = (Label)e.Item.FindControl("lblMovimentoItemId");
                Label lblItemMaterialId = (Label)e.Item.FindControl("lblItemMaterialId");
                Label lblUnidadeFornecimentoId = (Label)e.Item.FindControl("lblUnidadeFornecimentoId");
                this.Id = lblMovimentoId.Text;
                this.MovimentoItemId = subItemId;

                NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    MovimentoEntity mov = GetSession<MovimentoEntity>(sessaoMov);
                    mat.LerRegistro(mov, this.MovimentoItemId.Value);
                    //pnlGrid.Enabled = false;
                }
            }

        }

        protected void PreencherGridHeader(int tipoMovimento, RepeaterItemEventArgs e)
        {
            System.Web.UI.HtmlControls.HtmlTableCell celHdrQtdeLiq = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celHdrQtdeLiq");
            System.Web.UI.HtmlControls.HtmlTableCell hdr2QtdLiq1 = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdr2QtdLiq1");
            System.Web.UI.HtmlControls.HtmlTableCell hdr2QtdLiq2 = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdr2QtdLiq2");
            System.Web.UI.HtmlControls.HtmlTableCell rowLote = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdrLote");
            System.Web.UI.HtmlControls.HtmlTableCell rowDescricao = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdrDescricao");
            System.Web.UI.HtmlControls.HtmlTableCell hdrEdit = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdrEdit");
            System.Web.UI.HtmlControls.HtmlTableCell hdrPrecoTotal = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("hdrPrecoTotal");
            System.Web.UI.HtmlControls.HtmlTableCell celHdrUnidSiaf = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celHdrUnidSiaf");
            System.Web.UI.HtmlControls.HtmlTableCell celHdrUnidSub = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celHdrUnidSub");


            celHdrUnidSiaf.Style.Add("display", "none"); // Visible = false;
            celHdrUnidSub.Style.Add("display", "inherit"); // Visible = false;
            hdrPrecoTotal.Style.Add("display", "inherit"); // Visible = true;
            celHdrQtdeLiq.Style.Add("display", "none"); // Visible = false;
            hdr2QtdLiq1.Style.Add("display", "none"); // Visible = false;
            hdr2QtdLiq2.Style.Add("display", "none"); // Visible = false;
            hdrEdit.Style.Add("display", "inherit"); // Visible = true;
            hdrEdit.RowSpan = 4;
            rowLote.ColSpan = 6;
            rowDescricao.ColSpan = 6;

            //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                celHdrQtdeLiq.Style.Add("display", "inherit"); // Visible = true;
            //    if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
            //    {
            //        hdr2QtdLiq1.Style.Add("display", "none"); // Visible = false;
            //        hdr2QtdLiq2.Style.Add("display", "inherit"); // Visible = false;
            //        hdr2QtdLiq2.ColSpan = 2;
            //        celHdrUnidSiaf.Style.Add("display", "none"); // Visible = false;
            //        celHdrUnidSub.Style.Add("display", "inherit"); // Visible = false;
            //        celHdrUnidSub.ColSpan = 2;
            //        rowLote.ColSpan = 8;
            //        rowDescricao.ColSpan = 8;
            //    }
            //    else
            //    {
            //        hdr2QtdLiq1.Style.Add("display", "inherit"); // Visible = false;
            //        hdr2QtdLiq2.Style.Add("display", "inherit"); // Visible = false;
            //        hdr2QtdLiq2.ColSpan = 1;
            //        celHdrUnidSiaf.Style.Add("display", "inherit"); // Visible = false;
            //        celHdrUnidSub.Style.Add("display", "inherit"); // Visible = false;
            //        celHdrUnidSub.ColSpan = 1;
            //        rowLote.ColSpan = 8;
            //        rowDescricao.ColSpan = 8;
            //    }

            //    hdrPrecoTotal.Style.Add("display", "none"); // Visible = false;
            //    hdrEdit.Style.Add("display", "none");  // Visible = false;
            }
            else
            {
                //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia || this.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia || this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia || this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado || this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                {
                    hdrEdit.Style.Add("display", "none"); // Visible = false;
                }
                else
                {
                    celHdrQtdeLiq.Style.Add("display", "none"); // Visible = false;
                }
            }
        }

        protected void PreencherGridItem(int tipoMovimento, RepeaterItemEventArgs e,string lblNaturezaDespesaEmpenho)
        { 
            RepeaterItem ri = e.Item;
            NovaEntradaMaterialPresenter mat = new NovaEntradaMaterialPresenter(this);
            // fazer cast dos objetos dentro do grid
            DropDownList drpSubItens = ri.FindControl("ddlSubItemList") as DropDownList;
            Label lblItemMaterialId = ri.FindControl("lblItemMaterialId") as Label;
            Label lblSubItem = ri.FindControl("lblSubItem") as Label;
            Label lblUnidFornecimentoGridSiafisico = (Label)ri.FindControl("lblUnidFornecimentoGridSiafisico");
            System.Web.UI.HtmlControls.HtmlTableCell celQtdeLiq = ri.FindControl("celQtdeLiq") as System.Web.UI.HtmlControls.HtmlTableCell;
            System.Web.UI.HtmlControls.HtmlTableCell celQtdeLiq2 = ri.FindControl("celQtdeLiq2") as System.Web.UI.HtmlControls.HtmlTableCell;
            System.Web.UI.HtmlControls.HtmlTableCell celUnidSiaf = ri.FindControl("celUnidSiaf") as System.Web.UI.HtmlControls.HtmlTableCell;
            System.Web.UI.HtmlControls.HtmlTableCell celUnidSub = ri.FindControl("celUnidSub") as System.Web.UI.HtmlControls.HtmlTableCell;

            // dados do lote
            TextBox txtFabLote = (TextBox)e.Item.FindControl("txtFabLote");
            TextBox txtIdLote = (TextBox)e.Item.FindControl("txtIdLote");
            TextBox txtVctoLote = (TextBox)e.Item.FindControl("txtVctoLote");
            TextBox txtQtdeLiqGrid = (TextBox)e.Item.FindControl("txtQtdeLiqGrid");
            TextBox txtQtdeMovGrid = (TextBox)e.Item.FindControl("txtQtdeMovGrid");
            txtQtdeMovGrid.Style.Add("text-align", "center");
            txtQtdeLiqGrid.Style.Add("text-align", "center");
            txtFabLote.Style.Add("text-align", "center");
            txtIdLote.Style.Add("text-align", "center");
            txtVctoLote.Style.Add("text-align", "center");
            // colunas
            System.Web.UI.HtmlControls.HtmlTableCell celPosicao = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celPosicao");
            System.Web.UI.HtmlControls.HtmlTableCell cellEdit = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("cellEdit");
            System.Web.UI.HtmlControls.HtmlTableCell cellVlTotal = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("celPrecoTotal");
            // linhas
            System.Web.UI.HtmlControls.HtmlTableCell rowLote = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("rowLote");
            System.Web.UI.HtmlControls.HtmlTableCell rowDescricao = (System.Web.UI.HtmlControls.HtmlTableCell)e.Item.FindControl("rowDescricao");
            SubItemMaterialPresenter item = new SubItemMaterialPresenter();

            lblSubItem.Style.Add("display", "inherit");
            drpSubItens.Style.Add("display", "none");
            lblUnidFornecimentoGridSiafisico.Style.Add("display", "none");
            cellVlTotal.Style.Add("display", "inherit");
            cellEdit.Style.Add("display", "inherit");
            celQtdeLiq.Style.Add("display", "none");
            celQtdeLiq2.Style.Add("display", "none");
            celUnidSiaf.Style.Add("display", "none");
            celUnidSub.Style.Add("display", "inherit");

            TextBox txtVctoLote2 = (TextBox)e.Item.FindControl("txtVctoLote");
            ScriptManager.RegisterStartupScript(txtVctoLote2, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
            Label lblSubItemMaterialId = (Label)ri.FindControl("lblSubItemMaterialId");
            SubItemMaterialEntity subItem = mat.SelectSubItemMaterialRetorno(Convert.ToInt32(lblSubItemMaterialId.Text));
            if (subItem != null)
            {
                System.Web.UI.HtmlControls.HtmlTableRow tbl = (System.Web.UI.HtmlControls.HtmlTableRow)ri.FindControl("tblLote");
                if (!subItem.IsLote.Value)
                    tbl.Style.Add("display", "none"); // Visible = false;
                else
                    tbl.Style.Add("display", "inherit"); // Visible = true;
            }

            cellEdit.RowSpan = 3;
            celPosicao.RowSpan = 3;
            rowLote.ColSpan = 6;
            rowDescricao.ColSpan = 6;
            //if (TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                //if (Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.Estorno || Convert.ToInt16(rdoOperacaoEntrada.SelectedValue) == (int)GeralEnum.OperacaoEntrada.NotaRecebimento)
                //{
                //    drpSubItens.Style.Add("display", "none");
                //    lblSubItem.Style.Add("display", "inherit");
                //    celQtdeLiq.Style.Add("display", "none");
                //    celQtdeLiq2.Style.Add("display", "inherit");
                //    celQtdeLiq2.ColSpan = 2;
                //    txtQtdeLiqGrid.BorderStyle = BorderStyle.None;
                //    txtQtdeLiqGrid.Enabled = false;
                //    txtQtdeLiqGrid.Style.Add("background-color", "transparent");
                //    txtQtdeMovGrid.BorderStyle = BorderStyle.None;
                //    txtQtdeMovGrid.Enabled = false;
                //    txtQtdeMovGrid.Style.Add("background-color", "transparent");
                //    celUnidSiaf.Style.Add("display", "none");
                //    celUnidSub.Style.Add("display", "inherit");
                //    celUnidSub.ColSpan = 2;
                //    rowLote.ColSpan = 7;
                //    rowDescricao.ColSpan = 7;
                //}
                //else
                //{
                //    lblSubItem.Style.Add("display", "none");
                //    drpSubItens.Style.Add("display", "inherit");
                //    celQtdeLiq.Style.Add("display", "inherit");
                //    celQtdeLiq2.Style.Add("display", "inherit");
                //    txtQtdeLiqGrid.BorderStyle = BorderStyle.Solid;
                //    txtQtdeLiqGrid.Enabled = true;
                //    txtQtdeMovGrid.BorderStyle = BorderStyle.Solid;
                //    txtQtdeMovGrid.Enabled = true;
                //    celUnidSiaf.Style.Add("display", "inherit");
                //    celUnidSub.Style.Add("display", "inherit");
                //    // desabilitar campos caso a quantidade recebida atingiu a qtde total do item
                //    if (Convert.ToInt32(txtQtdeLiqGrid.Text) == 0)
                //    {
                //        txtQtdeLiqGrid.Enabled = false;
                //        txtQtdeMovGrid.Enabled = false;
                //    }
                //    rowLote.ColSpan = 8;
                //    rowDescricao.ColSpan = 8;
                //}
                cellEdit.Style.Add("display", "none");
                cellVlTotal.Style.Add("display", "none");
                carregarListaSubItens(drpSubItens, lblNaturezaDespesaEmpenho, Convert.ToInt32(lblItemMaterialId.Text));
                lblUnidFornecimentoGridSiafisico.Style.Add("display", "inherit");
            }
            else
            {
                //if (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
                if ((this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                {
                    cellEdit.Style.Add("display", "none");  // Visible = false;
                    lblSubItem.Style.Add("display", "inherit"); // Visible = true;
                    drpSubItens.Style.Add("display", "none"); // Visible = false;
                    if (txtFabLote != null)
                    {
                        txtFabLote.BorderStyle = BorderStyle.None;
                        txtFabLote.Enabled = false;
                        txtFabLote.Style.Add("background-color", "transparent");
                        txtIdLote.BorderStyle = BorderStyle.None;
                        txtIdLote.Enabled = false;
                        txtIdLote.Style.Add("background-color", "transparent");
                        txtVctoLote.BorderStyle = BorderStyle.None;
                        txtVctoLote.Enabled = false;
                        txtVctoLote.Style.Add("background-color", "transparent");
                    }
                    txtQtdeMovGrid.BorderStyle = BorderStyle.None;
                    txtQtdeMovGrid.Enabled = false;
                    txtQtdeMovGrid.Style.Add("background-color", "transparent");
                }
                else
                {
                    // outros tipos de entrada: campos serão desabilitados para edição
                    if (txtFabLote != null)
                    {
                        txtFabLote.BorderStyle = BorderStyle.None;
                        txtFabLote.Enabled = false;
                        txtFabLote.Style.Add("background-color", "transparent");
                        txtIdLote.BorderStyle = BorderStyle.None;
                        txtIdLote.Enabled = false;
                        txtIdLote.Style.Add("background-color", "transparent");
                        txtVctoLote.BorderStyle = BorderStyle.None;
                        txtVctoLote.Enabled = false;
                        txtVctoLote.Style.Add("background-color", "transparent");
                    }
                    txtQtdeMovGrid.BorderStyle = BorderStyle.None;
                    txtQtdeMovGrid.Enabled = false;
                    txtQtdeMovGrid.Style.Add("background-color", "transparent");
                    lblUnidFornecimentoGridSiafisico.Visible = false;
                }
            }
        }

        protected void gridSubItemMaterial_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemIndex > -1)
            {
                //ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
                //scriptManager.RegisterPostBackControl(e.Item.FindControl("linkID"));
                //Page.ClientScript.ValidateEvent(e.Item.FindControl("linkID").UniqueID);
                

                //var trigger = new PostBackTrigger();
                //trigger.ControlID = e.Item.FindControl("linkID").UniqueID;
                //udpPanel.Triggers.Add(trigger);
            }

            if (movimento.MovimentoItem == null)
            {
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    movimento = GetSession<MovimentoEntity>(sessaoMov);
                }
            }
            string lblNaturezaDespesaEmpenho = movimento.NaturezaDespesaEmpenho;

            if (e.Item.ItemType == ListItemType.Header)
            {
                PreencherGridHeader(TipoMovimento, e);
            }
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                PreencherGridItem(TipoMovimento, e, lblNaturezaDespesaEmpenho);
            }
        }

        #endregion

        protected void btnCarregarEmpenho_Click(object sender, EventArgs e)
        {
        }
    }
}
