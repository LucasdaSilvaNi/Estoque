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
        #region Atividade Material

        private AtividadeItemMaterialEntity atividadematerial = new AtividadeItemMaterialEntity();

        public AtividadeItemMaterialEntity AtividadeMaterial
        {
            get { return atividadematerial; }
            set { atividadematerial = value; }
        }

        public bool SalvarAtividadeMaterial()
        {
            this.Service<IAtividadeItemService>().Entity = this.AtividadeMaterial;
            this.ConsistirMaterial();
            if (this.Consistido)
            {
                this.Service<IAtividadeItemService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<AtividadeItemMaterialEntity> ListarAtividadeMaterial()
        {
            this.Service<IAtividadeItemService>().SkipRegistros = this.SkipRegistros;
            IList<AtividadeItemMaterialEntity> retorno = this.Service<IAtividadeItemService>().Listar();
            this.TotalRegistros = this.Service<IAtividadeItemService>().TotalRegistros();
            return retorno;
        }

        public bool ExcluirAtividadeMaterial()
        {
            this.Service<IAtividadeItemService>().Entity = this.AtividadeMaterial;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IAtividadeItemService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirAtividadeMaterial()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<AtividadeItemMaterialEntity>(ref this.atividadematerial);

            if (string.IsNullOrEmpty(this.AtividadeMaterial.Descricao))
                this.ListaErro.Add("É obrigatório informar a Atividade.");


            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IAtividadeItemService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion


    }
}
