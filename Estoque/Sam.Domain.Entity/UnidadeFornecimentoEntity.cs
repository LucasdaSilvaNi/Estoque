using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UnidadeFornecimentoEntity : BaseEntity
    {

        public UnidadeFornecimentoEntity()
        {
        }

        public UnidadeFornecimentoEntity(int? id)
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
