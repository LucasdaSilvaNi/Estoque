using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IMenuTabelasView : ICrudView
    {
        int Modulo
        {
            get;
        }

        bool EstruturaOrganizacional
        {
            set;
        }

        bool Catalogo
        {
            set;
        }

        bool Outros
        {
            set;
        }

        List<Sam.Entity.Transacao> Transacoes
        {
            set;
            get;
        }
    }
}
