using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class UsuarioLogadoEntity
    {
        public Int32 LoginId { get; set; }
        public DateTime DataHoraLogado { get; set; }
        public string IpLogado { get; set; }
        public Login Login { get; set; }
        public string SessionIdLogado { get; set; }
        public Int32 UsuarioLogadoId { get; set; }
        public string Usuario { get; set; }
        public Int32 UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
    }

    [Serializable]
    public class UsuarioLogadoPorGestorEntity : Gestor
    {
        public IList<UsuarioLogadoEntity> UsuarioLogado { get; set; }
    }
}