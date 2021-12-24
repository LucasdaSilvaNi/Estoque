using System;
using System.Net;

namespace Sam.View
{
    public interface ITesteWebserviceView
    {
        void TestarWebservices();
        void LimparControles();
    }

    public class clsTesteWebservice
    {
        #region Propriedades

        public static string PRODUCAO = @"https://www6.fazenda.sp.gov.br/SIAFISICO/RecebeMSG.asmx";
        public static string HOMOLOGACAO = @"https://siafemhom.intra.fazenda.sp.gov.br/siafisico/RecebeMSG.asmx";

        public string URL { get; private set; }
        public string Ambiente
        {
            //get { return ""; }
            get
            {
                string str = "";

                if (URL == PRODUCAO) { str = "Produção"; }
                if (URL == HOMOLOGACAO) { str = "Homologação"; }

                return str;
            }

            private set { }
        }
        public string Messagem { get; private set; }
        public string Erro { get; private set; }

        private clsTesteWebservice()
        { }
        public clsTesteWebservice(string pStrUrlWebservice)
        { this.URL = pStrUrlWebservice; }

        #endregion Propriedades

        #region Métodos

        //public void ObterStatusServico(string pStrEnderecoWebservice)
        public void ObterStatusServico()
        {

            string lStrRequisicaoWSDL = string.Empty;
            HttpWebRequest lObjRequisicaoWSDL = null;
            WebResponse lObjRetornoRequisicaoWSDL = null;

            try
            {
                //lStrRequisicaoWSDL = pStrEnderecoWebservice + "?wsdl";
                lStrRequisicaoWSDL = this.URL + "?wsdl";

                lObjRequisicaoWSDL = WebRequest.Create(lStrRequisicaoWSDL) as HttpWebRequest;
                //lObjRequisicaoWSDL.Timeout = 5000;

                lObjRetornoRequisicaoWSDL = lObjRequisicaoWSDL.GetResponse();

                if (lObjRetornoRequisicaoWSDL.ContentType.ToUpper() == "TEXT/XML; CHARSET=UTF-8")
                    this.Messagem = "Serviço operacional.";
                else
                {
                    this.Messagem = "Serviço inoperante.";
                    this.Erro = "Erro: Webservice não retornou XML válido.";
                }
            }
            catch (TimeoutException lObjTimeoutException)
            {
                this.Erro = "Erro de TimeOut: " + lObjTimeoutException.Message;

                if (lObjTimeoutException.InnerException != null && lObjTimeoutException.InnerException.Message != null)
                    this.Erro += "Inner Exception: " + lObjTimeoutException.InnerException.Message;

            }
            catch (Exception lObjException)
            {

                this.Erro = "Erro: " + lObjException.Message;

                if (lObjException.InnerException != null && lObjException.InnerException.Message != null)
                    this.Erro += "Inner Exception: " + lObjException.InnerException.Message;
            }

        }

        #endregion Métodos
    }
}
