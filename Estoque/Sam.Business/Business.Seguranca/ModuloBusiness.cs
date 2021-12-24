using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public class ModuloBusiness : BaseBusiness, ICrudBaseBusiness<TB_MODULO>
    {
        #region Métodos Customizados

        /// <summary>
        /// Grava o objeto no banco de dados, faz uma consulta, caso exista
        /// </summary>
        /// <param name="entity">Objeto a ser inserido</param>        
        public void Save(TB_MODULO entity)
        {
            ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    Expression<Func<TB_MODULO, bool>> where = a => a.TB_MODULO_ID == entity.TB_MODULO_ID;

                    int result = infraestrutura.GetCount(where);

                    if (result == 0)
                    {
                        infraestrutura.Insert(entity);
                    }
                    else if (result == 1)
                    {
                        infraestrutura.Update(entity);
                    }
                    else
                    {
                        throw new Exception("Existem mais de um registro com a mesma chave no banco de dados.");
                    }

                    infraestrutura.Context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }        

        public List<TB_MODULO> ListarModulo()
        {
            //Exemplo para sort
            Expression<Func<TB_MODULO, string>> sort = a => a.TB_MODULO_DESCRICAO;

            //Exemplo para Where
            Expression<Func<TB_MODULO, bool>> where = a => a.TB_MODULO_ID != 0;

            ModuloInfrastructure persistencia = new ModuloInfrastructure();
            persistencia.LazyLoadingEnabled = true;
            var result = persistencia.SelectWhere(sort, false, where, this.SkipRegistros);
            this.TotalRegistros = persistencia.TotalRegistros;

            return result;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_MODULO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public void Update(TB_MODULO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public void Delete(TB_MODULO entity)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MODULO_ID == entity.TB_MODULO_ID);
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
        public void DeleteRelatedEntries(TB_MODULO entity)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MODULO_ID == entity.TB_MODULO_ID);
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
        public void DeleteRelatedEntries(TB_MODULO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MODULO_ID == entity.TB_MODULO_ID);
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
        public TB_MODULO SelectOne(Expression<Func<TB_MODULO, bool>> where)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public List<TB_MODULO> SelectAll()
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public List<TB_MODULO> SelectAll(Expression<Func<TB_MODULO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public List<TB_MODULO> SelectWhere(Expression<Func<TB_MODULO, bool>> where)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public List<TB_MODULO> SelectWhere(Expression<Func<TB_MODULO, int>> sortExpression, bool desc, Expression<Func<TB_MODULO, bool>> where, int startRowIndex)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public List<TB_MODULO> SelectWhere(Expression<Func<TB_MODULO, string>> sortExpression, bool desc, Expression<Func<TB_MODULO, bool>> where, int startRowIndex)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public IQueryable<TB_MODULO> QueryAll()
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public IQueryable<TB_MODULO> QueryAll(Expression<Func<TB_MODULO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public int GetCount(Expression<Func<TB_MODULO, bool>> where)
        {
            try
            {
                ModuloInfrastructure infraestrutura = new ModuloInfrastructure();
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
        public void Consistir(TB_MODULO entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.TB_MODULO_DESCRICAO))
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
