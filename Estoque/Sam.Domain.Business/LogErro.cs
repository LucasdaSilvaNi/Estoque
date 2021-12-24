using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Infrastructure;
using Sam.Domain.Entity;
using System.Diagnostics;
using System.Reflection;

namespace Sam.Domain.Business
{
    public class LogErro
    {
        public void GravarLogErro(Exception ex)
        {
            new LogErroInfraestructure().GravarLogErro(ex);
        }

        public int QtdRegistros;
        public List<LogErroEntity> ListarLogErro(LogErroEntity logErro, int startIndex, int maxRowExibition)
        {
            LogErroInfraestructure erro =  new LogErroInfraestructure();

            List<LogErroEntity> lista = erro.ListarLogErro(logErro, startIndex, maxRowExibition);
            this.QtdRegistros = erro.QtdRegistros;
           return lista;  
        }

        public List<LogErroEntity> PopularListaLogErro(LogErroEntity logErro, int startIndex, int maxRowExibition)
        {
            LogErroInfraestructure erro = new LogErroInfraestructure();

            List<LogErroEntity> lista = erro.PopulaListaLogErro(logErro
                                                                , startIndex
                                                                , maxRowExibition);
            this.QtdRegistros = erro.QtdRegistros;
            return lista;
        }

        public static bool InserirEntradaNoLog(LogErroEntity logEvento)
        {
            return new LogErroInfraestructure().InserirEntradaNoLog(logEvento);
        }

        public static void GravarStackTrace()
        {
            GravarStackTrace("");
        }

        public static void GravarStackTrace(string strMsgDescritivoErro)
        {
            new LogErroInfraestructure().GravarStackTrace(strMsgDescritivoErro);
        }
        public static void GravarStackTrace(string strMsgDescritivoErro, bool filtraStackTraceDotNet = true)
        {
            new LogErroInfraestructure().GravarStackTrace(strMsgDescritivoErro, filtraStackTraceDotNet);
        }
        public static void GravarMsgInfo(string strCabecalhoErro, string strMsgDescritivoErro)
        {
            new LogErroInfraestructure().GravarMsgInfo(strCabecalhoErro, strMsgDescritivoErro);
        }

        public static string MontaMsgCompletaErro(Exception ex)
        {
            return new LogErroInfraestructure().MontaMsgCompletaErro(ex);
        }
    }
}
