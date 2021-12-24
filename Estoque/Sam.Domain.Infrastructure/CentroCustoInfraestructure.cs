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
    public class CentroCustoInfraestructure : BaseInfraestructure,ICentroCustoService
    {
        private CentroCustoEntity centroCusto = new CentroCustoEntity();
        
        public CentroCustoEntity Entity
        {
            get { return centroCusto; }
            set { centroCusto = value; }
        }

        public void Salvar()
        {
            TB_CENTRO_CUSTO tbCentroCusto = new TB_CENTRO_CUSTO();

            if (this.Entity.Id.HasValue)
                tbCentroCusto = this.Db.TB_CENTRO_CUSTOs.Where(a => a.TB_CENTRO_CUSTO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                Db.TB_CENTRO_CUSTOs.InsertOnSubmit(tbCentroCusto);

            tbCentroCusto.TB_CENTRO_CUSTO_CODIGO = this.Entity.Codigo;
            tbCentroCusto.TB_CENTRO_CUSTO_DESCRICAO = this.Entity.Descricao;
            tbCentroCusto.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;

            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_CENTRO_CUSTO tbCentroCusto
                = this.Db.TB_CENTRO_CUSTOs.Where(a => a.TB_CENTRO_CUSTO_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_CENTRO_CUSTOs.DeleteOnSubmit(tbCentroCusto);
            this.Db.SubmitChanges();
        }

        public IList<CentroCustoEntity> Listar()
        {
            IList<CentroCustoEntity> resultado = (from a in this.Db.TB_CENTRO_CUSTOs
                                                  orderby a.TB_CENTRO_CUSTO_CODIGO
                                                  select new CentroCustoEntity
                                                  {
                                                      Id = a.TB_CENTRO_CUSTO_ID,
                                                      Codigo = a.TB_CENTRO_CUSTO_CODIGO,
                                                      Descricao = a.TB_CENTRO_CUSTO_DESCRICAO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<CentroCustoEntity>();

            this.totalregistros = (from a in this.Db.TB_CENTRO_CUSTOs
                                   orderby a.TB_CENTRO_CUSTO_CODIGO
                                   select new
                                   {
                                       Id = a.TB_CENTRO_CUSTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<CentroCustoEntity> Listar(int GestorId)
        {
            IList<CentroCustoEntity> resultado = (from a in this.Db.TB_CENTRO_CUSTOs
                                                  where (a.TB_GESTOR_ID == GestorId)
                                                  orderby a.TB_CENTRO_CUSTO_CODIGO
                                                  select new CentroCustoEntity
                                                  {
                                                      Id = a.TB_CENTRO_CUSTO_ID,
                                                      Codigo = a.TB_CENTRO_CUSTO_CODIGO,
                                                      Descricao = a.TB_CENTRO_CUSTO_DESCRICAO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<CentroCustoEntity>();

            this.totalregistros = (from a in this.Db.TB_CENTRO_CUSTOs
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       Id = a.TB_CENTRO_CUSTO_ID
                                   }).Count();

            return resultado;
        }

        public IList<CentroCustoEntity> Imprimir(int OrgaoId, int GestorId)
        {
            IList<CentroCustoEntity> resultado = (from a in this.Db.TB_CENTRO_CUSTOs
                                                  where (a.TB_GESTOR_ID == GestorId)
                                                  orderby a.TB_CENTRO_CUSTO_ID
                                                  select new CentroCustoEntity
                                                  {
                                                      Id = a.TB_CENTRO_CUSTO_ID,
                                                      Codigo = a.TB_CENTRO_CUSTO_CODIGO,
                                                      Descricao = a.TB_CENTRO_CUSTO_DESCRICAO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  })
                                             .ToList<CentroCustoEntity>();

            return resultado;

        }

        public IList<CentroCustoEntity> Imprimir()
        {
            //IList<CentroCustoEntity> resultado = (from a in this.Db.VW_CENTRO_CUSTOs
                                                 
            //                                      orderby a.TB_CENTRO_CUSTO_ID
            //                                      select new CentroCustoEntity
            //                                      {
            //                                          Id = a.TB_CENTRO_CUSTO_ID,
            //                                          Codigo = a.TB_CENTRO_CUSTO_CODIGO,
            //                                          Descricao = a.TB_CENTRO_CUSTO_DESCRICAO,
            //                                          Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
            //                                          Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                          Ordem = a.ORDEM
            //                                      })
            //                                 .ToList<CentroCustoEntity>();

        

            //return resultado;
            return new List<CentroCustoEntity>();

        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            TB_CENTRO_CUSTO tbCentroCusto = this.Db.TB_CENTRO_CUSTOs.Where(a => a.TB_CENTRO_CUSTO_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbCentroCusto. > 0)
              //  return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_CENTRO_CUSTOs
                .Where(a => a.TB_CENTRO_CUSTO_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                .Where(a => a.TB_CENTRO_CUSTO_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_CENTRO_CUSTOs
                .Where(a => a.TB_CENTRO_CUSTO_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                .Count() > 0;
            }
            return retorno;
        }


        public CentroCustoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<CentroCustoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public IList<CentroCustoEntity> ListarTodosCodPorOrgao(int OrgaoId)
        {
            IList<CentroCustoEntity> resultado = (from a in this.Db.TB_CENTRO_CUSTOs
                                                  join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                  join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                                  where (org.TB_ORGAO_ID == OrgaoId)
                                                  orderby a.TB_CENTRO_CUSTO_CODIGO
                                                  select new CentroCustoEntity
                                                  {
                                                      Id = a.TB_CENTRO_CUSTO_ID,
                                                      Codigo = a.TB_CENTRO_CUSTO_CODIGO,
                                                      Descricao = string.Format("{0} - {1}", a.TB_CENTRO_CUSTO_CODIGO, a.TB_CENTRO_CUSTO_DESCRICAO),
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<CentroCustoEntity>();

            this.totalregistros = (from a in this.Db.TB_CENTRO_CUSTOs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (org.TB_ORGAO_ID == OrgaoId)
                                   orderby a.TB_CENTRO_CUSTO_CODIGO
                                   select new
                                   {
                                       Id = a.TB_CENTRO_CUSTO_ID
                                   }).Count();

            return resultado;
        }


        public IList<CentroCustoEntity> Listar(int OrgaoId, int GestorId)
        {
            throw new NotImplementedException();
        }
    }
}
