using Sam.Domain.Entity.DtoWs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Sam.Domain.Entity.DtoWs
{
    public class dtoWsItemMovimentacaoMaterial : dtoWsBase
    {
        public string Codigo;
        public string Descricao;
        public string UnidadeFornecimento;
        public string NaturezaDespesa;
        public decimal? Qtde_ItemMovimentacaoMaterial;
        public decimal? Qtde_ItemMovimentacaoMaterialSolicitada;
        public decimal? Valor_ItemMovimentacaoMaterial;
        public decimal? PrecoUnitario;
        public decimal? PrecoMedio;
        public decimal? Desdobro;

        public DateTime? DataMovimento;

        public string Fabricante_Lote;
        public string ID_Lote;
        public DateTime? DataVencimento_Lote;

        public string NL_Consumo;
        public string NL_Liquidacao;
        public string NL_Reclassificacao;

    }
}
