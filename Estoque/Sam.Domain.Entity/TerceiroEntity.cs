using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class TerceiroEntity : BaseEntity
    {

        public TerceiroEntity()
        {
        }

        public TerceiroEntity(int? id)
        {
            this.Id = id;
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

        string cnpj;
        public string Cnpj
        {
            get { return cnpj; }
            set { cnpj = value; }
        }

        string nome;
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        string logradouro;
        public string Logradouro
        {
            get { return logradouro; }
            set { logradouro = value; }
        }

        string numero;
        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }
        
        string complemento;
        public string Complemento
        {
            get { return complemento; }
            set { complemento = value; }
        }

        string bairro;
        public string Bairro
        {
            get { return bairro; }
            set { bairro = value; }
        }

        string cidade;
        public string Cidade
        {
            get { return cidade; }
            set { cidade = value; }
        }

        UFEntity uf;
        public UFEntity Uf
        {
            get { return uf; }
            set { uf = value; }
        }

        string cep;
        public string Cep
        {
            get { return cep; }
            set { cep = value; }
        }

        string telefone;
        public string Telefone
        {
            get { return telefone; }
            set { telefone = value; }
        }

        string fax;
        public string Fax
        {
            get { return fax; }
            set { fax = value; }
        }
               
        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        public string SiglaUF
        { 
            get { return Uf.Sigla; }
        }
    }
}
