using System;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class AuditoriaIntegracaoEntity : BaseEntity
    {
        public int? Id { get; set; }
        public string NomeSistema { get; set; }
        public string UsuarioSAM { get; set; }
        public string UsuarioSistemaExterno { get; set; }
        public string MsgEstimuloWS { get; set; }
        public string MsgRetornoWS { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataRetorno { get; set; }

        public AuditoriaIntegracaoEntity() { }
        public AuditoriaIntegracaoEntity(int? _id) 
        {
            this.Id = _id;
            base.Id = _id; 
        }
    }
}