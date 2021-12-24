using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure
{
    public class ItemMaterialInfrastructure : AbstractCrud<TB_ITEM_MATERIAL, SAMwebEntities>
    {
        /// <summary>
        /// Busca Pagindo os itens material por palavra descrição ou código.
        /// </summary>        
        /// <param name="palavraChave">Palavra Chave</param>
        /// <returns></returns>
        public IList<TB_ITEM_MATERIAL> BuscarItemMaterial(int startIndex, string palavraChave)
        {
            IQueryable<TB_ITEM_MATERIAL> result;
            result = (from m in Context.TB_ITEM_MATERIAL
                      select m);

            //Filtra palavra chave
            if (!String.IsNullOrEmpty(palavraChave))
            {
                long? codigo = Common.Util.TratamentoDados.TryParseLong(palavraChave);

                if (codigo != null)
                    result = result.Where(a => a.TB_ITEM_MATERIAL_CODIGO == codigo);
                else
                    result = result.Where(a => a.TB_ITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()));
            }

            result = result.Where(a => a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE == true);
            result = result.OrderBy(a => a.TB_ITEM_MATERIAL_DESCRICAO);
            base.TotalRegistros = result.Count();

            return Queryable.Take(Queryable.Skip(result, startIndex), 20).ToList();
        }

        
    }
}
