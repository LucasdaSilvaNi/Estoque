using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class GrupoEntity : BaseEntity
    {
        public GrupoEntity(){}
        
        public GrupoEntity(int _id)
        {
            this.Id = _id;
        }
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string CodigoDescricao { get { return String.Format("{0} - {1}", this.Codigo, this.Descricao); } private set { } }
    }
}
