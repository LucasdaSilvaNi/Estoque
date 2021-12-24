using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Domain.Entity;
using Sam.Common.Util;


namespace Sam.Business
{
    public class UgeBusiness : BaseBusiness, ICrudBaseBusiness<TB_UGE>
    {
        #region Métodos Customizados

        public IList<TB_UGE> ListarUGE(int UoId)
        {
            UgeInfrastructure infra = new UgeInfrastructure();
            return infra.ListarUGE(UoId);
        }

        public IList<TB_UGE> ObterUGE(int UgeId)
        {
            UgeInfrastructure infra = new UgeInfrastructure();
            return infra.ObterUGE(UgeId);
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_UGE entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public void Update(TB_UGE entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public void Delete(TB_UGE entity)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UGE_ID == entity.TB_UGE_ID);
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
        public void DeleteRelatedEntries(TB_UGE entity)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UGE_ID == entity.TB_UGE_ID);
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
        public void DeleteRelatedEntries(TB_UGE entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UGE_ID == entity.TB_UGE_ID);
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
        public TB_UGE SelectOne(Expression<Func<TB_UGE, bool>> where)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public List<TB_UGE> SelectAll()
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_UGE> SelectAll(Expression<Func<TB_UGE, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public List<TB_UGE> SelectWhere(Expression<Func<TB_UGE, bool>> where)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_UGE> SelectWhere(Expression<Func<TB_UGE, int>> sortExpression, bool desc, Expression<Func<TB_UGE, bool>> where, int startRowIndex)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_UGE> SelectWhere(Expression<Func<TB_UGE, string>> sortExpression, bool desc, Expression<Func<TB_UGE, bool>> where, int startRowIndex)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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


        public List<UGEEntity> SelectById(int ugeId)
        {
            try
            {
                Expression<Func<TB_UGE, bool>> expression = _ => _.TB_UGE_ID == ugeId;
                return new UgeInfrastructure().SelectByExpression(expression).ToList();
            }
            catch (Exception ex)
            {                
                base.GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_UGE> QueryAll()
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_UGE> QueryAll(Expression<Func<TB_UGE, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public int GetCount(Expression<Func<TB_UGE, bool>> where)
        {
            try
            {
                UgeInfrastructure infraestrutura = new UgeInfrastructure();
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
        public void Consistir(TB_UGE entity)
        {
            try
            {
                if (entity.TB_UGE_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_UGE_DESCRICAO))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
                }

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
