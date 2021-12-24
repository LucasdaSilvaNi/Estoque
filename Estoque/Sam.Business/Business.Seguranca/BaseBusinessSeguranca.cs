using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common.Util;

namespace Sam.Business
{
    public class BaseBusinessSeguranca : BaseBusiness
    {
        public static decimal _valorZero = 0.0000m; 

        public string _usuario { get; set; }
        public string _senha { get; set; }
        public int _loginId { get; set; }


        public BaseBusinessSeguranca(string usuario, string senha, int loginId)
        {
            this._usuario = usuario;
            this._senha = senha;
            this._loginId = loginId;
        }
        public BaseBusinessSeguranca(string usuario, string senha)
            :this(usuario, senha, 0)
        {}

        public BaseBusinessSeguranca(string login)
            :this(login, string.Empty, default(int))
        {}

        public BaseBusinessSeguranca(int loginId)
            : this(string.Empty, string.Empty, loginId)
        { }

        public BaseBusinessSeguranca()
        { }

        List<string> listaErro = new List<string>();

        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }

    }
}
