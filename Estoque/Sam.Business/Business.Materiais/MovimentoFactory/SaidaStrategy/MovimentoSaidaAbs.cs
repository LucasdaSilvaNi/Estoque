using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Diagnostics;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System;

namespace Sam.Business.MovimentoFactory
{
    internal abstract class MovimentoSaidaAbs : IMovimentoSaidaStrategy
    {
        private MovimentoInfrastructure infraMovimento = new MovimentoInfrastructure();
        private SaldoSubItemInfrastructure infraSaldo = new SaldoSubItemInfrastructure();
        private SaldoSubItemBusiness businessSaldo = new SaldoSubItemBusiness();
        private MovimentoBusiness businessMovimento = new MovimentoBusiness();

        /// <summary>
        /// Executa a saida de material
        /// </summary>
        /// <param name="movimento">Movimento de entrada</param>
        public void SairMaterial(MovimentoEntity movimento)
        {
            try
            {
                TB_MOVIMENTO tbMovimento = InserirMovimento(movimento);

                foreach (var tbMovItem in tbMovimento.TB_MOVIMENTO_ITEM)
                {
                    var saldo = businessSaldo.RetornaSaldoExistente(tbMovItem);

                    if (saldo == null)
                    {
                        throw new Exception("Não existe saldo para a saida do item");
                    }
                    else
                    {
                        AtualizarSaldo(businessSaldo.UpdateSaldoSubtrair(tbMovItem, saldo));
                    }
                }

                SalvarContexto();
            }
            catch (NullReferenceException exNull)
            {
                throw exNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Executa o Estorno de Materiais para o movimento de Entrada
        /// </summary>
        /// <param name="movimento">Movimento para ser estornado</param>
        public void EstornarSaidaMaterial(MovimentoEntity movimento)
        {
            int loginId = 0; //colocar como parametro
            //Retorna todos os Itens do movimento
            infraMovimento.LazyLoadingEnabled = true;

            try
            {
                TB_MOVIMENTO tbMovimento = this.infraMovimento.SelectOne(a => a.TB_MOVIMENTO_ID == movimento.Id);

                foreach (var movItem in tbMovimento.TB_MOVIMENTO_ITEM)
                {
                    movItem.TB_MOVIMENTO_ITEM_ATIVO = false;

                    //Atualiza o Saldo
                    var saldo = businessSaldo.RetornaSaldoExistente(movItem);

                    if (saldo == null)
                    {
                        throw new Exception("Não existe saldo para estornar a Saida");
                    }
                    else
                    {
                        AtualizarSaldo(businessSaldo.UpdateSaldoSomar(movItem, saldo));
                    }
                }

                //Desativa o Movimento
                tbMovimento.TB_MOVIMENTO_ATIVO = false;
                tbMovimento.TB_LOGIN_ID_ESTORNO = loginId;
                tbMovimento.TB_MOVIMENTO_DATA_ESTORNO = DateTime.Now;

                this.infraMovimento.Update(tbMovimento);

                SalvarContexto();
            }
            catch (NullReferenceException exNull)
            {
                throw exNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AtualizarSaldo(TB_SALDO_SUBITEM tbSaldo)
        {
            infraSaldo.Update(tbSaldo);
        }

        private TB_MOVIMENTO InserirMovimento(MovimentoEntity movimento)
        {
            var business = new MovimentoBusiness();

            //Retorna a entidade TB_MOVIMENTO e TB_MOVIMENTO_ITEM preenchida
            TB_MOVIMENTO tbMovimento = business.SetEntidade(movimento);

            //Faz um agrupamento de itens identicos e soma os valores e quantidades
            tbMovimento = business.AgrupaMovimentoItems(tbMovimento);

            infraMovimento.Insert(tbMovimento);

            return tbMovimento;
        }

        private void InserirSaldo(TB_MOVIMENTO_ITEM tbMovItem)
        {
            TB_SALDO_SUBITEM tbSaldo = new SaldoSubItemBusiness().SetEntidade(tbMovItem);
            infraSaldo.Insert(tbSaldo);
        }

        private void SalvarContexto()
        {
            infraMovimento.SaveChanges();
            infraSaldo.SaveChanges();
        }
    }
}
