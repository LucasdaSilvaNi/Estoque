using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;

namespace Sam.Domain.Infrastructure
{
    public class AtividadeItemInfraestructure : BaseInfraestructure, IAtividadeItemService
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

        public AtividadeItemMaterialEntity Entity { get; set; }

        public IList<AtividadeItemMaterialEntity> Listar()
        {
            //IList<AtividadeItemMaterialEntity> resultado = (from a in this.Db.TB_ATIVIDADE_ITEM_MATERIALs
            //                                 orderby a.TB_ATIVIDADE_ITEM_MATERIAL_ID
            //                                 select new AtividadeItemMaterialEntity
            //                                 {
            //                                     Id = a.TB_ATIVIDADE_ITEM_MATERIAL_ID,
            //                                     Descricao = a.TB_ATIVIDADE_ITEM_MATERIAL_DESCRICAO,
            //                                 }).Skip(this.SkipRegistros)
            //                              .Take(this.RegistrosPagina)
            //                              .ToList<AtividadeItemMaterialEntity>();

            //this.totalregistros = (from a in this.Db.TB_ATIVIDADE_ITEM_MATERIALs
            //                       select new
            //                       {
            //                           Id = a.TB_ATIVIDADE_ITEM_MATERIAL_ID,
            //                       }).Count();
            //return resultado;

            return new List<AtividadeItemMaterialEntity>();

        }

        public IList<AtividadeItemMaterialEntity> Imprimir()
        {
            //IList<AtividadeItemMaterialEntity> resultado = (from a in this.Db.TB_ATIVIDADE_ITEM_MATERIALs
            //                                                orderby a.TB_ATIVIDADE_ITEM_MATERIAL_ID
            //                                                select new AtividadeItemMaterialEntity
            //                                                {
            //                                                    Id = a.TB_ATIVIDADE_ITEM_MATERIAL_ID,
            //                                                    Descricao = a.TB_ATIVIDADE_ITEM_MATERIAL_DESCRICAO,
            //                                                })
            //                              .ToList<AtividadeItemMaterialEntity>();

         
            //return resultado;

            return new List<AtividadeItemMaterialEntity>();

        }



        public void Excluir()
        {
            //TB_ATIVIDADE_ITEM_MATERIAL item
            //       = this.Db.TB_ATIVIDADE_ITEM_MATERIALs.Where(a => a.TB_ATIVIDADE_ITEM_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            //this.Db.TB_ATIVIDADE_ITEM_MATERIALs.DeleteOnSubmit(item);
            //this.Db.SubmitChanges();
        }



        public void Salvar()
        {
            //TB_ATIVIDADE_ITEM_MATERIAL item = new TB_ATIVIDADE_ITEM_MATERIAL();

            //if (this.Entity.Id.HasValue)
            //    item = this.Db.TB_ATIVIDADE_ITEM_MATERIALs.Where(a => a.TB_ATIVIDADE_ITEM_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    this.Db.TB_ATIVIDADE_ITEM_MATERIALs.InsertOnSubmit(item);

            //item.TB_ATIVIDADE_ITEM_MATERIAL_DESCRICAO = this.Entity.Descricao;
            //this.Db.SubmitChanges();
        }


        public bool PodeExcluir()
        {

            bool retorno = true;

            //TB_ATIVIDADE_ITEM_MATERIAL uo = this.Db.TB_ATIVIDADE_ITEM_MATERIALs.Where(a => a. == this.UO.Id.Value).FirstOrDefault();
            // XUXA - implementar validação para verificar possibilidade de exclusão da grupo material
            // provavelmente ligacoes com as tabelas: classe, material, item material

            //if (UO.Count > 0)
            //    retorno = false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            return retorno;
        }



        public AtividadeItemMaterialEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<AtividadeItemMaterialEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
