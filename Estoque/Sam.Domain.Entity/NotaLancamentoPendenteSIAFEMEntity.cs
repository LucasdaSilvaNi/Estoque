using System;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class NotaLancamentoPendenteSIAFEMEntity : BaseEntity
    {
        public int? Id;
        public AlmoxarifadoEntity AlmoxarifadoVinculado;
        public MovimentoEntity MovimentoVinculado;
        public AuditoriaIntegracaoEntity AuditoriaIntegracaoVinculada;
        public string DocumentoSAM;
        public string ErroProcessamentoMsgWS;
        public DateTime? DataReenvioMsgWs;
        public DateTime DataEnvioMsgWs;
        public short StatusPendencia;
        public TipoNotaSIAF @TipoNotaSIAF;
        public string Tipo;


        public NotaLancamentoPendenteSIAFEMEntity() { }
        public NotaLancamentoPendenteSIAFEMEntity(int? _id)
        {
            this.Id = _id;
            base.Id = _id;
        }
    }
}