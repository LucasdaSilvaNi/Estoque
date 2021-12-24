using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
  public partial class FontesRecursoInfraestructure: BaseInfraestructure, IFontesRecursoService
    {
        #region ICrudBaseService<FontesRecursoEntity> Members

        private FontesRecursoEntity fontesRecursos = new FontesRecursoEntity();
        public FontesRecursoEntity Entity
        {
            get { return fontesRecursos; }
            set { fontesRecursos = value; }
        }

        public IList<FontesRecursoEntity> Listar()
        {
            //IList<FontesRecursoEntity> resultado = (from a in this.Db.TB_FONTES_RECURSOs
            //                                        orderby a.TB_FONTES_RECURSO_ID
            //                                        select new FontesRecursoEntity
            //                                        {
            //                                            Id = a.TB_FONTES_RECURSO_ID,
            //                                            Codigo = a.TB_FONTES_RECURSO_CODIGO,
            //                                            Descricao = a.TB_FONTES_RECURSO_DESCRICAO

            //                                        }).Skip(this.SkipRegistros)
            //                                          .Take(this.RegistrosPagina)
            //                                          .ToList<FontesRecursoEntity>();

            //this.totalregistros = (from a in this.Db.VW_FONTES_RECURSOs
            //                       select new 
            //                       {
            //                           Id = a.TB_FONTES_RECURSO_ID
            //                       }).Count();

            //return resultado;

            List<FontesRecursoEntity> fontesRecurso = new List<FontesRecursoEntity>();
            return fontesRecurso;
            
        }

        public IList<FontesRecursoEntity> Imprimir()
        {
            //IList<FontesRecursoEntity> resultado = (from a in this.Db.TB_FONTES_RECURSOs
            //                                        orderby a.TB_FONTES_RECURSO_ID
            //                                        select new FontesRecursoEntity
            //                                        {
            //                                            Id = a.TB_FONTES_RECURSO_ID,
            //                                            Codigo = a.TB_FONTES_RECURSO_CODIGO,
            //                                            Descricao = a.TB_FONTES_RECURSO_DESCRICAO

            //                                        })
            //                                          .ToList<FontesRecursoEntity>();


            List<FontesRecursoEntity> fontesRecurso = new List<FontesRecursoEntity>();
            return fontesRecurso;
            //return resultado;

        }

        public void Excluir()
        {
            //TB_FONTES_RECURSO tbFontesRecurso = new TB_FONTES_RECURSO();
            //tbFontesRecurso = this.Db.TB_FONTES_RECURSOs.Where(a => a.TB_FONTES_RECURSO_ID == this.Entity.Id).FirstOrDefault();
            //this.Db.TB_FONTES_RECURSOs.DeleteOnSubmit(tbFontesRecurso);
            //this.Db.SubmitChanges();
        }

        public void Salvar()
        {
          //TB_FONTES_RECURSO tbFontesRecurso = new TB_FONTES_RECURSO();

          //  if (this.Entity.Id.HasValue)
          //      tbFontesRecurso = this.Db.TB_FONTES_RECURSOs.Where(a => a.TB_FONTES_RECURSO_ID == this.Entity.Id.Value).FirstOrDefault();
          //  else
          //      this.Db.TB_FONTES_RECURSOs.InsertOnSubmit(tbFontesRecurso);

          //  tbFontesRecurso.TB_FONTES_RECURSO_CODIGO = this.Entity.Codigo.Value;
          //  tbFontesRecurso.TB_FONTES_RECURSO_DESCRICAO = this.Entity.Descricao;

          //  this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            return true;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;

            //if (this.Entity.Id.HasValue)
            //{
            //    retorno = this.Db.TB_FONTES_RECURSOs
            //        .Where(a => a.TB_FONTES_RECURSO_ID == this.Entity.Id)
            //        .Where(a => a.TB_FONTES_RECURSO_CODIGO == this.Entity.Codigo)
            //        .Count() > 0;
            //}
            //else
            //{
            //    retorno = this.Db.TB_FONTES_RECURSOs
            //        .Where(a => a.TB_FONTES_RECURSO_CODIGO == this.Entity.Codigo)
            //        .Count() > 0;
            //}

            return retorno;
        }

        #endregion



        public FontesRecursoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<FontesRecursoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
