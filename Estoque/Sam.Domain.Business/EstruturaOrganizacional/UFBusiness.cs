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
        #region UF

        private UFEntity uf = new UFEntity();

        public UFEntity Uf
        {
            get { return uf; }
            set { uf = value; }
        }

        public IList<UFEntity> ListarUF()
        {
            IList<UFEntity> retorno = this.Service<IUFService>().Listar();
            return retorno;
        }

        #endregion
    }
}
