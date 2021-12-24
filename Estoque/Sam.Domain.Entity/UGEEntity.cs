using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UGEEntity : BaseEntity
    {

        public UGEEntity()
        {
        }

        public UGEEntity(int? id)
        {
            this.Id = id;
        }
        
        private UOEntity uo;
        public UOEntity Uo
        {
            get { return uo; }
            set { uo = value; }
        }

        private OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        private string tipoUGE;
        public string TipoUGE
        {
            get { return tipoUGE; }
            set { tipoUGE = value; }
        }

        private int? codigo;
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

        public bool UgeDefault { get; set; }


        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo.GetValueOrDefault(), Descricao, 6) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
        private bool ativo;
        public bool Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }
        private bool? integracaoSIAFEM;
        public bool? IntegracaoSIAFEM
        {
            get { return integracaoSIAFEM; }
            set { integracaoSIAFEM = value; }
        }
        private bool? implantado;
        public bool? Implantado
        {
            get { return implantado; }
            set { implantado = value; }
        }

        private decimal? saldoValor;
        public decimal? SaldoValor
        {
            get { return saldoValor; }
            set { saldoValor = value; }
        }
    }
}
