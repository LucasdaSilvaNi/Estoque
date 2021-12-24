using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Entity;


namespace Sam.Business
{
    public class MaterialApoioBusiness : BaseBusiness//, ICrudBaseBusiness<TB_MATERIAL_APOIO>
    {
        #region Métodos Customizados

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        //public void Insert(TB_MATERIAL_APOIO entity)
        //{
        //    try
        //    {
        //        Consistir(entity);
        //        if (base.Consistido)
        //        {
        //            ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //            infraestrutura.Insert(entity);
        //            infraestrutura.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}

        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        //public void Update(TB_MATERIAL_APOIO entity)
        //{
        //    try
        //    {
        //        Consistir(entity);
        //        if (base.Consistido)
        //        {
        //            ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //            infraestrutura.Update(entity);
        //            infraestrutura.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}

        /// <summary>
        /// Exclui o objeto não relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        //public void Delete(TB_MATERIAL_APOIO entity)
        //{
        //    try
        //    {
        //        MaterialApoioInfrastructure infraestrutura = new MaterialApoioInfrastructure();

        //        //Atualiza o objeto
        //        entity = infraestrutura.SelectOne( a => a.TB_MATERIAL_APOIO_ID == entity.TB_MATERIAL_APOIO_ID);
        //        infraestrutura.Delete(entity);
        //        infraestrutura.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}

        /// <summary>
        /// Exclui todos os objetos relecionados
        /// </summary>
        /// <param name="entity">Objeto</param>
        //public void DeleteRelatedEntries(TB_MATERIAL_APOIO entity)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();

        //        //Atualiza o objeto
        //        entity = infraestrutura.SelectOne(a => a.TB_MATERIAL_APOIO_ID == entity.TB_MATERIAL_APOIO_ID);
        //        infraestrutura.DeleteRelatedEntries(entity);
        //        infraestrutura.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}

        /// <summary>
        /// Eclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        //public void DeleteRelatedEntries(TB_MATERIAL_APOIO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();

        //        //Atualiza o objeto
        //        entity = infraestrutura.SelectOne(a => a.TB_MATERIAL_APOIO_ID == entity.TB_MATERIAL_APOIO_ID);
        //        infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
        //        infraestrutura.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //    }
        //}

        /// <summary>
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        //public TB_MATERIAL_APOIO SelectOne(Expression<Func<TB_MATERIAL_APOIO, bool>> where)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.SelectOne(where);
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        //public List<TB_MATERIAL_APOIO> SelectAll()
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.SelectAll();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna todos os objetos encontrados, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        //public List<TB_MATERIAL_APOIO> SelectAll(Expression<Func<TB_MATERIAL_APOIO, object>> sortExpression, int maximumRows, int startRowIndex)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        var result = infraestrutura.SelectAll(sortExpression, startRowIndex);
        //        this.TotalRegistros = infraestrutura.TotalRegistros;

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        //public List<TB_MATERIAL_APOIO> SelectWhere(Expression<Func<TB_MATERIAL_APOIO, bool>> where)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.SelectWhere(where);
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        //public List<TB_MATERIAL_APOIO> SelectWhere(Expression<Func<TB_MATERIAL_APOIO, int >> sortExpression, bool desc, Expression<Func<TB_MATERIAL_APOIO, bool>> where, int startRowIndex)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
        //        this.TotalRegistros = infraestrutura.TotalRegistros;

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos STRING</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        //public List<TB_MATERIAL_APOIO> SelectWhere(Expression<Func<TB_MATERIAL_APOIO, string>> sortExpression, bool desc, Expression<Func<TB_MATERIAL_APOIO, bool>> where, int startRowIndex)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        var result =  infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
        //        this.TotalRegistros = infraestrutura.TotalRegistros;

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        //public IQueryable<TB_MATERIAL_APOIO> QueryAll()
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.QueryAll();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);                
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="desc">Ordem decrescente</param>        
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        //public IQueryable<TB_MATERIAL_APOIO> QueryAll(Expression<Func<TB_MATERIAL_APOIO, object>> sortExpression, int maximumRows, int startRowIndex)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        var result = infraestrutura.QueryAll(sortExpression, startRowIndex);
        //        this.TotalRegistros = infraestrutura.TotalRegistros;

        //        return result;
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        /// <summary>
        /// Retorna o total de registros encontrados na tabela
        /// </summary>
        /// <returns>Total de registros inteiro</returns>
        //public int GetCount()
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.GetCount();
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}
        
        /// <summary>
        /// Retorna o total de registros na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        //public int GetCount(Expression<Func<TB_MATERIAL_APOIO, bool>> where)
        //{
        //    try
        //    {
        //        ClasseInfrastructure infraestrutura = new ClasseInfrastructure();
        //        return infraestrutura.GetCount(where);
        //    }
        //    catch (Exception e)
        //    {
        //        base.GravarLogErro(e);
        //        throw new Exception(e.Message, e.InnerException);
        //        throw new Exception(e.Message, e);
        //    }
        //}

        #endregion

        #region Métodos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(MaterialApoioEntity entity)
        {
            try
            {
                if (entity.Codigo == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }

                if (string.IsNullOrEmpty(entity.Descricao))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
                }

                //if (string.IsNullOrEmpty(entity.DescricaoDetalhada))
                //{
                //    throw new Exception("O campo Descrição Detalhada é de preenchimento obritagório");
                //}
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public IList<MaterialApoioEntity> ListarRecursosPorPerfil(int? Perfil_ID)
        {
            IList<MaterialApoioEntity>  lstRetorno     = null;
            MaterialApoioInfrastructure infraEstrutura = new MaterialApoioInfrastructure();

            lstRetorno = infraEstrutura.ListarRecursosPorPerfil(Perfil_ID);

            return lstRetorno;
        }

        public MaterialApoioEntity ObterDadosMaterialApoio(int MaterialApoio_ID)
        {
            MaterialApoioEntity         objRetorno     = null;
            MaterialApoioInfrastructure infraEstrutura = new MaterialApoioInfrastructure();

            objRetorno = infraEstrutura.ObterDadosMaterialApoio(MaterialApoio_ID);

            return objRetorno;
        }
        #endregion
    }
}
