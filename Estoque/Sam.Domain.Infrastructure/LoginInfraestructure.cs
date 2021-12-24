using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Configuration;


namespace Sam.Domain.Infrastructure
{
    public class LoginInfraestructure : BaseInfraestructure,ILoginService
    {
        #region ICrudBaseService<UsuarioEntity> Members

        private UsuarioEntity usuario = new UsuarioEntity();

        public UsuarioEntity Entity
        {
            get { return usuario; }
            set { usuario = value; }
        }

        public bool Validar()
        {
            //bool result = (from a in this.Db.TB_USUARIOs
            //               where (a.TB_USUARIO_CPF == this.Entity.Cpf &&
            //                      a.TB_USUARIO_SENHA == this.Entity.Senha)
            //               select new
            //               {
            //                   a.TB_USUARIO_CPF,
            //               }).Count() > 0;
            //return result;

            return true;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;

            if (!string.IsNullOrEmpty(Entity.Cpf))
            {
                //retorno = base.Db
                //    .Where(a => a.TB_USUARIO_CPF == this.Entity.Cpf)
                //    .Count() > 0;
            }

            return retorno;
        }

        public bool PodeExcluir()
        {
            return true;
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public IList<UsuarioEntity> Listar()
        {
            return new List<UsuarioEntity>();

            //List<UsuarioEntity> listUser = (from a in Db
            //                                join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
            //                                orderby a.TB_USUARIO_CPF
            //                                select new UsuarioEntity
            //                                {
            //                                   // Almoxarifado = (new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID)),
            //                                   // Cargo = (new CargoEntity(a.TB_CARGO_ID)),
            //                                   // Cpf = a.TB_USUARIO_CPF,
            //                                   //// Email = a.TB_USUARIO_EMAIL,
            //                                   // EnderecoBairro = a.TB_USUARIO_END_BAIRRO,
            //                                   // EnderecoCep = a.TB_USUARIO_END_CEP,
            //                                   // EnderecoCompl = a.TB_USUARIO_END_COMPL,
            //                                   // EnderecoFone = a.TB_USUARIO_END_FONE,
            //                                   //// EnderecoMunicipio = a.TB_USUARIO_END_MUNICIPIO,
            //                                   // EnderecoRua = a.TB_USUARIO_END_RUA,
            //                                   // EnderecoUf = (new UFEntity(a.TB_UF_ID)),
            //                                   // Gestor = (new GestorEntity
            //                                   //                { Id = a.TB_GESTOR_ID, 
            //                                   //                  Nome = b.TB_GESTOR_NOME 
            //                                   //                } ),
            //                                   // Id = a.TB_USUARIO_ID,
            //                                   // Nome = a.TB_USUARIO_NOME,
            //                                   // Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                   // QtdAcesso = a.TB_USUARIO_QTD_ACESSO,
            //                                   // QtdAcessoInvalido = a.TB_USUARIO_QTD_ACESSO_INVALIDO,
            //                                   // Rg = a.TB_USUARIO_RG,
            //                                   // RgOrgaoEmissor = a.TB_USUARIO_RG_EMISSOR,
            //                                   // RgUf = (new UFEntity(a.TB_UF_ID_EMISSAO_RG)),
            //                                   // Senha = a.TB_USUARIO_SENHA
            //                                }).ToList<UsuarioEntity>();
            //    return listUser;
        }

        public IList<UsuarioEntity> Listar(string cpf)
        {
            return new List<UsuarioEntity>();

            //List<UsuarioEntity> listUser = (from a in dbSawDataContext
            //                                join b in Db.TB_GESTORs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
            //                                where (a.TB_USUARIO_CPF == cpf)
            //                                orderby a.TB_USUARIO_CPF
            //                                select new UsuarioEntity
            //                                {
            //                                    Almoxarifado = (new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID)),
            //                                    Cargo = (new CargoEntity(a.TB_CARGO_ID)),
            //                                    //Cpf = a.TB_USUARIO_CPF,
            //                                    //Email = a.TB_USUARIO_EMAIL,
            //                                    //EnderecoBairro = a.TB_USUARIO_END_BAIRRO,
            //                                    //EnderecoCep = a.TB_USUARIO_END_CEP,
            //                                    //EnderecoCompl = a.TB_USUARIO_END_COMPL,
            //                                    //EnderecoFone = a.TB_USUARIO_END_FONE,
            //                                    //EnderecoMunicipio = a.TB_USUARIO_END_MUNICIPIO,
            //                                    //EnderecoRua = a.TB_USUARIO_END_RUA,
            //                                    //EnderecoUf = (new UFEntity(a.TB_UF_ID)),
            //                                    //Gestor = (new GestorEntity { Id = a.TB_GESTOR_ID, Nome = b.TB_GESTOR_NOME }),
            //                                    //Id = a.TB_USUARIO_ID,
            //                                    //Nome = a.TB_USUARIO_NOME,
            //                                    //Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                    //QtdAcesso = a.TB_USUARIO_QTD_ACESSO,
            //                                    //QtdAcessoInvalido = a.TB_USUARIO_QTD_ACESSO_INVALIDO,
            //                                    //Rg = a.TB_USUARIO_RG,
            //                                    //RgOrgaoEmissor = a.TB_USUARIO_RG_EMISSOR,
            //                                    //RgUf = (new UFEntity(a.TB_UF_ID_EMISSAO_RG)),
            //                                    //Senha = a.TB_USUARIO_SENHA
            //                                }).ToList<UsuarioEntity>();
            //return listUser;
        }

        public IList<UsuarioEntity> Imprimir()
        {
            throw new NotImplementedException();
        }
             
             
        #endregion


        public UsuarioEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<UsuarioEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
