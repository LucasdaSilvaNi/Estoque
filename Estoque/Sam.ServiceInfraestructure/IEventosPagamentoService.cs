using System;
using Sam.Domain.Entity;
using System.Collections.Generic;

namespace Sam.ServiceInfraestructure
{
    public interface IEventosPagamentoService : ICatalogoBaseService, ICrudBaseService<EventosPagamentoEntity>
    {
        bool Excluir();
        //EventosPagamentoEntity ObterEventoPagamento(MovimentoEntity objMovimentacao);
        EventosPagamentoEntity ObterEventoPagamento(MovimentoEntity objMovimentacao, bool retornaApenasSeAtivo = false);
        EventosPagamentoEntity ObterEventoPagamento(Enum tipoMovimento);
        IList<string> ListarAnoEvento();
        IList<EventosPagamentoEntity> Listar(string Ano);
		List<EventoSiafemEntity> ListarItem(int i);
        EventoSiafemEntity ObterEventoSiafem(MovimentoEntity objMovimentacao);
        EventoSiafemEntity SalvarSiafem(EventoSiafemEntity objSiafem);
        bool InativarItemEventoSiafem(int Id, string usuario);
        bool AlterarItemEventoSiafem(int Id, string usuario, string txt1, string txt2,int subtipo, int subtipoOld, bool estimulo);
        List<EventoSiafemEntity> ListarPatrimonial();
    }
}
