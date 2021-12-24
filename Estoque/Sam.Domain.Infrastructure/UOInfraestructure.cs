using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;


namespace Sam.Domain.Infrastructure
{
    public partial class UOInfraestructure : BaseInfraestructure, IUOService
    {
        #region ICrudBaseService<UOEntity> Members

        private UOEntity uo = new UOEntity();
        
        public UOEntity Entity
        {
            get { return uo; }

            set { uo = value; }
        }

        
        public IList<UOEntity> Listar()
        {
            IList<UOEntity> resultado = (from a in this.Db.TB_UOs
                                         orderby a.TB_UO_DESCRICAO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO,
                                             Orgao = (new OrgaoEntity(a.TB_ORGAO_ID))
                                         }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<UOEntity>();

            this.totalregistros = (from a in this.Db.TB_UOs
                                   select new
                                   {
                                       Id = a.TB_UO_ID,
                                   }).Count();
            return resultado;
        }
                
        public IList<UOEntity> Listar(int OrgaoId)
        {
            IList<UOEntity> resultado = (from a in this.Db.TB_UOs
                                         where (a.TB_ORGAO_ID == OrgaoId || OrgaoId.Equals(int.MinValue))
                                         orderby a.TB_UO_CODIGO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO,
                                             Orgao = (new OrgaoEntity(a.TB_ORGAO_ID)),
                                             Ativo = a.TB_UO_STATUS
                                         }).Skip(this.SkipRegistros)
                                           .Take(this.RegistrosPagina)
                                           .ToList<UOEntity>();

            this.totalregistros = (from a in this.Db.TB_UOs
                                   where (a.TB_ORGAO_ID == OrgaoId)
                                   select new
                                   {
                                       Id = a.TB_UO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<UOEntity> Imprimir(int OrgaoId)
        {
            IList<UOEntity> resultado = (from a in this.Db.TB_UOs
                                         where (a.TB_ORGAO_ID == OrgaoId || OrgaoId.Equals(int.MinValue))
                                         orderby a.TB_UO_CODIGO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO,
                                             Orgao = (new OrgaoEntity(a.TB_ORGAO_ID))
                                         })
                                           .ToList<UOEntity>();

       
            return resultado;
        }

        public IList<UOEntity> Imprimir()
        {
            IList<UOEntity> resultado = (from a in this.Db.TB_UOs
                                         orderby a.TB_UO_CODIGO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO,
                                             Orgao = (new OrgaoEntity(a.TB_ORGAO_ID))
                                         })
                                           .ToList<UOEntity>();


            return resultado;
        }
        public void Salvar()
        {
            TB_UO uoDb = new TB_UO();

            if (this.Entity.Id.HasValue)
                uoDb = this.Db.TB_UOs.Where(a => a.TB_UO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_UOs.InsertOnSubmit(uoDb);

            uoDb.TB_UO_CODIGO = this.Entity.Codigo;
            uoDb.TB_UO_DESCRICAO = this.Entity.Descricao;
            uoDb.TB_ORGAO_ID = this.Entity.Orgao.Id.Value;
            uoDb.TB_UO_STATUS = this.Entity.Ativo;
            this.Db.SubmitChanges();
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_UOs
                .Where(a => a.TB_UO_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_UO_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_UOs
                .Where(a => a.TB_UO_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }

        public void Excluir()
        {
            TB_UO uo = this.Db.TB_UOs.Where(a => a.TB_UO_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_UOs.DeleteOnSubmit(uo);
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        #endregion


        public UOEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<UOEntity> ListarTodosCod()
        {
            return (from a in this.Db.TB_UOs
                                         orderby a.TB_UO_DESCRICAO
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO,
                                             Orgao = (new OrgaoEntity(a.TB_ORGAO_ID))
                                         }).ToList<UOEntity>();
        }

        public IList<UOEntity> ListarTodosCod(int OrgaoId)
        {
            IList<UOEntity> resultado = (from a in this.Db.TB_UOs
                                         orderby a.TB_UO_CODIGO
                                         where a.TB_ORGAO_ID == OrgaoId
                                         select new UOEntity
                                         {
                                             Id = a.TB_UO_ID,
                                             Descricao = a.TB_UO_DESCRICAO,
                                             Codigo = a.TB_UO_CODIGO
                                         })
                                   .ToList();
            return resultado;
        }

        public IList<UOEntity> ListarTodosCod(int OrgaoId, IList<DivisaoEntity> divisaoList)
        {
            List<UOEntity> resultadoCompleto = new List<UOEntity>();
            IList<UOEntity> resultado;

            foreach (var divisao in divisaoList)
            {
                resultado = (from a in this.Db.TB_UOs
                             join uge in this.Db.TB_UGEs on a.TB_UO_ID equals uge.TB_UO_ID
                             join ua in Db.TB_UAs on uge.TB_UGE_ID equals ua.TB_UGE_ID
                             join div in Db.TB_DIVISAOs on ua.TB_UA_ID equals div.TB_UA_ID
                             orderby a.TB_UO_CODIGO
                             where a.TB_ORGAO_ID == OrgaoId
                             where div.TB_DIVISAO_ID == divisao.Id
                             select new UOEntity
                             {
                                 Id = a.TB_UO_ID,
                                 Descricao = a.TB_UO_DESCRICAO,
                                 Codigo = a.TB_UO_CODIGO
                             }).ToList();

                foreach (var res in resultado)
                {
                    if (resultadoCompleto.Where(a => a.Id == res.Id).Count() == 0)
                        resultadoCompleto.Add(res);
                }
            }

            return resultadoCompleto;
        }

        public UOEntity ObterUoPorCodigoUGE(int codigoUGE)
        {
            UOEntity objEntidade = (from ugeSIAFEM in this.Db.TB_UGEs
                                    where ugeSIAFEM.TB_UGE_CODIGO == codigoUGE
                                    select new UOEntity
                                    {
                                        Id = ugeSIAFEM.TB_UO.TB_UO_ID,
                                        Descricao = ugeSIAFEM.TB_UO.TB_UO_DESCRICAO,
                                        Codigo = ugeSIAFEM.TB_UO.TB_UO_CODIGO
                                    })
                                   .FirstOrDefault();
            return objEntidade;
        }
        //Busca UGE(s) Associada(s) a UO
        public IList<UOEntity> ListarUgePorUo(int uoId)
        {
            var resultado = (from a in this.Db.TB_UOs
                             join uge in this.Db.TB_UGEs on a.TB_UO_ID equals uge.TB_UO_ID
                             where a.TB_UO_CODIGO == uoId && uge.TB_UGE_STATUS == true
                             select new UOEntity
                             {
                                 Id = a.TB_UO_ID,
                                 Codigo = uge.TB_UGE_CODIGO,
                                 Descricao = uge.TB_UGE_DESCRICAO,
                                 Ativo = uge.TB_UGE_STATUS
                             }).ToList<UOEntity>();
            return resultado;
        }

    }
}
