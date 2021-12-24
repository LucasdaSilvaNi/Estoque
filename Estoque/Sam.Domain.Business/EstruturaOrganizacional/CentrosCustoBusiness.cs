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
        #region Centros de Custo

        private CentroCustoEntity centroCusto = new CentroCustoEntity();

        public CentroCustoEntity CentroCusto
        {
            get { return centroCusto; }
            set { centroCusto = value; }
        }

        public bool SalvarCentroCusto()
        {
            this.Service<ICentroCustoService>().Entity = this.CentroCusto;
            this.ConsistirCentroCusto();
            if (this.Consistido)
            {
                this.Service<ICentroCustoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<CentroCustoEntity> ListarCentroCusto(int OrgaoId, int GestorId)
        {
            this.Service<ICentroCustoService>().SkipRegistros = this.SkipRegistros;
            IList<CentroCustoEntity> retorno = this.Service<ICentroCustoService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<ICentroCustoService>().TotalRegistros();
            return retorno;
        }

        public IList<CentroCustoEntity> ListarCentroCusto(int GestorId)
        {
            this.Service<ICentroCustoService>().SkipRegistros = this.SkipRegistros;
            IList<CentroCustoEntity> retorno = this.Service<ICentroCustoService>().Listar(GestorId);
            this.TotalRegistros = this.Service<ICentroCustoService>().TotalRegistros();
            return retorno;
        }

        public IList<CentroCustoEntity> ListarCentroCusto()
        {
            IList<CentroCustoEntity> retorno = this.Service<ICentroCustoService>().Listar();
            this.TotalRegistros = this.Service<ICentroCustoService>().TotalRegistros();
            return retorno;
        }

        public IList<CentroCustoEntity> ImprimirCentroCusto(int OrgaoId, int GestorId)
        {
            IList<CentroCustoEntity> retorno = this.Service<ICentroCustoService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirCentroCusto()
        {
            this.Service<ICentroCustoService>().Entity = this.CentroCusto;
            if (this.Consistido)
            {
                try
                {
                    this.Service<ICentroCustoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirCentroCusto()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<CentroCustoEntity>(ref this.centroCusto);

            if (!this.CentroCusto.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.CentroCusto.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (this.CentroCusto.Codigo == "" || this.CentroCusto.Codigo == null)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.CentroCusto.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<ICentroCustoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
       
    }
}
