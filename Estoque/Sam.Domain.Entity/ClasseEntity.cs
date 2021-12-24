using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class ClasseEntity : BaseEntity
    {
        public ClasseEntity() { }
        public ClasseEntity(int _classeId) { this.Id = _classeId; }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public GrupoEntity Grupo { get; set; }
        public string CodigoDescricao { get { return String.Format("{0} - {1}", this.Codigo, this.Descricao); } private set { } }
    }

}
