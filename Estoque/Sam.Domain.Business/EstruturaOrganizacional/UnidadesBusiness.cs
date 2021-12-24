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
        #region Unidade

        private UnidadeEntity unidade = new UnidadeEntity();

        public UnidadeEntity Unidade
        {
            get { return unidade; }
            set { unidade = value; }
        }

        public bool SalvarUnidade()
        {
            this.Service<IUnidadeService>().Entity = this.Unidade;
            this.ConsistirUnidade();
            if (this.Consistido)
            {
                this.Service<IUnidadeService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<UnidadeEntity> ListarUnidade(int OrgaoId, int GestorId)
        {
            this.Service<IUnidadeService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeEntity> retorno = this.Service<IUnidadeService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<IUnidadeService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeEntity> ListarUnidade(int? GestorId)
        {
            this.Service<IUnidadeService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeEntity> retorno = this.Service<IUnidadeService>().Listar(GestorId);
            this.TotalRegistros = this.Service<IUnidadeService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeEntity> ListarUnidade()
        {
            this.Service<IUnidadeService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeEntity> retorno = this.Service<IUnidadeService>().Listar();
            this.TotalRegistros = this.Service<IUnidadeService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeEntity> ImprimirUnidade(int OrgaoId, int GestorId)
        {
            IList<UnidadeEntity> retorno = this.Service<IUnidadeService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirUnidade()
        {
            this.Service<IUnidadeService>().Entity = this.Unidade;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IUnidadeService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirUnidade()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UnidadeEntity>(ref this.unidade);

            if (!this.Unidade.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.Unidade.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (!this.Unidade.Codigo.HasValue)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.Unidade.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUnidadeService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
