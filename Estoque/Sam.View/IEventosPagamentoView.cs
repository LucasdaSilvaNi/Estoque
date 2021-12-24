using Sam.Domain.Entity;



namespace Sam.View
{
    public interface IEventosPagamentoView : ICrudView
    {
        int? Id { get; set; }
        
        int PrimeiroCodigo { get; set; }
        int? PrimeiroCodigoEstorno { get; set; }
        string PrimeiraInscricao { get; set; }
        string PrimeiraClassificacao { get; set; }

        int SegundoCodigo { get; set; }
        int? SegundoCodigoEstorno { get; set; }
        string SegundaInscricao { get; set; }
        string SegundaClassificacao { get; set; }


        string AnoBase { get; set; }
        bool Ativo { get; set; }
        bool UGFavorecida { get; set; }
        int TipoMaterialAssociado { get; set; }

        TipoMovimentoEntity TipoMovimentoAssociado { get; set; }
        void PopularListaTiposMovimento();
        void PopularDdlAno();
    }
}
