using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UFEntity : BaseEntity
    {

        public UFEntity()
        {
        }

        public UFEntity(int? id)
        {
            this.Id = id;
        }

        public string Sigla { get; set; }
                
        public string Descricao { get; set; }


    }
}
