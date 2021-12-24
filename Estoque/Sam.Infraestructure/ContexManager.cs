using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Web;

namespace Sam.Infrastructure
{
    public sealed class ContextManager
    {
        /// Construtor privado, não será possível instanciar a classe usando new
        private ContextManager()
        {
        }

        private static T GetNewContex<T>()
            where T : ObjectContext
        {
            // Instancia o contexto através de Reflection
            T ctx = typeof(T).GetConstructor(System.Type.EmptyTypes)
                             .Invoke(System.Type.EmptyTypes) as T;

            return ctx;
        }

        /// Obtém o contexto do Entity Framework usando generics.
        public static T GetContext<T>()
            where T : ObjectContext
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    string ocKey = "ocm_" + HttpContext.Current.GetHashCode().ToString("x");

                    //Se o Contexto não existir, cria um novo.
                    if (!HttpContext.Current.Items.Contains(ocKey))
                    {
                        return GetNewContex<T>();
                    }
                    else
                    {
                        //verifica se o tipo do contexto existente é o mesmo do tipo generico.
                        if (typeof(T).Equals(HttpContext.Current.Items[ocKey].GetType()))
                        {
                            return HttpContext.Current.Items[ocKey] as T;
                        }
                        else
                        {
                            return GetNewContex<T>();
                        }
                    }
                }
                else
                {
                    // Caso a aplicação não seja web, instancia e retorna o contexto.
                    return GetNewContex<T>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
    }

}
