using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
namespace Sam.Domain.Infrastructure
{
    public class MaterialInfraestructure : BaseInfraestructure, IMaterialService
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


        public MaterialEntity Entity { get; set; }


        public MaterialEntity Select(int _id)
        {

            MaterialEntity info;

            TB_MATERIAL material = (from a in this.Db.TB_MATERIALs where a.TB_MATERIAL_ID == _id select a).FirstOrDefault();

            info = new MaterialEntity();

            if (material != null)
            {
                info.Id = _id;
                info.Codigo = material.TB_MATERIAL_CODIGO;
                info.Descricao = material.TB_MATERIAL_DESCRICAO;
                info.Classe = (new ClasseEntity(material.TB_CLASSE_MATERIAL_ID));
            }

            return info;
        }

        public IList<MaterialEntity> Listar()
        {
            IList<MaterialEntity> resultado = (from a in this.Db.TB_MATERIALs
                                               orderby a.TB_MATERIAL_DESCRICAO
                                               select new MaterialEntity
                                               {
                                                   Id = a.TB_MATERIAL_ID,
                                                   Descricao = a.TB_MATERIAL_DESCRICAO,
                                                   Codigo = a.TB_MATERIAL_CODIGO,
                                                   //Grupo = (new GrupoEntity(a.TB_GRUPO_MATERIAL_ID)),
                                                   Classe = (new ClasseEntity(a.TB_CLASSE_MATERIAL_ID))
                                               }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<MaterialEntity>();

            this.totalregistros = (from a in this.Db.TB_MATERIALs
                                   select new
                                   {
                                       Id = a.TB_MATERIAL_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MaterialEntity> Listar( int _classeId)
        {
            IList<MaterialEntity> resultado = (from a in this.Db.TB_MATERIALs
                                             where a.TB_CLASSE_MATERIAL_ID == _classeId
                                             orderby a.TB_MATERIAL_DESCRICAO
                                             select new MaterialEntity
                                             {
                                                 Id = a.TB_MATERIAL_ID,
                                                 Descricao = a.TB_MATERIAL_DESCRICAO,
                                                 Codigo = a.TB_MATERIAL_CODIGO,
                                                 //Grupo = (new GrupoEntity(a.TB_GRUPO_MATERIAL_ID)),
                                                 Classe = (new ClasseEntity(a.TB_CLASSE_MATERIAL_ID))
                                             }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<MaterialEntity>();

            this.totalregistros = (from a in this.Db.TB_MATERIALs
                                   where a.TB_CLASSE_MATERIAL_ID == _classeId
                                   select new
                                   {
                                       Id = a.TB_MATERIAL_ID,
                                   }).Count();
            return resultado;

        }

        public IList<MaterialEntity> ListarTodosCod(int _classeId)
        {

            MaterialEntity mat = new MaterialEntity();
            mat.Id = 0;
            mat.Descricao = "- Todos -";


            IList<MaterialEntity> resultado = (from a in this.Db.TB_MATERIALs
                                               where a.TB_CLASSE_MATERIAL_ID == _classeId
                                               orderby a.TB_MATERIAL_DESCRICAO
                                               select new MaterialEntity
                                               {
                                                   Id = a.TB_MATERIAL_ID,
                                                   Descricao = string.Format("{0} - {1}", a.TB_MATERIAL_CODIGO.ToString().PadLeft(8, '0'), a.TB_MATERIAL_DESCRICAO),
                                                   Codigo = a.TB_MATERIAL_CODIGO,                                                   
                                                   Classe = (new ClasseEntity(a.TB_CLASSE_MATERIAL_ID))
                                               }).ToList<MaterialEntity>();

            resultado.Insert(0, mat);

            return resultado;
        }


        public IList<MaterialEntity> ListarTodosCod(int _classeId, AlmoxarifadoEntity almoxarifado)
        {
            IList<MaterialEntity> resultado = (from material in this.Db.TB_MATERIALs
            join item_material in Db.TB_ITEM_MATERIALs on material.TB_MATERIAL_ID equals item_material.TB_MATERIAL_ID
            //join item_subitem_material in Db.TB_ITEM_SUBITEM_MATERIALs on item_material.TB_ITEM_MATERIAL_ID equals item_subitem_material.TB_ITEM_MATERIAL_ID
            //join subitem_material in Db.TB_SUBITEM_MATERIALs on item_subitem_material.TB_SUBITEM_MATERIAL_ID  equals subitem_material.TB_SUBITEM_MATERIAL_ID
            //join subitem_material_almox in Db.TB_SUBITEM_MATERIAL_ALMOXes on subitem_material.TB_SUBITEM_MATERIAL_ID equals subitem_material_almox.TB_SUBITEM_MATERIAL_ID
                                               where (material.TB_CLASSE_MATERIAL_ID == _classeId)
                                               //where (subitem_material_almox.TB_ALMOXARIFADO_ID  == almoxarifado.Id)
                                               orderby material.TB_MATERIAL_DESCRICAO
                                               select new MaterialEntity
                                               {
                                                   Id = material.TB_MATERIAL_ID,
                                                   Descricao = string.Format("{0} - {1}", material.TB_MATERIAL_CODIGO.ToString().PadLeft(8, '0'), material.TB_MATERIAL_DESCRICAO),
                                                   Codigo = material.TB_MATERIAL_CODIGO,
                                                   Classe = (new ClasseEntity(material.TB_CLASSE_MATERIAL_ID))
                                               }).Distinct().ToList<MaterialEntity>();

            return resultado;
        }

        public IList<MaterialEntity> Imprimir()
        {
            IList<MaterialEntity> resultado = (from a in this.Db.TB_MATERIALs
                                               orderby a.TB_MATERIAL_DESCRICAO
                                               select new MaterialEntity
                                               {
                                                   Id = a.TB_MATERIAL_ID,
                                                   Descricao = a.TB_MATERIAL_DESCRICAO,
                                                   Codigo = a.TB_MATERIAL_CODIGO,
                                                   //Grupo = (new GrupoEntity(a.TB_GRUPO_MATERIAL_ID)),
                                                   Classe = (new ClasseEntity(a.TB_CLASSE_MATERIAL_ID))
                                               }).ToList<MaterialEntity>();


            return resultado;

        }

        public IList<MaterialEntity> Imprimir(int _classeId)
        {
            IList<MaterialEntity> resultado = (from a in this.Db.TB_MATERIALs
                                               where a.TB_CLASSE_MATERIAL_ID == _classeId
                                               orderby a.TB_MATERIAL_DESCRICAO
                                               select new MaterialEntity
                                               {
                                                   Id = a.TB_MATERIAL_ID,
                                                   Descricao = a.TB_MATERIAL_DESCRICAO,
                                                   Codigo = a.TB_MATERIAL_CODIGO,
                                                   //Grupo = (new GrupoEntity(a.TB_GRUPO_MATERIAL_ID)),
                                                   Classe = (new ClasseEntity(a.TB_CLASSE_MATERIAL_ID))
                                               })
                                          .ToList<MaterialEntity>();

          
            return resultado;

        }

        public void Excluir()
        {
            TB_MATERIAL material
                   = this.Db.TB_MATERIALs.Where(a => a.TB_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_MATERIALs.DeleteOnSubmit(material);
            this.Db.SubmitChanges();
        }



        public void Salvar()
        {
            TB_MATERIAL material = new TB_MATERIAL();            

            if (this.Entity.Id.HasValue)
                material = this.Db.TB_MATERIALs.Where(a => a.TB_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_MATERIALs.InsertOnSubmit(material);

            material.TB_MATERIAL_CODIGO = this.Entity.Codigo;
            material.TB_MATERIAL_DESCRICAO = this.Entity.Descricao;
            material.TB_CLASSE_MATERIAL_ID = this.Entity.Classe.Id.Value;

            this.Db.SubmitChanges();
            this.Entity.Id = material.TB_MATERIAL_ID; // retornando ID devido ao cadastro do item de material via Siafisico

        }


        public bool PodeExcluir()
        {

            int qtd = int.MinValue;

            qtd = (from a in this.Db.TB_ITEM_MATERIALs
                   where a.TB_MATERIAL_ID == this.Entity.Id
                   select new
                   {
                       Id = a.TB_ITEM_MATERIAL_ID,
                   }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_MATERIALs
                .Where(a => a.TB_MATERIAL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_MATERIAL_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_MATERIALs
                .Where(a => a.TB_MATERIAL_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }

        
        MaterialEntity ICrudBaseService<MaterialEntity>.LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<MaterialEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }


        public MaterialEntity ObterMaterial(int codigoMaterial)
        {
            MaterialEntity objRetorno = (from material in this.Db.TB_MATERIALs
                                          where material.TB_MATERIAL_CODIGO == codigoMaterial
                                          orderby material.TB_MATERIAL_DESCRICAO
                                          select new MaterialEntity
                                          {
                                             Id        = material.TB_MATERIAL_ID,
                                             Descricao = material.TB_MATERIAL_DESCRICAO,
                                             Codigo    = material.TB_MATERIAL_CODIGO,
                                             Classe    = new ClasseEntity() {
                                                                               Id =  material.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_ID,
                                                                               Codigo = material.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_CODIGO,
                                                                               Descricao = material.TB_CLASSE_MATERIAL.TB_CLASSE_MATERIAL_DESCRICAO,
                                                                               Grupo = new GrupoEntity() {
                                                                                                            Id = material.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                                                                                            Codigo = material.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO,
                                                                                                            Descricao = material.TB_CLASSE_MATERIAL.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_DESCRICAO
                                                                               }
                                             },
                                             
                                          }).FirstOrDefault();
            return objRetorno;
        }

    }
}
