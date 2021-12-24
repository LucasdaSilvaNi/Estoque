using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sam.Domain.Entity;
using Sam.Presenter;
using Sam.View;
using System.Web.UI.HtmlControls;
using Sam.Web.Almoxarifado;
using Sam.Common.Util;
using Sam.Common.Enums.ChamadoSuporteEnums;


namespace Sam.Web.Controles
{
    public partial class ComboboxesHierarquiaPadrao : System.Web.UI.UserControl
    {
        #region Constructors

        public ComboboxesHierarquiaPadrao()
        {
        }

        public bool ExecutadaPorChamadoSuporte { get; set; }

        public ComboboxesHierarquiaPadrao(ICombosPadroesView _view)
        {
            this._view = _view;
        }

        #endregion

        #region Properties

        private string sessionListUAEntity = "listUAEntity";
        private string sessionListUGEEntity = "listUGEEntity";
        private string sessionListUOEntity = "listUOEntity";
        private string sessionListOrgaoEntity = "listOrgaoEntity";
        private static string cst_nome_combo_almoxarifados = "ddlAlmoxarifadosUGE";

        private IList<UAEntity> listUAEntity = new List<UAEntity>();
        private IList<UGEEntity> listUGEEntity = new List<UGEEntity>();
        private IList<UOEntity> listUOEntity = new List<UOEntity>();
        private IList<OrgaoEntity> listOrgaoEntity = new List<OrgaoEntity>();

        private ICombosPadroesView _view;
        public ICombosPadroesView View
        {
            get { return _view; }
            set { _view = value; }
        }

        public DropDownList DdlOrgao
        {
            get
            {
                return ddlOrgao;
            }
        }

        public DropDownList DdlUO
        {
            get
            {
                return ddlUO;
            }
        }

        public DropDownList DdlUGE
        {
            get
            {
                return ddlUGE;
            }

        }

        public DropDownList DdlUA
        {
            get
            {
                return ddlUA;
            }
        }

        public DropDownList DdlDivisao
        {
            get
            {
                return ddlDivisao;
            }
        }

        public DropDownList DdlPTRES
        {
            get
            {
                return ddlPTRES;
            }
        }

        public DropDownList DdlStatus
        {
            get
            {
                return ddlStatus;
            }
        }

        public DropDownList DdlStatusProdesp
        {
            get
            {
                return ddlStatusProdesp;
            }

        }

        public DropDownList DdlAlmoxarifado
        {
            get { return ((DropDownList)this.Controls[1].FindControl(cst_nome_combo_almoxarifados)); }
        }

        public string TextNumeroRequisicao
        {
            get
            {
                return txtNumRequisicao.Text;
            }
        }


        private bool _bloqueiaOrgao = false;
        public bool BloqueiaOrgao
        {
            set
            {
                _bloqueiaOrgao = value;
                ddlOrgao.Enabled = !_bloqueiaOrgao;
            }
        }

        private bool _bloqueiaUO;
        public bool BloqueiaUO
        {
            set
            {
                _bloqueiaUO = value;
                ddlUO.Enabled = !_bloqueiaUO;
            }
        }

        private bool _bloqueiaUGE;
        public bool BloqueiaUGE
        {
            set
            {
                _bloqueiaUGE = value;
                ddlUGE.Enabled = !_bloqueiaUGE;
            }
        }

        public bool BloqueiaUA
        {
            set { ddlUA.Enabled = !value; }
        }

        public bool BloqueiaDivisao
        {
            set { ddlDivisao.Enabled = !value; }
        }

        public bool BloqueiaPTRES
        {
            set { ddlPTRES.Enabled = !value; }
        }


        private bool _orgaoText = false;
        private bool _orgaoDdl = false;
        public bool OcultaOrgao
        {
            set
            {
                _orgaoText = value;
                _orgaoDdl = value;
                this.lblOrgao.Visible = !_orgaoText;
                this.ddlOrgao.Visible = !_orgaoDdl;
            }
        }

        private bool _uoText = false;
        private bool _uoDdl = false;
        public bool OcultaUO
        {
            set
            {
                _uoText = value;
                _uoDdl = value;
                this.lblUO.Visible = !_uoText;
                this.ddlUO.Visible = !_uoDdl;
            }
        }

        private bool _ugeText = false;
        private bool _ugeDdl = false;
        public bool OcultaUGE
        {
            set
            {
                _ugeText = value;
                _ugeDdl = value;
                this.lblUge.Visible = !_ugeText;
                this.ddlUGE.Visible = !_ugeDdl;
            }
        }

        private bool _uaText = false;
        private bool _uaDdl = false;
        public bool OcultaUA
        {
            set
            {
                _uaText = value;
                _uaDdl = value;
                this.lblUA.Visible = !_uaText;
                this.ddlUA.Visible = !_uaDdl;
            }
        }

        private bool _divisaoText = false;
        private bool _divisaoDdl = false;
        public bool OcultaDivisao
        {
            set
            {
                _divisaoText = value;
                _divisaoDdl = value;
                this.lblDivisao.Visible = !_divisaoText;
                this.ddlDivisao.Visible = !_divisaoDdl;
            }
        }

        private bool _ptResText = false;
        private bool _ptResDdl = false;
        public bool OcultaPTRES
        {
            set
            {
                _ptResText = value;
                _ptResDdl = value;
                this.lblPTRES.Visible = !_ptResText;
                this.ddlPTRES.Visible = !_ptResDdl;
            }
        }

        private bool _reqStatusText = false;
        private bool _reqStatusDDL = false;
        public bool ShowStatus
        {
            set
            {
                _reqStatusText = value;
                _reqStatusDDL = value;
                this.lblStatus.Visible = _reqStatusText;
                this.ddlStatus.Visible = _reqStatusDDL;
            }
        }

        private bool _numeroRequisicaoText = false;
        private bool _numeroRequisicaoDDL = false;
        public bool ShowNumeroRequisicao
        {
            set
            {
                _numeroRequisicaoText = value;
                _numeroRequisicaoDDL = value;
                this.lblNumeroRequisicao.Visible = _numeroRequisicaoText;                
                this.txtNumRequisicao.Visible = _numeroRequisicaoDDL;
                if (value)
                {
                    //this.search2.Attributes["class"] = "mostrarControle";
                    this.search2.Visible = true;
                }
                else
                {
                    //this.search2.Attributes["class"] = "esconderControle";
                    //this.search2.Attributes["style"] = "display: block;";
                    this.search2.Visible = false;
                }
            }
        }

        private bool _isRetornoPageConsulta;
        public bool PreservarComboboxValues
        {
            get
            {
                return _isRetornoPageConsulta;
            }
            set
            {
                _isRetornoPageConsulta = value;
            }
        }

        private bool _bindStatusAtendimento = false;
        public bool ListarStatusAtendimentoChamados
        {
            get { return _bindStatusAtendimento; }
            set { _bindStatusAtendimento = value; }
        }

        private bool _bindListaPersonalizadaStatusAtendimento = false;
        public bool ListarListaPersonalizadaStatusAtendimentoChamados
        {
            get { return _bindListaPersonalizadaStatusAtendimento; }
            set { _bindListaPersonalizadaStatusAtendimento = value; }
        }

        private IList<StatusAtendimentoChamado> _listaStatusPersonalizada = null;
        public IList<StatusAtendimentoChamado> ListaPersonalizadaStatusAtendimentoChamados
        {
            get { return _listaStatusPersonalizada; }
            set { _listaStatusPersonalizada = value; }
        }

        private bool _insereOpcaoTodos = false;
        public bool InserirOpcaoPesquisarTodos
        {
            get { return _insereOpcaoTodos; }
            set { _insereOpcaoTodos = value; }
        }

        EventHandler eventoBotaoPesquisar;
        #endregion


        #region Controles

        protected void Page_Load(object sender, EventArgs e)
        {
            OcultaPTRES = true;

            //Setados nas paginas que usam o userControl.
            //ShowStatus = true;
            //ShowNumeroRequisicao = true;

            var combosPresenter = new CombosPadroesPresenter(this.View);
            combosPresenter.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;

            if (!IsPostBack)
            {
                combosPresenter.Load();
                //this.BindComboOrgao(combosPresenter.PopularListaOrgaoTodosCod());

                listOrgaoEntity = combosPresenter.PopularListaOrgaoTodosCod();

                if (_insereOpcaoTodos)
                {
                    if (listOrgaoEntity.Count > 1)
                        listOrgaoEntity.Insert(0, (new OrgaoEntity() { Id = 0, Descricao = "Todos" }));
                }

                //Add to ViewState.
                AddViewState(sessionListOrgaoEntity, listOrgaoEntity);
                this.BindComboOrgao(listOrgaoEntity);


                if (_bindListaPersonalizadaStatusAtendimento && _listaStatusPersonalizada.HasElements())
                    BindComboStatus(_listaStatusPersonalizada);
                else if (_bindStatusAtendimento)
                    BindComboStatus();
                else
                    BindComboStatus(new CombosPadroesPresenter(this.View).PopularComboStatusRequisicao());

                BindComboStatusProdesp();

                this.PreservarComboboxValues = false;
            }
            
        }

        protected void ddlOrgao_DataBound(object sender, EventArgs e)
        {
            if (!_view.CascatearDDLOrgao) return;
            //CombosPadroesPresenter combosPadroesPresenter = new CombosPadroesPresenter(_view);
            //BindComboUO(combosPadroesPresenter.PopularComboUO());

            var _comboPadroes = new CombosPadroesPresenter(_view);
            _comboPadroes.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;
            listUOEntity = _comboPadroes.PopularComboUO();

            if (_insereOpcaoTodos)
            {
                if (listUOEntity.Count > 1)
                    listUOEntity.Insert(0, (new UOEntity() { Id = 0, Descricao = "Todos" }));
            }

            AddViewState(sessionListUOEntity, listUOEntity);            
            BindComboUO(this.listUOEntity);
        }

        protected void ddlOrgao_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CombosPadroesPresenter combosPadroesPresenter = new CombosPadroesPresenter(_view);
            //BindComboUO(combosPadroesPresenter.PopularComboUO());

            var _comboPadroes = new CombosPadroesPresenter(_view);
            _comboPadroes.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;
            listUOEntity = _comboPadroes.PopularComboUO();

            if (_insereOpcaoTodos)
            {
                if (listUOEntity.Count > 1)
                    listUOEntity.Insert(0, (new UOEntity() { Id = 0, Descricao = "Todos" }));
            }

            AddViewState(sessionListUOEntity, listUOEntity);
            BindComboUO(listUOEntity);
        }

        protected void ddlUO_DataBound(object sender, EventArgs e)
        {
            if (!_view.CascatearDDLUO) return;
            //CombosPadroesPresenter combosPadroesPresenter = new CombosPadroesPresenter(_view);
            //BindComboUGE(combosPadroesPresenter.PopularComboUGE());

            var _comboPadroes = new CombosPadroesPresenter(_view);
            _comboPadroes.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;
            listUGEEntity = _comboPadroes.PopularComboUGE();

            if (_insereOpcaoTodos)
            {
                if (listUGEEntity.Count > 1 || (this.DdlUO != null && this.DdlUO.SelectedItem != null && this.DdlUO.SelectedItem.Value == "0"))
                    listUGEEntity.Insert(0, (new UGEEntity() { Id = 0, Descricao = "Todos" }));
            }

            AddViewState(sessionListUGEEntity, listUGEEntity);
            BindComboUGE(listUGEEntity);
        }

        protected void ddlUO_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CombosPadroesPresenter combosPadroesPresenter = new CombosPadroesPresenter(_view);
            //BindComboUGE(combosPadroesPresenter.PopularComboUGE());

            listUGEEntity = new CombosPadroesPresenter(_view).PopularComboUGE();

            if (_insereOpcaoTodos)
            {
                if (listUGEEntity.Count > 1)
                    listUGEEntity.Insert(0, (new UGEEntity() { Id = 0, Descricao = "Todos" }));
            }

            AddViewState(sessionListUGEEntity, listUGEEntity);
            BindComboUGE(listUGEEntity);
        }

        protected void ddlUGE_DataBound(object sender, EventArgs e)
        {
            if (!_view.CascatearDDLUGE) return;

            CombosPadroesPresenter combosPadroesPresenter = new CombosPadroesPresenter(_view);
            combosPadroesPresenter.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;

            listUAEntity = combosPadroesPresenter.PopularComboUA();
            IList<AlmoxarifadoEntity> listAlmoxarifadoEntity = combosPadroesPresenter.PopularComboAlmoxarifado();

            if (_insereOpcaoTodos)
            {
                if (listUAEntity.Count > 1)
                    listUAEntity.Insert(0, (new UAEntity() { Id = 0, Descricao = "Todos" }));
                    //listAlmoxarifadoEntity.Insert(0, (new AlmoxarifadoEntity() { Id = 0, Descricao = "Todos" }));
            }

            BindComboUA(listUAEntity);
            BindComboAlmoxarifado(listAlmoxarifadoEntity);
        }

        protected void ddlUGE_SelectedIndexChanged(object sender, EventArgs e)
        {
            CombosPadroesPresenter requisicaoPresenter = new CombosPadroesPresenter(_view);
            requisicaoPresenter.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;

            listUAEntity = requisicaoPresenter.PopularComboUA();
            IList<AlmoxarifadoEntity> listAlmoxarifadoEntity = requisicaoPresenter.PopularComboAlmoxarifado();

            if (_insereOpcaoTodos)
            {
                if (listUAEntity.Count > 1)
                    listUAEntity.Insert(0, (new UAEntity() { Id = 0, Descricao = "Todos" }));

                if (listAlmoxarifadoEntity.Count > 1)
                    listAlmoxarifadoEntity.Insert(0, (new AlmoxarifadoEntity() { Id = 0, Descricao = "Todos" }));
            }

            BindComboUA(listUAEntity);
            BindComboAlmoxarifado(listAlmoxarifadoEntity);

            UGEEntity ugeEntity = new UGEPresenter().ListarUGEById(_view.UGEId).FirstOrDefault();
            if (ugeEntity == null) return;

            //listUOEntity = GetViewState(sessionListUOEntity, listUOEntity) ?? requisicaoPresenter.PopularComboUO();
            //listOrgaoEntity = GetViewState(sessionListOrgaoEntity, listOrgaoEntity) ?? requisicaoPresenter.PopularListaOrgaoTodosCod();

            //OrdenaDDLPai(ddlUO, listUOEntity, ugeEntity.Uo.Id);
            //OrdenaDDLPai(ddlOrgao, listOrgaoEntity, ugeEntity.Orgao.Id);

            //DropDownList ddlAlmoxarifados = (DropDownList)this.FindControl(cst_nome_combo_almoxarifados);
            //if (ddlAlmoxarifados.IsNotNull())
            //    OrdenaDDLPai(ddlAlmoxarifados, listAlmoxarifadoEntity, ugeEntity.Id);

        }

        //TODO: Terminar de implementar cascateamento com combos de UA/Divisão.
        protected void ddlAlmoxarifados_DataBound(object sender, EventArgs e)
        {
            if (!_view.CascatearDDLAlmoxarifado) return;
        }
        //TODO: Terminar de implementar cascateamento com combos de UA/Divisão.
        protected void ddlAlmoxarifados_SelectedIndexChanged(object sender, EventArgs e)
        { }

        protected void ddlUA_DataBound(object sender, EventArgs e)
        {
            if (!_view.CascatearDDLUA) return;

            CombosPadroesPresenter combosPadoresPresenter = new CombosPadroesPresenter(_view);
            int _uaId;
            int.TryParse(ddlUA.SelectedValue, out _uaId);

            var listDivisaoEntity = combosPadoresPresenter.PopularDivisao(_uaId);

            if (_insereOpcaoTodos)
            {
                if (listDivisaoEntity.Count > 1)
                    listDivisaoEntity.Insert(0, (new DivisaoEntity() { Id = 0, Descricao = "Todos" }));
            }

            if (listDivisaoEntity != null && listDivisaoEntity.Count > 0)
                BindComboDivisao(listDivisaoEntity);
        }

        protected void ddlUA_SelectedIndexChanged(object sender, EventArgs e)
        {
            CombosPadroesPresenter requisicao = new CombosPadroesPresenter(_view);
            requisicao.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;

            int _uaId;
            int.TryParse(ddlUA.SelectedValue, out _uaId);

            var listDivisaoEntity = requisicao.PopularDivisao(_uaId);

            if (_insereOpcaoTodos)
            {
                if (listDivisaoEntity.Count > 1)
                    listDivisaoEntity.Insert(0, (new DivisaoEntity() { Id = 0, Descricao = "Todos" }));
            }

            if (listDivisaoEntity != null && listDivisaoEntity.Count > 0)
                BindComboDivisao(listDivisaoEntity);

            //IMPLEMENTAR - Atualização de DDL acima (buscar dados do ViewState - lista carregada anteriormente).            
            //UAEntity uaEntity = new UAPresenter().ListarUaById(_view.UAId).FirstOrDefault();
            //if (uaEntity == null) return;

            //listUGEEntity = GetViewState(sessionListUGEEntity, listUGEEntity) ?? requisicao.PopularComboUGE();
            //listUOEntity = GetViewState(sessionListUOEntity, listUOEntity) ?? requisicao.PopularComboUO();
            //listOrgaoEntity = GetViewState(sessionListOrgaoEntity, listOrgaoEntity) ?? requisicao.PopularListaOrgaoTodosCod();

            //OrdenaDDLPai(ddlUGE, listUGEEntity, uaEntity.Uge.Id);
            //OrdenaDDLPai(ddlUO, listUOEntity, uaEntity.Uo.Id);
            //OrdenaDDLPai(ddlOrgao, listOrgaoEntity, uaEntity.Orgao.Id);
        }

        protected void ddlDivisao_DataBound(object sender, EventArgs e)
        {}

        protected void ddlDivisao_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Atualizar DDL acima.                
            CombosPadroesPresenter requisicao = new CombosPadroesPresenter(_view);
            requisicao.ExecutadaPorChamadoSuporte = ExecutadaPorChamadoSuporte;

            DivisaoEntity divisaoEntity = new DivisaoPresenter().ListarDivisaoById(_view.DivisaoId).SingleOrDefault();
            if (divisaoEntity == null) return;

            //int uaId = Convert.ToInt32(divisaoEntity.Ua.Id);
            //UAEntity uaEntity = new UAPresenter().ListarUaById(uaId).FirstOrDefault();
            //listUAEntity = GetViewState(sessionListUAEntity, listUAEntity) ?? requisicao.PopularComboUA();
            //listUGEEntity = GetViewState(sessionListUGEEntity, listUGEEntity) ?? requisicao.PopularComboUGE();
            //listUOEntity = GetViewState(sessionListUOEntity, listUOEntity) ?? requisicao.PopularComboUO();
            //listOrgaoEntity = GetViewState(sessionListOrgaoEntity, listOrgaoEntity) ?? requisicao.PopularListaOrgaoTodosCod();

            //OrdenaDDLPai(ddlUA, listUAEntity, uaEntity.Id);
            //OrdenaDDLPai(ddlUGE, listUGEEntity, uaEntity.Uge.Id);
            //OrdenaDDLPai(ddlUO, listUOEntity, uaEntity.Uo.Id);
            //OrdenaDDLPai(ddlOrgao, listOrgaoEntity, uaEntity.Orgao.Id);

            //Substituição por método genérico
            //OrdenaDDLPai<UAEntity>(ddlUA, listUAEntity, uaEntity.Id);
            //OrdenaDDLPai<UGEEntity>(ddlUGE, listUGEEntity, uaEntity.Uge.Id);
            //OrdenaDDLPai<UOEntity>(ddlUO, listUOEntity, uaEntity.Uo.Id);
            //OrdenaDDLPai<OrgaoEntity>(ddlOrgao, listOrgaoEntity, uaEntity.Orgao.Id);            
        }

        protected void ddlPTRES_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void ddlPTRES_DataBound(object sender, EventArgs e)
        {
        }

        public delegate void StatusChangedEventHandler(object sender, EventArgs e);
        public event StatusChangedEventHandler StatusChanged;
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.StatusChanged != null) 
                this.StatusChanged(sender, e);
        }
                

        #endregion


        #region Métodos

        #region BindsCombos
        private bool verificarDropDownVazio(DropDownList dropDown)
        {
            return !(dropDown.Items.Count < 1 || (dropDown.Items.Count == 1 && string.IsNullOrEmpty(dropDown.Items[0].Text))) ? false : dropDown.Items.FindByText("Todos") == null;
        }

        public void BindComboOrgao(IList<OrgaoEntity> listOrgaos)
        {
            ddlOrgao.DataSource = listOrgaos;
            ddlOrgao.DataBind();

            if (!_view.CascatearDDLOrgao || verificarDropDownVazio(ddlOrgao))
            {
                ddlOrgao.Items.Insert(0, new ListItem("Todos", "0"));
                ddlOrgao.SelectedIndex = 0;
            }
        }

        private void BindComboUO(IList<UOEntity> listUOs)
        {            
            ddlUO.DataSource = listUOs;            
            ddlUO.DataBind();

            if (!_view.CascatearDDLUO || verificarDropDownVazio(ddlUO))
            {
                ddlUO.Items.Insert(0, new ListItem("Todos", "0"));
                ddlUO.SelectedIndex = 0;
            }
        }

        private void BindComboUGE(IList<UGEEntity> listUGEs)
        {
            ddlUGE.DataSource = listUGEs;
            ddlUGE.DataBind();

            if (!_view.CascatearDDLUGE || verificarDropDownVazio(ddlUGE)) //ddlUGE.Items.Count <= 1)
            {
                ddlUGE.Items.Insert(0, new ListItem("Todos", "0"));
                ddlUGE.SelectedIndex = 0;
            }
        }

        private void BindComboUA(IList<UAEntity> listUAs)
        {            
            ddlUA.DataSource = listUAs;
            ddlUA.DataBind();

            if (!_view.CascatearDDLUA || verificarDropDownVazio(ddlUA))
            {
                ddlUA.Items.Insert(0, new ListItem("Todos", "0"));
                ddlUA.SelectedIndex = 0;
            }
        }

        private void BindComboDivisao(IList<DivisaoEntity> listDivisoes)
        {
            ddlDivisao.DataSource = listDivisoes;
            ddlDivisao.DataBind();
        }

        private void BindComboStatus(IList<TipoMovimentoEntity> listTipoMovimento)
        {
            ddlStatus.DataSource = listTipoMovimento;
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("Todos"));
        }

        private void BindComboStatus(IList<StatusAtendimentoChamado> lstStatusAtendimentoChamado)
        {
            IList<ListItem> listaStatusAtendimento = new List<ListItem>();


            foreach (StatusAtendimentoChamado valorEnum in lstStatusAtendimentoChamado)
                listaStatusAtendimento.Add(new ListItem(GeralEnum.GetEnumDescription(valorEnum), ((int)valorEnum).ToString(), true));

            listaStatusAtendimento.Insert(0, new ListItem("Todos", "0", true));

            ddlStatus.Items.Clear();
            ddlStatus.Items.AddRange(listaStatusAtendimento.ToArray());
        }

        private void BindComboStatus()
        {
            IList<ListItem> listaStatusAtendimento = new List<ListItem>();


            foreach (StatusAtendimentoChamado valorEnum in Enum.GetValues(typeof(StatusAtendimentoChamado)))
                listaStatusAtendimento.Add(new ListItem(GeralEnum.GetEnumDescription(valorEnum), ((int)valorEnum).ToString(), true));

            listaStatusAtendimento.Insert(0, new ListItem("Todos", "0", true));

            ddlStatus.Items.Clear();
            ddlStatus.Items.AddRange(listaStatusAtendimento.ToArray());
        }

        private void BindComboStatusProdesp()
        {
            IList<ListItem> listaStatusAtendimento = new List<ListItem>();


            foreach (StatusAtendimentoChamado valorEnum in gerarListaStatusAtendimentoProdesp())
                listaStatusAtendimento.Add(new ListItem(GeralEnum.GetEnumDescription(valorEnum), ((int)valorEnum).ToString(), true));

            ddlStatusProdesp.Items.Clear();
            ddlStatusProdesp.Items.AddRange(listaStatusAtendimento.ToArray());

            ddlStatusProdesp.InserirNovaOpcaoLista("Todos", 0);
        }

        private IList<StatusAtendimentoChamado> gerarListaStatusAtendimentoProdesp()
        { return (new List<StatusAtendimentoChamado>() { StatusAtendimentoChamado.Aberto, StatusAtendimentoChamado.EmAtendimento, StatusAtendimentoChamado.AguardandoRetornoUsuario, StatusAtendimentoChamado.Finalizado, StatusAtendimentoChamado.Reaberto }); }

        private void BindComboAlmoxarifado(IList<AlmoxarifadoEntity> listAlmoxarifados)
        {
            DropDownList ddlAlmoxarifado = (DropDownList)this.Controls[1].FindControl(cst_nome_combo_almoxarifados);
            if (ddlAlmoxarifado.IsNotNull())
            {
                ddlAlmoxarifado.Items.Clear();
                ddlAlmoxarifado.DataValueField = "Id";
                ddlAlmoxarifado.DataTextField = "CodigoDescricao";
                ddlAlmoxarifado.DataSource = listAlmoxarifados;
                ddlAlmoxarifado.DataBind();

                if (!_view.CascatearDDLAlmoxarifado || verificarDropDownVazio(ddlAlmoxarifado))
                    ddlAlmoxarifado.Items.Insert(0, new ListItem("Todos", "0"));
            }
        }
        #endregion


        #region Ordena DDLs acima (PAI)

        private void OrdenaDDLPai(DropDownList ddl, IList<UAEntity> listEntity, int? id)
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.Where(x => x.Id == id).FirstOrDefault().Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;                
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        private void OrdenaDDLPai(DropDownList ddl, IList<UGEEntity> listEntity, int? id)
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.Where(x => x.Id == id).FirstOrDefault().Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        private void OrdenaDDLPai(DropDownList ddl, IList<UOEntity> listEntity, int? id)
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.FirstOrDefault(x => x.Id == id).Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void OrdenaDDLPai(DropDownList ddl, IList<OrgaoEntity> listEntity, int? id)
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.FirstOrDefault(x => x.Id == id).Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private void OrdenaDDLPai<T>(DropDownList ddl, IList<T> listEntity, int? id) where T : BaseEntity
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.Where(x => x.Id == id).FirstOrDefault().Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }

        }

        private void OrdenaDDLPai(DropDownList ddl, IList<AlmoxarifadoEntity> listEntity, int? id)
        {
            try
            {
                ddl.SelectedItem.Value = listEntity.Where(x => x.Id == id).FirstOrDefault().Id.ToString();
                ddl.SelectedItem.Text = listEntity.FirstOrDefault(x => x.Id == id).Descricao;
            }
            catch (Exception ex)
            {
                new LogErroPresenter().GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }

        }
        #endregion

        private void AddViewState<T>(string key, IList<T> listaCacheada)
        {
            ViewState[key] = listaCacheada;
        }

        private IList<T> GetViewState<T>(string key, IList<T> listaPopuladaBaseDeDados)
        {
            listaPopuladaBaseDeDados = (IList<T>)ViewState[key]; //null
            return listaPopuladaBaseDeDados;
        }

        public void InsereCampoDePesquisa(EventHandler eventoBotaoPesquisa, string labelNomeCampoPesquisa)
        {
            #region InstanciaComponentes

            HtmlGenericControl divSearch = new HtmlGenericControl();
            divSearch.ID = "pSearch";
            divSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            divSearch.TagName = "p";

            HtmlGenericControl tableSearch = new HtmlGenericControl();
            tableSearch.ID = "tableSearch";
            tableSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            tableSearch.TagName = "table";
            tableSearch.Attributes["border"] = "0";
            tableSearch.Attributes["width"] = "100%";

            HtmlGenericControl trSearch = new HtmlGenericControl();
            trSearch.TagName = "tr";

            HtmlGenericControl tdSearch = new HtmlGenericControl();
            tdSearch.TagName = "td";
            tdSearch.Attributes["width"] = "80%";
            tdSearch.Attributes["style"] = "text-align:left;";

            Label labelSearch = new Label();
            labelSearch.ID = "labelPesquisar";
            labelSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            labelSearch.Attributes["class"] = "labelFormulario";
            labelSearch.Text = String.Format("{0}:", labelNomeCampoPesquisa);

            Label literalSearch = new Label();
            literalSearch.Text = "&nbsp";

            TextBox textboxSearch = new TextBox();
            textboxSearch.ID = "txtPesquisar";
            textboxSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            textboxSearch.Attributes["type"] = "text";
            textboxSearch.Attributes["style"] = "height:20px; line-height:22px;";
            textboxSearch.Attributes["style"] = "width:20%;";

            Button buttonSearch = new Button();
            buttonSearch.ID = "btnPesquisar";
            buttonSearch.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            buttonSearch.Attributes["type"] = "submit";
            buttonSearch.Attributes["style"] = "width:120px";
            buttonSearch.Attributes["class"] = "button";
            buttonSearch.Text = "Pesquisar";

            if (eventoBotaoPesquisa.IsNotNull())
                buttonSearch.Click += eventoBotaoPesquisa;

            #endregion

            #region AdicionaComponentes

            tdSearch.Controls.Add(labelSearch);
            tdSearch.Controls.Add(textboxSearch);
            tdSearch.Controls.Add(literalSearch);
            tdSearch.Controls.Add(buttonSearch);

            trSearch.Controls.Add(tdSearch);

            tableSearch.Controls.Add(trSearch);

            Panel pElement = (Panel)this.Controls[1].FindControl("panelSearch");
            pElement.Attributes["class"] = "panelSearch";
            pElement.Controls.Add(tableSearch);

            #endregion
        }

        public void InsereComboStatusProdesp()
        {
            painelStatusProdesp.CssClass = "mostrarControle";
        }

        public void InsereComboAlmoxarifados()
        {
            #region InstanciaComponentes

            HtmlGenericControl divComboAlmoxarifados = new HtmlGenericControl();
            divComboAlmoxarifados.ID = "pComboAlmoxarifados";
            divComboAlmoxarifados.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            divComboAlmoxarifados.TagName = "p";

            HtmlGenericControl tableComboAlmoxarifados = new HtmlGenericControl();
            tableComboAlmoxarifados.ID = "tableComboAlmoxarifados";
            tableComboAlmoxarifados.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            tableComboAlmoxarifados.TagName = "table";
            tableComboAlmoxarifados.Attributes["border"] = "0";
            tableComboAlmoxarifados.Attributes["width"] = "100%";

            HtmlGenericControl trSearch = new HtmlGenericControl();
            trSearch.TagName = "tr";

            HtmlGenericControl tdComboAlmoxarifados = new HtmlGenericControl();
            tdComboAlmoxarifados.TagName = "td";
            tdComboAlmoxarifados.Attributes["width"] = "80%";
            tdComboAlmoxarifados.Attributes["style"] = "text-align:left;";

            Label lblComboAlmoxarifados = new Label();
            lblComboAlmoxarifados.ID = "lblComboAlmoxarifados";
            lblComboAlmoxarifados.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            lblComboAlmoxarifados.Attributes["class"] = "labelFormulario";
            lblComboAlmoxarifados.Attributes["style"] = "width: 118px;font-size: 13px;";
            lblComboAlmoxarifados.Text = "Almoxarifado:";

            Label literalComboAlmoxarifados = new Label();
            literalComboAlmoxarifados.Text = "&nbsp";

            DropDownList ddlAlmoxarifados = new DropDownList();
            ddlAlmoxarifados.ID = cst_nome_combo_almoxarifados;
            ddlAlmoxarifados.ClientIDMode = System.Web.UI.ClientIDMode.Static;
            ddlAlmoxarifados.DataValueField = "Id";
            ddlAlmoxarifados.DataTextField = "Descricao";
            ddlAlmoxarifados.Attributes["style"] = "width:80.25%;";


            #endregion

            #region AdicionaComponentes

            tdComboAlmoxarifados.Controls.Add(lblComboAlmoxarifados);
            tdComboAlmoxarifados.Controls.Add(ddlAlmoxarifados);
            tdComboAlmoxarifados.Controls.Add(literalComboAlmoxarifados);

            trSearch.Controls.Add(tdComboAlmoxarifados);
            tableComboAlmoxarifados.Controls.Add(trSearch);

            Panel pElement = (Panel)this.Controls[1].FindControl("panelComboAlmoxarifados");
            pElement.Attributes["class"] = "panelComboAlmoxarifados";
            pElement.Controls.Add(tableComboAlmoxarifados);

            #endregion
        }

        public void RemoveCampoDePesquisa()
        {
            #region RemoveComponentes

            HtmlGenericControl tableSearch = new HtmlGenericControl();

            Panel pElement = (Panel)this.Controls[1].FindControl("panelSearch");
            pElement.Attributes["class"] = "panelSearch";

            tableSearch = (HtmlGenericControl)this.Controls[1].FindControl("tableSearch");
            pElement.Controls.Remove(tableSearch);

            #endregion
        }

        public void AlteraDescricaoLabelCampoStatus(string novoLabel)
        {
            if (!String.IsNullOrWhiteSpace(novoLabel))
                this.lblStatus.Text = novoLabel;
        }
        #endregion



        public bool ExisteChamadoSuporteID
        {
            get 
            {
                bool blnRetorno = false;


                TextBox txtPesquisar = (TextBox)this.Controls[1].FindControl("txtPesquisar");
                blnRetorno = (txtPesquisar.IsNotNull() && !String.IsNullOrWhiteSpace(txtPesquisar.Text));

                return blnRetorno;
            }
        }

        public bool ExisteComboAlmoxarifado
        {
            get
            {
                bool blnRetorno = false;


                DropDownList ddlAlmoxarifado = (DropDownList)this.Controls[1].FindControl(cst_nome_combo_almoxarifados);
                blnRetorno = (ddlAlmoxarifado.IsNotNull() && ddlAlmoxarifado.Items.Count > 0);

                return blnRetorno;
            }
        }
    }
}
