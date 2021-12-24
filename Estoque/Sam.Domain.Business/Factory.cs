using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Infrastructure;
using Sam.ServiceInfraestructure;
using System.Configuration;
namespace Sam.Domain.Business
{
    public class Factory<T> : BaseBusiness
    {
        /*Exemplo de compare contents*/
        T service;
        public T Service
        {
            get
            {
                if (service == null)
                    service = this.Create();

                return service;
            }
        }

        public T Create()
        {

            Type t = typeof(T);

            string variavel = ConfigurationManager.AppSettings["ACESSO_DADO"].ToString();
            object retorno = null;

            switch (variavel)
            {
                case "1":
                    if (t.Equals(typeof(IGrupoService)))
                        retorno = new GrupoInfraestructure();
                    else if (t.Equals(typeof(IClasseService)))
                        retorno = new ClasseInfraestructure();
                    else if (t.Equals(typeof(IAtividadeItemService)))
                        retorno = new AtividadeItemInfraestructure();
                    else if (t.Equals(typeof(IAtividadeNaturezaService)))
                        retorno = new AtividadeNaturezaInfraestructure();
                    else if (t.Equals(typeof(IContaAuxiliarService)))
                        retorno = new ContaAuxiliarInfraestructure();
                    else if (t.Equals(typeof(IMaterialService)))
                        retorno = new MaterialInfraestructure();
                    else if (t.Equals(typeof(INaturezaDespesaService)))
                        retorno = new NaturezaDespesaInfraestructure();
                   
                    break;
                default:
                    break;
            }

            return (T)retorno;

        }
    }
}
