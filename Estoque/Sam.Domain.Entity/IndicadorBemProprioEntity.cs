using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class IndicadorBemProprioEntity : BaseEntity
    {

        public IndicadorBemProprioEntity()
        {

        }

        public IndicadorBemProprioEntity(int? id)
        {
            this.Id = id;
        }

        private int? codigo;
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string descricao;
        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }
    }
}
