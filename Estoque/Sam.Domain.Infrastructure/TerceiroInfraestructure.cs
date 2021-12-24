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
    public class TerceiroInfraestructure : BaseInfraestructure, ITerceiroService
    {
        private TerceiroEntity Terceiro = new TerceiroEntity();

        public TerceiroEntity Entity
        {
            get { return Terceiro; }
            set { Terceiro = value; }
        }

        public void Salvar()
        {
            //TB_TERCEIRO tbTerceiro = new TB_TERCEIRO();

            //if (this.Entity.Id.HasValue)
            //    tbTerceiro = this.Db.TB_TERCEIROs.Where(a => a.TB_TERCEIRO_ID == this.Entity.Id.Value).FirstOrDefault();
            //else
            //    Db.TB_TERCEIROs.InsertOnSubmit(tbTerceiro);

            //tbTerceiro.TB_ORGAO_ID = this.Entity.Orgao.Id.Value;
            //tbTerceiro.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
            //tbTerceiro.TB_TERCEIRO_CNPJ = this.Entity.Cnpj;
            //tbTerceiro.TB_TERCEIRO_NOME = this.Entity.Nome;
            //tbTerceiro.TB_TERCEIRO_LOGRADOURO = this.Entity.Logradouro;
            //tbTerceiro.TB_TERCEIRO_NUMERO = this.Entity.Numero;
            //tbTerceiro.TB_TERCEIRO_COMPLEMENTO = this.Entity.Complemento;
            //tbTerceiro.TB_TERCEIRO_BAIRRO = this.Entity.Bairro;
            //tbTerceiro.TB_TERCEIRO_MUNICIPIO = this.Entity.Cidade;
            //tbTerceiro.TB_UF_ID = this.Entity.Uf.Id.Value;
            //tbTerceiro.TB_TERCEIRO_CEP = this.Entity.Cep;
            //tbTerceiro.TB_TERCEIRO_TELEFONE = this.Entity.Telefone;
            //tbTerceiro.TB_TERCEIRO_FAX = this.Entity.Fax;
            //this.Db.SubmitChanges();
        }

        public void Excluir()
        {
            //TB_TERCEIRO tbTerceiro
            //    = this.Db.TB_TERCEIROs.Where(a => a.TB_TERCEIRO_ID == this.Entity.Id.Value).FirstOrDefault();
            //this.Db.TB_TERCEIROs.DeleteOnSubmit(tbTerceiro);
            //this.Db.SubmitChanges();
        }

        public IList<TerceiroEntity> Listar()
        {
            //IList<TerceiroEntity> resultado = (from a in this.Db.VW_TERCEIROs  
            //                                   orderby a.ORDEM
            //                                   select new TerceiroEntity
            //                                   {
            //                                       Id = a.TB_TERCEIRO_ID,
            //                                       Cnpj = a.TB_TERCEIRO_CNPJ,
            //                                       Nome = a.TB_TERCEIRO_NOME,
            //                                       Logradouro = a.TB_TERCEIRO_LOGRADOURO,
            //                                       Numero = a.TB_TERCEIRO_NUMERO,
            //                                       Complemento = a.TB_TERCEIRO_COMPLEMENTO,
            //                                       Bairro = a.TB_TERCEIRO_BAIRRO,
            //                                       Cidade = a.TB_TERCEIRO_MUNICIPIO,
            //                                       Uf = (new UFEntity(a.TB_UF_ID)),
            //                                       Cep = a.TB_TERCEIRO_CEP,
            //                                       Telefone = a.TB_TERCEIRO_TELEFONE,
            //                                       Fax = a.TB_TERCEIRO_FAX,
            //                                       Ordem = a.ORDEM
            //                                   }).Skip(this.SkipRegistros)
            //                                  .Take(this.RegistrosPagina)
            //                                  .ToList<TerceiroEntity>();

            //this.totalregistros = (from a in this.Db.VW_TERCEIROs
            //                       select new
            //                       {
            //                           Id = a.TB_TERCEIRO_ID,
            //                       }).Count();

            return new List<TerceiroEntity>();

        }

        public IList<TerceiroEntity> Listar(int OrgaoId, int GestorId)
        {
            //IList<TerceiroEntity> resultado = (from a in this.Db.VW_TERCEIROs
            //                                   where (a.TB_ORGAO_ID == OrgaoId && a.TB_GESTOR_ID == GestorId)
            //                                   orderby a.ORDEM
            //                                   select new TerceiroEntity
            //                                   {
            //                                       Id = a.TB_TERCEIRO_ID,
            //                                       Cnpj = a.TB_TERCEIRO_CNPJ,
            //                                       Nome = a.TB_TERCEIRO_NOME,
            //                                       Logradouro = a.TB_TERCEIRO_LOGRADOURO,
            //                                       Numero = a.TB_TERCEIRO_NUMERO,
            //                                       Complemento = a.TB_TERCEIRO_COMPLEMENTO,
            //                                       Bairro = a.TB_TERCEIRO_BAIRRO,
            //                                       Cidade = a.TB_TERCEIRO_MUNICIPIO,
            //                                       Uf = (new UFEntity(a.TB_UF_ID)),
            //                                       Cep = a.TB_TERCEIRO_CEP,
            //                                       Telefone = a.TB_TERCEIRO_TELEFONE,
            //                                       Fax = a.TB_TERCEIRO_FAX,
            //                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                       Ordem = a.ORDEM
            //                                   }).Skip(this.SkipRegistros)
            //                                 .Take(this.RegistrosPagina)
            //                                 .ToList<TerceiroEntity>();

            //this.totalregistros = (from a in this.Db.VW_TERCEIROs
            //                       where (a.TB_ORGAO_ID == OrgaoId && a.TB_GESTOR_ID == GestorId)
            //                       select new
            //                       {
            //                           Id = a.TB_TERCEIRO_ID,
            //                       }).Count();

            return new List<TerceiroEntity>();

        }


        public IList<TerceiroEntity> Imprimir(int OrgaoId, int GestorId)
        {
            //IList<TerceiroEntity> resultado = (from a in this.Db.VW_TERCEIROs
            //                                   join b in this.Db.TB_UFs on a.TB_UF_ID equals b.TB_UF_ID
            //                                   where (a.TB_ORGAO_ID == OrgaoId && a.TB_GESTOR_ID == GestorId)
            //                                   orderby a.ORDEM
            //                                   select new TerceiroEntity
            //                                   {
            //                                       Id = a.TB_TERCEIRO_ID,
            //                                       Cnpj = a.TB_TERCEIRO_CNPJ,
            //                                       Nome = a.TB_TERCEIRO_NOME,
            //                                       Logradouro = a.TB_TERCEIRO_LOGRADOURO,
            //                                       Numero = a.TB_TERCEIRO_NUMERO,
            //                                       Complemento = a.TB_TERCEIRO_COMPLEMENTO,
            //                                       Bairro = a.TB_TERCEIRO_BAIRRO,
            //                                       Cidade = a.TB_TERCEIRO_MUNICIPIO,
            //                                       Uf = (new UFEntity { Id = b.TB_UF_ID, Descricao = b.TB_UF_DESCRICAO, Sigla = b.TB_UF_SIGLA}),
            //                                       Cep = a.TB_TERCEIRO_CEP,
            //                                       Telefone = a.TB_TERCEIRO_TELEFONE,
            //                                       Fax = a.TB_TERCEIRO_FAX,
            //                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                       Ordem = a.ORDEM
            //                                   })
            //                                 .ToList<TerceiroEntity>();



            return new List<TerceiroEntity>();

        }


        public IList<TerceiroEntity> Imprimir()
        {
            //IList<TerceiroEntity> resultado = (from a in this.Db.VW_TERCEIROs
                                             
            //                                   orderby a.ORDEM
            //                                   select new TerceiroEntity
            //                                   {
            //                                       Id = a.TB_TERCEIRO_ID,
            //                                       Cnpj = a.TB_TERCEIRO_CNPJ,
            //                                       Nome = a.TB_TERCEIRO_NOME,
            //                                       Logradouro = a.TB_TERCEIRO_LOGRADOURO,
            //                                       Numero = a.TB_TERCEIRO_NUMERO,
            //                                       Complemento = a.TB_TERCEIRO_COMPLEMENTO,
            //                                       Bairro = a.TB_TERCEIRO_BAIRRO,
            //                                       Cidade = a.TB_TERCEIRO_MUNICIPIO,
            //                                       Uf = (new UFEntity(a.TB_UF_ID)),
            //                                       Cep = a.TB_TERCEIRO_CEP,
            //                                       Telefone = a.TB_TERCEIRO_TELEFONE,
            //                                       Fax = a.TB_TERCEIRO_FAX,
            //                                       Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
            //                                       Ordem = a.ORDEM
            //                                   })
            //                                 .ToList<TerceiroEntity>();



            //return resultado;

            return new List<TerceiroEntity>();
        }


        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_TERCEIRO tbTerceiro = this.Db.TB_TERCEIROs.Where(a => a.TB_TERCEIRO_ID == this.Entity.Id.Value).FirstOrDefault();

            //if (tbTerceiro. > 0)
            //    return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            //if (this.Entity.Id.HasValue)
            //{
            //    retorno = this.Db.TB_TERCEIROs
            //    .Where(a => a.TB_TERCEIRO_CNPJ == this.Entity.Cnpj)
            //    .Where(a => a.TB_TERCEIRO_ID != this.Entity.Id.Value)
            //    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
            //    .Count() > 0;
            //}
            //else
            //{
            //    retorno = this.Db.TB_TERCEIROs
            //    .Where(a => a.TB_TERCEIRO_CNPJ == this.Entity.Cnpj)
            //    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
            //    .Count() > 0;
            //}
            return retorno;
        }


        public TerceiroEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<TerceiroEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }
    }
}
