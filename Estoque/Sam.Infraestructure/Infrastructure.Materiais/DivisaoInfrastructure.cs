using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;
using System.Linq.Expressions;

namespace Sam.Infrastructure
{
    public class DivisaoInfrastructure : AbstractCrud<TB_DIVISAO, SAMwebEntities>
    {
        public IList<DivisaoEntity> SelectByExpression(Expression<Func<TB_DIVISAO, bool>> _expression)
        {
            try
            {
                this.Context.ContextOptions.LazyLoadingEnabled = true;

                var query = this.Context.CreateQuery<TB_DIVISAO>(EntitySetName).Where(_expression).ToList();
                var result = (from a in query
                              orderby a.TB_DIVISAO_CODIGO
                              select new DivisaoEntity
                              {
                                  Id = a.TB_DIVISAO_ID,
                                  Codigo = a.TB_DIVISAO_CODIGO,
                                  Descricao = a.TB_DIVISAO_DESCRICAO,
                                  Responsavel = a.TB_RESPONSAVEL_ID != null ? new ResponsavelEntity(a.TB_RESPONSAVEL_ID) : new ResponsavelEntity(),
                                  Almoxarifado = a.TB_ALMOXARIFADO_ID != null ? new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID) : new AlmoxarifadoEntity(),
                                  EnderecoLogradouro = a.TB_DIVISAO_LOGRADOURO != null ? a.TB_DIVISAO_LOGRADOURO : string.Empty,
                                  EnderecoNumero = a.TB_DIVISAO_NUMERO != null ? a.TB_DIVISAO_NUMERO : string.Empty,
                                  EnderecoCompl = a.TB_DIVISAO_COMPLEMENTO != null ? a.TB_DIVISAO_COMPLEMENTO : string.Empty,
                                  EnderecoBairro = a.TB_DIVISAO_BAIRRO != null ? a.TB_DIVISAO_BAIRRO : string.Empty,
                                  EnderecoMunicipio = a.TB_DIVISAO_MUNICIPIO != null ? a.TB_DIVISAO_MUNICIPIO : string.Empty,
                                  Uf = new UFEntity(a.TB_UF_ID),
                                  EnderecoCep = a.TB_DIVISAO_CEP != null ? a.TB_DIVISAO_CEP : string.Empty,
                                  EnderecoTelefone = a.TB_DIVISAO_TELEFONE != null ? a.TB_DIVISAO_TELEFONE : string.Empty,
                                  EnderecoFax = a.TB_DIVISAO_FAX != null ? a.TB_DIVISAO_FAX : string.Empty,
                                  Area = a.TB_DIVISAO_AREA.HasValue ? Convert.ToInt32(a.TB_DIVISAO_AREA) : 0,
                                  NumeroFuncionarios = a.TB_DIVISAO_QTDE_FUNC.HasValue ? Convert.ToInt32(a.TB_DIVISAO_QTDE_FUNC) : 0,
                                  IndicadorAtividade = a.TB_DIVISAO_INDICADOR_ATIVIDADE.HasValue ? Convert.ToBoolean(a.TB_DIVISAO_INDICADOR_ATIVIDADE) : default(bool), 
                                  Ua = new UAEntity
                                  {
                                      Id = a.TB_UA.TB_UA_ID,
                                      Codigo = a.TB_UA.TB_UA_CODIGO,
                                      CodigoFormatado = a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'),
                                      CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UA_CODIGO.ToString().PadLeft(7, '0'), a.TB_UA.TB_UA_DESCRICAO),
                                      Descricao = a.TB_UA.TB_UA_DESCRICAO,
                                      Gestor = new GestorEntity
                                      {
                                          Id = a.TB_UA.TB_GESTOR.TB_GESTOR_ID,
                                          Nome = a.TB_UA.TB_GESTOR.TB_GESTOR_NOME
                                      },
                                      Uge = new UGEEntity
                                      {
                                          Id = a.TB_UA.TB_UGE.TB_UGE_ID,
                                          Codigo = a.TB_UA.TB_UGE.TB_UGE_CODIGO,
                                          CodigoFormatado = a.TB_UA.TB_UGE.TB_UGE_CODIGO.ToString().PadLeft(6, '0'),
                                          CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UGE.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), a.TB_UA.TB_UGE.TB_UGE_DESCRICAO),
                                          Descricao = a.TB_UA.TB_UGE.TB_UGE_DESCRICAO,
                                          Uo = new UOEntity
                                          {
                                              Id = a.TB_UA.TB_UGE.TB_UO.TB_UO_ID,
                                              Codigo = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO,
                                              CodigoFormatado = a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'),
                                              CodigoDescricao = string.Format("{0} - {1}", a.TB_UA.TB_UGE.TB_UO.TB_UO_CODIGO.ToString().PadLeft(6, '0'), a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO),
                                              Descricao = a.TB_UA.TB_UGE.TB_UO.TB_UO_DESCRICAO,
                                          }
                                      }
                                  }

                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }

        }

        public IList<DivisaoEntity> SelectSimplesByExpression(Expression<Func<TB_DIVISAO, bool>> _expression)
        {
            try
            {
                this.Context.ContextOptions.LazyLoadingEnabled = true;

                var query = this.Context.CreateQuery<TB_DIVISAO>(EntitySetName).Where(_expression).ToList();
                var result = (from a in query
                              orderby a.TB_DIVISAO_CODIGO
                              select new DivisaoEntity
                              {
                                  Id = a.TB_DIVISAO_ID,
                                  Codigo = a.TB_DIVISAO_CODIGO,
                                  Descricao = a.TB_DIVISAO_DESCRICAO
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, ex.InnerException);
            }

        }
    }
}
