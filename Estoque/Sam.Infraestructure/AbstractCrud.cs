using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Configuration;

namespace Sam.Infrastructure
{

    /// Repositório genérico para o Entity Framework com as operações mais 
    /// comuns de persistência, como inserção, atualização,
    /// exclusão e seleção.
    public abstract class AbstractCrud<E, C> : ICrudBase<E, C>, IDisposable
        where E : EntityObject
        where C : ObjectContext
    {
       
        /// <summary>
        ///  Contexto do Entity Framework
        /// </summary>
        private C context;
        public C Context
        {
            get { return context; }
            set { context = value; }
        }

        public int maximumRows = 50;

        
        private string _KeyProperty = "ID";
        public string KeyProperty
        {
            get
            {
                return _KeyProperty;
            }
            set
            {
                _KeyProperty = value;
            }
        }

        /// <summary>
        /// Lazy Loading, False retorna todos os objetos relacionados. Padrão True.
        /// </summary>
        private bool lazyLoadingEnabled = false;
        public bool LazyLoadingEnabled
        {
            get
            {
                return lazyLoadingEnabled;
            }
            set
            {
                lazyLoadingEnabled = value;
            }
        }

        private void LazyLoadingImpl()
        {
            if (LazyLoadingEnabled)
                context.ContextOptions.LazyLoadingEnabled = true;
            else
                context.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// //Retorna Total de registros encontrados quando é paginado
        /// </summary>
        public int TotalRegistros { get; set; }

        /// <summary>
        /// Retorna o nome do EntitySet do objeto persistente
        /// </summary>
        private string entitySetName;
        protected string EntitySetName
        {
            get
            {
                if (String.IsNullOrEmpty(entitySetName))
                {
                    entitySetName = GetEntitySetName(typeof(E).Name);
                }

                return entitySetName;
            }
        }

        /// <summary>
        /// Construtor padrão, sem argumentos. Obtém o contexto do 
        /// Entity Framework usando o ContextManager.
        /// </summary>
        public AbstractCrud()
        {
            // Obtém o contexto
            this.context = ContextManager.GetContext<C>();
        }

        /// <summary>
        /// Insere um novo objeto persistente
        /// </summary>
        public void Insert(E entity)
        {
            try
            {
                context.AddObject(EntitySetName, entity);                
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException); 
            }
        }

        /// <summary>
        /// Atualiza um objeto existente.
        /// </summary>
        public virtual void Update(E entity)
        {
            EntityKey key;
            object originalItem;

            if (entity.EntityKey == null)
                // Obtém o entity key do objeto que será atualizado
                key = Context.CreateEntityKey(EntitySetName, entity);
            else
                key = entity.EntityKey;
            try
            {
                // Obtém o objeto original
                if (Context.TryGetObjectByKey(key, out originalItem))
                {
                    if (originalItem is EntityObject &&
                        ((EntityObject)originalItem).EntityState != EntityState.Added)
                    {
                        // Autaliza o objeto

                        context.ApplyCurrentValues(key.EntitySetName, entity);
                        //context.ApplyPropertyChanges(key.EntitySetName, entity); //visual studio comentou como obsoleto e recomendou usar o de cima.
                    }
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException); 
            }
        }


        /// <summary>
        /// Exclui um objeto persistente
        /// </summary>
        public void Delete(E entity)
        {
            try
            {
                context.DeleteObject(entity);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Delete todas as Entidades relacionadas
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteRelatedEntries(E entity)
        {
            try
            {
                foreach (var relatedEntity in (((IEntityWithRelationships)entity).
            RelationshipManager.GetAllRelatedEnds().SelectMany(re =>
            re.CreateSourceQuery().OfType<EntityObject>()).Distinct()).ToArray())
                {
                    context.DeleteObject(relatedEntity);
                }//end foreach

                //Exclui o item
                context.DeleteObject(entity);
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Delete todas as Entidades relacionadas
        /// </summary>
        /// <param name="entity"></param>
        public void DeleteRelatedEntries(E entity, ObservableCollection<string>
                    keyListOfIgnoreEntites)
        {
            try
            {
                foreach (var relatedEntity in (((IEntityWithRelationships)entity).
                RelationshipManager.GetAllRelatedEnds().SelectMany(re =>
                re.CreateSourceQuery().OfType<EntityObject>()).Distinct()).ToArray())
                {
                    PropertyInfo propInfo = relatedEntity.GetType().GetProperty
                                (this._KeyProperty);
                    if (null != propInfo)
                    {
                        string value = (string)propInfo.GetValue(relatedEntity, null);
                        if (!string.IsNullOrEmpty(value) &&
                            keyListOfIgnoreEntites.Contains(value))
                        {
                            continue;
                        }//end if 
                    }//end if
                    context.DeleteObject(relatedEntity);
                }//end foreach

                //Exclui o item
                context.DeleteObject(entity);
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna um objeto que satisfaça a cláusula passada como parâmetro.
        /// </summary>
        public E SelectOne(Expression<Func<E, bool>> where)
        {
            try
            {
                LazyLoadingImpl();
                return context.CreateQuery<E>(EntitySetName).Where(where).AsEnumerable().FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Salva as alterações no banco de dados.
        /// </summary>
        public int SaveChanges(SaveOptions opt)
        {
            try
            {   
                return context.SaveChanges(opt);
                
            }
            catch (Exception ex)
            {
                //context.AcceptAllChanges();
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Salva as alterações no banco de dados.
        /// </summary>
        public int SaveChanges()
        {
            try
            {

                return context.SaveChanges();


            }
            catch (Exception ex)
            {
                //context.AcceptAllChanges();
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        public void AcceptAllChanges()
        {
            try
            {

                context.AcceptAllChanges();


            }
            catch (Exception ex)
            {
                //context.AcceptAllChanges();
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos persistentes.
        /// </summary>
        public List<E> SelectAll()
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).ToList();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna um IQueryable com todos os objetos
        /// </summary>
        public IQueryable<E> QueryAll()
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).AsQueryable<E>();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna um IQueryable filtrado
        /// </summary>
        public IQueryable<E> QueryAll(Expression<Func<E, bool>> where)
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).Where(where).AsQueryable<E>();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos usando paginação.
        /// </summary>
        public List<E> SelectAll(Expression<Func<E, object>> sortExpression, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();

                if (sortExpression != null)
                {
                    TotalRegistros = this.GetCount();
                    return context.CreateQuery<E>(EntitySetName).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();                }
                else
                {
                    return context.CreateQuery<E>(EntitySetName).ToList();
                }
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna um IQueryable com todos os objetos usando paginação
        /// </summary>
        public IQueryable<E> QueryAll(Expression<Func<E, object>> sortExpression, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();

                if (sortExpression != null)
                {
                    TotalRegistros = this.GetCount();
                    return context.CreateQuery<E>(EntitySetName).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows);
                }
                else
                    return context.CreateQuery<E>(EntitySetName).Skip<E>(startRowIndex).Take(maximumRows);
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos que satisfaçam a cláusula passada
        /// </summary>
        public List<E> SelectWhere(Expression<Func<E, bool>> where)
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).Where(where).ToList();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos que satisfaçam a cláusula passada, usando paginação
        /// </summary>
        public List<E> SelectWhere(Expression<Func<E, int>> sortExpression, bool desc, Expression<Func<E, bool>> where, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();

                if (sortExpression != null)
                {
                    TotalRegistros = this.GetCount(where);
                    if (desc)
                    {
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).ToList();
                    }
                    else
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).ToList();
                }
                else
                    return context.CreateQuery<E>(EntitySetName).Where(where).ToList();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos que satisfaçam a cláusula passada, usando paginação
        /// </summary>
        public List<E> SelectWhere(Expression<Func<E, DateTime>> sortExpression, bool desc, Expression<Func<E, bool>> where, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();

                if (sortExpression != null)
                {
                    TotalRegistros = this.GetCount(where);
                    if (desc)
                    {
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).ToList();
                    }
                    else
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).ToList();
                }
                else
                    return context.CreateQuery<E>(EntitySetName).Where(where).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os objetos usando paginação
        /// </summary>
        public List<E> SelectWhere(Expression<Func<E, string>> sortExpression, bool desc, Expression<Func<E, bool>> where, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();

                if (sortExpression != null)
                {
                    TotalRegistros = this.GetCount(where);
                    if (desc)
                    {
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderByDescending(sortExpression).ToList();
                    }
                    else
                        if (maximumRows > 0)
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                        else
                            return context.CreateQuery<E>(EntitySetName).Where(where).OrderBy(sortExpression).ToList();
                }
                else
                    return context.CreateQuery<E>(EntitySetName).Where(where).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        public List<E> Select(Expression<Func<E, string>> sortExpression, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();
                TotalRegistros = this.GetCount();
                return context.CreateQuery<E>(EntitySetName).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        public List<E> Select(Expression<Func<E, int>> sortExpression, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();
                TotalRegistros = this.GetCount();
                return context.CreateQuery<E>(EntitySetName).OrderBy(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        public List<E> Select(Expression<Func<E, DateTime?>> sortExpression, int startRowIndex)
        {
            try
            {
                LazyLoadingImpl();
                TotalRegistros = this.GetCount();
                return context.CreateQuery<E>(EntitySetName).OrderByDescending(sortExpression).Skip<E>(startRowIndex).Take(maximumRows).ToList();
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna o número de objetos
        /// </summary>
        public int GetCount()
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).Count();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna o número de objetos que satisfaçam a cláusula passada
        /// </summary>
        public int GetCount(Expression<Func<E, bool>> where)
        {
            try
            {
                LazyLoadingImpl();

                return context.CreateQuery<E>(EntitySetName).Where(where).Count();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Libera os recursos do Entity Framework.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (context != null)
                    context.Dispose();
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna o nome do EntitySet, possibilitando a criação de métodos genéricos.
        /// </summary>
        private string GetEntitySetName(string entityTypeName)
        {
            try
            {
                var container = context.MetadataWorkspace
                                                .GetEntityContainer(context.DefaultContainerName,
                                                                           DataSpace.CSpace);
                string entitySetName = (from meta in container.BaseEntitySets
                                        where meta.ElementType.Name == entityTypeName
                                        select meta.Name).FirstOrDefault();

                return entitySetName;
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }
        }
    }
}
