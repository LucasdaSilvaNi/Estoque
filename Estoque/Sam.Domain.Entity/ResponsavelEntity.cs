using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class ResponsavelEntity : BaseEntity
    {

        public ResponsavelEntity()
        {
        }

        public ResponsavelEntity(int? id)
        {
            this.Id = id;
        }

        long? codigo;
        public long? Codigo
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

        string cargo;
        public string Cargo
        {
            get { return cargo; }
            set { cargo = value; }
        }

        string endereco;
        public string Endereco
        {
            get { return endereco; }
            set { endereco = value; }
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
