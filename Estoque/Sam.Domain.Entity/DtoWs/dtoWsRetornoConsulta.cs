using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity.DtoWs
{
    public class dtoWsRetornoConsulta
    {
        public int? TotalPaginas;
        public int? NumeroPaginaConsulta;
        public string MensagemErro;

        public IList<object> DadosConsulta;
    }
}
