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
    public class SiglaInfraestructure : BaseInfraestructure,ISiglaService
    {
        private SiglaEntity Sigla = new SiglaEntity();

        public SiglaEntity Entity
        {
            get { return Sigla; }
            set { Sigla = value; }
        }

        public void Salvar()
        {
            //TB_SIGLA tbSigla = new TB_SIGLA();

            //if (this.Entity.Id.HasValue)
            //    tbSigla = this.Db.TB_SIGLAs.Where(a => a.TB_SIGLA_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    Db.TB_SIGLAs.InsertOnSubmit(tbSigla);

            //tbSigla.TB_SIGLA_CODIGO = this.Entity.Codigo.Value;
            //tbSigla.TB_SIGLA_DESCRICAO = this.Entity.Descricao;
            //tbSigla.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
            //tbSigla.TB_ORGAO_ID = this.Entity.Orgao.Id.Value;
            //tbSigla.TB_INDICADOR_BEM_PROPRIO_ID = this.Entity.IndicadorBemProprio.Id.Value;

            //this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            //TB_SIGLA tbSigla
            //    = this.Db.TB_SIGLAs.Where(a => a.TB_SIGLA_ID == this.Entity.Id.Value).FirstOrDefault();
            //this.Db.TB_SIGLAs.DeleteOnSubmit(tbSigla);
            //this.Db.SubmitChanges();
        }

        public IList<SiglaEntity> Listar()
        {
            //IList<SiglaEntity> resultado = (from a in this.Db.VW_SIGLAs
            //                                orderby a.TB_SIGLA_ID
            //                                select new SiglaEntity
            //                                {
            //                                    Id = a.TB_SIGLA_ID,
            //                                    Codigo = a.TB_SIGLA_CODIGO,
            //                                    Descricao = a.TB_SIGLA_DESCRICAO,
            //                                    Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                    Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                    IndicadorBemProprio = (new IndicadorBemProprioEntity(a.TB_INDICADOR_BEM_PROPRIO_ID)),
            //                                    Ordem = a.ORDEM
            //                                }).Skip(this.SkipRegistros)
            //                                  .Take(this.RegistrosPagina)
            //                                  .ToList<SiglaEntity>();

            //this.totalregistros = (from a in this.Db.VW_SIGLAs
            //                       select new
            //                       {
            //                           Id = a.TB_SIGLA_ID,
            //                       }).Count();

            return new List<SiglaEntity>();
        }
                
        public IList<SiglaEntity> Listar(int OrgaoId, int GestorId)
        {
            //IList<SiglaEntity> resultado = (from a in this.Db.VW_SIGLAs
            //                                where (a.TB_ORGAO_ID == OrgaoId)
            //                                where (a.TB_GESTOR_ID == GestorId)
            //                                orderby a.TB_SIGLA_ID
            //                                select new SiglaEntity
            //                                {
            //                                    Id = a.TB_SIGLA_ID,
            //                                    Codigo = a.TB_SIGLA_CODIGO,
            //                                    Descricao = a.TB_SIGLA_DESCRICAO,
            //                                    Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                    Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                    IndicadorBemProprio = (new IndicadorBemProprioEntity(a.TB_INDICADOR_BEM_PROPRIO_ID)),
            //                                    Ordem = a.ORDEM
            //                                }).Skip(this.SkipRegistros)
            //                                 .Take(this.RegistrosPagina)
            //                                 .ToList<SiglaEntity>();

            //this.totalregistros = (from a in this.Db.VW_SIGLAs
            //                       where (a.TB_ORGAO_ID == OrgaoId)
            //                       where (a.TB_GESTOR_ID == GestorId)
            //                       select new
            //                       {
            //                           Id = a.TB_SIGLA_ID,
            //                       }).Count();

            return new List<SiglaEntity>();

        }

        public IList<SiglaEntity> Imprimir()
        {
           //IList<SiglaEntity> resultado = (from a in this.Db.VW_SIGLAs
           //                                 orderby a.TB_SIGLA_ID
           //                                 select new SiglaEntity
           //                                 {
           //                                     Id = a.TB_SIGLA_ID,
           //                                     Codigo = a.TB_SIGLA_CODIGO,
           //                                     Descricao = a.TB_SIGLA_DESCRICAO,
           //                                     Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
           //                                     Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
           //                                     IndicadorBemProprio = (new IndicadorBemProprioEntity(a.TB_INDICADOR_BEM_PROPRIO_ID)),
           //                                     Ordem = a.ORDEM
           //                                 }).ToList<SiglaEntity>();

            return new List<SiglaEntity>();
        }

        public IList<SiglaEntity> Imprimir(int OrgaoId, int GestorId)
        {
            //IList<SiglaEntity> resultado = (from a in this.Db.VW_SIGLAs
            //                                where (a.TB_ORGAO_ID == OrgaoId)
            //                                where (a.TB_GESTOR_ID == GestorId)
            //                                orderby a.TB_SIGLA_ID
            //                                select new SiglaEntity
            //                                {
            //                                    Id = a.TB_SIGLA_ID,
            //                                    Codigo = a.TB_SIGLA_CODIGO,
            //                                    Descricao = a.TB_SIGLA_DESCRICAO,
            //                                    Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                    Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                    IndicadorBemProprio = (new IndicadorBemProprioEntity(a.TB_INDICADOR_BEM_PROPRIO_ID)),
            //                                    Ordem = a.ORDEM
            //                                }).ToList<SiglaEntity>();

            //return resultado;
            return new List<SiglaEntity>();
        }
        

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_SIGLA tbSigla = this.Db.TB_SIGLAs.Where(a => a.TB_SIGLA_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbSigla. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                //retorno = this.Db.TB_SIGLAs
                //.Where(a => a.TB_SIGLA_CODIGO == this.Entity.Codigo)
                //.Where(a => a.TB_SIGLA_ID != this.Entity.Id.Value)
                //.Where(a => a.TB_ORGAO_ID == this.Entity.Orgao.Id)
                //.Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                //.Count() > 0;
            }
            else
            {
                //retorno = this.Db.TB_SIGLAs
                //.Where(a => a.TB_SIGLA_CODIGO == this.Entity.Codigo)
                //.Where(a => a.TB_ORGAO_ID == this.Entity.Orgao.Id)
                //.Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                //.Count() > 0;
            }
            return retorno;
        }




        public SiglaEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<SiglaEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
