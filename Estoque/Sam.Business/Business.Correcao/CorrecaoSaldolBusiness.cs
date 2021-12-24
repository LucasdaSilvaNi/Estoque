using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Domain.Entity;

namespace Sam.Business
{
    public class CorrecaoSaldolBusiness : BaseBusiness, ICrudBaseBusiness<TB_CORRECAO_SALDO>
    {
        #region Métodos Customizados

        private void ExcluirMovimento(int movimentoId)
        {
            TB_MOVIMENTO_ITEM entity = new MovimentoItemBusiness().SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);
            TB_MOVIMENTO tb_movimento = new MovimentoBusiness().SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);
            new MovimentoItemBusiness().Delete(entity);
            new MovimentoBusiness().Delete(tb_movimento);
        }

        private void AlterarStatusCorrecao(TB_CORRECAO_SALDO correcao)
        {
            correcao.ATIVO = false;
            new CorrecaoSaldolBusiness().Update(correcao);
        }

        private IList<TB_CORRECAO_SALDO> ListarCorrecaoSaldo()
        {
            return this.SelectWhere(a => a.ATIVO == true);
        }

        private bool ValidarRecalculoSaldo(TB_CORRECAO_SALDO correcao)
        {
            return this.RecalcularSaldoMovimentoItem(correcao);
        }

        private MovimentoEntity GetMovimento(TB_CORRECAO_SALDO correcao)
        {
            MovimentoEntity entity = new MovimentoEntity();
            entity.MovimentoItem = new List<MovimentoItemEntity>();
            MovimentoItemEntity item = new MovimentoItemEntity();
            item.UGE = new UGEEntity(correcao.TB_UGE_ID);
            item.ItemMaterial= new ItemMaterialEntity(26820);
            item.SubItemMaterial = new SubItemMaterialEntity(correcao.TB_SUBITEM_MATERIAL_ID);
            item.QtdeMov = 1;
            item.QtdeLiq = (0);
            item.PrecoUnit = (1);
            item.SaldoValor = (1);
            item.SaldoQtde = (1);
            item.ValorMov = (1);
            item.Desd = (0);
            item.Ativo = (true);
            entity.MovimentoItem.Add(item);

            entity.Almoxarifado= new AlmoxarifadoEntity(correcao.TB_ALMOXARIFADO_ID);

            entity.AnoMesReferencia = ("201204");
            entity.Ativo = (true);
            entity.CodigoDescricao = ("123");
            entity.CodigoFormatado = ("123");
            entity.DataMovimento= new DateTime?(Convert.ToDateTime("01/04/2012"));
            entity.DataDocumento= new DateTime?(Convert.ToDateTime("01/04/2012"));
            entity.DataOperacao= new DateTime?(DateTime.Now);
            entity.Empenho = ("");
            entity.FonteRecurso = (string.Empty);
            entity.IdLogin = (1);
            entity.Instrucoes = ("");
            entity.NaturezaDespesaEmpenho = ("");
            entity.Fornecedor = new FornecedorEntity(1);
            entity.NumeroDocumento = ("201388888888");
            entity.Observacoes = ("INSERIDO AUTOMATICAMENTE PARA CORREÇÃO DA BASE");
            entity.TipoMovimento = new TipoMovimentoEntity(5);
            entity.UGE = new UGEEntity(correcao.TB_UGE_ID);
            entity.ValorDocumento = (1);
            entity.Divisao = new DivisaoEntity(348);
            return entity;
        }

        private DateTime RetornaMesAnoRefDate(string anoMesRef)
        {
            anoMesRef = anoMesRef.Insert(4, "-");
            return Convert.ToDateTime(anoMesRef + "-01");
        }

        private bool RecalcularSaldoMovimentoItem(TB_CORRECAO_SALDO correcao)
        {
            MovimentoItemBusiness business = new MovimentoItemBusiness();

            var qtdSaldo = _valorZero;
            var qtdSaldoFechamento = _valorZero;
            decimal? saldoValor = 0;
            decimal? saldoValorFechamento = 0;

            TB_ALMOXARIFADO almoxarifado = new AlmoxarifadoBusiness().SelectOne(a => a.TB_ALMOXARIFADO_ID == correcao.TB_ALMOXARIFADO_ID);
            if (almoxarifado != null)
            {
                List<TB_FECHAMENTO> list = new FechamentoMensalBusiness().SelectWhere(a => (((a.TB_ALMOXARIFADO_ID == correcao.TB_ALMOXARIFADO_ID) 
                    && (a.TB_SUBITEM_MATERIAL_ID == correcao.TB_SUBITEM_MATERIAL_ID)) 
                    && (a.TB_UGE_ID == correcao.TB_UGE_ID)) 
                    && (a.TB_FECHAMENTO_SITUACAO == 1));

                if (list == null)
                {
                    return false;
                }

                List<TB_MOVIMENTO_ITEM> source = new MovimentoItemBusiness().SelectWhereLazy(a => (((a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == correcao.TB_ALMOXARIFADO_ID) 
                    && (a.TB_SUBITEM_MATERIAL_ID == correcao.TB_SUBITEM_MATERIAL_ID)) 
                    && (a.TB_UGE_ID == correcao.TB_UGE_ID)) 
                    && a.TB_MOVIMENTO_ITEM_ATIVO);

                if (source.Count<TB_MOVIMENTO_ITEM>() == 0)
                {
                    return false;
                }
                foreach (TB_MOVIMENTO_ITEM tb_movimento_item in source)
                {
                    int? tipoAgrupamentoId = tb_movimento_item.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID;

                    if ((tipoAgrupamentoId.GetValueOrDefault() == 1) && tipoAgrupamentoId.HasValue)
                    {

                        qtdSaldo += Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_QTDE_MOV);
                        saldoValor += Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_VALOR_MOV);

                        if (tb_movimento_item.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO < this.RetornaMesAnoRefDate(almoxarifado.TB_ALMOXARIFADO_MES_REF))
                        {
                        	qtdSaldoFechamento += Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_QTDE_MOV);
                            saldoValorFechamento += Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_VALOR_MOV);
                        }
                    }
                    else
                    {
                        qtdSaldo -= Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_QTDE_MOV);
                        saldoValor -= Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_VALOR_MOV);

                        if (tb_movimento_item.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO < this.RetornaMesAnoRefDate(almoxarifado.TB_ALMOXARIFADO_MES_REF))
                        {
                            qtdSaldoFechamento -= Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_QTDE_MOV);
                            saldoValorFechamento -= Common.Util.TratamentoDados.ParseDecimalNull(tb_movimento_item.TB_MOVIMENTO_ITEM_VALOR_MOV);
                        }
                    }
                }

                if (qtdSaldo < 0)
                    return false;                
                
                if (saldoValor < 0)
                    return false;

                IEnumerable<TB_FECHAMENTO> enumerable = from a in list
                                                        where this.RetornaMesAnoRefDate(a.TB_FECHAMENTO_ANO_MES_REF.ToString()) == this.RetornaMesAnoRefDate(almoxarifado.TB_ALMOXARIFADO_MES_REF).AddMonths(-1)
                                                        select a;

                if (enumerable.Count<TB_FECHAMENTO>() > 0)
                {
                    return ((enumerable.FirstOrDefault<TB_FECHAMENTO>().TB_FECHAMENTO_SALDO_QTDE == qtdSaldoFechamento)
                        && (enumerable.FirstOrDefault<TB_FECHAMENTO>().TB_FECHAMENTO_SALDO_VALOR == saldoValorFechamento));
                }                
            }
            return false;
        }

        private TB_MOVIMENTO_ITEM RetornaUltimoMovimentoInserido(TB_CORRECAO_SALDO correcao)
        {
            return (from b in new MovimentoItemBusiness().SelectWhere(a => (((a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == correcao.TB_ALMOXARIFADO_ID) && (a.TB_SUBITEM_MATERIAL_ID == correcao.TB_SUBITEM_MATERIAL_ID)) && (a.TB_UGE_ID == correcao.TB_UGE_ID)) && (a.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO == "201388888888"))
                    orderby b.TB_MOVIMENTO_ITEM_DATA_OPERACAO descending
                    select b).FirstOrDefault<TB_MOVIMENTO_ITEM>();
        }

        public int CorrigirSaldo()
        {
            int num = 0;
            Sam.Domain.Business.MovimentoBusiness business = new Sam.Domain.Business.MovimentoBusiness();
            try
            {
                IList<TB_CORRECAO_SALDO> list = this.ListarCorrecaoSaldo();
                if (list.Count == 0)
                {
                    throw new Exception("Não existe correção para serem feitas");
                }

                foreach (TB_CORRECAO_SALDO tb_correcao_saldo in list)
                {
                    business.ListaErro = new List<string>();
                    if (this.ValidarRecalculoSaldo(tb_correcao_saldo))
                    {
                        business.Movimento = this.GetMovimento(tb_correcao_saldo);
                        business.GravarMovimento();
                        if (this.RetornaUltimoMovimentoInserido(tb_correcao_saldo) != null)
                        {
                            int movimentoId = this.RetornaUltimoMovimentoInserido(tb_correcao_saldo).TB_MOVIMENTO_ID;
                            business.Movimento = new MovimentoEntity(movimentoId);
                            business.EstornarMovimentoEntrada(1, "");
                            this.ExcluirMovimento(movimentoId);
                            this.AlterarStatusCorrecao(tb_correcao_saldo);
                            num++;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message, exception.InnerException);
            }
            return num;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_CORRECAO_SALDO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                    infraestrutura.Insert(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Update(TB_CORRECAO_SALDO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                    infraestrutura.Update(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui o objeto não relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Delete(TB_CORRECAO_SALDO entity)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CORRECAO_SALDO_ID == entity.TB_CORRECAO_SALDO_ID);
                infraestrutura.Delete(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui todos os objetos relecionados
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void DeleteRelatedEntries(TB_CORRECAO_SALDO entity)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CORRECAO_SALDO_ID == entity.TB_CORRECAO_SALDO_ID);
                infraestrutura.DeleteRelatedEntries(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_CORRECAO_SALDO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CORRECAO_SALDO_ID == entity.TB_CORRECAO_SALDO_ID);
                infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        public TB_CORRECAO_SALDO SelectOne(Expression<Func<TB_CORRECAO_SALDO, bool>> where)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        public List<TB_CORRECAO_SALDO> SelectAll()
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.SelectAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os objetos encontrados, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_CORRECAO_SALDO> SelectAll(Expression<Func<TB_CORRECAO_SALDO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                var result = infraestrutura.SelectAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_CORRECAO_SALDO> SelectWhere(Expression<Func<TB_CORRECAO_SALDO, bool>> where)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_CORRECAO_SALDO> SelectWhere(Expression<Func<TB_CORRECAO_SALDO, int>> sortExpression, bool desc, Expression<Func<TB_CORRECAO_SALDO, bool>> where, int startRowIndex)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos STRING</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_CORRECAO_SALDO> SelectWhere(Expression<Func<TB_CORRECAO_SALDO, string>> sortExpression, bool desc, Expression<Func<TB_CORRECAO_SALDO, bool>> where, int startRowIndex)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_CORRECAO_SALDO> QueryAll()
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.QueryAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="desc">Ordem decrescente</param>        
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_CORRECAO_SALDO> QueryAll(Expression<Func<TB_CORRECAO_SALDO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                var result = infraestrutura.QueryAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros encontrados na tabela
        /// </summary>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount()
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.GetCount();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount(Expression<Func<TB_CORRECAO_SALDO, bool>> where)
        {
            try
            {
                CorrecaoSaldoInfrastructure infraestrutura = new CorrecaoSaldoInfrastructure();
                return infraestrutura.GetCount(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region Métodos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(TB_CORRECAO_SALDO entity)
        {
        }

        #endregion
    }
}
