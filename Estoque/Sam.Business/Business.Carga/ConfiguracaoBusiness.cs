using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Business.Importacao;

namespace Sam.Business
{
    public class ControleBusiness : BaseBusiness, ICrudBaseBusiness<TB_CONTROLE>
    {

        #region Métodos Customizados

        public List<TB_TIPO_CONTROLE> SelectTipoControle()
        {
            ControleInfrastructure infraestrutura = new ControleInfrastructure();
            return infraestrutura.SelectTipoControle();
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_CONTROLE entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public void Update(TB_CONTROLE entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public void Delete(TB_CONTROLE entity)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CONTROLE_ID == entity.TB_CONTROLE_ID);
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
        public void DeleteRelatedEntries(TB_CONTROLE entity)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CONTROLE_ID == entity.TB_CONTROLE_ID);
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
        public void DeleteRelatedEntries(TB_CONTROLE entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CONTROLE_ID == entity.TB_CONTROLE_ID);
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
        public TB_CONTROLE SelectOne(Expression<Func<TB_CONTROLE, bool>> where)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public List<TB_CONTROLE> SelectAll()
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public List<TB_CONTROLE> SelectAll(Expression<Func<TB_CONTROLE, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public List<TB_CONTROLE> SelectWhere(Expression<Func<TB_CONTROLE, bool>> where)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public List<TB_CONTROLE> SelectWhere(Expression<Func<TB_CONTROLE, int>> sortExpression, bool desc, Expression<Func<TB_CONTROLE, bool>> where, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public List<TB_CONTROLE> SelectWhere(Expression<Func<TB_CONTROLE, string>> sortExpression, bool desc, Expression<Func<TB_CONTROLE, bool>> where, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        /// Retorna uma lista de objetos paginados
        /// </summary>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns></returns>
        public List<TB_CONTROLE> Select(Expression<Func<TB_CONTROLE, DateTime?>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
                var result = infraestrutura.Select(sortExpression, startRowIndex);                
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
        /// Retorna uma lista de objetos paginados
        /// </summary>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns></returns>
        public List<TB_CONTROLE> SelectSituacao(Expression<Func<TB_CONTROLE, DateTime?>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
                var result = infraestrutura.SelectSituacao(sortExpression, startRowIndex);
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
        public IQueryable<TB_CONTROLE> QueryAll()
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();                
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
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_CONTROLE> QueryAll(Expression<Func<TB_CONTROLE, bool>> where, bool LazyLoadingEnabled)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
                infraestrutura.LazyLoadingEnabled = LazyLoadingEnabled;
                return infraestrutura.QueryAll(where);
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
        public IQueryable<TB_CONTROLE> QueryAll(Expression<Func<TB_CONTROLE, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public int GetCount(Expression<Func<TB_CONTROLE, bool>> where)
        {
            try
            {
                ControleInfrastructure infraestrutura = new ControleInfrastructure();
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
        public void Consistir(TB_CONTROLE entity)
        {
            try
            {
                //if (entity. == null)
                //{
                //    throw new Exception("O campo Código é de preenchimento obritagório");
                //}
                //if (string.IsNullOrEmpty(entity.TB_CONTROLE_DESCRICAO))
                //{
                //    throw new Exception("O campo Descrição é de preenchimento obritagório");
                //}
                //if (entity.TB_GESTOR_ID == null)
                //{
                //    throw new Exception("O campo Gestor é de preenchimento obritagório");
                //}
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
