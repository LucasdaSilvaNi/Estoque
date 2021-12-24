using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Entity
{
    [Serializable()]
    public class Acesso: BaseLogin
    {
        public string SessionId { get; set; }
        public string ValorNivel { get; set; }

        public Estrutura Estruturas {get ; set;}
        public Login Transacoes { get; set; }
        public AlmoxarifadoEntity TipoCorrente { get; set; }
    }
}
