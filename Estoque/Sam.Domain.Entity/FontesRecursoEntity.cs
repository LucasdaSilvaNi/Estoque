using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
   public class FontesRecursoEntity: BaseEntity
    {
       public FontesRecursoEntity()
       {
 
       }

       public FontesRecursoEntity(int? _id)
       {
           base.Id = _id;
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
