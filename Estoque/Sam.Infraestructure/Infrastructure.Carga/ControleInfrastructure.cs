using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using Sam.Common.Util;

namespace Sam.Infrastructure
{
    public class ControleInfrastructure : AbstractCrud<TB_CONTROLE, SAMwebEntities>
    {

        public List<TB_CONTROLE> SelectSituacao(Expression<Func<TB_CONTROLE, DateTime?>> sortExpression, int startRowIndex)
        {
            var result = (from controle in Context.TB_CONTROLE.AsEnumerable()
                          join tipo in Context.TB_TIPO_CONTROLE on controle.TB_TIPO_CONTROLE_ID equals tipo.TB_TIPO_CONTROLE_ID
                          join situacao in Context.TB_CONTROLE_SITUACAO on controle.TB_CONTROLE_SITUACAO_ID equals situacao.TB_CONTROLE_SITUACAO_ID
                          where situacao.TB_CONTROLE_SITUACAO_ID != (int)GeralEnum.ControleSituacao.Cancelado
                          select new TB_CONTROLE
                          {
                              TB_CONTROLE_ID = controle.TB_CONTROLE_ID,
                              TB_CONTROLE_DATA_OPERACAO = controle.TB_CONTROLE_DATA_OPERACAO,
                              TB_CONTROLE_NOME_ARQUIVO = controle.TB_CONTROLE_NOME_ARQUIVO,
                              TB_TIPO_CONTROLE = (from a in Context.TB_TIPO_CONTROLE.AsEnumerable()
                                                  where controle.TB_TIPO_CONTROLE_ID == a.TB_TIPO_CONTROLE_ID
                                                  select new TB_TIPO_CONTROLE
                                                  {
                                                      TB_TIPO_CONTROLE_DESCRICAO = a.TB_TIPO_CONTROLE_DESCRICAO,
                                                      TB_TIPO_CONTROLE_ID = a.TB_TIPO_CONTROLE_ID
                                                  }).FirstOrDefault(),

                              TB_CONTROLE_SITUACAO = (from a in Context.TB_CONTROLE_SITUACAO.AsEnumerable()
                                                      where controle.TB_CONTROLE_SITUACAO_ID == a.TB_CONTROLE_SITUACAO_ID
                                                      select new TB_CONTROLE_SITUACAO
                                                  {
                                                      TB_CONTROLE_SITUACAO_DESCRICAO = a.TB_CONTROLE_SITUACAO_DESCRICAO,
                                                      TB_CONTROLE_SITUACAO_ID = a.TB_CONTROLE_SITUACAO_ID
                                                  }).FirstOrDefault()
                          }).OrderByDescending(a => a.TB_CONTROLE_DATA_OPERACAO).Take(maximumRows).Skip(startRowIndex).ToList();

            this.TotalRegistros = (from controle in Context.TB_CONTROLE.AsEnumerable()
                                   join tipo in Context.TB_TIPO_CONTROLE on controle.TB_TIPO_CONTROLE_ID equals tipo.TB_TIPO_CONTROLE_ID
                                   join situacao in Context.TB_CONTROLE_SITUACAO on controle.TB_CONTROLE_SITUACAO_ID equals situacao.TB_CONTROLE_SITUACAO_ID
                                   where situacao.TB_CONTROLE_SITUACAO_ID != (int)GeralEnum.ControleSituacao.Cancelado
                                   select new TB_CONTROLE
                          {
                              TB_CONTROLE_ID = controle.TB_CONTROLE_ID
                          }).Count();

            return result;

        }

        public List<TB_TIPO_CONTROLE> SelectTipoControle()
        {
            var result = (from tipo in Context.TB_TIPO_CONTROLE.AsEnumerable()
                          select new TB_TIPO_CONTROLE
                          {
                              TB_TIPO_CONTROLE_ID = tipo.TB_TIPO_CONTROLE_ID,
                              TB_TIPO_CONTROLE_DESCRICAO = tipo.TB_TIPO_CONTROLE_DESCRICAO
                          }).ToList();

            return result;
        }
    }
}
