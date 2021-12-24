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
    public class ItemMaterialBusiness : BaseBusiness, ICrudBaseBusiness<TB_ITEM_MATERIAL>
    {
        public List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();
        public List<GeralEnum.CargaErro> ListCargaErro
        {
            get { return listCargaErro; }
            set { listCargaErro = value; }
        }

        #region Métodos Customizados

        public IList<TB_ITEM_MATERIAL> BuscarItemMaterial(int startIndex, string palavraChave)
        {
            ItemMaterialInfrastructure infraItemMaterial = new ItemMaterialInfrastructure();
            
            var result = infraItemMaterial.BuscarItemMaterial(startIndex, palavraChave);
            TotalRegistros = infraItemMaterial.TotalRegistros;

            return result;
        }

        public bool InsertListControleImportacao(TB_CONTROLE entityList)
        {
            StringBuilder seq = new StringBuilder();

            try
            {
                bool isErro = false;
                ItemMaterialInfrastructure infraItemMaterial = new ItemMaterialInfrastructure();
                CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();
                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);

                    //var subItemMaterial = MontarListaSubItens(carga);

                    TB_ITEM_MATERIAL itemMaterial = new TB_ITEM_MATERIAL();

                    if (this.ConsistidoCargaErro)
                    {
                        //Realiza a importação dos registros
                        infraItemMaterial.Insert(itemMaterial);

                        //Atualiza a carga                        
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);
                    }
                    else
                    {
                        foreach (GeralEnum.CargaErro erroEnum in ListCargaErro)
                        {
                            TB_CARGA_ERRO cargaErro = new TB_CARGA_ERRO();
                            cargaErro.TB_CARGA_ID = carga.TB_CARGA_ID;
                            cargaErro.TB_ERRO_ID = (int)erroEnum;
                            infraCargaErro.Insert(cargaErro);

                            //Atualiza a carga
                            carga.TB_CARGA_VALIDO = false;
                            cargaBusiness.Update(carga);

                            //Se ocorreu ao menos 1 erro, retorna true;
                            isErro = true;
                        }
                    }
                }
                // Salva o contexto apenas se todos os registros foram inseridos com sucesso
                infraItemMaterial.SaveChanges();
                infraCargaErro.SaveChanges();
                return isErro;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public bool ConsistidoCargaErro
        {
            get
            {
                return this.ListCargaErro.Count == 0;
            }
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_ITEM_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public void Update(TB_ITEM_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public void Delete(TB_ITEM_MATERIAL entity)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne( a => a.TB_ITEM_MATERIAL_ID == entity.TB_ITEM_MATERIAL_ID);
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
        public void DeleteRelatedEntries(TB_ITEM_MATERIAL entity)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ITEM_MATERIAL_ID == entity.TB_ITEM_MATERIAL_ID);
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
        public void DeleteRelatedEntries(TB_ITEM_MATERIAL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ITEM_MATERIAL_ID == entity.TB_ITEM_MATERIAL_ID);
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
        public TB_ITEM_MATERIAL SelectOne(Expression<Func<TB_ITEM_MATERIAL, bool>> where)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public List<TB_ITEM_MATERIAL> SelectAll()
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public List<TB_ITEM_MATERIAL> SelectAll(Expression<Func<TB_ITEM_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public List<TB_ITEM_MATERIAL> SelectWhere(Expression<Func<TB_ITEM_MATERIAL, bool>> where)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public List<TB_ITEM_MATERIAL> SelectWhere(Expression<Func<TB_ITEM_MATERIAL, int >> sortExpression, bool desc, Expression<Func<TB_ITEM_MATERIAL, bool>> where, int startRowIndex)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public List<TB_ITEM_MATERIAL> SelectWhere(Expression<Func<TB_ITEM_MATERIAL, string>> sortExpression, bool desc, Expression<Func<TB_ITEM_MATERIAL, bool>> where, int startRowIndex)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public IQueryable<TB_ITEM_MATERIAL> QueryAll()
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public IQueryable<TB_ITEM_MATERIAL> QueryAll(Expression<Func<TB_ITEM_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public int GetCount(Expression<Func<TB_ITEM_MATERIAL, bool>> where)
        {
            try
            {
                ItemMaterialInfrastructure infraestrutura = new ItemMaterialInfrastructure();
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
        public void Consistir(TB_ITEM_MATERIAL entity)
        {
            try
            {
                if (entity.TB_ITEM_MATERIAL_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_ITEM_MATERIAL_DESCRICAO))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
                }
                if (entity.TB_MATERIAL_ID == null)
                {
                    throw new Exception("O campo Material é de preenchimento obritagório");
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
