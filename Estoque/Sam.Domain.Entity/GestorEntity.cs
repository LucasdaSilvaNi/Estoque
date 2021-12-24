using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class GestorEntity : BaseEntity
    {
        public  GestorEntity() {}
        public GestorEntity(int? _id)
        {
            base.Id = _id;
        }

        string nome;
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        string nomeReduzido;
        public string NomeReduzido
        {
            get { return nomeReduzido; }
            set { nomeReduzido = value; }
        }

        string enderecoLogradouro;
        public string EnderecoLogradouro
        {
            get { return enderecoLogradouro; }
            set { enderecoLogradouro = value; }
        }

        string enderecoNumero;
        public string EnderecoNumero
        {
            get { return enderecoNumero; }
            set { enderecoNumero = value; }
        }

        string enderecoCompl;
        public string EnderecoCompl
        {
            get { return enderecoCompl; }
            set { enderecoCompl = value; }
        }

        string enderecoTelefone;
        public string EnderecoTelefone
        {
            get { return enderecoTelefone; }
            set { enderecoTelefone = value; }
        }

        int? codigoGestao;
        public int? CodigoGestao
        {
            get { return codigoGestao; }
            set { codigoGestao = value; }
        }

        byte[] logotipo;
        public byte[] Logotipo
        {
            get { return logotipo; }
            set { logotipo = value; }
        }

        OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        UGEEntity uge;
        public UGEEntity Uge
        {
            get { return uge; }
            set { uge = value; }
        }

        UOEntity uo;
        public UOEntity Uo
        {
            get { return uo; }
            set { uo = value; }
        }

        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        int? tipoId;
        public int? TipoId
        {
            get { return tipoId; }
            set { tipoId = value; }
        }

        public bool GestorDefault { get; set; }

        public override string Descricao
        {
            get
            {
                if (base.Descricao == null && Nome != null)
                    return Nome;

                return base.Descricao;
            }

            set
            {
                base.Descricao = value;
            }
        }


        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(CodigoGestao.GetValueOrDefault(), Descricao, 2) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
    }
}
