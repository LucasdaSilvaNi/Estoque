using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Sam.Integracao.ConsultaWs.Parametro
{
    public class DadosCredenciais : dtoBaseWs
    {
        public string cpf { get; set; }
        public string senha { get; set; }
        public string codigoOrgao { get; set; }
    }
}