using Sam.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Domain.Entity
{
    public class ResumoSiafem
    {
        public string DocumentoSAM { get; set; }
        public string ErroProcessamentoMsgWS { get; set; }
        public int? Id { get; set; }
        public TipoNotaSIAF TipoNotaSIAF { get; set; }

        public static ResumoSiafem GetResumoSiafem(NotaLancamentoPendenteSIAFEMEntity nota)
        {
            return new  ResumoSiafem()
            {
                DocumentoSAM = nota.DocumentoSAM,
                ErroProcessamentoMsgWS = nota.ErroProcessamentoMsgWS,
                Id = nota.Id,
                TipoNotaSIAF = nota.TipoNotaSIAF
            };
        }
    }
}
