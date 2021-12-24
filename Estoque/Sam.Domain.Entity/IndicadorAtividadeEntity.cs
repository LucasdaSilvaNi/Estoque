using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class IndicadorAtividadeEntity : BaseEntity
    {
        public IndicadorAtividadeEntity()
        {
 
        }

        public IndicadorAtividadeEntity(int? id)
        {
            this.Id = id;
        }

        int? codigo;

        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        string descricao;

        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }
    }
}
