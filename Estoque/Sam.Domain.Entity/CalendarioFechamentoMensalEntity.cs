using System;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class CalendarioFechamentoMensalEntity : BaseEntity
    {
        public byte MesReferencia { get; set; }
        public int AnoReferencia { get; set; }
        public DateTime DataFechamentoDespesa { get; set; }

        public CalendarioFechamentoMensalEntity() { }
        public CalendarioFechamentoMensalEntity(int? _id) { base.Id = _id; }
    }
}
