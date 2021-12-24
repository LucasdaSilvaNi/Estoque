using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;
using System.Xml;
using Sam.Domain.Infrastructure;
using System.Transactions;
using Sam.Domain.Business;
using tipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using Sam.Common.Enums;
using Sam.Integracao.SIAF.Core;
using Sam.Domain.Business.SIAFEM;


namespace Sam.Domain.Business
{
    public class MovimentoBusiness : BaseBusiness
    {
        private MovimentoEntity movimento = new MovimentoEntity();
        public IList<MovimentoItemEntity> listaMovimentoItem { set; get; }
        public MovimentoEntity Movimento
        {
            get { return movimento; }
            set { movimento = value; }
        }

        public long? MaxNumeroDocumento()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    string ultimoDocumento = this.Service<IMovimentoService>().NumeroUltimoDocumento();

                    long? max;
                    if (string.IsNullOrEmpty(ultimoDocumento))
                        ultimoDocumento = "0";

                    max = Common.Util.TratamentoDados.TryParseLong(ultimoDocumento);
                    return max + 1;
                }
                catch
                {
                    this.ListaErro.Add("Não foi possível gerar o numero do Documento.");
                    return null;
                }
            }
        }
        /// <summary>
        /// Gera Numero de documento de saída seguindo os novos padrões (apartir de jan/2014)
        /// </summary>
        /// <param name="movimento"></param>
        /// <returns></returns>
        public MovimentoEntity GerarNumeroDocumentoSaida(MovimentoEntity movimento)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var ultimoDocumento = MaxNumeroDocumento().ToString();
                // geração do nÃºmero do documento: yyyy/00000000
                try
                {
                    if (!string.IsNullOrEmpty(movimento.AnoMesReferencia))
                        movimento.NumeroDocumento = movimento.AnoMesReferencia.Substring(0, 4) + movimento.UGE.Id + ultimoDocumento.PadLeft(8, '0');
                }
                catch
                {
                    this.ListaErro.Add("Não foi possível gerar o numero do Documento.");
                    return null;
                }
            }

            return movimento;
        }

        public MovimentoEntity GerarNovoDocumento(MovimentoEntity movimento)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var ultimoDocumento = MaxNumeroDocumento().ToString();
                // geração do nÃºmero do documento: yyyy/00000000
                try
                {
                    if (!string.IsNullOrEmpty(movimento.AnoMesReferencia))
                        movimento.NumeroDocumento = movimento.AnoMesReferencia.Substring(0, 4) + movimento.UGE.Id + ultimoDocumento.PadLeft(8, '0');
                }
                catch
                {
                    this.ListaErro.Add("Não foi possível gerar o numero do Documento.");
                    return null;
                }
            }

            return movimento;
        }

        public bool ExisteCodigoInformado()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().Entity = Movimento;

                var retorno = this.Service<IMovimentoService>().ExisteCodigoInformadoDados();
                if (retorno != null)
                {

                    //this.ListaErro.Add("Documento já existe! " + retorno.CodigoFormatado + ", fez o lançamento desta nota em " + retorno.DataMovimento.Value.Date.ToShortDateString() + ". Entrar em contato com a mesma.");
                    this.ListaErro.Add(String.Format("Numero Documento SAM informado já utilizado ({0}) em movimentação de material efetuada para UGE {1}, fornecedor {2} ({3}).", retorno.NumeroDocumento, retorno.UGE.Codigo, retorno.Fornecedor.CpfCnpj, retorno.Fornecedor.Nome));
                    return true;
                }
                return false;
            }
        }

        public bool ExisteCodigoConsumo()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().Entity = Movimento;

                var retorno = this.Service<IMovimentoService>().ExisteCodigoConsumoDados();
                if (retorno != null)
                {
                    //this.ListaErro.Add("Documento já existe! " + retorno.CodigoFormatado + ", fez o lançamento desta nota em " + retorno.DataMovimento.Value.Date.ToShortDateString() + ". Entrar em contato com a mesma.");
                    //this.ListaErro.Add(String.Format("Numero Documento SAM informado já utilizado ({0})!. Movimentação de Material efetuada por chave {1} ({2}), para empenho {3}, UGE {4}, UA {5}, fornecedor {6} ({7})", retorno.NumeroDocumento, "CPF", "NOME", retorno.Empenho, retorno.UGE.Codigo, retorno.UA.Codigo, retorno.Fornecedor.CpfCnpj, retorno.Fornecedor.Nome));
                    this.ListaErro.Add(String.Format("Numero Documento SAM informado já utilizado ({0}) em movimentação de material efetuada para empenho {1}, UGE {2}, UA {3}, fornecedor {4} ({5}).", retorno.NumeroDocumento, retorno.Empenho, retorno.UGE.Codigo, retorno.UA.Codigo, retorno.Fornecedor.CpfCnpj, retorno.Fornecedor.Nome));
                    return true;
                }
                return false;
            }
        }


        public bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId)
        {
            return this.Service<IMovimentoService>().ExisteRequisicaoMaterialPendenteEAtiva(almoxarifadoId);
        }

        public bool ExisteRequisicaoMaterialPendenteEAtiva(int almoxarifadoId, out int? numeroRequisicoesPendentes)
        {
            bool existeRequisicoesPendentes = false;
            int? _numeroRequisicoesPendentes = null;


            existeRequisicoesPendentes = this.Service<IMovimentoService>().ExisteRequisicaoMaterialPendenteEAtiva(almoxarifadoId, out _numeroRequisicoesPendentes);

            numeroRequisicoesPendentes = _numeroRequisicoesPendentes;
            return existeRequisicoesPendentes;
        }


        /// <summary>
        /// Utilizado na requisição de materiais
        /// </summary>
        /// <returns></returns>
        public bool SalvarMovimento()
        {
            //using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
            //{
            try
            {
                this.VerificaSeExisteDocumentoComLote(this.Movimento);
                this.Service<IMovimentoService>().Entity = this.Movimento;
                if (this.ConsistirMovimento())
                {
                    //this.Service<IMovimentoService>().SalvarRequisicao();
                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IMovimentoService>().SalvarRequisicao(false);
                    }
                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
                return false;
            }
            //finally
            //{
            //    tras.Complete();
            //}
            //}
        }

        /// <summary>
        /// Utilizado na requisição de materiais
        /// </summary>
        /// <returns></returns>
        public bool SalvarRequisicaoWs()
        {
            try
            {
                this.VerificaSeExisteDocumentoComLote(this.Movimento);
                this.Service<IMovimentoService>().Entity = this.Movimento;
                if (this.ConsistirMovimento())
                {
                    //this.Service<IMovimentoService>().SalvarRequisicao();
                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IMovimentoService>().SalvarRequisicao(false);
                    }
                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
                return false;
            }
        }

        public void AjustarItensEmpenho()
        {
            IList<MovimentoItemEntity> lstNewMovItem = new List<MovimentoItemEntity>();
            int i = 0;
            foreach (MovimentoItemEntity item in this.Movimento.MovimentoItem)
            {
                if (item.QtdeMov.HasValue || item.QtdeMov > 0)
                {
                    i++;
                    item.Id = i;
                    lstNewMovItem.Add(item);
                }
            }
            // retorna a nova lista de itens
            this.Movimento.MovimentoItem = lstNewMovItem;
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int tipoMovimento, string dataDigitada)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByAlmoxarifado(almoxarifadoId, tipoMovimento, dataDigitada);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();

                return retorno;
            }
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();

                return retorno;
            }
        }

        public IList<MovimentoEntity> ListarRequisicaoByAlmoxarifado(int almoxarifadoId, int divisaoId, int tipoMovimento, string mesRef, bool requisicoesParaEstorno, string dataDigitada)
        {
            this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
            IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByAlmoxarifado(almoxarifadoId, divisaoId, tipoMovimento, mesRef, requisicoesParaEstorno, dataDigitada);
            this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();

            return retorno;
        }

        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                this.Service<IMovimentoService>().Entity = this.Movimento;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarDocumentoByAlmoxarifadoUGE();
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        public IList<MovimentoEntity> ListarDocumentoByAlmoxarifadoUGE(bool docsParaRetorno)
        {

            if (!docsParaRetorno)
                return ListarDocumentoByAlmoxarifadoUGE();

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                this.Service<IMovimentoService>().Entity = this.Movimento;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarDocumentoByAlmoxarifadoUGE(docsParaRetorno);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        public IList<MovimentoEntity> ListarRequisicaoByDivisao(int divisaoId)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByDivisao(divisaoId);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        // Paremeter Divisao TipoMovimento especifico.    
        public IList<MovimentoEntity> ListarRequisicaoByDivisaoAndTipoMovimentoId(int divisaoId, Expression<Func<MovimentoEntity, bool>> expression)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByExpression(expression);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        public IList<MovimentoEntity> ListarRequisicao(int startRow, int maximoRow, string orgaoId = "", string uoId = "", string ugeId = "", string uaId = "", string divisaoId = "", int statusId = 0, string numeroDocumento = "")
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //MovimentoInfrastructure infra = new MovimentoInfrastructure();
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                //IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByExpression(expression);
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicao(startRow, maximoRow, orgaoId, uoId, ugeId, uaId, divisaoId, statusId, numeroDocumento);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        // Paremeter Expression    
        public IList<MovimentoEntity> ListarRequisicaoByNumeroDocumento(string numeroDocumento)
        {
            Expression<Func<MovimentoEntity, bool>> expression = null;
            expression = a => a.NumeroDocumento == numeroDocumento
                                && a.Ativo == true
                                && ((a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                || (a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoCancelada)
                                || (a.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoFinalizada));

            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoByExpression(expression);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        public MovimentoEntity ListarDocumentosRequisicaoByDocumento(string numeroDocumento)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                MovimentoEntity retorno = this.Service<IMovimentoService>().ListarDocumentosRequisicaoByDocumento(numeroDocumento);
                return retorno;
            }
        }

        public MovimentoEntity ListarDocumentosRequisicaoById(int id)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                MovimentoEntity retorno = this.Service<IMovimentoService>().ListarDocumentosRequisicaoById(id);
                return retorno;
            }
        }

        #region Métodos Entrada e saida


        public Boolean GravarMovimentoEntrada()
        {


            try
            {
                // checa se o movimento já existe, com exceção da Entrada por transferência.
                //if (movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                if ((movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                {
                    if (ExisteCodigoInformado())
                        return false;
                }

                // consistências no movimento (não salva nada)
                if (this.ConsistirMovimento())
                {
                    this.Service<IMovimentoService>().Entity = this.Movimento;
                    int? movimentoAnteriorId = this.movimento.Id;

                    this.VerificaMovimentoModificaPrecoMedio(this.Movimento, false);
                    movimento = this.Service<IMovimentoService>().gravarMovimentoEntrada();

                    //if (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                    if ((this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                        this.AtualizaTransferencia(movimentoAnteriorId, movimento.Id, false);

                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
                return false;
            }
        }

        public bool AlmoxarifadoContemFechamentos(int almoxarifadoId)
        {
            bool _almoxarifadoContemFechamentos = false;
            FechamentoMensalBusiness fechamentoBusiness = new FechamentoMensalBusiness();


            _almoxarifadoContemFechamentos = fechamentoBusiness.AlmoxarifadoContemFechamentos(almoxarifadoId);
            return _almoxarifadoContemFechamentos;
        }


        public bool ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(int almoxarifadoId)
        {
            bool _almoxarifadoPossuiMovimentacaoMaterialRetroativaAMesReferenciaAtual = false;
            EstruturaOrganizacionalBusiness almoxarifadoBusiness = new EstruturaOrganizacionalBusiness();


            _almoxarifadoPossuiMovimentacaoMaterialRetroativaAMesReferenciaAtual = almoxarifadoBusiness.ExisteMovimentacaoMaterialRetroativaAMesReferenciaAtual(almoxarifadoId);
            return _almoxarifadoPossuiMovimentacaoMaterialRetroativaAMesReferenciaAtual;
        }

        /// <summary>
        /// Método para fazer a gravação de uma nova entrada
        /// </summary>
        /// <returns></returns>
        public bool GravarMovimento()
        {

            try
            {
                MovimentoRetroativo businessRetroativo = null;
                // checa se o movimento já existe, com exceção da Entrada por transferência.
                //if (movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                if ((movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                {
                    //if (ExisteCodigoInformado())
                    if (ExisteCodigoInformado() || ExistePendenciaParaEsteDocumento())
                        return false;
                }

                // consistências no movimento (não salva nada)
                if (this.ConsistirMovimento())
                {
                    //movimento = this.AtualizarSaldosEntrada(movimento);
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IMovimentoService>().Entity = this.Movimento;
                        int? movimentoAnteriorId = this.movimento.Id;





                        int cont = 0;
                        foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                        {


                            if (movItem.QtdeMov != null && movItem.QtdeMov > 0)
                            {

                                movItem.Retroativo = this.Service<IMovimentoService>().VerificarRetroativoNovoItem(movItem);
                                if (movItem.Retroativo)
                                {

                                    businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                    businessRetroativo.setListaMovimentoSIAFEM(this.Service<IMovimentoService>());
                                    Tuple<IList<MovimentoItemEntity>, bool> retorno = businessRetroativo.RecalculoSIAFEM(this.Service<IMovimentoService>(), cont, movItem, "Entrada");

                                    if (!retorno.Item2)
                                    {
                                        string erro = "Movimentações retroativas que alteram o valor de saídas seguintes a data digitada que há NL,  não são mais permitidas, para executá-la deve se primeiro estornar as saídas ou escolher outra data de recebimento (Verificar Consulta analítico do subitem).";
                                        bool erroList = false;
                                        foreach (var item in retorno.Item1)
                                        {
                                            if (item.ValorOriginalDocumento != item.ValorMov)
                                            {
                                                decimal? diferenca = (item.ValorMov - item.ValorOriginalDocumento);

                                                erroList = true;

                                                if (!string.IsNullOrEmpty(item.NL_Consumo))
                                                    this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - NLConsumo: {5} - SubItem: {6} - NL: {7}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                                else if ((!string.IsNullOrEmpty(item.NL_Liquidacao)) || (!string.IsNullOrEmpty(item.NL_Reclassificacao)))
                                                    this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - SubItem: {5} - NL: {6}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));


                                            }

                                            if (item.PrecoUnit == 0)
                                                this.ListaErro.Add("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");
                                        }

                                        if (erroList)
                                            this.ListaErro.Add("Lançamento retroativo não permitido para movimento que altera o valor da NL lançada no SIAFEM.");


                                        throw new Exception(erro);
                                    }
                                }
                            }

                            cont++;
                        }


                        this.VerificaMovimentoModificaPrecoMedio(this.Movimento, false);
                        movimento = this.Service<IMovimentoService>().GravarMovimento();



                        //if (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                        if ((this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                            this.AtualizaTransferencia(movimentoAnteriorId, movimento.Id, false);

                        int indice = 0;
                        foreach (MovimentoItemEntity movItem in movimento.MovimentoItem)
                        {
                            //Atualiza o MovimentoItem.Movimento com o movimento
                            movItem.Movimento = movimento;

                            //Atualiza Movimento
                            this.Service<IMovimentoService>().Entity = this.Movimento;

                            movItem.AgrupamentoId = this.Movimento.TipoMovimento.AgrupamentoId.Value;
                            var serviceMovimento = this.Service<IMovimentoService>();

                            int IdSaldo = serviceMovimento.ExisteSaldoId(movItem);
                            serviceMovimento.SetEntradaPorEmpenho();
                            if (IdSaldo > 0)
                                serviceMovimento.AtualizarSaldo(movItem, true);
                            else
                                serviceMovimento.InserirSaldo(movItem);
                            //Só irá recalcular o preço médio caso a data seja menor que a data atual (retorativo)   

                            if (this.Service<IMovimentoService>().VerificarRetroativoNovoItem(movItem))
                            {
                                businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                                businessRetroativo.setListaMovimentoLote(this.Service<IMovimentoService>(), movItem);
                                businessRetroativo.Recalculo(this.Service<IMovimentoService>(), indice);

                            }

                            indice++;
                        }

                        ts.Complete();
                    }

                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public bool GravarMovimentoConsumo()
        {

            try
            {
                if (ExisteCodigoConsumo())
                    return false;

                // consistências no movimento (não salva nada)
                if (this.ConsistirMovimento())
                {
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.Service<IMovimentoService>().Entity = this.Movimento;
                        int? movimentoAnteriorId = this.movimento.Id;
                        
                        movimento = this.Service<IMovimentoService>().GravarMovimentoConsumo();
                        ts.Complete();
                    }

                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private bool ExistePendenciaParaEsteDocumento()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var svcPendenciaSIAFEM = this.Service<INotaLancamentoPendenteSIAFEMService>();
                var movMaterial = this.Movimento;

                if (!String.IsNullOrWhiteSpace(movMaterial.NumeroDocumento))
                {
                    var pendenciaEncontrada = svcPendenciaSIAFEM.ExistePendenciaSiafemParaMovimentacao(movMaterial.Almoxarifado.Id.Value, movMaterial.NumeroDocumento, true, true, movMaterial.AnoMesReferencia);
                    if (pendenciaEncontrada.IsNotNull())
                       this.ListaErro.Add(String.Format("Pendência SIAFEM (tipo {0}) existente para movimentação SAM, documento {1}, gerada em {2}, por chave SIAFEM {3}.", obterDescricaoTipoNotaSIAF(pendenciaEncontrada), pendenciaEncontrada.DocumentoSAM, pendenciaEncontrada.DataEnvioMsgWs, pendenciaEncontrada.AuditoriaIntegracaoVinculada.UsuarioSistemaExterno));
                }

                return (this.ListaErro.Count() >= 1);
            }
        }

        private string obterDescricaoTipoNotaSIAF(NotaLancamentoPendenteSIAFEMEntity notaPendenteSIAFEM)
        {
            string descricaoTipoNota = null;

            if (notaPendenteSIAFEM.TipoNotaSIAF == TipoNotaSIAFEM.NL_Reclassificacao)
                descricaoTipoNota = GeralEnum.GetEnumDescription((TipoNotaSIAFEM)notaPendenteSIAFEM.TipoNotaSIAF);
            else
            {
                switch ((tipoMovimento)notaPendenteSIAFEM.MovimentoVinculado.TipoMovimento.Id)
                {
                    case tipoMovimento.EntradaPorEmpenho:
                    case tipoMovimento.EntradaPorRestosAPagar:
                    case tipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                    case tipoMovimento.SaidaPorReclassificacao:
                                                                                        descricaoTipoNota = "NLEmLiq"; break;
                    case tipoMovimento.EntradaAvulsa:
                    case tipoMovimento.EntradaCovid19:
                    case tipoMovimento.EntradaPorDoacaoImplantado:
                    case tipoMovimento.EntradaPorDevolucao:
                    case tipoMovimento.EntradaPorMaterialTransformado:
                    case tipoMovimento.SaidaPorTransferencia:
                    case tipoMovimento.SaidaPorDoacao:
                    case tipoMovimento.OutrasSaidas:
                    case tipoMovimento.SaidaPorMaterialTransformado:
                    case tipoMovimento.SaidaPorExtravioFurtoRoubo:
                    case tipoMovimento.SaidaPorIncorporacaoIndevida:
                    case tipoMovimento.SaidaPorPerda:
                    case tipoMovimento.SaidaInservivelQuebra:
                    case tipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                    case tipoMovimento.SaidaParaAmostraExposicaoAnalise:
                                                                                        descricaoTipoNota = GeralEnum.GetEnumDescription((TipoNotaSIAFEM)notaPendenteSIAFEM.TipoNotaSIAF); break;
                }
            }

            return descricaoTipoNota;
        }

        private bool _isTransferenciaValida(MovimentoEntity movimentacaoMaterial)
        {
            bool transferenciaPodeSerFeita = false;
            int? tipoMovimentacao = null;
            int ugeCodigoAlmoxOrigem = -1;
            int ugeCodigoAlmoxDestino = -1;
            int codigoGestaoAlmoxOrigem = -1;
            int codigoGestaoAlmoxDestino = -1;

            var movimentacaoValida = ((movimentacaoMaterial.IsNotNull() && movimentacaoMaterial.TipoMovimento.IsNotNull()
                                                                        && movimentacaoMaterial.TipoMovimento.Id != 0));

            if (movimentacaoValida)
            {
                tipoMovimentacao = ((movimentacaoMaterial.IsNotNull() && movimentacaoMaterial.TipoMovimento.IsNotNull() && movimentacaoMaterial.TipoMovimento.Id != 0) ? movimentacaoMaterial.TipoMovimento.Id : (new int?()));

                var dadosParaTransferenciaOK = (movimentacaoMaterial.MovimAlmoxOrigemDestino.IsNotNull() && movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.IsNotNull()
                                                                                                         && movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Id.HasValue);

                if (tipoMovimentacao == (int)tipoMovimento.SaidaPorTransferencia && dadosParaTransferenciaOK)
                {
                    ugeCodigoAlmoxOrigem = movimentacaoMaterial.UGE.Codigo.Value;
                    codigoGestaoAlmoxOrigem = movimentacaoMaterial.Almoxarifado.Gestor.CodigoGestao.Value;
                    ugeCodigoAlmoxDestino = movimentacaoMaterial.MovimAlmoxOrigemDestino.Uge.Codigo.Value;
                    codigoGestaoAlmoxDestino = movimentacaoMaterial.MovimAlmoxOrigemDestino.Gestor.CodigoGestao.Value;

                    if (ugeCodigoAlmoxDestino == ugeCodigoAlmoxOrigem)
                    {
                        transferenciaPodeSerFeita = true;
                        //TODO: [DouglasBatista] Verificar como gerar NL de reclassificação.
                    }
                    else if (codigoGestaoAlmoxOrigem == codigoGestaoAlmoxDestino)
                    {
                        transferenciaPodeSerFeita = true;
                    }
                    else if (codigoGestaoAlmoxOrigem != codigoGestaoAlmoxDestino)
                    {
                       this.ListaErro.Add("Transferência de Materiais entre UGEs de gestões diferentes, não permitida, efetuar Movimentação de Doação");
                        throw new Exception("Ação Inválida");
                    }
                }
                else
                {
                    transferenciaPodeSerFeita = true;
                }
            }

            return transferenciaPodeSerFeita;
        }

        /// <summary>
        /// Serializa o movimento e seus relacionamentos e grava no banco de dados
        /// </summary>
        /// <param name="movimento">Movimento a ser serializado</param>
        public void SalvarRascunhoSaida(MovimentoEntity movimento)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                byte[] byteObj = Common.Util.SerializacaoUtil<MovimentoEntity>.Serializar(movimento);
                var movObj = Common.Util.SerializacaoUtil<MovimentoEntity>.Deserializar(byteObj);
            }
        }

        public MovimentoEntity GetMovimentoSerializado(int id)
        {
            byte[] byteObj = null; //GetOBJ banco

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var movObj = Common.Util.SerializacaoUtil<MovimentoEntity>.Deserializar(byteObj);

                return movObj;
            }
        }

        public MovimentoEntity ListaMovimentosRascunhoSaida(int transacaoId, int usuarioId)
        {
            return new MovimentoEntity();//Rascunho
        }


    
        /// <summary>
        /// Método para gravar o movimento da saida
        /// </summary>
        /// <returns>Retorna true caso tenha salvado a saída com sucesso</returns>
        public bool SalvarMovimentoSaida()
        {
            
            try
            {
                MovimentoRetroativo businessRetroativo = null;
                List<SaldoSubItemEntity> saldoList = new List<SaldoSubItemEntity>();
                this.Service<IMovimentoService>().Entity = this.Movimento;
                EstruturaOrganizacionalBusiness eoBusiness = new EstruturaOrganizacionalBusiness();
                int natureza = 0;
                string mesReferenciaPatrimonio;
                string mesReferenciaDestino = null;
                string mesReferenciaMovimentacao = null;

                if (this.Movimento.TipoMovimento.Id == tipoMovimento.RequisicaoAprovada.GetHashCode())
                {
                    var codigoOrgaoSAM = this.Service<IMovimentoService>().GetOrgaoPorUge(this.Movimento.UGE.Id.Value);
                    if (codigoOrgaoSAM == 38) //SAP
                    {
                        try
                        {
                            mesReferenciaPatrimonio = Convert.ToString(PatrimonioBusiness.GetManagerUnitDataReferencia(Movimento.UGE.Codigo.Value));

                        }
                        catch (Exception ex)
                        {
                            mesReferenciaPatrimonio = string.Empty;
                        }


                        var tipoMaterialMovimentacaoMaterial = this.Movimento.ObterTipoMaterial();
                        if (tipoMaterialMovimentacaoMaterial == GeralEnum.TipoMaterial.MaterialPermanente)
                        {
                            if (string.IsNullOrEmpty(mesReferenciaPatrimonio))
                                mesReferenciaPatrimonio = "Sem mês de referência do patrimônio";

                            if (mesReferenciaPatrimonio != this.Movimento.AnoMesReferencia)
                            {
                                string mensagem = string.Format("Erro na integração, mês de referência diferentes entre estoque e patrimônio, Estoque: {0} e Patrimônio: {1}", this.Movimento.AnoMesReferencia, mesReferenciaPatrimonio);
                                throw new Exception(mensagem);
                            }
                        }
                    }
                }
                else if (this.Movimento.TipoMovimento.Id == tipoMovimento.SaidaPorTransferencia.GetHashCode() || (this.Movimento.TipoMovimento.Id == tipoMovimento.SaidaPorDoacao.GetHashCode()))
                {
                    string msgErro = null;

                    if (this.Movimento.MovimAlmoxOrigemDestino.IsNotNull() && 
                        this.Movimento.MovimAlmoxOrigemDestino.Id.HasValue &&
                        this.Movimento.MovimAlmoxOrigemDestino.Id > 0)
                    {
                        var mesReferenciaAtual_Destino = eoBusiness.ObtemMesReferenciaAtual(this.Movimento.MovimAlmoxOrigemDestino.Id.GetValueOrDefault());
                        var mesReferenciaAtual_Origem = this.Movimento.AnoMesReferencia;

                        if (mesReferenciaAtual_Destino != mesReferenciaAtual_Origem)
                        {
                            string mesReferenciaAtual_DestinoFormatado = null;
                            mesReferenciaAtual_DestinoFormatado = String.Format("{0}/{1}", mesReferenciaAtual_Destino.Substring(4, 2), mesReferenciaAtual_Destino.Substring(0, 4));
                            msgErro = string.Format("Os dois almoxarifados devem estar no mesmo mês-referência para que a movimentação de transferência/doação seja efetuada (Mês-Referência Almoxarifado-Destino: {0})", mesReferenciaAtual_DestinoFormatado);
                            throw new Exception(msgErro);
                        }
                    }
                    else
                    {
                        msgErro = string.Format("Não foi possível obter os dados do almoxarifado de destino de transferência/doação");
                        throw new Exception(msgErro);
                    }
                    
                }


                natureza = 0;
                if (this.ConsistirMovimento())
                {

                    using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        this.VerificaMovimentoModificaPrecoMedioSaida(this.Movimento, false);
                        //Grava a Saída para retornar o MovimentoItemID e verificar o saldo retroativo

                        int cont = 0;
                        foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                        {
                            movItem.Retroativo = this.Service<IMovimentoService>().VerificarRetroativoNovoItem(movItem);
                            if (movItem.Retroativo)
                            {

                                businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                businessRetroativo.setListaMovimentoSIAFEM(this.Service<IMovimentoService>());
                                Tuple<IList<MovimentoItemEntity>, bool> retorno = businessRetroativo.RecalculoSIAFEM(this.Service<IMovimentoService>(), cont, movItem, "Saida");

                                if (!retorno.Item2)
                                {
                                    bool erroList = false;
                                    string erro = "Movimentações retroativas que alteram o valor de saídas seguintes a data digitada que há NL, não são mais permitidas, para executá-la deve se primeiro estornar as saídas mencionadas ou escolher outra data (Verificar Consulta analítico do subitem).";

                                    foreach (var item in retorno.Item1)
                                    {
                                        if (item.ValorOriginalDocumento != item.ValorMov)
                                        {
                                            erroList = true;
                                            decimal? diferenca = (item.ValorMov - item.ValorOriginalDocumento);
                                            item.ValorMov = item.ValorMov.Value.truncarDuasCasas();
                                            diferenca = diferenca.Value.truncarDuasCasas();

                                            if (!string.IsNullOrEmpty(item.NL_Consumo))
                                                this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - NLConsumo: {5} - SubItem: {6} - NL: {7}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                            else if ((!string.IsNullOrEmpty(item.NL_Liquidacao)) || (!string.IsNullOrEmpty(item.NL_Reclassificacao)))
                                                this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - SubItem: {5} - NL: {6}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));


                                        }

                                        if (item.PrecoUnit == 0)
                                            this.ListaErro.Add("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");
                                    }

                                    if (erroList)
                                        this.ListaErro.Add("Lançamento retroativo não permitido para movimento que altera o valor da NL lançada no SIAFEM.");


                                    throw new Exception(erro);
                                }
                            }

                            cont++;
                        }


                        this.Movimento = this.Service<IMovimentoService>().SalvarSaida();


                        //this.VerificaMovimentoModificaPrecoMedioSaida(this.movimento, false);
                        //Verifica se o saldo é positivo no movimento retroativo
                        VerificarSaldoPositivoMovimento();

                        //Contorna um problema no desdobro de saída por lote.
                        //Teste -- Varios Itens
                        //  VerificaSeExisteDocumentoComLote(this.movimento);

                        //Atualiza Objeto
                        this.Service<IMovimentoService>().Entity = this.Movimento;

                        //Recalcula o saldo dos itens
                        int indice = 0;
                        foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                        {
                            //Atualiza o MovimentoItem.Movimento com o movimento
                            movItem.Movimento = movimento;
                            natureza = movItem.NaturezaDespesaCodigo;

                            //Só irá recalcular o preço médio caso a data seja menor que a data atual (retorativo)
                            if (this.Service<IMovimentoService>().VerificarRetroativoNovoItem(movItem))
                            {
                                businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                                businessRetroativo.setListaMovimentoLote(this.Service<IMovimentoService>(), movItem);
                                businessRetroativo.Recalculo(this.Service<IMovimentoService>(), indice);
                            }
                            indice++;
                        }
                        tras.Complete();
                    }


                    if (natureza.ToString().Substring(0, 4) == Convert.ToString((int)GeralEnum.NaturezaDespesa.Permanente))
                        this.Service<IMovimentoService>().ExecutarIntegracao((int)this.Movimento.Id, false, false);
                }
                else
                    return false;

                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
                throw new Exception(e.Message, e.InnerException);

            }
        }

        public Tuple<string, bool> AtualizarMovimentoSubItem(MovimentoItemEntity movItem)
        {
            try
            {

                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    Tuple<string, bool> retorno = this.Service<IMovimentoService>().AtualizarMovimentoSubItem(movItem);

                    tras.Complete();

                    return retorno;
                }
            }
            catch (Exception e)
            {
                return Tuple.Create(e.Message, false);

            }
        }

        public Tuple<string, bool> AtualizarMovimentoBloquear(int movId, bool bloquear)
        {
            try
            {

                using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    Tuple<string, bool> retorno = this.Service<IMovimentoService>().AtualizarMovimentoBloquear(movId, bloquear);

                    tras.Complete();

                    return retorno;
                }
            }
            catch (Exception e)
            {
                return Tuple.Create(e.Message, false);

            }
        }

        public MovimentoItemEntity ConsultaSaldoMovimentoItem(MovimentoItemEntity movItem)
        {
            return this.Service<IMovimentoService>().ConsultaSaldoMovimentoItem(movItem);
        }

        //private void VerificaMovimentoModificaPrecoMedio(MovimentoItemEntity movItem)
        //{
        //    if (!this.Service<IMovimentoService>().VerificaMovimentoModificaPrecoMedio(movItem, true))
        //    throw new Exception(String.Format("Entrada retroativa inválida â€“ Existe transferência posterior."));
        //}



        public void VerificaMovimentoModificaPrecoMedio(MovimentoEntity mov, bool isEstorno)
        {
            string subItens = string.Empty;
            foreach (var movItem in mov.MovimentoItem)
            {
                if (VerificarRetroativo(movItem)) //Retroativo verifica o saldo no Movimento Item
                {
                    movItem.Movimento.DataMovimento = mov.DataMovimento;
                    movItem.Movimento.Almoxarifado = new AlmoxarifadoEntity(Convert.ToInt32(mov.Almoxarifado.Id));
                    if (this.Service<IMovimentoService>().VerificaMovimentoModificaPrecoMedio(movItem, isEstorno))
                    {
                        subItens += movItem.SubItemCodigoFormatado + ", ";

                    }
                }
            }
            if (!string.IsNullOrEmpty(subItens))
                if (isEstorno)
                    throw new Exception("Estorno de entrada inválido - Altera o preço médio  e/ou valor movimento de Saída(s) por Transferência(s) posterior à data desse movimento no(s) subitem(ns) " + subItens + " verificar Consulta Analítica.");
                else
                    throw new Exception("Entrada inválido - Altera o preço médio  e/ou valor movimento de Saída(s) por Transferência(s) posterior à data desse movimento no(s) subitem(ns) " + subItens + " verificar Consulta Analítica.");
        }

        public void VerificaMovimentoModificaPrecoMedioSaida(MovimentoEntity mov, bool isEstorno)
        {
            string subItens = string.Empty;
            foreach (var movItem in mov.MovimentoItem)
            {
                if (VerificarRetroativo(movItem)) //Retroativo verifica o saldo no Movimento Item
                {

                    if (VerificarEntrada(movItem))
                        if (this.Service<IMovimentoService>().VerificaMovimentoModificaPrecoMedio(movItem, isEstorno))
                        {
                            subItens += movItem.SubItemCodigoFormatado + ", ";

                        }
                }
            }
            if (!string.IsNullOrEmpty(subItens))
            {
                if (isEstorno)
                    throw new Exception("Estorno de saída inválido - Altera o preço médio e/ou valor movimento de Saída(s) por Transferência(s) posterior à data desse movimento no(s) subitem(ns) " + subItens + " verificar Consulta Analítica.");
                else
                    throw new Exception("Saída inválido - Altera o preço médio  e/ou valor movimento de Saída(s) por Transferência(s) posterior à data desse movimento no(s) subitem(ns) " + subItens + " verificar Consulta Analítica.");
            }
        }

        public void VerificarSaldoPositivoMovimento()
        {
            this.Service<IMovimentoService>().Entity = this.Movimento;
            foreach (var movItem in Movimento.MovimentoItem)
            {
                if (VerificarRetroativo(movItem)) //Retroativo verifica o saldo no Movimento Item
                {
                    var movimento = this.Service<IMovimentoService>().ConsultaSaldoMovimentoItemLote(movItem);
                    if ((movimento.SaldoQtdeLote - movItem.QtdeMov) < 0)
                    {
                        throw new Exception(String.Format("Saldo Insuficiente para a saída na data informada. Código SubItem: {0} - Descrição: {1} - Lote: {2} - Saldo Disponível em {3}: {4}", movimento.SubItemMaterial.Codigo, movimento.SubItemMaterial.Descricao, movimento.IdentificacaoLote, movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), movimento.SaldoQtdeLote));
                    }
                }
                else // saida normal, verifica o saldo no saldo sub item
                {
                    if (!this.Service<IMovimentoService>().VerificarSaldoPositivo(movItem))
                    {
                        //throw new Exception(String.Format("Saldo Insuficiente para a saída na data informada. Código SubItem: {0} - Descrição: {1}", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao));
                        throw new Exception(String.Format("Saldo Insuficiente para a saída na data informada. Código SubItem: {0} - Descrição: {1} - Lote: {2} - Saldo Disponível em {3}: {4}", movItem.SubItemMaterial.Codigo, movItem.SubItemMaterial.Descricao, movItem.IdentificacaoLote, movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), movItem.SaldoQtdeLote));
                    }
                }
            }
        }

        public bool VerificarLote(MovimentoItemEntity movItem)
        {
            bool isLote = false;

            if (!String.IsNullOrEmpty(movItem.FabricanteLote))
                isLote = true;
            if (!String.IsNullOrEmpty(movItem.IdentificacaoLote))
                isLote = true;
            if ((movItem.DataVencimentoLote != null))
                isLote = true;

            return isLote;
        }

        public bool VerificarRetroativo(MovimentoItemEntity movItem)
        {
            this.Service<IMovimentoService>().Entity = this.Movimento;
            var result = this.Service<IMovimentoService>().VerificarRetroativo(movItem);
            return result;

        }

        public bool VerificarRetroativoNovoItem(MovimentoItemEntity movItem)
        {
            this.Service<IMovimentoService>().Entity = this.Movimento;
            var result = this.Service<IMovimentoService>().VerificarRetroativoNovoItem(movItem);
            return result;

        }


        public bool VerificarEntrada(MovimentoItemEntity movItem)
        {
            this.Service<IMovimentoService>().Entity = this.Movimento;
            var result = this.Service<IMovimentoService>().VerificarEntrada(movItem);
            return result;

        }

        public bool VerificarSeEstoqueZerado(MovimentoEntity movimento)
        {
            foreach (MovimentoItemEntity movimentoItem in this.Movimento.MovimentoItem)
            {
                this.Service<IMovimentoService>().Entity = this.Movimento;
                bool result = this.Service<IMovimentoService>().VerificarSeEstoqueZerado(movimentoItem);
                return result;
            }

            return false;
        }

        /// <summary>
        /// Valida se existe um subitem com o mesmo UGE e almox no Movimento Item
        /// </summary>
        /// <param name="movimento">Movimento a ser validado</param>
        public void VerificaSeExisteDocumentoComLote(MovimentoEntity movimento)
        {
            var result = (from movItem in movimento.MovimentoItem
                          group movItem by new
                          {
                              almoxid = movimento.Almoxarifado.Id,
                              ugeId = movItem.UGE.Id,
                              subItemId = movItem.SubItemMaterial.Id
                          } into g
                          select new MovimentoItemEntity
                          {
                              Id = g.Key.subItemId
                          }).Count();
            if (result < movimento.MovimentoItem.Count())
                throw new Exception("Para lançar saida do mesmo material, utilizar um novo documento.");
        }

        private bool ConsistirEstorno()
        {
            return VerificarSaldoPositivo() && PermitirEstornarEntrada();
            //return VerificarSaldoPositivo() && PermitirEstornarEntrada()
            //                                && (ExisteNotaSiafemEstornoVinculada() || VerificaSePermiteEstornoSemSIAF(this.Movimento))
            //                                && ValidarCampoSiafemCE(); //Validações por conta da integracao com SIAFEM, para permissão de estorno.

        }

        private bool ExisteNotaSiafemEstornoVinculada()
        {
            return ExisteNotaSiafemVinculada(true);
        }

        private bool ExisteNotaSiafemVinculada(bool verificaEstornoNotaSIAF = false)
        {
            bool blnStatusPagamentoSIAF = false;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                try
                {
                    blnStatusPagamentoSIAF = this.Service<IMovimentoService>().ExisteNotaSiafemVinculada(this.Movimento.Id.Value, verificaEstornoNotaSIAF);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Erro ao realizar estorno da movimentação {0}, erro: {1}", this.Movimento.NumeroDocumento, ex.InnerException));
                }
                finally
                {
                    ts.Complete();
                }
            }

            return blnStatusPagamentoSIAF;
        }

        private bool ValidarCampoSiafemCE()
        {
            bool statusValido = false;
            bool campoCEVazio = false;
            bool naoEhEntradaPorTransferencia = true;
            bool ehEntradaPorEmpenho = false;

            naoEhEntradaPorTransferencia = ((this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) ||
                                            (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado));
            ehEntradaPorEmpenho = (this.Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.EntradaPorEmpenho);

            campoCEVazio = String.IsNullOrWhiteSpace(Movimento.InscricaoCE);

            statusValido = ((!campoCEVazio && naoEhEntradaPorTransferencia) || (!naoEhEntradaPorTransferencia) || (ehEntradaPorEmpenho));
            if (!statusValido)
               this.ListaErro.Add("Campo CE é de preenchimento obrigatório!");

            return statusValido;
        }

        public IList<string> ObterNLsMovimentacao(int movimentacaoMaterialID, Enum @TipoNotaSIAFEM, bool retornaNLEstorno = false, bool usaTransacao = false)
        {
            IList<string> lstNLsMovimentacaoMaterial = null;
            var srvInfra = this.Service<Sam.ServiceInfraestructure.IMovimentoItemService>();
            TransactionScopeOption opcaoConsulta = (usaTransacao ? TransactionScopeOption.RequiresNew : TransactionScopeOption.Suppress);

            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            using (TransactionScope ts = new TransactionScope(opcaoConsulta, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    lstNLsMovimentacaoMaterial = srvInfra.ObterNLsMovimentacao(movimentacaoMaterialID, @TipoNotaSIAFEM, retornaNLEstorno, usaTransacao);
                }
                catch (Exception excErroGravacao)
                {
                    throw excErroGravacao;
                }

                ts.Complete();
            }

            return lstNLsMovimentacaoMaterial;
        }

        /// <summary>
        /// Verifica se o movimento tem itens com saldo negativo ao estornar entrada ou efetuar saída
        /// </summary>
        /// <returns></returns>
        private bool VerificarSaldoPositivo()
        {
            int contFalse = 0;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                foreach (MovimentoItemEntity item in this.Movimento.MovimentoItem)
                {
                    if (!Service<IMovimentoService>().VerificarSaldoPositivo(item))
                    {
                       this.ListaErro.Add(String.Format("Saldo insuficiente no Subitem de Material: {0} - {1}", item.SubItemMaterial.Codigo.ToString().PadLeft(12, '0'), item.SubItemMaterial.Descricao));
                        contFalse++;
                    }
                }
            }
            if (contFalse > 0) return false;
            return true;
        }

        private bool PermitirEstornarEntrada()
        {
            int contFalse = 0;
            //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = TimeSpan.FromMinutes(60) }))
            //{
     
                foreach (MovimentoItemEntity item in this.Movimento.MovimentoItem)
                {
                    if (!Service<IMovimentoService>().PermitirEstornarEntrada(item))
                    {
                       this.ListaErro.Add(String.Format("Não é permitido estornar o item: {0} - {1}, pois existem movimentos de saida.",
                            item.SubItemMaterial.Codigo.ToString().PadLeft(12, '0'), item.SubItemMaterial.Descricao));
                        contFalse++;
                    }
                }
               
           // }
            if (contFalse > 0) return false;
            return true;
        }

        public MovimentoEntity GetMovimento()
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                this.Service<IMovimentoService>().Entity = this.Movimento;
                return this.Service<IMovimentoService>().GetMovimento();
            }
        }

        /// <summary>
        /// Implementação da regra de negócio R7.        
        /// </summary>
        public void AtualizaValorTotalDocumentoTransferencia()
        {
            try
            {
                //Esse método irá validar apenas os movimentos do tipo Saida por Transferencia
                if (this.movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia)
                    return;

                //Verifica se o valor do documento está diferente a soma dos Itens. Retorna uma exception
                this.VerificaValorDocumentoTransferenciaEx();
            }
            catch (InvalidOperationException e)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    GravaHistoricoDocumentoTransferencia();
                    AtualizaValorDocumento();
                    ts.Complete();
                }
            }
        }

        public void AtualizaValorTotalDocumentoAlterado()
        {
            try
            {
                //Esse método irá validar apenas os movimentos do tipo Saida por Transferencia
                //ALT 26/10/2015: e validar também movimentos do tipo Saida Por Doacao
                //if (this.movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia)
                //if (!((this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia) || (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao)))
                //    return;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    GravaHistoricoDocumentoTransferencia();
                    AtualizaValorDocumento();
                    ts.Complete();
                }
            }
            catch (InvalidOperationException e)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    GravaHistoricoDocumentoTransferencia();
                    AtualizaValorDocumento();
                    ts.Complete();
                }
            }
        }

        private void AtualizaValorDocumento()
        {

            decimal? valorMovimentoSomado = 0;

            foreach (MovimentoItemEntity item in this.movimento.MovimentoItem)
            {
                valorMovimentoSomado += item.ValorMov;
            }

            this.Service<IMovimentoService>().AtualizarMovimentoValorDocumento(this.movimento, (decimal)valorMovimentoSomado);
        }

        private void GravaHistoricoDocumentoTransferencia()
        {
            this.Service<IMovimentoService>().GravarMovimentoHistorico(this.movimento);

            foreach (var movItem in this.Movimento.MovimentoItem)
            {
                this.Service<IMovimentoService>().GravarMovimentoItemHistorico(movItem);
            }
        }

        public bool VerificarMovimentoEntradaAnterior()
        {
            this.Service<IMovimentoService>().Entity = this.Movimento;

            bool retorno = true;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                foreach (var movItem in Movimento.MovimentoItem)
                {
                    retorno = retorno && this.Service<IMovimentoService>().VerificarMovimentoEntradaAnterior(movItem);
                }

                if (!retorno)
                {
                   this.ListaErro.Add("Não existem movimentações de entrada anteriores a data de movimentação");
                    return false;
                }
                else
                    return true;
            }
        }

        /// <summary>
        /// Método que estorna a Entrada do almoxarifado
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, List<string>> EstornarMovimentoEntrada(int loginIdEstornante, string InscricaoCE)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    MovimentoRetroativo businessRetroativo = null;
                    this.Service<IMovimentoService>().Entity = this.Movimento;
                    this.Movimento = this.Service<IMovimentoService>().GetMovimento();



                    if (this.Movimento == null)
                        throw new Exception("O documento não está mais disponível no sistema.");

                    int cont = 0;
                    foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                    {
                        bool erroList = false;

                        movItem.Retroativo = this.Service<IMovimentoService>().VerificarRetroativo(movItem);
                        if (movItem.Retroativo)
                        {

                            businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                            businessRetroativo.Estorno = true;
                            List<MovimentoItemEntity> list = businessRetroativo.setListaMovimentoSIAFEM(this.Service<IMovimentoService>());

                            foreach (var item in list)
                            {
                                if (movItem.Movimento.Id == item.Movimento.Id)
                                {
                                    if (!string.IsNullOrEmpty(item.NL_Consumo))
                                    {
                                        erroList = true;
                                       this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor: {2} - NLConsumo: {3} - SubItem: {4} - NL: {5}", item.Documento, item.TipoMovimentoDescricao, item.ValorMov, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                        return new Tuple<bool, List<string>>(false, this.ListaErro);
                                    }
                                }
                            }

                            if (erroList)
                               this.ListaErro.Add("Lançamento estorno não permitido para movimento que tem LANÇAMENTO DE NL CONSUMO no SIAFEM.");

                            Tuple<IList<MovimentoItemEntity>, bool> retorno = businessRetroativo.RecalculoSIAFEM(this.Service<IMovimentoService>(), cont, movItem, "Estorno");

                            if (!retorno.Item2)
                            {
                                erroList = false;
                                foreach (var item in retorno.Item1)
                                {
                                    if (item.ValorOriginalDocumento != item.ValorMov)
                                    {
                                        decimal? diferenca = (item.ValorMov - item.ValorOriginalDocumento);

                                        erroList = true;

                                        if (!string.IsNullOrEmpty(item.NL_Consumo))
                                           this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - NLConsumo: {5} - SubItem: {6} - NL: {7}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                        else if ((!string.IsNullOrEmpty(item.NL_Liquidacao)) || (!string.IsNullOrEmpty(item.NL_Reclassificacao)))
                                           this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - SubItem: {5} - NL: {6}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                    }

                                    if (item.PrecoUnit == 0)
                                       this.ListaErro.Add("Erro ao calcular o preço unitário, problema na conexão, saia do sistema e entre novamente");
                                }

                                if (erroList)
                                {
                                   this.ListaErro.Add("Lançamento retroativo não permitido para movimento que altera o valor da NL lançada no SIAFEM.");
                                   this.ListaErro.Add("Movimentações de estorno retroativas que alteram o valor de saídas seguintes a data digitada que há NL, não são mais permitidas, para executá-la deve se primeiro estornar as saídas mencionadas (Verificar Consulta analítico do subitem).");
                                }

                                return new Tuple<bool, List<string>>(false, this.ListaErro);
                            }

                            cont++;
                        }
                    }

                    this.VerificaMovimentoModificaPrecoMedio(this.Movimento, true);
                    this.Service<IMovimentoService>().Entity = this.Movimento;

                    if (ConsistirEstorno())
                    {
                        //Retorna o movimento item atualizado do banco de dados
                        if (this.Movimento != null)
                        {


                            this.Service<IMovimentoService>().AtualizarMovimentoEstorno(loginIdEstornante, InscricaoCE);//Muda o status do movimento atual para false

                            //if (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                            if ((this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                                this.AtualizaTransferencia(null, this.Movimento.Id, true);

                            int indice = 0;
                            foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                            {
                                //Atualiza o saldo do estorno
                                this.Service<IMovimentoService>().Entity = this.Movimento;

                                movItem.AgrupamentoId = this.Movimento.TipoMovimento.AgrupamentoId.Value;

                                this.Service<IMovimentoService>().AtualizarSaldo(movItem, false);

                                //Atualiza o MovimentoItem.Movimento com o movimento
                                movItem.Movimento = movimento;

                                ////Existe movimentos para serem recalculados?
                                if (this.Service<IMovimentoService>().VerificarRetroativo(movItem))
                                {
                                    businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                    businessRetroativo.Estorno = true;
                                    businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                                    businessRetroativo.setListaMovimentoLote(this.Service<IMovimentoService>(), movItem);
                                    businessRetroativo.Recalculo(this.Service<IMovimentoService>(), indice);

                                    // Grava o movimento atual que foi estornado no historico
                                    this.Service<IMovimentoService>().GravarMovimentoItemHistorico(movItem);
                                }

                                indice++;
                            }
                        }
                        else
                        {
                            //Else movimento Nulo
                           this.ListaErro.Add("Não foi possível encontrar o movimento no banco de dados.");
                            return new Tuple<bool, List<string>>(false, new List<string> { "Não foi possível encontrar o movimento no banco de dados." });
                        }
                    }
                    else
                    {
                        return new Tuple<bool, List<string>>(false, this.ListaErro);
                    }

                    ts.Complete();
                    return new Tuple<bool, List<string>>(true, null);
                }
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
                return new Tuple<bool, List<string>>(false, new List<string> { e.Message });
            }

        }

        public Tuple<bool, List<string>> EstornarMovimentoConsumo(int loginIdEstornante, string InscricaoCE)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    MovimentoRetroativo businessRetroativo = null;
                    this.Service<IMovimentoService>().Entity = this.Movimento;
                    this.Movimento = this.Service<IMovimentoService>().GetMovimento();



                    if (this.Movimento == null)
                        throw new Exception("O documento não está mais disponível no sistema.");

                   
                    this.Service<IMovimentoService>().Entity = this.Movimento;

             
                        //Retorna o movimento item atualizado do banco de dados
                        if (this.Movimento != null)
                        {


                            this.Service<IMovimentoService>().AtualizarMovimentoEstorno(loginIdEstornante, InscricaoCE);//Muda o status do movimento atual para false

                        
                            if ((this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) || (this.movimento.TipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                                this.AtualizaTransferencia(null, this.Movimento.Id, true);

                          
                        }
                        else
                        {
                            //Else movimento Nulo
                           this.ListaErro.Add("Não foi possível encontrar o movimento no banco de dados.");
                            return new Tuple<bool, List<string>>(false, new List<string> { "Não foi possível encontrar o movimento no banco de dados." });
                        }
                 
                    ts.Complete();
                    return new Tuple<bool, List<string>>(true, null);
                }
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
                return new Tuple<bool, List<string>>(false, new List<string> { e.Message });
            }

        }

        public IList<MovimentoItemEntity> getListaItensCorrigidos(Int32? almoxarifadoId, Int64? subItemId, int gestorId)
        {
            IList<MovimentoItemEntity> listaPendencias = new List<MovimentoItemEntity>();


            IList<Int32> listaSubItem = null;
            IList<AlmoxarifadoEntity> listaAlmoxarifado = null;
            Int32? UgeId = null;


            listaAlmoxarifado = new List<AlmoxarifadoEntity>();
            AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(almoxarifadoId.Value);

            listaAlmoxarifado.Add(almoxarifado);

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {

                try
                {
                    UgeId = this.Service<IMovimentoService>().retornaUgeDoAlmoxarifado(almoxarifadoId.Value);

                    if (subItemId.Value > 0)
                        listaSubItem = this.Service<IMovimentoService>().retornaIdDoSubItem(subItemId.Value, gestorId);
                    else
                        listaSubItem = this.Service<IMovimentoService>().retornaIdDoSubItemPorAlmoxarifado(almoxarifadoId.Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
                finally
                {
                    ts.Complete();
                }
            }

            int indice = 0;
            foreach (Int32 SubItemId in listaSubItem)
            {


                MovimentoItemEntity item = new MovimentoItemEntity();
                MovimentoEntity movimento = new MovimentoEntity();


                item.Almoxarifado = almoxarifado;
                UGEEntity uge = new UGEEntity(UgeId);
                item.UGE = uge;

                SubItemMaterialEntity subMaterial = new SubItemMaterialEntity(SubItemId);
                item.SubItemMaterial = subMaterial;
                item.UGE = uge;
                movimento.Almoxarifado = almoxarifado;
                movimento.UGE = uge;
                movimento.AnoMesReferencia = "201301";
                movimento.DataMovimento = new DateTime(2013, 01, 01);
                item.Movimento = movimento;
                item.DataMovimento = movimento.DataMovimento;



                MovimentoRetroativo businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(item);


                this.Service<IMovimentoService>().Entity = movimento;

                businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                businessRetroativo.setListaMovimentoLote(this.Service<IMovimentoService>(), item);
                businessRetroativo.Correcao = true;
                businessRetroativo.gerarPendencias(this.Service<IMovimentoService>(), listaPendencias, indice);


            }

            return listaPendencias;
        }
        public void CorrigirSubItem(MovimentoItemEntity movimentoItem)
        {
            //lock (this)
            //{
            IList<MovimentoItemEntity> _listaMovimentoItem = new List<MovimentoItemEntity>();

            String mensagem = validarCorrecaoSubItem(movimentoItem);
            IList<Int32> listaSubItem = null;
            IList<AlmoxarifadoEntity> listaAlmoxarifado = null;
            Int32? UgeId = null;

            if (movimentoItem.Almoxarifado != null && movimentoItem.Almoxarifado.Id.Value > 0)
            {
                listaAlmoxarifado = new List<AlmoxarifadoEntity>();
                AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity(movimentoItem.Almoxarifado.Id.Value);
                almoxarifado.Codigo = movimentoItem.Almoxarifado.Codigo;
                listaAlmoxarifado.Add(almoxarifado);
            }
            else
            {
                listaAlmoxarifado = this.Service<IMovimentoService>().ListarAlmoxarifadoPorGestorMovimentoPendente(1);
            }



            foreach (AlmoxarifadoEntity almoxarifado in listaAlmoxarifado)
            {



                if (mensagem.Trim().Length > 0)
                    throw new Exception(mensagem);

                UgeId = this.Service<IMovimentoService>().retornaUgeDoAlmoxarifado(almoxarifado.Id.Value);

                if (movimentoItem.SubItemCodigo.Value > 0)
                    listaSubItem = this.Service<IMovimentoService>().retornaIdDoSubItem(movimentoItem.SubItemCodigo.Value, Convert.ToInt32(movimentoItem.Movimento.Almoxarifado.Gestor.Id));
                else
                    listaSubItem = this.Service<IMovimentoService>().retornaIdDoSubItemPorAlmoxarifado(almoxarifado.Id.Value);

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead }))
                {
                    try
                    {
                        int indice = 0;
                        foreach (Int32 SubItemId in listaSubItem)
                        {

                            // DateTime? MenorDataMovimento = this.Service<IMovimentoService>().primeiraDataMovimentoDoSubItemDoAlmoxarifado(movimentoItem.Almoxarifado.Id.Value, UgeId.Value,SubItemId);

                            MovimentoItemEntity item = new MovimentoItemEntity();
                            MovimentoEntity movimento = new MovimentoEntity();


                            item.Almoxarifado = almoxarifado;
                            UGEEntity uge = new UGEEntity(UgeId);
                            item.UGE = uge;

                            SubItemMaterialEntity subMaterial = new SubItemMaterialEntity(SubItemId);
                            item.SubItemMaterial = subMaterial;
                            item.UGE = uge;

                            movimento.Almoxarifado = almoxarifado;
                            movimento.AnoMesReferencia = movimentoItem.Movimento.AnoMesReferencia;
                            movimento.DataMovimento = movimentoItem.DataMovimento;
                            movimento.UGE = uge;
                            item.Movimento = movimento;
                            item.CodigoDescricao = movimentoItem.CodigoDescricao;
                            item.DataMovimento = movimentoItem.DataMovimento;



                            // movItem.DataMovimento.Value, movItem.SubItemMaterial.Id.Value, movItem.UGE.Id.Value, movItem.Movimento.Almoxarifado.Id.Value


                            MovimentoRetroativo businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(item);


                            this.Service<IMovimentoService>().Entity = movimento;


                            businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                            businessRetroativo.Correcao = true;
                            businessRetroativo.gerarRelatorio = false;
                            businessRetroativo.Corrigir(this.Service<IMovimentoService>(), indice);


                            if (businessRetroativo.listaMovimentoItemErro != null)
                            {
                                foreach (MovimentoItemEntity MovimentoItem in businessRetroativo.listaMovimentoItemErro)
                                {
                                    MovimentoItem.Movimento.Almoxarifado = almoxarifado;

                                    if (MovimentoItem.TipoMovimentoId == 12)
                                    {

                                        MovimentoInfrastructure infra = new MovimentoInfrastructure();
                                        var retorno = MovimentoBusiness.comverterMovimentoItemToMovimentoItemEntity(infra.getMovimentoEntradaPorTransferencia(item.SubItemMaterial.Id.Value, MovimentoItem));

                                        if (retorno != null)
                                        {
                                            _listaMovimentoItem.Add(MovimentoItem);
                                            _listaMovimentoItem.Add(retorno);
                                        }
                                    }
                                    else
                                    {
                                        _listaMovimentoItem.Add(MovimentoItem);
                                    }

                                }
                            }
                            indice++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message, ex.InnerException);
                    }
                    finally
                    {
                        ts.Complete();
                        this.listaMovimentoItem = _listaMovimentoItem;
                    }
                }
            }
            //}
        }

        private String validarCorrecaoSubItem(MovimentoItemEntity movimentoItem)
        {
            if (!movimentoItem.Almoxarifado.Id.HasValue)
                return "Favor informar o almoxarifado!";
            else if (movimentoItem.AnoMesReferencia.Trim().Length < 1)
                return "Favor informar a Mês de Referência";
            else
                return "";
        }
        public static MovimentoItemEntity comverterMovimentoItemToMovimentoItemEntity(TB_MOVIMENTO_ITEM item)
        {
            if (item != null)
            {

                MovimentoBusiness movimentoBusiness = new MovimentoBusiness();

                AlmoxarifadoEntity almoxarifado = movimentoBusiness.Service<IAlmoxarifadoService>().ObterAlmoxarifado(item.TB_MOVIMENTO.TB_ALMOXARIFADO_ID);

                MovimentoItemEntity movimentoItem = new MovimentoItemEntity();
                movimentoItem.Id = item.TB_MOVIMENTO_ITEM_ID;
                movimentoItem.DataMovimento = item.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO;
                TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity(item.TB_MOVIMENTO.TB_TIPO_MOVIMENTO_ID);
                MovimentoEntity movimento = new MovimentoEntity(item.TB_MOVIMENTO_ID);
                movimento.TipoMovimento = tipoMovimento;
                movimento.DataMovimento = item.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO;
                movimento.Almoxarifado = almoxarifado;
                movimento.NumeroDocumento = item.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO;
                movimento.Instrucoes = "Por favor realizar a correção do Almox e Subitem, pois o mesmo foi alterado devido uma transferência!";
                movimentoItem.Movimento = movimento;

                UGEEntity uge = new UGEEntity(item.TB_UGE_ID);
                SubItemMaterialEntity subItem = new SubItemMaterialEntity(item.TB_SUBITEM_MATERIAL_ID);
                subItem.Codigo = item.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO;
                movimentoItem.UGE = uge;
                movimentoItem.SubItemMaterial = subItem;
                movimentoItem.PrecoUnit = item.TB_MOVIMENTO_ITEM_PRECO_UNIT;
                movimentoItem.Desd = item.TB_MOVIMENTO_ITEM_DESD;
                movimentoItem.SaldoQtde = item.TB_MOVIMENTO_ITEM_SALDO_QTDE;
                movimentoItem.SaldoValor = item.TB_MOVIMENTO_ITEM_SALDO_VALOR;
                movimentoItem.QtdeMov = item.TB_MOVIMENTO_ITEM_QTDE_MOV;
                movimentoItem.ValorMov = item.TB_MOVIMENTO_ITEM_VALOR_MOV;


                return movimentoItem;
            }
            return null;


        }

        private void AtualizaTransferencia(int? movimentoSaidaId, int? movimentoEntradaId, bool isEstorno)
        {
            this.Service<IMovimentoService>().AtualizaTransferencia(movimentoSaidaId, movimentoEntradaId, isEstorno);
        }

        /// <summary>
        /// Retorna o Movimento Item com os dados atual, com o saldo atualizado
        /// </summary>
        /// <param name="movimentoItem"></param>
        /// <param name="movimentoItemSaldoAnterior"></param>
        /// <returns></returns>
        private MovimentoItemEntity RetornaMovimentoItemSaldo(MovimentoItemEntity movimentoItem, MovimentoItemEntity movimentoItemSaldoAnterior)
        {
            MovimentoItemEntity retornaMovimentoItemSaldo = new MovimentoItemEntity();
            retornaMovimentoItemSaldo = movimentoItem;

            retornaMovimentoItemSaldo.SaldoQtde = movimentoItemSaldoAnterior.SaldoQtde;
            retornaMovimentoItemSaldo.SaldoValor = movimentoItemSaldoAnterior.SaldoValor;
            retornaMovimentoItemSaldo.Desd = movimentoItemSaldoAnterior.Desd;
            retornaMovimentoItemSaldo.PrecoUnit = movimentoItemSaldoAnterior.PrecoUnit;
            retornaMovimentoItemSaldo.QtdeMov = movimentoItemSaldoAnterior.QtdeMov;
            retornaMovimentoItemSaldo.ValorMov = movimentoItemSaldoAnterior.ValorMov;
            retornaMovimentoItemSaldo.SaldoQtdeLote = movimentoItemSaldoAnterior.SaldoQtdeLote;

            return retornaMovimentoItemSaldo;
        }

        private void RecalcularPrecoMedioMovimento(MovimentoItemEntity movimentoItemRecalculado, bool isEstorno)
        {
            if (movimentoItemRecalculado == null)
                throw new Exception("Não é possível recalcular os movimentos. ");

            IList<MovimentoItemEntity> movimentoItemRecalcular = new List<MovimentoItemEntity>();

            //Lista os movimentos itens para serem recalculados a partir do movimento atual.
            movimentoItemRecalcular = Service<IMovimentoService>().ListarMovimentacaoItemPorIdEstorno(movimentoItemRecalculado, isEstorno);
            SaldoSubItemBusiness saldoSubItem = new SaldoSubItemBusiness();
            float precoUnitario = 0F;
            Boolean possuiMovimentoEntrada = false;
            MovimentoItemEntity movimento = null;

            if (movimentoItemRecalcular.Count > 0)
            {
                foreach (MovimentoItemEntity itemAtualizar in movimentoItemRecalcular)
                {
                    //Considera o saldo anterior do retroativo e concatena os valores dos movimentos conforme a ordem.
                    if (itemAtualizar.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                    {
                        // cálculo de preço médio de entrada de material
                        movimentoItemRecalculado.SaldoQtde += itemAtualizar.QtdeMov.Value;
                        movimentoItemRecalculado.SaldoValor += itemAtualizar.ValorMov.Value;
                        if (!movimentoItemRecalculado.SaldoQtde.HasValue)
                        {

                            saldoSubItem.gravarLogSaldoNulo(itemAtualizar, "MovimentoBusiness", "RecalcularPrecoMedioMovimento");
                        }

                        itemAtualizar.SaldoQtde = movimentoItemRecalculado.SaldoQtde;
                        itemAtualizar.SaldoValor = movimentoItemRecalculado.SaldoValor;
                        // precoUnitario = float.Parse(Service<IMovimentoService>().CalcularPrecoMedioSaldo(movimentoItemRecalculado.SaldoValor, movimentoItemRecalculado.SaldoQtde, movimentoItemRecalculado.AnoMesReferencia).ToString());

                        itemAtualizar.Movimento.AnoMesReferencia = movimentoItemRecalculado.AnoMesReferencia;

                        precoUnitario = float.Parse(Service<IMovimentoService>().CalcularPrecoMedioSaldo(movimentoItemRecalculado.SaldoValor.Value, movimentoItemRecalculado.SaldoQtde.Value, movimentoItemRecalculado.AnoMesReferencia).ToString());
                        itemAtualizar.PrecoUnit = precoUnitario.truncarQuatroCasas();
                        AtualizarDesdobroSaldo(itemAtualizar);
                        possuiMovimentoEntrada = true;
                        // itemAtualizar.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(movimentoItemRecalculado.SaldoValor, movimentoItemRecalculado.SaldoQtde, movimentoItemRecalculado.AnoMesReferencia);
                    }
                    else if (itemAtualizar.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                    {
                        // cálculo de preço médio de saída de material
                        if (!possuiMovimentoEntrada)
                        {

                            if (movimento == null)
                                movimento = Service<IMovimentoService>().RetornaMovimentoEntradaDaSaida(itemAtualizar);

                            AtualizarDesdobroSaldo(movimento);
                            precoUnitario = float.Parse(movimento.PrecoUnit.Value.ToString());


                        }

                        itemAtualizar.PrecoUnit = precoUnitario.truncarQuatroCasas();
                        //itemAtualizar.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(movimentoItemRecalculado.SaldoValor, movimentoItemRecalculado.SaldoQtde,movimentoItemRecalculado.AnoMesReferencia);
                        // itemAtualizar.Desd = Service<IMovimentoService>().CalcularDesdobro(movimentoItemRecalculado.SaldoValor, itemAtualizar.PrecoUnit, movimentoItemRecalculado.SaldoQtde);
                        movimentoItemRecalculado.SaldoQtde -= itemAtualizar.QtdeMov.Value;

                        decimal QtdeMov = itemAtualizar.QtdeMov.Value;
                        decimal PrecoUnit = itemAtualizar.PrecoUnit.Value;
                        decimal SaldoValor = movimentoItemRecalculado.SaldoValor.Value;
                        SaldoValor -= QtdeMov * PrecoUnit;// +(itemAtualizar.Desd.HasValue ? float.Parse(itemAtualizar.Desd.Value.truncarQuatroCasas().ToString()) : 0F);

                        movimentoItemRecalculado.SaldoValor = SaldoValor;

                        //movimentoItemRecalculado.SaldoValor -= (itemAtualizar.QtdeMov.Value.truncarQuatroCasas() * itemAtualizar.PrecoUnit.Value.truncarQuatroCasas()) + (itemAtualizar.Desd.HasValue ? itemAtualizar.Desd : 0);

                        decimal ValorMov = QtdeMov * PrecoUnit + (itemAtualizar.Desd.HasValue ? itemAtualizar.Desd.Value : 0M);

                        // itemAtualizar.ValorMov = (itemAtualizar.QtdeMov.Value.truncarQuatroCasas() * itemAtualizar.PrecoUnit.Value.truncarQuatroCasas()) + (itemAtualizar.Desd.HasValue ? itemAtualizar.Desd : 0);
                        itemAtualizar.ValorMov = ValorMov;

                        if (!movimentoItemRecalculado.SaldoQtde.HasValue)
                        {

                            saldoSubItem.gravarLogSaldoNulo(itemAtualizar, "MovimentoBusiness", "RecalcularPrecoMedioMovimento");
                        }
                        itemAtualizar.SaldoQtde = movimentoItemRecalculado.SaldoQtde;
                        itemAtualizar.SaldoValor = movimentoItemRecalculado.SaldoValor;
                    }

                    //Caso o saldo fique negativo, o sistema para o processo de recalculo
                    ValidaSaldoPositivo(itemAtualizar);

                    // atualiza o saldo na tabela de movimento item
                    this.Service<IMovimentoService>().AtualizarMovimentoItem(itemAtualizar);

                }

                //O movimentoItemRecalculado está com o saldo atualizado
                movimentoItemRecalculado.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(movimentoItemRecalculado.SaldoValor, movimentoItemRecalculado.SaldoQtde, movimentoItemRecalculado.AnoMesReferencia);
                this.Service<IMovimentoService>().RecalcularPrecoMedioSaldo(movimentoItemRecalculado);
                saldoSubItem = null;
            }
        }

        private Decimal gePrecoUnitarioSaldo(MovimentoItemEntity movimento)
        {
            return this.Service<IMovimentoService>().getPrecoUnitarioSaldo(movimento);
        }

        public Decimal? AtualizarDesdobroSaldo(MovimentoItemEntity movimento)
        {
            return this.Service<IMovimentoService>().AtualizarDesdobroSaldo(movimento, false);
        }

        public void ValidaSaldoPositivo(MovimentoItemEntity movimentoItem)
        {
            if (movimentoItem.SaldoQtde < 0)
                throw new Exception(String.Format("Esse lançamento compromete o Saldo qtd do SubItem: {0} {1}  - Verificar o Analítico, Documento: {2} - Data: {3} ", movimentoItem.SubItemMaterial.Codigo, movimentoItem.SubItemDescricao, movimentoItem.Movimento.NumeroDocumento, movimentoItem.Movimento.DataMovimento));

            if (movimentoItem.SaldoValor < 0)
                throw new Exception(String.Format("Esse lançamento compromete o Saldo Valor do SubItem: {0} {1}  - Verificar o Analítico, Documento: {2} - Data: {3} ", movimentoItem.SubItemMaterial.Codigo, movimentoItem.SubItemDescricao, movimentoItem.Movimento.NumeroDocumento, movimentoItem.Movimento.DataMovimento));
        }

        public IList<MovimentoItemEntity> SomarMovimentoItemPorLote(IList<MovimentoItemEntity> lstMovPorSubItem)
        {
            //Agrupado por lote            
            lstMovPorSubItem = (from movItemGroup in lstMovPorSubItem
                                group movItemGroup by new
                                {
                                    subItemId = movItemGroup.SubItemMaterial.Id,
                                    almoxId = movItemGroup.Movimento.Almoxarifado.Id,
                                    UgeId = movItemGroup.UGE.Id,
                                    movItemGroup.FabricanteLote,
                                    movItemGroup.IdentificacaoLote,
                                    movItemGroup.DataVencimentoLote,
                                    AgrupamentoId = movItemGroup.Movimento.TipoMovimento.TipoAgrupamento.Id,
                                    movItemGroup.QtdeMov,
                                    movItemGroup.ValorMov,

                                } into g
                                select new MovimentoItemEntity
                                {
                                    SubItemMaterial = new SubItemMaterialEntity(g.Key.subItemId),
                                    UGE = new UGEEntity(g.Key.UgeId),
                                    Movimento = new MovimentoEntity(0, new AlmoxarifadoEntity(g.Key.almoxId)),
                                    FabricanteLote = g.Key.FabricanteLote,
                                    IdentificacaoLote = g.Key.IdentificacaoLote,
                                    DataVencimentoLote = g.Key.DataVencimentoLote,
                                    QtdeMov = (g.Key.AgrupamentoId == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada) ? g.Sum(a => a.QtdeMov) :
                                    (g.Key.AgrupamentoId == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida) ? -g.Sum(a => a.QtdeMov) : 0,

                                    ValorMov = (g.Key.AgrupamentoId == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada) ? g.Sum(a => a.ValorMov) :
                                    (g.Key.AgrupamentoId == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida) ? -g.Sum(a => a.ValorMov) : 0,
                                }).ToList();


            lstMovPorSubItem = (from movItemGroup in lstMovPorSubItem
                                group movItemGroup by new
                                {
                                    subItemId = movItemGroup.SubItemMaterial.Id,
                                    almoxId = movItemGroup.Movimento.Almoxarifado.Id,
                                    UgeId = movItemGroup.UGE.Id,
                                    movItemGroup.FabricanteLote,
                                    movItemGroup.IdentificacaoLote,
                                    movItemGroup.DataVencimentoLote,

                                } into g
                                select new MovimentoItemEntity
                                {
                                    SubItemMaterial = new SubItemMaterialEntity(g.Key.subItemId),
                                    UGE = new UGEEntity(g.Key.UgeId),
                                    Movimento = new MovimentoEntity(0, new AlmoxarifadoEntity(g.Key.almoxId)),
                                    FabricanteLote = g.Key.FabricanteLote,
                                    IdentificacaoLote = g.Key.IdentificacaoLote,
                                    DataVencimentoLote = g.Key.DataVencimentoLote,
                                    QtdeMov = g.Sum(a => a.QtdeMov),
                                    ValorMov = g.Sum(a => a.ValorMov),
                                }).ToList();
            return lstMovPorSubItem;
        }

        private MovimentoItemEntity SaldoAcumuladoLote(MovimentoItemEntity movItem)
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                var movList = Service<IMovimentoService>().ListMovimentoItemLoteAgrupamento(movItem);

                var valorAcumulado = 0.00m;
                var qtdAcumulado = _valorZero;

                foreach (var item in movList)
                {
                    if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                    {
                        valorAcumulado += item.ValorMov.Value;
                        qtdAcumulado += item.QtdeMov.Value;
                    }
                    else if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                    {
                        valorAcumulado -= item.ValorMov.Value;
                        qtdAcumulado -= item.QtdeMov.Value;
                    }
                }

                //Atualiza o Saldo
                movItem.Movimento = this.Movimento;
                movItem.SaldoQtde = qtdAcumulado;
                movItem.SaldoValor = valorAcumulado;

                return movItem;
            }
        }

        /// <summary>
        /// Executa o processamento de estorno da Saída do Almoxarifado
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, List<string>> EstornarMovimentoSaida(int loginIdEstornante, string InscricaoCE)
        {
            try
            {
                MovimentoRetroativo businessRetroativo = null;
                int natureza = 0;

                this.Service<IMovimentoService>().Entity = this.Movimento;
                this.Service<IMovimentoService>().Entity = this.Service<IMovimentoService>().GetMovimento();
                natureza = this.Service<IMovimentoService>().Entity.MovimentoItem[0].NaturezaDespesaCodigo;

                if (natureza.ToString().Substring(0, 4) == Convert.ToString((int)GeralEnum.NaturezaDespesa.Permanente))
                    this.Service<IMovimentoService>().ExecutarIntegracao((int)this.Movimento.Id, true, true);
                   


                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    //Atualiza os objetos Movimentos com os Itens

                    // this.Service<IMovimentoService>().Entity = this.Service<IMovimentoService>().GetMovimento();

                    if (this.Service<IMovimentoService>().Entity.IdTransferencia != null)
                        throw new Exception("Não é possível estornar a Saída por Transferência sem estornar e entrada");

                    this.movimento = this.Service<IMovimentoService>().Entity;

                    this.VerificaExistenciaNotaSiafemVinculada(true);

                    if (movimento != null)
                    {


                        int cont = 0;
                        foreach (MovimentoItemEntity movItem in this.Movimento.MovimentoItem)
                        {
                            bool erroList = false;
                            movItem.Retroativo = this.Service<IMovimentoService>().VerificarRetroativo(movItem);
                            if (movItem.Retroativo)
                            {



                                businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                businessRetroativo.Estorno = true;
                                List<MovimentoItemEntity> list = businessRetroativo.setListaMovimentoSIAFEM(this.Service<IMovimentoService>());


                                foreach (var item in list)
                                {
                                    if (movItem.Movimento.Id == item.Movimento.Id)
                                    {
                                        if (!string.IsNullOrEmpty(item.NL_Consumo))
                                        {
                                            erroList = true;
                                           this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor: {2} - NLConsumo: {3} - SubItem: {4} - NL: {5}", item.Documento, item.TipoMovimentoDescricao, item.ValorMov, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                            return new Tuple<bool, List<string>>(false, this.ListaErro);
                                        }
                                    }
                                }

                                if (erroList)
                                   this.ListaErro.Add("Lançamento estorno não permitido para movimento que tem LANÇAMENTO DE NL CONSUMO no SIAFEM.");

                                Tuple<IList<MovimentoItemEntity>, bool> retorno = businessRetroativo.RecalculoSIAFEM(this.Service<IMovimentoService>(), cont, movItem, "Estorno");

                                if (!retorno.Item2)
                                {
                                    erroList = false;
                                    foreach (var item in retorno.Item1)
                                    {
                                        if (item.ValorOriginalDocumento != item.ValorMov)
                                        {
                                            decimal? diferenca = (item.ValorMov - item.ValorOriginalDocumento);

                                            erroList = true;

                                            if (!string.IsNullOrEmpty(item.NL_Consumo))
                                               this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - NLConsumo: {5} - SubItem: {6} - NL: {7}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.NL_Consumo, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));
                                            else if ((!string.IsNullOrEmpty(item.NL_Liquidacao)) || (!string.IsNullOrEmpty(item.NL_Reclassificacao)))
                                               this.ListaErro.Add(String.Format("Numero Documento: {0} - Tipo Movimentação: {1} - Valor Original: {2} - Novo Valor: {3} - Diferença: {4} - SubItem: {5} - NL: {6}", item.Documento, item.TipoMovimentoDescricao, item.ValorOriginalDocumento, item.ValorMov, diferenca, item.SubItemMaterial.Codigo, item.NL_Liquidacao == null ? item.NL_Reclassificacao : item.NL_Liquidacao));


                                        }

                                        if (item.PrecoUnit == 0)
                                           this.ListaErro.Add("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");
                                    }

                                    if (erroList)
                                    {
                                       this.ListaErro.Add("Lançamento retroativo não permitido para movimento que altera o valor da NL lançada no SIAFEM.");
                                       this.ListaErro.Add("Movimentações de estorno retroativas que alteram o valor de saídas seguintes que há NL, não são mais permitidas, para executá-la deve se primeiro estornar as saídas mencionadas (Verificar Consulta analítico do subitem).");
                                    }

                                    return new Tuple<bool, List<string>>(false, this.ListaErro);
                                }

                                cont++;
                            }
                        }


                        this.VerificaMovimentoModificaPrecoMedioSaida(this.movimento, true);

                        this.Service<IMovimentoService>().AtualizarMovimentoEstorno(loginIdEstornante, InscricaoCE);//Muda o status do movimento atual para false

                        int indice = 0;
                        foreach (MovimentoItemEntity movItem in movimento.MovimentoItem)
                        {
                            //Atualiza o MovimentoItem.Movimento com o movimento
                            movItem.Movimento = movimento;

                            // Recalcula o preço médio de todos os itens retroativos a data
                            if (this.Service<IMovimentoService>().ExisteSaldo(movItem))
                            {
                                movItem.AgrupamentoId = this.Movimento.TipoMovimento.AgrupamentoId.Value;

                                //caso exista saldo, atualiza
                                this.Service<IMovimentoService>().AtualizarSaldo(movItem, true);
                            }
                            else
                            {
                                //Insere um novo saldo
                                this.Service<IMovimentoService>().InserirSaldo(movItem);
                            }

                            //Existe movimentos para serem recalculados?

                            if (this.Service<IMovimentoService>().VerificarRetroativo(movItem))
                            {
                                var movimentoAnteriorEstorno = this.Service<IMovimentoService>().ConsultaSaldoMovimentoItem(movItem);
                                movimentoAnteriorEstorno = RetornaMovimentoItemSaldo(movItem, movimentoAnteriorEstorno);

                                businessRetroativo = MovimentoRetroativoBusiness.setMovimentoRetroativoBusiness(movItem);
                                businessRetroativo.Estorno = true;
                                businessRetroativo.setListaMovimento(this.Service<IMovimentoService>());
                                businessRetroativo.setListaMovimentoLote(this.Service<IMovimentoService>(), movItem);
                                businessRetroativo.Recalculo(this.Service<IMovimentoService>(), indice);

                                this.Service<IMovimentoService>().GravarMovimentoItemHistorico(movItem); // Grava o movimento atual que foi estornado no historico
                            }

                            indice++;
                        }
                    }

                    ts.Complete();

                    return new Tuple<bool, List<string>>(true, null);
                }

                if (natureza.ToString().Substring(0, 4) == Convert.ToString((int)GeralEnum.NaturezaDespesa.Permanente))
                    this.Service<IMovimentoService>().ExecutarIntegracao((int)this.Movimento.Id, true, false);
                  



                return new Tuple<bool, List<string>>(true, null);
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
                return new Tuple<bool, List<string>>(false, new List<string> { e.Message });
            }
        }

        //READEQUADO PARA NL RECLASSIFICACAO
        private void VerificaExistenciaNotaSiafemVinculada(bool verificaEstornoNotaSIAF = false)
        {
            string msgErroDetalhadaNLSIAF = null;
            string msgErroDetalhadaNLCONSUMO = null;
            string msgErroDetalhada = null;
            string elementoE = null;
            IList<string> NLsLiquidacaoSiafem = null;
            IList<string> NLsLiquidacaoSiafemEstornadas = null;
            IList<string> NLsReclassificacaoSiafem = null;
            IList<string> NLsReclassificacaoSiafemEstornadas = null;
            IList<string> nlsConsumo = null;


            if (ExisteNotaSiafemVinculada(verificaEstornoNotaSIAF))
            {
                NLsLiquidacaoSiafem = this.ObterNLsMovimentacao(this.Movimento.Id.Value, TipoNotaSIAFEM.NL_Liquidacao);
                NLsLiquidacaoSiafemEstornadas = this.ObterNLsMovimentacao(this.Movimento.Id.Value, TipoNotaSIAFEM.NL_Liquidacao, true);
                NLsReclassificacaoSiafem = this.ObterNLsMovimentacao(this.Movimento.Id.Value, TipoNotaSIAFEM.NL_Reclassificacao);
                NLsReclassificacaoSiafemEstornadas = this.ObterNLsMovimentacao(this.Movimento.Id.Value, TipoNotaSIAFEM.NL_Reclassificacao, true);
                nlsConsumo = this.ObterNLsMovimentacao(this.Movimento.Id.Value, TipoNotaSIAFEM.NL_Consumo);



                if (nlsConsumo.HasElements())
                    msgErroDetalhadaNLCONSUMO = String.Format("NL(s) Consumo {0}", String.Join(", ", nlsConsumo));

                if (NLsLiquidacaoSiafemEstornadas.HasElements())
                    msgErroDetalhadaNLSIAF = String.Format("NL(s) (estorno) SIAFEM {0}", String.Join(", ", NLsLiquidacaoSiafemEstornadas));

                if (NLsReclassificacaoSiafemEstornadas.HasElements())
                    msgErroDetalhadaNLSIAF = String.Format("NL(s) Reclassificacao (estorno) SIAFEM {0}", String.Join(", ", NLsReclassificacaoSiafemEstornadas));

                elementoE = ((!String.IsNullOrWhiteSpace(msgErroDetalhadaNLSIAF) && !String.IsNullOrWhiteSpace(msgErroDetalhadaNLCONSUMO)) ? " e " : string.Empty);
                msgErroDetalhada = String.Format("Não é possível estornar movimentação de materiais {0}, detectada(s) NL(s) associada(s): {1}{2}{3}.", this.Movimento.NumeroDocumento, msgErroDetalhadaNLSIAF, elementoE, msgErroDetalhadaNLCONSUMO);
                throw new Exception(msgErroDetalhada);
            }
        }

        public bool GravarNovoEmpenhoEvento(EmpenhoEventoEntity pObjEmpenhoEvento)
        {
            bool lBlnGravadoComSucesso = false;

            try
            {
                this.Service<IEmpenhoEventoService>().Entity = pObjEmpenhoEvento;

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    if (!String.IsNullOrEmpty(pObjEmpenhoEvento.Descricao) && (pObjEmpenhoEvento.Codigo != 0))
                    {
                        this.Service<IEmpenhoEventoService>().Salvar();
                        lBlnGravadoComSucesso = true;
                    }
                }
            }
            catch (Exception lExcExcecao)
            {
                new LogErro().GravarLogErro(lExcExcecao);
               this.ListaErro.Add(lExcExcecao.Message);
                return lBlnGravadoComSucesso;
            }

            return lBlnGravadoComSucesso;
        }

        #endregion

        #region Validadores

        public void ValidarDataDocumento()
        {
            if (this.Movimento.DataDocumento == null)
            {
               this.ListaErro.Add("Informar a data de emissão/documento.");
                return;
            }

            //Data de Emissão não ser maior que a data de recebimento
            if (this.Movimento.DataDocumento > this.Movimento.DataMovimento)
            {
               this.ListaErro.Add("A data de recebimento deve ser posterior a data de emissão.");
                return;
            }
        }

        private void ValidarDataMovimentoMesRef()
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (this.Movimento.DataMovimento != null)
                {
                    string formatAnoMesRef = "01/" + this.Movimento.AnoMesReferencia.Substring(4, 2) + "/" + this.Movimento.AnoMesReferencia.Substring(0, 4);

                    if ((this.Movimento.DataMovimento.Value.Year == Convert.ToDateTime(formatAnoMesRef).Year))
                    {
                        if ((this.Movimento.DataMovimento.Value.Month != Convert.ToDateTime(formatAnoMesRef).Month)) //Mês atual ou anterior
                        {
                           this.ListaErro.Add("Data do movimento ou Data de recebimento diferente do mês/ano de referência aberto.");
                        }
                    }
                    else
                    {
                       this.ListaErro.Add("Data do movimento ou Data de recebimento diferente do mês/ano de referência aberto.");
                    }
                }
            }
        }

        private void ValidarDataEmissaoMesRef()
        {
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (this.Movimento.DataDocumento != null && this.Movimento.TipoMovimento.Id !=(int) tipoMovimento.ConsumoImediatoEmpenho)
                {
                    string formatAnoMesRef = "01/" + this.Movimento.AnoMesReferencia.Substring(4, 2) + "/" + this.Movimento.AnoMesReferencia.Substring(0, 4);

                    if ((this.Movimento.DataDocumento.Value.Year == Convert.ToDateTime(formatAnoMesRef).Year))
                    {
                        if ((this.Movimento.DataDocumento.Value.Month != Convert.ToDateTime(formatAnoMesRef).Month)) //Mês atual ou anterior
                        {
                           this.ListaErro.Add("Data de emissão está diferente do mês/ano de referência aberto.");
                        }
                    }
                    else
                    {
                       this.ListaErro.Add("Data de emissão está diferente do mês/ano de referência aberto.");
                    }
                }
                ///Validar Data de recebimento(Data de movimento) do consumo imediato.
                if (this.Movimento.DataMovimento != null && this.Movimento.TipoMovimento.Id == (int)tipoMovimento.ConsumoImediatoEmpenho)
                {
                    string formatAnoMesRef = "01/" + this.Movimento.AnoMesReferencia.Substring(4, 2) + "/" + this.Movimento.AnoMesReferencia.Substring(0, 4);

                    if ((this.Movimento.DataMovimento.Value.Year == Convert.ToDateTime(formatAnoMesRef).Year))
                    {
                        if ((this.Movimento.DataMovimento.Value.Month != Convert.ToDateTime(formatAnoMesRef).Month)) //Mês atual ou anterior
                        {
                            this.ListaErro.Add("Data de recebimento está diferente do mês/ano de referência aberto.");
                        }
                    }
                    else
                    {
                        this.ListaErro.Add("Data de recebimento está diferente do mês/ano de referência aberto.");
                    }
                }
            }
        }

        public void ValidarPTRes()
        {
            /* Não remover o código
             * Por ora o campo não está sendo apresentado. A ser definido nova regra pela Leila e Marcia */
            string _avisoObrigatoriedadePTRes = "Antes de efetuar esta saída solicite ao requisitante que informe na requisição o numero de PTRes, para cada subitem da mesma.";
            Movimento.MovimentoItem.ToList().ForEach(_movItem =>
            {
                //_avisoObrigatoriedadePTRes = String.Format(_avisoObrigatoriedadePTRes, _movItem.SubItemCodigo, _movItem.SubItemDescricao);
                if ((_movItem.PTRes.IsNull() || (!_movItem.PTRes.Id.HasValue && !_movItem.PTRes.Codigo.HasValue)) && !this.ListaErro.Contains(_avisoObrigatoriedadePTRes))
                   this.ListaErro.Add(_avisoObrigatoriedadePTRes);
            });
        }

        public void ValidarPTResEmpenho()
        {
            /* Não remover o código
             * Por ora o campo não está sendo apresentado. A ser definido nova regra pela Leila e Marcia */

            //if (this.Movimento.PTRes.Codigo == null)
            //{
            //   this.ListaErro.Add("PTRes vinculada ao empenho não cadastrada no sistema. Acionar administrador do sistema.");
            //}
        }

        public void ValidarCampoObservacoes()
        {
            if (String.IsNullOrWhiteSpace(this.Movimento.Observacoes))
               this.ListaErro.Add("Campo (Observações) de preenchimento obrigatório.");
        }

        public void ValidarCampoInstrucoes()
        {
            if (String.IsNullOrWhiteSpace(this.Movimento.Instrucoes))
            {
               this.ListaErro.Add("Campo (Instruções) de preenchimento obrigatório.");
            }
        }

        public void ValidarDataMovimento()
        {
            ValidarDataMovimentoMesRef();

            if (this.Movimento.DataMovimento == null)
            {
               this.ListaErro.Add("Informar a data do Movimento ou data de Recebimento.");
            }
            else if (this.Movimento.DataMovimento > DateTime.Now)
            {
               this.ListaErro.Add("A data do movimento ou data de recebimento não pode ser maior que a data atual.");
            }
            else
            {
                if (this.Movimento.DataMovimento.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    this.Movimento.DataMovimento = DateTime.Now;
                }
            }
        }

        public void ValidarDataRecebimento()
        {
            
            if (this.Movimento.DataMovimento == null)
            {
               this.ListaErro.Add("Informar a data de Recebimento.");
            }
            else if (this.Movimento.DataMovimento > DateTime.Now)
            {
               this.ListaErro.Add("A data de recebimento não pode ser maior que a data atual.");
            }
            else
            {
                if (this.Movimento.DataMovimento.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    this.Movimento.DataMovimento = DateTime.Now;
                }
            }
        }

        public void ValidarDataEmissao()
        {
            ValidarDataEmissaoMesRef();

            if (this.Movimento.DataDocumento == null)
            {
               this.ListaErro.Add("Informar a data da Emissão ou data de Recebimento.");
            }
            else if (this.Movimento.DataDocumento > DateTime.Now)
            {
               this.ListaErro.Add("A data da emissão não pode ser maior que a data atual.");
            }
            else
            {
                if (this.Movimento.DataMovimento.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    this.Movimento.DataMovimento = DateTime.Now;
                }
            }
        }

        

        public void ValidarDataMovimento(bool desconsiderarValidacaoMesReferencia)
        {
            if (desconsiderarValidacaoMesReferencia)
            {
                ValidarDataMovimento();
                return;
            }

            if (this.Movimento.DataMovimento == null)
            {
               this.ListaErro.Add("Informar a data do Movimento ou data de Recebimento.");
            }
            else if (this.Movimento.DataMovimento > DateTime.Now)
            {
               this.ListaErro.Add("A data do movimento ou data de recebimento não pode ser maior que a data atual.");
            }
            else
            {
                if (this.Movimento.DataMovimento.Value.ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    this.Movimento.DataMovimento = DateTime.Now;
                }
            }
        }

        public void ValidarNumeroDocumento()
        {
            if (this.Movimento.NumeroDocumento == null || this.Movimento.NumeroDocumento.Equals(""))
            {
               this.ListaErro.Add("Informar o número do documento.");
            }
        }

        public void ValidarEmpenho()
        {
            if (this.Movimento.Empenho == null || this.Movimento.Empenho.Equals(""))
            {
               this.ListaErro.Add("Informar o empenho.");
            }
        }

        public void ValidarGeradorDescricao(string nomeMsg)
        {
            if (this.Movimento.GeradorDescricao == null || this.Movimento.GeradorDescricao.Equals(""))
            {
               this.ListaErro.Add("Informar " + nomeMsg + ".");
            }
        }

        public void ValidarDivisao()
        {
            if (this.Movimento.Divisao == null || this.Movimento.Divisao.Id == 0)
            {
               this.ListaErro.Add("Informar a divisão.");
            }
        }

        public void ValidarAlmoxOrigemDestino()
        {
            if (this.Movimento.MovimAlmoxOrigemDestino == null || this.Movimento.MovimAlmoxOrigemDestino.Id == 0)
            {
               this.ListaErro.Add("Informar o almoxarifado para transferência.");
            }
        }

        public void ValidarUGE()
        {
            if (this.Movimento.UGE == null || this.Movimento.UGE.Id == 0)
            {
               this.ListaErro.Add("Informar UGE.");
            }
        }

        public void ValidarFornecedor()
        {
            if (this.Movimento.Fornecedor == null || this.Movimento.Fornecedor.Id == 0)
            {
               this.ListaErro.Add("Informar o Fornecedor.");
            }
        }

        public void ValidarValorDocumento()
        {
            if (this.Movimento.ValorDocumento <= 0)
            {
               this.ListaErro.Add("Informar o Valor Total do documento.");
            }
        }
        #endregion

        #region Consistencias

        public bool ConsistirMovimento()
        {
            #region Consistir MovimentoItem
            if (Movimento.MovimentoItem == null || Movimento.MovimentoItem.Count == 0)
            {
                movimento.Bloquear = movimento.Bloquear == null ? false : movimento.Bloquear;
                if ((bool)movimento.Bloquear)
                    throw new Exception("Requisição esta sendo usada!");

                if (!this.ListaErro.Contains("Os itens da movimentação estão vazios."))
                   this.ListaErro.Add("Os itens da movimentação estão vazios.");
                return false;

               
            }
            #endregion

            #region Consistir Quantidade Subitens (Requisicao e Saida Por Transferencia)
            bool ehSaidaPorTransferencia = (Movimento.TipoMovimento.Id == (int)tipoMovimento.SaidaPorTransferencia);
            bool excedeNumeroMaximoSubitens = (Movimento.MovimentoItem.HasElements() && Movimento.MovimentoItem.DistinctBy(movItem => movItem.SubItemCodigo.Value).Count() >= Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO);
            if (ehSaidaPorTransferencia && excedeNumeroMaximoSubitens)
            {
               this.ListaErro.Add(String.Format("Número máximo de subitens por movimentação ({0}), excedido!", Constante.CST_NUMERO_MAXIMO_SUBITENS_POR_MOVIMENTACAO));
                return false;
            }
            #endregion

            #region Consistir DataMovimento
            if (movimento.DataMovimento == null)
                throw new Exception("A Data de movimento é de preenchimento obrigatório.");
            #endregion

            #region Consistencias Especificas Tipo Movimentacao
            switch (this.Movimento.TipoMovimento.Id)
            {
                case (int)GeralEnum.TipoMovimento.RequisicaoPendente:
                    ConsistirRequisicaoPendente();
                    break;
                //case (int)GeralEnum.TipoMovimento.AquisicaoCompraEmpenho:
                case (int)GeralEnum.TipoMovimento.EntradaPorEmpenho:
                    ConsistirEntradaPorEmpenho();
                    break;
                case (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho:
                    ConsistirConsumoPorEmpenho();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                    ConsistirMovimentacaoRestosAPagarEmpenhoConsumoImediato();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaAvulsa:
                case (int)GeralEnum.TipoMovimento.EntradaInventario:
                case (int)GeralEnum.TipoMovimento.EntradaCovid19:
                    ConsistirMovimentacaoAvulsa();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar:
                    ConsistirMovimentacaoRestosAPagar();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado:
                    ConsistirMovimentacaoTransfAlmxSemImp();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorTransferencia:
                    ConsistirMovimentacaoTransfer();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado:
                case (int)GeralEnum.TipoMovimento.EntradaPorDoacao:
                    ConsistirMovimentacaoDoacao();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorDevolucao:
                    ConsistirMovimentacaoDevolucao();
                    break;
                case (int)GeralEnum.TipoMovimento.EntradaPorMaterialTransformado:
                    ConsistirMovimentacaoMatTransformado();
                    break;
                case (int)GeralEnum.TipoMovimento.SaidaPorDoacao:
                //ConsistirMovimentacaoSaidaPorDoação();
                //break;
                case (int)GeralEnum.TipoMovimento.SaidaPorTransferencia:
                    ConsistirMovimentacaoSaidaPorTransferencia();
                    break;
                case (int)GeralEnum.TipoMovimento.RequisicaoAprovada:
                    ConsistirMovimentacaoRequisicaoAprovada();
                    break;
                case (int)GeralEnum.TipoMovimento.OutrasSaidas:
                case (int)GeralEnum.TipoMovimento.SaidaPorMaterialTransformado:
                case (int)GeralEnum.TipoMovimento.SaidaPorExtravioFurtoRoubo:
                case (int)GeralEnum.TipoMovimento.SaidaPorIncorporacaoIndevida:
                case (int)GeralEnum.TipoMovimento.SaidaPorPerda:
                case (int)GeralEnum.TipoMovimento.SaidaInservivelQuebra:
                case (int)GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado:
                case (int)GeralEnum.TipoMovimento.SaidaParaAmostraExposicaoAnalise:
                    ConsistirMovimentacaoOutrasSaidas();
                    break;
            }
            #endregion

            #region Consistencias SIAFEM
            if (String.IsNullOrWhiteSpace(Movimento.InscricaoCE) && ((this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorTransferencia) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorEmpenho) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagar) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.RequisicaoPendente) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.RequisicaoAprovada) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaPorTransferenciaDeAlmoxNaoImplantado) &&
                                                                     (this.Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.EntradaInventario)))
               this.ListaErro.Add("Campo CE é de preenchimento obrigatório!");
            #endregion

            if (this.ListaErro.Count > 0)
                return false;

            return true;
        }

        public bool ConsistirMovimentoItem()
        {
            foreach (var item in Movimento.MovimentoItem)
            {
                if ((item.QtdeMov == null && Movimento.TipoMovimento.Id != (int)GeralEnum.TipoMovimento.RequisicaoPendente) || (item.QtdeLiq == null && Movimento.TipoMovimento.Id == (int)GeralEnum.TipoMovimento.RequisicaoPendente))
                {
                   this.ListaErro.Add("Favor informar a quantidade fornecida.");
                    return false;
                }


                if (item.SubItemMaterial.Id == null)
                {
                   this.ListaErro.Add("Favor informar o SubItem.");
                    return false;
                }

                if (item.SaldoQtde < item.QtdeMov)
                {
                   this.ListaErro.Add(String.Format("Saldo Insuficiente para a saída. Codigo: {0} - Descrição: {1}", item.SubItemMaterial.CodigoFormatado, item.SubItemDescricao));
                    return false;
                }
            }
            return true;
        }

        public void ConsistirMovimentacaoDevolucao()
        {
            ValidarUGE();
            ValidarDivisao();
            ValidarDataDocumento();
            ValidarDataMovimento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
        }

        public void ConsistirMovimentacaoTransfer()
        {
            ValidarUGE();
            ValidarAlmoxOrigemDestino();
            ValidarDataDocumento();
            ValidarDataMovimento(true);
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
            VerificaValorDocumentoTransferencia();
        }

        private void VerificaValorDocumentoTransferencia()
        {
            try
            {
                VerificaValorDocumentoTransferenciaEx();
            }
            catch (InvalidOperationException e)
            {
                ListaErro.Add(String.Format(e.Message));
            }
        }

        private void VerificaValorDocumentoTransferenciaEx()
        {
            if (this.movimento != null)
            {

                //Retorna o movimento atual do banco de dados para garantir a consistencia.
                decimal? ValorDocumento = this.Service<IMovimentoService>().GetValorDocumento((int)this.movimento.Id);

                decimal? valorMovimentoSomado = 0;

                foreach (MovimentoItemEntity item in this.movimento.MovimentoItem)
                {
                    valorMovimentoSomado += item.ValorMov;
                }

                if (ValorDocumento.Value.truncarDuasCasas() != valorMovimentoSomado.Value.truncarDuasCasas())
                {
                    throw new InvalidOperationException("Entrada Inválida - Houve alteração no valor da Nota - Acionar Almoxarifado de Origem para reemissão da nota.");
                }
            }
        }

        public bool RetornaDocumentoTransferenciaAlterado()
        {
            bool movimentoAlterado = false;
            if (this.movimento != null)
            {
                //Inicializa a propriedade com valor 0
                this.movimento.ValorSomaItens = 0;

                //Retorna o movimento atual do banco de dados para garantir a consistencia.
                this.movimento.ValorOriginalDocumento = this.Service<IMovimentoService>().GetValorDocumento((int)this.movimento.Id);


                foreach (MovimentoItemEntity item in this.movimento.MovimentoItem)
                    this.movimento.ValorSomaItens += item.ValorMov;

                movimentoAlterado = (this.movimento.ValorOriginalDocumento != this.movimento.ValorSomaItens);
            }

            return movimentoAlterado;
        }

        public decimal? RetornaValorOriginalDocumentoTransferencia()
        {
            decimal? ValorDocumento = null;
            if (this.movimento != null)
            {
                //Retorna o movimento atual do banco de dados para garantir a consistencia.
                ValorDocumento = this.Service<IMovimentoService>().GetValorDocumento((int)this.movimento.Id);
            }
            return Convert.ToDecimal(ValorDocumento);
        }

        public void ValidarSubitensAtivos()
        {
            if (this.Movimento.MovimentoItem.IsNotNull() && this.Movimento.MovimentoItem.Count > 0)
            {
                //if (this.Movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia)
                if ((this.Movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorTransferencia) && (this.Movimento.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.EntradaPorDoacaoImplantado))
                {
                    CatalogoBusiness objBusiness = new CatalogoBusiness();
                    SubItemMaterialEntity subItem = null;

                    //Verificação para almoxarifado atual (eliminação divergências entre relatórios)
                    foreach (MovimentoItemEntity itemMovimentacao in this.Movimento.MovimentoItem)
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                        {
                            try
                            {
                                //subItem = objBusiness.SelectSubItemMaterialRetorno((int)itemMovimentacao.SubItemMaterial.Id);
                                objBusiness.SelectSubItemMaterialAlmox((int)itemMovimentacao.SubItemMaterial.Id, (int)this.Movimento.Almoxarifado.Id);
                                subItem = objBusiness.SubItemMaterial;

                                if (subItem.IsNotNull() && subItem.IndicadorAtividadeAlmox == false)
                                    ListaErro.Add(String.Format("Subitem {0} - {1} não está ativo no catálogo do almoxarifado.", subItem.Codigo, subItem.Descricao));
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(ex.Message, ex.InnerException);
                            }
                            finally
                            {
                                ts.Complete();
                            }
                        }
                    }
                }
                else
                {
                    AtivaSubitensMovimentoPorTransferencia();
                }

            }
        }

        public void ValidarSubitensMovimentacaoConsumoImediato()
        {
            if (this.Movimento.MovimentoItem.HasElements())
            {
                var listaMovimentacoesConsumoImediato = new int[] { tipoMovimento.ConsumoImediatoEmpenho.GetHashCode() };
                //string fmtValorFinanceiro = "#,##0.00";
                //decimal maximoValorItemEmpenho = -1.00m;
                //decimal valorUnitarioItemEmpenho = -1.0000m; ;
                //decimal valorItemEmpenhoDigitado = -1.00m;
                //decimal saldoValorRestante = -1.00m;

                if (listaMovimentacoesConsumoImediato.Contains(Movimento.TipoMovimento.Id))
                {
                    foreach (MovimentoItemEntity itemMovimentacao in this.Movimento.MovimentoItem)
                    {
                        CarregarFormatos(Movimento.AnoMesReferencia);

                        if (itemMovimentacao.IsNotNull())
                        {
                            if (!itemMovimentacao.ValorMov.HasValue)
                                ListaErro.Add(String.Format("Obrigatório digitar valor financeiro, para item de empenho #{0:D2}!", itemMovimentacao.Posicao));

                            //maximoValorItemEmpenho = itemMovimentacao.SaldoValor ?? (itemMovimentacao.SaldoLiqSiafisico ?? 0.00m);
                            //valorUnitarioItemEmpenho = itemMovimentacao.ValorUnit ?? 0.00m; 
                            //valorItemEmpenhoDigitado = itemMovimentacao.ValorMov ?? 0.00m;
                            //saldoValorRestante = (maximoValorItemEmpenho - valorItemEmpenhoDigitado);

                            //if ((valorItemEmpenhoDigitado != 0.00m) && (valorItemEmpenhoDigitado < valorUnitarioItemEmpenho))
                            //    ListaErro.Add(String.Format("Valor a liquidar para subitem #{0:D2} tem de ser maior ou igual a R$ {1}!", itemMovimentacao.Posicao, valorUnitarioItemEmpenho.ToString(fmtValorFinanceiro)));
                            //else if ((valorItemEmpenhoDigitado != 0.00m) && (valorItemEmpenhoDigitado > maximoValorItemEmpenho))
                            //    ListaErro.Add(String.Format("Valor digitado é superior ao saldo restante a liquidar para subitem #{0}! (Saldo restante: R$ {1})", itemMovimentacao.Posicao, maximoValorItemEmpenho.ToString(fmtValorFinanceiro)));
                            //else if ((valorItemEmpenhoDigitado != 0.00m) && (saldoValorRestante < valorUnitarioItemEmpenho))
                            //    ListaErro.Add(String.Format("Não é possível realizar movimentação com o valor digitado, pois o saldo restante a liquidar, será inferior ao menor valor a liquidar possível para subitem {0:D2}! (Menor valor restante possível: R$ {1}; Valor restante se permitido: R$ {2})", itemMovimentacao.Posicao, valorUnitarioItemEmpenho.ToString(fmtFracionarioMaterialValorUnitario), (maximoValorItemEmpenho - valorItemEmpenhoDigitado).ToString(fmtValorFinanceiro)));

                        }
                    }
                }
            }
        }

        public void AtivaSubitensMovimentoPorTransferencia()
        {
            CatalogoBusiness objBusiness = new CatalogoBusiness();
            SubItemMaterialEntity subItem = null;

            //Verificação para almoxarifado atual (eliminação divergências entre relatórios)
            foreach (MovimentoItemEntity itemMovimentacao in this.Movimento.MovimentoItem)
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    //subItem = objBusiness.SelectSubItemMaterialRetorno((int)itemMovimentacao.SubItemMaterial.Id);
                    objBusiness.SelectSubItemMaterialAlmox((int)itemMovimentacao.SubItemMaterial.Id, (int)this.Movimento.Almoxarifado.Id);
                    subItem = objBusiness.SubItemMaterial;

                    if (subItem.IsNotNull() && subItem.IndicadorAtividadeAlmox == false)
                    {
                        subItem.IndicadorAtividadeAlmox = true;
                        subItem.IndicadorDisponivelId = 2;
                        subItem.AlmoxarifadoId = (int)this.Movimento.Almoxarifado.Id;
                        subItem.EstoqueMaximo = 0;
                        subItem.EstoqueMinimo = 0;

                        if (objBusiness.SalvarSubItemAlmox())
                        {
                            ListaErro.Add(String.Format("Subitem {0} - {1} foi ativado automaticamente no catálogo do almoxarifado.", subItem.Codigo, subItem.Descricao));
                        }
                    }

                    ts.Complete();
                }
            }
        }



        public void ConsistirMovimentacaoDoacao()
        {
            ValidarUGE();
            ValidarGeradorDescricao("doador");
            ValidarDataDocumento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataMovimento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
        }

        public void ConsistirMovimentacaoMatTransformado()
        {
            ValidarUGE();
            ValidarGeradorDescricao("Origem");
            ValidarDataDocumento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataMovimento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
        }

        public void ConsistirEntradaPorEmpenho()
        {
            ValidarUGE();
            ValidarPTResEmpenho();
            ValidarEmpenho();
            ValidarFornecedor();
            ValidarDataDocumento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataMovimento();
            ValidarSaldoEntrada();
        }
        public void ConsistirConsumoPorEmpenho()
        {
            ValidarUGE();
            ValidarPTResEmpenho();
            ValidarEmpenho();
            ValidarFornecedor();
            ValidarDataEmissao();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataRecebimento();
            ValidarSubitensMovimentacaoConsumoImediato();
        }
        private void ConsistirMovimentacaoRestosAPagarEmpenhoConsumoImediato()
        {
            //ConsistirConsumoPorEmpenho//
            ValidarUGE();
            ValidarPTResEmpenho();
            ValidarEmpenho();
            ValidarFornecedor();
            ValidarDataEmissao();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataRecebimento();
            //ValidarSaldoEntrada();

            //ConsistirMovimentacaoRestosAPagar//
            //ValidarUGE(); /* JA CONSISTIDO POR RESTOS A PAGAR */
            //ValidarFornecedor(); /* JA CONSISTIDO POR RESTOS A PAGAR */
            //ValidarDataDocumento();
            //ValidarNumeroDocumento(); /* JA CONSISTIDO POR RESTOS A PAGAR */
            //ValidarValorDocumento(); /* JA CONSISTIDO POR RESTOS A PAGAR */


            //ValidarDataMovimento(); //TODO CONFERIR ESTE METODO

            
            //ValidarSaldoEntrada(); /* JA CONSISTIDO POR RESTOS A PAGAR */
            ValidarSubitensAtivos();
            //ValidarEmpenho(); /* JA CONSISTIDO POR RESTOS A PAGAR */
        }

        public List<int> GerarListaDeEmpenhoEventoIDs()
        {
            return Enum.GetValues(typeof(GeralEnum.EmpenhoEvento))
                       .Cast<GeralEnum.EmpenhoEvento>()
                       .Where(enumEventoEmpenho => GeralEnum.GetEnumDescription(enumEventoEmpenho).Contains("BEC"))
                       .Cast<int>()
                       .ToList();
        }

        public void ConsistirMovimentacaoAvulsa()
        {
            ValidarUGE();
            ValidarFornecedor();
            ValidarDataDocumento();
            ValidarDataMovimento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
        }

        public void ConsistirMovimentacaoRestosAPagar()
        {
            ValidarUGE();
            ValidarFornecedor();
            ValidarDataDocumento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarDataMovimento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
            ValidarEmpenho();
        }


        public void ConsistirMovimentacaoTransfAlmxSemImp()
        {
            ValidarUGE();
            ValidarDataDocumento();
            ValidarDataMovimento();
            ValidarNumeroDocumento();
            ValidarValorDocumento();
            ValidarSaldoEntrada();
            ValidarSubitensAtivos();
        }


        public void ConsistirMovimentacaoSaidaPorTransferencia()
        {
            ValidarUGE();
            ConsistirMovimentoItem();
            //ValidarNumeroDocumento();
            ValidarDataMovimento();

            this._isTransferenciaValida(this.Movimento);
        }

        public void ConsistirMovimentacaoRequisicaoAprovada()
        {
            ValidarUGE();
            ConsistirMovimentoItem();
            ValidarNumeroDocumento();
            ValidarDataMovimento(true);
            ValidarPTRes();
            //ValidarSaldoEntrada();
        }

        public void ConsistirRequisicaoPendente()
        {
            ConsistirMovimentoItem();
            ValidarPTRes();
        }

        public void ConsistirMovimentacaoOutrasSaidas()
        {
            ValidarUGE();
            ConsistirMovimentoItem();
            //ValidarNumeroDocumento();
            ValidarDataMovimento();
            ValidarCampoObservacoes();
        }

        //Esse método está fazendo a soma do Saldo.
        public void ValidarSaldoEntrada()
        {
            SaldoSubItemBusiness saldoSubItem = new SaldoSubItemBusiness();
            saldoSubItem.ConsistirSaldoSubItemEntrada(Movimento);

            //this.ListaErro = saldoSubItem.ListaErro;
           this.ListaErro.AddRange(saldoSubItem.ListaErro);
        }

        public bool ConsistirSubItem(MovimentoItemEntity movimentoItem)
        {
            bool valido = true;

            if (movimentoItem.QtdeLiq == null)
            {
               this.ListaErro.Add("Informe a quantidade do Subitem.");
                valido = false;
            }
            if (movimentoItem.QtdeLiq == 0)
            {
               this.ListaErro.Add("A quantidade do Subitem tem que ser maior que zero.");
                valido = false;
            }

            if (movimentoItem.SubItemMaterial.Codigo == 0)
            {
               this.ListaErro.Add("Informe um SubItem.");
                valido = false;
            }

            //if (movimentoItem.PTRes.Id == 0 || !movimentoItem.PTRes.Id.HasValue)
            if (movimentoItem.PTRes.IsNull() || (!movimentoItem.PTRes.Codigo.HasValue || movimentoItem.PTRes.Codigo.Value == 0))
            {
               this.ListaErro.Add("Informe dados referentes a PTRes para Subitem de Material.");
                valido = false;
            }
            return valido;
        }

        public bool VerificaStatusFechadoMesReferenciaSIAFEM(bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            bool mesFiscalFechadoSIAFEM = true;
            DateTime dtFechamentoSIAFEM = new DateTime();
            CalendarioFechamentoMensalEntity objDataFechamentoMensal = null;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = null;

            try
            {
                //if (!String.IsNullOrWhiteSpace(this.View.AnoMesReferencia) && this.estrutura.IsNotNull())
                if (this.movimento.IsNotNull() && !String.IsNullOrWhiteSpace(this.movimento.AnoMesReferencia))
                {
                    var anoReferencia = Int32.Parse(this.movimento.AnoMesReferencia.Substring(0, 4));
                    var mesReferencia = Int32.Parse(this.movimento.AnoMesReferencia.Substring(4, 2));

                    objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
                    mesFiscalFechadoSIAFEM = objBusinessCalendarioSIAFEM.StatusFechadoMesReferenciaSIAFEM(mesReferencia, anoReferencia, exibirMensagemErro);
                    objDataFechamentoMensal = objBusinessCalendarioSIAFEM.ObterDataFechamentoMensalSIAFEM(mesReferencia, anoReferencia);
                    dtFechamentoSIAFEM = objDataFechamentoMensal.DataFechamentoDespesa;

                    if (!mesFiscalFechadoSIAFEM)
                    {
                        dtFechamentoSIAFEM = dtFechamentoSIAFEM.AddHours(19);
                        blnRetorno = (DateTime.Now >= dtFechamentoSIAFEM);
                    }
                    else if ((!exibirMensagemErro && mesFiscalFechadoSIAFEM) || (this.ListaErro.IsNotNullAndNotEmpty()) || (objBusinessCalendarioSIAFEM.ListaErro.IsNotNullAndNotEmpty()))
                    {
                        //this.View.ListaErros = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy")) };
                        var listaMsgsErro = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4})* fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString(base.fmtDataFormatoBrasileiro)) };

                        if (this.ListaErro.IsNotNullAndNotEmpty() && !objBusinessCalendarioSIAFEM.ListaErro.HasElements())
                           this.ListaErro.AddRange(listaMsgsErro);
                        else
                           this.ListaErro.AddRange(objBusinessCalendarioSIAFEM.ListaErro);
                    }
                }
                else
                {
                    var msgErro = "Não foi possível determinar mês/ano de referência para movimentação atual";

                    if (this.ListaErro.IsNotNullAndNotEmpty())
                       this.ListaErro.Add(msgErro);
                    else
                        this.ListaErro = new List<string>() { msgErro };
                }
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
               this.ListaErro.AddRange(objBusinessCalendarioSIAFEM.ListaErro);
                return true;
            }


            return blnRetorno;
        }

        public bool _VerificaStatusFechadoMesReferenciaSIAFEM(bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            bool mesFiscalFechadoSIAFEM = true;
            DateTime dtFechamentoSIAFEM = new DateTime();
            CalendarioFechamentoMensalEntity objDataFechamentoMensal = null;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = null;

            try
            {
                //if (!String.IsNullOrWhiteSpace(this.View.AnoMesReferencia) && this.estrutura.IsNotNull())
                if (this.movimento.IsNotNull() && !String.IsNullOrWhiteSpace(this.movimento.AnoMesReferencia))
                {
                    var anoReferencia = Int32.Parse(this.movimento.AnoMesReferencia.Substring(0, 4));
                    var mesReferencia = Int32.Parse(this.movimento.AnoMesReferencia.Substring(4, 2));

                    objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
                    mesFiscalFechadoSIAFEM = objBusinessCalendarioSIAFEM.StatusFechadoMesReferenciaSIAFEM(mesReferencia, anoReferencia, exibirMensagemErro);
                    objDataFechamentoMensal = objBusinessCalendarioSIAFEM.ObterDataFechamentoMensalSIAFEM(mesReferencia, anoReferencia);
                    dtFechamentoSIAFEM = objDataFechamentoMensal.DataFechamentoDespesa;

                    if (!mesFiscalFechadoSIAFEM)
                    {
                        dtFechamentoSIAFEM = dtFechamentoSIAFEM.AddHours(19);
                        blnRetorno = (DateTime.Now >= dtFechamentoSIAFEM);
                    }
                    else if ((!exibirMensagemErro && mesFiscalFechadoSIAFEM) || (this.ListaErro.IsNotNullAndNotEmpty()) || (objBusinessCalendarioSIAFEM.ListaErro.IsNotNullAndNotEmpty()))
                    {
                        //this.View.ListaErros = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy")) };
                        var listaMsgsErro = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4})* fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy")) };

                        if (this.ListaErro.IsNotNullAndNotEmpty() && !objBusinessCalendarioSIAFEM.ListaErro.HasElements())
                           this.ListaErro.AddRange(listaMsgsErro);
                        else
                           this.ListaErro.AddRange(objBusinessCalendarioSIAFEM.ListaErro);
                    }
                }
                else
                {
                    var msgErro = "Não foi possível determinar mês/ano de referência para movimentação atual";

                    if (this.ListaErro.IsNotNullAndNotEmpty())
                       this.ListaErro.Add(msgErro);
                    else
                        this.ListaErro = new List<string>() { msgErro };
                }
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
               this.ListaErro.AddRange(objBusinessCalendarioSIAFEM.ListaErro);
                return true;
            }


            return blnRetorno;
        }
        #endregion

        #region Listar

        public IList<MovimentoEntity> ListarTodosCodPorUgeEmpenho(int _UgeId, string _numeroEmpenho, int _almoxId)
        {
            MovimentoEntity mov = new MovimentoEntity();
            mov.UGE = new UGEEntity(_UgeId);
            mov.Empenho = _numeroEmpenho;
            mov.Almoxarifado = new AlmoxarifadoEntity(_almoxId);
            mov.Ativo = true;
            IList<MovimentoEntity> retorno = null;
            if (_UgeId != 0)
                retorno = this.Service<IMovimentoService>().ListarTodosCodSimplif(mov);
            return retorno;
        }

        public IList<MovimentoEntity> ListarTodosCodPorUgeTipoMovimentoSimplif(int _UgeId, int _tipoMovimento, int _almoxId)
        {
            MovimentoEntity mov = new MovimentoEntity();
            mov.UGE = new UGEEntity(_UgeId);
            mov.TipoMovimento = new TipoMovimentoEntity(_tipoMovimento);
            mov.Ativo = true;
            mov.Almoxarifado = new AlmoxarifadoEntity(_almoxId);
            IList<MovimentoEntity> retorno = null;
            if (_UgeId != 0)
                retorno = this.Service<IMovimentoService>().ListarTodosCodSimplif(mov);
            return retorno;
        }

        public IList<MovimentoEntity> ListarTodosCodPorUgeAlmoxTipoMovimento(int _AlmoxId, int _UgeId, int _tipoMovimento)
        {
            MovimentoEntity mov = new MovimentoEntity();
            mov.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity(_AlmoxId);
            mov.UGE = new UGEEntity(_UgeId);
            mov.TipoMovimento = new TipoMovimentoEntity(_tipoMovimento);
            mov.Ativo = true;
            IList<MovimentoEntity> retorno = null;
            if (_UgeId != 0)
                retorno = this.Service<IMovimentoService>().ListarTodosCodSimplif(mov);
            return retorno;
        }

        public IList<MovimentoEntity> ListarMovimento(int _fornecedorId, string _documento)
        {
            this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
            IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().Listar(_documento);
            this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
            return retorno;
        }

        public MovimentoEntity ListarSolicitacaoMaterial(int UA, int divisao)
        {
            this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
            MovimentoEntity retorno = this.Service<IMovimentoService>().ListarSolicitacaoMaterial(UA, divisao);
            this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
            return retorno;
        }

        #endregion

        #region Webservice Empenho

        public IList<MovimentoEntity> CarregarListaEmpenho(int UgeId, string anoMesRef, string userName, string password)
        {
            try
            {
                List<MovimentoEntity> movList = new List<MovimentoEntity>();
                MovimentoEntity mov = null;
                UGEEntity uge = this.Service<IUGEService>().Listar(a => a.Id == UgeId).FirstOrDefault();
                GestorEntity lObjGestor = this.Service<IGestorService>().Listar(uge.Orgao.Id.Value).FirstOrDefault();

                string sUsuario = Siafem.userNameConsulta;
                string sSenha = Siafem.passConsulta;
                string sAnoBase = anoMesRef.Substring(0, 4);
                string sMes = MesExtenso.Mes[int.Parse(anoMesRef.Substring(4, 2))];
                string sUnidadeGestora = uge.Codigo.ToString();
                string sCodigoGestao = lObjGestor.CodigoGestao.ToString().PadLeft(5, '0');
                string[] naturezasDespesaPermitidas = null;

                if (sSenha == null || sSenha == "")
                {
                    mov = new MovimentoEntity();
                    mov.Observacoes = "LOGINERROR";
                    movList.Add(mov);
                    return movList;
                }

                // conectando WS
                string sRetorno = Siafem.recebeMsg(sUsuario, sSenha, sAnoBase, sUnidadeGestora,
                    //Siafem.wsSIAFDetaContaGen(uge.Codigo.ToString(), sCodigoGestao, sMes, "192410101", "1"), true);
                    //Siafem.SiafemDocDetaContaGen((int)uge.Codigo, (int)lObjGestor.CodigoGestao, sMes, "192410101", "1"), true);
                    GeradorEstimuloSIAF.SiafemDocDetaContaGen((int)uge.Codigo, (int)lObjGestor.CodigoGestao, sMes, "192410101", "1"), true);

                //string resposta = Siafem.trataErro(sRetorno);

                //if (resposta != "")
                //{
                //   this.ListaErro.Add(resposta);
                //    return null;
                //}
                //string retorno = Siafem.trataErro(sRetorno);

                string strNomeMensagem = "";
                string resposta = "";

                //if (retorno != "") 
                if (Siafem.VerificarErroMensagem(sRetorno, out strNomeMensagem, out resposta))
                {
                    //this.ListaErro.Add(resposta);
                    resposta.BreakLine(Environment.NewLine.ToCharArray()).ToList().ForEach(linhaErro =>this.ListaErro.Add(linhaErro));
                    return null;
                }

                // listar os documentos
                System.Xml.XmlNodeList elemento = Sam.Common.XmlUtil.lerNodeListXml(sRetorno,
                    "/MSG/SISERRO/Doc_Retorno/SIAFDOC/SiafemDetaconta/documento/Repete/Documento");

                naturezasDespesaPermitidas = new string[] { Constante.CST_NATUREZA_DESPESA__BEM_CONSUMO_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_GERAL__INICIO_SEIS_DIGITOS, Constante.CST_NATUREZA_DESPESA__BEM_PERMAMENTE_EQUIP_TI__INICIO_SEIS_DIGITOS };
                foreach (System.Xml.XmlNode node in elemento)
                {
                    string contaCorrente = node["ContaCorrente"].InnerText;
                    double valorLiquidar = Convert.ToDouble(node["ValorConta"].InnerText);

                    if (contaCorrente != "")
                    {
                        // 0 a 14 CNPJ
                        string cnpj = contaCorrente.Substring(0, 14); // "00366257000161";
                        //15 a 26 Nota de empenho
                        string empenho = contaCorrente.Substring(15, 11); // "2011NE01000";// 
                        //27 a 35 Natureza de Despesa
                        string natDespesa = contaCorrente.Substring(27, 8);
                        //36 a 38 Fonte
                        string fonte = contaCorrente.Substring(36, 3);

                        // procura fornecedor (no combo, irá considerar no SelectedValue o ID do fornecedor)
                        mov = new MovimentoEntity();
                        mov.GeradorDescricao = cnpj;
                        mov.NumeroDocumento = empenho;
                        mov.NaturezaDespesaEmpenho = natDespesa;
                        // adiciona somente os empenhos cujas naturezas de despesas são de material de consumo e permanente
                        //if (natDespesa.Substring(0, 6) == "339030" || natDespesa.Substring(0, 6) == "449052")
                        if (naturezasDespesaPermitidas.Contains(natDespesa.Substring(0, 6)))
                            movList.Add(mov);
                    }
                }

                if (movList.Count > 0)
                {
                    movList = movList.OrderBy(a => a.NumeroDocumento).ToList();
                }

                return movList;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add("Erro no sistema: " + e.Message);
                return null;
            }
        }

        public IList<MovimentoEntity> CarregarListaEmpenhosPorFornecedor(int UgeId, int AlmoxId, string anoMesRef, string userName, string password, string CnpjCpf)
        {
            try
            {
                List<MovimentoEntity> movList = new List<MovimentoEntity>();
                MovimentoEntity mov = null;
                UGEEntity uge = this.Service<IUGEService>().Listar(a => a.Id == UgeId).FirstOrDefault();

                string sUsuario = Siafem.userNameConsulta;
                string sSenha = Siafem.passConsulta;
                string sAnoBase = anoMesRef.Substring(0, 4);
                string sMes = MesExtenso.Mes[int.Parse(anoMesRef.Substring(4, 2))];
                string sUnidadeGestora = uge.Codigo.ToString();

                // retirar este comentário abaixo:

                if (sSenha == null || sSenha == "")
                {
                    mov = new MovimentoEntity();
                    mov.Observacoes = "LOGINERROR";
                    movList.Add(mov);
                    return movList;
                }

                IList<MovimentoEntity> MovimentoEmpenhos = this.CarregarListaEmpenho(UgeId, anoMesRef, userName, password);

                if (MovimentoEmpenhos != null)
                {
                    movList = new List<MovimentoEntity>();
                    movList = MovimentoEmpenhos.Where(a => a.GeradorDescricao == CnpjCpf).ToList();
                }

                if (movList.Count == 0)
                {
                   this.ListaErro.Add("Empenhos do fornecedor não encontrados!");
                    return null;
                }

                return movList;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add("Erro no sistema: " + e.Message);
                return null;
            }
        }

        public IList<EmpenhoEventoEntity> ListarEmpenhoEvento()
        {
            return this.Service<IEmpenhoEventoService>().Listar();
        }

        public IList<EmpenhoLicitacaoEntity> ListarEmpenhoLicitacao()
        {
            return this.Service<IEmpenhoLicitacaoService>().Listar();
        }

        public IList<PTResEntity> ListarPTRES()
        {
            return this.Service<IPTResService>().Listar();
        }

        public PTResEntity ObterPTRes(int codigoPTRes, int codigoUGE)
        {
            PTResEntity objRetorno = null;
            var servicePtRes = this.Service<IPTResService>();

            objRetorno = servicePtRes.ObterPTRes(codigoPTRes, codigoUGE);

            return objRetorno;
        }

        public IList<PTResEntity> ObterPTRes(int codigoPTRes, int codigoUGE, int? ptresAcao = null)
        {
            IList<PTResEntity> objRetorno = null;
            var servicePtRes = this.Service<IPTResService>();

            objRetorno = servicePtRes.ObterPTRes(codigoPTRes, codigoUGE, ptresAcao);

            return objRetorno;
        }

        public IList<PTResEntity> ObterListagemPTResDaUGE(int codigoUGE)
        {
            IList<PTResEntity> lstPTRes = null;
            var servicePtRes = this.Service<IPTResService>();

            lstPTRes = servicePtRes.Listar(codigoUGE, true);

            return lstPTRes;
        }

        public MovimentoEntity ObterMovimento(int movID, bool isEmpenho = false)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    var srvInfra = this.Service<IMovimentoService>();
                    MovimentoEntity objRetorno = null;

                    objRetorno = srvInfra.ObterMovimentacao(movID, isEmpenho);

                    return objRetorno;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public MovimentoEntity ObterMovimentacaoConsumoImediato(int movimentacaoMaterialID)
        {
            //this.Service<IMovimentoService>().Entity = new MovimentoEntity(movimentacaoMaterialID);
            var movimentacaoMaterial = this.Service<IMovimentoService>().GetMovimentacaoConsumoImediato(movimentacaoMaterialID);

            if (movimentacaoMaterial == null)
                throw new Exception();
            else
            {
                if (movimentacaoMaterial.MovimentoItem.Count() == 0)
                    throw new Exception();
                else
                {
                    foreach (var item in movimentacaoMaterial.MovimentoItem)
                    {
                        item.Movimento = movimentacaoMaterial;

                        item.ValorMov = ((decimal)item.ValorMov).Truncar(2);
                        item.PrecoUnit = ((decimal)item.ValorMov).Truncar(4);

                        //break;
                    }

                    movimentacaoMaterial.ValorOriginalDocumento = movimentacaoMaterial.ValorDocumento;
                    movimentacaoMaterial.ValorDocumento = movimentacaoMaterial.MovimentoItem.Sum(movItem => movItem.ValorMov.Value).Truncar(2);

                    movimentacaoMaterial.MovimentoItem.ToList().ForEach(movItem => movItem.ValorTotalDocEntrada = movimentacaoMaterial.ValorDocumento.ToString());
                    return movimentacaoMaterial;
                }
            }
        }

        #endregion

        #region pseudo-mocks

        public string ObterDescricaoTipoEmpenho(MovimentoEntity pObjMovimento)
        {

            int lIntCodigoEmpenhoEvento = 0;
            int lIntCodigoEmpenhoLicitacao = 0;
            string lStrRetorno = string.Empty;
            bool lBlnEmpenhoEventoIsNull = false;
            bool lBlnEmpenhoLicitacaoIsNull = false;
            bool lBlnIsBEC = false;


            //lBlnEmpenhoEventoIsNull = (pObjMovimento.EmpenhoEvento == null || pObjMovimento.EmpenhoEvento.Codigo == null);
            lBlnEmpenhoEventoIsNull = (pObjMovimento.EmpenhoEvento == null || pObjMovimento.EmpenhoEvento.Codigo <= 0);
            lBlnEmpenhoLicitacaoIsNull = (pObjMovimento.EmpenhoLicitacao == null || String.IsNullOrEmpty(pObjMovimento.EmpenhoLicitacao.CodigoFormatado));


            if (lBlnEmpenhoEventoIsNull || lBlnEmpenhoLicitacaoIsNull)
            {
                pObjMovimento.EmpenhoEvento = this.Service<IEmpenhoEventoService>().ObterEventoEmpenho(pObjMovimento.EmpenhoEvento.Id.Value);
                pObjMovimento.EmpenhoLicitacao = this.Service<IEmpenhoLicitacaoService>().ObterTipoLicitacao(pObjMovimento.EmpenhoLicitacao.Id.Value);

                lIntCodigoEmpenhoEvento = pObjMovimento.EmpenhoEvento.Codigo;
                lIntCodigoEmpenhoLicitacao = Int32.Parse(pObjMovimento.EmpenhoLicitacao.CodigoFormatado);
            }
            else if (!lBlnEmpenhoEventoIsNull)
            {
                lIntCodigoEmpenhoEvento = (int)pObjMovimento.EmpenhoEvento.Codigo;
                pObjMovimento.EmpenhoEvento.Codigo = lIntCodigoEmpenhoEvento;
            }
            else if (!lBlnEmpenhoLicitacaoIsNull)
            {
                lIntCodigoEmpenhoEvento = Int32.Parse(pObjMovimento.EmpenhoLicitacao.CodigoFormatado);
                pObjMovimento.EmpenhoLicitacao.CodigoFormatado = lIntCodigoEmpenhoEvento.ToString();
            }

            lBlnIsBEC = (lIntCodigoEmpenhoEvento == (int)GeralEnum.EmpenhoEvento.BEC);

            if (lBlnIsBEC)
                lStrRetorno = "BEC";
            else if (!lBlnIsBEC && !(lIntCodigoEmpenhoLicitacao == (int)GeralEnum.EmpenhoLicitacao.Pregao))
                lStrRetorno = "SIAFISICO";
            else
                lStrRetorno = pObjMovimento.EmpenhoLicitacao.Descricao;


            return lStrRetorno;
        }


        #endregion pseudo-mocks

        public void CancelarRequisicao(int idRequisicao)
        {
            try
            {
                this.Service<IMovimentoService>().Entity.IdLogin = this.Movimento.IdLogin;
                this.Service<IMovimentoService>().CancelarRequisicao(idRequisicao);
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
               this.ListaErro.Add(e.Message);
            }
        }

        public IList<MovimentoEntity> VerificarTransferenciasPendentes(int pIntAlmoxarifadoOrigem, int pIntMesReferencia)
        {
            IList<MovimentoEntity> lLstRetorno = null;
            IList<MovimentoEntity> lLstRetornoFiltro = null;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {
                    lLstRetorno = this.Service<IMovimentoService>().VerificarTransferenciasPendentes(pIntAlmoxarifadoOrigem, pIntMesReferencia, true);

                    if (lLstRetorno != null && lLstRetorno.Count > 0)
                    {
                       this.ListaErro.Add(String.Format("Transferências pendentes encontradas.\n"));

                        lLstRetornoFiltro = new List<MovimentoEntity>();
                        (lLstRetorno as List<MovimentoEntity>).ForEach(Movimentacoes =>
                                                                                        {
                                                                                            if (!String.IsNullOrWhiteSpace(Movimentacoes.Observacoes))
                                                                                                lLstRetornoFiltro.Add(Movimentacoes);
                                                                                        });

                        (lLstRetornoFiltro as List<MovimentoEntity>).ForEach(Movimentacoes =>this.ListaErro.Add(Movimentacoes.Observacoes));
                    }
                }
                catch (Exception lExcExcecao)
                {
                    new LogErro().GravarLogErro(lExcExcecao);
                   this.ListaErro.Add(lExcExcecao.Message);
                }
            }
            //return lLstRetorno;
            return lLstRetornoFiltro;
        }

        public IList<MovimentoItemEntity> ListarNotaRequisicao(int MovimentoId)
        {
            return this.Service<IMovimentoItemService>().ListarNotaRequisicao(MovimentoId);
        }


        public static bool visivelInativosTransacoesPendentes(string inativo, string pendentes)
        {
            if (inativo.Trim().Length > 0)
            {
                return true;
            }
            else if (pendentes.Trim().Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IList<MovimentoEntity> ListarNotasdeFornecimento(string palavraChave, int tipoMovimento, int idAlmox, string anoMesRef, string dataDigitada)
        {
            Expression<Func<MovimentoEntity, bool>> expression = null;

            if (string.IsNullOrEmpty(anoMesRef))
            {

                expression = a => a.NumeroDocumento == palavraChave
                                  && a.Ativo == true
                                  && (a.TipoMovimento.Id == tipoMovimento)
                                  && (a.Almoxarifado.Id == idAlmox);

            }
            if (!((string.IsNullOrEmpty(anoMesRef)) && !string.IsNullOrEmpty( dataDigitada)) && tipoMovimento==(int) GeralEnum.TipoMovimento.RequisicaoPendente)
            {
                DateTime data =new DateTime();
                data = Convert.ToDateTime(dataDigitada + " 23:00:00.190");              
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                    IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarRequisicaoPendente(palavraChave, tipoMovimento, idAlmox, data);
                    this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                    return retorno;
                }

            }
            if (!string.IsNullOrEmpty(anoMesRef))
            {
                expression = a => a.NumeroDocumento == palavraChave
                                      && a.Ativo == true
                                      && (a.TipoMovimento.Id == tipoMovimento)
                                      && (a.Almoxarifado.Id == idAlmox)
                                       && (a.AnoMesReferencia == anoMesRef);
            }
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                this.Service<IMovimentoService>().SkipRegistros = this.SkipRegistros;
                IList<MovimentoEntity> retorno = this.Service<IMovimentoService>().ListarNotasdeFornecimento(expression);
                this.TotalRegistros = this.Service<IMovimentoService>().TotalRegistros();
                return retorno;
            }
        }

        public IList<MovimentoEntity> ImprimirConsultaConsulmoAlmox(long _subitemCodigo, string _dataInicio, string _dataFinal, int _gestorId)
        {
            MovimentoInfrastructure _movInfra = new MovimentoInfrastructure();
            return _movInfra.ImprimirConsultaConsulmoAlmox(_subitemCodigo, _dataInicio, _dataFinal, _gestorId);
        }

        public IList<MovimentoEntity> ImprimirConsultaTransferencia(int _almoxId, DateTime _dataInicial, DateTime _dataFinal)
        {
            MovimentoInfrastructure _movInfra = new MovimentoInfrastructure();
            return _movInfra.ImprimirConsultaTransferencia(_almoxId, _dataInicial, _dataFinal);
        }



        //public void ExecutaEstornoSemSIAF(MovimentoEntity movimentacaoMaterial, int idLoginEstornante)
        //public void ExecutaEstornoSemSIAF(int idLoginEstornante)
        public Tuple<bool, List<string>> ExecutaEstornoSemSIAF(int idLoginEstornante, string InscricaoCE)
        {
            Tuple<bool, List<string>> statusRetorno = new Tuple<bool, List<string>>(true, null);
            NotaLancamentoPendenteSIAFEMBusiness objBusiness = null;

            if (!ExisteNotaSiafemVinculada())
            {
                //Processo de estorno normal
                if (this.Movimento.TipoMovimento.AgrupamentoId.Value == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                    statusRetorno = EstornarMovimentoEntrada(idLoginEstornante, InscricaoCE);
                else if ((this.Movimento.TipoMovimento.AgrupamentoId.Value == (int)GeralEnum.TipoMovimentoAgrupamento.Saida) || (this.Movimento.TipoMovimento.AgrupamentoId.Value == (int)GeralEnum.TipoMovimentoAgrupamento.Requisicao))
                    statusRetorno = EstornarMovimentoSaida(idLoginEstornante, InscricaoCE);
                else if ((this.Movimento.TipoMovimento.AgrupamentoId.Value == (int)GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato))
                    statusRetorno = EstornarMovimentoConsumo(idLoginEstornante, InscricaoCE);

                if (statusRetorno.Item1)
                {
                    //Se houver pendências SIAF ativas, tornar inativas
                    objBusiness = new NotaLancamentoPendenteSIAFEMBusiness();
                    objBusiness.InativarPendenciasPorMovimentacao(movimento.Id.Value);

                    if (objBusiness.ListaErro.HasElements() && ListaErro.HasElements())
                        ListaErro.AddRange(objBusiness.ListaErro);
                    else if (objBusiness.ListaErro.HasElements() && !ListaErro.HasElements())
                        ListaErro = objBusiness.ListaErro;
                }
            }

            return statusRetorno;
        }

        public bool VerificaSePermiteEstornoSemSIAF(MovimentoEntity movimentacaoMaterial, bool executarSePermitir = false)
        {
            bool retornoStatus = false;
            MovimentoBusiness objBusiness = null;

            objBusiness = new MovimentoBusiness();
            objBusiness.Movimento = movimentacaoMaterial;
            retornoStatus = !objBusiness.ExisteNotaSiafemVinculada();

            return retornoStatus;
        }


        public decimal ConsultarSaldoTotalAlmoxarifado(int idAlmoxarifado)
        {
            return this.Service<IMovimentoService>().ConsultarSaldoTotalAlmoxarifado(idAlmoxarifado);
        }

        public decimal ConsultarSaldoTotalAlmoxarifado33(int idAlmoxarifado)
        {
            return this.Service<IMovimentoService>().ConsultarSaldoTotalAlmoxarifado33(idAlmoxarifado);
        }

        public decimal ConsultarSaldoTotalAlmoxarifado44(int idAlmoxarifado)
        {
            return this.Service<IMovimentoService>().ConsultarSaldoTotalAlmoxarifado44(idAlmoxarifado);
        }
    }


}
