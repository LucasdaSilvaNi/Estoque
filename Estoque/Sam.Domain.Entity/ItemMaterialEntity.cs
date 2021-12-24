using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class ItemMaterialEntity : BaseEntity
    {
        public ItemMaterialEntity() { }
        public ItemMaterialEntity(int _itemId)
        {
            this.Id = _itemId;
        }
        
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public bool Atividade { get; set; }
        public MaterialEntity Material { get; set; }
        public int MaterialId { get; set; }
        public int ClasseId { get; set; }
        public int GrupoId { get; set; }
        public int? ItemSubItemMaterialId { get; set; }
        public decimal? SomaQtdeMov { get; set; }
        public List<NaturezaDespesaEntity> NaturezaDespesa { get; set; }
        public bool? Status { get; set; }
        public string StatusFormatado { get { return this.Status == true ? "Ativo" : "Inativo"; } }

        // naturezas de despesa do item de material extraídas do Siafisico
        public int NatDespSiafisicoCodigo1 { get; set; }
        public int NatDespSiafisicoCodigo2 { get; set; }
        public int NatDespSiafisicoCodigo3 { get; set; }
        public int NatDespSiafisicoCodigo4 { get; set; }
        public int NatDespSiafisicoCodigo5 { get; set; }

        public string CodigoDescricao { get { return String.Format("{0} - {1}", this.Codigo, this.Descricao); } private set { } }
        public string CodigoFormatado { get { return this.Codigo.ToString().PadLeft(9, '0'); } set { } }
    }
}
