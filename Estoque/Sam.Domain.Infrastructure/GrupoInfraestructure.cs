using System;
using System.Collections.Generic;
using Sam.Domain.Entity;
using System.Collections;
using System.Linq;
using Sam.ServiceInfraestructure;
using System.Configuration;



namespace Sam.Domain.Infrastructure
{
    public class GrupoInfraestructure : BaseInfraestructure, IGrupoService
    { 

        public GrupoEntity Entity { get; set; }

        public IList<GrupoEntity> Listar()
        {
            IList<GrupoEntity> resultado = (from a in this.Db.TB_GRUPO_MATERIALs
                                            orderby a.TB_GRUPO_MATERIAL_DESCRICAO
                                            select new GrupoEntity
                                         {
                                             Id = a.TB_GRUPO_MATERIAL_ID,
                                             Descricao = a.TB_GRUPO_MATERIAL_DESCRICAO,
                                             Codigo = a.TB_GRUPO_MATERIAL_CODIGO,
                                         }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<GrupoEntity>();

            this.totalregistros = (from a in this.Db.TB_GRUPO_MATERIALs
                                   select new
                                   {
                                       Id = a.TB_GRUPO_MATERIAL_ID,
                                   }).Count();
            
            return resultado;

        }

        public IList<GrupoEntity> ListarTodosCod()
        {
            IList<GrupoEntity> resultado = (from a in this.Db.TB_GRUPO_MATERIALs
                                            orderby a.TB_GRUPO_MATERIAL_DESCRICAO
                                            select new GrupoEntity
                                            {
                                                Id = a.TB_GRUPO_MATERIAL_ID,
                                                Descricao = string.Format("{0} - {1}", a.TB_GRUPO_MATERIAL_CODIGO.ToString().PadLeft(2,'0'), a.TB_GRUPO_MATERIAL_DESCRICAO),
                                                Codigo = a.TB_GRUPO_MATERIAL_CODIGO,
                                            })
                               .ToList<GrupoEntity>();
            return resultado;
        }

        public IList<GrupoEntity> Listar(System.Linq.Expressions.Expression<Func<GrupoEntity, bool>> where)
        {
            IList<GrupoEntity> resultado = (from a in this.Db.TB_GRUPO_MATERIALs
                                            orderby a.TB_GRUPO_MATERIAL_DESCRICAO
                                            select new GrupoEntity
                                            {
                                                Id = a.TB_GRUPO_MATERIAL_ID,
                                                Descricao = string.Format("{0} - {1}", a.TB_GRUPO_MATERIAL_CODIGO.ToString().PadLeft(2, '0'), a.TB_GRUPO_MATERIAL_DESCRICAO),
                                                Codigo = a.TB_GRUPO_MATERIAL_CODIGO,
                                            }).Where(where)
                                            .ToList<GrupoEntity>();
            return resultado;
        }

        public IList<GrupoEntity> ListarTodosCod(AlmoxarifadoEntity almoxarifado)
        {

            GrupoEntity grupo = new GrupoEntity();
            grupo.Id = 0;
            grupo.Descricao = "- Todos -";

            var resultado = (from g in this.Db.TB_GRUPO_MATERIALs
                             orderby g.TB_GRUPO_MATERIAL_DESCRICAO ascending
                             select new GrupoEntity
                             {
                                 Id = g.TB_GRUPO_MATERIAL_ID,
                                 Descricao = g.TB_GRUPO_MATERIAL_DESCRICAO,
                                 Codigo = g.TB_GRUPO_MATERIAL_CODIGO
                             }).Distinct().OrderBy(a => a.Descricao).ToList();

            foreach (var r in resultado)
            {
                r.Descricao = string.Format("{0} - {1}", r.Codigo.ToString().PadLeft(2, '0'), r.Descricao.ToString());
            }


            resultado.Insert(0, grupo);

            return resultado;
        }

        public IList<GrupoEntity> Imprimir()
        {
            IList<GrupoEntity> resultado = (from a in this.Db.TB_GRUPO_MATERIALs
                                            orderby a.TB_GRUPO_MATERIAL_DESCRICAO
                                            select new GrupoEntity
                                            {
                                                Id = a.TB_GRUPO_MATERIAL_ID,
                                                Descricao = a.TB_GRUPO_MATERIAL_DESCRICAO,
                                                Codigo = a.TB_GRUPO_MATERIAL_CODIGO,
                                            })
                                          .ToList<GrupoEntity>();

          
            return resultado;

        }

        public void Excluir()
        {
            TB_GRUPO_MATERIAL grupo
                   = this.Db.TB_GRUPO_MATERIALs.Where(a => a.TB_GRUPO_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_GRUPO_MATERIALs.DeleteOnSubmit(grupo);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_GRUPO_MATERIAL grupo = new TB_GRUPO_MATERIAL();

            if (this.Entity.Id.HasValue)
                grupo = this.Db.TB_GRUPO_MATERIALs.Where(a => a.TB_GRUPO_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_GRUPO_MATERIALs.InsertOnSubmit(grupo);

            grupo.TB_GRUPO_MATERIAL_CODIGO = this.Entity.Codigo;
            grupo.TB_GRUPO_MATERIAL_DESCRICAO = this.Entity.Descricao;

            this.Db.SubmitChanges();
            this.Entity.Id = grupo.TB_GRUPO_MATERIAL_ID; // retornando ID devido ao cadastro do item de material via Siafisico
        }

        public bool PodeExcluir()
        {

            int qtd = int.MinValue;

            qtd = (from a in this.Db.TB_CLASSE_MATERIALs
                   where a.TB_GRUPO_MATERIAL_ID == this.Entity.Id
                   select new
                   {
                       Id = a.TB_CLASSE_MATERIAL_ID
                   }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_GRUPO_MATERIALs
                .Where(a => a.TB_GRUPO_MATERIAL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GRUPO_MATERIAL_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_GRUPO_MATERIALs
                .Where(a => a.TB_GRUPO_MATERIAL_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }

        public GrupoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public GrupoEntity ObterGrupoMaterial(int codigoGrupoMaterial)
        {
            GrupoEntity lObjRetorno = (from GrupoMaterial in this.Db.TB_GRUPO_MATERIALs
                                       where GrupoMaterial.TB_GRUPO_MATERIAL_CODIGO == codigoGrupoMaterial
                                       orderby GrupoMaterial.TB_GRUPO_MATERIAL_DESCRICAO ascending
                                       select new GrupoEntity
                                       {
                                           Id        = GrupoMaterial.TB_GRUPO_MATERIAL_ID,
                                           Descricao = GrupoMaterial.TB_GRUPO_MATERIAL_DESCRICAO,
                                           Codigo    = GrupoMaterial.TB_GRUPO_MATERIAL_CODIGO
                                       }).FirstOrDefault();
            return lObjRetorno;
        }

    }
}
