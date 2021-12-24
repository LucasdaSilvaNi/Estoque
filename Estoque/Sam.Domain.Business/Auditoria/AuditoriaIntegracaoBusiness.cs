using System;
using System.Collections.Generic;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Transactions;



namespace Sam.Domain.Business.Auditoria
{
    public class AuditoriaIntegracaoBusiness : BaseBusiness
    {
        public bool InserirRegistro(AuditoriaIntegracaoEntity entidadeAuditoria)
        {
            bool blnRetorno = false;

            try
            {
                using (TransactionScope trOperacaoBancoDados = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    blnRetorno = this.Service<IAuditoriaIntegracaoService>().InserirRegistro(entidadeAuditoria);

                    trOperacaoBancoDados.Complete();
                }
            }
            catch (Exception excErroOperacaoBancoDados)
            {
                var msgErro = "SAM|Erro ao inserir registro na trilha de auditoria de integração.";

                new LogErro().GravarLogErro(excErroOperacaoBancoDados);

                if (this.ListaErro.IsNotNullAndNotEmpty())
                    this.ListaErro.Add(msgErro);
                else
                    this.ListaErro = new List<string>() { msgErro };
            }

            return blnRetorno;
        }
    }
}