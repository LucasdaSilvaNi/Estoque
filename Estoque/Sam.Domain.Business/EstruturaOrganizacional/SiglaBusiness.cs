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

        #region Sigla

        private SiglaEntity sigla = new SiglaEntity();

        public SiglaEntity Sigla
        {
            get { return sigla; }
            set { sigla = value; }
        }

        public bool SalvarSigla()
        {
            this.Service<ISiglaService>().Entity = this.Sigla;
            this.ConsistirSigla();
            if (this.Consistido)
            {
                this.Service<ISiglaService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<SiglaEntity> ListarSigla(int OrgaoId, int GestorId)
        {
            this.Service<ISiglaService>().SkipRegistros = this.SkipRegistros;
            IList<SiglaEntity> retorno = this.Service<ISiglaService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<ISiglaService>().TotalRegistros();
            return retorno;
        }

        public IList<SiglaEntity> ListarSigla()
        {
            this.Service<ISiglaService>().SkipRegistros = this.SkipRegistros;
            IList<SiglaEntity> retorno = this.Service<ISiglaService>().Listar();
            this.TotalRegistros = this.Service<ISiglaService>().TotalRegistros();
            return retorno;
        }

        public IList<SiglaEntity> ImprimirSigla(int OrgaoId, int GestorId)
        {
            IList<SiglaEntity> retorno = this.Service<ISiglaService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirSigla()
        {
            this.Service<ISiglaService>().Entity = this.Sigla;
            if (this.Consistido)
            {
                try
                {
                    this.Service<ISiglaService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirSigla()
        {
            
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<SiglaEntity>(ref this.sigla);

            if (!this.Sigla.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.Sigla.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (!this.Sigla.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.Sigla.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (!this.Sigla.IndicadorBemProprio.Id.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Indicador Próprio.");
            }
            
            if (this.ListaErro.Count == 0)
            {
                if (this.Service<ISiglaService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion

    }
}
