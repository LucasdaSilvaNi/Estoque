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
    public class MotivoBaixaInfraestructure : BaseInfraestructure, IMotivoBaixaService
    {
        private MotivoBaixaEntity motivoBaixa = new MotivoBaixaEntity();

        public MotivoBaixaEntity Entity
        {
            get { return motivoBaixa; }
            set { motivoBaixa = value; }
        }

        public void Salvar()
        {

            //TB_MOTIVO_BAIXA tbMotivoBaixa = new TB_MOTIVO_BAIXA();

            //if (this.Entity.Id.HasValue)
            //    tbMotivoBaixa = this.Db.TB_MOTIVO_BAIXAs.Where(a => a.TB_MOTIVO_BAIXA_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    Db.TB_MOTIVO_BAIXAs.InsertOnSubmit(tbMotivoBaixa);

            //tbMotivoBaixa.TB_MOTIVO_BAIXA_CODIGO = this.Entity.Codigo.Value;
            //tbMotivoBaixa.TB_MOTIVO_BAIXA_DESCRICAO = this.Entity.Descricao;
            //tbMotivoBaixa.TB_MOTIVO_BAIXA_CODIGO_TRANSACAO = this.Entity.CodigoTransacao;

            //this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            //TB_MOTIVO_BAIXA tbMotivoBaixa
            //    = this.Db.TB_MOTIVO_BAIXAs.Where(a => a.TB_MOTIVO_BAIXA_ID == this.Entity.Id.Value).FirstOrDefault();
            //this.Db.TB_MOTIVO_BAIXAs.DeleteOnSubmit(tbMotivoBaixa);
            //this.Db.SubmitChanges();
        }

        public IList<MotivoBaixaEntity> Imprimir()
        {
            //IList<MotivoBaixaEntity> resultado = (from a in this.Db.VW_MOTIVO_BAIXAs
            //                                      orderby a.TB_MOTIVO_BAIXA_ID
            //                                      select new MotivoBaixaEntity
            //                                      {
            //                                          Id = a.TB_MOTIVO_BAIXA_ID,
            //                                          Codigo = a.TB_MOTIVO_BAIXA_CODIGO,
            //                                          Descricao = a.TB_MOTIVO_BAIXA_DESCRICAO,
            //                                          CodigoTransacao = a.TB_MOTIVO_BAIXA_CODIGO_TRANSACAO,
            //                                          Ordem = a.ORDEM
            //                                      }).ToList<MotivoBaixaEntity>();

            //return resultado;
            return new List<MotivoBaixaEntity>();
        }

        public IList<MotivoBaixaEntity> Listar()
        {
            //IList<MotivoBaixaEntity> resultado = (from a in this.Db.VW_MOTIVO_BAIXAs
            //                                      orderby a.TB_MOTIVO_BAIXA_ID
            //                                      select new MotivoBaixaEntity
            //                                      {
            //                                          Id = a.TB_MOTIVO_BAIXA_ID,
            //                                          Codigo = a.TB_MOTIVO_BAIXA_CODIGO,
            //                                          Descricao = a.TB_MOTIVO_BAIXA_DESCRICAO,
            //                                          CodigoTransacao = a.TB_MOTIVO_BAIXA_CODIGO_TRANSACAO,
            //                                          Ordem = a.ORDEM
            //                                      }).Skip(this.SkipRegistros)
            //                                  .Take(this.RegistrosPagina)
            //                                  .ToList<MotivoBaixaEntity>();

            //this.totalregistros = (from a in this.Db.VW_MOTIVO_BAIXAs
            //                       select new
            //                       {
            //                           Id = a.TB_MOTIVO_BAIXA_ID,
            //                       }).Count();

            //return resultado;
            return new List<MotivoBaixaEntity>();

        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_MOTIVO_BAIXA tbMotivoBaixa = this.Db.TB_MotivoBaixa.Where(a => a.TB_MotivoBaixa_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbMotivoBaixa. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                //retorno = this.Db.TB_MOTIVO_BAIXAs
                //.Where(a => a.TB_MOTIVO_BAIXA_CODIGO == this.Entity.Codigo)
                //.Where(a => a.TB_MOTIVO_BAIXA_ID != this.Entity.Id.Value)
                //.Count() > 0;
            }
            else
            {
                //retorno = this.Db.TB_MOTIVO_BAIXAs
                //.Where(a => a.TB_MOTIVO_BAIXA_CODIGO == this.Entity.Codigo)
                //.Count() > 0;
            }
            return retorno;
        }



        public MotivoBaixaEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<MotivoBaixaEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
