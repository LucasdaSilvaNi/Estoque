using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;
using Sam.Domain.Entity.DtoWs;





namespace Sam.Infrastructure
{
    public class SubItemMaterialInfrastructure : AbstractCrud<TB_SUBITEM_MATERIAL, SAMwebEntities>
    {
        //public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterial(int startIndex, string palavraChave, bool filtraAlmoxarifado, bool filtraGestor, bool comSaldo, TB_ALMOXARIFADO almoxarifado)
        public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterial(int startIndex, string palavraChave, bool filtraAlmoxarifado, bool filtraGestor, bool comSaldo, TB_ALMOXARIFADO almoxarifado, bool filtraNaturezasDespesaConsumoImediato, int almox =0, int gestor = 0)
        {
            IQueryable<TB_SUBITEM_MATERIAL> result;
            Context.ContextOptions.LazyLoadingEnabled = true;

            //Filtra os itens com saldo do almoxarifado
            if (comSaldo)
            {
                result = (from s in Context.TB_SALDO_SUBITEM
                          join r in Context.TB_RESERVA_MATERIAL on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID }
                          equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                          from r in x.DefaultIfEmpty()
                          join si in Context.TB_SUBITEM_MATERIAL on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                          where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                          where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                 && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                 || (r.TB_RESERVA_MATERIAL_QUANT == null))
                          //where (s.TB_ALMOXARIFADO_ID.Equals(almoxarifado.TB_ALMOXARIFADO_ID))
                          select si).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();
            }
            else
            {

                //Filtra o almoxarifado
                if (filtraAlmoxarifado)
                {
                    long? codigo = Common.Util.TratamentoDados.TryParseLong(palavraChave);
                    //result = (from a in Context.TB_SUBITEM_MATERIAL_ALMOX
                    //          where a.TB_ALMOXARIFADO_ID == almoxarifado.TB_ALMOXARIFADO_ID
                    //          where a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                    //          select a.TB_SUBITEM_MATERIAL).AsQueryable<TB_SUBITEM_MATERIAL>();
                    result = (from a in Context.TB_SUBITEM_MATERIAL_ALMOX
                              join saldo in Context.TB_SUBITEM_MATERIAL
                                 on a.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                              // where a.TB_ALMOXARIFADO_ID == almox
                              where saldo.TB_GESTOR_ID == gestor
                              where saldo.TB_SUBITEM_MATERIAL_CODIGO == codigo
                              where saldo.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                              select a.TB_SUBITEM_MATERIAL).AsQueryable<TB_SUBITEM_MATERIAL>().Distinct();
                }
                else
                {
                    result = (from a in this.Context.TB_SUBITEM_MATERIAL
                              select a).AsQueryable<TB_SUBITEM_MATERIAL>();
                }
            }

            //PARA TODOS

            //Filtra palavra chave
            if (!String.IsNullOrEmpty(palavraChave))
            {
                long? codigo = Common.Util.TratamentoDados.TryParseLong(palavraChave);
                if (!filtraGestor)
                {
                    if (codigo != null)
                    {
                        //  result = result.Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == codigo);

                        result = (from a in Context.TB_SUBITEM_MATERIAL_ALMOX
                                  join saldo in Context.TB_SUBITEM_MATERIAL
                                     on a.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                                  // where a.TB_ALMOXARIFADO_ID == almox
                                  where saldo.TB_GESTOR_ID == gestor
                                  where saldo.TB_SUBITEM_MATERIAL_CODIGO == codigo
                                  where saldo.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                  select a.TB_SUBITEM_MATERIAL).AsQueryable<TB_SUBITEM_MATERIAL>().Distinct();


                    }
                    else
                        // result = result.Where(a => a.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()));
                        result = (from s in Context.TB_SUBITEM_MATERIAL
                                  join saldo in Context.TB_SALDO_SUBITEM
                                  on s.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                                  where s.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()) && saldo.TB_ALMOXARIFADO_ID == almoxarifado.TB_ALMOXARIFADO_ID
                                  select s).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();
                 
                }
                else
                {

                    if (codigo != null)
                    {
                        //result = result.Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == codigo);
                        result = (from a in Context.TB_SUBITEM_MATERIAL_ALMOX
                                  join saldo in Context.TB_SUBITEM_MATERIAL
                                     on a.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                                 // where a.TB_ALMOXARIFADO_ID == almox
                                  where saldo.TB_GESTOR_ID == gestor
                                  where saldo.TB_SUBITEM_MATERIAL_CODIGO == codigo
                                  where saldo.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                    select a.TB_SUBITEM_MATERIAL).AsQueryable<TB_SUBITEM_MATERIAL>().Distinct();// filtros de busca aqui na modal 
                                                                                                    //  iguais a tela de gerencia de catalogo @Anderson 23/04/2021
                    }
                    else
                        result = result.Where(a => a.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()));
                }

            }

            //Filtra gestor caso filtre pelo gestor não exibe
            if (filtraGestor)
            {
                result = result.Where(a => a.TB_GESTOR_ID == almoxarifado.TB_GESTOR_ID);

            }

            if (filtraNaturezasDespesaConsumoImediato)
            {
                IList<int> listaIDsSubitemMateriais = null;
                var infraSubitemMaterial = new Sam.Domain.Infrastructure.SubItemMaterialInfraestructure();
                listaIDsSubitemMateriais = infraSubitemMaterial.obterSubitemMaterial_IDs__NaturezasDespesa_ConsumoImediato();


                //result = result.Where(subitemMaterial => listaNaturezasDespesaConsumoImediato.Contains(subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO));
                result = result.Where(subitemMaterial => listaIDsSubitemMateriais.Contains(subitemMaterial.TB_SUBITEM_MATERIAL_ID));
            }

            //Ativo
          //  result = result.Where(a => a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true);

            result = result.OrderBy(a => a.TB_SUBITEM_MATERIAL_DESCRICAO);
            base.TotalRegistros = result.Count();

            return Queryable.Take(Queryable.Skip(result, startIndex), 20).ToList();
        }

        public TB_ALMOXARIFADO obterAlmoxarifado(int codigoOrgao, int codigoAlmox)
        {
            TB_ALMOXARIFADO rowTabela = null;

            rowTabela = Context.TB_ALMOXARIFADO.Where(almox => almox.TB_GESTOR.TB_ORGAO.TB_ORGAO_CODIGO == codigoOrgao
                                                            && almox.TB_ALMOXARIFADO_CODIGO == codigoAlmox)
                                               .FirstOrDefault();

            return rowTabela;
        }

        /// <summary>
        /// Gera a consulta em formato IQueryable, utilizada pelo webservice de consulta de subitens com saldo e disponiveis para requisicao
        /// </summary>
        /// <param name="codigoOrgao"></param>
        /// <param name="codigoAlmox"></param>
        /// <param name="dispRequisicao"></param>
        /// <returns></returns>
        private IQueryable<TB_SUBITEM_MATERIAL> geraConsultaRelacaoSubitensParaRequisicaoWs(int codigoOrgao, int codigoAlmox, bool dispRequisicao = true)
        {
            IQueryable<TB_SUBITEM_MATERIAL> qryConsulta;

            var rowAlmox = obterAlmoxarifado(codigoOrgao, codigoAlmox);
            if (!dispRequisicao)
                qryConsulta = (from s in Context.TB_SALDO_SUBITEM
                               join r in Context.TB_RESERVA_MATERIAL on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID }
                               equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                               from r in x.DefaultIfEmpty()
                               join si in Context.TB_SUBITEM_MATERIAL on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                               join sia in Context.TB_SUBITEM_MATERIAL_ALMOX on si.TB_SUBITEM_MATERIAL_ID equals sia.TB_SUBITEM_MATERIAL_ID

                               where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                               where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                       && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                       || (r.TB_RESERVA_MATERIAL_QUANT == null))
                               where (s.TB_ALMOXARIFADO_ID.Equals(rowAlmox.TB_ALMOXARIFADO_ID))
                               where (sia.TB_ALMOXARIFADO_ID.Equals(rowAlmox.TB_ALMOXARIFADO_ID))
                               where (sia.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true)
                               select si).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();
            else
                qryConsulta = (from s in Context.TB_SALDO_SUBITEM
                               join r in Context.TB_RESERVA_MATERIAL on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID }
                               equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                               from r in x.DefaultIfEmpty()
                               join si in Context.TB_SUBITEM_MATERIAL on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                               join sia in Context.TB_SUBITEM_MATERIAL_ALMOX on si.TB_SUBITEM_MATERIAL_ID equals sia.TB_SUBITEM_MATERIAL_ID into y
                               from sia in y.DefaultIfEmpty()

                               where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                               where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                       && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                       || (r.TB_RESERVA_MATERIAL_QUANT == null))
                               where (s.TB_ALMOXARIFADO_ID.Equals(rowAlmox.TB_ALMOXARIFADO_ID))
                               where (sia.TB_ALMOXARIFADO_ID.Equals(rowAlmox.TB_ALMOXARIFADO_ID))
                               where (sia.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true)
                               where (sia.TB_INDICADOR_DISPONIVEL_ID == 2)
                               select si).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();


            return qryConsulta;
        }
        private IQueryable<TB_SUBITEM_MATERIAL> aplicaFiltroParaConsultaRelacaoSubitensParaRequisicaoWs(IQueryable<TB_SUBITEM_MATERIAL> qryConsulta, string palavraChave, int gestorID)
        {
            //Filtra palavra chave
            if (!String.IsNullOrEmpty(palavraChave))
            {
                long? codigo = Common.Util.TratamentoDados.TryParseLong(palavraChave);

                if (codigo != null)
                    qryConsulta = qryConsulta.Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == codigo);
                else
                    qryConsulta = qryConsulta.Where(a => a.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()));
            }

            //Filtra gestor
            //if (filtraGestor)
            qryConsulta = qryConsulta.Where(a => a.TB_GESTOR_ID == gestorID);

            //Ativo
            qryConsulta = qryConsulta.Where(a => a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true);
            qryConsulta = qryConsulta.OrderBy(a => a.TB_SUBITEM_MATERIAL_DESCRICAO);


            return qryConsulta;
        }

        //// Só retorna subitem material do almoxarifado informado com saldo na TB_SALDO_SUBITEM e com TB_SALDO_SUBITEM_SALDO_QTDE - TB_RESERVA_MATERIAL_QUANT > 0
        public IList<dtoWsSubitemMaterial> BuscaSubItemMaterialRequisicaoParaWs(string palavraChave, int codigoOrgao, int codigoAlmox, bool dispRequisicao = true, int NumeroPaginaConsulta = 0)
        {
            IList<dtoWsSubitemMaterial> retornoConsulta = null;
            //Expression<Func<TB_SUBITEM_MATERIAL, bool>> expWhere;
            IQueryable<TB_SUBITEM_MATERIAL> qryConsulta;
            Context.ContextOptions.LazyLoadingEnabled = true;


            var rowAlmox = obterAlmoxarifado(codigoOrgao, codigoAlmox);

            try
            {
                qryConsulta = geraConsultaRelacaoSubitensParaRequisicaoWs(codigoOrgao, codigoAlmox, dispRequisicao);
                qryConsulta = aplicaFiltroParaConsultaRelacaoSubitensParaRequisicaoWs(qryConsulta, palavraChave, rowAlmox.TB_GESTOR_ID);


                if ((codigoOrgao == Constante.CST_CODIGO_ORGAO__CLIENTE_FCASA) && (qryConsulta.Count() == 0))
                {
                    int codigoAlmoxGMAN = Constante.CST_CODIGO_ALMOX_GMAN__CLIENTE_FCASA;
                    rowAlmox = obterAlmoxarifado(codigoOrgao, codigoAlmoxGMAN);

                    qryConsulta = geraConsultaRelacaoSubitensParaRequisicaoWs(codigoOrgao, codigoAlmoxGMAN, dispRequisicao);
                    qryConsulta = aplicaFiltroParaConsultaRelacaoSubitensParaRequisicaoWs(qryConsulta, palavraChave, rowAlmox.TB_GESTOR_ID);
                }


                
                int numeroRegistros = qryConsulta.Count();
                if (numeroRegistros > Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS && ((NumeroPaginaConsulta * Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS) < numeroRegistros))
                    qryConsulta = Queryable.Take(Queryable.Skip(qryConsulta, NumeroPaginaConsulta * Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS), Constante.CST_NUMERO_MAXIMO_REGISTROS_POR_CONSULTA_WS);

                string descricaoAlmoxarifado = String.Format("{0:D3} - {1}", rowAlmox.TB_ALMOXARIFADO_CODIGO, rowAlmox.TB_ALMOXARIFADO_DESCRICAO);
                retornoConsulta = qryConsulta.Select<TB_SUBITEM_MATERIAL, dtoWsSubitemMaterial>(_instanciadorDtoWsSubitemMaterial(descricaoAlmoxarifado))
                                             .ToList<dtoWsSubitemMaterial>();


                base.TotalRegistros = numeroRegistros;
            }
            catch (Exception excErroConsulta)
            {
                throw excErroConsulta;
            }


            return retornoConsulta;
        }


        public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterialRequisicao(int startIndex, string palavraChave, bool filtraGestor, TB_ALMOXARIFADO almoxarifado, bool dispRequisicao)
        {
            IQueryable<TB_SUBITEM_MATERIAL> result;
            Context.ContextOptions.LazyLoadingEnabled = true;

            //// Só retorna subitem material: 
            //// do almoxarifado informado;
            //// com saldo na TB_SALDO_SUBITEM;
            //// com TB_SALDO_SUBITEM_SALDO_QTDE - TB_RESERVA_MATERIAL_QUANT > 0
            try
            {

                if (!dispRequisicao)
                    result = (from s in Context.TB_SALDO_SUBITEM
                              join r in Context.TB_RESERVA_MATERIAL on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID }
                              equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                              from r in x.DefaultIfEmpty()
                              join si in Context.TB_SUBITEM_MATERIAL on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                              join sia in Context.TB_SUBITEM_MATERIAL_ALMOX on si.TB_SUBITEM_MATERIAL_ID equals sia.TB_SUBITEM_MATERIAL_ID

                              where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                              where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                        && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                     || (r.TB_RESERVA_MATERIAL_QUANT == null))
                              where (s.TB_ALMOXARIFADO_ID.Equals(almoxarifado.TB_ALMOXARIFADO_ID))
                              where (sia.TB_ALMOXARIFADO_ID.Equals(almoxarifado.TB_ALMOXARIFADO_ID))
                              where (sia.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true)
                             where (sia.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true)
                              where (sia.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 2)
                              select si).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();
                else
                    result = (from s in Context.TB_SALDO_SUBITEM
                              join r in Context.TB_RESERVA_MATERIAL on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID }
                              equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                              from r in x.DefaultIfEmpty()
                              join si in Context.TB_SUBITEM_MATERIAL on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                              join sia in Context.TB_SUBITEM_MATERIAL_ALMOX on si.TB_SUBITEM_MATERIAL_ID equals sia.TB_SUBITEM_MATERIAL_ID into y
                              from sia in y.DefaultIfEmpty()
                            
                              where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                        && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                     || (r.TB_RESERVA_MATERIAL_QUANT == null))
                              where (s.TB_ALMOXARIFADO_ID.Equals(almoxarifado.TB_ALMOXARIFADO_ID))
                              where (sia.TB_ALMOXARIFADO_ID.Equals(almoxarifado.TB_ALMOXARIFADO_ID))
                              where (sia.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true)
                              where (sia.TB_INDICADOR_DISPONIVEL_ID == 2 && sia.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 2)
                               || (sia.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 0 && s.TB_SALDO_SUBITEM_SALDO_QTDE > 0.0M)
                                || (sia.TB_INDICADOR_DISPONIVEL_ID == 2 && s.TB_SALDO_SUBITEM_SALDO_QTDE > 0.0M)
                              select si).Distinct().AsQueryable<TB_SUBITEM_MATERIAL>();

                
                            
                
                //Filtra palavra chave
                if (!String.IsNullOrEmpty(palavraChave))
                {
                    long? codigo = Common.Util.TratamentoDados.TryParseLong(palavraChave);

                    if (codigo != null)
                        result = result.Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == codigo);
                    else
                        result = result.Where(a => a.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(palavraChave.Trim().ToUpper()));
                }

                //Filtra gestor
                if (filtraGestor)
                    result = result.Where(a => a.TB_GESTOR_ID == almoxarifado.TB_GESTOR_ID);

                //Ativo
                //result = result.Where(a => a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true);

                result = result.OrderBy(a => a.TB_SUBITEM_MATERIAL_DESCRICAO);

                base.TotalRegistros = result.Count();

                return Queryable.Take(Queryable.Skip(result, startIndex), 20).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Retrieves the sql text command + simple representation of values
        /// </summary>
        public static string ToTraceString<T>(IQueryable<T> query)
        {
            if (query != null)
            {
                ObjectQuery<T> objectQuery = query as ObjectQuery<T>;

                StringBuilder sb = new StringBuilder();
                sb.Append(objectQuery.ToTraceString());
                foreach (var p in objectQuery.Parameters)
                    sb.AppendFormat("\r\n{0} ({2}): {1}", p.Name, p.Value, p.ParameterType);
                return sb.ToString();
            }
            return "No Trace string to return";
        }

        private Func<TB_SUBITEM_MATERIAL, dtoWsSubitemMaterial> _instanciadorDtoWsSubitemMaterial()
        {
            Func<TB_SUBITEM_MATERIAL, dtoWsSubitemMaterial> _actionSeletor = null;

            _actionSeletor = (rowTabela => new dtoWsSubitemMaterial()
                                                                    {
                                                                        Codigo = rowTabela.TB_SUBITEM_MATERIAL_CODIGO,
                                                                        Descricao = rowTabela.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                        UnidadeFornecimento = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                        NaturezaDespesa = rowTabela.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO
                                                                    });

            return _actionSeletor;
        }

        private Func<TB_SUBITEM_MATERIAL, dtoWsSubitemMaterial> _instanciadorDtoWsSubitemMaterial(string descricaoAlmoxarifado)
        {
            Func<TB_SUBITEM_MATERIAL, dtoWsSubitemMaterial> _actionSeletor = null;

            _actionSeletor = (rowTabela => new dtoWsSubitemMaterial()
                                                                    {
                                                                        Codigo = rowTabela.TB_SUBITEM_MATERIAL_CODIGO,
                                                                        Descricao = rowTabela.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                        UnidadeFornecimento = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                        NaturezaDespesa = rowTabela.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,

                                                                        Almoxarifado = descricaoAlmoxarifado
                                                                    });

            return _actionSeletor;
        }
    }
}
