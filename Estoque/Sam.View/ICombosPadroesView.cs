using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface ICombosPadroesView : ICrudView
    {
        void PrepararVisaoDeCombosPorPerfil(int perfilId);
        
        bool CascatearDDLOrgao { get; set; }
        bool CascatearDDLUO { get; set; }
        bool CascatearDDLUGE { get; set; }
        bool CascatearDDLUA { get; set; }
        bool CascatearDDLAlmoxarifado { get; set; }
        bool PreservarComboboxValues { get; set; }

        bool BloqueiaListaOrgao { set; }
        bool BloqueiaListaUO { set; }
        bool BloqueiaListaUGE { set; }
        bool BloqueiaListaUA { set; }
        bool BloqueiaListaDivisao { set; }

        int OrgaoId { get; set; }
        int UOId { get; set; }
        int UGEId { get; set; }
        int UAId { get; set; }
        int DivisaoId { get; set; }
        //int? PTResId { get; set; }
    }
}
