using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Common.Util;

namespace Sam.Business
{
    public class NaturezaDespesaBusiness : BaseBusiness, ICrudBaseBusiness<TB_NATUREZA_DESPESA>
    {
        public List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();
        public List<GeralEnum.CargaErro> ListCargaErro
        {
            get { return listCargaErro; }
            set { listCargaErro = value; }
        }

        #region Métodos Customizados

        internal bool InsertListControleImportacao(TB_CONTROLE entityList)
        {
            throw new NotImplementedException();
        }

        public bool ExisteItemNatureza(int idNatureza, int idItem, string indDisponivel)
        {
            //Se for S, verifica se está associado ao Item
            //Se for diferente de S, não precisa validar a associação ao Item
            if (indDisponivel == "S")
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
                return infraestrutura.ExisteItemNatureza(idNatureza, idItem);
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_NATUREZA_DESPESA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public void Update(TB_NATUREZA_DESPESA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public void Delete(TB_NATUREZA_DESPESA entity)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne( a => a.TB_NATUREZA_DESPESA_ID == entity.TB_NATUREZA_DESPESA_ID);
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
        public void DeleteRelatedEntries(TB_NATUREZA_DESPESA entity)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_NATUREZA_DESPESA_ID == entity.TB_NATUREZA_DESPESA_ID);
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
        public void DeleteRelatedEntries(TB_NATUREZA_DESPESA entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_NATUREZA_DESPESA_ID == entity.TB_NATUREZA_DESPESA_ID);
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
        public TB_NATUREZA_DESPESA SelectOne(Expression<Func<TB_NATUREZA_DESPESA, bool>> where)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public List<TB_NATUREZA_DESPESA> SelectAll()
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
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
        public List<TB_NATUREZA_DESPESA> SelectAll(Expression<Func<TB_NATUREZA_DESPESA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
                return infraestrutura.SelectAll(sortExpression, startRowIndex);
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
        public List<TB_NATUREZA_DESPESA> SelectWhere(Expression<Func<TB_NATUREZA_DESPESA, bool>> where)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public List<TB_NATUREZA_DESPESA> SelectWhere(Expression<Func<TB_NATUREZA_DESPESA, int >> sortExpression, bool desc, Expression<Func<TB_NATUREZA_DESPESA, bool>> where, int startRowIndex)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public List<TB_NATUREZA_DESPESA> SelectWhere(Expression<Func<TB_NATUREZA_DESPESA, string>> sortExpression, bool desc, Expression<Func<TB_NATUREZA_DESPESA, bool>> where, int startRowIndex)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public IQueryable<TB_NATUREZA_DESPESA> QueryAll()
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();                
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
        public IQueryable<TB_NATUREZA_DESPESA> QueryAll(Expression<Func<TB_NATUREZA_DESPESA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public int GetCount(Expression<Func<TB_NATUREZA_DESPESA, bool>> where)
        {
            try
            {
                NaturezaDespesaInfrastructure infraestrutura = new NaturezaDespesaInfrastructure();
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
        public void Consistir(TB_NATUREZA_DESPESA entity)
        {
            try
            {
                if (entity.TB_NATUREZA_DESPESA_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_NATUREZA_DESPESA_DESCRICAO))
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
