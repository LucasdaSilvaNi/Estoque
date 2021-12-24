using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class LogUsuarioEntity : BaseSeguranca
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int? IdAlmoxarifado { get; set; }
        public int? IdLogin { get; set; }
        public int? IdOperador { get; set; }
        public int? IdPerfil { get; set; }
        public int? IdTransacao { get; set; }
        public string IP { get; set; }
    }
}
