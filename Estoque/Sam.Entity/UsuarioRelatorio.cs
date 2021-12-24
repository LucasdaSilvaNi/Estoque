using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Entity
{
    public class UsuarioRelatorio : Usuario
    {
        public Boolean PerfilAtivo { get; set; }
        public string Descricao { get; set; }
        public string DescricaoComDescricaoEstrutura { get; set; }
        public string DescricaoComCodigoEstrutura { get; set; }
        public int IdLogin { get; set; }
        public bool LoginAtivo { get; set; }
        public bool LoginSenhaBloqueada { get; set; }
        public Int16 IdPerfil { get; set; }
        public int IdPerfilLogin { get; set; }
        public int? Peso { get; set; }
        public int CodigoOrgao { get; set; }
        public string NomeOrgao { get; set; }
        public int? IdGestor { get; set; }
        public int CodigoGestor { get; set; }
        public string NomeGestor { get; set; }
        public string NomeReduzidoGestor { get; set; }
        public int IdUo { get; set; }
        public int CodigoUo { get; set; }
        public  string DescricaoUo { get; set; }
        public int IdUge { get; set; }
        public int CodigoUge { get; set; }
        public string DescricaoUge { get; set; }
        public int IdUa { get; set; }
        public int CodigoUa { get; set; }
        public string DescricaoUa { get; set; }
        public int IdDivisao { get; set; }
        public int CodigoDivisao { get; set; }
        public string DescricaoDivisao { get; set; }
        public int IdAlmoxarifado { get; set; }
        public int CodigoAlmoxarifado { get; set; }
        public string DescricaoAlmoxarifado { get; set; }
        public DateTime? DataUltimoAcesso { get; set; }
        public int? TotalAcessos { get; set; }
        public int? AcessosUltimos30Dias { get; set; }
        public int? AcessosUltimos90Dias { get; set; }
        public int? AcessosUltimos180Dias { get; set; }
        public long? IdentificadorRegistro { get; set; }


    }
}
