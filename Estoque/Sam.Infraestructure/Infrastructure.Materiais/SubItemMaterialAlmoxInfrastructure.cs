using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Infrastructure
{
    public class SubItemMaterialAlmoxInfrastructure : AbstractCrud<TB_SUBITEM_MATERIAL_ALMOX, SAMwebEntities>
    {
        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int maximumRows, int startRowIndex, int divisaoId)
        {
            Context.ContextOptions.LazyLoadingEnabled = true;

            var result = (from subAlmox in Context.TB_SUBITEM_MATERIAL_ALMOX.ToList()
                          join subitem in Context.TB_SUBITEM_MATERIAL on subAlmox.TB_SUBITEM_MATERIAL_ID equals subitem.TB_SUBITEM_MATERIAL_ID
                          join almox in Context.TB_ALMOXARIFADO on subAlmox.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                          join div in Context.TB_DIVISAO on almox.TB_ALMOXARIFADO_ID equals div.TB_ALMOXARIFADO_ID
                          //orderby subitem.TB_SUBITEM_MATERIAL_DESCRICAO descending
                          where div.TB_DIVISAO_ID == divisaoId
                          where subAlmox.TB_INDICADOR_DISPONIVEL_ID != 0
                          select new TB_SUBITEM_MATERIAL_ALMOX
                          {
                              TB_SUBITEM_MATERIAL = subitem,
                              TB_INDICADOR_DISPONIVEL = subAlmox.TB_INDICADOR_DISPONIVEL,
                              TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                              TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                              TB_SUBITEM_MATERIAL_ALMOX_ID = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ID,
                              TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                              TB_SUBITEM_MATERIAL_ID = subAlmox.TB_SUBITEM_MATERIAL_ID,
                              TB_ALMOXARIFADO = almox
                          }).OrderBy(b => b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO).Skip<TB_SUBITEM_MATERIAL_ALMOX>(startRowIndex).Take(maximumRows).ToList();

            TotalRegistros = (from subAlmox in Context.TB_SUBITEM_MATERIAL_ALMOX.ToList()
                              join subitem in Context.TB_SUBITEM_MATERIAL on subAlmox.TB_SUBITEM_MATERIAL_ID equals subitem.TB_SUBITEM_MATERIAL_ID
                              join almox in Context.TB_ALMOXARIFADO on subAlmox.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                              join div in Context.TB_DIVISAO on almox.TB_ALMOXARIFADO_ID equals div.TB_ALMOXARIFADO_ID
                              where div.TB_DIVISAO_ID == divisaoId
                              where subAlmox.TB_INDICADOR_DISPONIVEL_ID != 0
                              select new TB_SUBITEM_MATERIAL_ALMOX
                              {
                                  TB_SUBITEM_MATERIAL_ID = subAlmox.TB_SUBITEM_MATERIAL_ID
                              }).Count();

            return result;
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int divisaoId, string pesquisa)
        {
            Context.ContextOptions.LazyLoadingEnabled = false;
            Context.CommandTimeout = 120;

            IQueryable<TB_SUBITEM_MATERIAL_ALMOX> result = (from subAlmox in Context.TB_SUBITEM_MATERIAL_ALMOX.ToList<TB_SUBITEM_MATERIAL_ALMOX>()
                          join subitem in Context.TB_SUBITEM_MATERIAL on subAlmox.TB_SUBITEM_MATERIAL_ID equals subitem.TB_SUBITEM_MATERIAL_ID
                          join almox in Context.TB_ALMOXARIFADO on subAlmox.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                          join div in Context.TB_DIVISAO on almox.TB_ALMOXARIFADO_ID equals div.TB_ALMOXARIFADO_ID
                          join unidFor in Context.TB_UNIDADE_FORNECIMENTO on subitem.TB_UNIDADE_FORNECIMENTO_ID equals unidFor.TB_UNIDADE_FORNECIMENTO_ID
                          join natureza in Context.TB_NATUREZA_DESPESA on subitem.TB_NATUREZA_DESPESA_ID equals natureza.TB_NATUREZA_DESPESA_ID
                          join indicadorDisp in Context.TB_INDICADOR_DISPONIVEL on subAlmox.TB_INDICADOR_DISPONIVEL_ID equals indicadorDisp.TB_INDICADOR_DISPONIVEL_ID
                          where div.TB_DIVISAO_ID == divisaoId
                          where subAlmox.TB_INDICADOR_DISPONIVEL_ID == 1 || subAlmox.TB_INDICADOR_DISPONIVEL_ID == 2
                          select new TB_SUBITEM_MATERIAL_ALMOX
                          {
                              TB_SUBITEM_MATERIAL = subitem,
                              TB_INDICADOR_DISPONIVEL = subAlmox.TB_INDICADOR_DISPONIVEL,
                              TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                              TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                              TB_SUBITEM_MATERIAL_ALMOX_ID = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_ID,
                              TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = subAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                              TB_SUBITEM_MATERIAL_ID = subAlmox.TB_SUBITEM_MATERIAL_ID,
                              TB_ALMOXARIFADO = almox
                          }).AsQueryable <TB_SUBITEM_MATERIAL_ALMOX>();


            result = result.Where(a => a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true);
            if (!String.IsNullOrEmpty(pesquisa))
            {
                result = result.Where(a => ((pesquisa == string.Empty) || ((a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Contains(pesquisa)))
                || ((pesquisa == string.Empty)) || (a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(pesquisa.ToUpper()))));
            }

            result = result.OrderBy(b => b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            return result.ToList<TB_SUBITEM_MATERIAL_ALMOX>();
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int divisaoId, string pesquisa, bool possuiSaldo)
        {
            if (!possuiSaldo)
                return SelectSubItemAlmoxByDivisao(divisaoId, pesquisa);

            Context.ContextOptions.LazyLoadingEnabled = false;
            Context.CommandTimeout = 120;

            IQueryable<TB_SUBITEM_MATERIAL_ALMOX> result = (from subitemAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX.ToList<TB_SUBITEM_MATERIAL_ALMOX>()
                                                            join subitem in Context.TB_SUBITEM_MATERIAL on subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals subitem.TB_SUBITEM_MATERIAL_ID
                                                            join almox in Context.TB_ALMOXARIFADO on subitemAlmoxarifado.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                                                            join div in Context.TB_DIVISAO on almox.TB_ALMOXARIFADO_ID equals div.TB_ALMOXARIFADO_ID
                                                            join unidFor in Context.TB_UNIDADE_FORNECIMENTO on subitem.TB_UNIDADE_FORNECIMENTO_ID equals unidFor.TB_UNIDADE_FORNECIMENTO_ID
                                                            join natureza in Context.TB_NATUREZA_DESPESA on subitem.TB_NATUREZA_DESPESA_ID equals natureza.TB_NATUREZA_DESPESA_ID
                                                            join indicadorDisp in Context.TB_INDICADOR_DISPONIVEL on subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL_ID equals indicadorDisp.TB_INDICADOR_DISPONIVEL_ID
                                                            join saldo in Context.TB_SALDO_SUBITEM on subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                                                            where div.TB_DIVISAO_ID == divisaoId
                                                            where subitem.TB_GESTOR_ID == almox.TB_GESTOR_ID
                                                            where saldo.TB_ALMOXARIFADO_ID == div.TB_ALMOXARIFADO_ID
                                                            where saldo.TB_SALDO_SUBITEM_SALDO_QTDE > 0
                                                            where subitemAlmoxarifado.TB_ALMOXARIFADO_ID == div.TB_ALMOXARIFADO_ID
                                                            where subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL_ID != 0
                                                            select new TB_SUBITEM_MATERIAL_ALMOX
                                                            {
                                                                TB_SUBITEM_MATERIAL = subitem,
                                                                TB_INDICADOR_DISPONIVEL = subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL,
                                                                TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                                                                TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                                                                TB_SUBITEM_MATERIAL_ALMOX_ID = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ID,
                                                                TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                                TB_SUBITEM_MATERIAL_ID = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID,
                                                                TB_ALMOXARIFADO = almox
                                                            }).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();


            result = result.Where(a => a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true);
            if (!String.IsNullOrEmpty(pesquisa))
            {
                result = result.Where(a => ((pesquisa == string.Empty) || ((a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Contains(pesquisa)))
                || ((pesquisa == string.Empty)) || (a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(pesquisa.ToUpper()))));
            }

            result = result.OrderBy(b => b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            return result.ToList<TB_SUBITEM_MATERIAL_ALMOX>();
        }

        /// <summary>
        /// Consulta Subitem Material Almox Por ND
        /// </summary>
        /// <param name="Almoxarifado_ID"></param>
        /// <param name="NaturezaDespesa_ID"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(int Almoxarifado_ID, int NaturezaDespesa_ID)
        {
            Context.ContextOptions.LazyLoadingEnabled = true;

            IQueryable<TB_SUBITEM_MATERIAL_ALMOX> lQryConsulta = null;
            List<SubItemMaterialEntity> subItensCatalogo = new List<SubItemMaterialEntity>();
            List<TB_SUBITEM_MATERIAL_ALMOX> rowsTabela = new List<TB_SUBITEM_MATERIAL_ALMOX>();


            if (NaturezaDespesa_ID != 0)
            {
                lQryConsulta = (from SubItemMaterialAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX
                                join SubItemMaterial in Context.TB_SUBITEM_MATERIAL on SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                join NaturezaDespesa in Context.TB_NATUREZA_DESPESA on SubItemMaterial.TB_NATUREZA_DESPESA_ID equals NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                                where SubItemMaterialAlmoxarifado.TB_ALMOXARIFADO_ID == Almoxarifado_ID
                                where NaturezaDespesa.TB_NATUREZA_DESPESA_ID == NaturezaDespesa_ID
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                select SubItemMaterialAlmoxarifado
                               ).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            }
            else if (NaturezaDespesa_ID == 0)
            {
                lQryConsulta = (from SubItemMaterialAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX
                                join SubItemMaterial in Context.TB_SUBITEM_MATERIAL on SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                join NaturezaDespesa in Context.TB_NATUREZA_DESPESA on SubItemMaterial.TB_NATUREZA_DESPESA_ID equals NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                                where SubItemMaterialAlmoxarifado.TB_ALMOXARIFADO_ID == Almoxarifado_ID
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                select SubItemMaterialAlmoxarifado
                               ).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            }

            this.TotalRegistros = lQryConsulta.Count();
            rowsTabela = lQryConsulta.OrderBy(subItemMaterial => subItemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO)
                                     .Cast<TB_SUBITEM_MATERIAL_ALMOX>()
                                     .ToList();

            rowsTabela.ForEach(linhaTabela => subItensCatalogo.Add(this.MapearDTO(linhaTabela, true)));

            return subItensCatalogo;
        }

        public IList<SubItemMaterialEntity> ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(int Almoxarifado_ID, int NaturezaDespesa_ID, int startRowIndex, int maximumRows)
        {
            Context.ContextOptions.LazyLoadingEnabled = true;

            IQueryable<TB_SUBITEM_MATERIAL_ALMOX> lQryConsulta = null;
            List<SubItemMaterialEntity> subItensCatalogo = new List<SubItemMaterialEntity>();
            List<TB_SUBITEM_MATERIAL_ALMOX> rowsTabela = new List<TB_SUBITEM_MATERIAL_ALMOX>();


            if (NaturezaDespesa_ID != 0)
            {
                lQryConsulta = (from SubItemMaterialAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX
                                join SubItemMaterial in Context.TB_SUBITEM_MATERIAL on SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                join NaturezaDespesa in Context.TB_NATUREZA_DESPESA on SubItemMaterial.TB_NATUREZA_DESPESA_ID equals NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                                where SubItemMaterialAlmoxarifado.TB_ALMOXARIFADO_ID == Almoxarifado_ID
                                where NaturezaDespesa.TB_NATUREZA_DESPESA_ID == NaturezaDespesa_ID
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                select SubItemMaterialAlmoxarifado
                               ).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            }
            else if (NaturezaDespesa_ID == 0)
            {
                lQryConsulta = (from SubItemMaterialAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX
                                join SubItemMaterial in Context.TB_SUBITEM_MATERIAL on SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                join NaturezaDespesa in Context.TB_NATUREZA_DESPESA on SubItemMaterial.TB_NATUREZA_DESPESA_ID equals NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                                where SubItemMaterialAlmoxarifado.TB_ALMOXARIFADO_ID == Almoxarifado_ID
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                                where SubItemMaterialAlmoxarifado.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true
                                select SubItemMaterialAlmoxarifado
                               ).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();
            }

            this.TotalRegistros = lQryConsulta.Count();
            rowsTabela     = lQryConsulta.OrderBy(subItemMaterial => subItemMaterial.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO)
                                     .Skip(startRowIndex)
                                     .Take(maximumRows)
                                     .Cast<TB_SUBITEM_MATERIAL_ALMOX>()
                                     .ToList();

            rowsTabela.ForEach(linhaTabela => subItensCatalogo.Add(this.MapearDTO(linhaTabela, true)));

            return subItensCatalogo;
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> ObterSubItensMaterialAlmoxarifadoPorDivisao(int DivisaoId, string textoPesquisa, bool? possuiSaldo)
        {
            int divisaoID = DivisaoId;
            string txtPesquisa = textoPesquisa;
            long fragmentoCodigoSubitem = -1;

            Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> expWhere = null;
            IQueryable<TB_SUBITEM_MATERIAL_ALMOX> qryConsulta = null;
            IList<TB_SUBITEM_MATERIAL_ALMOX> lstRetorno = null;

            Context.ContextOptions.LazyLoadingEnabled = true;


            lstRetorno = new List<TB_SUBITEM_MATERIAL_ALMOX>();

            qryConsulta = (from subitemAlmoxarifado in Context.TB_SUBITEM_MATERIAL_ALMOX//.ToList<TB_SUBITEM_MATERIAL_ALMOX>()
                            join subitem in Context.TB_SUBITEM_MATERIAL on subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals subitem.TB_SUBITEM_MATERIAL_ID
                            join almox in Context.TB_ALMOXARIFADO on subitemAlmoxarifado.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID
                            join div in Context.TB_DIVISAO on almox.TB_ALMOXARIFADO_ID equals div.TB_ALMOXARIFADO_ID
                            join unidFor in Context.TB_UNIDADE_FORNECIMENTO on subitem.TB_UNIDADE_FORNECIMENTO_ID equals unidFor.TB_UNIDADE_FORNECIMENTO_ID
                            join indicadorDisp in Context.TB_INDICADOR_DISPONIVEL on subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL_ID equals indicadorDisp.TB_INDICADOR_DISPONIVEL_ID
                            //join saldo in Context.TB_SALDO_SUBITEM on subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID equals saldo.TB_SUBITEM_MATERIAL_ID
                            where div.TB_DIVISAO_ID == divisaoID
                            where subitem.TB_GESTOR_ID == almox.TB_GESTOR_ID
                            //where saldo.TB_ALMOXARIFADO_ID == div.TB_ALMOXARIFADO_ID
                            //where saldo.TB_SALDO_SUBITEM_SALDO_QTDE > (int)0
                            where subitemAlmoxarifado.TB_ALMOXARIFADO_ID == div.TB_ALMOXARIFADO_ID
                            where subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL_ID != 0
                           where subitemAlmoxarifado.TB_INDICADOR_ITEM_SALDO_ZERADO_ID != 0  
                           where subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true
                           select subitemAlmoxarifado).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();


            if (Int64.TryParse(txtPesquisa, out fragmentoCodigoSubitem))
            {
                expWhere = (subitemAlmox => subitemAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Contains(fragmentoCodigoSubitem.ToString()));
                qryConsulta = qryConsulta.Where(expWhere);
            }

            else if (fragmentoCodigoSubitem == 0 && !String.IsNullOrWhiteSpace(txtPesquisa))
            {
                expWhere = (subitemAlmox => subitemAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO.Contains(txtPesquisa));
                qryConsulta = qryConsulta.Where(expWhere);
            }

            qryConsulta = qryConsulta.OrderBy(b => b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO).AsQueryable<TB_SUBITEM_MATERIAL_ALMOX>();

            qryConsulta.ToList<TB_SUBITEM_MATERIAL_ALMOX>()
                       .ForEach(subitemAlmoxarifado => lstRetorno.Add(new TB_SUBITEM_MATERIAL_ALMOX()
                                                                                                    {
                                                                                                        TB_SUBITEM_MATERIAL = subitemAlmoxarifado.TB_SUBITEM_MATERIAL,
                                                                                                        TB_INDICADOR_DISPONIVEL = subitemAlmoxarifado.TB_INDICADOR_DISPONIVEL,
                                                                                                        TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                                                                                                        TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                                                                                                        TB_SUBITEM_MATERIAL_ALMOX_ID = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_ID,
                                                                                                        TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                                                                        TB_SUBITEM_MATERIAL_ID = subitemAlmoxarifado.TB_SUBITEM_MATERIAL_ID,
                                                                                                        TB_ALMOXARIFADO = subitemAlmoxarifado.TB_ALMOXARIFADO
                                                                                                    }));

            return lstRetorno;
        }

        #region Mappers
        internal SubItemMaterialAlmoxEntity MapearDTO(TB_SUBITEM_MATERIAL_ALMOX rowTabela)
        {
            LazyLoadingEnabled = true;
            rowTabela.TB_SUBITEM_MATERIALReference.Load(MergeOption.PreserveChanges);

            SubItemMaterialAlmoxEntity objEntidade = new SubItemMaterialAlmoxEntity();

            objEntidade.Id                  = rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ID;
            objEntidade.CodigoDescricao     = String.Format("{0} - {1}", rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO);
            objEntidade.CodigoFormatado     = String.Format("{0}", rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadRight(8, '0'));
            objEntidade.EstoqueMinimo       = (decimal)rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN;
            objEntidade.EstoqueMaximo       = (decimal)rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX;
            objEntidade.IndicadorAtividade  = rowTabela.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE;
            objEntidade.SubItemMaterialId   = rowTabela.TB_SUBITEM_MATERIAL_ID;

            return objEntidade;
        }
        internal SubItemMaterialEntity      MapearDTO(TB_SUBITEM_MATERIAL_ALMOX rowTabela, bool convertToSubItemMaterial)
        {

            SubItemMaterialEntity objEntidade = new SubItemMaterialEntity();

            objEntidade.Id                      = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID;
            objEntidade.Codigo                  = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO;
            objEntidade.CodigoBarras            = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS;
            objEntidade.CodigoDescricao         = String.Format("{0} - {1}", rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO);
            objEntidade.CodigoFormatado         = String.Format("{0}", rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadRight(8, '0'));
            objEntidade.Descricao               = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO;
            objEntidade.IndicadorAtividade      = rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE;
            objEntidade.IndicadorAtividadeAlmox = rowTabela.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE;
            objEntidade.IsDecimal 				= rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DECIMOS;
            objEntidade.IsFracionado 			= rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_FRACIONA;
            //objEntidade.IsLote 					= rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE;

            objEntidade.Gestor 					= new GestorEntity() { Id = rowTabela.TB_SUBITEM_MATERIAL.TB_GESTOR_ID };


            //var objContaAuxiliar = rowTabela.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR;
            //if (!objContaAuxiliar.IsNull())
            //    objEntidade.ContaAuxiliar = new ContaAuxiliarEntity() { Id = objContaAuxiliar.TB_CONTA_AUXILIAR_ID, Codigo = objContaAuxiliar.TB_CONTA_AUXILIAR_CODIGO, Descricao = objContaAuxiliar.TB_CONTA_AUXILIAR_DESCRICAO };


            var objAlmoxarifado = rowTabela.TB_ALMOXARIFADO;
            if (!objAlmoxarifado.IsNull())
                objEntidade.Almoxarifado = new AlmoxarifadoEntity() { Id = objAlmoxarifado.TB_ALMOXARIFADO_ID, Codigo = objAlmoxarifado.TB_ALMOXARIFADO_CODIGO, Descricao = objAlmoxarifado.TB_ALMOXARIFADO_DESCRICAO };


            var objItemSubItemMaterial = rowTabela.TB_SUBITEM_MATERIAL.TB_ITEM_SUBITEM_MATERIAL.Where(row => row.TB_SUBITEM_MATERIAL_ID == rowTabela.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID).FirstOrDefault();
            if (!objItemSubItemMaterial.IsNull())
            {
                objItemSubItemMaterial.TB_ITEM_MATERIALReference.Load(MergeOption.PreserveChanges);

                var objItemMaterial = objItemSubItemMaterial.TB_ITEM_MATERIAL;
                objEntidade.ItemMaterial = new ItemMaterialEntity() { Id = objItemMaterial.TB_ITEM_MATERIAL_ID, Codigo = objItemMaterial.TB_ITEM_MATERIAL_CODIGO, Descricao = objItemMaterial.TB_ITEM_MATERIAL_DESCRICAO };
            }


            var objNaturezaDespesa = rowTabela.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA;
            if (!objNaturezaDespesa.IsNull())
                objEntidade.NaturezaDespesa = new NaturezaDespesaEntity() { Id = objNaturezaDespesa.TB_NATUREZA_DESPESA_ID, Codigo = objNaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO, Descricao = objNaturezaDespesa.TB_NATUREZA_DESPESA_DESCRICAO, Natureza = objNaturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE };


            var objUnidadeFornecimento = rowTabela.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO;
            if (!objUnidadeFornecimento.IsNull())
            {
                objEntidade.UnidadeFornecimento = new UnidadeFornecimentoEntity() { Id = objUnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID, Codigo = objUnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = objUnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO };
            }

            return objEntidade;
        }
        internal TB_SUBITEM_MATERIAL_ALMOX MapearEntity(SubItemMaterialAlmoxEntity objEntidade)
        {
            TB_SUBITEM_MATERIAL_ALMOX rowTabela = new TB_SUBITEM_MATERIAL_ALMOX();

            rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ID                  = objEntidade.Id.Value;
            rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX         = objEntidade.EstoqueMaximo;
            rowTabela.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN         = objEntidade.EstoqueMinimo;
            rowTabela.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = objEntidade.IndicadorAtividade;
            rowTabela.TB_SUBITEM_MATERIAL_ID                        = objEntidade.SubItemMaterialId;

            return rowTabela;
        }
        #endregion Mappers
    }
}
