using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class MaterialEntity : BaseEntity
    {
        public MaterialEntity() { }


        public MaterialEntity(int _meterialId)
        {
            this.Id = _meterialId;
        }

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        //public GrupoEntity Grupo { get; set; }
        public ClasseEntity Classe { get; set; }
        public string CodigoDescricao { get { return String.Format("{0} - {1}", this.Codigo, this.Descricao); } private set { } }
    }
}
