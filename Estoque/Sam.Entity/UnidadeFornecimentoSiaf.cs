using System;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Entity
{
    [Serializable]
    public class UnidadeFornecimentoSiafEntity : BaseSeguranca
    {
        public int    Codigo    { get; set; }
        public string Descricao { get; set; }
    }
}
