using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Sam.Domain.Entity.DtoWs
{
    public class dtoWsMovimentacaoMaterial
    {
        public string Almoxarifado;
        public string AlmoxarifadoOrigem;
        public string AlmoxarifadoDestino;
        public string UgeOrigem;
        public string UgeDestino;
        public string NumeroEmpenho;
        public string NumeroDocumento;
        public string TipoMovimentacao;
        public string CPF_Nome_Usuario;
        public string CPF_Nome_Requisitante;
        public string CPF_Nome_UsuarioEstorno;
        public string CPF_Nome_RequisitanteEstorno;
        public DateTime? DataMovimento;
        public DateTime? DataEmissao;
        public DateTime? DataRecebimento;
        public decimal? ValorTotalMovimentacaoMaterial;

        public string UsuarioSamID;
        public string UsuarioSamIDEstorno;

        public string RequisitanteSamID;
        public string RequisitanteSamIDEstorno;

        //public int codigoOrgao;
        //public int ugeCodigo;
        //public int codigoAlmox;
        //public int uaCodigo;
        //public int divisaoUaCodigo;
        //public int tipoMovimentacaoMaterialCodigo;
        //public int agrupamentoTipoMovimentacaoMaterialCodigo;
        public string CPF_CNPJ_Fornecedor;
        //public DateTime dataInicial;
        //public DateTime dataFinal;
        //public bool consultaTransfs;

        public string UA;
        public string UGE;

        public IList<dtoWsItemMovimentacaoMaterial> MovimentoItem;
    }
}
