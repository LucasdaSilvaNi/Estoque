using System;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Domain.Entity
{
    [Serializable]
    public class UnidadeFornecimentoSiafEntity
    {
        public int    Codigo    { get; set; }
        public string Descricao { get; set; }
    }
}
