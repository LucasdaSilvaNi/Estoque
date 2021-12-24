using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;
using Sam.Domain.Entity;

namespace Sam.Business.Importacao
{
    public interface ICarga
    {
        bool InsertImportacao(TB_CONTROLE entityList);
        List<GeralEnum.CargaErro> ValidarImportacao(TB_CARGA carga);
    }
}
