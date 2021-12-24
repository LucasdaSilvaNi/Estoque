using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.ServiceInfraestructure;

namespace Sam.Domain.Business
{
    public class UsuarioBusiness : BaseBusiness
    {
        UsuarioEntity usuario = new UsuarioEntity();

        public UsuarioEntity Usuario
        {
            get { return usuario; }
            set { usuario = value; }
            /**/
        }

        public bool ValidarLogin()
        {
            this.ConsistirLogin();

            if (this.Consistido)
            {
                this.Service<ILoginService>().Entity = Usuario;
                                                
                if (!this.Service<ILoginService>().Validar())
                {
                    ListaErro.Add("Senha inválida");
                }
            }

            return this.Consistido;
        }

        //public IList<UsuarioEntity> ListarUsuario(string cpf)
        //{
        //    IList<UsuarioEntity> retorno = this.Service<ILoginService>().Listar(cpf);
        //    return retorno;
        //}

    

        public void ConsistirLogin()
        {
            if (string.IsNullOrEmpty(this.Usuario.Cpf.Trim()))
            {
                this.ListaErro.Add("É obrigatório informar o Login");
            }

            if (string.IsNullOrEmpty(this.Usuario.Senha.Trim()))
            {
                this.ListaErro.Add("É obrigatório informar a Senha");
            }

            if (this.ListaErro.Count == 0)
            {
                this.Service<ILoginService>().Entity = Usuario;
                
                if (!this.Service<ILoginService>().ExisteCodigoInformado())
                    ListaErro.Add("Usuário inexistente");
            }
        }
    }
}
