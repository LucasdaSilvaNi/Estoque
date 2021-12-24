using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Sam.View
{
    public interface IOrgaoEditView : ICrudEditView
    {
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
    }
}
