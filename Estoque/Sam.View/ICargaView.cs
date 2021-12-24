using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Infrastructure;

namespace Sam.View
{
    public interface ICargaView : ICrudView
    {
        void CarregarArquivoExcel();
        void LerArquivosPasta();
        int TipoArquivo { get; }

        void ExportarToExcel(IEnumerable<TB_CARGA> listErros, IEnumerable<TB_CARGA_ERRO> listErroDescricao);
    }
}
