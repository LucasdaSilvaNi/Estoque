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
        #region Indicador de Atividade

        private IndicadorAtividadeEntity indicadorAtividade = new IndicadorAtividadeEntity();

        public IndicadorAtividadeEntity IndicadorAtividade
        {
            get { return indicadorAtividade; }
            set { indicadorAtividade = value; }
        }

        public IList<IndicadorAtividadeEntity> ListarIndicadorAtividade()
        {
            IList<IndicadorAtividadeEntity> retorno = this.Service<IIndicadorAtividadeService>().Listar();
            return retorno;
        }

        #endregion
    }
}
