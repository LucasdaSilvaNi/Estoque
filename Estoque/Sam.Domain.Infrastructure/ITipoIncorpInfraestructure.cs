using System;
namespace Sam.Domain.Infrastructure
{
    interface ITipoIncorpInfraestructure
    {
        Sam.Domain.Entity.TipoIncorpEntity Entity { get; set; }
        void Excluir();
        bool ExisteCodigoInformado();
        System.Collections.Generic.IList<Sam.Domain.Entity.TipoIncorpEntity> Imprimir();        
        System.Collections.Generic.IList<Sam.Domain.Entity.TipoIncorpEntity> Listar();
        System.Collections.Generic.IList<Sam.Domain.Entity.TipoIncorpEntity> ListarTodosCod();
        bool PodeExcluir();
        void Salvar();
    }
}
