using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class MovimentoPendenteEntity : BaseEntity
    {
        public MovimentoPendenteEntity() { }

        public MovimentoPendenteEntity(int _Id)
        {
            this.Id = _Id;
        }

        public MovimentoEntity Movimento { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public DateTime? DataVencimentoLote { get; set; }

        public string Destino { get; set; }
        public string DocumentoMovimento { get; set; }
        public string DocumentoOrigem { get; set; }
        public string DocumentoDestino { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime? OperacaoSaida { get; set; }
        public DateTime? EstornoSaida { get; set; }
        public string ValorTotalDocSaida { get; set; }
        public string SomatoriaSaida { get; set; }
        public DateTime? DataEntrada { get; set; }
        public DateTime? OperacaoEntrada { get; set; }
        public DateTime? EstornoEntrada { get; set; }
        public string ValorTotalDocEntrada { get; set; }
        public string SomatoriaEntrada { get; set; }
    }
}
