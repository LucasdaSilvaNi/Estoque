using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Domain.Entity.Relatorios;
using System.Data;


namespace Sam.Presenter
{
    public class FechamentoMensalPresenter : CrudPresenter<IFechamentoMensalView>
    {
        IFechamentoMensalView view;

        private StringBuilder PendenciasTransferencia = new StringBuilder();
        public StringBuilder GetPendenciasTransferencia() { return PendenciasTransferencia; }

        public string SubitensInativosFechamento { private set; get; }
        public FechamentoMensalEntity Fechamento { set; private get; }

        public IFechamentoMensalView View
        {
            get { return view; }
            set { view = value; }
        }

        public FechamentoMensalPresenter()
        { }

        public FechamentoMensalPresenter(IFechamentoMensalView _view) : base(_view)
        {
            this.View = _view;
        }

        public void SimularFechamento()
        {
            int idAlmox = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            int iAnoMesRef = (int)TratamentoDados.TryParseInt32(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef);

            IList<SaldoSubItemEntity> fechamentoList = new SaldoSubItemBusiness().AnalisarFechamentoMensal(idAlmox, iAnoMesRef);
            FechamentoMensalBusiness estrutura = new FechamentoMensalBusiness();
            estrutura.Fechamento.Almoxarifado = new AlmoxarifadoEntity(idAlmox);
            estrutura.Fechamento.AnoMesRef = iAnoMesRef;

            if (!estrutura.SalvarFechamentoMensal(fechamentoList, GeralEnum.SituacaoFechamento.Simular))
            {
                this.View.ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                this.View.ListaErros = estrutura.ListaErro;

                return;
            }
        }

        public IList<SaldoSubItemEntity> AnalisarFechamentoMensal(int? almoxId, int? anoMes)
        {
            IList<SaldoSubItemEntity> lstRetorno = null;
            SaldoSubItemBusiness objBusiness = null;

            objBusiness = new SaldoSubItemBusiness();
            lstRetorno = objBusiness.AnalisarFechamentoMensal(almoxId, anoMes);

            //Retirar subitens sem saldo ou movimetação
            int c = 0;

            while (c <= lstRetorno.Count - 1)
            {
                if (lstRetorno[c].SaldoAnterior == 0 && lstRetorno[c].QtdeEntrada == 0)
                {
                    lstRetorno.RemoveAt(c);
                    c--;
                }
                c++;
            }

            this.View.ListaErros = objBusiness.ListaErro;

            return lstRetorno;
        }

        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Nome = "rptRelMensalBalancete.rdlc";
            relatorioImpressao.Id = (int)RelatorioEnum.BalanceteSimulacao;
            relatorioImpressao.DataSet = "dsFechamentoMensal";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }


        public IList<FechamentoMensalEntity> ImprimirFechamentoMensal(int? almoxId, int? anoMes, bool? tipoNatureza, GeralEnum.SituacaoFechamento situacaoFechamento,bool saldoZero)
        {
            FechamentoMensalEntity fecha = new FechamentoMensalEntity();

            fecha.SituacaoFechamento = (int)situacaoFechamento;

            //Filtra almoxarifado
            fecha.Almoxarifado = new AlmoxarifadoEntity(almoxId);

            //Filtrar mês ano
            fecha.AnoMesRef = anoMes;

            //Filtrar Natureza Despesa
            fecha.SubItemMaterial = new SubItemMaterialEntity();
            fecha.SubItemMaterial.NaturezaDespesa = new NaturezaDespesaEntity();
            fecha.SubItemMaterial.NaturezaDespesa.Natureza = tipoNatureza;
            fecha.chkSaldoMaiorZero = saldoZero;

            FechamentoMensalBusiness estrutura = new FechamentoMensalBusiness();
            estrutura.Fechamento = fecha;
            return estrutura.ImprimirFechamentoMensal();
        }

        public IList<relInventarioFechamentoMensalEntity> ImprimirInventarioMensal(int almoxID, int anoMesRef)
        {
            IList<relInventarioFechamentoMensalEntity> lstInventarioMensal = null;
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            lstInventarioMensal = objBusiness.ImprimirInventarioMensal(almoxID, anoMesRef);

            return lstInventarioMensal;
        }

        public IList<relAnaliticoFechamentoMensalEntity> ImprimirAnaliticoBalanceteMensal(int almoxID, int anoMesRef)
        {
            IList<relAnaliticoFechamentoMensalEntity> lstInventarioMensal = null;
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            lstInventarioMensal = objBusiness.ImprimirAnaliticoBalanceteMensal(almoxID, anoMesRef);

            return lstInventarioMensal;
        }

        public bool EstornarFechamentoMensal(int? almoxId)
        {
            FechamentoMensalBusiness objBusiness = null;
            EstruturaOrganizacionalBusiness eoBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            eoBusiness = new EstruturaOrganizacionalBusiness();

            //estrutura.Fechamento.Almoxarifado = new EstruturaOrganizacionalBusiness().AlmoxarifadoTodosCod().Where(a => a.Id == almoxId).FirstOrDefault();//Refazer
            objBusiness.Fechamento.Almoxarifado = eoBusiness.ObterAlmoxarifado(almoxId.Value);
            objBusiness.Fechamento.AnoMesRef = Int32.Parse(objBusiness.Fechamento.Almoxarifado.MesRef);

            if (!objBusiness.EstornarFechamentoMensal())
            {
                this.View.ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                this.View.ListaErros = objBusiness.ListaErro;
                return false;
            }
            else
            {
                if (objBusiness.Fechamento.AnoMesRef == null)
                    objBusiness.Fechamento.AnoMesRef = int.Parse(objBusiness.Fechamento.Almoxarifado.RefInicial);

                this.View.ExibirMensagem(String.Format("O mês {0}/{1} foi reaberto com sucesso.", objBusiness.Fechamento.AnoMesRef.ToString().Substring(4, 2), objBusiness.Fechamento.AnoMesRef.ToString().Substring(0, 4)));
                return true;
            }
        }

        public void ExecutarFechamento(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {
            FechamentoMensalBusiness business = new FechamentoMensalBusiness();
            try
            {
                business.ExecutarFechamento(AlmoxarifadoId, mesAnoReferencia, usuarioSamLoginId);

                if (business.ListaErro.IsNotNullAndNotEmpty())
                {
                    this.View.ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                    this.View.ListaErros = business.ListaErro;

                    this.View.BloqueiaBotaoNLConsumo = this.View.BloqueiaBotaoReabertura = false;
                }
                else
                {
                    this.View.ExibirMensagem("Fechamento concluído com sucesso.");

                    this.View.BloqueiaBotaoNLConsumo = this.View.BloqueiaBotaoReabertura = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void ExecutarSimulacao(Int32 AlmoxarifadoId, Int32 mesAnoReferencia, int usuarioSamLoginId)
        {
            FechamentoMensalBusiness business = new FechamentoMensalBusiness();
            try
            {
                business.ExecutarSimulacao(AlmoxarifadoId, mesAnoReferencia, usuarioSamLoginId);

                if (business.ListaErro.IsNotNullAndNotEmpty())
                {
                    this.View.ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                    this.View.ListaErros = business.ListaErro;
                }
                else
                {
                    this.View.ExibirMensagem("Simulação concluída com sucesso.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public bool EfetuarFechamentoMensal(int? almoxId, int? anoMes, GeralEnum.SituacaoFechamento situacao)
        {
            IList<SaldoSubItemEntity> fechamentoList = new SaldoSubItemBusiness().AnalisarFechamentoMensal(almoxId, anoMes);
            FechamentoMensalBusiness estrutura = new FechamentoMensalBusiness();
            estrutura.Fechamento.Almoxarifado = new AlmoxarifadoEntity(almoxId);
            estrutura.Fechamento.AnoMesRef = anoMes;

            if (!estrutura.SalvarFechamentoMensal(fechamentoList, situacao))
            {
                this.View.ExibirMensagem("Inconsistências encontradas. Favor verificar.");
                this.View.ListaErros = estrutura.ListaErro;

                return false;
            }
            else
            {
                if (situacao == GeralEnum.SituacaoFechamento.Executar)
                    this.View.ExibirMensagem("Fechamento concluí­do com sucesso.");
                else if (situacao == GeralEnum.SituacaoFechamento.Simular)
                    this.View.ExibirMensagem("Simulação concluída com sucesso.");

                return true;
            }
        }

        public IList<string> ListarFechamentosEfetuados(int pIntAlmoxarifadoID)
        {
            IList<string> listaRetorno = null;

            FechamentoMensalBusiness lObjBusiness = new FechamentoMensalBusiness();
            listaRetorno = lObjBusiness.ListarMesesFechados(pIntAlmoxarifadoID);

            return listaRetorno;
        }
        public IList<FechamentoMensalEntity> ListarFechamentosEfetuados(int pIntAlmoxarifadoID, bool pBlnAgruparResultados)
        {
            IList<FechamentoMensalEntity> listaRetorno = null;

            FechamentoMensalBusiness lObjBusiness = new FechamentoMensalBusiness();
            listaRetorno = lObjBusiness.Listar(pIntAlmoxarifadoID, pBlnAgruparResultados);

            return listaRetorno;
        }

        #region Consumo Fechamento

        public IList<PTResMensalEntity> processarConsumoUAsAlmoxarifadoEmLote(int anoMesReferencia, int almoxID, string loginSiafem, string senhaSiafem, bool isEstorno = false)
        {
            this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            lstPTResParaConsumo = objBusiness.processarConsumoUAsAlmoxarifadoEmLote(almoxID, anoMesReferencia, loginSiafem, senhaSiafem, isEstorno);
            view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }
        public IList<PTResMensalEntity> processarConsumoUAsAlmoxarifadoEmLote(IList<PTResMensalEntity> notasConsumoParaPagamento, string loginSiafem, string senhaSiafem, bool isEstorno = false)
        {
            this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            objBusiness.Fechamento = Fechamento;
            lstPTResParaConsumo = objBusiness.processarConsumoUAsAlmoxarifadoEmLote(notasConsumoParaPagamento, loginSiafem, senhaSiafem, isEstorno);

            if (this.view.IsNotNull())
                view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoAlmox(int gestorId, int anoMesReferencia, int almoxId)
        {
            if (this.View.IsNotNull())
                this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            PTResMensalBusiness objBusiness = null;

            objBusiness = new PTResMensalBusiness();
            lstPTResParaConsumo = objBusiness.ProcessarNLsConsumoAlmox(gestorId, anoMesReferencia, almoxId).ToList();

            if (this.View.IsNotNull())
                view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }

        public IList<PTResMensalEntity> ProcessarNLsConsumoImediato(int almoxID, int anoMesReferencia, int orgaoId, int idPerfil)
        {
            if (this.View.IsNotNull())
                this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            PTResMensalBusiness objBusiness = null;

            objBusiness = new PTResMensalBusiness();
            lstPTResParaConsumo = objBusiness.ProcessarNLsConsumoImediato(almoxID, anoMesReferencia, orgaoId, idPerfil).ToList();

            if (this.View.IsNotNull())
                view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }

        public IList<PTResMensalEntity> ObterNLConsumoNulas(int anoMesReferencia, int almoxID, bool retornaEstornadas = false)
        {
            if (this.view.IsNotNull())
                this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            PTResMensalBusiness objBusiness = null;

            objBusiness = new PTResMensalBusiness();
            lstPTResParaConsumo = objBusiness.ObterNLsConsumoNulas(almoxID, anoMesReferencia, retornaEstornadas).ToList();

            if (this.view.IsNotNull())
                view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }

        public IList<PTResMensalEntity> ObterNLConsumoPagas(int anoMesReferencia, int almoxID, bool retornaEstornadas = false)
        {
            if (this.view.IsNotNull())
                this.view.ListaErros = null;

            IList<PTResMensalEntity> lstPTResParaConsumo = null;
            PTResMensalBusiness objBusiness = null;

            objBusiness = new PTResMensalBusiness();
            lstPTResParaConsumo = objBusiness.ObterNLsConsumoPagas(almoxID, anoMesReferencia, retornaEstornadas).ToList();

            if (this.view.IsNotNull())
                view.ListaErros = objBusiness.ListaErro;

            return lstPTResParaConsumo;
        }

        public PTResMensalEntity GerarNLConsumo(string loginSiafem, string senhaSiafem, int almoxID, int uaID, int ptresID, int natDespesaID, decimal valorNotaConsumo, string movimentoItemIDs, bool isEstorno = false)
        {
            var usuarioLogado = this.Acesso.Transacoes.Perfis[0].IdLogin;
            var dtSolicitacaoPagamento = DateTime.Now;
            this.view.ListaErros = null;

            PTResMensalEntity ptResParaConsumo = null;
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
            ptResParaConsumo = objBusiness.GerarNLConsumo(usuarioLogado, loginSiafem, senhaSiafem, almoxID, uaID, ptresID, natDespesaID, valorNotaConsumo, movimentoItemIDs, isEstorno);
            view.ListaErros = objBusiness.ListaErro;


            return ptResParaConsumo;
        }

        public PTResMensalEntity ObterNLConsumoPaga(int almoxID, int anoMesRef, int uaID, int ptresID, int natDespesaID, decimal valorNotaConsumo, bool isEstorno = false)
        {
            this.view.ListaErros = null;

            PTResMensalEntity ptResParaConsumo = null;
            PTResMensalBusiness objBusiness = null;

            objBusiness = new PTResMensalBusiness();
            ptResParaConsumo = objBusiness.ObterNLConsumoPaga(almoxID, anoMesRef, uaID, ptresID, natDespesaID, valorNotaConsumo);
            view.ListaErros = objBusiness.ListaErro;


            return ptResParaConsumo;
        }

        /// <summary>
        /// Função de verificação de transferências pendentes.
        /// Caso exista alguma movimentação pendente, a função retornará verdadeiro (true).
        /// </summary>
        /// <param name="pIntAlmoxarifadoOrigem"></param>
        /// <param name="pIntMesReferencia"></param>
        /// <returns></returns>
        public bool VerificarTransferenciasPendentes(int pIntAlmoxarifadoOrigem, int pIntMesReferencia)
        {
            bool lBlnRetorno = false;

            MovimentoBusiness lObjBusiness = null;
            IList<MovimentoEntity> lLstRetorno = null;

            lObjBusiness = new MovimentoBusiness();
            lLstRetorno = lObjBusiness.VerificarTransferenciasPendentes(pIntAlmoxarifadoOrigem, pIntMesReferencia);//.ToList();

            if (lLstRetorno != null && lLstRetorno.Count > 0)
            {
                lBlnRetorno = true;
                //this.View.ListaErros = lObjBusiness.ListaErro; // Não está mais visivel na tela, apenas no relatório

                foreach (var erro in lObjBusiness.ListaErro)
                {
                    PendenciasTransferencia.AppendLine(String.Format("  - {0}", erro.ToString()));
                }
            }


            return lBlnRetorno;
        }

        /// <summary>
        /// Função de verificação de subitens de material inativos que possam influenciar valores de relatórios.
        /// </summary>
        /// <param name="AlmoxarifadoId"></param>
        /// <returns></returns>
        public bool VerificarSubitensInativos(int AlmoxarifadoId)
        {
            bool blnRetorno = false;
            CatalogoBusiness objBusiness = null;
            StringBuilder sbSubitensInativosFechamento = null;

            objBusiness = new CatalogoBusiness();
            blnRetorno = objBusiness.VerificarSubitensInativos(AlmoxarifadoId);

            sbSubitensInativosFechamento = new StringBuilder();
            objBusiness.ListaErro.ForEach(subitemInativo => sbSubitensInativosFechamento.AppendLine(subitemInativo.ToUpperInvariant()));

            this.SubitensInativosFechamento = sbSubitensInativosFechamento.ToString();

            return blnRetorno;
        }

        #endregion Consumo Fechamento

        public List<FechamentoAnualEntity> GerarBalanceteAnual(int idAlmoxarifado, string mesrefAnoAnterior, string mesRefInicial, string mesRefFinal)
        {

            FechamentoMensalBusiness business = new FechamentoMensalBusiness();
            return business.GerarBalanceteAnual(idAlmoxarifado, mesrefAnoAnterior, mesRefInicial, mesRefFinal);
        }

        public bool PodeExecutarPagamentoConsumoAlmox(int almoxID)
        {
            bool blnRetorno = false;
            FechamentoMensalBusiness objBusiness = null;


            objBusiness = new FechamentoMensalBusiness();
            blnRetorno = objBusiness.PodeExecutarPagamentoConsumoAlmox(almoxID);

            return blnRetorno;
        }

        public bool VerificaRestricaoFechamentoExercicioFiscal(int almoxID)
        {
            bool blnRetorno = false;
            FechamentoMensalBusiness objBusiness = null;


            objBusiness = new FechamentoMensalBusiness();
            blnRetorno = objBusiness.VerificaRestricaoFechamentoExercicioFiscal(almoxID);

            return blnRetorno;
        }

        public bool VerificaRestricaoEntradaPorInventario(int almoxID)
        {
            bool blnRetorno = false;
            FechamentoMensalBusiness objBusiness = null;


            objBusiness = new FechamentoMensalBusiness();
            blnRetorno = objBusiness.VerificaRestricaoEntradaPorInventario(almoxID);

            return blnRetorno;
        }


        public bool PodeExecutarFechamentoMensal(int almoxID)
        {
            bool blnRetorno = false;
            FechamentoMensalBusiness objBusiness = null;


            objBusiness = new FechamentoMensalBusiness();
            blnRetorno = objBusiness.PodeExecutarFechamentoMensal(almoxID);

            return blnRetorno;
        }


        public AlmoxarifadoEntity VerificarAlmoxarifadoInativos(int? AlmoxarifadoId)
        {
            FechamentoMensalBusiness objBusiness = null;

            objBusiness = new FechamentoMensalBusiness();
                       
            return objBusiness.VerificarAlmoxarifadoInativos(AlmoxarifadoId);           
        }

        public bool ContemFechamento(int almoxarifadoId)
        {
            bool almoxarifadoPossuiFechamentos = false;


            FechamentoMensalBusiness objBusiness = new FechamentoMensalBusiness();
            almoxarifadoPossuiFechamentos = objBusiness.ContemFechamento(almoxarifadoId);

            return almoxarifadoPossuiFechamentos;
        }

    }
}
