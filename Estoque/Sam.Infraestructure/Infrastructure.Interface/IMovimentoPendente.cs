using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Infrastructure.Infrastructure.Interface
{
    public interface IMovimentoPendente
    {
        DateTime? TB_MOVIMENTO_DATA_MOVIMENTO { get; set; }
        DateTime? TB_MOVIMENTO_DATA_DOCUMENTO { get; set; }
        Nullable<DateTime> TB_MOVIMENTO_DATA_OPERACAO { get; set; }
        String TB_MOVIMENTO_ANO_MES_REFERENCIA { get; set; }
        String TB_TIPO_MOVIMENTO_DESCRICAO { get; set; }
        Int64 TB_SUBITEM_MATERIAL_CODIGO { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_VALOR_MOV { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_SALDO_VALOR { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_PRECO_UNIT { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_DESD { get; set; }
        Int32 TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID { get; set; }
        Nullable<Boolean> TB_MOVIMENTO_ATIVO { get; set; }
        Int32 TB_MOVIMENTO_ID { get; set; }
        Int32 TB_MOVIMENTO_ITEM_ID { get; set; }
        Int32 TB_SUBITEM_MATERIAL_ID { get; set; }
        String TB_MOVIMENTO_NUMERO_DOCUMENTO { get; set; }
        Nullable<Int32> TB_UGE_ID { get; set; }
        Int32 TB_ALMOXARIFADO_ID { get; set; }
        Int32 TB_ALMOXARIFADO_CODIGO { get; set; }
        Nullable<Int32> TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO { get; set; }
        Nullable<Int32> TB_LOGIN_ID { get; set; }
        String TB_MOVIMENTO_EMPENHO { get; set; }
        String STATUS { get; set; }
		Nullable<Decimal> TB_MOVIMENTO_ITEM_QTDE_MOV { get; set; }
		Nullable<Decimal> TB_MOVIMENTO_ITEM_QTDE_MOV_CORRIGIDO { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_SALDO_QTDE { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_SALDO_QTDE_CORRIGIDO { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_VALOR_MOV_CORRIGIDO { get;set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_SALDO_VALOR_CORRIGIDO { get;set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_PRECO_UNIT_CORRIGIDO { get; set; }
        Nullable<Decimal> TB_MOVIMENTO_ITEM_DESD_CORRIGIDO { get; set; }
    }
}
