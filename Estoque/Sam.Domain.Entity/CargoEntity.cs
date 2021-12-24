using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class CargoEntity : BaseEntity
    {
        public CargoEntity() {}

        public CargoEntity(int? _Id)
        {
            base.Id = _Id;
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
