using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using TipoLicitacaoEmpenho = Sam.Common.Util.GeralEnum.EmpenhoLicitacao;
using EventoLiquidacao = Sam.Common.Util.GeralEnum.LiquidacaoEvento;

namespace Sam.Business
{
    public class Liquidacao //: BaseBusiness
    {

        #region pseudo-mocks

        [Obsolete("MARCADO PARA EXTINÇÃO - NÃO UTILIZAR")]
        private string ConverterEmpenhoEventoParaLiquidacaoEvento(ref MovimentoItemEntity pObjMovimentoItem)
        {
            string lStrCodigoNaturezaDespesa = pObjMovimentoItem.SubItemMaterial.NaturezaDespesa.Codigo.ToString();
            int lIntComprimentoCodigoNatDespesa = lStrCodigoNaturezaDespesa.Length;
            int lIntTipoNaturezaDespesa = 0;
            int lIntCodigoEmpenhoEvento = 0;
            int lIntCodigoEmpenhoLicitacao = 0;
            string lStrRetorno = string.Empty;
            bool lBlnEmpenhoEventoIsNull = false;
            bool lBlnEmpenhoLicitacaoIsNull = false;
            bool lBlnIsBEC = false;
            bool lBlnMovimentoTemNotaFiscal = false;

            //lIntTipoNaturezaDespesa    = Int32.Parse(lStrCodigoNaturezaDespesa[lIntComprimentoCodigoNatDespesa - 2].ToString());
            lIntTipoNaturezaDespesa = Int32.Parse(lStrCodigoNaturezaDespesa[6].ToString());
            lBlnEmpenhoEventoIsNull = (pObjMovimentoItem.Movimento.EmpenhoEvento == null || pObjMovimentoItem.Movimento.EmpenhoEvento.Codigo == null);
            lBlnEmpenhoLicitacaoIsNull = (pObjMovimentoItem.Movimento.EmpenhoLicitacao == null || String.IsNullOrEmpty(pObjMovimentoItem.Movimento.EmpenhoLicitacao.CodigoFormatado));
            lBlnMovimentoTemNotaFiscal = (!String.IsNullOrEmpty(pObjMovimentoItem.Documento));


            if (lBlnEmpenhoEventoIsNull || lBlnEmpenhoLicitacaoIsNull)
            {
                //lIntCodigoEmpenhoEvento = (int)(this.Service<IEmpenhoEventoService>().LerRegistro(pObjMovimentoItem.Movimento) as EmpenhoEventoEntity).Codigo;
                //lIntCodigoEmpenhoLicitacao = Int32.Parse((this.Service<IEmpenhoLicitacaoService>().LerTipoLicitacaoDoMovimento(pObjMovimentoItem.Movimento.Id.Value) as EmpenhoLicitacaoEntity).CodigoFormatado);

                //pObjMovimentoItem.Movimento.EmpenhoEvento = this.Service<IEmpenhoEventoService>().LerRegistro(pObjMovimentoItem.Movimento);
                //pObjMovimentoItem.Movimento.EmpenhoLicitacao = this.Service<IEmpenhoLicitacaoService>().LerTipoLicitacaoDoMovimento(pObjMovimentoItem.Movimento.Id.Value);
            }
            else if (!lBlnEmpenhoEventoIsNull)
            {
                lIntCodigoEmpenhoEvento = (int)pObjMovimentoItem.Movimento.EmpenhoEvento.Codigo;
                pObjMovimentoItem.Movimento.EmpenhoEvento.Codigo = lIntCodigoEmpenhoEvento;
            }
            else if (!lBlnEmpenhoLicitacaoIsNull)
            {
                lIntCodigoEmpenhoEvento = Int32.Parse(pObjMovimentoItem.Movimento.EmpenhoLicitacao.CodigoFormatado);
                pObjMovimentoItem.Movimento.EmpenhoLicitacao.CodigoFormatado = lIntCodigoEmpenhoEvento.ToString();
            }

            if (!lBlnMovimentoTemNotaFiscal)
            {
                int _lMovID = pObjMovimentoItem.Movimento.Id.Value;

                //pObjMovimentoItem.Movimento.NumeroDocumento = (this.ListarDocumentos(pObjMovimentoItem.Movimento.Id.Value)
                //                                              .Where(Movimento => Movimento.Id == _lMovID)
                //                                              .ToList()
                //                                              .FirstOrDefault() as MovimentoEntity).NumeroDocumento;
            }

            lBlnIsBEC = (lIntCodigoEmpenhoEvento == (int)GeralEnum.EmpenhoEvento.BEC);


            switch (lIntCodigoEmpenhoLicitacao)
            {
                case (int)GeralEnum.EmpenhoLicitacao.Convite:
                case (int)GeralEnum.EmpenhoLicitacao.Dispensa:
                case (int)GeralEnum.EmpenhoLicitacao.Inexigivel:
                    if (lIntTipoNaturezaDespesa == 4) //Material de Consumo
                        lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialDeConsumo).Value.ToString();
                    if (lIntTipoNaturezaDespesa == 3) //Item Permanente
                        lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialPermanente).Value.ToString();
                    break;
                case (int)GeralEnum.EmpenhoLicitacao.Pregao:
                    if (lIntTipoNaturezaDespesa == 4) //Material de Consumo
                        lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialDeConsumoPregao).Value.ToString();
                    if (lIntTipoNaturezaDespesa == 3) //Item Permanente
                        lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialPermanentePregao).Value.ToString();
                    break;
            }

            if (lBlnIsBEC)
            {
                if (lIntTipoNaturezaDespesa == 4) //Material de Consumo
                    lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialDeConsumoBEC).Value.ToString();
                if (lIntTipoNaturezaDespesa == 3) //Item Permanente
                    lStrRetorno = ((int?)GeralEnum.LiquidacaoEvento.MaterialPermanenteBEC).Value.ToString();
            }

            return lStrRetorno;
        }

        #endregion pseudo-mocks

    }

    public static class LiquidacaoEmpenhoExtensionMethods
    {
        public static EventoLiquidacao _xpObterEventoEmpenhoParaLiquidacao(this MovimentoItemEntity _itemMovimentacao)
        {
            EventoLiquidacao eventoLiquidacao;

            string strNaturezaDespesa = _itemMovimentacao.SubItemMaterial.NaturezaDespesa.Codigo.ToString();
            int compNatDespesa = strNaturezaDespesa.Length;
            int tipoNaturezaDespesa = -1;
            bool isBEC = false;

            
            tipoNaturezaDespesa = (strNaturezaDespesa.Length == 8) ? Int32.Parse(strNaturezaDespesa[0].ToString()) : Int32.Parse(strNaturezaDespesa[strNaturezaDespesa.Length - 2].ToString());
            eventoLiquidacao = EventoLiquidacao.MaterialDeConsumo;

            //Verificar - Douglas
            //var tipoEmpenho = _itemMovimentacao.Movimento.RetornarDescricaoEmpenho();

            //Verificar - Douglas
            //isBEC = (tipoEmpenho == "BEC");
            var _tipoLicitacao = (TipoLicitacaoEmpenho)_itemMovimentacao.Movimento.EmpenhoLicitacao.Id.Value;

            if (isBEC)
            {
                if (tipoNaturezaDespesa == 4) //Material de Consumo
                    eventoLiquidacao = EventoLiquidacao.MaterialDeConsumoBEC;
                if (tipoNaturezaDespesa == 3) //Item Permanente
                    eventoLiquidacao = EventoLiquidacao.MaterialPermanenteBEC;
            }
            else
            {
                switch (_tipoLicitacao)
                {
                    case TipoLicitacaoEmpenho.Convite:
                    case TipoLicitacaoEmpenho.Dispensa:
                    case TipoLicitacaoEmpenho.Inexigivel:
                                                            if (tipoNaturezaDespesa == 4) //Material de Consumo
                                                                eventoLiquidacao = EventoLiquidacao.MaterialDeConsumo;
                                                            if (tipoNaturezaDespesa == 3) //Item Permanente
                                                                eventoLiquidacao = EventoLiquidacao.MaterialPermanente;
                                                            break;

                    case TipoLicitacaoEmpenho.Pregao:
                                                            if (tipoNaturezaDespesa == 4) //Material de Consumo
                                                                eventoLiquidacao = EventoLiquidacao.MaterialDeConsumoPregao;
                                                            if (tipoNaturezaDespesa == 3) //Item Permanente
                                                                eventoLiquidacao = EventoLiquidacao.MaterialPermanentePregao;
                                                            break;
                }
            }

            return eventoLiquidacao;
        }
    }


}
