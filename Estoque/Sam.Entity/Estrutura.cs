using System;
using System.Collections.Generic;
using Sam.Domain.Entity;

namespace Sam.Entity
{
    [Serializable]
    public class Estrutura
    {
        public List<OrgaoEntity> Orgao { get; set; }
        public List<UOEntity> Uo { get; set; }
        public List<UGEEntity> Uge { get; set; }
        public List<UAEntity> Ua { get; set; }
        public List<DivisaoEntity> Divisao { get; set; }
        public List<GestorEntity> Gestor { get; set; }
        public List<AlmoxarifadoEntity> Almoxarifado { get; set; }
    }
}
