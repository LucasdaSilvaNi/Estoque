using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Sam.Integracao.ConsultaWs.Parametro
{
    public class DadosRelatorioWs : dtoBaseWs
    {
        public string cpf { get; set; }
        public string senha { get; set; }

        public string termoParaPesquisa { get; set; }
        public string codigoOrgao { get; set; }
        public string codigoAlmox { get; set; }
        public string dispRequisicao { get; set; }

        public string codigoUge { get; set; }
        public string codigoUa { get; set; }
        public string codigoDivisaoUa { get; set; }
        public string codigoTipoMovimentacaoMaterial { get; set; }
        public string codigoAgrupamentoTipoMovimentacaoMaterial { get; set; }

        public string dataConsultaInicial { get; set; }
        public string dataConsultaFinal { get; set; }
        public string mesReferenciaInicial { get; set; }
        public string mesReferenciaFinal { get; set; }

        public string cpfCnpjFornecedor { get; set; }
    }
}