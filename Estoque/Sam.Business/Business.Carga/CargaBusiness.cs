using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public class CargaBusiness : BaseBusiness, ICrudBaseBusiness<TB_CARGA>
    {
        #region Métodos Customizados



        public IEnumerable<TB_CARGA> RetornarCargaErros(int controleId)
        {
              bool LazyLoadingEnabled = true;
                var controle = new ControleBusiness().QueryAll(a => a.TB_CONTROLE_ID == controleId, LazyLoadingEnabled).FirstOrDefault();

                return new CargaInfrastructure().RetornarCargaErros(controle);
        }

        public List<TB_CARGA> RetornarCarga(int controleId)
        {
            bool LazyLoadingEnabled = true;
            var controle = new ControleBusiness().QueryAll(a => a.TB_CONTROLE_ID == controleId, LazyLoadingEnabled).FirstOrDefault();
            return new CargaInfrastructure().RetornarCarga(controle);
        }

        public IEnumerable<TB_CARGA_ERRO> RetornaCargaErroDescricao(IEnumerable<TB_CARGA> cargaList)
        {
            return new CargaInfrastructure().RetornaCargaErroDescricao(cargaList);
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_CARGA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    CargaInfrastructure infraestrutura = new CargaInfrastructure();
                    infraestrutura.Insert(entity);                    
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
        public void Update(TB_CARGA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public void Delete(TB_CARGA entity)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CARGA_ID == entity.TB_CARGA_ID);
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
        public void DeleteRelatedEntries(TB_CARGA entity)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CARGA_ID == entity.TB_CARGA_ID);
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
        public void DeleteRelatedEntries(TB_CARGA entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_CARGA_ID == entity.TB_CARGA_ID);
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
        public TB_CARGA SelectOne(Expression<Func<TB_CARGA, bool>> where)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public List<TB_CARGA> SelectAll()
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public List<TB_CARGA> SelectAll(Expression<Func<TB_CARGA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public List<TB_CARGA> SelectWhere(Expression<Func<TB_CARGA, bool>> where)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public List<TB_CARGA> SelectWhere(Expression<Func<TB_CARGA, int>> sortExpression, bool desc, Expression<Func<TB_CARGA, bool>> where, int startRowIndex)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public List<TB_CARGA> SelectWhere(Expression<Func<TB_CARGA, string>> sortExpression, bool desc, Expression<Func<TB_CARGA, bool>> where, int startRowIndex)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public IQueryable<TB_CARGA> QueryAll()
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public IQueryable<TB_CARGA> QueryAll(Expression<Func<TB_CARGA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public int GetCount(Expression<Func<TB_CARGA, bool>> where)
        {
            try
            {
                CargaInfrastructure infraestrutura = new CargaInfrastructure();
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
        public void Consistir(TB_CARGA entity)
        {
            try
            {
                //if (entity.TB_CARGA_CODIGO == null)
                //{
                //    throw new Exception("O campo Código é de preenchimento obritagório");
                //}
                //if (string.IsNullOrEmpty(entity.TB_CARGA_DESCRICAO))
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
