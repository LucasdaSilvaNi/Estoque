using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Indicador Disponivel Business

        private IndicadorDisponivelEntity Indicador = new IndicadorDisponivelEntity();

        public IndicadorDisponivelEntity IndicadorDisponibilidade
        {
            get { return Indicador; }
            set { Indicador = value; }
        }

        public IList<IndicadorDisponivelEntity> ListarIndicadorDisponivelTodosCod()
        {
            IList<IndicadorDisponivelEntity> retorno = this.Service<IIndicadorDisponivelService>().Listar();
            return retorno;
        }

    }
        #endregion
}
