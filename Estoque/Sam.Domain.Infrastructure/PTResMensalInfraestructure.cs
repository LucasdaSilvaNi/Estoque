using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using System.Linq.Expressions;
using Sam.Common.LambdaExpression;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using casasDecimais = Sam.Common.Util.GeralEnum.casasDecimais;
using System.Data;

namespace Sam.Domain.Infrastructure
{
    public partial class PTResMensalInfraestructure : BaseInfraestructure, IPTResMensalService
    {
        private PTResMensalEntity PTResMensal = new PTResMensalEntity();
        //TODO: Criar métodos de extensão especifico para isso, para poupar banco.
        private static IDictionary<string, BaseEntity> dicObjetoMemoria = new Dictionary<string, BaseEntity>(StringComparer.InvariantCultureIgnoreCase);
        private static IDictionary<string, string> preCache_NL_Consumo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        private const string nomeCampoMovItemIDs = "MovimentoItemIDs";
        private const string nomeCampoNumeroDocumento = "DocumentoRelacionado";
        private const string nomeCampoValorNLConsumo = "ValorNLConsumo";
        private const string nomeCampoNLConsumo = "NLConsumo";
        private const string nomeCampoNLLiquidacao = "NLLiquidacao";
        private const string nomeCampoNLReclassificacao = "NLReclassificacao";
        private const string fmtChaveConsultaMovItem = "TB_MOVIMENTO_ITEM__id: {0}";
        private const string fmtChaveConsultaGenerico = "{0}{1}";


        public PTResMensalEntity Entity
        {
            get { return PTResMensal; }
            set { PTResMensal = value; }
        }

        public Tuple<string, bool> AtualizarPTResMensal(PTResMensalEntity ptResM)
        {

            var ptResMensal = new TB_PTRES_MENSAL();
            ptResMensal = Db.TB_PTRES_MENSALs.Where(a => a.TB_PTRES_MENSAL_ID == ptResM.Id).FirstOrDefault();

            try
            {
                if (ptResMensal != null)
                {
                    if (ptResM.NlLancamento != null)
                    {
                        ptResMensal.TB_PTRES_MENSAL_NL = ptResM.NlLancamento;
                    }



                    Db.Refresh(RefreshMode.KeepChanges, ptResMensal);
                    Db.SubmitChanges();

                    return Tuple.Create("MovimentoItem: " + ptResM.Id + " atualizado.", true);


                }
                else
                {
                    return Tuple.Create("MovimentoItem não encontrado para este Almoxarifado.", false);

                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }




        }

        public void Salvar()
        {
            TB_PTRES_MENSAL tbPTResMensal = new TB_PTRES_MENSAL();

            if (this.Entity.Id.HasValue)
                tbPTResMensal = this.Db.TB_PTRES_MENSALs.Where(a => a.TB_PTRES_MENSAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                Db.TB_PTRES_MENSALs.InsertOnSubmit(tbPTResMensal);

            tbPTResMensal.TB_PTRES_ID = (int)this.Entity.PtRes.Id;
            tbPTResMensal.TB_PTRES_MENSAL_ANO_MES_REF = (int)this.Entity.AnoMesRef;
            tbPTResMensal.TB_PTRES_MENSAL_FLAG_LANC = this.Entity.FlagLancamento;
            tbPTResMensal.TB_PTRES_MENSAL_MENSAGEM_WS = this.Entity.MensagemWs;
            tbPTResMensal.TB_PTRES_MENSAL_NL = this.Entity.NlLancamento;
            tbPTResMensal.TB_PTRES_MENSAL_OBS = this.Entity.Obs;
            tbPTResMensal.TB_PTRES_MENSAL_RETORNO = this.Entity.Retorno;
            //tbPTResMensal.TB_PTRES_MENSAL_SEQ_PTRES                 = this.Entity.Seq_PtRes;
            tbPTResMensal.TB_PTRES_MENSAL_TIPO_LANC = this.Entity.TipoLancamento;
            tbPTResMensal.TB_PTRES_MENSAL_VALOR = this.Entity.Valor;
            //tbPTResMensal.TB_PTRES_MENSAL_WS_ID                     = this.Entity.WsID;
            tbPTResMensal.TB_ALMOXARIFADO_ID = (int)this.Entity.Almoxarifado.Id;
            tbPTResMensal.TB_UA_ID = (int)this.Entity.UA.Id;
            tbPTResMensal.TB_UGE_ID = (int)this.Entity.UGE.Id;
            tbPTResMensal.TB_UGE_ID_ALMOXARIFADO = (int)this.Entity.UgeAlmoxarifado.Id;
            tbPTResMensal.TB_NATUREZA_DESPESA_ID = (int)this.Entity.NaturezaDespesa.Id;
            tbPTResMensal.TB_MOVIMENTO_NUMERO_DOCUMENTO_RELACIONADO = this.Entity.DocumentoRelacionado;
            tbPTResMensal.TB_MOVIMENTO_ITEM_RELACIONADOS = this.Entity.MovimentoItemIDs;

            //Auditoria pagamentos SIAFEM
            tbPTResMensal.TB_LOGIN_ID = this.Entity.UsuarioSamId;
            tbPTResMensal.TB_PTRES_MENSAL_DATA_PAGAMENTO = DateTime.Now;

            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_PTRES_MENSAL tbPTResMensal
                = this.Db.TB_PTRES_MENSALs.Where(a => a.TB_PTRES_MENSAL_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_PTRES_MENSALs.DeleteOnSubmit(tbPTResMensal);
            this.Db.SubmitChanges();
        }

        public PTResMensalEntity Listar(int _id)
        {
            throw new NotImplementedException();
        }

        public IList<PTResMensalEntity> Listar()
        {
            throw new NotImplementedException();
        }

        public IList<PTResMensalEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public PTResMensalEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public IList<PTResMensalEntity> ListarNLsConsumo(Expression _expWhere)
        {
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IList<PTResMensalEntity> lstRetorno = null;


            expWhere = ((Expression<Func<TB_PTRES_MENSAL, bool>>)_expWhere);
            qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);
            lstRetorno = qryConsulta.Select(_instanciadorDTOPTResMensal())
                                    .ToList();

            if (lstRetorno.HasElements())
                this.totalregistros = lstRetorno.Count;
            else
                lstRetorno = new List<PTResMensalEntity>();

            return lstRetorno;
        }

        public IList<PTResMensalEntity> _xpListar(Expression<Func<TB_PTRES_MENSAL, bool>> _expWhere)
        {
            IList<PTResMensalEntity> lstRetorno = null;

            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            //string strSQL = null;

            expWhere = _expWhere;

            qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);//.AsQueryable<TB_PTRES_MENSAL>();
            lstRetorno = new List<PTResMensalEntity>();


            lstRetorno = qryConsulta.Select(_instanciadorDTOPTResMensal())
                                    .ToList();

            if (lstRetorno.HasElements())
                this.totalregistros = lstRetorno.Count;
            else
                lstRetorno = new List<PTResMensalEntity>();

            return lstRetorno;
        }


        public PTResMensalEntity ObterNLConsumoPaga(int almoxID, int anoMesRef, int uaID, int ptresID, int natDespesaID, decimal valorNotaConsumo)
        {
            #region Verificacao Argumentos
            if ((almoxID == 0) || (anoMesRef == 0) || (uaID == 0) || (ptresID == 0) || (natDespesaID == 0) || (valorNotaConsumo == 0))
                throw new ArgumentException("Argumento Inválido fornecido para a função");
            #endregion

            PTResMensalEntity objRetorno = null;

            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;

            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && ptResMensal.TB_UA_ID == uaID
                                    && ptResMensal.TB_PTRES_ID == ptresID
                                    && ptResMensal.TB_NATUREZA_DESPESA_ID == natDespesaID
                                    && ptResMensal.TB_PTRES_MENSAL_VALOR == valorNotaConsumo);

            //objRetorno = _xpListar(expWhere).FirstOrDefault();
            objRetorno = ListarNLsConsumo(expWhere).FirstOrDefault();

            return objRetorno;

        }

        public IList<PTResMensalEntity> ObterNLsConsumoPagas(int almoxID, int anoMesRef, bool retornaNLEstornadas = false)
        {
            IList<PTResMensalEntity> lstRetorno = null;

            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            char tipoLancamento = '\0';
            char flagLancamento = 'S';

            if (!retornaNLEstornadas)
                tipoLancamento = 'N';
            else
                tipoLancamento = 'E';

            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && (ptResMensal.TB_PTRES_MENSAL_NL != null || ptResMensal.TB_PTRES_MENSAL_NL != string.Empty) //sem NL, não foi liquidado ou estornado
                                    && (ptResMensal.TB_PTRES_MENSAL_RETORNO != null || ptResMensal.TB_PTRES_MENSAL_RETORNO != string.Empty) //Se não há dados neste campo, não foi processado
                                    && (ptResMensal.TB_PTRES_MENSAL_TIPO_LANC == tipoLancamento) //&& ptResMensal.TB_PTRES_MENSAL_FLAG_LANC != '\0' && ptResMensal.TB_PTRES_MENSAL_FLAG_LANC != ' ') //Se for registro estornado, não retornar
                                    && (ptResMensal.TB_PTRES_MENSAL_FLAG_LANC == flagLancamento)); //Processado

            //lstRetorno = _xpListar(expWhere);
            lstRetorno = ListarNLsConsumo(expWhere);
            lstRetorno = lstRetorno.OrderByDescending(_notaConsumo => _notaConsumo.Id).ToList();

            return lstRetorno;

        }
        public IList<PTResMensalEntity> ObterNLsConsumoNulas(int almoxID, int anoMesRef, bool retornaNLEstornadas = false)
        {
            IList<PTResMensalEntity> lstRetorno = null;
            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;

            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && (ptResMensal.TB_PTRES_MENSAL_NL == null || ptResMensal.TB_PTRES_MENSAL_NL == string.Empty)
                                    && (ptResMensal.TB_PTRES_MENSAL_RETORNO == null || ptResMensal.TB_PTRES_MENSAL_RETORNO == string.Empty));

            //lstRetorno = _xpListar(expWhere);
            lstRetorno = ListarNLsConsumo(expWhere);
            lstRetorno = lstRetorno.OrderByDescending(_notaConsumo => _notaConsumo.Id).ToList();

            return lstRetorno;
        }
        public PTResMensalEntity ObterUltimaNLConsumoRegistradaParaAgrupamento(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, decimal valorNotaConsumo, bool consideraValorNotaConsumo = false, bool verificaStatusAtivoMovimentacoes = false)
        {
            PTResMensalEntity objEntidade = null;

            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            char flagLancamento = 'S'; //Registro lancado com sucesso, no SIAFEM.

            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_UA_ID == uaID
                                    && ptResMensal.TB_PTRES_ID == PTResID
                                    && ptResMensal.TB_NATUREZA_DESPESA_ID == naturezaDespesaID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && ptResMensal.TB_PTRES_MENSAL_FLAG_LANC == flagLancamento);

            if (consideraValorNotaConsumo)
                expWhere = LambaExpressionHelper.And(expWhere, (ptresMensal => ptresMensal.TB_PTRES_MENSAL_VALOR == valorNotaConsumo));

            qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);


            objEntidade = qryConsulta.Where(expWhere)
                                     .OrderByDescending(ptresMensal => ptresMensal.TB_PTRES_MENSAL_ID)
                                     .Select(_instanciadorDTOPTResMensal())
                                     .FirstOrDefault();

            if (objEntidade.IsNotNull() && verificaStatusAtivoMovimentacoes)
            {
                bool movimentacaoEstornada = true;
                movimentacaoEstornada &= !StatusMovimentacao_Por_MovimentoItem(objEntidade.MovimentoItemIDs.BreakLine());

                if (movimentacaoEstornada)
                {
                    objEntidade.Status = "C";
                    objEntidade.FlagLancamento = 'I';
                }
            }

            return objEntidade;

        }

        public IList<PTResMensalEntity> ObterUltimasNLConsumoRegistradasParaAgrupamento(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, bool verificaStatusAtivoMovimentacoes = false)
        {
            IList<PTResMensalEntity> listaEntidades = null;

            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            char flagLancamento = 'S'; //Registro lancado com sucesso, no SIAFEM.

            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_UA_ID == uaID
                                    && ptResMensal.TB_PTRES_ID == PTResID
                                    && ptResMensal.TB_NATUREZA_DESPESA_ID == naturezaDespesaID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && ptResMensal.TB_PTRES_MENSAL_FLAG_LANC == flagLancamento
                                    && ((ptResMensal.TB_PTRES_MENSAL_NL == null || ptResMensal.TB_PTRES_MENSAL_NL != null) || (ptResMensal.TB_PTRES_MENSAL_NL == string.Empty || ptResMensal.TB_PTRES_MENSAL_NL != string.Empty)));

            qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);
            var strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            listaEntidades = qryConsulta.Where(expWhere)
                                     .OrderByDescending(ptresMensal => ptresMensal.TB_PTRES_MENSAL_ID)
                                     //.DistinctBy(distintor => new { tipoLancamentoSIAFEM = distintor.TipoLancamento.Value, valorNotaConsumo = distintor.Valor.Value })
                                     .DistinctBy(rowTabela => new { tipoLancamentoSIAFEM = rowTabela.TB_PTRES_MENSAL_TIPO_LANC, valorNotaConsumo = rowTabela.TB_PTRES_MENSAL_VALOR })
                                     .Select(_instanciadorDTOPTResMensal())
                                     .ToList();

            if (listaEntidades.HasElements() && verificaStatusAtivoMovimentacoes)
            {
                bool movimentacaoEstornada = true;
                listaEntidades.ToList()
                              .ForEach(nlConsumo =>
                              {
                                  movimentacaoEstornada &= !StatusMovimentacao_Por_MovimentoItem(nlConsumo.MovimentoItemIDs.BreakLine());

                                  if (movimentacaoEstornada)
                                  {
                                      nlConsumo.Status = "C";
                                      nlConsumo.FlagLancamento = 'I';
                                  }

                                  movimentacaoEstornada = true;
                              });

            }

            return listaEntidades;
        }

        private bool PossuiParPagamento_Estorno(int almoxID, int uaID, int PTResID, int naturezaDespesaID, int anoMesRef, string tipoLancamentoSIAFEM)
        {
            bool NLsSemelhantes = false;
            char opInversa = '\0';


            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            char flagLancamento = 'S'; //Registro lancado com sucesso, no SIAFEM.


            opInversa = ((tipoLancamentoSIAFEM == "N") ? 'E' : 'N');
            expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                    && ptResMensal.TB_UA_ID == uaID
                                    && ptResMensal.TB_PTRES_ID == PTResID
                                    && ptResMensal.TB_NATUREZA_DESPESA_ID == naturezaDespesaID
                                    && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                    && ptResMensal.TB_PTRES_MENSAL_FLAG_LANC == flagLancamento
                                    && ptResMensal.TB_PTRES_MENSAL_TIPO_LANC == opInversa);

            qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);


            NLsSemelhantes = qryConsulta.Where(expWhere)
                                     .OrderByDescending(ptresMensal => ptresMensal.TB_PTRES_MENSAL_ID)
                                     .Select(_instanciadorDTOPTResMensal())
                                     .Count() == 1;

            return NLsSemelhantes;
        }

        public bool PossuiParPagamento_Estorno(PTResMensalEntity nlConsumo)
        {
            PTResMensalEntity objEntidade = null;
            Expression<Func<TB_PTRES_MENSAL, bool>> expWhere = null;
            IQueryable<TB_PTRES_MENSAL> qryConsulta = null;
            char flagLancamento = 'S'; //Registro lancado com sucesso, no SIAFEM.
            bool NLsSemelhantes = false;
            char opInversa = '\0';


            int almoxID = 0;
            int uaID = 0;
            int PTResID = 0;
            int naturezaDespesaID = 0;
            int anoMesRef = 0;
            string tipoLancamentoSIAFEM = null;
            decimal valorNota = 0.00m;

            if (nlConsumo.IsNotNull())
            {
                almoxID = nlConsumo.Almoxarifado.Id.Value;
                uaID = nlConsumo.UA.Id.Value;
                PTResID = nlConsumo.PtRes.Id.Value;
                naturezaDespesaID = nlConsumo.NaturezaDespesa.Id.Value;
                anoMesRef = nlConsumo.AnoMesRef;
                tipoLancamentoSIAFEM = nlConsumo.TipoLancamento.ToString();
                valorNota = nlConsumo.Valor.Value;

                opInversa = ((tipoLancamentoSIAFEM == "N") ? 'E' : 'N');
                expWhere = (ptResMensal => ptResMensal.TB_ALMOXARIFADO_ID == almoxID
                                        && ptResMensal.TB_UA_ID == uaID
                                        && ptResMensal.TB_PTRES_ID == PTResID
                                        && ptResMensal.TB_NATUREZA_DESPESA_ID == naturezaDespesaID
                                        && ptResMensal.TB_PTRES_MENSAL_ANO_MES_REF == anoMesRef
                                        && ptResMensal.TB_PTRES_MENSAL_FLAG_LANC == flagLancamento
                                        && ptResMensal.TB_PTRES_MENSAL_TIPO_LANC == opInversa
                                        && ptResMensal.TB_PTRES_MENSAL_VALOR == valorNota);

                qryConsulta = this.Db.TB_PTRES_MENSALs.Where(expWhere);
                //.OrderByDescending(ptresMensal => ptresMensal.TB_PTRES_MENSAL_ID);

                var strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

                objEntidade = qryConsulta.Where(expWhere)
                                         //.OrderByDescending(ptresMensal => ptresMensal.TB_PTRES_MENSAL_ID)
                                         .Select(_instanciadorDTOPTResMensal())
                                         .FirstOrDefault();

                NLsSemelhantes = objEntidade.IsNotNull();
            }


            return NLsSemelhantes;

        }

        private bool EhParPagamentoEstorno_NLConsumo(PTResMensalEntity nlConsumoGerada, PTResMensalEntity nlConsumoPaga)
        {
            bool NLsComValorDivergente = false;

            NLsComValorDivergente = (nlConsumoGerada.UA.Codigo == nlConsumoPaga.UA.Codigo &&
                                     nlConsumoGerada.PtRes.Codigo == nlConsumoPaga.PtRes.Codigo &&
                                     nlConsumoGerada.NaturezaDespesa.Codigo == nlConsumoPaga.NaturezaDespesa.Codigo &&
                                     nlConsumoGerada.Valor == nlConsumoPaga.Valor &&
                                     nlConsumoGerada.TipoLancamento != nlConsumoPaga.TipoLancamento);

            return NLsComValorDivergente;
        }

        public Tuple<string, bool> ObterNLConsumoAlterada(PTResMensalEntity ptResM)
        {
            var ptResMensal = new TB_PTRES_MENSAL();
            ptResMensal = Db.TB_PTRES_MENSALs.Where(a => a.TB_PTRES_MENSAL_ID == ptResM.Id).FirstOrDefault();

            try
            {
                if (ptResMensal != null)
                {
                    if (ptResM.NlLancamento != null)
                    {
                        ptResMensal.TB_PTRES_MENSAL_NL = ptResM.NlLancamento;
                    }



                    Db.Refresh(RefreshMode.KeepChanges, ptResMensal);
                    Db.SubmitChanges();

                    return Tuple.Create("PTResMensal: " + ptResM.Id + " atualizado.", true);


                }
                else
                {
                    return Tuple.Create("PTResMensal não encontrado para este Almoxarifado.", false);

                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }


        }

        public IList<PTResMensalEntity> Imprimir()
        {
            IList<PTResMensalEntity> resultado = (from a in this.Db.TB_PTRES_MENSALs
                                                  orderby a.TB_PTRES_MENSAL_ID
                                                  select new PTResMensalEntity
                                                  {
                                                      Id = a.TB_PTRES_MENSAL_ID,
                                                      Obs = a.TB_PTRES_MENSAL_OBS
                                                  })
                                              .ToList<PTResMensalEntity>();
            return resultado;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_PTRES_MENSALs
                .Where(a => a.TB_PTRES_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_PTRES_MENSALs
                .Where(a => a.TB_PTRES_MENSAL_ID == this.Entity.Id)
                .Count() > 0;
            }
            return retorno;
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoAlmox(int gestorId, int anoMesRef, int? almoxId)
        {
            List<PTResMensalEntity> lstRetorno = null;
            IList<PTResMensalEntity> nlConsumoComLote = null;
            IList<string> controleLote = null;
            IDictionary<string, string> cacheDadosNLConsumoGeradas = new Dictionary<string, string>();
            IQueryable qryConsulta = null;
            PTResMensalEntity nlConsumo = null;
            string strSQL = null;
            string chaveAgrupamentoNLConsumo = null;
            string fmtChaveAgrupamentoNLConsumo = null;
            string strContadorLote = null;
            string[] chavesComLotes = null;
            string[][] chavesComLotesTemp = null;

            int contadorLote = 0;
            bool possuiLote = false;

            qryConsulta = this.obterMassaDadosParaGeracaoNLConsumo(gestorId, anoMesRef, almoxId);

            if (Constante.isSamWebDebugged)
            {
                strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            }

            nlConsumoComLote = new List<PTResMensalEntity>();
            controleLote = new List<string>();
            lstRetorno = qryConsulta.Cast<PTResMensalEntity>()
                                    .ToListNoLock();

            cacheDadosNLConsumoGeradas = gerarCacheNLConsumo(lstRetorno);

            //Correcao efetuada em 04/11/2015: UnidadeGestora é a UGE do almoxarifado, UGConsumidora é a UGE da UA da requisição.
            lstRetorno = lstRetorno.GroupBy(consumoAlmox => new
            {
                ugeAlmoxarifadoID = consumoAlmox.UgeAlmoxarifado.Id.Value,
                ugeUaID = consumoAlmox.UA.Uge.Id.Value,
                uaID = consumoAlmox.UA.Id.Value,
                codigoPTRes = consumoAlmox.PtRes.Codigo.Value,
                codigoNaturezaDespesa = consumoAlmox.NaturezaDespesa.Codigo,
                nlSiafem = consumoAlmox.NlLancamento
            })
                                    .Select(grpConsumoAlmox =>
                                    {
                                        UAEntity _ua = base.ObterEntidade<UAEntity>(String.Format("TB_UA__id: {0}", grpConsumoAlmox.Key.uaID));
                                        GestorEntity _gestor = (_ua.IsNotNull() ? _ua.Gestor : null);
                                        UOEntity _uo = (_ua.IsNotNull() ? _ua.Uo : null);

                                        UGEEntity ugeAlmox = base.ObterEntidade<UGEEntity>(String.Format("TB_UGE__id: {0}", grpConsumoAlmox.Key.ugeAlmoxarifadoID));
                                        UGEEntity ugeUA = base.ObterEntidade<UGEEntity>(String.Format("TB_UGE__id: {0}", grpConsumoAlmox.Key.ugeUaID));
                                        NaturezaDespesaEntity natDespesa = base.ObterEntidade<NaturezaDespesaEntity>(grpConsumoAlmox.Key.codigoNaturezaDespesa, true);
                                        PTResEntity ptresMovItem = ((grpConsumoAlmox.Key.codigoNaturezaDespesa.IsNotNull()) ? base.ObterEntidade<PTResEntity>(String.Format("TB_PTRE__codigo: {0}", grpConsumoAlmox.Key.codigoPTRes), true) : null);

                                        fmtChaveAgrupamentoNLConsumo = "{0}|{1}|{2}";
                                        //chaveAgrupamentoNLConsumo = String.Format(fmtChaveAgrupamentoNLConsumo, grpConsumoAlmox.Key.uaID, grpConsumoAlmox.Key.ptresID, grpConsumoAlmox.Key.natDespesaID);
                                        chaveAgrupamentoNLConsumo = String.Format(fmtChaveAgrupamentoNLConsumo, grpConsumoAlmox.Key.uaID, grpConsumoAlmox.Key.codigoPTRes, grpConsumoAlmox.Key.codigoNaturezaDespesa);

                                        possuiLote = (cacheDadosNLConsumoGeradas.Keys.Where(chaveConsulta => chaveConsulta.Contains(String.Format("{0}{1}", chaveAgrupamentoNLConsumo, "|c"))).Count() > 1);
                                        if (possuiLote && !controleLote.Contains(chaveAgrupamentoNLConsumo))
                                        {
                                            controleLote.Add(chaveAgrupamentoNLConsumo);


                                            var documentosRelacionados = "";
                                            var movimentoItemIDs = "";
                                            decimal valorAgrupamento = 0.00m;
                                            var numeroNLConsumo = grpConsumoAlmox.Key.nlSiafem;
                                            string NL_Liquidacao = string.Empty;
                                            string NL_Reclassificacao = string.Empty;

                                            fmtChaveAgrupamentoNLConsumo = "{0}|{1}";
                                            chavesComLotesTemp = (cacheDadosNLConsumoGeradas.Keys
                                                                                    .Where(chaveConsulta => chaveConsulta.Contains(String.Format("{0}{1}", chaveAgrupamentoNLConsumo, "|c")))
                                                                                    .Select(chaveConsulta => new[]  { chaveConsulta.Replace(String.Format("|{0}", chaveConsulta.BreakLine("|", 4)), null),
                                                                                                                     chaveConsulta.Replace(String.Format("|{0}", chaveConsulta.BreakLine("|", 3)), null).Replace(String.Format("|{0}", chaveConsulta.BreakLine("|", 4)), null)})
                                                                                    .ToArray());


                                            chavesComLotes = chavesComLotesTemp.Where(ch => ch[1].Equals(chaveAgrupamentoNLConsumo)).Select(a => a[0]).Distinct().ToArray();


                                            foreach (var chaveLote in chavesComLotes)
                                            {
                                                nlConsumo = null;
                                                numeroNLConsumo = null;
                                                strContadorLote = contadorLote.ToString();
                                                chaveAgrupamentoNLConsumo = chaveLote;
                                                documentosRelacionados = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "DocumentoRelacionado")];
                                                movimentoItemIDs = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "MovimentoItemIDs")];
                                                valorAgrupamento = Decimal.Parse(cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "ValorNLConsumo")]);
                                                numeroNLConsumo = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "NLConsumo")];
                                                NL_Liquidacao = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "NLLiquidacao")];
                                                NL_Reclassificacao = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "NLReclassificacao")];

                                                contadorLote++;

                                                if (!String.IsNullOrWhiteSpace(numeroNLConsumo) && numeroNLConsumo == "NULL")
                                                    numeroNLConsumo = null;

                                                nlConsumo = this._instanciadorDTOPTResMensal(
                                                                                                anoMesRef,
                                                                                                grpConsumoAlmox.Key.uaID,
                                                                                                (_uo.IsNotNull() ? _uo.Id.Value : 0),
                                                                                                grpConsumoAlmox.Key.codigoPTRes,
                                                                                                grpConsumoAlmox.Key.codigoNaturezaDespesa,
                                                                                                grpConsumoAlmox.Key.ugeUaID,
                                                                                                grpConsumoAlmox.Key.ugeAlmoxarifadoID,
                                                                                                documentosRelacionados,
                                                                                                movimentoItemIDs,
                                                                                                valorAgrupamento,
                                                                                                numeroNLConsumo,
                                                                                                null,
                                                                                                0,
                                                                                                true,
                                                                                                contadorLote);


                                                nlConsumo.UgeAlmoxarifado_Codigo = ugeAlmox.Codigo;
                                                nlConsumo.UgeAlmoxarifado_Descricao = ugeAlmox.Descricao;
                                                nlConsumo.Uge_Codigo = ugeUA.Codigo;
                                                nlConsumo.Uge_Descricao = ugeUA.Descricao;
                                                nlConsumo.Uo_Codigo = _uo.Codigo;
                                                nlConsumo.Uo_Descricao = _uo.Descricao;
                                                nlConsumo.Ua_Codigo = _ua.Codigo;
                                                nlConsumo.Ua_Descricao = _ua.Descricao;
                                                nlConsumo.NaturezaDespesa_Codigo = natDespesa.Codigo;
                                                nlConsumo.NaturezaDespesa_Descricao = natDespesa.Descricao;
                                                nlConsumo.PtRes_Codigo = ptresMovItem.Codigo;

                                                nlConsumo.NL_Liquidacao = NL_Liquidacao;
                                                nlConsumo.NL_Reclassificacao = NL_Reclassificacao;

                                                nlConsumoComLote.Add(nlConsumo);
                                            }

                                            contadorLote = 0;
                                            nlConsumo = null;
                                        }
                                        else
                                        {
                                            if (!controleLote.Contains(chaveAgrupamentoNLConsumo))
                                            {
                                                fmtChaveAgrupamentoNLConsumo = "{0}|{1}";
                                                nlConsumo = new PTResMensalEntity()
                                                {
                                                    UA = _ua,
                                                    Gestor = _ua.Gestor,
                                                    PtRes = ((grpConsumoAlmox.Key.codigoNaturezaDespesa.IsNotNull()) ? base.ObterEntidade<PTResEntity>(String.Format("TB_PTRE__codigo: {0}", grpConsumoAlmox.Key.codigoPTRes), true) : null),
                                                    NaturezaDespesa = base.ObterEntidade<NaturezaDespesaEntity>(grpConsumoAlmox.Key.codigoNaturezaDespesa, true),
                                                    //Almoxarifado = base.ObterEntidade<AlmoxarifadoEntity>(String.Format("TB_ALMOXARIFADO__id: {0}", almoxId)),
                                                    UGE = base.ObterEntidade<UGEEntity>(String.Format("TB_UGE__id: {0}", grpConsumoAlmox.Key.ugeUaID)),
                                                    UO = _uo,
                                                    UgeAlmoxarifado = base.ObterEntidade<UGEEntity>(String.Format("TB_UGE__id: {0}", grpConsumoAlmox.Key.ugeAlmoxarifadoID)),

                                                    AnoMesRef = anoMesRef,
                                                    //NlLancamento = grpConsumoAlmox.Key.nlSiafem,
                                                    NlLancamento = ((!String.IsNullOrWhiteSpace(grpConsumoAlmox.Key.nlSiafem) && (grpConsumoAlmox.Key.nlSiafem.ToLowerInvariant() == "NULL")) ? null : grpConsumoAlmox.Key.nlSiafem),
                                                    TipoLancamento = (!String.IsNullOrWhiteSpace(grpConsumoAlmox.Key.nlSiafem) ? 'N' : (char?)null),
                                                    Valor = Decimal.Parse(cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "ValorNLConsumo")]),
                                                    DocumentoRelacionado = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "DocumentoRelacionado")],
                                                    MovimentoItemIDs = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "MovimentoItemIDs")],

                                                    UgeAlmoxarifado_Codigo = ugeAlmox.Codigo,
                                                    UgeAlmoxarifado_Descricao = ugeAlmox.Descricao,
                                                    Uge_Codigo = ugeUA.Codigo,
                                                    Uge_Descricao = ugeUA.Descricao,
                                                    Uo_Codigo = _uo.Codigo,
                                                    Uo_Descricao = _uo.Descricao,
                                                    Ua_Codigo = _ua.Codigo,
                                                    Ua_Descricao = _ua.Descricao,
                                                    NaturezaDespesa_Codigo = natDespesa.Codigo,
                                                    NaturezaDespesa_Descricao = natDespesa.Descricao,
                                                    PtRes_Codigo = ptresMovItem.Codigo,

                                                    NL_Liquidacao = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "NLLiquidacao")],
                                                    NL_Reclassificacao = cacheDadosNLConsumoGeradas[String.Format(fmtChaveAgrupamentoNLConsumo, chaveAgrupamentoNLConsumo, "NLReclassificacao")]
                                                };
                                            }
                                        }

                                        return nlConsumo;
                                    })
                                    .Where(linhaConsumoAlmox => linhaConsumoAlmox.IsNotNull())
                                    .DistinctBy(_distintor => new { ua = _distintor.UA.Id.Value, ptres = ((_distintor.PtRes.IsNotNull()) ? _distintor.PtRes.Codigo.Value : 0), nd = _distintor.NaturezaDespesa.Codigo, isLote = _distintor.IntegraLote, sequenciador = ((_distintor.NumeradorSequencia.IsNotNull()) ? _distintor.NumeradorSequencia.Value : 0) })
                                    .ToList();

            lstRetorno.AddRange(nlConsumoComLote);

            this.totalregistros = lstRetorno.Count;


            var objAlmox = base.ObterEntidade<AlmoxarifadoEntity>(String.Format("TB_ALMOXARIFADO__id: {0}", almoxId));
            lstRetorno.ForEach(x => x.Almoxarifado = objAlmox);

            return lstRetorno;
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoImediato(int almoxID, int anoMesRef, int orgaoId, int idPerfil)
        {
            List<PTResMensalEntity> lstRetorno = null;
            IQueryable qryConsulta = null;
            string strSQL = null;

            if ((idPerfil == (int)GeralEnum.TipoPerfil.AdministradorGeral || idPerfil == (int)GeralEnum.TipoPerfil.AdministradorOrgao) && almoxID == 0) //almoxID = 0 - representa todos almoxarifados 
            {
                qryConsulta = this.obterMassaDadosParaGeracaoNLConsumoImediatoPorOrgao(orgaoId, anoMesRef);
            }
            else
            {
                qryConsulta = this.obterMassaDadosParaGeracaoNLConsumoImediatoPorAlmox(almoxID, anoMesRef);
            }

            if (Constante.isSamWebDebugged)
            {
                strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            }

            lstRetorno = qryConsulta.Cast<PTResMensalEntity>()
                                    .ToListNoLock();

            lstRetorno = (from l in lstRetorno
                          group l by new
                          {
                              l.DocumentoRelacionado
                          }
                                into grp
                          select new PTResMensalEntity
                          {
                              AnoMesRef = grp.Select(x => x.AnoMesRef).FirstOrDefault(),

                              Almoxarifado = grp.Select(x => x.Almoxarifado).FirstOrDefault(),
                              UgeAlmoxarifado = grp.Select(x => x.UgeAlmoxarifado).FirstOrDefault(),
                              UGE = grp.Select(x => x.UGE).FirstOrDefault(),
                              UA = grp.Select(x => x.UA).FirstOrDefault(),
                              UO = grp.Select(x => x.UO).FirstOrDefault(),
                              PtRes = grp.Select(x => x.PtRes).FirstOrDefault(),
                              NaturezaDespesa = grp.Select(x => x.NaturezaDespesa).FirstOrDefault(),
                              Gestor = grp.Select(x => x.Gestor).FirstOrDefault(),

                              Valor = grp.Sum(x => x.Valor),
                              DocumentoRelacionado = grp.Key.DocumentoRelacionado,
                              NlLancamento = grp.Select(x => x.NlLancamento).FirstOrDefault(),
                              MovItemID = grp.Select(x => x.MovItemID).FirstOrDefault(),

                              UgeAlmoxarifado_Codigo = grp.Select(x => x.UgeAlmoxarifado_Codigo).FirstOrDefault(),
                              UgeAlmoxarifado_Descricao = grp.Select(x => x.UgeAlmoxarifado_Descricao).FirstOrDefault(),
                              Uge_Codigo = grp.Select(x => x.Uge_Codigo).FirstOrDefault(),
                              Uge_Descricao = grp.Select(x => x.Uge_Descricao).FirstOrDefault(),
                              Uo_Codigo = grp.Select(x => x.Uo_Codigo).FirstOrDefault(),
                              Uo_Descricao = grp.Select(x => x.Uo_Descricao).FirstOrDefault(),
                              Ua_Codigo = grp.Select(x => x.Ua_Codigo).FirstOrDefault(),
                              Ua_Descricao = grp.Select(x => x.Ua_Descricao).FirstOrDefault(),
                              NaturezaDespesa_Codigo = grp.Select(x => x.NaturezaDespesa_Codigo).FirstOrDefault(),
                              NaturezaDespesa_Descricao = grp.Select(x => x.NaturezaDespesa_Descricao).FirstOrDefault(),
                              PtRes_Codigo = grp.Select(x => x.PtRes_Codigo).FirstOrDefault(),

                              NL_Liquidacao = grp.Select(x => x.NL_Liquidacao).FirstOrDefault(),
                              NL_Reclassificacao = grp.Select(x => x.NL_Reclassificacao).FirstOrDefault()

                          }).ToList();

            this.totalregistros = lstRetorno.Count;

            return lstRetorno;
        }


        private IDictionary<string, string> gerarCacheNLConsumo(IList<PTResMensalEntity> lstRetorno)
        {
            //int numRowsAgrupadas = 0;
            //int numRowsProcessadas = 0;

            string agrupamentoNLConsumo;
            string chaveCampo;
            string documentosRelacionados;
            string itensMovIDs;
            string numeroNLConsumo;
            decimal? valorNLConsumo;
            string nlLiquidacao;
            string nlReclassificacao;

            preCache_NL_Consumo = new Dictionary<string, string>();

            //TODO: Incluir NL's estornadas, cujo agrupamento não esteja sendo relacionado, devido a estorno de requisição originária;
            lstRetorno.GroupBy(linhaConsumoAlmox => new
            {
                ugeAlmoxarifadoId = linhaConsumoAlmox.UgeAlmoxarifado.Id.Value,
                ugeUaId = linhaConsumoAlmox.UA.Uge.Id.Value,
                uaID = linhaConsumoAlmox.UA.Id.Value,
                codigoPTRes = linhaConsumoAlmox.PtRes.Codigo.Value,
                codigoNaturezaDespesa = linhaConsumoAlmox.NaturezaDespesa.Codigo,
                nlSiafem = linhaConsumoAlmox.NlLancamento
            })
                      .ToList()
                      .ForEach(grpConsumoAlmox =>
                      {
                          agrupamentoNLConsumo = String.Format("{0}|{1}|{2}", grpConsumoAlmox.Key.uaID, grpConsumoAlmox.Key.codigoPTRes, grpConsumoAlmox.Key.codigoNaturezaDespesa);
                          var massaDadosGeracao = lstRetorno.Where(nlConsumo => nlConsumo.UA.Id == grpConsumoAlmox.Key.uaID
                                                                             && nlConsumo.PtRes.Codigo == grpConsumoAlmox.Key.codigoPTRes
                                                                             && nlConsumo.NaturezaDespesa.Codigo == grpConsumoAlmox.Key.codigoNaturezaDespesa)
                                                            .ToList();

                          documentosRelacionados = String.Join("\n", massaDadosGeracao.DistinctBy(nlConsumo => nlConsumo.DocumentoRelacionado)
                                                                                      .Select(nlConsumo => nlConsumo.DocumentoRelacionado));

                          itensMovIDs = String.Join("\n", massaDadosGeracao.DistinctBy(nlConsumo => nlConsumo.MovItemID.ToString())
                                                                           .Select(nlConsumo => nlConsumo.MovItemID));

                          valorNLConsumo = massaDadosGeracao.DistinctBy(nlConsumo => nlConsumo.NlLancamento)
                                                            .Sum(consumoAlmox => consumoAlmox.Valor.Value.Truncar((int)casasDecimais.paraValorMonetario))
                                                            .Truncar((int)casasDecimais.paraValorMonetario);

                          numeroNLConsumo = String.Join("&", massaDadosGeracao.Select(nlConsumo => nlConsumo.NlLancamento));

                          nlLiquidacao = String.Join("\n", massaDadosGeracao.DistinctBy(nlConsumo => nlConsumo.NL_Liquidacao)
                                                                                      .Select(nlConsumo => nlConsumo.NL_Liquidacao));

                          nlReclassificacao = String.Join("\n", massaDadosGeracao.DistinctBy(nlConsumo => nlConsumo.NL_Reclassificacao)
                                                                                          .Select(nlConsumo => nlConsumo.NL_Reclassificacao));

                          chaveCampo = String.Format("{0}|{1}", agrupamentoNLConsumo, "DocumentoRelacionado");
                          preCache_NL_Consumo.InserirValor<string, string>(chaveCampo, documentosRelacionados);

                          chaveCampo = String.Format("{0}|{1}", agrupamentoNLConsumo, "MovimentoItemIDs");
                          preCache_NL_Consumo.InserirValor<string, string>(chaveCampo, itensMovIDs);

                          chaveCampo = String.Format("{0}|{1}", agrupamentoNLConsumo, "ValorNLConsumo");
                          preCache_NL_Consumo.InserirValor<string, string>(chaveCampo, valorNLConsumo.ToString());

                          chaveCampo = String.Format("{0}|{1}", agrupamentoNLConsumo, "NLLiquidacao");
                          preCache_NL_Consumo.InserirValor<string, string>(chaveCampo, nlLiquidacao);

                          chaveCampo = String.Format("{0}|{1}", agrupamentoNLConsumo, "NLReclassificacao");
                          preCache_NL_Consumo.InserirValor<string, string>(chaveCampo, nlReclassificacao);
                      });

            gerarCache_NumeroDocumento_ValorItemMovimentacao_NLConsumo__Por_MovimentoItemID(lstRetorno);
            gerarCache_StatusMovimentacoes(preCache_NL_Consumo, true);
            gerarLotesParticionadosDeNLConsumoParaPagamento(preCache_NL_Consumo);
            return preCache_NL_Consumo;
        }


        public bool StatusMovimentacao_Por_MovimentoItem(IList<string> listaMovimentoItemIDs)
        {
            IDictionary<string, string> chaves_Status_Por_NumeroDocumento = null;
            bool statusMovimentacao = true;

            chaves_Status_Por_NumeroDocumento = gerarCache_StatusMovimentacoes(listaMovimentoItemIDs, true);
            chaves_Status_Por_NumeroDocumento.Values
                                             .ToList()
                                             .ForEach(statusMovimentacaoConsulta => statusMovimentacao &= bool.Parse(statusMovimentacaoConsulta));

            return statusMovimentacao;
        }

        private IDictionary<string, string> gerarCache_NumeroDocumento_ValorItemMovimentacao_NLConsumo__Por_MovimentoItemID(IList<PTResMensalEntity> lstRetorno, bool insereEmCache = true)
        {
            IDictionary<string, string> dbCaching_MovimentoItemID_x_NumeroDocumento = null;
            string chaveInsercao = null;
            string valorInsercao = null;
            string fmtChaveInsercao_MovimentoItemID_NumeroDocumento = null;
            string fmtChaveValor_NumeroDocumento_ValorMovimentoItem = null;

            fmtChaveInsercao_MovimentoItemID_NumeroDocumento = "TB_MOVIMENTO_ITEM__id: {0}";
            fmtChaveValor_NumeroDocumento_ValorMovimentoItem = "NumeroDocumento:{0}|ValorMovItem:{1}|NLConsumo:{2}";
            dbCaching_MovimentoItemID_x_NumeroDocumento = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            lstRetorno.ToList()
                      .ForEach(preNLConsumo =>
                      {
                          chaveInsercao = String.Format(fmtChaveInsercao_MovimentoItemID_NumeroDocumento, preNLConsumo.MovItemID);
                          valorInsercao = String.Format(fmtChaveValor_NumeroDocumento_ValorMovimentoItem, preNLConsumo.DocumentoRelacionado, preNLConsumo.Valor, (!String.IsNullOrWhiteSpace(preNLConsumo.NlLancamento) ? preNLConsumo.NlLancamento : "NULL"));
                          dbCaching_MovimentoItemID_x_NumeroDocumento.InserirValor<string, string>(chaveInsercao, valorInsercao);

                          chaveInsercao = valorInsercao = null;
                      });



            if (insereEmCache)
                dbCaching_MovimentoItemID_x_NumeroDocumento.Keys.ToList().ForEach(chaveRegistro => preCache_NL_Consumo.Add(chaveRegistro, dbCaching_MovimentoItemID_x_NumeroDocumento[chaveRegistro]));


            return dbCaching_MovimentoItemID_x_NumeroDocumento;
        }
        private IDictionary<string, string> gerarCache_StatusMovimentacoes(IList<string> listaMovimentoItemIDs, bool insereEmCache = false)
        {
            IDictionary<string, string> dbCaching_DocumentoSAM_x_StatusAtivo = null;
            MovimentoItemEntity itemMovimentacaoMaterial = null;
            string chaveInsercao = null;
            string fmtChaveInsercao_NumeroDocumento_StatusMovimentacao = null;



            fmtChaveInsercao_NumeroDocumento_StatusMovimentacao = "StatusNumeroDocumento|{0}";
            dbCaching_DocumentoSAM_x_StatusAtivo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            listaMovimentoItemIDs.Distinct()
                                 .ToList()
                                 .ForEach(itemMovimentacaoID =>
                                 {
                                     itemMovimentacaoMaterial = base.ObterEntidade<MovimentoItemEntity>(itemMovimentacaoID);
                                     chaveInsercao = String.Format(fmtChaveInsercao_NumeroDocumento_StatusMovimentacao, itemMovimentacaoMaterial.Movimento.NumeroDocumento);
                                     dbCaching_DocumentoSAM_x_StatusAtivo.InserirValor<string, string>(chaveInsercao, itemMovimentacaoMaterial.Movimento.Ativo.ToString());
                                 });

            if (insereEmCache)
                dbCaching_DocumentoSAM_x_StatusAtivo.Keys.ToList().ForEach(chaveRegistro => preCache_NL_Consumo.InserirValor(chaveRegistro, dbCaching_DocumentoSAM_x_StatusAtivo[chaveRegistro]));


            return dbCaching_DocumentoSAM_x_StatusAtivo;
        }
        private IDictionary<string, string> gerarCache_StatusMovimentacoes(IDictionary<string, string> cacheNLsConsumo, bool insereEmCache = false)
        {
            IDictionary<string, string> dbCaching_DocumentoSAM_x_StatusAtivo = null;
            MovimentoItemEntity itemMovimentacaoMaterial = null;
            string chaveInsercao = null;
            string fmtChaveInsercao_NumeroDocumento_StatusMovimentacao = null;



            fmtChaveInsercao_NumeroDocumento_StatusMovimentacao = "StatusNumeroDocumento|{0}";
            dbCaching_DocumentoSAM_x_StatusAtivo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            cacheNLsConsumo.Keys
                           .Where(chaveConsulta => chaveConsulta.Contains("MovimentoItemIDs"))
                           .Select(chaveConsulta => cacheNLsConsumo[chaveConsulta].BreakLine())
                           .ToList()
                           .ForEach(itemMovimentacaoIDs =>
                           {
                               foreach (var itemMovimentacaoID in itemMovimentacaoIDs)
                               {
                                   itemMovimentacaoMaterial = base.ObterEntidade<MovimentoItemEntity>(Int32.Parse(itemMovimentacaoID));
                                   chaveInsercao = String.Format(fmtChaveInsercao_NumeroDocumento_StatusMovimentacao, itemMovimentacaoMaterial.Movimento.NumeroDocumento);
                                   dbCaching_DocumentoSAM_x_StatusAtivo.InserirValor<string, string>(chaveInsercao, itemMovimentacaoMaterial.Movimento.Ativo.ToString());
                               }
                           });

            if (insereEmCache)
                dbCaching_DocumentoSAM_x_StatusAtivo.Keys.ToList().ForEach(chaveRegistro => preCache_NL_Consumo.InserirValor(chaveRegistro, dbCaching_DocumentoSAM_x_StatusAtivo[chaveRegistro]));

            return dbCaching_DocumentoSAM_x_StatusAtivo;
        }


        private IDictionary<string, string> gerarCache_NLConsumo_Por_MovimentoItem(IList<string> listaMovimentoItemIDs, bool insereEmCache = false)
        {
            IDictionary<string, string> dbCaching_MovimentoItemID_x_NLConsumo = null;
            MovimentoItemEntity itemMovimentacaoMaterial = null;
            string chaveInsercao = null;
            string fmtChaveInsercao_MovimentoItemID_NLConsumo = null;



            fmtChaveInsercao_MovimentoItemID_NLConsumo = "TB_MOVIMENTO_ITEM__id:{0}|NLConsumo:{1}";
            dbCaching_MovimentoItemID_x_NLConsumo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            listaMovimentoItemIDs.Distinct()
                                 .ToList()
                                 .ForEach(itemMovimentacaoID =>
                                 {
                                     itemMovimentacaoMaterial = base.ObterEntidade<MovimentoItemEntity>(itemMovimentacaoID);
                                     chaveInsercao = String.Format(fmtChaveInsercao_MovimentoItemID_NLConsumo, itemMovimentacaoID, itemMovimentacaoMaterial.NL_Consumo);
                                     dbCaching_MovimentoItemID_x_NLConsumo.InserirValor<string, string>(chaveInsercao, itemMovimentacaoMaterial.Movimento.Ativo.ToString());
                                 });

            if (insereEmCache)
                dbCaching_MovimentoItemID_x_NLConsumo.Keys.ToList().ForEach(chaveRegistro => preCache_NL_Consumo.InserirValor(chaveRegistro, dbCaching_MovimentoItemID_x_NLConsumo[chaveRegistro]));


            return dbCaching_MovimentoItemID_x_NLConsumo;
        }
        private IDictionary<string, string> gerarCache_NLConsumo_Por_MovimentoItem(IDictionary<string, string> cacheNLsConsumo, bool insereEmCache = false)
        {
            IDictionary<string, string> dbCaching_MovimentoItemID_x_NLConsumo = null;
            MovimentoItemEntity itemMovimentacaoMaterial = null;
            string chaveInsercao = null;
            string fmtChaveInsercao_MovimentoItemID_NLConsumo = null;



            fmtChaveInsercao_MovimentoItemID_NLConsumo = "TB_MOVIMENTO_ITEM__id:{0}|NLConsumo:{1}";
            dbCaching_MovimentoItemID_x_NLConsumo = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            cacheNLsConsumo.Keys
                           .Where(chaveConsulta => chaveConsulta.Contains("MovimentoItemIDs"))
                           .Select(chaveConsulta => cacheNLsConsumo[chaveConsulta].BreakLine())
                           .ToList()
                           .ForEach(itemMovimentacaoIDs =>
                           {
                               foreach (var itemMovimentacaoID in itemMovimentacaoIDs)
                               {
                                   itemMovimentacaoMaterial = base.ObterEntidade<MovimentoItemEntity>(itemMovimentacaoID);
                                   chaveInsercao = String.Format(fmtChaveInsercao_MovimentoItemID_NLConsumo, itemMovimentacaoID, itemMovimentacaoMaterial.NL_Consumo);
                                   dbCaching_MovimentoItemID_x_NLConsumo.InserirValor<string, string>(chaveInsercao, itemMovimentacaoMaterial.Movimento.Ativo.ToString());
                               }
                           });

            if (insereEmCache)
                dbCaching_MovimentoItemID_x_NLConsumo.Keys.ToList().ForEach(chaveRegistro => preCache_NL_Consumo.InserirValor(chaveRegistro, dbCaching_MovimentoItemID_x_NLConsumo[chaveRegistro]));

            return dbCaching_MovimentoItemID_x_NLConsumo;
        }

        private void gerarLotesParticionadosDeNLConsumoParaPagamento(IDictionary<string, string> cacheNLsConsumo)
        {
            SortedList<string, string> miniCache_NumeroDocumentoSAM_MovItemIDs = null;
            SortedList<string, string> miniCache_MovItemNLConsumo = null;
            SortedList<string, string> miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs = null;
            SortedList<string, SortedList<string, string>> objPreProcessamentoCache = null;
            IList<IList<KeyValuePair<string, string>>> LotesDeN_DocsSAM = null;


            string chaveNomeCampo;
            string chaveAgrupamento;
            string agrupamentoDocumentoSAM;
            string agrupamentoMovItemIDs;
            string _agrupamentoMovItemIDs = null;
            string tmpNLConsumo = null;
            string[] movItemIDs;
            string nlLiquidacao;
            string nlReclassificacao;
            int contadorLote = 0;
            string historicoListagemMovItemID = null;


            var chavesConsulta = cacheNLsConsumo.Keys
                                                .Where(chaveConsultaCampo => chaveConsultaCampo.Contains(nomeCampoMovItemIDs))
                                                .Select(chaveConsultaCampo => chaveConsultaCampo)
                                                .ToList();

            miniCache_NumeroDocumentoSAM_MovItemIDs = new SortedList<string, string>();
            miniCache_MovItemNLConsumo = new SortedList<string, string>();



            foreach (var chaveAgrupamentoNLConsumo in chavesConsulta)
            {
                chaveAgrupamento = chaveAgrupamentoNLConsumo.BreakLine(nomeCampoMovItemIDs, 0);
                chaveNomeCampo = String.Format(fmtChaveConsultaGenerico, chaveAgrupamento, nomeCampoMovItemIDs);
                movItemIDs = cacheNLsConsumo[chaveNomeCampo].BreakLine();

                chaveNomeCampo = String.Format(fmtChaveConsultaGenerico, chaveAgrupamento, nomeCampoNLLiquidacao);
                nlLiquidacao = cacheNLsConsumo[chaveNomeCampo];

                chaveNomeCampo = String.Format(fmtChaveConsultaGenerico, chaveAgrupamento, nomeCampoNLReclassificacao);
                nlReclassificacao = cacheNLsConsumo[chaveNomeCampo];

                objPreProcessamentoCache = preProcessamentoParaGerarLotesParticionados(preCache_NL_Consumo, movItemIDs);
                miniCache_NumeroDocumentoSAM_MovItemIDs = objPreProcessamentoCache["Cache_NumeroDocumentoSAM_MovItemIDs"];
                miniCache_MovItemNLConsumo = objPreProcessamentoCache["Cache_MovItemNLConsumo"];

                LotesDeN_DocsSAM = miniCache_NumeroDocumentoSAM_MovItemIDs.Cast<KeyValuePair<string, string>>().ToArray().ParticionadorLista(Constante.CST_NUMERO_MAXIMO_DOCUMENTOS_TRANSACAO_SIAFEM_NL);
                miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs = new SortedList<string, string>();


                List<string> chavesSubAgrupamento = new List<string>();
                foreach (var loteN_Docs in LotesDeN_DocsSAM)
                {
                    //contadorLote = 0;
                    tmpNLConsumo = null;

                    agrupamentoDocumentoSAM = String.Join("\n", loteN_Docs.Select(parChaveValor => parChaveValor.Key).ToArray());
                    agrupamentoMovItemIDs = String.Join("\n", loteN_Docs.Select(parChaveValor => parChaveValor.Value.BreakLine("|", 0)).ToArray());
                    agrupamentoMovItemIDs.BreakLine()
                                            .ToList()
                                            .ForEach(movItem =>
                                            {
                                                miniCache_MovItemNLConsumo.TryGetValue(movItem, out tmpNLConsumo);
                                                if (!String.IsNullOrWhiteSpace(tmpNLConsumo))
                                                {
                                                    miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs.TryGetValue(tmpNLConsumo, out historicoListagemMovItemID);
                                                    if (!String.IsNullOrWhiteSpace(historicoListagemMovItemID) && historicoListagemMovItemID.Contains(movItem))
                                                        historicoListagemMovItemID.Replace(movItem, null);

                                                    miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs.InserirValor<string, string>(tmpNLConsumo, String.Format("{0}\n{1}", movItem, historicoListagemMovItemID));
                                                    historicoListagemMovItemID = null;
                                                }
                                            });

                    chavesSubAgrupamento = miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs.Keys.ToList();
                    //Se agrupamento já possui alguma(s) NL(s) associada(s), quebra o agrupamento entre o que jah pago e nao-pago com NL Consumo
                    if (chavesSubAgrupamento.Count() > 1)
                    {
                        contadorLote = 0;

                        foreach (string chaveSubGrupo in chavesSubAgrupamento)
                        {
                            #region Limpeza
                            //Limpeza de chaves de agrupamentos com >14 doc's
                            limpezaEstruturaPreCache(preCache_NL_Consumo, chaveAgrupamento, contadorLote);
                            #endregion

                            _agrupamentoMovItemIDs = miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs[chaveSubGrupo];
                            _agrupamentoMovItemIDs.BreakLine()
                                                  .ToList()
                                                  .ForEach(movItem => processarMovimentoItemDeAgrupamento(preCache_NL_Consumo, movItem, chaveAgrupamento, contadorLote, chaveSubGrupo, nlLiquidacao, nlReclassificacao));
                            contadorLote++;
                        }

                        contadorLote = 0;
                    }
                    //Quebra o agrupamento que nao possui NL associada em lotes de 14doc's
                    else
                    {
                        #region Limpeza
                        //Limpeza de chaves de agrupamentos com >14 doc's
                        limpezaEstruturaPreCache(preCache_NL_Consumo, chaveAgrupamento, contadorLote);
                        #endregion

                        _agrupamentoMovItemIDs = agrupamentoMovItemIDs;
                        _agrupamentoMovItemIDs.BreakLine()
                                              .ToList()
                                              .ForEach(movItem => processarMovimentoItemDeAgrupamento(preCache_NL_Consumo, movItem, chaveAgrupamento, contadorLote, chavesSubAgrupamento[0], nlLiquidacao, nlReclassificacao));
                        contadorLote++;
                    }

                    //contadorLote = 0;
                }

                miniCache_NumeroDocumentoSAM_MovItemIDs = null;
                miniCache_NLConsumo_PorLote_NumeroDocumentoSAM_MovItemIDs = null;

                LotesDeN_DocsSAM = null;
                contadorLote = 0;
            }
        }

        private SortedList<string, SortedList<string, string>> preProcessamentoParaGerarLotesParticionados(IDictionary<string, string> estruturaPreCache, string[] arrMovimentoItemID)
        {
            SortedList<string, SortedList<string, string>> miniCache = null;
            SortedList<string, string> miniCache_NumeroDocumentoSAM_MovItemIDs = null;
            SortedList<string, string> miniCache_MovItemNLConsumo = null;
            string chaveConsulta = null;
            string numeroDocumentoSAM = null;
            string numeroNLConsumo = null;
            string historicoListagemMovItemID = null;
            string tmpNLConsumo = null;
            decimal valorMovItem = 0.00m;



            miniCache_NumeroDocumentoSAM_MovItemIDs = new SortedList<string, string>();
            miniCache_MovItemNLConsumo = new SortedList<string, string>();
            foreach (var rowMovItemID in arrMovimentoItemID)
            {
                chaveConsulta = String.Format(fmtChaveConsultaMovItem, rowMovItemID);

                numeroDocumentoSAM = estruturaPreCache[chaveConsulta].BreakLine("|", 0).BreakLine(":", 1);
                Decimal.TryParse(estruturaPreCache[chaveConsulta].BreakLine("|", 1).BreakLine(":", 1), out valorMovItem);
                numeroNLConsumo = estruturaPreCache[chaveConsulta].BreakLine("|", 2).BreakLine(":", 1);


                miniCache_NumeroDocumentoSAM_MovItemIDs.TryGetValue(numeroDocumentoSAM, out historicoListagemMovItemID);
                if (!String.IsNullOrWhiteSpace(historicoListagemMovItemID) && historicoListagemMovItemID.Contains(rowMovItemID))
                    historicoListagemMovItemID.Replace(rowMovItemID, null);

                if (!String.IsNullOrWhiteSpace(tmpNLConsumo) && tmpNLConsumo.Contains(numeroNLConsumo))
                    tmpNLConsumo.Replace(numeroNLConsumo, null);


                miniCache_NumeroDocumentoSAM_MovItemIDs.InserirValor<string, string>(numeroDocumentoSAM, String.Format("{0}\n{1}", rowMovItemID, historicoListagemMovItemID));
                miniCache_MovItemNLConsumo.InserirValor<string, string>(rowMovItemID, numeroNLConsumo);

                historicoListagemMovItemID = null;
            }

            miniCache = new SortedList<string, SortedList<string, string>>(StringComparer.InvariantCultureIgnoreCase);
            miniCache.Add("Cache_NumeroDocumentoSAM_MovItemIDs", miniCache_NumeroDocumentoSAM_MovItemIDs);
            miniCache.Add("Cache_MovItemNLConsumo", miniCache_MovItemNLConsumo);



            return miniCache;
        }

        private void inserirDadosEstruturaPreCache(IDictionary<string, string> estruturaPreCache, string chaveAgrupamento, int contadorLote, string listagemDocumentoSAM, string listagemMovimentoItemID, decimal historicoValor, string nlConsumo, string nlLiquidacao, string nlReclassificacao)
        {
            string chaveInsercao = null;

            //Documentos SAM Agrupamento
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNumeroDocumento);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, listagemDocumentoSAM);

            //MovimentoItemIDs Agrupamento
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoMovItemIDs);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, listagemMovimentoItemID);

            //Somatoria Valor (MovimentoItem) Agrupamento
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoValorNLConsumo);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, historicoValor.ToString());


            //NL(s) Consumo associada a agrupamento (caso exista)
            nlConsumo = (String.IsNullOrWhiteSpace(nlConsumo) ? "NULL" : nlConsumo);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLConsumo);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, nlConsumo);

            nlLiquidacao = (String.IsNullOrWhiteSpace(nlLiquidacao) ? "NULL" : nlLiquidacao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLLiquidacao);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, nlLiquidacao);

            nlReclassificacao = (String.IsNullOrWhiteSpace(nlReclassificacao) ? "NULL" : nlReclassificacao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLReclassificacao);
            estruturaPreCache.InserirValor<string, string>(chaveInsercao, nlReclassificacao);
        }
        private void limpezaEstruturaPreCache(IDictionary<string, string> estruturaPreCache, string chaveAgrupamento, int contadorLote)
        {
            string chaveInsercao = null;


            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoMovItemIDs);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoNumeroDocumento);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoValorNLConsumo);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoNLConsumo);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoNLLiquidacao);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}|{1}", chaveAgrupamento, nomeCampoNLReclassificacao);
            estruturaPreCache.Remove(chaveInsercao);


            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoMovItemIDs);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNumeroDocumento);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoValorNLConsumo);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLConsumo);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLLiquidacao);
            estruturaPreCache.Remove(chaveInsercao);
            chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNLReclassificacao);
            estruturaPreCache.Remove(chaveInsercao);
        }

        private void processarMovimentoItemDeAgrupamento(IDictionary<string, string> estruturaPreCache, string movItemID, string chaveAgrupamento, int contadorLote, string nlConsumo = null, string nlLiquidacao = null, string nlReclassificacao = null)
        {
            //Chaves
            string chaveInsercao = null;
            string chaveConsulta = null;
            //string chaveAgrupamento = null;

            //DOC's SAM Agrupamento
            string numeroDocumentoSAM = null;
            string historicoListagemDOC = null;
            string listagemDocumentoSAM = null;

            //MovimentoItemID's Agrupamento
            string historicoListagemMovItemID = null;
            string listagemMovimentoItemID = null;

            //Valor Agrupamento
            string strHistoricoValor = null;
            decimal valorMovItem = 0.00m;
            decimal historicoValor = 0.00m;


            if (!String.IsNullOrWhiteSpace(movItemID))
            {
                chaveConsulta = String.Format(fmtChaveConsultaMovItem, movItemID);
                numeroDocumentoSAM = estruturaPreCache[chaveConsulta].BreakLine("|", 0).BreakLine(":", 1);


                chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoValorNLConsumo);
                estruturaPreCache.TryGetValue(chaveInsercao, out strHistoricoValor);
                Decimal.TryParse(estruturaPreCache[chaveConsulta].BreakLine("|", 1).BreakLine(":", 1), out valorMovItem);
                Decimal.TryParse(strHistoricoValor, out historicoValor);
                if (historicoValor > 0.00m)
                    historicoValor += valorMovItem;
                else
                    historicoValor = valorMovItem;


                chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoNumeroDocumento);
                estruturaPreCache.TryGetValue(chaveInsercao, out historicoListagemDOC);
                numeroDocumentoSAM = estruturaPreCache[chaveConsulta].BreakLine("|", 0).BreakLine(":", 1);
                if (!String.IsNullOrWhiteSpace(historicoListagemDOC) && historicoListagemDOC.Contains(numeroDocumentoSAM))
                    historicoListagemDOC = historicoListagemDOC.Replace(String.Format("{0}\n", numeroDocumentoSAM), null);


                chaveInsercao = String.Format("{0}c{1}|{2}", chaveAgrupamento, contadorLote, nomeCampoMovItemIDs);
                estruturaPreCache.TryGetValue(chaveInsercao, out historicoListagemMovItemID);
                if (!String.IsNullOrWhiteSpace(historicoListagemMovItemID) && historicoListagemMovItemID.Contains(movItemID))
                    historicoListagemMovItemID.Replace(movItemID, null);



                listagemDocumentoSAM = String.Format("{0}\n{1}", numeroDocumentoSAM, historicoListagemDOC);
                listagemMovimentoItemID = String.Format("{0}\n{1}", movItemID, historicoListagemMovItemID);

                //Gravacao no Dictionary utilizado como estrutura de cache
                inserirDadosEstruturaPreCache(estruturaPreCache, chaveAgrupamento, contadorLote, listagemDocumentoSAM, listagemMovimentoItemID, historicoValor, nlConsumo, nlLiquidacao, nlReclassificacao);


                #region Limpeza Variaveis Loop
                chaveInsercao = null;
                historicoValor = 0.00m;
                valorMovItem = 0.00m;
                historicoListagemMovItemID = null;
                historicoListagemDOC = null;
                strHistoricoValor = null;

                listagemMovimentoItemID = null;
                listagemMovimentoItemID = null;
                #endregion
            }
        }

        private TB_MOVIMENTO obterMovimentacaoMaterialPorItemMovimentacao(int movimentoItemID)
        {
            TB_MOVIMENTO rowTabela;


            rowTabela = Db.TB_MOVIMENTOs.SelectMany(movimentacaoMaterial => movimentacaoMaterial.TB_MOVIMENTO_ITEMs.Cast<TB_MOVIMENTO_ITEM>())
                                        .Where(itemMovimentacao => itemMovimentacao.TB_MOVIMENTO_ITEM_ID == movimentoItemID)
                                        .Select(itemMovimentacao => itemMovimentacao.TB_MOVIMENTO)
                                        .FirstOrDefault();
            return rowTabela;
        }

        private IQueryable obterMassaDadosParaGeracaoNLConsumo(int gestorId, int anoMesRef, int? almoxId)
        {
            IQueryable qryRetorno = null;
            string strAnoMesRef = null;


            strAnoMesRef = anoMesRef.ToString();

            qryRetorno = (from mov in Db.TB_MOVIMENTOs
                          join movItem in Db.TB_MOVIMENTO_ITEMs on mov.TB_MOVIMENTO_ID equals movItem.TB_MOVIMENTO_ID into _left_movItem

                          from movItem in _left_movItem.DefaultIfEmpty()
                          join div in Db.TB_DIVISAOs on mov.TB_DIVISAO_ID equals div.TB_DIVISAO_ID into _left_div

                          from div in _left_div.DefaultIfEmpty()
                          join ua in Db.TB_UAs on div.TB_UA_ID equals ua.TB_UA_ID into _left_ua

                          from ua in _left_ua.DefaultIfEmpty()
                          join subItem in Db.TB_SUBITEM_MATERIALs on movItem.TB_SUBITEM_MATERIAL_ID equals subItem.TB_SUBITEM_MATERIAL_ID into _left_subItem

                          from subItem in _left_subItem.DefaultIfEmpty()
                          join natDespesa in Db.TB_NATUREZA_DESPESAs on subItem.TB_NATUREZA_DESPESA_ID equals natDespesa.TB_NATUREZA_DESPESA_ID into _left_natDespesa

                          from natDespesa in _left_natDespesa.DefaultIfEmpty()
                          join ptr in Db.TB_PTREs on movItem.TB_PTRES_ID equals ptr.TB_PTRES_ID into _left_ptr

                          from ptr in _left_ptr.DefaultIfEmpty()
                          join tipoMov in Db.TB_TIPO_MOVIMENTOs on mov.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov

                          from tipoMov in _left_tipoMov.DefaultIfEmpty()
                          join tipoMovAgr in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals tipoMovAgr.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID into _left_tipoMovAgr

                          from tipoMovAgr in _left_tipoMovAgr.DefaultIfEmpty()
                          join almox in Db.TB_ALMOXARIFADOs on mov.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID into _left_almox

                          from almox in _left_almox.DefaultIfEmpty()
                          join gestor in Db.TB_GESTORs on almox.TB_GESTOR_ID equals gestor.TB_GESTOR_ID into _left_gestor

                          //orderby ua.TB_UGE_ID, ptr.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO
                          orderby ua.TB_UGE_ID, ua.TB_UA_ID, movItem.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO, movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO

                          //Otimizacao consulta L2S 'syntax mode'
                          let ugeUA = ua.TB_UGE
                          let uoUgeUA = ugeUA.TB_UO
                          let gestorAlmox = almox.TB_GESTOR
                          let ugeAlmox = almox.TB_UGE
                          let ptresMovItem = movItem.TB_PTRE
                          let almoxMovimentacao = mov.TB_ALMOXARIFADO

                          where (almoxMovimentacao.TB_ALMOXARIFADO_ID == (almoxId == null ? almoxMovimentacao.TB_ALMOXARIFADO_ID : almoxId))
                          where (almox.TB_GESTOR_ID == gestorId)
                          where (mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == strAnoMesRef)
                          where (mov.TB_MOVIMENTO_ATIVO == true)
                          where (movItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                          //Filtro/trava por conta do caos com PTRES em 12/2014 --> Se 12/2014 trazer tudo com e sem PTRES, para calcular, caso contrario, 
                          //considerar apenas MovimentoItem que possua valor no TB_PTRES_ID, com os demais filtros
                          where ((anoMesRef == 201412) ? (movItem.TB_PTRES_ID == null || movItem.TB_PTRES_ID != null) : (movItem.TB_PTRES_ID != null))
                          where (tipoMov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.RequisicaoAprovada)
                          select new PTResMensalEntity
                          {
                              AnoMesRef = Int32.Parse(mov.TB_MOVIMENTO_ANO_MES_REFERENCIA),
                              Almoxarifado = new AlmoxarifadoEntity() { Id = mov.TB_ALMOXARIFADO_ID, Codigo = almoxMovimentacao.TB_ALMOXARIFADO_CODIGO, Descricao = almoxMovimentacao.TB_ALMOXARIFADO_DESCRICAO, MesRef = almoxMovimentacao.TB_ALMOXARIFADO_MES_REF, Gestor = new GestorEntity() { Id = gestorAlmox.TB_GESTOR_ID, NomeReduzido = gestorAlmox.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestorAlmox.TB_GESTOR_CODIGO_GESTAO }, Uge = new UGEEntity() { Id = ugeAlmox.TB_UGE_ID, Codigo = ugeAlmox.TB_UGE_CODIGO, Descricao = ugeAlmox.TB_UGE_DESCRICAO } },
                              UgeAlmoxarifado = new UGEEntity() { Id = ugeAlmox.TB_UGE_ID, Codigo = ugeAlmox.TB_UGE_CODIGO, Descricao = ugeAlmox.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D6} - {1}", ugeAlmox.TB_UGE_CODIGO, ugeAlmox.TB_UGE_DESCRICAO) },
                              UGE = new UGEEntity() { Id = ugeUA.TB_UGE_ID, Codigo = ugeUA.TB_UGE_CODIGO, Descricao = ugeUA.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D7} - {1}", ugeUA.TB_UGE_CODIGO, ugeUA.TB_UGE_DESCRICAO) },
                              UA = new UAEntity() { Id = ua.TB_UA_ID, Codigo = ua.TB_UA_CODIGO, Descricao = ua.TB_UA_DESCRICAO, CodigoDescricao = String.Format("{0:D7} - {1}", ua.TB_UA_CODIGO, ua.TB_UA_DESCRICAO), Uge = ((ugeUA.IsNotNull()) ? (new UGEEntity() { Id = ugeUA.TB_UGE_ID, Codigo = ugeUA.TB_UGE_CODIGO, Descricao = ugeUA.TB_UGE_DESCRICAO }) : null) },
                              UO = new UOEntity() { Id = uoUgeUA.TB_UO_ID, Codigo = uoUgeUA.TB_UO_CODIGO, CodigoDescricao = String.Format("{0} - {1}", uoUgeUA.TB_UO_CODIGO.ToString(), uoUgeUA.TB_UO_DESCRICAO), Descricao = uoUgeUA.TB_UO_DESCRICAO },
                              //PtRes = new PTResEntity() { Id = ptresMovItem.TB_PTRES_ID, Codigo = ptresMovItem.TB_PTRES_CODIGO, Descricao = ptresMovItem.TB_PTRES_DESCRICAO },
                              PtRes = (ptresMovItem.IsNotNull() ? new PTResEntity() { Id = ptresMovItem.TB_PTRES_ID, Codigo = ptresMovItem.TB_PTRES_CODIGO, Descricao = ptresMovItem.TB_PTRES_DESCRICAO } : new PTResEntity() { Codigo = movItem.TB_PTRES_CODIGO }),
                              NaturezaDespesa = new NaturezaDespesaEntity() { Id = natDespesa.TB_NATUREZA_DESPESA_ID, Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO, Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO },
                              Gestor = new GestorEntity() { Id = gestorAlmox.TB_GESTOR_ID, NomeReduzido = gestorAlmox.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestorAlmox.TB_GESTOR_CODIGO_GESTAO },
                              Valor = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                              DocumentoRelacionado = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                              NlLancamento = movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO,
                              MovItemID = movItem.TB_MOVIMENTO_ITEM_ID,

                              UgeAlmoxarifado_Codigo = ugeAlmox.TB_UGE_CODIGO,
                              UgeAlmoxarifado_Descricao = ugeAlmox.TB_UGE_DESCRICAO,
                              Uge_Codigo = ugeUA.TB_UGE_CODIGO,
                              Uge_Descricao = ugeUA.TB_UGE_DESCRICAO,
                              Uo_Codigo = uoUgeUA.TB_UO_CODIGO,
                              Uo_Descricao = uoUgeUA.TB_UO_DESCRICAO,
                              Ua_Codigo = ua.TB_UA_CODIGO,
                              Ua_Descricao = ua.TB_UA_DESCRICAO,
                              NaturezaDespesa_Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO,
                              NaturezaDespesa_Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                              PtRes_Codigo = ptresMovItem.TB_PTRES_CODIGO,

                              NL_Liquidacao = movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                              NL_Reclassificacao = movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO

                          }).AsQueryable();


            return qryRetorno;
        }

        private IQueryable obterMassaDadosParaGeracaoNLConsumoImediatoPorAlmox(int almoxID, int anoMesRef)
        {
            IQueryable qryRetorno = null;
            string strAnoMesRef = null;


            strAnoMesRef = anoMesRef.ToString();

            qryRetorno = (from mov in Db.TB_MOVIMENTOs
                          join movItem in Db.TB_MOVIMENTO_ITEMs on mov.TB_MOVIMENTO_ID equals movItem.TB_MOVIMENTO_ID into _left_movItem

                          //from movItem in _left_movItem.DefaultIfEmpty()
                          //join div in Db.TB_DIVISAOs on mov.TB_DIVISAO_ID equals div.TB_DIVISAO_ID into _left_div

                          from movItem in _left_movItem.DefaultIfEmpty()
                          join ua in Db.TB_UAs on mov.TB_UA_ID equals ua.TB_UA_ID into _left_ua

                          from ua in _left_ua.DefaultIfEmpty()
                          join subItem in Db.TB_SUBITEM_MATERIALs on movItem.TB_SUBITEM_MATERIAL_ID equals subItem.TB_SUBITEM_MATERIAL_ID into _left_subItem

                          from subItem in _left_subItem.DefaultIfEmpty()
                          join natDespesa in Db.TB_NATUREZA_DESPESAs on subItem.TB_NATUREZA_DESPESA_ID equals natDespesa.TB_NATUREZA_DESPESA_ID into _left_natDespesa

                          //from natDespesa in _left_natDespesa.DefaultIfEmpty()
                          //join ptr in Db.TB_PTREs on movItem.TB_PTRES_ID equals ptr.TB_PTRES_ID into _left_ptr

                          from natDespesa in _left_natDespesa.DefaultIfEmpty()
                          join tipoMov in Db.TB_TIPO_MOVIMENTOs on mov.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov

                          from tipoMov in _left_tipoMov.DefaultIfEmpty()
                          join tipoMovAgr in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals tipoMovAgr.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID into _left_tipoMovAgr

                          from tipoMovAgr in _left_tipoMovAgr.DefaultIfEmpty()
                          join almox in Db.TB_ALMOXARIFADOs on mov.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID into _left_almox

                          from almox in _left_almox.DefaultIfEmpty()
                          join gestor in Db.TB_GESTORs on almox.TB_GESTOR_ID equals gestor.TB_GESTOR_ID into _left_gestor

                          //orderby ua.TB_UGE_ID, ptr.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO
                          orderby ua.TB_UGE_ID, ua.TB_UA_ID, movItem.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO, movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO

                          //Otimizacao consulta L2S 'syntax mode'
                          let ugeUA = ua.TB_UGE
                          let uoUgeUA = ugeUA.TB_UO
                          let gestorAlmox = almox.TB_GESTOR
                          let ugeAlmox = almox.TB_UGE
                          let ptresMovItem = movItem.TB_PTRE
                          let almoxMovimentacao = mov.TB_ALMOXARIFADO

                          where (almoxMovimentacao.TB_ALMOXARIFADO_ID == almoxID)
                          where (mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == strAnoMesRef)
                          where (mov.TB_MOVIMENTO_ATIVO == true)
                          where (movItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                          //Filtro/trava por conta do caos com PTRES em 12/2014 --> Se 12/2014 trazer tudo com e sem PTRES, para calcular, caso contrario, 
                          //considerar apenas MovimentoItem que possua valor no TB_PTRES_ID, com os demais filtros
                          //where ((anoMesRef == 201412) ? (movItem.TB_PTRES_ID == null || movItem.TB_PTRES_ID != null) : (movItem.TB_PTRES_ID != null))
                          where ((anoMesRef == 201412) ? (movItem.TB_PTRES_ID == null || movItem.TB_PTRES_ID != null) : 0 == 0)
                          where (tipoMov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho || tipoMov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
                          where (tipoMovAgr.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato)
                          select new PTResMensalEntity
                          {
                              AnoMesRef = Int32.Parse(mov.TB_MOVIMENTO_ANO_MES_REFERENCIA),
                              Almoxarifado = new AlmoxarifadoEntity() { Id = mov.TB_ALMOXARIFADO_ID, Codigo = almoxMovimentacao.TB_ALMOXARIFADO_CODIGO, Descricao = almoxMovimentacao.TB_ALMOXARIFADO_DESCRICAO, MesRef = almoxMovimentacao.TB_ALMOXARIFADO_MES_REF, Gestor = new GestorEntity() { Id = gestorAlmox.TB_GESTOR_ID, NomeReduzido = gestorAlmox.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestorAlmox.TB_GESTOR_CODIGO_GESTAO }, Uge = new UGEEntity() { Id = ugeAlmox.TB_UGE_ID, Codigo = ugeAlmox.TB_UGE_CODIGO, Descricao = ugeAlmox.TB_UGE_DESCRICAO } },
                              UgeAlmoxarifado = new UGEEntity() { Id = ugeAlmox.TB_UGE_ID, Codigo = ugeAlmox.TB_UGE_CODIGO, Descricao = ugeAlmox.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D6} - {1}", ugeAlmox.TB_UGE_CODIGO, ugeAlmox.TB_UGE_DESCRICAO) },
                              UGE = new UGEEntity() { Id = ugeUA.TB_UGE_ID, Codigo = ugeUA.TB_UGE_CODIGO, Descricao = ugeUA.TB_UGE_DESCRICAO, CodigoDescricao = String.Format("{0:D7} - {1}", ugeUA.TB_UGE_CODIGO, ugeUA.TB_UGE_DESCRICAO) },
                              UA = new UAEntity() { Id = ua.TB_UA_ID, Codigo = ua.TB_UA_CODIGO, Descricao = ua.TB_UA_DESCRICAO, CodigoDescricao = String.Format("{0:D7} - {1}", ua.TB_UA_CODIGO, ua.TB_UA_DESCRICAO), Uge = ((ugeUA.IsNotNull()) ? (new UGEEntity() { Id = ugeUA.TB_UGE_ID, Codigo = ugeUA.TB_UGE_CODIGO, Descricao = ugeUA.TB_UGE_DESCRICAO }) : null) },
                              UO = new UOEntity() { Id = uoUgeUA.TB_UO_ID, Codigo = uoUgeUA.TB_UO_CODIGO, CodigoDescricao = String.Format("{0} - {1}", uoUgeUA.TB_UO_CODIGO.ToString(), uoUgeUA.TB_UO_DESCRICAO), Descricao = uoUgeUA.TB_UO_DESCRICAO },
                              //PtRes = new PTResEntity() { Id = ptresMovItem.TB_PTRES_ID, Codigo = ptresMovItem.TB_PTRES_CODIGO, Descricao = ptresMovItem.TB_PTRES_DESCRICAO },
                              PtRes = (ptresMovItem.IsNotNull() ? new PTResEntity() { Id = ptresMovItem.TB_PTRES_ID, Codigo = ptresMovItem.TB_PTRES_CODIGO, Descricao = ptresMovItem.TB_PTRES_DESCRICAO } : new PTResEntity() { Codigo = movItem.TB_PTRES_CODIGO }),
                              NaturezaDespesa = new NaturezaDespesaEntity() { Id = natDespesa.TB_NATUREZA_DESPESA_ID, Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO, Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO },
                              Gestor = new GestorEntity() { Id = gestorAlmox.TB_GESTOR_ID, NomeReduzido = gestorAlmox.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestorAlmox.TB_GESTOR_CODIGO_GESTAO },
                              Valor = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                              DocumentoRelacionado = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                              NlLancamento = movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO,
                              MovItemID = movItem.TB_MOVIMENTO_ITEM_ID,

                              UgeAlmoxarifado_Codigo = ugeAlmox.TB_UGE_CODIGO,
                              UgeAlmoxarifado_Descricao = ugeAlmox.TB_UGE_DESCRICAO,
                              Uge_Codigo = ugeUA.TB_UGE_CODIGO,
                              Uge_Descricao = ugeUA.TB_UGE_DESCRICAO,
                              Uo_Codigo = uoUgeUA.TB_UO_CODIGO,
                              Uo_Descricao = uoUgeUA.TB_UO_DESCRICAO,
                              Ua_Codigo = ua.TB_UA_CODIGO,
                              Ua_Descricao = ua.TB_UA_DESCRICAO,
                              NaturezaDespesa_Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO,
                              NaturezaDespesa_Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                              PtRes_Codigo = movItem.TB_PTRES_CODIGO,

                              NL_Liquidacao = movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                              NL_Reclassificacao = movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO

                          }).AsQueryable();


            return qryRetorno;
        }

        private IQueryable obterMassaDadosParaGeracaoNLConsumoImediatoPorOrgao(int orgaoId, int anoMesRef)
        {
            IQueryable qryRetorno = null;
            string strAnoMesRef = null;


            strAnoMesRef = anoMesRef.ToString();

            qryRetorno = (from mov in Db.TB_MOVIMENTOs
                          join movItem in Db.TB_MOVIMENTO_ITEMs on mov.TB_MOVIMENTO_ID equals movItem.TB_MOVIMENTO_ID into _left_movItem

                          from movItem in _left_movItem.DefaultIfEmpty()
                          join div in Db.TB_DIVISAOs on mov.TB_DIVISAO_ID equals div.TB_DIVISAO_ID into _left_div

                          from div in _left_div.DefaultIfEmpty()
                          join ua in Db.TB_UAs on div.TB_UA_ID equals ua.TB_UA_ID into _left_ua

                          from ua in _left_ua.DefaultIfEmpty()
                          join subItem in Db.TB_SUBITEM_MATERIALs on movItem.TB_SUBITEM_MATERIAL_ID equals subItem.TB_SUBITEM_MATERIAL_ID into _left_subItem

                          from subItem in _left_subItem.DefaultIfEmpty()
                          join natDespesa in Db.TB_NATUREZA_DESPESAs on subItem.TB_NATUREZA_DESPESA_ID equals natDespesa.TB_NATUREZA_DESPESA_ID into _left_natDespesa

                          //from natDespesa in _left_natDespesa.DefaultIfEmpty()
                          //join ptr in Db.TB_PTREs on movItem.TB_PTRES_ID equals ptr.TB_PTRES_ID into _left_ptr

                          from natDespesa in _left_natDespesa.DefaultIfEmpty()
                          join tipoMov in Db.TB_TIPO_MOVIMENTOs on mov.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID into _left_tipoMov

                          from tipoMov in _left_tipoMov.DefaultIfEmpty()
                          join tipoMovAgr in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals tipoMovAgr.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID into _left_tipoMovAgr

                          from tipoMovAgr in _left_tipoMovAgr.DefaultIfEmpty()
                          join almox in Db.TB_ALMOXARIFADOs on mov.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID into _left_almox

                          from almox in _left_almox.DefaultIfEmpty()
                          join gestor in Db.TB_GESTORs on almox.TB_GESTOR_ID equals gestor.TB_GESTOR_ID into _left_gestor

                          //orderby ua.TB_UGE_ID, ptr.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO
                          orderby ua.TB_UGE_ID, ua.TB_UA_ID, movItem.TB_PTRES_CODIGO, natDespesa.TB_NATUREZA_DESPESA_CODIGO, movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO

                          //Otimizacao consulta L2S 'syntax mode'
                          let ugeUA = ua.TB_UGE
                          let uoUgeUA = ugeUA.TB_UO
                          let gestorAlmox = almox.TB_GESTOR
                          let ugeAlmox = almox.TB_UGE
                          let ptresMovItem = movItem.TB_PTRE
                          let almoxMovimentacao = mov.TB_ALMOXARIFADO


                          where (mov.TB_MOVIMENTO_ANO_MES_REFERENCIA == strAnoMesRef)
                          where (mov.TB_MOVIMENTO_ATIVO == true)
                          where (movItem.TB_MOVIMENTO_ITEM_ATIVO == true)
                          where (gestorAlmox.TB_ORGAO_ID == orgaoId)
                          //Filtro/trava por conta do caos com PTRES em 12/2014 --> Se 12/2014 trazer tudo com e sem PTRES, para calcular, caso contrario, 
                          //considerar apenas MovimentoItem que possua valor no TB_PTRES_ID, com os demais filtros
                          //where ((anoMesRef == 201412) ? (movItem.TB_PTRES_ID == null || movItem.TB_PTRES_ID != null) : (movItem.TB_PTRES_ID != null))
                          where (tipoMov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.ConsumoImediatoEmpenho || tipoMov.TB_TIPO_MOVIMENTO_ID == (int)GeralEnum.TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
                          where (tipoMovAgr.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato)
                          select new PTResMensalEntity
                          {
                              AnoMesRef = Int32.Parse(mov.TB_MOVIMENTO_ANO_MES_REFERENCIA),
                              UA = new UAEntity() { Codigo = ua.TB_UA_CODIGO },
                              //PtRes = (ptresMovItem.IsNotNull() ? new PTResEntity() { Codigo = ptresMovItem.TB_PTRES_CODIGO } : new PTResEntity() { Codigo = 2222 }),
                              PtRes = (movItem.TB_PTRES_CODIGO.IsNotNull() ? new PTResEntity() { Codigo = movItem.TB_PTRES_CODIGO } : new PTResEntity() { Codigo = 0 }),
                              NaturezaDespesa = new NaturezaDespesaEntity() { Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO },
                              Gestor = new GestorEntity() { Id = gestorAlmox.TB_GESTOR_ID, NomeReduzido = gestorAlmox.TB_GESTOR_NOME_REDUZIDO, CodigoGestao = gestorAlmox.TB_GESTOR_CODIGO_GESTAO },
                              Valor = movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                              DocumentoRelacionado = mov.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                              NlLancamento = movItem.TB_MOVIMENTO_ITEM_NL_CONSUMO,
                              MovItemID = movItem.TB_MOVIMENTO_ITEM_ID,

                              UgeAlmoxarifado_Codigo = ugeAlmox.TB_UGE_CODIGO,
                              UgeAlmoxarifado_Descricao = ugeAlmox.TB_UGE_DESCRICAO,
                              Uge_Codigo = ugeUA.TB_UGE_CODIGO,
                              Uge_Descricao = ugeUA.TB_UGE_DESCRICAO,
                              Uo_Codigo = (uoUgeUA == null ? 0 : uoUgeUA.TB_UO_CODIGO),
                              Uo_Descricao = (uoUgeUA == null ? "" : uoUgeUA.TB_UO_DESCRICAO),
                              Ua_Codigo = ua.TB_UA_CODIGO,
                              Ua_Descricao = ua.TB_UA_DESCRICAO,
                              NaturezaDespesa_Codigo = natDespesa.TB_NATUREZA_DESPESA_CODIGO,
                              NaturezaDespesa_Descricao = natDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                              PtRes_Codigo = movItem.TB_PTRES_CODIGO.IsNotNull() ? movItem.TB_PTRES_CODIGO : 0,

                              NL_Liquidacao = movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                              NL_Reclassificacao = movItem.TB_MOVIMENTO_ITEM_NL_RECLASSIFICACAO

                          }).AsQueryable();


            return qryRetorno;
        }

        public PTResMensalEntity getRetornaPtResMensalParaConsumo(int almoxarifadoId, int anoMesReferencia, int uaId, int ptResId, int[] naturezaDespesa, decimal ptResMensalValor)
        {
            IQueryable<PTResMensalEntity> query = (from q in Db.TB_PTRES_MENSALs
                                                   where q.TB_ALMOXARIFADO_ID == almoxarifadoId
                                                     && q.TB_PTRES_MENSAL_ANO_MES_REF == anoMesReferencia
                                                     && naturezaDespesa.Contains(q.TB_NATUREZA_DESPESA_ID.Value)
                                                     && q.TB_PTRES_MENSAL_NL != null && q.TB_PTRES_MENSAL_NL != string.Empty
                                                     && q.TB_PTRES_MENSAL_RETORNO != null && q.TB_PTRES_MENSAL_RETORNO != string.Empty
                                                     && q.TB_PTRES_MENSAL_FLAG_LANC != 'E' && q.TB_PTRES_MENSAL_FLAG_LANC != ' '
                                                   select new PTResMensalEntity
                                                   {
                                                       Id = q.TB_PTRES_MENSAL_ID,
                                                       Almoxarifado = new AlmoxarifadoEntity(almoxarifadoId),
                                                       AnoMesRef = anoMesReferencia
                                                   }).AsQueryable();

            return query.FirstOrDefault();
        }

        private Func<TB_PTRES_MENSAL, PTResMensalEntity> _instanciadorDTOPTResMensal()
        {
            Func<TB_PTRES_MENSAL, PTResMensalEntity> _actionSeletor = null;

            _actionSeletor = (PTResMensal => new PTResMensalEntity()
            {
                Id = PTResMensal.TB_PTRES_MENSAL_ID,
                AnoMesRef = PTResMensal.TB_PTRES_MENSAL_ANO_MES_REF,
                Almoxarifado = base.ObterEntidade<AlmoxarifadoEntity>(PTResMensal.TB_ALMOXARIFADO_ID),
                FlagLancamento = PTResMensal.TB_PTRES_MENSAL_FLAG_LANC,
                TipoLancamento = PTResMensal.TB_PTRES_MENSAL_TIPO_LANC,


                UA = base.ObterEntidade<UAEntity>(PTResMensal.TB_UA_ID),
                UO = base.ObterEntidade<UOEntity>(PTResMensal.TB_ALMOXARIFADO.TB_UGE.TB_UO.TB_UO_ID),
                PtRes = base.ObterEntidade<PTResEntity>(PTResMensal.TB_PTRES_ID),
                NaturezaDespesa = base.ObterEntidade<NaturezaDespesaEntity>(PTResMensal.TB_NATUREZA_DESPESA_ID.Value),

                UGE = base.ObterEntidade<UGEEntity>(PTResMensal.TB_UGE_ID),
                UgeAlmoxarifado = base.ObterEntidade<UGEEntity>(PTResMensal.TB_UGE_ID_ALMOXARIFADO),
                DocumentoRelacionado = PTResMensal.TB_MOVIMENTO_NUMERO_DOCUMENTO_RELACIONADO,
                MovimentoItemIDs = PTResMensal.TB_MOVIMENTO_ITEM_RELACIONADOS,
                Valor = PTResMensal.TB_PTRES_MENSAL_VALOR,
                NlLancamento = PTResMensal.TB_PTRES_MENSAL_NL,
                Obs = PTResMensal.TB_PTRES_MENSAL_OBS,

                DataHoraPagamento = PTResMensal.TB_PTRES_MENSAL_DATA_PAGAMENTO,
                UsuarioSamId = PTResMensal.TB_LOGIN_ID,
            });

            return _actionSeletor;
        }
        private PTResMensalEntity _instanciadorDTOPTResMensal(int anoMesRef, int uaID, int uoID, int codigoPTRes, int codigoNaturezaDespesa, int ugeID, int ugeAlmoxID, string documentosRelacionados, string movItemIDs, decimal valorNLConsumo, string nlConsumo, string observacao, int loginUsuarioSamID, bool integraLote = false, int? numeroSequencia = null)
        {
            PTResMensalEntity objRetorno = null;

            objRetorno = new PTResMensalEntity()
            {
                AnoMesRef = anoMesRef,
                //Almoxarifado = base.ObterEntidade<AlmoxarifadoEntity>(almoxID),
                UA = base.ObterEntidade<UAEntity>(uaID),
                UO = base.ObterEntidade<UOEntity>(uoID),
                PtRes = base.ObterEntidade<PTResEntity>(codigoPTRes, true),
                NaturezaDespesa = base.ObterEntidade<NaturezaDespesaEntity>(codigoNaturezaDespesa, true),
                UGE = base.ObterEntidade<UGEEntity>(ugeID),
                UgeAlmoxarifado = base.ObterEntidade<UGEEntity>(ugeAlmoxID),
                DocumentoRelacionado = documentosRelacionados,
                MovimentoItemIDs = movItemIDs,
                Valor = valorNLConsumo,
                NlLancamento = nlConsumo,
                Obs = observacao,
                UsuarioSamId = loginUsuarioSamID,

                TipoLancamento = ((!String.IsNullOrWhiteSpace(nlConsumo)) ? 'N' : 'E'),
                IntegraLote = integraLote,
                NumeradorSequencia = numeroSequencia,
            };

            return objRetorno;
        }
    }
}