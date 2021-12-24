using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common;

namespace Sam.Entity
{
    public class Usuario: BaseLogin
    {

        public Login Login { get; set; }
        public NivelAcesso NivelAcesso { get; set; }
        public bool UsuarioAtivo { get; set; }
        public string NomeUsuario { get; set; }
        public string Rg { get; set; }
        public string RgUf { get; set; }
        public string Endereco { get; set; }
        public Int16 Numero { get; set; }
        public string Complemento { get; set; }
        public string DescricaoPerfil { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string Uf { get; set; }
        public Int32 Cep { get; set; }
        public string Fone { get; set; }
        public string Email { get; set; }
        public int? UgeId { get; set; }
        public Int32? OrgaoPadrao { get; set; }
        public Int32? GestorPadrao { get; set; }

        public string OrgaoEmissor { get; set; }

        public Int32 OrgaoId { get; set; }
        public Int32 NivelAcessoId { get; set; }
        public Int64 NivelId { get; set; }
        public Int32 GestorId { get; set; }

        public Int32 OrgaoPdId { get; set; }
        public Int32 GestorPdId { get; set; }

        public Int32? Perfil { get; set; }

        public string TextoEmail
        {
            get
            {
                return Msg.MailMessage;
            }
        }
    }   
}
