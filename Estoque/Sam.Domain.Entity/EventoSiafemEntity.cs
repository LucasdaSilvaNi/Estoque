using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class EventoSiafemEntity : BaseEntity
    {
        public EventoSiafemEntity() { }

        public EventoSiafemEntity(int _Id)
        {
            this.Id = _Id;
            base.Id = _Id;
        }


        public string EventoTipoMaterial { get; set; }
        public string EventoTipoEntradaSaidaReclassificacaoDepreciacao { get; set; }
        public string EventoTipoEstoque { get; set; }
        public string EventoEstoque { get; set; }
        public string EventoTipoMovimentacao { get; set; }
        public string EventoTipoMovimento { get; set; }
        public bool DetalheAtivo { get; set; }
        public bool EstimuloAtivo { get; set; }
        public bool Ativo { get; set; }
        public int SubTipoMovimentoId { get; set; }
        public SubTipoMovimentoEntity SubTipoMovimento { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool inseriu { get; set; }
        public string TipoMovSAM { get; set; }
        public string LoginAtivacao { get; set; }
        public string LoginAlteracao { get; set; }
    }
}
