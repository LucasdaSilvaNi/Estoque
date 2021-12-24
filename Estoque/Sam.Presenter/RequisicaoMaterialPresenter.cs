using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Business.SIAFEM;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using Sam.View;
using Sam.Common.Enums;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;



namespace Sam.Presenter
{
    public class RequisicaoMaterialPresenter : CrudPresenter<IRequisicaoMaterialView>
    {
        #region Constructors

        public RequisicaoMaterialPresenter()
        {
        }

        public RequisicaoMaterialPresenter(IRequisicaoMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        #endregion

        #region Properties
        private MovimentoEntity movimentoEditados { get; set; }

        IRequisicaoMaterialView view;
        public IRequisicaoMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        int PerfilId;
        int LoginId;
        public IEnumerable<object> listaUOSByPerfil { get; set; }

        #endregion

        #region Methods

        public TB_MOVIMENTO SelectOneMovimento(int movimentoId)
        {
            Business.MovimentoBusiness business = new Business.MovimentoBusiness();
            return business.SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);
        }

        public MovimentoEntity RemoverSubItem(MovimentoEntity movimento)
        {
            MovimentoItemEntity itemDelete = new MovimentoItemEntity();

            foreach (var item in movimento.MovimentoItem)
            {
                if (Common.Util.TratamentoDados.TryParseLong(this.View.Id) == item.Id)
                    itemDelete = item;

            }
            if (itemDelete.Id != null)
                movimento.MovimentoItem.Remove(itemDelete);

            return movimento;
        }

        public MovimentoEntity EditSubItem(MovimentoEntity movimento, PTResEntity ptres = null)
        {
            MovimentoItemEntity itemEdit = new MovimentoItemEntity();

            for (int i = 0; i < movimento.MovimentoItem.Count; i++)
            {
                if (Common.Util.TratamentoDados.TryParseLong(this.View.Id) == movimento.MovimentoItem[i].Id)
                {
                    movimento.MovimentoItem[i].SubItemMaterial.Id = this.View.SubItemId;
                    movimento.MovimentoItem[i].SubItemMaterial.Codigo = (long)Common.Util.TratamentoDados.TryParseLong(this.View.Codigo);
                    movimento.MovimentoItem[i].SubItemMaterial.Descricao = this.View.Descricao;
                    movimento.MovimentoItem[i].QtdeLiq = this.View.Quantidade;
                    //movimento.MovimentoItem[i].PTRes = new PTResEntity() { Id = this.View.PTResId, Codigo = this.View.PTResCodigo, Descricao = this.View.PTResDescricao, ProgramaTrabalho = new ProgramaTrabalho(this.View.PTAssociadoPTRes) };
                    movimento.MovimentoItem[i].PTRes = ptres;
                }
            }

            return movimento;
        }

        public MovimentoItemEntity EditSubItemRequisicao(MovimentoEntity movimento, PTResEntity ptres = null)
        {
            MovimentoItemEntity itemEdit = null;

            foreach (MovimentoItemEntity item in movimento.MovimentoItem)
            {
                if (Common.Util.TratamentoDados.TryParseLong(this.View.Id) == item.Id)
                {
                    itemEdit = item;
                    itemEdit.SubItemMaterial.Id = this.View.SubItemId;
                    itemEdit.SubItemMaterial.Codigo = (long)Common.Util.TratamentoDados.TryParseLong(this.View.Codigo);
                    itemEdit.SubItemMaterial.Descricao = this.View.Descricao;
                    itemEdit.QtdeLiq = this.View.Quantidade;
                   
                    itemEdit.PTRes = ptres;

                    movimento.MovimentoItem.Remove(item);
                    break;
                }
            }

            return itemEdit;
           
        }
        public IList<MovimentoItemEntity> ListarNotaRequisicao(int movimentoId)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            return estrutura.ListarNotaRequisicao(movimentoId);
        }

        public MovimentoEntity AdicionarSubItem(MovimentoEntity movimento, PTResEntity ptRes, bool verificaTipoMaterial = false)
        {
            List<string> listaErrosTotal = null;
            bool tipoMaterialSubitemDivergenteTipoMaterialMovimentacao = false;
            bool excedeNumeroMaximoSubitens = false;

            AlmoxarifadoPresenter presenterAlmoxarifado = new AlmoxarifadoPresenter();
            MovimentoBusiness estrutura = new MovimentoBusiness();
            MovimentoItemEntity movimentoItem = new MovimentoItemEntity();


            if (movimento == null)
                movimento = new MovimentoEntity();

            if (movimento.MovimentoItem == null)
                movimento.MovimentoItem = new List<MovimentoItemEntity>();


            //Trava de número de subitens por movimentação de material (40 subitens)
            excedeNumeroMaximoSubitens = (movimento.MovimentoItem.HasElements() && movimento.MovimentoItem.DistinctBy(movItem => movItem.SubItemCodigo.Value).Count() >= Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO);
            if (movimento.IsNotNull() && excedeNumeroMaximoSubitens)
            {
                this.View.ExibirMensagem(String.Format("Ação não permitida, por exceder número máximo de subitens por movimentação ({0}).", Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                return movimento;
            }

            movimentoItem.SubItemMaterial = new SubItemMaterialEntity();
            movimentoItem.SubItemMaterial.UnidadeFornecimento = new UnidadeFornecimentoEntity();
            movimentoItem.UGE = new UGEEntity();
            estrutura.Movimento.Divisao = new DivisaoEntity();

            if (!string.IsNullOrEmpty(this.View.Codigo))
                movimentoItem.SubItemMaterial.Codigo = (long)(Common.Util.TratamentoDados.TryParseLong(this.View.Codigo));

            movimentoItem.Id = new Random().Next(999999999);
            movimentoItem.SubItemMaterial.Id = this.View.SubItemId;
            movimentoItem.SubItemMaterial.Descricao = this.View.Descricao;
            movimentoItem.QtdeLiq = this.View.Quantidade;
            movimentoItem.SubItemMaterial.UnidadeFornecimento.Descricao = this.View.UnidadeFornecimentoDescricao;
            movimentoItem.PTRes = ptRes;

            movimentoItem.DataVencimentoLote = DateTime.MinValue;

            //Retorna os dados do almoxarifado que atende a Divisão selecionada no combo.
            estrutura.Movimento.Divisao.Id = View.DivisaoId;
            estrutura.Movimento.Almoxarifado = presenterAlmoxarifado.GetAlmoxarifadoByDivisao((int)estrutura.Movimento.Divisao.Id);
            movimentoItem.UGE.Id = estrutura.Movimento.Almoxarifado.Uge.Id ?? 0;


            listaErrosTotal = new List<string>();
            #region Trava por TipoMaterial
            if (verificaTipoMaterial)
            {
                tipoMaterialSubitemDivergenteTipoMaterialMovimentacao = VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(movimento, movimentoItem);
                if (tipoMaterialSubitemDivergenteTipoMaterialMovimentacao)
                {
                    var tipoMaterialMovimentacao = movimento.ObterTipoMaterial();
                    var msgErro = String.Format("Requisição de material permitirá apenas inclusão de subitens de material do tipo {0}", EnumUtils.GetEnumDescription<TipoMaterialParaLancamento>(tipoMaterialMovimentacao));

                    listaErrosTotal.Add(msgErro);
                }
            }
            #endregion Trava por TipoMaterial

            //if (estrutura.ConsistirSubItem(movimentoItem) && !ehDivergente)
            if (estrutura.ConsistirSubItem(movimentoItem) && !tipoMaterialSubitemDivergenteTipoMaterialMovimentacao && !excedeNumeroMaximoSubitens)
                movimento.MovimentoItem.Add(movimentoItem);
            else if (estrutura.ConsistirSubItem(movimentoItem) && !tipoMaterialSubitemDivergenteTipoMaterialMovimentacao && excedeNumeroMaximoSubitens)
                listaErrosTotal.Add(String.Format("Ação não permitida, por exceder número máximo de subitens por movimentação ({0}).", Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));

             //this.View.ListaErros = estrutura.ListaErro;
            listaErrosTotal.AddRange(estrutura.ListaErro);

            this.View.ListaErros = listaErrosTotal;

            return movimento;
        }

        public bool VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(MovimentoEntity movimentacaoMaterial, MovimentoItemEntity itemMovimentacao)
        {
            bool ehDivergente = false;
            MovimentoPresenter movimentacaoMaterialPresenter = null;


            movimentacaoMaterialPresenter = new MovimentoPresenter();
            ehDivergente = movimentacaoMaterialPresenter.VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(movimentacaoMaterial, itemMovimentacao);

            return ehDivergente;
        }

        public bool ConsistirDivisao()
        {
            List<string> listaErro = new List<string>();
            if (View.DivisaoId == 0)
            {
                listaErro.Add("É necessário informar a Divisão.");
                this.View.ExibirMensagem("Registro com inconsistências, verificar mensagens!");
            }

            this.View.ListaErros = listaErro;

            if (listaErro.Count > 0)
            {
                return false;
            }
            else
                return true;
        }

        public void CancelarRequisicao()
        {
            int? requisicaoId = Common.Util.TratamentoDados.TryParseInt32(this.View.Id);
            int usuarioLogado_ID = this.Acesso.Transacoes.Perfis[0].IdLogin;

            if (requisicaoId != null)
            {
                MovimentoBusiness estrutura = new MovimentoBusiness();

                estrutura.Movimento.IdLogin = usuarioLogado_ID;
                estrutura.CancelarRequisicao((int)requisicaoId);               
                if (estrutura.ListaErro.Count == 0)
                { // this.View.ExibirMensagem("Requisição Cancelada com Sucesso.");
                    var _msgSaida = String.Format("Requisição {0} cancelada com sucesso!", this.View.NumeroDocumento);
                    this.View.InserirMensagemCancelada("_transPageNumDocRequisicao", _msgSaida);
                }
                else
                {
                    this.View.ExibirMensagem("Registro com inconsistências, verificar mensagens!");
                    this.View.ListaErros = estrutura.ListaErro;
                }
            }
            else
            {
                this.View.ExibirMensagem("Registro com inconsistências, verificar mensagens!");

                List<string> erros = new List<string>();
                //erros.Add("O ID do movimento está nulo.");
                erros.Add("Os dados alterados precisam serem salvos antes de cancelar a requisição.");
                this.View.ListaErros = erros;
                this.View.PopularGrid();
            }
        }

        public void Gravar(MovimentoEntity movimento)
        {
            if (!ConsistirDivisao())
                return;

            AlmoxarifadoPresenter presenterAlmoxarifado = new AlmoxarifadoPresenter();
            MovimentoBusiness estrutura = new MovimentoBusiness();

            estrutura.Movimento = movimento;
            estrutura.Movimento.Almoxarifado = new AlmoxarifadoEntity();
            estrutura.Movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();
            //estrutura.Movimento.UGE = new UGEEntity();
            //estrutura.Movimento.Divisao = new DivisaoEntity();
            if (estrutura.Movimento.Divisao == null && (!estrutura.Movimento.Divisao.Id.HasValue && String.IsNullOrWhiteSpace(estrutura.Movimento.Divisao.Descricao)))
                estrutura.Movimento.Divisao = new DivisaoEntity();

            //Retorna os dados do almoxarifado referente a divisão selecionada no lista.
            estrutura.Movimento.Divisao.Id = View.DivisaoId;
            estrutura.Movimento.Almoxarifado = presenterAlmoxarifado.GetAlmoxarifadoByDivisao((int)estrutura.Movimento.Divisao.Id);

            //Carrega o restante dos dados
            estrutura.Movimento.UGE.Id = estrutura.Movimento.Almoxarifado.Uge.Id ?? 0;
            estrutura.Movimento.Observacoes = this.View.Observacao;
            //estrutura.Movimento.Instrucoes = this.View.Instrucao;
            estrutura.Movimento.Instrucoes = null;
            estrutura.Movimento.MovimAlmoxOrigemDestino.Id = null;
            estrutura.Movimento.DataDocumento = DateTime.Now;
            estrutura.Movimento.DataMovimento = DateTime.Now;
            estrutura.Movimento.Empenho = string.Empty;
            estrutura.Movimento.FonteRecurso = string.Empty;
            estrutura.Movimento.GeradorDescricao = estrutura.Movimento.Divisao.Descricao;
            estrutura.Movimento.AnoMesReferencia = estrutura.Movimento.Almoxarifado.MesRef;
            //if (String.IsNullOrWhiteSpace(estrutura.Movimento.NumeroDocumento))
            //estrutura.Movimento = estrutura.GerarNovoDocumento(estrutura.Movimento);

            estrutura.Movimento.ValorDocumento = 0.00m;

            //estrutura.Movimento.PTRes = new PTResEntity();
            //estrutura.Movimento.PTRes.Codigo = View.PTResId;
            //estrutura.Movimento.PTRes.Descricao = View.PtResDescricao;

            estrutura.Movimento.TipoMovimento = new TipoMovimentoEntity((int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente);
            estrutura.Movimento.Ativo = true;

            //Adiciona o perfil do usuário Logado no cadastro do movimento
            estrutura.Movimento.IdLogin = Acesso.Transacoes.Perfis[0].IdLogin;

            //Adiciona a data da operação como data atual
            estrutura.Movimento.DataOperacao = DateTime.Now;

            if (estrutura.Movimento.MovimentoItem != null)
            {
                for (int i = 0; i < estrutura.Movimento.MovimentoItem.Count; i++)
                    estrutura.Movimento.MovimentoItem[i].Ativo = true;
            }

            if (estrutura.SalvarMovimento())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                //this.View.ExibirMensagem("Registro Salvo com Sucesso.");
                var _msgSaida = String.Format("Requisição {0} salva com sucesso!", estrutura.Movimento.NumeroDocumento);
                this.View.InserirMensagemEmSessao("_transPageNumDocRequisicao", _msgSaida);
            }
            else
                this.View.ExibirMensagem("Registro com inconsistências, verificar mensagens!");

            this.View.ListaErros = estrutura.ListaErro;
            this.View.PopularGrid();
        }

        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.Divisao;
            relatorioImpressao.Nome = "rptDivisao.rdlc";
            relatorioImpressao.DataSet = "dsDivisao";

            this.View.DadosRelatorio = relatorioImpressao;
        }

        public override void RegistroSelecionado()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

            this.View.Codigo = String.Format("{0:D3}", estrutura.Divisao.Codigo);
            this.View.Descricao = estrutura.Divisao.Descricao;
            base.RegistroSelecionado();

        }

        public void NovaRequisicao()
        {
            this.View.MostrarPainelRequisicao = true;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            this.View.Observacao = string.Empty;
            this.View.Quantidade = _valorZero;
            this.View.Descricao = string.Empty;
            this.View.UnidadeFornecimentoDescricao = string.Empty;
            this.View.Id = string.Empty;
            this.View.MostrarPainelRequisicao = false;
        }

        public void AdicionadoSucesso()
        {
            base.GravadoSucesso();
            this.View.Quantidade = _valorZero;
            this.View.Descricao = string.Empty;
            this.View.Id = string.Empty;
            this.View.UnidadeFornecimentoDescricao = string.Empty;

            this.View.PTResCodigo = null;
            this.View.Codigo = string.Empty;
            this.View.UnidadeFornecimentoDescricao = string.Empty;
        }

        public override void ExcluidoSucesso()
        {
            base.ExcluidoSucesso();
        }

        public override void Novo()
        {
            //Alteração consistência divisão - Requisitante Geral.
            if (!ConsistirDivisao())
                return;

            if (!VerificaSeAlmoxarifadoPossuiFechamentos())
                return;

            base.Novo();
            this.View.BloqueiaCodigo = false;
            this.View.VisivelAdicionar = true;
            this.View.VisivelEditar = false;
            this.View.Quantidade = _valorZero;
            this.View.UnidadeFornecimentoDescricao = string.Empty;
        }

        public bool VerificaSeAlmoxarifadoPossuiFechamentos()
        {
            bool existemFechamentosNesteAlmoxarifado = false;
            int almoxarifadoId = 0;
            FechamentoMensalPresenter fechamentoPresenter = null;
            IList<string> listaMsgsErro = null;



            almoxarifadoId = (this.Acesso.IsNotNull() ? this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.GetValueOrDefault() : 0);
            listaMsgsErro = new List<string>();
            if (almoxarifadoId > 0)
            {
                fechamentoPresenter = new FechamentoMensalPresenter();
                existemFechamentosNesteAlmoxarifado = fechamentoPresenter.ContemFechamento(almoxarifadoId);

                if (!existemFechamentosNesteAlmoxarifado)
                {
                    listaMsgsErro.Add("Almoxarifado deve ter o mês-referência inicial fechado (após transporte de saldo) para que se possa efetuar/atender Requisição de Material!");

                    this.View.ExibirMensagem("Almoxarifado em estado de implantação (transporte de saldo) não-finalizado! Verificar mensagem!");
                    this.View.ListaErros = listaMsgsErro.ToList();
                }
            }


            return existemFechamentosNesteAlmoxarifado;
        }

        public override void Cancelar()
        {
            this.View.Quantidade = _valorZero;
            this.View.Descricao = string.Empty;
            this.View.Id = string.Empty;
            this.View.UnidadeFornecimentoDescricao = string.Empty;

            base.Cancelar();
            this.View.VisivelAdicionar = false;
            this.View.VisivelEditar = false;
        }

        public void Editar()
        {
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaCodigo = false;
            this.View.VisivelAdicionar = false;
            this.View.VisivelEditar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaExcluir = true;
        }

        public void EditarSucesso()
        {
            this.View.MostrarPainelEdicao = false;
            this.View.Id = string.Empty;
            this.View.PTResCodigo = null;
            this.View.PTResId = 0;
            this.View.PTResDescricao = string.Empty;
            this.View.PTResAcao = string.Empty;
            this.View.Quantidade = _valorZero;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.UnidadeFornecimentoDescricao = string.Empty;
            this.View.BloqueiaNovo = true;
        }

        public void imprimirRequisicao(SortedList ParametrosRelatorio)
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.RequisicaoMaterial;
            relatorioImpressao.Nome = "AlmoxRequisicaoMaterial.rdlc";
            relatorioImpressao.DataSet = "dsRequisicaoMaterial";
            relatorioImpressao.Parametros = ParametrosRelatorio;

            this.View.DadosRelatorio = relatorioImpressao;
        }

        public void imprimirSaida(SortedList ParametrosRelatorio)
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.SaidaMaterial;
            relatorioImpressao.Nome = "AlmoxSaidaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSaidaMaterial";

            relatorioImpressao.Parametros = ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int uoId, int ugeId, int _uaId, int divisaoId, int stastuid)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _orgaoId, int _uaId)
        {
            return this.TotalRegistrosGrid;
        }

        private void PrepararVisaoDeCombosPorPerfil()
        {
            //if (PerfilId != (int)Sam.Common.Perfil.REQUISITANTE && PerfilId != (int)Sam.Common.Perfil.REQUISITANTE_GERAL
            //                                                    && PerfilId != (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
            //{
            //    this.view.BloqueiaListaOrgao = true;
            //    this.view.BloqueiaListaUO = true;
            //    this.view.BloqueiaListaUGE = true;
            //}

            //else if (PerfilId == (int)Sam.Common.Perfil.REQUISITANTE || PerfilId == (int)Sam.Common.Perfil.REQUISITANTE_GERAL)
            //    this.view.BloqueiaListaOrgao = true;
            //else
            //    //Sam.Common.Perfil.ADMINISTRADOR_GERAL
            //    this.view.BloqueiaListaOrgao = false;


            switch (PerfilId)
            {
                //case (int)GeralEnum.TipoPerfil.OperadorAlmoxarifado:
                //    this.view.BloqueiaListaOrgao = true;
                //    this.view.BloqueiaListaUO = true;
                //    this.view.BloqueiaListaUGE = true;
                //    break;
                //case (int)GeralEnum.TipoPerfil.AdministradorGestor:
                ////case (int)GeralEnum.TipoPerfil.RequisitanteGeral:
                //    this.view.BloqueiaListaOrgao = true;
                //    this.view.BloqueiaListaUO = true;
                //    break;
                //case (int)GeralEnum.TipoPerfil.AdministradorOrgao:
                //    this.view.BloqueiaListaOrgao = true;
                //    break;
                //case(int)GeralEnum.TipoPerfil.AdministradorGeral:
                //    this.view.BloqueiaListaOrgao = false;
                //    break;
                //default:
                //    this.view.BloqueiaListaUO = true;
                //    break;
            }
        }


        private void ObterAcessosPerfilLogado()
        {
            PerfilId = Acesso.Transacoes.Perfis[0].IdPerfil;
            LoginId = Acesso.Transacoes.Perfis[0].IdLogin;
        }

        public ObjectDataSource FillSourceGridRequisicoes(ObjectDataSourceSelectingEventHandler objectDataSourceSelectingEventHandler)
        {
            var fillSourceGridView = new FillSourceGridView("Sam.Presenter.MovimentoPresenter",
                                                            "sourceGridRequisicoes",
                                                            "startRowIndexParameterName",
                                                            "maximumRowsParameterName",
                                                            "TotalRegistros",
                                                            "original_{0}",
                                                            true);
            fillSourceGridView.SetSelectParameters(new Parameter("maximumRowsParameterName", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("startRowIndexParameterName", System.Data.DbType.Int32));


            if (!string.IsNullOrEmpty(this.View.NumeroDocumento))
            {
                fillSourceGridView.SetSelectMethod("ListarRequisicaoByNumeroDocumento");
                PrepareParametersSourceGridRequisicoes(this.View.NumeroDocumento, fillSourceGridView);
            }

            else 
            {
                fillSourceGridView.SetSelectMethod("ListarRequisicao");
                //PrepareParametersSourceGridRequisicoes(Convert.ToInt32(this.View.DivisaoId), 0, this.View.StatusId, fillSourceGridView);
                PrepareParametersSourceGridRequisicoes((int)this.View.UOId
                                                        , (int)this.View.UGEId
                                                        , (int)this.View.UAId
                                                        , (int)this.View.DivisaoId
                                                        , (int)this.View.StatusId
                                                        , fillSourceGridView);
            }
            
            fillSourceGridView.SetObjectDataSourceSelectingEventHandler(objectDataSourceSelectingEventHandler);
            fillSourceGridView.ObjectDataSource.DataBind();

            return fillSourceGridView.ObjectDataSource;
        }

        public IList<MovimentoEntity> ListarRequisicao(int startRow, int maximoRow, string orgaoId = "", string uoId = "", string ugeId = "", string uaId = "", string divisaoId = "", int statusId = 0, string numeroDocumento = "")
        {
            IList<MovimentoEntity> retorno = new List<MovimentoEntity>();
            MovimentoBusiness movimentoBusiness = new MovimentoBusiness();
            movimentoBusiness.SkipRegistros = startRow;
            //int _uoId = this.View.UOId;
            //int _ugeId = this.View.UGEId;
            //int _uaId = this.View.UAId;
            //int _divisaoId = this.View.DivisaoId;
            //int _statusId = this.View.StatusId;


            retorno = movimentoBusiness.ListarRequisicao(startRow, maximoRow, orgaoId, uoId, ugeId, uaId, divisaoId, statusId, numeroDocumento).ToList();

            this.TotalRegistrosGrid = movimentoBusiness.TotalRegistros;

            return retorno;
        }

        private ObjectDataSource PrepareParametersSourceGridRequisicoes(string numeroDocumento, FillSourceGridView fillSourceGridView)
        {
            fillSourceGridView.SetSelectParameters(new Parameter("numeroDocumento", System.Data.DbType.String));
            return fillSourceGridView.ObjectDataSource;
        }

        private ObjectDataSource PrepareParametersSourceGridRequisicoes(int divisaoId, int almoxarifadoId, FillSourceGridView fillSourceGridView)
        {
            fillSourceGridView.SetSelectParameters(new Parameter("AlmoxarifadoId", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("divisaoId", System.Data.DbType.Int32));
            return fillSourceGridView.ObjectDataSource;
        }

        private ObjectDataSource PrepareParametersSourceGridRequisicoes(int uoId, int ugeId, int uaId, int divisaoId, int statusId, FillSourceGridView fillSourceGridView)
        {
            fillSourceGridView.SetSelectParameters(new Parameter("uoId", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("ugeId", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("uaId", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("divisaoId", System.Data.DbType.Int32));
            fillSourceGridView.SetSelectParameters(new Parameter("statusId", System.Data.DbType.Int32));
            return fillSourceGridView.ObjectDataSource;
        }

        public void SetTipoDePesquisaEventoGridview_Selecting(int uoId, int ugeId, int uaId, int divisaoId, DropDownList ddlStatusRequisicao, string numeroDocumento, ObjectDataSourceSelectingEventArgs e)
        //public void SetTipoDePesquisaEventoGridview_Selecting(string divisaoId, DropDownList ddlStatusRequisicao, string numeroDocumento, ObjectDataSourceSelectingEventArgs e)
        {
            int _statusId = ddlStatusRequisicao.SelectedIndex > 0 ? Convert.ToInt32(ddlStatusRequisicao.SelectedValue) : 0;

            string _numeroDocumento = !string.IsNullOrEmpty(numeroDocumento) ? numeroDocumento : string.Empty;

            if (this.view.ResetarStartRowIndex)
            {
                e.Arguments.StartRowIndex = 0;
                this.view.ResetGridViewPageIndex();
            }

            if (!string.IsNullOrEmpty(_numeroDocumento))
                e.InputParameters["numeroDocumento"] = _numeroDocumento;

            e.InputParameters["statusId"] = _statusId;
            e.InputParameters["uoId"] = uoId != null ? uoId : 0;
            e.InputParameters["ugeId"] = ugeId != null ? ugeId : 0;
            e.InputParameters["uaId"] = uaId != null ? uaId : 0;
            e.InputParameters["divisaoId"] = divisaoId != null ? divisaoId : 0;
        }


        #endregion

        #region Controllers

        public override void Load()
        {
            base.Load();

            this.View.BloqueiaCodigo = false;
            this.View.VisivelAdicionar = false;
            this.View.VisivelEditar = false;
            this.View.MostrarPainelRequisicao = false;
            this.ObterAcessosPerfilLogado();
            this.PrepararVisaoDeCombosPorPerfil();
        }


        #endregion
    }

    #region Extension Method
    public static class _extensionMovimentacoes
    {
        public static bool ConsistirPTRes(this MovimentoEntity movRequisicao, IRequisicaoMaterialView presenterRequisicao)
        {
            List<string> listaErro = new List<string>();

            if (movRequisicao.IsNotNull() && movRequisicao.MovimentoItem.IsNotNullAndNotEmpty())
            {
                movRequisicao.MovimentoItem.Cast<MovimentoItemEntity>()
                                           .ToList()
                                           .ForEach(_subitem =>
                                           {
                                               if (_subitem.PTRes.IsNull() || !_subitem.PTRes.Id.HasValue)
                                                   listaErro.Add(String.Format("Obrigatório informar PTRes para subitem {0} - {1}.", _subitem.SubItemCodigo, _subitem.SubItemDescricao));
                                           });
            }

            if (listaErro.HasElements())
            {
                presenterRequisicao.ListaErros = listaErro;
                presenterRequisicao.ExibirMensagem("Registro com inconsistências, verificar mensagens!");
            }

            return (listaErro.Count > 0);
        }
    }
    #endregion

}
