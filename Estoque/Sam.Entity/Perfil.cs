using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Entity;
using Sam.Domain.Entity;

namespace Sam.Entity
{
    [Serializable]
    public class Perfil: BaseSeguranca
    {
        public List<Modulo> Modulos { get; set; }
        public List<Transacao> Transacoes { get; set; }
        public Boolean Ativo { get; set; }
        public string Descricao { get; set; }
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cpf { get; set; }
        public string DescricaoComDescricaoEstrutura { get; set; }
        public string DescricaoComCodigoEstrutura { get; set; }
        public int ? PerfilLoginNivelAcessoValor { get; set; }
        public int? NivelId { get; set; }
        public Int16 IdPerfil { get; set; }
        public int? Peso {get; set; }
        
        public OrgaoEntity OrgaoPadrao { get; set; }
        public GestorEntity GestorPadrao { get; set; }
        public AlmoxarifadoEntity AlmoxarifadoPadrao { get; set; }
        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public UOEntity UOPadrao { get; set; }
        public UGEEntity UGEPadrao { get; set; }
        public UAEntity UAPadrao { get; set; }
        public DivisaoEntity DivisaoPadrao { get; set; }
        public DivisaoEntity Divisao { get; set; }
        public int PerfilLoginId { get; set; }
        public AlmoxarifadoEntity AlmoxarifadoLogado { get; set; }
        public int IdLogin { get; set; }
        public List<PerfilLoginNivelAcesso> PerfilLoginNivelAcesso { get; set; }
        public bool IsAlmoxarifadoPadrao { get; set; }
        public Usuario Usuario { get; set; }               
    }
}
