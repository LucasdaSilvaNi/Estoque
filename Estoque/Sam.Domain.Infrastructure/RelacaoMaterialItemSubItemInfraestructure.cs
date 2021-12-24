using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public class RelacaoMaterialItemSubItemInfraestructure : BaseInfraestructure, IRelacaoMaterialItemSubItemService 
    {
        public int totalregistros
        {
            get;
            set;
        }

        public int TotalRegistros()
        {
            return totalregistros;
        }

        public RelacaoMaterialItemSubItemEntity Entity { get; set; }

        public RelacaoMaterialItemSubItemEntity Select(int _id)
        {
           
            return new RelacaoMaterialItemSubItemEntity();

        }

        public IList<RelacaoMaterialItemSubItemEntity> Listar()
        {
            return this.Listar(int.MinValue, int.MinValue, int.MinValue);
        }
        public IList<RelacaoMaterialItemSubItemEntity> Listar(int _itemId, int _subItemId, int _gestorId)
        {
            //IList<RelacaoMaterialItemSubItemEntity> resultado = (from a in this.Db.TB_RELACAO_ITEMXSUBITEMs
            //                                          where (a.TB_ITEM_MATERIAL_ID == _itemId && a.TB_SUBITEM_MATERIAL_ID == _subItemId && a.TB_GESTOR_ID == _gestorId)
            //                                          orderby a.TB_ITEM_MATERIAL_ID
            //                                          select new RelacaoMaterialItemSubItemEntity
            //                                          {
            //                                              Id = a.TB_RELACAO_ITEMXSUBITEM_ID,
            //                                              Item = (new ItemMaterialEntity(a.TB_ITEM_MATERIAL_ID)),
            //                                              SubItem = (new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID))
            //                                          }).Skip(this.SkipRegistros)
            //                              .Take(this.RegistrosPagina)
            //                              .ToList<RelacaoMaterialItemSubItemEntity>();

            //this.totalregistros = (from a in this.Db.TB_RELACAO_ITEMXSUBITEMs
            //                       where (a.TB_ITEM_MATERIAL_ID == _itemId && a.TB_SUBITEM_MATERIAL_ID == _subItemId && a.TB_GESTOR_ID == _gestorId)
            //                       select new RelacaoMaterialItemSubItemEntity
            //                       {
            //                           Id = a.TB_RELACAO_ITEMXSUBITEM_ID,
            //                       }).Count();
            //return resultado;

            return new List<RelacaoMaterialItemSubItemEntity>();

        }

        public IList<RelacaoMaterialItemSubItemEntity> Imprimir(int _itemId, int _subItemId, int _gestorId)
        {
            IList<RelacaoMaterialItemSubItemEntity> resultado = (from a in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                 join d in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals d.TB_GESTOR_ID
                                                                 where (a.TB_SUBITEM_MATERIAL_ID == _subItemId)
                                                                 where (a.TB_GESTOR_ID == _gestorId)
                                                                 orderby a.TB_ITEM_MATERIAL_ID
                                                                 select new RelacaoMaterialItemSubItemEntity
                                                                 {
                                                                     Id = a.TB_ITEM_SUBITEM_MATERIAL_ID,
                                                                     Item = (new ItemMaterialEntity { Id = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID, Codigo = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO, Descricao = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO }),
                                                                     SubItem = (new SubItemMaterialEntity { Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO, Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO }),
                                                                     Gestor = (new GestorEntity { Id = d.TB_GESTOR_ID, Nome = d.TB_GESTOR_NOME })
                                                                 })
                                          .ToList<RelacaoMaterialItemSubItemEntity>();


            return resultado;
        }


        public IList<RelacaoMaterialItemSubItemEntity> Imprimir()
        {
            //IList<RelacaoMaterialItemSubItemEntity> resultado = (from a in this.Db.TB_RELACAO_ITEMXSUBITEMs
                                                                
            //                                                     orderby a.TB_ITEM_MATERIAL_ID
            //                                                     select new RelacaoMaterialItemSubItemEntity
            //                                                     {
            //                                                         Id = a.TB_RELACAO_ITEMXSUBITEM_ID,
            //                                                         Item = (new ItemMaterialEntity(a.TB_ITEM_MATERIAL_ID)),
            //                                                         SubItem = (new SubItemMaterialEntity(a.TB_SUBITEM_MATERIAL_ID))
            //                                                     })
            //                              .ToList<RelacaoMaterialItemSubItemEntity>();

          
            //return resultado;
            return new List<RelacaoMaterialItemSubItemEntity>();

        }

        public void Excluir()
        {
            TB_ITEM_SUBITEM_MATERIAL itemSubItem = new TB_ITEM_SUBITEM_MATERIAL();

            itemSubItem = this.Db.TB_ITEM_SUBITEM_MATERIALs.Where
                   (a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItem.Id.Value
                       && a.TB_ITEM_MATERIAL_ID == this.Entity.Item.Id.Value).FirstOrDefault();

            this.Db.TB_ITEM_SUBITEM_MATERIALs.DeleteOnSubmit(itemSubItem);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_ITEM_SUBITEM_MATERIAL itemSubItem = new TB_ITEM_SUBITEM_MATERIAL();

            if (this.Entity.Id.HasValue)
                itemSubItem = this.Db.TB_ITEM_SUBITEM_MATERIALs.Where
                    (a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItem.Id.Value
                        && a.TB_ITEM_MATERIAL_ID == this.Entity.Item.Id.Value).FirstOrDefault();
            else
                this.Db.TB_ITEM_SUBITEM_MATERIALs.InsertOnSubmit(itemSubItem);

            itemSubItem.TB_SUBITEM_MATERIAL_ID = this.Entity.SubItem.Id.Value;
            itemSubItem.TB_ITEM_MATERIAL_ID = this.Entity.ItemEdit.Id.Value;
            itemSubItem.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {

            return true;
            int qtd = int.MinValue;

            qtd = (from a in this.Db.TB_ITEM_SUBITEM_MATERIALs
                   where a.TB_ITEM_SUBITEM_MATERIAL_ID == this.Entity.Id
                   select new
                   {
                       Id = a.TB_ITEM_SUBITEM_MATERIAL_ID,
                   }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;

            retorno = this.Db.TB_ITEM_SUBITEM_MATERIALs
            .Where(a => a.TB_ITEM_MATERIAL_ID == this.Entity.ItemEdit.Id.Value)            
            .Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItem.Id.Value)
            .Count() > 0;
            
            return retorno;
        }


        public RelacaoMaterialItemSubItemEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// só para efeito de contagem (consistência entre relacionamento com item x subitem de material)
        /// </summary>
        /// <returns></returns>
        public IList<RelacaoMaterialItemSubItemEntity> ListarTodosCod()
        {
            IList<RelacaoMaterialItemSubItemEntity> retorno = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                               where a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID == this.Entity.SubItem.Id
                                                               select new RelacaoMaterialItemSubItemEntity
                                                               {
                                                                   Id = a.TB_ITEM_SUBITEM_MATERIAL_ID,
                                                                   SubItem = new SubItemMaterialEntity { Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, 
                                                                                                         Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                                         Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO}
                                                               }).ToList();
            return retorno;
        }
    }
}
