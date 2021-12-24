using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;
using Sam.Common.Util;
using System.Diagnostics;
using System.Reflection;

namespace Sam.Domain.Infrastructure
{
    public class LogErroInfraestructure : BaseInfraestructure, ILogErroService
    {
        public void GravarLogErro(Exception ex)
        {
            if (ex.Message == "Thread was being aborted.")
                return;

            if (ex != null)
            {
                string TB_LOGERRO_MESSAGE;
                TB_LOGERRO_MESSAGE = ex.Message;

                TB_LOGERRO tbLogErro = new TB_LOGERRO();
                tbLogErro.TB_LOGERRO_MESSAGE = TB_LOGERRO_MESSAGE;
                tbLogErro.TB_LOGERRO_STACKTRACE = ex.StackTrace;
                tbLogErro.TB_LOGERRO_DATA = DateTime.Now;

                this.Db.TB_LOGERROs.InsertOnSubmit(tbLogErro);
                this.Db.SubmitChanges();
            }
        }

        public int QtdRegistros;
        public List<LogErroEntity> ListarLogErro(LogErroEntity logErro, int startIndex, int maxRowExibition)
        {
            IQueryable<LogErroEntity> retorno = (from a in Db.TB_LOGERROs
                           orderby a.TB_LOGERRO_DATA descending
                           select new LogErroEntity
                           {
                               Id = a.TB_LOGERRO_ID,
                               Message = a.TB_LOGERRO_MESSAGE,
                               StrackTrace = a.TB_LOGERRO_STACKTRACE,
                               Data = a.TB_LOGERRO_DATA
                           });

            this.QtdRegistros = retorno.Count();
            if (retorno.Count() < startIndex)
                startIndex = 0;
            var result = Queryable.Take(Queryable.Skip(retorno, startIndex), maxRowExibition).ToList();
            return result;
        }

        public List<LogErroEntity> PopulaListaLogErro(LogErroEntity logErro, int startIndex, int maxRowExibition)
        {
            IQueryable<LogErroEntity> retorno = (from a in Db.TB_LOGERROs
                                                 orderby a.TB_LOGERRO_DATA descending
                                                 select new LogErroEntity
                                                 {
                                                     Id = a.TB_LOGERRO_ID,
                                                     Message = a.TB_LOGERRO_MESSAGE,
                                                     StrackTrace = a.TB_LOGERRO_STACKTRACE,
                                                     Data = a.TB_LOGERRO_DATA
                                                 });

            this.QtdRegistros = retorno.Count();
            if (retorno.Count() < startIndex)
                startIndex = 0;
            var result = Queryable.Take(Queryable.Skip(retorno, startIndex), maxRowExibition).ToList();
            return result;
        }

        public bool InserirEntradaNoLog(LogErroEntity logEvento)
        {
            if (logEvento != null)
            {
                string TB_LOGERRO_MESSAGE;
                TB_LOGERRO_MESSAGE = logEvento.Message;

                TB_LOGERRO tbLogErro = new TB_LOGERRO();
                tbLogErro.TB_LOGERRO_MESSAGE = logEvento.Message;
                tbLogErro.TB_LOGERRO_STACKTRACE = logEvento.StrackTrace;
                tbLogErro.TB_LOGERRO_DATA = DateTime.Now;

                this.Db.TB_LOGERROs.InsertOnSubmit(tbLogErro);
                this.Db.SubmitChanges();
            }

            return true;
        }

        public void GravarStackTrace(string strMsgDescritivoErro)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase metodoChamador = stackTrace.GetFrame(1).GetMethod();
            string[] arrCallStack = null;


            var fmtMsgLog = (String.IsNullOrWhiteSpace(strMsgDescritivoErro)) ? "Trilha de execução:\n\n{0}" : "Evento: {0}\nTrilha de execução:\n\n{1}";

            var idxInicioCallStack = stackTrace.GetFrames().ToList().FindLastIndex(stack => stack.ToString().Contains("System.Web.UI") || stack.ToString().Contains("Page_Load"));

            arrCallStack = new string[idxInicioCallStack + 2];
            for (int idxContador = 1; idxContador < idxInicioCallStack + 1; idxContador++)
                arrCallStack.SetValue(String.Format("{0}.{1}()\n", stackTrace.GetFrame(idxContador).GetMethod().ReflectedType.FullName, stackTrace.GetFrame(idxContador).GetMethod().Name), idxContador);

            var strLogGerado = String.Format(fmtMsgLog, strMsgDescritivoErro, string.Join("", arrCallStack.Reverse()));

            LogErroEntity entradaLog = new LogErroEntity() { Message = "Procedimento trilhado para debug/análise de erros", StrackTrace = strLogGerado, Data = DateTime.Now };
            InserirEntradaNoLog(entradaLog);
        }

        public void GravarStackTrace(string strMsgDescritivoErro, bool filtraStackTraceDotNet = true)
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase metodoChamador = stackTrace.GetFrame(1).GetMethod();
            string[] arrCallStack = null;


            var fmtMsgLog = (String.IsNullOrWhiteSpace(strMsgDescritivoErro)) ? "Trilha de execução (erro detectado):\n\n{0}" : "Evento: {0}\n\n";
            var cabecalhoLog = String.Format(fmtMsgLog, strMsgDescritivoErro);

            Predicate<StackFrame> predicateMatch = (filtraStackTraceDotNet) ? (Predicate<StackFrame>)(stack => stack.ToString().Contains("System.Web.UI") || stack.ToString().Contains("Page_Load")) : (Predicate<StackFrame>)(stack => !String.IsNullOrWhiteSpace(stack.ToString()));
            var idxInicioCallStack = stackTrace.GetFrames().ToList().FindLastIndex(predicateMatch);

            arrCallStack = new string[idxInicioCallStack + 2];
            for (int idxContador = 1; idxContador < idxInicioCallStack + 1; idxContador++)
                arrCallStack.SetValue(String.Format("{0}.{1}()\n", stackTrace.GetFrame(idxContador).GetMethod().ReflectedType.FullName, stackTrace.GetFrame(idxContador).GetMethod().Name), idxContador);

            var callStack = string.Join("", arrCallStack.Reverse());
            fmtMsgLog = (!String.IsNullOrWhiteSpace(callStack)) ? (fmtMsgLog + "\n\nTrilha de Execução:\n{1}") : fmtMsgLog;

            var strLogGerado = String.Format("{0}{1}", cabecalhoLog, callStack);

            LogErroEntity entradaLog = new LogErroEntity() { Message = "Procedimento trilhado para debug/análise de erros", StrackTrace = strLogGerado, Data = DateTime.Now };
            InserirEntradaNoLog(entradaLog);
        }

        public void GravarMsgInfo(string strCabecalhoErro, string strMsgDescritivoErro)
        {
            var fmtMsgLog = (String.IsNullOrWhiteSpace(strMsgDescritivoErro)) ? "Trilha de execução (erro detectado):\n\n{0}" : "Trailed-Code: {0}\n\n";
            var txtLog = String.Format(fmtMsgLog, strMsgDescritivoErro);

            var fmtHeaderMsgLog = (String.IsNullOrWhiteSpace(strCabecalhoErro)) ? "Procedimento trilhado para debug/análise de erros" : "{0}";
            var headerLog = String.Format(fmtHeaderMsgLog, strCabecalhoErro);

            LogErroEntity entradaLog = new LogErroEntity() { Message = headerLog, StrackTrace = txtLog, Data = DateTime.Now };
            InserirEntradaNoLog(entradaLog);
        }

        public string MontaMsgCompletaErro(Exception ex)
        {
            var sb_msgErroDetalhada = new StringBuilder();
            sb_msgErroDetalhada.Append(ex.InnerException.IsNotNull() ? ("Message: " + ex.Message + " || " + "InnerException: " + ex.InnerException.Message) : ("Message: " + ex.Message));
            sb_msgErroDetalhada.Append(" || " + "StackTrace: " + ex.StackTrace);

            return sb_msgErroDetalhada.ToString();
        }
    }
}
