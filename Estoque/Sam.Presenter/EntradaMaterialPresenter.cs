using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Enums;
using Sam.Common.Util;
using Sam.Domain.Business;
using Sam.Domain.Business.SIAFEM;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using Sam.View;
using TipoMaterialParaLancamento = Sam.Common.Util.GeralEnum.TipoMaterial;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoOperacao = Sam.Common.Util.GeralEnum.OperacaoEntrada;



namespace Sam.Presenter
{
    public class EntradaMaterialPresenter : CrudPresenter<IEntradaMaterialView>
    {

        IEntradaMaterialView view;
        private MovimentoBusiness estrutura = null;
        string statusRetornoSIAF_SAM = null;
        public IEntradaMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public EntradaMaterialPresenter(IEntradaMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public EntradaMaterialPresenter()
        {
        }

        public void HabilitarControlesEdit()
        {
            this.View.BloqueiaEstornar = false;
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaCancelar = false;
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

        private decimal? CalcularValorMovimento(MovimentoEntity mov)
        {
            decimal? valorDocumento = 0.00m;

            foreach (var movItem in mov.MovimentoItem)
            {
                valorDocumento += movItem.ValorMov;
            }

            return valorDocumento;
        }

        public void CarregarMovimentoTela(MovimentoEntity mov)
        {
            var anoMesRefAlmox = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            var anoMesRef = (!String.IsNullOrWhiteSpace(mov.AnoMesReferencia)) ? mov.AnoMesReferencia : anoMesRefAlmox;

            this.View.Id = mov.Id.Value.ToString();
            this.View.Empenho = mov.Empenho;

            this.View.ValorDocumento = CalcularValorMovimento(mov);
            this.View.DataDocumento = mov.DataDocumento; 

            this.View.GeradorDescricao = mov.GeradorDescricao;
            //this.View.FornecedorId = mov.Fornecedor.Id;
            this.View.FornecedorId = ((mov.Fornecedor.IsNotNull() && mov.Fornecedor.Id.HasValue) ? mov.Fornecedor.Id.Value : 0);
            if (mov.Divisao != null) this.View.DivisaoId = mov.Divisao.Id;
            this.View.TipoMovimento = mov.TipoMovimento.Id;
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.SaidaPorTransferencia)
                if (mov.Almoxarifado != null) this.View.AlmoxarifadoIdOrigem = mov.Almoxarifado.Id;
                else
                    if (mov.MovimAlmoxOrigemDestino != null) this.View.AlmoxarifadoIdOrigem = mov.MovimAlmoxOrigemDestino.Id;
            if (mov.NumeroDocumento.IsNotNull() && mov.NumeroDocumento.Length == 14)
                this.View.NumeroDocumento = mov.NumeroDocumento == null ? null : (mov.NumeroDocumento.Length == 14 ? mov.NumeroDocumento.Substring(4, 10) : mov.NumeroDocumento);
            else
                this.View.NumeroDocumento = mov.NumeroDocumento == null ? null : (mov.NumeroDocumento.Length == 12 ? mov.NumeroDocumento.Substring(4, 8) : mov.NumeroDocumento);


            this.View.Observacoes = mov.Observacoes;

            if (mov.ValorDocumento.HasValue)
                this.View.ValorDocumento = mov.ValorDocumento.Value.Truncar(anoMesRef, true);

            this.View.EmpenhoLicitacaoId = mov.EmpenhoLicitacao.Id;
            if (mov.EmpenhoEvento.Id.HasValue && mov.EmpenhoEvento.Id != 0)
                this.View.EmpenhoEventoId = mov.EmpenhoEvento.Id;

           // this.View.InscricaoCE = mov.InscricaoCE;
            this.View.InscricaoCEOld = mov.InscricaoCE;

            if (((mov.TipoMovimento.Id == (int)TipoMovimento.EntradaPorTransferencia) || (mov.TipoMovimento.Id == (int)TipoMovimento.EntradaPorDoacaoImplantado)) && (this.View.TipoOperacao == (int)TipoOperacao.Nova))
                this.View.DataDocumento = mov.DataMovimento;           

            if ((this.View.TipoOperacao == (int)TipoOperacao.NotaRecebimento) ||(this.View.TipoOperacao == (int)TipoOperacao.Estorno))
                this.View.DataMovimento = mov.DataMovimento;

            if ((this.View.TipoOperacao == (int)TipoOperacao.NotaRecebimento) && ((mov.TipoMovimento.Id == (int)TipoMovimento.EntradaPorEmpenho) || (mov.TipoMovimento.Id == (int)TipoMovimento.EntradaPorRestosAPagar)))
            {
                this.View.ExibirListaEmpenho = false;
                this.View.ExibirNumeroEmpenho = true;
                this.View.Empenho = mov.Empenho;
            }
        }

        public IList<DivisaoEntity> PopularListaDivisao(int _almoxId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<DivisaoEntity> retorno = estrutura.ListarDivisaoPorAlmoxTodosCod(_almoxId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno.OrderBy(x => x.Descricao).ToList();
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
            if ((View.QtdeItem != _valorZero) && (View.QtdeItem > _valorZero))
                this.View.PrecoUnitItem = (this.View.ValorMovItem / View.QtdeItem);
        }

        public override void Load()
        {
            this.View.BloqueiaNovo = true;
            this.View.PopularListaTipoMovimentoEntrada();
            this.View.MostrarPainelEdicao = false;
            this.View.BloqueiaImprimir = true;
            this.View.BloqueiaValorTotal = true;
            this.View.DataMovimento = DateTime.Now;
        }

        public IList<MovimentoEntity> CarregarListaEmpenho(int _ugeID, string userName, string password)
        {
            EmpenhoBusiness estruturaEmpenho = new EmpenhoBusiness();

            string LoginUsuario = userName;
            string SenhaUsuario = password;
            if (!String.IsNullOrWhiteSpace(userName) && String.IsNullOrWhiteSpace(password))
            {
                LoginUsuario = Siafem.userNameConsulta;
                SenhaUsuario = Siafem.passConsulta;
            }
            //IList<MovimentoEntity> movList = estruturaEmpenho.CarregarListaEmpenho(_ugeID, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef, LoginUsuario, SenhaUsuario, true);
            IList<MovimentoEntity> movList = estruturaEmpenho.CarregarListaEmpenho(_ugeID, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef, LoginUsuario, SenhaUsuario, true);

            if (movList != null)
            {
                if (movList.Count != 0)
                {
                    if (movList[0].Observacoes == "LOGINERROR")
                        estruturaEmpenho.ListaErro.Add("Favor alterar a sua senha de acesso ao Siafem!");
                }
            }

            if (estruturaEmpenho.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estruturaEmpenho.ListaErro;
            }
            return movList;
        }

        //Utilizado
        public IList<string> ObterListaEmpenhos(int almoxarifadoId, int gestorId, int codigoUGE, string loginSiafemUsuario, string senhaSiafemUsuario, bool listarLiquidados = true, bool listarRestosAPagar = false)
        {
            List<string> lLstRetorno = null;
            EmpenhoBusiness lObjBusiness = null;
            IList<string> lILstEmpenhos = null;
            string lStrLoginUsuario = null;
            string lStrSenhaUsuario = null;
            string almoxMesRef = null;

            try
            {
                var almoxLogado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;
                almoxMesRef = almoxLogado.MesRef;

                lLstRetorno = new List<string>();
                lObjBusiness = new EmpenhoBusiness();

                lStrLoginUsuario = Siafem.userNameConsulta;
                lStrSenhaUsuario = Siafem.passConsulta;

                lILstEmpenhos = lObjBusiness.obterListaEmpenhosSiafem(almoxarifadoId, gestorId, codigoUGE, almoxMesRef, loginSiafemUsuario, senhaSiafemUsuario, listarRestosAPagar);
                if (lILstEmpenhos.IsNullOrEmpty() || lObjBusiness.ListaErro.Count > 0)
                {
                    lLstRetorno = null;

                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = lObjBusiness.ListaErro;
                }

                return lILstEmpenhos;
            }
            catch (Exception e)
            {
                lObjBusiness.ListaErro.Add(String.Format("Erro ao executar solicitação servidor: {0}", e.Message));

                new LogErro().GravarLogErro(e);
                this.View.ListaErros = lObjBusiness.ListaErro;
                return null;
            }
        }

        public MovimentoEntity ObterMovimentoEmpenho(int codigoUGE, int almoxID, string numeroEmpenho, string loginUsuarioSiafem, string senhaUsuarioSiafem)
        {
            var anoMesRef = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            EmpenhoBusiness estruturaEmpenho = new EmpenhoBusiness();
            estruturaEmpenho.CarregarFormatos(anoMesRef);

            this.LimparMovimento();
            bool consumoImediato = false;
            MovimentoEntity movEmpenho = estruturaEmpenho.ObterMovimentoEmpenho(codigoUGE, almoxID, numeroEmpenho, anoMesRef, loginUsuarioSiafem, senhaUsuarioSiafem, consumoImediato);
            if (movEmpenho.IsNotNull())
            {
                movEmpenho.Id = 0;
                movEmpenho.TipoMovimento = new TipoMovimentoEntity(this.View.TipoMovimento);
            }
            if (estruturaEmpenho.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estruturaEmpenho.ListaErro;
                return null;
            }

            return movEmpenho;
        }

        public MovimentoEntity obterItensEmpenho(int codigoUGE, int almoxID, string numeroEmpenho, string loginUsuarioSiafem, string senhaUsuarioSiafem)
        {
            var anoMesRef = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            EmpenhoBusiness estruturaEmpenho = new EmpenhoBusiness();
            estruturaEmpenho.CarregarFormatos(anoMesRef);

            this.LimparMovimento();
            bool consumoImediato = false;
            MovimentoEntity movEmpenho = estruturaEmpenho.ObterMovimentoEmpenho(codigoUGE, almoxID, numeroEmpenho, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef, loginUsuarioSiafem, senhaUsuarioSiafem, consumoImediato);
            if (movEmpenho.IsNotNull())
            {
                movEmpenho.Id = 0;
                movEmpenho.TipoMovimento = new TipoMovimentoEntity(this.View.TipoMovimento);
            }
            if (estruturaEmpenho.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estruturaEmpenho.ListaErro;
                return null;
            }

            return movEmpenho;
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

        public IList<MovimentoItemEntity> ListarItemMaterialLote(string Lote, int idFornecedor, int idSubItem)
        {
            MovimentoItemBusiness estruturaItem = new MovimentoItemBusiness();
            IList<MovimentoItemEntity> retorno = estruturaItem.ListarMovimentacaoItemPorSubItemLote(Lote, idFornecedor, idSubItem);

            return retorno;

        }

        public MovimentoEntity BuscaItemLote(MovimentoEntity mov)
        {
            MovimentoItemEntity movItem = new MovimentoItemEntity();
            if (this.View.MovimentoItemId != 0)
                movItem = mov.MovimentoItem.Where(a => a.FabricanteLote == this.View.IdentificacaoLoteItem).FirstOrDefault();

            List<MovimentoItemEntity> newList = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
            newList.Add(movItem);
            mov.MovimentoItem = newList;
            return mov;
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
            return mov;
        }


        public MovimentoEntity GravarItem(MovimentoEntity mov, int flLote, string botao)
        {
            var anoMesReferencia = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;

            if (flLote == 0)
            {
                this.View.FabricLoteItem = null;
                this.View.DataVctoLoteItem = null;
                this.View.IdentificacaoLoteItem = null;
            }

            CatalogoBusiness estrutura = new CatalogoBusiness();
            MovimentoItemBusiness moviItemBus = new MovimentoItemBusiness();
            List<MovimentoItemEntity> movItems = (List<MovimentoItemEntity>)mov.MovimentoItem;
            MovimentoItemEntity movItem = null;
            SaldoSubItemBusiness estruturaSaldo = new SaldoSubItemBusiness();
            movItem = new MovimentoItemEntity();
            if (!mov.Id.HasValue)
                movItem.Movimento = new MovimentoEntity(0);
            else
                movItem.Movimento = new MovimentoEntity(mov.Id.Value);


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

            //if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaEmpenho)
            if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                this.View.QtdeLiqItem = 0;


            movItem.QtdeLiq = this.View.QtdeLiqItem;
            movItem.QtdeMov = this.View.QtdeItem;
            movItem.PrecoUnit = this.View.PrecoUnitItem;
            movItem.UGE = new UGEEntity(this.View.UgeId);
            movItem.ValorMov = this.View.ValorMovItem;

            if (this.View.QtdeItem != _valorZero && this.View.QtdeItem > _valorZero)
            {
                // validar (não salva em banco)
                moviItemBus.CarregarFormatos(anoMesReferencia);
                if (!moviItemBus.ConsistirItem(movItem))
                {
                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = moviItemBus.ListaErro;
                    return null;
                }
                else
                {
                    movItem.PrecoUnit = this.View.PrecoUnitItem.Truncar(anoMesReferencia, true);
                }
            }

            movItem.SaldoQtde = this.View.SaldoQtdeItem;
            movItem.SaldoQtdeLote = this.View.SaldoQtdeLoteItem;
            movItem.SaldoValor = this.View.SaldoValorItem;
            movItem.SubItemMaterial = estrutura.SelectSubItemMaterialRetorno(this.View.SubItemMaterialId.Value);

            if (botao != "btnEditar")
            {
                if (VerificaSeSubitemInserido(mov, movItem))
                {
                    this.View.ExibirMensagem(String.Format("Subitem {0} - {1} já inserido na relação de entrada com este LOTE. Inserir linhas duplicadas não permitido!", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao));
                    return null;
                }

                #region Trava por TipoMaterial
                if (VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(mov, movItem))
                {
                    var tipoMaterialMovimentacao = mov.ObterTipoMaterial();
                    var msgErro = String.Format("Movimentação de entrada permitirá apenas inclusão de subitens de material do tipo {0}", EnumUtils.GetEnumDescription<TipoMaterialParaLancamento>(tipoMaterialMovimentacao));

                    this.View.ExibirMensagem(msgErro);
                    return null;
                }
                #endregion Trava por TipoMaterial
            }

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
        
            
            //if (cat.SubItemMaterial.IsLote.Value == true)        
            if (cat.SubItemMaterial.IsLote.HasValue==true)
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
            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaEmpenho)
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

        public bool VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(MovimentoEntity movimentacaoMaterial, MovimentoItemEntity itemMovimentacao)
        {
            bool ehDivergente = false;
            MovimentoPresenter movimentacaoMaterialPresenter = null;


            movimentacaoMaterialPresenter = new MovimentoPresenter();
            ehDivergente = movimentacaoMaterialPresenter.VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(movimentacaoMaterial, itemMovimentacao);

            return ehDivergente;
        }

        private bool VerificaSeSubitemInserido(MovimentoEntity mov, MovimentoItemEntity movItem)
        {
            MovimentoItemEntity rowSubiteMaterial = null;
            CatalogoBusiness catalogoAlmox = null;

            if (mov.IsNotNull() && mov.MovimentoItem.HasElements() && movItem.IsNotNull())
            {
                catalogoAlmox = new CatalogoBusiness();
                rowSubiteMaterial = mov.MovimentoItem.Where(_movItem => _movItem.SubItemMaterial.Id == movItem.SubItemMaterial.Id && _movItem.IdentificacaoLote == movItem.IdentificacaoLote).FirstOrDefault();
            }

            return rowSubiteMaterial.IsNotNull();
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

            if (mov.MovimentoItem != null)
                mov.MovimentoItem = mov.MovimentoItem.Where(a => a.Id != movItem.Id).ToList();
           
            int i = 0;
            foreach (var item in mov.MovimentoItem)
            {
                var teste = item.Id;
                var teste2 = item.Posicao;
                MovimentoItemEntity movItemNew;
                MovimentoItemEntity movItemAux;
                movItemNew = new MovimentoItemEntity();
                movItemAux = new MovimentoItemEntity();
                movItemAux = mov.MovimentoItem.Where(a => a.Id == item.Id).FirstOrDefault();              
                movItemNew.SubItemMaterial = new SubItemMaterialEntity();
                movItemNew.SubItemMaterial.ItemMaterial = new ItemMaterialEntity();              
                i++;
                movItemAux.Posicao = i;
                movItemAux.Id = i;
                movItemNew.Posicao = i;
                movItemNew.Id = i;
                movItem.Posicao = i;

                movItem.Id = i;               
                
            }          
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
                case (int)GeralEnum.TipoMovimento.EntradaCovid19:
                case (int)GeralEnum.TipoMovimento.EntradaAvulsa:                 
                    this.View.HabilitarAquisicaoAvulsa(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar:
                    this.View.HabilitarAquisicaoRestosAPagar(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferencia:
                    this.View.HabilitarTransferencia(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado:             
                    //this.View.HabilitarDoacao(Editar);
                    this.View.HabilitarDoacaoImplantado(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacao:
                    this.View.HabilitarDoacao(Editar);
                    break;              
                case (int)GeralEnum.TipoMovimento.EntradaPorDevolucao:
                    this.View.HabilitarDevolucao(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado:
                    this.View.HabilitarMaterialTransformado(Editar);
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado:
                    this.View.HabilitarOrgaoTransfSemImpl();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaInventario:
                    this.View.HabilitarInventario(Editar);
                    break;
                default:
                    break;
            }
            this.View.TipoMovimento = tipoMovimento;
        }

        public void LimparCamposMovimento(bool limparListaSubitens = true)
        {
            if (!limparListaSubitens)
                this.View.LimparGridSubItemMaterial();

            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.GeradorDescricao = null;
            this.View.FornecedorId = 0;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
            this.View.PrecoUnitItem = _valorZero;
        }

        public void LimparCamposMovimentoTransfer(bool limparListaSubitens = true)
        {
            if (!limparListaSubitens)
                this.View.LimparGridSubItemMaterial();

            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.GeradorDescricao = null;
            this.View.FornecedorId = 0;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
            this.View.PrecoUnitItem = _valorZero;
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


        public void LerRegistro(MovimentoEntity retorno, int _itemId)
        {
            foreach (MovimentoItemEntity item in retorno.MovimentoItem)
            {
                if (item.Id == _itemId)
                {
                    this.View.UnidadeCodigo = item.SubItemMaterial.UnidadeFornecimento.Codigo + "-" + item.SubItemMaterial.UnidadeFornecimento.Descricao;
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
                            this.View.ListaErros = new List<string>() { "Item já liquidado!" };
                            this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                            this.View.MostrarPainelEdicao = false;
                        }
                    }

                    break;
                }
            }
        }

        //public MovimentoEntity Estornar(MovimentoEntity mov)
        public MovimentoEntity Estornar(string loginSiafemUsuario, string senhaSiafemUsuario, MovimentoEntity mov)
        {
            string msgProcessamentoSIAF = null;
            string msgErroGravacaoEstornoMovimentacao = null;
            string msgGravacaoMovimentacaoSucesso = null;
            string nlLiquidacaoMovimentacaoMaterial = null;
            string nlReclassificacaoMovimentacaoMaterial = null;
            string fmtMsgInformeAoUsuario = null;
            string strDescricaoTipoMovimentacao = null;
            string sistemaOrigemErro = null;
            string descricaoErroSistema = null;
            string[] dadosMsgErroLancamento = null;
            int tipoMovID = 0;

            var perfilLogado_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            MovimentoBusiness estrutura = new MovimentoBusiness();
            estrutura.Movimento = mov;

            if (mov.AnoMesReferencia != Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef)
            {
                estrutura.ListaErro.Add("Não é permitido estornar documentos com o Ano/Mês referência diferentes do almoxarifado.");
                this.View.ListaErros = estrutura.ListaErro;
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                return null;
            }
            else
            {
                // atualizar saldo do BD via SALDO_SUBITEM
                estrutura.Movimento.InscricaoCE = this.View.InscricaoCE;
                estrutura.Movimento.Ativo = false;


                //Verificar se apesar de movimento existir na base, existe pendencia SIAF relacionada ao mesmo, e/ou não tem NL vinculada ao mesmo.
                //Caso não exista vínculos, proceder com estorno.
                var podeEstornarSemSiafem = estrutura.VerificaSePermiteEstornoSemSIAF(estrutura.Movimento);
                if (podeEstornarSemSiafem)
                {
                    //var statusEstornoMovimentacaoSAM = estrutura.EstornarMovimentoEntrada(perfilLogado_ID);
                    var statusEstornoMovimentacaoSAM = estrutura.ExecutaEstornoSemSIAF(perfilLogado_ID, this.View.InscricaoCE);
                    if (statusEstornoMovimentacaoSAM.Item1)
                    {

                        tipoMovID = estrutura.Movimento.TipoMovimento.Id;
                        strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);

                        msgGravacaoMovimentacaoSucesso = String.Format(@"Entrada do tipo ""{0}"", documento {1}, estornada com sucesso!", strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento);
                        this.View.ExibirMensagem(msgGravacaoMovimentacaoSucesso);
                    }
                    else
                    {
                        //msgErroGravacaoEstornoMovimentacao = statusEstornoMovimentacaoSAM.Item2;
                        //estrutura.ListaErro.Insert(0, msgErroGravacaoEstornoMovimentacao);

                        this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                        this.View.ListaErros = statusEstornoMovimentacaoSAM.Item2;
                        return null;
                    }

                    LimparItem();
                    this.Cancelar();
                }
                //Caso contrário, proceder de acordo com regras de negócio acertadas com cliente
                else
                {
                    //Primeiro efetuar estorno no SAM...
                    var statusEstornoMovimentacaoSAM = estrutura.EstornarMovimentoEntrada(perfilLogado_ID, this.View.InscricaoCE);
                    if (statusEstornoMovimentacaoSAM.Item1)
                    {
                        tipoMovID = estrutura.Movimento.TipoMovimento.Id;
                        strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);

                        //...depois no SIAFEM
                        //this.statusRetornoSIAF = ExecutaProcessamentoMovimentacaoNoSIAF(loginSiafemUsuario, senhaSiafemUsuario, estrutura.Movimento);
                        this.statusRetornoSIAF_SAM = ExecutaProcessamentoMovimentacaoNoSIAF(loginSiafemUsuario, senhaSiafemUsuario, "E", estrutura.Movimento);

                        var statusEstornoMovimentacaoSIAFEM = String.IsNullOrWhiteSpace(this.statusRetornoSIAF_SAM);
                        if (statusEstornoMovimentacaoSIAFEM)
                        {
                            //msgGravacaoMovimentacaoSucesso = "Entrada de materiais estornada com sucesso.";
                            msgGravacaoMovimentacaoSucesso = String.Format(@"Entrada do tipo ""{0}"", documento {1}, estornada com sucesso!", strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento);
                            fmtMsgInformeAoUsuario = @"Gerada NL (estorno) número ""{0}"" no SIAFEM";
                            if (String.IsNullOrWhiteSpace(this.statusRetornoSIAF_SAM))
                            {

                                nlLiquidacaoMovimentacaoMaterial = estrutura.Movimento.ObterNLsMovimentacao(false, true);
                                nlReclassificacaoMovimentacaoMaterial = estrutura.Movimento.ObterNLsMovimentacao(false, true, TipoNotaSIAF.NL_Reclassificacao);

                                if(!String.IsNullOrWhiteSpace(nlReclassificacaoMovimentacaoMaterial))
                                    fmtMsgInformeAoUsuario = @"Geradas NLs (estorno) números ""{0}"" (Liquidação) e ""{1}"" (Reclassificação) no SIAFEM";

                                msgProcessamentoSIAF = String.Format(fmtMsgInformeAoUsuario, nlLiquidacaoMovimentacaoMaterial, nlReclassificacaoMovimentacaoMaterial);
                            }

                            this.View.ExibirMensagem(String.Format("{0}\\n{1}", msgGravacaoMovimentacaoSucesso, msgProcessamentoSIAF));
                        }
                        else
                        {
                            dadosMsgErroLancamento = statusRetornoSIAF_SAM.BreakLine("|");
                            if (dadosMsgErroLancamento.Count() == 1)
                                dadosMsgErroLancamento = new string[] { "SAM", statusRetornoSIAF_SAM.BreakLine("|")[0] };

                            sistemaOrigemErro = dadosMsgErroLancamento[0];
                            descricaoErroSistema = dadosMsgErroLancamento[1];
                            msgProcessamentoSIAF = String.Format("Erro {0}: {1}", sistemaOrigemErro, descricaoErroSistema);
                            msgErroGravacaoEstornoMovimentacao = ", ao estornar entrada de materiais no SIAFEM.";
                            msgGravacaoMovimentacaoSucesso = String.Format(@"Entrada do tipo ""{0}"", documento {1}, estornada no SAM com sucesso!", strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento);

                            estrutura.ListaErro.Add((String.Format("{0}{1} {2}", msgProcessamentoSIAF, msgErroGravacaoEstornoMovimentacao, msgGravacaoMovimentacaoSucesso)));
                            estrutura.ListaErro.Add("SIAFEM não registrou, verificar na Tela de Pendências.");

                          
                            this.View.ExibirMensagem("Inconsistências encontradas no SIAFEM, verificar mensagens!");
                            this.View.ListaErros = estrutura.ListaErro;
                        }
                    }
                    else
                    {

                        this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                        this.View.ListaErros = statusEstornoMovimentacaoSAM.Item2;
                        return null;
                    }

                    LimparItem();
                    this.Cancelar();
  
                }
            }

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
            this.View.UnidadeCodigo = subItem.UnidadeFornecimento.Codigo + "-" + subItem.UnidadeFornecimento.Descricao;
        }

        public void LimparItem()
        {
            this.View.IdentificacaoLoteItem = null;
            this.View.MovimentoItemId = null;
            this.View.NaturezaDespesaIdItem = null;
            this.View.PrecoUnitItem = _valorZero;
            this.View.QtdeItem = _valorZero;
            this.View.QtdeLiqItem = _valorZero;
            this.View.SaldoQtdeItem = _valorZero;
            this.View.SaldoQtdeLoteItem = null;
            this.View.ItemMaterialId = null;
            this.View.ItemMaterialCodigo = null;
            this.View.SubItemMaterialCodigo = null;
            this.View.SubItemMaterialTxt = "";
            this.View.SubItemMaterialDescricao = null;
            this.View.SubItemMaterialId = null;
            this.View.UnidadeCodigo = "";
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
            this.View.AlmoxarifadoTransferencia = string.Empty;
            this.View.ValorDocumento = null;
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
        }

        public void LimparMovimentoCompraDireta()
        {
            this.View.BloqueiaDocumento = false;
            this.View.BloqueiaListaDivisao = true;
            this.View.ExibirGeradorDescricao = true;
            this.View.Id = null;
            this.View.NumeroDocumento = null;
        }

        public void LimparMovimento(bool limparListaSubitens = true)
        {
            this.View.RemoverSessao();

            if (limparListaSubitens)
                this.View.LimparGridSubItemMaterial();

            this.View.Id = null;
            this.View.DataDocumento = null;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = null;
            this.View.UgeId = 0;
            this.View.GeradorDescricao = null;
            //this.View.FornecedorId = 0;
            this.View.FonteRecurso = null;
            this.View.Instrucoes = null;
            this.View.Observacoes = null;
            this.View.ValorDocumento = null;
            this.View.Empenho = null;
            this.View.DivisaoId = null;
            this.View.OrgaoTransferencia = null;
           // this.View.BloqueiaImprimir = true;
            //this.View.BloqueiaEstornar = true;
           // this.View.BloqueiaGravar = false;
            this.View.SetarUGELogado();
           // this.View.BloqueiaTipoMovimento = false;
           // this.View.BloqueiaListaUGE = false;
            this.View.AlmoxarifadoTransferencia = string.Empty;
            this.View.ValorTotalMovimento = null;
           // this.View.InscricaoCE = null;
            this.View.InscricaoCEOld = null;
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

        public MovimentoEntity GetMovimentoGrid(int? idMovimento)
        {
            MovimentoBusiness estrutura = new MovimentoBusiness();

            if (idMovimento != null)
            {
                MovimentoEntity mov = new MovimentoEntity();
                mov.Id = idMovimento;

                estrutura.Movimento = mov;
                MovimentoEntity retorno = estrutura.GetMovimento();

                for (int i = 0; i < retorno.MovimentoItem.Count; i++)
                    retorno.MovimentoItem[i].ValorMov = Common.Util.ExtensionMethods.Truncar(Convert.ToDecimal(retorno.MovimentoItem[i].ValorMov), 2);

                return retorno;
            }
            else
            {
                return new MovimentoEntity();
            }
        }

        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.EntradaMaterial;
            relatorioImpressao.Nome = "AlmoxEntradaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsEntradaMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
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
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado:
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacao:
                case (int)GeralEnum.TipoMovimento.EntradaCovid19:
                    this.View.HabilitarAquisicaoAvulsaNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar:
                    this.View.HabilitarAquisicaoRestosAPagarNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferencia:
                    this.View.HabilitarTransferenciaNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado:
                    //this.View.HabilitarDoacaoNovo();
                    this.View.HabilitarTransferenciaNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDevolucao:
                    this.View.HabilitarDevolucaoNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado:
                    this.View.HabilitarMaterialTransformadoNovo();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaInventario:
                    this.View.HabilitarAquisicaoInventario();
                    break;
                default:
                    break;
            }
        }

        #region Métodos Utilizados

        private MovimentoEntity ExecutaEntradaMaterial(string loginSiafemUsuario, string senhaSiafemUsuario, string tipo, MovimentoEntity mov)
        {
            estrutura = new MovimentoBusiness();
            estrutura.Movimento = mov;
            string UgeCPFCnpj = mov.UgeCPFCnpjDestino;

            //Se mês estiver fechado, informar via GUI e abortar processo.
            var mesFiscalFechadoSIAFEM = estrutura.VerificaStatusFechadoMesReferenciaSIAFEM(true);
            if (mesFiscalFechadoSIAFEM)
                return null;


            SaldoSubItemBusiness estruturaSaldo = new SaldoSubItemBusiness();
            List<string> Inconsistencias = new List<string>();
            //mov.Id = null; porque ?
            estrutura.Movimento = mov;
            MovimentoEntity movStatus = new MovimentoEntity();

            if (estrutura.Movimento.Almoxarifado.Id == 0)
                estrutura.Movimento.Almoxarifado = null;

            if (this.View.DataMovimento == null)
                Inconsistencias.Add("Favor informar a data do documento");

            // entrada por transferência: usar o mesmo numero do documento da saÃ­da da origem
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

            if (this.View.NumeroDocumento == "")
            {
                Inconsistencias.Add("Favor informar o número do documento.");
            }

            if (!((this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)))
                estrutura.Movimento.NumeroDocumento = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef.Substring(0, 4) + this.View.NumeroDocumento;

            if (estrutura.Movimento.Divisao.Id == 0)
                estrutura.Movimento.Divisao = null;

            foreach (MovimentoItemEntity item in estrutura.Movimento.MovimentoItem)                
                item.UGE = new UGEEntity(this.View.UgeId);
            

            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                if (this.View.NumeroEmpenhoCombo != null && this.View.NumeroEmpenhoCombo != "")
                {
                    estrutura.Movimento.Empenho = this.View.NumeroEmpenhoCombo;
                    //Estes dados (EmpenhoLicitacao/EmpenhoEvento) são preenchidos pela página, antes da passagem de camada.
                    //estrutura.Movimento.EmpenhoLicitacao = new EmpenhoLicitacaoEntity(this.View.EmpenhoLicitacaoId);
                }
                // ajustar a movimentação para gravar apenas os itens movimentados 
                //estrutura.Movimento.ValorDocumento = estrutura.Movimento.MovimentoItem.Sum(a => a.ValorUnit * a.QtdeLiq);

                estrutura.Movimento.ValorDocumento = estrutura.Movimento.MovimentoItem.Sum(a => a.ValorMov.Value.truncarDuasCasas());
            }
            else if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar)
            {
                estrutura.Movimento.Empenho = this.View.NumeroEmpenhoCombo;
            }           
            // transfer - seta a UGE selecionada aqui
            if ((this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
                || (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)
                || (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado)
                || (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorDoacao))
            {
                estrutura.Movimento.UGE = new UGEEntity(this.View.UgeId);
                //estrutura.Movimento.TipoMovimento.Id = (int)GeralEnum.TipoMovimento.EntradaPorTransferencia;
                estrutura.Movimento.TipoMovimento = new TipoMovimentoEntity(this.View.TipoMovimento);
            }

            estrutura.Movimento.FonteRecurso = "1";
            //if (this.View.FornecedorId.HasValue)
            if (this.View.FornecedorId > 0)
                estrutura.Movimento.Fornecedor = new FornecedorEntity(this.View.FornecedorId);

            if ((this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado) && (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorDoacao))
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
            //estrutura.Movimento.UGE = new UGEEntity(this.View.UgeId);
            if (estrutura.Movimento.UGE.IsNull() || !estrutura.Movimento.UGE.Id.HasValue)
                estrutura.Movimento.UGE = new UGEEntity(this.View.UgeId);

            //Adiciona o perfil do usuário Logado no cadastro do movimento
            estrutura.Movimento.IdLogin = Acesso.Transacoes.Perfis[0].IdLogin;

            //Adiciona a data da operação como data atual
            estrutura.Movimento.DataOperacao = DateTime.Now;

            //if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
                estrutura.Movimento.ValorDocumento = this.View.ValorDocumento.Value.truncarDuasCasas();

            foreach (MovimentoItemEntity item in estrutura.Movimento.MovimentoItem)
                item.Ativo = true;

            //Adiciona o Evento do Empenho no Movimento associado...
            //if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)
            if (this.View.TipoMovimento == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)
            {
                //Estes dados (EmpenhoLicitacao/EmpenhoEvento) são preenchidos pela página, antes da passagem de camada.
                //EmpenhoEventoIdValido = (mov.EmpenhoEvento.Id.HasValue && mov.EmpenhoEvento.Id.Value != 0);
                //EmpenhoEventoIdValido &= estrutura.ListarEmpenhoEvento().Select(a => a.Id == mov.EmpenhoEvento.Id).First();//Refazer

                //if (EmpenhoEventoIdValido)
                //    estrutura.Movimento.EmpenhoEvento = new EmpenhoEventoEntity(mov.EmpenhoEvento.Id.Value);

                if (estrutura.Movimento.MovimentoItem.HasElements()
                   && estrutura.Movimento.MovimentoItem[0].IsNotNull()
                   && estrutura.Movimento.MovimentoItem[0].PTRes.IsNotNull()
                   && (estrutura.Movimento.MovimentoItem[0].PTRes.Codigo.HasValue && estrutura.Movimento.MovimentoItem[0].PTRes.Codigo.Value > 0))
                {
                    PTResEntity tmpPTRes = estrutura.ObterPTRes(estrutura.Movimento.MovimentoItem[0].PTRes.Codigo.Value, this.View.CodigoUGE);
                    foreach (MovimentoItemEntity item in estrutura.Movimento.MovimentoItem)
                        item.PTRes = tmpPTRes;
                }
            }

            //if (mov.TipoMovimento != null && mov.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia)
            if (mov.TipoMovimento != null && ((mov.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) || (mov.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado)))
            {
                AlmoxarifadoEntity almoxarifadoTransfer = null;
                EstruturaOrganizacionalBusiness lObjBusiness = new EstruturaOrganizacionalBusiness();
                //int idGestor = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
                almoxarifadoTransfer = lObjBusiness.ObterAlmoxarifado(this.View.AlmoxarifadoIdOrigem.Value);

                if (almoxarifadoTransfer != null && almoxarifadoTransfer.Id.HasValue)
                    mov.GeradorDescricao = almoxarifadoTransfer.Descricao;
            }

            if (mov.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorDevolucao)
            {
                string strDescricaoDivisao = string.Empty;

                var Divisao = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Divisao;

                if (Divisao != null)
                    strDescricaoDivisao = Divisao.Descricao;

                mov.GeradorDescricao = strDescricaoDivisao;
            }

            if (Inconsistencias.Count > 0)
            {
                //this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                //this.View.ListaErros = Inconsistencias;
                estrutura.ListaErro = Inconsistencias;
                return null;
            }

            if (estrutura.ListaErro.Count > 0)
                estrutura.ListaErro.Clear();

            try
            {
                //estrutura.GravarMovimento();
                estrutura.Movimento.InscricaoCE = this.View.InscricaoCE;
                var statusGravacaoMovimentacao = estrutura.GravarMovimento();

                if (!string.IsNullOrEmpty(UgeCPFCnpj))
                    estrutura.Movimento.UgeCPFCnpjDestino = UgeCPFCnpj;

                if (statusGravacaoMovimentacao)
                    this.statusRetornoSIAF_SAM = ExecutaProcessamentoMovimentacaoNoSIAF(loginSiafemUsuario, senhaSiafemUsuario, tipo, estrutura.Movimento);

                return estrutura.Movimento;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private string ExecutaProcessamentoMovimentacaoNoSIAF(string loginSiafemUsuario, string senhaSiafemUsuario, string tipo, MovimentoEntity movimentacaoMaterial)
        {
            SiafemBusiness siafBusiness = null;
            UsuarioPresenter usuarioPresenter = null;
            Entity.Usuario dadosUsuario = null;


            siafBusiness = new SiafemBusiness();
            usuarioPresenter = new UsuarioPresenter();

            dadosUsuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(movimentacaoMaterial.IdLogin.Value);
            siafBusiness.ExecutaProcessamentoMovimentacaoNoSIAF(dadosUsuario.Cpf, loginSiafemUsuario, senhaSiafemUsuario, tipo, movimentacaoMaterial);

            return siafBusiness.ErroProcessamentoWs;
        }

        /// <summary>
        /// Executa a entrada de Materiais
        /// </summary>
        /// <param name="mov">Movimento de entrada</param>
        /// <returns>Movimento</returns>
        public MovimentoEntity Gravar(string loginSiafemUsuario, string senhaSiafemUsuario, MovimentoEntity mov)
        {
            string msgProcessamentoSIAF = null;
            string msgGravacaoMovimentacaoStatus = null;
            string nlLiquidacaoMovimentacaoMaterial = null;
            string nlReclassificacaoMovimentacaoMaterial = null;
            string fmtMsgInformeAoUsuario = null;
            string msgExecucaoEntrada = null;
            string strDescricaoTipoMovimentacao = null;
            string sistemaOrigemErro = null;
            string descricaoErroSistema = null;
            string msgErroReclassificacao = null;
            string[] dadosMsgErroLancamento = null;
            bool isEntradaSemSIAFEM = false;
            int tipoMovID = 0;


            for (int i = 0; i < 3; i++)
            {
                try
                {
                    ExecutaEntradaMaterial(loginSiafemUsuario, senhaSiafemUsuario, "N", mov);
                    break;
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);

                    estrutura.ListaErro.Add(ex.Message);

                    var erroBanco = ex.Message.ToUpper().Contains("TIMEOUT EXPIRED") || ex.Message.ToUpper().Contains("DEADLOCK");

                    if (!erroBanco)
                        break;
                }
            }

            if (estrutura.ListaErro != null && estrutura.ListaErro.Count() > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                var items = estrutura.ListaErro.GroupBy(x => x).Select(t => t.Key).ToList();
                this.View.ListaErros = items;
                return null;
            }

            if (estrutura.Movimento.IsNotNull() && estrutura.Movimento.Id.HasValue)
            {
                tipoMovID = estrutura.Movimento.TipoMovimento.Id;
                strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);

                isEntradaSemSIAFEM = ((tipoMovID == (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) || (tipoMovID == (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado) || (tipoMovID == (int)GeralEnum.TipoMovimento.EntradaInventario));
                //msgGravacaoMovimentacaoSucesso = "Entrada de materiais concluída com sucesso.";
                msgGravacaoMovimentacaoStatus = String.Format(@"Entrada de materiais do tipo ""{0}"", documento {1} salva com sucesso!", strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento);

                if (isEntradaSemSIAFEM)
                {
                    msgExecucaoEntrada = msgGravacaoMovimentacaoStatus = String.Format(@"Entrada de materiais do tipo ""{0}"", documento {1} salva com sucesso!", strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(this.statusRetornoSIAF_SAM))
                    {
                        fmtMsgInformeAoUsuario = @"Gerada NL número ""{0}"" no SIAFEM";
                        nlLiquidacaoMovimentacaoMaterial = estrutura.Movimento.ObterNLsMovimentacao();
                        nlReclassificacaoMovimentacaoMaterial = estrutura.Movimento.ObterNLsMovimentacao(false, false, TipoNotaSIAF.NL_Reclassificacao);

                        if (!String.IsNullOrWhiteSpace(nlReclassificacaoMovimentacaoMaterial))
                            fmtMsgInformeAoUsuario = @"Geradas NLs números ""{0}"" (Liquidação) e ""{1}"" (Reclassificação) no SIAFEM";

                        msgProcessamentoSIAF = String.Format(fmtMsgInformeAoUsuario, nlLiquidacaoMovimentacaoMaterial, nlReclassificacaoMovimentacaoMaterial);
                    }
                    else
                    {
                        fmtMsgInformeAoUsuario = @"Entrada de materiais do tipo ""{0}"", documento {1} salva com pendências!";
                        dadosMsgErroLancamento = statusRetornoSIAF_SAM.BreakLine("|");
                        if (dadosMsgErroLancamento.Count() == 1)
                            dadosMsgErroLancamento = new string[] { "SAM", statusRetornoSIAF_SAM.BreakLine("|")[0] };

                        sistemaOrigemErro = dadosMsgErroLancamento[0];
                        descricaoErroSistema = dadosMsgErroLancamento[1];

                        
                        nlLiquidacaoMovimentacaoMaterial = estrutura.Movimento.ObterNLsMovimentacao();
                        if (!String.IsNullOrWhiteSpace(nlLiquidacaoMovimentacaoMaterial))
                        {
                            fmtMsgInformeAoUsuario = String.Format("{0}{1}", fmtMsgInformeAoUsuario, @"\nGerada NL número ""{2}"" (Liquidação) no SIAFEM");
                            msgErroReclassificacao = "(reclassificação) ";
                        }

                        msgGravacaoMovimentacaoStatus = String.Format(fmtMsgInformeAoUsuario, strDescricaoTipoMovimentacao, estrutura.Movimento.NumeroDocumento, nlLiquidacaoMovimentacaoMaterial);
                        msgProcessamentoSIAF = String.Format("Erro {0}{1}: \\n{2}", msgErroReclassificacao, sistemaOrigemErro, descricaoErroSistema);
                    }

                    msgExecucaoEntrada = String.Format("{0}\\n{1}", msgGravacaoMovimentacaoStatus, msgProcessamentoSIAF);
                }

                this.View.ExibirMensagem(msgExecucaoEntrada);
                this.Cancelar();
                this.View.hiddenMovimentoId = "";
            }
            return estrutura.Movimento;
        }

        private string _obterDescricaoTipoMovimentacao(int tipoMovID)
        {
            //return GeralEnum.GetEnumDescription((GeralEnum.TipoMovimento)tipoMovID);

            GeralEnum.TipoMovimento tipoMovimento = (GeralEnum.TipoMovimento)tipoMovID;

            if (tipoMovimento == GeralEnum.TipoMovimento.RequisicaoPendente)
                tipoMovimento = GeralEnum.TipoMovimento.RequisicaoAprovada;

            return GeralEnum.GetEnumDescription(tipoMovimento);
        }

        #endregion

        public Tuple<decimal,decimal> RecalculoQtdeLiqItemSiafisico(int itemMaterialCodigo, int iUge_ID, string strEmpenhoSiafisicoCodigo, int iAlmoxarifado_ID, int gestorId, int iNaturezaDespesaCodigo, string unidadeFornSiafem)
        {
            var qtdeLiquidaItemSiafisico = (_valorZero);
            var saldoLiquidaItemSiafisico = (_valorZero);

            EmpenhoBusiness objBusiness = null;

            objBusiness = new EmpenhoBusiness();
            bool consumoImediato = false;
            var somaLiq=objBusiness.RecalculoQtdeLiqItemSiafisico(itemMaterialCodigo, iUge_ID, strEmpenhoSiafisicoCodigo, iAlmoxarifado_ID, gestorId, iNaturezaDespesaCodigo, unidadeFornSiafem, consumoImediato);
            qtdeLiquidaItemSiafisico = somaLiq.Item1;
            saldoLiquidaItemSiafisico = somaLiq.Item2;

            return new Tuple<decimal,decimal>(qtdeLiquidaItemSiafisico,saldoLiquidaItemSiafisico);
        }
    }
}
