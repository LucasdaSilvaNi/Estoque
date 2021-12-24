using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Data.Objects.DataClasses;
using Sam.Common.Util;
using System.Linq.Expressions;

namespace Sam.Business.Importacao
{
    public class ImportacaoInventario : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new MovimentoBusiness().InsertListControleImportacao(entityList);
                base.AtualizaControle(isErro, entityList.TB_CONTROLE_ID);
                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<Sam.Common.Util.GeralEnum.CargaErro> ValidarImportacao(Infrastructure.TB_CARGA carga)
        {
            throw new NotImplementedException();
        }

        public bool IsDocumentoCadastrado(string documento)
        {
            if (String.IsNullOrEmpty(documento))
            {
                return true;
            }

            var objDoc = new MovimentoBusiness().SelectOne(a => a.TB_MOVIMENTO_NUMERO_DOCUMENTO.Trim() == documento.Trim());
            if (objDoc != null)
            {
                return true;
            }
            else
                return false;
        }

        public TB_FORNECEDOR RetornaFornecedorInventarioPadrao()
        {
            string cpfFornecedorPadrao = "99999999999999";
            var fornecedorBusiness = new FornecedorBusiness();

            var fornecedorPadrao = fornecedorBusiness.SelectOne(a => a.TB_FORNECEDOR_CPFCNPJ == cpfFornecedorPadrao);

            if (fornecedorPadrao == null)
            {
                //Caso não exista o fornecedor cadastrado, insere no banco
                TB_FORNECEDOR novoFornecedorPadrao = new TB_FORNECEDOR();
                novoFornecedorPadrao.TB_FORNECEDOR_NOME = "Transferencia de saldo.";
                novoFornecedorPadrao.TB_FORNECEDOR_CPFCNPJ = cpfFornecedorPadrao;

                fornecedorBusiness.Insert(novoFornecedorPadrao);

                return novoFornecedorPadrao;
            }

            else
            {
                return fornecedorPadrao;
            }

        }

        public bool ValidarData(string data)
        {
            try
            {
                var dataObj = Sam.Common.Util.TratamentoDados.TryParseDateTime(data);

                if (dataObj == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        //public TB_MOVIMENTO_ITEM MontarMovimentoItem(TB_CARGA carga)
        //{
        //    TB_SUBITEM_MATERIAL subItem = new ImportacaoSubItemMaterial().ValidaSubItem(carga);
        //    TB_MOVIMENTO_ITEM movimentoItem = new TB_MOVIMENTO_ITEM();

        //    if (subItem != null)
        //    {
        //        movimentoItem.TB_SUBITEM_MATERIAL_ID = subItem.TB_SUBITEM_MATERIAL_ID;
        //        carga.TB_SUBITEM_MATERIAL_ID = subItem.TB_SUBITEM_MATERIAL_ID;
        //    }
        //    else
        //    {
        //        //ListCargaErro.Add(GeralEnum.CargaErro.CodigoSubItemNaoCadastradoGestor);
        //    }

        //    //movimentoItem.TB_UGE_ID = carga.TB_UGE_ID;

        //    return movimentoItem;
        //}

        public bool ExisteSaldoSubItem(TB_CARGA carga)
        {
            int count = new SaldoSubItemBusiness().GetCount(a => a.TB_SUBITEM_MATERIAL_ID == carga.TB_SUBITEM_MATERIAL_ID &&
                a.TB_UGE_ID == carga.TB_UGE_ID && a.TB_ALMOXARIFADO_ID == carga.TB_ALMOXARIFADO_ID);

            if (count > 0)
                return true;
            else
                return false;
        }

        //public List<TB_CARGA> CalculaPrecoMedio(EntityCollection<TB_CARGA> listcarga)
        //{

        //    listcarga.Sum(a => a.TB_SUBITEM_MATERIAL_ID);
        //    //.GroupBy(a => a.TB_UGE_ID).GroupBy(a => a.TB_ALMOXARIFADO_ID);

        //    foreach (var carga in listcarga)
        //    {

        //        var result = (from a in listcarga.AsEnumerable()
        //                      where a.TB_SUBITEM_MATERIAL_ID == carga.TB_SUBITEM_MATERIAL_ID
        //                      where a.TB_UGE_ID == carga.TB_UGE_ID
        //                      where a.TB_ALMOXARIFADO_ID == carga.TB_ALMOXARIFADO_ID
        //                      group a by a.TB_SUBITEM_MATERIAL_ID into aGroup
        //                      select new TB_CARGA
        //                      {   
        //                          TB_SALDO_SUBITEM_SALDO_VALOR = aGroup.Sum(b => Convert.ToDecimal(b.TB_SALDO_SUBITEM_SALDO_VALOR)).ToString(),
        //                          TB_SALDO_SUBITEM_SALDO_QTDE = aGroup.Sum(b => Convert.ToDecimal(b.TB_SALDO_SUBITEM_SALDO_QTDE)).ToString()
        //                      }).FirstOrDefault();


        //        decimal saldoValor = Convert.ToDecimal(carga.TB_SALDO_SUBITEM_SALDO_VALOR);
        //        decimal saldoQtd = Convert.ToDecimal(carga.TB_SALDO_SUBITEM_SALDO_QTDE);

        //        if (Convert.ToDecimal(result.TB_SALDO_SUBITEM_SALDO_QTDE) > 0)
        //        {
        //            decimal precoMedio = Convert.ToDecimal(result.TB_SALDO_SUBITEM_SALDO_VALOR) / Convert.ToDecimal(result.TB_SALDO_SUBITEM_SALDO_QTDE);
        //        }
        //    }

        //    return null;
        //}

        public decimal? CalcularPrecoUnitario(TB_CARGA carga)
        {
            try
            {
                var TB_MOVIMENTO_ITEM_QTDE_MOV = TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_QTDE_MOV);
                var TB_MOVIMENTO_ITEM_VALOR_MOV = TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV, true);
                var result = TB_MOVIMENTO_ITEM_VALOR_MOV / TB_MOVIMENTO_ITEM_QTDE_MOV;
                //double precoUnit2Casas = ((double)((int)((double)result * 100.0))) / 100.0;
                //decimal precoUnit2Casas = result.Value.Truncar(2, true);
				decimal precoUnit = result.Value.Truncar(carga.TB_MOVIMENTO_ANO_MES_REFERENCIA, true);

                return precoUnit;
            }
            catch
            {
                return null;
            }

        }

        public decimal? CalcularPrecoUnitarioLote(IList<TB_SALDO_SUBITEM> saldoList, TB_CARGA carga)
        {
            try
            {
                var TB_MOVIMENTO_ITEM_QTDE_MOV_SOMA = _valorZero;
                var TB_MOVIMENTO_ITEM_VALOR_MOV_SOMA = 0.00m;

                //soma os valores dos saldos existentes
                foreach (var saldo in saldoList)
                {
                    TB_MOVIMENTO_ITEM_QTDE_MOV_SOMA += saldo.TB_SALDO_SUBITEM_SALDO_QTDE.Value;
                    TB_MOVIMENTO_ITEM_VALOR_MOV_SOMA += saldo.TB_SALDO_SUBITEM_SALDO_VALOR.Value;
                }
                //soma os valores do novo movimento
                

                TB_MOVIMENTO_ITEM_QTDE_MOV_SOMA += TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_QTDE_MOV).Value;

//                TB_MOVIMENTO_ITEM_QTDE_MOV_SOMA += TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_QTDE_MOV).Value;

                TB_MOVIMENTO_ITEM_VALOR_MOV_SOMA += TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV).Value;

                var result = TB_MOVIMENTO_ITEM_VALOR_MOV_SOMA / TB_MOVIMENTO_ITEM_QTDE_MOV_SOMA;
                //double precoUnit2Casas = ((double)((int)((double)result * 100.0))) / 100.0;
                //decimal precoUnit2Casas = result.Value.Truncar(2, true);
				decimal precoUnit  = result.Truncar(carga.TB_MOVIMENTO_ANO_MES_REFERENCIA, true);
                
                return precoUnit;
            }
            catch
            {
                return null;
            }
        }

        public decimal? CalcularDesdobro(TB_CARGA carga)
        {
            try
            {
                 var TB_MOVIMENTO_ITEM_QTDE_MOV = TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_QTDE_MOV);
                var TB_MOVIMENTO_ITEM_VALOR_MOV = TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV);
                var precoMedio = TB_MOVIMENTO_ITEM_VALOR_MOV / TB_MOVIMENTO_ITEM_QTDE_MOV;

                var desdrobro = precoMedio - CalcularPrecoUnitario(carga);

                return desdrobro;
            }
            catch
            {
                return null;
            }

        }

        public bool ValidarInventarioCadastrado(TB_CARGA carga)
        {
            var TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = TratamentoDados.TryParseDateTime(carga.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC);

            Expression<Func<TB_SALDO_SUBITEM, bool>> where = a => a.TB_ALMOXARIFADO_ID == carga.TB_ALMOXARIFADO_ID
            && a.TB_UGE_ID == carga.TB_UGE_ID
            && a.TB_SUBITEM_MATERIAL_ID == carga.TB_SUBITEM_MATERIAL_ID
            && (a.TB_SALDO_SUBITEM_LOTE_DT_VENC == TB_MOVIMENTO_ITEM_LOTE_DATA_VENC || TB_MOVIMENTO_ITEM_LOTE_DATA_VENC == null)
            && (a.TB_SALDO_SUBITEM_LOTE_FAB == carga.TB_SALDO_SUBITEM_LOTE_FAB || string.IsNullOrEmpty(carga.TB_SALDO_SUBITEM_LOTE_FAB))
            && (a.TB_SALDO_SUBITEM_LOTE_IDENT == carga.TB_SALDO_SUBITEM_LOTE_IDENT || string.IsNullOrEmpty(carga.TB_SALDO_SUBITEM_LOTE_IDENT));

            var count = new SaldoSubItemBusiness().GetCount(where);

            if (count > 0)
                return false;
            else
                return true;
        }

        public IList<TB_SALDO_SUBITEM> VerificaExisteSaldo(TB_CARGA carga)
        {
            Expression<Func<TB_SALDO_SUBITEM, bool>> where = a =>
            a.TB_ALMOXARIFADO_ID == carga.TB_ALMOXARIFADO_ID
            && a.TB_UGE_ID == carga.TB_UGE_ID
            && a.TB_SUBITEM_MATERIAL_ID == carga.TB_SUBITEM_MATERIAL_ID;

            return new SaldoSubItemBusiness().SelectWhere(where);           
        }
    }
}
