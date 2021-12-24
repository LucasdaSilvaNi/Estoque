using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ILoginView
    {
        string Usuario
        {
            get;
            set;
        }

        string Senha
        {
            get;
            set;
        }

        IList ListaErros
        {
           set;
        }

        string SessionId { get;}

        string TrocarPerfil
        { get; }
    }
}
