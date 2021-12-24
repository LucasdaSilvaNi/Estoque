using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public class NotificacaoBusiness : BaseBusiness, ICrudBaseBusiness<TB_NOTIFICACAO>
    {

        #region Métodos Customizados

        public IList<TB_NOTIFICACAO> ListarNotificacao(Expression<Func<TB_NOTIFICACAO, bool>> where)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();                

                var result = infraestrutura.ListarNotificacao(where, this.SkipRegistros);
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
        /// Desativa as notificacoes anteriores quando inserido ou modificado a notificação
        /// </summary>
        /// <param name="infraestrutura">Infraestrutura do escopo atual, para continuar com a transação.</param>
        /// <param name="entity">Notificação que será desativada</param>
        private void DesativaNotificacoesAnteriores(NotificacaoInfrastructure infraestrutura, TB_NOTIFICACAO entity)
        {
            var perfilAntigos = infraestrutura.SelectWhere(a => (a.TB_PERFIL_ID == entity.TB_PERFIL_ID || (a.TB_PERFIL_ID == null && entity.TB_PERFIL_ID == null)) && a.TB_NOTIFICACAO_IND_ATIVO == true);

            foreach (var p in perfilAntigos)
            {
                p.TB_NOTIFICACAO_IND_ATIVO = false;
                infraestrutura.Update(p);
            }
        }


        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_NOTIFICACAO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
                    DesativaNotificacoesAnteriores(infraestrutura, entity);

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
        public void Update(TB_NOTIFICACAO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
                    DesativaNotificacoesAnteriores(infraestrutura, entity);

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
        public void Delete(TB_NOTIFICACAO entity)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_NOTIFICACAO_ID == entity.TB_NOTIFICACAO_ID);
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
        public void DeleteRelatedEntries(TB_NOTIFICACAO entity)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_NOTIFICACAO_ID == entity.TB_NOTIFICACAO_ID);
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
        public void DeleteRelatedEntries(TB_NOTIFICACAO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_NOTIFICACAO_ID == entity.TB_NOTIFICACAO_ID);
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
        public TB_NOTIFICACAO SelectOne(Expression<Func<TB_NOTIFICACAO, bool>> where)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public List<TB_NOTIFICACAO> SelectAll()
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public List<TB_NOTIFICACAO> SelectAll(Expression<Func<TB_NOTIFICACAO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public List<TB_NOTIFICACAO> SelectWhere(Expression<Func<TB_NOTIFICACAO, bool>> where)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public List<TB_NOTIFICACAO> SelectWhere(Expression<Func<TB_NOTIFICACAO, int>> sortExpression, bool desc, Expression<Func<TB_NOTIFICACAO, bool>> where, int startRowIndex)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public List<TB_NOTIFICACAO> SelectWhere(Expression<Func<TB_NOTIFICACAO, string>> sortExpression, bool desc, Expression<Func<TB_NOTIFICACAO, bool>> where, int startRowIndex)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public IQueryable<TB_NOTIFICACAO> QueryAll()
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public IQueryable<TB_NOTIFICACAO> QueryAll(Expression<Func<TB_NOTIFICACAO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public int GetCount(Expression<Func<TB_NOTIFICACAO, bool>> where)
        {
            try
            {
                NotificacaoInfrastructure infraestrutura = new NotificacaoInfrastructure();
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
        public void Consistir(TB_NOTIFICACAO entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.TB_NOTIFICACAO_TITULO))
                {
                    throw new Exception("O campo Titulo é de preenchimento obritagório");
                }

                if (string.IsNullOrEmpty(entity.TB_NOTIFICACAO_MENSAGEM))
                {
                    throw new Exception("O campo Mensagem é de preenchimento obritagório");
                }

                if (entity.TB_NOTIFICACAO_DATA == DateTime.MinValue)
                {
                    throw new Exception("O campo Data é de preenchimento obritagório");
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
