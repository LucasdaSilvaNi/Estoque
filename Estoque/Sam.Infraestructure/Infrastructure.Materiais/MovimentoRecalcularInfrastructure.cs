using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace Sam.Infrastructure.Infrastructure.Materiais
{
    public class MovimentoRecalcularInfrastructure : AbstractCrud<TB_SUBITEM_MATERIAL, SAMwebEntities>
    {
        private List<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> lista;
        private Int32 maximumRows = 20;

        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> RetornarListaDeMovimentoPendente(int startIndex, Int32? almoxarifadoId, Int64? subtItemCodigo)
        {
            try
            {
                if (lista == null)
                {
                    Context.CommandTimeout = 2000;
                    Context.ContextOptions.LazyLoadingEnabled = false;

                    if (almoxarifadoId.Value > 0 && subtItemCodigo.Value > 0)
                        lista = Context.retornaSamMovimentosPendentes(almoxarifadoId, subtItemCodigo).ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();
                    else if (almoxarifadoId.Value > 0 && subtItemCodigo.Value == 0)
                        lista = Context.retornaSamMovimentosPendentes(almoxarifadoId, null).ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();
                    else if (almoxarifadoId.Value == 0 && subtItemCodigo.Value > 0)
                        lista = Context.retornaSamMovimentosPendentes(null, subtItemCodigo).ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();
                    else
                        lista = Context.retornaSamMovimentosPendentes(null, null).ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();
                }

                base.TotalRegistros = lista.Count();

                if (base.TotalRegistros < (startIndex + maximumRows))
                    //startIndex = base.TotalRegistros - maximumRows;
                    maximumRows = TotalRegistros - startIndex;

                var retorno = lista.GetRange(startIndex, maximumRows);

                return retorno;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public Int32 getSubItensCount(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> lista)
        {
            return (from l in lista
                    group l by new { l.TB_SUBITEM_MATERIAL_CODIGO } into grupo
                    select new TB_SUBITEM_MATERIAL
                    {
                        TB_SUBITEM_MATERIAL_CODIGO = grupo.Key.TB_SUBITEM_MATERIAL_CODIGO
                    }).Count();
        }

        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> getListaDeMovimentoPendente()
        {
            return this.lista.ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();
        }
        public void setListaDeMovimentoPendente(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> ListaDeMovimentoPendente)
        {
            lista =(List<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>) ListaDeMovimentoPendente;
        }
        private IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> orderListaDeMovimentoPendente()
        {
            return this.lista.OrderBy(m => m.TB_ALMOXARIFADO_ID).OrderBy(m => m.TB_UGE_ID).OrderBy(m => m.TB_SUBITEM_MATERIAL_ID).OrderBy(m => m.TB_MOVIMENTO_DATA_MOVIMENTO).OrderBy(m => m.TB_MOVIMENTO_ITEM_ID).ToList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente>();

        }
    }
}
