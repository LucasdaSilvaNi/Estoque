using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using tipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;
using Sam.Common.Enums;
using Sam.Domain.Business.SIAFEM;



namespace Sam.Domain.Business
{
    public partial class EventosPagamentoBusiness : BaseBusiness
    {
        private EventosPagamentoEntity entity = new EventosPagamentoEntity();
        public EventosPagamentoEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public bool Salvar()
        {
            this.Service<IEventosPagamentoService>().Entity = this.Entity;
            this.ConsistirRegistroEventosPagamento();
            if (this.Consistido)
            {
                this.Service<IEventosPagamentoService>().Salvar();
            }
            return this.Consistido;
        }

        public EventoSiafemEntity SalvarSiafem(EventoSiafemEntity itemSiafem)
        {
            this.Service<IEventosPagamentoService>().Entity = this.Entity;
            return this.Service<IEventosPagamentoService>().SalvarSiafem(itemSiafem);

        }

        public bool InativarItemEventoSiafem(int id, string usuario)
        {
            this.Service<IEventosPagamentoService>().Entity = this.Entity;
            return this.Service<IEventosPagamentoService>().InativarItemEventoSiafem(id, usuario);

        }

        public bool AlterarItemEventoSiafem(int id, string usuario, string txt1, string txt2, int subtipo, int subtipoOld, bool estimulo)
        {
            this.Service<IEventosPagamentoService>().Entity = this.Entity;
            return this.Service<IEventosPagamentoService>().AlterarItemEventoSiafem(id, usuario, txt1, txt2, subtipo, subtipoOld, estimulo);

        }


        public IList<EventosPagamentoEntity> ListarEventos()
        {
            IList<EventosPagamentoEntity> lstRetorno = null;

            this.Service<IEventosPagamentoService>().SkipRegistros = this.SkipRegistros;
            lstRetorno = this.Service<IEventosPagamentoService>().Listar();
            this.TotalRegistros = this.Service<IEventosPagamentoService>().TotalRegistros();

            return lstRetorno;
        }
        public IList<EventoSiafemEntity> ListarEventoPatrimonial()
        {
            IList<EventoSiafemEntity> lstRetorno = null;

            lstRetorno = this.Service<IEventosPagamentoService>().ListarPatrimonial();

            return lstRetorno;
        }
        public IList<EventosPagamentoEntity> ListarEventos(string Ano)
        {
            IList<EventosPagamentoEntity> lstRetorno = null;

            this.Service<IEventosPagamentoService>().SkipRegistros = this.SkipRegistros;
            lstRetorno = this.Service<IEventosPagamentoService>().Listar(Ano);
            this.TotalRegistros = this.Service<IEventosPagamentoService>().TotalRegistros();

            return lstRetorno;
        }

        public IList<string> ListarAnoEventos()
        {
            IList<string> lstRetorno = null;

            this.Service<IEventosPagamentoService>().SkipRegistros = this.SkipRegistros;
            lstRetorno = this.Service<IEventosPagamentoService>().ListarAnoEvento();
            this.TotalRegistros = this.Service<IEventosPagamentoService>().TotalRegistros();

            return lstRetorno;
        }

        public IList<EventosPagamentoEntity> Imprimir()
        {
            IList<EventosPagamentoEntity> lstRetorno = null;

            lstRetorno = this.Service<IEventosPagamentoService>().Imprimir();

            return lstRetorno;
        }

        public bool Excluir()
        {
            this.Service<IEventosPagamentoService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IEventosPagamentoService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirRegistroEventosPagamento()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<EventosPagamentoEntity>(ref this.entity);

            //Fazer validação
            if ((this.Entity.PrimeiroCodigo <= 0) || (this.Entity.SegundoCodigo <= 0))
                this.ListaErro.Add("É obrigatório informar código de evento, obrigatoriamente maior que 0 (zero).");

            if (string.IsNullOrEmpty(this.Entity.AnoBase.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar Ano-Base do evento.");

            if (!(this.Entity.TipoMaterialAssociado == (int)tipoMaterial.MaterialConsumo || this.Entity.TipoMaterialAssociado == (int)tipoMaterial.MaterialPermanente))
                this.ListaErro.Add("É obrigatório escolher Tipo de Material associado.");

            if (this.Entity.TipoMovimentoAssociado.IsNull() || (this.Entity.TipoMovimentoAssociado.IsNotNull() && this.Entity.TipoMovimentoAssociado.Id == 0))
                this.ListaErro.Add("É obrigatório associar um tipo de movimentação.");

            if ((this.Entity.PrimeiraInscricao.ToUpperInvariant() == "CE" && (this.Entity.SegundaInscricao.ToUpperInvariant() == "CE")) ||
                  (this.Entity.PrimeiraInscricao.ToUpperInvariant() == "NÃO" && (this.Entity.SegundaInscricao.ToUpperInvariant() == "NÃO")) ||
                  (this.Entity.PrimeiraInscricao.ToUpperInvariant() == "CE PADRÃO" && (this.Entity.SegundaInscricao.ToUpperInvariant() == "NÃO")) ||
                  (this.Entity.PrimeiraInscricao.ToUpperInvariant() == "NÃO" && (this.Entity.SegundaInscricao.ToUpperInvariant() == "CE PADRÃO"))
               )
                this.ListaErro.Add("Não é permitida utilizar conjunto de Inscrições selecionados para o grupo de eventos.");

        }

        public EventosPagamentoEntity ObterEventoPagamento(MovimentoEntity objMovimentacao, bool retornaApenasSeAtivo = false)
        {
            EventosPagamentoEntity objRetorno = null;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<IEventosPagamentoService>().Entity = this.Entity;
                    objRetorno = this.Service<IEventosPagamentoService>().ObterEventoPagamento(objMovimentacao, retornaApenasSeAtivo);
                }
                catch (Exception excErroConsulta)
                {
                    throw excErroConsulta;
                }

                ts.Complete();
            }

            return objRetorno;
        }

        public EventosPagamentoEntity ObterEventoSiafem(MovimentoEntity objMovimentacao, bool retornaApenasSeAtivo = false)
        {
            EventosPagamentoEntity objRetorno = null;


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<IEventosPagamentoService>().Entity = this.Entity;
                    objRetorno = new EventosPagamentoEntity();
                    objRetorno.EventoSiafemItem = new EventoSiafemEntity();
                    objRetorno.EventoSiafemItem = this.Service<IEventosPagamentoService>().ObterEventoSiafem(objMovimentacao);

                    if (objRetorno.EventoSiafemItem.IsNotNull())
                    {
                        objRetorno.NlPatrimonial = true;
                        objRetorno.Ativo = true;
                        objRetorno.EstimuloAtivo = objRetorno.EventoSiafemItem.EstimuloAtivo;

                    }

                }
                catch (Exception excErroConsulta)
                {
                    throw excErroConsulta;
                }

                ts.Complete();
            }

            return objRetorno;
        }


        public EventosPagamentoEntity ObterEventoPagamentoReclassificacao(MovimentoEntity movimentacaoMaterial, string inscricaoCE)
        {
            tipoMaterial @tipoMaterial = default(tipoMaterial);
            EventosPagamentoEntity eventosPagamento = null;

            //@tipoMaterial = movimentacaoMaterial.ObterTipoMaterialEmpenho();
            @tipoMaterial = movimentacaoMaterial.ObterTipoMaterial();

            switch (@tipoMaterial)
            {
                case GeralEnum.TipoMaterial.MaterialConsumo: eventosPagamento = obterEventosPagamentoReclassificacao_MaterialDeConsumo(movimentacaoMaterial, inscricaoCE); break;
                case GeralEnum.TipoMaterial.MaterialPermanente: eventosPagamento = obterEventosPagamentoReclassificacao_MaterialPermanente(movimentacaoMaterial, inscricaoCE); break;
            }

            return eventosPagamento;
        }

        private EventosPagamentoEntity obterEventosPagamentoReclassificacao_MaterialDeConsumo(MovimentoEntity movimentacaoMaterial, string inscricaoCE)
        {
            EventosPagamentoEntity eventosPagamento = null;

            /*
                540469 para o exercício atual 
                540169 para o exercício anterior
                540173 para o exercício anteriores
            */

            int primeiroEventoReclassificacao = 0;
            int primeiroEventoEstornoReclassificacao = 0;

            //VARIA DE ACORDO COM A SITUACAO DO EMPENHO, EM RELACAO AO EXERCICIO ATUAL
            int eventoReclassificacaoExercicioAtual = 540469;
            int eventoEstornoReclassificacaoExercicioAtual = 545469;
            int eventoReclassificacaoExercicioAnterior = 540169;
            int eventoEstornoReclassificacaoExercicioAnterior = 545169;
            int eventoReclassificacaoExerciciosAnteriores = 540173;
            int eventoEstornoReclassificacaoExerciciosAnteriores = 545173;

            //FIXOS
            int segundoEventoReclassificacao = 540461;
            int segundoEventoEstornoReclassificacao = 545461;
            string segundoEventoClassificacao = "115610102";

            int terceiroEventoReclassificacao = 0;
            int terceiroEventoEstornoReclassificacao = 0;
            int terceiroReclassificacaoExercicioAnteriores = 0;
            int terceiroEstornoReclassificacaoExercicioAnteriores = 0;      
   
            string terceiroEventoInscricao = "";
           



            switch (movimentacaoMaterial.TipoMovimento.Id)
            {
                case (int)TipoMovimento.EntradaPorRestosAPagar:
                    primeiroEventoReclassificacao = eventoReclassificacaoExercicioAnterior;
                    primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAnterior;

                    eventosPagamento = new EventosPagamentoEntity()
                    {
                        Ativo = true,
                        PrimeiroCodigo = primeiroEventoReclassificacao,
                        PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                        SegundoCodigo = segundoEventoReclassificacao,
                        SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                        //FIXOS
                        PrimeiraInscricao = movimentacaoMaterial.Empenho,
                        PrimeiraClassificacao = null,
                        SegundaInscricao = inscricaoCE,
                        SegundaClassificacao = "115610102",
                        UGFavorecida = false,
                        TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                        TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
                    };

                    break;

                case (int)TipoMovimento.EntradaPorEmpenho:
                    primeiroEventoReclassificacao = eventoReclassificacaoExercicioAtual;
                    primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAtual;

                    eventosPagamento = new EventosPagamentoEntity()
                    {
                        Ativo = true,
                        PrimeiroCodigo = primeiroEventoReclassificacao,
                        PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                        SegundoCodigo = segundoEventoReclassificacao,
                        SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                        //FIXOS
                        PrimeiraInscricao = movimentacaoMaterial.Empenho,
                        PrimeiraClassificacao = null,
                        SegundaInscricao = inscricaoCE,
                        SegundaClassificacao = "115610102",
                        UGFavorecida = false,
                        TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                        TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
                    };
                    break;

                case (int)TipoMovimento.ConsumoImediatoEmpenho:
                    {
                        //terceiroEventoReclassificacao = 540429;
                        //terceiroEventoEstornoReclassificacao = 545429;
                        //terceiroReclassificacaoExercicioAnteriores = 540427;
                        //terceiroEstornoReclassificacaoExercicioAnteriores = 545427;

                        //Re-alteracao durante homologacao
                        //terceiroEventoReclassificacao = 540432;
                        //terceiroEventoEstornoReclassificacao = 545432;

                        terceiroEventoReclassificacao = 540429;
                        terceiroEventoEstornoReclassificacao = 545429;
                        terceiroReclassificacaoExercicioAnteriores = terceiroEventoReclassificacao;
                        terceiroEstornoReclassificacaoExercicioAnteriores = terceiroEventoEstornoReclassificacao;

                        if (DateTime.Now.Year == Convert.ToInt32(movimentacaoMaterial.Empenho.Substring(0, 4)))
                        {
                            primeiroEventoReclassificacao = eventoReclassificacaoExercicioAtual;
                            primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAtual;
                        }
                        else if ((movimentacaoMaterial.DataMovimento.GetValueOrDefault() > DateTime.MinValue) && 
                                 (movimentacaoMaterial.DataMovimento.GetValueOrDefault().Year == Convert.ToInt32(movimentacaoMaterial.Empenho.Substring(0, 4)))
                                )
                        {
                            primeiroEventoReclassificacao = eventoReclassificacaoExercicioAtual;
                            primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAtual;
                        }
                        else if ((DateTime.Now.Year - 1) == Convert.ToInt32(movimentacaoMaterial.Empenho.Substring(0, 4)))
                        {
                            primeiroEventoReclassificacao = eventoReclassificacaoExercicioAnterior;
                            primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAnterior;
                            terceiroEventoReclassificacao = terceiroReclassificacaoExercicioAnteriores;
                            terceiroEventoEstornoReclassificacao = terceiroEstornoReclassificacaoExercicioAnteriores;
                        }
                        else
                        {
                            primeiroEventoReclassificacao = eventoReclassificacaoExerciciosAnteriores;
                            primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExerciciosAnteriores;
                            terceiroEventoReclassificacao = terceiroReclassificacaoExercicioAnteriores;
                            terceiroEventoEstornoReclassificacao = terceiroEstornoReclassificacaoExercicioAnteriores;
                        }
                        
                        segundoEventoReclassificacao = 540541;
                        segundoEventoEstornoReclassificacao = 545541;
                        segundoEventoClassificacao = "331119906";
                        terceiroEventoInscricao = movimentacaoMaterial.InscricaoCE;

                        eventosPagamento = new EventosPagamentoEntity()
                        {
                            Ativo = true,
                            PrimeiroCodigo = primeiroEventoReclassificacao,
                            PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                            SegundoCodigo = segundoEventoReclassificacao,
                            SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                            TerceiroCodigo = terceiroEventoReclassificacao,
                            TerceiroCodigoEstorno = terceiroEventoEstornoReclassificacao,
                            //FIXOS
                            PrimeiraInscricao = movimentacaoMaterial.Empenho,
                            PrimeiraClassificacao = null,
                            SegundaInscricao = inscricaoCE,
                            SegundaClassificacao = segundoEventoClassificacao, //"115610102",
                            TerceiroInscricao = terceiroEventoInscricao,
                            UGFavorecida = false,
                            TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                            TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
                        };


                        break;
                    }

                case (int)TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho:
                    primeiroEventoReclassificacao = eventoReclassificacaoExercicioAnterior;
                    primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExercicioAnterior;

                    eventosPagamento = new EventosPagamentoEntity()
                    {
                        Ativo = true,
                        PrimeiroCodigo = primeiroEventoReclassificacao,
                        PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                        SegundoCodigo = segundoEventoReclassificacao,
                        SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                        //FIXOS
                        PrimeiraInscricao = movimentacaoMaterial.Empenho,
                        PrimeiraClassificacao = null,
                        SegundaInscricao = inscricaoCE,
                        SegundaClassificacao = "115610102",
                        UGFavorecida = false,
                        TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                        TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
                    };

                    break;

                default:
                    primeiroEventoReclassificacao = eventoReclassificacaoExerciciosAnteriores;
                    primeiroEventoEstornoReclassificacao = eventoEstornoReclassificacaoExerciciosAnteriores;

                    eventosPagamento = new EventosPagamentoEntity()
                    {
                        Ativo = true,
                        PrimeiroCodigo = primeiroEventoReclassificacao,
                        PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                        SegundoCodigo = segundoEventoReclassificacao,
                        SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                        //FIXOS
                        PrimeiraInscricao = movimentacaoMaterial.Empenho,
                        PrimeiraClassificacao = null,
                        SegundaInscricao = inscricaoCE,
                        SegundaClassificacao = "115610102",
                        UGFavorecida = false,
                        TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                        TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
                    };

                    break;
            }



            
            return eventosPagamento;
        }
        private EventosPagamentoEntity obterEventosPagamentoReclassificacao_MaterialPermanente(MovimentoEntity movimentacaoMaterial, string inscricaoCE)
        {
            EventosPagamentoEntity eventosPagamento = null;

            /*
                540469/540470 para o exercício atual 
                540169/540170 para o exercício anterior
                540171/540172 para o exercício anteriores
            */

            int primeiroEventoReclassificacao = 0;
            int primeiroEventoEstornoReclassificacao = 0;
            int segundoEventoReclassificacao = 0;
            int segundoEventoEstornoReclassificacao = 0;


            /*
                 1 – reclassificação       >NL                       540470            545470                NE                 Não
                                                                     540481            545481               CE                 123110812

                 RECLASSIFICAÇÃO PARA O EXERCÍCIO ANTERIOR
                 1 – reclassificação  >NL                       540170            545470                NE                 Não
                                                                540481            545481               CE                 123110812


                 RECLASSIFICAÇÃO PARA O EXERCÍCIO ANTERIORES

                 1 – reclassificação       >NL              540172            545470                NE                 Não
                                                            540481            545481               CE                 123110812


             */
            //VARIAM DE ACORDO COM A SITUACAO DO EMPENHO, EM RELACAO AO EXERCICIO ATUAL
            //PRIMEIRO PAR DE EVENTOS
            int primeiroEventoReclassificacaoExercicioAtual = 540470;
            int primeiroEventoEstornoReclassificacaoExercicioAtual = 545470;

            int primeiroEventoReclassificacaoExercicioAnterior = 540170;
            int primeiroEventoEstornoReclassificacaoExercicioAnterior = 545470;

            int primeiroEventoReclassificacaoExerciciosAnteriores = 540172;
            int primeiroEventoEstornoReclassificacaoExerciciosAnteriores = 545470;

            //SEGUNDO PAR DE EVENTOS
            int segundoEventoReclassificacaoExercicioAtual = 540481;
            int segundoEventoEstornoReclassificacaoExercicioAtual = 545481;

            int segundoEventoReclassificacaoExercicioAnterior = 540481;
            int segundoEventoEstornoReclassificacaoExercicioAnterior = 545481;

            int segundoEventoReclassificacaoExerciciosAnteriores = 540481;
            int segundoEventoEstornoReclassificacaoExerciciosAnteriores = 545481;


            if (movimentacaoMaterial.TipoMovimento.Id == (int)TipoMovimento.EntradaPorRestosAPagar)
            {
                primeiroEventoReclassificacao = primeiroEventoReclassificacaoExercicioAnterior;
                primeiroEventoEstornoReclassificacao = primeiroEventoEstornoReclassificacaoExercicioAnterior;

                segundoEventoReclassificacao = segundoEventoReclassificacaoExercicioAnterior;
                segundoEventoEstornoReclassificacao = segundoEventoEstornoReclassificacaoExercicioAnterior;
            }
            else if (movimentacaoMaterial.TipoMovimento.Id == (int)TipoMovimento.EntradaPorEmpenho)
            {
                primeiroEventoReclassificacao = primeiroEventoReclassificacaoExercicioAtual;
                primeiroEventoEstornoReclassificacao = primeiroEventoEstornoReclassificacaoExercicioAtual;

                segundoEventoReclassificacao = segundoEventoReclassificacaoExercicioAtual;
                segundoEventoEstornoReclassificacao = segundoEventoEstornoReclassificacaoExercicioAtual;
            }
            else if (movimentacaoMaterial.TipoMovimento.Id == (int)TipoMovimento.EntradaPorRestosAPagarConsumoImediatoEmpenho)
            {
                primeiroEventoReclassificacao = primeiroEventoReclassificacaoExercicioAnterior;
                primeiroEventoEstornoReclassificacao = primeiroEventoEstornoReclassificacaoExercicioAnterior;

                segundoEventoReclassificacao = segundoEventoReclassificacaoExercicioAnterior;
                segundoEventoEstornoReclassificacao = segundoEventoEstornoReclassificacaoExercicioAnterior;
            }
            else
            {
                segundoEventoReclassificacao = primeiroEventoReclassificacaoExerciciosAnteriores;
                segundoEventoEstornoReclassificacao = primeiroEventoEstornoReclassificacaoExerciciosAnteriores;

                segundoEventoReclassificacao = segundoEventoReclassificacaoExerciciosAnteriores;
                segundoEventoEstornoReclassificacao = segundoEventoEstornoReclassificacaoExerciciosAnteriores;
            }


            eventosPagamento = new EventosPagamentoEntity()
            {
                Ativo = true,
                PrimeiroCodigo = primeiroEventoReclassificacao,
                PrimeiroCodigoEstorno = primeiroEventoEstornoReclassificacao,
                SegundoCodigo = segundoEventoReclassificacao,
                SegundoCodigoEstorno = segundoEventoEstornoReclassificacao,
                //FIXOS
                PrimeiraInscricao = movimentacaoMaterial.Empenho,
                PrimeiraClassificacao = null,
                SegundaInscricao = inscricaoCE,
                SegundaClassificacao = "123110812",
                UGFavorecida = false,
                TipoMovimentoAssociado = movimentacaoMaterial.TipoMovimento,
                TipoMaterialAssociado = (int)movimentacaoMaterial.TipoMaterial
            };

            return eventosPagamento;
        }

        public EventosPagamentoEntity ObterEventoPagamento(TipoMovimento tipoMovimento)
        {
            EventosPagamentoEntity objRetorno = null;

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<IEventosPagamentoService>().Entity = this.Entity;
                    objRetorno = this.Service<IEventosPagamentoService>().ObterEventoPagamento(tipoMovimento);
                }
                catch (Exception excErroConsulta)
                {
                    throw excErroConsulta;
                }

                ts.Complete();
            }

            return objRetorno;
        }
    }
}
