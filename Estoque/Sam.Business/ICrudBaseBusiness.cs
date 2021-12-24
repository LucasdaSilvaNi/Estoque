using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Sam.Business
{
    public interface ICrudBaseBusiness<E>
    {
        #region Generic Methods

        /// Inserção
        void Insert(E entity);

        /// Atualização
        void Update(E entity);

        /// Exclusão
        void Delete(E entity);

        void DeleteRelatedEntries(E entity);

        void DeleteRelatedEntries (E entity, ObservableCollection<string> keyListOfIgnoreEntites);

        /// Retorna o objeto que satisfaça a cláusula passada como argumento (cláusula WHERE)
        E SelectOne(Expression<Func<E, bool>> where);

        /// Retorna todos os objetos de um tipo
        List<E> SelectAll();

        /// Retorna os objetos usando paginação
        List<E> SelectAll(Expression<Func<E, object>> sortExpression, int maximumRows, int startRowIndex);

        /// Retorna todos os objetos que satisfaçam a cláusula passada
        List<E> SelectWhere(Expression<Func<E, bool>> where);

        /// Retorna os objetos que satisfaçam a cláusula passada, usando paginação
        List<E> SelectWhere(Expression<Func<E, int>> sortExpression, bool desc, Expression<Func<E, bool>> where, int startRowIndex);

        /// Retorna os objetos que satisfaçam a cláusula passada, usando paginação
        List<E> SelectWhere(Expression<Func<E, string>> sortExpression, bool desc, Expression<Func<E, bool>> where, int startRowIndex);

        /// Retorna um objeto IQueryable, possibilitando formar queries usando expressões Lambda
        IQueryable<E> QueryAll();

        /// Retorna um IQueryable com os objetos usando paginação
        IQueryable<E> QueryAll(Expression<Func<E, object>> sortExpression, int maximumRows, int startRowIndex);

        /// Retorna a quantidade de objetos persistentes.
        int GetCount();

        /// Retorna a quantidade de objetos persistentes que satisfaçam a cláusula WHERE
        int GetCount(Expression<Func<E, bool>> where);

        #endregion

        void Consistir(E entity);
    }

}
