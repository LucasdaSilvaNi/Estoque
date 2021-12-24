using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
   public class TipoIncorpInfraestructure : BaseInfraestructure, ITipoIncorpService
    {

       private TipoIncorpEntity tipoIncorp;

        #region ICrudBaseService<TiposIncorpEntity> Members

        public TipoIncorpEntity Entity
        {
            get
            {
                return tipoIncorp;
            }
            set
            {
                tipoIncorp = value;
            }
        }

        public IList<TipoIncorpEntity> Listar()
        {
            //IList<TipoIncorpEntity> resultado = (from a in this.Db.TB_TIPO_INCORPs
            //                                      orderby a.TB_TIPO_INCORP_ID
            //                                      select new TipoIncorpEntity
            //                                      {
            //                                          Id = a.TB_TIPO_INCORP_ID,
            //                                          Codigo = a.TB_TIPO_INCORP_CODIGO,
            //                                          Descricao = a.TB_TIPO_INCORP_DESCRICAO,
            //                                          CodigoTransacao = a.TB_TIPO_INCORP_CODIGO_TRANSACAO,
            //                                      }).Skip(this.SkipRegistros)
            //                                      .Take(this.RegistrosPagina)
            //                                      .ToList<TipoIncorpEntity>();

            //this.totalregistros = (from a in this.Db.VW_TIPO_INCORPs
            //                       select new
            //                       {
            //                           id = a.TB_TIPO_INCORP_ID
            //                       }).Count();

            //return resultado;

            return new List<TipoIncorpEntity>();
        }

        public IList<TipoIncorpEntity> Imprimir()
        {
            //IList<TipoIncorpEntity> resultado = (from a in this.Db.TB_TIPO_INCORPs
            //                                     orderby a.TB_TIPO_INCORP_ID
            //                                     select new TipoIncorpEntity
            //                                     {
            //                                         Id = a.TB_TIPO_INCORP_ID,
            //                                         Codigo = a.TB_TIPO_INCORP_CODIGO,
            //                                         Descricao = a.TB_TIPO_INCORP_DESCRICAO,
            //                                         CodigoTransacao = a.TB_TIPO_INCORP_CODIGO_TRANSACAO,
            //                                     })
            //                                      .ToList<TipoIncorpEntity>();

        

            //return resultado;
            return new List<TipoIncorpEntity>();
        }

        public void Excluir()
        {
            //TB_TIPO_INCORP tipoIncorp = (this.Db.TB_TIPO_INCORPs.Where(a => a.TB_TIPO_INCORP_ID == this.Entity.Id.Value).FirstOrDefault());
            //this.Db.TB_TIPO_INCORPs.DeleteOnSubmit(tipoIncorp);
            //this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            //TB_TIPO_INCORP tipoIncorp = new TB_TIPO_INCORP();

            //if (this.Entity.Id.HasValue)
            //    tipoIncorp = this.Db.TB_TIPO_INCORPs.Where(a => a.TB_TIPO_INCORP_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    this.Db.TB_TIPO_INCORPs.InsertOnSubmit(tipoIncorp);

            //tipoIncorp.TB_TIPO_INCORP_CODIGO = this.Entity.Codigo.Value;
            //tipoIncorp.TB_TIPO_INCORP_DESCRICAO = this.Entity.Descricao;
            //tipoIncorp.TB_TIPO_INCORP_CODIGO_TRANSACAO = this.Entity.CodigoTransacao;

            //this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            bool retorno = true;
            //TB_TIPO_INCORP tipoIncorp = this.Db.TB_TIPO_INCORPs.Where(a => a.TB_TIPO_INCORP_ID == this.Entity.Id.Value).FirstOrDefault();
            //if (tipoIncorp.
            // return false

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                //retorno = this.Db.TB_TIPO_INCORPs
                //    .Where(a => a.TB_TIPO_INCORP_ID == this.Entity.Id.Value)
                //    .Where(a => a.TB_TIPO_INCORP_CODIGO != this.Entity.Codigo.Value)
                //    .Count() > 0;
            }
            else
            {
                //retorno = this.Db.TB_TIPO_INCORPs
                //    .Where(a => a.TB_TIPO_INCORP_CODIGO == this.Entity.Codigo)
                //    .Count() > 0;
            }

            return retorno;
        }

        #endregion


        TipoIncorpEntity ICrudBaseService<TipoIncorpEntity>.LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<TipoIncorpEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
