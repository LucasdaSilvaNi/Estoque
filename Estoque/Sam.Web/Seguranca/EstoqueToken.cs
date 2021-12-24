using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Sam.Web.Seguranca
{
    public class EstoqueToken
    {
       // private const String UrlPatrimonio = "sam.homologacao.sp.gov.br";
        private const String UrlPatrimonio = "10.200.240.5";
        
        private EstoqueToken() { encoding = new ASCIIEncoding(); }
        private String token = String.Empty;
        private String login = String.Empty;
        private DateTime? dataDoToken = null;
        private Guid? tokenKey = null;
        private String senha = String.Empty;
        private String usuario = string.Empty;

        private ASCIIEncoding encoding = null;

        public static EstoqueToken getEstoqueToken()
        {
            return new EstoqueToken();
        }

        public static String GetUrlPatrimonio()
        {
            return UrlPatrimonio;
        }
        public void CreateToken()
        {

            Byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            Byte[] key = Guid.NewGuid().ToByteArray();

            this.token = Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public void DecodeToken(String token)
        {
            byte[] data = Convert.FromBase64String(token);
            Byte[] keyArray = new Byte[16];

            for (int i = 8; i <= 23; i++)
            {
                keyArray[i - 8] = data[i];
            }

            DateTime dateTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            Guid key = new Guid(keyArray);

            dataDoToken = dateTime;
            tokenKey = key;
        }

        public void CreateLogin(String usuario, String senha)
        {

            string usuarioParametro = usuario +";";

            Byte[] _usuario = encoding.GetBytes(usuarioParametro);
            Byte[] _senha = encoding.GetBytes(senha);

            this.login = Convert.ToBase64String(_usuario.Concat(_senha).ToArray());
        }

        public void DecodeLogin(String login)
        {
            byte[] data = Convert.FromBase64String(login);

            string _login = encoding.GetString(data);
            char[] _split ={';'};

            string[] arrayLogin = _login.Split(_split);

            this.usuario = arrayLogin[0];
            this.senha = arrayLogin[1];

        }

        public String  getToken()
        {
            return this.token;
        }

        public String getLogin()
        {
            return this.login;
        }

        public DateTime? getDataDoToken()
        {
            return this.dataDoToken;
        }

        public Guid? getTokenKey()
        {
            return this.tokenKey;
        }

        public String getSenha()
        {
            return this.senha;
        }
        public String getUsuario()
        {
            return this.usuario;
        }

    }
}