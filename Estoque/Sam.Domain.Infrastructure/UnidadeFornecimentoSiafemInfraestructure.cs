using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;


namespace Sam.Domain.Infrastructure
{
    public class UnidadeFornecimentoSiafemInfraestructure : BaseInfraestructure, IUnidadeFornecimentoSiafemService
    {
        private UnidadeFornecimentoSiafemEntity unidadeFornecimento = new UnidadeFornecimentoSiafemEntity();

        public UnidadeFornecimentoSiafemEntity Entity
        {
            get { return unidadeFornecimento; }
            set { unidadeFornecimento = value; }
        }
        public void Salvar()
        {
            TB_UNIDADE_FORNECIMENTO_SIAF objUnidadeFornecimentoSiaf = this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs.Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO == this.Entity.Id.Value).FirstOrDefault();
            if (objUnidadeFornecimentoSiaf == null)
            {
                objUnidadeFornecimentoSiaf = new Infrastructure.TB_UNIDADE_FORNECIMENTO_SIAF();

                Db.TB_UNIDADE_FORNECIMENTO_SIAFs.InsertOnSubmit(objUnidadeFornecimentoSiaf);
            }
            objUnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_CODIGO = int.Parse(this.Entity.Codigo);
            objUnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_DESCRICAO = this.Entity.Descricao;
            objUnidadeFornecimentoSiaf.TB_UNIDADE_FORNECIMENTO_QTDE = 1;
            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_UNIDADE_FORNECIMENTO_SIAF tbUnidade
                = this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs.Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs.DeleteOnSubmit(tbUnidade);
            this.Db.SubmitChanges();
        }

        public IList<UnidadeFornecimentoSiafemEntity> Listar()
        {
            IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs 
                                                                orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                select new UnidadeFornecimentoSiafemEntity
                                                                {
                                                                    Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                    Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
                                                                    Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<UnidadeFornecimentoSiafemEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                   }).Count();

            return resultado;
        }

        //public IList<UnidadeFornecimentoSiafemEntity> Listar(int? OrgaoId)
        //{
        //    IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                                                  orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
        //                                                  select new UnidadeFornecimentoSiafemEntity
        //                                                  {
        //                                                      Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                                      Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
        //                                                      Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
        //                                                      //Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
        //                                                  }).Skip(this.SkipRegistros)
        //                                     .Take(this.RegistrosPagina)
        //                                     .ToList<UnidadeFornecimentoSiafemEntity>();

        //    this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                           select new
        //                           {
        //                               Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                           }).Count();

        //    return resultado;
        //}

        //public IList<UnidadeFornecimentoSiafemEntity> Listar(int? OrgaoId, bool blnNoSkipResultSet)
        //{
        //    IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                                                  //      join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
        //                                                  //where (g.TB_ORGAO_ID == OrgaoId)
        //                                                  orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
        //                                                  select new UnidadeFornecimentoSiafemEntity
        //                                                  {
        //                                                      Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                                      Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
        //                                                      Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
        //                                                      ///Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
        //                                                  }).ToList<UnidadeFornecimentoSiafemEntity>();

        //    this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                           ///join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
        //                           ///where (g.TB_ORGAO_ID == OrgaoId)
        //                           select new
        //                           {
        //                               Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                           }).Count();

        //    return resultado;
        //}

        //public IList<UnidadeFornecimentoSiafemEntity> Listar(int OrgaoId, int GestorId)
        //{
        //    IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                                                  //where (a.TB_GESTOR_ID == GestorId)
        //                                                  orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
        //                                                  select new UnidadeFornecimentoSiafemEntity
        //                                                  {
        //                                                      Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                                      Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
        //                                                      Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
        //                                                      ///Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
        //                                                  }).Skip(this.SkipRegistros)
        //                                     .Take(this.RegistrosPagina)
        //                                     .ToList<UnidadeFornecimentoSiafemEntity>();

        //    this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                           ////where (a.TB_GESTOR_ID == GestorId)
        //                           select new
        //                           {
        //                               Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                           }).Count();

        //    return resultado;
        //}

        public IList<UnidadeFornecimentoSiafemEntity> Imprimir()
        {
            IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                                                                orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoSiafemEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              ///Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                          }).ToList<UnidadeFornecimentoSiafemEntity>();

            return resultado;
        }

        //public IList<UnidadeFornecimentoSiafemEntity> Imprimir(int OrgaoId, int GestorId)
        //{
        //    IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                                                        //join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
        //                                                 /// where (g.TB_ORGAO_ID == OrgaoId)
        //                                                  ///where (a.TB_GESTOR_ID == GestorId)
        //                                                  orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
        //                                                  select new UnidadeFornecimentoSiafemEntity
        //                                                  {
        //                                                      Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                                      Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
        //                                                      Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
        //                                                     /// Gestor = (new GestorEntity(a.TB_GESTOR_ID))
        //                                                  }).ToList<UnidadeFornecimentoSiafemEntity>();

        //    return resultado;
        //}

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_UNIDADE_FORNECIMENTO_SIAF tbUnidade = this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs.Where(a => a.TB_UNIDADE_FORNECIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbUnidade. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (!this.Entity.Id.HasValue)
            {
                  retorno = this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                .Where(a => Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO) == this.Entity.Codigo)
                .Count() > 0;
            }
            this.Entity.Id = int.Parse(this.Entity.Codigo);
            return retorno;
        }



        public UnidadeFornecimentoSiafemEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<UnidadeFornecimentoSiafemEntity> ListarTodosCod(int OrgaoId, int GestorId)
        {
            IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                                                               /// where (a.TB_GESTOR_ID == GestorId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoSiafemEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
                                                              Descricao = string.Format("{0} - {1}", a.TB_UNIDADE_FORNECIMENTO_CODIGO.ToString(), a.TB_UNIDADE_FORNECIMENTO_DESCRICAO),
                                                          }) .ToList<UnidadeFornecimentoSiafemEntity>();

            return resultado;
        }


        public IList<UnidadeFornecimentoSiafemEntity> ListarTodosCod()
        {
            IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
                                                                    //where (a.TB_GESTOR_ID == gestorId)
                                                                orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                                select new UnidadeFornecimentoSiafemEntity
                                                                {
                                                                    Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                    Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
                                                                    Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                }).ToList<UnidadeFornecimentoSiafemEntity>();

            return resultado;
        }


        //public IList<UnidadeFornecimentoSiafemEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId)
        //{
        //    // procurar o gestor da UGE
        //    UGEInfraestructure ugeInfra = new UGEInfraestructure();
        //    int gestorId = (from a in this.Db.TB_UGEs
        //                    join g in this.Db.TB_GESTORs on a.TB_UGE_ID equals g.TB_UGE_ID
        //                    where a.TB_UGE_ID == _ugeId
        //                    select g.TB_GESTOR_ID).FirstOrDefault();

        //    IList<UnidadeFornecimentoSiafemEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTO_SIAFs
        //                                                        //where (a.TB_GESTOR_ID == gestorId)
        //                                                  orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
        //                                                  select new UnidadeFornecimentoSiafemEntity
        //                                                  {
        //                                                      Id = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                                      Codigo = Convert.ToString(a.TB_UNIDADE_FORNECIMENTO_CODIGO),
        //                                                      Descricao = string.Format("{0} - {1}", a.TB_UNIDADE_FORNECIMENTO_CODIGO.ToString().PadLeft(12, '0'), a.TB_UNIDADE_FORNECIMENTO_DESCRICAO),
        //                                                  }).ToList<UnidadeFornecimentoSiafemEntity>();

        //    return resultado;
        //}
    }
}
