using System;
using System.Collections.Generic;
using System.Linq;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class AlmoxarifadoEntity : BaseEntity
    {
        public  AlmoxarifadoEntity() {}
        public AlmoxarifadoEntity(int? _id)
        {
            base.Id = _id;
        }

        OrgaoEntity orgao;
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        DivisaoEntity divisao;
        public DivisaoEntity Divisao
        {
            get{ return divisao;}
            set { divisao = value; }
        }

        GestorEntity gestor;
        public GestorEntity Gestor
        {
            get { return gestor; }
            set { gestor = value; }
        }

        int? codigo;
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private bool indicAtividade;
        public bool IndicAtividade
        {
            get { return indicAtividade; }
            set { indicAtividade = value; }
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

        string enderecoBairro;
        public string EnderecoBairro
        {
            get { return enderecoBairro; }
            set { enderecoBairro = value; }
        }

        string enderecoMunicipio;
        public string EnderecoMunicipio
        {
            get { return enderecoMunicipio; }
            set { enderecoMunicipio = value; }
        }

        UFEntity uf;
        public UFEntity Uf
        {
            get { return uf; }
            set { uf = value; }
        }
        
        string enderecoCep;
        public string EnderecoCep
        {
            get { return enderecoCep; }
            set { enderecoCep = value; }
        }

        string enderecoTelefone;
        public string EnderecoTelefone
        {
            get { return enderecoTelefone; }
            set { enderecoTelefone = value; }
        }

        string enderecoFax;
        public string EnderecoFax
        {
            get { return enderecoFax; }
            set { enderecoFax = value; }
        }

        string responsavel;
        public string Responsavel
        {
            get { return responsavel; }
            set { responsavel = value; }
        }

        UGEEntity uge;
        public UGEEntity Uge
        {
            get { return uge; }
            set { uge = value; }
        }

        string refInicial;
        public string RefInicial
        {
            get { return refInicial; }
            set { refInicial = value; }
        }

        string refFaturamento;
        public string RefFaturamento
        {
            get { return refFaturamento; }
            set { refFaturamento = value; }
        }

        string tipoAlmoxarifado;
        public string TipoAlmoxarifado
        {
            get { return tipoAlmoxarifado; }
            set { tipoAlmoxarifado = value; }
        }


        string mesRef;
        public string MesRef
        {
            get { return mesRef; }
            set { mesRef = value; }
        }

        IndicadorAtividadeEntity indicadorAtividade;
        public IndicadorAtividadeEntity IndicadorAtividade
        {
            get { return indicadorAtividade; }
            set { indicadorAtividade = value; }
        }

        long? ordem;
        public long? Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        public string SiglaUF
        {
            get {                
                    if (Uf != null) 
                        return Uf.Sigla;
                     else
                        return "";
                }
        }

        public bool AlmoxDefault { get; set; }

        public bool IgnoraCalendarioSiafemParaReabertura { get; set; }

        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo.GetValueOrDefault(), Descricao, 7) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
       
    }
}
