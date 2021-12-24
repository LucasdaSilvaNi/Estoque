using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class TipoMovimentoEntity : BaseEntity
    {
        public TipoMovimentoEntity() { }

        public TipoMovimentoEntity(int _Id)
        {
            this.Id = _Id;
            base.Id = _Id;
        }

        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public int? Aux_ID { get; set; }
        public int? AgrupamentoId { get; set; }
        public int? SubTipoMovimentoId { get; set; }
        public bool? AtivoNL { get; set; }
        public bool? Ativo { get; set; }

        public SubTipoMovimentoEntity SubTipoMovimentoItem { get; set; }
        public IList<SubTipoMovimentoEntity> SubTipoMovimento { get; set; }
        public TipoMovimentoAgrupamentoEntity TipoAgrupamento { get; set; }
    }
}
