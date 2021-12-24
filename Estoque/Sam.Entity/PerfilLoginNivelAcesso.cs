using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common;

namespace Sam.Entity
{
    public class PerfilLoginNivelAcesso : BaseSeguranca
    {
        public int IdLoginNivelAcesso { get; set; }
        public int PerfilLoginId { get; set; }
        public int IdPerfil { get; set; }
        public int? PerfiLoginNivelValor { get; set; }
        public NivelAcesso NivelAcesso { get; set; }
        public string DescricaoValor { get; set; }
        public string DescricaoPerfil { get; set; }
        public string NomeUsuario { get; set; }
        public string Cpf { get; set; }
        public int? Valor { get; set; }
        public string PerfilDescricao { get; set; }

        public int? OrgaoId { get; set; }
        public int? UoId { get; set; }
        public int? UgeId { get; set; }
        public int? UaId { get; set; }
        public int? GestorId { get; set; }
        public int? DivisaoId { get; set; }
        public int? AlmoxId { get; set; }
        public Common.Perfil Perfil { get; set; }
        public Usuario Usuario { get; set; }
    }
}
