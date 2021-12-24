using Sam.Domain.Entity;
using System;

namespace Sam.Entity
{
    [Serializable]
    public class ControleAcesso
    {
        public int Id { get; set; }
        public int QtdeRequisitante { get; set; }
        public int QtdeOperadorAlmoxarifado { get; set; }
        public Modulo Modulo { get; set; }
        public GestorEntity Gestor { get; set; }
    }
}
