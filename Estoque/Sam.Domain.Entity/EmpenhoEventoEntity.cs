using System;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class EmpenhoEventoEntity : BaseEntity
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }

        public EmpenhoEventoEntity() { }
        public EmpenhoEventoEntity(int? _id) { base.Id = _id; }


        public string AnoBase { get; set; }
        public int? CodigoEstorno { get; set; }
        public bool Ativo { get; set; }
        public int TipoMaterialAssociado { get; set; }
        public TipoMovimentoEntity TipoMovimentoAssociado { get; set; }
    }
}
