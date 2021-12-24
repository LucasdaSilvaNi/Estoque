using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Entity;
using Sam.Common.Util;
using Sam.Common;
using Sam.Business;
using System.ComponentModel;


namespace Sam.Presenter
{
    public class UsuarioPerfilNivelAcessoPresenter : CrudPresenter<IUsuarioPerfilNivelAcessoView>
    {
        IUsuarioPerfilNivelAcessoView view;
        EstruturaBusiness estrutura = new EstruturaBusiness();

        public IUsuarioPerfilNivelAcessoView View
        {
            get { return view; }
            set { view = value; }
        }

        private PerfilLoginNivelAcesso perfilLoginNivelAcesso = new PerfilLoginNivelAcesso();

        public PerfilLoginNivelAcesso PerfilLoginNivelAcesso
        {
            get { return perfilLoginNivelAcesso; }
            set { perfilLoginNivelAcesso = value; }
        }

        private Sam.Entity.Perfil perfil = new Sam.Entity.Perfil();

        public Sam.Entity.Perfil Perfil
        {
            get { return perfil; }
            set { perfil = value; }
        }

        private Login login = new Login();

        public Login Login
        {
            get { return login; }
            set { login = value; }
        }
 
        public UsuarioPerfilNivelAcessoPresenter()
        {
        }

        public UsuarioPerfilNivelAcessoPresenter(IUsuarioPerfilNivelAcessoView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public IList<PerfilLoginNivelAcesso> ListarPerfilLoginNivelAcesso(int _perfilId, int _loginId, int _perfilLoginId)
        {
            PerfilBusiness estruturaPerfil = new PerfilBusiness(_loginId);
            return estruturaPerfil.ListarPerfilLoginNivelAcesso(_perfilId, _perfilLoginId);
        }

        public void Gravar()
        {
            PerfilBusiness estrutura = new PerfilBusiness(Login.ID);
            PerfilLoginNivelAcessoBusiness estruturaPerfilLoginNivelAcesso = new PerfilLoginNivelAcessoBusiness();

            if (this.View.Id != null)
                this.PerfilLoginNivelAcesso = ListarPerfilLoginNivelAcesso(this.View.PerfilId.Value, Login.ID, this.View.PerfilLoginId.Value)
                .Where(a => a.IdLoginNivelAcesso == Convert.ToInt32(this.View.Id)).FirstOrDefault();
            else
                Perfil = estrutura.ListarPerfis(null).Where(a => a.IdPerfil == Convert.ToInt32(this.View.PerfilId)).FirstOrDefault();

            Perfil.IdPerfil = Convert.ToInt16(this.View.PerfilId.Value);

            estrutura.Perfil = this.Perfil;
            LoginBusiness estruturaLogin = new LoginBusiness(Login.Usuario.Cpf, Login.Senha);

            this.PerfilLoginNivelAcesso.PerfilLoginId = this.View.PerfilLoginId.Value;
            this.PerfilLoginNivelAcesso.NivelAcesso = new NivelAcesso();
            this.PerfilLoginNivelAcesso.NivelAcesso.NivelId = Convert.ToInt16(this.View.NivelAcessoId.Value);

            // pegar o valor selecionado
            string nivel = this.View.NivelAcessoId.ToString();
            
            if(nivel == Sam.Common.NivelAcessoEnum.Orgao.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.OrgaoId.Value;                    
                }
                if(nivel == Sam.Common.NivelAcessoEnum.UO.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.UoId.Value;                    
                }
                if(nivel == Sam.Common.NivelAcessoEnum.UGE.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.UgeId.Value;
                }
                if(nivel == Sam.Common.NivelAcessoEnum.UA.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.UaId.Value;
                }
                if(nivel == Sam.Common.NivelAcessoEnum.GESTOR.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.GestorId.Value;
                }
                if(nivel == Sam.Common.NivelAcessoEnum.ALMOXARIFADO.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.AlmoxId.Value;
                }
                if(nivel == Sam.Common.NivelAcessoEnum.DIVISAO.ToString())
                {
                    this.PerfilLoginNivelAcesso.Valor = this.View.DivisaoId.Value;
                }           

            estruturaPerfilLoginNivelAcesso.Login = this.Login;
            estruturaPerfilLoginNivelAcesso.Perfil = this.Perfil;
            estruturaPerfilLoginNivelAcesso.PerfilLoginNivelAcesso = this.PerfilLoginNivelAcesso;

            if (!estruturaPerfilLoginNivelAcesso.Salvar())
            {
                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = estruturaPerfilLoginNivelAcesso.ListaErro;
                return;
            }

            this.View.ExibirMensagem("Associação salva com sucesso.");
            this.View.BloqueiaNivel = false;
            this.View.PopularGrid();
            Cancelar();
        }

        public void LerRegistro(int? _idLogin, int _idLoginNivelAcesso, int? _perfilId, int? _idLoginPerfil)
        {

            if (_idLogin.HasValue)
            {
                PerfilBusiness estruturaPerfil = new PerfilBusiness(Convert.ToInt32(_idLogin));
                PerfilLoginNivelAcesso perfilNivelAcesso = estruturaPerfil.ListarPerfilLoginNivelAcesso(_perfilId.Value, _idLoginPerfil.Value).Where
                    (a => a.IdLoginNivelAcesso == _idLoginNivelAcesso).FirstOrDefault();
                if (perfilNivelAcesso != null)
                {
                    this.View.OrgaoId = perfilNivelAcesso.OrgaoId;
                    this.View.UoId = perfilNivelAcesso.UoId;
                    this.View.UgeId = perfilNivelAcesso.UgeId;
                    this.View.UaId = perfilNivelAcesso.UaId;
                    this.View.GestorId = perfilNivelAcesso.GestorId;
                    this.View.AlmoxId = perfilNivelAcesso.AlmoxId;
                    this.View.DivisaoId = perfilNivelAcesso.DivisaoId;
                }
                this.View.BloqueiaCancelar = false;
                this.View.BloqueiaGravar = false;
                this.View.BloqueiaNovo = true;
                this.View.MostrarPainelEdicao = true;
            }
        }

        public IList<Domain.Entity.UFEntity> PopularDadosUf()
        {
            Domain.Business.EstruturaOrganizacionalBusiness estrut = new Domain.Business.EstruturaOrganizacionalBusiness();
            return estrut.ListarUF();
        }

        public IList<Login> PopularDadosLoginPorUserPerfilLoginId(int _loginId, int _perfilLoginId)
        {
            PerfilBusiness perf = new PerfilBusiness(_loginId);
            return null;
//            return perf.RecuperarPerfil();
        }

        public void Excluir()
        {
            PerfilLoginNivelAcessoBusiness estruturaPerfilLoginNivelAcesso = new PerfilLoginNivelAcessoBusiness();
            estruturaPerfilLoginNivelAcesso.PerfilLoginNivelAcesso = this.PerfilLoginNivelAcesso;
            if (!estruturaPerfilLoginNivelAcesso.Excluir())
            {
                this.View.ExibirMensagem("Inconsistências encontradas.");
                this.View.ListaErros = estruturaPerfilLoginNivelAcesso.ListaErro;
                return;
            }

            this.View.ExibirMensagem("Desassociação realizada com sucesso.");
            this.View.PopularGrid();
            Cancelar();
        }

        public void Imprimir()
        {

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
            this.View.Id = null;
            this.View.NivelAcessoId = null;
            Perfil.IdPerfil = Convert.ToInt16(this.View.PerfilId.Value);
            this.View.Valor = null;
            this.View.DivisaoId = null;
            this.View.AlmoxId = null;
            this.View.GestorId = null;
            this.View.UaId = null;
            this.View.UgeId = null;
            this.View.UoId = null;
            this.View.OrgaoId = null;
            this.View.MostrarPainelEdicao = true;
            this.View.BloqueiaNivel = true;
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
            this.View.BloqueiaNivel = false;
        }

    }
}
