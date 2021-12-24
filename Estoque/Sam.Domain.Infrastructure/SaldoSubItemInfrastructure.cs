using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.Domain.Infrastructure;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;


namespace Sam.Domain.Infrastructure
{
    public partial class SaldoSubItemInfrastructure : BaseInfraestructure, ISaldoSubItemService
    {
        private SaldoSubItemEntity SaldoSubItem = new SaldoSubItemEntity();

        public SaldoSubItemEntity Entity
        {
            get { return SaldoSubItem; }
            set { SaldoSubItem = value; }
        }

        public bool VerificaSubItemUtilizado(int subItemId)
        {

            var result = (from a in this.Db.TB_SALDO_SUBITEMs
                          where a.TB_SUBITEM_MATERIAL_ID == subItemId
                          select a);

            if (result.Count() > 0)
                return true;
            else
                return false;
        }

        public IList<SaldoSubItemEntity> Listar()
        {
            return (from a in this.Db.TB_SALDO_SUBITEMs
                    where a.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItemMaterial.Id
                    where a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id
                    where a.TB_UGE_ID == (this.Entity.UGE.Id ?? 0)
                    select new SaldoSubItemEntity
                    {
                        Id = a.TB_SALDO_SUBITEM_ID,
                        Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                        UGE = (from uge in Db.TB_UGEs
                               where uge.TB_UGE_ID == a.TB_UGE_ID
                               select new UGEEntity
                               {
                                   Id = uge.TB_UGE_ID,
                                   Codigo = uge.TB_UGE_CODIGO,
                                   Descricao = string.Format("{0} - {1}", uge.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), uge.TB_UGE_DESCRICAO)
                               }
                               ).FirstOrDefault(),
                        SubItemMaterial = (from b in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                           join c in Db.TB_SUBITEM_MATERIALs on b.TB_SUBITEM_MATERIAL_ID equals c.TB_SUBITEM_MATERIAL_ID
                                           join d in Db.TB_ALMOXARIFADOs on b.TB_ALMOXARIFADO_ID equals d.TB_ALMOXARIFADO_ID
                                           where b.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
                                           where b.TB_ALMOXARIFADO_ID == a.TB_ALMOXARIFADO_ID
                                           select new SubItemMaterialEntity
                                           {
                                               Id = b.TB_SUBITEM_MATERIAL_ID,
                                               Descricao = c.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0') + " - " + c.TB_SUBITEM_MATERIAL_DESCRICAO
                                           }
                                           ).FirstOrDefault(),
                        LoteDataVenc = a.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                        LoteFabr = a.TB_SALDO_SUBITEM_LOTE_FAB,
                        LoteIdent = a.TB_SALDO_SUBITEM_LOTE_IDENT,
                        PrecoUnit = a.TB_SALDO_SUBITEM_PRECO_UNIT,
                        SaldoQtde = a.TB_SALDO_SUBITEM_SALDO_QTDE,
                        SaldoValor = a.TB_SALDO_SUBITEM_SALDO_VALOR
                    }).ToList<SaldoSubItemEntity>();
        }

        public IList<SaldoSubItemEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public SaldoSubItemEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public static int calcularMeses(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        //"dsConsultasConsumoAlmoxarifado"
        [Obsolete]
        public IList<SubItemMaterialEntity> ImprimirConsumoAlmox(int? almoxarifado, DateTime? dataInicial, DateTime? dataFinal)
        {
            int meses = calcularMeses(dataFinal.Value.AddMonths(-1), dataInicial.Value);

            IEnumerable<SubItemMaterialEntity> resultado = (from am in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                            join s in Db.TB_SALDO_SUBITEMs on
                                                                new { am.TB_ALMOXARIFADO_ID, am.TB_SUBITEM_MATERIAL_ID }
                                                                equals new { s.TB_ALMOXARIFADO_ID, s.TB_SUBITEM_MATERIAL_ID } into xxx
                                                            select new SubItemMaterialEntity
                                                            {
                                                                Id = am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                Codigo = am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                CodigoFormatado = am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                Descricao = am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                Almoxarifado = new AlmoxarifadoEntity
                                                                {
                                                                    Id = am.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                    Descricao = am.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                },
                                                                UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                {
                                                                    Id = am.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                    Codigo = am.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                    Descricao = am.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                },
                                                                SaldoAtual = xxx.Select(a => a.TB_SALDO_SUBITEM_SALDO_QTDE).FirstOrDefault() ?? 0,
                                                                NumeroMesesRelatorio = meses,
                                                                QtdePeriodo = (from mi in Db.TB_MOVIMENTO_ITEMs
                                                                               where mi.TB_SUBITEM_MATERIAL_ID == am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                               where mi.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == am.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID
                                                                               where (mi.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= dataInicial &&
                                                                                      mi.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO < dataFinal)
                                                                               select new MovimentoItemEntity
                                                                               {
                                                                                   QtdeMov = mi.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                                                   Movimento = new MovimentoEntity
                                                                                   {
                                                                                       Id = mi.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                                                                                       Almoxarifado = new AlmoxarifadoEntity
                                                                                       {
                                                                                           Id = mi.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                                           Descricao = mi.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                                       },
                                                                                       Divisao = new DivisaoEntity
                                                                                       {
                                                                                           Id = mi.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_ID,
                                                                                           Descricao = mi.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_DESCRICAO
                                                                                       }
                                                                                   }
                                                                               }).Sum(d => d.QtdeMov) ?? 0,
                                                                SaldoReservaMat = (from rm in Db.TB_RESERVA_MATERIALs
                                                                                   where rm.TB_SUBITEM_MATERIAL_ID == am.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                                   where rm.TB_ALMOXARIFADO_ID == am.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID
                                                                                   select new ReservaMaterialEntity
                                                                                   {
                                                                                       Quantidade = rm.TB_RESERVA_MATERIAL_QUANT.Value,
                                                                                   }).Sum(d => d.Quantidade)
                                                            });

            //var str_Consulta = resultado.RetornarConsultaValorada();
            var str_Consulta = resultado.ToString();

            if (almoxarifado.HasValue && almoxarifado != 0)
                resultado = resultado.Where(a => a.Almoxarifado.Id == almoxarifado);

            IList<SubItemMaterialEntity> result = resultado.ToList();

            if (result.Count > 0)
            {
                foreach (SubItemMaterialEntity sub in result)
                {
                    if (!sub.SaldoReservaMat.HasValue)
                        sub.SaldoReservaMat = 0;
                }
            }


            return result;
        }
        /// <summary>
        /// Relatório de consumo
        /// Método para retornar os itens consumidos pelo almoxarifado, no período de tempo informado.
        /// </summary>
        /// <param name="almoxarifado"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="NewMethod"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ImprimirConsumoAlmox(int? pIntAlmoxarifado_ID, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod = true)
        {
            DateTime? pDtDataInicial = (dataInicial.HasValue) ? new DateTime(dataInicial.Value.Year, dataInicial.Value.Month, 01) : new DateTime(0);
            DateTime? pDtDataFinal = (dataFinal.HasValue) ? new DateTime(dataFinal.Value.Year, dataFinal.Value.Month, 01) : new DateTime(0);

            int numMeses = dataInicial.Value.MonthDiff(dataFinal.Value);

            #region Variaveis
            string strConsultaGeral = null;
            //string strConsultaAuxMovimentos = null;
            //string strConsultaAuxItensMovimentos = null;

            IQueryable<TB_MOVIMENTO> listaMovimentos = null;
            //IQueryable<TB_MOVIMENTO_ITEM> listaMovimentoItens = null;

            AlmoxarifadoInfraestructure lObjAlmoxarifadoInfra = null;
            AlmoxarifadoEntity almoxarifadoConsulta = null;

            List<SubItemMaterialEntity> lstRetornoConsulta = null;
            List<SubItemMaterialEntity> lstConsultaFiltrada = null;

            #endregion Variaveis

            #region Consultas Auxiliares LINQ
            listaMovimentos = from Movimento in Db.TB_MOVIMENTOs
                              where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO >= pDtDataInicial
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO < pDtDataFinal
                              select Movimento;

            //listaMovimentoItens = from ItemMovimento in Db.TB_MOVIMENTO_ITEMs
            //                      join Movimento in listaMovimentos on ItemMovimento.TB_MOVIMENTO_ID equals Movimento.TB_MOVIMENTO_ID
            //                      join SaldoSubItem in Db.TB_SALDO_SUBITEMs on Movimento.TB_ALMOXARIFADO_ID equals SaldoSubItem.TB_ALMOXARIFADO_ID
            //                      select ItemMovimento;
            #endregion Consultas Auxiliares LINQ

            #region Consulta LINQ
            IQueryable<SubItemMaterialEntity> resultSet = (from SaldoSubItem in Db.TB_SALDO_SUBITEMs
                                                           join Movimento in listaMovimentos on SaldoSubItem.TB_ALMOXARIFADO_ID equals Movimento.TB_ALMOXARIFADO_ID
                                                           join MovimentoItem in Db.TB_MOVIMENTO_ITEMs on Movimento.TB_MOVIMENTO_ID equals MovimentoItem.TB_MOVIMENTO_ID //into grpMovItem
                                                           join TipoMovimento in Db.TB_TIPO_MOVIMENTOs on Movimento.TB_TIPO_MOVIMENTO_ID equals TipoMovimento.TB_TIPO_MOVIMENTO_ID
                                                           join TipoMovimentoAgrupamento in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                           join SubItemMaterial in Db.TB_SUBITEM_MATERIALs on SaldoSubItem.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs on SubItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID
                                                           //join ItemSubMaterialMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on SubItemMaterial.TB_SUBITEM_MATERIAL_ID equals ItemSubMaterialMaterial.TB_SUBITEM_MATERIAL_ID
                                                           //join ItemMaterial in Db.TB_ITEM_MATERIALs on ItemSubMaterialMaterial.TB_ITEM_MATERIAL_ID equals ItemMaterial.TB_ITEM_MATERIAL_ID

                                                           where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                                                           where Movimento.TB_MOVIMENTO_ATIVO == true
                                                           where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                           where TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_CODIGO == (int)GeralEnum.TipoMovimentoAgrupamento.Saida
                                                           where SaldoSubItem.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL_ID
                                                           orderby SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE
                                                           group SubItemMaterial
                                                           by new
                                                           {
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                               SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC
                                                               //ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                               //ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                           }
                                                               into grpSubItemMaterial
                                                               select new SubItemMaterialEntity
                                                               {
                                                                   Descricao = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                   Codigo = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),

                                                                   Almoxarifado = new AlmoxarifadoEntity(pIntAlmoxarifado_ID),
                                                                   NumeroMesesRelatorio = numMeses,

                                                                   UnidadeFornecimento = (from UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                          where UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO == grpSubItemMaterial.Key.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                                          select new UnidadeFornecimentoEntity
                                                                                          {
                                                                                              Id = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                              Codigo = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                              Descricao = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                          }).FirstOrDefault(),

                                                                   SaldoAtual = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE,

                                                                   //ItemMaterial = (new ItemMaterialInfraestructure()).GetItemMaterialByItem(grpSubItemMaterial.Key.TB_ITEM_MATERIAL_ID),

                                                                   CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),        //Usado para transportar MovimentoItem_ID do MovimentoItem (tratamento elementos com lote)
                                                                   CodigoBarras = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem

                                                                   IsLote = ((grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_FABR != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue)),

                                                                   //Quase! ??? sum(mi.tb_movimento_item_qtde_mov) ???
                                                                   QtdePeriodo = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_QTDE_MOV
                                                               });

            #endregion Consulta LINQ

            #region "debug session"
            strConsultaGeral = resultSet.ToString();
            //strConsultaAuxMovimentos = listaMovimentos.ToString();
            //strConsultaAuxItensMovimentos = listaMovimentoItens.ToString();

            Db.GetCommand(resultSet as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaGeral = strConsultaGeral.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            //Db.GetCommand(listaMovimentos as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxMovimentos = strConsultaAuxMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            //Db.GetCommand(listaMovimentoItens as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxItensMovimentos = strConsultaAuxItensMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion "debug session"

            #region Filtro Retorno Consulta
            lObjAlmoxarifadoInfra = new AlmoxarifadoInfraestructure();
            almoxarifadoConsulta = lObjAlmoxarifadoInfra.ObterAlmoxarifado(pIntAlmoxarifado_ID.Value);

            lstRetornoConsulta = resultSet.ToList();
            lstConsultaFiltrada = lstRetornoConsulta.Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                            {
                                                                                                                Codigo = SubItemMaterial.Codigo,
                                                                                                                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                                                                                                                Descricao = SubItemMaterial.Descricao,
                                                                                                                Almoxarifado = almoxarifadoConsulta,
                                                                                                                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                                                                                                                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                                                                                                                ItemMaterial = SubItemMaterial.ItemMaterial,
                                                                                                                SaldoAtual = SubItemMaterial.SaldoAtual,
                                                                                                                QtdePeriodo = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo)
                                                                                                            }).DistinctBy(SubItemMaterial => SubItemMaterial.Codigo).ToList();

            //#region SubItens em Lote
            //#region "Filtro Passo 1"
            //var lstConsultaFiltradaComLote = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            //lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.DistinctBy(SubItemMaterial => SubItemMaterial.SaldoAtual).ToList();

            //lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.Select(SubItemMaterial => new SubItemMaterialEntity()
            //                                                                                                                {
            //                                                                                                                    Codigo = SubItemMaterial.Codigo,
            //                                                                                                                    CodigoFormatado = SubItemMaterial.CodigoFormatado,
            //                                                                                                                    Descricao = SubItemMaterial.Descricao,
            //                                                                                                                    Almoxarifado = almoxarifadoConsulta,
            //                                                                                                                    NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
            //                                                                                                                    UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
            //                                                                                                                    ItemMaterial = SubItemMaterial.ItemMaterial,
            //                                                                                                                    //SaldoAtual = SubItemMaterial.SaldoAtual,
            //                                                                                                                    //QtdePeriodo = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
            //                                                                                                                    //SaldoAtual = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
            //                                                                                                                    //QtdePeriodo = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
            //                                                                                                                    QtdePeriodo = SubItemMaterial.QtdePeriodo,
            //                                                                                                                    SaldoAtual = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
            //                                                                                                                })
            //                                                          .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
            //                                                          .ToList();
            //#endregion "Filtro Passo 1"

            //#region "Filtro Passo 2"
            ///*
            //        CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),       //Usado para transportar MovimentoItem_ID do MovimentoItem (tratar elementos com lote)
            //        CodigoBarras    = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem
            //    */

            //var lstConsultaFiltradaComLoteIntermediaria = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            //lstConsultaFiltradaComLoteIntermediaria = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            //{
            //    SubItemMaterial.CodigoDescricao,
            //    SubItemMaterial.Codigo,
            //    SubItemMaterial.QtdePeriodo,
            //    SubItemMaterial.Descricao,
            //})
            //                                                             .Select(SubItemMaterial => new SubItemMaterialEntity()
            //                                                                                                                    {
            //                                                                                                                        CodigoDescricao = SubItemMaterial.Key.CodigoDescricao,
            //                                                                                                                        Codigo = SubItemMaterial.Key.Codigo,
            //                                                                                                                        Descricao = SubItemMaterial.Key.Descricao,
            //                                                                                                                        Almoxarifado = almoxarifadoConsulta,
            //                                                                                                                        QtdePeriodo = SubItemMaterial.Key.QtdePeriodo,
            //                                                                                                                        SaldoAtual = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.CodigoDescricao == SubItemMaterial.Key.CodigoDescricao).Sum(Somatoria => Somatoria.SaldoAtual)
            //                                                                                                                    })
            //                                                            .ToList<SubItemMaterialEntity>();

            //var lstConsultaFiltradaComLoteSaida = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            //{
            //    SubItemMaterial.CodigoDescricao,
            //    SubItemMaterial.Codigo,
            //    SubItemMaterial.QtdePeriodo,
            //    SubItemMaterial.Descricao,
            //    SubItemMaterial.SaldoAtual,
            //})
            //                                                                   .Select(SubItemMaterial => new SubItemMaterialEntity()
            //                                                                                                                        {
            //                                                                                                                            Codigo = SubItemMaterial.Key.Codigo,
            //                                                                                                                            Descricao = SubItemMaterial.Key.Descricao,
            //                                                                                                                            Almoxarifado = almoxarifadoConsulta,
            //                                                                                                                            QtdePeriodo = lstConsultaFiltradaComLoteIntermediaria.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Key.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
            //                                                                                                                            SaldoAtual = SubItemMaterial.Key.SaldoAtual
            //                                                                                                                        })
            //                                                                   .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
            //                                                                   .ToList();

            //#endregion "Filtro Passo 2"
            //#endregion SubItens em Lote

            //#region Merging Linhas Relatorio

            //SubItemMaterialEntity _filtro = null;
            //foreach (var item in lstConsultaFiltradaComLoteSaida)
            //{
            //    _filtro = lstConsultaFiltrada.Where(SubItemMaterial2 => SubItemMaterial2.Codigo == item.Codigo).FirstOrDefault();
            //    if (_filtro != null)
            //    {
            //        item.CodigoFormatado = _filtro.CodigoFormatado;
            //        item.NumeroMesesRelatorio = _filtro.NumeroMesesRelatorio;
            //        item.ItemMaterial = _filtro.ItemMaterial;
            //        item.UnidadeFornecimento = _filtro.UnidadeFornecimento;

            //        lstConsultaFiltrada.Remove(_filtro);
            //        lstConsultaFiltrada.Add(item);
            //    }
            //}

            //#endregion Merging Linhas Relatorio

            #endregion Filtro Retorno Consulta

            return lstConsultaFiltrada;
        }


        /// <summary>
        /// Relatório de consumo Divisão
        /// </summary>
        /// <param name="pIntDivisao_ID"></param>
        /// <param name="pIntAlmoxarifado_ID"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="NewMethod"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ImprimirConsumoDivisao(int? pIntDivisao_ID, int? pIntAlmoxarifado_ID, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod)
        {
            DateTime? pDtDataInicial = (dataInicial.HasValue) ? new DateTime(dataInicial.Value.Year, dataInicial.Value.Month, 01) : new DateTime(0);
            DateTime? pDtDataFinal = (dataFinal.HasValue) ? new DateTime(dataFinal.Value.Year, dataFinal.Value.Month, 01) : new DateTime(0);

            int numMeses = dataInicial.Value.MonthDiff(dataFinal.Value);

            #region Variaveis
            string strConsultaGeral = null;
            string strConsultaAuxMovimentos = null;
            string strConsultaAuxItensMovimentos = null;

            IQueryable<TB_MOVIMENTO> listaMovimentos = null;
            IQueryable<TB_MOVIMENTO_ITEM> listaMovimentoItens = null;

            AlmoxarifadoInfraestructure lObjAlmoxarifadoInfra = null;
            AlmoxarifadoEntity almoxarifadoConsulta = null;

            List<SubItemMaterialEntity> lstRetornoConsulta = null;
            List<SubItemMaterialEntity> lstConsultaFiltrada = null;

            #endregion Variaveis

            #region Consultas Auxiliares LINQ
            listaMovimentos = from Movimento in Db.TB_MOVIMENTOs
                              where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                              where Movimento.TB_DIVISAO_ID == pIntDivisao_ID
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO >= pDtDataInicial
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO < pDtDataFinal
                              select Movimento;

            listaMovimentoItens = from ItemMovimento in Db.TB_MOVIMENTO_ITEMs
                                  join Movimento in listaMovimentos on ItemMovimento.TB_MOVIMENTO_ID equals Movimento.TB_MOVIMENTO_ID
                                  join SaldoSubItem in Db.TB_SALDO_SUBITEMs on Movimento.TB_ALMOXARIFADO_ID equals SaldoSubItem.TB_ALMOXARIFADO_ID
                                  select ItemMovimento;
            #endregion Consultas Auxiliares LINQ

            #region Consulta LINQ
            IQueryable<SubItemMaterialEntity> resultSet = (from SaldoSubItem in Db.TB_SALDO_SUBITEMs
                                                           join Movimento in listaMovimentos on SaldoSubItem.TB_ALMOXARIFADO_ID equals Movimento.TB_ALMOXARIFADO_ID
                                                           join MovimentoItem in Db.TB_MOVIMENTO_ITEMs on Movimento.TB_MOVIMENTO_ID equals MovimentoItem.TB_MOVIMENTO_ID //into grpMovItem
                                                           join TipoMovimento in Db.TB_TIPO_MOVIMENTOs on Movimento.TB_TIPO_MOVIMENTO_ID equals TipoMovimento.TB_TIPO_MOVIMENTO_ID
                                                           join TipoMovimentoAgrupamento in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                           join SubItemMaterial in Db.TB_SUBITEM_MATERIALs on SaldoSubItem.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs on SubItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID
                                                           join ItemSubMaterialMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on SubItemMaterial.TB_SUBITEM_MATERIAL_ID equals ItemSubMaterialMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join ItemMaterial in Db.TB_ITEM_MATERIALs on ItemSubMaterialMaterial.TB_ITEM_MATERIAL_ID equals ItemMaterial.TB_ITEM_MATERIAL_ID

                                                           where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                                                           where Movimento.TB_MOVIMENTO_ATIVO == true
                                                           where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                           where TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_CODIGO == (int)GeralEnum.TipoMovimentoAgrupamento.Saida
                                                           where SaldoSubItem.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL_ID
                                                           orderby SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE
                                                           group SubItemMaterial
                                                           by new
                                                           {
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                               SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                               ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                               ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                           }
                                                               into grpSubItemMaterial
                                                               select new SubItemMaterialEntity
                                                               {
                                                                   Descricao = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                   Codigo = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),

                                                                   Almoxarifado = new AlmoxarifadoEntity(pIntAlmoxarifado_ID),
                                                                   NumeroMesesRelatorio = numMeses,

                                                                   UnidadeFornecimento = (from UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                          where UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO == grpSubItemMaterial.Key.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                                          select new UnidadeFornecimentoEntity
                                                                                          {
                                                                                              Id = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                              Codigo = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                              Descricao = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                          }).FirstOrDefault(),

                                                                   SaldoAtual = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE,

                                                                   ItemMaterial = (new ItemMaterialInfraestructure()).GetItemMaterialByItem(grpSubItemMaterial.Key.TB_ITEM_MATERIAL_ID),

                                                                   CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),        //Usado para transportar MovimentoItem_ID do MovimentoItem (tratamento elementos com lote)
                                                                   CodigoBarras = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem

                                                                   IsLote = ((grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_FABR != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue)),

                                                                   //Quase! ??? sum(mi.tb_movimento_item_qtde_mov) ???
                                                                   QtdePeriodo = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_QTDE_MOV
                                                               });

            #endregion Consulta LINQ

            #region "debug session"
            strConsultaGeral = resultSet.ToString();
            strConsultaAuxMovimentos = listaMovimentos.ToString();
            strConsultaAuxItensMovimentos = listaMovimentoItens.ToString();

            Db.GetCommand(resultSet as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaGeral = strConsultaGeral.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentos as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxMovimentos = strConsultaAuxMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentoItens as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxItensMovimentos = strConsultaAuxItensMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion "debug session"

            #region Filtro Retorno Consulta
            lObjAlmoxarifadoInfra = new AlmoxarifadoInfraestructure();
            almoxarifadoConsulta = lObjAlmoxarifadoInfra.ObterAlmoxarifado(pIntAlmoxarifado_ID.Value);

            lstRetornoConsulta = resultSet.ToList();
            lstConsultaFiltrada = lstRetornoConsulta.Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                            {
                                                                                                                Codigo = SubItemMaterial.Codigo,
                                                                                                                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                                                                                                                Descricao = SubItemMaterial.Descricao,
                                                                                                                Almoxarifado = almoxarifadoConsulta,
                                                                                                                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                                                                                                                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                                                                                                                ItemMaterial = SubItemMaterial.ItemMaterial,
                                                                                                                SaldoAtual = SubItemMaterial.SaldoAtual,
                                                                                                                QtdePeriodo = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo)
                                                                                                            }).DistinctBy(SubItemMaterial => SubItemMaterial.Codigo).ToList();

            #region SubItens em Lote
            #region "Filtro Passo 1"
            var lstConsultaFiltradaComLote = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.DistinctBy(SubItemMaterial => SubItemMaterial.SaldoAtual).ToList();

            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                            {
                                                                                                                                Codigo = SubItemMaterial.Codigo,
                                                                                                                                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                                                                                                                                Descricao = SubItemMaterial.Descricao,
                                                                                                                                Almoxarifado = almoxarifadoConsulta,
                                                                                                                                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                                                                                                                                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                                                                                                                                ItemMaterial = SubItemMaterial.ItemMaterial,
                                                                                                                                //SaldoAtual = SubItemMaterial.SaldoAtual,
                                                                                                                                //QtdePeriodo = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                //SaldoAtual = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                //QtdePeriodo = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                                                                                                                                QtdePeriodo = SubItemMaterial.QtdePeriodo,
                                                                                                                                SaldoAtual = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                            })
                                                                      .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                      .ToList();
            #endregion "Filtro Passo 1"

            #region "Filtro Passo 2"
            /*
                    CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),       //Usado para transportar MovimentoItem_ID do MovimentoItem (tratar elementos com lote)
                    CodigoBarras    = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem
                */

            var lstConsultaFiltradaComLoteIntermediaria = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLoteIntermediaria = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
            })
                                                                         .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                                {
                                                                                                                                    CodigoDescricao = SubItemMaterial.Key.CodigoDescricao,
                                                                                                                                    Codigo = SubItemMaterial.Key.Codigo,
                                                                                                                                    Descricao = SubItemMaterial.Key.Descricao,
                                                                                                                                    Almoxarifado = almoxarifadoConsulta,
                                                                                                                                    QtdePeriodo = SubItemMaterial.Key.QtdePeriodo,
                                                                                                                                    SaldoAtual = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.CodigoDescricao == SubItemMaterial.Key.CodigoDescricao).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                })
                                                                        .ToList<SubItemMaterialEntity>();

            var lstConsultaFiltradaComLoteSaida = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
                SubItemMaterial.SaldoAtual,
            })
                                                                               .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                                    {
                                                                                                                                        Codigo = SubItemMaterial.Key.Codigo,
                                                                                                                                        Descricao = SubItemMaterial.Key.Descricao,
                                                                                                                                        Almoxarifado = almoxarifadoConsulta,
                                                                                                                                        QtdePeriodo = lstConsultaFiltradaComLoteIntermediaria.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Key.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                                                                                                                                        SaldoAtual = SubItemMaterial.Key.SaldoAtual
                                                                                                                                    })
                                                                               .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                               .ToList();

            #endregion "Filtro Passo 2"
            #endregion SubItens em Lote

            #region Merging Linhas Relatorio

            SubItemMaterialEntity _filtro = null;
            foreach (var item in lstConsultaFiltradaComLoteSaida)
            {
                _filtro = lstConsultaFiltrada.Where(SubItemMaterial2 => SubItemMaterial2.Codigo == item.Codigo).FirstOrDefault();
                if (_filtro != null)
                {
                    item.CodigoFormatado = _filtro.CodigoFormatado;
                    item.NumeroMesesRelatorio = _filtro.NumeroMesesRelatorio;
                    item.ItemMaterial = _filtro.ItemMaterial;
                    item.UnidadeFornecimento = _filtro.UnidadeFornecimento;

                    lstConsultaFiltrada.Remove(_filtro);
                    lstConsultaFiltrada.Add(item);
                }
            }

            #endregion Merging Linhas Relatorio

            #endregion Filtro Retorno Consulta

            return lstConsultaFiltrada;
        }

        public IList<SubItemMaterialEntity> ImprimirPrevisaoConsumoAlmox(int? pIntAlmoxarifado_ID, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod = true)
        {
            DateTime? pDtDataInicial = (dataInicial.HasValue) ? new DateTime(dataInicial.Value.Year, dataInicial.Value.Month, 01) : new DateTime(0);
            DateTime? pDtDataFinal = (dataFinal.HasValue) ? new DateTime(dataFinal.Value.Year, dataFinal.Value.Month, 01) : new DateTime(0);

            int numMeses = dataInicial.Value.MonthDiff(dataFinal.Value);

            #region Variaveis
            string strConsultaGeral = null;
            string strConsultaAuxMovimentos = null;
            string strConsultaAuxItensMovimentos = null;

            IQueryable<TB_MOVIMENTO> listaMovimentos = null;
            IQueryable<TB_MOVIMENTO_ITEM> listaMovimentoItens = null;

            AlmoxarifadoInfraestructure lObjAlmoxarifadoInfra = null;
            AlmoxarifadoEntity almoxarifadoConsulta = null;

            List<SubItemMaterialEntity> lstRetornoConsulta = null;
            List<SubItemMaterialEntity> lstConsultaFiltrada = null;

            #endregion Variaveis

            #region Consultas Auxiliares LINQ
            listaMovimentos = from Movimento in Db.TB_MOVIMENTOs
                              where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO >= pDtDataInicial
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO < pDtDataFinal
                              select Movimento;

            listaMovimentoItens = from ItemMovimento in Db.TB_MOVIMENTO_ITEMs
                                  join Movimento in listaMovimentos on ItemMovimento.TB_MOVIMENTO_ID equals Movimento.TB_MOVIMENTO_ID
                                  join SaldoSubItem in Db.TB_SALDO_SUBITEMs on Movimento.TB_ALMOXARIFADO_ID equals SaldoSubItem.TB_ALMOXARIFADO_ID
                                  select ItemMovimento;
            #endregion Consultas Auxiliares LINQ

            #region Consulta LINQ
            IQueryable<SubItemMaterialEntity> resultSet = (from SaldoSubItem in Db.TB_SALDO_SUBITEMs
                                                           join Movimento in listaMovimentos on SaldoSubItem.TB_ALMOXARIFADO_ID equals Movimento.TB_ALMOXARIFADO_ID
                                                           join MovimentoItem in Db.TB_MOVIMENTO_ITEMs on Movimento.TB_MOVIMENTO_ID equals MovimentoItem.TB_MOVIMENTO_ID //into grpMovItem
                                                           join TipoMovimento in Db.TB_TIPO_MOVIMENTOs on Movimento.TB_TIPO_MOVIMENTO_ID equals TipoMovimento.TB_TIPO_MOVIMENTO_ID
                                                           join TipoMovimentoAgrupamento in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                           join SubItemMaterial in Db.TB_SUBITEM_MATERIALs on SaldoSubItem.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs on SubItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID
                                                           join ItemSubMaterialMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on SubItemMaterial.TB_SUBITEM_MATERIAL_ID equals ItemSubMaterialMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join ItemMaterial in Db.TB_ITEM_MATERIALs on ItemSubMaterialMaterial.TB_ITEM_MATERIAL_ID equals ItemMaterial.TB_ITEM_MATERIAL_ID

                                                           where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                                                           where Movimento.TB_MOVIMENTO_ATIVO == true
                                                           where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                           where TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_CODIGO == (int)GeralEnum.TipoMovimentoAgrupamento.Saida
                                                           where SaldoSubItem.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL_ID
                                                           orderby SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO, SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE
                                                           group SubItemMaterial
                                                           by new
                                                           {
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                               SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                               ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                               ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                           }
                                                               into grpSubItemMaterial
                                                               select new SubItemMaterialEntity
                                                               {
                                                                   Descricao = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                   Codigo = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),

                                                                   Almoxarifado = new AlmoxarifadoEntity(pIntAlmoxarifado_ID),
                                                                   NumeroMesesRelatorio = numMeses,

                                                                   UnidadeFornecimento = (from UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                          where UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO == grpSubItemMaterial.Key.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                                          select new UnidadeFornecimentoEntity
                                                                                          {
                                                                                              Id = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                              Codigo = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                              Descricao = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                          }).FirstOrDefault(),

                                                                   SaldoAtual = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE,

                                                                   ItemMaterial = (new ItemMaterialInfraestructure()).GetItemMaterialByItem(grpSubItemMaterial.Key.TB_ITEM_MATERIAL_ID),

                                                                   CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),        //Usado para transportar MovimentoItem_ID do MovimentoItem (tratamento elementos com lote)
                                                                   CodigoBarras = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem

                                                                   IsLote = ((grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_FABR != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue)),

                                                                   //Quase! ??? sum(mi.tb_movimento_item_qtde_mov) ???
                                                                   QtdePeriodo = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_QTDE_MOV
                                                               });

            #endregion Consulta LINQ

            #region "debug session"
            strConsultaGeral = resultSet.ToString();
            strConsultaAuxMovimentos = listaMovimentos.ToString();
            strConsultaAuxItensMovimentos = listaMovimentoItens.ToString();

            Db.GetCommand(resultSet as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaGeral = strConsultaGeral.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentos as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxMovimentos = strConsultaAuxMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentoItens as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxItensMovimentos = strConsultaAuxItensMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion "debug session"

            #region Filtro Retorno Consulta
            lObjAlmoxarifadoInfra = new AlmoxarifadoInfraestructure();
            almoxarifadoConsulta = lObjAlmoxarifadoInfra.ObterAlmoxarifado(pIntAlmoxarifado_ID.Value);

            lstRetornoConsulta = resultSet.ToList();
            lstConsultaFiltrada = lstRetornoConsulta.Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                            {
                                                                                                                Codigo = SubItemMaterial.Codigo,
                                                                                                                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                                                                                                                Descricao = SubItemMaterial.Descricao,
                                                                                                                Almoxarifado = almoxarifadoConsulta,
                                                                                                                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                                                                                                                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                                                                                                                ItemMaterial = SubItemMaterial.ItemMaterial,
                                                                                                                SaldoAtual = SubItemMaterial.SaldoAtual,
                                                                                                                QtdePeriodo = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo)
                                                                                                            }).DistinctBy(SubItemMaterial => SubItemMaterial.Codigo).ToList();

            #region SubItens em Lote
            #region "Filtro Passo 1"
            var lstConsultaFiltradaComLote = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.DistinctBy(SubItemMaterial => SubItemMaterial.SaldoAtual).ToList();

            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                            {
                                                                                                                                Codigo = SubItemMaterial.Codigo,
                                                                                                                                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                                                                                                                                Descricao = SubItemMaterial.Descricao,
                                                                                                                                Almoxarifado = almoxarifadoConsulta,
                                                                                                                                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                                                                                                                                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                                                                                                                                ItemMaterial = SubItemMaterial.ItemMaterial,
                                                                                                                                //SaldoAtual = SubItemMaterial.SaldoAtual,
                                                                                                                                //QtdePeriodo = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                //SaldoAtual = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                //QtdePeriodo = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                                                                                                                                QtdePeriodo = SubItemMaterial.QtdePeriodo,
                                                                                                                                SaldoAtual = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                            })
                                                                      .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                      .ToList();
            #endregion "Filtro Passo 1"

            #region "Filtro Passo 2"
            /*
                    CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),       //Usado para transportar MovimentoItem_ID do MovimentoItem (tratar elementos com lote)
                    CodigoBarras    = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem
                */

            var lstConsultaFiltradaComLoteIntermediaria = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLoteIntermediaria = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
            })
                                                                         .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                                {
                                                                                                                                    CodigoDescricao = SubItemMaterial.Key.CodigoDescricao,
                                                                                                                                    Codigo = SubItemMaterial.Key.Codigo,
                                                                                                                                    Descricao = SubItemMaterial.Key.Descricao,
                                                                                                                                    Almoxarifado = almoxarifadoConsulta,
                                                                                                                                    QtdePeriodo = SubItemMaterial.Key.QtdePeriodo,
                                                                                                                                    SaldoAtual = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.CodigoDescricao == SubItemMaterial.Key.CodigoDescricao).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                                                                                })
                                                                        .ToList<SubItemMaterialEntity>();

            var lstConsultaFiltradaComLoteSaida = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
                SubItemMaterial.SaldoAtual,
            })
                                                                               .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                                                                                    {
                                                                                                                                        Codigo = SubItemMaterial.Key.Codigo,
                                                                                                                                        Descricao = SubItemMaterial.Key.Descricao,
                                                                                                                                        Almoxarifado = almoxarifadoConsulta,
                                                                                                                                        QtdePeriodo = lstConsultaFiltradaComLoteIntermediaria.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Key.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                                                                                                                                        SaldoAtual = SubItemMaterial.Key.SaldoAtual
                                                                                                                                    })
                                                                               .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                               .ToList();

            #endregion "Filtro Passo 2"
            #endregion SubItens em Lote

            #region Merging Linhas Relatorio

            SubItemMaterialEntity _filtro = null;
            foreach (var item in lstConsultaFiltradaComLoteSaida)
            {
                _filtro = lstConsultaFiltrada.Where(SubItemMaterial2 => SubItemMaterial2.Codigo == item.Codigo).FirstOrDefault();
                if (_filtro != null)
                {
                    item.CodigoFormatado = _filtro.CodigoFormatado;
                    item.NumeroMesesRelatorio = _filtro.NumeroMesesRelatorio;
                    item.ItemMaterial = _filtro.ItemMaterial;
                    item.UnidadeFornecimento = _filtro.UnidadeFornecimento;

                    lstConsultaFiltrada.Remove(_filtro);
                    lstConsultaFiltrada.Add(item);
                }
            }

            #endregion Merging Linhas Relatorio

            #endregion Filtro Retorno Consulta

            return lstConsultaFiltrada;
        }

        public IList<SubItemMaterialEntity> ImprimirConsumoSubItem(int? pIntDivisao_ID, int? pIntAlmoxarifado_ID, DateTime? dataInicial, DateTime? dataFinal, bool NewMethod)
        {
            DateTime? pDtDataInicial = (dataInicial.HasValue) ? new DateTime(dataInicial.Value.Year, dataInicial.Value.Month, 01) : new DateTime(0);
            DateTime? pDtDataFinal = (dataFinal.HasValue) ? new DateTime(dataFinal.Value.Year, dataFinal.Value.Month, 01) : new DateTime(0);

            int numMeses = dataInicial.Value.MonthDiff(dataFinal.Value);

            #region Variaveis
            string strConsultaGeral = null;
            string strConsultaAuxMovimentos = null;
            string strConsultaAuxItensMovimentos = null;

            IQueryable<TB_MOVIMENTO> listaMovimentos = null;
            IQueryable<TB_MOVIMENTO_ITEM> listaMovimentoItens = null;

            AlmoxarifadoInfraestructure lObjAlmoxarifadoInfra = null;
            AlmoxarifadoEntity almoxarifadoConsulta = null;

            List<SubItemMaterialEntity> lstRetornoConsulta = null;
            List<SubItemMaterialEntity> lstConsultaFiltrada = null;

            #endregion Variaveis

            #region Consultas Auxiliares LINQ
            listaMovimentos = from Movimento in Db.TB_MOVIMENTOs
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO >= pDtDataInicial
                              where Movimento.TB_MOVIMENTO_DATA_MOVIMENTO < pDtDataFinal
                              where Movimento.TB_MOVIMENTO_ATIVO == true
                              select Movimento;

            listaMovimentoItens = from ItemMovimento in Db.TB_MOVIMENTO_ITEMs
                                  join Movimento in listaMovimentos on ItemMovimento.TB_MOVIMENTO_ID equals Movimento.TB_MOVIMENTO_ID
                                  where ItemMovimento.TB_MOVIMENTO_ITEM_ATIVO == true
                                  select ItemMovimento;
            #endregion Consultas Auxiliares LINQ

            #region Consulta LINQ
            IQueryable<SubItemMaterialEntity> resultSet = (from MovimentoItem in Db.TB_MOVIMENTO_ITEMs
                                                           join Movimento in listaMovimentos on MovimentoItem.TB_MOVIMENTO_ID equals Movimento.TB_MOVIMENTO_ID
                                                           join TipoMovimento in Db.TB_TIPO_MOVIMENTOs on Movimento.TB_TIPO_MOVIMENTO_ID equals TipoMovimento.TB_TIPO_MOVIMENTO_ID
                                                           join TipoMovimentoAgrupamento in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                           join SubItemMaterial in Db.TB_SUBITEM_MATERIALs on MovimentoItem.TB_SUBITEM_MATERIAL_ID equals SubItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs on SubItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID
                                                           join ItemSubMaterialMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on SubItemMaterial.TB_SUBITEM_MATERIAL_ID equals ItemSubMaterialMaterial.TB_SUBITEM_MATERIAL_ID
                                                           join ItemMaterial in Db.TB_ITEM_MATERIALs on ItemSubMaterialMaterial.TB_ITEM_MATERIAL_ID equals ItemMaterial.TB_ITEM_MATERIAL_ID

                                                           where Movimento.TB_ALMOXARIFADO_ID == pIntAlmoxarifado_ID
                                                           where Movimento.TB_MOVIMENTO_ATIVO == true
                                                           where MovimentoItem.TB_MOVIMENTO_ITEM_ATIVO == true
                                                           where TipoMovimentoAgrupamento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_CODIGO == (int)GeralEnum.TipoMovimentoAgrupamento.Saida
                                                           where MovimentoItem.TB_SUBITEM_MATERIAL_ID == MovimentoItem.TB_SUBITEM_MATERIAL_ID
                                                           group SubItemMaterial
                                                           by new
                                                           {
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                               //SaldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                               SubItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_ID,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                               MovimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                               ItemMaterial.TB_ITEM_MATERIAL_ID,
                                                               ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                                           }
                                                               into grpSubItemMaterial
                                                               select new SubItemMaterialEntity
                                                               {
                                                                   Descricao = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                   Codigo = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = grpSubItemMaterial.Key.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),

                                                                   Almoxarifado = new AlmoxarifadoEntity(pIntAlmoxarifado_ID),
                                                                   NumeroMesesRelatorio = numMeses,

                                                                   UnidadeFornecimento = (from UnidadeFornecimento in Db.TB_UNIDADE_FORNECIMENTOs
                                                                                          where UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO == grpSubItemMaterial.Key.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                                          select new UnidadeFornecimentoEntity
                                                                                          {
                                                                                              Id = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                              Codigo = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                              Descricao = UnidadeFornecimento.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                          }).FirstOrDefault(),

                                                                   //SaldoAtual = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE,

                                                                   ItemMaterial = (new ItemMaterialInfraestructure()).GetItemMaterialByItem(grpSubItemMaterial.Key.TB_ITEM_MATERIAL_ID),

                                                                   CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),        //Usado para transportar MovimentoItem_ID do MovimentoItem (tratamento elementos com lote)
                                                                   //CodigoBarras = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem

                                                                   IsLote = ((grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_FABR != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT != null) || (grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC.HasValue)),

                                                                   //Quase! ??? sum(mi.tb_movimento_item_qtde_mov) ???
                                                                   QtdePeriodo = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_QTDE_MOV
                                                               });

            #endregion Consulta LINQ

            #region "debug session"
            strConsultaGeral = resultSet.ToString();
            strConsultaAuxMovimentos = listaMovimentos.ToString();
            strConsultaAuxItensMovimentos = listaMovimentoItens.ToString();

            Db.GetCommand(resultSet as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaGeral = strConsultaGeral.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentos as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxMovimentos = strConsultaAuxMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            Db.GetCommand(listaMovimentoItens as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strConsultaAuxItensMovimentos = strConsultaAuxItensMovimentos.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion "debug session"

            #region Filtro Retorno Consulta
            lObjAlmoxarifadoInfra = new AlmoxarifadoInfraestructure();
            almoxarifadoConsulta = lObjAlmoxarifadoInfra.ObterAlmoxarifado(pIntAlmoxarifado_ID.Value);

            lstRetornoConsulta = resultSet.ToList();
            lstConsultaFiltrada = lstRetornoConsulta.Select(SubItemMaterial => new SubItemMaterialEntity()
            {
                Codigo = SubItemMaterial.Codigo,
                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                Descricao = SubItemMaterial.Descricao,
                Almoxarifado = almoxarifadoConsulta,
                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                ItemMaterial = SubItemMaterial.ItemMaterial,
                SaldoAtual = SubItemMaterial.SaldoAtual,
                QtdePeriodo = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo)
            }).DistinctBy(SubItemMaterial => SubItemMaterial.Codigo).ToList();

            #region SubItens em Lote
            #region "Filtro Passo 1"
            var lstConsultaFiltradaComLote = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.DistinctBy(SubItemMaterial => SubItemMaterial.SaldoAtual).ToList();

            lstConsultaFiltradaComLote = lstConsultaFiltradaComLote.Select(SubItemMaterial => new SubItemMaterialEntity()
            {
                Codigo = SubItemMaterial.Codigo,
                CodigoFormatado = SubItemMaterial.CodigoFormatado,
                Descricao = SubItemMaterial.Descricao,
                Almoxarifado = almoxarifadoConsulta,
                NumeroMesesRelatorio = SubItemMaterial.NumeroMesesRelatorio,
                UnidadeFornecimento = SubItemMaterial.UnidadeFornecimento,
                ItemMaterial = SubItemMaterial.ItemMaterial,
                SaldoAtual = SubItemMaterial.SaldoAtual
                //QtdePeriodo = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                //SaldoAtual = __lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
                //QtdePeriodo = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                //QtdePeriodo = SubItemMaterial.QtdePeriodo,
                //SaldoAtual = lstConsultaFiltradaComLote.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Codigo).Sum(Somatoria => Somatoria.SaldoAtual)
            })
                                                                      .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                      .ToList();
            #endregion "Filtro Passo 1"

            #region "Filtro Passo 2"
            /*
                    CodigoDescricao = grpSubItemMaterial.Key.TB_MOVIMENTO_ITEM_ID.ToString(),       //Usado para transportar MovimentoItem_ID do MovimentoItem (tratar elementos com lote)
                    CodigoBarras    = grpSubItemMaterial.Key.TB_SALDO_SUBITEM_SALDO_QTDE.ToString(), //Usado para transportar saldo do MovimentoItem
                */

            var lstConsultaFiltradaComLoteIntermediaria = lstRetornoConsulta.Where(SubItemMaterial => (SubItemMaterial.IsLote.HasValue && SubItemMaterial.IsLote == true)).ToList();
            lstConsultaFiltradaComLoteIntermediaria = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
            })
                                                                         .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                         {
                                                                             CodigoDescricao = SubItemMaterial.Key.CodigoDescricao,
                                                                             Codigo = SubItemMaterial.Key.Codigo,
                                                                             Descricao = SubItemMaterial.Key.Descricao,
                                                                             Almoxarifado = almoxarifadoConsulta,
                                                                             QtdePeriodo = SubItemMaterial.Key.QtdePeriodo,
                                                                             SaldoAtual = lstRetornoConsulta.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.CodigoDescricao == SubItemMaterial.Key.CodigoDescricao).Sum(Somatoria => Somatoria.SaldoAtual)
                                                                         })
                                                                        .ToList<SubItemMaterialEntity>();

            var lstConsultaFiltradaComLoteSaida = lstConsultaFiltradaComLoteIntermediaria.GroupBy(SubItemMaterial => new
            {
                SubItemMaterial.CodigoDescricao,
                SubItemMaterial.Codigo,
                SubItemMaterial.QtdePeriodo,
                SubItemMaterial.Descricao,
                SubItemMaterial.SaldoAtual,
            })
                                                                               .Select(SubItemMaterial => new SubItemMaterialEntity()
                                                                               {
                                                                                   Codigo = SubItemMaterial.Key.Codigo,
                                                                                   Descricao = SubItemMaterial.Key.Descricao,
                                                                                   Almoxarifado = almoxarifadoConsulta,
                                                                                   QtdePeriodo = lstConsultaFiltradaComLoteIntermediaria.Where(SubItemMaterialConsultado => SubItemMaterialConsultado.Codigo == SubItemMaterial.Key.Codigo).Sum(Somatoria => Somatoria.QtdePeriodo),
                                                                                   SaldoAtual = SubItemMaterial.Key.SaldoAtual
                                                                               })
                                                                               .DistinctBy(SubItemMaterial => SubItemMaterial.Codigo)
                                                                               .ToList();

            #endregion "Filtro Passo 2"
            #endregion SubItens em Lote

            #region Merging Linhas Relatorio

            SubItemMaterialEntity _filtro = null;
            foreach (var item in lstConsultaFiltradaComLoteSaida)
            {
                _filtro = lstConsultaFiltrada.Where(SubItemMaterial2 => SubItemMaterial2.Codigo == item.Codigo).FirstOrDefault();
                if (_filtro != null)
                {
                    item.CodigoFormatado = _filtro.CodigoFormatado;
                    item.NumeroMesesRelatorio = _filtro.NumeroMesesRelatorio;
                    item.ItemMaterial = _filtro.ItemMaterial;
                    item.UnidadeFornecimento = _filtro.UnidadeFornecimento;

                    lstConsultaFiltrada.Remove(_filtro);
                    lstConsultaFiltrada.Add(item);
                }
            }

            #endregion Merging Linhas Relatorio

            #endregion Filtro Retorno Consulta

            return lstConsultaFiltrada;
        }

        public IList<SaldoSubItemEntity> ListarFechamento(int? almoxId, int? anomes)
        {
            this.Db.CommandTimeout = 180; //3 minutos

            DateTime dataInicial = new DateTime(Convert.ToInt32(anomes.ToString().PadLeft(6, '0').Substring(0, 4)), Convert.ToInt32(anomes.ToString().PadLeft(6, '0').Substring(4, 2)), 1);
            DateTime dataFinal = new DateTime(Convert.ToInt32(anomes.ToString().PadLeft(6, '0').Substring(0, 4)), Convert.ToInt32(anomes.ToString().PadLeft(6, '0').Substring(4, 2)), 1);

            // sempre acrescentar 1 mês
            dataFinal = dataFinal.AddMonths(1);

            // soma saldo anterior
            //IEnumerable<SaldoSubItemEntity> resultado = (from am in Db.TB_SUBITEM_MATERIAL_ALMOXes
            IQueryable<SaldoSubItemEntity> resultado = (from am in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                        join s in Db.TB_SALDO_SUBITEMs on
                                                            new { am.TB_ALMOXARIFADO_ID, am.TB_SUBITEM_MATERIAL_ID }
                                                            equals new { s.TB_ALMOXARIFADO_ID, s.TB_SUBITEM_MATERIAL_ID }
                                                        where s.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == almoxId
                                                        group s by new
                                                        {
                                                            s.TB_SUBITEM_MATERIAL,
                                                            s.TB_ALMOXARIFADO,
                                                            s.TB_UGE,
                                                        } into g
                                                        select new SaldoSubItemEntity
                                                        {
                                                            Id = g.Key.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                            CodigoFormatado = g.Key.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            SubItemMaterial = new SubItemMaterialEntity
                                                            {
                                                                Id = g.Key.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                Codigo = g.Key.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                Descricao = g.Key.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                NaturezaDespesa = new NaturezaDespesaEntity
                                                                {
                                                                    Id = g.Key.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                    Codigo = g.Key.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                    CodigoFormatado = g.Key.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().PadLeft(8, '0'),
                                                                    Descricao = g.Key.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                }
                                                            },
                                                            AnoMesReferencia_Impressao = anomes,
                                                            UGE = new UGEEntity
                                                            {
                                                                Id = g.Key.TB_UGE.TB_UGE_ID,
                                                                Codigo = g.Key.TB_UGE.TB_UGE_CODIGO,
                                                                Descricao = g.Key.TB_UGE.TB_UGE_DESCRICAO

                                                            },
                                                            Almoxarifado = new AlmoxarifadoEntity
                                                            {
                                                                Id = g.Key.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                Descricao = g.Key.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                            },
                                                            SaldoQtde = g.Sum(a => a.TB_SALDO_SUBITEM_SALDO_QTDE),
                                                            SaldoValor = g.Sum(a => a.TB_SALDO_SUBITEM_SALDO_VALOR)
                                                        }).AsQueryable();

            IList<SaldoSubItemEntity> result = resultado.ToList();

            // calcular movimentação do mês.

            var movimento = (from mi in Db.TB_MOVIMENTO_ITEMs
                             where (mi.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO >= dataInicial &&
                                    mi.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO < dataFinal)
                             where mi.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == almoxId
                             where mi.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             group mi by new
                             {
                                 mi.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                 mi.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                 mi.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                 mi.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                 mi.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                 mi.TB_UGE.TB_UGE_ID
                             } into g
                             select new
                             {
                                 SubitemMaterialId = g.Key.TB_SUBITEM_MATERIAL_ID,
                                 AlmoxId = g.Key.TB_ALMOXARIFADO_ID,
                                 UgeId = g.Key.TB_UGE_ID,
                                 LoteIdent = g.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                 LoteFabric = g.Key.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                 LoteDtVenc = g.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                 SomaEntradaPeriodo = g.Where(a => a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 1).Where(a => a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true).Sum(a => a.TB_MOVIMENTO_ITEM_QTDE_MOV) ?? 0,
                                 SomaSaidaPeriodo = g.Where(a => a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 2).Where(a => a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true).Sum(a => a.TB_MOVIMENTO_ITEM_QTDE_MOV) ?? 0,
                                 SomaValorEntradaPeriodo = g.Where(a => a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 1).Where(a => a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true).Sum(a => a.TB_MOVIMENTO_ITEM_VALOR_MOV) ?? 0,
                                 SomaValorSaidaPeriodo = g.Where(a => a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 2).Where(a => a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true).Sum(a => a.TB_MOVIMENTO_ITEM_VALOR_MOV) ?? 0
                             }).ToList();

            // procurar o saldo anterior
            var anoMesAnt = (from r in Db.TB_FECHAMENTOs
                             where r.TB_ALMOXARIFADO_ID == almoxId
                             where r.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                             select r).OrderByDescending(r => r.TB_FECHAMENTO_ANO_MES_REF).FirstOrDefault();

            var saldoAnt = (from sa in Db.TB_FECHAMENTOs
                            where sa.TB_FECHAMENTO_ANO_MES_REF ==
                                (anoMesAnt != null ? (int)anoMesAnt.TB_FECHAMENTO_ANO_MES_REF : 0)
                            where sa.TB_ALMOXARIFADO_ID == almoxId
                            where sa.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                            group sa by new
                            {
                                sa.TB_SUBITEM_MATERIAL_ID,
                                sa.TB_ALMOXARIFADO_ID,
                                sa.TB_UGE_ID,
                            } into g
                            select new
                            {
                                SubitemMaterialId = g.Key.TB_SUBITEM_MATERIAL_ID,
                                AlmoxId = g.Key.TB_ALMOXARIFADO_ID,
                                UgeId = g.Key.TB_UGE_ID,
                                SaldoAnt = g.Sum(sl => sl.TB_FECHAMENTO_SALDO_QTDE) ?? 0,
                                ValorAnt = g.Sum(sl => sl.TB_FECHAMENTO_SALDO_VALOR) ?? 0
                            }).ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].SaldoAnterior = saldoAnt.Where(
                            a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            a.AlmoxId == result[i].Almoxarifado.Id &&
                            a.UgeId == result[i].UGE.Id).Sum(a => a.SaldoAnt);

                    result[i].SaldoAnteriorValor = saldoAnt.Where(
                            a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            a.AlmoxId == result[i].Almoxarifado.Id &&
                            a.UgeId == result[i].UGE.Id).Sum(a => a.ValorAnt);

                    if (!result[i].SaldoAnterior.HasValue) result[i].SaldoAnterior = 0;
                    if (!result[i].SaldoAnteriorValor.HasValue) result[i].SaldoAnteriorValor = 0;

                    if (movimento != null)
                    {
                        result[i].QtdeSaida = movimento.Where(
                        a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                        a.AlmoxId == result[i].Almoxarifado.Id &&
                        a.UgeId == result[i].UGE.Id).Sum(a => a.SomaSaidaPeriodo);

                        result[i].ValSaida = movimento.Where(
                            a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            a.AlmoxId == result[i].Almoxarifado.Id &&
                            a.UgeId == result[i].UGE.Id).Sum(a => a.SomaValorSaidaPeriodo);

                        if (saldoAnt.Count > 0) // esse tratamento é temporário e será excluído após a limpeza no BD
                        {
                            result[i].QtdeEntrada = movimento.Where(
                                a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                                a.AlmoxId == result[i].Almoxarifado.Id &&
                                a.UgeId == result[i].UGE.Id).Sum(a => a.SomaEntradaPeriodo);

                            result[i].ValEntrada = movimento.Where(
                                a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                                a.AlmoxId == result[i].Almoxarifado.Id &&
                                a.UgeId == result[i].UGE.Id).Sum(a => a.SomaValorEntradaPeriodo);
                        }
                        else // este trecho: apagar caso reajuste o BD inteiro!
                        {
                            result[i].QtdeEntrada = //result[i].SaldoQtde + 
                                movimento.Where(
                                a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                                a.AlmoxId == result[i].Almoxarifado.Id &&
                                a.UgeId == result[i].UGE.Id).Sum(a => a.SomaEntradaPeriodo);

                            result[i].ValEntrada = //result[i].SaldoValor + 
                                movimento.Where(
                                a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                                a.AlmoxId == result[i].Almoxarifado.Id &&
                                a.UgeId == result[i].UGE.Id).Sum(a => a.SomaValorEntradaPeriodo);
                        }

                        if (!result[i].QtdeEntrada.HasValue) result[i].QtdeEntrada = 0;
                        if (!result[i].ValEntrada.HasValue) result[i].ValEntrada = 0;

                        // abaixo, o cálculo correto para o saldo mensal
                        result[i].QtdeFechamento = (result[i].SaldoAnterior ?? 0) + (result[i].QtdeEntrada - result[i].QtdeSaida);
                        result[i].ValFechamento = (result[i].SaldoAnteriorValor ?? 0) + (result[i].ValEntrada - result[i].ValSaida);
                    }
                }
            }

            result = result.OrderBy(saldoSubitem => saldoSubitem.SubItemMaterialDescr).ToList();

            return result;
        }

        public IList<SaldoSubItemEntity> Imprimir()
        {
            return (from a in this.Db.TB_SALDO_SUBITEMs
                    where a.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItemMaterial.Id
                    where a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id
                    where a.TB_UGE_ID == (this.Entity.UGE.Id ?? 0)
                    select new SaldoSubItemEntity
                    {
                        Id = a.TB_SALDO_SUBITEM_ID,
                        Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                        UGE = (from uge in Db.TB_UGEs
                               where uge.TB_UGE_ID == a.TB_UGE_ID
                               select new UGEEntity
                               {
                                   Id = uge.TB_UGE_ID,
                                   Codigo = uge.TB_UGE_CODIGO,
                                   Descricao = string.Format("{0} - {1}", uge.TB_UGE_CODIGO.ToString().PadLeft(6, '0'), uge.TB_UGE_DESCRICAO)
                               }
                               ).FirstOrDefault(),
                        SubItemMaterial = (from b in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                           join c in Db.TB_SUBITEM_MATERIALs on b.TB_SUBITEM_MATERIAL_ID equals c.TB_SUBITEM_MATERIAL_ID
                                           join d in Db.TB_ALMOXARIFADOs on b.TB_ALMOXARIFADO_ID equals d.TB_ALMOXARIFADO_ID
                                           where b.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
                                           where b.TB_ALMOXARIFADO_ID == a.TB_ALMOXARIFADO_ID
                                           select new SubItemMaterialEntity
                                           {
                                               Id = b.TB_SUBITEM_MATERIAL_ID,
                                               Descricao = c.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0') + " - " + c.TB_SUBITEM_MATERIAL_DESCRICAO,
                                               UnidadeFornecimento = (from u in Db.TB_UNIDADE_FORNECIMENTOs
                                                                      where c.TB_UNIDADE_FORNECIMENTO_ID == u.TB_UNIDADE_FORNECIMENTO_ID
                                                                      select new UnidadeFornecimentoEntity
                                                                      {
                                                                          Id = u.TB_UNIDADE_FORNECIMENTO_ID,
                                                                          Codigo = u.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                          Descricao = u.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                      }).FirstOrDefault()
                                           }).FirstOrDefault(),
                        LoteDataVenc = a.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                        LoteFabr = a.TB_SALDO_SUBITEM_LOTE_FAB,
                        LoteIdent = a.TB_SALDO_SUBITEM_LOTE_IDENT,
                        PrecoUnit = a.TB_SALDO_SUBITEM_PRECO_UNIT,
                        SaldoQtde = a.TB_SALDO_SUBITEM_SALDO_QTDE,
                        SaldoValor = a.TB_SALDO_SUBITEM_SALDO_VALOR,
                        QtdeReservaMaterial = (from r in Db.TB_RESERVA_MATERIALs
                                               where a.TB_SUBITEM_MATERIAL_ID == r.TB_SUBITEM_MATERIAL_ID
                                               where a.TB_ALMOXARIFADO_ID == r.TB_ALMOXARIFADO_ID
                                               where a.TB_UGE_ID == r.TB_UGE_ID
                                               select new ReservaMaterialEntity
                                               {
                                                   Quantidade = r.TB_RESERVA_MATERIAL_QUANT.Value
                                               }
                                               ).Sum(d => d.Quantidade)
                    }).ToList<SaldoSubItemEntity>();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            TB_SALDO_SUBITEM saldoSubItem = new TB_SALDO_SUBITEM();
            if (this.Entity.Id.HasValue)
            {

                IEnumerable<TB_SALDO_SUBITEM> resultado = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_SALDO_SUBITEM_ID == this.Entity.Id.Value &&
                                 a.TB_UGE_ID == this.Entity.UGE.Id.Value &&
                                 a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id.Value);

                if (this.Entity.LoteFabr != null)
                    resultado = resultado.Where(a => a.TB_SALDO_SUBITEM_LOTE_FAB == this.Entity.LoteFabr);

                if (this.Entity.LoteIdent != null)
                    resultado = resultado.Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == this.Entity.LoteIdent);

                if (this.Entity.LoteDataVenc != null)
                    resultado = resultado.Where(a => a.TB_SALDO_SUBITEM_LOTE_DT_VENC == this.Entity.LoteDataVenc);

                saldoSubItem = resultado.FirstOrDefault<TB_SALDO_SUBITEM>();

                //saldoSubItem = this.Db.TB_SALDO_SUBITEMs.Where(a => a.TB_SALDO_SUBITEM_ID == this.Entity.Id.Value &&
                //                 a.TB_UGE_ID == this.Entity.UGE.Id.Value &&
                //                 a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id.Value &&
                //                 (string.IsNullOrEmpty(this.Entity.LoteIdent) ? a.TB_SALDO_SUBITEM_LOTE_IDENT == null : a.TB_SALDO_SUBITEM_LOTE_IDENT == this.Entity.LoteIdent)).FirstOrDefault();

                if (saldoSubItem == null)
                {
                    saldoSubItem = new TB_SALDO_SUBITEM();
                    saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT = this.Entity.LoteIdent;
                    saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB = this.Entity.LoteFabr;
                    saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC = this.Entity.LoteDataVenc;
                }
            }
            else
            {
                this.Db.TB_SALDO_SUBITEMs.InsertOnSubmit(saldoSubItem);
                saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT = this.Entity.LoteIdent;
                saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB = this.Entity.LoteFabr;
                saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC = this.Entity.LoteDataVenc;
            }

            saldoSubItem.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id.Value;
            saldoSubItem.TB_SUBITEM_MATERIAL_ID = this.Entity.SubItemMaterial.Id.Value;
            saldoSubItem.TB_SALDO_SUBITEM_PRECO_UNIT = this.Entity.PrecoUnit;
            saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE = this.Entity.SaldoQtde;
            saldoSubItem.TB_SALDO_SUBITEM_SALDO_VALOR = this.Entity.SaldoValor;
            saldoSubItem.TB_UGE_ID = this.Entity.UGE.Id.Value;
            this.Db.SubmitChanges();
        }

        public void Salvar(List<SaldoSubItemEntity> SubItens)
        {
            foreach (SaldoSubItemEntity saldo in SubItens)
            {
                this.Entity = saldo;
                Salvar();
            }
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        public IList<UGEEntity> ConsultarUgesBySubItemAlmox(int almoxarifado, int subItem, int ugeId)
        {
            var res = (from reserva in Db.TB_RESERVA_MATERIALs
                       where (reserva.TB_SUBITEM_MATERIAL_ID == subItem)
                       where (reserva.TB_UGE_ID == ugeId)
                       where (reserva.TB_ALMOXARIFADO_ID == almoxarifado)
                       where (reserva.TB_SUBITEM_MATERIAL_ID == subItem)
                       group reserva by new { reserva.TB_RESERVA_MATERIAL_QUANT }
                           into reservaGrup
                           select new ReservaMaterialEntity
                           {
                               Quantidade = reservaGrup.Sum(a => a.TB_RESERVA_MATERIAL_QUANT.Value)
                           }).FirstOrDefault();

            IList<UGEEntity> lista = (from saldo in Db.TB_SALDO_SUBITEMs
                                      where (saldo.TB_ALMOXARIFADO_ID == almoxarifado)
                                      where (saldo.TB_SUBITEM_MATERIAL_ID == subItem)
                                      select new UGEEntity
                                      {
                                          Id = saldo.TB_UGE_ID,
                                          Descricao = string.Format("{0} - {1} - Saldo: {2}"
                                          , saldo.TB_UGE.TB_UGE_ID.ToString().PadLeft(6, '0')
                                          , saldo.TB_UGE.TB_UGE_DESCRICAO
                                          , saldo.TB_SALDO_SUBITEM_SALDO_QTDE - ((res == null) ? 0 : res.Quantidade)),
                                      }
                                      ).ToList();
            return lista;
        }

        public SaldoSubItemEntity Consultar(int? SubItemMaterialId, int? AlmoxId, int? UgeId, string _LoteIdent, string fabricanteLote, DateTime? dataVencimentoLote)
        {
            SubItemMaterialInfraestructure subItem = new SubItemMaterialInfraestructure();

            SaldoSubItemEntity retorno = (from a in this.Db.TB_SALDO_SUBITEMs.
                                              //  Where(a => a.TB_SALDO_SUBITEM_LOTE_FAB == fabricanteLote || a.TB_SALDO_SUBITEM_LOTE_FAB == null).
                                              // Where(a => a.TB_SALDO_SUBITEM_LOTE_DT_VENC == dataVencimentoLote || a.TB_SALDO_SUBITEM_LOTE_DT_VENC == null).
                                              //Where(a => a.TB_SALDO_SUBITEM_LOTE_IDENT == _LoteIdent || a.TB_SALDO_SUBITEM_LOTE_IDENT == null).
                    Where(a => a.TB_ALMOXARIFADO_ID == AlmoxId).
                    Where(a => a.TB_UGE_ID == UgeId).
                    Where(a => a.TB_SUBITEM_MATERIAL_ID == SubItemMaterialId)
                                          select new SaldoSubItemEntity
                                          {
                                              Id = a.TB_SALDO_SUBITEM_ID,
                                              Almoxarifado = new AlmoxarifadoEntity(a.TB_ALMOXARIFADO_ID),
                                              UGE = new UGEEntity(a.TB_UGE_ID),
                                              SubItemMaterial = (from b in Db.TB_SUBITEM_MATERIALs
                                                                 where b.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
                                                                 select new SubItemMaterialEntity
                                                                 {
                                                                     Id = b.TB_SUBITEM_MATERIAL_ID,
                                                                     CodigoDescricao = b.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0') + " - " + b.TB_SUBITEM_MATERIAL_DESCRICAO
                                                                 }
                                                                 ).FirstOrDefault(),
                                              LoteDataVenc = a.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                              LoteFabr = a.TB_SALDO_SUBITEM_LOTE_FAB,
                                              LoteIdent = a.TB_SALDO_SUBITEM_LOTE_IDENT,
                                              PrecoUnit = a.TB_SALDO_SUBITEM_PRECO_UNIT,
                                              SaldoQtde = a.TB_SALDO_SUBITEM_SALDO_QTDE,
                                              SaldoValor = a.TB_SALDO_SUBITEM_SALDO_VALOR
                                          }).FirstOrDefault();

            return retorno;
        }

        public SaldoSubItemEntity ConsultarSaldoSubItem(AlmoxarifadoEntity almoxarifado, UGEEntity uge, SubItemMaterialEntity subItem)
        {

            SaldoSubItemEntity rSaldo = (from saldo in this.Db.TB_SALDO_SUBITEMs
                                         where saldo.TB_SUBITEM_MATERIAL_ID == subItem.Id.Value
                                         where saldo.TB_ALMOXARIFADO_ID == almoxarifado.Id.Value
                                         where saldo.TB_UGE_ID == uge.Id.Value
                                         group saldo by new { saldo.TB_SUBITEM_MATERIAL_ID, saldo.TB_ALMOXARIFADO_ID, saldo.TB_UGE_ID } into g
                                         select new SaldoSubItemEntity
                                         {
                                             SaldoQtde = g.Sum(saldo => saldo.TB_SALDO_SUBITEM_SALDO_QTDE),
                                             SaldoValor = g.Sum(saldo => saldo.TB_SALDO_SUBITEM_SALDO_VALOR)
                                             //QtdeReservaMaterial = rReserva.Quantidade
                                         }).FirstOrDefault();

            return rSaldo;
        }

        /// <summary>
        /// Consulta utilizada no estoque Sintético
        /// </summary>
        /// <param name="UgeId"></param>
        /// <param name="AlmoxId"></param>
        /// <param name="GrupoId"></param>
        /// <param name="ComSemSaldo"></param>
        /// <returns></returns>
        public IList<SaldoSubItemEntity> ImprimirConsultaEstoqueSintetico(int UgeId, int AlmoxId, int GrupoId, int ComSemSaldo, int? _ordenarPor = 0)
        {
            IQueryable<SaldoSubItemEntity> resultado = (from a in this.Db.TB_SALDO_SUBITEMs
                                                        join l in this.Db.TB_SALDO_SUBITEM_LOTEs on a.TB_SALDO_SUBITEM_ID equals l.TB_SALDO_SUBITEM_ID
                                                        where (a.TB_ALMOXARIFADO_ID == AlmoxId)
                                                            select new SaldoSubItemEntity
                                                            {
                                                                Id = a.TB_SALDO_SUBITEM_ID,
                                                                Almoxarifado = new AlmoxarifadoEntity
                                                                {
                                                                    Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                    Codigo = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                                                                    Descricao = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                }
                                                                ,
                                                                UGE = new UGEEntity
                                                                       {
                                                                           Id = a.TB_UGE.TB_UGE_ID,
                                                                           Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                                           Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                                                       },
                                                                NaturezaDespesaId = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                SubItemMaterial = new SubItemMaterialEntity
                                                                       {
                                                                           Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                           Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                           Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,                                                                          
                                                                           UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                                                 {
                                                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                                 },
                                                                           NaturezaDespesa = new NaturezaDespesaEntity
                                                                                             {
                                                                                                 Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                                 CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().PadLeft(8, '0'),
                                                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                                             },
                                                                           ItemMaterial = (from c in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                           where a.TB_SUBITEM_MATERIAL_ID == c.TB_SUBITEM_MATERIAL_ID
                                                                                           select new ItemMaterialEntity
                                                                                           {
                                                                                               Id = c.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                               Codigo = c.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                               Descricao = c.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                               Material = new MaterialEntity
                                                                                               {
                                                                                                   Id = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID,
                                                                                                   Codigo = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                                                                                   Classe = new ClasseEntity
                                                                                                   {
                                                                                                       Id = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                                                                                       Codigo = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO,
                                                                                                       Grupo = new GrupoEntity
                                                                                                       {
                                                                                                           Id = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                                                                                           Codigo = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO,
                                                                                                           Descricao = c.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_DESCRICAO
                                                                                                       }
                                                                                                   }
                                                                                               }
                                                                                           }).FirstOrDefault()
                                                                       },
                                                                LoteDataVenc = l.TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                                                LoteFabr = a.TB_SALDO_SUBITEM_LOTE_FAB,
                                                                LoteIdent = l.TB_SALDO_SUBITEM_LOTE_IDENT,
                                                                LoteSaldoQtde = l.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE,
                                                                PrecoUnit = a.TB_SALDO_SUBITEM_PRECO_UNIT == null ? 0 : a.TB_SALDO_SUBITEM_PRECO_UNIT,
                                                                SaldoQtde = a.TB_SALDO_SUBITEM_SALDO_QTDE,
                                                                SaldoValor = a.TB_SALDO_SUBITEM_SALDO_VALOR,
                                                                QtdeReservaMaterial = (from r in Db.TB_RESERVA_MATERIALs
                                                                                       where a.TB_SUBITEM_MATERIAL_ID == r.TB_SUBITEM_MATERIAL_ID
                                                                                       where a.TB_ALMOXARIFADO_ID == r.TB_ALMOXARIFADO_ID
                                                                                       where a.TB_UGE_ID == r.TB_UGE_ID
                                                                                       select new ReservaMaterialEntity
                                                                                       {
                                                                                           Quantidade = r.TB_RESERVA_MATERIAL_QUANT == null ? 0 : r.TB_RESERVA_MATERIAL_QUANT.Value
                                                                                       }
                                                                                       ).Sum(d => d.Quantidade)
                                                            });

            if (UgeId != 0)
                resultado = resultado.Where(a => a.UGE.Id == UgeId);

            if (GrupoId != 0)
                resultado = resultado.Where(a => a.SubItemMaterial.ItemMaterial.Material.Classe.Grupo.Id == GrupoId);

            //Itens com saldo
            if (ComSemSaldo == 1)
            {
                resultado = resultado.Where(a => a.LoteSaldoQtde > 0);
            }
            //Itens sem saldo
            else if (ComSemSaldo == 2)
            {
                resultado = resultado.Where(a => a.SaldoQtde <= 0);
            }



            #region Truncar Valores Retorno
            resultado.Cast<SaldoSubItemEntity>().ToList().ForEach(_rowSaldoSubitem =>
            {
                _rowSaldoSubitem.PrecoUnit = ((_rowSaldoSubitem.PrecoUnit.HasValue) ? (decimal.Parse(_rowSaldoSubitem.PrecoUnit.ToString()).truncarDuasCasas()) : 0.00m);
                _rowSaldoSubitem.QtdeReservaMaterial = ((_rowSaldoSubitem.QtdeReservaMaterial.HasValue) ? ((decimal.Parse(_rowSaldoSubitem.QtdeReservaMaterial.Value.Truncar(3).ToString()))) : 0.000m);
                _rowSaldoSubitem.SaldoValor = ((_rowSaldoSubitem.SaldoValor.HasValue) ? (decimal.Parse(_rowSaldoSubitem.SaldoValor.ToString()).truncarDuasCasas()) : 0.00m);
            });

            #endregion Truncar Valores Retorno

            List<SaldoSubItemEntity> lstTeste = new List<SaldoSubItemEntity>();
            switch (_ordenarPor)
            {
                case 0:
                    var ordenadoPorCodigo = resultado.ToList().OrderBy(x => x.SubItemMaterialCodigo).ToList();
                    lstTeste = ordenadoPorCodigo.ToList();
                    break;
                case 1:
                    var ordenadoPorDescricao = resultado.ToList().OrderBy(x => x.SubItemMaterialDescr).ToList();
                    lstTeste = ordenadoPorDescricao.ToList();
                    break;
                case 2:
                    var ordenadoPorSaldo = resultado.ToList().OrderByDescending(x => x.SaldoQtde).ToList();
                    lstTeste = ordenadoPorSaldo.ToList();
                    break;
                default:
                    break;
            }

            var query = (from t in resultado.ToList()
                         group t by new { t.Id, t.NaturezaDespesaId, t.SaldoQtde, t.SaldoValor }
                             into grp
                             select new
                             {
                                 grp.Key.Id,
                                 grp.Key.NaturezaDespesaId,
                                 grp.Key.SaldoQtde,
                                 grp.Key.SaldoValor
                             }).ToList();


            var somatoriaTotalQtdeSubitens = query.Sum(SubitemMaterial => SubitemMaterial.SaldoQtde.Value);
            var somatoriaTotalValorSubitens = query.Sum(SubitemMaterial => SubitemMaterial.SaldoValor.Value);
            int numeroSubitens = lstTeste.DistinctBy(x => x.SubItemMaterialCodigo).Count();

            foreach (var item in lstTeste)
            {
                var consultaParaSomatoria = query.Where(x => x.NaturezaDespesaId == item.NaturezaDespesaId);

                item.SaldoValorTotal = consultaParaSomatoria.Sum(y => y.SaldoValor);
                item.SaldoQtdTotal = consultaParaSomatoria.Sum(y => y.SaldoQtde);

                //Campos Somatoria Relatorio Sintético
                item.SomatoriaTotalQtdeSubitens = somatoriaTotalQtdeSubitens;
                item.SomatoriaTotalValorSubitens = somatoriaTotalValorSubitens;

                item.SomatoriaQtdeSubitensMaterial = numeroSubitens;
            }
            return lstTeste;
            
        }

        public decimal? CalculaTotalSaldoUGEsReserva(int? subItemId, int? almoxId)
        {

            var res = (from reserva in this.Db.TB_RESERVA_MATERIALs
                       where reserva.TB_ALMOXARIFADO_ID == almoxId
                       where reserva.TB_SUBITEM_MATERIAL_ID == subItemId
                       select new ReservaMaterialEntity
                       {
                           Quantidade = reserva.TB_RESERVA_MATERIAL_QUANT.Value
                       }).ToList().Sum(a => a.Quantidade);


            var saldo = (from saldoSubItem in this.Db.TB_SALDO_SUBITEMs
                         where saldoSubItem.TB_ALMOXARIFADO_ID == almoxId
                         where saldoSubItem.TB_SUBITEM_MATERIAL_ID == subItemId
                         select new SaldoSubItemEntity
                         {
                             SaldoQtde = saldoSubItem.TB_SALDO_SUBITEM_SALDO_QTDE
                         }).ToList().Sum(a => a.SaldoQtde);

            return (saldo - res);
        }

        public Tuple<decimal?,decimal?,decimal?> SaldoMovimentoItemDataMovimento(int? idSubItem, int? idAlmoxarifado, int? idUge, DateTime dataMovimento)
        {
            var resultado = (from a in this.Db.TB_MOVIMENTO_ITEMs
                             where a.TB_SUBITEM_MATERIAL_ID == idSubItem
                             where a.TB_MOVIMENTO.TB_ALMOXARIFADO_ID == idAlmoxarifado
                             where a.TB_UGE_ID == idUge
                             where a.TB_MOVIMENTO_ITEM_ATIVO == true
                             where a.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true
                             //where a.TB_MOVIMENTO_ITEM_LOTE_IDENT == lote 
                             where (a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada
                             || a.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Sam.Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                             orderby a.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO.Date descending, a.TB_MOVIMENTO_ITEM_ID descending
                             select new MovimentoItemEntity
                             {
                                 Id = a.TB_MOVIMENTO_ITEM_ID,
                                 SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                 SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                 SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                 PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                 IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                 Movimento = (from m in Db.TB_MOVIMENTOs
                                              where a.TB_MOVIMENTO_ID == m.TB_MOVIMENTO_ID
                                              select new MovimentoEntity
                                              {
                                                  Id = m.TB_MOVIMENTO_ID,
                                                  TipoMovimento = (from t in Db.TB_TIPO_MOVIMENTOs
                                                                   where m.TB_TIPO_MOVIMENTO_ID == t.TB_TIPO_MOVIMENTO_ID
                                                                   select new TipoMovimentoEntity
                                                                   {
                                                                       Id = t.TB_TIPO_MOVIMENTO_ID,
                                                                       TipoAgrupamento = new TipoMovimentoAgrupamentoEntity((int)t.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID)
                                                                   }).FirstOrDefault(),
                                                  DataMovimento = m.TB_MOVIMENTO_DATA_MOVIMENTO,
                                                  Almoxarifado = new AlmoxarifadoEntity(m.TB_ALMOXARIFADO_ID),
                                              }).FirstOrDefault()


                             }).AsQueryable();


            if (dataMovimento != DateTime.MinValue)
                resultado = resultado.Where(a => a.Movimento.DataMovimento.Value.Date <= dataMovimento);


            var resultadoFiltro = resultado.OrderByDescending(a => a.Id).OrderByDescending(a => a.Movimento.DataMovimento);


            if (resultadoFiltro.FirstOrDefault() != null)
                 return new Tuple<decimal?,decimal?,decimal?>(resultadoFiltro.FirstOrDefault().SaldoQtde,resultadoFiltro.FirstOrDefault().SaldoValor,resultadoFiltro.FirstOrDefault().PrecoUnit); 
            else
               return new Tuple<decimal?,decimal?,decimal?>(0,0,0);


        }

        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? ugeId, int? almoxId)
        {
            List<SaldoSubItemEntity> retorno = new List<SaldoSubItemEntity>();

            var retornot = (from saldoSubItem in this.Db.TB_SALDO_SUBITEMs
                            where saldoSubItem.TB_ALMOXARIFADO_ID == almoxId
                            && saldoSubItem.TB_SUBITEM_MATERIAL_ID == subItemId
                            && saldoSubItem.TB_UGE_ID == ugeId

                            group saldoSubItem by new { saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC, saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT, saldoSubItem.TB_SALDO_SUBITEM_LOTE_FAB }
                                into saldoGrup
                                select new SaldoSubItemEntity
                                {
                                    Id = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_ID
                                }).FirstOrDefault();

            if (retornot != null)
            {
                int id = Convert.ToInt32(retornot.Id);

                retorno = (from saldoSubItem in this.Db.TB_SALDO_SUBITEM_LOTEs
                           where saldoSubItem.TB_SALDO_SUBITEM_ID == id
                           where saldoSubItem.TB_SALDO_SUBITEM_LOTE_SALDO_QTDE > 0
                           group saldoSubItem by new { saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC, saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT }
                               into saldoGrup
                               select new SaldoSubItemEntity
                               {
                                   Id = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_ID,
                                   LoteDataVenc = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                   LoteIdent = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_IDENT,
                                   SaldoQtde = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_SALDO_QTDE,
                                   IdLote = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_ID
                               }).ToList();
            }


            return retorno;
        }


        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? almoxId, string lote)
        {
            List<SaldoSubItemEntity> retorno = new List<SaldoSubItemEntity>();

            int id = (from saldo in Db.TB_SALDO_SUBITEMs
                      where saldo.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id
                      where saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == subItemId
                      select saldo).FirstOrDefault().TB_SALDO_SUBITEM_ID;


            if (id != null)
            {
                retorno = (from saldoSubItem in this.Db.TB_SALDO_SUBITEM_LOTEs
                           where saldoSubItem.TB_SALDO_SUBITEM_ID == id
                           && (saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT == lote || lote == "0")
                           group saldoSubItem by new { saldoSubItem.TB_SALDO_SUBITEM_LOTE_DT_VENC, saldoSubItem.TB_SALDO_SUBITEM_LOTE_IDENT }
                               into saldoGrup
                               select new SaldoSubItemEntity
                               {
                                   Id = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_ID,
                                   LoteDataVenc = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_DT_VENC,
                                   LoteIdent = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_IDENT,
                                   SaldoQtde = saldoGrup.FirstOrDefault().TB_SALDO_SUBITEM_LOTE_SALDO_QTDE
                               }).ToList();
            }

            return retorno;
        }
    }
}
