using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Domain.Entity.ValidationAttributes;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class DivisaoEntity : BaseEntity
    {
        public DivisaoEntity() { }
        public DivisaoEntity(int? _id)
        {
            base.Id = _id;
        }

        OrgaoEntity orgao;

        //[InstanceClass(ErrorMessage="É obrigatório informar o Órgão.")]
        public OrgaoEntity Orgao
        {
            get { return orgao; }
            set { orgao = value; }
        }

        UAEntity ua;
       //[InstanceClass(ErrorMessage="É obrigatório informar a UA.")]
        public UAEntity Ua
        {
            get { return ua; }
            set { ua = value; }
        }

        int? codigo;
        //[Required(ErrorMessage = "É obrigatório informar o Código.")]
        public int? Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        ResponsavelEntity responsavel;
        
        //[InstanceClassAttribute(ErrorMessage = "É obrigatório informar o Responsável.")]
        public ResponsavelEntity Responsavel
        {
            get { return responsavel; }
            set { responsavel = value; }
        }

        AlmoxarifadoEntity almoxarifado;
        
        //[InstanceClass(ErrorMessage="É obrigatorio informar o Almoxarifado.")]
        public AlmoxarifadoEntity Almoxarifado
        {
            get { return almoxarifado; }
            set { almoxarifado = value; }
        }

        string enderecoLogradouro;
        //[Required(ErrorMessage = "É obrigatório informar o Logradouro.")]
        public string EnderecoLogradouro
        {
            get { return enderecoLogradouro; }
            set { enderecoLogradouro = value; }
        }

        string enderecoNumero;
        //[Required(ErrorMessage = "É obrigatório informar o Número.")]
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
        //[Required(ErrorMessage = "É obrigatório informar o Bairro.")]
        public string EnderecoBairro
        {
            get { return enderecoBairro; }
            set { enderecoBairro = value; }
        }

        string enderecoMunicipio;
        //[Required(ErrorMessage = "É obrigatório informar o Municipio.")]
        public string EnderecoMunicipio
        {
            get { return enderecoMunicipio; }
            set { enderecoMunicipio = value; }
        }

        UFEntity uf;
      
        //[InstanceClass(ErrorMessage="É obrigatório informar a UF.")]
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

        int? area;
        public int? Area
        {
            get { return area; }
            set { area = value; }
        }

        int? numeroFuncionarios;
        public int? NumeroFuncionarios
        {
            get { return numeroFuncionarios; }
            set { numeroFuncionarios = value; }
        }

        bool? indicadorAtividade;
       
        //[InstanceClass(ErrorMessage="É obrigatório informar o Indicador de Atividade.")]
        public bool? IndicadorAtividade
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
            get { return Uf.Sigla; }
        }
        public bool DivisaoDefault { get; set; }

        public string UACodigoDescricao 
        { 
            get
            {
                if (Ua != null)
                    return Ua.CodigoDescricao;
                else
                    return "";
            }
        }

        public string UGECodigoDescricao
        {
            get { 
                    if(Ua != null && ua.Uge != null)
                        return Ua.Uge.CodigoDescricao;
                    else
                        return "";
                }
        }

        public string UOCodigoDescricao
        {
            get {
                if (Ua != null && ua.Uge != null && ua.Uge.Uo != null)
                    return Ua.Uge.Uo.CodigoDescricao;
                else
                    return "";
            }
        }

        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo.GetValueOrDefault(), Descricao, 7) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
    }
}
