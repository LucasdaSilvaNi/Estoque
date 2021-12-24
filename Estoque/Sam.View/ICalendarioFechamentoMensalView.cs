using System;



namespace Sam.View
{
    public interface ICalendarioFechamentoMensalView : ICrudView
    {
        int? Id { get; set; }
        byte MesReferencia { get; set; }
        int AnoReferencia { get; set; }
        DateTime DataFechamentoDespesa { get; set; }
        void PopularDdlAno();
    }
}
