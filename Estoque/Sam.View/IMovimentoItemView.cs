using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IMovimentoItemView : ICrudView
    {
        string IdItem { get; set; }
        DateTime? DataVctoLoteItem { get; set; }
        //string IdentLoteItem { get; set; }
        string IdentificacaoLoteItem { get; set; }
        string FabricLoteItem { get; set; }
        decimal QtdeItem { get; set; }
        decimal QtdeLiqItem { get; set; }
        decimal PrecoUnitItem { get; set; }
        decimal? SaldoValorItem { get; set; }
        decimal SaldoQtdeItem { get; set; }
        decimal? SaldoQtdeLoteItem { get; set; }
        decimal ValorMovItem { get; set; }
        decimal DesdItem { get; set; }
        int? MovimentoItemId { get; set; }
        bool? AtivoItem { get; set; }
        int? SubItemMatItem { get; set; }
        int? SubItemMaterialId { get; set; }
        long? SubItemMaterialCodigo { get; set; }
        string SubItemMaterialDescricao { get; set; }
        int? ItemMaterialId { get; set; }
        int? ItemMaterialCodigo { get; set; }
        bool HabilitarLote { set; }
        int? NaturezaDespesaIdItem { get; set; }
        string SubItemMaterialTxt { get; set; }

    }
}
