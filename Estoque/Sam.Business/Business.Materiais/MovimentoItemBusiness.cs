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
    public class MovimentoItemBusiness : BaseBusiness, ICrudBaseBusiness<TB_MOVIMENTO_ITEM>
    {
        #region Métodos Customizados

        /// <summary>
        /// Retorna IQueryable do Movimento Item por filtros
        /// </summary>
        /// <param name="tbMovItem">Filtros: SubItemId, UGEID, Almoxarifado, Lote, Ativo e DataMovimento</param>
        /// <returns>IQueryable de TB_MOVIMENTO_ITEM </returns>
        public IQueryable<TB_MOVIMENTO_ITEM> QueryMovimentoBy(MovimentoItemEntity tbMovItem)
        {
            MovimentoItemInfrastructure infraMovimentoItem = new MovimentoItemInfrastructure();
            infraMovimentoItem.LazyLoadingEnabled = true;

            IQueryable<TB_MOVIMENTO_ITEM> tbMov = infraMovimentoItem.QueryAll(a => a.TB_SUBITEM_MATERIAL_ID == tbMovItem.SubItemMaterial.Id &&
                    a.TB_UGE_ID == tbMovItem.UGE.Id &&
                    a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == tbMovItem.Movimento.Almoxarifado.Id &&
                    (a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == tbMovItem.DataVencimentoLote || tbMovItem.DataVencimentoLote == null) &&
                    (a.TB_MOVIMENTO_ITEM_LOTE_FABR == tbMovItem.FabricanteLote || tbMovItem.FabricanteLote == null) &&
                    (a.TB_MOVIMENTO_ITEM_LOTE_IDENT == tbMovItem.IdentificacaoLote || tbMovItem.IdentificacaoLote == null) &&
                    ((a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada) ||
                    (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)) &&
                //a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= DateTime.Now &&
                    (a.TB_MOVIMENTO_ITEM_ATIVO == true)
                    );
            return tbMov;
        }

        public void ListarMovimentoItem()
        {
            //Exemplo para sort
            Expression<Func<TB_MOVIMENTO_ITEM, int>> sort = a => a.TB_MOVIMENTO_ITEM_ID;

            //Exemplo para Where
            Expression<Func<TB_MOVIMENTO_ITEM, bool>> where = a => a.TB_MOVIMENTO_ITEM_ID == 1;

            MovimentoItemInfrastructure persistencia = new MovimentoItemInfrastructure();

            persistencia.LazyLoadingEnabled = false;
            var result = persistencia.SelectAll();

            persistencia.LazyLoadingEnabled = true;
            var result2 = persistencia.SelectAll();

            var result3 = persistencia.SelectWhere(sort, false, where,  0);


            foreach (var i in result)
            {

            }
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_MOVIMENTO_ITEM entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public void Update(TB_MOVIMENTO_ITEM entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public void Delete(TB_MOVIMENTO_ITEM entity)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ITEM_ID == entity.TB_MOVIMENTO_ITEM_ID);
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
        public void DeleteRelatedEntries(TB_MOVIMENTO_ITEM entity)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ITEM_ID == entity.TB_MOVIMENTO_ITEM_ID);
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
        public void DeleteRelatedEntries(TB_MOVIMENTO_ITEM entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ITEM_ID == entity.TB_MOVIMENTO_ITEM_ID);
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
        public TB_MOVIMENTO_ITEM SelectOne(Expression<Func<TB_MOVIMENTO_ITEM, bool>> where)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public List<TB_MOVIMENTO_ITEM> SelectAll()
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public List<TB_MOVIMENTO_ITEM> SelectAll(Expression<Func<TB_MOVIMENTO_ITEM, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public List<TB_MOVIMENTO_ITEM> SelectWhere(Expression<Func<TB_MOVIMENTO_ITEM, bool>> where)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
                var result = infraestrutura.SelectWhere(where);
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
        public List<TB_MOVIMENTO_ITEM> SelectWhereLazy(Expression<Func<TB_MOVIMENTO_ITEM, bool>> where)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
                var result = infraestrutura.SelectWhere(where);
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
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO_ITEM> SelectWhere(Expression<Func<TB_MOVIMENTO_ITEM, int>> sortExpression, bool desc, Expression<Func<TB_MOVIMENTO_ITEM, bool>> where, int startRowIndex)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public List<TB_MOVIMENTO_ITEM> SelectWhere(Expression<Func<TB_MOVIMENTO_ITEM, string>> sortExpression, bool desc, Expression<Func<TB_MOVIMENTO_ITEM, bool>> where, int startRowIndex)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public IQueryable<TB_MOVIMENTO_ITEM> QueryAll()
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public IQueryable<TB_MOVIMENTO_ITEM> QueryAll(Expression<Func<TB_MOVIMENTO_ITEM, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public int GetCount(Expression<Func<TB_MOVIMENTO_ITEM, bool>> where)
        {
            try
            {
                MovimentoItemInfrastructure infraestrutura = new MovimentoItemInfrastructure();
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
        public void Consistir(TB_MOVIMENTO_ITEM entity)
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
