using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
   public class MotivoBaixaEntity: BaseEntity
    {

       public MotivoBaixaEntity()
       {
 
       }

       public MotivoBaixaEntity(int? _id)
       {
           base.Id = _id;
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

       private string codigoTransacao;
       public string CodigoTransacao
       {
           get { return codigoTransacao; }
           set { codigoTransacao = value; }
       }

       private long? ordem;
       public long? Ordem
       {
           get { return ordem; }
           set { ordem = value; }
       }


    }
}
