using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Entity;
using Sam.Infrastructure;
using System.Text.RegularExpressions;

namespace Sam.Business
{
    public class UsuarioBusiness : BaseBusiness
    {
        private Sam.Entity.Usuario usuario = new Sam.Entity.Usuario();

        public Sam.Entity.Usuario Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }




        public int? ListarGetIdUsuarioOrgao(int? usuarioId)
        {
            UsuarioInfrastructure userinfra = new UsuarioInfrastructure();
            int? retorno = userinfra.GetIdUsuarioOrgao(usuarioId);
            return retorno;
        }



        List<string> listaErro = new List<string>();

        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }

        List<string> usuarioPerfil = new List<string>();

        public List<string> UsuarioPerfil
        {
            get { return usuarioPerfil; }
            set { usuarioPerfil = value; }
        }

        public List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();
        public List<GeralEnum.CargaErro> ListCargaErro
        {
            get { return listCargaErro; }
            set { listCargaErro = value; }
        }
        public bool ConsistidoCargaErro
        {
            get
            {
                return this.ListCargaErro.Count == 0;
            }
        }



        public static IList<Sam.Entity.Usuario> RecuperarInformacoesUsuario(string login)
        {
            return new Infrastructure.UsuarioInfrastructure().Listar(login);
        }

        public IList<Sam.Entity.Usuario> ListarUsuarios(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {

            if (PerfilId > 0)
            {
                return new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil(OrgaoId, GestorId, UgeId, AlmoxarifadoId, Convert.ToInt32(PerfilId), login, Peso, pesquisa);
            }

            //else if (PerfilId == -1)
            //{
            //    return new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil_SemPerfil(OrgaoId, GestorId, PerfilId, login, Peso, pesquisa);
            //}
            else// possivel alteração para que ele busque perfis zerados
            {
                List<Sam.Entity.Usuario> _resultado = new List<Usuario>();

                var _comPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestor(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId)).ToList();
                if (_comPerfil.Count > 0)
                    _resultado.AddRange(_comPerfil);

                if (UgeId == 0)
                {
                    var _semPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil_SemPerfil(OrgaoId, GestorId, PerfilId, login, Peso, pesquisa);
                    if (_semPerfil.Count > 0)
                        _resultado.AddRange(_semPerfil);
                }

                return _resultado;
            }
        }

        public IList<Sam.Entity.Usuario> ListarUsuariosGridExcel(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, int pagina = 0, bool gerarExcel = true)
        {
            List<Sam.Entity.Usuario> _resultado = new List<Usuario>();
            if (PerfilId > 0)
            {

                var _comPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestor(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), gerarExcel).ToList();
                if (_comPerfil.Count > 0)
                    _resultado.AddRange(_comPerfil);

                if (_resultado.Count > 0)
                {
                    _resultado = _resultado.OrderBy(o => o.NomeUsuario).ToList();
                    new Infrastructure.UsuarioInfrastructure().BuscaComplementosDosUsuariosEXCEL(_resultado, OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), pagina, gerarExcel);
                }

                return _resultado;
            }

            else// possivel alteração para que ele busque perfis zerados
            {

                var _comPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestor(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), gerarExcel).ToList();
                if (_comPerfil.Count > 0)
                    _resultado.AddRange(_comPerfil);

                if (UgeId == 0)
                {
                    var _semPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil_SemPerfilEXCEL(OrgaoId, GestorId, PerfilId, login, Peso, pesquisa, gerarExcel);
                    if (_semPerfil.Count > 0)
                        _resultado.AddRange(_semPerfil);
                }

                if (_resultado.Count > 0)
                {
                    _resultado = _resultado.OrderBy(o => o.NomeUsuario).ToList();
                    new Infrastructure.UsuarioInfrastructure().BuscaComplementosDosUsuariosEXCEL(_resultado, OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), pagina, gerarExcel);
                }

                return _resultado;
            }
        }



        public IList<Sam.Entity.Usuario> ListarUsuariosGrid(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, int pagina = 0, bool gerarExcel = true)
        {
            List<Sam.Entity.Usuario> _resultado = new List<Usuario>();
            if (PerfilId > 0)
            {
                //return new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil(OrgaoId, GestorId, UgeId, AlmoxarifadoId, Convert.ToInt32(PerfilId), login, Peso, pesquisa);


                var _comPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestor(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), gerarExcel).ToList();
                if (_comPerfil.Count > 0)
                    _resultado.AddRange(_comPerfil);

                if (_resultado.Count > 0)
                {
                    _resultado = _resultado.OrderBy(o => o.NomeUsuario).ToList();
                    new Infrastructure.UsuarioInfrastructure().BuscaComplementosDosUsuarios(_resultado, OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), pagina, gerarExcel);
                }

                return _resultado;
            }

            //else if (PerfilId == -1)
            //{
            //    return new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil_SemPerfil(OrgaoId, GestorId, PerfilId, login, Peso, pesquisa);
            //}
            else// possivel alteração para que ele busque perfis zerados
            {
                //List<Sam.Entity.Usuario> _resultado = new List<Usuario>();

                var _comPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestor(OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), gerarExcel).ToList();
                if (_comPerfil.Count > 0)
                    _resultado.AddRange(_comPerfil);

                if (UgeId == 0)
                {
                    var _semPerfil = new Infrastructure.UsuarioInfrastructure().ListarOrgarGestorPerfil_SemPerfil(OrgaoId, GestorId, PerfilId, login, Peso, pesquisa);
                    if (_semPerfil.Count > 0)
                        _resultado.AddRange(_semPerfil);
                }

                if (_resultado.Count > 0)
                {
                    _resultado = _resultado.OrderBy(o => o.NomeUsuario).ToList();
                    new Infrastructure.UsuarioInfrastructure().BuscaComplementosDosUsuarios(_resultado, OrgaoId, GestorId, UgeId, AlmoxarifadoId, login, Peso, pesquisa, Convert.ToInt32(PerfilId), pagina, gerarExcel);
                }

                return _resultado;
            }
        }

        public IList<Sam.Entity.Usuario> ListarTodosPerfis(int OrgaoId, int GestorId)
        {
            return new Infrastructure.UsuarioInfrastructure().ListarTodosPerfis(OrgaoId, GestorId);
        }

        public IList<Sam.Entity.UsuarioRelatorio> ListarTodosPerfisRelatorio(int OrgaoId, int GestorId, int? PerfilId, string pesquisa)
        {
            return new Infrastructure.UsuarioInfrastructure().ListarTodosPerfisRelatorio(OrgaoId, GestorId, PerfilId, pesquisa);
        }

        public IList<Sam.Entity.Usuario> ListarAdminGeral() => new Infrastructure.UsuarioInfrastructure().ListarAdminGeral();


        public Sam.Entity.Usuario LerDadosUsuario(int UsuarioId)
        {
            return new Infrastructure.UsuarioInfrastructure().LerRegistro(UsuarioId);
        }

        public bool SalvarUsuario()
        {
            try
            {
                Infrastructure.UsuarioInfrastructure infra = new Infrastructure.UsuarioInfrastructure();
                if (!ConsistirUsuario())
                    return false;

                //Criptografar senha
                if (!string.IsNullOrEmpty(Usuario.Login.Senha))
                    Usuario.Login.Senha = new LoginBusiness().CriptografarSenha(Usuario.Login.Senha);

                infra.Entity = Usuario;

                infra.Salvar();
                return true;
            }
            catch (Exception e)
            {
                this.ListaErro.Add("Erro no sistema:" + e.Message + "\r\n " + e.StackTrace);
                return false;
            }
        }

        public bool AlterarSenha(string NovaSenha)
        {
            try
            {
                Infrastructure.UsuarioInfrastructure infra = new Infrastructure.UsuarioInfrastructure();
                infra.Entity = Usuario;
                infra.AlterarSenha(NovaSenha);
                return true;
            }
            catch (Exception e)
            {
                this.ListaErro.Add("Erro no sistema:" + e.Message + "\r\n " + e.StackTrace);
                return false;
            }
        }

        public bool ConsistirUsuario()
        {
            listaErro = new List<string>();
            listCargaErro = new List<GeralEnum.CargaErro>();

            if (string.IsNullOrWhiteSpace(Usuario.Cpf))
            {
                this.ListaErro.Add("CPF obrigatório!");
                ListCargaErro.Add(GeralEnum.CargaErro.CPFObrigatorio);
            }

            if (string.IsNullOrWhiteSpace(Usuario.NomeUsuario))
            {
                this.ListaErro.Add("Nome do usuário obrigatório!");
                ListCargaErro.Add(GeralEnum.CargaErro.NomeObrigatorio);
            }

            //if (string.IsNullOrWhiteSpace(Usuario.Endereco))
            //{
            //     this.ListaErro.Add("Logradouro/Endereço obrigatório!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.LogradouroInvalido);
            //}

            ////if (Usuario.Numero == 0)
            ////{
            ////     this.ListaErro.Add("NÃºmero obrigatório!");
            ////     ListCargaErro.Add(GeralEnum.CargaErro.NumeroLogradouroInvalido);
            ////}

            //if (string.IsNullOrWhiteSpace(Usuario.Bairro))
            //{
            //     this.ListaErro.Add("Bairro obrigatório!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.BairroInvalido);
            //}

            //if (string.IsNullOrWhiteSpace(Usuario.Municipio))
            //{
            //     this.ListaErro.Add("MunicÃ­pio obrigatório!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.MunicipioInvalido);
            //}

            //if (string.IsNullOrWhiteSpace(Usuario.Uf))
            //{
            //     this.ListaErro.Add("UF obrigatória!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.SiglaUFInvalida);
            //}

            //if (Usuario.Cep == 0)
            //{
            //     this.ListaErro.Add("CEP obrigatório!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.CEPInvalido);
            //}

            if (!string.IsNullOrWhiteSpace(Usuario.Complemento))
            {
                if (Usuario.Complemento.Length > 30)
                {
                    this.ListaErro.Add("Complemento acima do permitido.");
                    ListCargaErro.Add(GeralEnum.CargaErro.ComplementoAcimaPermitido);
                }
            }

            if (!TratamentoDados.ValidarCPF(Usuario.Cpf))
            {
                this.ListaErro.Add("CPF Inválido!");
                ListCargaErro.Add(GeralEnum.CargaErro.CPFInvalido);
            }

            //if (Usuario.Fone == null)
            //    Usuario.Fone = "";

            //if (TratamentoDados.ValidarTelefoneComDDDComMascara(Usuario.Fone))
            //{
            //     this.ListaErro.Add("DDD Inválido!");
            //     ListCargaErro.Add(GeralEnum.CargaErro.FoneInvalido);
            //}

            Infrastructure.UsuarioInfrastructure infra = new Infrastructure.UsuarioInfrastructure();
            var _usuarios = infra.Listar(Usuario.Cpf);

            if (_usuarios != null)
            {
                if (_usuarios.Any(u => u.UsuarioAtivo && u.Id != Usuario.Id) && Usuario.UsuarioAtivo)
                {
                    var _orgaoId = _usuarios.Where(u => u.UsuarioAtivo).First();
                    var _orgao = new OrgaoBusiness().SelectOne(o => o.TB_ORGAO_ID == _orgaoId.OrgaoId);
                    if (_orgao != null)
                    {
                        this.ListaErro.Add(string.Format("CPF informado está associado a outro usuário Ativo no Sistema (Órgão: {0})", _orgao.TB_ORGAO_DESCRICAO));
                    }
                    else
                    {
                        this.ListaErro.Add(string.Format("CPF informado está associado a outro usuário Ativo no Sistema"));
                    }
                    ListCargaErro.Add(GeralEnum.CargaErro.CPFAtivoComOutroUsuario);
                }
                else if (_usuarios.Any(u => u.OrgaoId == Usuario.OrgaoPdId))
                {
                    var _usuario = _usuarios.Where(u => u.OrgaoId == Usuario.OrgaoPdId).FirstOrDefault();

                    if (((Usuario.Id == 0) || (Usuario.Id > 0 && _usuario.Id != Usuario.Id)) && Usuario.OrgaoPdId == _usuario.OrgaoId)
                    {
                        this.ListaErro.Add("CPF já cadastrado neste Órgão!");
                        ListCargaErro.Add(GeralEnum.CargaErro.CPFCadastrado);
                    }
                }
            }

            if (this.ListaErro.Count > 0)
            {
                return false;
            }

            return true;
        }

        //Traz Nome/CPF passando o TB_Login_ID
        public Usuario SelecionaUsuarioPor_LoginID(int LoginId)
        {

            Infrastructure.UsuarioInfrastructure infra = new Infrastructure.UsuarioInfrastructure();
            return infra.SelecionaUsuarioPor_LoginID(LoginId);

        }

        public Usuario ExisteCPFCadastrado(string cpf)
        {

            Infrastructure.UsuarioInfrastructure infra = new Infrastructure.UsuarioInfrastructure();

            return infra.Listar().Where(a => a.Cpf == cpf).FirstOrDefault();
        }

        public bool InsertListControleImportacao(TB_CONTROLE entityList)
        {
            StringBuilder seq = new StringBuilder();

            try
            {
                bool isErro = false;


                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    UsuarioInfrastructure infra = new UsuarioInfrastructure();

                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);


                    usuario = new Sam.Entity.Usuario();
                    usuario.Login = new Sam.Entity.Login();


                    usuario.Bairro = carga.TB_USUARIO_END_BAIRRO;
                    usuario.Cep = Convert.ToInt32(carga.TB_USUARIO_END_CEP);
                    usuario.Complemento = carga.TB_USUARIO_END_COMPL;
                    usuario.Cpf = carga.TB_USUARIO_CPF.Replace(".", "").Replace("-", "");
                    usuario.Email = carga.TB_USUARIO_EMAIL;
                    usuario.Criacao = DateTime.Now;
                    usuario.Endereco = carga.TB_USUARIO_END_RUA;
                    usuario.Fone = carga.TB_USUARIO_END_FONE;
                    usuario.GestorPdId = Convert.ToInt32(carga.TB_GESTOR_ID);
                    usuario.OrgaoPdId = Convert.ToInt32(carga.TB_ORGAO_ID);
                    usuario.NomeUsuario = carga.TB_USUARIO_NOME_USUARIO;


                    if (carga.TB_USUARIO_END_NUMERO != null)
                    {
                        long number1 = 0;
                        bool canConvert = long.TryParse(carga.TB_USUARIO_END_NUMERO, out number1);
                        if (canConvert == true)
                            usuario.Numero = Convert.ToInt16(carga.TB_USUARIO_END_NUMERO);
                        else
                        {
                            this.ListaErro.Add("Numero do endereço inválido, obrigatório valor númerico");
                            ListCargaErro.Add(GeralEnum.CargaErro.NumeroLogradouroInvalido);
                        }
                    }
                    else
                        usuario.Numero = (short)0;

                    //   usuario.Numero = carga.TB_USUARIO_END_NUMERO != null ? Convert.ToInt16(carga.TB_USUARIO_END_NUMERO) : (short)0;


                    usuario.Municipio = carga.TB_USUARIO_END_MUNIC;
                    usuario.Uf = carga.TB_USUARIO_END_UF;
                    usuario.Rg = carga.TB_USUARIO_NUM_RG;
                    usuario.RgUf = carga.TB_USUARIO_RG_UF;
                    usuario.OrgaoEmissor = carga.TB_USUARIO_ORGAO_EMISSOR;
                    usuario.Fone = carga.TB_USUARIO_END_FONE;
                    usuario.UsuarioAtivo = true;
                    usuario.Login.NumeroTentativasInvalidas = 0;
                    usuario.Login.Criacao = DateTime.Now;
                    usuario.Login.LoginAtivo = true;
                    usuario.Login.PalavraSecreta = string.Empty;
                    usuario.Login.AcessoBloqueado = false;
                    usuario.Login.TrocarSenha = true;
                    Usuario.Login.Senha = new LoginBusiness().CriptografarSenha(carga.TB_GESTOR_NOME_REDUZIDO.ToLower());

                    ConsistirUsuario();

                    if (this.ConsistidoCargaErro)
                    {
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);

                        infra.Entity = Usuario;

                        infra.Salvar();


                    }
                    else
                    {
                        foreach (GeralEnum.CargaErro erroEnum in ListCargaErro)
                        {
                            TB_CARGA_ERRO cargaErro = new TB_CARGA_ERRO();
                            CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();

                            carga.TB_CARGA_VALIDO = false;
                            cargaBusiness.Update(carga);

                            cargaErro.TB_CARGA_ID = carga.TB_CARGA_ID;
                            cargaErro.TB_ERRO_ID = (int)erroEnum;

                            infraCargaErro.Insert(cargaErro);
                            infraCargaErro.SaveChanges();
                            infraCargaErro.Dispose();



                            isErro = true;
                        }
                    }
                }
                // Salva o contexto apenas se todos os registros foram inseridos com sucesso

                return isErro;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }


        public List<Sam.Entity.Perfil> ListarPerfilLoginNivelAcessoExportacao(int OrgaoId, int GestorId, int? UgeId, int? AlmozarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            List<Sam.Entity.Perfil> result = null;
            UsuarioBusiness bussiness = new UsuarioBusiness();
            result = bussiness.ListarPerfilLoginNivelAcessoExportacao(OrgaoId, GestorId, UgeId, AlmozarifadoId, PerfilId, login, Peso, pesquisa);
            return result;
        }
    }
}
