using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.ServiceInfraestructure
{
    public interface ITipoMovimentoService : ICatalogoBaseService, ICrudBaseService<TipoMovimentoEntity>
    {
        IList<TipoMovimentoEntity> Listar(int TipoMovimentoId);
        IList<TipoMovimentoEntity> ListarTodosCod(int TipoMovimentoId);
        IList<TipoMovimentoEntity> ListarTipoMovimentoTodosEntrada();
        IList<TipoMovimentoEntity> ListarTipoMovimentoTodosSaida();
        TipoMovimentoEntity LerRegistro(int MovimentoId);
        IList<TipoMovimentoEntity> ListarTipoMovimento(TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento);
        TipoMovimentoEntity ListarTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID);
        IList<TipoMovimentoEntity> RetirarTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID);
        List<SubTipoMovimentoEntity> ListarSubTipoMovimento();
        List<TipoMovimentoEntity> ListarTipoMovimentoAtivoNl();
        SubTipoMovimentoEntity ListarInserirSubTipoMovimento(SubTipoMovimentoEntity objSubTipo);
        TipoMovimentoEntity Recupera(int Id);
    }
}
