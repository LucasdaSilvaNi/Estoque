using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class MovimentoEstEntity : BaseEntity
    {
        public MovimentoEstEntity() { }

        public MovimentoEstEntity(int _Id)
        {
            this.Id = _Id;
        }

        public MovimentoEntity Movimento { get; set; }
        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public int TipoMovimento { get; set; }
        public string GeradorDescricao { get; set; }
        public string NumeroDocumento { get; set; }
        public string AnoMesReferencia { get; set; }
        public DateTime? DataDocumento { get; set; }
        public DateTime? DataMovimento { get; set; }
        public string FonteRecurso { get; set; }
        public decimal ValorDocumento { get; set; }
        public string Observacoes { get; set; }
        public string Instrucoes { get; set; }
        public string Empenho { get; set; }
        public AlmoxarifadoEntity MovimAlmoxOrigemDestino { get; set; }
        public PTResEntity PTRes { get; set; }
        public string NlLiquidacao { get; set; }
        //public List<MovimentoEstEntity> MovimentoEstorno { get; set; }
        public IList<MovimentoItemEstEntity> MovimentoItemEst { get; set; }
        public FornecedorEntity Fornecedor { get; set; }
        public DivisaoEntity Divisao { get; set; }
        public UGEEntity UGE { get; set; }
    }
}



