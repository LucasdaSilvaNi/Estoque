using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class EmpenhoLicitacaoEntity : BaseEntity
    {
        public string Descricao { get; set; }

        public EmpenhoLicitacaoEntity() { }

        public EmpenhoLicitacaoEntity(int? _id)
        {
            base.Id = _id;
        }

    }
}
