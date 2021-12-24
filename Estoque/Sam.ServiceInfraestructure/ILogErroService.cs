using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ILogErroService
    {
        void GravarLogErro(Exception ex);
        List<LogErroEntity> ListarLogErro(LogErroEntity logErro, int startIndex, int maxRowExibition);
        bool InserirEntradaNoLog(LogErroEntity logErro);
    }
}
