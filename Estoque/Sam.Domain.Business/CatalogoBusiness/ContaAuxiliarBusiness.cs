using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Conta Auxiliar

        private ContaAuxiliarEntity conta = new ContaAuxiliarEntity();

        public ContaAuxiliarEntity Conta
        {
            get { return conta; }
            set { conta = value; }
        }

        public bool SalvarConta()
        {
            this.Service<IContaAuxiliarService>().Entity = this.Conta;
            this.ConsistirConta();
            if (this.Consistido)
            {
                this.Service<IContaAuxiliarService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<ContaAuxiliarEntity> ListarConta()
        {
            this.Service<IContaAuxiliarService>().SkipRegistros = this.SkipRegistros;
            IList<ContaAuxiliarEntity> retorno = this.Service<IContaAuxiliarService>().Listar();
            this.TotalRegistros = this.Service<IContaAuxiliarService>().TotalRegistros();
            return retorno;
        }

        public IList<ContaAuxiliarEntity> ListarContaTodosCod()
        {
            IList<ContaAuxiliarEntity> retorno = this.Service<IContaAuxiliarService>().ListarTodosCod();
            return retorno;
        }

        public IList<ContaAuxiliarEntity> ImprimirContaAuxiliar()
        {
            this.SkipRegistros = 0;
            IList<ContaAuxiliarEntity> retorno = this.Service<IContaAuxiliarService>().Imprimir();
            return retorno;
        }

        public bool ExcluirConta()
        {
            this.Service<IContaAuxiliarService>().Entity = this.Conta;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IContaAuxiliarService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirConta()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ContaAuxiliarEntity>(ref this.conta);

            if (this.Conta.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o código.");

            if (string.IsNullOrEmpty(this.Conta.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.Conta.ContaContabil < 1)
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IContaAuxiliarService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
