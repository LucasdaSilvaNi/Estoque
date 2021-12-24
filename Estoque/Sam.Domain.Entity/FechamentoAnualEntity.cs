using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class FechamentoAnualEntity
    {
        public string NATUREZA_DESPESA_CODIGO { get; set; }
        public decimal SALDO_ANO_ANTERIOR { get; set; }
        public decimal ENTRADA { get; set; }
        public decimal SAIDA { get; set; }
        public decimal RESUMO_ANO_ATUAL { get; set; }
        public string TB_ALMOXARIFADO_ID { get; set; }
        public string TB_ALMOXARIFADO_CODIGO { get; set; }
        public string TB_ALMOXARIFADO_DESCRICAO { get; set; }
        public string TB_ALMOXARIFADO_CEP { get; set; }
        public string TB_ALMOXARIFADO_BAIRRO { get; set; }
        public string TB_ALMOXARIFADO_COMPLEMENTO { get; set; }
        public string TB_ALMOXARIFADO_LOGRADOURO { get; set; }
        public string TB_ALMOXARIFADO_MUNICIPIO { get; set; }
        public string TB_ALMOXARIFADO_NUMERO { get; set; }
    }
}
