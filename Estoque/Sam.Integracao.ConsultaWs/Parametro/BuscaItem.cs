using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Sam.Integracao.ConsultaWs.Parametro
{
    public class BuscaItem
    {
        public string cpf { get; set; }
        public string senha { get; set; }

        public string termoParaPesquisa { get; set; }
        public string codigoOrgao { get; set; }
        public string codigoAlmox { get; set; }
        public string dispRequisicao { get; set; }

        public string ugeCodigo { get; set; }
        public string uaCodigo { get; set; }
        public string divisaoUaCodigo { get; set; }
        public string tipoMovimentacaoMaterialCodigo { get; set; }
        public string agrupamentoTipoMovimentacaoMaterialCodigo { get; set; }
        public string cpfCnpjFornecedor { get; set; }

        public string dataInicial { get; set; }
        public string dataFinal { get; set; }
        public string consultaTransfs { get; set; }

        public string TotalPaginas;
        public string NumeroPaginaConsulta;
    }
}