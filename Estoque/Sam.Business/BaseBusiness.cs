using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using Sam.Entity;
using Sam.Common.Util;

namespace Sam.Business
{
    public class BaseBusiness: BaseLogin
    {

        public static decimal _valorZero = 0.0000m; 

        public BaseBusiness()
        {
            
        }
        
        #region Propriedades

        protected string ErroSistema
        {
            get
            {
                return "Erro no sistema: ";
            }
        }

        public int SkipRegistros { get; set; }
        
        public int TotalRegistros
        {
            get;
            set;
        }

        #endregion

        List<string> listaErro = new List<string>();

        public List<string> ListaErro
        {
            get { return listaErro; }
            set { listaErro = value; }
        }

        public  bool Consistido
        {
            get
            {
                return this.ListaErro.Count == 0;
            }
        }
        
        public void TratarErro(Exception message)
        {
            //new LogErro().GravarLogErro(message);
            //string mensagem = message.Message;
            //if (message.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            //    mensagem = "Esse registro não pode ser excluido! Existem informações relacionadas.";
            //throw new Exception(mensagem);
        }

        public void GravarLogErro(Exception ex)
        {
            LogErroInfrastructure infraestrutura = new LogErroInfrastructure();

            TB_LOGERRO logErro = new TB_LOGERRO();
            logErro.TB_LOGERRO_DATA = DateTime.Now;
            logErro.TB_LOGERRO_MESSAGE = ex.Message;
            logErro.TB_LOGERRO_STACKTRACE = ex.StackTrace;

            infraestrutura.Insert(logErro);
            infraestrutura.SaveChanges();
        }

        public List<TB_LOGERRO> ListarLogErro()
        {
            LogErroInfrastructure infraestrutura = new LogErroInfrastructure();
            return infraestrutura.SelectAll();
        }
    }
}
