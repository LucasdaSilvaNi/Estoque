using Sam.Domain.Entity;



namespace Sam.View
{
    public interface IEmpenhoEventoView : ICrudView
    {
        int? Id { get; set; }
        int Codigo { get; set; }
        string Descricao { get; set; }
        string AnoBase { get; set; }
        int? CodigoEstorno { get; set; }
        bool Ativo { get; set; }
        int TipoMaterialAssociado { get; set; }

        TipoMovimentoEntity TipoMovimentoAssociado { get; set; }
        void PopularListaTiposMovimento();
    }
}
