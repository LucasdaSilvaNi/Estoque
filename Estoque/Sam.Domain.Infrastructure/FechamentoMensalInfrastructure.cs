using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;
using Sam.Domain.Entity.Relatorios;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;

namespace Sam.Domain.Infrastructure
{
    public partial class FechamentoMensalInfrastructure : BaseInfraestructure, IFechamentoMensalService
    {
        private FechamentoMensalEntity Fechamento = new FechamentoMensalEntity();

        public FechamentoMensalEntity Entity
        {
            get { return Fechamento; }
            set { Fechamento = value; }
        }

        public IList<String> fechamentoErro { get; set; }

        private IList<FechamentoMensalEntity> ListaFechamento = new List<FechamentoMensalEntity>();
        private readonly int i;

        public AlmoxarifadoEntity ObterAlmoxarifado(int? idAlmoxarifado)
        {
            AlmoxarifadoEntity lObjRetorno = null;

            if (idAlmoxarifado.HasValue)
            {
                lObjRetorno = (from Almoxarifado in Db.TB_ALMOXARIFADOs
                               where Almoxarifado.TB_ALMOXARIFADO_ID == idAlmoxarifado
                               select new AlmoxarifadoEntity
                               {
                                   Id = Almoxarifado.TB_ALMOXARIFADO_ID,
                                   Codigo = Almoxarifado.TB_ALMOXARIFADO_CODIGO,
                                   Descricao = Almoxarifado.TB_ALMOXARIFADO_DESCRICAO,
                                   EnderecoLogradouro = Almoxarifado.TB_ALMOXARIFADO_LOGRADOURO,
                                   EnderecoNumero = Almoxarifado.TB_ALMOXARIFADO_NUMERO,
                                   EnderecoCompl = Almoxarifado.TB_ALMOXARIFADO_COMPLEMENTO,
                                   EnderecoBairro = Almoxarifado.TB_ALMOXARIFADO_BAIRRO,
                                   EnderecoMunicipio = Almoxarifado.TB_ALMOXARIFADO_MUNICIPIO,
                                   EnderecoCep = Almoxarifado.TB_ALMOXARIFADO_CEP,
                                   EnderecoTelefone = Almoxarifado.TB_ALMOXARIFADO_TELEFONE,
                                   EnderecoFax = Almoxarifado.TB_ALMOXARIFADO_FAX,
                                   Responsavel = Almoxarifado.TB_ALMOXARIFADO_RESPONSAVEL,
                                   MesRef = Almoxarifado.TB_ALMOXARIFADO_MES_REF,
                                   Uge = new UGEEntity(Almoxarifado.TB_UGE_ID),
                                   Gestor = new GestorEntity(Almoxarifado.TB_GESTOR_ID),
                                   RefInicial = Almoxarifado.TB_ALMOXARIFADO_MES_REF_INICIAL,                                  
                                   IndicadorAtividade = new IndicadorAtividadeEntity(Convert.ToInt32(Almoxarifado.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))
                               }).FirstOrDefault();
            }

            return lObjRetorno;
        }

        public bool ContemFechamento(int almoxarifadoId)
        {
            var qryConsulta = (from rowFechamentoAlmox in Db.TB_FECHAMENTOs
                               where rowFechamentoAlmox.TB_ALMOXARIFADO_ID == almoxarifadoId
                               where rowFechamentoAlmox.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                               select rowFechamentoAlmox).Take(1);

            if (qryConsulta.ToList().Count > 0)
                return true;
            else
                return false;

        }

        public int? ListarUltimoFechamento(int? almoxarifadoId)
        {
            int anoMesRef = -1;

            if (!almoxarifadoId.HasValue)
                throw new Exception("almoxarifadoId está nulo");

            //int? anoMesRef = (from a in Db.TB_FECHAMENTOs
            IQueryable<TB_FECHAMENTO> qryConsulta = (from rowFechamentoAlmox in Db.TB_FECHAMENTOs
                                                     where rowFechamentoAlmox.TB_ALMOXARIFADO_ID == almoxarifadoId
                                                     where rowFechamentoAlmox.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                                                     orderby rowFechamentoAlmox.TB_FECHAMENTO_ANO_MES_REF descending
                                                     select rowFechamentoAlmox).Take(1);

            TB_FECHAMENTO rowFechamento = qryConsulta.FirstOrDefault();

            if (rowFechamento.IsNotNull())
            {
                anoMesRef = rowFechamento.TB_FECHAMENTO_ANO_MES_REF;
                return anoMesRef;
            }
            else
                return null;

        }

        public IList<FechamentoMensalEntity> Listar()
        {
            throw new NotImplementedException();
        }

        public IList<FechamentoMensalEntity> Listar(int pIntAlmoxarifadoID, bool agruparFechamentos)
        {
            string lStrSQL = null;

            List<FechamentoMensalEntity> lLstRetorno = null;
            IEnumerable<FechamentoMensalEntity> lObjEnumRetorno = null;

            lObjEnumRetorno = (from Fechamento in Db.TB_FECHAMENTOs
                               where Fechamento.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoID
                               where Fechamento.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                               group Fechamento by
                               new
                               {
                                   AnoMesRef = Fechamento.TB_FECHAMENTO_ANO_MES_REF,
                                   //Almoxarifado = new EstoquePatrimonio.Domain.Entity.AlmoxarifadoEntity(Fechamento.TB_ALMOXARIFADO_ID),
                                   //UGE = new EstoquePatrimonio.Domain.Entity.UGEEntity(Fechamento.TB_UGE_ID),
                                   Almoxarifado_ID = Fechamento.TB_ALMOXARIFADO_ID,
                                   Uge_ID = Fechamento.TB_UGE_ID,
                                   SubItemMaterial_ID = Fechamento.TB_SUBITEM_MATERIAL_ID
                                  


                               } into grpFechamentos
                               select new FechamentoMensalEntity()
                               {
                                   AnoMesRef = grpFechamentos.Key.AnoMesRef,
                                   Almoxarifado = new AlmoxarifadoEntity(grpFechamentos.Key.Almoxarifado_ID),
                                   UGE = new UGEEntity(grpFechamentos.Key.Uge_ID),
                                   SubItemMaterial = new SubItemMaterialEntity(grpFechamentos.Key.SubItemMaterial_ID),
                                   SaldoQtde = grpFechamentos.Sum(Somatoria => Somatoria.TB_FECHAMENTO_SALDO_QTDE),
                                   SaldoValor = grpFechamentos.Sum(Somatoria => Somatoria.TB_FECHAMENTO_SALDO_VALOR),

                
                               }
                                 

                               
                               );

            lStrSQL = lObjEnumRetorno.ToString();
            Db.GetCommand(lObjEnumRetorno as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            lLstRetorno = lObjEnumRetorno.ToList();

            return lLstRetorno;
        }

        public IList<FechamentoMensalEntity> _xpListar(int almoxID, bool agruparFechamentos)
        {
            string strSQL = null;

            IQueryable<TB_FECHAMENTO> qryConsulta = null;
            IList<TB_FECHAMENTO> lstRowsFechamentoAlmox = null;
            IList<FechamentoMensalEntity> lstRetornoFechamentoAlmox = null;
            int statusFechamento = (int)GeralEnum.SituacaoFechamento.Executar;


            qryConsulta = (from rowFechamentoAlmox in Db.TB_FECHAMENTOs
                           where rowFechamentoAlmox.TB_ALMOXARIFADO_ID == almoxID
                           where rowFechamentoAlmox.TB_FECHAMENTO_SITUACAO == statusFechamento
                           select rowFechamentoAlmox).AsQueryable<TB_FECHAMENTO>();

            strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            lstRowsFechamentoAlmox = qryConsulta.ToList();
            lstRetornoFechamentoAlmox = lstRowsFechamentoAlmox.GroupBy(rowFechamentoAlmox => new
            {
                rowFechamentoAlmox.TB_FECHAMENTO_ANO_MES_REF,
                rowFechamentoAlmox.TB_ALMOXARIFADO_ID,
                rowFechamentoAlmox.TB_UGE_ID,
                rowFechamentoAlmox.TB_SUBITEM_MATERIAL_ID
            }).Select(grpFechamentoAlmox => new FechamentoMensalEntity()
            {
                AnoMesRef = grpFechamentoAlmox.Key.TB_FECHAMENTO_ANO_MES_REF,
                Almoxarifado = new AlmoxarifadoEntity() { Id = grpFechamentoAlmox.Key.TB_ALMOXARIFADO_ID },
                UGE = new UGEEntity() { Id = grpFechamentoAlmox.Key.TB_UGE_ID },
                SubItemMaterial = new SubItemMaterialEntity() { Id = grpFechamentoAlmox.Key.TB_SUBITEM_MATERIAL_ID },
                SaldoQtde = grpFechamentoAlmox.Sum(somatoriaQtde => somatoriaQtde.TB_FECHAMENTO_SALDO_QTDE),
                SaldoValor = grpFechamentoAlmox.Sum(somatoriaQtde => somatoriaQtde.TB_FECHAMENTO_SALDO_VALOR),



            }).ToList();
            return lstRetornoFechamentoAlmox;
        }

        public IList<string> ListarMesesFechados(int almoxId)
        {
            string strSQL = null;
            int statusFechamento = (int)GeralEnum.SituacaoFechamento.Executar;
            //Listagem da Pagina Balancete
            List<string> lstRetorno = null;
            Expression<Func<TB_FECHAMENTO, bool>> expWhere = null;
            IQueryable<TB_FECHAMENTO> qryConsulta = null;

            expWhere = (fechamentoConsulta => fechamentoConsulta.TB_ALMOXARIFADO_ID == almoxId
                                           && fechamentoConsulta.TB_FECHAMENTO_SITUACAO == statusFechamento);

            qryConsulta = Db.TB_FECHAMENTOs.Where(expWhere);

            lstRetorno = qryConsulta.GroupBy(fechamentoCampos => new { fechamentoCampos.TB_FECHAMENTO_ANO_MES_REF, fechamentoCampos.TB_ALMOXARIFADO_ID })
                                    .OrderBy(grpFechamentos => grpFechamentos.Key.TB_FECHAMENTO_ANO_MES_REF)
                                    .Select(mesRef => (String.Format("{0}/{1}", mesRef.Key.TB_FECHAMENTO_ANO_MES_REF.ToString().Substring(4, 2), mesRef.Key.TB_FECHAMENTO_ANO_MES_REF.ToString().Substring(0, 4))))
                                    .ToList();


            strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            return lstRetorno;
        }

        public IList<FechamentoMensalEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public FechamentoMensalEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        private IList<FechamentoMensalEntity> ProcessarSaldoAnterior(IQueryable<FechamentoMensalEntity> fechamentoList)
        {
            string anoMesAnt = Common.Util.TratamentoDados.ValidarAnoMesRef(Entity.AnoMesRef.ToString(), -1);

            var saldoAnt = (from sa in Db.TB_FECHAMENTOs
                            where sa.TB_FECHAMENTO_ANO_MES_REF ==
                                (anoMesAnt != null ? Convert.ToInt32(anoMesAnt) : 0)
                            where sa.TB_ALMOXARIFADO_ID == Entity.Almoxarifado.Id
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

            IList<FechamentoMensalEntity> result = fechamentoList.ToList();

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
                }

                System.Diagnostics.Trace.WriteLine(String.Format("Saldo Anterior: {0}", result.Sum(a => a.SaldoAnterior).ToString()));
                System.Diagnostics.Trace.WriteLine(String.Format("Saldo Anterior Valor: {0}", result.Sum(a => a.SaldoAnteriorValor).ToString()));
            }

            return result;
        }

        /// <summary>
        /// Relatórios:
        /// MensalAnalitico:
        /// MensalAnaliticoConsumo:
        /// MensalAnaliticoPatrimonio:
        /// MensalBalancete:
        /// MensalBalanceteConsumo:
        /// BalanceteSimulacao:
        /// MensalBalancetePatrimonio:
        /// MensalGrupoClasseMaterial:
        /// MensalInventario:
        /// </summary>
        /// <returns></returns>
        public IList<FechamentoMensalEntity> Imprimir()
        {
            Db.CommandTimeout = 0;
            IQueryable<FechamentoMensalEntity> resultado = (from a in Db.TB_FECHAMENTOs
                                                            where a.TB_FECHAMENTO_SITUACAO == Entity.SituacaoFechamento
                                                            join b in Db.TB_SUBITEM_MATERIALs.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                                equals b.TB_SUBITEM_MATERIAL_ID
                                                            select new FechamentoMensalEntity
                                                            {
                                                                Id = a.TB_FECHAMENTO_ID,
                                                                Almoxarifado = new AlmoxarifadoEntity
                                                                {
                                                                    Id = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                                                                    Codigo = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                                                                    Descricao = a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO
                                                                },
                                                                AnoMesRef = a.TB_FECHAMENTO_ANO_MES_REF,
                                                                SubItemMaterial = new SubItemMaterialEntity
                                                                {
                                                                    CodigoDescricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                    Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                    Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                    CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                    Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                    NaturezaDespesa = new NaturezaDespesaEntity
                                                                    {
                                                                        Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                        Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                        CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().PadLeft(8, '0'),
                                                                        Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                                        Natureza = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                                                                    },
                                                                    UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                    {
                                                                        Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                        Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                        Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                    },
                                                                    ItemMaterial = (from item in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                    where b.TB_SUBITEM_MATERIAL_ID == item.TB_SUBITEM_MATERIAL_ID
                                                                                    select new ItemMaterialEntity
                                                                                    {
                                                                                        Id = item.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                        Codigo = item.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                        CodigoFormatado = item.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(8, '0'),
                                                                                        Descricao = item.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,

                                                                                        Material = new MaterialEntity
                                                                                        {
                                                                                            Id = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID,
                                                                                            Codigo = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                                                                            CodigoFormatado = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_CODIGO.ToString().PadLeft(8, '0'),
                                                                                            Descricao = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_DESCRICAO,

                                                                                            Classe = new ClasseEntity
                                                                                            {
                                                                                                Id = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                                                                                Codigo = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO,
                                                                                                CodigoFormatado = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO.ToString().PadLeft(2, '0'),
                                                                                                Descricao = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_DESCRICAO,
                                                                                                Grupo = new GrupoEntity
                                                                                                {
                                                                                                    Id = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                                                                                    Codigo = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO,
                                                                                                    CodigoFormatado = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO.ToString().PadLeft(2, '0'),
                                                                                                    Descricao = item.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_DESCRICAO
                                                                                                }
                                                                                            }
                                                                                        }

                                                                                    }).FirstOrDefault(),
                                                                },
                                                                UGE = new UGEEntity
                                                                {
                                                                    Id = a.TB_UGE.TB_UGE_ID,
                                                                    Codigo = a.TB_UGE.TB_UGE_CODIGO,
                                                                    CodigoFormatado = a.TB_UGE.TB_UGE_CODIGO.ToString().PadLeft(6, '0'),
                                                                    Descricao = a.TB_UGE.TB_UGE_DESCRICAO
                                                                },
                                                                QtdeEntrada = a.TB_FECHAMENTO_QTDE_MOV_ENT,
                                                                ValorEntrada = a.TB_FECHAMENTO_VALOR_MOV_ENT,
                                                                QtdeSaida = a.TB_FECHAMENTO_QTDE_MOV_SAI,
                                                                ValorSaida = a.TB_FECHAMENTO_VALOR_MOV_SAI,
                                                                SaldoQtde = a.TB_FECHAMENTO_SALDO_QTDE,
                                                                SaldoValor = (a.TB_FECHAMENTO_SALDO_VALOR == null ? 0 : a.TB_FECHAMENTO_SALDO_VALOR),
                                                                SituacaoFechamento = a.TB_FECHAMENTO_SITUACAO
                                                            });

            // filtrar
            if (Entity.AnoMesRef.HasValue)
                resultado = resultado.Where(a => a.AnoMesRef == Entity.AnoMesRef);

            if (!Entity.chkSaldoMaiorZero)
                resultado = resultado.Where(a => (a.QtdeSaida > 0 || a.SaldoQtde > 0 || a.SaldoValor != 0));

            if (Entity.Almoxarifado != null && Entity.Almoxarifado.Id.HasValue)
                resultado = resultado.Where(a => a.Almoxarifado.Id == Entity.Almoxarifado.Id);

            if (Entity.SubItemMaterial.NaturezaDespesa.Natureza != null)
                resultado = resultado.Where(a => a.SubItemMaterial.NaturezaDespesa.Natureza == Entity.SubItemMaterial.NaturezaDespesa.Natureza);

            var strSQL = resultado.ToString();
            Db.GetCommand(resultado).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            // procurar o saldo anterior
            string anoMesAnt = Common.Util.TratamentoDados.ValidarAnoMesRef(Entity.AnoMesRef.ToString(), -1);

            var query = (from sa in Db.TB_FECHAMENTOs
                            where sa.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                            where sa.TB_FECHAMENTO_ANO_MES_REF ==
                                (anoMesAnt != null ? Convert.ToInt32(anoMesAnt) : 0)
                            where sa.TB_ALMOXARIFADO_ID == Entity.Almoxarifado.Id
                            group sa by new
                            {
                                sa.TB_SUBITEM_MATERIAL_ID,
                                sa.TB_ALMOXARIFADO_ID,
                                sa.TB_UGE_ID,
                                sa.TB_FECHAMENTO_SITUACAO
                            } into g
                            select new
                            {
                                SubitemMaterialId = g.Key.TB_SUBITEM_MATERIAL_ID,
                                AlmoxId = g.Key.TB_ALMOXARIFADO_ID,
                                UgeId = g.Key.TB_UGE_ID,
                                SaldoAnt = (g.Sum(sl => sl.TB_FECHAMENTO_SALDO_QTDE) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraQuantidade),
                                ValorAnt = (g.Sum(sl => sl.TB_FECHAMENTO_SALDO_VALOR) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraValorMonetario),
                                ValorEntrada33 = (g.Where(x => x.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "33")
                                .Sum(sl => sl.TB_FECHAMENTO_VALOR_MOV_ENT) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraValorMonetario),
                                ValorSaida33 = (g.Where(x => x.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "33")
                                .Sum(sl => sl.TB_FECHAMENTO_VALOR_MOV_SAI) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraValorMonetario),
                                ValorEntrada44 = (g.Where(x => x.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "44")
                                .Sum(sl => sl.TB_FECHAMENTO_VALOR_MOV_ENT) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraValorMonetario),
                                ValorSaida44 = (g.Where(x => x.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().Substring(0, 2) == "44")
                                .Sum(sl => sl.TB_FECHAMENTO_VALOR_MOV_SAI) ?? 0.00m).Truncar((int)GeralEnum.casasDecimais.paraValorMonetario),

                                //NaturezaDespesa = (from n in Db.TB_NATUREZA_DESPESAs
                                //                   join m in Db.TB_SUBITEM_MATERIALs on n.TB_NATUREZA_DESPESA_ID equals m.TB_NATUREZA_DESPESA_ID
                                //                   where m.TB_SUBITEM_MATERIAL_ID == g.Key.TB_SUBITEM_MATERIAL_ID
                                //                   select n.TB_NATUREZA_DESPESA_CODIGO).FirstOrDefault(),
                                situacao = g.Key.TB_FECHAMENTO_SITUACAO

                            });

            strSQL = query.ToString();

            Db.GetCommand(query).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            var saldoAnt = query.AsNoTracking().ToList();


            IList<FechamentoMensalEntity> result = resultado.ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].SaldoAnterior = saldoAnt.Where(
                            a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            a.AlmoxId == result[i].Almoxarifado.Id &&
                            a.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&

                            a.UgeId == result[i].UGE.Id).Sum(a => a.SaldoAnt.Truncar((int)GeralEnum.casasDecimais.paraQuantidade));

                    result[i].SaldoAnteriorValor = saldoAnt.Where(
                            a => a.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            a.AlmoxId == result[i].Almoxarifado.Id &&
                            a.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&
                            a.UgeId == result[i].UGE.Id).Sum(a => a.ValorAnt.Truncar((int)GeralEnum.casasDecimais.paraValorMonetario));

                    var saldoAnterior33 = result.Where(x => x.NaturezaDespesa.ToString().Substring(0, 2) == "33").FirstOrDefault();

                    if (saldoAnterior33 != null)
                    {
                        if (result[i].NaturezaDespesa.ToString().Substring(0, 2) == "33")
                            result[i].SaldoAnteriorValor33 = result[i].SaldoAnteriorValor + (result[i].ValorEntrada - result[i].ValorSaida);
                    }

                    var saldoAnterior44 = result.Where(x => x.NaturezaDespesa.ToString().Substring(0, 2) == "44").FirstOrDefault();

                    if (saldoAnterior44 != null)
                    {
                        if (result[i].NaturezaDespesa.ToString().Substring(0, 2) == "44")
                            result[i].SaldoAnteriorValor44 = result[i].SaldoAnteriorValor + (result[i].ValorEntrada - result[i].ValorSaida);


                    }



                    if (!result[i].SaldoAnterior.HasValue) result[i].SaldoAnterior = 0;
                    if (!result[i].SaldoAnteriorValor.HasValue) result[i].SaldoAnteriorValor = 0;
                    if (!result[i].SaldoAnteriorValor33.HasValue) result[i].SaldoAnteriorValor33 = 0;
                    if (!result[i].SaldoAnteriorValor44.HasValue) result[i].SaldoAnteriorValor44 = 0;


                }
            }

            #region Truncar Valores Retorno
            result.Cast<FechamentoMensalEntity>().ToList().ForEach(_rowFechamento =>
            {
                //_rowFechamento.SaldoAnterior = ((_rowFechamento.SaldoAnterior.HasValue) ? (decimal.Parse(_rowFechamento.SaldoAnterior.ToString()).truncarDuasCasas()):0.00m);
                //_rowFechamento.SaldoAnteriorValor = ((_rowFechamento.SaldoAnteriorValor.HasValue) ? (decimal.Parse(_rowFechamento.SaldoAnteriorValor.ToString()).truncarDuasCasas()):0.00m);
                //_rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? (decimal.Parse(_rowFechamento.SaldoValor.ToString()).truncarDuasCasas()) : 0.00m);

                //_rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? (decimal.Parse(_rowFechamento.ValorEntrada.ToString()).truncarDuasCasas()):0.00m);
                //_rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? (decimal.Parse(_rowFechamento.ValorSaida.ToString()).truncarDuasCasas()):0.00m);

                //_rowFechamento.SaldoAnterior = ((_rowFechamento.SaldoAnterior.HasValue) ? _rowFechamento.SaldoAnterior.Value.Truncar(2) : 0.00m);
                //_rowFechamento.SaldoAnteriorValor = ((_rowFechamento.SaldoAnteriorValor.HasValue) ? _rowFechamento.SaldoAnteriorValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.ValorEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.ValorSaida.Value.Truncar(2) : 0.00m);
            });

            #endregion Truncar Valores Retorno

            if (Entity.SituacaoFechamento == 0)
                Excluir(Entity.AnoMesRef.Value, Entity.Almoxarifado.Id.Value, Entity.SituacaoFechamento);

            return result;

        }

        public IQueryable<T> gerarConsultaRelatorioMensalFechamento<T>(int almoxID, int anoMesRef, int? statusSituacao) where T : relFechamentoMensalBase, new()
        {

            IQueryable<T> qryConsulta = (from rowFechamento in Db.TB_FECHAMENTOs
                                         join subitemMaterial in Db.TB_SUBITEM_MATERIALs on rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID equals subitemMaterial.TB_SUBITEM_MATERIAL_ID
                                         where rowFechamento.TB_ALMOXARIFADO_ID == almoxID
                                         where rowFechamento.TB_FECHAMENTO_ANO_MES_REF == anoMesRef
                                         //where rowFechamento.TB_FECHAMENTO_SITUACAO == statusSituacao
                                         group rowFechamento by new
                                         {
                                             anoMesRef = rowFechamento.TB_FECHAMENTO_ANO_MES_REF,
                                             natDespesa = rowFechamento.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA,
                                             subitemMaterial = rowFechamento.TB_SUBITEM_MATERIAL,
                                             almox = rowFechamento.TB_ALMOXARIFADO,
                                             uge = rowFechamento.TB_UGE,
                                             StatusSituacao = rowFechamento.TB_FECHAMENTO_SITUACAO

                                         } into grpAnaliticoBalanceteMensal
                                         select new T
                                         {
                                             Almoxarifado = new AlmoxarifadoEntity
                                             {
                                                 Id = grpAnaliticoBalanceteMensal.Key.almox.TB_ALMOXARIFADO_ID,
                                                 Codigo = grpAnaliticoBalanceteMensal.Key.almox.TB_ALMOXARIFADO_CODIGO,
                                                 Descricao = grpAnaliticoBalanceteMensal.Key.almox.TB_ALMOXARIFADO_DESCRICAO
                                             },
                                             AnoMesRef = anoMesRef,
                                             SubItemMaterial = new SubItemMaterialEntity
                                             {
                                                 //CodigoFormatado = rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                 Id = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                 Codigo = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                 Descricao = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                 ItemMaterial = (from itemSubitemmaterial in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                 where grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID == itemSubitemmaterial.TB_SUBITEM_MATERIAL_ID
                                                                 select new ItemMaterialEntity
                                                                 {
                                                                     Id = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                     Codigo = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                     //CodigoFormatado = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(8, '0'),
                                                                     Descricao = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                                                 }).FirstOrDefault(),
                                                 NaturezaDespesa = new NaturezaDespesaEntity
                                                 {
                                                     Id = grpAnaliticoBalanceteMensal.Key.natDespesa.TB_NATUREZA_DESPESA_ID,
                                                     Codigo = grpAnaliticoBalanceteMensal.Key.natDespesa.TB_NATUREZA_DESPESA_CODIGO,
                                                     Descricao = grpAnaliticoBalanceteMensal.Key.natDespesa.TB_NATUREZA_DESPESA_DESCRICAO
                                                 },
                                                 UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                 {
                                                     Id = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                     Codigo = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                     Descricao = grpAnaliticoBalanceteMensal.Key.subitemMaterial.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                 }
                                             },
                                             UGE = new UGEEntity
                                             {
                                                 Id = grpAnaliticoBalanceteMensal.Key.uge.TB_UGE_ID,
                                                 Codigo = grpAnaliticoBalanceteMensal.Key.uge.TB_UGE_CODIGO,
                                                 Descricao = grpAnaliticoBalanceteMensal.Key.uge.TB_UGE_DESCRICAO
                                             },
                                             SituacaoFechamento = grpAnaliticoBalanceteMensal.Key.StatusSituacao.Value,
                                             QtdeEntrada = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_ENT),
                                             QtdeSaida = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_SAI),
                                             ValorEntrada = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_ENT),
                                             ValorSaida = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_SAI),
                                             SaldoQtde = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_QTDE),
                                             SaldoValor = grpAnaliticoBalanceteMensal.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_VALOR),
                                         }).AsQueryable();

            if (statusSituacao.HasValue)
                qryConsulta = qryConsulta.Where(rowFechamento => rowFechamento.SituacaoFechamento == (int)statusSituacao);

            var strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            return qryConsulta;
        }

        public IList<FechamentoMensalEntity> __xpImprimirInventarioMensalOLD(int almoxID, int anoMesRef)
        {
            IQueryable<FechamentoMensalEntity> resultado = (from rowFechamento in Db.TB_FECHAMENTOs
                                                            join subitemMaterial in Db.TB_SUBITEM_MATERIALs.DefaultIfEmpty() on rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID equals subitemMaterial.TB_SUBITEM_MATERIAL_ID
                                                            where rowFechamento.TB_ALMOXARIFADO_ID == almoxID
                                                            where rowFechamento.TB_FECHAMENTO_ANO_MES_REF == anoMesRef
                                                            group rowFechamento by new
                                                            {
                                                                subitemMaterial = rowFechamento.TB_SUBITEM_MATERIAL,
                                                                almox = rowFechamento.TB_ALMOXARIFADO,
                                                                uge = rowFechamento.TB_UGE,
                                                                sitFechamento = rowFechamento.TB_FECHAMENTO_SITUACAO
                                                            } into grpFechamentoSubitemMaterial
                                                            select new FechamentoMensalEntity
                                                            {
                                                                //Id = rowFechamento.TB_FECHAMENTO_ID,
                                                                Almoxarifado = new AlmoxarifadoEntity
                                                                {
                                                                    Id = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_DESCRICAO
                                                                },
                                                                AnoMesRef = anoMesRef,
                                                                SubItemMaterial = new SubItemMaterialEntity
                                                                {
                                                                    //CodigoFormatado = rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                    Id = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                    ItemMaterial = (from itemSubitemmaterial in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                    where grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID == itemSubitemmaterial.TB_SUBITEM_MATERIAL_ID
                                                                                    select new ItemMaterialEntity
                                                                                    {
                                                                                        Id = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                        Codigo = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                        //CodigoFormatado = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(8, '0'),
                                                                                        Descricao = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                    }).FirstOrDefault(),
                                                                },
                                                                UGE = new UGEEntity
                                                                {
                                                                    Id = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_DESCRICAO
                                                                },
                                                                QtdeEntrada = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_ENT),
                                                                QtdeSaida = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_SAI),
                                                                ValorEntrada = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_ENT),
                                                                ValorSaida = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_SAI),
                                                                SaldoQtde = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_QTDE),
                                                                SaldoValor = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_VALOR),
                                                            }).AsQueryable();//GroupBy(rowFechamentosubitemMaterial => new { ItemMaterialCodigo = rowFechamentosubitemMaterial. ItemMaterialCodigo });


            // procurar o saldo anterior
            string anoMesAnt = Common.Util.TratamentoDados.ValidarAnoMesRef(anoMesRef.ToString(), -1);

            var saldoAnt = (from rowFechamentoAnterior in Db.TB_FECHAMENTOs
                            where rowFechamentoAnterior.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                            where rowFechamentoAnterior.TB_FECHAMENTO_ANO_MES_REF == ((!String.IsNullOrWhiteSpace(anoMesAnt)) ? Int32.Parse(anoMesAnt) : 0)
                            where rowFechamentoAnterior.TB_ALMOXARIFADO_ID == almoxID
                            group rowFechamentoAnterior by new
                            {
                                rowFechamentoAnterior.TB_SUBITEM_MATERIAL_ID,
                                rowFechamentoAnterior.TB_ALMOXARIFADO_ID,
                                rowFechamentoAnterior.TB_UGE_ID,
                                rowFechamentoAnterior.TB_FECHAMENTO_SITUACAO
                            } into grpFechamentoAnterior
                            select new
                            {
                                SubitemMaterialId = grpFechamentoAnterior.Key.TB_SUBITEM_MATERIAL_ID,
                                AlmoxId = grpFechamentoAnterior.Key.TB_ALMOXARIFADO_ID,
                                UgeId = grpFechamentoAnterior.Key.TB_UGE_ID,
                                SaldoAnt = (grpFechamentoAnterior.Sum(sl => sl.TB_FECHAMENTO_SALDO_QTDE) ?? 0.00m).Truncar(2),
                                ValorAnt = (grpFechamentoAnterior.Sum(sl => sl.TB_FECHAMENTO_SALDO_VALOR) ?? 0.00m).Truncar(2),
                                situacao = grpFechamentoAnterior.Key.TB_FECHAMENTO_SITUACAO
                            }).ToList();

            IList<FechamentoMensalEntity> result = resultado.ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].SaldoAnterior = saldoAnt.Where(
                            rowFechamento => rowFechamento.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            rowFechamento.AlmoxId == result[i].Almoxarifado.Id &&
                            rowFechamento.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UgeId == result[i].UGE.Id).Sum(rowFechamento => rowFechamento.SaldoAnt.Truncar(2)).Truncar(2);

                    result[i].SaldoAnteriorValor = saldoAnt.Where(
                            rowFechamento => rowFechamento.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            rowFechamento.AlmoxId == result[i].Almoxarifado.Id &&
                            rowFechamento.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UgeId == result[i].UGE.Id).Sum(rowFechamento => rowFechamento.ValorAnt.Truncar(2)).Truncar(2);

                    if (!result[i].SaldoAnterior.HasValue) result[i].SaldoAnterior = 0;
                    if (!result[i].SaldoAnteriorValor.HasValue) result[i].SaldoAnteriorValor = 0;
                }
            }

            #region Truncar Valores Retorno
            var _somatoriaValorQtdeSubitens = result.Sum(_rowInventario => _rowInventario.SaldoQtde);
            var _somatoriaValorSubitensUGE = result.Sum(_rowInventario => _rowInventario.SaldoValor);
            result.Cast<FechamentoMensalEntity>().ToList().ForEach(_rowFechamento =>
            {
                _rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.ValorEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.ValorSaida.Value.Truncar(2) : 0.00m);

                _rowFechamento.SubItemMaterial.SomaSaldoTotal = _somatoriaValorQtdeSubitens;
                _rowFechamento.SubItemMaterial.SaldoAtual = _somatoriaValorSubitensUGE;
            });

            #endregion Truncar Valores Retorno

            return result;

        }

        public IList<FechamentoMensalEntity> _xpImprimirInventarioMensal(int almoxID, int anoMesRef)
        {
            IQueryable<FechamentoMensalEntity> resultado = (from rowFechamento in Db.TB_FECHAMENTOs
                                                            join subitemMaterial in Db.TB_SUBITEM_MATERIALs.DefaultIfEmpty() on rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID equals subitemMaterial.TB_SUBITEM_MATERIAL_ID
                                                            where rowFechamento.TB_ALMOXARIFADO_ID == almoxID
                                                            where rowFechamento.TB_FECHAMENTO_ANO_MES_REF == anoMesRef
                                                            group rowFechamento by new
                                                            {
                                                                subitemMaterial = rowFechamento.TB_SUBITEM_MATERIAL,
                                                                almox = rowFechamento.TB_ALMOXARIFADO,
                                                                uge = rowFechamento.TB_UGE,
                                                                sitFechamento = rowFechamento.TB_FECHAMENTO_SITUACAO
                                                            } into grpFechamentoSubitemMaterial
                                                            select new FechamentoMensalEntity
                                                            {
                                                                //Id = rowFechamento.TB_FECHAMENTO_ID,
                                                                Almoxarifado = new AlmoxarifadoEntity
                                                                {
                                                                    Id = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.almox.TB_ALMOXARIFADO_DESCRICAO
                                                                },
                                                                AnoMesRef = anoMesRef,
                                                                SubItemMaterial = new SubItemMaterialEntity
                                                                {
                                                                    //CodigoFormatado = rowFechamento.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                    Id = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                    ItemMaterial = (from itemSubitemmaterial in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                    where grpFechamentoSubitemMaterial.Key.subitemMaterial.TB_SUBITEM_MATERIAL_ID == itemSubitemmaterial.TB_SUBITEM_MATERIAL_ID
                                                                                    select new ItemMaterialEntity
                                                                                    {
                                                                                        Id = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                        Codigo = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                        //CodigoFormatado = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(8, '0'),
                                                                                        Descricao = itemSubitemmaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                    }).FirstOrDefault(),
                                                                },
                                                                UGE = new UGEEntity
                                                                {
                                                                    Id = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_ID,
                                                                    Codigo = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_CODIGO,
                                                                    Descricao = grpFechamentoSubitemMaterial.Key.uge.TB_UGE_DESCRICAO
                                                                },
                                                                QtdeEntrada = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_ENT),
                                                                QtdeSaida = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_QTDE_MOV_SAI),
                                                                ValorEntrada = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_ENT),
                                                                ValorSaida = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_VALOR_MOV_SAI),
                                                                SaldoQtde = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_QTDE),
                                                                SaldoValor = grpFechamentoSubitemMaterial.Sum(grpFechamento => grpFechamento.TB_FECHAMENTO_SALDO_VALOR),
                                                            }).AsQueryable();//GroupBy(rowFechamentosubitemMaterial => new { ItemMaterialCodigo = rowFechamentosubitemMaterial. ItemMaterialCodigo });


            // procurar o saldo anterior
            string anoMesAnt = Common.Util.TratamentoDados.ValidarAnoMesRef(anoMesRef.ToString(), -1);

            var saldoAnt = (from rowFechamentoAnterior in Db.TB_FECHAMENTOs
                            where rowFechamentoAnterior.TB_FECHAMENTO_SITUACAO == (int)GeralEnum.SituacaoFechamento.Executar
                            where rowFechamentoAnterior.TB_FECHAMENTO_ANO_MES_REF == ((!String.IsNullOrWhiteSpace(anoMesAnt)) ? Int32.Parse(anoMesAnt) : 0)
                            where rowFechamentoAnterior.TB_ALMOXARIFADO_ID == almoxID
                            group rowFechamentoAnterior by new
                            {
                                rowFechamentoAnterior.TB_SUBITEM_MATERIAL_ID,
                                rowFechamentoAnterior.TB_ALMOXARIFADO_ID,
                                rowFechamentoAnterior.TB_UGE_ID,
                                rowFechamentoAnterior.TB_FECHAMENTO_SITUACAO
                            } into grpFechamentoAnterior
                            select new
                            {
                                SubitemMaterialId = grpFechamentoAnterior.Key.TB_SUBITEM_MATERIAL_ID,
                                AlmoxId = grpFechamentoAnterior.Key.TB_ALMOXARIFADO_ID,
                                UgeId = grpFechamentoAnterior.Key.TB_UGE_ID,
                                SaldoAnt = (grpFechamentoAnterior.Sum(sl => sl.TB_FECHAMENTO_SALDO_QTDE) ?? 0.00m).Truncar(2),
                                ValorAnt = (grpFechamentoAnterior.Sum(sl => sl.TB_FECHAMENTO_SALDO_VALOR) ?? 0.00m).Truncar(2),
                                situacao = grpFechamentoAnterior.Key.TB_FECHAMENTO_SITUACAO
                            }).ToList();

            IList<FechamentoMensalEntity> result = resultado.ToList();

            if (result.Count > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    result[i].SaldoAnterior = saldoAnt.Where(
                            rowFechamento => rowFechamento.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            rowFechamento.AlmoxId == result[i].Almoxarifado.Id &&
                            rowFechamento.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UgeId == result[i].UGE.Id).Sum(rowFechamento => rowFechamento.SaldoAnt.Truncar(2)).Truncar(2);

                    result[i].SaldoAnteriorValor = saldoAnt.Where(
                            rowFechamento => rowFechamento.SubitemMaterialId == result[i].SubItemMaterial.Id &&
                            rowFechamento.AlmoxId == result[i].Almoxarifado.Id &&
                            rowFechamento.situacao == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UgeId == result[i].UGE.Id).Sum(rowFechamento => rowFechamento.ValorAnt.Truncar(2)).Truncar(2);

                    if (!result[i].SaldoAnterior.HasValue) result[i].SaldoAnterior = 0;
                    if (!result[i].SaldoAnteriorValor.HasValue) result[i].SaldoAnteriorValor = 0;
                }
            }

            #region Truncar Valores Retorno
            var _somatoriaValorQtdeSubitens = result.Sum(_rowInventario => _rowInventario.SaldoQtde);
            var _somatoriaValorSubitensUGE = result.Sum(_rowInventario => _rowInventario.SaldoValor);
            result.Cast<FechamentoMensalEntity>().ToList().ForEach(_rowFechamento =>
            {
                _rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.ValorEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.ValorSaida.Value.Truncar(2) : 0.00m);

                _rowFechamento.SubItemMaterial.SomaSaldoTotal = _somatoriaValorQtdeSubitens;
                _rowFechamento.SubItemMaterial.SaldoAtual = _somatoriaValorSubitensUGE;
            });

            #endregion Truncar Valores Retorno

            return result;

        }

        public IList<relInventarioFechamentoMensalEntity> _xpImprimirInventarioBalanceteMensal(int almoxID, int anoMesRef)
        {
            int statusSituacao = (int)GeralEnum.SituacaoFechamento.Executar;

            IQueryable<relInventarioFechamentoMensalEntity> qryConsultaMesCorrente = gerarConsultaRelatorioMensalFechamento<relInventarioFechamentoMensalEntity>(almoxID, anoMesRef, statusSituacao);
            IList<relInventarioFechamentoMensalEntity> inventarioBalanceteConsultado = qryConsultaMesCorrente.ToList();

            #region Truncar Valores Retorno
            var _somatoriaValorQtdeSubitens = inventarioBalanceteConsultado.Sum(_rowInventario => _rowInventario.SaldoQtde).Value;
            var _somatoriaValorSubitensUGE = inventarioBalanceteConsultado.Sum(_rowInventario => _rowInventario.SaldoValor).Value;
            inventarioBalanceteConsultado.Cast<relInventarioFechamentoMensalEntity>().ToList().ForEach(_rowFechamento =>
            {
                _rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.ValorEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.ValorSaida.Value.Truncar(2) : 0.00m);

                _rowFechamento.SomatoriaQtdeSubitemMaterial = _somatoriaValorQtdeSubitens;
                _rowFechamento.SomatoriaValorSubitemMaterial = _somatoriaValorSubitensUGE;
            });

            #endregion Truncar Valores Retorno
            return inventarioBalanceteConsultado;

        }

        public IList<relAnaliticoFechamentoMensalEntity> ImprimirAnaliticoBalanceteMensal(int almoxID, int anoMesRef)
        {
            int statusSituacao = (int)GeralEnum.SituacaoFechamento.Executar;

            //IQueryable<TB_FECHAMENTO> resultado = (from rowFechamento in Db.TB_FECHAMENTOs
            IQueryable<relAnaliticoFechamentoMensalEntity> qryConsultaMesCorrente = gerarConsultaRelatorioMensalFechamento<relAnaliticoFechamentoMensalEntity>(almoxID, anoMesRef, null);

            // procurar o saldo anterior
            int anoMesAnt = Int32.Parse(TratamentoDados.ValidarAnoMesRef(anoMesRef.ToString(), -1));
            IQueryable<relAnaliticoFechamentoMensalEntity> qryConsultaMesAnterior = gerarConsultaRelatorioMensalFechamento<relAnaliticoFechamentoMensalEntity>(almoxID, anoMesAnt, statusSituacao);

            IList<relAnaliticoFechamentoMensalEntity> balanceteMesCorrente = qryConsultaMesCorrente.ToList();
            IList<relAnaliticoFechamentoMensalEntity> balanceteMesAnterior = qryConsultaMesAnterior.ToList();

            if (balanceteMesCorrente.Count > 0)
            {
                for (int i = 0; i < balanceteMesCorrente.Count; i++)
                {
                    balanceteMesCorrente[i].SaldoAnterior = balanceteMesAnterior.Where(
                            rowFechamento => rowFechamento.SubItemMaterial.Id == balanceteMesCorrente[i].SubItemMaterial.Id &&
                            rowFechamento.Almoxarifado.Id == balanceteMesCorrente[i].Almoxarifado.Id &&
                            rowFechamento.SituacaoFechamento == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UGE.Id == balanceteMesCorrente[i].UGE.Id).Sum(rowFechamento => rowFechamento.SaldoQtde.Value.Truncar(2)).Truncar(2);

                    balanceteMesCorrente[i].SaldoAnteriorValor = balanceteMesAnterior.Where(
                            rowFechamento => rowFechamento.SubItemMaterial.Id == balanceteMesCorrente[i].SubItemMaterial.Id &&
                            rowFechamento.Almoxarifado.Id == balanceteMesCorrente[i].Almoxarifado.Id &&
                            rowFechamento.SituacaoFechamento == (int)GeralEnum.SituacaoFechamento.Executar &&
                            rowFechamento.UGE.Id == balanceteMesCorrente[i].UGE.Id).Sum(rowFechamento => rowFechamento.SaldoValor.Value.Truncar(2)).Truncar(2);

                    if (!balanceteMesCorrente[i].SaldoAnterior.HasValue) balanceteMesCorrente[i].SaldoAnterior = 0;
                    if (!balanceteMesCorrente[i].SaldoAnteriorValor.HasValue) balanceteMesCorrente[i].SaldoAnteriorValor = 0;
                }
            }

            #region Truncar Valores Retorno
            balanceteMesCorrente.Cast<relAnaliticoFechamentoMensalEntity>().ToList().ForEach(_rowFechamento =>
            {
                _rowFechamento.SaldoValor = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoValor.Value.Truncar(2) : 0.00m);
                _rowFechamento.SaldoQtde = ((_rowFechamento.SaldoValor.HasValue) ? _rowFechamento.SaldoQtde.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.ValorEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.ValorSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.ValorSaida.Value.Truncar(2) : 0.00m);
                _rowFechamento.QtdeEntrada = ((_rowFechamento.ValorEntrada.HasValue) ? _rowFechamento.QtdeEntrada.Value.Truncar(2) : 0.00m);
                _rowFechamento.QtdeSaida = ((_rowFechamento.ValorSaida.HasValue) ? _rowFechamento.QtdeSaida.Value.Truncar(2) : 0.00m);
            });

            balanceteMesCorrente.ToList().ForEach(rowFechamento =>
                {
                    rowFechamento.SomatoriaQtdeAnteriorNaturezaDespesa = balanceteMesAnterior.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamentoAnterior => new { _grpFechamentoAnterior.SubItemMaterial.NaturezaDespesa.Id, _grpFechamentoAnterior.SaldoQtde }).Sum(__rowFechamentoAnterior => __rowFechamentoAnterior.Key.SaldoQtde.Value).Truncar(2);

                    rowFechamento.SomatoriaSaldoAnteriorNaturezaDespesa = balanceteMesAnterior.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamentoAnterior => new { _grpFechamentoAnterior.SubItemMaterial.NaturezaDespesa.Id, _grpFechamentoAnterior.SaldoValor }).Sum(__rowFechamentoAnterior => __rowFechamentoAnterior.Key.SaldoValor.Value).Truncar(2);

                    rowFechamento.SomatoriaSaldoEntradaNaturezaDespesa = balanceteMesCorrente.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamento => new { _grpFechamento.SubItemMaterial.NaturezaDespesa.Codigo, _grpFechamento.ValorEntrada }).ToList().Sum(__rowFechamento => __rowFechamento.Key.ValorEntrada.Value).Truncar(2);
                    rowFechamento.SomatoriaQtdeEntradaNaturezaDespesa = balanceteMesCorrente.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamento => new { _grpFechamento.SubItemMaterial.NaturezaDespesa.Codigo, _grpFechamento.QtdeEntrada }).ToList().Sum(__rowFechamento => __rowFechamento.Key.QtdeEntrada.Value).Truncar(2);

                    rowFechamento.SomatoriaSaldoSaidaNaturezaDespesa = balanceteMesCorrente.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamento => new { _grpFechamento.SubItemMaterial.NaturezaDespesa.Codigo, _grpFechamento.ValorSaida }).ToList().Sum(__rowFechamento => __rowFechamento.Key.ValorSaida.Value).Truncar(2);
                    rowFechamento.SomatoriaQtdeSaidaNaturezaDespesa = balanceteMesCorrente.Where(rowFechamentoAnterior => rowFechamentoAnterior.SubItemMaterial.Id == rowFechamento.SubItemMaterial.Id
                                                                                                                          && rowFechamentoAnterior.Almoxarifado.Id == rowFechamento.Almoxarifado.Id
                                                                                                                          && rowFechamentoAnterior.UGE.Id == rowFechamento.UGE.Id
                                                                                                                          && rowFechamentoAnterior.SituacaoFechamento == rowFechamento.SituacaoFechamento)
                                                                                             .GroupBy(_grpFechamento => new { _grpFechamento.SubItemMaterial.NaturezaDespesa.Codigo, _grpFechamento.QtdeSaida }).ToList().Sum(__rowFechamento => __rowFechamento.Key.QtdeSaida.Value).Truncar(2);

                    var qtdeFinalNatDespesa = rowFechamento.SomatoriaQtdeAnteriorNaturezaDespesa + rowFechamento.SomatoriaQtdeEntradaNaturezaDespesa - rowFechamento.SomatoriaQtdeSaidaNaturezaDespesa;
                    var saldoFinalNatDespesa = rowFechamento.SomatoriaSaldoAnteriorNaturezaDespesa + rowFechamento.SomatoriaSaldoEntradaNaturezaDespesa - rowFechamento.SomatoriaSaldoSaidaNaturezaDespesa;

                    rowFechamento.QtdeFinalNaturezaDespesa = qtdeFinalNatDespesa.Truncar(2);
                    rowFechamento.SaldoFinalNaturezaDespesa = saldoFinalNatDespesa.Truncar(2);
                });

            balanceteMesCorrente.ToList().ForEach(rowFechamento =>
            {
                rowFechamento.QtdeFinalMes = balanceteMesCorrente.Sum(__rowFechamento => __rowFechamento.QtdeFinalNaturezaDespesa);
                rowFechamento.SaldoFinalMes = balanceteMesCorrente.Sum(__rowFechamento => __rowFechamento.SaldoFinalNaturezaDespesa);
            });
            #endregion Truncar Valores Retorno

            return balanceteMesCorrente;

        }

        /// <summary>
        /// Atualiza o mês ref ao realizar o fechamento
        /// </summary>
        /// <param name="almoxarifadoId"></param>
        /// <param name="AnoMesRef"></param>
        public void AtualizarMesRefAlmoxarifadoFechamento(int almoxarifadoId, string AnoMesRef)
        {
            TB_ALMOXARIFADO almox = (from a in Db.TB_ALMOXARIFADOs
                                     where a.TB_ALMOXARIFADO_ID == almoxarifadoId
                                     select a).FirstOrDefault();
            if (almox != null)
            {
                almox.TB_ALMOXARIFADO_MES_REF = AnoMesRef;
                Db.SubmitChanges();
            }
            else
                throw new Exception("Não foi possível realizar o fechamento.");
        }


        public void Excluir()
        {
            var item = this.Db.TB_FECHAMENTOs
                .Where(a => a.TB_FECHAMENTO_ANO_MES_REF == this.Entity.AnoMesRef)
                .Where(a => a.TB_ALMOXARIFADO_ID == this.Entity.Almoxarifado.Id)
                .ToList();
            Db.TB_FECHAMENTOs.DeleteAllOnSubmit(item);
            Db.SubmitChanges();
        }


        /// <summary>
        /// Sobrecarga para amarrar todos os passos do Fechamento via Almoxarifado/DataReferencia.
        /// </summary>
        /// <param name="pIntAnoMesReferencia"></param>
        /// <param name="pIntAlmoxarifadoID"></param>
        internal void Excluir(int pIntAnoMesReferencia, int pIntAlmoxarifadoID, int? situacaoFechamento)
        {
            string lStrSqlTracing = string.Empty;
            List<TB_FECHAMENTO> lLstFechamentos = null;


            lLstFechamentos = this.Db.TB_FECHAMENTOs.Where(Fechamento => Fechamento.TB_FECHAMENTO_ANO_MES_REF == pIntAnoMesReferencia)
                                                    .Where(Fechamento => Fechamento.TB_ALMOXARIFADO_ID == pIntAlmoxarifadoID)
                                                    .Where(Fechamento => Fechamento.TB_FECHAMENTO_SITUACAO == situacaoFechamento)
                                                    .ToList();

            Db.TB_FECHAMENTOs.DeleteAllOnSubmit(lLstFechamentos);
            Db.SubmitChanges();
        }
        public void ExecutarFechamento(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {
            //try
            //{
            //    Excluir(mesAnoReferencia, AlmoxarifadoId, 1);

            //    IQueryable<VW_RETORNAR_PARAMETRO_FECHAMENTO> query = (from fe in this.Db.VW_RETORNAR_PARAMETRO_FECHAMENTOs
            //                                                          orderby fe.TB_ALMOXARIFADO_ID ascending, fe.TB_MOVIMENTO_ANO_MES_REFERENCIA ascending, fe.TB_UGE_ID ascending, fe.TB_SUBITEM_MATERIAL_ID ascending
            //                                                          where fe.TB_ALMOXARIFADO_ID == AlmoxarifadoId
            //                                                            && fe.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesAnoReferencia
            //                                                          select fe).AsQueryable();

            //    IList<VW_RETORNAR_PARAMETRO_FECHAMENTO> listaFechamento = query.ToList();

            //    foreach (VW_RETORNAR_PARAMETRO_FECHAMENTO item in listaFechamento)
            //    {

            //        Int32 retorno = this.Db.SAM_CORRIGIR_FECHAMENTO(AlmoxarifadoId, item.TB_UGE_ID, mesAnoReferencia, mesAnoReferencia, item.TB_SUBITEM_MATERIAL_ID,1);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message, ex.InnerException);
            //}


            Executar(AlmoxarifadoId, mesAnoReferencia, 1, usuarioSamLoginId);

        }

        private void Executar(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, byte situacao, int usuarioSamLoginId)
        {
            this.fechamentoErro = new List<String>();
            DateTime dataExecucaoFechamento = DateTime.Now;

            SqlConnection cnn = new SqlConnection(this.Db.Connection.ConnectionString);
            cnn.Open();
            SqlCommand cmd;
            SqlTransaction transaction;
            if (situacao == 0)
                transaction = cnn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted, "SIMULACAOFECHAMENTO");
            else
                //transaction = cnn.BeginTransaction(System.Data.IsolationLevel.RepeatableRead, "FECHAMENTOMENSAL");
                transaction = cnn.BeginTransaction(System.Data.IsolationLevel.Serializable, "FECHAMENTOMENSAL");

            try
            {
                Excluir(mesAnoReferencia, AlmoxarifadoId, situacao);

                IQueryable<VW_RETORNAR_PARAMETRO_FECHAMENTO> query = (from fe in this.Db.VW_RETORNAR_PARAMETRO_FECHAMENTOs
                                                                      orderby fe.TB_ALMOXARIFADO_ID ascending, fe.TB_MOVIMENTO_ANO_MES_REFERENCIA ascending, fe.TB_UGE_ID ascending, fe.TB_SUBITEM_MATERIAL_ID ascending
                                                                      where fe.TB_ALMOXARIFADO_ID == AlmoxarifadoId
                                                                        && fe.TB_MOVIMENTO_ANO_MES_REFERENCIA == mesAnoReferencia
                                                                      select fe).AsQueryable();

                IList<VW_RETORNAR_PARAMETRO_FECHAMENTO> listaFechamento = query.ToList();

                cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandText = "[dbo].[SAM_CORRIGIR_FECHAMENTO]";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Transaction = transaction;



                foreach (VW_RETORNAR_PARAMETRO_FECHAMENTO item in listaFechamento)
                {
                    cmd.Parameters.AddWithValue("@TB_ALMOXARIFADO_ID", AlmoxarifadoId);
                    cmd.Parameters.AddWithValue("@TB_UGE_ID", item.TB_UGE_ID);
                    cmd.Parameters.AddWithValue("@TB_FECHAMENTO_ANO_MES_REF_MAX", mesAnoReferencia);
                    cmd.Parameters.AddWithValue("@TB_FECHAMENTO_ANO_MES_REF_MIN", mesAnoReferencia);
                    cmd.Parameters.AddWithValue("@TB_SUBITEM_MATERIAL_ID", item.TB_SUBITEM_MATERIAL_ID);
                    cmd.Parameters.AddWithValue("@TB_FECHAMENTO_SITUACAO", situacao);

                    cmd.Parameters.AddWithValue("@TB_LOGIN_ID", usuarioSamLoginId);
                    cmd.Parameters.AddWithValue("@TB_FECHAMENTO_DATA_PROCESSAMENTO", dataExecucaoFechamento);

                    cmd.Parameters.AddWithValue("@RETORNO", 1).Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@SALDOQTD", 0).Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@SALDOVALOR", 0).Direction = System.Data.ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    Int32 retornoParametro = Convert.ToInt32(cmd.Parameters["@RETORNO"].Value);

                    //retornoParametro igual a "0" foi gerado pela procedure "SAM_CORRIGIR_FECHAMENTO" um erro durante o processo de fechamento   
                    if (retornoParametro == 0)
                    {
                        SubItemMaterialInfraestructure subItem = new SubItemMaterialInfraestructure();
                        SubItemMaterialEntity subitem = subItem.Select(item.TB_SUBITEM_MATERIAL_ID);

                        this.fechamentoErro.Add("Saldo negativo, por favor rever os movimentos para o SubItem : " + subitem.Codigo + "  Saldo Quantidade : " + Convert.ToDecimal(cmd.Parameters["@SALDOQTD"].Value) + "  Saldo Valor : " + Convert.ToDecimal(cmd.Parameters["@SALDOVALOR"].Value).ToString("#.###,##"));

                        // throw new Exception("Saldo negativo, por favor rever os movimentos para o SubItem : " + subitem.Codigo + "  Saldo quantidade : " + Convert.ToDecimal(cmd.Parameters["@SALDOQTD"].Value) + "  Saldo Valor : " + Convert.ToDecimal(cmd.Parameters["@SALDOVALOR"].Value).ToString("#.###,##"));
                    }

                    cmd.Parameters.Clear();
                }

                if (this.fechamentoErro.Count > 0)
                    transaction.Rollback();
                else
                    transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                this.fechamentoErro.Add(ex.Message);
            }
            finally
            {
                transaction.Dispose();
                transaction = null;
                cmd = null;
                cnn.Close();
            }
        }
        public void ExecutarSimulacao(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {

            Executar(AlmoxarifadoId, mesAnoReferencia, 0, usuarioSamLoginId);


        }

        private IList<FechamentoMensalEntity> getListaFechamentoSimulacao(IList<TB_FECHAMENTO> fechamento)
        {
            List<FechamentoMensalEntity> listaFecha = new List<FechamentoMensalEntity>();

            try
            {
                foreach (TB_FECHAMENTO fecha in fechamento)
                {
                    FechamentoMensalEntity fechamentoMes = new FechamentoMensalEntity();

                    fechamentoMes.Almoxarifado = new AlmoxarifadoEntity(fecha.TB_ALMOXARIFADO_ID);
                    fechamentoMes.SubItemMaterial = new SubItemMaterialEntity(fecha.TB_SUBITEM_MATERIAL_ID);
                    fechamentoMes.UGE = new UGEEntity(fecha.TB_UGE_ID);
                    fechamentoMes.AnoMesRef = fecha.TB_FECHAMENTO_ANO_MES_REF;
                    fechamentoMes.QtdeEntrada = fecha.TB_FECHAMENTO_QTDE_MOV_ENT;
                    fechamentoMes.QtdeSaida = fecha.TB_FECHAMENTO_QTDE_MOV_SAI;
                    fechamentoMes.ValorEntrada = fecha.TB_FECHAMENTO_VALOR_MOV_ENT;
                    fechamentoMes.ValorSaida = fecha.TB_FECHAMENTO_VALOR_MOV_SAI;
                    fechamentoMes.SaldoQtde = fecha.TB_FECHAMENTO_SALDO_QTDE;
                    fechamentoMes.SaldoValor = fecha.TB_FECHAMENTO_SALDO_VALOR;

                    listaFecha.Add(fechamentoMes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return listaFecha;
        }
        public void Salvar(IList<FechamentoMensalEntity> ListaFechamento)
        {
            this.Db.CommandTimeout = 180; //3 minutos

            // excluir primeiro o registro (para não duplicar)
            //this.EntityList = ListaFechamento;

            int lIntAnoMesReferencia = this.Entity.AnoMesRef.Value;
            int lIntAlmoxarifadoID = this.Entity.Almoxarifado.Id.Value;

            if (ListaFechamento.Count == 0)
                throw new Exception("Erro no processamento: O fechamento está nulo");

            //Excluir();
            Excluir(lIntAnoMesReferencia, lIntAlmoxarifadoID, ListaFechamento[0].SituacaoFechamento);

            EntitySet<TB_FECHAMENTO> item = new EntitySet<TB_FECHAMENTO>();

            //foreach(FechamentoMensalEntity fechaItem in this.ListaFechamento)
            foreach (FechamentoMensalEntity fechaItem in ListaFechamento)
            {
                TB_FECHAMENTO fecha = new TB_FECHAMENTO();

                if (fechaItem.Almoxarifado != null)
                {
                    fecha.TB_ALMOXARIFADO_ID = fechaItem.Almoxarifado.Id.Value;
                }
                fecha.TB_FECHAMENTO_ANO_MES_REF = fechaItem.AnoMesRef.Value;
                fecha.TB_FECHAMENTO_QTDE_MOV_ENT = fechaItem.QtdeEntrada;
                fecha.TB_FECHAMENTO_VALOR_MOV_ENT = fechaItem.ValorEntrada;
                fecha.TB_FECHAMENTO_QTDE_MOV_SAI = fechaItem.QtdeSaida;
                fecha.TB_FECHAMENTO_VALOR_MOV_SAI = fechaItem.ValorSaida;
                fecha.TB_FECHAMENTO_SALDO_QTDE = fechaItem.SaldoQtde;
                fecha.TB_FECHAMENTO_SALDO_VALOR = fechaItem.SaldoValor;
                fecha.TB_FECHAMENTO_SITUACAO = fechaItem.SituacaoFechamento;

                if (fechaItem.SubItemMaterial != null)
                {
                    fecha.TB_SUBITEM_MATERIAL_ID = fechaItem.SubItemMaterial.Id.Value;
                }
                if (fechaItem.UGE != null)
                {
                    fecha.TB_UGE_ID = fechaItem.UGE.Id.Value;
                }
                item.Add(fecha);
            }
            this.Db.TB_FECHAMENTOs.InsertAllOnSubmit(item);
            this.Db.SubmitChanges();
        }

        public void Salvar(IList<FechamentoMensalEntity> pIListaFechamentos, int pIntAlmoxarifadoID, int pIntAnoMesReferencia)
        {
            this.Db.CommandTimeout = 180; //3 minutos

            TB_FECHAMENTO lEntFechamentoMensal = null;
            EntitySet<TB_FECHAMENTO> lSetFechamentos = null;

            if (pIListaFechamentos.Count == 0)
                throw new Exception("Erro no processamento: O fechamento está nulo");

            //eliminação de duplicatas
            Excluir(pIntAnoMesReferencia, pIntAlmoxarifadoID, pIListaFechamentos[0].SituacaoFechamento);

            lSetFechamentos = new EntitySet<TB_FECHAMENTO>();

            foreach (FechamentoMensalEntity FechamentoMensalItem in pIListaFechamentos)
            {
                lEntFechamentoMensal = new TB_FECHAMENTO();

                if (FechamentoMensalItem.Almoxarifado != null && FechamentoMensalItem.Almoxarifado.Id.HasValue)
                    lEntFechamentoMensal.TB_ALMOXARIFADO_ID = FechamentoMensalItem.Almoxarifado.Id.Value;

                lEntFechamentoMensal.TB_FECHAMENTO_ANO_MES_REF = FechamentoMensalItem.AnoMesRef.Value;
                lEntFechamentoMensal.TB_FECHAMENTO_QTDE_MOV_ENT = FechamentoMensalItem.QtdeEntrada;
                lEntFechamentoMensal.TB_FECHAMENTO_VALOR_MOV_ENT = FechamentoMensalItem.ValorEntrada;
                lEntFechamentoMensal.TB_FECHAMENTO_QTDE_MOV_SAI = FechamentoMensalItem.QtdeSaida;
                lEntFechamentoMensal.TB_FECHAMENTO_VALOR_MOV_SAI = FechamentoMensalItem.ValorSaida;
                lEntFechamentoMensal.TB_FECHAMENTO_SALDO_QTDE = FechamentoMensalItem.SaldoQtde;
                lEntFechamentoMensal.TB_FECHAMENTO_SALDO_VALOR = FechamentoMensalItem.SaldoValor;

                if (FechamentoMensalItem.SubItemMaterial != null && FechamentoMensalItem.SubItemMaterial.Id.HasValue)
                    lEntFechamentoMensal.TB_SUBITEM_MATERIAL_ID = FechamentoMensalItem.SubItemMaterial.Id.Value;

                if (FechamentoMensalItem.UGE != null && FechamentoMensalItem.UGE.Id.HasValue)
                    lEntFechamentoMensal.TB_UGE_ID = FechamentoMensalItem.UGE.Id.Value;

                lSetFechamentos.Add(lEntFechamentoMensal);
                lEntFechamentoMensal = null;
            }

            this.Db.TB_FECHAMENTOs.InsertAllOnSubmit(lSetFechamentos);
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Listagem dos subitens inativos no catálogo do almoxarifado consultado, que possuam saldo
        /// </summary>
        /// <param name="almoxId"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> VerificarSubitensInativos(int almoxId)
        {
            bool statusInativo = false;

            string strSQL = null;
            IList<SubItemMaterialEntity> lstRetorno = new List<SubItemMaterialEntity>();
            IQueryable<TB_SUBITEM_MATERIAL> qryConsulta = null;

            qryConsulta = (from saldoSubitem in Db.TB_SALDO_SUBITEMs
                           join subitemAlmox in Db.TB_SUBITEM_MATERIAL_ALMOXes on saldoSubitem.TB_SUBITEM_MATERIAL_ID equals subitemAlmox.TB_SUBITEM_MATERIAL_ID
                           join subitemMaterial in Db.TB_SUBITEM_MATERIALs on subitemAlmox.TB_SUBITEM_MATERIAL_ID equals subitemMaterial.TB_SUBITEM_MATERIAL_ID
                           where subitemAlmox.TB_ALMOXARIFADO_ID == saldoSubitem.TB_ALMOXARIFADO_ID
                           where subitemAlmox.TB_ALMOXARIFADO_ID == almoxId
                           where subitemAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == statusInativo
                           select subitemMaterial).AsQueryable<TB_SUBITEM_MATERIAL>();

            strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            qryConsulta.ToList().ForEach(rowTabela => lstRetorno.Add(new SubItemMaterialEntity
            {
                Id = rowTabela.TB_SUBITEM_MATERIAL_ID,
                Codigo = rowTabela.TB_SUBITEM_MATERIAL_CODIGO,
                Descricao = rowTabela.TB_SUBITEM_MATERIAL_DESCRICAO,
                IndicadorAtividade = rowTabela.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                IndicadorAtividadeAlmox = false,
                UnidadeFornecimento = new UnidadeFornecimentoEntity(rowTabela.TB_UNIDADE_FORNECIMENTO_ID) { Codigo = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, Descricao = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO },
                AlmoxarifadoId = almoxId
            }));

            this.totalregistros = qryConsulta.Count();

            return lstRetorno;

        }

        public List<FechamentoAnualEntity> GerarBalanceteAnual(int idAlmoxarifado, string mesrefAnoAnterior, string mesRefInicial, string mesRefFinal)
        {
            DataSet dsRetorno = new DataSet();
            List<FechamentoAnualEntity> listaRetorno = new List<FechamentoAnualEntity>();
            try
            {
                SqlConnection conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["Connection"].ConnectionString);
                SqlCommand cmd = new SqlCommand("SAM_BALANCETE_ANUAL ", conexao);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID_ALMOXARIFADO", idAlmoxarifado);
                cmd.Parameters.AddWithValue("@MES_REF_ANO_ANTERIOR", mesrefAnoAnterior);
                cmd.Parameters.AddWithValue("@MES_REF_INICIAL", mesRefInicial);
                cmd.Parameters.AddWithValue("@MES_REF_FINAL", mesRefFinal);

                SqlDataAdapter daRetorno = new SqlDataAdapter(cmd);
                daRetorno.Fill(dsRetorno);

                foreach (DataRow linha in dsRetorno.Tables[0].Rows)
                {
                    FechamentoAnualEntity fechamento = new FechamentoAnualEntity();
                    //Alteração para Decimal resolveu o problema de formatação do relatório
                    fechamento.NATUREZA_DESPESA_CODIGO = linha["NATUREZA_DESPESA_CODIGO"].ToString();
                    fechamento.SALDO_ANO_ANTERIOR = Convert.ToDecimal(linha["SALDO_ANO_ANTERIOR"].ToString());
                    fechamento.ENTRADA = Convert.ToDecimal(linha["ENTRADA"].ToString());
                    fechamento.SAIDA = Convert.ToDecimal(linha["SAIDA"].ToString());
                    fechamento.RESUMO_ANO_ATUAL = Convert.ToDecimal(linha["RESUMO_ANO_ATUAL"].ToString());
                    fechamento.TB_ALMOXARIFADO_ID = linha["TB_ALMOXARIFADO_ID"].ToString();
                    fechamento.TB_ALMOXARIFADO_CODIGO = linha["TB_ALMOXARIFADO_CODIGO"].ToString();
                    fechamento.TB_ALMOXARIFADO_DESCRICAO = linha["TB_ALMOXARIFADO_DESCRICAO"].ToString();
                    fechamento.TB_ALMOXARIFADO_CEP = linha["TB_ALMOXARIFADO_CEP"].ToString();
                    fechamento.TB_ALMOXARIFADO_BAIRRO = linha["TB_ALMOXARIFADO_BAIRRO"].ToString();
                    fechamento.TB_ALMOXARIFADO_COMPLEMENTO = linha["TB_ALMOXARIFADO_COMPLEMENTO"].ToString();
                    fechamento.TB_ALMOXARIFADO_LOGRADOURO = linha["TB_ALMOXARIFADO_LOGRADOURO"].ToString();
                    fechamento.TB_ALMOXARIFADO_MUNICIPIO = linha["TB_ALMOXARIFADO_MUNICIPIO"].ToString();
                    fechamento.TB_ALMOXARIFADO_NUMERO = linha["TB_ALMOXARIFADO_NUMERO"].ToString();

                    listaRetorno.Add(fechamento);
                    fechamento = null;
                }

                return listaRetorno;
            }
            catch (Exception)
            {

                throw;
            }


        }

        //Lista status almoxarifado
        public AlmoxarifadoEntity ListarStatusAlmoxarifado(int? almoxId)
        {
            AlmoxarifadoEntity listaFecha = null;

            listaFecha = (from almo in this.Db.TB_ALMOXARIFADOs
                             where almo.TB_ALMOXARIFADO_ID == almoxId
                             select new AlmoxarifadoEntity
                             {
                                 Id=almo.TB_ALMOXARIFADO_ID,
                                 IndicAtividade= Convert.ToBoolean(almo.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE)                                 
                             }).FirstOrDefault();
            return listaFecha;
        } 
    }
}
