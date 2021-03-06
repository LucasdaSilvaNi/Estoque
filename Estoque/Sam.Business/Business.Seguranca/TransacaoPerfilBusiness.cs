using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public class TransacaoPerfilBusiness : BaseBusiness, ICrudBaseBusiness<TB_TRANSACAO_PERFIL>
    {
        #region Métodos Customizados

        public List<TB_TRANSACAO_PERFIL> ListarTransacaoPerfil(int moduloId, int perfilId, bool ativo)
        {
            //Exemplo para sort
            Expression<Func<TB_TRANSACAO_PERFIL, string>> sort = a => a.TB_TRANSACAO.TB_TRANSACAO_DESCRICAO;

            //Exemplo para Where
            Expression<Func<TB_TRANSACAO_PERFIL, bool>> where = a =>
                a.TB_PERFIL_ID == perfilId
                && a.TB_TRANSACAO.TB_MODULO_ID == moduloId
                && a.TB_TRANSACAO_ATIVO == ativo;

            TransacaoPerfilInfrastructure persistencia = new TransacaoPerfilInfrastructure();

            persistencia.LazyLoadingEnabled = true;
            var result = persistencia.SelectWhere(sort, false, where, this.SkipRegistros);

            this.TotalRegistros = persistencia.TotalRegistros;

            foreach (var t in result)
            {
                string a = t.TB_PERFIL.TB_PERFIL_DESCRICAO;
            }

            return result;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_TRANSACAO_PERFIL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public void Update(TB_TRANSACAO_PERFIL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public void Delete(TB_TRANSACAO_PERFIL entity)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_TRANSACAO_PERFIL_ID == entity.TB_TRANSACAO_PERFIL_ID);
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
        public void DeleteRelatedEntries(TB_TRANSACAO_PERFIL entity)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_TRANSACAO_PERFIL_ID == entity.TB_TRANSACAO_PERFIL_ID);
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
        public void DeleteRelatedEntries(TB_TRANSACAO_PERFIL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_TRANSACAO_PERFIL_ID == entity.TB_TRANSACAO_PERFIL_ID);
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
        public TB_TRANSACAO_PERFIL SelectOne(Expression<Func<TB_TRANSACAO_PERFIL, bool>> where)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public List<TB_TRANSACAO_PERFIL> SelectAll()
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public List<TB_TRANSACAO_PERFIL> SelectAll(Expression<Func<TB_TRANSACAO_PERFIL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public List<TB_TRANSACAO_PERFIL> SelectWhere(Expression<Func<TB_TRANSACAO_PERFIL, bool>> where)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public List<TB_TRANSACAO_PERFIL> SelectWhere(Expression<Func<TB_TRANSACAO_PERFIL, int>> sortExpression, bool desc, Expression<Func<TB_TRANSACAO_PERFIL, bool>> where, int startRowIndex)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public List<TB_TRANSACAO_PERFIL> SelectWhere(Expression<Func<TB_TRANSACAO_PERFIL, string>> sortExpression, bool desc, Expression<Func<TB_TRANSACAO_PERFIL, bool>> where, int startRowIndex)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public IQueryable<TB_TRANSACAO_PERFIL> QueryAll()
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public IQueryable<TB_TRANSACAO_PERFIL> QueryAll(Expression<Func<TB_TRANSACAO_PERFIL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public int GetCount(Expression<Func<TB_TRANSACAO_PERFIL, bool>> where)
        {
            try
            {
                TransacaoPerfilInfrastructure infraestrutura = new TransacaoPerfilInfrastructure();
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
        public void Consistir(TB_TRANSACAO_PERFIL entity)
        {
            try
            {
                if (entity.TB_TRANSACAO_ID == null)
                {
                    throw new Exception("O campo Transação é de preenchimento obritagório");
                }
                if (entity.TB_PERFIL_ID == null)
                {
                    throw new Exception("O campo Perfil é de preenchimento obritagório");
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
