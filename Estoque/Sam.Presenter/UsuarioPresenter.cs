using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using Sam.Business;
using System.ComponentModel;
using Sam.Business.Business.Seguranca;
using Sam.Common;
using static Sam.Common.Util.GeralEnum;
using static Sam.Common.Enums.PerfilNivelAcessoEnum;
using Sam.Domain.Entity;

namespace Sam.Presenter
{
    public class UsuarioPresenter : CrudPresenter<IUsuarioView>
    {
        public Int32 TotalRegistro { get; set; }

        IUsuarioView view;
        UsuarioBusiness estruturaUsuario = new UsuarioBusiness();
        EstruturaBusiness estrutura = new EstruturaBusiness();

        IUsuarioLogado usuarioLogado = null;

        public IUsuarioView View
        {
            get { return view; }
            set { view = value; }
        }

        public UsuarioPresenter()
        {
        }

        public int? PopularGetIdUsuarioOrgao(int usuarioId)
        {
            Sam.Business.UsuarioBusiness businessUsuario = new UsuarioBusiness();

            int? retorno = businessUsuario.ListarGetIdUsuarioOrgao(usuarioId);

            return retorno;
        }





        public UsuarioPresenter(IUsuarioView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public void Gravar()
        {

            Usuario usuario = new Usuario();
            if (!string.IsNullOrWhiteSpace(this.View.Id))
            {
                usuario = estruturaUsuario.LerDadosUsuario(Convert.ToInt32(this.View.Id));
            }
            //usuario.Bairro = this.View.Bairro;

            //if(this.View.CEP.HasValue)
            //    usuario.Cep = this.View.CEP.Value;

            //usuario.Complemento = this.View.Complemento;
            usuario.Cpf = this.View.CPF.Replace(".", "").Replace("-", "");
            usuario.Email = this.View.Email;
            //usuario.Endereco = this.View.Logradouro;

            if (this.View.Telefone.HasValue)
                usuario.Fone = this.View.Telefone.ToString();

            //usuario.Municipio = this.View.Municipio;
            usuario.NomeUsuario = this.View.NomeUsuario;

            //if(this.View.Numero.HasValue)
            //    usuario.Numero = this.View.Numero.Value;

            //usuario.OrgaoEmissor = this.View.OrgaoEmissor;
            //usuario.Rg = this.View.RG;
            //usuario.RgUf = this.View.UFEmissor;
            //usuario.Uf = this.View.UF;

            //padroes
            if (this.View.OrgaoPdId.HasValue)
                usuario.OrgaoPdId = this.View.OrgaoPdId.Value;

            if (this.View.GestorPdId.HasValue)
                usuario.GestorPdId = this.View.GestorPdId.Value;

            if (this.View.Ativo.HasValue)
                usuario.UsuarioAtivo = this.View.Ativo.Value;

            // login 
            LoginBusiness estruturaLogin = new LoginBusiness(usuario.Cpf, this.View.Senha);

            if (usuario.Id > 0)//verificar o pq desse if
            {
                usuario.Login = estruturaLogin.RecuperarInformacoesLoginPorUserId(usuario.Id);
                if (this.View.Senha != "")
                {
                    usuario.Login.Senha = this.View.Senha;
                    usuario.Login.LoginAtivo = usuario.UsuarioAtivo;
                }
                else
                {
                    usuario.Login.Senha = string.Empty;
                    usuario.Login.LoginAtivo = usuario.UsuarioAtivo;
                }
            }
            else
            {
                usuario.Login = new Login();// verificar pq passa login novamente
                if (this.View.Senha != "")
                    usuario.Login.Senha = this.View.Senha;
                else
                    usuario.Login.Senha = Sam.Common.Senha.GerarSenha();

                usuario.Login.SenhaBloqueada = false;
                usuario.Login.Criacao = DateTime.Now;
                usuario.Login.AcessoBloqueado = false;
                usuario.Login.NumeroTentativasInvalidas = 0;
                usuario.Login.PalavraSecreta = "";
                usuario.Login.LoginAtivo = usuario.UsuarioAtivo;
            }
            //Entity.Perfil _perfil = new Entity.Perfil();
            //_perfil.Id = this.View.PerfilId.Value;
            //usuario.Login.Perfil = _perfil;


            //Senha sem criptografia, usada para mostrar na mensagem para o usuário
            string senha = usuario.Login.Senha;

            estruturaUsuario.Usuario = usuario;
            if (!estruturaUsuario.SalvarUsuario())
            {
                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = estruturaUsuario.ListaErro;
                return;
            }

            if (!string.IsNullOrEmpty(senha))
                this.View.ExibirMensagem("Usuário salvo com sucesso. Senha: " + senha);

            this.View.PopularGrid();
            Cancelar();
        }

        public void Prosseguir()
        {

        }

        public void AlterarSenha(int Id, String Senha)
        {

            Usuario usuario = new Usuario();

            usuario = estruturaUsuario.LerDadosUsuario(Convert.ToInt32(Id));


            estruturaUsuario.Usuario = usuario;
            if (!estruturaUsuario.AlterarSenha(Senha))
            {
                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = estruturaUsuario.ListaErro;
                return;
            }

            this.View.ExibirMensagem("Senha alterada com sucesso.Senha: " + Senha + ".Faça o Login novamente. ");
            Cancelar();
        }

        public void GerarSenha()
        {
            this.View.Senha = Sam.Common.Senha.GerarSenha();
            StringBuilder msg = new StringBuilder();
            msg.Insert(0, this.View.Senha);
            msg.Insert(0, "Foi gerada uma nova senha de acesso ao sistema: ");
            msg.Insert(0, "Prezado Usuário " + this.View.NomeUsuario + "<br />");
            msg.Insert(0, "===========================================<br/><br/>");
            msg.Insert(0, "SAM - Sistema de Administração de Materiais <br />");
            this.View.MsgSenha = msg.ToString();
        }

        public void LerRegistro(string _Id)
        {
            Usuario usuario;
            if (!string.IsNullOrWhiteSpace(_Id))
            {
                this.View.MostrarPainelEdicao = true;
                usuario = estruturaUsuario.LerDadosUsuario(Convert.ToInt32(_Id));
                this.View.CPF = usuario.Cpf;
                this.View.NomeUsuario = usuario.NomeUsuario;
                //this.View.Logradouro = usuario.Endereco;
                //this.View.Numero = usuario.Numero;
                //this.View.Complemento = usuario.Complemento;
                //this.View.Bairro = usuario.Bairro;
                //this.View.Municipio = usuario.Municipio;
                //this.View.UF = usuario.Uf;
                //this.View.CEP = usuario.Cep;
                //this.View.RG = usuario.Rg;
                //this.View.UFEmissor = usuario.RgUf;
                //this.View.OrgaoEmissor = usuario.OrgaoEmissor;
                //this.View.Telefone = TratamentoDados.TryParseLong(usuario.Fone);
                this.View.Ativo = usuario.UsuarioAtivo;
                this.View.Senha = usuario.Login.Senha;
                this.View.Email = usuario.Email;
                this.View.BloqueiaNovo = true;
                this.View.BloqueiaGravar = false;
                this.View.BloqueiaCancelar = false;

                //this.view.OrgaoPdId = usuario.OrgaoPdId;
                //this.view.GestorPdId = usuario.GestorPdId;
            }
        }

        public void Limpar()
        {
            this.View.Ativo = false;
            //this.View.Bairro = null;
            //this.View.CEP = null;
            //this.View.Complemento = null;
            this.View.CPF = null;
            this.View.Email = null;
            this.View.Id = null;
            //this.View.Logradouro = null;
            this.View.MsgSenha = null;
            //this.View.Municipio = null;
            this.View.NomeUsuario = null;
            //this.View.Numero = null;
            //this.View.OrgaoEmissor = null;
            //this.View.RG = null;
            //this.View.Senha = null;
            //this.view.Telefone = null;
            //this.View.UF = null;
            //this.View.UFEmissor = null;
            this.View.UsuarioIdResponsavel = null;
        }

        public IList<Domain.Entity.UFEntity> PopularDadosUf()
        {
            Domain.Business.EstruturaOrganizacionalBusiness estrut = new Domain.Business.EstruturaOrganizacionalBusiness();
            return estrut.ListarUF();
        }
        public IList<Usuario> PopularDadosUsuarioGrid(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            return estruturaUsuario.ListarUsuarios(OrgaoId, GestorId, UgeId, AlmoxarifadoId, PerfilId, login, Peso, pesquisa);
        }

        //public IList<Usuario> PopularDadosUsuarioGridExcel(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, bool gerarExcel)
        //{
        //    return estruturaUsuario.ListarUsuariosGridExcel(OrgaoId, GestorId, UgeId, AlmoxarifadoId, PerfilId, login, Peso, pesquisa, gerarExcel);
        //}

        public IList<Usuario> PopularDadosUsuarioSomenteGrid(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, bool gerarExcel, int pagina = 0)
        {
            return estruturaUsuario.ListarUsuariosGrid(OrgaoId, GestorId, UgeId, AlmoxarifadoId, PerfilId, login, Peso, pesquisa, pagina, gerarExcel);
            
        }

        public IList<Usuario> PopularDadosUsuarioSomenteGridExcel(int OrgaoId, int GestorId, int? UgeId, int? AlmoxarifadoId, int? PerfilId, string login, int? Peso, string pesquisa, bool gerarExcel, int pagina = 0)
        {
            return estruturaUsuario.ListarUsuariosGridExcel(OrgaoId, GestorId, UgeId, AlmoxarifadoId, PerfilId, login, Peso, pesquisa, pagina, gerarExcel);
        }



        public void Excluir()
        {

        }

        public void Imprimir(RelatorioEnum relatorio)
        {

            RelatorioEntity relatorioImpressao = new RelatorioEntity();

            relatorioImpressao.Id = (int)relatorio;
            relatorioImpressao.Nome = relatorio == RelatorioEnum.UsuariosSemPerfil ? "rptUsuarioSemPerfil.rdlc" : "rptUsuario.rdlc";
            relatorioImpressao.DataSet = "dsUsuario";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public override void Load()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaExcluir = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.MostrarPainelEdicao = false;
        }

        public override void Novo()
        {
            this.View.BloqueiaGravar = false;
            this.View.BloqueiaCancelar = false;
            this.View.Descricao = string.Empty;
            this.Limpar();
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNovo = true;
        }

        public override void Cancelar()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaNovo = false;
            this.View.Id = null;
            this.View.Codigo = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.MostrarPainelEdicao = false;
        }

        //Traz Nome/CPF passando o TB_Login_ID
        public Usuario SelecionaUsuarioPor_LoginID(int LoginId)
        {
            UsuarioBusiness estrutura = new UsuarioBusiness();
            return estrutura.SelecionaUsuarioPor_LoginID(LoginId);
        }

        public void UsuarioLogadoCreate(UsuarioLogadoEntity usuario)
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            this.usuarioLogado.CreateUsuarioLogado(usuario);

            this.usuarioLogado = null;
        }
        public Entity.UsuarioLogadoEntity UsuarioLogadoGet(Int32 id)
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            return this.usuarioLogado.Get(id);

        }

        public List<Entity.UsuarioLogadoEntity> UsuarioLogado(Int32 maximumRowsParameterName, Int32 StartRowIndexParameterName, string cpf, int gestorId = default(int))
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            var retorno = this.usuarioLogado.UsuarioLogado(maximumRowsParameterName, StartRowIndexParameterName, cpf, gestorId);
            TotalRegistro = this.usuarioLogado.TotalRegistro;

            return retorno;
        }

        public IList<Entity.UsuarioLogadoPorGestorEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId = default(int))
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            var retorno = this.usuarioLogado.ListarTodosUsuariosLogadoPorGestor(gestorId);
            TotalRegistro = this.usuarioLogado.TotalRegistro;

            return retorno;
        }


        public IList<Entity.UsuarioLogadoEntity> ListarTodosUsuariosLogadoPorGestor(int gestorId, int perfilId = default(int))
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            var retorno = this.usuarioLogado.ListarTodosUsuariosLogadoPorGestor(gestorId, perfilId);
            TotalRegistro = this.usuarioLogado.TotalRegistro;

            return retorno;
        }
        public IList<Entity.UsuarioLogadoEntity> ListaUsuarioLogadosSessions()
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            return this.usuarioLogado.ListaUsuarioLogadosSessions();
        }
        public void RemoveUsuarioLogadoId(Int32 id)
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            this.usuarioLogado.RemoveUsuarioLogadoId(id);

            this.usuarioLogado = null;
        }

        public void RemoveUsuarioLogadoSessionId(String sessionId)
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            this.usuarioLogado.RemoveUsuarioLogadoSessionId(sessionId);

            this.usuarioLogado = null;
        }
        public void RemoveAllUsuarioLogado()
        {
            this.usuarioLogado = UsuarioLogadoBusiness.CreateInstance();

            this.usuarioLogado.RemoveAllUsuarioLogado();

            this.usuarioLogado = null;
        }

        public IList<Entity.Usuario> ListarAdminGeral()
        {
            UsuarioBusiness _usuario = new UsuarioBusiness();
            return _usuario.ListarAdminGeral();
        }

        public IList<Entity.Usuario> ListarTodosPerfis(int OrgaoId, int GestorId)
        {
            UsuarioBusiness _usuario = new UsuarioBusiness();
            return _usuario.ListarTodosPerfis(OrgaoId, GestorId);
        }

        public IList<Entity.UsuarioRelatorio> ListarTodosPerfisRelatorio(int OrgaoId, int GestorId, int? PerfilId, string pesquisa)
        {
            UsuarioBusiness _usuario = new UsuarioBusiness();
            return _usuario.ListarTodosPerfisRelatorio(OrgaoId, GestorId, PerfilId, pesquisa);
        }

        public List<Entity.UsuarioLogadoEntity> ListarUsuariosOnlinePorGestor(int gestorId)
        {
            return UsuarioLogadoBusiness.CreateInstance().ListarUsuarioLogadoPorGestor(gestorId);
        }

        public List<Entity.UsuarioLogadoEntity> ListarUsuariosOnlinePorGestor(int gestorId, PerfilNivelAcesso perfil)
        {
            return UsuarioLogadoBusiness.CreateInstance().ListarUsuarioLogadoPorGestor(gestorId, perfil);
        }

        public int ObterQtdeUsuariosOnlinePorGestorEPerfil(int gestorId, PerfilNivelAcesso perfil)
        {
            return UsuarioLogadoBusiness.CreateInstance().ObterQtdeUsuarioLogadoPorGestorEPerfil(gestorId, perfil);
        }

        public int ObterQtdeUsuariosContratadoPorPerfil(int gestorId, PerfilNivelAcesso perfil)
        {
            return UsuarioLogadoBusiness.CreateInstance().ObterQtdeUsuariosContratado(gestorId, perfil);
        }

        public bool ConsistirUsuarioOnline(int gestorId, PerfilNivelAcesso perfil)
        {
            return UsuarioLogadoBusiness.CreateInstance().ConsistirUsuariosOnline(gestorId, perfil);
        }
        public List<Entity.UsuarioLogadoEntity> ListarUsuariosOnlinePorOrgao(int orgaoId)
        {
            return UsuarioLogadoBusiness.CreateInstance().ObterUsuarioLogadoPorOrgao(orgaoId);
        }
        //public List<Sam.Entity.Perfil> ListarPerfilLoginNivelAcessoExportacao(int OrgaoId, int GestorId, int?UgeId, int ?AlmozarifadoId)
        //{
        //    List<Sam.Entity.Perfil> result = null;
        //    UsuarioBusiness bussiness = new UsuarioBusiness();
        //    result = bussiness.ListarPerfilLoginNivelAcessoExportacao(OrgaoId, GestorId, UgeId, AlmozarifadoId);
        //    return result;
        //}
        public List<Sam.Entity.Perfil> ListarPerfilLoginNivelAcessoExportacao(int OrgaoId, int GestorId, int? UgeId, int? AlmozarifadoId, int? PerfilId, string login, int? Peso, string pesquisa)
        {
            List<Sam.Entity.Perfil> result = null;
            UsuarioBusiness bussiness = new UsuarioBusiness();
            result = bussiness.ListarPerfilLoginNivelAcessoExportacao(OrgaoId, GestorId, UgeId, AlmozarifadoId, PerfilId, login, Peso, pesquisa);
            return result;
        }
    }
}
