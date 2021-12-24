using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Transactions;
using Sam.Domain.Entity.DtoWs;




namespace Sam.Domain.Business
{
    public class MovimentoItemBusiness : BaseBusiness
    {
        private bool forcaFormatoFracionario = false;
        private MovimentoItemEntity movimentoItem = new MovimentoItemEntity();

        public MovimentoItemEntity MovimentoItem
        {
            get { return movimentoItem; }
            set { movimentoItem = value; }
        }

        public IList<MovimentoItemEntity> ListarItensPorMovimento(string Documento)
        {
            MovimentoEntity mov = new MovimentoEntity();
            mov.NumeroDocumento = Documento;
            this.Service<IMovimentoItemService>().SkipRegistros = this.SkipRegistros;
            IList<MovimentoItemEntity> retorno = this.Service<IMovimentoItemService>().ListarPorMovimento(mov);
            this.TotalRegistros = this.Service<IMovimentoItemService>().TotalRegistros();
            return retorno;
        }

        public IList<MovimentoEntity> ListarItensPorMovimentoTodos(MovimentoEntity mov, int TipoRequisicao)
        {
             mov.DataDocumento = mov.DataDocumento == null ? DateTime.MinValue : mov.DataDocumento;
             mov.DataMovimento = mov.DataMovimento == null ? DateTime.MinValue : mov.DataMovimento;
            IList<MovimentoEntity> retorno = this.Service<IMovimentoItemService>().ListarMovimentoItemSaldoTodos(mov);
            return FormatarLote(retorno, TipoRequisicao);
        }



        public IList<MovimentoEntity> FormatarLote(IList<MovimentoEntity> movimento, int TipoRequisicao)
        {
            if (movimento == null)
                return movimento;

            if (movimento.Count == 0)
                return movimento;

            foreach (var saldo in movimento.FirstOrDefault().MovimentoItem)
            {
                StringBuilder sb = new StringBuilder();

                if (saldo.FabricanteLote != null)
                {
                    saldo.SaldoQtde = (saldo.SaldoQtde == null || saldo.SaldoQtde < 0 ? 0 : saldo.SaldoQtde);

                    if (saldo.SaldoQtde.HasValue)
                    {

                        if (saldo.DataVencimentoLote != null || !String.IsNullOrEmpty(saldo.IdentificacaoLote) || !String.IsNullOrEmpty(saldo.FabricanteLote))
                        {
                            if (!saldo.FabricanteLote.Contains("Vários"))
                            {
                                if (saldo.DataVencimentoLote != null)
                                    sb.AppendFormat("Dt Venc: {0}", saldo.DataVencimentoLote.Value.ToShortDateString());
                                else
                                    sb.Append("Dt Venc: N/I");
                            }

                            if (saldo.FabricanteLote != null)
                            {
                                if (saldo.FabricanteLote.Contains("Vários") && (TipoRequisicao == (int)Common.Util.GeralEnum.OperacaoEntrada.NotaRecebimento))
                                {
                                    if (!String.IsNullOrEmpty(saldo.IdentificacaoLote))
                                        sb.AppendFormat("Disponível");
                                       // sb.AppendFormat(" - Lote: {0}", saldo.IdentificacaoLote.ToString());
                                    else
                                        sb.Append(" - Lote: N/I");
                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(saldo.FabricanteLote))
                                        sb.AppendFormat("Disponível");
                                    else
                                        sb.Append(" - Lote: N/I");
                                }
                            }
                            else {
                                if (!String.IsNullOrEmpty(saldo.IdentificacaoLote))
                                    sb.AppendFormat(" - Lote: {0}", saldo.IdentificacaoLote.ToString());
                                else
                                    sb.Append(" - Lote: N/I");
                            }
                        }
                        else
                        {
                            sb.Append("Não Informado");
                        }
                        if (!saldo.FabricanteLote.Contains("Vários"))
                        {
                            if (saldo.QtdeMov != null)
                                sb.AppendFormat(" - Saldo: {0}", (saldo.QtdeMov.Value.ToString(base.fmtFracionarioMaterialQtde)));
                            else if (saldo.SaldoQtde != null)
                                sb.AppendFormat(" - Saldo: {0}", (saldo.SubItemMaterial.SomaSaldoTotal.Value.ToString(base.fmtFracionarioMaterialQtde)));
                            else
                                sb.Append(" - Saldo: 0,000");
                        }

                        saldo.CodigoDescricao = sb.ToString();
                    }
                    else
                    {
                        saldo.SubItemMaterial.SomaSaldoLote.SaldoQtde = 0;
                        saldo.CodigoDescricao = "Não Disponível";
                    }
                }
                else
                {
                    saldo.SubItemMaterial.SomaSaldoLote = new SaldoSubItemEntity();
                    saldo.SubItemMaterial.SomaSaldoLote.SaldoQtde = 0;
                    saldo.CodigoDescricao = "Não Disponível";
                }
            }

            return movimento;
        }

        // impressão de relatórios
        public IList<MovimentoItemEntity> ImprimirMovimentacao(int? _almoxId, int? _tipoMovimento, int? _tipoMovimentoAgrup, int? _fornecedorId, int? _divisaoId, DateTime _dtInicial, DateTime _dtFinal, bool? consultaTransf = false)
        {
            MovimentoItemEntity item = new MovimentoItemEntity();
            item.Movimento = new MovimentoEntity();
            item.Movimento.Ativo = true;
            item.Movimento.TipoMovimento = new TipoMovimentoEntity

            {
                Id = 0,
                AgrupamentoId = 0
            };

            if (_almoxId.HasValue)
            {
                if (_almoxId != 0)
                    item.Movimento.Almoxarifado = new AlmoxarifadoEntity(_almoxId);
            }

            if (_tipoMovimento.HasValue && _tipoMovimento != 0)
            {
                item.Movimento.TipoMovimento.Id = _tipoMovimento.Value;
            }

            if (_tipoMovimentoAgrup.HasValue)
            {
                item.Movimento.TipoMovimento.AgrupamentoId = _tipoMovimentoAgrup;
            }

            if (_fornecedorId != 0)
            {
                item.Movimento.Fornecedor = new FornecedorEntity(_fornecedorId);
            }

            if (_divisaoId != 0)
            {
                item.Movimento.Divisao = new DivisaoEntity(_divisaoId);
            }

            this.Service<IMovimentoItemService>().Entity = item;
            return this.Service<IMovimentoItemService>().ImprimirMovimento(_dtInicial, _dtFinal, consultaTransf);


        }

        /// <summary>
        /// Geração de relatórios de movimentações, para exportação via WS RESTful JSON
        /// </summary>
        /// <param name="orgaoCodigo"></param>
        /// <param name="almoxRequisitanteCodigo"></param>
        /// <param name="uaCodigo"></param>
        /// <param name="DivisaoUaCodigo"></param>
        /// <param name="tipoMovimentacaoMaterialCodigo"></param>
        /// <param name="cpfCnpjFornecedor"></param>
        /// <param name="dataInicialIntervalo"></param>
        /// <param name="dataFinalIntervalo"></param>
        /// <param name="consultaTransf"></param>
        /// <returns></returns>
        public IList<dtoWsMovimentacaoMaterial> GeraRelacaoMovimentacaoMaterialWs(int orgaoCodigo, int ugeCodigo, int codigoAlmox, int? uaCodigo, int? divisaoUaCodigo, int tipoMovimentacaoMaterialCodigo, int agrupamentoTipoMovimentacaoMaterialCodigo, string cpfCnpjFornecedor, DateTime dataInicial, DateTime dataFinal, bool? consultaTransf = false, int numeroPaginaConsulta = 0)
        {
            IList<dtoWsMovimentacaoMaterial> retornoConsulta = null;

            var service = new Sam.Domain.Infrastructure.MovimentoItemInfrastructure();
            retornoConsulta = service.GeraRelatorioMovimentacaoMaterialWs(orgaoCodigo, ugeCodigo, codigoAlmox, uaCodigo, divisaoUaCodigo, tipoMovimentacaoMaterialCodigo, agrupamentoTipoMovimentacaoMaterialCodigo, cpfCnpjFornecedor, dataInicial, dataFinal, consultaTransf, numeroPaginaConsulta);

            this.TotalRegistros = service.TotalRegistros();
            this.SkipRegistros = service.SkipRegistros;
            return retornoConsulta;
        }

        public IList<MovimentoItemEntity> ImprimirMovimentacaoPorDoc(int? _tipoMovimento, int? _ugeId, int? _fornecedorId, int? _divisaoId, string _numeroDocumento)
        {
            return ImprimirMovimentacaoPorDoc(0, _tipoMovimento, _ugeId, _fornecedorId, _divisaoId, _numeroDocumento);
        }


        public IList<MovimentoItemEntity> ImprimirMovimentacaoPorDoc(int? _movimentoId, int? _tipoMovimento, int? _ugeId, int? _fornecedorId, int? _divisaoId, string _numeroDocumento)
        {
            MovimentoItemEntity item = new MovimentoItemEntity();
            item.Movimento = new MovimentoEntity();
            item.Movimento.TipoMovimento = new TipoMovimentoEntity
            {
                Id = _tipoMovimento.Value
            };

            if (_movimentoId.HasValue)
            {
                if (_movimentoId != 0)
                    item.Movimento.Id = _movimentoId;
            }

            if (_fornecedorId != 0)
            {
                item.Movimento.Fornecedor = new FornecedorEntity(_fornecedorId);
            }

            if (_divisaoId != 0)
            {
                item.Movimento.Divisao = new DivisaoEntity(_divisaoId);
            }

            if (_ugeId != 0)
            {
                item.Movimento.UGE = new UGEEntity(_ugeId);
            }


            if (_numeroDocumento != null && _numeroDocumento != "")
            {
                item.Movimento.NumeroDocumento = _numeroDocumento;
            }

            item.Movimento.Ativo = true;

            this.Service<IMovimentoItemService>().Entity = item;
            return this.Service<IMovimentoItemService>().ImprimirMovimento();
        }

        public IList<MovimentoItemEntity> ListarMovimentoEntradaFornecimento(int movimentoId)
        {
            this.Service<IMovimentoService>().Entity = new MovimentoEntity(movimentoId);

            var result = this.Service<IMovimentoService>().GetMovimento();

            if (result == null)
                throw new Exception();
            else
            {
                if (result.MovimentoItem.Count() == 0)
                    throw new Exception();
                else
                {
                    foreach (var item in result.MovimentoItem)
                    {
                        item.Movimento = result;

                        item.ValorMov = ((decimal)item.ValorMov).Truncar(2);
                        item.PrecoUnit = Common.Util.TratamentoDados.CalcularPrecoMedioSaldo(item.ValorMov, item.QtdeMov);

                        //break;
                    }

                    result.ValorOriginalDocumento = result.ValorDocumento;
                    result.ValorDocumento = result.MovimentoItem.Sum(movItem => movItem.ValorMov.Value).Truncar(2);

                    result.MovimentoItem.ToList().ForEach(movItem => movItem.ValorTotalDocEntrada = result.ValorDocumento.ToString());
                    return result.MovimentoItem;
                }
            }
        }

        public IList<MovimentoItemEntity> ListarItemLote(string _lote, int _fornecedorId)
        {
            MovimentoItemEntity item = new MovimentoItemEntity();
            item.Movimento = new MovimentoEntity();
            item.Movimento.Ativo = true;
            item.IdentificacaoLote = _lote;
            item.Movimento.Fornecedor = new FornecedorEntity(_fornecedorId);

            this.Service<IMovimentoItemService>().Entity = item;
            return this.Service<IMovimentoItemService>().ImprimirMovimento();
        }
        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorSubItemUgeData(int? _almoxId, long? _subItemMatId, int? _ugeId, DateTime _dtInicial, DateTime _dtFinal, bool comEstorno)
        {
            if (!_subItemMatId.HasValue)
                _subItemMatId = 0;
            if (!_ugeId.HasValue)
                _ugeId = 0;

            var result = this.Service<IMovimentoItemService>().ListarMovimentacaoItem(_almoxId, _subItemMatId, _ugeId, _dtInicial, _dtFinal, comEstorno);

            if (result.Count == 0)
                throw new Exception();
            else return result;
        }

        public IList<MovimentoItemEntity> ListarMovimentacaoItemPorSubItemLote(string Lote, int idFornecedor, int  idSubItem)
        {
            var result = this.Service<IMovimentoItemService>().ListarSubItemLote(Lote, idFornecedor, idSubItem);
            return result;
		}
      
        public MovimentoItemEntity LerMovimentoItemRegistro(int MovimentoItemId)
        {
            MovimentoItemEntity retorno = this.Service<IMovimentoItemService>().LerRegistroItem(MovimentoItemId);
            return retorno;
        }

        public bool ConsistirItemGravar(MovimentoItemEntity movItem)
        {
            if (!ConsistirItem(movItem))
            {
                return false;
            }

            if (this.ListaErro.Count == 0)
            {
                return true;
            }
            return false;
        }

        // consistir se a qtde a entrar é maior que a quantidade a liquidar 
        public bool ConsistirItemLiquidar(MovimentoItemEntity movItem)
        {
            if (movItem.QtdeMov > movItem.QtdeLiq)
            {
                this.ListaErro.Add("Quantidade a entrar maior do que quantidade a liquidar.");
                return false;
            }
            return true;
        }

        public bool ConsistirItem(MovimentoItemEntity movItem)
        {
            if (!movItem.ValorMov.HasValue || movItem.ValorMov <= 0)
            {
            	string strValorMinimo = Math.Pow(10, -base.numCasasDecimaisValorUnitario).ToString();
                this.ListaErro = new List<string>() { String.Format("Entradas de subitens com preço unitário abaixo de R$ {0} não são permitidas. Verifique qtde/preço.", strValorMinimo) };
            }
            else
            {
                if (!movItem.PrecoUnit.HasValue || movItem.PrecoUnit <= 0)
                {
                    this.ListaErro.Add("Preço unitário obrigatório");
                }
            }

            //if (!movItem.QtdeLiq.HasValue && (movItem.QtdeLiq == 0 && (movItem.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho)))
            if (!movItem.QtdeLiq.HasValue && (movItem.QtdeLiq == 0 && (movItem.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho)))
                this.ListaErro.Add("Quantidade a Liquidar obrigatória");

            if (!movItem.QtdeMov.HasValue || movItem.QtdeMov <= 0)
                this.ListaErro.Add("Quantidade a Entrar obrigatório");

            if (movItem.UGE == null || !movItem.UGE.Id.HasValue)
                this.ListaErro.Add("Informar a UGE!");

            return (this.ListaErro.Count == 0);
        }

        public bool PrecoUnitarioZeroEMilesimal(MovimentoItemEntity movItem)
        {
            int _numCasasDecimais = base.numCasasDecimaisValorUnitario;
            int iPosicaoVirgula = -1;
            int iValorInteiro = 0;
            int iValorQuatroCasasDecimais = 0;
            int iQuantasCasas = 0;
            string strPrecoUnitario = string.Empty;
            bool isMuitoPequeno = false;

            strPrecoUnitario = movItem.PrecoUnit.Value.ToString(base.fmtFracionarioMaterialValorUnitario);
            isMuitoPequeno = movItem.ValorMov < ((decimal)Math.Pow(10, -_numCasasDecimais));

            if (!String.IsNullOrWhiteSpace(strPrecoUnitario) && strPrecoUnitario.Contains(","))
            {
                iPosicaoVirgula = strPrecoUnitario.IndexOf(",");
                iQuantasCasas = strPrecoUnitario.Substring(iPosicaoVirgula + 1).Length;
            }
            else if (!String.IsNullOrWhiteSpace(strPrecoUnitario) && !strPrecoUnitario.Contains(","))
            {
                strPrecoUnitario = String.Format("{0},0000", strPrecoUnitario);
            }


            if (iQuantasCasas == 1)
                strPrecoUnitario = String.Format("{0}000", strPrecoUnitario);


            iPosicaoVirgula = strPrecoUnitario.IndexOf(",");
            iQuantasCasas = strPrecoUnitario.Substring(iPosicaoVirgula + 1).Length;
            Int32.TryParse(strPrecoUnitario.Substring(0, iPosicaoVirgula), out iValorInteiro);
            Int32.TryParse(strPrecoUnitario.Substring(iPosicaoVirgula + 1, _numCasasDecimais), out iValorQuatroCasasDecimais);

            return ((iQuantasCasas > _numCasasDecimais && (iValorInteiro == 0 && iValorQuatroCasasDecimais == 0)) || isMuitoPequeno);
        }

        public string ObterNumerosDocumentoPorMovimentoItemIDs(IList<int> movItems)
        {
            using (TransactionScope transScope = base.ObterConfiguracoesPadraoDeConsulta())
            {
                try
                {
                    return this.Service<IMovimentoItemService>().ObterNumerosDocumentoPorMovimentoItemIDs(movItems);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    transScope.Complete();
                }
            }
        }
    }
}
