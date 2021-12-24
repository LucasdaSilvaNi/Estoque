using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UnidadeEntity : BaseEntity
    {

        public UnidadeEntity()
        {
        }

        public UnidadeEntity(int? id)
        {
            this.Id = id;
        }
        
        Int64? codigo;
        public Int64? Codigo
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

        OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }
    }
}
