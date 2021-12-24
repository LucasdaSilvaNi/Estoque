using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    [Serializable]
    public class Login : BaseLogin
    {

        public Login(string login)
        {
            this.LoginBase = login;
        }

        public Login()
        {
        }

        /// <summary>
        /// Senha Criptografada MD5
        /// </summary>
        public string Senha { get; set; }
        public bool AcessoBloqueado { get; set; }
        public bool LoginAtivo { get; set; }
        public Int32 ID { get; set; }
        //public Int32? Peso { get; set; }
        public string PalavraSecreta { get; set; }
        public List<Perfil> Perfis { get; set; }
        public Usuario Usuario { get; set; }
        public Perfil Perfil { get; set; }
        public int NumeroTentativasInvalidas { get; set; }
        public bool SenhaBloqueada { get; set; }
        public bool TrocarSenha { get; set; }
        public int? QtdAcessos { get; set; }
        public int? QtdAcessos30Dias { get; set; }
        public int? QtdAcessos90Dias { get; set; }
        public int? QtdAcessos180Dias { get; set; }
        public DateTime? DataUltimoAcesso { get; set; }
    }
}
