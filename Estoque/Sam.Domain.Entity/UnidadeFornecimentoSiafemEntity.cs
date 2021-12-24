using System;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Domain.Entity
{ 
    [Serializable]
    public class UnidadeFornecimentoSiafemEntity : BaseEntity
    {

        public UnidadeFornecimentoSiafemEntity()
        {
        }

        public UnidadeFornecimentoSiafemEntity(int? id)
        {
            this.Id = id;
        }

        string codigo;
        public string Codigo
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

        GestorEntity gestor;
        public GestorEntity Gestor
        {
            get { return gestor; }
            set { gestor = value; }
        }
    }

}
