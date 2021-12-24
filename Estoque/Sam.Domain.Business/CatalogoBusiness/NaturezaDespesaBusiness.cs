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
        #region Natureza de Despesa

        private NaturezaDespesaEntity naturezadespesa = new NaturezaDespesaEntity();

        public NaturezaDespesaEntity NaturezaDespesa
        {
            get { return naturezadespesa; }
            set { naturezadespesa = value; }
        }

        public bool SalvarNaturezaDespesa()
        {
            this.Service<INaturezaDespesaService>().Entity = this.NaturezaDespesa;
            this.ConsistirNaturezaDespesa();
            if (this.Consistido)
            {
                this.Service<INaturezaDespesaService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<NaturezaDespesaEntity> ListarNaturezaDespesa()
        {
            this.Service<INaturezaDespesaService>().SkipRegistros = this.SkipRegistros;
            IList<NaturezaDespesaEntity> retorno = this.Service<INaturezaDespesaService>().Listar();
            this.TotalRegistros = this.Service<INaturezaDespesaService>().TotalRegistros();
            return retorno;
        }

        public IList<NaturezaDespesaEntity> ListarNaturezaDespesaTodosCod()
        {
            IList<NaturezaDespesaEntity> retorno = this.Service<INaturezaDespesaService>().ListarTodosCod();
            return retorno;
        }

        public IList<NaturezaDespesaEntity> ImprimirNaturezaDespesa()
        {
            IList<NaturezaDespesaEntity> retorno = this.Service<INaturezaDespesaService>().Imprimir();
            return retorno;
        }

        public bool ExcluirNaturezaDespesa()
        {
            this.Service<INaturezaDespesaService>().Entity = this.NaturezaDespesa;

            try
            {
                return this.Service<INaturezaDespesaService>().ExcluirNaturezaDespesa();
            }
            catch(SqlException)
            {
                this.ListaErro = new List<string>() { "Impossível excluir registro. Natureza de Despesa possui alguns Itens de Material vinculados." };

                return false;
            }
        }

        public void ConsistirNaturezaDespesa()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<NaturezaDespesaEntity>(ref this.naturezadespesa);

            if (this.NaturezaDespesa.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o código.");

            if (string.IsNullOrEmpty(this.NaturezaDespesa.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<INaturezaDespesaService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public NaturezaDespesaEntity ObterNaturezaDespesa(int idNaturezaDespesa)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<INaturezaDespesaService>().ObterNaturezaDespesa(idNaturezaDespesa);
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

        public NaturezaDespesaEntity ObterNaturezaDespesa(string codigoNaturezaDespesa)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    //NaturezaDespesaInfraestructure infra = new NaturezaDespesaInfraestructure();
                    return this.Service<INaturezaDespesaService>().ObterNaturezaDespesa(codigoNaturezaDespesa);
                    //return infra.ObterNaturezaDespesa(codigoNaturezaDespesa);
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
