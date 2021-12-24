using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;


namespace Sam.Domain.Infrastructure
{
    public class ClasseInfraestructure : BaseInfraestructure, IClasseService
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


        public ClasseEntity Entity { get; set; }


        public ClasseEntity Select(int _id)
        {

            ClasseEntity info;

            TB_CLASSE_MATERIAL classe = (from a in this.Db.TB_CLASSE_MATERIALs where a.TB_CLASSE_MATERIAL_ID == _id select a).FirstOrDefault();

            info = new ClasseEntity();

            if (classe != null)
            {
                info.Id = _id;
                info.Codigo = classe.TB_CLASSE_MATERIAL_CODIGO;
                info.Descricao = classe.TB_CLASSE_MATERIAL_DESCRICAO;
                info.Grupo = (new GrupoEntity(classe.TB_GRUPO_MATERIAL_ID));
            }

            return info;
        }

        public IList<ClasseEntity> Listar()
        {
            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = a.TB_CLASSE_MATERIAL_DESCRICAO,
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             }).Skip(this.SkipRegistros)
                                         .Take(this.RegistrosPagina)
                                         .ToList<ClasseEntity>();

            this.totalregistros = (from a in this.Db.TB_CLASSE_MATERIALs
                                   select new
                                   {
                                       Id = a.TB_CLASSE_MATERIAL_ID,
                                   }).Count();
            return resultado;
        }

        public IList<ClasseEntity> Listar(int _grupoId)
        {
            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             where a.TB_GRUPO_MATERIAL_ID == _grupoId
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = a.TB_CLASSE_MATERIAL_DESCRICAO,
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<ClasseEntity>();

            this.totalregistros = (from a in this.Db.TB_CLASSE_MATERIALs
                                   where a.TB_GRUPO_MATERIAL_ID == _grupoId 
                                   select new
                                   {
                                       Id = a.TB_CLASSE_MATERIAL_ID,
                                   }).Count();
            return resultado;

        }

        public IList<ClasseEntity> ListarTodosCod(int _grupoId)
        {

            ClasseEntity classe = new ClasseEntity();
            classe.Id = 0;
            classe.Descricao = "- Todos -";

            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             where a.TB_GRUPO_MATERIAL_ID == _grupoId
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = string.Format("{0} - {1}", a.TB_CLASSE_MATERIAL_CODIGO.ToString().PadLeft(4, '0'), a.TB_CLASSE_MATERIAL_DESCRICAO),                                                 
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             }).ToList<ClasseEntity>();

            resultado.Insert(0, classe);

            return resultado;
        }

        public IList<ClasseEntity> ListarTodosCod(int _grupoId, bool blnRetornarTodas)
        {
            if (!blnRetornarTodas)
                return this.ListarTodosCod(_grupoId);

            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = string.Format("{0} - {1}", a.TB_CLASSE_MATERIAL_CODIGO.ToString().PadLeft(4, '0'), a.TB_CLASSE_MATERIAL_DESCRICAO),
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             }).ToList<ClasseEntity>();
            return resultado;
        }

        public IList<ClasseEntity> Imprimir()
        {
            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = a.TB_CLASSE_MATERIAL_DESCRICAO,
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             })
                                         .ToList<ClasseEntity>();
       
            return resultado;
        }

        public IList<ClasseEntity> Imprimir(int _grupoId)
        {
            IList<ClasseEntity> resultado = (from a in this.Db.TB_CLASSE_MATERIALs
                                             where a.TB_GRUPO_MATERIAL_ID == _grupoId
                                             orderby a.TB_CLASSE_MATERIAL_DESCRICAO
                                             select new ClasseEntity
                                             {
                                                 Id = a.TB_CLASSE_MATERIAL_ID,
                                                 Descricao = a.TB_CLASSE_MATERIAL_DESCRICAO,
                                                 Codigo = a.TB_CLASSE_MATERIAL_CODIGO,
                                                 Grupo = (new GrupoEntity(a.TB_CLASSE_MATERIAL_ID))
                                             })
                                          .ToList<ClasseEntity>();

            return resultado;
        }


        public void Excluir()
        {
            TB_CLASSE_MATERIAL classe
                   = this.Db.TB_CLASSE_MATERIALs.Where(a => a.TB_CLASSE_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_CLASSE_MATERIALs.DeleteOnSubmit(classe);
            this.Db.SubmitChanges();
        }



        public void Salvar()
        {
            TB_CLASSE_MATERIAL classe = new TB_CLASSE_MATERIAL();

            if (this.Entity.Id.HasValue)
                classe = this.Db.TB_CLASSE_MATERIALs.Where(a => a.TB_CLASSE_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_CLASSE_MATERIALs.InsertOnSubmit(classe);

            classe.TB_CLASSE_MATERIAL_CODIGO = this.Entity.Codigo;
            classe.TB_CLASSE_MATERIAL_DESCRICAO = this.Entity.Descricao;
            classe.TB_GRUPO_MATERIAL_ID = this.Entity.Grupo.Id.Value;
            this.Db.SubmitChanges();
            this.Entity.Id = classe.TB_CLASSE_MATERIAL_ID; // retornando ID devido ao cadastro do item de material via Siafisico
        }


        public bool PodeExcluir()
        {

            int qtd = int.MinValue;

            qtd = (from a in this.Db.TB_MATERIALs
                   where a.TB_CLASSE_MATERIAL_ID == this.Entity.Id
                   select new
                   {
                       Id = a.TB_MATERIAL_ID,
                   }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_CLASSE_MATERIALs
                .Where(a => a.TB_CLASSE_MATERIAL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_CLASSE_MATERIAL_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_CLASSE_MATERIALs
                .Where(a => a.TB_CLASSE_MATERIAL_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }

        public ClasseEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<ClasseEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public ClasseEntity ObterClasse(int codigoClasseMaterial)
        {
            ClasseEntity objRetorno = (from classeMaterial in this.Db.TB_CLASSE_MATERIALs
                                       where classeMaterial.TB_CLASSE_MATERIAL_CODIGO == codigoClasseMaterial
                                       orderby classeMaterial.TB_CLASSE_MATERIAL_CODIGO
                                       select new ClasseEntity
                                       {
                                           Id = classeMaterial.TB_CLASSE_MATERIAL_ID,
                                           Descricao = classeMaterial.TB_CLASSE_MATERIAL_DESCRICAO,
                                           Codigo = classeMaterial.TB_CLASSE_MATERIAL_CODIGO,
                                           Grupo = new GrupoEntity()
                                           {
                                               Id = classeMaterial.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_ID,
                                               Codigo = classeMaterial.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_CODIGO,
                                               Descricao = classeMaterial.TB_GRUPO_MATERIAL.TB_GRUPO_MATERIAL_DESCRICAO
                                           },
                                       }).FirstOrDefault();
            return objRetorno;
        }

    }
}
