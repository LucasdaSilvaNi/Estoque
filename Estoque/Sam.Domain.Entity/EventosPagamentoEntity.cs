using System;
using System.Collections.Generic;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class EventosPagamentoEntity : BaseEntity
    {
        public EventosPagamentoEntity() { }
        public EventosPagamentoEntity(int? _id) { base.Id = _id; }

        public int PrimeiroCodigo { get; set; }
        public int? PrimeiroCodigoEstorno { get; set; }
        public string PrimeiraInscricao { get; set; }
        public string PrimeiraClassificacao { get; set; }

        public int SegundoCodigo { get; set; }
        public int? SegundoCodigoEstorno { get; set; }
        public string SegundaInscricao { get; set; }
        public string SegundaClassificacao { get; set; }

        public int TerceiroCodigo { get; set; }
        public int? TerceiroCodigoEstorno { get; set; }
        public string TerceiroInscricao { get; set; }

        public string AnoBase { get; set; }
        public bool Ativo { get; set; }
        public bool UGFavorecida { get; set; }


        public int TipoMaterialAssociado { get; set; }
        public TipoMovimentoEntity TipoMovimentoAssociado { get; set; }

        public List<EventoSiafemEntity> EventoSiafem { get; set;}
        public EventoSiafemEntity EventoSiafemItem { get; set; }
        public bool NlPatrimonial { get; set; }
        public bool EstimuloAtivo { get; set; }
    }
}
