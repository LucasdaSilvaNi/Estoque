using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class OrgaoEntity : BaseEntity
    {

        public OrgaoEntity()
        {
 
        }


        public OrgaoEntity(int? _id)
        {
            this.Id = _id;
        }

        int? codigo;
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }


        private long? ordem;

        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        public bool OrgaoDefault { get; set; }


        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo.GetValueOrDefault(), Descricao, 2) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
        private bool? _implantado;
        public bool? Implantado
        {
            get
            { return _implantado; }
            set
            { _implantado = value; }
        }

        private bool? integracaoSIAFEM;
        public bool? IntegracaoSIAFEM
        {
            get
            { return integracaoSIAFEM; }
            set
            { integracaoSIAFEM = value; }
        }
    }
}
