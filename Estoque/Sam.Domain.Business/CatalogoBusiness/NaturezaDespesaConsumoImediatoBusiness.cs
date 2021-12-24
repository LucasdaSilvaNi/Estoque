using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;
using System.Transactions;
using Sam.Domain.Infrastructure;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Naturezas de Despesa - Consumo Imediato

        private NaturezaDespesaConsumoImediatoEntity naturezaDespesaConsumoImediato = new NaturezaDespesaConsumoImediatoEntity();

        public NaturezaDespesaConsumoImediatoEntity NaturezaDespesaConsumoImediato
        {
            get { return naturezaDespesaConsumoImediato; }
            set { naturezaDespesaConsumoImediato = value; }
        }

        public bool SalvarNaturezaDespesaConsumoImediato()
        {
            this.Service<INaturezaDespesaConsumoImediatoService>().Entity = this.NaturezaDespesaConsumoImediato;
            this.ConsistirNaturezaDespesaConsumoImediato();
            if (this.Consistido)
            {
                this.Service<INaturezaDespesaConsumoImediatoService>().Salvar();
            }
            return this.Consistido;
        }

        //public IList<NaturezaDespesaConsumoImediatoEntity> ListarNaturezasDespesaConsumoImediato()
        //{
        //    this.Service<INaturezaDespesaConsumoImediatoService>().SkipRegistros = this.SkipRegistros;
        //    IList<NaturezaDespesaConsumoImediatoEntity> retorno = this.Service<INaturezaDespesaConsumoImediatoService>().Listar();
        //    this.TotalRegistros = this.Service<INaturezaDespesaConsumoImediatoService>().TotalRegistros();
        //    return retorno;
        //}

        public IList<NaturezaDespesaConsumoImediatoEntity> ListarNaturezasDespesaConsumoImediatoTodosCod()
        {
            this.Service<INaturezaDespesaConsumoImediatoService>().SkipRegistros = this.SkipRegistros;
            IList<NaturezaDespesaConsumoImediatoEntity> retorno = this.Service<INaturezaDespesaConsumoImediatoService>().ListarTodosCod();
            this.TotalRegistros = this.Service<INaturezaDespesaConsumoImediatoService>().TotalRegistros();

            return retorno;
        }

        public IList<NaturezaDespesaConsumoImediatoEntity> ImprimirNaturezasDespesaConsumoImediato()
        {
            IList<NaturezaDespesaConsumoImediatoEntity> retorno = this.Service<INaturezaDespesaConsumoImediatoService>().Imprimir();
            return retorno;
        }

        public bool ExcluirNaturezaDespesaConsumoImediato()
        {
            this.Service<INaturezaDespesaConsumoImediatoService>().Entity = this.NaturezaDespesaConsumoImediato;

            try
            {
                return this.Service<INaturezaDespesaConsumoImediatoService>().ExcluirNaturezaDespesaConsumoImediato();
            }
            catch(SqlException)
            {
                this.ListaErro = new List<string>() { "Impossível excluir registro. Natureza de Despesa possui alguns Itens de Material vinculados." };

                return false;
            }
        }

        public void ConsistirNaturezaDespesaConsumoImediato()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<NaturezaDespesaConsumoImediatoEntity>(ref this.naturezaDespesaConsumoImediato);

            if (this.NaturezaDespesaConsumoImediato.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar um código válido de Natureza de Despesa (Consumo Imediato).");

            if (string.IsNullOrEmpty(this.NaturezaDespesaConsumoImediato.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<INaturezaDespesaConsumoImediatoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public NaturezaDespesaConsumoImediatoEntity ObterNaturezaDespesaConsumoImediato(int codigoNaturezaDespesa)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<INaturezaDespesaConsumoImediatoService>().ObterNaturezaDespesaConsumoImediato(codigoNaturezaDespesa);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<String> ListarNaturezasDespesaConsumoImediato()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<INaturezaDespesaConsumoImediatoService>().ListarNaturezasDespesaConsumoImediato();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }
        

        #endregion
    }
}
