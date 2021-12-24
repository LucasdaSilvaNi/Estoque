using System;
using System.Net;

namespace Sam.View
{
    public interface ITransacaoPerfilView : ICrudView
    {
        void PopularListaEdit();
        void PopularListaModulo();

        int ID { get; set; }
        int TRANSACAO_ID { get; set; }
        short PERFIL_ID { get; set; }
        bool ATIVO { get; set; }
        bool EDITA { get; set; }
        bool? FILTRA_COMBO { get; set; }
    }
}
