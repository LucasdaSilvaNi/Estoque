using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;
using System.Transactions;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Grupo

        private GrupoEntity grupo = new GrupoEntity();

        public GrupoEntity Grupo
        {
            get { return grupo; }
            set { grupo = value; }
        }

        public bool SalvarGrupo()
        {
            this.Service<IGrupoService>().Entity = this.Grupo;
            this.ConsistirGrupo();
            if (this.Consistido)
            {
                this.Service<IGrupoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<GrupoEntity> ListarGrupo()
        {
            this.Service<IGrupoService>().SkipRegistros = this.SkipRegistros;

            IList<GrupoEntity> retorno = this.Service<IGrupoService>().Listar();
            this.TotalRegistros = this.Service<IGrupoService>().TotalRegistros();

            return retorno;
        }

        public IList<GrupoEntity> ListarGrupoTodosCod()
        {
            IList<GrupoEntity> retorno = this.Service<IGrupoService>().ListarTodosCod();
            return retorno;
        }

        public IList<GrupoEntity> ListarGrupoTodosCod(AlmoxarifadoEntity almoxarifado)
        {
            IList<GrupoEntity> retorno = this.Service<IGrupoService>().ListarTodosCod(almoxarifado);
            return retorno;
        }

        public IList<GrupoEntity> ImprimirGrupo()
        {
            IList<GrupoEntity> retorno = this.Service<IGrupoService>().Imprimir();
            return retorno;
        }

        public bool ExcluirGrupo()
        {
            this.Service<IGrupoService>().Entity = this.Grupo;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IGrupoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirGrupo()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<GrupoEntity>(ref this.grupo);

            if (this.Grupo.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.Grupo.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IGrupoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public GrupoEntity ObterGrupoMaterial(int codigoGrupoMaterial)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IGrupoService>().ObterGrupoMaterial(codigoGrupoMaterial);
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
