using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Business.MovimentoFactory;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.View;
using Sam.Infrastructure;
using System.Linq.Expressions;

namespace Sam.Presenter
{
    public class NovaEntradaMaterialPresenter : CrudPresenter<INovaEntradaMaterialView>
    {

        INovaEntradaMaterialView view;

        public INovaEntradaMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public NovaEntradaMaterialPresenter(INovaEntradaMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public NovaEntradaMaterialPresenter()
        {
        }

        public void HabilitarControlesEdit()
        {
            this.View.BloqueiaEstornar = false;
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaCancelar = false;
        }

        public MovimentoEntity GetMovimento(int movimentoId)
        {
            MovimentoEntity movimentoParam = new MovimentoEntity(movimentoId);
            movimentoParam.Ativo = true;

            Sam.Domain.Business.MovimentoBusiness business = new MovimentoBusiness();
            business.Movimento = movimentoParam;
            return business.GetMovimento();
        }

        public TB_MOVIMENTO SelectOneMovimento(int movimentoId)
        {
            Business.MovimentoBusiness business = new Business.MovimentoBusiness();
            return business.SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);
        }

        public IList<TB_MOVIMENTO> ConsultarMovimentos(int startRowIndexParameterName, int maximumRowsParameterName, IConsultarEntradaView consultaView)
        {
            var business = new Business.MovimentoBusiness();
            TB_MOVIMENTO mov = new TB_MOVIMENTO();

            mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = consultaView.MOVIMENTO_NUMERO_DOCUMENTO;

            int almoxId = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            string mesRef = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            //Montando Where
            Expression<Func<TB_MOVIMENTO, bool>> where = a => a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almoxId
            && (a.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesRef)
            && (a.TB_MOVIMENTO_NUMERO_DOCUMENTO == consultaView.MOVIMENTO_NUMERO_DOCUMENTO || consultaView.MOVIMENTO_NUMERO_DOCUMENTO == string.Empty)
            && (a.TB_MOVIMENTO_EMPENHO == consultaView.EMPENHO_COD || consultaView.EMPENHO_COD == string.Empty)
            && (a.TB_UGE_ID == consultaView.UGE_ID || consultaView.UGE_ID == 0)
            && ((a.TB_MOVIMENTO_DATA_DOCUMENTO >= consultaView.MOVIMENTO_DATA_DOCUMENTO && a.TB_MOVIMENTO_DATA_DOCUMENTO <= consultaView.MOVIMENTO_DATA_DOCUMENTO_ATE) || (consultaView.MOVIMENTO_DATA_DOCUMENTO == null && consultaView.MOVIMENTO_DATA_DOCUMENTO_ATE == null))
            && ((a.TB_MOVIMENTO_DATA_MOVIMENTO >= consultaView.MOVIMENTO_DATA_MOVIMENTO_ATE && a.TB_MOVIMENTO_DATA_MOVIMENTO <= consultaView.MOVIMENTO_DATA_MOVIMENTO_ATE) || (consultaView.MOVIMENTO_DATA_MOVIMENTO == null && consultaView.MOVIMENTO_DATA_MOVIMENTO_ATE == null))
            && (a.TB_TIPO_MOVIMENTO_ID == consultaView.TIPO_MOVIMENTO_ID);

            var result = business.ConsultaMovimento(startRowIndexParameterName, where);
            this.TotalRegistrosGrid = business.TotalRegistros;
            return result;
        }

        public IList<UGEEntity> PopularListaUGE(int _orgaoId, int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<UGEEntity> retorno = estrutura.ListarTodosCodPorGestor(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public void CarregarMovimentoTela(MovimentoEntity mov)
        {
            this.View.Id = mov.Id.Value.ToString();
            this.View.Empenho = mov.Empenho;
            this.View.DataDocumento = mov.DataDocumento;
            this.View.DataMovimento = mov.DataMovimento;
            this.View.GeradorDescricao = mov.GeradorDescricao;
            this.View.FornecedorId = mov.Fornecedor.Id;
            if(mov.Divisao != null) this.View.DivisaoId = mov.Divisao.Id;
            this.View.TipoMovimento = mov.TipoMovimento.Id;
            if(this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
                if (mov.Almoxarifado != null) this.View.AlmoxarifadoIdOrigem = mov.Almoxarifado.Id;
            else
                if (mov.MovimAlmoxOrigemDestino != null) this.View.AlmoxarifadoIdOrigem = mov.MovimAlmoxOrigemDestino.Id;
            this.View.NumeroDocumento = mov.NumeroDocumento == null ? null : (mov.NumeroDocumento.Length == 12 ? mov.NumeroDocumento.Substring(4, 8) : mov.NumeroDocumento);
            this.View.Observacoes = mov.Observacoes;

            if(mov.ValorDocumento.HasValue)
                this.View.ValorDocumento = mov.ValorDocumento.Value.Truncar(mov.AnoMesReferencia, true);

            this.View.EmpenhoLicitacaoId = mov.EmpenhoLicitacao.Id;
            if (mov.EmpenhoEvento.Id.HasValue && mov.EmpenhoEvento.Id != 0)
                this.View.EmpenhoEventoId = mov.EmpenhoEvento.Id;
            
        }

        public IList<DivisaoEntity> PopularListaDivisao(int _almoxId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ListarDivisaoPorAlmoxTodosCod(_almoxId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<AlmoxarifadoEntity> PopularListaAlmoxarifado(int _gestorId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifadoPorGestorTodosCod(_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public void CalcularPrecoUnitarioItem()
        {
            var _anoMesRef = View.AnoMesReferencia;

            if (View.QtdeItem != _valorZero && this.View.QtdeItem > _valorZero)
                this.View.PrecoUnitItem = (this.View.ValorMovItem / View.QtdeItem);
        }
        
        public override void Load()
        {
            this.View.ConfiguracaoPagina();
            this.View.CarregarPermissaoAcesso();
            this.View.GetSessao();            
            this.CarregarEntradaPadrao();
            this.View.GetSessaoEdicao();
        }

        public virtual void NotPostBackInicialize()
        {
            this.View.RemoveSessao();
            base.Load();
            this.View.PopularListaTipoMovimentoEntrada();
            this.View.NotPostBackInicialize();
            this.View.MostrarPainelEdicao = false;
            this.View.BloqueiaImprimir = true;
            this.View.BloqueiaValorTotal = true;
            this.View.TratarEntrada();
        }

        public virtual void CarregarEntradaPadrao()
        {
            this.View.PopularDadosUGETodosCod();
            //this.View.PopularDadosSubItemClassif();
            //this.View.PopularListaAlmoxarifado();
            //this.View.PopularListaEmpenhoEvento();
            //this.View.PopularUnidFornecimentoTodosPorGestor();
            //this.View.PopularListaDivisao();
        }

        public virtual void PrepararNovaEntrada()
        {
            this.LimparCamposMovimento();
            this.LimparCamposMovimentoTransfer();
            this.LimparItem();
            this.LimparMovimento();
            this.LimparMovimentoCompraDireta();
            this.View.BloqueiaGravar = true;
            this.view.BloqueiaGravarItem = true;
        }

        public virtual void PreprarEdicaoEntrada()
        {
            //if (!this.View.SubItemMaterialCodigo.HasValue)
            //    this.View.PopularDadosSubItemMaterial(this.View.SubItemMaterialCodigo.ToString());

            this.View.PreencherDadosEdicao();
            this.view.BloqueiaGravarItem = false;            
        }

        public IList<string> CarregarListaEmpenhos(int almoxarifadoId, int gestorId, int codigoUGE, string pStrLoginUsuario, string pStrSenhaUsuario)
        {
            List<string>    lLstRetorno      = null;
            EmpenhoBusiness lObjBusiness     = null;
            IList<string>   lILstEmpenhos    = null;
            IList<string>   lILstLiquidados  = null;
            List<string>    lLstFiltro       = null;
            string          lStrLoginUsuario = null;
            string          lStrSenhaUsuario = null;

            try
            {
                var AlmoxarifadoLogado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;


                lLstRetorno = new List<string>();
                lObjBusiness = new EmpenhoBusiness();

                lStrLoginUsuario = Siafem.userNameConsulta;
                lStrSenhaUsuario = Siafem.passConsulta;

                //lILstEmpenhos = lObjBusiness.CarregarListaCodigosEmpenhos(pIntUgeID, AlmoxarifadoLogado.MesRef, pStrLoginUsuario, pStrSenhaUsuario, true);
                lILstEmpenhos = lObjBusiness.obterListaEmpenhosSiafem(almoxarifadoId, gestorId, codigoUGE, AlmoxarifadoLogado.MesRef, lStrLoginUsuario, lStrSenhaUsuario);

                if (lILstEmpenhos != null && lILstEmpenhos.Count > 0)
                {
                    //listagem de empenhos já liquidados (ou a liquidar no sistema)
                    lILstLiquidados = lObjBusiness.obterListaEmpenhos(AlmoxarifadoLogado.Id.Value, AlmoxarifadoLogado.MesRef, true);
                    lLstFiltro = lILstEmpenhos.Where(Linha => !lILstLiquidados.Contains(Linha)).ToList();

                    lLstRetorno = lLstFiltro;
                }
                else
                {
                    //lObjBusiness.ListaErro.Add("Favor alterar a sua senha de acesso ao Siafem!");

                    this.View.ListaErros = lObjBusiness.ListaErro;
                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                }

                return lLstRetorno;
            }
            catch (Exception e)
            {
                lObjBusiness.ListaErro.Add(String.Format("Erro ao executar solicitação servidor: {0}", e.Message));

                new LogErro().GravarLogErro(e);
                this.View.ListaErros = lObjBusiness.ListaErro;
                return null;
            }
        }


        public void ProcurarItemMaterialId(int _itemMaterialCodigo)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            ItemMaterialEntity itemMat = new ItemMaterialEntity();
            itemMat.Codigo = _itemMaterialCodigo;
            itemMat = estrutura.SelectPorItemMaterial(itemMat);
            if (itemMat.Id.HasValue)
                this.View.ItemMaterialId = itemMat.Id.Value;
        }

        public MovimentoEntity CancelarItemEmpenho(MovimentoEntity mov)
        {
            MovimentoItemEntity movItem = new MovimentoItemEntity();

            if (this.View.MovimentoItemId != 0)
                movItem = mov.MovimentoItem.Where(a => a.Id == this.View.MovimentoItemId).FirstOrDefault();
            movItem.QtdeMov = null;

            List<MovimentoItemEntity> newList = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
            newList.Add(movItem);
            mov.MovimentoItem = newList;
            //this.View.BloqueiaTipoMovimento = false;

            return mov;
        }

        public MovimentoEntity GravarItem(MovimentoEntity mov)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            MovimentoItemBusiness moviItemBus = new MovimentoItemBusiness();
            List<MovimentoItemEntity> movItems = (List<MovimentoItemEntity>)mov.MovimentoItem;
            MovimentoItemEntity movItem = null;
            SaldoSubItemBusiness estruturaSaldo = new SaldoSubItemBusiness();
            movItem = new MovimentoItemEntity();
            if (!mov.Id.HasValue)
            {
                movItem.Movimento = new MovimentoEntity(0);
            }
            else
            {
                movItem.Movimento = new MovimentoEntity(mov.Id.Value);
            }
            movItem.Movimento.TipoMovimento = new TipoMovimentoEntity(this.View.TipoMovimento);

            if (this.View.MovimentoItemId != 0)
            {
                movItem.Id = this.View.MovimentoItemId;
                movItem.Posicao = this.View.MovimentoItemId.Value;
            }
            movItem.Ativo = false;
            movItem.DataVencimentoLote = this.View.DataVctoLoteItem;
            movItem.Desd = 0;
            movItem.FabricanteLote = this.View.FabricLoteItem == "" ? null : this.View.FabricLoteItem;
            movItem.IdentificacaoLote = this.View.IdentificacaoLoteItem == "" ? null : this.View.IdentificacaoLoteItem;

            //if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                this.View.QtdeLiqItem = 0;
            }

            movItem.QtdeLiq = this.View.QtdeLiqItem;
            movItem.QtdeMov = this.View.QtdeItem;
            movItem.PrecoUnit = this.View.PrecoUnitItem;
            movItem.SaldoQtde = this.View.SaldoQtdeItem;
            movItem.SaldoQtdeLote = this.View.SaldoQtdeLoteItem;
            movItem.SaldoValor = this.View.SaldoValorItem;
            movItem.SubItemMaterial = estrutura.SelectSubItemMaterialRetorno(this.View.SubItemMaterialId.Value);  //  new SubItemMaterialEntity(this.View.SubItemMaterialId);

            if (movItem.SubItemMaterial != null)
            {
                ItemMaterialEntity item = new ItemMaterialEntity();
                item.Codigo = this.View.ItemMaterialCodigo.Value;
                movItem.SubItemMaterial.ItemMaterial = estrutura.SelectPorItemMaterial(item);
                movItem.SubItemMaterial.ItemMaterial.CodigoFormatado = movItem.SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9, '0');
                movItem.SubItemMaterial.CodigoFormatado = movItem.SubItemMaterial.Codigo.ToString().PadLeft(12, '0');
            }
            else
            {
                this.View.ExibirMensagem("Favor informar o subitem de material!");
                return null;
            }
            movItem.UGE = new UGEEntity(this.View.UgeId);
            movItem.ValorMov = this.View.ValorMovItem;

            // lote
            CatalogoBusiness cat = new CatalogoBusiness();
            cat.SubItemMaterial = movItem.SubItemMaterial;
            if (!movItem.SubItemMaterial.Id.HasValue)
            {
                this.View.ExibirMensagem("Favor informar o subitem de material!");
                return null;
            }
            if (cat.SubItemMaterial.IsLote.Value == true)
            {
                if (this.View.DataVctoLoteItemTexto == "" && this.View.IdentificacaoLoteItem == "" && this.View.FabricLoteItem == "") 
                {
                    this.View.ExibirMensagem("Favor preencher um dos campos de lote!");
                    return null;
                }

                if (this.View.DataVctoLoteItemTexto != "" && !movItem.DataVencimentoLote.HasValue)
                {
                    this.View.ExibirMensagem("Data de vencimento do lote inválida!");
                    this.View.FocarItemDataVencLote();
                    return null;
                }
            }

            MovimentoItemBusiness estruturaItem = new MovimentoItemBusiness();
            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                if (!estruturaItem.ConsistirItemLiquidar(movItem))
                {
                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = estruturaItem.ListaErro;
                    this.View.FocarItemQtdeEntrar();
                    return null;
                }
            }

            // validar (não salva em banco)
            if (!moviItemBus.ConsistirItem(movItem))
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = moviItemBus.ListaErro;
                return null;
            }

            if (mov.MovimentoItem == null)
                mov.MovimentoItem = new List<MovimentoItemEntity>();

            int newId = 1;
            if (movItem.Id == null)
            {
                newId += mov.MovimentoItem.Count();
                movItem.Id = newId;
                movItem.Posicao = newId;
                mov.MovimentoItem.Add(movItem);
            }
            else
            {
                List<MovimentoItemEntity> newList = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
                newList.Add(movItem);
                mov.MovimentoItem = newList;
            }

            return mov;
        }

        public MovimentoEntity ExcluirItem(MovimentoEntity mov)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            MovimentoItemBusiness moviItemBus = new MovimentoItemBusiness();
            List<MovimentoItemEntity> movItems = (List<MovimentoItemEntity>)mov.MovimentoItem;
            MovimentoItemEntity movItem = null;
            SaldoSubItemBusiness estruturaSaldo = new SaldoSubItemBusiness();

            movItem = new MovimentoItemEntity();
            if (!mov.Id.HasValue)
            {
                movItem.Movimento = new MovimentoEntity(0);
            }
            else
            {
                movItem.Movimento = new MovimentoEntity(mov.Id.Value);
            }

            if (this.View.MovimentoItemId != 0)
            {
                movItem.Id = this.View.MovimentoItemId;
            }

            if(mov.MovimentoItem != null)
                mov.MovimentoItem = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();

            return mov;
        }


        public void HabilitarControles(int tipoMovimento, bool Editar)
        {
            switch (tipoMovimento)
            {
                //case (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho:
                case (int)GeralEnum.TipoMovimento.EntradaPorEmpenho:
                    this.View.HabilitarCompraDireta(Editar);
                    break;
                //case (int)GeralEnum.TipoMovimento.AquisicaoAvulsa:
                case (int)GeralEnum.TipoMovimento.EntradaAvulsa:
                case (int)GeralEnum.TipoMovimento.EntradaCovid19:
                    this.View.HabilitarAquisicaoAvulsa(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferencia:
                    this.View.HabilitarTransferencia(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado:
                    this.View.HabilitarDoacao(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDevolucao:
                    this.View.HabilitarDevolucao(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado:
                    this.View.HabilitarMaterialTransformado(Editar);
                    break;
                default:
                    break;
            }
            this.View.TipoMovimento = tipoMovimento;
        }

        public void LimparCamposMovimento() 
        {
            this.View.LimparGridSubItemMaterial();
            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.GeradorDescricao = null;
            this.View.FornecedorId = null;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
        }

        public void LimparCamposMovimentoTransfer()
        {
            this.View.LimparGridSubItemMaterial();
            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.GeradorDescricao = null;
            this.View.FornecedorId = null;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
        }

        public IList<MovimentoEntity> PopularDadosDocumentoTodosCodPorUgeSimplif(int _ugeId, int _tipoMovimento, int _almoxId)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarTodosCodPorUgeTipoMovimentoSimplif(_ugeId, _tipoMovimento, _almoxId);
            return retorno;
        }

        public IList<MovimentoEntity> PopularDadosDocumentoTodosCodPorUgeEmpenho(int _ugeId, string _numeroEmpenho, int _almoxId)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarTodosCodPorUgeEmpenho(_ugeId, _numeroEmpenho, _almoxId);
            return retorno;
        }

        public IList<MovimentoEntity> PopularDadosDocumentoTodosCodPorUgeAlmox(int _almoxId, int _ugeId, int _tipoMovimento)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            IList<MovimentoEntity> retorno = estrutura.ListarTodosCodPorUgeAlmoxTipoMovimento(_almoxId, _ugeId, _tipoMovimento);
            return retorno;
        }

        public MovimentoEntity DividirRegistro(MovimentoEntity retorno, int _itemId)
        {
            LerRegistro(retorno, _itemId);
            MovimentoItemEntity itemNovo = null;
            foreach (MovimentoItemEntity item in retorno.MovimentoItem)
            {
                if (item.Id == _itemId)
                {
                    itemNovo = new MovimentoItemEntity();
                    itemNovo.Ativo = item.Ativo;
                    itemNovo.DataVencimentoLote = item.DataVencimentoLote;
                    itemNovo.Desd = item.Desd;
                    itemNovo.FabricanteLote = item.FabricanteLote;
                    itemNovo.IdentificacaoLote = item.IdentificacaoLote;
                    itemNovo.Movimento = item.Movimento;
                    itemNovo.PrecoUnit = item.PrecoUnit;
                    itemNovo.QtdeLiq = item.QtdeLiq;
                    itemNovo.QtdeMov = item.QtdeMov;
                    itemNovo.SaldoQtde = item.SaldoQtde;
                    itemNovo.SaldoQtdeLote = item.SaldoQtdeLote;
                    itemNovo.SaldoValor = item.SaldoValor;
                    itemNovo.SubItemMaterial = item.SubItemMaterial;
                    itemNovo.UGE = item.UGE;
                    itemNovo.ValorMov = item.ValorMov;
                    itemNovo.Id = 1;
                    if (retorno.MovimentoItem.Count > 0)
                        itemNovo.Id += retorno.MovimentoItem.Count;
                    retorno.MovimentoItem.Add(itemNovo);
                    break;
                }
            }
            return retorno;
        }
        public IList<string> ObterListaEmpenhos(int almoxarifadoId, int gestorId, int codigoUGE, string loginSiafemUsuario, string senhaSiafemUsuario, bool listarLiquidados = false)
        {
            List<string> lLstRetorno = null;
            EmpenhoBusiness lObjBusiness = null;
            IList<string> lILstEmpenhos = null;
            IList<string> lILstLiquidados = null;
            List<string> lLstFiltro = null;
            string lStrLoginUsuario = null;
            string lStrSenhaUsuario = null;

            try
            {
                var AlmoxarifadoLogado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;


                lLstRetorno = new List<string>();
                lObjBusiness = new EmpenhoBusiness();

                lStrLoginUsuario = Siafem.userNameConsulta;
                lStrSenhaUsuario = Siafem.passConsulta;

                lILstEmpenhos = lObjBusiness.obterListaEmpenhosSiafem(almoxarifadoId, gestorId, codigoUGE, AlmoxarifadoLogado.MesRef, loginSiafemUsuario, senhaSiafemUsuario);

                if (lILstEmpenhos != null && lILstEmpenhos.Count > 0)
                {
                    //lILstLiquidados = lObjBusiness.ListarEmpenhos(codigoUGE, AlmoxarifadoLogado.Id.Value, AlmoxarifadoLogado.MesRef, null, true);
                    lILstLiquidados = lObjBusiness.obterListaEmpenhos(AlmoxarifadoLogado.Id.Value, AlmoxarifadoLogado.MesRef, listarLiquidados);
                    lLstFiltro = lILstEmpenhos.Where(Linha => !lILstLiquidados.Contains(Linha)).ToList();

                    lLstRetorno = lLstFiltro;
                }
                //else if (lILstEmpenhos == null || lObjBusiness.ListaErro.Count > 0)
                else if (lILstEmpenhos.IsNullOrEmpty() || lObjBusiness.ListaErro.Count > 0)
                {
                    lLstRetorno = null;

                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = lObjBusiness.ListaErro;
                }

                return lLstRetorno;
            }
            catch (Exception e)
            {
                lObjBusiness.ListaErro.Add(String.Format("Erro ao executar solicitação servidor: {0}", e.Message));

                new LogErro().GravarLogErro(e);
                this.View.ListaErros = lObjBusiness.ListaErro;
                return null;
            }
        }

        public void LerRegistro(MovimentoEntity retorno, int _itemId)
        {
            foreach (MovimentoItemEntity item in retorno.MovimentoItem)
            {
                if (item.Id == _itemId)
                {
                    this.View.MovimentoItemId = item.Id.Value;
                    this.View.ItemMaterialId = item.SubItemMaterial.ItemMaterial.Id.Value;
                    this.View.ItemMaterialCodigo = item.SubItemMaterial.ItemMaterial.Codigo;
                    this.View.SubItemMaterialTxt = item.SubItemMaterial.Id.Value.ToString();
                    this.View.SubItemMaterialId = item.SubItemMaterial.Id.Value;
                    this.View.SubItemMaterialCodigo = item.SubItemMaterial.Codigo;
                    this.View.SubItemMaterialDescricao = item.SubItemMaterial.Descricao;
                    this.View.QtdeItem = item.QtdeMov.Value;
                    this.View.QtdeLiqItem = item.QtdeLiq.Value;
                    this.View.PrecoUnitItem = item.PrecoUnit.Value;
                    this.View.ValorMovItem = item.ValorMov.Value;
                    this.View.FabricLoteItem = item.FabricanteLote;
                    this.View.DataVctoLoteItem = item.DataVencimentoLote;
                    this.View.IdentificacaoLoteItem = item.IdentificacaoLote;
                    this.View.UnidadeId = item.SubItemMaterial.UnidadeFornecimento.Id.Value;
                    this.View.MostrarPainelEdicao = true;
                    this.View.NaturezaDespesaIdItem = item.SubItemMaterial.NaturezaDespesa.Id.Value;
                    this.View.HabilitarLote = false; // deixar inicializado
                    if (item.Ativo)
                    {
                        this.View.BloqueiaItemEfetivado = true;
                    };
                    if (item.SubItemMaterial.IsLote.HasValue) 
                    {
                        this.View.HabilitarLote = item.SubItemMaterial.IsLote.Value;
                    }

                    //if(this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
                    if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                    {
                        if (!item.QtdeLiq.HasValue || item.QtdeLiq == 0)
                        {
                            this.View.ListaErros = new List<string>() { "Item já liquidado!"};
                            this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                            this.View.MostrarPainelEdicao = false;
                        }
                    }

                    this.View.BloqueiaGravarItem = false;

                    break;
                }
            }
        }

        public MovimentoEntity Estornar(MovimentoEntity mov)
        {
            var perfilLogado_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            MovimentoBusiness estrutura = new MovimentoBusiness();

            estrutura.Movimento = mov;

            // atualizar saldo do BD via SALDO_SUBITEM
            //if (!estrutura.EstornarMovimentoEntrada())
            if (!estrutura.EstornarMovimentoEntrada(perfilLogado_ID, mov.InscricaoCE).Item1)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return null;
            }

            this.View.ExibirMensagem("Estorno de materiais concluído com sucesso.");
            LimparItem();
            this.Cancelar();
            return mov;
        }

        public SubItemMaterialEntity SelectSubItemMaterialRetorno(long SubItemMaterialId)
        {
            return new CatalogoBusiness().SelectSubItemMaterialRetorno((int)SubItemMaterialId);
        }

        public void LerSubItemMaterial(int SubItemMaterialId)
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            SubItemMaterialEntity subItem = catalogo.SelectSubItemMaterialRetorno(SubItemMaterialId);
            this.View.SubItemMaterialDescricao = subItem.Descricao;
            this.View.FabricLoteItem = "";
            this.View.DataVctoLoteItem = null;
            this.View.IdentificacaoLoteItem = "";
            this.View.HabilitarLote = subItem.IsLote.Value ? true : false;
            this.View.NaturezaDespesaIdItem = subItem.NaturezaDespesa.Id.Value;
            this.View.UnidadeId = subItem.UnidadeFornecimento.Id.Value;
        }

        public void LimparItem()
        {
            this.View.IdentificacaoLoteItem = null;
            this.View.MovimentoItemId = null;
            this.View.NaturezaDespesaIdItem = null;
            this.View.PrecoUnitItem = _valorZero;
            this.View.QtdeItem = _valorZero;
            this.View.QtdeLiqItem = _valorZero;
            this.View.SaldoQtdeItem = 0;
			this.View.SaldoQtdeLoteItem = null;
            this.View.ItemMaterialId = null;
            this.View.ItemMaterialCodigo = null;
            this.View.SubItemMaterialCodigo = null;
            this.View.SubItemMaterialTxt = "";
            this.View.SubItemMaterialDescricao = null;
            this.View.SubItemMaterialId = null;
            this.View.UnidadeId = 0;
            this.View.ValorMovItem = _valorZero;
            this.View.AtivoItem = false;
			this.View.DataVctoLoteItem = null;
            this.View.DesdItem = _valorZero;
            this.View.Descricao = null;
            this.View.FabricLoteItem = null;
            this.View.FonteRecurso = null;
            this.View.HabilitarLote = false;
            this.View.ItemMaterialCodigo = null;
            this.View.ItemMaterialDescricao = null;
        }

        public override void Cancelar()
        {
            LimparMovimento();
            base.Cancelar();
            this.View.BloqueiaNovo = false;
            this.View.hiddenMovimentoId = string.Empty;
        }

        public void CancelarItem()
        {
            LimparItem();
            this.View.BloqueiaNovo = false;
        }

        public void LimparMovimentoCompraDireta()
        {
            this.View.BloqueiaDocumento = false;
            this.View.BloqueiaListaDivisao = true;
            this.View.ExibirGeradorDescricao = true;
            this.View.Id = null;
            this.View.NumeroDocumento = null;
        }

        public void LimparMovimento()
        {
            this.View.LimparGridSubItemMaterial();
            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.UgeId = 0;
            this.View.GeradorDescricao = null;
            this.View.FornecedorId = null;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
            this.View.BloqueiaImprimir = true;
            this.View.BloqueiaEstornar = true;
            this.View.BloqueiaGravar = true;
            this.View.SetarUGELogado();
            //this.View.BloqueiaTipoMovimento = false;
            this.View.BloqueiaListaUGE = false;
        }

        public IList<SubItemMaterialEntity> ListarSubItensTodosCod(int _ItemMaterialID)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterialTodosCod(_ItemMaterialID, 7);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, string _documento)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, IConsultarEntradaView consultaView)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName)
        {
            return this.TotalRegistrosGrid;
        }

        public IList<MovimentoItemEntity> ListarMovimentoItens(int startRowIndexParameterName,
                int maximumRowsParameterName, string _documento)
        {
            MovimentoItemBusiness estruturaItem = new MovimentoItemBusiness();
            estruturaItem.SkipRegistros = startRowIndexParameterName;
            IList<MovimentoItemEntity> retorno = estruturaItem.ListarItensPorMovimento(_documento);
            this.TotalRegistrosGrid = estruturaItem.TotalRegistros;
            return retorno;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.EntradaMaterial;
            //RelatorioEntity.Nome = "AlmoxEntradaMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsEntradaMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.EntradaMaterial;
            relatorioImpressao.Nome = "AlmoxEntradaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsEntradaMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void Imprimir(System.Collections.SortedList ParametrosRelatorio)
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.EntradaMaterial;
            //RelatorioEntity.Nome = "AlmoxEntradaMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsEntradaMaterial";
            //RelatorioEntity.Parametros = ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.EntradaMaterial;
            relatorioImpressao.Nome = "AlmoxEntradaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsEntradaMaterial";
            relatorioImpressao.Parametros = ParametrosRelatorio;

            this.View.DadosRelatorio = relatorioImpressao;
        }

        public IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<UnidadeFornecimentoEntity> retorno = estrutura.PopularUnidFornecimentoTodosPorUge(_ugeId);
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorGestor(int _gestorId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<UnidadeFornecimentoEntity> retorno = estrutura.ListarUnidadeFornecimentoTodosCod(0, _gestorId);
            return retorno;
        }

        public IList<NaturezaDespesaEntity> PopularDadosSubItemClassif()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            return estrutura.ListarNaturezaDespesaTodosCod();
        }

        public IList<EmpenhoEventoEntity> PopularEmpenhoEvento() 
        {
            //return new MovimentoBusiness().ListarEmpenhoEvento();
            return new EmpenhoBusiness().ListarEmpenhoEvento();
        }

        public IList<EmpenhoLicitacaoEntity> PopularEmpenhoLicitacao()
        {
            //return new MovimentoBusiness().ListarEmpenhoLicitacao();
            return new EmpenhoBusiness().ListarEmpenhoLicitacao();
        }

        public void HabilitarControlesNovo(int tipoMovimento)
        {
            // bloqueia listagem para evitar erros de usuário.
            switch (tipoMovimento)
            {
                //case (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho:
                case (int)GeralEnum.TipoMovimento.EntradaPorEmpenho:
                    this.View.HabilitarCompraDiretaNovo();
                    break;
                //case (int)GeralEnum.TipoMovimento.AquisicaoAvulsa:
                case (int)GeralEnum.TipoMovimento.EntradaAvulsa:
                    this.View.HabilitarAquisicaoAvulsaNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferencia:
                    this.View.HabilitarTransferenciaNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado:
                    this.View.HabilitarDoacaoNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDevolucao:
                    this.View.HabilitarDevolucaoNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado:
                    this.View.HabilitarMaterialTransformadoNovo();
                    break;
                default:
                    break;
            }
        }

        #region Métodos Utilizados

        /// <summary>
        /// Executa a entrada de Materiais
        /// </summary>
        /// <param name="mov">Movimento de entrada</param>
        /// <returns>Movimento</returns>
        public MovimentoEntity Gravar(MovimentoEntity mov)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();
            SaldoSubItemBusiness estruturaSaldo = new SaldoSubItemBusiness();
            List<string> Inconsistencias = new List<string>();
            mov.Id = null;
            estrutura.Movimento = mov;
            MovimentoEntity movStatus = new MovimentoEntity();
            bool EmpenhoEventoIdValido = false;

            if (estrutura.Movimento.Almoxarifado.Id == 0)
            {
                estrutura.Movimento.Almoxarifado = null;
            }

            if (this.View.DataMovimento == null)
            {
                Inconsistencias.Add("Favor informar a data do documento");
            }

            // entrada por transferência: usar o mesmo nÃºmero do documento da saÃ­da da origem
            estrutura.Movimento.DataDocumento = this.View.DataDocumento;
            estrutura.Movimento.DataMovimento = this.View.DataMovimento;

            if (estrutura.Movimento.DataMovimento < Convert.ToDateTime(DateTime.Now.Date.ToShortDateString()))
            {
                //Se a data for menor que a data atual (retroativo), insere como o ultimo minuto do dia.
                estrutura.Movimento.DataMovimento = estrutura.Movimento.DataMovimento.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else if (estrutura.Movimento.DataMovimento == Convert.ToDateTime(DateTime.Now.Date.ToShortDateString()))
            {
                //Atualiza a data atual com o horario
                estrutura.Movimento.DataMovimento = DateTime.Now;
            }

            estrutura.Movimento.Divisao = new DivisaoEntity(this.View.DivisaoId);
            //if (estrutura.Movimento.NumeroDocumento == Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4))
            if (this.View.NumeroDocumento == "")
            {
                Inconsistencias.Add("Favor informar o nÃºmero do documento.");
            }

            estrutura.Movimento.NumeroDocumento = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + this.View.NumeroDocumento;

            if (estrutura.Movimento.Divisao.Id == 0)
                estrutura.Movimento.Divisao = null;

            foreach (MovimentoItemEntity item in estrutura.Movimento.MovimentoItem)
            {
                item.UGE = new UGEEntity(this.View.UgeId);
            }

            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                if (this.View.NumeroEmpenhoCombo != null && this.View.NumeroEmpenhoCombo != "")
                {
                    estrutura.Movimento.Empenho = this.View.NumeroEmpenhoCombo;
                    estrutura.Movimento.EmpenhoLicitacao = new EmpenhoLicitacaoEntity(this.View.EmpenhoLicitacaoId);
                }
                // ajustar a movimentação para gravar apenas os itens movimentados 
                estrutura.Movimento.ValorDocumento = estrutura.Movimento.MovimentoItem.Sum(a => a.PrecoUnit * a.QtdeMov);
            }
            else
            {
                estrutura.Movimento.Empenho = this.View.Empenho;
            }

            // transfer - seta a UGE selecionada
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
            {
                estrutura.Movimento.Id = null;
                estrutura.Movimento.UGE = new UGEEntity(this.View.UgeId);
            }

            estrutura.Movimento.FonteRecurso = "1";
            if (this.View.FornecedorId.HasValue)
                estrutura.Movimento.Fornecedor = new FornecedorEntity(this.View.FornecedorId);

            estrutura.Movimento.GeradorDescricao = this.View.GeradorDescricao;
            estrutura.Movimento.Instrucoes = "";
            estrutura.Movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(this.View.AlmoxarifadoIdOrigem);
            if (estrutura.Movimento.MovimAlmoxOrigemDestino.Id == 0)
            {
                estrutura.Movimento.MovimAlmoxOrigemDestino = null;
            }
            estrutura.Movimento.Ativo = true;
            estrutura.Movimento.Observacoes = this.View.Observacoes;
            estrutura.Movimento.TipoMovimento = new TipoMovimentoEntity(this.View.TipoMovimento);
            estrutura.Movimento.UGE = new UGEEntity(this.View.UgeId);

            //Adiciona o perfil do usuário Logado no cadastro do movimento
            estrutura.Movimento.IdLogin = Acesso.Transacoes.Perfis[0].IdLogin;

            //Adiciona a data da operação como data atual
            estrutura.Movimento.DataOperacao = DateTime.Now;

            //if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                estrutura.Movimento.ValorDocumento = this.View.ValorDocumento;

            foreach (MovimentoItemEntity item in estrutura.Movimento.MovimentoItem)
            {
                item.Ativo = true;
            }

            //Adiciona o Evento do Empenho no Movimento associado...
            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                EmpenhoEventoIdValido = (mov.EmpenhoEvento.Id.HasValue && mov.EmpenhoEvento.Id.Value != 0);
                EmpenhoEventoIdValido &= estrutura.ListarEmpenhoEvento().Select(a => a.Id == mov.EmpenhoEvento.Id).First();//Refazer

                if (EmpenhoEventoIdValido)
                    estrutura.Movimento.EmpenhoEvento = new EmpenhoEventoEntity(mov.EmpenhoEvento.Id.Value);
            }


            if (Inconsistencias.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = Inconsistencias;
                return null;
            }
            if (!estrutura.GravarMovimento())
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return null;
            }

            ParametrizacaoEntrada param = new ParametrizacaoEntrada();
            //param.tipoMovimento = GeralEnum.TipoMovimento.AquisicaoAvulsa;
            param.tipoMovimento = GeralEnum.TipoMovimento.EntradaAvulsa;

            try
            {
                //Arquitetura Nova
                //MovimentoFactory.GetIMovimentoFactory().EntrarMaterial(param, mov);
            }
            catch (Exception ex)
            {
                estrutura.ListaErro.Add(ex.Message);
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return null;
            }

            this.View.ExibirMensagem("Entrada de materiais concluÃ­da com sucesso.");
            Imprimir();
            this.Cancelar();
            this.View.hiddenMovimentoId = "";
            return estrutura.Movimento;
        }

        #region Tratamento Entrada Empenho

        //public IList<MovimentoEntity> ListarEmpenhos(int pIntUgeId, int pIntAlmoxarifadoId, string pStrCodigoEmpenho, bool pBlnListarJaLiquidados)
        //{
        //    List<MovimentoEntity> lLstRetorno  = null;
        //    EmpenhoBusiness       lObjBusiness = null;

        //    lObjBusiness = new EmpenhoBusiness();
        //    lLstRetorno  = lObjBusiness.ListarEmpenhos(pIntUgeId, pIntAlmoxarifadoId, pStrCodigoEmpenho, null, pBlnListarJaLiquidados, true);

        //    return lLstRetorno;
        //}

        #endregion Tratamento Entrada Empenho

        #endregion
    }
}
