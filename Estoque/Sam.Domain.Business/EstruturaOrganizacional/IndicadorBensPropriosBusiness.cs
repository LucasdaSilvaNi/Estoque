using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region Indicador de Bens Próprios

        private IndicadorBemProprioEntity indicadorBemProprio = new IndicadorBemProprioEntity();

        public IndicadorBemProprioEntity IndicadorBemProprio
        {
            get { return indicadorBemProprio; }
            set { indicadorBemProprio = value; }
        }

        public IList<IndicadorBemProprioEntity> ListarIndicadorBemProprio()
        {
            IList<IndicadorBemProprioEntity> retorno = this.Service<IIndicadorBemProprioService>().Listar();
            return retorno;
        }

        #endregion
    }
}
