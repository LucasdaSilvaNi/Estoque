using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common.Util;
using Sam.Infrastructure;

namespace Sam.Business.Importacao
{
    public class ImportacaoClasseMaterial : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new ClasseBusiness().InsertListControleImportacao(entityList);
                base.AtualizaControle(isErro, entityList.TB_CONTROLE_ID);
                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public List<GeralEnum.CargaErro> ValidarImportacao(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida SubItemMaterial
            if (string.IsNullOrEmpty(carga.TB_SUBITEM_MATERIAL_CODIGO))
            {
                listCargaErro.Add(GeralEnum.CargaErro.CodigoSubitemInvalido);
            }
            else
            {
                long? codigoLong = Sam.Common.Util.TratamentoDados.TryParseLong(carga.TB_SUBITEM_MATERIAL_CODIGO);

                if (codigoLong == null)
                    listCargaErro.Add(GeralEnum.CargaErro.CodigoSubitemInvalido);
            }

            return listCargaErro;
        }

        #region Validações
        
        public TB_SUBITEM_MATERIAL ValidaSubItem(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do SubItem
            var subItemCodigo = TratamentoDados.TryParseLong(carga.TB_SUBITEM_MATERIAL_CODIGO);
            if (subItemCodigo != null)
            {
                var subItem = new SubItemMaterialBusiness().SelectWhere(a => a.TB_SUBITEM_MATERIAL_CODIGO == subItemCodigo
                    && a.TB_GESTOR_ID == carga.TB_GESTOR_ID).FirstOrDefault();

                if (subItem != null)
                {
                    //O SubItem existe para o gestor
                    return subItem;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }
        
        #endregion
    }
}
