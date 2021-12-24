using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Sam.Integracao.ConsultaWs.Parametro
{
    public class DadosMovimentacaoWs : dtoBaseWs
    {
        public string cpf { get; set; }
        public string senha { get; set; }

        public string codigoOrgao { get; set; }
        public string codigoAlmox { get; set; }
        public string codigoUge { get; set; }
        public string codigoUa { get; set; }
        public string codigoDivisaoUa { get; set; }
        public string codigoTipoMovimentacaoMaterial { get; set; }
        public string cpfCnpjFornecedor { get; set; }

        public string cpfUsuarioSAM { get; set; }
        public string dataMovimento { get; set; }
        public string dataDocumento { get; set; }
        public string observacoes { get; set; }
        public string MovimentoItem { get; set; }

        //public string statusPrioritaria { get; set; }
    }
}