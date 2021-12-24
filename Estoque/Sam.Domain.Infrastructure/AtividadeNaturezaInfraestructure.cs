using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public class AtividadeNaturezaInfraestructure : BaseInfraestructure, IAtividadeNaturezaService
    {
        

        public int TotalRegistros()
        {
            return totalregistros;
        }


        public AtividadeNaturezaDespesaEntity Entity { get; set; }

        public IList<AtividadeNaturezaDespesaEntity> Listar()
        {
           IList<AtividadeNaturezaDespesaEntity> resultado = (from a in this.Db.TB_NATUREZA_DESPESAs
                                                               orderby a.TB_NATUREZA_DESPESA_CODIGO
                                                               select new AtividadeNaturezaDespesaEntity
                                                               {
                                                                   Id = a.TB_NATUREZA_DESPESA_ID,
                                                                   Descricao = a.TB_NATUREZA_DESPESA_DESCRICAO,                                                                   
                                                               }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<AtividadeNaturezaDespesaEntity>();

            this.totalregistros = (from a in this.Db.TB_NATUREZA_DESPESAs
                                   select new
                                   {
                                       Id = a.TB_NATUREZA_DESPESA_ID,
                                   }).Count();

            return resultado;

        }

        public IList<AtividadeNaturezaDespesaEntity> Imprimir()
        {
            //IList<AtividadeNaturezaDespesaEntity> resultado = (from a in this.Db.TB_ATIVIDADE_NATUREZA_DESPESAs
            //                                                   orderby a.TB_ATIVIDADE_NATUREZA_DESPESA_ID
            //                                                   select new AtividadeNaturezaDespesaEntity
            //                                                   {
            //                                                       Id = a.TB_ATIVIDADE_NATUREZA_DESPESA_ID,
            //                                                       Descricao = a.TB_ATIVIDADE_NATUREZA_DESPESA_DESCRICAO,
            //                                                   })
            //                              .ToList<AtividadeNaturezaDespesaEntity>();


            var resultado = new List<AtividadeNaturezaDespesaEntity>();
            return resultado;

        }


        public void Excluir()
        {
            //TB_ATIVIDADE_NATUREZA_DESPESA natureza
            //       = this.Db.TB_ATIVIDADE_NATUREZA_DESPESAs.Where(a => a.TB_ATIVIDADE_NATUREZA_DESPESA_ID == this.Entity.Id).FirstOrDefault();
            //this.Db.TB_ATIVIDADE_NATUREZA_DESPESAs.DeleteOnSubmit(natureza);
            //this.Db.SubmitChanges();
        }



        public void Salvar()
        {
            //TB_ATIVIDADE_NATUREZA_DESPESA natureza = new TB_ATIVIDADE_NATUREZA_DESPESA();

            //if (this.Entity.Id.HasValue)
            //    natureza = this.Db.TB_ATIVIDADE_NATUREZA_DESPESAs.Where(a => a.TB_ATIVIDADE_NATUREZA_DESPESA_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    this.Db.TB_ATIVIDADE_NATUREZA_DESPESAs.InsertOnSubmit(natureza);

            //natureza.TB_ATIVIDADE_NATUREZA_DESPESA_DESCRICAO = this.Entity.Descricao;
            //this.Db.SubmitChanges();
        }


        public bool PodeExcluir()
        {

           bool retorno = true;

            //TB_NATUREZA_DESPESA natureza
            //    = this.Db.TB_NATUREZA_DESPESAs.Where(a => a.TB_ATIVIDADE_NATUREZA_DESPESA_ID == this.Entity.Id.Value).FirstOrDefault();
            //if (natureza.TB_ATIVIDADE_NATUREZA_DESPESA.TB_NATUREZA_DESPESAs.Count > 0)
            //    retorno = false;

            return retorno;

        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            return retorno;
        }


        public AtividadeNaturezaDespesaEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<AtividadeNaturezaDespesaEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
