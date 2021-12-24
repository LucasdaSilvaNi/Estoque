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
        #region Material

        private MaterialEntity material = new MaterialEntity();

        public MaterialEntity Material
        {
            get { return material; }
            set { material = value; }
        }

        public bool SalvarMaterial()
        {
            this.Service<IMaterialService>().Entity = this.Material;
            this.ConsistirMaterial();
            if (this.Consistido)
            {
                this.Service<IMaterialService>().Salvar();
            }
            return this.Consistido;
        }

        public void SelectMaterial(int _MaterialId)
        {
            this.Material = this.Service<IMaterialService>().Select(_MaterialId);
        }

        public IList<MaterialEntity> ListarMaterial(int _classeId)
        {
            this.Service<IMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<MaterialEntity> retorno = this.Service<IMaterialService>().Listar(_classeId);
            this.TotalRegistros = this.Service<IMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<MaterialEntity> ListarTodosMaterialCod(int _classeId)
        {
            IList<MaterialEntity> retorno = this.Service<IMaterialService>().ListarTodosCod(_classeId);
            return retorno;
        }

        public IList<MaterialEntity> ListarTodosMaterialCod(int _classeId, AlmoxarifadoEntity almoxarifado)
        {
            IList<MaterialEntity> retorno = this.Service<IMaterialService>().ListarTodosCod(_classeId, almoxarifado);
            return retorno;
        }

        public IList<MaterialEntity> ImprimirMaterial(int _classeId)
        {
            IList<MaterialEntity> retorno = this.Service<IMaterialService>().Imprimir(_classeId);
            return retorno;
        }

        public bool ExcluirMaterial()
        {
            this.Service<IMaterialService>().Entity = this.Material;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IMaterialService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirMaterial()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<MaterialEntity>(ref this.material);

            if (this.Material.Classe.Id == null)
                this.ListaErro.Add("É obrigatório informar a Classe.");

            //if (this.Material.Grupo.Id < 1)
            //    this.ListaErro.Add("É obrigatório informar o Grupo.");

            if (this.Material.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o código.");

            if (string.IsNullOrEmpty(this.Material.Descricao))
                this.ListaErro.Add("É obrigatório informar a descrição.");


            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IMaterialService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        public MaterialEntity ObterMaterial(int codigoMaterial)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IMaterialService>().ObterMaterial(codigoMaterial);
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
