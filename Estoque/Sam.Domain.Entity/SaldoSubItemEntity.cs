using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class SaldoSubItemEntity : BaseEntity
    {
        public SaldoSubItemEntity() { }

        public SaldoSubItemEntity(int _Id)
        {
            this.Id = _Id;
        }

        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public decimal? SaldoValor { get; set; }
        public decimal? SaldoValorTotal { get; set; }
        public decimal? PrecoUnit { get; set; }
        public DateTime? LoteDataVenc { get; set; }
        public string LoteIdent { get; set; }
        public string LoteFabr { get; set; }
        public decimal? LoteSaldoQtde { get; set; }
        public int IdLote { get; set; }
        public int? NaturezaDespesaId { get; set; }

        public string SubItemMaterialDescr
        {
            get { return SubItemMaterial.Descricao; }
        }

        public string SubItemMaterialCodigo
        {
            get { return SubItemMaterial.Codigo.ToString().PadLeft(12, '0'); }
        }

        public string UnidadeFornecimentoCodigo
        {
            get { return SubItemMaterial.UnidadeFornecimento.Codigo; }
        }

        public string UgeCodigo
        {
            get { return UGE.Codigo.ToString().PadLeft(6, '0'); }
        }

        public string AlmoxarifadoDesc
        {
            get { return String.Format("{0} - {1}", Almoxarifado.Codigo.ToString().PadLeft(3, '0'), Almoxarifado.Descricao); }
        }

        public string UgeDescricao
        {
            get { return UGE.Descricao; }
        }

        public string NaturezaDespesaCompleta
        {
            get { return SubItemMaterial.NaturezaDespesa.CodigoFormatado + " - " + SubItemMaterial.NaturezaDespesa.Descricao; }
        }

        // para fechamento

        public decimal? SaldoAnterior { get; set; }
        public decimal? QtdeEntrada { get; set; }
        public decimal? QtdeSaida { get; set; }
        public decimal? QtdeFechamento { get; set; }
        public decimal? SaldoQtde { get; set; }
        public decimal? QtdeReservaMaterial { get; set; }
        public decimal? SaldoQtdTotal { get; set; }
        public decimal? SaldoAnteriorValor { get; set; }
        public decimal? ValEntrada { get; set; }
        public decimal? ValSaida { get; set; }
        public decimal? ValFechamento { get; set; }
        public int? AnoMesReferencia_Impressao { get; set; }

        public decimal? SomatoriaTotalQtdeSubitens { get; set; }
        public decimal? SomatoriaTotalValorSubitens { get; set; }
        public int? SomatoriaQtdeSubitensMaterial { get; set; }
    }
}



