using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using System.Web;
using Sam.Infrastructure;
using System.Data.Objects;

namespace Sam.Presenter
{
    public class MovimentoItemPresenter : CrudPresenter<IMovimentoItemView>
    {
        public IList<MovimentoItemEntity> PopularMovimentacaoItem(int? _almoxId, long? _subItemId, int? _ugeId, DateTime _dtInicial, DateTime _dtFinal, bool comEstorno) 
        {
            MovimentoItemBusiness estrutura = new MovimentoItemBusiness();
            estrutura.MovimentoItem = new MovimentoItemEntity();
            return estrutura.ListarMovimentacaoItemPorSubItemUgeData(_almoxId, _subItemId, _ugeId, _dtInicial, _dtFinal, comEstorno);
        }

        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? almoxId)
        {
            SaldoSubItemBusiness estrutura = new SaldoSubItemBusiness();

            almoxId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            var ugeId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id ?? 0;
            if (almoxId.HasValue)
                estrutura.CarregarFormatos(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef);

            return estrutura.ListarSaldoSubItemPorLote(subItemId,ugeId, almoxId);
        }
        public IList<SaldoSubItemEntity> ListarSaldoPorLote(int? subItemId,int? almoxId)
        {
            SaldoSubItemBusiness estrutura = new SaldoSubItemBusiness();

            almoxId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            var ugeId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id ?? 0;
            if (almoxId.HasValue)
                estrutura.CarregarFormatos(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef);

            return estrutura.ListarSaldoPorLote(subItemId,ugeId, almoxId);
        }
        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? almoxId, DateTime? dataMovimento)
        {
            SaldoSubItemBusiness estrutura = new SaldoSubItemBusiness();

            almoxId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            var ugeId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id ?? 0;
            if (almoxId.HasValue)
                estrutura.CarregarFormatos(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef);

            return estrutura.ListarSaldoSubItemPorLote(subItemId, ugeId, almoxId, dataMovimento);
        }
       
        public IList<MovimentoItemEntity> ImprimirMovimentacao(int? _almoxId, int? _tipoMovimento, int? _tipoMovimentoAgrup, int? _fornecedorId, int? _divisaoId, DateTime _dtInicial, DateTime _dtFinal, bool? consultaTransf = false)
        {
            MovimentoItemBusiness estrutura = new MovimentoItemBusiness();
            var result = estrutura.ImprimirMovimentacao(_almoxId, _tipoMovimento, _tipoMovimentoAgrup, _fornecedorId, _divisaoId, _dtInicial, _dtFinal, consultaTransf);

            if (result.Count == 0)
                throw new Exception();
            else return result;
        }

        public IList<MovimentoItemEntity> ImprimirMovimentacaoPorDoc(int? _movimentoId, int? _tipoMovimento, int? _ugeId, int? _fornecedorId, int? _divisaoId, string _numeroDocumentoCombo)
        {
            MovimentoItemBusiness estrutura = new MovimentoItemBusiness();
            return estrutura.ImprimirMovimentacaoPorDoc(_movimentoId, _tipoMovimento, _ugeId, _fornecedorId, _divisaoId, _numeroDocumentoCombo);
        }

        public IList<MovimentoItemEntity> ImprimirMovimentacaoPorDoc(int? _tipoMovimento, int? _ugeId, int? _fornecedorId, int? _divisaoId, string _numeroDocumentoCombo)
        {
            MovimentoItemBusiness estrutura = new MovimentoItemBusiness();
            return estrutura.ImprimirMovimentacaoPorDoc(_tipoMovimento, _ugeId, _fornecedorId, _divisaoId, _numeroDocumentoCombo);
        }

        public IList<MovimentoItemEntity> ListarMovimentoEntradaFornecimento(int movimentoId)
        {
            MovimentoItemBusiness estrutura = new MovimentoItemBusiness();
            return estrutura.ListarMovimentoEntradaFornecimento(movimentoId);
        }

        public ObjectResult<MOVIMENTO_ITEM_SALDO_Result> ConsultarMovimentoItemSaldo()
        {
            return new Sam.Infrastructure.MovimentoItemInfrastructure().ConsultarMovimentoItemSaldo();
        }

    }
}

