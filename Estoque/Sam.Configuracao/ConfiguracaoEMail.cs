using Sam.Common.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;



namespace Sam.Configuracao
{
    public class ConfiguracaoEMail
    {
        //Configurações de Email
        private static SmtpClient servidorEMail = null;
        private static string enderecoServidorEMail = VariaveisWebConfigEnum.EnderecoServidorEMail;
        private static int portaEnvioEmail = VariaveisWebConfigEnum.PortaEnvioEmail;
        private static bool utilizaSSL = VariaveisWebConfigEnum.UtilizaSSL;

        private static string loginEMailSuporteSam = VariaveisWebConfigEnum.eMailParaEnvioSuporteSam;
        private static string senhaEMailSuporteSam = VariaveisWebConfigEnum.senhaEMailParaEnvioSuporteSam;

        //Configuração de Proxy
        private static string enderecoProxy = VariaveisWebConfigEnum.EnderecoProxy;

        public static SmtpClient ObterServidorEnvioEMail()
        {
            servidorEMail = new SmtpClient();
            servidorEMail.Host = enderecoServidorEMail;
            servidorEMail.Port = portaEnvioEmail;
            servidorEMail.UseDefaultCredentials = true;
            servidorEMail.Credentials = ObterCredenciais();
            servidorEMail.EnableSsl = utilizaSSL;
            servidorEMail.DeliveryMethod = SmtpDeliveryMethod.Network;

            return servidorEMail;
        }

        private WebProxy ObterServidorProxy()
        {
            var proxy = new WebProxy(enderecoProxy);
            return proxy;
        }

        private static ICredentialsByHost ObterCredenciais()
        {
            return new NetworkCredential(loginEMailSuporteSam, senhaEMailSuporteSam);
        }
    }
}
