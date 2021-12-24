using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;


namespace Sam.Domain.Infrastructure
{
    public class AuditoriaIntegracaoSistemaInfraestructure : BaseInfraestructure, IAuditoriaIntegracaoService
    {
        private int totalregistros
        { get; set; }

        public int TotalRegistros()
        { return totalregistros; }

        public AuditoriaIntegracaoEntity Entity { get; set; }


        public void Excluir()
        { throw new NotImplementedException(); }
        public bool PodeExcluir()
        { throw new NotImplementedException(); }
        public bool ExisteCodigoInformado()
        { throw new NotImplementedException(); }
        public AuditoriaIntegracaoEntity LerRegistro()
        { throw new NotImplementedException(); }
        public IList<AuditoriaIntegracaoEntity> ListarTodosCod()
        { throw new NotImplementedException(); }

        

        public void Salvar()
        { throw new NotImplementedException(); }
        public IList<AuditoriaIntegracaoEntity> Listar()
        { throw new NotImplementedException(); }
        public IList<AuditoriaIntegracaoEntity> Imprimir()
        { throw new NotImplementedException(); }


        public AuditoriaIntegracaoEntity ObterEventoAuditoria(int eventoAuditoriaIntegracaoID)
        {
            AuditoriaIntegracaoEntity objRetorno = null;

            objRetorno = Db.TB_AUDITORIA_INTEGRACAOs.Where(registroAuditoria => registroAuditoria.TB_AUDITORIA_INTEGRACAO_ID == eventoAuditoriaIntegracaoID)
                                                    .Select(_instanciadorDTOAuditoriaIntegracaoSistema())
                                                    .FirstOrDefault();

            return objRetorno;
        }
        public bool InserirRegistro(AuditoriaIntegracaoEntity registroAuditoriaIntegracao)
        {
            TB_AUDITORIA_INTEGRACAO rowTabela = new TB_AUDITORIA_INTEGRACAO();


            Db.TB_AUDITORIA_INTEGRACAOs.InsertOnSubmit(rowTabela);
            rowTabela.TB_AUDITORIA_INTEGRACAO_DATA_ENVIO = registroAuditoriaIntegracao.DataEnvio;
            rowTabela.TB_AUDITORIA_INTEGRACAO_DATA_RETORNO = registroAuditoriaIntegracao.DataRetorno;
            rowTabela.TB_AUDITORIA_INTEGRACAO_MSG_ESTIMULO_WS = registroAuditoriaIntegracao.MsgEstimuloWS;
            rowTabela.TB_AUDITORIA_INTEGRACAO_MSG_RETORNO_WS = registroAuditoriaIntegracao.MsgRetornoWS;
            rowTabela.TB_AUDITORIA_INTEGRACAO_NOME_SISTEMA = registroAuditoriaIntegracao.NomeSistema;
            rowTabela.TB_AUDITORIA_INTEGRACAO_USUARIO_SAM = registroAuditoriaIntegracao.UsuarioSAM;
            rowTabela.TB_AUDITORIA_INTEGRACAO_USUARIO_SISTEMA_EXTERNO = registroAuditoriaIntegracao.UsuarioSistemaExterno;

            this.Db.SubmitChanges();
            registroAuditoriaIntegracao.Id = rowTabela.TB_AUDITORIA_INTEGRACAO_ID;

            return (registroAuditoriaIntegracao.Id.HasValue);
        }

        private Func<TB_AUDITORIA_INTEGRACAO, AuditoriaIntegracaoEntity> _instanciadorDTOAuditoriaIntegracaoSistema()
        {
            Func<TB_AUDITORIA_INTEGRACAO, AuditoriaIntegracaoEntity> _actionSeletor = null;

            _actionSeletor = (registroAuditoria => new AuditoriaIntegracaoEntity()
                                                                                    {
                                                                                        Id = registroAuditoria.TB_AUDITORIA_INTEGRACAO_ID,
                                                                                        NomeSistema = registroAuditoria.TB_AUDITORIA_INTEGRACAO_NOME_SISTEMA,
                                                                                        UsuarioSAM = registroAuditoria.TB_AUDITORIA_INTEGRACAO_USUARIO_SAM,
                                                                                        UsuarioSistemaExterno = registroAuditoria.TB_AUDITORIA_INTEGRACAO_USUARIO_SISTEMA_EXTERNO,
                                                                                        DataEnvio = registroAuditoria.TB_AUDITORIA_INTEGRACAO_DATA_ENVIO,
                                                                                        DataRetorno = registroAuditoria.TB_AUDITORIA_INTEGRACAO_DATA_RETORNO,
                                                                                        MsgEstimuloWS = registroAuditoria.TB_AUDITORIA_INTEGRACAO_MSG_ESTIMULO_WS,
                                                                                        MsgRetornoWS = registroAuditoria.TB_AUDITORIA_INTEGRACAO_MSG_RETORNO_WS,
                                                                                    });

            return _actionSeletor;
        }
    }
}
