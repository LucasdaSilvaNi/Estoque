using Sam.Domain.Entity;
using Sam.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Sam.Common.Util.GeralEnum;

namespace Sam.Entity
{
    public class UsuarioLogado
    {
        public string Usuario { get; set; }
        public string UsuarioNome { get; set; }
        public DateTime DataHoraLogado { get; set; }
        public int? Id { get; set; }
        public string SessionIdLogado { get; set; }
        public TipoNotaSIAF TipoNotaSIAF { get; set; }

        public static UsuarioLogado GetResumoUsuario(UsuarioLogadoEntity usuario)
        {
            return new UsuarioLogado()
            {
                Usuario = usuario.Usuario,
                UsuarioNome = usuario.UsuarioNome,
                DataHoraLogado = usuario.DataHoraLogado,
                Id = usuario.UsuarioId,
                SessionIdLogado = usuario.SessionIdLogado

            };
        }
    }

}
