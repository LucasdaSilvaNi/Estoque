using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ILiquidacaoEmpenhoView : ICrudView 
    {
        //int? FornecedorId { get; set; }
        //string Empenho { get; set; }
        string CodigoEmpenho { get; set; }
        //string UgeCodigoDescricao { get; set; }
        decimal? ValorDocumento { get; set; }
        //decimal? ValorTotalMovimento { get; set; }
    }
}
