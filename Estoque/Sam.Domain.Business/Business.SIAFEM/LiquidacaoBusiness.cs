using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sam.Common;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Domain.Business.SIAFEM;
using DTOs = Sam.Domain.Entity;
using System.Transactions;
using System.Globalization;
using TipoLicitacaoEmpenho = Sam.Common.Util.GeralEnum.EmpenhoLicitacao;
using TipoEventoEmpenho = Sam.Common.Util.GeralEnum.EmpenhoEvento;
using TipoMaterialParaLiquidacao = Sam.Common.Util.GeralEnum.TipoMaterial;
using TipoMovimentacao = Sam.Common.Util.GeralEnum.TipoMovimento;
using TipoNotaSIAFEM = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using TipoLancamentoSIAFEM = Sam.Common.Util.GeralEnum.TipoLancamentoSIAF;



namespace Sam.Domain.Business.SIAFEM
{
    public class LiquidacaoBusiness : BaseBusiness
    {
        private List<MovimentoItemEntity> AgrupaMovimentoItem(IList<MovimentoEntity> lstMovimentacoesEmpenho)
        {
            List<MovimentoItemEntity> auxLstItensMovimento = new List<MovimentoItemEntity>();
            List<MovimentoItemEntity> lstItensMovimento = new List<MovimentoItemEntity>();

            if (!lstMovimentacoesEmpenho.IsNullOrEmpty())
            {
                foreach (MovimentoEntity movimento in lstMovimentacoesEmpenho)
                    auxLstItensMovimento.AddRange(movimento.MovimentoItem);


                lstItensMovimento = auxLstItensMovimento.GroupBy(_itemMov => _itemMov.ItemMaterial.Codigo)
                                                        .Select(_itemMovSaida => new MovimentoItemEntity()
                                                        {
                                                            ItemMaterial = new ItemMaterialEntity() { Codigo = _itemMovSaida.Key },
                                                            QtdeMov = auxLstItensMovimento.Where(_itemMov => _itemMov.ItemMaterial.Codigo == _itemMovSaida.Key).Sum(_movEmpenho => _movEmpenho.QtdeMov),
                                                            SubItemMaterial = ((auxLstItensMovimento.Where(_itemMov => _itemMov.ItemMaterial.Codigo == _itemMovSaida.Key).FirstOrDefault().IsNotNull()) ? (auxLstItensMovimento.Where(_itemMov => _itemMov.ItemMaterial.Codigo == _itemMovSaida.Key).FirstOrDefault().SubItemMaterial) : null),
                                                        })
                                                        .DistinctBy(_itemMov => _itemMov.ItemMaterial.Codigo)
                                                        .ToList();
            }

            return lstItensMovimento;
        }

        public IList<MovimentoEntity> GerarMovimentacoesParaLiquidacao(IList<MovimentoEntity> lstMovimentacoesEmpenho)
        {
            IList<MovimentoEntity> lstMovimentacoes = null;

            if (lstMovimentacoesEmpenho.IsNotNullAndNotEmpty())
                lstMovimentacoes = lstMovimentacoesEmpenho.ParticionadorPorNumeroDocumento();

            //IMPORTANTE
            //Ajuste para valor a liquidar
            lstMovimentacoes.ToList().ForEach(movParaLiquidar => movParaLiquidar.MovimentoItem.ToList().ForEach(movItemParaLiquidar => movItemParaLiquidar.ValorMov = movItemParaLiquidar.PrecoUnit * movItemParaLiquidar.QtdeLiq));

            return lstMovimentacoes;
        }

        public IList<string> ObterNLsPagamentoEmpenho(int almoxID, string anoMesRef, string codigoEmpenho, bool detalharNLs = false)
        {
            IList<string> listaNLsEmpenho = null;
            var srvInfra = this.Service<Sam.ServiceInfraestructure.IMovimentoService>();

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    listaNLsEmpenho = srvInfra.ObterNLsPagamentoEmpenho(almoxID, anoMesRef, codigoEmpenho, detalharNLs);
                }
                catch (Exception excErroGravacao)
                {
                    throw excErroGravacao;
                }
            }

            return listaNLsEmpenho;
        }
        public IList<string> ObterNLsPagamentoMovimento(int movimentacaoEmpenhoID)
        {
            IList<string> listaNLsEmpenho = null;
            var srvInfra = this.Service<Sam.ServiceInfraestructure.IMovimentoService>();

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    listaNLsEmpenho = srvInfra.ObterNLsPagamentoMovimentoEmpenho(movimentacaoEmpenhoID);
                }
                catch (Exception excErroGravacao)
                {
                    throw excErroGravacao;
                }
            }

            return listaNLsEmpenho;
        }

        private string ObterDataPagamentoEmpenho(DateTime? dataMovimento)
        {
            if (!dataMovimento.HasValue)
                throw new ArgumentException("DataMovimento inválida informada!");

            DateTime _dataMovimento = dataMovimento.Value;

            int mesRef = -1;
            int anoRef = -1;
            int diaLiquidacao = -1;
            string strDataEmissao = null;


            mesRef = _dataMovimento.Month;
            anoRef = _dataMovimento.Year;
            diaLiquidacao = _dataMovimento.Day;

            strDataEmissao = String.Format("{0:00}{1}{2:0000}", diaLiquidacao, MesExtenso.Mes[mesRef].ToUpperInvariant(), anoRef);

            return strDataEmissao;
        }
    }

    #region Tipo Empenho
    public static partial class ExtensionMethodsSIAF
    {
        public static string RetornarDescricaoEmpenho(this MovimentoEntity objMovimento)
        {
            string strDescricaoFormatada;

            object[] argsFormatacao = null;
            string tipoEmpenho = objMovimento.RetornarTipoEmpenho();
            string tipoLicitacaoEmpenho = objMovimento.RetornarTipoLicitacaoEmpenho();
            string fmtDescricao = null;

            if (!String.IsNullOrWhiteSpace(tipoEmpenho) && !String.IsNullOrWhiteSpace(tipoLicitacaoEmpenho))
                if (tipoEmpenho == tipoLicitacaoEmpenho)
                {
                    fmtDescricao = "{0}";
                    argsFormatacao = new object[] { tipoEmpenho };
                }
                else
                {
                    fmtDescricao = "{0} ({1})";
                    argsFormatacao = new object[] { tipoEmpenho, tipoLicitacaoEmpenho };
                }

            strDescricaoFormatada = String.Format(fmtDescricao, argsFormatacao);

            return strDescricaoFormatada;
        }

        public static bool IsTipoBEC(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_BEC);
        }
        public static bool IsTipoPregao(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_PREGAO_ELETRONICO);
        }
        public static bool IsTipoSiafisico(this MovimentoEntity objMovimento)
        {
            return (objMovimento.RetornarTipoEmpenho() == Constante.CST_DESCRICAO_EMPENHO_TIPO_SIAFISICO);
        }

        private static string RetornarTipoEmpenho(this MovimentoEntity objMovimento)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");

            #region Variaveis
            int codigoEventoEmpenho = 0;
            int tipoLicitacaoEmpenho = 0;
            string strRetorno = string.Empty;
            bool eventoEmpenhoNulo = false;
            bool licitacaoEmpenhoNulo = false;
            #endregion Variaveis

            eventoEmpenhoNulo = (objMovimento.EmpenhoEvento.IsNull() || objMovimento.EmpenhoEvento.Codigo.IsNull());
            licitacaoEmpenhoNulo = (objMovimento.EmpenhoLicitacao.IsNull() || objMovimento.EmpenhoLicitacao.Id.IsNull());

            if ((!eventoEmpenhoNulo && licitacaoEmpenhoNulo) || (eventoEmpenhoNulo && !licitacaoEmpenhoNulo))
            {
                strRetorno = "NÃO FOI POSSÍVEL DEFINIR TIPO DE EMPENHO/LICITACAO.";
            }
            else
            {
                tipoLicitacaoEmpenho = (int)objMovimento.EmpenhoLicitacao.Id;
                codigoEventoEmpenho = (int)objMovimento.EmpenhoEvento.Codigo;
            }

            if (tipoLicitacaoEmpenho == (int)TipoLicitacaoEmpenho.Pregao)
            {
                strRetorno = "PREGAO";
            }
            else if (tipoLicitacaoEmpenho != (int)TipoLicitacaoEmpenho.Pregao)
            {
                codigoEventoEmpenho = (int)objMovimento.EmpenhoEvento.Codigo;

                switch (codigoEventoEmpenho)
                {
                    case (int)TipoEventoEmpenho.BEC: strRetorno = "BEC"; break;
                    case (int)TipoEventoEmpenho.DotacaoReservada: strRetorno = "SIAFISICO"; break;
                }
            }

            return strRetorno;
        }
        private static string RetornarTipoLicitacaoEmpenho(this MovimentoEntity objMovimento)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");


            #region Variaveis

            string strRetorno = string.Empty;
            bool eventoEmpenhoNulo = false;
            bool licitacaoEmpenhoNulo = false;
            #endregion Variaveis

            licitacaoEmpenhoNulo = (objMovimento.EmpenhoLicitacao.IsNull() || objMovimento.EmpenhoLicitacao.Id.IsNull());

            if ((!eventoEmpenhoNulo && licitacaoEmpenhoNulo) || (eventoEmpenhoNulo && !licitacaoEmpenhoNulo))
            {
                strRetorno = "NÃO FOI POSSÍVEL DEFINIR TIPO DE LICITACAO.";
            }
            else
            {
                strRetorno = GeralEnum.GetEnumDescription((TipoLicitacaoEmpenho)objMovimento.EmpenhoLicitacao.Id);
            }

            return strRetorno;
        }

        public static string ObterNLsMovimentacao(this MovimentoEntity objMovimento, bool retornaNLConsumoSeRequisicaoMaterial = false, bool retornaNLEstorno = false, TipoNotaSIAFEM @TipoNotaSIAFEM = TipoNotaSIAFEM.NL_Liquidacao, bool usaTransacao = false)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");

            #region Variaveis
            string strRetorno = null;
            MovimentoBusiness objBusiness = null;
            IList<string> notasLancamentoMovimentacao = null;
            int tipoMovimentacao = -1;
            TipoNotaSIAFEM tipoNotaSIAFEM = default(TipoNotaSIAFEM);
            #endregion Variaveis

            objBusiness = new MovimentoBusiness();
            tipoMovimentacao = objMovimento.TipoMovimento.Id;
            tipoNotaSIAFEM = @TipoNotaSIAFEM;

            if (tipoMovimentacao == (int)TipoMovimentacao.RequisicaoAprovada)
                tipoNotaSIAFEM = ((retornaNLConsumoSeRequisicaoMaterial) ? TipoNotaSIAFEM.NL_Consumo : TipoNotaSIAFEM.NL_Liquidacao);


            notasLancamentoMovimentacao = objBusiness.ObterNLsMovimentacao(objMovimento.Id.Value, tipoNotaSIAFEM, retornaNLEstorno, usaTransacao);

            if (notasLancamentoMovimentacao.HasElements())
                strRetorno = String.Join("|", notasLancamentoMovimentacao);

            return strRetorno;
        }

        public static string ObterNLsPagamentoMovimento(this MovimentoEntity objMovimento)
        {
            if (objMovimento.IsNull())
                throw new ArgumentException("Movimentação Nula");

            #region Variaveis
            string strRetorno = null;
            LiquidacaoBusiness objBusiness = null;
            IList<string> notasLancamentoMovimentacao = null;
            #endregion Variaveis

            objBusiness = new LiquidacaoBusiness();
            notasLancamentoMovimentacao = objBusiness.ObterNLsPagamentoMovimento(objMovimento.Id.Value);

            if (notasLancamentoMovimentacao.HasElements())
                strRetorno = String.Join("|", notasLancamentoMovimentacao);

            return strRetorno;
        }
        public static string ObterNLsPagamentoMovimentoEmpenho(this IList<MovimentoItemEntity> itensMovimentacoes)
        {
            if (!itensMovimentacoes.IsNotNullAndNotEmpty())
                throw new ArgumentException("Não há itens de movimentações para processar.");

            string strRetorno = null;
            IList<string> notasLancamentoEmpenho = null;

            notasLancamentoEmpenho = itensMovimentacoes.Select(movItem => movItem.NL_Liquidacao).ToList();

            if (notasLancamentoEmpenho.HasElements() && notasLancamentoEmpenho.Count > 1)
                strRetorno = String.Join("|", notasLancamentoEmpenho);
            else if (notasLancamentoEmpenho.HasElements() && notasLancamentoEmpenho.Count == 1)
                strRetorno = notasLancamentoEmpenho[0];

            return strRetorno;
        }

        public static TipoMaterialParaLiquidacao ObterTipoMaterial(this MovimentoEntity objMovimentacaoMaterial, bool preencheTipoMaterial = false)
        {
            TipoMaterialParaLiquidacao tipoMaterialMovimentacao;

            bool mesmaNaturezaDespesaTodosSubitens = false;
            string[] arrDadosSubitensMovimentacao = null;
            string[] arrInfoSubitem = null;
            IList<int> validarMesmoTipoMaterial = null;
            IList<string> errosExecucao = null;
            int tipoNaturezaDespesa = -1;
            string codigoSubitem = null;
            string natDespesaSubitemMaterial = null;


            validarMesmoTipoMaterial = new List<int>(objMovimentacaoMaterial.MovimentoItem.Count());
            arrDadosSubitensMovimentacao = objMovimentacaoMaterial.MovimentoItem.Select(movItem => { 
                                                                                                     string saidaConsulta = null;
                                                                                                     CatalogoBusiness objBusiness = null;

                                                                                                     if (movItem.SubItemMaterial.NaturezaDespesa.IsNull())
                                                                                                     {
                                                                                                         objBusiness = new CatalogoBusiness();
                                                                                                         objBusiness.SelectSubItemMaterial(movItem.SubItemMaterial.Id.Value);
                                                                                                         movItem.SubItemMaterial = objBusiness.SubItemMaterial;
                                                                                                     }

                                                                                                    saidaConsulta = String.Format("{0:12}|{1}", movItem.SubItemCodigoFormatado, movItem.NaturezaDespesaCodigo.ToString());
                                                                                                    return saidaConsulta;
                                                                                                   }).ToArray();
            tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.Indeterminado;
            errosExecucao = new List<string>();

            foreach (var dadosSubitem in arrDadosSubitensMovimentacao)
            {
                arrInfoSubitem = dadosSubitem.BreakLine("|");
                codigoSubitem = arrInfoSubitem[0];
                natDespesaSubitemMaterial = arrInfoSubitem[1];

                tipoNaturezaDespesa = (natDespesaSubitemMaterial.Length == 8) ? Int32.Parse(natDespesaSubitemMaterial[0].ToString()) : Int32.Parse(natDespesaSubitemMaterial[natDespesaSubitemMaterial.Length - 2].ToString());

                if (tipoNaturezaDespesa == 3) //Material de Consumo
                    tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.MaterialConsumo;
                else if (tipoNaturezaDespesa == 4) //Item Permanente
                    tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.MaterialPermanente;
                else
                    //errosExecucao.Add(String.Format("Subitem {0} (movimentação SAM {1}), não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, objMovimentacaoMaterial.NumeroDocumento, natDespesaSubitemMaterial));
                    throw new Exception(String.Format("Subitem {0} (movimentação SAM {1}), não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, objMovimentacaoMaterial.NumeroDocumento, natDespesaSubitemMaterial));

                validarMesmoTipoMaterial.Add((int)tipoMaterialMovimentacao);
                tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.Indeterminado;
            }

            mesmaNaturezaDespesaTodosSubitens = (validarMesmoTipoMaterial.Distinct().Count() == 1);

            if (!mesmaNaturezaDespesaTodosSubitens)
                //errosExecucao.Add(String.Format("Movimentação SAM {0} possui subitens de tipos de material diferentes (Consumo/Permanente)", objMovimentacaoMaterial.NumeroDocumento));
                throw new Exception(String.Format("Movimentação SAM {0} possui subitens de tipos de material diferentes (Consumo/Permanente)", objMovimentacaoMaterial.NumeroDocumento));
            else
                tipoMaterialMovimentacao = (TipoMaterialParaLiquidacao)validarMesmoTipoMaterial[0];

            if (preencheTipoMaterial)
                objMovimentacaoMaterial.TipoMaterial = tipoMaterialMovimentacao;

            return tipoMaterialMovimentacao;
        }
        public static TipoMaterialParaLiquidacao ObterTipoMaterial(this MovimentoItemEntity objItemMovimentacao, bool preencheTipoMaterial = false)
        {
            TipoMaterialParaLiquidacao tipoMaterialMovimentacao;

            CatalogoBusiness objBusiness = null;
            IList<string> errosExecucao = null;
            int tipoNaturezaDespesa = -1;
            string codigoSubitem = null;
            string natDespesaSubitemMaterial = null;



            tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.Indeterminado;
            errosExecucao = new List<string>();

            if (objItemMovimentacao.SubItemMaterial.IsNull())
            {
                objBusiness = new CatalogoBusiness();

                objBusiness.SelectSubItemMaterial(objItemMovimentacao.SubItemMaterial.Id.Value);
                objItemMovimentacao.SubItemMaterial = objBusiness.SubItemMaterial;
            }

            codigoSubitem = objItemMovimentacao.SubItemCodigoFormatado;
            natDespesaSubitemMaterial = objItemMovimentacao.NaturezaDespesaCodigo.ToString();
            tipoNaturezaDespesa = (natDespesaSubitemMaterial.Length == 8) ? Int32.Parse(natDespesaSubitemMaterial[0].ToString()) : Int32.Parse(natDespesaSubitemMaterial[natDespesaSubitemMaterial.Length - 2].ToString());

            if (tipoNaturezaDespesa == 3) //Material de Consumo
                tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.MaterialConsumo;
            else if (tipoNaturezaDespesa == 4) //Item Permanente
                tipoMaterialMovimentacao = TipoMaterialParaLiquidacao.MaterialPermanente;
            else
                //errosExecucao.Add(String.Format("Subitem {0} (movimentação SAM {1}), não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, objItemMovimentacao.Movimento.NumeroDocumento, natDespesaSubitemMaterial));
                throw new Exception(String.Format("Subitem {0} (movimentação SAM {1}), não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, objItemMovimentacao.Movimento.NumeroDocumento, natDespesaSubitemMaterial));


            objItemMovimentacao.SubItemMaterial.ObterTipoMaterial(true);
            tipoMaterialMovimentacao = objItemMovimentacao.SubItemMaterial.TipoMaterial;

            if (preencheTipoMaterial)
                objItemMovimentacao.SubItemMaterial.TipoMaterial = tipoMaterialMovimentacao;
            
            return tipoMaterialMovimentacao;
        }
        public static TipoMaterialParaLiquidacao ObterTipoMaterial(this SubItemMaterialEntity objSubitemMaterial, bool preencheTipoMaterial = false)
        {
            TipoMaterialParaLiquidacao tipoMaterialSubitem;

            IList<string> errosExecucao = null;
            int tipoNaturezaDespesa = -1;
            string codigoSubitem = null;
            string natDespesaSubitemMaterial = null;
            CatalogoBusiness objBusiness = null;


            tipoMaterialSubitem = TipoMaterialParaLiquidacao.Indeterminado;
            errosExecucao = new List<string>();


            codigoSubitem = objSubitemMaterial.Codigo.ToString();

            if (objSubitemMaterial.NaturezaDespesa.IsNull())
            {
                objBusiness = new CatalogoBusiness();
                objBusiness.SelectSubItemMaterial(objSubitemMaterial.Id.Value);
                objSubitemMaterial = objBusiness.SubItemMaterial;
            }

            natDespesaSubitemMaterial = objSubitemMaterial.NaturezaDespesa.Codigo.ToString();
            tipoNaturezaDespesa = (natDespesaSubitemMaterial.Length == 8) ? Int32.Parse(natDespesaSubitemMaterial[0].ToString()) : Int32.Parse(natDespesaSubitemMaterial[natDespesaSubitemMaterial.Length - 2].ToString());

            if (tipoNaturezaDespesa == 3) //Material de Consumo
                tipoMaterialSubitem = TipoMaterialParaLiquidacao.MaterialConsumo;
            else if (tipoNaturezaDespesa == 4) //Item Permanente
                tipoMaterialSubitem = TipoMaterialParaLiquidacao.MaterialPermanente;
            else
                //errosExecucao.Add(String.Format("Subitem {0}, não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, natDespesaSubitemMaterial));
                throw new Exception(String.Format("Subitem {0}, não foi possível identificar tipo de material (Consumo/Permanente) - Natureza Despesa {2}", codigoSubitem, natDespesaSubitemMaterial));


            if (preencheTipoMaterial)
                objSubitemMaterial.TipoMaterial = tipoMaterialSubitem;

            return tipoMaterialSubitem;
        }
    }
    #endregion Metodos Extensão
}
