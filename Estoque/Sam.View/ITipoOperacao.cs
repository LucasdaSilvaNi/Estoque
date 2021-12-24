using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ITipoOperacao
    {
        enTipoOperacao Operacao { get; set; }
    }

    public enum enTipoOperacao
    {
        Insert = 1,
        Update = 2
    }
}
