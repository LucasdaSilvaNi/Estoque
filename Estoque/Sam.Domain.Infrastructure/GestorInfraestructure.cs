using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Configuration;


namespace Sam.Domain.Infrastructure
{
    public class GestorInfraestructure : BaseInfraestructure,IGestorService
    {
        private GestorEntity gestor = new GestorEntity();
        public GestorEntity Entity
        {
            get { return gestor; }
            set { gestor = value; }
        }

        public IList<GestorEntity> Listar()
        {
            IList<GestorEntity> resultado = (from a in Db.TB_GESTORs
                                             orderby (a.TB_GESTOR_NOME)
                                             select new GestorEntity
                                             {

                                                 Id = a.TB_GESTOR_ID,
                                                 //Codigo = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Nome = a.TB_GESTOR_NOME,
                                                 NomeReduzido = a.TB_GESTOR_NOME_REDUZIDO,
                                                 EnderecoLogradouro = a.TB_GESTOR_LOGRADOURO,
                                                 EnderecoNumero = a.TB_GESTOR_NUMERO,
                                                 EnderecoCompl = a.TB_GESTOR_COMPLEMENTO,
                                                 EnderecoTelefone = a.TB_GESTOR_TELEFONE,
                                                 CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Logotipo = a.TB_GESTOR_IMAGEM != null ? a.TB_GESTOR_IMAGEM.ToArray() : null,
                                                 Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                 Uo = (a.TB_UO_ID.Value != null ? new UOEntity(a.TB_UO_ID.Value) : new UOEntity()),
                                                 Uge = (a.TB_UGE_ID.Value != null ? new UGEEntity(a.TB_UGE_ID.Value) : new UGEEntity()),
                                                 TipoId = a.TB_UO_ID != null && a.TB_UO_ID > 0 ? 1 : (a.TB_UGE_ID != null && a.TB_UGE_ID > 0 ? 2 : 0),
                                                 //Ordem = a.ORDEM,
                                             })
//                                             .Skip(this.SkipRegistros)
//                                             .Take(this.RegistrosPagina)
                                             .ToList<GestorEntity>();

            this.totalregistros = (from a in Db.TB_GESTORs
                                   select new
                                   {
                                       a.TB_GESTOR_ID,
                                   }).Count();
            return resultado;
        }

        public IList<GestorEntity> Listar(int OrgaoId)
        {
            IList<GestorEntity> resultado = (from a in Db.TB_GESTORs
                                             where (a.TB_ORGAO_ID == OrgaoId)
                                             orderby (a.TB_GESTOR_NOME)
                                             select new GestorEntity
                                             {
                                                 Id = a.TB_GESTOR_ID,
                                                 //Codigo = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Nome = a.TB_GESTOR_NOME,
                                                 NomeReduzido = a.TB_GESTOR_NOME_REDUZIDO,
                                                 EnderecoLogradouro = a.TB_GESTOR_LOGRADOURO,
                                                 EnderecoNumero = a.TB_GESTOR_NUMERO,
                                                 EnderecoCompl = a.TB_GESTOR_COMPLEMENTO,
                                                 EnderecoTelefone = a.TB_GESTOR_TELEFONE,
                                                 CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Logotipo = a.TB_GESTOR_IMAGEM != null ? a.TB_GESTOR_IMAGEM.ToArray() : null,
                                                 Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                                 Uo = (a.TB_UO_ID.Value != null ? new UOEntity(a.TB_UO_ID.Value) : new UOEntity()),
                                                 Uge = (a.TB_UGE_ID.Value != null ? new UGEEntity(a.TB_UGE_ID.Value) : new UGEEntity()),
                                                 TipoId = a.TB_UO_ID != null && a.TB_UO_ID > 0 ? 1 : (a.TB_UGE_ID != null && a.TB_UGE_ID > 0 ? 2 : 0),
                                             })
                                             .Skip(this.SkipRegistros)
                                             .Take(this.RegistrosPagina)
                                             .ToList<GestorEntity>();

            this.totalregistros = (from a in Db.TB_GESTORs
                                   where (a.TB_ORGAO_ID == OrgaoId)
                                   select new
                                   {
                                        a.TB_GESTOR_ID,
                                   }).Count();
            return resultado;
        }

        public GestorEntity Selecionar(int GestorId)
        {
            TB_GESTOR tbGestor = (from a in this.Db.TB_GESTORs
                                  where a.TB_GESTOR_ID == GestorId
                                  select a).FirstOrDefault();

            GestorEntity gestor = new GestorEntity();

            if (gestor != null)
            {
                gestor.Id = tbGestor.TB_GESTOR_ID;
                //gestor.Codigo = tbGestor.TB_GESTOR_CODIGO_GESTAO;
                gestor.Nome = tbGestor.TB_GESTOR_NOME;
                gestor.NomeReduzido = tbGestor.TB_GESTOR_NOME_REDUZIDO;
                gestor.EnderecoLogradouro = tbGestor.TB_GESTOR_LOGRADOURO;
                gestor.EnderecoNumero = tbGestor.TB_GESTOR_NUMERO;
                gestor.EnderecoTelefone = tbGestor.TB_GESTOR_TELEFONE;
                gestor.EnderecoCompl = tbGestor.TB_GESTOR_COMPLEMENTO;
                gestor.CodigoGestao = tbGestor.TB_GESTOR_CODIGO_GESTAO;
                gestor.Logotipo = tbGestor.TB_GESTOR_IMAGEM != null ? tbGestor.TB_GESTOR_IMAGEM.ToArray() : null;
                gestor.Orgao = new OrgaoEntity(tbGestor.TB_ORGAO_ID);
                gestor.Uge = new UGEEntity(tbGestor.TB_UGE_ID);
                gestor.Uo = new UOEntity(tbGestor.TB_UO_ID);
            }
            return gestor;
        }

        public int RetornaGestorOrganizacional(int? orgaoId, int? uoId, int? ugeId)
        {

            var result = (from a in Db.TB_GESTORs
                          where a.TB_UGE_ID == ugeId
                          select a.TB_GESTOR_ID).FirstOrDefault();

            if (result == 0)
            {
                result = (from a in Db.TB_GESTORs
                          where a.TB_UO_ID == uoId
                          select a.TB_GESTOR_ID).FirstOrDefault();
            }
            if (result == 0)
            {
                result = (from a in Db.TB_GESTORs
                          where a.TB_ORGAO_ID == orgaoId
                          select a.TB_GESTOR_ID).FirstOrDefault();
            }

            return result;
        }

        public IList<GestorEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public IList<GestorEntity> Imprimir(int OrgaoId)
        {
            IList<GestorEntity> resultado = (from a in Db.TB_GESTORs 
                                             where (a.TB_ORGAO_ID == OrgaoId)
                                             orderby (a.TB_GESTOR_ID)
                                             select new GestorEntity
                                             {
                                                 Id = a.TB_GESTOR_ID,
                                                 //Codigo = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Nome = a.TB_GESTOR_NOME,
                                                 NomeReduzido = a.TB_GESTOR_NOME_REDUZIDO,
                                                 EnderecoLogradouro = a.TB_GESTOR_LOGRADOURO,
                                                 EnderecoNumero = a.TB_GESTOR_NUMERO,
                                                 EnderecoCompl = a.TB_GESTOR_COMPLEMENTO,
                                                 EnderecoTelefone = a.TB_GESTOR_TELEFONE,
                                                 CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO
                                             })
                                             .ToList<GestorEntity>();
            return resultado;
        }

        public void Excluir()
        {
            TB_GESTOR tbGestor = this.Db.TB_GESTORs
                .Where(a => a.TB_GESTOR_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_GESTORs.DeleteOnSubmit(tbGestor);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_GESTOR tbGestor = new TB_GESTOR();

            if (this.Entity.Id.HasValue)
            {
                tbGestor = this.Db.TB_GESTORs.Where(a => a.TB_GESTOR_ID == this.Entity.Id).FirstOrDefault();
            }
            else
                Db.TB_GESTORs.InsertOnSubmit(tbGestor);

            tbGestor.TB_GESTOR_NOME = this.Entity.Nome;
            tbGestor.TB_GESTOR_NOME_REDUZIDO = this.Entity.NomeReduzido;
            tbGestor.TB_GESTOR_LOGRADOURO = this.Entity.EnderecoLogradouro;
            tbGestor.TB_GESTOR_NUMERO = this.Entity.EnderecoNumero;
            tbGestor.TB_GESTOR_COMPLEMENTO = this.Entity.EnderecoCompl;
            tbGestor.TB_GESTOR_TELEFONE = this.Entity.EnderecoTelefone;
            
            if(this.Entity.CodigoGestao.HasValue)
                tbGestor.TB_GESTOR_CODIGO_GESTAO = this.Entity.CodigoGestao.Value;
    
            tbGestor.TB_ORGAO_ID = this.Entity.Orgao.Id.Value;
            tbGestor.TB_UGE_ID = this.Entity.Uge.Id.HasValue ? this.Entity.Uge.Id : null;
            tbGestor.TB_UO_ID = this.Entity.Uo.Id.HasValue ? this.Entity.Uo.Id : null;
            
            if (this.Entity.Logotipo != null)
                tbGestor.TB_GESTOR_IMAGEM = this.Entity.Logotipo;
            

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            //TB_GESTOR tbGestor = this.Db.TB_GESTORs
            //    .Where(a => a.TB_GESTOR_ID == this.Entity.Id).FirstOrDefault();

            //if (tbGestor.TB_CENTRO_CUSTOs.Count > 0 ||
            //    tbGestor.TB_RESPONSAVELs.Count > 0 ||
                //tbGestor.TB_GESTOR_NUMERO.Count > 0)
                //return false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = true;

            //if (this.Entity.Id.HasValue)
            //{
            //    retorno = this.Db.TB_GESTORs
            //       // .Where(a => a.TB_GESTOR_CODIGO_GESTAO == this.Entity.Codigo.Value)
            //        .Where(a => a.TB_GESTOR_ID != this.Entity.Id.Value).Count() > 0;
            //}
            //else
            //    retorno = this.Db.TB_GESTORs
            //        .Where(a => a.TB_GESTOR_CODIGO_GESTAO == this.Entity.Codigo).Count() > 0;

            return retorno;
        }


        public GestorEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<GestorEntity> ListarTodosCod()
        {
            return (from a in Db.TB_GESTORs
                    orderby (a.TB_GESTOR_NOME)
                    select new GestorEntity
                    {
                        Id = a.TB_GESTOR_ID,
                        //Codigo = a.TB_GESTOR_CODIGO_GESTAO,
                        Nome = a.TB_GESTOR_NOME,
                        NomeReduzido = a.TB_GESTOR_NOME_REDUZIDO,
                        EnderecoLogradouro = a.TB_GESTOR_LOGRADOURO,
                        EnderecoNumero = a.TB_GESTOR_NUMERO,
                        EnderecoCompl = a.TB_GESTOR_COMPLEMENTO,
                        EnderecoTelefone = a.TB_GESTOR_TELEFONE,
                        CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO,
                        Logotipo = a.TB_GESTOR_IMAGEM != null ? a.TB_GESTOR_IMAGEM.ToArray() : null,
                        Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                        Uo = (a.TB_UO_ID.Value != null ? new UOEntity(a.TB_UO_ID.Value) : new UOEntity()),
                        Uge = (a.TB_UGE_ID.Value != null ? new UGEEntity(a.TB_UGE_ID.Value) : new UGEEntity()),
                        TipoId = a.TB_UO_ID != null && a.TB_UO_ID > 0 ? 1 : (a.TB_UGE_ID != null && a.TB_UGE_ID > 0 ? 2 : 0),
                    }).ToList<GestorEntity>();
        }

        public IList<GestorEntity> Listar(System.Linq.Expressions.Expression<Func<GestorEntity, bool>> where)
        {
            return (from a in Db.TB_GESTORs
                    orderby (a.TB_GESTOR_NOME)
                    select new GestorEntity
                    {
                        Id = a.TB_GESTOR_ID,
                        //Codigo = a.TB_GESTOR_CODIGO_GESTAO,
                        Nome = a.TB_GESTOR_NOME,
                        NomeReduzido = a.TB_GESTOR_NOME_REDUZIDO,
                        EnderecoLogradouro = a.TB_GESTOR_LOGRADOURO,
                        EnderecoNumero = a.TB_GESTOR_NUMERO,
                        EnderecoCompl = a.TB_GESTOR_COMPLEMENTO,
                        EnderecoTelefone = a.TB_GESTOR_TELEFONE,
                        CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO,
                        Logotipo = a.TB_GESTOR_IMAGEM != null ? a.TB_GESTOR_IMAGEM.ToArray() : null,
                        Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                        Uo = (a.TB_UO_ID.Value != null ? new UOEntity(a.TB_UO_ID.Value) : new UOEntity()),
                        Uge = (a.TB_UGE_ID.Value != null ? new UGEEntity(a.TB_UGE_ID.Value) : new UGEEntity()),
                        TipoId = a.TB_UO_ID != null && a.TB_UO_ID > 0 ? 1 : (a.TB_UGE_ID != null && a.TB_UGE_ID > 0 ? 2 : 0),
                    }).Where(where).ToList<GestorEntity>();
        }


        public IList<GestorEntity> ListarTodosCod(int OrgaoId)
        {
            IList<GestorEntity> resultado = (from a in Db.TB_GESTORs
                                             where (a.TB_ORGAO_ID == OrgaoId)
                                             orderby (a.TB_GESTOR_NOME)
                                             select new GestorEntity
                                             {
                                                 Id = a.TB_GESTOR_ID,
                                                 CodigoGestao = a.TB_GESTOR_CODIGO_GESTAO,
                                                 Nome = a.TB_GESTOR_NOME
                                                
                                             })
                                  .ToList<GestorEntity>();
            return resultado;
        }

        public int RecuperaCodigoOrgao(string nomeOrgaoReduzido)
        {
            int resultado = (from a in Db.TB_GESTORs
                             join b in Db.TB_ORGAOs on a.TB_ORGAO_ID equals b.TB_ORGAO_ID
                             where (a.TB_GESTOR_NOME_REDUZIDO == nomeOrgaoReduzido)
                             select b.TB_ORGAO_CODIGO
                            ).FirstOrDefault();
            return resultado;
        }

     
    }
}
