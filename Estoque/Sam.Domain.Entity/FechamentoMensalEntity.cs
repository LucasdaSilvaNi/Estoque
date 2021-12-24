using Sam.Domain.Entity.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;


namespace Sam.Domain.Entity
{
    public class FechamentoMensalEntity : BaseEntity
    {
        public decimal? SaldoAtual33;

        public FechamentoMensalEntity() { }

        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public decimal? QtdeEntrada { get; set; }
		public decimal? QtdeSaida { get; set; }
		public decimal? SaldoQtde { get; set; }
        public decimal? SaldoAnterior { get; set; }

        public decimal? ValorEntrada { get; set; }
        public decimal? ValorSaida { get; set; }
        public decimal? SaldoValor { get; set; }
        public int? AnoMesRef { get; set; }

        public bool chkSaldoMaiorZero { get; set; }

        public DateTime? LoteDataVenc { get; set; }
        public string LoteIdent { get; set; }
        public string LoteFabr { get; set; }
        public int? SituacaoFechamento { get; set; }
        public Decimal? SaldoAnteriorValor33 { get; set; }
        public Decimal? SaldoAnteriorValor44 { get; set; }

        public decimal? SaldoAnteriorValor { get; set; }

        public string NaturezaDespesa 
        { 
            get { return SubItemMaterial.NaturezaDespesa.CodigoFormatado + " - " + SubItemMaterial.NaturezaDespesa.Descricao; } 
        }

        public string UgeCodigo
        {
            get { return UGE.CodigoFormatado; }
        }

        public string UgeDescricao
        {
            get { return UGE.Descricao; }
        }

        public string SubItemCodigo 
        {
            get { return SubItemMaterial.CodigoFormatado; } 
        }

        public string SubItemDescricao
        {
            get { return SubItemMaterial.Descricao; }
        }

        public string UnidadeFornecimento 
        {
            get { return SubItemMaterial.UnidadeFornecimento.Codigo; }
        }

        public string GrupoMaterialDescricao
        {
            get { return String.Format("{0} - {1}", SubItemMaterial.ItemMaterial.Material.Classe.Grupo.CodigoFormatado, SubItemMaterial.ItemMaterial.Material.Classe.Grupo.Descricao); }
        }

        public string ClasseMaterialDescricao
        {
            get { return String.Format("{0} - {1}", SubItemMaterial.ItemMaterial.Material.Classe.CodigoFormatado, SubItemMaterial.ItemMaterial.Material.Classe.Descricao); }
        }

        public string MaterialDescricao
        {
            get { return String.Format("{0} - {1}", SubItemMaterial.ItemMaterial.Material.CodigoFormatado, SubItemMaterial.ItemMaterial.Material.Descricao); }
        }

        public string ItemMaterialCodigo
        {
            get { return SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9, '0'); }
        }

        public string AlmoxarifadoDesc 
        {
            get { return String.Format("{0} - {1}", Almoxarifado.Codigo.ToString().PadLeft(3, '0'), Almoxarifado.Descricao); }
        }

        public string ItemMaterialDescricao
        {
            get { return SubItemMaterial.ItemMaterial.Descricao; }
        }

    }
}
