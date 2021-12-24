using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class MovimentoItemEstEntity : BaseEntity
    {
        public MovimentoItemEstEntity() { }

        public MovimentoItemEstEntity(int _Id)
        {
            this.Id = _Id;
        }

        public MovimentoEntity Movimento { get; set; }
        public MovimentoItemEntity MovimentoItem { get; set; }
        public MovimentoEstEntity MovimentoEst { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public DateTime? DataVencimentoLote { get; set; }
        public string IdentificacaoLote { get; set; }
        public string FabricanteLote { get; set; }
        public decimal? QtdeMov { get; set; }
        public decimal? QtdeLiq { get; set; }
        public decimal? SaldoQtde { get; set; }
        public decimal? SaldoQtdeLote { get; set; }
        public decimal? PrecoUnit { get; set; }
        public decimal? SaldoValor { get; set; }
        public decimal? ValorMov { get; set; }
        public decimal? Desd { get; set; }
        public bool Ativo { get; set; }
    }

}
