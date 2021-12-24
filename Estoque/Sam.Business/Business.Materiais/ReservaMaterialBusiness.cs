using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public class ReservaMaterialBusiness : BaseBusiness, ICrudBaseBusiness<TB_RESERVA_MATERIAL>
    {
        #region Métodos Customizados

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_RESERVA_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                    infraestrutura.Insert(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
            }
        }

        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Update(TB_RESERVA_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                    infraestrutura.Update(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
            }
        }

        /// <summary>
        /// Exclui o objeto não relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Delete(TB_RESERVA_MATERIAL entity)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne( a => a.TB_RESERVA_MATERIAL_ID == entity.TB_RESERVA_MATERIAL_ID);
                infraestrutura.Delete(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
            }
        }

        /// <summary>
        /// Exclui todos os objetos relecionados
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void DeleteRelatedEntries(TB_RESERVA_MATERIAL entity)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_RESERVA_MATERIAL_ID == entity.TB_RESERVA_MATERIAL_ID);
                infraestrutura.DeleteRelatedEntries(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
            }
        }

        /// <summary>
        /// Eclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_RESERVA_MATERIAL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_RESERVA_MATERIAL_ID == entity.TB_RESERVA_MATERIAL_ID);
                infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
            }
        }

        /// <summary>
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        public TB_RESERVA_MATERIAL SelectOne(Expression<Func<TB_RESERVA_MATERIAL, bool>> where)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        public List<TB_RESERVA_MATERIAL> SelectAll()
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.SelectAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
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
        public List<TB_RESERVA_MATERIAL> SelectAll(Expression<Func<TB_RESERVA_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                var result = infraestrutura.SelectAll(sortExpression, maximumRows, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_RESERVA_MATERIAL> SelectWhere(Expression<Func<TB_RESERVA_MATERIAL, bool>> where)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
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
        public List<TB_RESERVA_MATERIAL> SelectWhere(Expression<Func<TB_RESERVA_MATERIAL, int >> sortExpression, bool desc, Expression<Func<TB_RESERVA_MATERIAL, bool>> where, int maximumRows, int startRowIndex)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, maximumRows, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
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
        public List<TB_RESERVA_MATERIAL> SelectWhere(Expression<Func<TB_RESERVA_MATERIAL, string>> sortExpression, bool desc, Expression<Func<TB_RESERVA_MATERIAL, bool>> where, int maximumRows, int startRowIndex)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                var result =  infraestrutura.SelectWhere(sortExpression, desc, where, maximumRows, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_RESERVA_MATERIAL> QueryAll()
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.QueryAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);                
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
        public IQueryable<TB_RESERVA_MATERIAL> QueryAll(Expression<Func<TB_RESERVA_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                var result = infraestrutura.QueryAll(sortExpression, maximumRows, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
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
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.GetCount();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }
        
        /// <summary>
        /// Retorna o total de registros na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount(Expression<Func<TB_RESERVA_MATERIAL, bool>> where)
        {
            try
            {
                ReservaMaterialInfrastructure infraestrutura = new ReservaMaterialInfrastructure();
                return infraestrutura.GetCount(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region Métodos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(TB_RESERVA_MATERIAL entity)
        {
            try
            {
   
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                this.ListaErro.Add(ErroSistema + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        #endregion
    }
}
