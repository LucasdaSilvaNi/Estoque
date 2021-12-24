using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UsuarioEntity : BaseEntity
    {
        public UsuarioEntity() {}

        public UsuarioEntity(int? _id)
        {
            base.Id = _id;
        }
        
        string cpf;
        public string Cpf
        {
            get { return cpf; }
            set { cpf = value; }
        }

        string nome;
        public string Nome
        {
            get { return nome; }
            set { nome = value; }
        }

        string senha;
        public string Senha
        {
            get { return senha; }
            set { senha = value; }
        }

        string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        string rg;
        public string Rg
        {
            get { return rg; }
            set { rg = value; }
        }

        string rgOrgaoEmissor;
        public string RgOrgaoEmissor
        {
            get { return rgOrgaoEmissor; }
            set { rgOrgaoEmissor = value; }
        }

        UFEntity rgUf;
        public UFEntity RgUf
        {
            get { return rgUf; }
            set { rgUf = value; }
        }

        string enderecoRua;
        public string EnderecoRua
        {
            get { return enderecoRua; }
            set { enderecoRua = value; }
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
        
        UFEntity enderecoUf;
        public  UFEntity EnderecoUf
        {
            get { return enderecoUf; }
            set { enderecoUf = value; }
        }

        string enderecoCep;
        public string EnderecoCep
        {
            get { return enderecoCep; }
            set { enderecoCep = value; }
        }

        string enderecoFone;
        public string EnderecoFone
        {
            get { return enderecoFone; }
            set { enderecoFone = value; }
        }
        
        int? qtdAcesso;
        public int? QtdAcesso
        {
            get { return qtdAcesso; }
            set { qtdAcesso = value; }
        }

        int? qtdAcessoInvalido;
        public int? QtdAcessoInvalido
        {
            get { return qtdAcessoInvalido; }
            set { qtdAcessoInvalido = value; }
        }

        int flgAtivo;
        public int FlgAtivo
        {
            get { return flgAtivo; }
            set { flgAtivo = value; }
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

        CargoEntity cargo;
        public CargoEntity Cargo
        {
            get { return cargo; }
            set { cargo = value; }
        }
        
        AlmoxarifadoEntity almoxarifado;
        public AlmoxarifadoEntity Almoxarifado
        {
            get { return almoxarifado; }
            set { almoxarifado = value; }
        }        
    }
}
