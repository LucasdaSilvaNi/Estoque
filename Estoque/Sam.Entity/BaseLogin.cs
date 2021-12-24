using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    /// <summary>
    /// Classe Base componente de segurança
    /// </summary>
    public class BaseLogin : BaseSeguranca
    {
        /// <summary>
        /// Login usuário conectado
        /// </summary>
        public string LoginBase { get; set; }
        /// <summary>
        /// Cpf usuário conectado
        /// </summary>
        public string Cpf { get; set; }
        /// <summary>
        /// Nome Usário conectado
        /// </summary>
        public string NomeUsuario { get; set; }

    }


}
