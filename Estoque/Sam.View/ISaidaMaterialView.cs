using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ISaidaMaterialView : ICrudView, IMovimentoView, IMovimentoItemView
    {

        # region Controlar Botoes

        bool BloqueiaListaUA { set; }
        bool BloqueiaListaDivisao { set; }
        bool BloqueiaNovoItem { set; }
        void GravadoSucessoAtualizar();

        # endregion

        #region Propriedades

        bool isRascunho { get; set; }
        string Unidade { set; }
        string Saldo { set; }
        string QtdFornecida { set; }

        #endregion

        #region Metodos

        void PopularListaDivisao();
        void PopularListaTipoMovimentoSaida();
        void PopularListaUGE();
        void PopularListaLote(int almoxarifadoId);

        #endregion


        int rascunhoId { get; set; }
        bool BloqueiaRascunho { set; }
        int? PTResCodigo { get; set; }
        string InscricaoCE { get; set; }
        bool ModoGravacaoOuEstorno { get; }
    }
}
