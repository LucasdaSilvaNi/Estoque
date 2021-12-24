using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;
using Sam.Domain.Entity;

namespace Sam.Business.Importacao
{
    public class ImportacaoCatalogo : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new SubItemMaterialAlmoxBusiness().InsertListControleImportacao(entityList);
                base.AtualizaControle(isErro, entityList.TB_CONTROLE_ID);
                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public List<GeralEnum.CargaErro> ValidarImportacao(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();            

            //Valida ItemMaterial
            //if (carga.TB_ITEM_MATERIAL_ID == null)
            //    listCargaErro.Add(GeralEnum.CargaErro.CodigoItemInvalido);
            //else
            //{
            //    if (String.IsNullOrEmpty(carga.TB_ITEM_MATERIAL_CODIGO))
            //        listCargaErro.Add(GeralEnum.CargaErro.CodigoItemInvalido);

            //    else
            //    {
            //        long? codItemLong = TratamentoDados.TryParseLong(carga.TB_ITEM_MATERIAL_CODIGO);
            //        if (codItemLong == null)
            //            listCargaErro.Add(GeralEnum.CargaErro.CodigoItemInvalido);
            //    }
            //}

            //Verifica se o subitem já está no catálogo do almoxarifado
            if (carga.TB_SUBITEM_MATERIAL_ID != null)
            {
                if (new SubItemMaterialAlmoxBusiness().SelectWhere(a => a.TB_SUBITEM_MATERIAL_ID == (int)carga.TB_SUBITEM_MATERIAL_ID
                    && a.TB_ALMOXARIFADO_ID == (int)carga.TB_ALMOXARIFADO_ID).Count() > 0)
                {
                    listCargaErro.Add(GeralEnum.CargaErro.CodigoCadastrado);
                }
            }


            return listCargaErro;
        }

        public override IQueryable<TB_CARGA> ProcessarExcel(CargaEntity cargaEntity, int loginId)
        {
            return base.ProcessarExcel(cargaEntity, loginId);
        }
    }
}
