using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.View;
using Sam.Domain.Business;
using Sam.Common.Util;
using System.ComponentModel;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using Sam.Domain.Business.SIAFEM;


namespace Sam.Presenter
{
    public class SaidaMaterialPresenter : CrudPresenter<ISaidaMaterialView>
    {
        private MovimentoBusiness movimentoBusiness = null;

        ISaidaMaterialView view;
        string statusRetornoSIAF_SAM = null;
        public ISaidaMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public SaidaMaterialPresenter(ISaidaMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public SaidaMaterialPresenter()
        {

        }

        #region Eventos de Pagina

        public override void Load()
        {
            base.Load();
            this.View.BloqueiaNovo = false;
            this.View.BloqueiaRascunho = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaGravar = false;
            this.View.MostrarPainelEdicao = false;
            //this.View.PopularListaUGE();
            this.View.isRascunho = false;

            LimparMovimento();
        }

        public override void Novo()
        {
            this.View.BloqueiaGravar = true;
            this.View.BloqueiaRascunho = false;
            this.View.BloqueiaCancelar = true;
            this.View.BloqueiaCodigo = true;
            this.View.BloqueiaDescricao = true;
            this.View.BloqueiaExcluir = false;
            this.View.MostrarPainelEdicao = ValidaDataMovimento();
            this.View.BloqueiaNovo = false;
            this.View.isRascunho = false;

            this.View.PopularListaUGE();
           // this.View.InscricaoCE = null;
            this.View.ValorDocumento = null;
        }

        public override void Cancelar()
        {
            LimparItem();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;
            this.View.isRascunho = false;
            this.View.ValorDocumento = null;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            this.View.BloqueiaGravar = true;
            this.LimparMovimento();
            this.View.GravadoSucessoAtualizar();
            this.view.DataMovimento = new DateTime(0);
            this.View.isRascunho = false;
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
        }

        public void LimparMovimento()
        {
            this.View.Id = string.Empty;
            this.View.Observacoes = string.Empty;
            this.View.DataMovimento = null;
            this.View.NumeroDocumento = string.Empty;
            this.View.DivisaoId = null;
            this.View.isRascunho = false;

            this.View.GeradorDescricao = null;
            this.View.DataDocumento = null;
            this.View.InscricaoCE = null;
            this.View.ValorDocumento = null;

            LimparItem();
        }

        public void LimparItem()
        {
            this.View.MovimentoItemId = null;
            this.View.SubItemMaterialTxt = string.Empty;
            this.View.Descricao = string.Empty;
            this.View.Unidade = string.Empty;
            //this.View.PopularListaUGE();
            //this.View.PopularListaLote();
            this.View.Saldo = string.Empty;
            this.View.QtdFornecida = string.Empty;
            this.View.QtdeLiqItem = 0;
            this.View.PTResCodigo = null;
        }

        public void AdicionadoSucesso()
        {
            LimparItem();

            this.View.BloqueiaGravar = false;
            this.View.BloqueiaExcluir = false;
            this.View.BloqueiaCancelar = false;
            this.View.BloqueiaCodigo = false;
            this.View.BloqueiaDescricao = false;
            this.View.BloqueiaNovo = true;
            this.View.BloqueiaNovo = true;
            this.View.MostrarPainelEdicao = false;
        }

        public void CancelarItem()
        {
            LimparItem();
        }

        #endregion

        #region Eventos Popular Pagina

        //public IList<MovimentoEntity> PopularDadosDocumentoTodosCod(int _fornecedorId)
        //{
        //    MovimentoBusiness estrutura = new MovimentoBusiness();
        //    IList<MovimentoEntity> retorno = estrutura.ListarTodosCodPorFornecedor(_fornecedorId);
        //    return retorno;
        //}

        //public IList<MovimentoEntity> PopularDadosDocumentoTodosCodPorUge(int _ugeId, int _tipoMovimento)
        //{
        //    MovimentoBusiness estrutura = new MovimentoBusiness();
        //    IList<MovimentoEntity> retorno = estrutura.ListarTodosCodPorUgeTipoMovimento(_ugeId, _tipoMovimento);
        //    return retorno;
        //}

        public void CarregarMovimentoTela(MovimentoEntity mov)
        {
            if (mov.Id.HasValue)
                this.View.Id = mov.Id.Value.ToString();

            this.View.Empenho = mov.Empenho;
            this.View.DataDocumento = mov.DataDocumento;
            this.View.DataMovimento = mov.DataMovimento;
            this.View.GeradorDescricao = mov.GeradorDescricao;

            if (mov.Fornecedor != null)
                this.View.FornecedorId = mov.Fornecedor.Id;

            if (mov.Divisao != null)
                this.View.DivisaoId = mov.Divisao.Id;

            if (mov.MovimAlmoxOrigemDestino != null)
                this.View.AlmoxarifadoIdOrigem = mov.MovimAlmoxOrigemDestino.Id;

            this.View.NumeroDocumento = mov.NumeroDocumento;
            this.View.Observacoes = mov.Observacoes;
            this.View.TipoMovimento = mov.TipoMovimento.Id;

            if (mov.ValorDocumento.HasValue)
                this.View.ValorDocumento = mov.ValorDocumento;
        }


        //public IList<UGEEntity> PopularListaUGE(int _orgaoId, int _gestorId)
        //{
        //    EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();

        //    IList<UGEEntity> retorno = estrutura.ListarTodosCodPorGestor(_gestorId);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;
        //    return retorno;
        //}

        //public IList<DivisaoEntity> PopularListaDivisao(int _almoxId)
        //{
        //    EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
        //    IList<DivisaoEntity> retorno = estrutura.ListarDivisaoPorAlmoxTodosCod(_almoxId);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;
        //    return retorno;
        //}

        //public IList<AlmoxarifadoEntity> PopularListaAlmoxarifado(int _gestorId)
        //{
        //    EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
        //    IList<AlmoxarifadoEntity> retorno = estrutura.ListarAlmoxarifadoPorGestorTodosCod(_gestorId);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;
        //    return retorno;
        //}

        #endregion

        #region Eventos Principais

        public decimal? CalculaTotalSaldoUGEsReserva(int? subItemId, int? almoxId)
        {
            SaldoSubItemBusiness business = new SaldoSubItemBusiness();
            return business.CalculaTotalSaldoUGEsReserva(subItemId, almoxId);
        }

        public Tuple<decimal?, decimal?, decimal?> SaldoMovimentoItemDataMovimento(int? subItemId, int? almoxId, int? ugeId, DateTime dtMovimento)
        {        
             SaldoSubItemBusiness business = new SaldoSubItemBusiness();
             return business.SaldoMovimentoItemDataMovimento(subItemId, almoxId, ugeId, dtMovimento);
        }
       

        public MovimentoEntity ExcluirItem(MovimentoEntity mov)
        {
            var result = (from m in mov.MovimentoItem
                          where m.Id != this.View.MovimentoItemId
                          select m).ToList();
            mov.MovimentoItem = result;
            return mov;
        }

        public bool VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(MovimentoEntity movimentacaoMaterial, MovimentoItemEntity itemMovimentacao)
        {
            bool ehDivergente = false;
            MovimentoPresenter movimentacaoMaterialPresenter = null;


            movimentacaoMaterialPresenter = new MovimentoPresenter();
            ehDivergente = movimentacaoMaterialPresenter.VerificaSeTipoMaterialSubitemDivergenteTipoMaterialMovimentacao(movimentacaoMaterial, itemMovimentacao);

            return ehDivergente;
        }

        public decimal? getPrecoUnitSubItem(int? _subItemMaterialId, int? _almoxId, int? _ugeId, string _loteIdent, string _loteFabric, DateTime? _loteDataVenc)
        {
            SaldoSubItemBusiness saldoBusiness = new SaldoSubItemBusiness();
            SaldoSubItemEntity subItem = saldoBusiness.ConsultarSaldoSubItem(_subItemMaterialId, _almoxId, _ugeId, _loteIdent, _loteFabric, _loteDataVenc);
            return subItem.PrecoUnit;
        }


        //public void Estornar(MovimentoEntity movimento)
        public Tuple<string,List<string>> Estornar(string loginSiafemUsuario, string senhaSiafemUsuario, MovimentoEntity movimento)
        {
            int tipoMovID = View.TipoMovimento;
            string nlMovimentacaoMaterial = null;
            string msgSaida = null;
            string msgProcessamentoSIAF = null;
            string msgEstornoMovimentacaoSucesso = null;
            string msgErroGravacaoEstornoMovimentacao = null;
            string msgRetorno = string.Empty;
            List<string> msgRetornoErro = null;
            string strDescricaoTipoMovimentacao = null;
            string sistemaOrigemErro = null;
            string descricaoErroSistema = null;
            string[] dadosMsgErroLancamento = null;


            var perfilLogado_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            MovimentoBusiness movimentoBusiness = new MovimentoBusiness();

            //if (!movimentoBusiness.EstornarMovimentoSaida())
            //if (!movimentoBusiness.EstornarMovimentoSaida(perfilLogado_ID))
            movimento = movimentoBusiness.ObterMovimento(movimento.Id.Value);
            movimentoBusiness.Movimento = movimento;

            movimentoBusiness.Movimento.InscricaoCE = this.View.InscricaoCE;
            movimentoBusiness.Movimento.Ativo = false;

            //Verificar se apesar de movimento existir na base, existe pendencia SIAF relacionada ao mesmo, e/ou não tem NL vinculada ao mesmo.
            //Caso não exista vínculos, proceder com estorno.
            var podeEstornarSemSiafem = movimentoBusiness.VerificaSePermiteEstornoSemSIAF(movimentoBusiness.Movimento);
            if (podeEstornarSemSiafem)
            {
                //var statusEstornoMovimentacaoSAM = estrutura.EstornarMovimentoEntrada(perfilLogado_ID);
                var statusEstornoMovimentacaoSAM = movimentoBusiness.ExecutaEstornoSemSIAF(perfilLogado_ID, movimento.InscricaoCE);
                if (statusEstornoMovimentacaoSAM.Item1)
                {

                    tipoMovID = movimentoBusiness.Movimento.TipoMovimento.Id;
                    strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);

                    msgEstornoMovimentacaoSucesso = String.Format(@"Saída do tipo ""{0}"", documento {1}, estornada com sucesso!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento);
                    msgRetorno = msgEstornoMovimentacaoSucesso;
                }
                else
                {
                    //msgErroGravacaoEstornoMovimentacao = statusEstornoMovimentacaoSAM.Item2;
                    //movimentoBusiness.ListaErro.Insert(0, msgErroGravacaoEstornoMovimentacao);

                  //  this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = statusEstornoMovimentacaoSAM.Item2;
                    movimentoBusiness.ListaErro = statusEstornoMovimentacaoSAM.Item2;

                    msgRetorno = "Inconsistências encontradas, verificar mensagens!";
                    msgRetornoErro = movimentoBusiness.ListaErro;
                }

                //LimparItem();
                //this.Cancelar();
            }
            else
            {
               
                //if (statusEstornoMovimentacaoSIAFEM)
                //{
                    strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);
                    nlMovimentacaoMaterial = movimentoBusiness.Movimento.ObterNLsMovimentacao(false, true);

                    var statusEstornoMovimentacaoSAM = movimentoBusiness.EstornarMovimentoSaida(perfilLogado_ID, movimento.InscricaoCE);
                    if (statusEstornoMovimentacaoSAM.Item1)
                    {

                        this.GravadoSucesso();


                    // Saída por Transferência Para Almoxarifado Não Implantado
                    // Acrescenta a informação do Almoxarifado que receberá o material
                    if (movimentoBusiness.Movimento.TipoMovimento.Id == GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode())
                    {
                        int ugeFavorecida = 0;

                        if (movimentoBusiness.Movimento.GeradorDescricao.IsNotNull() && movimentoBusiness.Movimento.GeradorDescricao.Split('/').Length > 1)
                            int.TryParse(movimentoBusiness.Movimento.GeradorDescricao.Split('/')[1].Trim(), out ugeFavorecida);

                        var almoxDestino = new AlmoxarifadoEntity { Uge = new UGEEntity { Codigo = ugeFavorecida }, Gestor = new GestorEntity { CodigoGestao = Acesso.Transacoes.Perfis[0].GestorPadrao.CodigoGestao } };

                        movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = almoxDestino;
                    }


                    this.statusRetornoSIAF_SAM = ExecutaProcessamentoMovimentacaoNoSIAF(loginSiafemUsuario, senhaSiafemUsuario, "E", movimentoBusiness.Movimento);
                        var statusEstornoMovimentacaoSIAFEM = String.IsNullOrWhiteSpace(this.statusRetornoSIAF_SAM);

                        if (statusEstornoMovimentacaoSIAFEM)
                        {
                            msgEstornoMovimentacaoSucesso = String.Format(@"Saída do tipo ""{0}"", documento {1}, estornada com sucesso!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento);
                            msgProcessamentoSIAF = String.Format(@"Gerada NL (estorno) número ""{0}"" no SIAFEM", nlMovimentacaoMaterial);
                            msgSaida = String.Format(@"{0}\n{1}", msgEstornoMovimentacaoSucesso, msgProcessamentoSIAF);
                            msgRetorno = msgSaida;
                        }
                        else
                        {
                            dadosMsgErroLancamento = statusRetornoSIAF_SAM.BreakLine("|");
                            if (dadosMsgErroLancamento.Count() == 1)
                                dadosMsgErroLancamento = new string[] { "SAM", statusRetornoSIAF_SAM.BreakLine("|")[0] };

                            sistemaOrigemErro = dadosMsgErroLancamento[0];
                            descricaoErroSistema = dadosMsgErroLancamento[1];
                            msgProcessamentoSIAF = String.Format(@"Erro {0}: ""{1}""", sistemaOrigemErro, descricaoErroSistema);
                            msgErroGravacaoEstornoMovimentacao = ", ao estornar saída de materiais no Siafem.  ";
                            msgEstornoMovimentacaoSucesso = String.Format(@"Saída do tipo ""{0}"", documento {1}, estornada no SAM com sucesso!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento);
                            msgSaida = String.Format("{0}{1}{2}", msgProcessamentoSIAF, msgErroGravacaoEstornoMovimentacao, msgEstornoMovimentacaoSucesso);


                            movimentoBusiness.ListaErro.Add(msgSaida);
                            movimentoBusiness.ListaErro.Add("SIAFEM não registrou, verificar na Tela Pendências.");
                         
                           // this.Cancelar();

                            msgRetorno = "Inconsistências encontradas no SIAFEM, verificar mensagens!";
                            msgRetornoErro = movimentoBusiness.ListaErro;
                        }
                    }
                    else
                    {
                        // validar (não salva em banco)
                     //   this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                        this.View.ListaErros = movimentoBusiness.ListaErro;
                     

                        //msgErroGravacaoEstornoMovimentacao = String.Format(@"Saída do tipo ""{0}"", documento {1} estornada no SIAFEM (NL (estorno) ""{2}""), com pendências no SAM!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento, nlMovimentacaoMaterial);
                        //msgSaida = String.Format("{0}", msgErroGravacaoEstornoMovimentacao);
                        //movimentoBusiness.ListaErro.Add(msgErroGravacaoEstornoMovimentacao);

                        msgRetorno = "Inconsistências encontradas, verificar mensagens!";
                        msgRetornoErro = movimentoBusiness.ListaErro;
                    }

                   // this.View.ExibirMensagem(msgSaida);
                //}
                //else
                //{
                //    dadosMsgErroLancamento = statusRetornoSIAF_SAM.BreakLine("|");
                //    if (dadosMsgErroLancamento.Count() == 1)
                //        dadosMsgErroLancamento = new string[] { "SAM", statusRetornoSIAF_SAM.BreakLine("|")[0] };

                //    sistemaOrigemErro = dadosMsgErroLancamento[0];
                //    descricaoErroSistema = dadosMsgErroLancamento[1];
                //    //msgProcessamentoSIAF = String.Format(@"Erro SIAFEM: ""{0}""", statusRetornoSIAF);
                //    msgProcessamentoSIAF = String.Format(@"Erro {0}: ""{1}""", sistemaOrigemErro, descricaoErroSistema);
                //    msgErroGravacaoEstornoMovimentacao = ", ao estornar saída de materiais.";
                //    msgSaida = String.Format("{0}{1}", msgProcessamentoSIAF, msgErroGravacaoEstornoMovimentacao);
                  

                //    movimentoBusiness.ListaErro.Add(msgSaida);
                //    //movimentoBusiness.ListaErro.Add("SAM não estornará movimentação enquanto inconsistência SIAFEM persistir.");
                //    movimentoBusiness.ListaErro.Add("SAM não estornará movimentação enquanto inconsistência(s) persistir.");


                //   // this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                //   // this.View.ListaErros = movimentoBusiness.ListaErro;


                //    //LimparMovimento();
                //    //this.View.GravadoSucessoAtualizar();
                //    this.Cancelar();

                //    //msgRetorno = "SAM não estornará movimentação enquanto inconsistência SIAFEM persistir.";
                //    msgRetorno = "SAM não estornará movimentação enquanto inconsistência(s) persistir.";
                //    msgRetornoErro = msgSaida;
                //}
            }

            LimparItem();
            this.Cancelar();
            return new Tuple<string, List<string>>(msgRetorno, msgRetornoErro); 
        }

        public bool AlmoxarifadoContemFechamentos()
        {
            int almoxarifadoId = 0;
            bool _almoxarifadoContemFechamentos = false;
            movimentoBusiness = new MovimentoBusiness();


            almoxarifadoId = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.GetValueOrDefault();
            _almoxarifadoContemFechamentos = movimentoBusiness.AlmoxarifadoContemFechamentos(almoxarifadoId);
            return _almoxarifadoContemFechamentos;
        }

        private bool ExecutaMovimentoSaida(string loginSiafemUsuario, string senhaSiafemUsuario, MovimentoEntity movimento, bool AcessoSIAFEM)
        {
            AlmoxarifadoEntity almoxDestino = null;
            movimentoBusiness = new MovimentoBusiness();
            movimentoBusiness.Movimento = movimento;
            string UgeCPFCnpj = movimento.UgeCPFCnpjDestino;

            //Para cada movimento Item, atualizar Movimento relacionado
            foreach (var item in movimento.MovimentoItem)
                item.Movimento = movimento;

            //Adiciona o perfil do usuário Logado no cadastro do movimento
            movimentoBusiness.Movimento.IdLogin = Acesso.Transacoes.Perfis[0].IdLogin;

            //Adiciona a data da operação como data atual
            movimentoBusiness.Movimento.DataOperacao = DateTime.Now;

            movimentoBusiness.Movimento.Id = Common.Util.TratamentoDados.TryParseInt32(View.Id);
            //movimentoBusiness.Movimento.Almoxarifado.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            movimentoBusiness.Movimento.Almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado ?? null;
            movimentoBusiness.Movimento.AnoMesReferencia = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef ?? string.Empty;
            movimentoBusiness.Movimento.UGE.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id ?? 0;
            movimentoBusiness.Movimento.UGE.Codigo = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Codigo;
            movimentoBusiness.Movimento.TipoMovimento.Id = View.TipoMovimento;

            //if (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao)
            //{
            //    //movimentoBusiness.Movimento.GeradorDescricao = ;
            //    EstruturaOrganizacionalBusiness lObjBusiness = new EstruturaOrganizacionalBusiness();
            //    AlmoxarifadoEntity almoxDestinoDoacao = lObjBusiness.ObterAlmoxarifado(Convert.ToInt32(this.View.GeradorDescricao));
            //    movimentoBusiness.Movimento.GeradorDescricao = almoxDestinoDoacao.Descricao;
            //}
            //else if ((View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia) || (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao))
            if ((View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia) || (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao))
            {
              
                EstruturaOrganizacionalBusiness lObjBusiness = new EstruturaOrganizacionalBusiness();
                //int idGestor = 0;

                if (movimentoBusiness.Movimento.MovimAlmoxOrigemDestino == null)
                    movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();
                movimentoBusiness.Movimento.MovimAlmoxOrigemDestino.Id = View.AlmoxarifadoIdOrigem;

                //idGestor = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
                almoxDestino = lObjBusiness.ObterAlmoxarifado(this.View.AlmoxarifadoIdOrigem.Value);

                if ((View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorDoacao))
                {
                    if (!string.IsNullOrEmpty(movimento.GeradorDescricao))
                    {
                        movimentoBusiness.Movimento.GeradorDescricao = movimento.GeradorDescricao;
                        movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = null;
                        almoxDestino = null;
                    }
                }

                if (almoxDestino != null && almoxDestino.Id.HasValue)
                {
                    movimentoBusiness.Movimento.GeradorDescricao = almoxDestino.Descricao;
                    movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = almoxDestino;
                }
            }
            //else if (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.OutrasSaidas)
            else if ((View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.OutrasSaidas) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorMaterialTransformado) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorExtravioFurtoRoubo) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorIncorporacaoIndevida) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorPerda) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaInservivelQuebra) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado) ||
                     (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.SaidaParaAmostraExposicaoAnalise))
            {
                movimentoBusiness.Movimento.GeradorDescricao = this.View.GeradorDescricao;
            }
            else if (View.TipoMovimento == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
            {
                movimentoBusiness.Movimento.NumeroDocumento = View.NumeroDocumento;
                //movimentoBusiness.Movimento.GeradorDescricao = string.Empty;
                movimentoBusiness.Movimento.GeradorDescricao = movimentoBusiness.Movimento.Divisao.Descricao;
                //Atualiza status para requisição Aprovada
                movimentoBusiness.Movimento.TipoMovimento.Id = (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoAprovada;
            }

            movimentoBusiness.Movimento.Ativo = true;
            movimentoBusiness.Movimento.DataDocumento = this.View.DataDocumento;

            //Caso o usuário não insira a data de Movimento ou seja uma operação que não seja obrigatório. O sistema pega a data atual.
            if (this.View.DataMovimento != null)
                movimentoBusiness.Movimento.DataMovimento = this.View.DataMovimento.Value.Date;
            else
                movimentoBusiness.Movimento.DataMovimento = null;


            movimentoBusiness.Movimento.Empenho = string.Empty;
            movimentoBusiness.Movimento.FonteRecurso = string.Empty;
            movimentoBusiness.Movimento.Instrucoes = View.Instrucoes;
            movimentoBusiness.Movimento.Observacoes = View.Observacoes;


            //Se mês estiver fechado, informar via GUI e abortar processo.
            //var mesFiscalFechadoSIAFEM = statusFechadoMesReferenciaSIAFEM(true);
            var mesFiscalFechadoSIAFEM = movimentoBusiness.VerificaStatusFechadoMesReferenciaSIAFEM(true);
            //if (mesFiscalFechadoSIAFEM)
            //    return false;

            //movimentoBusiness.ListaErro.Add("Verificar lista de erro");
            //if (movimentoBusiness.ListaErro.Count > 0)
            //movimentoBusiness.ListaErro.Clear();


            try
            {

                //return movimentoBusiness.SalvarMovimentoSaida();
                movimentoBusiness.Movimento.InscricaoCE = this.View.InscricaoCE;
                var statusGravacaoMovimentacao = movimentoBusiness.SalvarMovimentoSaida();

                if (statusGravacaoMovimentacao)
                {

                    if (almoxDestino != null && almoxDestino.Id.HasValue)
                    {
                        movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = almoxDestino;
                    }

 					if (!string.IsNullOrEmpty(UgeCPFCnpj))
                        movimentoBusiness.Movimento.UgeCPFCnpjDestino = UgeCPFCnpj;

                    // Saída por Transferência Para Almoxarifado Não Implantado
                    // Acrescenta a informação do Almoxarifado que receberá o material
                    if (movimentoBusiness.Movimento.TipoMovimento.Id == GeralEnum.TipoMovimento.SaidaPorTransferenciaParaAlmoxNaoImplantado.GetHashCode())
                    {
                        int ugeFavorecida = 0;

                        if (movimentoBusiness.Movimento.GeradorDescricao.IsNotNull() && movimentoBusiness.Movimento.GeradorDescricao.Split('/').Length > 1)
                            int.TryParse(movimentoBusiness.Movimento.GeradorDescricao.Split('/')[1].Trim(), out ugeFavorecida);

                        almoxDestino = new AlmoxarifadoEntity { Uge = new UGEEntity { Codigo = ugeFavorecida }, Gestor = new GestorEntity { CodigoGestao = Acesso.Transacoes.Perfis[0].GestorPadrao.CodigoGestao } };

                        movimentoBusiness.Movimento.MovimAlmoxOrigemDestino = almoxDestino;
                    }

                    if (AcessoSIAFEM)
                    {
                        this.statusRetornoSIAF_SAM = ExecutaProcessamentoMovimentacaoNoSIAF(loginSiafemUsuario, senhaSiafemUsuario, "N", movimentoBusiness.Movimento);
                    }
                }

                return statusGravacaoMovimentacao;

            }
            catch (Exception ex)
            {
                new LogErro().GravarLogErro(ex);
                this.View.ExibirMensagem(ex.Message);
                return true;
            }
        }

        private string ExecutaProcessamentoMovimentacaoNoSIAF(string loginSiafemUsuario, string senhaSiafemUsuario, string tipo, MovimentoEntity movimentacaoMaterial)
        {
            SiafemBusiness siafBusiness = null;
            UsuarioPresenter usuarioPresenter = null;
            Entity.Usuario dadosUsuario = null;

            siafBusiness = new SiafemBusiness();
            usuarioPresenter = new UsuarioPresenter();

            dadosUsuario = usuarioPresenter.SelecionaUsuarioPor_LoginID(movimentacaoMaterial.IdLogin.Value);
            siafBusiness.ExecutaProcessamentoMovimentacaoNoSIAF(dadosUsuario.Cpf, loginSiafemUsuario, senhaSiafemUsuario, tipo, movimentacaoMaterial);

            return siafBusiness.ErroProcessamentoWs;
        }

        public bool GravarMovimentoSaida(string loginSiafemUsuario, string senhaSiafemUsuario, MovimentoEntity movimento, bool AcessoSIAFEM)
        {
            var tipoMovID = View.TipoMovimento;
            string msgProcessamentoSIAF = null;
            string msgGravacaoMovimentacaoSucesso = null;
            string nlMovimentacaoMaterial = null;
            string strDescricaoTipoMovimentacao = null;
            string sistemaOrigemErro = null;
            string descricaoErroSistema = null;
            string[] dadosMsgErroLancamento = null;


            if (!this.view.isRascunho)
            {

                //for (int i = 0; i < 3; i++)
                //{

                //    try
                //    {
                //        ExecutaMovimentoSaida(loginSiafemUsuario, senhaSiafemUsuario, movimento, AcessoSIAFEM);
                //        break;
                //    }
                //    catch (Exception ex)
                //    {
                //        new LogErro().GravarLogErro(ex);
                //        //movimentoBusiness.ListaErro.Add(ex.Message);
                //        foreach (var msgErro in movimentoBusiness.ListaErro)
                //            if (!movimentoBusiness.ListaErro.Contains(msgErro))
                //                movimentoBusiness.ListaErro.Add(ex.Message);

                //        var erroBanco = ex.Message.ToUpper().Contains("TIMEOUT EXPIRED") || ex.Message.ToUpper().Contains("DEADLOCK");

                //        if (!erroBanco)
                //            break;

                //    }
                //}
        
                ExecutaMovimentoSaida(loginSiafemUsuario, senhaSiafemUsuario, movimento, AcessoSIAFEM);
                if (movimentoBusiness.ListaErro == null || movimentoBusiness.ListaErro.Count() < 1)
                {
                    this.View.PopularGrid();
                    this.GravadoSucesso();

                    strDescricaoTipoMovimentacao = _obterDescricaoTipoMovimentacao(tipoMovID);
                    msgGravacaoMovimentacaoSucesso = String.Format(@"Saída do tipo ""{0}"", documento {1} salva com sucesso!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento);

                    if (AcessoSIAFEM)
                    {
                        if (String.IsNullOrWhiteSpace(this.statusRetornoSIAF_SAM))
                        {
                            nlMovimentacaoMaterial = movimentoBusiness.Movimento.ObterNLsMovimentacao();
                            msgProcessamentoSIAF = String.Format(@"Gerada NL número ""{0}"" no SIAFEM", nlMovimentacaoMaterial);
                        }
                        else
                        {
                            dadosMsgErroLancamento = statusRetornoSIAF_SAM.BreakLine("|");
                            if (dadosMsgErroLancamento.Count() == 1)
                                dadosMsgErroLancamento = new string[] { "SAM", statusRetornoSIAF_SAM.BreakLine("|")[0] };

                            sistemaOrigemErro = dadosMsgErroLancamento[0];
                            descricaoErroSistema = dadosMsgErroLancamento[1];
                            msgGravacaoMovimentacaoSucesso = String.Format(@"Saída do tipo ""{0}"", documento {1} salva com pendências!", strDescricaoTipoMovimentacao, movimentoBusiness.Movimento.NumeroDocumento);
                            //msgProcessamentoSIAF = String.Format(@"Erro SIAFEM: \n{0}", statusRetornoSIAF);
                            msgProcessamentoSIAF = String.Format(@"Erro {0}: \n{1}", sistemaOrigemErro, descricaoErroSistema);
                        }
                    }

                    var _msgSaida = String.Format(@"{0}\n{1}", msgGravacaoMovimentacaoSucesso, msgProcessamentoSIAF);
                    this.View.ExibirMensagem(_msgSaida);
                }
                else
                {
                    this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                    this.View.ListaErros = movimentoBusiness.ListaErro;
                    return false;
                }
            }
            else
            {
                this.SalvarRascunho(movimentoBusiness.Movimento);
            }

            return true;
        }

        private bool statusFechadoMesReferenciaSIAFEM(bool exibirMensagemErro = false)
        {
            bool blnRetorno = true;
            bool mesFiscalFechadoSIAFEM = true;
            DateTime dtFechamentoSIAFEM = new DateTime();
            CalendarioFechamentoMensalEntity objDataFechamentoMensal = null;
            CalendarioFechamentoMensalBusiness objBusinessCalendarioSIAFEM = null;

            try
            {
                if (!String.IsNullOrWhiteSpace(this.View.AnoMesReferencia) && this.movimentoBusiness.IsNotNull())
                {
                    var anoReferencia = Int32.Parse(this.View.AnoMesReferencia.Substring(0, 4));
                    var mesReferencia = Int32.Parse(this.View.AnoMesReferencia.Substring(4, 2));

                    objBusinessCalendarioSIAFEM = new CalendarioFechamentoMensalBusiness();
                    mesFiscalFechadoSIAFEM = objBusinessCalendarioSIAFEM.StatusFechadoMesReferenciaSIAFEM(mesReferencia, anoReferencia, exibirMensagemErro);
                    objDataFechamentoMensal = objBusinessCalendarioSIAFEM.ObterDataFechamentoMensalSIAFEM(mesReferencia, anoReferencia);
                    dtFechamentoSIAFEM = objDataFechamentoMensal.DataFechamentoDespesa;

                    if (!mesFiscalFechadoSIAFEM)
                    {
                        dtFechamentoSIAFEM = dtFechamentoSIAFEM.AddHours(19);
                        blnRetorno = (DateTime.Now >= dtFechamentoSIAFEM);
                    }
                    else if ((!exibirMensagemErro && mesFiscalFechadoSIAFEM) || (this.movimentoBusiness.ListaErro.IsNotNullAndNotEmpty()))
                    {
                        //this.View.ListaErros = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4}) fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy")) };
                        var listaMsgsErro = new List<string>() { String.Format("Mês/ano referência ({0:D2}/{1:D4})* fechado em {2} 19:00.", mesReferencia, anoReferencia, dtFechamentoSIAFEM.ToString("dd/MM/yyyy")) };

                        if (this.movimentoBusiness.ListaErro.IsNotNullAndNotEmpty() && !objBusinessCalendarioSIAFEM.ListaErro.HasElements())
                            movimentoBusiness.ListaErro.AddRange(listaMsgsErro);
                        else
                            movimentoBusiness.ListaErro.AddRange(objBusinessCalendarioSIAFEM.ListaErro);
                    }
                }
            }
            catch (Exception excErro)
            {
                new LogErro().GravarLogErro(excErro);
                this.View.ExibirMensagem("Módulo Saída de Materiais: Erro ao determinar data de fechamento de mês de referência no SIAFEM.");
                this.View.ListaErros = objBusinessCalendarioSIAFEM.ListaErro;
                return true;
            }


            return blnRetorno;
        }

        private string _obterDescricaoTipoMovimentacao(int tipoMovID)
        {
            //return GeralEnum.GetEnumDescription((GeralEnum.TipoMovimento)tipoMovID);

            GeralEnum.TipoMovimento tipoMovimento = (GeralEnum.TipoMovimento)tipoMovID;

            if (tipoMovimento == GeralEnum.TipoMovimento.RequisicaoPendente)
                tipoMovimento = GeralEnum.TipoMovimento.RequisicaoAprovada;
            
            return GeralEnum.GetEnumDescription(tipoMovimento);
        }

        public MovimentoEntity CarregarRascunho(int idRascunho)
        {
            Sam.Business.SerializacaoBusiness serializacaoBusiness = new Sam.Business.SerializacaoBusiness();

            try
            {
                var rascunho = serializacaoBusiness.SelectOne(a => a.TB_SERIALIZACAO_ID == idRascunho);
                return Common.Util.SerializacaoUtil<MovimentoEntity>.Deserializar(rascunho.TB_SERIALIZACAO_OBJETO);
            }
            catch (Exception e)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

                List<string> erros = new List<string>();
                erros.Add(e.Message);

                this.View.ListaErros = (erros);
            }

            return null;
        }

        public void SalvarRascunho(MovimentoEntity Movimento)
        {
            var tipoMovimento = new TipoMovimentoPresenter().PopularListaTipoMovimentoSaida().Where(a => a.Id == Movimento.TipoMovimento.Id).FirstOrDefault();

            TB_SERIALIZACAO serializacao = new TB_SERIALIZACAO();
            serializacao.TB_SERIALIZACAO_DESCRICAO = String.Format("{0} - {1}", tipoMovimento.Descricao, Movimento.Observacoes);
            serializacao.TB_SERIALIZACAO_DATA = DateTime.Now;
            serializacao.TB_SERIALIZACAO_ANO_MES_REFERENCIA = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef;
            serializacao.TB_SERIALIZACAO_LABEL = String.Format("LABEL");
            serializacao.TB_SERIALIZACAO_NOME = "MovimentoEntity";
            serializacao.TB_LOGIN_ID = Acesso.Transacoes.Perfis[0].IdLogin;
            serializacao.TB_TRANSACAO_ID = Acesso.Transacoes.Perfis[0].IdTransacaoOrigem;
            serializacao.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
            serializacao.TB_SERIALIZACAO_OBJETO = Common.Util.SerializacaoUtil<MovimentoEntity>.Serializar(Movimento);

            Sam.Business.SerializacaoBusiness serializacaoBusiness = new Sam.Business.SerializacaoBusiness();

            if (this.view.rascunhoId > 0)
            {
                serializacao.TB_SERIALIZACAO_ID = this.view.rascunhoId;
                serializacaoBusiness.Update(serializacao);
            }
            else
            {
                serializacaoBusiness.Insert(serializacao);
            }
            this.GravadoSucesso();
            this.View.ExibirMensagem("Rascunho salvo com sucesso!");
        }

        public IList<MovimentoItemEntity> ListarMovimentoItens(int startRowIndexParameterName,
                int maximumRowsParameterName, string _documento)
        {
            MovimentoItemBusiness estruturaItem = new MovimentoItemBusiness();
            estruturaItem.SkipRegistros = startRowIndexParameterName;
            IList<MovimentoItemEntity> retorno = estruturaItem.ListarItensPorMovimento(_documento);
            this.TotalRegistrosGrid = estruturaItem.TotalRegistros;
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItensTodosCod(int _ItemMaterialID)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterialTodosCod(_ItemMaterialID, 7);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, string _documento)
        {
            return this.TotalRegistrosGrid;
        }

        public MovimentoEntity ListarMovimentoItensTodos(int startRowIndexParameterName,
        int maximumRowsParameterName, MovimentoEntity mov, int TipoRequisicao)
        {
            MovimentoItemBusiness estruturaItem = new MovimentoItemBusiness();
            mov.Almoxarifado.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id ?? 0;
            mov.UGE.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id ?? 0;


            MovimentoEntity movimentoRetornoItem = null;
            List<MovimentoEntity> movimentoRetorno = estruturaItem.ListarItensPorMovimentoTodos(mov, TipoRequisicao).ToList();

            if (movimentoRetorno.Count > 1)
            {
                if ((TipoRequisicao == (int)Common.Util.GeralEnum.OperacaoEntrada.Estorno))
                    movimentoRetornoItem = movimentoRetorno.Where(a => a.AnoMesReferencia.Replace("/","") == Convert.ToString(Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.MesRef)).FirstOrDefault();
      
            }else
                movimentoRetornoItem = movimentoRetorno.FirstOrDefault();

            if (movimentoRetornoItem != null)
                return TratamentoMovimentoItensLote(movimentoRetornoItem);
            else
                return null;
        }

        public IList<MovimentoItemEntity> ImprimirRelatorioSaida(MovimentoEntity movimento)
        {
            MovimentoBusiness business = new MovimentoBusiness();
            business.Movimento = movimento;
            movimento = business.GetMovimento();

            business.Movimento = movimento;


            if (business.RetornaDocumentoTransferenciaAlterado())
            {
                business.AtualizaValorTotalDocumentoAlterado();
            }

            if (movimento != null)
            {
                for (int i = 0; i < movimento.MovimentoItem.Count; i++)
                {
                    movimento.MovimentoItem[i].ValorOriginalDocumento = movimento.ValorOriginalDocumento;
                    movimento.MovimentoItem[i].ValorSomaItens = movimento.ValorSomaItens;
                    //movimento.MovimentoItem[i].ValorMov = movimento.ValorDocumento;

                    movimento.MovimentoItem[i].Movimento = movimento;
                    movimento.MovimentoItem[i].TotalItens = movimento.MovimentoItem.Sum(a => a.QtdeMov).ToString();
                    movimento.MovimentoItem[i].SubItemDescricao = movimento.MovimentoItem[i].SubItemMaterial.Descricao;
                    movimento.MovimentoItem[i].UnidadeFornecimentoDes = movimento.MovimentoItem[i].SubItemMaterial.UnidadeFornecimento.Descricao;
                    movimento.MovimentoItem[i].UnidadeFornecimento = movimento.MovimentoItem[i].SubItemMaterial.UnidadeFornecimento.Codigo;
                    if (!String.IsNullOrWhiteSpace(movimento.MovimentoItem[i].Movimento.GeradorDescricao))
                        movimento.MovimentoItem[i].DestinatarioAlmoxarifado = movimento.MovimentoItem[i].Movimento.GeradorDescricao;

                    if (!string.IsNullOrEmpty(movimento.MovimentoItem[i].CodigoDescricao))
                        movimento.MovimentoItem[i].CodigoDescricao = movimento.MovimentoItem[i].CodigoDescricao.Split('-').FirstOrDefault();

                    movimento.MovimentoItem[i].Movimento = movimento;

                    if (!String.IsNullOrWhiteSpace(movimento.Observacoes) && movimento.MovimentoItem[i].Movimento != null)
                        movimento.MovimentoItem[i].Movimento.Observacoes = movimento.Observacoes;

                    if (String.IsNullOrWhiteSpace(movimento.MovimentoItem[i].AlmoxarifadoDesc))
                    {
                        if (!String.IsNullOrWhiteSpace(movimento.MovimentoItem[i].Movimento.Almoxarifado.Descricao))
                            movimento.MovimentoItem[i].AlmoxarifadoDesc = movimento.MovimentoItem[i].Movimento.Almoxarifado.Descricao;
                    }

                    break;
                }

                int idReq = 0;
                MovimentoEntity Requisitante = new MovimentoEntity();
                Requisitante = business.ListarDocumentosRequisicaoByDocumento(movimento.NumeroDocumento);
                                
                if (Requisitante != null)
                {
                    idReq = Requisitante.IdLogin ?? 0;
                    if (idReq > 0)
                    {
                        movimento.NomeRequisitante = new Sam.Business.LoginBusiness().SelectOne(a => a.TB_LOGIN_ID == idReq).TB_USUARIO.TB_USUARIO_NOME_USUARIO;
                    }
                }
               
            }
            else
            {
                throw new Exception();
            }

            #region Truncar Valores Retorno

            movimento.MovimentoItem.Cast<MovimentoItemEntity>().ToList().ForEach(_movItem =>
                                                                                            {
                                                                                                _movItem.PrecoMedio = ((_movItem.PrecoMedio.HasValue) ? (decimal.Parse(_movItem.PrecoMedio.ToString()).truncarQuatroCasas()) : 0.0000m);
                                                                                                _movItem.ValorMov = ((_movItem.ValorMov.HasValue) ? (decimal.Parse(_movItem.ValorMov.ToString()).truncarDuasCasas()) : 0.00m);
                                                                                                _movItem.Desd = ((_movItem.Desd.HasValue) ? (decimal.Parse(_movItem.Desd.ToString()).truncarQuatroCasas()) : 0.0000m);
                                                                                                //_movItem.SomatoriaSaida = _somatoriaValorNota.Value.Truncar(2).ToString();
                                                                                            });

            decimal? _somatoriaValorNota = movimento.MovimentoItem.Cast<MovimentoItemEntity>().ToList().Sum(_movItem => _movItem.ValorMov.Value.Truncar(2));
            _somatoriaValorNota = (_somatoriaValorNota.HasValue) ? (_somatoriaValorNota) : 0.00m;

            movimento.MovimentoItem.Cast<MovimentoItemEntity>().ToList().ForEach(_movItem => _movItem.SomatoriaSaida = _somatoriaValorNota.Value.Truncar(2).ToString());
            #endregion Truncar Valores Retorno

            return FormatarLote(movimento.MovimentoItem);
        }

        public IList<MovimentoItemEntity> FormatarLote(IList<MovimentoItemEntity> movimento)
        {
            if (movimento == null)
                return movimento;

            if (movimento.Count == 0)
                return movimento;

            foreach (var saldo in movimento)
            {
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(saldo.IdentificacaoLote))
                {
                    if (saldo.DataVencimentoLote != null)
                        sb.AppendFormat("Dt Venc: {0}", saldo.DataVencimentoLote.Value.ToShortDateString());
                    else
                        sb.Append("Dt Venc: N/I");


                    if (!String.IsNullOrEmpty(saldo.IdentificacaoLote))
                        sb.AppendFormat(" - Lote: {0}", saldo.IdentificacaoLote.ToString());
                    else
                        sb.Append(" - Lote: N/I");
                }
                else
                {
                    sb.Append("Dt Venc: N/I - Lote: N/I");
                }

                saldo.CodigoDescricao = sb.ToString();
            }

            return movimento;
        }


        public static MovimentoEntity FormataPosicaoMovimentoItensLote(MovimentoEntity retorno)
        {
            if (retorno == null)
                return new MovimentoEntity();

            retorno.MovimentoItem = retorno.MovimentoItem.OrderBy(a => a.SubItemDescricao).ToList();

            for (int i = 0; i < retorno.MovimentoItem.Count; i++)
            {
                retorno.MovimentoItem[i].Posicao = i + 1;
                retorno.MovimentoItem[i].Visualizar = !retorno.Bloquear;
            }

            return retorno;
        }

        public static MovimentoEntity TratamentoMovimentoItensLote(MovimentoEntity retorno)
        {
            if (retorno.TipoMovimento.Id != (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                return retorno;

            for (int i = 0; i < retorno.MovimentoItem.Count; i++)
            {
                if (retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote != null)
                {
                    if (retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.SaldoQtde == null)
                    {
                        retorno.MovimentoItem[i].SaldoQtdeLote = 0;
                        retorno.MovimentoItem[i].QtdeMov = 0;
                        retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.SaldoQtde = 0;
                    }

                    if (retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.SaldoQtde > 0)
                    {

                        if (retorno.MovimentoItem[i].FabricanteLote.Contains("Vários"))
                        {                         
                            retorno.MovimentoItem[i].QtdeMov = 0;
                        }
                        else
                        {
                            if (retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.SaldoQtde >= retorno.MovimentoItem[i].QtdeLiq)
                                retorno.MovimentoItem[i].QtdeMov = retorno.MovimentoItem[i].QtdeLiq;
                            else
                                retorno.MovimentoItem[i].QtdeMov = retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.SaldoQtde;
                        }
                        //Atualiza as informações do lote
                        retorno.MovimentoItem[i].FabricanteLote = retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.LoteFabr;
                        retorno.MovimentoItem[i].IdentificacaoLote = retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.LoteIdent;
                        retorno.MovimentoItem[i].DataVencimentoLote = retorno.MovimentoItem[i].SubItemMaterial.SomaSaldoLote.LoteDataVenc;
                    }
                }
            }

            return retorno;
        }

        public void Imprimir()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.SaidaMaterial;
            relatorioImpressao.Nome = "AlmoxSaidaMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSaidaMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public bool ValidaDataMovimento()
        {
            var blnRetorno = true;                                 
            if (!this.View.DataMovimento.HasValue)
            {
                this.View.ExibirMensagem("Campo de preenchimento obrigatório: Data de Movimento");
                blnRetorno = false;

            }            
            return blnRetorno;
        }

        public bool VerificaSeAlmoxarifadoPossuiFechamentos()
        {
            bool existemFechamentosNesteAlmoxarifado = false;
            int almoxarifadoId = 0;
            FechamentoMensalPresenter fechamentoPresenter = null;
            IList<string> listaMsgsErro = null;



            almoxarifadoId = (this.Acesso.IsNotNull() ? this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.GetValueOrDefault() : 0);
            listaMsgsErro = new List<string>();
            if (almoxarifadoId > 0)
            {
                fechamentoPresenter = new FechamentoMensalPresenter();
                existemFechamentosNesteAlmoxarifado = fechamentoPresenter.ContemFechamento(almoxarifadoId);

                if (!existemFechamentosNesteAlmoxarifado)
                {
                    listaMsgsErro.Add("Almoxarifado deve ter o mês-referência inicial fechado (após transporte de saldo) para que se possa efetuar/atender Requisição de Material!");

                    this.View.ExibirMensagem("Almoxarifado em estado de implantação (transporte de saldo) não-finalizado! Verificar mensagem!");
                    this.View.ListaErros = listaMsgsErro.ToList();
                }
            }


            return existemFechamentosNesteAlmoxarifado;
        }

        #endregion
    }
}

