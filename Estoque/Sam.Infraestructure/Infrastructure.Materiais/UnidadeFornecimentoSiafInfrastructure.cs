using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;
using System.Linq.Expressions;



namespace Sam.Infrastructure
{
    public class UnidadeFornecimentoSiafInfrastructure : AbstractCrud<TB_UNIDADE_FORNECIMENTO_SIAF, SAMwebEntities>
    {
        public IList<UnidadeFornecimentoSiafEntity> ListarUnidadeFornecimentoSiafisico()
        {
            List<UnidadeFornecimentoSiafEntity> lstRetorno   = new List<UnidadeFornecimentoSiafEntity>();
            List<TB_UNIDADE_FORNECIMENTO_SIAF>  lstRetornoBD = null;

            Expression<Func<TB_UNIDADE_FORNECIMENTO_SIAF, bool>> expWhere;

            expWhere = (UnidadeFornecimentoSiaf => UnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO > 0);


            lstRetornoBD = this.SelectWhere(expWhere);
            lstRetornoBD.ForEach(rowUnidadeFornecimentoSiaf => lstRetorno.Add(this.MapearDTO(rowUnidadeFornecimentoSiaf)));
            lstRetornoBD.OrderBy(UnidadeFornecimentoSiaf => UnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO);

            return lstRetorno;
        }
        public UnidadeFornecimentoSiafEntity        ObterDadosUnidadeFornecimentoSiafisico(int UnidadeFornecimentoSiaf_ID)
        {
            UnidadeFornecimentoSiafEntity         objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_SIAF          rowUnidadeFornecimentoSiaf = null;
            UnidadeFornecimentoSiafInfrastructure infraEstrutura                  = new UnidadeFornecimentoSiafInfrastructure();

            
            rowUnidadeFornecimentoSiaf = this.SelectOne(UnidadeFornecimentoSiaf => UnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO == UnidadeFornecimentoSiaf_ID);

            if (rowUnidadeFornecimentoSiaf != null)
                objRetorno = this.MapearDTO(rowUnidadeFornecimentoSiaf);


                return objRetorno;
        }
        public UnidadeFornecimentoSiafEntity        ObterDadosUnidadeFornecimentoSiafisico(string strDescricaoUnidadeFornecimentoSiaf)
        {
            UnidadeFornecimentoSiafEntity         objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_SIAF          rowUnidadeFornecimentoSiaf = null;
            UnidadeFornecimentoSiafInfrastructure infraEstrutura                  = new UnidadeFornecimentoSiafInfrastructure();


            rowUnidadeFornecimentoSiaf = this.SelectOne(UnidadeFornecimentoSiaf => UnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_DESCRICAO == strDescricaoUnidadeFornecimentoSiaf);

            if (rowUnidadeFornecimentoSiaf != null)
                objRetorno = this.MapearDTO(rowUnidadeFornecimentoSiaf);


                return objRetorno;
        }

        #region Mappers
        internal UnidadeFornecimentoSiafEntity MapearDTO(TB_UNIDADE_FORNECIMENTO_SIAF rowUnidadeFornecimentoSiaf)
        {
            UnidadeFornecimentoSiafEntity lObjRetorno = new UnidadeFornecimentoSiafEntity();

            lObjRetorno.Codigo    = rowUnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO;
            lObjRetorno.Descricao = rowUnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_DESCRICAO;

            return lObjRetorno;
        }
        #endregion Mappers
    }
}
