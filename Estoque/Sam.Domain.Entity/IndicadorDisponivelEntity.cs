using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class IndicadorDisponivelEntity : BaseEntity
    {
        public IndicadorDisponivelEntity() { }
        public IndicadorDisponivelEntity(int _id) { base.Id = _id; }

        public long Codigo { get; set; }

        public string Descricao { get; set; }        
    }
}
