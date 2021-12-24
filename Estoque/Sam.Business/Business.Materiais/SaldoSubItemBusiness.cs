using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;


namespace Sam.Business
{
    public class SaldoSubItemBusiness : BaseBusiness, ICrudBaseBusiness<TB_SALDO_SUBITEM>
    {
        #region Métodos Customizados

        /// <summary>
        /// Subtrai o saldo com o movimento. Utilizado na Saida e Estorno da Entrada
        /// </summary>
        /// <param name="tbMovItem">Movimento com os valores da operação</param>
        /// <param name="saldo">Entidade de saldo para realizar a atualização</param>
        /// <returns>Entidade de estorno atualizada</returns>
        public TB_SALDO_SUBITEM UpdateSaldoSubtrair(TB_MOVIMENTO_ITEM tbMovItem, TB_SALDO_SUBITEM saldo)
        {
            //?????
            saldo.TB_SALDO_SUBITEM_SALDO_VALOR -= tbMovItem.TB_MOVIMENTO_ITEM_VALOR_MOV;

            if (tbMovItem.TB_MOVIMENTO_ITEM_QTDE_LIQ != null && tbMovItem.TB_MOVIMENTO_ITEM_QTDE_LIQ != 0)//Saida
            {
                saldo.TB_SALDO_SUBITEM_SALDO_QTDE -= tbMovItem.TB_MOVIMENTO_ITEM_QTDE_LIQ;
            }
            else
            {
                saldo.TB_SALDO_SUBITEM_SALDO_QTDE -= tbMovItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
            }

            return saldo;
        }
        /// <summary>
        /// Soma o saldo com o movimento. Utilizado na Entrada e Estorno da Saida
        /// </summary>
        /// <param name="tbMovItem">Movimento com os valores da operação</param>
        /// <param name="saldo">Entidade de saldo para realizar a atualização</param>
        /// <returns>Entidade de estorno atualizada</returns>
        public TB_SALDO_SUBITEM UpdateSaldoSomar(TB_MOVIMENTO_ITEM tbMovItem, TB_SALDO_SUBITEM saldo)
        {
            saldo.TB_SALDO_SUBITEM_SALDO_QTDE += tbMovItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
            saldo.TB_SALDO_SUBITEM_SALDO_VALOR += tbMovItem.TB_MOVIMENTO_ITEM_VALOR_MOV;

            return saldo;
        }

        /// <summary>
        /// Retorna a Entidade Saldo preenchida com os valores do Movimento
        /// </summary>
        /// <param name="tbMovItem">Movimento item da operação</param>
        public TB_SALDO_SUBITEM SetEntidade(TB_MOVIMENTO_ITEM tbMovItem)
        {
            TB_SALDO_SUBITEM tbSaldo = new TB_SALDO_SUBITEM();

            tbSaldo.TB_ALMOXARIFADO_ID = tbMovItem.TB_MOVIMENTO.TB_ALMOXARIFADO_ID;
            tbSaldo.TB_UGE_ID = tbMovItem.TB_UGE_ID;
            tbSaldo.TB_SUBITEM_MATERIAL_ID = tbMovItem.TB_SUBITEM_MATERIAL_ID;
            tbSaldo.TB_SALDO_SUBITEM_SALDO_QTDE = tbMovItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
            tbSaldo.TB_SALDO_SUBITEM_SALDO_VALOR = tbMovItem.TB_MOVIMENTO_ITEM_VALOR_MOV;
            tbSaldo.TB_SALDO_SUBITEM_PRECO_UNIT = 0;
            tbSaldo.TB_SALDO_SUBITEM_LOTE_DT_VENC = tbMovItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC;
            tbSaldo.TB_SALDO_SUBITEM_LOTE_IDENT = tbMovItem.TB_MOVIMENTO_ITEM_LOTE_IDENT;
            tbSaldo.TB_SALDO_SUBITEM_LOTE_FAB = tbMovItem.TB_MOVIMENTO_ITEM_LOTE_FABR;

            return tbSaldo;
        }

        /// <summary>
        /// Retorna o saldo
        /// </summary>
        /// <param name="tbMovItem">SubItemId, AlmoxarifadoId, UGEID, por lote</param>
        /// <returns>Se existir retorna o saldo, se não null</returns>
        public TB_SALDO_SUBITEM RetornaSaldoExistente(TB_MOVIMENTO_ITEM tbMovItem)
        {
            var saldo = this.SelectOne(a => a.TB_SUBITEM_MATERIAL_ID == tbMovItem.TB_SUBITEM_MATERIAL_ID &&
                a.TB_UGE_ID == tbMovItem.TB_UGE_ID &&
                a.TB_ALMOXARIFADO_ID == tbMovItem.TB_MOVIMENTO.TB_ALMOXARIFADO_ID &&
                (a.TB_SALDO_SUBITEM_LOTE_DT_VENC == tbMovItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC || tbMovItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null) &&
                (a.TB_SALDO_SUBITEM_LOTE_FAB == tbMovItem.TB_MOVIMENTO_ITEM_LOTE_FABR || tbMovItem.TB_MOVIMENTO_ITEM_LOTE_FABR == null) &&
                (a.TB_SALDO_SUBITEM_LOTE_IDENT == tbMovItem.TB_MOVIMENTO_ITEM_LOTE_IDENT || tbMovItem.TB_MOVIMENTO_ITEM_LOTE_IDENT == null)
                );

            return saldo;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_SALDO_SUBITEM entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public void Update(TB_SALDO_SUBITEM entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public void Delete(TB_SALDO_SUBITEM entity)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SALDO_SUBITEM_ID == entity.TB_SALDO_SUBITEM_ID);
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
        public void DeleteRelatedEntries(TB_SALDO_SUBITEM entity)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SALDO_SUBITEM_ID == entity.TB_SALDO_SUBITEM_ID);
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
        /// Eclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_SALDO_SUBITEM entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SALDO_SUBITEM_ID == entity.TB_SALDO_SUBITEM_ID);
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
        public TB_SALDO_SUBITEM SelectOne(Expression<Func<TB_SALDO_SUBITEM, bool>> where)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public List<TB_SALDO_SUBITEM> SelectAll()
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public List<TB_SALDO_SUBITEM> SelectAll(Expression<Func<TB_SALDO_SUBITEM, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public List<TB_SALDO_SUBITEM> SelectWhere(Expression<Func<TB_SALDO_SUBITEM, bool>> where)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public List<TB_SALDO_SUBITEM> SelectWhere(Expression<Func<TB_SALDO_SUBITEM, int>> sortExpression, bool desc, Expression<Func<TB_SALDO_SUBITEM, bool>> where, int startRowIndex)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public List<TB_SALDO_SUBITEM> SelectWhere(Expression<Func<TB_SALDO_SUBITEM, string>> sortExpression, bool desc, Expression<Func<TB_SALDO_SUBITEM, bool>> where, int startRowIndex)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public IQueryable<TB_SALDO_SUBITEM> QueryAll()
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public IQueryable<TB_SALDO_SUBITEM> QueryAll(Expression<Func<TB_SALDO_SUBITEM, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public int GetCount(Expression<Func<TB_SALDO_SUBITEM, bool>> where)
        {
            try
            {
                SaldoSubItemInfrastructure infraestrutura = new SaldoSubItemInfrastructure();
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
        public void Consistir(TB_SALDO_SUBITEM entity)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        
    }
}
