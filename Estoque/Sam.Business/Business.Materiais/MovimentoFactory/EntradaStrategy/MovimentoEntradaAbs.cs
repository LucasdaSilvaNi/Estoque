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
    internal abstract class MovimentoEntradaAbs : IMovimentoEntradaStrategy
    {
        private MovimentoInfrastructure infraMovimento = new MovimentoInfrastructure();
        private SaldoSubItemInfrastructure infraSaldo = new SaldoSubItemInfrastructure();
        private SaldoSubItemBusiness businessSaldo = new SaldoSubItemBusiness();
        private MovimentoBusiness businessMovimento = new MovimentoBusiness();

        /// <summary>
        /// Executa a entrada de materiais
        /// </summary>
        /// <param name="movimento">Movimento de entrada</param>
        public virtual void EntrarMaterial(MovimentoEntity movimento)
        {
            TB_MOVIMENTO tbMovimento = new TB_MOVIMENTO();

            try
            {
                tbMovimento = InserirMovimento(movimento);

                foreach (var tbMovItem in tbMovimento.TB_MOVIMENTO_ITEM)
                {
                    var saldo = businessSaldo.RetornaSaldoExistente(tbMovItem);

                    if (saldo == null)//Se não existir saldo, insere um novo registro
                    {
                        InserirSaldo(tbMovItem);
                    }
                    else //Se existir saldo, realiza update
                    {
                        infraSaldo.Update(businessSaldo.UpdateSaldoSomar(tbMovItem, saldo));
                    }
                    SalvarContexto();
                }
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
        /// Estorna a Entrada de Material
        /// </summary>
        /// <param name="movimento">Movimento/MovimentoItem a ser estornado</param>
        /// 
        public virtual void EstornarEntradaMaterial(Domain.Entity.MovimentoEntity movimento)
        {
            TB_MOVIMENTO tbMovimento = new TB_MOVIMENTO();

            //Retorna todos os Itens do movimento
            infraMovimento.LazyLoadingEnabled = true;

            try
            {
                tbMovimento = this.infraMovimento.SelectOne(a => a.TB_MOVIMENTO_ID == movimento.Id);
                foreach (var movItem in tbMovimento.TB_MOVIMENTO_ITEM)
                {
                    movItem.TB_MOVIMENTO_ITEM_ATIVO = false;

                    //Atualiza o Saldo
                    var saldo = businessSaldo.RetornaSaldoExistente(movItem);

                    if (saldo == null)
                    {
                        throw new Exception("Não existe saldo para estornar a Entrada");
                    }
                    else
                    {
                        AtualizarSaldo(businessSaldo.UpdateSaldoSubtrair(movItem, saldo));
                    }
                }
            }
            catch (NullReferenceException exNull)
            {
                throw exNull;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Desativa o Movimento
            tbMovimento.TB_MOVIMENTO_ATIVO = false;
            this.infraMovimento.Update(tbMovimento);

            SalvarContexto();
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
