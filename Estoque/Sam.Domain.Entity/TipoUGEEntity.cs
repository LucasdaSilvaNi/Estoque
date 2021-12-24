using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class TipoUGEEntity : BaseEntity
    {

        public TipoUGEEntity()
        {

        }

        public TipoUGEEntity(int? id)
        {
            this.Id = id;
        }
        
        private string descricao;
        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }
    }
}
