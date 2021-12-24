using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class SubTipoMovimentoEntity : BaseEntity
    {
        public SubTipoMovimentoEntity() { }

        public SubTipoMovimentoEntity(int _Id)
        {
            this.Id = _Id;
            base.Id = _Id;
        }

        public bool Ativo { get; set; }
        public int? TipoMovimentoId { get; set; }
        public TipoMovimentoEntity TipoMovimento { get; set; }
        //public EventoSiafemEntity EventoSiafem { get; set; }
        public List<EventoSiafemEntity> ListEventoSiafem { get; set; }
        public string TipoMaterial { get; set; }
    }
}
