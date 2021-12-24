using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class TipoMovimentoAgrupamentoEntity : BaseEntity
    {
        public TipoMovimentoAgrupamentoEntity() { }

        public TipoMovimentoAgrupamentoEntity(int _Id)
        {
            this.Id = _Id;
        }

        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}
