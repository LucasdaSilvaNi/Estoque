using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sam.View
{
    public interface ICrudEditView : ICrudView
    {
        bool VisivelEditar { set; }
        bool VisivelAdicionar { set; }
    }
}
