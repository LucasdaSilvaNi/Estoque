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
        #region Relacao Sub Item de Material

        private RelacaoMaterialItemSubItemEntity relitem = new RelacaoMaterialItemSubItemEntity();

        public RelacaoMaterialItemSubItemEntity RelacaoSubItemMaterial
        {
            get { return relitem; }
            set { relitem = value; }
        }


        public void SelectRelacaoMaterialItemSubItem(int _relacaoId)
        {
            this.RelacaoSubItemMaterial = this.Service<IRelacaoMaterialItemSubItemService>().Select(_relacaoId);

        }

        public bool SalvarRelacaoSubItemMaterial()
        {
            this.Service<IRelacaoMaterialItemSubItemService>().Entity = this.RelacaoSubItemMaterial;
            this.ConsistirRelacaoSubItemMaterial();
            if (this.Consistido)
            {
                this.Service<IRelacaoMaterialItemSubItemService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<RelacaoMaterialItemSubItemEntity> ListarRelacaoSubItemMaterial(int _itemId, int _subItemId, int _gestorId)
        {
            this.Service<IRelacaoMaterialItemSubItemService>().SkipRegistros = this.SkipRegistros;
            IList<RelacaoMaterialItemSubItemEntity> retorno = this.Service<IRelacaoMaterialItemSubItemService>().Listar(_itemId, _subItemId, _gestorId);
            this.TotalRegistros = this.Service<IRelacaoMaterialItemSubItemService>().TotalRegistros();
            return retorno;
        }

        public IList<RelacaoMaterialItemSubItemEntity> ImprimirRelacaoItemSubitem(int _itemId, int _subItemId, int _gestorId)
        {
            //IList<RelacaoMaterialItemSubItemEntity> retorno = this.Service<IRelacaoMaterialItemSubItemService>().Imprimir(_itemId, _subItemId, _gestorId);
            IList<RelacaoMaterialItemSubItemEntity> retorno = this.Service<IRelacaoMaterialItemSubItemService>().Imprimir(_itemId, _subItemId, _gestorId);
            return retorno;
        }

        public bool ExcluirRelacaoSubItemMaterial()
        {
            this.Service<IRelacaoMaterialItemSubItemService>().Entity = this.RelacaoSubItemMaterial;
            this.ConsistirExcluirRelacaoSubItemMaterial();
            if (this.Consistido)
            {
                try
                {
                    this.Service<IRelacaoMaterialItemSubItemService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirExcluirRelacaoSubItemMaterial()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<RelacaoMaterialItemSubItemEntity>(ref this.relitem);

            if (this.Service<IRelacaoMaterialItemSubItemService>().ListarTodosCod().Count() == 1)
                this.ListaErro.Add("Não poderá mais excluir relacionamento de item x subitem. O sistema deverá ter pelo menos uma relação de item x subitem de material.");
        }

        public void ConsistirRelacaoSubItemMaterial()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<RelacaoMaterialItemSubItemEntity>(ref this.relitem);

            if (this.RelacaoSubItemMaterial.Item.Id == null || this.RelacaoSubItemMaterial.Item.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Item.");

            if (this.RelacaoSubItemMaterial.SubItem.Id == null || this.RelacaoSubItemMaterial.SubItem.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Subitem.");


            if (this.RelacaoSubItemMaterial.Gestor.Id == null || this.RelacaoSubItemMaterial.Gestor.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IRelacaoMaterialItemSubItemService>().ExisteCodigoInformado())
                    this.ListaErro.Add("A relação já existe.");
            }
        }

        #endregion
    }
}
