using System;



namespace Sam.View
{
    public interface INotaLancamentoPendenteSIAFEMView : ICrudView
    {

        int? Id { get; set; }
        int AlmoxarifadoID { get; set; }
        int MovimentoID { get; set; }
        int AuditoriaIntegracaoID { get; set; }
        string DocumentoSAM { get; set; }
        string ErroSIAFEM { get; set; }
        DateTime DataReenvioMsgWs { get; set; }
        DateTime DataEnvioMsgWs { get; set; }
    }
}
