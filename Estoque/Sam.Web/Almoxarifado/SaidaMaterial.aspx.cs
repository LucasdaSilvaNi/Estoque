using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Enums;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using Sam.Domain.Business.SIAFEM;
using enumTipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using enumTipoRequisicao = Sam.Common.Util.GeralEnum.TipoRequisicao;
using helperTratamento = Sam.Common.Util.TratamentoDados;
using tipoMovimentacao = Sam.Common.Util.GeralEnum.TipoMovimento;
using tipoOperacao = Sam.Common.Util.GeralEnum.OperacaoSaida;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;
using Sam.Domain.Business;
using System.Drawing;

namespace Sam.Web.Almoxarifado
{
    public partial class SaidaMaterial : PageBase, ISaidaMaterialView, IPostBackEventHandler
    {
        private MovimentoEntity movimento;
        private MovimentoEntity movimentoFiltro = new MovimentoEntity();
        private string sessaoMov = "movimento";
        private string sessaoLote = "lote";
        private readonly string cstListagemPTRes = "listagemPTRes";
        private readonly string cstListagemPTResAcao = "listagemPTResAcao";
        private readonly string ChaveSessao_CampoTipoMaterialDivergente_LancamentoSIAFEM = "valorCampoTipoMaterialDivergente";
        private readonly string ChaveSessao_CampoTipoMaterial_LancamentoSIAFEM_MSG_ERRO = "valorCampoTipoMaterial_MSG_ERRO";
        private int ugeCodigo = -1;
        private string loginSiafem = string.Empty;
        private string senhaSiafem = string.Empty;
        private bool dadosAcessoSiafemPreenchidos = false;



        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            ucAcessoSIAFEM.EvchamaEvento += new Controles.WSSenha.chamaEvento(ExecutaEvento);
            ucAcessoSIAFEM.EvchamaEventoCancelar += new Controles.WSSenha.EventoCancelar(ExecutaEventoCancelar);

            InstanciaObjetoMovimento();
            AtualizarSessao();
            RegistraJSDataMovimento();

            ConsultarLimparDataMovimento();
            if (!IsPostBack)
            {

                RegistraJavascript();

                SaidaMaterialPresenter saidaPresenter = new SaidaMaterialPresenter(this);
                saidaPresenter.Load();

                RemoveSession(sessaoMov);
                PopularListaTipoMovimentoSaida();
                //PopularListaDivisao();
                MontarViewEspecificaParaTipoMovimentacaoSaida();
                DataMovimento = null;
                CarregarRascunho();
                SetRequisicao();



            }

            BloqueiaGravar = true;
            PesquisaSubitem1.UsaSaldo = true;
            PesquisaSubitem1.FiltrarAlmox = true;
            BloqueiaBotaoAdicionar();

            VerificaMesFechadoSIAFEM();
            OcultarOrgaoAlmox();


            if (!((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.NotaFornecimento).ToString()) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada)))
            {
                if (!dadosAcessoSiafemPreenchidos)
                    efetuarAcessoSIAFEM();
            }
        }


        public void InativarAdItem()
        {

            rblTipoOperacao.Items[0].Enabled = false;
            ExibeBotaoNovo = false;
            if (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString())
                rblTipoOperacao.SelectedValue = ((int)enumTipoRequisicao.NotaFornecimento).ToString();

        }

        public void SetRequisicao()
        {

            Session["rblTipoOperacao"] = rblTipoOperacao.SelectedValue;

            if (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente)
            {
                if (Convert.ToInt32(rblTipoOperacao.SelectedValue) == (int)enumTipoRequisicao.Nova)
                {
                    ddlDivisao.Enabled = true;
                    Session["rblTipoMovimento"] = (int)enumTipoMovimento.RequisicaoPendente;
                }
                else
                    Session["rblTipoMovimento"] = (int)enumTipoMovimento.RequisicaoAprovada;
            }
            else
                Session["rblTipoMovimento"] = Convert.ToInt32(rblTipoMovimento.SelectedValue);
        }
        /// <summary>
        /// Caso o mês esteja fechado, permitir apenas reimpressão de notas.
        /// </summary>
        private void VerificaMesFechadoSIAFEM()
        {
            SaidaMaterialPresenter objPresenter = null;
            string anoMesRef = null;
            bool mesJahFechado = false;

            anoMesRef = this.AnoMesReferencia;
            objPresenter = new SaidaMaterialPresenter(this);
            mesJahFechado = objPresenter.VerificaStatusFechadoMesReferenciaSIAFEM(anoMesRef, true);

            if (mesJahFechado)
            {
                BloqueiaTipoOperacao = BloqueiaBotaoNovo = BloqueiaBotaoEstornar = true;
                TipoOperacao = tipoOperacao.NotaFornecimento.GetHashCode();
                AtivarModeloTipoOperacaoFechado();
            }

        }

        public void ConsultarLimparDataMovimento()
        {
            if (!string.IsNullOrEmpty(txtDataMovimento.Text))
            {

                if (txtDataMovimento.Text == "99/99/9999")
                    txtDataMovimento.Text = string.Empty;
                else
                {
                    if (Convert.ToDateTime(txtDataMovimento.Text) == DateTime.MinValue)
                        txtDataMovimento.Text = string.Empty;
                    else
                        Session["txtDataMovimento"] = txtDataMovimento.Text;
                }
            }
        }

        public void RegistraJSDataMovimento()
        {
            ScriptManager.RegisterStartupScript(this.txtDataMovimento, GetType(), "dataFormat", "$('.dataFormat').mask('99/99/9999');", true);
        }

        public void RegistraJavascript()
        {
            btnAdd.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar a gravação.');");

            txtSaldo.Attributes.Add("onblur", "return formatarEntradaNumerica();");
            txtSaldo.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
            txtQtdSolicitada.Attributes.Add("onblur", "return formatarEntradaNumerica();");
            txtQtdSolicitada.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
            txtQtdeMov.Attributes.Add("onblur", "return formatarEntradaNumerica();");
            txtQtdeMov.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));

            for (int i = 0; i < rptLote.Items.Count; i++)
            {
                TextBox txtQtdeLote = (TextBox)rptLote.Items[i].FindControl("txtQtdeLote");
                txtQtdeLote.Attributes.Add("onblur", "return formatarEntradaNumerica();");
                txtQtdeLote.Attributes.Add("onkeypress", String.Format("return {0}(event)", ((base.numCasasDecimaisMaterialQtde == 0) ? "SomenteNumero" : "SomenteNumeroDecimal")));
            }

            if (base.numCasasDecimaisMaterialQtde == 3)
            {
                ScriptManager.RegisterStartupScript(this.txtSaldo, GetType(), "numerico", "$('.numerico').floatnumber(',',3);", true);
                ScriptManager.RegisterStartupScript(this.txtQtdSolicitada, GetType(), "numerico", "$('.numerico').floatnumber(',',3);", true);
                ScriptManager.RegisterStartupScript(this.txtQtdeMov, GetType(), "numerico", "$('.numerico').floatnumber(',',3);", true);
            }
        }

        public void CarregarRascunho()
        {
            if (Page.Session["idRascunho"] != null)
            {
                int idRascunho = GetSession<int>("idRascunho");
                SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);
                MovimentoEntity movimento = presenter.CarregarRascunho(idRascunho);
                presenter.CarregarMovimentoTela(movimento);

                SetSession<MovimentoEntity>(movimento, sessaoMov);
                ListarMovimentoItensRascunho();
                //MontarPainelRequisicoes();
                MontarViewEspecificaParaTipoMovimentacaoSaida();

                RemoveSession("idRascunho");
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {

            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) && (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString()))
            {
                List<String> erro = new List<string>();
                if (!PermitirSaidaRequisicao(movimento))
                {
                    ExibirMensagem("Inconsistências encontradas");
                    this.ListaErros = new List<string>() { string.Format($"Não é possível reclassificar o bem patrimonial, pois a UGE de destino não está no mesmo mês ref. contábil que o estoque.") };
                    return;
                }
            }
            SaidaMaterialPresenter mat = new SaidaMaterialPresenter(this);
            movimento = mat.ExcluirItem(movimento);

            if (movimento == null)
                return;

            RemoveSession(sessaoMov);
            SetSession(movimento, sessaoMov);
            ListarMovimentoItensRascunho();
            mat.Cancelar();
            LimparLote(null, null);

            ValorDocumento = movimento.ValorDocumento = movimento.MovimentoItem.Sum(movItem => movItem.ValorMov ?? 0.00m).truncarDuasCasas();
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            SaidaMaterialPresenter mat = new SaidaMaterialPresenter(this);
            mat.Imprimir();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) && (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString()))
            {
                ////List<String> erro = new List<string>();
                //if (almoxarifadoNaoPossuiFechamentosMensais())
                //{
                //    ExibirMensagem("Inconsistências encontradas");
                //    this.ListaErros = new List<string>() { string.Format($"Não é possível atender Requisição de Material pois o almoxarifado não possui nenhum Fechamento Mensal realizado!") };
                //    return;
                //}

                if (!PermitirSaidaRequisicao(movimento))
                {
                    ExibirMensagem("Inconsistências encontradas");
                    this.ListaErros = new List<string>() { string.Format($"Não é possível reclassificar o bem patrimonial, pois a UGE de destino não está no mesmo mês ref. contábil que o estoque.") };
                    return;
                }
            }
            if (!ValidaAdicionar())
            {
                return;
            }
            ViewState["botao"] = "btnNovo";
            imgLupaSubItem.Visible = true;
            txtSubItem.Enabled = true;
            SaidaMaterialPresenter saidaPresenter = new SaidaMaterialPresenter(this);
            saidaPresenter.Novo();
            MudarTextoBotaoEditar(false);

            ExibirCamposPTRes();           
        }

        private bool almoxarifadoNaoPossuiFechamentosMensais()
        {
            bool _almoxarifadoNaoPossuiFechamentosMensais = false;
            SaidaMaterialPresenter saidaMaterialPresenter = new SaidaMaterialPresenter(this);


            _almoxarifadoNaoPossuiFechamentosMensais = !saidaMaterialPresenter.AlmoxarifadoContemFechamentos();
            return _almoxarifadoNaoPossuiFechamentosMensais;
        }

        public bool ValidaAdicionar()
        {
            bool validado = true;
            if (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString())
            {
                ddlUGE.Enabled = false;
                var mensagem = string.Empty;
                string alertCE = string.IsNullOrEmpty(txtInfoSiafemCE.Text) ? "Inscrição (CE)// " : "";
                string alertData = string.IsNullOrEmpty(txtDataMovimento.Text) ? "Data de Movimento// " : "";
                string almoxarifadoOuCpf = "";
                switch (int.Parse(rblTipoMovimento.SelectedValue))
                {
                    case (int)enumTipoMovimento.SaidaPorDoacao:                                               
                        if ((string)(rblOpcoes.SelectedValue).ToString() == "orgao_almoxarifado")
                        {
                            almoxarifadoOuCpf = ddlAmoxarifado_TransferenciaDoacao.SelectedValue == "0" ? "Almoxarifado//" : "";
                        }
                        if ((string)(rblOpcoes.SelectedValue).ToString() == "cpf_cnpj")
                        {
                            almoxarifadoOuCpf = string.IsNullOrEmpty(txtCPF_CNPJ.Text) ? "CPF/CNPJ//" : "";
                        }
                        mensagem = alertCE + alertData + almoxarifadoOuCpf;
                        break;                 
                    case (int)enumTipoMovimento.SaidaPorTransferencia:
                        almoxarifadoOuCpf = ddlAmoxarifado_TransferenciaDoacao.SelectedValue == "0" ? "Almoxarifado//" : "";
                        mensagem = alertCE + almoxarifadoOuCpf + alertData;                     
                        break;                   
                    case (int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                        string alertUge = string.IsNullOrEmpty(txtDescricaoAvulsa.Text) ? "UGE Destino// " : "";
                        mensagem = alertCE + alertUge + alertData;
                        break;
                    case (int)enumTipoMovimento.RequisicaoPendente:
                        mensagem = alertData;
                        break;
                    default:
                        //SaidaPorExtravioFurtoRoubo:
                        //SaidaPorIncorporacaoIndevida:
                        //SaidaPorPerda:
                        //SaidaInservivelQuebra:
                        //SaidaParaAmostraExposicaoAnalise:
                        mensagem = alertCE + alertData;
                        break;
                }
                if (!string.IsNullOrEmpty(mensagem))
                {
                    ExibirMensagem("Inconsistências encontradas.");
                    this.ListaErros = new List<string>() { string.Format($"Preencher o(s) campo(s): {mensagem} ") };
                    validado = false;
                }
            }         
            return  validado;
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            var almoxarifadoId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            PopularListaLote(almoxarifadoId.Value);
        }

        protected void rblTipoOperacao_SelectedIndexChanged(object sender, EventArgs e)
        {

            AtivarModeloTipoOperacao();


        }

        public void AtivarModeloTipoOperacao()
        {
            txtInfoSiafemCE.Enabled = true;
            LimparConteudo();
            Session["tipoRequisicao"] = Convert.ToInt32(rblTipoOperacao.SelectedValue);
            //MontarPainelRequisicoes();
            MontarViewEspecificaParaTipoMovimentacaoSaida();
            HabilitarBotoes();
            SetRequisicao();
            PopularSubTipoMovimento(Convert.ToInt32(rblTipoMovimento.SelectedValue));

            if (!this.ModoGravacaoOuEstorno)
                BloqueiaCamposInfoSiafemCE = true;

            OcultarOrgaoAlmox();



            if (int.Parse(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.SaidaPorDoacao)
            {
                TratamentoOpcoes();

                if (rblTipoOperacao.SelectedValue == "1")
                {
                    txtCPF_CNPJ.Enabled = true;
                    ddlOrgao_TransferenciaDoacao.Enabled = true;
                    ddlAmoxarifado_TransferenciaDoacao.Enabled = true;
                }
                else
                {
                    txtCPF_CNPJ.Enabled = false;
                    ddlOrgao_TransferenciaDoacao.Enabled = false;
                    ddlAmoxarifado_TransferenciaDoacao.Enabled = false;
                }
            }
            else
            {
                trCampo_CPF_CNPJ.Visible = false;
                trRadioOpcoes.Visible = false;
            }
        }

        public void AtivarModeloTipoOperacaoFechado()
        {
            txtInfoSiafemCE.Enabled = true;
            MontarViewEspecificaParaTipoMovimentacaoSaida();
            HabilitarBotoes();
            SetRequisicao();
            PopularSubTipoMovimento(Convert.ToInt32(rblTipoMovimento.SelectedValue));

            if (!this.ModoGravacaoOuEstorno)
                BloqueiaCamposInfoSiafemCE = true;

            OcultarOrgaoAlmox();



            if (int.Parse(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.SaidaPorDoacao)
            {
                TratamentoOpcoes();

                if (rblTipoOperacao.SelectedValue == "1")
                {
                    txtCPF_CNPJ.Enabled = true;
                    ddlOrgao_TransferenciaDoacao.Enabled = true;
                    ddlAmoxarifado_TransferenciaDoacao.Enabled = true;
                }
                else
                {
                    txtCPF_CNPJ.Enabled = false;
                    ddlOrgao_TransferenciaDoacao.Enabled = false;
                    ddlAmoxarifado_TransferenciaDoacao.Enabled = false;
                }
            }
            else
            {
                trCampo_CPF_CNPJ.Visible = false;
                trRadioOpcoes.Visible = false;
            }
        }

        public void OcultarOrgaoAlmox()
        {


            if ((rblTipoMovimento.SelectedValue == ((int)enumTipoMovimento.SaidaPorTransferencia).ToString() &&
                (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.NotaFornecimento).ToString()) ||
               (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Estorno).ToString())))
            {
                trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = false;
                ExibeCamposInfoTransferenciaDoacao = false;
            }
            else
            {
                if (rblTipoMovimento.SelectedValue == ((int)enumTipoMovimento.SaidaPorTransferencia).ToString())
                {
                    trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = true;
                    ExibeCamposInfoTransferenciaDoacao = true;
                    ddlOrgao_TransferenciaDoacao.Enabled = true;
                    ddlAmoxarifado_TransferenciaDoacao.Enabled = true;
                }
            }

            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada))
            { ExibeCamposInfoSiafemCEPadrao = false; }
            else
            { ExibeCamposInfoSiafemCEPadrao = true; }

        }

        protected void rblTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtInfoSiafemCE.Enabled = true;
            txtInfoSiafemCE.Text = string.Empty;
            SetRequisicao();
            VerificaMesFechadoSIAFEM();

            if (!((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.NotaFornecimento).ToString()) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada)))
            {
                if (!dadosAcessoSiafemPreenchidos)
                    efetuarAcessoSIAFEM();
            }

            if (int.Parse(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.SaidaPorDoacao)
            {
                trRadioOpcoes.Visible = true;
                rblOpcoes.ClearSelection();
                rblOpcoes.SelectedValue = "orgao_almoxarifado";
            }
            else
            {
                trRadioOpcoes.Visible = false;
                trCampo_CPF_CNPJ.Visible = false;
            }

            if (this.movimento.IsNotNull() && String.IsNullOrWhiteSpace(this.movimento.AnoMesReferencia))
                this.movimento.AnoMesReferencia = this.AnoMesReferencia;

            //MontarPainelRequisicoes();
            MontarViewEspecificaParaTipoMovimentacaoSaida();
            PopularSubTipoMovimento(Convert.ToInt32(rblTipoMovimento.SelectedValue));
            HabilitarBotoes();
            LimparConteudo();


            if (!this.ModoGravacaoOuEstorno)
                BloqueiaCamposInfoSiafemCE = true;



        }


        public void LimparConteudo()
        {
           // txtInfoSiafemCE.Text = string.Empty;
            txtRequisicao.Text = string.Empty;
            txtValorTotal.Text = string.Empty;
            txtDataMovimento.Text = string.Empty;
            Session["txtDataMovimento"] = null;
            txtObservacoes.Text = string.Empty;
            txtDescricaoAvulsa.Text = string.Empty;
            ddlSubTipoMovimento.SelectedValue = "0";
        }



        protected void ddlOrgao_TransferenciaDoacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var orgaoId = 0;
            Int32.TryParse(ddlOrgao_TransferenciaDoacao.SelectedValue, out orgaoId);

            if (orgaoId > 0)
            {
                OrgaoId = orgaoId;
                AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();
                List<AlmoxarifadoEntity> almox = new List<AlmoxarifadoEntity>();

                InicializarDropDownList(ddlAmoxarifado_TransferenciaDoacao);
                //Implementado para permitir fazer saída apenas para almoxarifado com o mesmo mês ref 
                string mesRef = AnoMesReferencia;
                ddlAmoxarifado_TransferenciaDoacao.AppendDataBoundItems = true;                
                almox = objPresenter.ListarAlmoxarifadoPorOrgaoMesRef(OrgaoId, AnoMesReferencia).Where(a => a.Id != this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id).ToList();
                ddlAmoxarifado_TransferenciaDoacao.DataSource = almox.OrderBy(a => a.Descricao);
                ddlAmoxarifado_TransferenciaDoacao.DataBind();
                ddlAmoxarifado_TransferenciaDoacao.Enabled = true;
                if (almox.Count==0 )
                {
                    this.ListaErros = new List<string>() { string.Format($"Orgão selecionado não possui nenhum almoxarifado no mesmo mês ref que você.") };
                }             
            }
        }

        protected void rdl_Opcoes_SelectedIndexChanged(object sender, EventArgs e)
        {
            TratamentoOpcoes();
        }

        protected void ddlAmoxarifado_TransferenciaDoacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var almoxID = 0;
            Int32.TryParse(ddlAmoxarifado_TransferenciaDoacao.SelectedValue, out almoxID);

            if (almoxID > 0)
            {
                AlmoxarifadoIdOrigem = almoxID;
                AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();

                InicializarDropDownList(ddlAmoxarifado_TransferenciaDoacao);
                ddlAmoxarifado_TransferenciaDoacao.DataSource = objPresenter.ListarAlmoxarifadosPorOrgao(OrgaoId);
                ddlAmoxarifado_TransferenciaDoacao.DataBind();
            }
        }

        protected void imgLupaPesquisaDocumento_Click(object sender, ImageClickEventArgs e)
        {
            if (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString())
            {
                if (rptSubItem.Items.Count>0)
                {
                   txtRequisicao.Text = string.Empty;                                   
                }

                if (string.IsNullOrEmpty(txtDataMovimento.Text))
                {
                    txtRequisicao.Text = string.Empty;
                    rptSubItem.Visible = false;
                    ExibirMensagem("Campo de preenchimento obrigatório: Data de Movimento");
                    return;
                }
                DateTime dataDigitada = new DateTime();
                dataDigitada = Convert.ToDateTime(txtDataMovimento.Text);
                if (dataDigitada>DateTime.Now)
                {
                    rptSubItem.Visible = false;
                    ExibirMensagem("Data de Movimento não pode ser maior do que hoje");
                    return;
                }
                if ((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString()))
                {
                    string data = txtDataMovimento.Text.ToString().Replace("/", "");
                    string ano = data.Substring(4, 4);
                    string mes = data.Substring(2, 2);
                    string mesRef = AnoMesReferencia;
                    if (mesRef != ano + mes)
                    {
                        ExibirMensagem("Data do movimento deve estar no mês de referência.");                       
                        return;
                    }
                }

                if (!VerificaSeAlmoxarifadoPossuiFechamentos())
                    return;

            }
            rptSubItem.Visible = true;

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "OpenModal", "javascript:OpenModal();", true);

            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);

            presenter.Cancelar();    
            RemoveSession(sessaoMov);
            InstanciaObjetoMovimento();
            movimento.MovimentoItem = null;
            movimento.TipoMovimento.Id = TipoMovimento;
            movimento.Divisao.Id = helperTratamento.TryParseInt32(ddlDivisao.SelectedValue);
            movimento.DataDigitada =(txtDataMovimento.Text).ToString();            
            bool isEstorno = (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Estorno).ToString());
            PesquisaRequisicao1.PopularDadosGridRequisicao(movimento, 0, isEstorno);

            SetSession<MovimentoEntity>(movimento, sessaoMov);         
            ValorDocumento = movimento.ValorDocumento;// = movimento.MovimentoItem.Sum(movItem => movItem.ValorMov ?? 0.00m).truncarDuasCasas();
            //Implemetada para não permitir editar o almoxarifado
            if (txtRequisicao.Text !="")
            {
                ddlDivisao.Enabled = false;
            }
           else
            {
                ddlDivisao.Enabled = true;
            }
        }

        private bool VerificaSeAlmoxarifadoPossuiFechamentos()
        {
            bool existemFechamentosParaEsteAlmoxarifado = false;
            SaidaMaterialPresenter saidaMaterialPresenter = null;


            saidaMaterialPresenter = new SaidaMaterialPresenter(this);
            existemFechamentosParaEsteAlmoxarifado = saidaMaterialPresenter.VerificaSeAlmoxarifadoPossuiFechamentos();
            return existemFechamentosParaEsteAlmoxarifado;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidaValorItem())
            {
                return;
            }  
            AdicionarEditarItem();
            if (GetSession<MovimentoEntity>(sessaoMov) != null)
                rptSubItem.Visible = true;
            
        }
        public bool ValidaValorItem()
        {
            double? QtdValor = string.IsNullOrEmpty(txtQtdeMov.Text) ? 0.000 : Convert.ToDouble(txtQtdeMov.Text);
            string ptr = string.Empty;
            bool validado = true;
            string mensagem = string.Empty;
            mensagem = QtdValor == 0.000 ? "Qtd. Fornecer// " : "";
            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente))
            {
                 ptr = ddlPTResAcao.SelectedValue=="0"? "PTRes / PTRes Ação":"";                
                 mensagem = ptr + mensagem;
            }            
            if (!string.IsNullOrEmpty(mensagem))
            {
                this.ListaErros = new List<string>() {$"Preencha o(s) campo(s): { mensagem}" };
                validado = false;
            }
            return validado;
        }

        public void AdicionarEditarItem()
        {
            if (btnAdd.Text != "Editar")
            {
                ViewState["botao"] = "btnAdd";
                imgLupaSubItem.Visible = true;
                txtSubItem.Enabled = true;
            }
            else
            {
                imgLupaSubItem.Visible = false;
                txtSubItem.Enabled = false;
            }

            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);

            if (btnAdd.Text != "Editar")
            {
                if (movimento != null)
                {
                    if (movimento.MovimentoItem != null)
                    {
                        foreach (var item in movimento.MovimentoItem)
                        {
                            if (item.SubItemCodigo.Value == this.SubItemMaterialCodigo)
                            {
                                this.ListaErros = new List<string>() { "SubItem: " + this.SubItemMaterialCodigo + ", já foi adicionado." };
                                CancelarItem();
                                return;
                            }
                            else
                            {
                                item.ValorMov = item.PrecoUnit * item.QtdeMov;
                            }
                        }
                    }
                }
            }
            
            if (rptLote.Items.Count > 0)
            {
                for (int i = 0; i < rptLote.Items.Count; i++)
                {
                    TextBox txtQtdeLote = new TextBox();
                    if (btnAdd.Text == "Editar" || rptLote.Items.Count == 1)
                        if (((TipoMovimento == (int)enumTipoMovimento.RequisicaoAprovada) ||
                            (TipoMovimento == (int)enumTipoMovimento.RequisicaoCancelada) ||
                            (TipoMovimento == (int)enumTipoMovimento.RequisicaoFinalizada) ||
                           (TipoMovimento == (int)enumTipoMovimento.RequisicaoPendente)) &&
                               rptLote.Items.Count > 1)
                        {
                            txtQtdeLote.Text = ((TextBox)rptLote.Items[i].FindControl("txtQtdeLote")).Text;
                            ViewState["botao"] = "btnAdd";
                            imgLupaSubItem.Visible = true;
                            txtSubItem.Enabled = true;
                        }
                        else
                        {
                            txtQtdeLote.Text = txtQtdeMov.Text;
                            imgLupaSubItem.Visible = false;
                            txtSubItem.Enabled = false;
                        }
                    else
                        txtQtdeLote.Text = ((TextBox)rptLote.Items[i].FindControl("txtQtdeLote")).Text;

                    if (!string.IsNullOrEmpty(txtQtdeLote.Text))
                    {
                        if (Convert.ToInt32(txtQtdeLote.Text.Replace(",", "")) > 0)
                            AdicionarItem(i);
                    }
                }
            }
            else
            {
                AdicionarItem(0);
            }

            ValorDocumento = movimento.MovimentoItem.Sum(movItem => movItem.ValorMov ?? 0.00m).truncarDuasCasas();
            //presenter.AdicionadoSucesso();
            var tipoMaterialSubitemDivergente = PageBase.GetSession<bool>(ChaveSessao_CampoTipoMaterialDivergente_LancamentoSIAFEM);
            if (!tipoMaterialSubitemDivergente)
                presenter.AdicionadoSucesso();

            ViewState["botao"] = "";
            LimparLote(null, null);

            PageBase.SetSession<bool>(false, ChaveSessao_CampoTipoMaterialDivergente_LancamentoSIAFEM);
        }

        public void PopularSubTipoMovimento(int IdTipoMovimento)
        {
            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter();
            List<SubTipoMovimentoEntity> subTipo = new List<SubTipoMovimentoEntity>();
            subTipo = presenter.ExibirTipoMovimentoEntrada(IdTipoMovimento).FirstOrDefault().SubTipoMovimento.ToList();
            ddlSubTipoMovimento.Items.Clear();
            ddlSubTipoMovimento.DataSource = subTipo;

            if (subTipo.Count > 0)
            {
                idSubTipo.Visible = true;
                ddlSubTipoMovimento.Enabled = true;
                ViewState["SubTipo"] = subTipo;
            }
            else
                idSubTipo.Visible = false;

            ddlSubTipoMovimento.Items.Insert(0, new ListItem("Selecione", "0"));
            ddlSubTipoMovimento.AppendDataBoundItems = true;
            ddlSubTipoMovimento.DataValueField = "Id";
            ddlSubTipoMovimento.DataTextField = "Descricao";
            ddlSubTipoMovimento.DataBind();



        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            CancelarItem();

            if (movimento.MovimentoItem != null)
                ValorDocumento = movimento.MovimentoItem.Sum(movItem => movItem.ValorMov ?? 0.00m).truncarDuasCasas();
        }

        public void CancelarItem()
        {
            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);
            presenter.Cancelar();
            LimparLote(null, null);
        }

        public void LimparLote(object LoteItens, object LoteItem)
        {
            rptLote.DataSource = LoteItens;
            ddlLote.DataSource = LoteItem;
            rptLote.DataBind();
            ddlLote.DataBind();
            rptLote.Visible = LoteItens == null ? false : true;
            ddlLote.Visible = LoteItens != null ? false : true;
        }

        public void TratamentoOpcoes()
        {
            if (rblOpcoes.SelectedValue == "cpf_cnpj")
            {
                trCampo_CPF_CNPJ.Visible = true;
                trCampoTransferenciaDoacao_ComboOrgao.Visible = false;
                trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = false;
            }
            else if (rblOpcoes.SelectedValue == "orgao_almoxarifado")
            {
                trCampo_CPF_CNPJ.Visible = false;
                trCampoTransferenciaDoacao_ComboOrgao.Visible = true;
                trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = true;
            }
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            bool AcessoSIAFEM = true;
            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada))
            {
                AcessoSIAFEM = false;
            }
            if ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) && (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString()))
            {
                List<String> erro = new List<string>();
                if (!PermitirSaidaRequisicao(movimento))
                {
                    ExibirMensagem("Inconsistências encontradas");
                    this.ListaErros = new List<string>() { string.Format($"Não é possível reclassificar o bem patrimonial, pois a UGE de destino não está no mesmo mês ref. contábil que o estoque.") };
                    return;
                }
            }
            switch (TipoMovimento)
            {

                case ((int)tipoMovimentacao.RequisicaoPendente):
                    if ((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString()))
                    {
                        string data = txtDataMovimento.Text.ToString().Replace("/", "");
                        string ano = data.Substring(4, 4);
                        string mes = data.Substring(2, 2);
                        string mesRef = AnoMesReferencia;
                        if (mesRef != ano + mes)
                        {
                            this.ListaErros = new List<string>() { string.Format($"Data do movimento deve estar no mês de referência.") };
                            return;
                        }
                    }
                    break;
                case ((int)tipoMovimentacao.SaidaPorTransferenciaParaAlmoxNaoImplantado):

                    int _codigoUge;


                    if (int.TryParse(txtDescricaoAvulsa.Text, out _codigoUge) == false)
                    {
                        this.ListaErros = new List<string>() { "UGE Destino favor preencher somente com números (6 dígitos)" };
                        return;
                    }

                    if (txtDescricaoAvulsa.Text.Length > 6)
                    {
                        this.ListaErros = new List<string>() { "UGE Destino tamanho máximo permitido 6 dígitos" };
                        return;
                    }
                    break;

                case ((int)tipoMovimentacao.SaidaPorDoacao):
                    if (trtxtAvulso.Visible)
                    {
                        if (!string.IsNullOrWhiteSpace(txtDescricaoAvulsa.Text))
                        {
                            bool validar = false;
                            if (txtDescricaoAvulsa.Text.Length < 12)
                                validar = TratamentoDados.ValidarCPF(txtDescricaoAvulsa.Text);
                            else
                                validar = TratamentoDados.ValidarCNPJ(txtDescricaoAvulsa.Text);

                            if (!validar)
                            {
                                ExibirMensagem("Inconsistências encontradas. CPF/CNPJ inválido");
                                return;
                            }

                        }
                        else
                        {
                            ExibirMensagem("Inconsistências encontradas. Favor Preencher o CPF/CNPJ que não esta implantado.");
                            return;
                        }
                    }
                    if (trCampo_CPF_CNPJ.Visible)
                    {
                        if (!string.IsNullOrWhiteSpace(txtCPF_CNPJ.Text))
                        {
                            bool validar = false;
                            if (txtCPF_CNPJ.Text.Length < 12)
                                validar = TratamentoDados.ValidarCPF(txtCPF_CNPJ.Text);
                            else
                                validar = TratamentoDados.ValidarCNPJ(txtCPF_CNPJ.Text);

                            if (!validar)
                            {
                                ExibirMensagem("Inconsistências encontradas. CPF/CNPJ inválido");
                                return;
                            }

                        }
                        else
                        {
                            ExibirMensagem("Inconsistências encontradas. Favor Preencher o CPF/CNPJ que não esta implantado.");
                            return;
                        }
                    }
                    break;
                default:
                    break;


            }

            if (trCamposInfoSiafemCE.Visible)
            {

                if (string.IsNullOrEmpty(txtInfoSiafemCE.Text))
                {
                    ExibirMensagem("Preencher Inscrição (CE)");
                    return;
                }
                else
                {
                    txtInfoSiafemCE.Text = txtInfoSiafemCE.Text.Replace(" ", "");
                    if (txtInfoSiafemCE.Text == "999")
                        txtInfoSiafemCE.Text = "CE999";
                }
            }

            if (!((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.NotaFornecimento).ToString()) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada)))
            {
                if (!dadosAcessoSiafemPreenchidos)
                    efetuarAcessoSIAFEM();
            }

            //if (TipoMovimento == tipoMovimentacao.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode())
            //{
            //    int _codigoUge;

            //    //if (string.IsNullOrEmpty(txtDescricaoAvulsa.Text))
            //    //{
            //    //    this.ListaErros = new List<string>() { "UGE Destino por favor, preencher." };
            //    //    return;
            //    //}

            //    if (int.TryParse(txtDescricaoAvulsa.Text, out _codigoUge) == false)
            //    {
            //        this.ListaErros = new List<string>() { "UGE Destino favor preencher somente com números (6 dígitos)" };
            //        return;
            //    }

            //    if (txtDescricaoAvulsa.Text.Length > 6)
            //    {
            //        this.ListaErros = new List<string>() { "UGE Destino tamanho máximo permitido 6 dígitos" };
            //        return;
            //    }

            //    //movimento.GeradorDescricao = txtDescricaoAvulsa.Text;
            //}

            //if ((TipoMovimento == (int)tipoMovimentacao.SaidaPorTransferencia) || (TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao))
            //{
            //    if (ddlAmoxarifado_TransferenciaDoacao.SelectedValue == "0")
            //    {
            //        this.ListaErros = new List<string>() { "Selecionar o Almoxarifado de destino." };
            //        return;
            //    }

            //    //if ((TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao))
            //    //{
            //    //    if ((ddlAmoxarifado_TransferenciaDoacao.SelectedItem.Text == "Sem almoxarifado"))
            //    //    {
            //    //        if (!string.IsNullOrEmpty(txtDescricaoAvulsa.Text))
            //    //        {
            //    //            movimento.GeradorDescricao = txtDescricaoAvulsa.Text;
            //    //        }
            //    //        else
            //    //        {
            //    //            this.ListaErros = new List<string>() { "UGE/CPF/CNPJ por favor, preencher." };
            //    //            return;
            //    //        }
            //    //    }
            //    //}
            //}


            if (trtxtAvulso.Visible)
            {
                if (!string.IsNullOrEmpty(txtDescricaoAvulsa.Text))
                {
                    movimento.GeradorDescricao = txtDescricaoAvulsa.Text;
                }
                else
                {
                    this.ListaErros = new List<string>() { lblTextoAvulso.Text + " por favor, preencher." };
                    return;
                }

            }

            if (trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible)
            {
                if (ddlAmoxarifado_TransferenciaDoacao.SelectedValue == "0")
                {
                    this.ListaErros = new List<string>() { "Selecionar o Almoxarifado de destino." };
                    return;
                }
            }

            if (idSubTipo.Visible)
            {
                if (ddlSubTipoMovimento.SelectedValue == "0")
                {
                    this.ListaErros = new List<string>() { "Selecionar o SubTipo." };
                    return;
                }

                var tipoMaterialMovimentacao = movimento.ObterTipoMaterial();
                if (!(hdfNaturaSelecionada.Value.Contains(tipoMaterialMovimentacao.ToString())))
                {
                    this.ListaErros = new List<string>() { String.Format("Selecione o SubTipo de {0}", EnumUtils.GetEnumDescription<TipoMaterialParaLancamento>(tipoMaterialMovimentacao)) };

                    return;
                }

            }

            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);
            foreach (var item in movimento.MovimentoItem)
            {
               
                if (!item.ItemValidado)
                {
                    item.PrecoUnit = presenter.getPrecoUnitSubItem(item.SubItemMaterial.Id, new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id, item.UGE.Id, item.IdentificacaoLote, item.FabricanteLote, item.DataVencimentoLote).Value.Truncar(base.numCasasDecimaisValorUnitario, true);
                    item.ValorMov = (item.QtdeMov ?? 0.0m).Truncar(base.numCasasDecimaisValorUnitario, true) * (item.PrecoUnit ?? 0.0m).Truncar(base.numCasasDecimaisValorUnitario, true);

                    if (!ValidarMovimentoItem(item))
                    {
                        return;
                    }
                        
                }

                if (item.QtdeMov == 0)
                {
                    this.ListaErros = new List<string>() { "SubItem: " + item.SubItemCodigoFormatado + " está zerado." };
                    {
                        return;
                    }
                }
                foreach (RepeaterItem itemR in rptSubItem.Items)
                {
                    decimal valorSaldoQtdDt = 0.0m;
                    Label idMovItem = (Label)itemR.FindControl("movimentoItemId");
                    HtmlTableCell celSaldoQtdeDt = (HtmlTableCell)itemR.FindControl("celSaldoQtdeDt");
                    valorSaldoQtdDt = (helperTratamento.TryParseDecimal(celSaldoQtdeDt.InnerText.Trim(), true) ?? 0.0m);

                    if (Convert.ToInt32(idMovItem.Text) == item.Id)
                    {
                        item.SaldoQtde = valorSaldoQtdDt;
                        break;
                    }
                }
            }


            #region Verifica Evento Pagamento (CE)
            movimento.InscricaoCE = this.InscricaoCE;
            #endregion Verifica Evento Pagamento (CE)

            if (ddlSubTipoMovimento.SelectedValue != "0" && !string.IsNullOrEmpty(ddlSubTipoMovimento.SelectedValue))
                movimento.SubTipoMovimentoId = Convert.ToInt32(ddlSubTipoMovimento.SelectedValue);

            if (txtDescricaoAvulsa.Visible)
                movimento.UgeCPFCnpjDestino = txtDescricaoAvulsa.Text;

            if (trCampo_CPF_CNPJ.Visible)
            {
                movimento.GeradorDescricao = txtCPF_CNPJ.Text;
            }

            
            //presenter.GravarMovimentoSaida(movimento);
            bool retorno = presenter.GravarMovimentoSaida(this.loginSiafem, this.senhaSiafem, movimento, AcessoSIAFEM);
            BloqueiaBotaoNovo = true;
            ConsultarLimparDataMovimento();
            //Implementado teste 08/11/2019
            ddlDivisao.Enabled = true;
            if (retorno)
                MontarViewEspecificaParaTipoMovimentacaoSaida();

            //Recarrega a página
            if (movimento.TipoMovimento.Id == (int)tipoMovimentacao.SaidaPorDoacao)
                Response.Redirect(Request.RawUrl);
        }
        
        protected void btnEstornar_Click(object sender, EventArgs e)
        {
            if (trCamposInfoSiafemCE.Visible)
            {
                if (string.IsNullOrEmpty(txtInfoSiafemCE.Text))
                {
                    ExibirMensagem("Preencher Inscrição (CE)");
                    return;
                }
                else
                {
                    txtInfoSiafemCE.Text = txtInfoSiafemCE.Text.Replace(" ", "");
                    if (txtInfoSiafemCE.Text == "999")
                        txtInfoSiafemCE.Text = "CE999";
                }
            }

            if (!((rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.NotaFornecimento).ToString()) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente) ||
                 (Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoAprovada)))
            {
                if (!dadosAcessoSiafemPreenchidos)
                    efetuarAcessoSIAFEM();
            }

            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);
            //presenter.Estornar(movimento);
            Tuple<string, List<string>> retorno = presenter.Estornar(this.loginSiafem, this.senhaSiafem, movimento);

            if (retorno.Item2 == null)
            {
                RemoveSession(sessaoMov);
                rptSubItem.DataSourceID = null;
                rptSubItem.DataBind();
                LimparConteudo();
            }
            else
                this.ListaErros = retorno.Item2;

            ExibirMensagem(retorno.Item1);
            return;


        }

        protected void ddlDivisao_DataBound(object sender, EventArgs e)
        {
            ddlDivisao.Items.Insert(0, new ListItem("TODAS DIVISÕES DO ALMOXARIFADO", "00"));

            if (ddlDivisao.Items.Count > 0)
            {
                if (ddlDivisao.SelectedIndex == -1)
                    ddlDivisao.SelectedIndex = 0;

                //imgLupaRequisicao.Visible = true;
                ExibeBotaoPesquisaDocumento = true;
            }
            else
            {
                //imgLupaRequisicao.Visible = false;
                ExibeBotaoPesquisaDocumento = false;
            }
        }

        protected void rblTipoMovimento_DataBound(object sender, EventArgs e)
        {
            if (rblTipoMovimento.Items.Count > 0)
                if (rblTipoMovimento.SelectedIndex == -1)
                    rblTipoMovimento.SelectedIndex = 0;

            //MontarPainelRequisicoes();
            //PrepararViewEspecificaParaTipoMovimentacaoSaida();
        }

        protected void rptSubItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            //MovimentoItemEntity _movItem = (e.Item.DataItem as MovimentoItemEntity);
            //var ugeID = (_movItem.IsNotNull() && _movItem.UGE.IsNotNull() && _movItem.UGE.Id.Value != 0) ? _movItem.UGE.Id.Value : 0;

            //if (ugeID != 0 && ddlPTResItemMaterial.Items.Cast<ListItem>().Count() <= 1)
            //{
            //    var _dadosUGEMovimento = (new UGEPresenter().CarregarRegistroUGE(ugeID));
            //    var _ugeConsultaPTRes = (_dadosUGEMovimento.IsNotNull() ? _dadosUGEMovimento.Codigo : null);
            //    this.ugeCodigo = (int)_ugeConsultaPTRes;

            //   // PreencherListaPTRes();
            //    //PreencherListaPTResAcao();
            //}

            // 
            //if (Session["DivisaoId"] != null && rblTipoMovimento.SelectedValue == Convert.ToString((int)enumTipoMovimento.RequisicaoPendente)
            //    && Convert.ToInt32(rblTipoOperacao.SelectedValue) != (int)enumTipoRequisicao.Nova)
            // O teste validando a operação como diferente de nova, foi excluída pois não estava atualizando o valor da divisão no DropDown
            // Como há 3 valores possíveis para esta Operação, a alteração que causa menor impacto é suprir o tratamento para Nova. Uma vez
            // que era a única Operação excluída.
            if (Session["DivisaoId"] != null && rblTipoMovimento.SelectedValue == Convert.ToString((int)enumTipoMovimento.RequisicaoPendente))
                DivisaoId = int.Parse(Session["DivisaoId"].ToString());

            if (Session["txtDataMovimento"] != null && Convert.ToInt32(rblTipoOperacao.SelectedValue) != (int)enumTipoRequisicao.Nova)
                txtDataMovimento.Text = Session["txtDataMovimento"].ToString();

            BloqueiaBotaoNovo = true;
            BloqueiaGravar = true;

            ColorirCelulaQtd(e.Item);
            ExibirCamposPTRes(e.Item);

            switch (Convert.ToInt32(rblTipoOperacao.SelectedValue))
            {
                case ((int)enumTipoRequisicao.Nova): ExibirColunaSaldo(e.Item, true); break;
                case ((int)enumTipoRequisicao.Estorno):
                    ExibirColunaSaldo(e.Item, false);
                    if (e.Item.ItemIndex == 0)
                        ListarMovimentoItens(0, 1, movimento.NumeroDocumento, Convert.ToString(movimento.DataMovimento));
                    PreencherSubTipo();

                    break;
                case ((int)enumTipoRequisicao.NotaFornecimento):

                    ExibirColunaSaldo(e.Item, false);
                    if (e.Item.ItemIndex == 0)
                        ListarMovimentoItens(0, 1, movimento.NumeroDocumento, Convert.ToString(movimento.DataMovimento));
                    PreencherSubTipo();
                    break;
                default: ExibirColunaSaldo(e.Item, true); break;
            }

            if (rblTipoMovimento.SelectedValue == Convert.ToString((int)enumTipoMovimento.RequisicaoPendente))
                ExibirColunaRequisito(e.Item, true);
            else
                ExibirColunaRequisito(e.Item, false);

            if (e.Item.ItemType == ListItemType.Footer && Session["numeroDocumento"] != null)
            {
                PreencherListaPTRes();
                PreencherListaPTResAcao();
            }
            if (txtRequisicao.Text!="")
            {
                ddlDivisao.Enabled = false;
            }
            else
            { ddlDivisao.Enabled = true; }

        }        
        private void PreencherSubTipo()
        {
            if (movimento != null)
            {
                if (movimento.SubTipoMovimentoId != null)
                {
                    if (ddlSubTipoMovimento.Items.Count > 1)
                    {
                        idSubTipo.Visible = true;
                        ddlSubTipoMovimento.SelectedValue = Convert.ToString(movimento.SubTipoMovimentoId);
                        AtivarDetalhe();
                        txtDescricaoAvulsa.Enabled = false;
                        ddlSubTipoMovimento.Enabled = false;

                        if (movimento.MovimAlmoxOrigemDestino != null)
                        {

                            AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();
                            List<AlmoxarifadoEntity> listAlm = new List<AlmoxarifadoEntity>();
                            listAlm.Add(objPresenter.ObterAlmoxarifado((int)movimento.MovimAlmoxOrigemDestino.Id));
                            ddlAmoxarifado_TransferenciaDoacao.Items.Clear();
                            ddlAmoxarifado_TransferenciaDoacao.DataSource = listAlm.ToList();
                            ddlAmoxarifado_TransferenciaDoacao.DataBind();
                        }

                        txtDescricaoAvulsa.Text = movimento.GeradorDescricao;
                    }
                }
                else
                {
                    idSubTipo.Visible = false;
                    if (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Estorno).ToString() && ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado)))
                    {                      
                        trtxtAvulso.Visible = true;
                        if (movimento.GeradorDescricao != null)
                        {
                            var CodigoUge= movimento.GeradorDescricao.Substring(movimento.GeradorDescricao.Length - 6);
                            txtDescricaoAvulsa.Text = CodigoUge;
                        }                      
                    }
                    else
                    { 
                    trtxtAvulso.Visible = false;
                    }
                }
            }

        }

        protected void ddlUGE_DataBound(object sender, EventArgs e)
        {
            var almoxarifadoId = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            PopularListaLote(almoxarifadoId.Value);
            BloqueiaBotaoAdicionar();
        }

        protected void ddlLote_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSaldo.Text = RecuperaSaldo(false);
            BloqueiaBotaoAdicionar();
        }

        protected void ddlLote_DataBound(object sender, EventArgs e)
        {
            txtSaldo.Text = RecuperaSaldo(false);
            BloqueiaBotaoAdicionar();
        }

        protected void rblTipoOperacao_DataBound(object sender, EventArgs e)
        {
            //MontarPainelRequisicoes();
            //MontarViewEspecificaParaTipoMovimentacaoSaida();
        }




        protected void rptSubItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            btnCancelar_Click(source, e);

            int subItemId = 0;
            PTResEntity _ptres = null;

            if (e.CommandName == "Select")
            {
                subItemId = Convert.ToInt32(e.CommandArgument);
                SubItemMaterialId = Convert.ToInt32(((HiddenField)e.Item.FindControl("hdfidSubItem") as HiddenField).Value);

                var ugeID = (movimento.UGE.Id.HasValue && (this.UgeId != movimento.UGE.Id.Value)) ? movimento.UGE.Id.Value : this.UgeId;
                var _dadosUGEMovimento = (new UGEPresenter().CarregarRegistroUGE(ugeID));
                var _ugeConsultaPTRes = (_dadosUGEMovimento.IsNotNull() ? _dadosUGEMovimento.Codigo : null);
                this.ugeCodigo = (int)_ugeConsultaPTRes;

                ddlUGE.DataSourceID = "odsUGE";


                if (_ugeConsultaPTRes != 0 && ddlPTResItemMaterial.Items.Cast<ListItem>().Count() <= 1)
                    PreencherListaPTRes();

                if (_ugeConsultaPTRes != 0 && ddlPTResAcao.Items.Cast<ListItem>().Count() <= 1)
                    PreencherListaPTResAcao();

                if (movimento.MovimentoItem != null)
                {
                    var movimentoEdit = (MovimentoItemEntity)movimento.MovimentoItem.Where(a => a.Id == subItemId).ToList().FirstOrDefault();
                    var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTRes);
                    var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);

                    ViewState["LoteTemp"] = !string.IsNullOrWhiteSpace(((HtmlTableCell)e.Item.FindControl("celQtdeMov")).InnerText) ? movimentoEdit.IdLote : 0;
                    ViewState["botao"] = "btnEditar";
                    imgLupaSubItem.Visible = false;
                    txtSubItem.Enabled = false;
                    this.MovimentoItemId = helperTratamento.TryParseInt32(((Label)e.Item.FindControl("movimentoItemId")).Text);
                    this.SubItemMaterialId = movimentoEdit.SubItemMaterial.Id;
                    this.SubItemMaterialCodigo = helperTratamento.TryParseLong(movimentoEdit.SubItemMaterial.CodigoFormatado);
                    this.SubItemMaterialDescricao = movimentoEdit.SubItemDescricao;
                    this.Unidade = movimentoEdit.SubItemMaterial.UnidadeFornecimento.Descricao;
                    this.SaldoValorItem = (movimentoEdit.SaldoQtde.HasValue && movimentoEdit.SubItemMaterial.SaldoSubItems.HasElements() ? movimentoEdit.SubItemMaterial.SaldoSubItems[0].SaldoQtde : 0);
                    movimentoEdit.SaldoQtde = this.SaldoQtdeItem;
                    movimentoEdit.PrecoUnit = (movimentoEdit.PrecoUnit.HasValue && movimentoEdit.SubItemMaterial.SaldoSubItems.HasElements() ? movimentoEdit.SubItemMaterial.SaldoSubItems[0].PrecoUnit : 0);

                    var _labelPTRes = ((Label)e.Item.FindControl("lblPTRes") as Label);
                    var _labelPTResAcao = ((Label)e.Item.FindControl("lblPTResAcao") as Label);
                    var _ptresCodigo = -1;
                    var _ptresAcao = -1;
                    if (_labelPTRes.IsNotNull() && !String.IsNullOrWhiteSpace(_labelPTRes.Text) && Int32.TryParse(_labelPTRes.Text, out _ptresCodigo) ||
                        _labelPTResAcao.IsNotNull() && !String.IsNullOrWhiteSpace(_labelPTResAcao.Text) && Int32.TryParse(_labelPTResAcao.Text, out _ptresAcao))
                    {
                        _ptres = listaPTRes.Where(ptres => ptres.Codigo == _ptresCodigo).FirstOrDefault();
                        if (_ptres.IsNull())
                            _ptres = listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull() && ptres.ProgramaTrabalho.ProjetoAtividade == _ptresAcao.ToString()).FirstOrDefault();

                        this.PTResId = (_ptres.IsNotNull() ? _ptres.Id : -1);
                        this.PTResCodigo = (_ptres.IsNotNull() ? _ptres.Codigo : -1);
                        this.PTResAcao = (_ptres.IsNotNull() ? _ptres.ProgramaTrabalho.ProjetoAtividade : "");
                        this.PTResDescricao = (_ptres.IsNotNull() ? _ptres.Descricao : "");

                        if (_labelPTResAcao.IsNotNull())
                            //_labelPTResAcao.Text = this.PTResAcao;
                            this.PTResAcao = _labelPTResAcao.Text;

                        movimentoEdit.PTRes = _ptres;
                    }
                    else
                    {
                        this.PTResId = -1;
                        this.PTResCodigo = -1;
                        this.PTResDescricao = string.Empty;
                        this.PTResAcao = string.Empty;
                        this.PTAssociadoPTRes = string.Empty;
                    }

                    if (!movimentoEdit.QtdeMov.HasValue) movimentoEdit.QtdeMov = _valorZero;
                    this.QtdFornecida = movimentoEdit.QtdeMov.Value.ToString(base.fmtFracionarioMaterialQtde);
                    this.QtdeLiqItem = movimentoEdit.QtdeLiq.Value;

                    MudarTextoBotaoEditar(true);
                }
            }

            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);
            presenter.RegistroSelecionado();
        }

        public void PreencherListaPTResAcao()
        {
            this.ddlPTResAcao.InserirElementoSelecione(true);

            if (this.ugeCodigo != 0)
            {
                if (Session["numeroDocumento"] != null)
                {
                    MovimentoPresenter presenter = new MovimentoPresenter();

                    Infrastructure.TB_UGE uge = presenter.RetornarMovimentoAlmoxUge(Session["numeroDocumento"].ToString());
                    if (uge != null)
                        this.ugeCodigo = uge.TB_UGE_CODIGO;
                    else
                        this.ugeCodigo = -1;
                }
                //var ugeCodigo = Int32.Parse(this.ddlUGE.SelectedItem.Text.BreakLine('-', 0).Trim());
                if (this.ugeCodigo != -1)
                {
                    PTResPresenter objPresenter = new PTResPresenter();
                    var lstPTResAcao = objPresenter.CarregarListaPTResAcao(this.ugeCodigo);
                    PageBase.SetSession<IList<PTResEntity>>(lstPTResAcao, cstListagemPTResAcao);

                    if (lstPTResAcao.IsNotNullAndNotEmpty())
                    {
                        //lstPTResAcao.DistinctBy(p => p.Codigo).ToList()
                        //        .ForEach(_ptRes => this.ddlPTResAcao.Items.Add(new ListItem(_ptRes.ProgramaTrabalho.ProjetoAtividade, _ptRes.Codigo.ToString())));
                        lstPTResAcao.DistinctBy(p => new { p.Codigo, p.ProgramaTrabalho.ProjetoAtividade }).ToList()
                                    .ForEach(_ptRes => this.ddlPTResAcao.Items.Add(new ListItem(_ptRes.ProgramaTrabalho.ProjetoAtividade, _ptRes.Codigo.ToString())));
                    }
                }
            }
        }

        public void PreencherListaPTRes()
        {
            this.ddlPTResItemMaterial.InserirElementoSelecione(true);

            //Retornar a UGE selecionada no combo a cima.
            if (this.ugeCodigo != 0)
            {
                if (Session["numeroDocumento"] != null)
                {
                    MovimentoPresenter presenter = new MovimentoPresenter();

                    Infrastructure.TB_UGE uge = presenter.RetornarMovimentoAlmoxUge(Session["numeroDocumento"].ToString());
                    if (uge != null)
                        this.ugeCodigo = uge.TB_UGE_CODIGO;
                    else
                        this.ugeCodigo = -1;
                }
                //var ugeCodigo = Int32.Parse(this.ddlUGE.SelectedItem.Text.BreakLine('-', 0).Trim());
                if (this.ugeCodigo != -1)
                {
                    PTResPresenter objPresenter = new PTResPresenter();
                    var lstPTRes = objPresenter.CarregarListaPTRes(this.ugeCodigo);
                    PageBase.SetSession<IList<PTResEntity>>(lstPTRes, cstListagemPTRes);

                    if (lstPTRes.IsNotNullAndNotEmpty())
                    {
                        lstPTRes.DistinctBy(p => p.Codigo).ToList()
                                .OrderBy(f=>f.Codigo).ToList()
                                .ForEach(_ptRes => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptRes.Codigo.ToString())));
                    }
                }
            }
        }

        private void LimparTodosOsCamposPTRes(IList<PTResEntity> listaPreenchimento)
        {
            LimparCamposPTRes();

            if (listaPreenchimento.IsNotNullAndNotEmpty())
            {
                this.ddlPTResAcao.InserirElementoSelecione(true);
                this.ddlPTResItemMaterial.InserirElementoSelecione(true);

                listaPreenchimento.Where(ptres => ptres.ProgramaTrabalho.IsNotNull())
                                  .DistinctBy(d => d.Codigo).ToList()
                                  .OrderBy(f=>f.Codigo).ToList()
                                  .ForEach(_ptresVinculado => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptresVinculado.Codigo.ToString())));

                listaPreenchimento.Where(ptres => ptres.ProgramaTrabalho.IsNotNull())
                                  //.DistinctBy(d => d.Codigo).ToList()
                                  .DistinctBy(d => new { d.Codigo, d.ProgramaTrabalho.ProjetoAtividade }).ToList()
                                  .ForEach(_ptresVinculado => this.ddlPTResAcao.Items.Add(new ListItem(_ptresVinculado.ProgramaTrabalho.ProjetoAtividade, _ptresVinculado.Codigo.ToString())));
            }
            else
            {
                PreencherListaPTRes();
                PreencherListaPTResAcao();
            }
        }

        private void LimparCamposPTRes()
        {
            this.PTResId = -1;
            this.PTResCodigo = -1;
            this.PTResDescricao = null;
            this.PTResAcao = null;
            this.PTAssociadoPTRes = null;
        }

        protected void ddlPTResItemMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTRes);

            if (this.ddlPTResItemMaterial.SelectedIndex != 0 && listaPTRes.IsNotNullAndNotEmpty())
            {
                this.ddlPTResAcao.InserirElementoSelecione(true);

                listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                           && !String.IsNullOrWhiteSpace(ptres.ProgramaTrabalho.ProjetoAtividade)
                                           && ptres.Codigo.ToString() == this.ddlPTResItemMaterial.SelectedItem.Text)
                              //.DistinctBy(p => p.Codigo).ToList()
                              .DistinctBy(p => new { p.Codigo, p.ProgramaTrabalho.ProjetoAtividade }).ToList()
                              .ForEach(_ptresAcaoVinculada => this.ddlPTResAcao.Items.Add(new ListItem(_ptresAcaoVinculada.ProgramaTrabalho.ProjetoAtividade)));

                if (this.ddlPTResAcao.Items.Count >= 2)
                {
                    var _ptresVinculado = listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                                && !String.IsNullOrWhiteSpace(ptres.ProgramaTrabalho.ProjetoAtividade)
                                                                && ptres.Codigo.ToString() == this.ddlPTResItemMaterial.SelectedItem.Text)
                                               .FirstOrDefault();

                    if (_ptresVinculado.IsNotNull())
                    {
                        this.PTResId = _ptresVinculado.Id;
                        this.PTResDescricao = _ptresVinculado.Descricao;
                        this.PTResCodigo = _ptresVinculado.Codigo;
                        this.PTResAcao = ((_ptresVinculado.ProgramaTrabalho.IsNotNull()) ? _ptresVinculado.ProgramaTrabalho.ProjetoAtividade : "");
                        this.PTAssociadoPTRes = _ptresVinculado.CodigoPT;
                    }
                    else
                    {
                        LimparCamposPTRes();
                    }
                }
            }
            else if (this.ddlPTResAcao.SelectedIndex == 0 && this.ddlPTResItemMaterial.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTRes);
            }
            else if (this.ddlPTResItemMaterial.SelectedIndex == 0)
            {
                this.ddlPTResItemMaterial.InserirElementoSelecione(true);

                listaPTRes.Where(ptres => ptres.ProgramaTrabalho.IsNotNull())
                              .DistinctBy(d => new { d.Codigo, d.ProgramaTrabalho.ProjetoAtividade }).ToList()
                              .ForEach(_ptresVinculado => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptresVinculado.Codigo.ToString())));

                LimparCamposPTRes();
            }
            else if (this.ddlPTResItemMaterial.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTRes);
            }
        }

        protected void ddlPTResAcao_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);

            if (this.ddlPTResAcao.SelectedIndex != 0 && listaPTResAcao.IsNotNullAndNotEmpty())
            {
                this.ddlPTResItemMaterial.InserirElementoSelecione(true);

                listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                           && ptres.ProgramaTrabalho.ProjetoAtividade == this.ddlPTResAcao.SelectedItem.Text)
                              .DistinctBy(p => p.Codigo).ToList()
                              .ForEach(_ptresVinculado => this.ddlPTResItemMaterial.Items.Add(new ListItem(_ptresVinculado.Codigo.ToString())));

                if (this.ddlPTResItemMaterial.Items.Count >= 2)
                {
                    var _ptresAcao = listaPTResAcao.Where(ptres => ptres.ProgramaTrabalho.IsNotNull()
                                                                && ptres.ProgramaTrabalho.ProjetoAtividade == this.ddlPTResAcao.SelectedItem.Text)
                                                   .FirstOrDefault();
                    if (_ptresAcao.IsNotNull())
                    {
                        this.PTResId = _ptresAcao.Id;
                        this.PTResDescricao = _ptresAcao.Descricao;
                        this.PTResCodigo = _ptresAcao.Codigo;
                        this.PTResAcao = ((_ptresAcao.ProgramaTrabalho.IsNotNull()) ? _ptresAcao.ProgramaTrabalho.ProjetoAtividade : "");
                        this.PTAssociadoPTRes = _ptresAcao.CodigoPT;
                    }
                }
                else
                {
                    LimparCamposPTRes();
                }
            }
            else if (this.ddlPTResAcao.SelectedIndex == 0)
            {
                LimparTodosOsCamposPTRes(listaPTResAcao);
            }
        }

        #endregion Eventos

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
                return txtDescricao.Text;
            }
            set
            {
                txtDescricao.Text = value;
            }
        }

        public int OrgaoId { get; set; }
        //{
        //    get
        //    {
        //        return 1;
        //    }
        //}

        public string DescricaoDoacao
        {
            get { return txtDescricaoAvulsa.Text; }
            set { txtDescricaoAvulsa.Text = value; }
        }

        public int UgeId
        {
            get
            {
                if (ddlUGE.Items.Count > 0)
                    return (int)helperTratamento.TryParseInt32(ddlUGE.SelectedValue);
                else
                    return 0;
            }
            set
            { }
        }

        public string UgeDesc
        {
            get
            {
                if (ddlUGE.Items.Count > 0)
                    return ddlUGE.SelectedItem.Text;
                else
                    return string.Empty;
            }
        }

        public string Unidade
        {
            get
            {
                return txtUnidadeFornecimento.Text;
            }
            set
            {
                txtUnidadeFornecimento.Text = value;
            }
        }

        public int UnidadeId
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public int? FornecedorId
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public int TipoMovimento
        {
            get
            {
                if (rblTipoMovimento != null)
                {
                    if (rblTipoMovimento.Items.Count > 0)
                    {
                        if ((int)helperTratamento.TryParseInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.RequisicaoPendente)
                        {
                            if ((int)helperTratamento.TryParseInt32(rblTipoOperacao.SelectedValue) == (int)enumTipoRequisicao.Nova)
                                return (int)enumTipoMovimento.RequisicaoPendente;
                            else

                                return (int)enumTipoMovimento.RequisicaoAprovada;
                        }
                        else
                        {
                            return (int)helperTratamento.TryParseInt32(rblTipoMovimento.SelectedValue);
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value != null)
                {
                    if (rblTipoMovimento.Items.Count > 0)
                        //rblTipoMovimento.SelectedValue = value.ToString();
                        rblTipoMovimento.SelectedIndex = value;
                }
            }
        }

        public int TipoOperacao
        {
            get
            {
                if (rblTipoOperacao.SelectedIndex >= 0)
                    return Convert.ToInt16(rblTipoOperacao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                ListItem item = rblTipoOperacao.Items.FindByValue(value.ToString());
                if (item != null)
                {
                    rblTipoOperacao.ClearSelection();
                    item.Selected = true;
                }
            }
        }

        public string NumeroDocumento
        {
            get { return txtRequisicao.Text; }
            set { txtRequisicao.Text = value; }
        }

        public string AnoMesReferencia
        {
            get
            {
                string vlRetorno = string.Empty;

                if (this.GetAcesso.IsNotNull() &&
                   this.GetAcesso.Transacoes.IsNotNull() &&
                   this.GetAcesso.Transacoes.Perfis.IsNotNullAndNotEmpty() &&
                   !String.IsNullOrWhiteSpace(this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef))
                {
                    vlRetorno = this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
                }

                return vlRetorno;
            }
            set { }
        }

        public int? DivisaoId
        {
            get
            {
                return helperTratamento.TryParseInt32(ddlDivisao.SelectedValue);
            }
            set
            {
                var _valor = value.GetValueOrDefault().ToString();

                if (ddlDivisao.Items.Count > 0)
                {
                    if (_valor.Equals("0"))
                        ddlDivisao.SelectedIndex = 0;
                    else if (ddlDivisao.Items.FindByValue(_valor) != null)
                        ddlDivisao.SelectedValue = _valor;
                }
            }
        }

        public DateTime? DataDocumento
        {
            get
            {
                //A Data do documento igual a data do movimento
                return DataMovimento;
            }
            set
            {
            }
        }

        public DateTime? DataMovimento
        {
            get
            {
                return RetornoDataMovimento();
            }
            set
            {
                if (value != null)
                {
                    txtDataMovimento.Text = value.Value.GetDateTimeFormats().ToList()[0];
                    //txtDataMovimentoTrans.Text = value.Value.GetDateTimeFormats().ToList()[0];
                    //txtDataMovimentoDoacao.Text = value.Value.GetDateTimeFormats().ToList()[0];
                    //txtDataMovimentoOutras.Text = value.Value.GetDateTimeFormats().ToList()[0];
                }
                else
                {
                    txtDataMovimento.Text = string.Empty;
                    //txtDataMovimentoTrans.Text = string.Empty;
                }
            }
        }

        public string FonteRecurso
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public decimal? ValorDocumento
        {
            get
            {
                decimal decValorMovimentacao = -1.00m;
                string strValorMovimentacao = null;

                strValorMovimentacao = txtValorTotal.Text.Remove(0, 2);
                if (!String.IsNullOrWhiteSpace(strValorMovimentacao))
                    Decimal.TryParse(strValorMovimentacao, out decValorMovimentacao);

                return decValorMovimentacao;
            }

            set
            {
                string strValor = null;

                if (value.HasValue)
                    strValor = String.Format("{0:#,##0.00}", value.Value);

                txtValorTotal.Text = strValor;
            }
        }

        public string Observacoes
        {
            get
            {
                string strRetorno = string.Empty;

                //if (txtObservacoes == null)
                //    txtObservacoes = new TextBox();

                //if (TipoMovimento == (int)enumTipoMovimento.SaidaPorTransferencia)
                //{ strRetorno = txtObsTransferencia.Text; }
                //else if (TipoMovimento == (int)enumTipoMovimento.SaidaPorDoação)
                //{ strRetorno = txtObsDoacao.Text; }
                //else if ( (TipoMovimento == (int)enumTipoMovimento.OutrasSaidas) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaMaterialTransformado)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaPorExtravioFurtoRoubo)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaPorIncorporacaoIndevida)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaPorPerda)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaInservivelQuebra)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado)) ||
                //          (TipoMovimento == ((int)enumTipoMovimento.SaidaParaAmostraExposicaoAnalise)))
                //{ strRetorno = txtObsOutros.Text; }
                //else
                //{
                if (!string.IsNullOrEmpty(txtObservacoes.Text))
                    strRetorno = txtObservacoes.Text;
                //}

                return strRetorno;
            }
            set
            {
                //if (txtObservacoes == null)
                //    txtObservacoes = new TextBox();
                //txtObsOutros.Text = value;
                txtObservacoes.Text = value;
                //txtObsDoacao.Text = value;
                //txtObsTransferencia.Text = value;
            }
        }

        public string Instrucoes
        { get; set; }

        public string Empenho
        { get; set; }

        public string NlLiquidacao
        { get; set; }

        public int? AlmoxarifadoIdOrigem
        {
            get
            {
                int almoxDestinoTransferenciaID = -1;
                if (ddlAmoxarifado_TransferenciaDoacao.IsNotNull() && ddlAmoxarifado_TransferenciaDoacao.Items.Cast<ListItem>().HasElements())
                    Int32.TryParse(ddlAmoxarifado_TransferenciaDoacao.SelectedItem.Value, out almoxDestinoTransferenciaID);

                return almoxDestinoTransferenciaID;
            }
            set
            {
                if (value.HasValue)
                    ddlAmoxarifado_TransferenciaDoacao.SelectedValue = (ddlAmoxarifado_TransferenciaDoacao.Items.FindByValue(value.ToString())).Value;
            }
        }

        public string GeradorDescricao
        {
            get
            {
                string strRetorno = string.Empty;

                //if (this.TipoMovimento == (int)enumTipoMovimento.OutrasSaidas)
                if ((this.TipoMovimento == (int)enumTipoMovimento.OutrasSaidas) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorMaterialTransformado)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorExtravioFurtoRoubo)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorIncorporacaoIndevida)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorPerda)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaInservivelQuebra)) ||
                    //(this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaParaAmostraExposicaoAnalise)))
                {
                    strRetorno = EnumUtils.GetEnumDescription<enumTipoMovimento>((enumTipoMovimento)this.TipoMovimento);
                }
                else if ((this.TipoMovimento == (int)enumTipoMovimento.SaidaPorDoacao) || (this.TipoMovimento == (int)enumTipoMovimento.SaidaPorTransferencia))
                {
                    //strRetorno = txtDestino.Text;
                    //strRetorno = ddlAlmoxDestinoDoacao.SelectedValue;
                    strRetorno = ddlAmoxarifado_TransferenciaDoacao.SelectedValue;
                }
                else if (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado))
                {
                    strRetorno = string.Format("{0} / {1}", EnumUtils.GetEnumDescription<enumTipoMovimento>((enumTipoMovimento)this.TipoMovimento),
                                                txtDescricaoAvulsa.Text.ToUpper());
                }

                return strRetorno;
            }
            set
            {
                if ((this.TipoMovimento == (int)enumTipoMovimento.OutrasSaidas) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorMaterialTransformado)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorExtravioFurtoRoubo)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorIncorporacaoIndevida)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorPerda)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaInservivelQuebra)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado)) ||
                    (this.TipoMovimento == ((int)enumTipoMovimento.SaidaParaAmostraExposicaoAnalise)))
                {
                    if (value.IsNotNull())
                        this.TipoMovimento = (int)EnumUtils.ParseDescriptionToEnum<enumTipoMovimento>(value);
                    else
                        this.TipoMovimento = 0;
                }
                else if ((this.TipoMovimento == (int)enumTipoMovimento.SaidaPorDoacao) || (this.TipoMovimento == (int)enumTipoMovimento.SaidaPorTransferencia))
                {
                    if (value.IsNotNull())
                        ddlAmoxarifado_TransferenciaDoacao.SelectedValue = ddlAmoxarifado_TransferenciaDoacao.Items.FindByText(value.ToString()).Value;
                    else
                        ddlAmoxarifado_TransferenciaDoacao.SelectedIndex = 0;
                }
            }
        }

        public string IdItem
        { get; set; }

        public DateTime? DataVctoLoteItem
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string IdentificacaoLoteItem
        { get; set; }

        public string FabricLoteItem
        { get; set; }

        public decimal QtdeItem
        {
            get { return 1.00m; }
            set { }
        }

        public decimal QtdeLiqItem
        {
            get { return helperTratamento.TryParseDecimal(this.txtQtdSolicitada.Text, true).Value; }
            set
            {
                if (value != 0)
                    this.txtQtdSolicitada.Text = value.ToString(base.fmtFracionarioMaterialQtde);
                else
                    this.txtQtdSolicitada.Text = string.Empty;
            }
        }

        public decimal? SaldoValorItem
        {
            get
            {
                decimal? _decValor = 0.00m;
                if (!String.IsNullOrWhiteSpace(txtSaldo.Text))
                    _decValor = helperTratamento.TryParseDecimal(txtSaldo.Text).Value;

                return _decValor;
            }
            set
            {
                if (value.IsNotNull() && value.Value != _valorZero)
                    txtSaldo.Text = value.Value.ToString(base.fmtValorFinanceiro);
            }
        }

        public decimal SaldoQtdeItem
        {
            get { return 1.00m; }
            set { }
        }

        public decimal? SaldoQtdeLoteItem
        {
            get { return 1.00m; }
            set { }
        }

        public decimal PrecoUnitItem
        {
            get { return 1.00m; }
            set { }
        }

        public decimal ValorMovItem
        {
            get { return 1.00m; }
            set { }
        }

        public decimal DesdItem
        {
            get { return 1.00m; }
            set { }
        }

        public int TipoRequisicao
        { get; set; }

        public int? MovimentoItemId
        {
            get
            {
                if (hfdMovimentoItemId == null)
                    return null;

                if (hfdMovimentoItemId.Value != string.Empty)
                    return helperTratamento.TryParseInt32(hfdMovimentoItemId.Value);
                else
                    return new Random().Next(100000000, 999999999);
            }
            set
            {
                if (value != null)
                    hfdMovimentoItemId.Value = value.ToString();
                else
                {
                    hfdMovimentoItemId.Value = string.Empty;
                }
            }
        }

        public bool? AtivoItem
        {
            get
            {
                return true;
            }
            set
            {
            }
        }

        public int? SubItemMatItem
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public int? SubItemMaterialId
        {
            get
            {
                return helperTratamento.TryParseInt32(idSubItem.Value);
            }
            set
            {
                idSubItem.Value = value.ToString();
            }
        }

        public long? SubItemMaterialCodigo
        {
            get
            {
                return helperTratamento.TryParseLong(txtSubItem.Text);
            }
            set
            {
                txtSubItem.Text = value.ToString();
            }
        }

        public string SubItemMaterialDescricao
        {
            get
            {
                return txtDescricao.Text;
            }
            set
            {
                txtDescricao.Text = value.ToString();
            }
        }

        public int? ItemMaterialId
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public int? ItemMaterialCodigo
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public bool HabilitarLote
        {
            set { }
        }

        public int? NaturezaDespesaIdItem
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public string SubItemMaterialTxt
        {
            get
            {
                return txtSubItem.Text;
            }
            set
            {
                txtSubItem.Text = value;
                idSubItem.Value = value;
            }
        }

        public string Saldo
        {
            set
            {
                txtSaldo.Text = value;
            }
        }

        public string QtdFornecida
        {
            get
            {
                return txtQtdeMov.Text;
            }

            set
            {
                txtQtdeMov.Text = value;
            }
        }

        public int? PTResId { get; set; }

        public int? PTResCodigo
        {
            get
            {
                int? _codigoPTRes = null;
                var _tryParsePTResCodigo = -1;

                //if (this.ddlPTResItemMaterial.Items.Cast<ListItem>().ToList().HasElements())
                if (this.ddlPTResItemMaterial.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(this.ddlPTResItemMaterial.SelectedItem.Text.BreakLine(0), out _tryParsePTResCodigo))
                    _codigoPTRes = Int32.Parse(this.ddlPTResItemMaterial.SelectedItem.Text.BreakLine(0));
                else
                    _codigoPTRes = _tryParsePTResCodigo;

                return _codigoPTRes;
            }
            set
            {
                int idxOpcaoCombo = -1;

                if (!String.IsNullOrWhiteSpace(value.ToString()))
                {
                    ListItem _itemCombo = this.ddlPTResItemMaterial.Items.Cast<ListItem>().Where(_ptres => _ptres.Text.Contains(value.ToString())).FirstOrDefault();
                    idxOpcaoCombo = this.ddlPTResItemMaterial.Items.IndexOf(_itemCombo);

                    if (_itemCombo.IsNotNull() && idxOpcaoCombo != -1)
                        this.ddlPTResItemMaterial.SelectedIndex = idxOpcaoCombo;
                }
            }
        }

        public string PTResAcao
        {
            get
            {
                string _codigoPTResAcao = null;
                var _tryParsePTResAcao = -1;

                try
                {
                    if (this.ddlPTResAcao.Items.Cast<ListItem>().ToList().HasElements() && Int32.TryParse(this.ddlPTResAcao.SelectedItem.Text, out _tryParsePTResAcao))
                        _codigoPTResAcao = this.ddlPTResAcao.SelectedItem.Text;
                    else
                        _codigoPTResAcao = _tryParsePTResAcao.ToString();
                }
                catch
                {
                    //logErro.GravarMsgInfo("Tela Requisição Material", "Erro ao obter valor campo PTResCodigo");
                }

                return _codigoPTResAcao;
            }
            set
            {
                int idxOpcaoCombo = -1;

                if (!String.IsNullOrWhiteSpace(value))
                {
                    ListItem _itemCombo = this.ddlPTResAcao.Items.Cast<ListItem>().Where(_ptres => _ptres.Text.Contains(value)).FirstOrDefault();
                    idxOpcaoCombo = this.ddlPTResAcao.Items.IndexOf(_itemCombo);

                    if (_itemCombo.IsNotNull() && idxOpcaoCombo != -1)
                        this.ddlPTResAcao.SelectedIndex = idxOpcaoCombo;
                }
            }
        }

        public string PTAssociadoPTRes { get; set; }

        public string PTResDescricao
        {
            get { return txtDescricaoPTRes.Text; }
            set { this.txtDescricaoPTRes.Text = value; }
        }

        public bool ExibirListaEmpenho
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public bool ExibirNumeroEmpenho
        {
            get { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }

        public bool BloqueiaEmpenho
        {
            set { throw new NotImplementedException("Campo/Funcionalidade não utilizado por esta tela!"); }
        }
        #endregion Propriedades

        #region Metodos

        #region Bloqueia Exibe

        public void ExibirMensagem(string _mensagem)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + _mensagem + "');", true);
        }

        /// <summary>
        /// Retrocompatibilidade com interface ja existente
        /// </summary>
        public bool BloqueiaNovo
        {
            set { BloqueiaBotaoNovo = value; }
        }
        /// <summary>
        /// Retrocompatibilidade com interface ja existente
        /// </summary>
        public bool BloqueiaExcluir
        {
            set { BloqueiaBotaoExcluir = value; }
        }

        public bool ExibeBotaoNovo
        {
            get { return (btnNovo.Visible); }
            set { btnNovo.Visible = value; }
        }

        public bool BloqueiaTipoOperacao { set { rblTipoOperacao.Enabled = !value; } }

        public bool BloqueiaBotaoNovo
        {
            set
            {
                if (String.IsNullOrEmpty(txtRequisicao.Text) && TipoMovimento == (int)enumTipoMovimento.RequisicaoPendente)
                    btnNovo.Enabled = false;
                else
                {
                    if (pnlEditar.Visible == false)
                        btnNovo.Enabled = true;
                    else
                    {
                        btnNovo.Enabled = false;
                    }
                }
            }
        }

        public bool BloqueiaGravar
        {
            set
            {
                HabilitarBotoes();
            }
        }

        public bool ExibeBotaoExcluir
        {
            get
            { return (btnExcluir.Visible); }

            set
            { btnExcluir.Visible = value; }
        }
        public bool BloqueiaBotaoExcluir
        {
            set { btnExcluir.Enabled = value; }
        }

        public bool BloqueiaCancelar
        {
            set
            {
                btnCancelar.Enabled = value;
            }
        }

        public bool BloqueiaCodigo
        {
            set { }
        }

        public bool BloqueiaDescricao
        {
            set { }
        }

        public bool BloqueiaListaUA
        {
            set
            {
            }
        }

        public bool BloqueiaNovoItem
        {
            set
            {
            }
        }

        public void PopularGrid()
        {
        }

        public bool BloqueiaListaDivisao
        {
            set
            {
                ddlDivisao.Enabled = value;
            }
        }

        public System.Collections.SortedList ParametrosRelatorio
        {
            get
            {
                SortedList paramList = new SortedList();
                paramList.Add("MovimentoId", hdfMovimentoId.Value);

                return paramList;
            }
        }

        public bool ExibeBotaoEstornar
        {
            get { return (btnEstornar.Visible); }
            set { btnEstornar.Visible = value; }
        }
        public bool BloqueiaBotaoEstornar
        { set { btnEstornar.Enabled = !value; } }

        public RelatorioEntity DadosRelatorio { get; set; }

        #endregion Bloqueia Exibe

        private string TipoMovimentoTituloDocumento()
        {
            string strTituloDocumento = null;

            switch (TipoMovimento)
            {
                case ((int)enumTipoMovimento.SaidaPorTransferencia): strTituloDocumento = "Nota de Transferência"; break;
                case ((int)enumTipoMovimento.SaidaPorDoacao): strTituloDocumento = "Nota de Doação"; break;

                case ((int)enumTipoMovimento.RequisicaoAprovada):
                case ((int)enumTipoMovimento.OutrasSaidas):
                case ((int)enumTipoMovimento.SaidaPorMaterialTransformado):
                case ((int)enumTipoMovimento.SaidaPorExtravioFurtoRoubo):
                case ((int)enumTipoMovimento.SaidaPorIncorporacaoIndevida):
                case ((int)enumTipoMovimento.SaidaPorPerda):
                case ((int)enumTipoMovimento.SaidaInservivelQuebra):
                case ((int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado):
                case ((int)enumTipoMovimento.SaidaParaAmostraExposicaoAnalise):
                default: strTituloDocumento = "Nota de Fornecimento"; break;
            }

            return strTituloDocumento;
        }

        private void InstanciaObjetoMovimento()
        {
            if (movimento == null)
                movimento = new MovimentoEntity();

            movimento.MovimentoItem = null;

            if (movimento.TipoMovimento == null)
                movimento.TipoMovimento = new TipoMovimentoEntity();

            if (movimento.Almoxarifado == null)
                movimento.Almoxarifado = new AlmoxarifadoEntity();

            if (movimento.Divisao == null)
                movimento.Divisao = new DivisaoEntity();

            if (movimento.MovimAlmoxOrigemDestino == null)
                movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();

            if (movimento.UGE == null)
                movimento.UGE = new UGEEntity();
        }

        public IList ListaErros
        {
            set
            {
                if (value != null)
                    this.ListInconsistencias.ExibirLista(value);
            }
        }

        //public void PopularListaAlmoxarifado()
        //{
        //    ddlOrgao_TransferenciaDoacao.DataSourceID = "obsAlmoxarifado";
        //}

        public void PopularListaOrgaos()
        {
            IList<OrgaoEntity> lstOrgaosFiltradosPorGestao = null;
            OrgaoPresenter objPresenter = null;
            int codigoGestao = 0;
            var orgaoId = 0;

            var objAcesso = this.GetAcesso;
            if (objAcesso.IsNotNull() &&
                objAcesso.Transacoes.IsNotNull() &&
                objAcesso.Transacoes.Perfis.IsNotNullAndNotEmpty())
                codigoGestao = objAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.CodigoGestao.Value;

            objPresenter = new OrgaoPresenter();

            if (TipoMovimento == (int)tipoMovimentacao.SaidaPorTransferencia)
                lstOrgaosFiltradosPorGestao = objPresenter.ListarOrgaosPorGestaoImplantado(codigoGestao);
            else if (TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao)
                lstOrgaosFiltradosPorGestao = objPresenter.ListarOrgaosExcetoPorGestaoImplantado(codigoGestao);

            InicializarDropDownList(ddlOrgao_TransferenciaDoacao);
            ddlOrgao_TransferenciaDoacao.DataSource = lstOrgaosFiltradosPorGestao.OrderBy(a => a.Codigo);
            ddlOrgao_TransferenciaDoacao.DataTextField = "CodigoDescricao";
            ddlOrgao_TransferenciaDoacao.DataValueField = "Id";
            ddlOrgao_TransferenciaDoacao.DataBind();
            ddlOrgao_TransferenciaDoacao.Enabled = true;

            if ((ddlOrgao_TransferenciaDoacao.Items.Count > 0) && Int32.TryParse(ddlOrgao_TransferenciaDoacao.SelectedValue, out orgaoId))
            {
                var almoxPresenter = new AlmoxarifadoPresenter();
                OrgaoId = orgaoId;

                List<AlmoxarifadoEntity> almox = new List<AlmoxarifadoEntity>();


                InicializarDropDownList(ddlAmoxarifado_TransferenciaDoacao);
                almox = almoxPresenter.ListarAlmoxarifadosPorOrgao(OrgaoId).Where(a => a.Id != this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id).ToList();

                //if (TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao)
                //{
                //    ddlAmoxarifado_TransferenciaDoacao.Items.Add(new ListItem("Sem almoxarifado"));
                //}
                ddlAmoxarifado_TransferenciaDoacao.AppendDataBoundItems = true;

                ddlAmoxarifado_TransferenciaDoacao.DataSource = almox;
                //ddlAmoxarifado_TransferenciaDoacao.DataSource = almoxPresenter.PopularListaAlmoxarifadoMenosAlmoxLogado();
                ddlAmoxarifado_TransferenciaDoacao.DataBind();
            }
        }

        public void PopularListaDivisao()
        {
            ddlDivisao.DataSourceID = "odsDivisao";
        }

        public void PopularListaTipoMovimentoSaida()
        {
            TipoMovimentoPresenter presenter = new TipoMovimentoPresenter();
            var result = presenter.PopularListaTipoMovimentoSaida();

            rblTipoMovimento.DataSource = result;
            rblTipoMovimento.DataBind();
            // rblTipoMovimento.DataSourceID = "sourceListaTipoMovimento";


        }

        public void PopularListaUGE()
        {
            ddlUGE.DataSourceID = "odsUGE";
           if (TipoMovimento== (int)(enumTipoMovimento.SaidaPorDoacao))
            {
                ddlUGE.Enabled = false;
            }
        }

        public DateTime? RetornoDataMovimento()
        {
            DateTime? dtRetorno = null;

            switch (TipoMovimento)
            {
                //case (int)(enumTipoMovimento.SaidaPorTransferencia): dtRetorno = helperTratamento.TryParseDateTime(txtDataMovimentoTrans.Text); break;
                //case (int)(enumTipoMovimento.SaidaPorDoação): dtRetorno = helperTratamento.TryParseDateTime(txtDataMovimentoDoacao.Text); break;
                case (int)(enumTipoMovimento.SaidaPorTransferencia):
                case (int)(enumTipoMovimento.SaidaPorDoacao):
                case (int)(enumTipoMovimento.OutrasSaidas):
                case (int)(enumTipoMovimento.SaidaPorMaterialTransformado):
                case (int)(enumTipoMovimento.SaidaPorExtravioFurtoRoubo):
                case (int)(enumTipoMovimento.SaidaPorIncorporacaoIndevida):
                case (int)(enumTipoMovimento.SaidaPorPerda):
                case (int)(enumTipoMovimento.SaidaInservivelQuebra):
                case (int)(enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado):
                //case (int)(enumTipoMovimento.SaidaParaAmostraExposicaoAnalise): dtRetorno = helperTratamento.TryParseDateTime(txtDataMovimentoOutras.Text); break;
                case (int)(enumTipoMovimento.SaidaParaAmostraExposicaoAnalise):
                default:
                    dtRetorno = helperTratamento.TryParseDateTime(txtDataMovimento.Text); break;
            }

            return dtRetorno;
        }

        public void PopularListaLote(int almoxarifadoId)
        {
            MovimentoItemPresenter presenter = new MovimentoItemPresenter();
            bool soma = false;

            DateTime? dtRetorno = RetornoDataMovimento();

            var resultado = presenter.ListarSaldoSubItemPorLote(SubItemMaterialId,almoxarifadoId , dtRetorno);

            LimparLote(null, resultado);

            if (resultado.Count > 0)
            {
                if (ViewState["botao"] != null)
                {
                    if (ViewState["botao"].ToString() == "btnNovo" && resultado.Count > 1)
                    {
                        soma = true;
                        txtQtdeMov.Enabled = false;
                        LimparLote(resultado, null);
                    }
                    else
                    {
                        txtQtdeMov.Enabled = true;

                        if (ViewState["botao"].ToString() == "btnNovo")
                            txtQtdeMov.Text = "";

                        if (ViewState["botao"].ToString() == "btnEditar")
                        {
                            if (ddlLote.Items.Count > 1)
                            {
                                if (ViewState["LoteTemp"].ToString() != "0")
                                {
                                    ddlLote.SelectedValue = ViewState["LoteTemp"].ToString();
                                }
                                else
                                {
                                    soma = true;
                                    txtQtdeMov.Enabled = false;
                                    LimparLote(resultado, null);
                                }
                            }
                        }
                    }
                }
            }

            SetSession<IList<SaldoSubItemEntity>>(resultado, sessaoLote);

            txtSaldo.Text = RecuperaSaldo(soma);
        }

        public void ExibirRelatorio()
        {
            SetSession<RelatorioEntity>(this.DadosRelatorio, base.ChaveImpressaoUsuario);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), Guid.NewGuid().ToString(), Constante.ReportScript, false);
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                pnlEditar.Visible = value;
                BloqueiaBotaoNovo = true;
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        { }

        private bool ExisteSessaoMovimentoItem()
        {
            if (movimento != null)
            {
                if (movimento.MovimentoItem != null)
                {
                    if (movimento.MovimentoItem.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void AtualizarSessao()
        {
            bool atualizar = false;
            if (movimento == null)
            {
                atualizar = true;
            }
            else
            {
                if (movimento.MovimentoItem == null)
                    atualizar = true;
                else if (movimento.MovimentoItem.Count == 0)
                    atualizar = true;
            }

            if (atualizar == true)
            {
                if (GetSession<MovimentoEntity>(sessaoMov) != null)
                {
                    movimento = GetSession<MovimentoEntity>(sessaoMov);
                }
            }
        }

        public void PrepararVisualizacaoBotoes(bool travaComboFiltroPesquisa = true)
        {
            tipoOperacao tipoOperacaoSelecionada = (tipoOperacao)TipoOperacao;

            if (tipoOperacaoSelecionada == tipoOperacao.Nova)
            {
                ExibeBotaoGravar = true;
                ExibeBotaoEstornar = false;
                ExibeBotaoImprimir = false;
                ExibeBotaoNovo = true;

                ExibeBotaoExcluir = true;
                ExibeBotaoAdicionar = true;
                BloqueiaRascunho = false;
                BloqueiaCampoDataMovimento = false;
                BloqueiaCampoObservacoes = false;
            }
            else if (tipoOperacaoSelecionada == tipoOperacao.Estorno)
            {
                ExibeBotaoGravar = false;
                ExibeBotaoEstornar = true;
                ExibeBotaoImprimir = false;
                ExibeBotaoNovo = false;

                ExibeBotaoExcluir = false;
                ExibeBotaoAdicionar = false;
                BloqueiaRascunho = true;
                ExibeCampoPesquisaDocumento = true;

                if (TipoMovimento == (int)enumTipoMovimento.RequisicaoAprovada)
                    BloqueiaCamposInfoRequisicao = travaComboFiltroPesquisa;
                else if ((TipoMovimento == (int)enumTipoMovimento.SaidaPorDoacao) || (TipoMovimento == (int)enumTipoMovimento.SaidaPorTransferencia))
                    BloqueiaCamposInfoTransferenciaDoacao = travaComboFiltroPesquisa;

                BloqueiaCampoDataMovimento = true;
                BloqueiaCampoObservacoes = true;
                DataMovimento = null;
            }
            else if (tipoOperacaoSelecionada == tipoOperacao.NotaFornecimento)
            {
                ExibeBotaoGravar = false;
                ExibeBotaoEstornar = false;
                ExibeBotaoImprimir = true;
                ExibeBotaoNovo = false;

                ExibeBotaoExcluir = false;
                ExibeBotaoAdicionar = false;
                BloqueiaRascunho = true;
                BloqueiaCampoDataMovimento = true;
                BloqueiaCampoObservacoes = true;
                DataMovimento = null;
            }

            Observacoes = string.Empty;
        }

        public void HabilitarBotoes()
        {
            bool habilitar = false;

            if (rptSubItem.Items.Count > 0)
            {
                habilitar = true;
                rptSubItem.Visible = true;
                if (this.DataMovimento != null)
                    ((Label)rptSubItem.Controls[0].Controls[0].FindControl("lblGridDtaMov")).Text = this.DataMovimento.Value.ToShortDateString();
            }

            btnGravar.Enabled = habilitar;
            btnEstornar.Enabled = habilitar;
            btnImprimir.Enabled = habilitar;
        }

        public bool isRascunho
        {
            get
            {
                if (cbxRascunho.Checked == true)
                {
                    return true;
                }
                else
                    return false;
            }

            set
            {
                cbxRascunho.Checked = value;
            }
        }

        public bool ModoGravacaoOuEstorno
        {
            get { return ((TipoOperacao == (int)tipoOperacao.Nova) || (TipoOperacao == (int)tipoOperacao.Estorno)); }
        }

        public int rascunhoId
        {
            get
            {
                return 0;
            }
            set
            { }
        }

        public bool BloqueiaRascunho
        {
            set
            {
                cbxRascunho.Visible = false; //desativado
                //cbxRascunho.Visible = !value;
            }
        }

        [Obsolete("MARCADO PARA EXTINCAO")]
        private void HabilitaPainelRequisicao(bool notaFornecimento)
        {
            //pnlAprovacao.Visible = true;
            //pnlTransferencia.Visible = false;
            //pnlDoacao.Visible = false;
            //pnlOutras.Visible = false;

            //Label1.Visible = !notaFornecimento;
            lblDataMovimento.Visible = !notaFornecimento;
            txtDataMovimento.Visible = !notaFornecimento;

            lblObservacoes.Visible = !notaFornecimento;
            txtObservacoes.Visible = !notaFornecimento;

            PopularListaDivisao();
        }

        private void MontarViewRequisicao()
        {
            ExibeCamposInfoRequisicao = true;
            ExibeCamposInfoTransferenciaDoacao = false;
            idSubTipo.Visible = false;

            PopularListaDivisao();
        }

        private void MontarViewTransferenciaDoacao()
        {
            var isTransferencia = ((TipoMovimento == (int)tipoMovimentacao.SaidaPorTransferencia) || (TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao));

            ExibeCamposInfoRequisicao = !isTransferencia;
            ExibeCamposInfoTransferenciaDoacao = isTransferencia;

            PopularListaOrgaos();
        }

        private void MontarViewSaidasOutras()
        {
            ExibeCamposInfoRequisicao = false;
            ExibeCamposInfoTransferenciaDoacao = false;
        }

        private void MontarViewImpressaoNotaDocumento()
        {
            ExibeCamposInfoRequisicao = false;
            ExibeCampoPesquisaDocumento = true;


            enumTipoMovimento tipoMovimento = (enumTipoMovimento)TipoMovimento;
            switch (tipoMovimento)
            {
                case GeralEnum.TipoMovimento.RequisicaoAprovada: ExibeCamposInfoRequisicao = true; break;

                case GeralEnum.TipoMovimento.SaidaPorTransferencia:
                case GeralEnum.TipoMovimento.SaidaPorDoacao:
                case GeralEnum.TipoMovimento.OutrasSaidas:
                case GeralEnum.TipoMovimento.RequisicaoFinalizada:
                case GeralEnum.TipoMovimento.RequisicaoCancelada:
                case GeralEnum.TipoMovimento.SaidaPorMaterialTransformado:
                case GeralEnum.TipoMovimento.SaidaPorExtravioFurtoRoubo:
                case GeralEnum.TipoMovimento.SaidaPorIncorporacaoIndevida:
                case GeralEnum.TipoMovimento.SaidaPorPerda:
                case GeralEnum.TipoMovimento.SaidaInservivelQuebra:
                case GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                case GeralEnum.TipoMovimento.SaidaParaAmostraExposicaoAnalise:
                    break;
            }

            ExibeCampoDataMovimento = false;

            ExibeCampoObservacoes = true;
            BloqueiaCampoObservacoes = true;
        }

        private void MontarViewEspecificaParaTipoMovimentacaoSaida()
        {
            SaidaMaterialPresenter objPresenter = null;
            //txtRequisicao.Text = string.Empty;
            int tipoMovimentoID = -1;
            int tipoOperacaoID = -1;

            objPresenter = new SaidaMaterialPresenter(this);
            InstanciaObjetoMovimento();
            objPresenter.Cancelar();
            BloqueiaBotaoNovo = true;
            rptSubItem.DataBind();
            ddlSubTipoMovimento.SelectedValue = "0";
            txtDescricaoAvulsa.Text = string.Empty;

            if (Int32.TryParse(rblTipoMovimento.SelectedValue, out tipoMovimentoID) &&
                Int32.TryParse(rblTipoOperacao.SelectedValue, out tipoOperacaoID))
            {
                movimento.TipoMovimento.Id = tipoMovimentoID;
                lblTextoAvulso.Text = "UGE/CPF/CNPJ:";
                trtxtAvulso.Visible = false;
                rblTipoOperacao.Items[0].Enabled = true;

                enumTipoMovimento tipoMovimento = (enumTipoMovimento)tipoMovimentoID;
                switch (tipoMovimento)
                {
                    case GeralEnum.TipoMovimento.RequisicaoPendente:
                    case GeralEnum.TipoMovimento.RequisicaoAprovada:
                        MontarViewRequisicao(); break;
                    case GeralEnum.TipoMovimento.SaidaPorTransferencia:
                        MontarViewTransferenciaDoacao(); break;
                    case GeralEnum.TipoMovimento.SaidaPorDoacao:
                        lblTextoAvulso.Text = "CPF/CNPJ:";
                        MontarViewTransferenciaDoacao(); break;
                    case GeralEnum.TipoMovimento.OutrasSaidas:
                        MontarViewSaidasOutras();
                        InativarAdItem();
                        break;
                    case GeralEnum.TipoMovimento.SaidaPorMaterialTransformado:
                    case GeralEnum.TipoMovimento.SaidaPorExtravioFurtoRoubo:
                    case GeralEnum.TipoMovimento.SaidaPorIncorporacaoIndevida:
                    case GeralEnum.TipoMovimento.SaidaPorPerda:
                    case GeralEnum.TipoMovimento.SaidaInservivelQuebra:
                    case GeralEnum.TipoMovimento.SaidaParaAmostraExposicaoAnalise:
                        MontarViewSaidasOutras(); break;
                    case GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                        MontarViewSaidasOutras();
                        lblTextoAvulso.Text = "UGE Destino:";
                        /* Comentar ao entrar subtipo*/
                        this.trtxtAvulso.Visible = true;
                        this.txtDescricaoAvulsa.Enabled = tipoOperacaoID == tipoOperacao.Nova.GetHashCode();

                        break;
                }

                PrepararVisualizacaoBotoes();
            }

            ExibirCamposPTRes();
            ListarMovimentoItensRascunho();
            BloqueiaGravar = true;
            if (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Estorno).ToString() && ((Convert.ToInt32(rblTipoMovimento.SelectedValue) == (int)enumTipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado)))
            {
                trtxtAvulso.Visible = true;
                txtDescricaoAvulsa.Text = movimento.UgeCPFCnpjDestino;
            }

            if (TipoOperacao == (int)tipoOperacao.NotaFornecimento)
            {
                MontarViewImpressaoNotaDocumento();
                //return;
            }
            else
                ExibeCampoDataMovimento = true;



        }

        public string InscricaoCE
        {
            get { return txtInfoSiafemCE.Text; }
            set { txtInfoSiafemCE.Text = value; }
        }

        private void ExibirColunaRequisito(RepeaterItem item, bool Mostrar)
        {
            HtmlTableCell celMediaQtde = (HtmlTableCell)item.FindControl("celMediaQtde");
            HtmlTableCell celDescricao = (HtmlTableCell)item.FindControl("celDescricao");
            HtmlTableCell hdrMedia = (HtmlTableCell)item.FindControl("hdrMedia");
            HtmlTableCell hdrDescricao = (HtmlTableCell)item.FindControl("hdrDescricao");
            HtmlTableCell hdrTDAlerta = (HtmlTableCell)item.FindControl("hdrTDAlerta");
            HtmlTableRow hdrAlerta = (HtmlTableRow)item.FindControl("hdrAlerta");

            HtmlTableRow hdrSubItem = (HtmlTableRow)item.FindControl("hdrSubItem");
            HtmlTableCell celSaldoQtdeDt = (HtmlTableCell)item.FindControl("celSaldoQtdeDt");
            HtmlTableCell celSaldoQtde = (HtmlTableCell)item.FindControl("celSaldoQtde");
            HtmlTableCell celEditar = (HtmlTableCell)item.FindControl("celEditar");


            if (hdrMedia.IsNotNull()) hdrMedia.Visible = Mostrar; // false;// Mostrar;
            if (celMediaQtde.IsNotNull()) celMediaQtde.Visible = Mostrar; // false; // Mostrar;


            if (Convert.ToInt32(rblTipoOperacao.SelectedValue) == (int)enumTipoRequisicao.Nova)
            {
                if (movimento != null && Mostrar && Session["Bloquear"] != null)
                {
                    if ((bool)Session["Bloquear"])
                    {
                        if (hdrTDAlerta.IsNotNull())
                        {
                            hdrTDAlerta.InnerText = "Requisição está sendo editada pelo Requisitante";

                            hdrAlerta.Visible = true;
                            hdrSubItem.Visible = false;
                            hdrDescricao.Visible = false;
                        }
                        if (celSaldoQtdeDt != null && celSaldoQtde != null && celMediaQtde != null && celEditar != null)
                        {
                            celSaldoQtdeDt.Visible = false;
                            celSaldoQtde.Visible = false;
                            celMediaQtde.Visible = false;
                            celEditar.Visible = false;
                        }

                        btnGravar.Visible = false;
                        btnNovo.Visible = false;


                    }
                    else
                    {
                        if (hdrTDAlerta.IsNotNull())
                        {
                            hdrAlerta.Visible = false;
                            hdrSubItem.Visible = true;
                            hdrDescricao.Visible = true;
                        }
                        if (celSaldoQtdeDt != null && celSaldoQtde != null && celMediaQtde != null && celEditar != null)
                        {
                            celSaldoQtdeDt.Visible = true;
                            celSaldoQtde.Visible = true;
                            celMediaQtde.Visible = true;
                            celEditar.Visible = true;
                        }

                        btnGravar.Visible = true;
                        btnNovo.Visible = true;
                    }
                }
            }

            if (celDescricao.IsNotNull())
            {
                if (Mostrar)
                    celDescricao.ColSpan = 10; // 9; //10 MEDIA
                else
                    celDescricao.ColSpan = 7;
            }

            if (hdrDescricao.IsNotNull())
            {
                if (Mostrar)
                    hdrDescricao.ColSpan = 10; // 9; //10 MEDIA
                else
                    hdrDescricao.ColSpan = 7;
            }

        }

        private void ExibirColunaSaldo(RepeaterItem item, bool Mostrar)
        {
            HtmlTableCell hdrSaltoTotalDt = (HtmlTableCell)item.FindControl("hdrSaltoTotalDt");
            HtmlTableCell hdrSaltoTotal = (HtmlTableCell)item.FindControl("hdrSaltoTotal");
            HtmlTableCell hdrMedia = (HtmlTableCell)item.FindControl("hdrMedia");
            HtmlTableCell celSaldoQtdeDt = (HtmlTableCell)item.FindControl("celSaldoQtdeDt");
            HtmlTableCell celMediaQtde = (HtmlTableCell)item.FindControl("celMediaQtde");
            HtmlTableCell celSaldoQtde = (HtmlTableCell)item.FindControl("celSaldoQtde");
            HtmlTableCell hdrEditar = (HtmlTableCell)item.FindControl("hdrEditar");
            HtmlTableCell celEditar = (HtmlTableCell)item.FindControl("celEditar");

            if (hdrSaltoTotalDt.IsNotNull()) hdrSaltoTotalDt.Visible = Mostrar;
            if (hdrSaltoTotal.IsNotNull()) hdrSaltoTotal.Visible = Mostrar;
            if (celMediaQtde.IsNotNull()) celMediaQtde.Visible = Mostrar; // false;// Mostrar;
            if (celSaldoQtdeDt.IsNotNull()) celSaldoQtdeDt.Visible = Mostrar;
            if (celSaldoQtde.IsNotNull()) celSaldoQtde.Visible = Mostrar;
            if (hdrEditar.IsNotNull()) hdrEditar.Visible = Mostrar;
            if (celEditar.IsNotNull()) celEditar.Visible = Mostrar;
            if (hdrMedia.IsNotNull()) hdrMedia.Visible = Mostrar; // false;//Mostrar;
        }

        private void ExibirCamposPTRes(RepeaterItem rptIemGrid = null)
        {
            HtmlTableCell celPTRes = null;
            HtmlTableCell _colunaPTRes = null;

            HtmlTableCell celPTResAcao = null;
            HtmlTableCell _colunaPTResAcao = null;

            if (rptIemGrid.IsNotNull())
            {
                celPTRes = (HtmlTableCell)rptIemGrid.FindControl("celPTRes");
                _colunaPTRes = (HtmlTableCell)rptIemGrid.FindControl("hdrPTRes");

                celPTResAcao = (HtmlTableCell)rptIemGrid.FindControl("celPTResAcao");
                _colunaPTResAcao = (HtmlTableCell)rptIemGrid.FindControl("hdrPTResAcao");
            }

            var isRequisicao = ((this.TipoMovimento != 0) &&
                                (this.TipoMovimento == (int)enumTipoMovimento.RequisicaoAprovada || this.TipoMovimento == (int)enumTipoMovimento.RequisicaoPendente || this.TipoMovimento == (int)enumTipoMovimento.RequisicaoFinalizada));

            //this.lblIndicativoPTRes.Visible = this.lblIndicadorDescricaoPTRes.Visible = this.lblIndicadorAcaoPTRes.Visible = isRequisicao;
            //this.ddlPTResItemMaterial.Visible = this.txtDescricaoPTRes.Visible = this.txtAcaoPTRes.Visible = isRequisicao;

            this.lblIndicadorPTRes.Visible = this.lblIndicadorDescricaoPTRes.Visible = this.lblComboPTResAcao.Visible = isRequisicao;
            this.ddlPTResItemMaterial.Visible = this.ddlPTResAcao.Visible = this.txtDescricaoPTRes.Visible = isRequisicao;

            var _hasPTResRowGrid = (isRequisicao &&
                                    (rptIemGrid.IsNotNull()) &&
                                    (rptIemGrid.ItemType == ListItemType.Item || rptIemGrid.ItemType == ListItemType.AlternatingItem));

            var _hasPTResHeader = (isRequisicao &&
                                   rptIemGrid.IsNotNull() &&
                                   rptIemGrid.ItemType == ListItemType.Header);

            if (!_hasPTResHeader && _colunaPTRes.IsNotNull())
                //_colunaPTRes.Visible = isRequisicao;
                _colunaPTRes.Visible = _colunaPTResAcao.Visible = isRequisicao;
            else if (!_hasPTResRowGrid && celPTRes.IsNotNull())
                //celPTRes.Visible = isRequisicao;
                celPTRes.Visible = celPTResAcao.Visible = isRequisicao;
            else
                return;
        }

        public void VisivelControlesDivisao(bool blnVisivel)
        {
            ddlDivisao.Visible = blnVisivel;
            lblDivisao.Visible = blnVisivel;
        }

        public bool ExibeCamposInfoSiafemCEPadrao
        {
            set
            {
                if (trCamposInfoSiafemCE.IsNotNull())
                    this.trCamposInfoSiafemCE.Visible = value;
            }
        }

        public bool BloqueiaCamposInfoSiafemCE
        {
            set
            {
                if (txtInfoSiafemCE.IsNotNull())
                    this.txtInfoSiafemCE.Enabled = !value;
            }
        }

        public bool ExibeCamposInfoRequisicao
        {
            get
            { return (trCampoRequisicao_DivisaoRequisicao.Visible = trCampoRequisicao_NumeroDocumentoRequisicao.Visible); }

            set
            { trCampoRequisicao_DivisaoRequisicao.Visible = trCampoRequisicao_NumeroDocumentoRequisicao.Visible = value; }
        }
        public bool BloqueiaCamposInfoRequisicao
        {
            set
            { ddlDivisao.Enabled = txtRequisicao.Enabled = !value; }
        }

        public bool ExibeCampoPesquisaDocumento
        {
            get
            { return (trCampoRequisicao_NumeroDocumentoRequisicao.Visible); }

            set
            { trCampoRequisicao_NumeroDocumentoRequisicao.Visible = value; }
        }
        public bool BloqueiaCampoPesquisaDocumento
        {
            get
            { return (trCampoRequisicao_NumeroDocumentoRequisicao.Disabled); }

            set
            { trCampoRequisicao_NumeroDocumentoRequisicao.Disabled = value; }
        }

        public bool ExibeCampoObservacoes
        {
            get
            { return (trCampoObservacoes.Visible); }

            set
            { trCampoObservacoes.Visible = value; }
        }
        public bool ExibeCampoValorTotal
        {
            get
            { return (trCampoValorTotal.Visible); }

            set
            { trCampoValorTotal.Visible = value; }
        }
        public bool BloqueiaCampoObservacoes
        {
            get
            { return (txtObservacoes.Enabled); }

            set
            { txtObservacoes.Enabled = !value; }
        }

        public bool ExibeBotaoPesquisaDocumento
        {
            get
            { return (imgLupaPesquisaDocumento.Visible); }

            set
            { trCampoDataMovimento.Visible = value; }
        }
        public bool BloqueiaBotaoPesquisaDocumento
        {
            get
            { return (imgLupaPesquisaDocumento.Enabled); }

            set
            { trCampoDataMovimento.Disabled = value; }
        }

        public bool ExibeCampoDataMovimento
        {
            get
            { return (trCampoDataMovimento.Visible); }

            set
            { trCampoDataMovimento.Visible = value; }
        }
        public bool BloqueiaCampoDataMovimento
        {
            get
            { return (txtDataMovimento.Enabled); }

            set
            { txtDataMovimento.Enabled = !value; }
        }

        public bool ExibeCamposInfoTransferenciaDoacao
        {
            get
            { return (trCampoTransferenciaDoacao_ComboOrgao.Visible = trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible); }

            set
            { trCampoTransferenciaDoacao_ComboOrgao.Visible = trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = value; }
        }
        public bool BloqueiaCamposInfoTransferenciaDoacao
        {
            set { ddlAmoxarifado_TransferenciaDoacao.Enabled = ddlOrgao_TransferenciaDoacao.Enabled = !value; }
        }

        public bool ExibeBotaoImprimir
        {
            get
            { return (btnImprimir.Visible); }

            set
            { btnImprimir.Visible = value; }
        }
        public bool BloqueiaBotaoImprimir
        {
            get
            { return (btnImprimir.Enabled); }

            set
            { btnImprimir.Enabled = !value; }
        }

        public bool ExibeBotaoGravar
        {
            get
            { return (btnGravar.Visible); }

            set
            { btnGravar.Visible = value; }
        }
        public bool BloqueiaBotaoGravar
        {
            get
            { return (btnGravar.Enabled); }

            set
            { btnGravar.Enabled = !value; }
        }

        public void ColorirCelulaQtd(RepeaterItem item)
        {
            bool numerosFracionarios = false;
            bool pintaGrid = false;
            var fmtValorDecimal = "";
            decimal valorSaldoQtd = 0.0m;
            decimal valorQtdLiq = 0.0m;
            decimal valorQtdMov = 0.0m;
            decimal valorSaldoQtdDt = 0.0m;
            decimal valorcelMediaQtde = 0.0m;
            decimal valorTotal = 0.0m;

            HtmlTableCell celSaldoQtde = (HtmlTableCell)item.FindControl("celSaldoQtde");
            HtmlTableCell celQtdeLiq = (HtmlTableCell)item.FindControl("celQtdeLiq");
            HtmlTableCell celQtdeMov = (HtmlTableCell)item.FindControl("celQtdeMov");
            HiddenField hdfSaldoQtde = (HiddenField)item.FindControl("hdfSaldoQtde");
            HtmlTableCell celSaldoQtdeDt = (HtmlTableCell)item.FindControl("celSaldoQtdeDt");
            HtmlTableCell celMediaQtde = (HtmlTableCell)item.FindControl("celMediaQtde");

            //Se for Estorno, Relatório não pinta o grid e/ou não houver valores retornados pela base de dados
            pintaGrid = (rblTipoOperacao.SelectedValue == ((int)enumTipoRequisicao.Nova).ToString());

            if ((celSaldoQtde.IsNull() || celQtdeLiq.IsNull() || celQtdeMov.IsNull() || celSaldoQtdeDt.IsNull() || celMediaQtde.IsNull()))
            {
                return;
            }
            else if (celSaldoQtde.IsNotNull() || celQtdeLiq.IsNotNull() || celQtdeMov.IsNotNull() || celSaldoQtdeDt.IsNotNull() || celMediaQtde.IsNotNull())
            {
                valorSaldoQtd = (helperTratamento.TryParseDecimal(celSaldoQtde.InnerText.Trim(), true) ?? 0.0m);
                valorQtdLiq = (helperTratamento.TryParseDecimal(celQtdeLiq.InnerText.Trim(), true) ?? 0.0m);
                valorQtdMov = (helperTratamento.TryParseDecimal(celQtdeMov.InnerText.Trim(), true) ?? 0.0m);
                valorSaldoQtdDt = (helperTratamento.TryParseDecimal(celSaldoQtdeDt.InnerText.Trim(), true) ?? 0.0m);
                valorcelMediaQtde = (helperTratamento.TryParseDecimal(celMediaQtde.InnerText.Trim(), true) ?? 0.0m);

                numerosFracionarios = (valorSaldoQtd.PossuiValorFracionario() || valorQtdLiq.PossuiValorFracionario() || valorQtdMov.PossuiValorFracionario() || (base.numCasasDecimaisMaterialQtde == 3)
                    || valorSaldoQtdDt.PossuiValorFracionario() || valorcelMediaQtde.PossuiValorFracionario());
                fmtValorDecimal = (numerosFracionarios) ? "#,##0.000" : base.fmtFracionarioMaterialQtde;

                ReformatarValoresExibidosControles(fmtValorDecimal, celSaldoQtde, celQtdeLiq, celQtdeMov, celSaldoQtdeDt, celMediaQtde);

                if (!string.IsNullOrEmpty(txtValorTotal.Text))
                    valorTotal = Convert.ToDecimal(txtValorTotal.Text);

                valorTotal += valorQtdMov;

                txtValorTotal.Text = valorTotal.ToString();
            }

            if (pintaGrid)
            {
                string verde = "#66FF66";
                string amarelo = "#FFF68F";
                string vermelho = "#FF6347";
                string azul = "#00FFFF";

                //Se o saldo quantidade estiver zerado, não poderá atender a saida
                if (valorSaldoQtd == 0 || valorSaldoQtdDt == 0)
                {
                    celQtdeMov.BgColor = vermelho;
                    celQtdeMov.InnerText = "";
                }
                else
                {
                    if (valorQtdMov == 0 || string.IsNullOrEmpty(valorQtdMov.ToString()))
                    {
                        celQtdeMov.BgColor = azul;
                        celQtdeMov.InnerText = "";
                    }
                    else
                    {
                        //Se o saldo estiver positivo e atender 100% do subitem
                        if ((valorQtdLiq <= valorSaldoQtd) || ((valorQtdLiq == 0 || valorQtdLiq == 0) && valorQtdMov > 0))
                            celQtdeMov.BgColor = verde;
                        //Item atendido parcialmente
                        else if ((valorQtdLiq > valorSaldoQtd && valorQtdLiq > 0))
                        {
                            celQtdeMov.BgColor = amarelo;
                            //celQtdeMov.InnerText = valorSaldoQtd.ToString();
                        }
                        else
                        {
                            celQtdeMov.BgColor = vermelho;
                            celQtdeMov.InnerText = "";
                        }
                    }
                }
            }
        }

        public bool MovimentoItemNull()
        {
            bool isNUll = false;

            if (movimento == null)
            {
                isNUll = true;
            }
            else
            {
                if (movimento.MovimentoItem == null)
                {
                    isNUll = true;
                }
            }
            return isNUll;
        }

        private void MudarTextoBotaoEditar(bool editar)
        {
            if (editar)
                btnAdd.Text = "Editar";
            else
                btnAdd.Text = "Adicionar";
        }
        //aqui
        public IList<MovimentoItemEntity> ListarMovimentoItens(int startRowIndexParameterName, int maximumRowsParameterName, string _documento, string dataMovimento)
        {
            Session.Add("numeroDocumento", _documento);

            InstanciaObjetoMovimento();
            AtualizarSessao();

            if (MovimentoItemNull())
            {
                movimento.Id = helperTratamento.TryParseInt32(this.Id);
                movimento.NumeroDocumento = _documento;
                if (Session["rblTipoMovimento"] != null)
                    movimento.TipoMovimento.Id = Convert.ToInt32(Session["rblTipoMovimento"]);

                if (!string.IsNullOrEmpty(dataMovimento))
                    movimento.DataMovimento = helperTratamento.TryParseDateTime(dataMovimento);

                SaidaMaterialPresenter mat = new SaidaMaterialPresenter(this);
                movimento = mat.ListarMovimentoItensTodos(startRowIndexParameterName, maximumRowsParameterName, movimento, Session["tipoRequisicao"] == null ? 0 : Convert.ToInt32(Session["tipoRequisicao"]));              
                if (movimento != null)
                {
                    Session["Bloquear"] = movimento.Bloquear = movimento.Bloquear == null ? false : movimento.Bloquear;

                    if (movimento.DataDocumento != null)
                        Session["txtDataMovimento"] = movimento.DataMovimento;

                    if (movimento.Divisao != null)
                        Session["DivisaoId"] = movimento.Divisao.Id;


                }

                SetSession<MovimentoEntity>(movimento, sessaoMov);
           
            }


            if (!MovimentoItemNull())
            {
                return SaidaMaterialPresenter.FormataPosicaoMovimentoItensLote(movimento).MovimentoItem;
            }
            else
            {
                return null;
            }
        }
        //Trava SAP
        private bool PermitirSaidaRequisicao(MovimentoEntity mov)
        {
            bool permitir = true;
            if (movimento.TipoMovimento.Id == ((int)enumTipoMovimento.RequisicaoPendente))
            {
                var mesRef = this.AnoMesReferencia;
                var orgao = new PageBase().GetAcesso.Transacoes.Perfis[0].OrgaoPadrao.Codigo;
                SaidaMaterialPresenter mat = new SaidaMaterialPresenter(this);
                //   SetSession<MovimentoEntity>(movimento, sessaoMov);

                if (mesRef == "202005" && orgao == 38)
                {
                    var retorno = mat.ListarMovimentoItensTodos(0, 1, movimento, Session["tipoRequisicao"] == null ? 0 : Convert.ToInt32(Session["tipoRequisicao"]));
                    if (retorno != null)
                    {
                        List<String> erro = new List<string>();

                        foreach (var item in retorno.MovimentoItem)
                        {
                            string natureza = Convert.ToString(item.NaturezaDespesaCodigo);
                            var res = natureza.Substring(0, 4);
                            if (res == "4490")
                            {
                                permitir = false;
                                break;
                            }
                        }
                    }

                }
            }
            return permitir;
        }
        public void ListarMovimentoItensRascunho()
        {
            rptSubItem.DataSourceID = "sourceListaGridSaidaMaterial";
        }

        public bool ExibeBotaoAdicionar
        {
            get
            { return (btnAdd.Visible); }

            set
            { btnAdd.Visible = value; }
        }
        private void BloqueiaBotaoAdicionar()
        {
            bool returno = true;

            if (ddlUGE.Items.Count < 1)
                returno = false;

            if (ddlLote.Items.Count < 1)
                returno = false;

            btnAdd.Enabled = returno;
        }

        private void AdicionarItem(int i)
        {
            bool tipoMaterialSubitemDivergenteTipoMaterialMovimentacao = false;
            bool ehSaidaPorTransferencia = false;
            bool excedeNumeroMaximoSubitens = false;
            SaidaMaterialPresenter presenter = new SaidaMaterialPresenter(this);

            AtualizarSessao();

            if (movimento != null)
            {
                var listaPTRes = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTRes);
                var listaPTResAcao = PageBase.GetSession<IList<PTResEntity>>(cstListagemPTResAcao);

                var numCasasDecimais = 0;
                if ((helperTratamento.TryParseDecimal(this.txtSaldo.Text, true).Value.PossuiValorFracionario()) ||
                   (helperTratamento.TryParseDecimal(this.txtQtdSolicitada.Text, true).Value.PossuiValorFracionario()))
                { numCasasDecimais = 3; }

                movimento.Id = helperTratamento.TryParseInt32(this.Id);

                MovimentoItemEntity movimentoItem;
                var catBusiness = new CatalogoBusiness();
                movimentoItem = new MovimentoItemEntity();
                catBusiness.SelectSubItemMaterial(this.SubItemMaterialId.Value);
                movimentoItem.SubItemMaterial = catBusiness.SubItemMaterial;
                movimentoItem.ItemMaterial = catBusiness.ItemMaterial;
                movimentoItem.UGE = new UGEEntity() { Id = UgeId };
                movimentoItem.Id = this.MovimentoItemId;
                movimentoItem.SubItemMaterial.SomaSaldoLote = new SaldoSubItemEntity() { UGE = new UGEEntity() { Id = UgeId, Descricao = UgeDesc } };
                movimentoItem.UGE.Descricao = ddlUGE.SelectedItem.Text;
                this.Unidade = movimentoItem.SubItemMaterial.UnidadeFornecimento.Codigo;
                this.SubItemMaterialCodigo = movimentoItem.SubItemMaterial.Codigo;
                this.SubItemMaterialDescricao = movimentoItem.SubItemMaterial.Descricao;
                movimentoItem.SubItemMaterial.CodigoFormatado = movimentoItem.SubItemMaterial.Codigo.ToString();
                movimentoItem.Visualizar = true;

                if (rptLote.Items.Count > 1)
                {
                    Label lblDescricaoLote = (Label)rptLote.Items[i].FindControl("lblDescricaoLote");
                    HiddenField Saldo = (HiddenField)rptLote.Items[i].FindControl("hdFSaldo");
                    movimentoItem.CodigoDescricao = lblDescricaoLote.Text;
                    txtSaldo.Text = Saldo.Value;
                }
                else
                {
                    movimentoItem.CodigoDescricao = ddlLote.SelectedItem.Text;
                }

                movimentoItem.SaldoQtde = helperTratamento.TryParseDecimal(this.txtSaldo.Text, true).Value.Truncar(numCasasDecimais, true);
                movimentoItem.QtdeLiq = helperTratamento.TryParseDecimal(this.txtQtdSolicitada.Text, true).Value.Truncar(numCasasDecimais, true);
                movimentoItem.SubItemMaterial.SomaSaldoLote.SaldoQtde = helperTratamento.TryParseDecimal(this.txtSaldo.Text, true).Value.Truncar(numCasasDecimais, true);

                var _tryParsePTResAcao = -1;
                var isRequisicao = (this.TipoMovimento == (int)enumTipoMovimento.RequisicaoPendente ||
                                    this.TipoMovimento == (int)enumTipoMovimento.RequisicaoAprovada ||
                                    this.TipoMovimento == (int)enumTipoMovimento.RequisicaoFinalizada);

                var _existeValorPTRes = (this.PTResCodigo.IsNotNull() && (this.PTResCodigo.Value > 0));
                var _existeValorAcaoPTRes = ((Int32.TryParse(this.PTResAcao, out _tryParsePTResAcao) && _tryParsePTResAcao != 0));

                if (isRequisicao)
                {
                    if (_existeValorPTRes && _existeValorAcaoPTRes)
                    {
                        if (listaPTRes.IsNotNullAndNotEmpty())
                        {
                            //var _ptres = listaPTRes.Where(ptres => ptres.Codigo == this.PTResCodigo).FirstOrDefault();
                            var _ptres = listaPTRes.Where(ptres => ptres.Codigo == this.PTResCodigo && ptres.ProgramaTrabalho.ProjetoAtividade == this.PTResAcao).FirstOrDefault();
                            if (_existeValorPTRes && _ptres.IsNotNull())
                            {
                                movimentoItem.PTRes = _ptres;

                                this.PTResDescricao = _ptres.Descricao;
                                this.PTResCodigo = _ptres.Codigo;
                                this.PTResAcao = _ptres.ProgramaTrabalho.ProjetoAtividade;
                                this.PTResId = _ptres.Id;
                                this.PTAssociadoPTRes = _ptres.CodigoPT;
                            }
                            else
                            {
                                LimparCamposPTRes();
                            }
                        }
                    }
                    else
                    {
                        ExibirMensagem("Selecionar valor válido para campo PTRes / PTRes Ação!");
                        return;
                    }
                }

                movimentoItem.Ativo = true;

                // procura o preço unitário ao inserir um novo item
                if (movimento.MovimentoItem != null)
                {
                    var item = movimento.MovimentoItem.Where(a => a.Id == Convert.ToInt32(movimentoItem.Id)).FirstOrDefault();

                    if (item != null)
                    {
                        if (item.SubItemMaterial.SaldoSubItems != null)
                        {
                            if (item.SubItemMaterial.SaldoSubItems.Count() > 0)
                                movimentoItem.PrecoUnit = presenter.getPrecoUnitSubItem(movimentoItem.SubItemMaterial.Id, new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id, movimentoItem.UGE.Id, movimentoItem.IdentificacaoLote, movimentoItem.FabricanteLote, movimentoItem.DataVencimentoLote).Value.Truncar(base.numCasasDecimaisValorUnitario, true);
                            //movimentoItem.PrecoUnit = item.SubItemMaterial.SaldoSubItems[0].PrecoUnit.Value.Truncar(base.numCasasDecimaisValorUnitario, true);
                        }
                    }
                }

                //Caso for um novo movimento, pega o Almoxarifado id da sessão
                if (movimento.Almoxarifado.Id == null)
                {
                    movimento.Almoxarifado.Id = new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                }

                //efetuado a cada insercao em sessao, da movimentacao de saida.
                #region Recalculo
                //{
                //Saldo total menos reserva
                movimentoItem.SubItemMaterial.SomaSaldoTotal = 0;
                movimentoItem.SubItemMaterial.SomaSaldoTotal += presenter.CalculaTotalSaldoUGEsReserva(movimentoItem.SubItemMaterial.Id, movimento.Almoxarifado.Id).Value.Truncar(numCasasDecimais, true);

                Tuple<decimal?, decimal?, decimal?> retornoSaldo = presenter.SaldoMovimentoItemDataMovimento(movimentoItem.SubItemMaterial.Id, movimento.Almoxarifado.Id, UgeId, Convert.ToDateTime(Session["txtDataMovimento"]));
                movimentoItem.SubItemMaterial.SomaSaldoTotalDataMovimento = retornoSaldo.Item1;
                movimentoItem.SubItemMaterial.SomaSaldoTotalDataMovimento = movimentoItem.SubItemMaterial.SomaSaldoTotal < movimentoItem.SubItemMaterial.SomaSaldoTotalDataMovimento ? movimentoItem.SubItemMaterial.SomaSaldoTotal : movimentoItem.SubItemMaterial.SomaSaldoTotalDataMovimento;

                if (movimentoItem.PrecoUnitDtMov == null && movimentoItem.SaldoValor == null)
                {
                    movimentoItem.SaldoValor = retornoSaldo.Item2;
                    movimentoItem.PrecoUnitDtMov = retornoSaldo.Item3;
                }


                var resultado = GetSession<IList<SaldoSubItemEntity>>(sessaoLote);

                if (resultado != null)
                {
                    if (resultado.Count > 1)
                    {
                        if (i >= 0)
                        {
                            if (resultado[i].LoteDataVenc != null)
                                movimentoItem.DataVencimentoLote = resultado[i].LoteDataVenc;

                            if (resultado[i].LoteFabr != null)
                                movimentoItem.FabricanteLote = resultado[i].LoteFabr;

                            if (resultado[i].LoteIdent != null)
                                movimentoItem.IdentificacaoLote = resultado[i].LoteIdent;

                            if (resultado[i].IdLote != null)
                                movimentoItem.IdLote = resultado[i].IdLote;
                        }
                        else
                        {
                            if (resultado[ddlLote.SelectedIndex].LoteDataVenc != null)
                                movimentoItem.DataVencimentoLote = resultado[ddlLote.SelectedIndex].LoteDataVenc;

                            if (resultado[ddlLote.SelectedIndex].LoteFabr != null)
                                movimentoItem.FabricanteLote = resultado[ddlLote.SelectedIndex].LoteFabr;

                            if (resultado[ddlLote.SelectedIndex].LoteIdent != null)
                                movimentoItem.IdentificacaoLote = resultado[ddlLote.SelectedIndex].LoteIdent;

                            if (resultado[ddlLote.SelectedIndex].IdLote != null)
                                movimentoItem.IdLote = resultado[ddlLote.SelectedIndex].IdLote;
                        }
                    }
                    else if (resultado.Count == 1)
                    {
                        if (resultado[ddlLote.SelectedIndex].LoteDataVenc != null)
                            movimentoItem.DataVencimentoLote = resultado[ddlLote.SelectedIndex].LoteDataVenc;

                        if (resultado[ddlLote.SelectedIndex].LoteFabr != null)
                            movimentoItem.FabricanteLote = resultado[ddlLote.SelectedIndex].LoteFabr;

                        if (resultado[ddlLote.SelectedIndex].LoteIdent != null)
                            movimentoItem.IdentificacaoLote = resultado[ddlLote.SelectedIndex].LoteIdent;

                        if (resultado[ddlLote.SelectedIndex].IdLote != null)
                            movimentoItem.IdLote = resultado[ddlLote.SelectedIndex].IdLote;
                    }
                }

                //}
                #endregion

                if (!movimentoItem.PrecoUnit.HasValue)
                    movimentoItem.PrecoUnit = presenter.getPrecoUnitSubItem(movimentoItem.SubItemMaterial.Id, new PageBase().GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id, movimentoItem.UGE.Id, movimentoItem.IdentificacaoLote, movimentoItem.FabricanteLote, movimentoItem.DataVencimentoLote).Value.Truncar(base.numCasasDecimaisValorUnitario, true);

                if (!String.IsNullOrEmpty(this.txtQtdeMov.Text))
                {
                    TextBox txtQtdeLote = new TextBox();
                    if (rptLote.Items.Count > 0)
                    {
                        if (btnAdd.Text != "Editar" && rptLote.Items.Count > 1)
                        {
                            txtQtdeLote.Text = ((TextBox)rptLote.Items[i].FindControl("txtQtdeLote")).Text;
                            txtQtdeMov.Text = txtQtdeLote.Text;
                        }
                        else
                        {
                            if (((TipoMovimento == (int)enumTipoMovimento.RequisicaoAprovada) ||
                               (TipoMovimento == (int)enumTipoMovimento.RequisicaoCancelada) ||
                               (TipoMovimento == (int)enumTipoMovimento.RequisicaoFinalizada) ||
                              (TipoMovimento == (int)enumTipoMovimento.RequisicaoPendente)) &&
                                  rptLote.Items.Count > 1)
                            {
                                txtQtdeLote.Text = ((TextBox)rptLote.Items[i].FindControl("txtQtdeLote")).Text;
                                txtQtdeMov.Text = txtQtdeLote.Text;
                            }
                        }
                    }

                    movimentoItem.QtdeMov = helperTratamento.TryParseDecimal(this.txtQtdeMov.Text, true).Value.Truncar(base.numCasasDecimaisMaterialQtde, true);
                    movimentoItem.ValorMov = (movimentoItem.QtdeMov ?? 0.0m).Truncar(base.numCasasDecimaisValorUnitario, true) * (movimentoItem.PrecoUnit ?? 0.0m).Truncar(base.numCasasDecimaisValorUnitario, true);
                }
                else
                    movimentoItem.QtdeMov = null;

                if (ValidarMovimentoItem(movimentoItem))
                {
                    #region Trava por TipoMaterial
                    tipoMaterialSubitemDivergenteTipoMaterialMovimentacao = presenter.VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(movimento, movimentoItem);
                    if (tipoMaterialSubitemDivergenteTipoMaterialMovimentacao)
                    {
                        var tipoMaterialMovimentacao = movimento.ObterTipoMaterial();
                        var msgErro = String.Format("Movimentação de saida permitirá apenas inclusão de subitens de material do tipo {0}", EnumUtils.GetEnumDescription<TipoMaterialParaLancamento>(tipoMaterialMovimentacao));

                        PageBase.SetSession<bool>(tipoMaterialSubitemDivergenteTipoMaterialMovimentacao, ChaveSessao_CampoTipoMaterialDivergente_LancamentoSIAFEM);
                        ExibirMensagem(msgErro);
                    }
                    #endregion Trava por TipoMaterial

                    movimentoItem.ItemValidado = true;
                    if (movimento.MovimentoItem == null)
                    {
                        List<MovimentoItemEntity> movimentoItemList = new List<MovimentoItemEntity>();
                        movimentoItemList.Add(movimentoItem);
                        movimento.MovimentoItem = movimentoItemList;
                    }
                    else
                    {
                        //TODO: Verificar com o Celso este trecho muito louco. =/
                        if (this.MovimentoItemId != null)
                        {
                            movimento.MovimentoItem = (from m in movimento.MovimentoItem
                                                       where m.Id != this.MovimentoItemId
                                                       select m).ToList();
                        }

                        //if (!tipoMaterialSubitemDivergenteTipoMaterialMovimentacao)
                        ehSaidaPorTransferencia = (TipoMovimento == (int)tipoMovimentacao.SaidaPorTransferencia);
                        excedeNumeroMaximoSubitens = (movimento.MovimentoItem.HasElements() && movimento.MovimentoItem.DistinctBy(movItem => movItem.SubItemCodigo.Value).Count() >= Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO);
                        if (!(ehSaidaPorTransferencia && excedeNumeroMaximoSubitens) && !tipoMaterialSubitemDivergenteTipoMaterialMovimentacao)
                            movimento.MovimentoItem.Add(movimentoItem);
                        else if (ehSaidaPorTransferencia && excedeNumeroMaximoSubitens)
                            this.ExibirMensagem(String.Format("Ação não permitida, por exceder número máximo de subitens por movimentação ({0}).", Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                    }

                    this.MovimentoItemId = null;


                    SetSession<MovimentoEntity>(movimento, sessaoMov);
                    ListarMovimentoItensRascunho();

                    if (ViewState["botao"] != null)
                    {
                        if (ViewState["botao"].ToString() != "btnAdd")

                            presenter.AdicionadoSucesso();
                    }
                }
            }
        }

        //public static bool ValidarMovimentoItem(MovimentoItemEntity movimentoItem, ISaidaMaterialView IPaginaSaida)
        public bool ValidarMovimentoItem(MovimentoItemEntity movimentoItem)
        {
            List<string> lstErros = new List<string>();

            if (movimentoItem.QtdeMov == 0.000m)
                lstErros.Add(String.Format("Favor informar a quantidade fornecida, do subItem {0} - {1}.", movimentoItem.SubItemCodigoFormatado, movimentoItem.SubItemDescricao));

            if (movimentoItem.QtdeMov < 0.001m)
                lstErros.Add(String.Format("Quantidade de itens, deve ser maior ou igual a 0,001 unidade, do subItem {0} - {1}.", movimentoItem.SubItemCodigoFormatado, movimentoItem.SubItemDescricao));

            if (movimentoItem.QtdeMov > movimentoItem.SubItemMaterial.SomaSaldoTotal)
                lstErros.Add(String.Format("Saldo Insuficiente para o subitem {0} - {1}.", movimentoItem.SubItemCodigoFormatado, movimentoItem.SubItemDescricao));

            if (movimentoItem.PrecoUnit < 0.0001m)
                lstErros.Add(String.Format("Preço Unitário para o subitem {0} - {1} deve ser maior que R$ 0,0001.", movimentoItem.SubItemCodigoFormatado, movimentoItem.SubItemDescricao));

            if (movimentoItem.QtdeMov > 0.000m && movimentoItem.ValorMov < 0.01m)
                lstErros.Add(String.Format("Valor de Nota de Fornecimento para o subitem {0} - {1} não pode ser igual a R$ 0,00.", movimentoItem.SubItemCodigoFormatado, movimentoItem.SubItemDescricao));

            this.ListaErros = lstErros;

            return (lstErros.Count == 0);
        }

        public void GravadoSucessoAtualizar()
        {
            RemoveSession(sessaoMov);
            ListarMovimentoItensRascunho();
        }

        private string RecuperaSaldo(bool soma)
        {
            string saldo = "0";

            if (ddlLote.Items.Count == 0)
                return saldo;

            var resultado = GetSession<IList<SaldoSubItemEntity>>(sessaoLote);

            if (resultado != null)
            {
                if (resultado.Count > 0)
                {
                    decimal _saldoTemp = 0;
                    if (resultado.Count > 1 && soma)
                        _saldoTemp = resultado.Sum(x => x.SaldoQtde).Value;
                    else
                        _saldoTemp = resultado[ddlLote.SelectedIndex].SaldoQtde.Value;

                    if (_saldoTemp.PossuiValorFracionario() || base.numCasasDecimaisMaterialQtde == 3)
                        saldo = _saldoTemp.ToString(base.fmtFracionarioMaterialQtde);
                }
            }

            return saldo;
        }

        protected int NumeroCasasDecimais_QtdeItem()
        {
            return base.numCasasDecimaisMaterialQtde;
        }

        protected int NumeroCasasDecimais_ValorUnitario()
        {
            return base.numCasasDecimaisValorUnitario;
        }

        protected void btnCarregarSubItem_Click(object sender, EventArgs e)
        {
            CarregarSubItensPorCodigo();
        }

        private void CarregarSubItensPorCodigo()
        {
            long? codigoSubItem = Common.Util.TratamentoDados.TryParseLong(txtSubItem.Text.Trim());
            SubItemMaterialPresenter presenter = new SubItemMaterialPresenter();
            var gestorId = GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            var subItem = presenter.CarregarSubItem(codigoSubItem, gestorId);

            if (subItem != null)
            {
                SubItemMaterialDescricao = subItem.Descricao;
                SubItemMaterialId = subItem.Id;
                Unidade = subItem.UnidadeFornecimento.Descricao;
                ItemMaterialCodigo = subItem.ItemMaterial.Codigo;
            }
            else
            {
                List<String> erro = new List<string>();
                erro.Add("Subitem não encontrado");
                this.ListaErros = erro;

                SubItemMaterialDescricao = string.Empty;
                SubItemMaterialId = null;
                Unidade = string.Empty;
                ItemMaterialCodigo = null;
            }
        }

        private void ReformatarValoresExibidosControles(string strFormatoNumerico, params HtmlTableCell[] arrControlesWeb)
        {
            foreach (var objHtmlExibidorDeTexto in arrControlesWeb)
            {
                if (objHtmlExibidorDeTexto.GetType().BaseType == typeof(HtmlContainerControl))
                    ((HtmlTableCell)objHtmlExibidorDeTexto).ReformatarValorNumerico(strFormatoNumerico);
            }
        }

        protected void txtDataMovimento_TextChanged(object sender, EventArgs e)
        {
            Session["txtDataMovimento"] = txtDataMovimento.Text;
            Session["_numeroDocumento"] = txtRequisicao.Text;
            // rptSubItem.Visible = false;
            txtRequisicao.Text = string.Empty;

            if (!String.IsNullOrWhiteSpace(Session["_numeroDocumento"].ToString()))
            {
                txtRequisicao.Text = Session["_numeroDocumento"].ToString();
                DesabilitarDivisao(true);
                Session["_numeroDocumento"] = null;
            }
        }

        private void InicializarDropDownList(DropDownList comboLista)
        {
            comboLista.DataSourceID = string.Empty;
            comboLista.InserirElementoSelecione(true);
        }

        #endregion Metodos

        #region Acesso SIAF

        protected void efetuarAcessoSIAFEM()
        {
            var opcaoNotaFornecimentoSelecionada = (TipoOperacao == (int)tipoOperacao.NotaFornecimento);
            if (!opcaoNotaFornecimentoSelecionada)
            {
                WebUtil webUtil = new WebUtil();
                if (String.IsNullOrWhiteSpace(GetSession<string>("senhaWsSiafem")))
                {
                    webUtil.runJScript(this, "OpenModalSenhaWs();");
                    return;
                }

                if (!String.IsNullOrWhiteSpace(GetSession<string>("loginWsSiafem")))
                    loginSiafem = GetSession<string>("loginWsSiafem");

                if (!String.IsNullOrWhiteSpace(GetSession<string>("senhaWsSiafem")))
                    senhaSiafem = GetSession<string>("senhaWsSiafem");
                else
                {
                    ExibirMensagem("Senha em branco não-permitida.");
                    SetSession<string>(null, "senhaWsSiafem");
                }
            }

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }

        protected void RecuperarDadosSessao()
        {
            this.loginSiafem = GetSession<string>("loginWsSiafem");
            this.senhaSiafem = GetSession<string>("senhaWsSiafem");

            this.dadosAcessoSiafemPreenchidos = (!String.IsNullOrWhiteSpace(this.loginSiafem) && !String.IsNullOrWhiteSpace(this.senhaSiafem));
        }

        public void ExecutaEvento()
        {
            RecuperarDadosSessao();
        }

        public void ExecutaEventoCancelar()
        {
            this.loginSiafem = this.senhaSiafem = null;
        }

        #endregion Acesso SIAF

        protected void imgLupaSubItem_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void txtUge_TextChanged(object sender, EventArgs e)
        {

        }

        protected void ddlAmoxarifado_TransferenciaDoacao_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (ddlAmoxarifado_TransferenciaDoacao.SelectedItem.Text == "Sem almoxarifado")
                trtxtAvulso.Visible = true;
            else
                trtxtAvulso.Visible = false;
        }

        protected void ddlSubTipoMovimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAmoxarifado_TransferenciaDoacao.Items.Count > 0)
                ddlAmoxarifado_TransferenciaDoacao.SelectedIndex = 0;

            AtivarDetalhe();
        }

        private void AtivarDetalhe()
        {
            if (ViewState["SubTipo"] != null)
            {
                List<SubTipoMovimentoEntity> subtipo = new List<SubTipoMovimentoEntity>();
                subtipo = (List<SubTipoMovimentoEntity>)ViewState["SubTipo"];
                trtxtAvulso.Visible = false;

                foreach (var item in subtipo)
                {
                    if (Convert.ToInt32(ddlSubTipoMovimento.SelectedValue) == item.Id)
                    {
                        //lblTextoAvulso.Text = "UGE/CPF/CNPJ:";
                        var isTransferencia = ((TipoMovimento == (int)tipoMovimentacao.SaidaPorTransferencia) || (TipoMovimento == (int)tipoMovimentacao.SaidaPorDoacao));
                        trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = isTransferencia;
                        ExibeCamposInfoTransferenciaDoacao = isTransferencia;

                        if (item.ListEventoSiafem.FirstOrDefault().DetalheAtivo)
                        {
                            trCampoTransferenciaDoacao_ComboAlmoxarifado.Visible = false;
                            ExibeCamposInfoTransferenciaDoacao = false;
                            trtxtAvulso.Visible = item.ListEventoSiafem.FirstOrDefault().DetalheAtivo;
                            txtDescricaoAvulsa.Enabled = true;
                        }

                        hdfNaturaSelecionada.Value = string.Empty;
                        foreach (var itemtipo in item.ListEventoSiafem)
                        {
                            hdfNaturaSelecionada.Value += itemtipo.EventoTipoMaterial + "/";
                        }

                        break;
                    }


                }

            }
            else
                idSubTipo.Visible = false;
        }

        private void DesabilitarDivisao(bool campo)
        {

            if (campo)
            {
                ddlDivisao.Enabled = false;
            }
            else
            { ddlDivisao.Enabled = true; }

        }

    }
}
