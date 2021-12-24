using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;


namespace Sam.Domain.Infrastructure
{
    public class ItemMaterialInfraestructure : BaseInfraestructure, IItemMaterialService
    {
        public int totalregistros{ get; set; }

        public int TotalRegistros()
        {
            return totalregistros;
        }


        public ItemMaterialEntity Entity { get; set; }

        public ItemMaterialEntity Select(int _id)
        {
            ItemMaterialEntity info;
            TB_ITEM_MATERIAL item = (from a in this.Db.TB_ITEM_MATERIALs where a.TB_ITEM_MATERIAL_ID == _id select a).FirstOrDefault();
            info = new ItemMaterialEntity();
            if (item != null)
            {
                info.Id = _id;
                info.Codigo = item.TB_ITEM_MATERIAL_CODIGO;
                info.Descricao = item.TB_ITEM_MATERIAL_DESCRICAO;                
                info.Material = (new MaterialEntity(item.TB_MATERIAL_ID));
            }
           return info;
        }

        public IList<ItemMaterialEntity> Listar()
        {
            return this.Listar(int.MinValue);
        }

        public IList<ItemMaterialEntity> Listar(int _materialId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   where (a.TB_MATERIAL_ID == _materialId) 
                                                      orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                      select new ItemMaterialEntity
                                                      {
                                                          Id = a.TB_ITEM_MATERIAL_ID,
                                                          Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                          Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                          Status = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),                                                          
                                                      }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<ItemMaterialEntity>();

            this.totalregistros = (resultado.IsNotNull()) ? resultado.Count() : 0;

            return resultado;

        }

        public IList<ItemMaterialEntity> ListarAlmox(int _materialId, int _gestorId, int _almoxarifadoId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_ITEM_MATERIAL_ID equals b.TB_ITEM_MATERIAL_ID
                                                   where a.TB_ITEM_MATERIAL_ID == _materialId//  && b.TB_GESTOR_ID == _gestorId
                                                   orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(8, '0'), a.TB_ITEM_MATERIAL_DESCRICAO),                                                       
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                   }).Distinct().ToList<ItemMaterialEntity>();

            return resultado;

        }

        public IList<ItemMaterialEntity> ListarItemSaldoByAlmox(int almoxarifado){
            IList<ItemMaterialEntity> resultado = (from s  in Db.TB_SALDO_SUBITEMs
                                                   join r  in Db.TB_RESERVA_MATERIALs on new {s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID} equals new {r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID} into x
                                                   from r in x.DefaultIfEmpty()
                                                   join si in Db.TB_ITEM_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                                   join i  in Db.TB_ITEM_MATERIALs on si.TB_ITEM_MATERIAL_ID equals i.TB_ITEM_MATERIAL_ID
                                                   where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                                                   where ((r.TB_RESERVA_MATERIAL_QUANT != null 
                                                          && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0 )
                                                          || (r.TB_RESERVA_MATERIAL_QUANT == null))
                                                   where (s.TB_ALMOXARIFADO_ID.Equals(almoxarifado))
                                                        select new ItemMaterialEntity
                                                        {
                                                            Id = i.TB_ITEM_MATERIAL_ID,
                                                            Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                            Descricao = i.TB_ITEM_MATERIAL_DESCRICAO,
                                                        }
                                                   ).Distinct().ToList();
            return resultado;
        }

        public IList<ItemMaterialEntity> ListarBySubItem(int _subItem)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_ITEM_MATERIAL_ID equals b.TB_ITEM_MATERIAL_ID
                                                   join c  in this.Db.TB_MATERIALs on a.TB_MATERIAL_ID equals c.TB_MATERIAL_ID
                                                   join d in this.Db.TB_CLASSE_MATERIALs on c.TB_CLASSE_MATERIAL_ID equals d.TB_CLASSE_MATERIAL_ID
                                                   where (b.TB_SUBITEM_MATERIAL_ID == _subItem)
                                                   orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity
                                                       {
                                                           Id = a.TB_ITEM_MATERIAL_ID,
                                                           Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                           Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       }),
                                                       
                                                       MaterialId = c.TB_MATERIAL_ID,
                                                       ClasseId = d.TB_CLASSE_MATERIAL_ID,
                                                       GrupoId = d.TB_GRUPO_MATERIAL_ID
                                                       
                                                   }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<ItemMaterialEntity>();

            this.totalregistros = (resultado.IsNotNull()) ? resultado.Count() : 0;

            return resultado;

        }

        public IList<ItemMaterialEntity> ListarBySubItem(int _subItem,int _gestorId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_ITEM_MATERIAL_ID equals b.TB_ITEM_MATERIAL_ID
                                                   where (b.TB_SUBITEM_MATERIAL_ID == _subItem)
                                                   where (b.TB_GESTOR_ID == _gestorId)
                                                   orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity
                                                       {
                                                           Id = a.TB_MATERIAL.TB_MATERIAL_ID,
                                                           Codigo = a.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                                           Descricao = a.TB_MATERIAL.TB_MATERIAL_DESCRICAO,
                                                           //Id = a.TB_ITEM_MATERIAL_ID,
                                                           //Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                           //Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       }),

                                                       MaterialId = b.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID,
                                                       ClasseId = b.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                                       GrupoId = b.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID
                                                   }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<ItemMaterialEntity>();

            this.totalregistros = (resultado.IsNotNull()) ? resultado.Count() : 0;

            return resultado;

        }


        public ItemMaterialEntity GetItemMaterialBySubItem(int subItemId)
        {            var result = (from a in Db.TB_ITEM_MATERIALs
                                   where a.TB_ITEM_SUBITEM_MATERIALs.Where(b => b.TB_SUBITEM_MATERIAL_ID == subItemId).Count() > 0
                                   
                          select new ItemMaterialEntity
                          {
                              Id = a.TB_ITEM_MATERIAL_ID,
                              Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                              Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                              Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                              Material = (new MaterialEntity
                              {
                                  Id = a.TB_MATERIAL.TB_MATERIAL_ID,
                                  Codigo = a.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                  Descricao = a.TB_MATERIAL.TB_MATERIAL_DESCRICAO,
                              }),

                              MaterialId = a.TB_MATERIAL_ID,
                              ClasseId = a.TB_MATERIAL.TB_CLASSE_MATERIAL_ID,                              
                              GrupoId = a.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL_ID,
                              NaturezaDespesa = (from ItemNaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs
                                                 where ItemNaturezaDespesa.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID == a.TB_ITEM_MATERIAL_ID
                                                 select new NaturezaDespesaEntity
                                                 {
                                                     Id = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                     Codigo = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                     Descricao = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                 }).ToList<NaturezaDespesaEntity>()
                          }).FirstOrDefault();
            return result;
        }

        public string NaturezaSubItem(int subItemId) {

            var result = (from a in Db.TB_SUBITEM_MATERIALs
                          where a.TB_SUBITEM_MATERIAL_ID == subItemId

                          select a.TB_NATUREZA_DESPESA_ID.ToString()).FirstOrDefault();
                          
            return result;
                      
        }

        public ItemMaterialEntity GetItemMaterialByItem(int itemId)
        {
            var result = (from a in Db.TB_ITEM_MATERIALs
                          where a.TB_ITEM_MATERIAL_ID == itemId
                          select new ItemMaterialEntity
                          {
                              Id = a.TB_ITEM_MATERIAL_ID,
                              Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                              Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                              Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                              Material = (new MaterialEntity
                              {
                                  Id = a.TB_MATERIAL.TB_MATERIAL_ID,
                                  Codigo = a.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                  Descricao = a.TB_MATERIAL.TB_MATERIAL_DESCRICAO,
                              }),

                              MaterialId = a.TB_MATERIAL_ID,
                              ClasseId = a.TB_MATERIAL.TB_CLASSE_MATERIAL_ID,
                              GrupoId = a.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL_ID,
                              NaturezaDespesa = (from ItemNaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs
                                                 where ItemNaturezaDespesa.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID == itemId
                                                 select new NaturezaDespesaEntity
                                                 {
                                                     Id = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                     Codigo = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                     Descricao = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                 }).ToList<NaturezaDespesaEntity>()
                          }).FirstOrDefault();
            return result;
        }

        public IList<ItemMaterialEntity> Imprimir()
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                       //Atividade = (new AtividadeItemMaterialEntity(a.TB_ATIVIDADE_ITEM_MATERIAL_ID))
                                                   })
                                          .ToList<ItemMaterialEntity>();

        
            return resultado;
        }

        public IList<ItemMaterialEntity> Imprimir(int _materialId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   where (a.TB_MATERIAL_ID == _materialId)
                                                   orderby a.TB_ITEM_MATERIAL_DESCRICAO
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                       //Atividade = (new AtividadeItemMaterialEntity(a.TB_ATIVIDADE_ITEM_MATERIAL_ID))
                                                   })
                                          .ToList<ItemMaterialEntity>();
            return resultado;
        }

        public void Excluir()
        {
            TB_ITEM_MATERIAL item = this.Db.TB_ITEM_MATERIALs.Where(a => a.TB_ITEM_MATERIAL_ID == this.Entity.Id).FirstOrDefault();

            if (item.IsNotNull())
            {
                this.Db.TB_ITEM_MATERIALs.DeleteOnSubmit(item);

                IList<TB_ITEM_NATUREZA_DESPESA> lstNatDespesaRelacionadas = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(_itemNatDespesa => _itemNatDespesa.TB_ITEM_MATERIAL_ID == this.Entity.Id).ToList();
                lstNatDespesaRelacionadas.ToList().ForEach(relItemNatDespesa => this.Db.TB_ITEM_NATUREZA_DESPESAs.DeleteOnSubmit(relItemNatDespesa));

                this.Db.SubmitChanges();
            }
        }



        public void Salvar()
        {
            TB_ITEM_MATERIAL item = new TB_ITEM_MATERIAL();

            if (this.Entity.Id.HasValue)
                item = this.Db.TB_ITEM_MATERIALs.Where(a => a.TB_ITEM_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_ITEM_MATERIALs.InsertOnSubmit(item);

            item.TB_ITEM_MATERIAL_CODIGO = this.Entity.Codigo;
            item.TB_ITEM_MATERIAL_DESCRICAO = this.Entity.Descricao;
            item.TB_MATERIAL_ID = this.Entity.Material.Id.Value;
            item.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE = this.Entity.Atividade;
            this.Db.SubmitChanges();
            this.Entity.Id = item.TB_ITEM_MATERIAL_ID;
        }


        public bool PodeExcluir()
        {
            int qtd = int.MinValue;

            //qtd = (from a in this.Db.TB_SUBITEM_MATERIALs
            //       where a.TB_ITEM_MATERIAL_ID == this.Entity.Id
            //       select new
            //       {
            //           Id = a.TB_SUBITEM_MATERIAL_ID,
            //       }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_ITEM_MATERIALs
                .Where(a => a.TB_ITEM_MATERIAL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_ITEM_MATERIAL_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_ITEM_MATERIALs
                .Where(a => a.TB_ITEM_MATERIAL_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }



        public ItemMaterialEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<ItemMaterialEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }


        public IList<ItemMaterialEntity> ListarTodosCod(int _materialId, bool todos)
        {
            ItemMaterialEntity mat = new ItemMaterialEntity();
            mat.Id = 0;
            mat.Descricao = "- Todos -";
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   where (a.TB_MATERIAL_ID == _materialId)
                                                   orderby a.TB_ITEM_MATERIAL_ID
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'), a.TB_ITEM_MATERIAL_DESCRICAO),                                               
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                   }).ToList<ItemMaterialEntity>();

            if (todos)
            {
                resultado.Insert(0, mat);
            }
            return resultado;
        }

        IList<ItemMaterialEntity> ListarSubItemCod(int _materialId, int subItemId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_ITEM_MATERIAL_ID equals b.TB_ITEM_MATERIAL_ID
                                                   join c in this.Db.TB_SUBITEM_MATERIALs on b.TB_SUBITEM_MATERIAL_ID equals c.TB_SUBITEM_MATERIAL_ID 
                                                   where (a.TB_MATERIAL_ID == _materialId)
                                                   orderby a.TB_ITEM_MATERIAL_ID
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'), a.TB_ITEM_MATERIAL_DESCRICAO),
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                   }).ToList<ItemMaterialEntity>();

            //var naturezaLista = (from nat in this.Db.TB_NATUREZA_DESPESAs
            //                     join i in this.Db.TB_ITEM_NATUREZA_DESPESAs on nat.TB_NATUREZA_DESPESA_ID equals i.TB_NATUREZA_DESPESA_ID
            //                     join itemSub in this.Db.TB_ITEM_SUBITEM_MATERIALs on i.TB_ITEM_MATERIAL_ID equals itemSub.TB_ITEM_MATERIAL_ID
            //                     where itemSub.TB_SUBITEM_MATERIAL_ID == subItemId
            //                     select new NaturezaDespesaEntity
            //                     {
            //                         Id = nat.TB_NATUREZA_DESPESA_ID,
            //                         Codigo = nat.TB_NATUREZA_DESPESA_CODIGO,
            //                         Descricao = nat.TB_NATUREZA_DESPESA_DESCRICAO
            //                     }).ToList();

            return resultado;
        }

        public IList<ItemMaterialEntity> ListarSubItemCod(int _materialId)
        {
            IList<ItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_MATERIALs
                                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_ITEM_MATERIAL_ID equals b.TB_ITEM_MATERIAL_ID
                                                   join c in this.Db.TB_SUBITEM_MATERIALs on b.TB_SUBITEM_MATERIAL_ID equals c.TB_SUBITEM_MATERIAL_ID
                                                   where (a.TB_MATERIAL_ID == _materialId)
                                                   orderby a.TB_ITEM_MATERIAL_ID
                                                   select new ItemMaterialEntity
                                                   {
                                                       Id = a.TB_ITEM_MATERIAL_ID,
                                                       Descricao = string.Format("{0} - {1}", a.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'), a.TB_ITEM_MATERIAL_DESCRICAO),
                                                       Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                                       Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                       Material = (new MaterialEntity(a.TB_ITEM_MATERIAL_ID)),
                                                   }).ToList<ItemMaterialEntity>();
            return resultado;
        }

        public ItemMaterialEntity Select(ItemMaterialEntity itemMat)
        {
            ItemMaterialEntity info;
            TB_ITEM_MATERIAL item = (from a in this.Db.TB_ITEM_MATERIALs
                                     where (itemMat.Id.HasValue ? a.TB_ITEM_MATERIAL_ID == itemMat.Id : 1 == 1)
                                     where (itemMat.Codigo != null ? a.TB_ITEM_MATERIAL_CODIGO == itemMat.Codigo : 1 == 1)
                                     select a).FirstOrDefault();
            info = new ItemMaterialEntity();
            if (item != null)
            {
                info.Id = item.TB_ITEM_MATERIAL_ID;
                info.Codigo = item.TB_ITEM_MATERIAL_CODIGO;
                info.Descricao = item.TB_ITEM_MATERIAL_DESCRICAO;
                info.Material = (new MaterialEntity(item.TB_MATERIAL_ID));
            }
            return info;
        }

        public IList<ItemMaterialEntity> ListarPorPalavraChaveTodosCod(int? Id, int? Codigo, string Descricao, int? AlmoxId, int? GestorId)
        {
            if (Descricao == null)
                Descricao = "";

            long codigoItem;
            long.TryParse(Descricao, out codigoItem);

            IEnumerable<ItemMaterialEntity> resultado = new List<ItemMaterialEntity>();

            if (AlmoxId == 0)
            {
                resultado = (from a in this.Db.TB_ITEM_MATERIALs
                             where a.TB_ITEM_MATERIAL_CODIGO == codigoItem || codigoItem == 0                             
                             orderby a.TB_ITEM_MATERIAL_DESCRICAO
                             select new ItemMaterialEntity
                             {
                                 Id = a.TB_ITEM_MATERIAL_ID,
                                 Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                 Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                 Atividade = a.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                 Material = (new MaterialEntity(a.TB_MATERIAL.TB_MATERIAL_ID)),
                                 MaterialId = a.TB_MATERIAL.TB_MATERIAL_ID,
                                 ClasseId = a.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                 GrupoId = a.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID
                             });
            }
            else
            {

                resultado = (from a in this.Db.TB_ITEM_SUBITEM_MATERIALs
                             join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, a.TB_GESTOR_ID } equals
                                new { almox.TB_SUBITEM_MATERIAL_ID, almox.TB_SUBITEM_MATERIAL.TB_GESTOR_ID }
                             where almox.TB_ALMOXARIFADO_ID == AlmoxId || AlmoxId == 0
                             where almox.TB_SUBITEM_MATERIAL.TB_GESTOR_ID == GestorId || GestorId == 0
                             where a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == codigoItem || codigoItem == 0
                             orderby a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO
                             group a by new { a.TB_ITEM_MATERIAL } into b
                             select new ItemMaterialEntity
                             {
                                 Id = b.Key.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                 Descricao = b.Key.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                 Codigo = b.Key.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                 Atividade = b.Key.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                 Material = (new MaterialEntity(b.Key.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID)),
                                 MaterialId = b.Key.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID,
                                 ClasseId = b.Key.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                 GrupoId = b.Key.TB_ITEM_MATERIAL.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID
                             });
            }

            if (codigoItem == 0)
            {
                List<String> palavras = Descricao.Split(' ').ToList<String>();
                foreach (var p in palavras)
                {
                    String palavra = p.ToUpper();
                    resultado = resultado.Where(a => a.Descricao.ToUpper().Contains(palavra));
                }
            }

            this.totalregistros = resultado.Count();
            return resultado.ToList<ItemMaterialEntity>();
        }

        //TODO [EVOLUCAO INFRA] Otimizar
        public IList<ItemMaterialEntity> ListarItemMaterialPorCodigoSiafisico(int? Codigo)
        {
            List<ItemMaterialEntity>        lLstRetorno    = null;
            //IEnumerable<ItemMaterialEntity> lIEnumConsulta = null;
            IQueryable<ItemMaterialEntity> qryConsulta = null;
            string lStrSQL = null;


            lLstRetorno = new List<ItemMaterialEntity>();

            qryConsulta = (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                              where   ItemMaterial.TB_ITEM_MATERIAL_CODIGO == Codigo 
                              orderby ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO
                              select new ItemMaterialEntity
                              {
                                  Id              = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                  Descricao       = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                  Codigo          = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                  Atividade       = ItemMaterial.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                  Material        = new MaterialEntity
                                                    {
                                                        Id        = ItemMaterial.TB_MATERIAL.TB_MATERIAL_ID,
                                                        Codigo    = ItemMaterial.TB_MATERIAL.TB_MATERIAL_CODIGO,
                                                        Descricao = ItemMaterial.TB_MATERIAL.TB_MATERIAL_DESCRICAO,
                                                        Classe    = new ClasseEntity
                                                                    {
                                                                        Id        = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                                                        Codigo    = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO,
                                                                        Descricao = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_DESCRICAO,
                                                                        Grupo     = new GrupoEntity
                                                                                    {
                                                                                        Id        = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                                                                        Codigo    = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO,
                                                                                        Descricao = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_DESCRICAO
                                                                                    }
                                                                    }
                                                    },
                                  MaterialId      = ItemMaterial.TB_MATERIAL.TB_MATERIAL_ID,
                                  ClasseId        = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                  GrupoId         = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                  NaturezaDespesa = (from ItemNaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs
                                                     where ItemNaturezaDespesa.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == Codigo.Value
                                                     select new NaturezaDespesaEntity
                                                     {
                                                         Id        = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                         Codigo    = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                         Descricao = ItemNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                     }).ToList<NaturezaDespesaEntity>()
                              }).AsQueryable();


            lStrSQL = qryConsulta.ToString();
            this.Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => lStrSQL = lStrSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            lLstRetorno = qryConsulta.ToList();

            this.totalregistros = lLstRetorno.Count();

            return lLstRetorno;
        }

        /// <summary>
        /// Procura as naturezas de despesas para o item de material selecionado
        /// </summary>
        /// <returns></returns>

        public ItemMaterialEntity GetItemMaterialNaturezaDespesa()
        {
            ItemMaterialEntity item = (from a in Db.TB_ITEM_MATERIALs
                                       where a.TB_ITEM_MATERIAL_ID == this.Entity.Id
                                       select new ItemMaterialEntity 
                                       { 
                                            Id = a.TB_ITEM_MATERIAL_ID,
                                            Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                            Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                            NaturezaDespesa = (from b in Db.TB_ITEM_NATUREZA_DESPESAs where
                                                               b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID == a.TB_ITEM_MATERIAL_ID
                                                               select new NaturezaDespesaEntity
                                                               {
                                                                    Id = b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                    Codigo = b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                    CodigoDescricao = string.Format("{0} - {1}", b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString(), b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO),
                                                                    Descricao = b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                               }).ToList()
                                        }
                                        ).FirstOrDefault();
            return item;
        }



        public bool SalvarSiafisico()
        {
            // armazenar dados do grupo
            GrupoEntity grupo = (from a in Db.TB_GRUPO_MATERIALs
                                 where a.TB_GRUPO_MATERIAL_CODIGO == Entity.Material.Classe.Grupo.Codigo
                                 select new GrupoEntity {   Id = a.TB_GRUPO_MATERIAL_ID,
                                                            Codigo = a.TB_GRUPO_MATERIAL_CODIGO,
                                                            Descricao = a.TB_GRUPO_MATERIAL_DESCRICAO
                                                            })
                                 .FirstOrDefault();

            if (grupo != null)
            {
                Entity.Material.Classe.Grupo = grupo;
            }
            else
            {
                // incluir o grupo Siafisico não existente no SAM
                GrupoInfraestructure grupoInfra = new GrupoInfraestructure();
                grupoInfra.Entity = new GrupoEntity { Codigo = Entity.Material.Classe.Grupo.Codigo, Descricao = Entity.Material.Classe.Grupo.Descricao };
                grupoInfra.Salvar();
                Entity.Material.Classe.Grupo = grupoInfra.Entity;
            }

            // armazenar dados da classe
            ClasseEntity classe = (from a in Db.TB_CLASSE_MATERIALs
                                   where a.TB_CLASSE_MATERIAL_CODIGO == Entity.Material.Classe.Codigo && 
                                         a.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO == Entity.Material.Classe.Grupo.Codigo
                                   select new ClasseEntity
                                   {
                                       Id = a.TB_CLASSE_MATERIAL_ID,
                                       Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                       Descricao = a.TB_CLASSE_MATERIAL_DESCRICAO,
                                       Grupo = Entity.Material.Classe.Grupo
                                   })
                                 .FirstOrDefault();

            if (classe != null)
            {
                Entity.Material.Classe = classe;
            }
            else
            {
                // incluir a classe Siafisico não existente no SAM
                ClasseInfraestructure classeInfra = new ClasseInfraestructure();
                classeInfra.Entity = new ClasseEntity 
                                     { 
                                        Codigo = Entity.Material.Classe.Codigo,
                                        Descricao = Entity.Material.Classe.Descricao, 
                                        Grupo = Entity.Material.Classe.Grupo 
                                     };
                classeInfra.Salvar();
                Entity.Material.Classe = classeInfra.Entity;
            }

            // armazenar dados do material
            MaterialEntity material = (from a in Db.TB_MATERIALs
                                       where a.TB_MATERIAL_CODIGO == Entity.Material.Codigo &&
                                             a.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO == Entity.Material.Classe.Codigo &&
                                             a.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO == Entity.Material.Classe.Grupo.Codigo
                                   select new MaterialEntity
                                   {
                                       Id = a.TB_MATERIAL_ID,
                                       Codigo = a.TB_MATERIAL_CODIGO,
                                       Descricao = a.TB_MATERIAL_DESCRICAO,
                                       Classe = Entity.Material.Classe
                                   })
                                 .FirstOrDefault();

            if (material != null)
            {
                Entity.Material = material;
            }
            else
            {
                // incluir o material Siafisico não existente no SAM
                MaterialInfraestructure materialInfra = new MaterialInfraestructure();
                materialInfra.Entity = new MaterialEntity
                                       {
                                           Codigo = Entity.Material.Codigo,
                                           Descricao = Entity.Material.Descricao,
                                           Classe = Entity.Material.Classe
                                       };
                materialInfra.Salvar();
                Entity.Material = materialInfra.Entity;
            }

            // armazenar dados do item de material
            ItemMaterialEntity itemMaterial = (from a in Db.TB_ITEM_MATERIALs
                                       where a.TB_ITEM_MATERIAL_CODIGO == Entity.Codigo &&
                                             a.TB_MATERIAL.TB_MATERIAL_CODIGO == Entity.Material.Codigo 
                                       select new ItemMaterialEntity
                                       {
                                           Id = a.TB_ITEM_MATERIAL_ID,
                                           Codigo = a.TB_ITEM_MATERIAL_CODIGO,
                                           Descricao = a.TB_ITEM_MATERIAL_DESCRICAO,
                                           ClasseId = Entity.Material.Classe.Id.Value,
                                           GrupoId = Entity.Material.Classe.Grupo.Id.Value,
                                           MaterialId = Entity.Material.Id.Value,
                                           Atividade = true,
                                           Material = Entity.Material
                                       })
                                 .FirstOrDefault();

            if (itemMaterial != null)
            {
                int[] naturezas = new int[] { Entity.NatDespSiafisicoCodigo1, Entity.NatDespSiafisicoCodigo2, Entity.NatDespSiafisicoCodigo3, Entity.NatDespSiafisicoCodigo4, Entity.NatDespSiafisicoCodigo5 };
                this.Entity = itemMaterial;
                this.Entity.NatDespSiafisicoCodigo1 = naturezas[0];
                this.Entity.NatDespSiafisicoCodigo2 = naturezas[1];
                this.Entity.NatDespSiafisicoCodigo3 = naturezas[2];
                this.Entity.NatDespSiafisicoCodigo4 = naturezas[3];
                this.Entity.NatDespSiafisicoCodigo5 = naturezas[4];
            }
            else
            {
                // incluir o item de material Siafisico não existente no SAM (nova instÃ¢ncia criada para manter dados do objeto a ser salvo)
                ItemMaterialInfraestructure itemMaterialInfra = new ItemMaterialInfraestructure();
                itemMaterialInfra.Entity = new ItemMaterialEntity
                {
                    Codigo = Entity.Codigo,
                    Descricao = Entity.Descricao,
                    Atividade = true,
                    Material = Entity.Material
                };
                itemMaterialInfra.Salvar();
                ItemMaterialEntity newItem = itemMaterialInfra.Entity;
                newItem.NatDespSiafisicoCodigo1 = Entity.NatDespSiafisicoCodigo1;
                newItem.NatDespSiafisicoCodigo2 = Entity.NatDespSiafisicoCodigo2;
                newItem.NatDespSiafisicoCodigo3 = Entity.NatDespSiafisicoCodigo3;
                newItem.NatDespSiafisicoCodigo4 = Entity.NatDespSiafisicoCodigo4;
                newItem.NatDespSiafisicoCodigo5 = Entity.NatDespSiafisicoCodigo5;
                this.Entity = newItem;
            }

            // incluir relação iten x naturezas de despesa
            this.SalvarItemNatDespesa();

            return true;
        }

        public bool SalvarItemNatDespesa()
        {
            try
            {
                ItemNaturezaDespesaEntity itemXnat = new ItemNaturezaDespesaEntity();
                itemXnat.NaturezasDespesa = new List<NaturezaDespesaEntity>();
                itemXnat.NaturezasDespesa.Add(new NaturezaDespesaEntity { Codigo = this.Entity.NatDespSiafisicoCodigo1 });
                itemXnat.NaturezasDespesa.Add(new NaturezaDespesaEntity { Codigo = this.Entity.NatDespSiafisicoCodigo2 });
                itemXnat.NaturezasDespesa.Add(new NaturezaDespesaEntity { Codigo = this.Entity.NatDespSiafisicoCodigo3 });
                itemXnat.NaturezasDespesa.Add(new NaturezaDespesaEntity { Codigo = this.Entity.NatDespSiafisicoCodigo4 });
                itemXnat.NaturezasDespesa.Add(new NaturezaDespesaEntity { Codigo = this.Entity.NatDespSiafisicoCodigo5 });
                foreach (NaturezaDespesaEntity nat in itemXnat.NaturezasDespesa)
                {
                    NaturezaDespesaEntity nat1 = (from a in this.Db.TB_NATUREZA_DESPESAs
                                where a.TB_NATUREZA_DESPESA_CODIGO == nat.Codigo
                                select new NaturezaDespesaEntity
                                {
                                    Id = a.TB_NATUREZA_DESPESA_ID,
                                    Codigo = a.TB_NATUREZA_DESPESA_CODIGO,
                                    Descricao = a.TB_NATUREZA_DESPESA_DESCRICAO
                                })
                        .FirstOrDefault();
                    if (nat1 != null && nat.Codigo != 0)
                    {
                        TB_ITEM_NATUREZA_DESPESA dbItemNat = new TB_ITEM_NATUREZA_DESPESA();

                        // verifica se já existe o item relacionado Ã  natureza de despesa
                        var itemXnatExiste = (from b in this.Db.TB_ITEM_NATUREZA_DESPESAs 
                                         where b.TB_ITEM_MATERIAL_ID == Entity.Id.Value &&
                                               b.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == nat1.Codigo
                                         select b).FirstOrDefault();

                        if (itemXnatExiste == null)
                        {
                        this.Db.TB_ITEM_NATUREZA_DESPESAs.InsertOnSubmit(dbItemNat);
                        dbItemNat.TB_NATUREZA_DESPESA_ID = nat1.Id.Value;
                        dbItemNat.TB_ITEM_MATERIAL_ID = Entity.Id.Value;
                        this.Db.SubmitChanges();
                    }
                }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int iNaturezaDespesa_ID)
        {
            bool blnRetorno = false;

            TB_ITEM_NATUREZA_DESPESA rowItemNaturezaDespesa = null;

            if (iItemMaterial_ID != 0 && iNaturezaDespesa_ID != 0)
                rowItemNaturezaDespesa = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(ItemMaterialNaturezaDespesa => ItemMaterialNaturezaDespesa.TB_ITEM_MATERIAL_ID == iItemMaterial_ID &&
                                                                                                                ItemMaterialNaturezaDespesa.TB_NATUREZA_DESPESA_ID == iNaturezaDespesa_ID)
                                                                          .FirstOrDefault();
            else
                this.Db.TB_ITEM_NATUREZA_DESPESAs.InsertOnSubmit(rowItemNaturezaDespesa);

            if (rowItemNaturezaDespesa == null || rowItemNaturezaDespesa.TB_ITEM_NATUREZA_DESPESA_ID != 0)
            {
                rowItemNaturezaDespesa = new TB_ITEM_NATUREZA_DESPESA();
                rowItemNaturezaDespesa.TB_ITEM_MATERIAL_ID = iItemMaterial_ID;
                rowItemNaturezaDespesa.TB_NATUREZA_DESPESA_ID = iNaturezaDespesa_ID;

                this.Db.TB_ITEM_NATUREZA_DESPESAs.InsertOnSubmit(rowItemNaturezaDespesa);
                this.Db.SubmitChanges();
                blnRetorno = true;
            }

            return blnRetorno;
        }

        public bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa)
        {
            bool blnRetorno = false;
            int iCodigoNaturezaDespesa = 0;
            int iNaturezaDespesa_ID = 0;

            TB_ITEM_NATUREZA_DESPESA rowItemNaturezaDespesa = null;


            Int32.TryParse(strCodigoNaturezaDespesa, out iCodigoNaturezaDespesa);

            if (this.Db.DeferredLoadingEnabled)
                this.Db.DeferredLoadingEnabled = false;

            var rowNaturezaDespesa = this.Db.TB_NATUREZA_DESPESAs.Where(naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == iCodigoNaturezaDespesa)
                                                                 .FirstOrDefault();

            //iNaturezaDespesa_ID = rowNaturezaDespesa.TB_NATUREZA_DESPESA_ID;
            iNaturezaDespesa_ID = (rowNaturezaDespesa.IsNotNull() ? rowNaturezaDespesa.TB_NATUREZA_DESPESA_ID : 0);

            if (iItemMaterial_ID != 0 && iNaturezaDespesa_ID != 0)
                rowItemNaturezaDespesa = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(ItemMaterialNaturezaDespesa => ItemMaterialNaturezaDespesa.TB_ITEM_MATERIAL_ID == iItemMaterial_ID &&
                                                                                                                ItemMaterialNaturezaDespesa.TB_NATUREZA_DESPESA_ID == iNaturezaDespesa_ID)
                                                                          .FirstOrDefault();
            else if (rowItemNaturezaDespesa.IsNotNull())
                this.Db.TB_ITEM_NATUREZA_DESPESAs.InsertOnSubmit(rowItemNaturezaDespesa);

            //if (rowItemNaturezaDespesa == null || rowItemNaturezaDespesa.TB_ITEM_NATUREZA_DESPESA_ID != 0)
            if (rowItemNaturezaDespesa.IsNull() && iNaturezaDespesa_ID != 0)
            {
                rowItemNaturezaDespesa = new TB_ITEM_NATUREZA_DESPESA();
                rowItemNaturezaDespesa.TB_ITEM_MATERIAL_ID = iItemMaterial_ID;
                rowItemNaturezaDespesa.TB_NATUREZA_DESPESA_ID = iNaturezaDespesa_ID;

                this.Db.TB_ITEM_NATUREZA_DESPESAs.InsertOnSubmit(rowItemNaturezaDespesa);
                this.Db.SubmitChanges();
                blnRetorno = true;
            }

            return blnRetorno;
        }

        public bool SalvarRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa, bool throwSeNaoExistir = false)
        {
            bool blnRetorno = false;

            var rowNaturezaDespesa = this.Db.TB_NATUREZA_DESPESAs.Where(naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == Int32.Parse(strCodigoNaturezaDespesa)).FirstOrDefault();
            var rowItemMaterial = this.Db.TB_ITEM_MATERIALs.Where(itemMaterial => itemMaterial.TB_ITEM_MATERIAL_ID == iItemMaterial_ID).FirstOrDefault();

            if (throwSeNaoExistir && rowNaturezaDespesa.IsNull())
                throw new Exception(String.Format("Natureza de Despesa {0} não cadastrada no sistema.", strCodigoNaturezaDespesa));
            else if (!throwSeNaoExistir)
                return this.SalvarRelacaoItemNaturezaDespesa(iItemMaterial_ID, strCodigoNaturezaDespesa);

            return blnRetorno;
        }

        public bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, int iNaturezaDespesa_ID)
        {
            bool blnRetorno = false;

            TB_ITEM_NATUREZA_DESPESA rowItemNaturezaDespesa = null;

            if (iItemMaterial_ID != 0 && iNaturezaDespesa_ID != 0)
                rowItemNaturezaDespesa = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(ItemMaterialNaturezaDespesa => ItemMaterialNaturezaDespesa.TB_ITEM_MATERIAL_ID == iItemMaterial_ID &&
                                                                                                                ItemMaterialNaturezaDespesa.TB_NATUREZA_DESPESA_ID == iNaturezaDespesa_ID)
                                                                          .FirstOrDefault();
            blnRetorno = !rowItemNaturezaDespesa.IsNull();

            return blnRetorno;
        }

        public bool ExisteRelacaoItemNaturezaDespesa(int iItemMaterial_ID, string strCodigoNaturezaDespesa)
        {
            bool blnRetorno = false;
            int  iCodigoNaturezaDespesa = 0;

            TB_ITEM_NATUREZA_DESPESA rowItemNaturezaDespesa = null;

            Int32.TryParse(strCodigoNaturezaDespesa, out iCodigoNaturezaDespesa);

            if (iItemMaterial_ID != 0 && iCodigoNaturezaDespesa != 0)
                rowItemNaturezaDespesa = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(ItemMaterialNaturezaDespesa => ItemMaterialNaturezaDespesa.TB_ITEM_MATERIAL_ID == iItemMaterial_ID &&
                                                                                                                ItemMaterialNaturezaDespesa.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == iCodigoNaturezaDespesa)
                                                                          .FirstOrDefault();
            blnRetorno = !rowItemNaturezaDespesa.IsNull();

            return blnRetorno;
        }

        public ItemMaterialEntity ObterItemMaterial(int iCodigoItemMaterial)
        {
            ItemMaterialEntity objRetorno;

            objRetorno  =  (from ItemMaterial in this.Db.TB_ITEM_MATERIALs
                            where ItemMaterial.TB_ITEM_MATERIAL_CODIGO == iCodigoItemMaterial
                            select new ItemMaterialEntity()
                            {
                                Id = ItemMaterial.TB_ITEM_MATERIAL_ID,
                                Codigo = ItemMaterial.TB_ITEM_MATERIAL_CODIGO,
                                Descricao = ItemMaterial.TB_ITEM_MATERIAL_DESCRICAO,
                                GrupoId = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                ClasseId = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                MaterialId = ItemMaterial.TB_MATERIAL.TB_CLASSE_MATERIAL_ID,
                            }).FirstOrDefault();
            
            return objRetorno;
        }
    }
}
