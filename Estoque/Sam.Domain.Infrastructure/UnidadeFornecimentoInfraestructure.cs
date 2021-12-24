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
    public class UnidadeFornecimentoInfraestructure : BaseInfraestructure, IUnidadeFornecimentoService
    {
        private UnidadeFornecimentoEntity unidadeFornecimento = new UnidadeFornecimentoEntity();

        public UnidadeFornecimentoEntity Entity
        {
            get { return unidadeFornecimento; }
            set { unidadeFornecimento = value; }
        }

        public void Salvar()
        {
            TB_UNIDADE_FORNECIMENTO tbUnidade = new TB_UNIDADE_FORNECIMENTO();

            if (this.Entity.Id.HasValue)
                tbUnidade = this.Db.TB_UNIDADE_FORNECIMENTOs.Where(a => a.TB_UNIDADE_FORNECIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                Db.TB_UNIDADE_FORNECIMENTOs.InsertOnSubmit(tbUnidade);

            tbUnidade.TB_UNIDADE_FORNECIMENTO_CODIGO = this.Entity.Codigo;
            tbUnidade.TB_UNIDADE_FORNECIMENTO_DESCRICAO = this.Entity.Descricao;
            tbUnidade.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
            //tbUnidade.TB_ORGAO_ID = this.Entity.Orgao.Id.Value;

            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_UNIDADE_FORNECIMENTO tbUnidade
                = this.Db.TB_UNIDADE_FORNECIMENTOs.Where(a => a.TB_UNIDADE_FORNECIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UNIDADE_FORNECIMENTOs.DeleteOnSubmit(tbUnidade);
            this.Db.SubmitChanges();
        }

        public IList<UnidadeFornecimentoEntity> Listar()
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs.Where(a => a.TB_GESTOR_ID == this.Entity.Id.Value)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<UnidadeFornecimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UnidadeFornecimentoEntity> Listar(int? OrgaoId)
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                          where (g.TB_ORGAO_ID == OrgaoId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          }).Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<UnidadeFornecimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   where (g.TB_ORGAO_ID == OrgaoId)
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UnidadeFornecimentoEntity> Listar(int? OrgaoId, bool blnNoSkipResultSet)
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                          where (g.TB_ORGAO_ID == OrgaoId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          }).ToList<UnidadeFornecimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   where (g.TB_ORGAO_ID == OrgaoId)
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UnidadeFornecimentoEntity> Listar(int OrgaoId, int GestorId)
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          where (a.TB_GESTOR_ID == GestorId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          }).Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<UnidadeFornecimentoEntity>();

            this.totalregistros = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                   }).Count();

            return resultado;
        }

        public IList<UnidadeFornecimentoEntity> Imprimir()
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_ID
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                          }).ToList<UnidadeFornecimentoEntity>();

            return resultado;
        }

        public IList<UnidadeFornecimentoEntity> Imprimir(int OrgaoId, int GestorId)
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                          where (g.TB_ORGAO_ID == OrgaoId)
                                                          where (a.TB_GESTOR_ID == GestorId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = a.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                              Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                          }).ToList<UnidadeFornecimentoEntity>();

            return resultado;
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_UNIDADE_FORNECIMENTO tbUnidade = this.Db.TB_UNIDADE_FORNECIMENTOs.Where(a => a.TB_UNIDADE_FORNECIMENTO_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbUnidade. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_UNIDADE_FORNECIMENTOs
                .Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_UNIDADE_FORNECIMENTO_ID != this.Entity.Id.Value)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_UNIDADE_FORNECIMENTOs
                .Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            return retorno;
        }



        public UnidadeFornecimentoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<UnidadeFornecimentoEntity> ListarTodosCod(int OrgaoId, int GestorId)
        {
            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          where (a.TB_GESTOR_ID == GestorId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_CODIGO
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = string.Format("{0} - {1}", a.TB_UNIDADE_FORNECIMENTO_CODIGO.ToString(), a.TB_UNIDADE_FORNECIMENTO_DESCRICAO),
                                                          }) .ToList<UnidadeFornecimentoEntity>();

            return resultado;
        }


        public IList<UnidadeFornecimentoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }


        public IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId)
        {
            // procurar o gestor da UGE
            UGEInfraestructure ugeInfra = new UGEInfraestructure();
            int gestorId = (from a in this.Db.TB_UGEs
                            join g in this.Db.TB_GESTORs on a.TB_UGE_ID equals g.TB_UGE_ID
                            where a.TB_UGE_ID == _ugeId
                            select g.TB_GESTOR_ID).FirstOrDefault();

            IList<UnidadeFornecimentoEntity> resultado = (from a in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                          where (a.TB_GESTOR_ID == gestorId)
                                                          orderby a.TB_UNIDADE_FORNECIMENTO_ID
                                                          select new UnidadeFornecimentoEntity
                                                          {
                                                              Id = a.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = a.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = string.Format("{0} - {1}", a.TB_UNIDADE_FORNECIMENTO_CODIGO.ToString().PadLeft(12, '0'), a.TB_UNIDADE_FORNECIMENTO_DESCRICAO),
                                                          }).ToList<UnidadeFornecimentoEntity>();

            return resultado;
        }
    }
}
