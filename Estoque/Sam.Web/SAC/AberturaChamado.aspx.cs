using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Common.Enums.ChamadoSuporteEnums;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using tipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using statusAtendimentoChamado = Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado;
using tipoPerfil = Sam.Common.Util.GeralEnum.TipoPerfil;
using System.Web.UI.HtmlControls;
using Sam.Common.Enums;
using Sam.Common;
using Sam.Entity;
using Sam.Business;
using System.Drawing;

namespace Sam.Web.SAC
{
    /// <summary>
    /// Página criada para controle de chamados de suporte a usuário.
    /// </summary>
    public partial class AberturaChamado : PageBase, IChamadoSuporteView, ICombosPadroesView
    {
        private const string CHECKBOXATUALIZARSTATUS = "chkAtualizarStatusLote";
        private const string NOMECOLUNALOTE = "Lote";

        private string sessionChamadosSuporte = "sessionChamadosSuporte";
        private string cstListaChamadosSuporte = "listaChamadosSuporte";

        private List<GridViewRow> _itensSelecionados = new List<GridViewRow>();

        private const string ALMOXARIFADO = "Almoxarifado";
        private const string ALMOXARIFADOID = "AlmoxarifadoID";

        protected void Page_Load(object sender, EventArgs e)
        {
            ChamadoSuportePresenter objPresenter = null;

            this.combosEstruturaOrganizacionalPadrao.View = this;
            this.combosEstruturaOrganizacionalPadrao.InserirOpcaoPesquisarTodos = true;
            this.combosEstruturaOrganizacionalPadrao.PreservarComboboxValues = false;
            this.combosEstruturaOrganizacionalPadrao.InsereComboStatusProdesp();
            this.combosEstruturaOrganizacionalPadrao.InsereCampoDePesquisa(btnPesquisar_Click, "N.º Chamado");
            this.combosEstruturaOrganizacionalPadrao.InsereComboAlmoxarifados();
            this.combosEstruturaOrganizacionalPadrao.ExecutadaPorChamadoSuporte = true;

            this.CascatearDDLOrgao = true;
            this.CascatearDDLUO = true;
            this.CascatearDDLUGE = true;
            this.CascatearDDLUA = true;
            this.CascatearDDLAlmoxarifado = true;

            if (!IsPostBack)
            {
                if (Page.Session["orgaoId"].IsNotNull())
                    CarregarViewComDDLSelectedValues();

                initTela();
                initOpcoesStatusAtendimentoUsuario();

                MostrarBotaoStatusAtualizar = false;
                objPresenter = new ChamadoSuportePresenter(this);
                objPresenter.Load();
            }

            MostrarPainelEdicao = false;
            MostrarPainelStatusAtualizar = false;

            if (IsUploadingFile)
            {
                MostrarPainelEdicao = true;
                IsUploadingFile = false;
            }

            gdvAtendimentoChamados.PageSize = 20;
            RegistraJavascript();
        }

        private void RegistraJavascript()
        {
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnDownloadAnexos);
        }

        #region IEntidadesAuxiliaresView Members

        private int _orgaoID
        {
            get
            {
                var perfilLogado = GetAcesso;
                int? _orgaoID = (perfilLogado.IsNotNull() && perfilLogado.Estruturas.Orgao.IsNotNullAndNotEmpty()) ? perfilLogado.Estruturas.Orgao[0].Id : null;

                return _orgaoID.HasValue ? _orgaoID.Value : -1;
            }
            set { }
        }

        private string cstSessionKey_ListaAnexos = "listaAnexos";
        public int gestorID;
        public int ugeID;

        public IList ListaErros
        {
            set
            {
                this.ListInconsistencias.ExibirLista(value);
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
                //paramList.Add("CodigoOrgao", ddlOrgao.SelectedValue.ToString());
                //paramList.Add("DescricaoOrgao", this.ddlOrgao.SelectedItem.Text);
                //paramList.Add("CodigoGestor", ddlGestor.SelectedValue.ToString());
                //paramList.Add("DescricaoGestor", this.ddlGestor.SelectedItem.Text);
                paramList.Add("CodigoOrgao", combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedValue.ToString());
                paramList.Add("DescricaoOrgao", combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedItem.Text);
                paramList.Add("CodigoGestor", combosEstruturaOrganizacionalPadrao.DdlUO.SelectedValue.ToString());
                paramList.Add("DescricaoGestor", combosEstruturaOrganizacionalPadrao.DdlUO.SelectedItem.Text);

                return paramList;
            }
        }

        public RelatorioEntity DadosRelatorio { get; set; }

        public bool IsUploadingFile
        {
            get { return PageBase.GetSession<bool>("isUploadingFiles"); }
            set { PageBase.SetSession<bool>(value, "isUploadingFiles"); }
        }

        public DateTime DataAberturaChamado
        {
            get
            {
                DateTime _dataHora;

                DateTime.TryParse(txtDataAberturaChamado.Text, out _dataHora);

                return _dataHora;
            }

            set { txtDataAberturaChamado.Text = value.ToString("dd/MM/yyyy HH:mm"); }
        }

        public DateTime? DataFechamentoChamado
        {
            get
            {
                DateTime _dataHora = new DateTime(0);

                if (!String.IsNullOrWhiteSpace(txtDataFechamentoChamado.Text))
                    DateTime.TryParse(txtDataFechamentoChamado.Text, out _dataHora);

                return _dataHora;
            }

            set
            {
                string strDataHora = null;

                if (value.HasValue)
                    strDataHora = value.Value.ToString("dd/MM/yyyy HH:mm");
                else
                    strDataHora = string.Empty;

                txtDataFechamentoChamado.Text = strDataHora;
            }
        }

        public int NumeroChamado
        {
            get
            {
                int _numeroChamado = 0;

                Int32.TryParse(txtNumeroChamado.Text, out _numeroChamado);
                return _numeroChamado;
            }

            set { txtNumeroChamado.Text = value.ToString("D7"); }
        }

        public string NomeReduzidoCliente
        {
            get { return txtNomeReduzidoCliente.Text; }
            set { txtNomeReduzidoCliente.Text = txtNomeReduzidoCliente.ToolTip = value; }
        }
        public string NomeUsuario
        {
            get { return txtNomeUsuario.Text; }
            set { txtNomeUsuario.Text = value; }
        }
        public string EMailUsuario
        {
            get { return txtEMailUsuario.Text; }
            set { txtEMailUsuario.Text = value; }
        }
        public long CpfUsuario
        {
            get
            {
                long cpfUsuario = -1;
                string strCpfUsuario = (!String.IsNullOrWhiteSpace(txtCpfUsuario.Text) ? txtCpfUsuario.Text : null);
                Int64.TryParse(strCpfUsuario, out cpfUsuario);

                return cpfUsuario;
            }
            set
            {
                txtCpfUsuario.Text = value.ToString();
            }
        }

        public long CpfUsuarioLogado
        {
            get
            {
                long cpfUsuarioLogado = -1;
                Int64.TryParse(this.GetAcesso.Transacoes.Usuario.Cpf, out cpfUsuarioLogado);
                return cpfUsuarioLogado;
            }
        }

        public byte PerfilUsuarioAberturaChamadoID
        {
            get
            {
                byte _retorno = 0;

                byte.TryParse(perfilUsuarioAberturaChamado.Text, out _retorno);

                return _retorno;
            }

            set
            {
                perfilUsuarioAberturaChamado.Text = value.ToString();
            }
        }

        public AlmoxarifadoEntity Almoxarifado
        {
            get
            {
                AlmoxarifadoEntity objEntidade = null;

                if (this.AlmoxarifadoID != 0)
                {
                    AlmoxarifadoPresenter objPresenter = new AlmoxarifadoPresenter();
                    objEntidade = PageBase.GetSession<AlmoxarifadoEntity>(ALMOXARIFADO);

                    if (objEntidade == null)
                        objEntidade = objPresenter.ObterAlmoxarifado(this.AlmoxarifadoID);
                }

                return objEntidade;
            }

            private set
            {
                if (IsAdminGeral())
                {
                    this.OrgaoID = value.Uge.Orgao.Id.Value;
                    this.UOId = value.Uge.Uo.Id.Value;
                    this.UgeID = value.Uge.Id.Value;
                }

                this.AlmoxarifadoID = (value.IsNotNull() ? value.Id.Value : 0);

                PageBase.SetSession<AlmoxarifadoEntity>(value, ALMOXARIFADO);
            }
        }
        public DivisaoEntity Divisao
        {
            get
            {
                DivisaoEntity objEntidade = null;
                DivisaoPresenter objPresenter = null;


                if (this.DivisaoId != 0)
                {
                    objPresenter = new DivisaoPresenter();
                    objEntidade = objPresenter.ObterDivisao(DivisaoId);
                }

                return objEntidade;
            }

            private set
            {
                this.DivisaoId = (value.IsNotNull() ? value.Id.Value : -1);
            }
        }
        public UAEntity Ua
        {
            get
            {
                UAEntity objEntidade = null;
                UAPresenter objPresenter = null;


                if (this.UAId != 0)
                {
                    objPresenter = new UAPresenter();
                    objEntidade = objPresenter.ObterUA(UAId);
                }

                return objEntidade;
            }

            private set
            {
                this.UAId = (value.IsNotNull() ? value.Id.Value : -1);
            }
        }
        public int AlmoxarifadoID
        {
            get
            {
                int almoxID = PageBase.GetSession<int>(ALMOXARIFADOID);

                if (this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado && almoxID == 0)
                    Int32.TryParse(this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.SelectedItem.Value, out almoxID);

                return almoxID;
            }

            set
            {
                bool isAdminGeral = false;

                isAdminGeral = IsAdminGeral();
                if (value > 0)
                {

                    if (isAdminGeral)
                    {
                        var almoxsUGE = (new Sam.Domain.Business.EstruturaOrganizacionalBusiness().ListarAlmoxarifadoPorUGE(this.UgeID));
                        if (almoxsUGE.HasElements())
                        {
                            if (this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado)
                            {
                                this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.InserirElementoSelecione(true);
                                almoxsUGE.ToList().ForEach(almoxUGE => this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.Items.Add(new ListItem(String.Format("{0} - {1}", almoxUGE.Codigo, almoxUGE.Descricao), almoxUGE.Id.Value.ToString())));
                            }
                        }
                    }

                    PageBase.SetSession<int>(value, ALMOXARIFADOID);

                    if (this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado && this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.Items.FindByValue(value.ToString()) != null)
                        this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.SelectedValue = value.ToString();
                }
            }
        }
        public int UgeID
        {
            get { return this.UGEId; }
            set { this.UGEId = value; }
        }
        public int GestorID
        {
            get
            {
                int _retorno = -1;
                int _indiceCombo = combosEstruturaOrganizacionalPadrao.DdlUO.SelectedIndex;

                if (_indiceCombo != -1)
                    Int32.TryParse(combosEstruturaOrganizacionalPadrao.DdlUO.SelectedValue, out _retorno);


                return _retorno;
            }
            set { combosEstruturaOrganizacionalPadrao.DdlUO.SelectedValue = value.ToString(); }
        }
        public int OrgaoID
        {
            get
            {
                int _retorno = -1;
                int _indiceCombo = combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedIndex;

                if (_indiceCombo != -1)
                    Int32.TryParse(combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedValue, out _retorno);

                return _retorno;
            }
            set { combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedValue = value.ToString(); }
        }

        public byte SistemaModulo
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlSistemaModulo.SelectedIndex;

                if (_indiceCombo != -1)
                    //_retorno = (byte)_indiceCombo;
                    byte.TryParse(ddlSistemaModulo.SelectedValue, out _retorno);

                //return (SistemaModulo)_retorno;
                return _retorno;
            }

            set { ddlSistemaModulo.SelectedValue = value.ToString(); }
        }
        public byte StatusChamadoAtendimentoProdesp
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlStatusChamadoAtendimentoProdesp.SelectedIndex;

                if (_indiceCombo != -1)
                    //_retorno = (byte)_indiceCombo;
                    byte.TryParse(ddlStatusChamadoAtendimentoProdesp.SelectedValue, out _retorno);


                return _retorno;
            }

            set { ddlStatusChamadoAtendimentoProdesp.SelectedValue = value.ToString(); }
        }

        public byte StatusPesquisaChamadoAtendimentoUsuario
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = this.combosEstruturaOrganizacionalPadrao.DdlStatus.SelectedIndex;

                if (_indiceCombo != -1)
                    byte.TryParse(this.combosEstruturaOrganizacionalPadrao.DdlStatus.SelectedValue, out _retorno);


                return _retorno;
            }

            set { this.combosEstruturaOrganizacionalPadrao.DdlStatus.SelectedValue = value.ToString(); }
        }

        public byte StatusPesquisaChamadoAtendimentoProdesp
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = this.combosEstruturaOrganizacionalPadrao.DdlStatusProdesp.SelectedIndex;

                if (_indiceCombo != -1)
                {
                    StatusAtendimentoChamado _status = (StatusAtendimentoChamado)Enum.Parse(typeof(StatusAtendimentoChamado), this.combosEstruturaOrganizacionalPadrao.DdlStatusProdesp.SelectedValue);
                    byte.TryParse(_status.GetHashCode().ToString(), out _retorno);
                }

                return _retorno;
            }

            set { this.combosEstruturaOrganizacionalPadrao.DdlStatusProdesp.SelectedValue = value.ToString(); }
        }
        public byte StatusChamadoAtendimentoUsuario
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlStatusChamadoAtendimentoUsuario.SelectedIndex;

                if (_indiceCombo != -1)
                    byte.TryParse(ddlStatusChamadoAtendimentoUsuario.SelectedValue, out _retorno);

                return _retorno;
            }

            set
            {
                if (ddlStatusChamadoAtendimentoUsuario.Items.FindByValue(value.ToString()) != null)
                    ddlStatusChamadoAtendimentoUsuario.SelectedValue = value.ToString();
            }
        }
        public byte AmbienteSistema
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlAmbienteSistema.SelectedIndex;

                if (_indiceCombo != -1)
                    byte.TryParse(ddlAmbienteSistema.SelectedValue, out _retorno);

                return _retorno;
            }

            set { ddlAmbienteSistema.SelectedValue = value.ToString(); }
        }

        public byte FuncionalidadeSistemaID
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlFuncionalidadeSistema.SelectedIndex;

                if (_indiceCombo != -1)
                    byte.TryParse(ddlFuncionalidadeSistema.SelectedValue, out _retorno);

                return _retorno;
            }

            set { ddlFuncionalidadeSistema.SelectedValue = value.ToString(); }
        }
        public byte TipoChamado
        {
            get
            {
                byte _retorno = 0;
                int _indiceCombo = ddlTipoChamado.SelectedIndex;

                if (_indiceCombo != -1)
                    byte.TryParse(ddlTipoChamado.SelectedValue, out _retorno);

                return _retorno;
            }
            set { ddlTipoChamado.SelectedValue = value.ToString(); }
        }
        public string LogHistoricoAtendimento
        {
            get { return logHistorico.InnerHtml; }
            set { logHistorico.InnerHtml = value; }
        }
        public string Responsavel
        {
            get { return txtNomeAtendenteSuporte.Text; }
            set { txtNomeAtendenteSuporte.Text = value; }
        }
        public string Observacoes
        {
            get { return txtObservacoesChamado.Text; }
            set { txtObservacoesChamado.Text = value; }
        }
        public string AnexoSelecionado
        {
            get
            {
                string strNomeArquivo = null;

                if (this.lstListaArquivosAnexos.SelectedIndex != -1)
                    strNomeArquivo = lstListaArquivosAnexos.SelectedValue;

                return strNomeArquivo;
            }

            private set { }
        }
        public IList<AnexoChamadoSuporte> Anexos
        {
            get
            {
                return PageBase.GetSession<IList<AnexoChamadoSuporte>>(cstSessionKey_ListaAnexos);
            }
            set
            {
                IList<AnexoChamadoSuporte> _listaAnexos = PageBase.GetSession<IList<AnexoChamadoSuporte>>(cstSessionKey_ListaAnexos);

                if (_listaAnexos.IsNullOrEmpty())
                    _listaAnexos = value;
                else if (value.IsNotNullAndNotEmpty())
                    _listaAnexos = new List<AnexoChamadoSuporte>(value);

                PageBase.SetSession<IList<AnexoChamadoSuporte>>(_listaAnexos, cstSessionKey_ListaAnexos);
                lstListaArquivosAnexos.Items.Clear();

                if (_listaAnexos.IsNotNullAndNotEmpty())
                    _listaAnexos.ToList().ForEach(anexoChamado => lstListaArquivosAnexos.Items.Add(new ListItem(anexoChamado.NomeArquivo)));

                if (value.IsNullOrEmpty())
                    lstListaArquivosAnexos.Items.Clear();
            }
        }
        public IList<string> ListaArquivosAnexados
        {
            get { return lstListaArquivosAnexos.Items.Cast<ListItem>().Select(itemLista => itemLista.Value).ToList(); }
        }
        public string ArquivoSelecionadoParaUpload
        {
            get
            {
                string strNomeArquivo = null;

                if (this.fuplAnexo.PostedFile.IsNotNull() && !String.IsNullOrWhiteSpace(this.fuplAnexo.PostedFile.FileName))
                    strNomeArquivo = this.fuplAnexo.PostedFile.FileName;

                return strNomeArquivo;
            }

            private set { }
        }

        public object FileUploader
        {
            get { return this.fuplAnexo; }
        }
        public object RelacaoArquivosAnexados
        {
            get { return this.lstListaArquivosAnexos; }
        }
        public object WebResponse
        {
            get { return this.Response; }
        }
        public object WebContext
        {
            get { return HttpContext.Current; }
        }

        public string DescricaoPerfilUsuario
        {
            get { return txtDescricaoPerfilUsuario.Text; }
            set { txtDescricaoPerfilUsuario.Text = value; }
        }
        #endregion

        protected void ddlGestor_DataBound(object sender, EventArgs e)
        {
            //PopularGrid();
        }

        protected void ProcessarLinhaGrid(object sender, GridViewCommandEventArgs e)
        {
            var perfil = (GetAcesso.Transacoes.Perfis[0].IdPerfil);
            #region Variaveis
            Label lblValorCampo = null;
            string _nomeControleWeb = null;
            int _indiceAtual = -1;
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter();
            ChamadoSuporteEntity chamadoSuporte = null;
            AlmoxarifadoEntity almoxChamadoSuporte = null;

            bool modoEdicao = false;
            bool usuarioQueAbriuChamado = false;
            #endregion

            #region Init
            var _argsEvento = ((e as GridViewCommandEventArgs).IsNotNull() ? (e as GridViewCommandEventArgs) : null);
            var _webControl = ((_argsEvento).IsNotNull() ? (_argsEvento.CommandSource as WebControl) : null);
            var _gridRow = ((_webControl.IsNotNull()) ? (_webControl.NamingContainer as GridViewRow) : null);

            objPresenter = new ChamadoSuportePresenter(this);

            if (_gridRow.IsNotNull())
                _indiceAtual = _gridRow.RowIndex;
            else if (_gridRow.IsNull() || _gridRow.RowIndex == -1)
                return;


            _nomeControleWeb = "lblNumeroChamado";
            if (gdvAtendimentoChamados.Rows[_indiceAtual].FindControl(_nomeControleWeb).IsNotNull())
            {
                var _tmpNumero = -1;
                lblValorCampo = (Label)gdvAtendimentoChamados.Rows[_indiceAtual].FindControl(_nomeControleWeb);
                Int32.TryParse(Server.HtmlDecode(lblValorCampo.Text), out _tmpNumero);

                NumeroChamado = _tmpNumero;
            }
            #endregion

            var usuarioLogado = this.GetAcesso.Transacoes.Usuario;

            chamadoSuporte = objPresenter.ObterChamadoSuporte(NumeroChamado);
            almoxChamadoSuporte = ((chamadoSuporte.Almoxarifado.IsNotNull() && chamadoSuporte.Almoxarifado.Id.HasValue) ? chamadoSuporte.Almoxarifado : ((chamadoSuporte.Divisao.Almoxarifado.IsNotNull() && chamadoSuporte.Divisao.Almoxarifado.Id.HasValue) ? chamadoSuporte.Divisao.Almoxarifado : null));

            if (chamadoSuporte.IsNotNull())
            {
                DataAberturaChamado = chamadoSuporte.DataAbertura;
                DataFechamentoChamado = chamadoSuporte.DataFechamento;
                NomeReduzidoCliente = ((almoxChamadoSuporte.IsNotNull() && almoxChamadoSuporte.Id.HasValue) ? (String.Format("{0} / ({1:D6}) {2}", almoxChamadoSuporte.Gestor.NomeReduzido, almoxChamadoSuporte.Uge.Codigo, almoxChamadoSuporte.Descricao)) : null);
                CpfUsuario = chamadoSuporte.CpfUsuario;
                NomeUsuario = chamadoSuporte.NomeUsuario;
                EMailUsuario = chamadoSuporte.EMailUsuario;
                SistemaModulo = chamadoSuporte.SistemaModulo;
                StatusChamadoAtendimentoProdesp = chamadoSuporte.StatusChamadoAtendimentoProdesp;
                FuncionalidadeSistemaID = (byte)chamadoSuporte.Funcionalidade.Id;
                TipoChamado = chamadoSuporte.TipoChamado;
                Responsavel = chamadoSuporte.Responsavel;
                LogHistoricoAtendimento = objPresenter.ProcessarHistoricoAtendimento(chamadoSuporte.HistoricoAtendimento);
                Anexos = chamadoSuporte.Anexos;
                Almoxarifado = almoxChamadoSuporte;

                PerfilUsuarioAberturaChamadoID = chamadoSuporte.PerfilUsuarioAberturaChamado;
                DescricaoPerfilUsuario = EnumUtils.GetEnumDescription<tipoPerfil>((tipoPerfil)PerfilUsuarioAberturaChamadoID);
                StatusChamadoAtendimentoUsuario = chamadoSuporte.StatusChamadoAtendimentoUsuario;
            }

            Observacoes = null;
            MostrarPainelEdicao = true;
            modoEdicao = (StatusChamadoAtendimentoProdesp != (byte)statusAtendimentoChamado.Finalizado);
            usuarioQueAbriuChamado = (chamadoSuporte.CpfUsuario.ToString().PadLeft(11, '0') == usuarioLogado.Cpf.PadLeft(11));

            if (IsAdminGeral())
            {
                BloqueiaResponsavel = BloqueiaFuncionalidade = BloqueiaTipoChamado = false;
            }
            else if (!IsAdminGeral() || modoEdicao)
            {
                BloqueiaStatusAtendimentoChamadoUsuario = BloqueiaTipoChamado = false;
                BloqueiaStatusAtendimentoChamadoProdesp = BloqueiaAmbienteSistema = BloqueiaResponsavel = BloqueiaFuncionalidade = BloqueiaTipoChamado = true;
            }

            if (!IsAdminGeral() && !modoEdicao)
                BloqueiaStatusAtendimentoChamadoUsuario = BloqueiaTipoChamado = true;

            if ((!modoEdicao && !IsAdminGeral()) || (!usuarioQueAbriuChamado && !IsAdminGeral()))
                BloqueiaEdicaoChamado = BloqueiaGravar = BloqueiaExcluir = BloqueiaStatusAtendimentoChamadoProdesp = BloqueiaUploaderArquivos = BloqueriaListaArquivos = true;
            else if (modoEdicao && IsAdminGeral())
                BloqueiaStatusAtendimentoChamadoProdesp = false;
            else if (modoEdicao)
                BloqueiaObservacoes = BloqueiaGravar = BloqueiaStatusAtendimentoChamadoUsuario = BloqueiaEMail = BloqueiaUploaderArquivos = BloqueriaListaArquivos = false;

            if (StatusChamadoAtendimentoProdesp != (byte)statusAtendimentoChamado.Aberto)
                BloqueiaBotaoRemoverAnexo = true;

            BloqueiaBotaoDownloadAnexos = Anexos.IsNullOrEmpty();
            BloqueiaCancelar = false;
            if ((perfil == (int)GeralEnum.TipoPerfil.AdministradorGeral))
            BloqueiaBotaoRemoverAnexo = false;
            else
            BloqueiaBotaoRemoverAnexo = true; 
        }


        private ChamadoSuporteEntity obterChamadoSuporte(GridViewRow grdViewRow)
        {
            Label _lblValorCampo = null;
            string _nomeControleWeb = "lblNumeroChamado";

            ChamadoSuportePresenter _objPresenter = null;
            ChamadoSuporteEntity _chamadoSuporte = null;

            _objPresenter = new ChamadoSuportePresenter(this);

            var NumeroChamado = -1;
            _lblValorCampo = (Label)gdvAtendimentoChamados.Rows[grdViewRow.RowIndex].FindControl(_nomeControleWeb);
            Int32.TryParse(Server.HtmlDecode(_lblValorCampo.Text), out NumeroChamado);

            var usuarioLogado = this.GetAcesso.Transacoes.Usuario;

            _chamadoSuporte = _objPresenter.ObterChamadoSuporte(NumeroChamado);

            return _chamadoSuporte;
        }

        protected void gdvAtendimentoChamados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var gridRow = e.Row;

            if (gridRow.DataItem.IsNotNull() && gridRow.RowType == DataControlRowType.DataRow)
            {
                ChamadoSuporteEntity chamadoSuporte = null;
                ChamadoSuportePresenter objPresenter = null;
                Color corStatusDeAguardo = Color.Red;
                AlmoxarifadoEntity almoxChamadoSuporte = null;

                ImageButton imgEdicao;
                ImageButton imgHistorico;
                Label lblNumeroChamado;
                Label lblDataAberturaChamado;
                Label lblDataFechamentoChamado;
                Label lblNomeReduzidoCliente;
                Label lblIdentificacaoUsuario;
                //Label lblSistemaModulo;
                Label lblDataHoraUltimaEdicao;
                Label lblStatusChamadoAtendimentoProdesp;
                Label lblStatusChamadoAtendimentoUsuario;
                Label lblCpfUsuarioSolicitante;
                Label lblFuncionalidadeSistema;
                Label lblTipoChamado;
                Label lblObservacoesChamado;
                Label lblLogHistoricoAtendimento;
                Label lblResponsavelUsuarioSAM;
                CheckBox chkAtualizarStatusLote;


                chamadoSuporte = (ChamadoSuporteEntity)gridRow.DataItem;
                imgEdicao = (ImageButton)gridRow.FindControl("imgEdicao");
                imgHistorico = (ImageButton)gridRow.FindControl("imgHistorico");
                lblNumeroChamado = (Label)gridRow.FindControl("lblNumeroChamado");
                lblDataAberturaChamado = (Label)gridRow.FindControl("lblDataAberturaChamado");
                lblDataFechamentoChamado = (Label)gridRow.FindControl("lblDataFechamentoChamado");
                lblNomeReduzidoCliente = (Label)gridRow.FindControl("lblNomeReduzidoCliente");
                lblIdentificacaoUsuario = (Label)gridRow.FindControl("lblIdentificacaoUsuario");
                //lblSistemaModulo			        = (Label)gridRow.FindControl("lblSistemaModulo");
                lblDataHoraUltimaEdicao = (Label)gridRow.FindControl("lblDataHoraUltimaEdicao");
                lblStatusChamadoAtendimentoProdesp = (Label)gridRow.FindControl("lblStatusChamadoAtendimentoProdesp");
                lblStatusChamadoAtendimentoUsuario = (Label)gridRow.FindControl("lblStatusChamadoAtendimentoUsuario");
                lblCpfUsuarioSolicitante = (Label)gridRow.FindControl("lblCpfUsuarioSolicitante");
                lblFuncionalidadeSistema = (Label)gridRow.FindControl("lblFuncionalidadeSistema");
                lblTipoChamado = (Label)gridRow.FindControl("lblTipoChamado");
                lblObservacoesChamado = (Label)gridRow.FindControl("lblObservacoesChamado");
                lblLogHistoricoAtendimento = (Label)gridRow.FindControl("lblLogHistoricoAtendimento");
                lblResponsavelUsuarioSAM = (Label)gridRow.FindControl("lblResponsavelUsuarioSAM");

                if (IsAdminGeral())
                {
                    chkAtualizarStatusLote = (CheckBox)gridRow.FindControl("chkAtualizarStatusLote");

                    if (chamadoSuporte.ChamadoFinalizado)
                        chkAtualizarStatusLote.Visible = false;
                }

                if (lblNumeroChamado.IsNotNull())
                    lblNumeroChamado.Text = chamadoSuporte.Id.Value.ToString("D7");

                if (lblDataAberturaChamado.IsNotNull())
                    lblDataAberturaChamado.Text = chamadoSuporte.DataAbertura.ToString("dd/MM/yyyy HH:mm");

                if (lblDataFechamentoChamado.IsNotNull() && chamadoSuporte.DataFechamento.HasValue)
                    lblDataFechamentoChamado.Text = chamadoSuporte.DataFechamento.Value.ToString("dd/MM/yyyy HH:mm");

                if (lblNomeReduzidoCliente.IsNotNull())
                {
                    almoxChamadoSuporte = ((chamadoSuporte.Almoxarifado.IsNotNull() && chamadoSuporte.Almoxarifado.Id.HasValue) ? chamadoSuporte.Almoxarifado : ((chamadoSuporte.Divisao.Almoxarifado.IsNotNull() && chamadoSuporte.Divisao.Almoxarifado.Id.HasValue) ? chamadoSuporte.Divisao.Almoxarifado : null));

                    if (almoxChamadoSuporte.IsNotNull())
                    {
                        lblNomeReduzidoCliente.Text = String.Format("{0} / ({1:D6})", almoxChamadoSuporte.Gestor.NomeReduzido, almoxChamadoSuporte.Uge.Codigo);
                        lblNomeReduzidoCliente.ToolTip = String.Format("{0} / ({1:D6}) {2}", almoxChamadoSuporte.Gestor.NomeReduzido, almoxChamadoSuporte.Uge.Codigo, almoxChamadoSuporte.Descricao);
                    }
                }

                if (lblIdentificacaoUsuario.IsNotNull())
                    lblIdentificacaoUsuario.Text = chamadoSuporte.CpfUsuario.ToString();

                if (lblResponsavelUsuarioSAM.IsNotNull())
                    lblResponsavelUsuarioSAM.Text = chamadoSuporte.Responsavel;

                try
                {
                    if (lblDataHoraUltimaEdicao.IsNotNull())
                        lblDataHoraUltimaEdicao.Text = chamadoSuporte.DataHoraUltimaEdicao.ToString(base.fmtDataHoraFormatoBrasileiro);
                }
                catch (Exception)
                {
                    ExibirMensagem(String.Format("Erro ao recuperar status de atendimento, para chamado de suporte código {0:D7}", chamadoSuporte.Id.Value));
                }

                try
                {
                    if (lblStatusChamadoAtendimentoProdesp.IsNotNull())
                        lblStatusChamadoAtendimentoProdesp.Text = GeralEnum.GetEnumDescription((Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado)chamadoSuporte.StatusChamadoAtendimentoProdesp);

                    objPresenter = new ChamadoSuportePresenter();
                    if (objPresenter.UltimaInteracaoDoUsuario(chamadoSuporte.LogHistoricoAtendimento))
                        corStatusDeAguardo = Color.Green;

                    if (chamadoSuporte.StatusChamadoAtendimentoProdesp == (byte)statusAtendimentoChamado.AguardandoRetornoUsuario)
                    {
                        lblStatusChamadoAtendimentoProdesp.ForeColor = corStatusDeAguardo;
                        lblStatusChamadoAtendimentoProdesp.Font.Bold = true;
                    }
                }
                catch (Exception)
                {
                    ExibirMensagem(String.Format("Erro ao recuperar status de atendimento, para chamado de suporte código {0:D7}", chamadoSuporte.Id.Value));
                }

                try
                {
                    if (lblStatusChamadoAtendimentoUsuario.IsNotNull())
                        lblStatusChamadoAtendimentoUsuario.Text = GeralEnum.GetEnumDescription((Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado)chamadoSuporte.StatusChamadoAtendimentoUsuario);
                }
                catch (Exception)
                {
                    ExibirMensagem(String.Format("Erro ao recuperar status de atendimento, para chamado de suporte código {0:D7}", chamadoSuporte.Id.Value));
                }

                if (lblCpfUsuarioSolicitante.IsNotNull())
                {
                    lblCpfUsuarioSolicitante.Text = chamadoSuporte.CpfUsuario.ToString();
                    var usuariosSAM = UsuarioBusiness.RecuperarInformacoesUsuario(lblCpfUsuarioSolicitante.Text);

                    if (usuariosSAM.HasElements())
                        lblCpfUsuarioSolicitante.ToolTip = usuariosSAM[0].NomeUsuario;
                }


                if (lblFuncionalidadeSistema.IsNotNull())
                    if (chamadoSuporte.Funcionalidade.IsNotNull() && !String.IsNullOrWhiteSpace(chamadoSuporte.Funcionalidade.Descricao))
                        lblFuncionalidadeSistema.Text = chamadoSuporte.Funcionalidade.Descricao;

                try
                {
                    if (lblTipoChamado.IsNotNull())
                        lblTipoChamado.Text = GeralEnum.GetEnumDescription((Sam.Common.Enums.ChamadoSuporteEnums.TipoChamadoSuporte)chamadoSuporte.TipoChamado);
                }
                catch (Exception)
                {
                    ExibirMensagem(String.Format("Erro ao recuperar tipo de chamado, para chamado de suporte código {0:D7}", chamadoSuporte.Id.Value));
                }

                if (lblObservacoesChamado.IsNotNull())
                    lblObservacoesChamado.Text = chamadoSuporte.Observacoes;

                if (lblLogHistoricoAtendimento.IsNotNull())
                    lblLogHistoricoAtendimento.Text = chamadoSuporte.LogHistoricoAtendimento;

                var chamadoFinalizado = chamadoSuporte.ChamadoFinalizado;
                if (imgEdicao.IsNotNull())
                    imgEdicao.Visible = !chamadoFinalizado;

                if (imgHistorico.IsNotNull())
                    imgHistorico.Visible = chamadoFinalizado;

                BloqueiaStatusAtendimentoChamadoProdesp = BloqueiaStatusAtendimentoChamadoUsuario = (chamadoSuporte.Id == 0);
            }
        }
        protected void gdvAtendimentoChamados_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        protected void gdvAtendimentoChamados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            CarregarDadosTela(sender);
            ProcessarLinhaGrid(sender, e);
        }
        protected void gdvAtendimentoChamados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            int indicePagina = 0;
            IList<ChamadoSuporteEntity> listaChamadosSuporte = null;

            listaChamadosSuporte = GetSession<IList<ChamadoSuporteEntity>>(cstListaChamadosSuporte);
            if (!listaChamadosSuporte.HasElements())
            {
                PopularGrid();
                listaChamadosSuporte = GetSession<IList<ChamadoSuporteEntity>>(cstListaChamadosSuporte);
            }

            indicePagina = e.NewPageIndex;

            gdvAtendimentoChamados.PageIndex = e.NewPageIndex;
            gdvAtendimentoChamados.DataSource = listaChamadosSuporte;
            gdvAtendimentoChamados.DataBind();
        }

        protected void btnDownloadAnexos_Click(object sender, EventArgs e)
        {
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.IniciarDownload();
        }

        protected void btnNovo_Click(object sender, EventArgs e)
        {
            bool painelVisivel = false;
            ChamadoSuportePresenter objPresenter = null;


            BloqueiaEdicaoChamado = false;
            BloqueiaUploaderArquivos = BloqueriaListaArquivos = false;
            this.NumeroChamado = 0;
            painelVisivel = (NumeroChamado != 0);

            objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.Novo();

            obterDadosUsuarioLogado();
            CarregarDadosTela(sender);

            if (painelVisivel)
                BloqueiaFuncionalidade = BloqueiaTipoChamado = BloqueiaAmbienteSistema = BloqueiaSistemaModulo = BloqueiaUploaderArquivos = BloqueriaListaArquivos = false;
        }

        protected void btnGravar_Click(object sender, EventArgs e)
        {
            bool ehAdminGeral = IsAdminGeral();
            ChamadoSuportePresenter objPresenter = null;

            //obterDadosPerfilLogado();
            objPresenter = new ChamadoSuportePresenter(this);

            if (objPresenter.Gravar(ehAdminGeral))
            {
                selecionarDadosPesquisa();
                PopularGrid();
                limparSession();
            }
        }

        private void limparSession()
        {
            RemoveSession(cstSessionKey_ListaAnexos);
            RemoveSession(ALMOXARIFADO);
            RemoveSession(ALMOXARIFADOID);
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.Excluir();
        }
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            selecionarDadosPesquisa();
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.Cancelar();

            MostrarPainelEdicao = false;
            limparSession();
        }

        protected void btnImportarArquivo_Click(object sender, EventArgs e)
        {
            ChamadoSuportePresenter objPresenter = null;

            objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.AnexarArquivo();

            MostrarPainelEdicao = true;
        }
        protected void btnRemoverArquivo_Click(object sender, EventArgs e)
        {
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.RemoverAnexo();

            MostrarPainelEdicao = true;
        }
        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            persistirDadosPesquisa();
            PopularGrid();
        }

        private void persistirDadosPesquisa()
        {
            this.persistirSelecaoDropDownNaSession(this.combosEstruturaOrganizacionalPadrao.DdlOrgao);
            this.persistirSelecaoDropDownNaSession(this.combosEstruturaOrganizacionalPadrao.DdlUO);
            this.persistirSelecaoDropDownNaSession(this.combosEstruturaOrganizacionalPadrao.DdlUGE);
            this.persistirSelecaoDropDownNaSession(this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado);
            this.persistirSelecaoDropDownNaSession(this.combosEstruturaOrganizacionalPadrao.DdlStatus);
        }

        private void selecionarDadosPesquisa()
        {
            this.selecionarItemPesquisaPersistidoSession(this.combosEstruturaOrganizacionalPadrao.DdlOrgao);
            this.selecionarItemPesquisaPersistidoSession(this.combosEstruturaOrganizacionalPadrao.DdlUO);
            this.selecionarItemPesquisaPersistidoSession(this.combosEstruturaOrganizacionalPadrao.DdlUGE);
            this.selecionarItemPesquisaPersistidoSession(this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado);
            this.selecionarItemPesquisaPersistidoSession(this.combosEstruturaOrganizacionalPadrao.DdlStatus);
        }

        private void selecionarItemPesquisaPersistidoSession(DropDownList dropDownList)
        {
            var _valor = obterDadosPesquisaDaSession(dropDownList).ToString();

            if (dropDownList.Items.FindByValue(_valor) != null)
                dropDownList.SelectedValue = _valor;
        }

        private int obterDadosPesquisaDaSession(DropDownList dropDownList)
        {
            int _retorno = default(int);
            int.TryParse(PageBase.GetSession<string>(dropDownList.ID), out _retorno);

            return _retorno;
        }

        private void persistirSelecaoDropDownNaSession(DropDownList dropDownList)
        {
            PageBase.SetSession(dropDownList.SelectedValue, dropDownList.ID);
        }

        public bool MostrarBotaoStatusAtualizar
        {
            set
            {
                this.btnEditarStatusLote.CssClass = (value && IsAdminGeral()) ? "mostrarControle" : "esconderControle";
            }
        }

        public bool MostrarPainelEdicao
        {
            set
            {
                if (value == false)
                    this.pnlEditar.CssClass = "esconderControle";
                else
                    this.pnlEditar.CssClass = "mostrarControle";
            }
        }

        public bool MostrarPainelStatusAtualizar
        {
            set
            {
                if (value == false)
                    this.pnlStatusAtualizar.CssClass = "esconderControle";
                else
                    this.pnlStatusAtualizar.CssClass = "mostrarControle";
            }
        }

        public void PopularListaFuncionalidades(int[] perfilIDs)
        {
            FuncionalidadeSistemaPresenter objPresenter = new FuncionalidadeSistemaPresenter();
            var _relacaoRegistros = objPresenter.CarregarListaFuncionalidadesSistema(perfilIDs);
            if (_relacaoRegistros.HasElements())
            {
                ddlFuncionalidadeSistema.InserirElementoSelecione(true);
                _relacaoRegistros.ToList().ForEach(funcionalidadeSistema => ddlFuncionalidadeSistema.Items.Add(new ListItem(funcionalidadeSistema.Descricao, funcionalidadeSistema.Id.Value.ToString())));
            }
        }
        public void PopularGrid()
        {
            tipoPesquisa filtroTipoPesquisa = default(tipoPesquisa);
            statusAtendimentoChamado filtroStatusChamadoAtendenteProdesp = default(statusAtendimentoChamado);
            statusAtendimentoChamado filtroStatusChamadoUsuario = default(statusAtendimentoChamado);

            bool? retornarApenasChamadosAtivos = null;
            long numeroChamado = -1;
            long cpfUsuario = -1;
            long rowTabelaID = -1;

            ChamadoSuportePresenter objPresenter = null;
            IList<ChamadoSuporteEntity> listaChamados = null;

            //Pesquisa por ID
            TextBox txtPesquisar = (TextBox)this.combosEstruturaOrganizacionalPadrao.FindControl("txtPesquisar");
            string termoParaPesquisa = txtPesquisar.Text.Trim();

            if (this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Items.Count >= 1)
            {
                //AdminGeral
                if (IsAdminGeral())
                {
                    MostrarBotaoStatusAtualizar = true;
                    BloqueiaResponsavel = false;
                    filtroTipoPesquisa = obterFiltroPesquisa();
                }
                //Nivel 1 - Enxergará todos os chamdos do Orgão
                else if (IsUsuarioNivel1())
                {
                    BloqueiaResponsavel = false;
                    filtroTipoPesquisa = obterFiltroPesquisa();
                }
                //Nivel 2 - Enxergará todos os chamdos da UGE
                else if (IsUsuarioNivel2())
                {
                    BloqueiaResponsavel = false;
                    filtroTipoPesquisa = obterFiltroPesquisa();
                }
                //Requisitante Geral - Enxergará todos os chamados das UO's em que possui perfil
                else if (IsRequisitanteGeral())
                {
                    BloqueiaResponsavel = false;
                    filtroTipoPesquisa = obterFiltroPesquisa();
                }
                //OperadorAlmox ou Requisitante - Enxergará somente os próprios chamados (mero mortal)
                else if (IsOperadorAlmoxOuRequisitanteOuConsultorRelatorio() && !String.IsNullOrWhiteSpace(base.Usuario))
                {
                    var _cpfUsuario = GetAcesso.Transacoes.Usuario.Cpf;
                    Int64.TryParse(_cpfUsuario, out cpfUsuario);

                    filtroTipoPesquisa = obterFiltroPesquisa();
                }

                //Por ora, retornara apenas chamados ativos
                retornarApenasChamadosAtivos = true;
                filtroStatusChamadoUsuario = (statusAtendimentoChamado)StatusPesquisaChamadoAtendimentoUsuario;
                filtroStatusChamadoAtendenteProdesp = (statusAtendimentoChamado)StatusPesquisaChamadoAtendimentoProdesp;
            }

            switch (filtroTipoPesquisa)
            {
                case GeralEnum.TipoPesquisa.SemFiltro: rowTabelaID = -1; break;
                case GeralEnum.TipoPesquisa.Orgao: rowTabelaID = OrgaoID; break;
                case GeralEnum.TipoPesquisa.UO: rowTabelaID = UOId; break;
                case GeralEnum.TipoPesquisa.Gestor: rowTabelaID = GestorID; break;
                case GeralEnum.TipoPesquisa.UGE: rowTabelaID = UgeID; break;
                case GeralEnum.TipoPesquisa.Almox: rowTabelaID = AlmoxarifadoID; break;
                case GeralEnum.TipoPesquisa.Usuario: rowTabelaID = cpfUsuario; break;
                case GeralEnum.TipoPesquisa.ID: Int64.TryParse(termoParaPesquisa, out rowTabelaID); break;

                //pesquisa não-implementada
                case GeralEnum.TipoPesquisa.UA:
                case GeralEnum.TipoPesquisa.Divisao:
                default: ExibirMensagem("Filtro de pesquisa não-implementada!"); break;
            }

            objPresenter = new ChamadoSuportePresenter();
            listaChamados = objPresenter.SelecionarChamados(filtroTipoPesquisa, filtroStatusChamadoAtendenteProdesp, filtroStatusChamadoUsuario, rowTabelaID, retornarApenasChamadosAtivos);

            if (filtroStatusChamadoAtendenteProdesp != statusAtendimentoChamado.Finalizado && filtroStatusChamadoUsuario == statusAtendimentoChamado.Todos && rowTabelaID < 0)
                listaChamados = listaChamados.Where(chamados => ((statusAtendimentoChamado)chamados.StatusChamadoAtendimentoProdesp == statusAtendimentoChamado.Finalizado &&
                                                                 (statusAtendimentoChamado)chamados.StatusChamadoAtendimentoUsuario == statusAtendimentoChamado.Concluido) == false)
                                            .ToList<ChamadoSuporteEntity>();


            if (!String.IsNullOrWhiteSpace(termoParaPesquisa) && Int64.TryParse(termoParaPesquisa, out numeroChamado))
            {
                listaChamados = listaChamados.Where(c => c.Id == numeroChamado).ToList<ChamadoSuporteEntity>();
            }

            SetSession<IList<ChamadoSuporteEntity>>(listaChamados, cstListaChamadosSuporte);

            gdvAtendimentoChamados.PageIndex = 0;
            gdvAtendimentoChamados.DataSource = listaChamados;
            gdvAtendimentoChamados.DataBind();

        }
        public void LimparCamposView()
        {
            this.NumeroChamado = 0;
            this.DataAberturaChamado = new DateTime(0);
            this.Descricao = null;
            this.Observacoes = null;
            this.Responsavel = null;
            this.LogHistoricoAtendimento = null;
            this.SistemaModulo = 0;
            this.FuncionalidadeSistemaID = 0;
            this.StatusChamadoAtendimentoProdesp = 1;
            this.StatusChamadoAtendimentoUsuario = 0;
            this.AmbienteSistema = 0;
            this.TipoChamado = 0;
            this.NomeUsuario = null;
            this.CpfUsuario = 0;
            this.Anexos = null;

            this.DescricaoPerfilUsuario = null;
            this.NomeReduzidoCliente = null;


            //Criar propriedade na página depois.
            this.lstListaArquivosAnexos.Items.Clear();
            //this.ListaArquivosAnexados = null;
            this.EMailUsuario = null;
        }

        public void initTela()
        {
            //var perfilID = this.GetAcesso.Transacoes.Perfis[0].IdPerfil;
            int[] perfilIDs = null;
            int loginID = 0;


            loginID = this.GetAcesso.Transacoes.ID;
            perfilIDs = obterPerfisUsuario(loginID);

            this.PopularListaFuncionalidades(perfilIDs);
            gdvAtendimentoChamados.Visible = true;
        }

        private void PopularOrgao()
        {
            CombosPadroesPresenter objPresenter = new CombosPadroesPresenter();
            IList<OrgaoEntity> listaOrgao = objPresenter.PopularOrgao();

            if (listaOrgao.Count > 0)
            {
                this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Items.Clear();
                this.combosEstruturaOrganizacionalPadrao.DdlOrgao.DataValueField = "Id";
                this.combosEstruturaOrganizacionalPadrao.DdlOrgao.DataTextField = "Descricao";
                this.combosEstruturaOrganizacionalPadrao.DdlOrgao.DataSource = listaOrgao;

                //this.combosEstruturaOrganizacionalPadrao.DdlOrgao.DataBind();
                listaOrgao.Add(new OrgaoEntity() { Id = 0, Descricao = "Todos" });
            }

            if (combosEstruturaOrganizacionalPadrao.DdlOrgao.Items.Count > 1 && IsAdminGeral())
                this.combosEstruturaOrganizacionalPadrao.DdlOrgao.InserirNovaOpcaoLista("Todos", 0);
        }
        private void PopularUO()
        {
            int orgaoSelecionado = OrgaoID;

            if (orgaoSelecionado != 0)
            {
                CombosPadroesPresenter objPresenter = new CombosPadroesPresenter();
                IList<UOEntity> listaUo = objPresenter.PopularUO(orgaoSelecionado);

                if (listaUo.Count > 0)
                {
                    this.combosEstruturaOrganizacionalPadrao.DdlUO.Items.Clear();
                    this.combosEstruturaOrganizacionalPadrao.DdlUO.DataValueField = "Id";
                    this.combosEstruturaOrganizacionalPadrao.DdlUO.DataTextField = "Descricao";
                    this.combosEstruturaOrganizacionalPadrao.DdlUO.DataSource = listaUo;
                    this.DataBind();


                    if (this.combosEstruturaOrganizacionalPadrao.DdlUO.Items.Count > 1)
                    {
                        this.combosEstruturaOrganizacionalPadrao.DdlUO.InserirNovaOpcaoLista("Todos", 0);
                        this.combosEstruturaOrganizacionalPadrao.DdlUO.SelectedIndex = 0;
                    }
                }
            }
        }
        private void PopularUGE()
        {
            int uoSelecionado = UOId;

            if (uoSelecionado != 0)
            {
                CombosPadroesPresenter objPresenter = new CombosPadroesPresenter();
                IList<UGEEntity> listaUge = objPresenter.PopularUGE(uoSelecionado);

                if (listaUge.Count > 0)
                {
                    this.combosEstruturaOrganizacionalPadrao.DdlUGE.Items.Clear();
                    this.combosEstruturaOrganizacionalPadrao.DdlUGE.DataValueField = "Id";
                    this.combosEstruturaOrganizacionalPadrao.DdlUGE.DataTextField = "Descricao";
                    this.combosEstruturaOrganizacionalPadrao.DdlUGE.DataSource = listaUge;
                    this.DataBind();

                    if (this.combosEstruturaOrganizacionalPadrao.DdlUGE.Items.Count > 1)
                    {
                        this.combosEstruturaOrganizacionalPadrao.DdlUGE.InserirNovaOpcaoLista("Todos", 0);
                        this.combosEstruturaOrganizacionalPadrao.DdlUGE.SelectedIndex = 0;
                    }
                }
            }
        }

        public void CarregarDadosTela(object sender)
        {
            int[] perfilIDs = null;
            int loginID = 0;


            loginID = this.GetAcesso.Transacoes.ID;
            perfilIDs = obterPerfisUsuario(loginID);
            if (this.UgeID == 0 || this.AlmoxarifadoID == 0)
            {
                if (sender.GetType().ToString() == typeof(System.Web.UI.WebControls.Button).FullName)
                    ExibirMensagem("Selecionar UGE/Almoxarifado antes de efetuar abertura de chamado!.");

                MostrarPainelEdicao = false;
                MostrarPainelStatusAtualizar = false;
                return;
            }

            if (!combosEstruturaOrganizacionalPadrao.DdlUO.Items.Cast<ListItem>().HasElements())
                PopularListaFuncionalidades(perfilIDs);

            DataAberturaChamado = DateTime.Now;
            StatusChamadoAtendimentoUsuario = StatusChamadoAtendimentoProdesp = (byte)statusAtendimentoChamado.Aberto;
            Responsavel = "SuporteSAM";

            BloqueiaStatusAtendimentoChamadoProdesp = BloqueiaStatusAtendimentoChamadoUsuario = (this.NumeroChamado == 0);

            obterDadosSelecaoControleHierarquiaPadrao();

            MostrarPainelEdicao = true;
        }

        private int[] obterPerfisUsuario(int loginID)
        {
            int[] perfilIDs = null;

            if (loginID == 1)
                return (new int[] { (int)tipoPerfil.AdministradorGeral });

            UsuarioPerfilPresenter objPresenter = new UsuarioPerfilPresenter();
            var perfisSAM = objPresenter.PopularDadosUsuarioPerfilGrid(loginID);

            if (perfisSAM.HasElements())
                perfilIDs = perfisSAM.Select(perfilSAM => (int)perfilSAM.IdPerfil)
                                     .Distinct()
                                     .ToArray();


            return perfilIDs;
        }

        private void obterDadosSelecaoControleHierarquiaPadrao()
        {
            var perfilID = this.GetAcesso.Transacoes.Perfis[0].IdPerfil;
            AlmoxarifadoEntity almoxSelecionado = null;


            if (this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado && this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.SelectedItem.Text.ToLowerInvariant() == "Todos".ToLowerInvariant())
            {
                this.AlmoxarifadoID = ((almoxSelecionado.IsNotNull()) ? almoxSelecionado.Id.Value : 0);
            }
            else if (this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado && this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.SelectedItem.Text.ToLowerInvariant() != "Todos".ToLowerInvariant())
            {
                almoxSelecionado = (new AlmoxarifadoPresenter()).ObterAlmoxarifado(this.AlmoxarifadoID);
            }
            else if (!this.combosEstruturaOrganizacionalPadrao.ExisteComboAlmoxarifado)
            {
                int divisaoID = -1;

                if (Int32.TryParse(this.combosEstruturaOrganizacionalPadrao.DdlDivisao.SelectedItem.Value, out divisaoID))
                {
                    DivisaoId = divisaoID;
                    almoxSelecionado = ((this.GetAcesso.IsNotNull() && this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado.IsNotNull()) ? this.GetAcesso.Transacoes.Perfis[0].AlmoxarifadoLogado : null);
                }
            }

            //Almoxarifado = almoxSelecionado;
            var nomeReduzidoGestorAlmox = (((almoxSelecionado.IsNotNull() && almoxSelecionado.Gestor.IsNotNull()) ? (almoxSelecionado.Gestor.NomeReduzido) : null));
            var fmtNomeReduzido = string.Empty;

            var codigoUGE = this.combosEstruturaOrganizacionalPadrao.DdlUGE.SelectedItem.Text.BreakLine(new char[] { '-' })[0].Trim();
            var descricaoUGE = this.combosEstruturaOrganizacionalPadrao.DdlUGE.SelectedItem.Text.Remove(0, codigoUGE.Length + 1).Trim();


            if (!String.IsNullOrWhiteSpace(nomeReduzidoGestorAlmox))
                fmtNomeReduzido = "{0} / ({1}) {2}";
            else
                fmtNomeReduzido = "{0}({1}) {2}";

            NomeReduzidoCliente = String.Format(fmtNomeReduzido, nomeReduzidoGestorAlmox, codigoUGE, descricaoUGE);

        }
        private void obterDadosUsuarioLogado()
        {
            var usuarioLogado = this.GetAcesso.Transacoes.Usuario;
            var perfilUsuarioLogado = this.GetAcesso.Transacoes.Perfis[0];
            long cpfUsuario = 0;

            if (usuarioLogado.IsNotNull() && Int64.TryParse(usuarioLogado.Cpf, out cpfUsuario))
            {
                CpfUsuario = cpfUsuario;
                NomeUsuario = usuarioLogado.NomeUsuario;
                EMailUsuario = usuarioLogado.Email;

                PerfilUsuarioAberturaChamadoID = (byte)perfilUsuarioLogado.IdPerfil;
                DescricaoPerfilUsuario = perfilUsuarioLogado.Descricao;
            }
        }

        private void obterDadosPerfilLogado()
        {
            var usuarioLogado = this.GetAcesso.Transacoes.Usuario;
            var perfilUsuarioLogado = this.GetAcesso.Transacoes.Perfis[0];
            var almoxLogado = perfilUsuarioLogado.AlmoxarifadoLogado;
            long cpfUsuario = 0;

            if (perfilUsuarioLogado.IsNotNull() && perfilUsuarioLogado.IdPerfil != 0 && Int64.TryParse(usuarioLogado.Cpf, out cpfUsuario))
            {
                PerfilUsuarioAberturaChamadoID = (byte)perfilUsuarioLogado.IdPerfil;
                DescricaoPerfilUsuario = perfilUsuarioLogado.Descricao;

                CpfUsuario = cpfUsuario;
            }
        }

        private tipoPesquisa obterFiltroPesquisa()
        {
            tipoPesquisa filtroPesquisa = tipoPesquisa.Usuario;

            #region AdminGeral
            {
                if (this.combosEstruturaOrganizacionalPadrao.DdlOrgao.OpcaoTodosSelecionada() && !this.combosEstruturaOrganizacionalPadrao.ExisteChamadoSuporteID)
                    filtroPesquisa = tipoPesquisa.SemFiltro;

                else if (!this.combosEstruturaOrganizacionalPadrao.DdlOrgao.OpcaoTodosSelecionada() && this.combosEstruturaOrganizacionalPadrao.DdlUO.OpcaoTodosSelecionada())
                    filtroPesquisa = tipoPesquisa.Orgao;

                else if (!this.combosEstruturaOrganizacionalPadrao.DdlUO.OpcaoTodosSelecionada() && this.combosEstruturaOrganizacionalPadrao.DdlUGE.OpcaoTodosSelecionada())
                    filtroPesquisa = tipoPesquisa.UO;

                else if (!this.combosEstruturaOrganizacionalPadrao.DdlUGE.OpcaoTodosSelecionada() && (this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.IsNotNull()
                                                                                                  && this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.OpcaoTodosSelecionada()))
                    filtroPesquisa = tipoPesquisa.UGE;

                else if (!this.combosEstruturaOrganizacionalPadrao.DdlAlmoxarifado.OpcaoTodosSelecionada() /*&& !this.combosEstruturaOrganizacionalPadrao.ExisteChamadoSuporteID*/)
                    filtroPesquisa = tipoPesquisa.Almox;

                else if (this.combosEstruturaOrganizacionalPadrao.ExisteChamadoSuporteID)
                    filtroPesquisa = tipoPesquisa.ID;
            }
            #endregion


            return filtroPesquisa;
        }

        public bool IsAdminGeral()
        {
            return (base.obterPerfilID() == GeralEnum.TipoPerfil.AdministradorGeral.GetHashCode());
        }
        public bool IsUsuarioNivel1()
        {
            return (base.obterPerfilID() == GeralEnum.TipoPerfil.AdministradorOrgao.GetHashCode());
        }
        public bool IsUsuarioNivel2()
        {
            return (base.obterPerfilID() == GeralEnum.TipoPerfil.AdministradorGestor.GetHashCode());
        }
        public bool IsOperadorAlmoxOuRequisitanteOuConsultorRelatorio()
        {
            int[] _perfilTratado = { GeralEnum.TipoPerfil.OperadorAlmoxarifado.GetHashCode(),
                                     GeralEnum.TipoPerfil.Requisitante.GetHashCode(),
                                     GeralEnum.TipoPerfil.ConsultaRelatorio.GetHashCode() };

            return _perfilTratado.Contains(base.obterPerfilID());
        }
        public bool IsRequisitanteGeral()
        {
            return (base.obterPerfilID() == (int)GeralEnum.TipoPerfil.RequisitanteGeral);
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Id { get; set; }
        public bool BloqueiaNovo { get; set; }

        public bool BloqueiaGravar
        {
            get { return btnGravar.Enabled; }
            set { btnGravar.Enabled = !value; }
        }
        public bool BloqueiaExcluir
        {
            get { return btnExcluir.Enabled; }
            set { btnExcluir.Enabled = !value; }
        }
        public bool BloqueiaCancelar
        {
            get { return btnCancelar.Enabled; }
            set { btnCancelar.Enabled = !value; }
        }
        public bool BloqueiaCodigo { get; set; }
        public bool BloqueiaDescricao { get; set; }

        public bool BloqueiaResponsavel
        {
            get { return txtNomeAtendenteSuporte.Enabled; }
            set { txtNomeAtendenteSuporte.Enabled = !value; }
        }
        public bool BloqueiaFuncionalidade
        {
            get { return ddlFuncionalidadeSistema.Enabled; }
            set { ddlFuncionalidadeSistema.Enabled = !value; }
        }
        public bool BloqueiaTipoChamado
        {
            get { return ddlTipoChamado.Enabled; }
            set { ddlTipoChamado.Enabled = !value; }
        }
        public bool BloqueiaSistemaModulo
        {
            get { return ddlSistemaModulo.Enabled; }
            set { ddlSistemaModulo.Enabled = !value; }
        }
        public bool BloqueiaAmbienteSistema
        {
            get { return ddlAmbienteSistema.Enabled; }
            set { ddlAmbienteSistema.Enabled = !value; }
        }
        public bool BloqueiaStatusAtendimentoChamadoUsuario
        {
            get { return ddlStatusChamadoAtendimentoUsuario.Enabled; }
            set { ddlStatusChamadoAtendimentoUsuario.Enabled = !value; }
        }
        public bool BloqueiaPesquisaStatusAtendimentoChamadoUsuario
        {
            get { return this.combosEstruturaOrganizacionalPadrao.DdlStatus.Enabled; }
            set { this.combosEstruturaOrganizacionalPadrao.DdlStatus.Enabled = !value; }
        }
        public bool BloqueiaStatusAtendimentoChamadoProdesp
        {
            get { return ddlStatusChamadoAtendimentoProdesp.Enabled; }
            set { ddlStatusChamadoAtendimentoProdesp.Enabled = !value; }
        }
        public bool BloqueiaObservacoes
        {
            get { return txtObservacoesChamado.Enabled; }
            set { txtObservacoesChamado.Enabled = !value; }
        }
        public bool BloqueiaBotaoAdicionarAnexo
        {
            set { fuplAnexo.Enabled = btnImportarArquivo.Enabled = !value; }
        }
        public bool BloqueiaBotaoDownloadAnexos
        {
            set { btnDownloadAnexos.Enabled = !value; }
        }
        public bool BloqueiaBotaoRemoverAnexo
        {
            set { btnRemoverArquivo.Enabled = !value; }
        }
        public bool BloqueiaEMail
        {
            set { txtEMailUsuario.Enabled = !value; }
        }

        public bool BloqueiaUploaderArquivos
        {
            set { fuplAnexo.Enabled = !value; }
        }

        public bool BloqueriaListaArquivos
        {
            set { lstListaArquivosAnexos.Enabled = !value; }
        }
        public bool BloqueiaEdicaoChamado
        {
            set
            {
                BloqueiaStatusAtendimentoChamadoProdesp
              = BloqueiaStatusAtendimentoChamadoUsuario

              = BloqueiaAmbienteSistema
              = BloqueiaResponsavel
              = BloqueiaFuncionalidade
              = BloqueiaTipoChamado
              = BloqueiaObservacoes
              = BloqueiaEMail

              = BloqueiaGravar
              = BloqueiaExcluir
              = value;
            }
        }

        #region ComboBoxesHierarquiaPadrao

        public int OrgaoId
        {
            get
            {
                if (this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0 && this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Items.Count > 0)
                    this.combosEstruturaOrganizacionalPadrao.DdlOrgao.SelectedValue = value.ToString();
            }
        }
        public int UOId
        {
            get
            {
                if (this.combosEstruturaOrganizacionalPadrao.DdlUO.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.combosEstruturaOrganizacionalPadrao.DdlUO.SelectedValue);
                else
                    return 0;
            }
            set
            {
                bool isAdminGeral = false;

                isAdminGeral = IsAdminGeral();
                if (value > 0)
                {
                    if (isAdminGeral)
                    {
                        var uosOrgao = (new Sam.Domain.Business.EstruturaOrganizacionalBusiness().ListarUOs(this.OrgaoID));
                        if (uosOrgao.HasElements())
                        {
                            this.combosEstruturaOrganizacionalPadrao.DdlUO.InserirElementoSelecione(true);
                            uosOrgao.ToList().ForEach(uoOrgao => this.combosEstruturaOrganizacionalPadrao.DdlUO.Items.Add(new ListItem(String.Format("{0} - {1}", uoOrgao.Codigo, uoOrgao.Descricao), uoOrgao.Id.Value.ToString())));
                        }
                    }

                    this.combosEstruturaOrganizacionalPadrao.DdlUO.SelectedValue = value.ToString();
                }
            }
        }
        public int UGEId
        {
            get
            {
                if (this.combosEstruturaOrganizacionalPadrao.DdlUGE.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.combosEstruturaOrganizacionalPadrao.DdlUGE.SelectedValue);
                else
                    return 0;
            }
            set
            {
                bool isAdminGeral = false;

                isAdminGeral = IsAdminGeral();
                if (value > 0)
                {
                    if (isAdminGeral)
                    {
                        var ugesUO = (new Sam.Domain.Business.EstruturaOrganizacionalBusiness().ListarUgesTodosCodPorUo(this.UOId));
                        if (ugesUO.HasElements())
                        {
                            this.combosEstruturaOrganizacionalPadrao.DdlUGE.InserirElementoSelecione(true);
                            ugesUO.ToList().ForEach(ugeUO => this.combosEstruturaOrganizacionalPadrao.DdlUGE.Items.Add(new ListItem(String.Format("{0} - {1}", ugeUO.Codigo, ugeUO.Descricao), ugeUO.Id.Value.ToString())));
                        }
                    }

                    this.combosEstruturaOrganizacionalPadrao.DdlUGE.SelectedValue = value.ToString();
                }
            }
        }
        public int UAId
        {
            get
            {
                if (this.combosEstruturaOrganizacionalPadrao.DdlUA.Items.Count > 0)
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.combosEstruturaOrganizacionalPadrao.DdlUA.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.combosEstruturaOrganizacionalPadrao.DdlUA.SelectedValue = value == 0 ? string.Empty : value.ToString();
            }

        }
        public int DivisaoId
        {
            get
            {
                if (!String.IsNullOrEmpty(this.combosEstruturaOrganizacionalPadrao.DdlDivisao.SelectedValue))
                    return (int)Common.Util.TratamentoDados.TryParseInt32(this.combosEstruturaOrganizacionalPadrao.DdlDivisao.SelectedValue);
                else
                    return 0;
            }
            set
            {
                if (value > 0)
                    this.combosEstruturaOrganizacionalPadrao.DdlDivisao.SelectedValue = value == 0 ? string.Empty : value.ToString();
            }
        }

        public bool OcultaOrgao
        {
            set { this.combosEstruturaOrganizacionalPadrao.OcultaOrgao = value; }
        }
        public bool OcultaUO
        {
            set { this.combosEstruturaOrganizacionalPadrao.OcultaUO = value; }
        }
        public bool OcultaUGE
        {
            set { this.combosEstruturaOrganizacionalPadrao.OcultaUGE = value; }
        }

        private bool _cascatearDDLUA = true;
        private bool _cascatearDDLUGE = true;
        private bool _cascatearDDLUO = true;
        private bool _cascatearDDLOrgao = true;

        public bool CascatearDDLOrgao
        {
            get { return _cascatearDDLOrgao; }
            set { _cascatearDDLOrgao = value; }
        }
        public bool CascatearDDLUO
        {
            get { return _cascatearDDLUO; }
            set { _cascatearDDLUO = value; }
        }
        public bool CascatearDDLUGE
        {
            get { return _cascatearDDLUGE; }
            set { _cascatearDDLUGE = value; }
        }
        public bool CascatearDDLUA
        {
            get { return _cascatearDDLUA; }
            set { _cascatearDDLUA = value; }
        }
        private bool _cascatearDDLAlmoxarifado = true;
        public bool CascatearDDLAlmoxarifado
        {
            get { return _cascatearDDLAlmoxarifado; }
            set { _cascatearDDLAlmoxarifado = value; }
        }
        public bool PreservarComboboxValues
        {
            get { return this.combosEstruturaOrganizacionalPadrao.PreservarComboboxValues; }
            set { this.combosEstruturaOrganizacionalPadrao.PreservarComboboxValues = value; }
        }

        public bool BloqueiaListaOrgao
        {
            set { this.combosEstruturaOrganizacionalPadrao.BloqueiaOrgao = value; }
        }
        public bool BloqueiaListaUO
        {
            set { this.combosEstruturaOrganizacionalPadrao.BloqueiaUO = value; }
        }
        public bool BloqueiaListaUGE
        {
            set { this.combosEstruturaOrganizacionalPadrao.BloqueiaUGE = value; }
        }
        public bool BloqueiaListaUA
        {
            set { this.combosEstruturaOrganizacionalPadrao.BloqueiaUA = value; }
        }
        public bool BloqueiaListaDivisao
        {
            set { this.combosEstruturaOrganizacionalPadrao.BloqueiaDivisao = value; }
        }


        private void ConfigurarAlertDeConfirmacaoDDLs()
        {
            this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.combosEstruturaOrganizacionalPadrao.DdlUO.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.combosEstruturaOrganizacionalPadrao.DdlUA.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.combosEstruturaOrganizacionalPadrao.DdlUGE.Attributes.Add("onChange", "return ShowConfirm(this);");
            this.combosEstruturaOrganizacionalPadrao.DdlDivisao.Attributes.Add("onChange", "return ShowConfirm(this);");
        }
        private void ConfigurarAlertDeConfirmacaoBotoes()
        {
            btnGravar.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
            btnExcluir.Attributes.Add("OnClick", "return confirm('Pressione OK para confirmar.');");
        }

        private void SairModoEdicao()
        {
            RemoveSession(sessionChamadosSuporte);


            this.combosEstruturaOrganizacionalPadrao.DdlOrgao.Attributes.Remove("onChange");
            this.combosEstruturaOrganizacionalPadrao.DdlUO.Attributes.Remove("onChange");
            this.combosEstruturaOrganizacionalPadrao.DdlUA.Attributes.Remove("onChange");
            this.combosEstruturaOrganizacionalPadrao.DdlUGE.Attributes.Remove("onChange");
            this.combosEstruturaOrganizacionalPadrao.DdlDivisao.Attributes.Remove("onChange");
        }
        public void PrepararVisaoDeCombosPorPerfil(int perfilId)
        {
            bool exibirControlesEspecificosAdminGeral = false;
            int perfilID = perfilId;



            this.combosEstruturaOrganizacionalPadrao.ListarStatusAtendimentoChamados = true;
            this.combosEstruturaOrganizacionalPadrao.ShowStatus = true;
            this.combosEstruturaOrganizacionalPadrao.ShowNumeroRequisicao = false;

            this.combosEstruturaOrganizacionalPadrao.AlteraDescricaoLabelCampoStatus("Status (Usuário):");

            this.combosEstruturaOrganizacionalPadrao.OcultaDivisao = true;
            this.combosEstruturaOrganizacionalPadrao.OcultaUA = true;


            exibirControlesEspecificosAdminGeral = IsAdminGeral();
            this.combosEstruturaOrganizacionalPadrao.ListaPersonalizadaStatusAtendimentoChamados = gerarListaStatusAtendimentoUsuario();
            //this.combosEstruturaOrganizacionalPadrao.ListarListaPersonalizadaStatusAtendimentoChamados = exibirControlesEspecificosAdminGeral;
            this.combosEstruturaOrganizacionalPadrao.ListarListaPersonalizadaStatusAtendimentoChamados = true;
            this.BloqueiaPesquisaStatusAtendimentoChamadoUsuario = !exibirControlesEspecificosAdminGeral;

            //initOpcoesStatusAtendimentoUsuario();
        }

        private void initOpcoesStatusAtendimentoUsuario()
        {
            List<statusAtendimentoChamado> listaPersonalizadaStatus = null;
            var comboStatus = this.combosEstruturaOrganizacionalPadrao.DdlStatus;


            comboStatus.Items.Clear();
            listaPersonalizadaStatus = gerarListaStatusAtendimentoUsuario().ToList();
            listaPersonalizadaStatus.ForEach(statusAtendimento => this.combosEstruturaOrganizacionalPadrao.DdlStatus.Items.Add(new ListItem() { Value = ((int)statusAtendimento).ToString(), Text = GeralEnum.GetEnumDescription(statusAtendimento) }));

            comboStatus.InserirNovaOpcaoLista("Todos", 0);
        }
        private IList<statusAtendimentoChamado> gerarListaStatusAtendimentoUsuario()
        { return (new List<statusAtendimentoChamado>() { statusAtendimentoChamado.Aberto, statusAtendimentoChamado.Reaberto, statusAtendimentoChamado.Concluido }); }


        private void initOpcoesStatusAtendimentoProdesp()
        {
            List<statusAtendimentoChamado> listaPersonalizadaStatus = null;
            var comboStatus = this.ddlStatusChamadoAtendimentoProdesp;


            comboStatus.Items.Clear();
            listaPersonalizadaStatus = gerarListaStatusAtendimentoProdesp().ToList();
            listaPersonalizadaStatus.ForEach(statusAtendimento => comboStatus.Items.Add(new ListItem() { Value = ((int)statusAtendimento).ToString(), Text = GeralEnum.GetEnumDescription(statusAtendimento) }));

            // comboStatus.InserirNovaOpcaoLista("Todos", 0);
        }
        private IList<statusAtendimentoChamado> gerarListaStatusAtendimentoProdesp()
        { return (new List<statusAtendimentoChamado>() { statusAtendimentoChamado.Aberto, statusAtendimentoChamado.EmAtendimento, statusAtendimentoChamado.AguardandoRetornoUsuario, statusAtendimentoChamado.Finalizado, statusAtendimentoChamado.Reaberto }); }

        private void CarregarViewComDDLSelectedValues()
        {
            this.OrgaoId = Convert.ToInt32(Session["orgaoId"]);
            this.UOId = Convert.ToInt32(Session["uoId"]);
            this.UGEId = Convert.ToInt32(Session["ugeId"]);
            this.UAId = Convert.ToInt32(Session["uaId"]);
            this.DivisaoId = Convert.ToInt32(Session["divId"]);

            PageBase.RemoveSession("orgaoId");
            PageBase.RemoveSession("uoId");
            PageBase.RemoveSession("ugeId");
            PageBase.RemoveSession("uaId");
            PageBase.RemoveSession("divId");
        }
        private void PopulaSessionsComDDLSelectedValues()
        {
            PageBase.SetSession<int>(OrgaoId, "orgaoId");
            PageBase.SetSession<int>(UOId, "uoId");
            PageBase.SetSession<int>(UGEId, "ugeId");
            PageBase.SetSession<int>(UAId, "uaId");
            PageBase.SetSession<int>(DivisaoId, "divId");
            PageBase.SetSession<bool>(PreservarComboboxValues, "PreservarCombosValues");
        }

        #endregion

        protected void gdvAtendimentoChamados_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (IsAdminGeral())
                ((DataControlField)gdvAtendimentoChamados.Columns
                        .Cast<DataControlField>()
                        .Where(fld => fld.HeaderText == NOMECOLUNALOTE)
                        .SingleOrDefault()).Visible = true;
        }

        protected void btnEditarStatusLote_Click(object sender, EventArgs e)
        {

            limparLista(this._itensSelecionados);
            obterItemSelecionado(gdvAtendimentoChamados);
            if (this._itensSelecionados.Count == 0)
            {
                ExibirMensagem("Necessário selecionar os itens que terão o status alterado");
                return;
            }

            ChamadoSuportePresenter objPresenter = null;

            BloqueiaEdicaoChamado = false;
            BloqueiaUploaderArquivos = BloqueriaListaArquivos = true;
            this.NumeroChamado = 0;

            objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.StatusAtualizar();
        }

        private void obterItemSelecionado(GridView grid)
        {
            foreach (GridViewRow _row in grid.Rows)
            {
                if (((CheckBox)_row.FindControl(CHECKBOXATUALIZARSTATUS)).Checked)
                    _itensSelecionados.Add(_row);
            }
        }

        private void limparLista(IList lista)
        {
            lista.Clear();
        }

        protected void btnStatusAtualizar_Click(object sender, EventArgs e)
        {
            limparLista(this._itensSelecionados);
            obterItemSelecionado(gdvAtendimentoChamados);

            if (this._itensSelecionados.Count == 0)
            {
                ExibirMensagem("Necessário selecionar os itens que terão o status alterado");
                return;
            }

            List<ChamadoSuporteEntity> _chamadosEmLote = new List<ChamadoSuporteEntity>();

            #region Criar Lista dos Chamados a Serem Atualizados
            foreach (var _linha in this._itensSelecionados)
            {
                var _chamado = this.obterChamadoSuporte(_linha);

                _chamado.Responsavel = txtResponsavelStatusAtualizar.Text;
                _chamado.StatusChamadoAtendimentoProdesp = byte.Parse(ddlStatusAtualizar.SelectedValue);
                _chamado.Observacoes = txtObservacoesStatusAtualizar.Text;

                _chamadosEmLote.Add(_chamado);
            }
            #endregion

            ChamadoSuportePresenter _objPresenter = new ChamadoSuportePresenter();
            IEnumerable<string> _mensagemErro;
            if (_objPresenter.AtualizarStatus(_chamadosEmLote, this, out _mensagemErro))
            {
                IEnumerable<string> _chamados = _chamadosEmLote.Select(p => new { id = p.Id.Value.ToString() }).Select(n => n.id);
                this.ExibirMensagem(String.Format("Status dos Chamados Suporte [#{0:D7}] salvos com sucesso!", String.Join<string>(", ", _chamados)));
                this.PopularGrid();
                this.MostrarPainelEdicao = false;
                this.MostrarPainelStatusAtualizar = false;
                this.MostrarBotaoStatusAtualizar = true;
            }
            else
            {
                this.ExibirMensagem(string.Format("Registro com Inconsistências, verificar mensagens!\\n\\n{0}", string.Join("\\n", _mensagemErro)));
                this.MostrarPainelStatusAtualizar = true;
                this.MostrarBotaoStatusAtualizar = false;
                this.MostrarPainelEdicao = false;
            }

            selecionarDadosPesquisa();
        }

        protected void btnCancelarStatusAtualizar_Click(object sender, EventArgs e)
        {
            selecionarDadosPesquisa();
            ChamadoSuportePresenter objPresenter = new ChamadoSuportePresenter(this);
            objPresenter.Cancelar();

            MostrarPainelEdicao = false;
        }
    }

    public static class FileUploadExtension
    {
        public static byte[] _getAnexoUploaded(this FileUpload controlFileUpload)
        {
            System.IO.MemoryStream _retornoArquivo = null;
            using (System.IO.Stream anexoParaUpload = controlFileUpload.PostedFile.InputStream)
            {
                anexoParaUpload.CopyTo(_retornoArquivo);
            }

            return _retornoArquivo.ToArray();
        }
    }
    public static partial class ControlExtensions
    {
        public static int ObterIndiceValorTexto(this ListControl ctrlListaDeDados, string valorTextoPesquisa)
        {
            int idxRetorno = -1;
            ListItem itemPesquisado = null;

            var listaItens = ctrlListaDeDados.Items.Cast<ListItem>().ToList();

            if (listaItens.IsNotNullAndNotEmpty())
            {
                itemPesquisado = listaItens.Where(item => item.Text == valorTextoPesquisa).FirstOrDefault();

                if (itemPesquisado.IsNotNull())
                    Int32.TryParse(itemPesquisado.Value, out idxRetorno);
            }

            return idxRetorno;
        }
    }
}
