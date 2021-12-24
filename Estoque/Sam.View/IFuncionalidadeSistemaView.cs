using System;



namespace Sam.View
{
    public interface IFuncionalidadeSistemaView : ICrudView
    {
        byte? Id { get; set; }
        string Descricao { get; set; }

        void PopularListaTiposMovimento();
    }
}
