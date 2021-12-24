using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
namespace Sam.Domain.Infrastructure
{
    public class ContaAuxiliarInfraestructure : BaseInfraestructure, IContaAuxiliarService
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

        public ContaAuxiliarEntity Entity { get; set; }


        public IList<ContaAuxiliarEntity> Listar()
        {
            IList<ContaAuxiliarEntity> resultado = (from a in this.Db.TB_CONTA_AUXILIARs
                                                    orderby a.TB_CONTA_AUXILIAR_CODIGO
                                                    select new ContaAuxiliarEntity
                                                    {
                                                        Id = a.TB_CONTA_AUXILIAR_ID,
                                                        Descricao = a.TB_CONTA_AUXILIAR_DESCRICAO,
                                                        Codigo = a.TB_CONTA_AUXILIAR_CODIGO,
                                                        ContaContabil = (a.TB_CONTA_AUXILIAR_CONTA_CONTABIL == null) ? 0 : (int)a.TB_CONTA_AUXILIAR_CONTA_CONTABIL,
                                                    }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<ContaAuxiliarEntity>();

            this.totalregistros = (from a in this.Db.TB_CONTA_AUXILIARs
                                   select new
                                   {
                                       Id = a.TB_CONTA_AUXILIAR_ID,
                                   }).Count();
            return resultado;
        }

        public IList<ContaAuxiliarEntity> Imprimir()
        {
            IList<ContaAuxiliarEntity> resultado = (from a in this.Db.TB_CONTA_AUXILIARs
                                                    orderby a.TB_CONTA_AUXILIAR_CODIGO
                                                    select new ContaAuxiliarEntity
                                                    {
                                                        Id = a.TB_CONTA_AUXILIAR_ID,
                                                        Descricao = a.TB_CONTA_AUXILIAR_DESCRICAO,
                                                        Codigo = a.TB_CONTA_AUXILIAR_CODIGO,
                                                        ContaContabil = (a.TB_CONTA_AUXILIAR_CONTA_CONTABIL == null) ? 0 : (int)a.TB_CONTA_AUXILIAR_CONTA_CONTABIL,
                                                    }).ToList<ContaAuxiliarEntity>();


            return resultado;
        }


        public void Excluir()
        {
            TB_CONTA_AUXILIAR conta
                   = this.Db.TB_CONTA_AUXILIARs.Where(a => a.TB_CONTA_AUXILIAR_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_CONTA_AUXILIARs.DeleteOnSubmit(conta);
            this.Db.SubmitChanges();
        }


       

        public void Salvar()
        {
            TB_CONTA_AUXILIAR conta = new TB_CONTA_AUXILIAR();

            if (this.Entity.Id.HasValue)
                conta = this.Db.TB_CONTA_AUXILIARs.Where(a => a.TB_CONTA_AUXILIAR_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_CONTA_AUXILIARs.InsertOnSubmit(conta);

            conta.TB_CONTA_AUXILIAR_CODIGO = this.Entity.Codigo;
            conta.TB_CONTA_AUXILIAR_DESCRICAO = this.Entity.Descricao;
            conta.TB_CONTA_AUXILIAR_CONTA_CONTABIL = this.Entity.ContaContabil;
            this.Db.SubmitChanges();
        }


        public bool PodeExcluir()
        {

            bool retorno = true;

            //TB_CONTAS_AUXILIARES uo = this.Db.TB_CONTAS_AUXILIARESs.Where(a => a. == this.UO.Id.Value).FirstOrDefault();
            // XUXA - implementar validação para verificar possibilidade de exclusão da grupo material
            // provavelmente ligacoes com as tabelas: classe, material, item material

            //if (UO.Count > 0)
            //    retorno = false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_CONTA_AUXILIARs
                .Where(a => a.TB_CONTA_AUXILIAR_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_CONTA_AUXILIAR_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_CONTA_AUXILIARs
                .Where(a => a.TB_CONTA_AUXILIAR_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }


        ContaAuxiliarEntity ICrudBaseService<ContaAuxiliarEntity>.LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<ContaAuxiliarEntity> ListarTodosCod()
        {
            IList<ContaAuxiliarEntity> resultado = (from a in this.Db.TB_CONTA_AUXILIARs
                                                    orderby a.TB_CONTA_AUXILIAR_CODIGO
                                                    select new ContaAuxiliarEntity
                                                    {
                                                        Id = a.TB_CONTA_AUXILIAR_ID,                                                        
                                                        Codigo = a.TB_CONTA_AUXILIAR_CODIGO,
                                                        Descricao = string.Format("{0} - {1}", a.TB_CONTA_AUXILIAR_CODIGO.ToString().PadLeft(4, '0'), a.TB_CONTA_AUXILIAR_DESCRICAO),
                                                    }).ToList<ContaAuxiliarEntity>();
            return resultado;
        }
    }
}
