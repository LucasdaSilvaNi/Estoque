using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;
using Sam.Common.Util;
using Sam.ServiceInfraestructure;

namespace Sam.Domain.Business
{
    public class MovimentoRetroativoBusiness : MovimentoRetroativo
    {
        public override IList<MovimentoItemEntity> listaMovimentoItemErro { set; get; }
        public override IList<MovimentoItemEntity> listaPendencias { set; get; }
        public override bool Correcao { set; get; }
        public override bool gerarRelatorio { set; get; }
        public override Boolean Estorno { get; set; }

        private MovimentoItemEntity movimentoItemEntityCalculado = null;
        MovimentoBusiness business = new MovimentoBusiness();

        private MovimentoRetroativoBusiness(MovimentoItemEntity movimento)
            : base(movimento)
        {

        }

        public static MovimentoRetroativoBusiness setMovimentoRetroativoBusiness(MovimentoItemEntity movimento)
        {
            return new MovimentoRetroativoBusiness(movimento);
        }

        public override void setListaMovimento(IMovimentoService service)
        {
            this.ListaMovimento = service.RetornaListaTodosMovimentos(this.movimento, Estorno);

        }

        public override List<MovimentoItemEntity> setListaMovimentoSIAFEM(IMovimentoService service)
        {
            this.ListaMovimentoSIAFEM = service.RetornaListaTodosMovimentos(this.movimento, Estorno);
            return this.ListaMovimentoSIAFEM.ToList();

        }

        public override void setListaMovimentoLote(IMovimentoService service, MovimentoItemEntity movimento)
        {
            this.ListaMovimentoLote = service.RetornaListaTodosMovimentosLote(movimento, Estorno);
        }


        public IList<MovimentoItemEntity> RecalculoLote(IMovimentoService service, int indice)
        {
            movimentoItemEntityCalculado = new MovimentoItemEntity();
            string IdentLote = service.Entity.MovimentoItem[indice].IdentificacaoLote.ToString();
            DateTime? primeiraDataMovimentoLote = service.primeiraDataMovimentoDoSubItemDoAlmoxarifado(this.movimento.Almoxarifado.Id.Value, this.movimento.UGE.Id.Value, this.movimento.SubItemMaterial.Id.Value, IdentLote);

            //List<MovimentoItemEntity> listaLote = this.ListaMovimentoLote.Where(x => x.IdentificacaoLote == IdentLote).ToList();

            int cont = 0;

            foreach (var itemLote in this.ListaMovimentoLote)
            {
                if (itemLote.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                {
                    if (cont == 0)
                    {
                        if (this.movimento.Movimento.DataMovimento <= primeiraDataMovimentoLote)
                        {
                            movimentoItemEntityCalculado.SaldoQtdeLote = itemLote.QtdeMov;
                            itemLote.SaldoQtdeLote = itemLote.QtdeMov;
                        }
                        else
                        {
                            movimentoItemEntityCalculado.SaldoQtdeLote = itemLote.SaldoQtdeLote;
                        }
                    }
                    else
                    {
                        movimentoItemEntityCalculado.SaldoQtdeLote += itemLote.QtdeMov;
                        itemLote.SaldoQtdeLote = movimentoItemEntityCalculado.SaldoQtdeLote;
                    }
                }
                else if (itemLote.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                {

                    Decimal? SaldoQtdeLote = (movimentoItemEntityCalculado.SaldoQtdeLote - itemLote.QtdeMov);
                    itemLote.SaldoQtdeLote = SaldoQtdeLote;
                    movimentoItemEntityCalculado.SaldoQtdeLote = SaldoQtdeLote;

                    if (SaldoQtdeLote < 0)
                    {
                        var objBusiness = new CatalogoBusiness();
                        objBusiness.SelectSubItemMaterial(itemLote.SubItemMaterial.Id.Value);
                        itemLote.SubItemMaterial = objBusiness.SubItemMaterial;

                        var objMovBusiness = new MovimentoBusiness();
                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível no Analítico para o SubItem! Data Movimento: {2}, Movimentação: {3}, Saldo Ficará: {4}, Lote: {5}", itemLote.SubItemCodigo, itemLote.SubItemDescricao, itemLote.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), itemLote.Movimento.NumeroDocumento, SaldoQtdeLote, itemLote.IdentificacaoLote));
                        //var _movConsulta = objMovBusiness.ObterMovimento(itemLote.Movimento.Id.Value);
                        //throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível para o SubItem! Data Movimento: {2}, Movimentação: {3}, Saldo Estoque: {4}, Lote: {5}", itemLote.SubItemCodigo, itemLote.SubItemDescricao, _movConsulta.DataMovimento.Value.ToString("dd/MM/yyyy"), _movConsulta.NumeroDocumento, SaldoQtdeLote, itemLote.IdentificacaoLote));
                    }
                }

                cont++;
            }

            return ListaMovimentoLote;

        }


        public IList<MovimentoItemEntity> RecalculoLoteCorrigir(IMovimentoService service, string lote)
        {
            movimentoItemEntityCalculado = new MovimentoItemEntity();
            DateTime? primeiraDataMovimentoLote = service.primeiraDataMovimentoDoSubItemDoAlmoxarifado(this.movimento.Almoxarifado.Id.Value, this.movimento.UGE.Id.Value, this.movimento.SubItemMaterial.Id.Value, lote);

            //List<MovimentoItemEntity> listaLote = this.ListaMovimentoLote.Where(x => x.IdentificacaoLote == IdentLote).ToList();

            int cont = 0;

            foreach (var itemLote in this.ListaMovimentoLote)
            {
                if (itemLote.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                {
                    if (cont == 0)
                    {
                        if (this.movimento.Movimento.DataMovimento <= primeiraDataMovimentoLote)
                        {
                            movimentoItemEntityCalculado.SaldoQtdeLote = itemLote.QtdeMov;
                            itemLote.SaldoQtdeLote = itemLote.QtdeMov;
                        }
                        else
                        {
                            movimentoItemEntityCalculado.SaldoQtdeLote = itemLote.SaldoQtdeLote;
                        }
                    }
                    else
                    {
                        movimentoItemEntityCalculado.SaldoQtdeLote += itemLote.QtdeMov;
                        itemLote.SaldoQtdeLote = movimentoItemEntityCalculado.SaldoQtdeLote;
                    }
                }
                else if (itemLote.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                {

                    Decimal? SaldoQtdeLote = (movimentoItemEntityCalculado.SaldoQtdeLote - itemLote.QtdeMov);
                    itemLote.SaldoQtdeLote = SaldoQtdeLote;
                    movimentoItemEntityCalculado.SaldoQtdeLote = SaldoQtdeLote;

                    if (SaldoQtdeLote < 0)
                    {
                        var objBusiness = new CatalogoBusiness();
                        objBusiness.SelectSubItemMaterial(itemLote.SubItemMaterial.Id.Value);
                        itemLote.SubItemMaterial = objBusiness.SubItemMaterial;

                        var objMovBusiness = new MovimentoBusiness();
                        var _movConsulta = objMovBusiness.ObterMovimento(itemLote.Movimento.Id.Value);
                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível no Analítico para o SubItem! Data Movimento: {2}, Movimentação: {3}, Saldo Ficará: {4}, Lote: {5}", itemLote.SubItemCodigo, itemLote.SubItemDescricao, _movConsulta.DataMovimento.Value.ToString("dd/MM/yyyy"), _movConsulta.NumeroDocumento, SaldoQtdeLote, itemLote.IdentificacaoLote));
                    }
                }

                cont++;
            }

            return ListaMovimentoLote;

        }
        public override Tuple<IList<MovimentoItemEntity>, bool> RecalculoSIAFEM(IMovimentoService service, int indice, MovimentoItemEntity movItem, string TipoMov)
        {

            Decimal? desdobro = 0;
            Decimal? desdobroPrimeiraSaida = null;
            movimentoItemEntityCalculado = new MovimentoItemEntity();
            Decimal? PrecoUnitario = null;
            bool adicionouDesdobro = false;
            bool EnviaSiafem = true;
            movItem.Movimento.IdTemp = movItem.Movimento.Id;
            int TempIdMov = 0;

            DateTime? primeiraDataMovimento = service.primeiraDataMovimentoDoSubItemDoAlmoxarifado(this.movimento.Almoxarifado.Id.Value, this.movimento.UGE.Id.Value, this.movimento.SubItemMaterial.Id.Value);

            
            if (movItem.Movimento.TipoMovimento == null)
                movItem.Movimento.TipoMovimento = new TipoMovimentoEntity();
            if (movItem.Movimento.TipoMovimento.TipoAgrupamento == null)
                movItem.Movimento.TipoMovimento.TipoAgrupamento = new TipoMovimentoAgrupamentoEntity();

            switch (TipoMov)
            {

                case "Entrada":
                case "Saida":

                    int index = this.ListaMovimentoSIAFEM.Count();
                    int? idMov = this.ListaMovimentoSIAFEM.OrderByDescending(a => a.Movimento.IdTemp).FirstOrDefault().Movimento.IdTemp;
                     TempIdMov = (int)idMov + 1;
                    movItem.Movimento.IdTemp = TempIdMov;
                    this.ListaMovimentoSIAFEM.Insert(index, movItem);

                    if(TipoMov == "Entrada")
                    movItem.Movimento.TipoMovimento.TipoAgrupamento.Id = (int)GeralEnum.TipoMovimentoAgrupamento.Entrada;
                    else             
                    movItem.Movimento.TipoMovimento.TipoAgrupamento.Id = (int)GeralEnum.TipoMovimentoAgrupamento.Saida;

                    break;

                case "Estorno":

                   

                    for (int i = 0; i < this.ListaMovimentoSIAFEM.Count; i++)
                    {                      
                        if (this.ListaMovimentoSIAFEM[i].Movimento.IdTemp == movItem.Movimento.IdTemp)
                        {
                            int cont =  (from list in this.ListaMovimentoSIAFEM
                                         where list.Movimento.IdTemp == Convert.ToInt32(movItem.Movimento.IdTemp)
                                            select list).Count();
                                

                            for (int y = 0; y < cont; y++)
                            {
                                if (this.ListaMovimentoSIAFEM[i].Movimento.IdTemp == movItem.Movimento.IdTemp)
                                this.ListaMovimentoSIAFEM.RemoveAt(i);
                            }
                                                     
                        }

                    }


                    break;

                default:
                  

                    break;
            }





            this.ListaMovimentoSIAFEM = this.ListaMovimentoSIAFEM.OrderBy(a => a.Movimento.IdTemp).OrderBy(b => b.DataMovimento).ToList();

            foreach (MovimentoItemEntity item in this.ListaMovimentoSIAFEM)
            {
                //bool NLexiste = (item.NL_Liquidacao.IsNotNullAndNotEmpty() || item.NL_LiquidacaoEstorno.IsNotNullAndNotEmpty() || item.NlConsumo.IsNotNullAndNotEmpty());
                bool NLexiste = (!String.IsNullOrWhiteSpace(item.NL_Liquidacao) || !String.IsNullOrWhiteSpace(item.NL_LiquidacaoEstorno) ||
                                 !String.IsNullOrWhiteSpace(item.NL_Reclassificacao) || !String.IsNullOrWhiteSpace(item.NL_ReclassificacaoEstorno) || !String.IsNullOrWhiteSpace(item.NL_Consumo));

                item.ValorOriginalDocumento = item.ValorMov.Value.truncarDuasCasas();

                if (this.ListaMovimentoSIAFEM.IndexOf(item) == 0)
                {
                    if (item.DataMovimento < new DateTime(2014, 08, 1))
                        throw new Exception(String.Format("Não é permitido fazer movimentação retroativa para o subitem {0}, pois a última entrada a acumular desdobro foi antes de 01/08/2014 (quando a regra de desdobro foi alterada)! Faça a saída deste subitem na data corrente!",item.SubItemCodigo));
                }

                
                if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                {
                    item.TipoMovimentoDescricao = "Entrada";
                    if (this.ListaMovimentoSIAFEM.IndexOf(item) == 0)
                    {
                        item.QtdeMov = item.QtdeMov == null ? 0 : item.QtdeMov;
                        item.ValorMov = item.QtdeMov == 0 ? 0 : item.ValorMov;

                        string anoMesReferencia;

                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        if (this.movimento.Movimento.DataMovimento <= primeiraDataMovimento)
                        {

                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.QtdeMov;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.ValorMov;

                            item.SaldoValor = movimentoItemEntityCalculado.ValorMov;
                            item.SaldoQtde = movimentoItemEntityCalculado.QtdeMov;

                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);



                        }
                        else
                        {

                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.SaldoQtde;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.SaldoValor;


                            item.SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                            item.SaldoQtde = movimentoItemEntityCalculado.SaldoQtde;


                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);



                        }


                        if (EnviaSiafem)
                        {
                            if (item.Movimento.IdTemp != TempIdMov)
                            {
                                if (item.ValorMov != item.ValorOriginalDocumento && NLexiste)
                                    EnviaSiafem = false;
                            }
                            else
                            {
                                item.ValorOriginalDocumento = item.ValorMov.Value.truncarDuasCasas();
                                movItem.ValorMov = item.ValorMov.Value.truncarDuasCasas();
                            }
                        }

                    }
                    else
                    {
                        string anoMesReferencia;

                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                        movimentoItemEntityCalculado.SaldoQtde += item.QtdeMov;
                        movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                        movimentoItemEntityCalculado.SaldoValor += item.ValorMov;

                        item.SaldoQtde = movimentoItemEntityCalculado.SaldoQtde;
                        item.SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                        item.ValorMov = movimentoItemEntityCalculado.ValorMov;

                        PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                        item.PrecoUnit = PrecoUnitario;
                        if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                            desdobro = service.AtualizarDesdobroSaldo(item, false);


                        if (EnviaSiafem)
                        {
                            if (item.Movimento.IdTemp != TempIdMov)
                            {
                                if (item.ValorMov != item.ValorOriginalDocumento && NLexiste)
                                    EnviaSiafem = false;
                            }
                            else
                            {
                                item.ValorOriginalDocumento = item.ValorMov.Value.truncarDuasCasas();
                                movItem.ValorMov = item.ValorMov.Value.truncarDuasCasas();
                            }
                        }


                    }
                    adicionouDesdobro = false;
                    item.Desd = null;
                    desdobroPrimeiraSaida = desdobro;
                }
                else if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                {
                    item.TipoMovimentoDescricao = "Saida";

                    if (this.ListaMovimentoSIAFEM.IndexOf(item) == 0)
                    {
                        movimentoItemEntityCalculado.SaldoValor = item.SaldoValor;
                        movimentoItemEntityCalculado.SaldoQtde = item.SaldoQtde;
                        item.PrecoUnit = item.PrecoUnitDtMov == null ? item.PrecoUnit : item.PrecoUnitDtMov;
                        PrecoUnitario = item.PrecoUnit;
                        if (item.PrecoUnit != null)
                            item.ValorOriginalDocumento = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                    }

                    Decimal? SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                    Decimal? SaldoQtde = (movimentoItemEntityCalculado.SaldoQtde - item.QtdeMov);

                    item.PrecoUnit = PrecoUnitario;

                    if (item.DataMovimento.Value < new DateTime(2014, 08, 1) && desdobroPrimeiraSaida.HasValue && desdobroPrimeiraSaida.Value > 0)
                    {
                        item.Desd = desdobroPrimeiraSaida;
                        adicionouDesdobro = true;
                    }
                    else
                        item.Desd = null;

                    decimal? ValorMovAnterior = item.ValorMov;

                    if (this.ListaMovimentoSIAFEM.IndexOf(item) == this.ListaMovimentoSIAFEM.Count - 1 && SaldoQtde == 0 || (item.DataMovimento.Value >= new DateTime(2014, 08, 1) && SaldoQtde == 0))
                    {

                        if (movimentoItemEntityCalculado.SaldoQtde == item.QtdeMov)
                        {
                            desdobro = SaldoValor - (item.QtdeMov * item.PrecoUnit);

                            item.ValorMov = (item.QtdeMov * item.PrecoUnit) + desdobro.Value;

                            SaldoValor -= item.ValorMov;
                            item.Desd = desdobro.Value;


                        }
                        else
                            if (item.Desd.HasValue && item.Desd.Value > 0)
                            {
                                item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                                SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            }
                            else
                            {
                                item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                                SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            }
                    }
                    else
                        if (item.Desd.HasValue && item.Desd.Value > 0)
                        {
                            item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                        }
                        else
                        {
                            item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                        }

                    if (SaldoValor < 0 || SaldoQtde < 0)
                    {
                        var objBusiness = new CatalogoBusiness();
                        objBusiness.SelectSubItemMaterial(item.SubItemMaterial.Id.Value);
                        item.SubItemMaterial = objBusiness.SubItemMaterial;

                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível no Analítico para o SubItem! Data Movimento: {2}, Movimentação: {3}, Saldo Ficará: {4}, Valor Estoque: {5}", item.SubItemCodigo, item.SubItemDescricao, item.Movimento.DataMovimento.Value.ToString("dd/MM/yyyy"), item.Movimento.NumeroDocumento, SaldoQtde, SaldoValor));
                    }

                    item.SaldoQtde = SaldoQtde;
                    item.SaldoValor = SaldoValor;

                    movimentoItemEntityCalculado.SaldoQtde = SaldoQtde;
                    movimentoItemEntityCalculado.SaldoValor = SaldoValor;

                    desdobroPrimeiraSaida = null;


                    if (EnviaSiafem)
                    {
                        if (item.Movimento.IdTemp != TempIdMov)
                        {
                            if (item.ValorMov != item.ValorOriginalDocumento && NLexiste)
                                EnviaSiafem = false;
                        }
                        else
                        {
                            item.ValorOriginalDocumento = item.ValorMov.Value.truncarDuasCasas();
                            movItem.ValorMov = item.ValorMov.Value.truncarDuasCasas();
                        }
                    }



                }

                //Caso o saldo fique negativo, o sistema para o processo de recalculo
                business.ValidaSaldoPositivo(item);

                // atualiza o saldo na tabela de movimento item
                item.Desd = desdobro;


                if (adicionouDesdobro)
                    desdobro = 0;
            }
            return new Tuple<IList<MovimentoItemEntity>, bool>(this.ListaMovimentoSIAFEM, EnviaSiafem);

        }

        public override void Recalculo(IMovimentoService service, int indice)
        {

            Decimal? desdobro = 0;
            Decimal? desdobroPrimeiraSaida = null;
            movimentoItemEntityCalculado = new MovimentoItemEntity();
            Decimal? PrecoUnitario = null;
            bool adicionouDesdobro = false;


            var listaLote = RecalculoLote(service, indice);
            DateTime? primeiraDataMovimento = service.primeiraDataMovimentoDoSubItemDoAlmoxarifado(this.movimento.Almoxarifado.Id.Value, this.movimento.UGE.Id.Value, this.movimento.SubItemMaterial.Id.Value);


            foreach (MovimentoItemEntity item in this.ListaMovimento)
            {
                if (this.ListaMovimento.IndexOf(item) == 0)
                {
                    if (item.DataMovimento < new DateTime(2014, 08, 1))
                        throw new Exception(String.Format("Não é permitido fazer movimentação retroativa para o subitem {0}, pois a última entrada a acumular desdobro foi antes de 01/08/2014 (quando a regra de desdobro foi alterada)! Faça a saída deste subitem na data corrente!", item.SubItemCodigo));
                }


                foreach (var itemLote in listaLote)
                {
                    if (itemLote.Id == item.Id)
                        item.SaldoQtdeLote = itemLote.SaldoQtdeLote;
                }

                if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                {
                    if (this.ListaMovimento.IndexOf(item) == 0)
                    {
                        string anoMesReferencia; //= item.AnoMesReferencia;

                        //if (item.DataMovimento.Value >= new DateTime(2014, 08, 1))
                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        if (this.movimento.Movimento.DataMovimento <= primeiraDataMovimento)
                        {
                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.QtdeMov;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.ValorMov;

                            item.SaldoValor = movimentoItemEntityCalculado.ValorMov;
                            item.SaldoQtde = movimentoItemEntityCalculado.QtdeMov;

                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);

                        }
                        else
                        {

                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.SaldoQtde;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.SaldoValor;


                            item.SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                            item.SaldoQtde = movimentoItemEntityCalculado.SaldoQtde;


                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);
                        }

                    }
                    else
                    {
                        string anoMesReferencia; //= item.AnoMesReferencia;

                        //if (item.DataMovimento.Value.Year >= 2014 && item.DataMovimento.Value.Month > 7)
                        //if (item.DataMovimento.Value >= new DateTime(2014, 08, 1))
                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                        movimentoItemEntityCalculado.SaldoQtde += item.QtdeMov;
                        movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                        movimentoItemEntityCalculado.SaldoValor += item.ValorMov;

                        item.SaldoQtde = movimentoItemEntityCalculado.SaldoQtde;
                        item.SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                        item.ValorMov = movimentoItemEntityCalculado.ValorMov;

                        PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                        item.PrecoUnit = PrecoUnitario;
                        if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                            desdobro = service.AtualizarDesdobroSaldo(item, false);


                    }
                    adicionouDesdobro = false;
                    item.Desd = null;
                    desdobroPrimeiraSaida = desdobro;
                }
                else if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                {


                    Decimal? SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                    Decimal? SaldoQtde = (movimentoItemEntityCalculado.SaldoQtde - item.QtdeMov);

                    item.PrecoUnit = PrecoUnitario;

                    if (item.DataMovimento.Value < new DateTime(2014, 08, 1) && desdobroPrimeiraSaida.HasValue && desdobroPrimeiraSaida.Value > 0)
                    {
                        item.Desd = desdobroPrimeiraSaida;
                        adicionouDesdobro = true;
                    }
                    else
                        item.Desd = null;

                    decimal? ValorMovAnterior = item.ValorMov;

                    //if (this.ListaMovimento.IndexOf(item) == this.ListaMovimento.Count - 1)
                    if (this.ListaMovimento.IndexOf(item) == this.ListaMovimento.Count - 1 && SaldoQtde == 0 || (item.DataMovimento.Value >= new DateTime(2014, 08, 1) && SaldoQtde == 0))
                    {
                        var saldoQtde = service.GetSaldoQuantidade(item);

                        if (saldoQtde == item.QtdeMov)
                        {
                            desdobro = SaldoValor - (item.QtdeMov * item.PrecoUnit);

                            item.ValorMov = (item.QtdeMov * item.PrecoUnit) + desdobro.Value;

                            SaldoValor -= item.ValorMov;
                            item.Desd = desdobro.Value;


                        }
                        else
                            if (item.Desd.HasValue && item.Desd.Value > 0)
                            {
                                item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                                SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            }
                            else
                            {
                                item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                                SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            }
                    }
                    else
                        if (item.Desd.HasValue && item.Desd.Value > 0)
                        {
                            item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                        }
                        else
                        {
                            item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                        }

                    if (SaldoValor < 0 || SaldoQtde < 0)
                    {
                        var objBusiness = new CatalogoBusiness();
                        objBusiness.SelectSubItemMaterial(item.SubItemMaterial.Id.Value);
                        item.SubItemMaterial = objBusiness.SubItemMaterial;

                        var objMovBusiness = new MovimentoBusiness();
                        var _movConsulta = objMovBusiness.ObterMovimento(item.Movimento.Id.Value);
                        throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, verifique a quantidade disponível no Analítico para o SubItem! Data Movimento: {2}, Movimentação: {3}, Saldo Ficará: {4}, Valor Estoque: {5}", item.SubItemCodigo, item.SubItemDescricao, _movConsulta.DataMovimento.Value.ToString("dd/MM/yyyy"), _movConsulta.NumeroDocumento, SaldoQtde, SaldoValor));
                    }

                    item.SaldoQtde = SaldoQtde;
                    item.SaldoValor = SaldoValor;

                    movimentoItemEntityCalculado.SaldoQtde = SaldoQtde;
                    movimentoItemEntityCalculado.SaldoValor = SaldoValor;

                    desdobroPrimeiraSaida = null;

                    TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity(service.getTipoMovimento(item.Movimento.Id.Value));


                    if (tipoMovimento.Id == (int)Common.Util.GeralEnum.TipoMovimento.SaidaPorTransferencia)
                    {
                        if (service.ListarDocumentoByDocTransf(item.Movimento.NumeroDocumento.ToString()) > 0)
                        {
                            if (item.ValorMov != ValorMovAnterior)
                            {
                                throw new Exception(String.Format("Essa movimentação alterar o Valor da Saída por Transferência documento {0}.", item.Movimento.NumeroDocumento));
                                return;
                            }
                        }

                    }

                   

                }

                //Caso o saldo fique negativo, o sistema para o processo de recalculo
                business.ValidaSaldoPositivo(item);

                if (item.PrecoUnit == 0)
                    throw new Exception("Erro ao calcular preço unitário, problema na conexão, saia do sistema e entre novamente");

                // atualiza o saldo na tabela de movimento item
                service.AtualizarMovimentoItem(item);
                item.Desd = desdobro;

                service.AtualizarSaldoMovimentoDoSubItem(item);

                if (adicionouDesdobro)
                    desdobro = 0;
            }
        }

        public override void Corrigir(IMovimentoService service, int indice)
        {
            Decimal? desdobro = 0;
            Decimal? desdobroPrimeiraSaida = null;
            movimentoItemEntityCalculado = new MovimentoItemEntity();
            Decimal? PrecoUnitario = null;
            bool adicionouDesdobro = false;
            Decimal? desdobroRecalculo = null;
            MovimentoItemEntity movItem = new MovimentoItemEntity();
            movItem.Movimento = new MovimentoEntity();
            movItem.Movimento.Almoxarifado = new AlmoxarifadoEntity();
            movItem.UGE = new UGEEntity();
            movItem.SubItemMaterial = new SubItemMaterialEntity();

            var listaLote = new List<MovimentoItemEntity>();

            IList<MovimentoItemEntity> _listaMovimentoItemErro = new List<MovimentoItemEntity>();
            IList<MovimentoItemEntity> _listaMovimentoItemRelatorio = new List<MovimentoItemEntity>();

            DateTime? primeiraDataMovimento = null;

            var lote = this.ListaMovimento.Distinct().GroupBy(x => x.IdentificacaoLote).Select(g => new
            {
                Lote = g.ToList().First().IdentificacaoLote,
                DataVenc = g.ToList().First().DataVencimentoLote
            }).ToList();

            foreach (var item in lote)
            {
                movItem.IdentificacaoLote = item.Lote;
                movItem.Movimento.Almoxarifado.Id = this.movimento.Almoxarifado.Id.Value;
                movItem.UGE.Id = this.movimento.UGE.Id.Value;
                movItem.SubItemMaterial.Id = this.movimento.SubItemMaterial.Id.Value;
                movItem.DataMovimento = service.Entity.DataMovimento;
                movItem.Movimento.DataMovimento = service.Entity.DataMovimento;


                setListaMovimentoLote(this.Service<IMovimentoService>(), movItem);
                listaLote = RecalculoLoteCorrigir(service, item.Lote).ToList();

                foreach (MovimentoItemEntity itemLM in this.ListaMovimento)
                {
                    int qdteLote = listaLote.Count();

                    int count = 1;

                    foreach (var itemLote in listaLote)
                    {
                        if (itemLote.Id == itemLM.Id)
                        {
                            itemLM.SaldoQtdeLote = itemLote.SaldoQtdeLote;

                            if (count == qdteLote)
                            {
                                movItem.DataVencimentoLote = item.DataVenc;
                                movItem.QtdeMov = itemLote.SaldoQtdeLote;

                                int IdSaldo = this.Service<IMovimentoService>().ExisteSaldoId(movItem);
                                if (IdSaldo > 0)
                                    this.Service<IMovimentoService>().InserirAtualizarSaldoLoteItemCorrigir(movItem, IdSaldo);
                                else
                                    this.Service<IMovimentoService>().InserirAtualizarSaldoLoteItemCorrigir(movItem, null);

                            }
                        }

                        count++;
                    }
                }

            }



            primeiraDataMovimento = service.primeiraDataMovimentoDoSubItemDoAlmoxarifado(this.movimento.Almoxarifado.Id.Value, this.movimento.UGE.Id.Value, this.movimento.SubItemMaterial.Id.Value);

            if (primeiraDataMovimento == null)
                return;

         
            foreach (MovimentoItemEntity item in this.ListaMovimento)
            {
                if (this.ListaMovimento.IndexOf(item) == 0)
                {
                    if (item.DataMovimento < new DateTime(2014, 08, 1))
                        throw new Exception(String.Format("Não é permitido fazer movimentação retroativa para o subitem {0}, pois a última entrada a acumular desdobro foi antes de 01/08/2014 (quando a regra de desdobro foi alterada)! Faça a saída deste subitem na data corrente!", item.SubItemCodigo));
                }


                foreach (var itemLote in listaLote)
                {
                    if (itemLote.Id == item.Id)
                        item.SaldoQtdeLote = itemLote.SaldoQtdeLote;
                }

                TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity(service.getTipoMovimento(item.Movimento.Id.Value));
                item.Movimento.TipoMovimento.Id = tipoMovimento.Id;

                if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada)
                {
                    if (this.ListaMovimento.IndexOf(item) == 0)
                    {
                        string anoMesReferencia; //= item.AnoMesReferencia;

                        //if (item.DataMovimento.Value >= new DateTime(2014, 08, 1))
                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        if (this.movimento.Movimento.DataMovimento <= primeiraDataMovimento)
                        {

                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.QtdeMov;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.ValorMov;

                            item.SaldoValor = movimentoItemEntityCalculado.ValorMov;
                            item.SaldoQtde = movimentoItemEntityCalculado.QtdeMov;

                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);

                        }
                        else
                        {
                            PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                            item.PrecoUnit = PrecoUnitario;

                            movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                            movimentoItemEntityCalculado.SaldoQtde = item.SaldoQtde;
                            movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                            movimentoItemEntityCalculado.SaldoValor = item.SaldoValor;

                            if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                                desdobro = service.AtualizarDesdobroSaldo(item, false);
                        }

                    }
                    else
                    {
                        string anoMesReferencia; //= item.AnoMesReferencia;

                        anoMesReferencia = item.Movimento.DataMovimento.Value.Year.ToString() + item.Movimento.DataMovimento.Value.Month.ToString("00");

                        movimentoItemEntityCalculado.QtdeMov = item.QtdeMov;
                        movimentoItemEntityCalculado.SaldoQtde += item.QtdeMov;
                        movimentoItemEntityCalculado.ValorMov = item.ValorMov;
                        movimentoItemEntityCalculado.SaldoValor += item.ValorMov;

                        item.SaldoQtde = movimentoItemEntityCalculado.SaldoQtde;
                        item.SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                        item.ValorMov = movimentoItemEntityCalculado.ValorMov;

                        PrecoUnitario = service.CalcularPrecoMedioSaldo(item.SaldoValor.Value, item.SaldoQtde.Value, anoMesReferencia);
                        item.PrecoUnit = PrecoUnitario;
                        if (item.DataMovimento.Value < new DateTime(2014, 08, 1))
                            desdobro = service.AtualizarDesdobroSaldo(item, false);


                    }
                    adicionouDesdobro = false;
                    item.Desd = null;
                    desdobroPrimeiraSaida = desdobro;
                }
                else if (item.Movimento.TipoMovimento.TipoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                {
                    Decimal? SaldoValor = movimentoItemEntityCalculado.SaldoValor;
                    Decimal? SaldoQtde = (movimentoItemEntityCalculado.SaldoQtde - item.QtdeMov);

                    item.PrecoUnit = PrecoUnitario;

                    if (item.DataMovimento.Value < new DateTime(2014, 08, 1) && desdobroPrimeiraSaida.HasValue && desdobroPrimeiraSaida.Value > 0)
                    {
                        item.Desd = desdobroPrimeiraSaida;
                        adicionouDesdobro = true;
                        desdobroRecalculo = desdobroPrimeiraSaida;
                    }
                    else
                        item.Desd = null;



                    //if (this.ListaMovimento.IndexOf(item) == this.ListaMovimento.Count - 1)
                    if (this.ListaMovimento.IndexOf(item) == this.ListaMovimento.Count - 1 && SaldoQtde == 0 || (item.DataMovimento.Value >= new DateTime(2014, 08, 1) && SaldoQtde == 0))
                    {
                        var saldoQtde = service.GetSaldoQuantidade(item);

                        if (saldoQtde == item.QtdeMov)
                        {
                            desdobro = SaldoValor - (item.QtdeMov * item.PrecoUnit);

                            item.ValorMov = (item.QtdeMov * item.PrecoUnit) + desdobro.Value;

                            SaldoValor -= item.ValorMov;
                            item.Desd = desdobro.Value;
                            desdobroRecalculo = desdobro.Value;

                        }
                        else
                            if (item.Desd.HasValue && item.Desd.Value > 0)
                            {
                                item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                                SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            }
                            else
                            {
                                item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                                SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            }
                    }
                    else
                        if (item.Desd.HasValue && item.Desd.Value > 0)
                        {
                            item.ValorMov = ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                            SaldoValor -= ((item.QtdeMov * item.PrecoUnit) + item.Desd.Value).Value.truncarDuasCasas();
                        }
                        else
                        {
                            item.ValorMov = (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                            SaldoValor -= (item.QtdeMov * item.PrecoUnit).Value.truncarDuasCasas();
                        }

                    if (SaldoValor < 0 || SaldoQtde < 0)
                    {
                        if (!gerarRelatorio)
                        {
                            if (Correcao)
                            {
                                service.AtualizarMovimentoItem(item);
                                var objBusiness = new CatalogoBusiness();
                                objBusiness.SelectSubItemMaterial(item.SubItemMaterial.Id.Value);
                                item.SubItemMaterial = objBusiness.SubItemMaterial;

                                _listaMovimentoItemErro.Add(item);
                                break;
                            }
                            else
                            {
                                throw new Exception(String.Format("Quantidade Indisponível para o SubItem {0} - {1}, Verifique a quantidade disponível para o SubItem!", item.SubItemCodigo, item.SubItemDescricao));
                            }
                        }
                    }

                    if (tipoMovimento.Id == 12)
                        if (!gerarRelatorio)
                            if (Correcao)
                            {
                                service.AtualizarMovimentoItem(item);
                                var objBusiness = new CatalogoBusiness();
                                objBusiness.SelectSubItemMaterial(item.SubItemMaterial.Id.Value);
                                item.SubItemMaterial = objBusiness.SubItemMaterial;
                                _listaMovimentoItemErro.Add(item);

                            }

                    item.SaldoQtde = SaldoQtde;
                    item.SaldoValor = SaldoValor;

                    movimentoItemEntityCalculado.SaldoQtde = SaldoQtde;
                    movimentoItemEntityCalculado.SaldoValor = SaldoValor;

                    desdobroPrimeiraSaida = null;
                }

                try
                {
                    //Caso o saldo fique negativo, o sistema para o processo de recalculo
                    //business.ValidaSaldoPositivo(item);
                }
                catch (Exception ex)
                {
                    if (Correcao)
                    {
                        _listaMovimentoItemErro.Add(item);
                        break;
                    }
                }

                if (!gerarRelatorio)
                {
                    // atualiza o saldo na tabela de movimento item
                    service.AtualizarMovimentoItem(item);

                    item.Desd = desdobro;
                    service.AtualizarSaldoMovimentoDoSubItem(item);
                }

                if (gerarRelatorio)
                {
                    service.AtualizarMovimentoItem(item);
                    var objBusiness = new CatalogoBusiness();
                    objBusiness.SelectSubItemMaterial(item.SubItemMaterial.Id.Value);
                    item.SubItemMaterial = objBusiness.SubItemMaterial;

                    item.Desd = desdobroRecalculo;
                    this.listaPendencias.Add(item);
                }
                if (adicionouDesdobro)
                    desdobro = 0;

                desdobroRecalculo = null;
            }

            this.listaMovimentoItemErro = _listaMovimentoItemErro;
        }

        public override void gerarPendencias(IMovimentoService service, IList<MovimentoItemEntity> listaPendencias, int indice)
        {
            this.gerarRelatorio = true;
            this.Correcao = false;
            this.listaPendencias = listaPendencias;
            Corrigir(service, indice);
        }
    }
}
