using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface IConsultarCatalogoView : ICrudView
    {
        void PopularListaDivisao();

        bool BloqueiaListaDivisao { set; }
        bool BloqueiaListaUA { set; }
        void PopularListaUA();
    }
}
