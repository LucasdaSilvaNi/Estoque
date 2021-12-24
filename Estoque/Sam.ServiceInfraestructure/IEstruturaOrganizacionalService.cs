using System;
using System.Collections.Generic;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;

namespace Sam.ServiceInfraestructure
{
    public interface IEstruturaOrganizacionalService
    {
        int SkipRegistros
        {
            get;
            set;
        }

        int TotalRegistros
        {
            get;
        }

        #region UO

        UOEntity UO { get; set; }

        bool ExisteUOCodigoInformado();
        bool PodeExcluirUO();
        void SalvarUO();
        void ExcluirUO();
        IList<UOEntity> ListarUO(int OrgaoId);

        #endregion

        #region UGE

        UGEEntity UGE { get; set; }

        bool ExisteUGECodigoInformado();
        bool PodeExcluirUGE();
        void SalvarUGE();
        void ExcluirUGE();
        IList<UGEEntity> ListarUGE(int OrgaoId, int UoId);
        IList<TipoUGEEntity> ListarTipoUGE();

        #endregion



    }
}
