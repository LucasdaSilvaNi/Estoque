using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Transactions;

namespace Sam.Domain.Business
{
    public class SaldoSubItemBusiness : BaseBusiness
    {
        private SaldoSubItemEntity saldoSubItem = new SaldoSubItemEntity();

        public SaldoSubItemEntity SaldoSubItem { get; set; }

        private bool VerificarLote(SaldoSubItemEntity saldo)
        {
            if (saldo.LoteDataVenc == null && string.IsNullOrEmpty(saldo.LoteIdent) && saldo.LoteDataVenc == null)
                return false;
            else
                return true;
        }

        private bool VerificarLote(MovimentoItemEntity saldo)
        {
            if (saldo.DataVencimentoLote == null && string.IsNullOrEmpty(saldo.IdentificacaoLote) && string.IsNullOrEmpty(saldo.FabricanteLote))
                return false;
            else
                return true;
        }

        public SaldoSubItemEntity CalcularSaldoSubItemComESemLote(List<MovimentoItemEntity> saldoSubItens, MovimentoItemEntity item)
        {
            try
            {
                SaldoSubItemEntity saldoTotalComLote = (from a in saldoSubItens
                                                        where
                                                            a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
                                                            a.UGE.Id == item.UGE.Id &&
                                                            a.Movimento.Almoxarifado.Id == item.Movimento.Almoxarifado.Id
                                                        group a by new { a.SubItemMaterial.Id }
                                                            into sss
                                                            select new SaldoSubItemEntity
                                                            {
                                                                Id = 0,
                                                                SubItemMaterial = sss.FirstOrDefault().SubItemMaterial,
                                                                SaldoQtde = sss.Sum(a => a.SaldoQtde),
                                                                SaldoValor = sss.Sum(a => a.SaldoValor)
                                                            }).FirstOrDefault<SaldoSubItemEntity>();

                if (saldoTotalComLote == null)
                {
                    saldoTotalComLote = new SaldoSubItemEntity();
                    saldoTotalComLote.SubItemMaterial = new SubItemMaterialEntity(item.SubItemMaterial.Id);
                    saldoTotalComLote.UGE = new UGEEntity(item.UGE.Id);
                    saldoTotalComLote.SaldoQtde = 0;
                    saldoTotalComLote.SaldoValor = 0;
                    saldoTotalComLote.PrecoUnit = 0;
                }

                return saldoTotalComLote;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MovimentoItemEntity SaldoAcumulado(IList<MovimentoItemEntity> movimentoItemList, MovimentoItemEntity movItem)
        {
            MovimentoItemEntity result = null;

            try
            {
                result = (from mi in movimentoItemList
                          where mi.SubItemMaterial.Id == movItem.SubItemMaterial.Id
                          group mi by new
                          {
                              ugeid = mi.UGE.Id,
                              subItemId = mi.SubItemMaterial.Id,
                              almoxId = mi.Movimento.Almoxarifado.Id,
                              //mi.DataVencimentoLote,
                              //mi.IdentificacaoLote,
                              //mi.FabricanteLote,

                          } into g
                          select new MovimentoItemEntity
                          {
                              SaldoQtde = g.Sum(s => s.QtdeMov),
                              SaldoValor = g.Sum(s => s.ValorMov),
                          }).First();

            }
            catch (Exception excErroSaldo)
            {
                throw new Exception("Erro ao executar método .SaldoAcumulado(...)", excErroSaldo);
            }

            return result;

        }

        public MovimentoItemEntity SaldoAcumuladoLote(IList<MovimentoItemEntity> movimentoItemList, MovimentoItemEntity movItem)
        {
            MovimentoItemEntity result = null;

            try
            {
                result = (from mi in movimentoItemList
                          where mi.SubItemMaterial.Id == movItem.SubItemMaterial.Id
                          && mi.IdentificacaoLote == movItem.IdentificacaoLote
                          group mi by new
                          {
                              ugeid = mi.UGE.Id,
                              subItemId = mi.SubItemMaterial.Id,
                              almoxId = mi.Movimento.Almoxarifado.Id,
                          } into g
                          select new MovimentoItemEntity
                          {
                              SaldoQtdeLote = g.Sum(s => s.QtdeMov)
                          }).First();

            }
            catch (Exception excErroSaldo)
            {
                throw new Exception("Erro ao executar método .SaldoAcumulado(...)", excErroSaldo);
            }

            return result;

        }

        public bool VerificaSubItemUtilizado(int subItemId)
        {
            return this.Service<ISaldoSubItemService>().VerificaSubItemUtilizado(subItemId);
        }

        public void gravarLogSaldoNulo(MovimentoEntity mov, string classe, string metodo)
        {

            try
            {
                //string pathArquivoXml = System.Environment.CurrentDirectory + @"\lOGsaldo.xml";  //System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process) + @"\lOGsaldo.xml"; //\Relatorios\lOGsaldo.xml;

                StringBuilder arquivoXml = new StringBuilder();
                XmlWriter xmlMontadorEstimulo = null;
                XmlWriterSettings xmlSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };


                xmlMontadorEstimulo = XmlWriter.Create(arquivoXml, xmlSettings);
                xmlMontadorEstimulo.WriteStartDocument(false);
                xmlMontadorEstimulo.WriteStartElement("Movimento");
                xmlMontadorEstimulo.WriteElementString("MovimentoId", mov.Id.ToString());
                xmlMontadorEstimulo.WriteElementString("TipoMovimento", mov.TipoMovimento.Descricao);
                xmlMontadorEstimulo.WriteElementString("DataDocumento", (mov.DataDocumento.HasValue ? mov.DataDocumento.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("DataMovimento", (mov.DataMovimento.HasValue ? mov.DataMovimento.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("DataOperacao", (mov.DataOperacao.HasValue ? mov.DataOperacao.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("ValorDocumento", (mov.ValorDocumento.HasValue ? mov.ValorDocumento.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("Classe", classe);
                xmlMontadorEstimulo.WriteElementString("Metodo", metodo);
                foreach (var item in mov.MovimentoItem)
                {

                    xmlMontadorEstimulo.WriteStartElement("MovimentoItem");
                    xmlMontadorEstimulo.WriteElementString("MovimentoItemId", item.Id.ToString());
                    xmlMontadorEstimulo.WriteElementString("ItemMaterialCodigo", item.ItemMaterialCodigo);
                    xmlMontadorEstimulo.WriteElementString("QtdeMov", (item.QtdeMov.HasValue ? item.QtdeMov.Value.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("ValorMov", (item.ValorMov.HasValue ? item.ValorMov.Value.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("SaldoQtde", (item.SaldoQtde.HasValue ? item.SaldoQtde.Value.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("SaldoValor", (item.SaldoValor.HasValue ? item.SaldoValor.Value.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("SubItemCodigo", (item.SubItemCodigo.HasValue ? item.SubItemCodigo.Value.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("SubItemMaterialId", (item.SubItemMaterial.Id.HasValue ? item.SubItemMaterial.Id.ToString() : "Nulo"));
                    xmlMontadorEstimulo.WriteElementString("SubItemDescricao", item.SubItemDescricao);

                    //movItem.SubItemMaterial.Id

                    xmlMontadorEstimulo.WriteEndElement();

                }


                xmlMontadorEstimulo.WriteEndElement();
                xmlMontadorEstimulo.WriteEndDocument();

                xmlMontadorEstimulo.Flush();
                xmlMontadorEstimulo.Close();

                Exception exception = new Exception(arquivoXml.ToString());
                new LogErro().GravarLogErro(exception);

                exception = null;

            }
            catch (Exception ex)
            {

            }


        }
        public void gravarLogSaldoNulo(MovimentoItemEntity item, string classe, string metodo)
        {

            try
            {
                //string pathArquivoXml = System.Environment.CurrentDirectory + @"\lOGsaldo.xml";  //System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process) + @"\lOGsaldo.xml"; //\Relatorios\lOGsaldo.xml;

                StringBuilder arquivoXml = new StringBuilder();
                XmlWriter xmlMontadorEstimulo = null;
                XmlWriterSettings xmlSettings = new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8, OmitXmlDeclaration = true };


                xmlMontadorEstimulo = XmlWriter.Create(arquivoXml, xmlSettings);
                xmlMontadorEstimulo.WriteStartDocument(false);
                xmlMontadorEstimulo.WriteStartElement("Movimento");
                xmlMontadorEstimulo.WriteElementString("MovimentoId", item.Id.ToString());
                xmlMontadorEstimulo.WriteElementString("Classe", classe);
                xmlMontadorEstimulo.WriteElementString("Metodo", metodo);
                xmlMontadorEstimulo.WriteStartElement("MovimentoItem");
                xmlMontadorEstimulo.WriteElementString("MovimentoItemId", item.Id.ToString());
                xmlMontadorEstimulo.WriteElementString("ItemMaterialCodigo", item.ItemMaterialCodigo);
                xmlMontadorEstimulo.WriteElementString("QtdeMov", (item.QtdeMov.HasValue ? item.QtdeMov.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("ValorMov", (item.ValorMov.HasValue ? item.ValorMov.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("SaldoQtde", (item.SaldoQtde.HasValue ? item.SaldoQtde.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("SaldoValor", (item.SaldoValor.HasValue ? item.SaldoValor.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("SubItemCodigo", (item.SubItemCodigo.HasValue ? item.SubItemCodigo.Value.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("SubItemMaterialId", (item.SubItemMaterial.Id.HasValue ? item.SubItemMaterial.Id.ToString() : "Nulo"));
                xmlMontadorEstimulo.WriteElementString("SubItemDescricao", item.SubItemDescricao);


                xmlMontadorEstimulo.WriteEndElement();
                xmlMontadorEstimulo.WriteEndElement();
                xmlMontadorEstimulo.WriteEndDocument();

                xmlMontadorEstimulo.Flush();
                xmlMontadorEstimulo.Close();


                Exception exception = new Exception(arquivoXml.ToString());
                new LogErro().GravarLogErro(exception);

                exception = null;


            }
            catch (Exception ex)
            {

            }


        }

        public void ConsistirSaldoSubItemEntrada(MovimentoEntity mov)
        {


            try
            {
                List<MovimentoItemEntity> MovimentoItemAcumulado = new List<MovimentoItemEntity>();

                //Para cada item do movimento
                foreach (var item in mov.MovimentoItem)
                {
                    item.Movimento = mov;
                    MovimentoItemAcumulado.Add(item);

                    //Inicializa o saldo com zero caso esteja nulo
                    if (item.SaldoValor == null)
                        item.SaldoValor = 0;

                    if (item.SaldoQtde == null)
                        item.SaldoQtde = 0;

                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        var movimentoService = this.Service<IMovimentoService>();
                        movimentoService.Entity = mov;

                        try
                        {
                            //Retorna o saldo do movimentoItem
                            var saldoMovimentoItem = movimentoService.RetornaPrecoMedioMovimentoItemRetroativo(item);
                            var saldoMovimentoItemLote = movimentoService.RetornaPrecoMedioMovimentoItemRetroativoLote(item);
                            var saldoAcumulado = SaldoAcumulado(MovimentoItemAcumulado, item);
                            var saldoAcumuladoLote = SaldoAcumuladoLote(MovimentoItemAcumulado, item);

                            item.Desd = 0; //Entrada sempre desdobro 0

                            if (saldoMovimentoItem == null)
                            {
                                if (!saldoAcumulado.SaldoQtde.HasValue)
                                {
                                    gravarLogSaldoNulo(mov, "SaldoSubItemBusiness", "ConsistirSaldoSubItemEntrada");

                                }
                                //Não existe saldo anterior
                                item.SaldoQtde = saldoAcumulado.SaldoQtde;
                                item.SaldoValor = saldoAcumulado.SaldoValor;
                                item.SaldoQtdeLote = saldoAcumuladoLote.SaldoQtdeLote;
                            }
                            else
                            {
                                if (!saldoAcumulado.SaldoQtde.HasValue)
                                {

                                    gravarLogSaldoNulo(mov, "SaldoSubItemBusiness", "ConsistirSaldoSubItemEntrada");
                                }
                                //Existe saldo anterior
                                item.SaldoQtde = saldoAcumulado.SaldoQtde + saldoMovimentoItem.SaldoQtde;
                                item.SaldoValor = saldoAcumulado.SaldoValor + saldoMovimentoItem.SaldoValor;
                                if (saldoMovimentoItemLote != null)
                                    item.SaldoQtdeLote = saldoAcumuladoLote.SaldoQtdeLote + saldoMovimentoItemLote.SaldoQtdeLote;
                                else
                                    item.SaldoQtdeLote = saldoAcumuladoLote.SaldoQtdeLote;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message, ex.InnerException);
                        }
                        finally
                        {
                            ts.Complete();
                        }

                    }
                    //Atualiza preço médio
                    using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
                    {
                        try
                        {
                            item.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(item.SaldoValor, item.SaldoQtde, item.AnoMesReferencia);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message, ex.InnerException);
                        }
                        finally
                        {
                            ts.Complete();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
            }
        }

        //public bool ConsistirSaldoSubItemEntrada(MovimentoEntity mov) {

        //    try
        //    {
        //        //List<SaldoSubItemEntity> saldoSubItens = new List<SaldoSubItemEntity>();

        //        List<MovimentoItemEntity> saldoSubItens = new List<MovimentoItemEntity>();
        //        //SaldoSubItemEntity saldo = null;
        //        SaldoSubItemEntity saldoTotalComLote = null;
        //        MovimentoItemEntity saldoMovimentoItem = new MovimentoItemEntity();
        //        var movimentoService = this.Service<IMovimentoService>();
        //        movimentoService.Entity = mov;


        //        foreach (MovimentoItemEntity item in mov.MovimentoItem)
        //        {
        //            //Atualiza o movimento
        //            item.Movimento = mov;

        //            if(item.QtdeMov.HasValue && item.QtdeMov != 0)
        //            {
        //                // procurar por subitem-uge-almox por lote
        //                //saldo = ConsultarSaldoSubItem(item.SubItemMaterial.Id, mov.Almoxarifado.Id, item.UGE.Id, item.IdentificacaoLote, item.FabricanteLote, item.DataVencimentoLote);
        //                //saldoMovimentoItem =

        //                // somar por subitem-uge-almox com e sem lote
        //                    // OK - agrupou

        //                var saldoMovimento = movimentoService.RetornaPrecoMedioMovimentoItemRetroativo(item);


        //                item.Movimento = new MovimentoEntity 
        //                                    { 
        //                                        Almoxarifado = new AlmoxarifadoEntity(mov.Almoxarifado.Id),
        //                                    };

        //                item.UGE = new UGEEntity(mov.UGE.Id);
        //                Service<IMovimentoService>().CalcularSaldoTotal(item);
        //                saldoTotalComLote = CalcularSaldoSubItemComESemLote(saldoSubItens, item);

        //                // fazer somatoria
        //                if (saldoMovimento != null)
        //                {
        //                    saldoMovimento.DataVencimentoLote = item.DataVencimentoLote;
        //                    saldoMovimento.FabricanteLote = item.FabricanteLote;
        //                    saldoMovimento.IdentificacaoLote = item.IdentificacaoLote;

        //                    if (!VerificarLote(saldoMovimento) && saldoSubItens.Where(a => a.SubItemMaterial.Id == saldoMovimento.SubItemMaterial.Id).Count() > 0)
        //                    {
        //                        saldoSubItens.Where(a => a.SubItemMaterial.Id == saldoMovimento.SubItemMaterial.Id).FirstOrDefault().SaldoQtde += item.QtdeMov;
        //                        saldoSubItens.Where(a => a.SubItemMaterial.Id == saldoMovimento.SubItemMaterial.Id).FirstOrDefault().SaldoValor += item.ValorMov; // item.QtdeMov * item.PrecoUnit;

        //                        item.SaldoQtde += item.QtdeMov;
        //                        item.SaldoValor += item.SaldoValor;
        //                        //item.SaldoQtde = saldoTotalComLote.SaldoQtde + item.QtdeMov;
        //                        //item.SaldoValor = saldoTotalComLote.SaldoQtde + item.SaldoValor;
        //                        saldoMovimento.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(item.SaldoValor, item.SaldoQtde);
        //                        item.Desd = 0;
        //                    }
        //                    else
        //                    {
        //                        saldoMovimento.SaldoQtde += item.QtdeMov;
        //                        saldoMovimento.SaldoValor += item.ValorMov.Value;
        //                        item.SaldoQtde += item.QtdeMov;
        //                        item.SaldoValor += item.ValorMov;
        //                        //item.SaldoQtde = saldo.SaldoQtde;
        //                        //item.SaldoValor = saldo.SaldoValor;

        //                        // identifica preço médio
        //                        saldoMovimento.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(item.SaldoValor, item.SaldoQtde);
        //                        item.Desd = 0;

        //                        if (saldoSubItens.Where(a => a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
        //                                                 a.UGE.Id == item.UGE.Id &&
        //                                                 a.Movimento.Almoxarifado.Id == mov.Almoxarifado.Id).Count() == 0)
        //                            saldoSubItens.Add(saldoMovimento);
        //                        else
        //                        {
        //                            saldoSubItens.Remove(saldoSubItens.Where(a => a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
        //                                                                             a.UGE.Id == item.UGE.Id &&
        //                                                                             a.Movimento.Almoxarifado.Id == mov.Almoxarifado.Id &&
        //                                                                             a.DataVencimentoLote == item.DataVencimentoLote &&
        //                                                                             a.FabricanteLote == item.FabricanteLote &&
        //                                                                             a.IdentificacaoLote == item.IdentificacaoLote).FirstOrDefault());
        //                            saldoSubItens.Add(saldoMovimento);
        //                        }

        //                    }
        //                }
        //                // para novos registros de saldo
        //                else
        //                {
        //                    saldoMovimento = saldoSubItens.Where(a => a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
        //                                                a.UGE.Id == item.UGE.Id &&
        //                                                a.Movimento.Almoxarifado.Id == mov.Almoxarifado.Id &&
        //                                                a.DataVencimentoLote == item.DataVencimentoLote &&
        //                                                a.FabricanteLote == item.FabricanteLote &&
        //                                                a.IdentificacaoLote == item.IdentificacaoLote).FirstOrDefault();
        //                    if (saldoMovimento == null)
        //                    {
        //                        saldoMovimento = new MovimentoItemEntity();
        //                        saldoMovimento.SaldoQtde = 0;
        //                        saldoMovimento.SaldoValor = 0;
        //                    }

        //                    saldoMovimento.Movimento = new MovimentoEntity();
        //                    saldoMovimento.Movimento.Almoxarifado = new AlmoxarifadoEntity(mov.Almoxarifado.Id);
        //                    saldoMovimento.SubItemMaterial = new SubItemMaterialEntity(item.SubItemMaterial.Id.Value);
        //                    saldoMovimento.UGE = new UGEEntity(item.UGE.Id);
        //                    saldoMovimento.DataVencimentoLote = item.DataVencimentoLote;
        //                    saldoMovimento.FabricanteLote = item.FabricanteLote;
        //                    saldoMovimento.IdentificacaoLote = item.IdentificacaoLote;

        //                    // procura o saldo do item SEM LOTE
        //                    saldoMovimento.SaldoQtde += item.QtdeMov;
        //                    saldoMovimento.SaldoValor += item.ValorMov.Value;

        //                    // soma com o saldo de TODO O SUBITEM no campo de movimentação itens
        //                    item.SaldoQtde = saldoTotalComLote.SaldoQtde + item.QtdeMov;
        //                    item.SaldoValor = saldoTotalComLote.SaldoValor + item.ValorMov.Value;

        //                    // identifica preço médio
        //                    saldoMovimento.PrecoUnit = Service<IMovimentoService>().CalcularPrecoMedioSaldo(item.SaldoValor, item.SaldoQtde);
        //                    item.Desd = 0;

        //                    if (saldoSubItens.Where(a => a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
        //                                             a.UGE.Id == item.UGE.Id &&
        //                                             a.Movimento.Almoxarifado.Id == mov.Almoxarifado.Id).Count() == 0)
        //                        saldoSubItens.Add(saldoMovimento);
        //                    else
        //                    {
        //                        saldoSubItens.Remove(saldoSubItens.Where(a => a.SubItemMaterial.Id == item.SubItemMaterial.Id.Value &&
        //                                                                         a.UGE.Id == item.UGE.Id &&
        //                                                                         a.Movimento.Almoxarifado.Id == mov.Almoxarifado.Id &&
        //                                                                         a.DataVencimentoLote == item.DataVencimentoLote &&
        //                                                                         a.FabricanteLote == item.FabricanteLote &&
        //                                                                         a.IdentificacaoLote == item.IdentificacaoLote).FirstOrDefault());
        //                        saldoSubItens.Add(saldoMovimento);
        //                    }
        //                }

        //                item.PrecoUnit = saldoMovimento.PrecoUnit;
        //            }
        //        }

        //        //this.Service<ISaldoSubItemService>().Salvar(saldoSubItens);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        new LogErro().GravarLogErro(e);
        //        this.ListaErro.Add(e.Message);
        //        return false;
        //    }
        //}

        public IList<SaldoSubItemEntity> AnalisarFechamentoMensal(int? almoxId, int? anomes)
        {
            IList<SaldoSubItemEntity> lstRetorno = this.Service<ISaldoSubItemService>().ListarFechamento(almoxId, anomes);

            CatalogoBusiness objBusiness = new CatalogoBusiness();
            objBusiness.VerificarSubitensInativos((int)almoxId);
            ListaErro = objBusiness.ListaErro;

            return lstRetorno;
        }

        public decimal? CalculaTotalSaldoUGEsReserva(int? subItemId, int? almoxId)
        {
            var saldo = this.Service<ISaldoSubItemService>().CalculaTotalSaldoUGEsReserva(subItemId, almoxId);
            return saldo <= _valorZero ? _valorZero : saldo;
        }

        public Tuple<decimal?, decimal?, decimal?> SaldoMovimentoItemDataMovimento(int? subItemId, int? almoxId, int? ugeId, DateTime dtMovimento)
        {

            Tuple<decimal?, decimal?, decimal?> saldo = this.Service<ISaldoSubItemService>().SaldoMovimentoItemDataMovimento(subItemId, almoxId, ugeId, dtMovimento);
            return saldo;
        }

        public IList<SaldoSubItemEntity> ImprimirConsultaEstoqueSintetico(int UgeId, int AlmoxId, int GrupoId, int ComSemSaldo, int? _ordenarPor)
        {
            this.Service<ISaldoSubItemService>().Entity = SaldoSubItem;
            return this.Service<ISaldoSubItemService>().ImprimirConsultaEstoqueSintetico(UgeId, AlmoxId, GrupoId, ComSemSaldo, _ordenarPor);
        }

        public IList<SubItemMaterialEntity> ImprimirConsumoSubitemMaterialAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            return this.Service<ISaldoSubItemService>().ImprimirConsumoAlmox(_almoxId, dataInicial, dataFinal, true);
        }

        public IList<SubItemMaterialEntity> ImprimirConsumoSubitemMaterialDivisao(int? _divisaoId, int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            return this.Service<ISaldoSubItemService>().ImprimirConsumoDivisao(_divisaoId, _almoxId, dataInicial, dataFinal, true);
        }

        public IList<SubItemMaterialEntity> ImprimirPrevisaoConsumoSubitemMaterialAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            return this.Service<ISaldoSubItemService>().ImprimirPrevisaoConsumoAlmox(_almoxId, dataInicial, dataFinal, true);
        }

        public IList<UGEEntity> ConsultarUgesBySubItemAlmox(int almoxarifado, int subItem, int ugeId)
        {

            return this.Service<ISaldoSubItemService>().ConsultarUgesBySubItemAlmox(almoxarifado, subItem, ugeId);
        }


        public IList<SaldoSubItemEntity> ListarSaldoSubItem(SaldoSubItemEntity subItem)
        {
            this.Service<ISaldoSubItemService>().Entity = SaldoSubItem;
            return this.Service<ISaldoSubItemService>().Listar();
        }

        public bool ConsistirSaldoSubItemSaida(MovimentoEntity mov)
        {

            try
            {
                List<SaldoSubItemEntity> saldoSubItens = new List<SaldoSubItemEntity>();
                SaldoSubItemEntity saldo = null;
                SaldoSubItemEntity saldoTotalComLote = null;

                foreach (MovimentoItemEntity item in mov.MovimentoItem)
                {

                    saldoTotalComLote = ConsultarSaldoSubItem(new AlmoxarifadoEntity(mov.Almoxarifado.Id), new UGEEntity(item.UGE.Id), new SubItemMaterialEntity(item.SubItemMaterial.Id));
                    if (saldoTotalComLote == null)
                    {
                        saldoTotalComLote = new SaldoSubItemEntity();
                        saldoTotalComLote.SaldoQtde = 0;
                        saldoTotalComLote.SaldoValor = 0;
                    }

                    saldo = ConsultarSaldoSubItem(item.SubItemMaterial.Id, mov.Almoxarifado.Id, mov.UGE.Id, item.IdentificacaoLote, item.FabricanteLote, item.DataVencimentoLote);
                    // fazer somatoria
                    if (saldo != null)
                    {
                        saldo.SaldoQtde -= item.QtdeMov;
                        if (saldo.SaldoQtde < 0)
                        {
                            this.ListaErro.Add("Saldo insuficiente: " + saldo.SubItemMaterial.CodigoDescricao);
                        }

                        if (!VerificarLote(saldo) && saldoSubItens.Where(a => a.SubItemMaterial.Id == saldo.SubItemMaterial.Id).Count() > 0)
                        {
                            saldoSubItens.Where(a => a.SubItemMaterial.Id == saldo.SubItemMaterial.Id).FirstOrDefault().SaldoQtde += item.QtdeMov;
                            saldoSubItens.Where(a => a.SubItemMaterial.Id == saldo.SubItemMaterial.Id).FirstOrDefault().SaldoValor += item.ValorMov; // item.QtdeMov * item.PrecoUnit;
                            item.SaldoQtde = saldoTotalComLote.SaldoQtde - item.QtdeMov;
                            item.SaldoValor = saldoTotalComLote.SaldoQtde - item.SaldoValor;
                            saldo.PrecoUnit = (saldoTotalComLote.SaldoValor) / (saldoTotalComLote.SaldoQtde);
                            saldo.PrecoUnit = saldoTotalComLote.PrecoUnit.Value.Truncar(mov.AnoMesReferencia, true);
                            item.Desd = 0;
                        }
                        else
                        {
                            // calcula o desdobro e ajusta para o valor
                            decimal? desd = saldo.SaldoValor - (saldo.PrecoUnit * saldo.SaldoQtde);
                            saldo.PrecoUnit = saldo.PrecoUnit + desd;
                            saldo.SaldoValor -= item.QtdeMov * item.PrecoUnit;
                            item.SaldoQtde = saldoTotalComLote.SaldoQtde + item.QtdeMov;
                            item.SaldoValor = saldoTotalComLote.SaldoQtde + item.SaldoValor;
                            saldo.PrecoUnit = (saldoTotalComLote.SaldoValor) / (saldoTotalComLote.SaldoQtde);
                            saldo.PrecoUnit = saldoTotalComLote.PrecoUnit.Value.Truncar(mov.AnoMesReferencia, true);
                            item.Desd = 0;
                            saldoSubItens.Add(saldo);
                        }
                    }
                    else
                    {
                        saldo = new SaldoSubItemEntity();
                        saldo.Almoxarifado = new AlmoxarifadoEntity(mov.Almoxarifado.Id);
                        saldo.SubItemMaterial = new SubItemMaterialEntity(item.SubItemMaterial.Id.Value);
                        saldo.UGE = new UGEEntity(mov.UGE.Id);
                        saldo.LoteDataVenc = item.DataVencimentoLote;
                        saldo.LoteFabr = item.FabricanteLote;
                        saldo.LoteIdent = item.IdentificacaoLote;
                        saldo.PrecoUnit = item.PrecoUnit;
                        saldo.SaldoValor = item.SaldoValor;
                        saldo.PrecoUnit = saldoTotalComLote.PrecoUnit.Value.Truncar(mov.AnoMesReferencia, true);
                        item.Desd = 0;
                        saldoSubItens.Add(saldo);
                    }
                    // recalcular custo médio a partir do zero (por subitem, almox e uge)
                }

                if (this.ListaErro.Count > 0)
                    return false;

                this.Service<ISaldoSubItemService>().Salvar(saldoSubItens);
                return true;
            }
            catch (Exception e)
            {
                new LogErro().GravarLogErro(e);
                this.ListaErro.Add(e.Message);
                return false;
            }
        }

        public void AtualizarSaldoSubItens(MovimentoEntity mov)
        {
            int contador = 0;
            if (mov.MovimentoItem.Count > 0)
            {
                foreach (MovimentoItemEntity movItem in mov.MovimentoItem)
                {
                    SaldoSubItemEntity saldo = this.Service<ISaldoSubItemService>().Consultar(movItem.SubItemMaterial.Id,
                        mov.Almoxarifado.Id, mov.UGE.Id,
                        movItem.IdentificacaoLote,
                        movItem.FabricanteLote,
                        movItem.DataVencimentoLote);
                    if (saldo == null)
                    {
                        saldo = new SaldoSubItemEntity();
                        saldo.UGE = new UGEEntity(mov.UGE.Id);
                        saldo.Almoxarifado = new AlmoxarifadoEntity(mov.Almoxarifado.Id);
                        saldo.LoteDataVenc = movItem.DataVencimentoLote;
                        saldo.LoteFabr = movItem.FabricanteLote;
                        saldo.LoteIdent = movItem.IdentificacaoLote;
                        saldo.PrecoUnit = movItem.PrecoUnit;
                        saldo.SaldoQtde = 0;
                        saldo.SaldoValor = 0;
                        saldo.SubItemMaterial = new SubItemMaterialEntity(movItem.SubItemMaterial.Id.Value);
                    }

                    mov.MovimentoItem[contador].SaldoQtde = movItem.QtdeMov + (saldo.SaldoQtde ?? 0);
                    mov.MovimentoItem[contador].SaldoValor = saldo.SaldoValor + ((movItem.QtdeMov * movItem.PrecoUnit) ?? 0);
                    // reverte o valor caso estorne
                    if (movItem.QtdeMov < 0)
                        movItem.QtdeMov = -movItem.QtdeMov;
                    contador++;
                }
            }

        }

        public SaldoSubItemEntity ConsultarSaldoSubItem(int? SubItemMaterialId, int? AlmoxId, int? UgeId, string _LoteIdent, string fabricanteLote, DateTime? dataVencimentoLote)
        {
            return this.Service<ISaldoSubItemService>().Consultar(SubItemMaterialId, AlmoxId, UgeId, _LoteIdent, fabricanteLote, dataVencimentoLote);
        }

        public SaldoSubItemEntity ConsultarSaldoSubItem(AlmoxarifadoEntity almoxarifado, UGEEntity uge, SubItemMaterialEntity subItem)
        {
            return this.Service<ISaldoSubItemService>().ConsultarSaldoSubItem(almoxarifado, uge, subItem);
        }

        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? ugeId, int? almoxId)
        {
            IList<SaldoSubItemEntity> retorno = this.Service<ISaldoSubItemService>().ListarSaldoSubItemPorLote(subItemId, ugeId, almoxId);
            return FormatarLote(retorno);
        }

        public IList<SaldoSubItemEntity> ListarSaldoPorLote(int? subItemId, int? ugeId,int? almoxId)
        {
            IList<SaldoSubItemEntity> retorno = this.Service<ISaldoSubItemService>().ListarSaldoSubItemPorLote(subItemId, ugeId, almoxId);

            return FormatarLote(retorno);
        }
        public IList<SaldoSubItemEntity> ListarSaldoSubItemPorLote(int? subItemId, int? ugeId, int? almoxId, DateTime? dataMovimento)
        {
            IList<SaldoSubItemEntity> retorno = this.Service<ISaldoSubItemService>().ListarSaldoSubItemPorLote(subItemId,ugeId, almoxId);
            IList<SaldoSubItemEntity> retornoFiltro = new List<SaldoSubItemEntity>(); 
            if (dataMovimento != null)
            {               
                SaldoSubItemEntity retornoItem = new SaldoSubItemEntity();
                foreach (var item in retorno)
                {
                    var movimentoService = this.Service<IMovimentoService>();
                    decimal? valor = movimentoService.SaldoMovimentoItemLote(subItemId, almoxId, Convert.ToDateTime(dataMovimento), item.LoteIdent, item.LoteDataVenc, ugeId);

                    if (valor > 0) {
                        retornoItem = new SaldoSubItemEntity();
                        retornoItem.Id = item.Id;

                        retornoItem.LoteDataVenc = item.LoteDataVenc;
                        retornoItem.LoteIdent = item.LoteIdent;
                        retornoItem.IdLote = item.IdLote;
                        retornoItem.SaldoQtde = valor > item.SaldoQtde ? item.SaldoQtde : valor;
                        retornoFiltro.Add(retornoItem);
                    }
                }
            }

            return FormatarLote(retornoFiltro);
        }

        public IList<SaldoSubItemEntity> FormatarLote(IList<SaldoSubItemEntity> retorno)
        {
            foreach (var saldo in retorno)
            {
                StringBuilder sb = new StringBuilder();
                saldo.SaldoQtde = (saldo.SaldoQtde == null || saldo.SaldoQtde < 0) ? 0.000m : saldo.SaldoQtde.Value;

                if (saldo.LoteDataVenc != null || !String.IsNullOrEmpty(saldo.LoteIdent) || !String.IsNullOrEmpty(saldo.LoteFabr))
                {
                    if (saldo.LoteDataVenc != null)
                        sb.AppendFormat("Dt Venc: {0}", saldo.LoteDataVenc.Value.ToShortDateString());
                    else
                        sb.Append("Dt Venc: N/I");

                    if (!String.IsNullOrEmpty(saldo.LoteIdent))
                        sb.AppendFormat(" - Lote: {0}", saldo.LoteIdent.ToString());
                    else
                        sb.Append(" - Lote: N/I");

                    //TODO: Ver este trecho com a Nanci
                    //                    if (!String.IsNullOrEmpty(saldo.LoteFabr))
                    //                        sb.AppendFormat(" - Fabr: {0}", saldo.LoteFabr.ToString());
                    //                    else
                    //                        sb.Append(" - Fabr: N/I");
                }
                else
                {
                    sb.Append("Não Informado");
                }

                saldo.SaldoQtde = (saldo.SaldoQtde == null || saldo.SaldoQtde < 0 ? 0 : saldo.SaldoQtde);

                if (saldo.SaldoQtde != null)
                    sb.AppendFormat(" - Saldo: {0}", saldo.SaldoQtde.Value.ToString(base.fmtFracionarioMaterialQtde));
                else
                    sb.Append(" - Saldo: 0");

                saldo.CodigoDescricao = sb.ToString();
            }

            return retorno;
        }

    }
}
