using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class SiglaEntity : BaseEntity
    {

        public SiglaEntity()
        {
        }

        public SiglaEntity(int? id)
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

        OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        GestorEntity gestor;
        public GestorEntity Gestor
        {
            get { return gestor; }
            set { gestor = value; }
        }

        IndicadorBemProprioEntity indicadorBemProprio;
        public IndicadorBemProprioEntity IndicadorBemProprio
        {
            get { return indicadorBemProprio; }
            set { indicadorBemProprio = value; }
        }

        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }
    }
}
