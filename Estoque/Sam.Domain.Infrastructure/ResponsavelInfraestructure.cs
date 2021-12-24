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
    public class ResponsavelInfraestructure : BaseInfraestructure,IResponsavelService
    {
        private ResponsavelEntity responsavel = new ResponsavelEntity();

        public ResponsavelEntity Entity
        {
            get { return responsavel; }
            set { responsavel = value; }
        }

        public void Salvar()
        {
            TB_RESPONSAVEL tbResponsavel = new TB_RESPONSAVEL(); ;

            if (this.Entity.Id.HasValue)
                tbResponsavel = this.Db.TB_RESPONSAVELs.Where(a => a.TB_RESPONSAVEL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                Db.TB_RESPONSAVELs.InsertOnSubmit(tbResponsavel);

            tbResponsavel.TB_RESPONSAVEL_CODIGO = this.Entity.Codigo.Value;
            tbResponsavel.TB_RESPONSAVEL_NOME = this.Entity.Descricao;
            tbResponsavel.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
            tbResponsavel.TB_RESPONSAVEL_CARGO = this.Entity.Cargo;
            tbResponsavel.TB_RESPONSAVEL_ENDERECO = this.Entity.Endereco;
            
            this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            TB_RESPONSAVEL tbResponsavel
                = this.Db.TB_RESPONSAVELs.Where(a => a.TB_RESPONSAVEL_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_RESPONSAVELs.DeleteOnSubmit(tbResponsavel);
            this.Db.SubmitChanges();
        }

        public IList<ResponsavelEntity> Listar()
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  orderby a.TB_RESPONSAVEL_CODIGO
                                                  select new ResponsavelEntity
                                              {
                                                  Id = a.TB_RESPONSAVEL_ID,
                                                  Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                  Descricao = a.TB_RESPONSAVEL_NOME,
                                                  Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                  Cargo = a.TB_RESPONSAVEL_CARGO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<ResponsavelEntity>();

            this.totalregistros = (from a in this.Db.TB_CENTRO_CUSTOs
                                   select new
                                   {
                                       Id = a.TB_CENTRO_CUSTO_ID,
                                   }).Count();

            return resultado;

        }

        public IList<ResponsavelEntity> Listar(int GestorId)
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  where (a.TB_GESTOR_ID == GestorId)
                                                  orderby a.TB_RESPONSAVEL_CODIGO
                                                  select new ResponsavelEntity
                                              {
                                                  Id = a.TB_RESPONSAVEL_ID,
                                                  Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                  Descricao = a.TB_RESPONSAVEL_NOME,
                                                  Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                  Cargo = a.TB_RESPONSAVEL_CARGO,
                                                  Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                              }).Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<ResponsavelEntity>();

            this.totalregistros = (from a in this.Db.TB_RESPONSAVELs
                                   where (a.TB_GESTOR_ID == GestorId)
                                   select new
                                   {
                                       Id = a.TB_RESPONSAVEL_ID
                                   }).Count();

            return resultado;
        }

        public IList<ResponsavelEntity> ListarTodosCodPorOrgao(int orgaoId) {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                  join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                                  where (org.TB_ORGAO_ID == orgaoId)
                                                  orderby a.TB_RESPONSAVEL_NOME
                                                  select new ResponsavelEntity
                                                  {
                                                      Id = a.TB_RESPONSAVEL_ID,
                                                      Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                      Descricao = a.TB_RESPONSAVEL_NOME,
                                                      Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                      Cargo = a.TB_RESPONSAVEL_CARGO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<ResponsavelEntity>();

            this.totalregistros = (from a in this.Db.TB_RESPONSAVELs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (org.TB_ORGAO_ID == orgaoId)
                                   select new
                                   {
                                       Id = a.TB_RESPONSAVEL_ID
                                   }).Count();

            return resultado;
        }

        public IList<ResponsavelEntity> ListarTodosPorGestor(int gestorId)
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                  join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                                  where (g.TB_GESTOR_ID == gestorId)
                                                  orderby a.TB_RESPONSAVEL_NOME
                                                  select new ResponsavelEntity
                                                  {
                                                      Id = a.TB_RESPONSAVEL_ID,
                                                      Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                      Descricao = a.TB_RESPONSAVEL_NOME,
                                                      Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                      Cargo = a.TB_RESPONSAVEL_CARGO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<ResponsavelEntity>();

            this.totalregistros = (from a in this.Db.TB_RESPONSAVELs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (g.TB_GESTOR_ID == gestorId)
                                   select new
                                   {
                                       Id = a.TB_RESPONSAVEL_ID
                                   }).Count();

            return resultado;
        }

        public IList<ResponsavelEntity> ListarTodosPorOrgaoGestor(int OrgaoId, int gestorId)
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                                  join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                                  where (org.TB_ORGAO_ID == OrgaoId )
                                                  where (g.TB_GESTOR_ID == gestorId)
                                                  orderby a.TB_RESPONSAVEL_NOME
                                                  select new ResponsavelEntity
                                                  {
                                                      Id = a.TB_RESPONSAVEL_ID,
                                                      Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                      Descricao = a.TB_RESPONSAVEL_NOME,
                                                      Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                      Cargo = a.TB_RESPONSAVEL_CARGO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<ResponsavelEntity>();

            this.totalregistros = (from a in this.Db.TB_RESPONSAVELs
                                   join g in this.Db.TB_GESTORs on a.TB_GESTOR_ID equals g.TB_GESTOR_ID
                                   join org in this.Db.TB_ORGAOs on g.TB_ORGAO_ID equals org.TB_ORGAO_ID
                                   where (org.TB_ORGAO_ID == OrgaoId)
                                   where (g.TB_GESTOR_ID == gestorId)
                                   select new
                                   {
                                       Id = a.TB_RESPONSAVEL_ID
                                   }).Count();

            return resultado;
        }

        public IList<ResponsavelEntity> Imprimir()
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  orderby a.TB_RESPONSAVEL_CODIGO
                                                  select new ResponsavelEntity
                                                  {
                                                      Id = a.TB_RESPONSAVEL_ID,
                                                      Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                      Descricao = a.TB_RESPONSAVEL_NOME,
                                                      Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                      Cargo = a.TB_RESPONSAVEL_CARGO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<ResponsavelEntity>();

            return resultado;
        }

        public IList<ResponsavelEntity> Imprimir(int OrgaoId, int GestorId)
        {
            IList<ResponsavelEntity> resultado = (from a in this.Db.TB_RESPONSAVELs
                                                  where (a.TB_GESTOR_ID == GestorId)
                                                  orderby a.TB_RESPONSAVEL_CODIGO
                                                  select new ResponsavelEntity
                                                  {
                                                      Id = a.TB_RESPONSAVEL_ID,
                                                      Codigo = a.TB_RESPONSAVEL_CODIGO,
                                                      Descricao = a.TB_RESPONSAVEL_NOME,
                                                      Endereco = a.TB_RESPONSAVEL_ENDERECO,
                                                      Cargo = a.TB_RESPONSAVEL_CARGO,
                                                      Gestor = (new GestorEntity(a.TB_GESTOR_ID))
                                                  }).ToList<ResponsavelEntity>();

            return resultado;
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            TB_RESPONSAVEL tbResponsavel = this.Db.TB_RESPONSAVELs.Where(a => a.TB_RESPONSAVEL_ID == this.Entity.Id.Value).FirstOrDefault();

            //if ( tbResponsavel. > 0)
                //return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_RESPONSAVELs
                .Where(a => a.TB_RESPONSAVEL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_RESPONSAVEL_ID != this.Entity.Id.Value)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_RESPONSAVELs
                .Where(a => a.TB_RESPONSAVEL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            }
            return retorno;
        }



        public ResponsavelEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<ResponsavelEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }


        public IList<ResponsavelEntity> ListarTodosCodUa()
        {
            throw new NotImplementedException();
        }
    }
}
