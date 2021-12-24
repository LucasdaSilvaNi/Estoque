using System;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Entity
{
    [Serializable]
    public class UnidadeFornecimentoConversaoEntity : BaseSeguranca
    {
        public int?    Id                     { get; set; }
        public string  Codigo                 { get; set; }
        public string  Descricao              { get; set; }
        public int     SistemaSamId           { get; set; }
        public int     SistemaSiafisicoCodigo { get; set; }
        public decimal FatorUnitario          { get; set; }

        public UnidadeFornecimentoConversaoEntity()
        { }
        public UnidadeFornecimentoConversaoEntity(UnidadeFornecimentoEntity unidadeFornecimentoSAM, UnidadeFornecimentoSiafEntity unidadeFornecimentoSIAFISICO)
        { this.UnidadeFornecimentoSAM = unidadeFornecimentoSAM; this.UnidadeFornecimentoSiafisico = unidadeFornecimentoSIAFISICO; }

        public readonly UnidadeFornecimentoEntity UnidadeFornecimentoSAM;
        public readonly UnidadeFornecimentoSiafEntity UnidadeFornecimentoSiafisico;

        public string SistemaSamDescricao
        {
            get
            {
                if (!this.UnidadeFornecimentoSAM.IsNull() && !String.IsNullOrWhiteSpace(this.UnidadeFornecimentoSAM.Descricao))
                    return this.UnidadeFornecimentoSAM.Descricao;
                else
                    return string.Empty;
            }
        }

        public string SistemaSiafisicoDescricao
        {
            get
            {
                if (!this.UnidadeFornecimentoSiafisico.IsNull() && !String.IsNullOrWhiteSpace(this.UnidadeFornecimentoSiafisico.Descricao))
                    return this.UnidadeFornecimentoSiafisico.Descricao;
                else
                    return string.Empty;
            }
        }
    }
}
