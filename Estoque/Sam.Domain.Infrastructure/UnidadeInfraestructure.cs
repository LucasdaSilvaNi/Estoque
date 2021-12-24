using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public class UnidadeInfraestructure : BaseInfraestructure,IUnidadeService
    {
        private UnidadeEntity unidade = new UnidadeEntity();
        
        public UnidadeEntity Entity
        {
            get { return unidade; }
            set { unidade = value; }
        }

        public void Salvar()
        {
            TB_UNIDADE tbUnidade = new TB_UNIDADE();

            if (this.Entity.Id.HasValue)
                tbUnidade = this.Db.TB_UNIDADEs.Where(a => a.TB_UNIDADE_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                Db.TB_UNIDADEs.InsertOnSubmit(tbUnidade);

            tbUnidade.TB_UNIDADE_CODIGO = (int)this.Entity.Codigo.Value;
            tbUnidade.TB_UNIDADE_DESCRICAO = this.Entity.Descricao;
            tbUnidade.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;

            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_UNIDADE tbUnidade
                = this.Db.TB_UNIDADEs.Where(a => a.TB_UNIDADE_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UNIDADEs.DeleteOnSubmit(tbUnidade);
            this.Db.SubmitChanges();
        }

        public IList<UnidadeEntity> Listar()
        {
            IList<UnidadeEntity> resultado = (from a in this.Db.TB_UNIDADEs
                                              orderby a.TB_UNIDADE_ID
                                              select new UnidadeEntity
                                              {
                                                  Id = a.TB_UNIDADE_ID,
                                                  Codigo = a.TB_UNIDADE_CODIGO,
                                                  Descricao = a.TB_UNIDADE_DESCRICAO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<UnidadeEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADEs
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_ID,
                                   }).Count();

            return resultado;

        }

        public IList<UnidadeEntity> Listar(int? GestorId)
        {
            IList<UnidadeEntity> resultado = (from a in this.Db.TB_UNIDADEs
                                              where (a.TB_GESTOR_ID == GestorId)
                                              orderby a.TB_UNIDADE_CODIGO
                                              select new UnidadeEntity
                                              {
                                                  Id = a.TB_UNIDADE_ID,
                                                  Codigo = a.TB_UNIDADE_CODIGO,
                                                  Descricao = a.TB_UNIDADE_DESCRICAO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              }).Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<UnidadeEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADEs
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UnidadeEntity> Listar(int OrgaoId, int GestorId)
        {
            //IList<UnidadeEntity> resultado = (from a in this.Db.VW_UNIDADEs
            //                                  where (a.TB_ORGAO_ID == OrgaoId)
            //                                  where (a.TB_GESTOR_ID == GestorId)
            //                                  orderby a.TB_UNIDADE_ID
            //                                  select new UnidadeEntity
            //                                  {
            //                                      Id = a.TB_UNIDADE_ID,
            //                                      Codigo = a.TB_UNIDADE_CODIGO,
            //                                      Descricao = a.TB_UNIDADE_DESCRICAO,
            //                                      Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                      Ordem = a.ORDEM
            //                                  }).Skip(this.SkipRegistros)
            //                                 .Take(this.RegistrosPagina)
            //                                 .ToList<UnidadeEntity>();

            //this.totalregistros = (from a in this.Db.VW_UNIDADEs
            //                       where (a.TB_ORGAO_ID == OrgaoId)
            //                       where (a.TB_GESTOR_ID == GestorId)
            //                       select new
            //                       {
            //                           Id = a.TB_UNIDADE_ID,
            //                       }).Count();

            //return resultado;

            return new List<UnidadeEntity>();

        }

        public IList<UnidadeEntity> Imprimir()
        {
            //IList<UnidadeEntity> resultado = (from a in this.Db.VW_UNIDADEs
            //                                  orderby a.TB_UNIDADE_ID
            //                                  select new UnidadeEntity
            //                                  {
            //                                      Id = a.TB_UNIDADE_ID,
            //                                      Codigo = a.TB_UNIDADE_CODIGO,
            //                                      Descricao = a.TB_UNIDADE_DESCRICAO,
            //                                      Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                      Ordem = a.ORDEM
            //                                  }).ToList<UnidadeEntity>();

            //return resultado;

            return new List<UnidadeEntity>();
        }

        public IList<UnidadeEntity> Imprimir(int OrgaoId, int GestorId)
        {
            IList<UnidadeEntity> resultado = (from a in this.Db.TB_UNIDADEs
                                              join ges in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals ges.TB_GESTOR_ID
                                              join org in this.Db.TB_ORGAOs on ges.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                              where (org.TB_ORGAO_ID == OrgaoId)
                                              where (a.TB_GESTOR_ID == GestorId)
                                              orderby a.TB_UNIDADE_CODIGO
                                              select new UnidadeEntity
                                              {
                                                  Id = a.TB_UNIDADE_ID,
                                                  Codigo = a.TB_UNIDADE_CODIGO,
                                                  Descricao = a.TB_UNIDADE_DESCRICAO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              }).ToList<UnidadeEntity>();

            return resultado;

        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_UNIDADE tbUnidade = this.Db.TB_UNIDADEs.Where(a => a.TB_UNIDADE_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbUnidade. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_UNIDADEs
                .Where(a => a.TB_UNIDADE_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_UNIDADE_ID != this.Entity.Id.Value)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_UNIDADEs
                .Where(a => a.TB_UNIDADE_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            return retorno;
        }



        public UnidadeEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<UnidadeEntity> ListarTodosCod() { 
            return new List<UnidadeEntity>();
        }

        public IList<UnidadeEntity> ListarTodosCod(int OrgaoId)
        {
            IList<UnidadeEntity> resultado = (from a in this.Db.TB_UNIDADEs
                                              join ges in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals ges.TB_GESTOR_ID
                                              join org in this.Db.TB_ORGAOs on ges.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                              where (org.TB_ORGAO_ID == OrgaoId)
                                              orderby a.TB_UNIDADE_DESCRICAO
                                              select new UnidadeEntity
                                              {
                                                  Id = a.TB_UNIDADE_ID,
                                                  Codigo = a.TB_UNIDADE_CODIGO,
                                                  Descricao = a.TB_UNIDADE_DESCRICAO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              })
                                             .ToList();

            this.totalregistros = (from a in this.Db.TB_UNIDADEs
                                   join ges in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals ges.TB_GESTOR_ID
                                   join org in this.Db.TB_ORGAOs on ges.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (org.TB_ORGAO_ID == OrgaoId)
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_ID,
                                   }).Count();

            return resultado;
        }
    }
}
