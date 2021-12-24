using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region MotivoBaixa

        private MotivoBaixaEntity motivoBaixa = new MotivoBaixaEntity();

        public MotivoBaixaEntity MotivoBaixa
        {
            get { return motivoBaixa; }
            set { motivoBaixa = value; }
        }

        public bool SalvarMotivoBaixa()
        {
            this.Service<IMotivoBaixaService>().Entity = this.MotivoBaixa;
            this.ConsistirMotivoBaixa();
            if (this.Consistido)
            {
                this.Service<IMotivoBaixaService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<MotivoBaixaEntity> ListarMotivoBaixa()
        {
            this.Service<IMotivoBaixaService>().SkipRegistros = this.SkipRegistros;
            IList<MotivoBaixaEntity> retorno = this.Service<IMotivoBaixaService>().Listar();
            this.TotalRegistros = this.Service<IMotivoBaixaService>().TotalRegistros();
            return retorno;
        }

        public IList<MotivoBaixaEntity> ImprimirMotivoBaixa()
        {
            IList<MotivoBaixaEntity> retorno = this.Service<IMotivoBaixaService>().Imprimir();
            return retorno;
        }

        public bool ExcluirMotivoBaixa()
        {
            this.Service<IMotivoBaixaService>().Entity = this.MotivoBaixa;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IMotivoBaixaService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirMotivoBaixa()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<MotivoBaixaEntity>(ref this.motivoBaixa);

            if (!this.MotivoBaixa.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.MotivoBaixa.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (string.IsNullOrEmpty(this.MotivoBaixa.CodigoTransacao))
                this.ListaErro.Add("É obrigatório informar o Código de Transação.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IMotivoBaixaService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
