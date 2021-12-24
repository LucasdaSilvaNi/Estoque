using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using System.Linq.Expressions;


namespace Sam.Domain.Infrastructure
{
    public partial class OrgaoInfraestructure : BaseInfraestructure, IOrgaoService
    {
        #region ICrudBaseService<OrgaoEntity> Members

        private OrgaoEntity orgao = new OrgaoEntity();

        public OrgaoEntity Entity
        {
            get { return orgao; }
            set { orgao = value; }
        }

        public IList<OrgaoEntity> Listar()
        {
            IList<OrgaoEntity> resultado = (from a in this.Db.TB_ORGAOs
                                            orderby a.TB_ORGAO_CODIGO
                                            select new OrgaoEntity
                                            {
                                                Id = a.TB_ORGAO_ID,
                                                Descricao = a.TB_ORGAO_DESCRICAO,
                                                Codigo = a.TB_ORGAO_CODIGO,
                                                Ativo = Convert.ToBoolean(a.TB_ORGAO_STATUS),
                                                Implantado = a.TB_ORGAO_IMPLANTADO,
                                                IntegracaoSIAFEM=a.TB_ORGAO_INTEGRACAO_SIAFEM
                                            }).Skip(this.SkipRegistros)
                                   .Take(this.RegistrosPagina)
                                   .ToList();

            this.totalregistros = (from a in this.Db.TB_ORGAOs
                                   select new
                                   {
                                       Id = a.TB_ORGAO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<OrgaoEntity> Imprimir()
        {
            IList<OrgaoEntity> resultado = (from a in this.Db.TB_ORGAOs
                                            orderby a.TB_ORGAO_CODIGO
                                            select new OrgaoEntity
                                            {
                                                Id = a.TB_ORGAO_ID,
                                                Descricao = a.TB_ORGAO_DESCRICAO,
                                                Codigo = a.TB_ORGAO_CODIGO,
                                            })
                                   .ToList();

            return resultado;
        }

        public void Excluir()
        {
            TB_ORGAO orgao
               = this.Db.TB_ORGAOs.Where(a => a.TB_ORGAO_ID == this.Entity.Id.Value).FirstOrDefault();
            this.Db.TB_ORGAOs.DeleteOnSubmit(orgao);
            this.Db.SubmitChanges();
        }


        public void Salvar()
        {
            TB_ORGAO orgaoDb = new TB_ORGAO();

            if (this.Entity.Id.HasValue)
                orgaoDb = this.Db.TB_ORGAOs.Where(a => a.TB_ORGAO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_ORGAOs.InsertOnSubmit(orgaoDb);
            orgaoDb.TB_ORGAO_CODIGO = Convert.ToInt32(this.Entity.Codigo.Value);
            orgaoDb.TB_ORGAO_DESCRICAO = Convert.ToString(this.Entity.Descricao);
            orgaoDb.TB_ORGAO_STATUS = Convert.ToBoolean(this.Entity.Ativo);
            orgaoDb.TB_ORGAO_IMPLANTADO = Convert.ToBoolean(this.Entity.Implantado);
            orgaoDb.TB_ORGAO_INTEGRACAO_SIAFEM = Convert.ToBoolean(this.Entity.IntegracaoSIAFEM);
            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            bool retorno = true;

            TB_ORGAO orgao
                = this.Db.TB_ORGAOs.Where(a => a.TB_ORGAO_ID == this.Entity.Id.Value).FirstOrDefault();
            //if (orgao.TB_USUARIOs.Count > 0)
            //    retorno = false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_ORGAOs
                .Where(a => a.TB_ORGAO_CODIGO == this.Entity.Codigo.Value)
                .Where(a => a.TB_ORGAO_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_ORGAOs
                .Where(a => a.TB_ORGAO_CODIGO == this.Entity.Codigo.Value)
                .Count() > 0;
            }
            return retorno;
        }

        #endregion


        public OrgaoEntity LerRegistro()
        {
            OrgaoEntity resultado = (from a in this.Db.TB_ORGAOs
                                     where a.TB_ORGAO_ID == this.orgao.Id
                                     select new OrgaoEntity
                                     {
                                         Id = a.TB_ORGAO_ID,
                                         Descricao = a.TB_ORGAO_DESCRICAO,
                                         Codigo = a.TB_ORGAO_CODIGO
                                     })
                                   .First();

            this.orgao = resultado;
            return resultado;
        }

        public OrgaoEntity LerRegistro(int orgaoId)
        {
            OrgaoEntity resultado = (from a in this.Db.TB_ORGAOs
                                     where a.TB_ORGAO_ID == orgaoId
                                     select new OrgaoEntity
                                     {
                                         Id = a.TB_ORGAO_ID,
                                         Descricao = a.TB_ORGAO_DESCRICAO,
                                         Codigo = a.TB_ORGAO_CODIGO
                                     })
                                   .First();

            this.orgao = resultado;
            return resultado;
        }


        public IList<OrgaoEntity> ListarTodosCod(int? orgaoId)
        {
            IList<OrgaoEntity> resultado = null;


            if (orgaoId.HasValue)
            {
                resultado = (from a in this.Db.TB_ORGAOs
                             where a.TB_ORGAO_ID == orgaoId
                             orderby a.TB_ORGAO_CODIGO
                             select new OrgaoEntity
                             {
                                 Id = a.TB_ORGAO_ID,
                                 Descricao = a.TB_ORGAO_DESCRICAO,
                                 Codigo = a.TB_ORGAO_CODIGO,
                             })
                                    .ToList();
                return resultado;

            }
            else
            {
                resultado = (from a in this.Db.TB_ORGAOs
                            
                             orderby a.TB_ORGAO_CODIGO
                             select new OrgaoEntity
                             {
                                 Id = a.TB_ORGAO_ID,
                                 Descricao = a.TB_ORGAO_DESCRICAO,
                                 Codigo = a.TB_ORGAO_CODIGO,
                             })
                                  .ToList();
                return resultado;

            }
            
            return resultado;
        }

        public IList<OrgaoEntity> ListarOrgaosPorGestao(int codigoGestao, bool excluirOrgaosGestaoDoRetorno = false, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            IQueryable<TB_ORGAO> qryConsulta = null;



            if (excluirOrgaosGestaoDoRetorno)
            {
                //Retornar apenas Orgaos de Gestoes diferentes
                qryConsulta = (from orgao in Db.TB_ORGAOs
                               join gestorOrgao in Db.TB_GESTORs on orgao.TB_ORGAO_ID equals gestorOrgao.TB_ORGAO_ID
                               where gestorOrgao.TB_GESTOR_CODIGO_GESTAO != codigoGestao
                               select orgao).AsQueryable();
            }
            else
            {
                //Retornar apenas Orgaos da mesma Gestoes
                qryConsulta = (from orgao in Db.TB_ORGAOs
                               join gestorOrgao in Db.TB_GESTORs on orgao.TB_ORGAO_ID equals gestorOrgao.TB_ORGAO_ID
                               where gestorOrgao.TB_GESTOR_CODIGO_GESTAO == codigoGestao
                               select orgao).AsQueryable();
            }


            lstOrgaos = qryConsulta.Select(_instanciadorDTOOrgao())
                                   .ToList();

            return lstOrgaos;
        }
        //Retornar órgãos implantados
        public IList<OrgaoEntity> ListarOrgaosPorGestaoImplantado(int codigoGestao, bool excluirOrgaosGestaoDoRetorno = false, bool gerarComCodigoDescricao = true)
        {
            IList<OrgaoEntity> lstOrgaos = null;
            IQueryable<TB_ORGAO> qryConsulta = null;

            if (excluirOrgaosGestaoDoRetorno)
            {
                //Retornar apenas Orgaos de Gestoes diferentes
                qryConsulta = (from orgao in Db.TB_ORGAOs
                               join gestorOrgao in Db.TB_GESTORs on orgao.TB_ORGAO_ID equals gestorOrgao.TB_ORGAO_ID
                               where gestorOrgao.TB_GESTOR_CODIGO_GESTAO != codigoGestao && orgao.TB_ORGAO_IMPLANTADO == true
                                        && orgao.TB_ORGAO_STATUS == true
                                    orderby orgao.TB_ORGAO_ID ascending 

                               select orgao).AsQueryable();
            }
            else
            { 

            qryConsulta = (from orgao in Db.TB_ORGAOs
                           join gestorOrgao in Db.TB_GESTORs on orgao.TB_ORGAO_ID equals gestorOrgao.TB_ORGAO_ID
                           where gestorOrgao.TB_GESTOR_CODIGO_GESTAO == codigoGestao && orgao.TB_ORGAO_IMPLANTADO == true
                                    && orgao.TB_ORGAO_STATUS == true
                           orderby orgao.TB_ORGAO_ID ascending
                           select orgao).AsQueryable();
            }
            lstOrgaos = qryConsulta.Select(_instanciadorDTOOrgao())
                                  .ToList();          

            return lstOrgaos;
        }


        private Func<TB_ORGAO, OrgaoEntity> _instanciadorDTOOrgao()
        {
            Func<TB_ORGAO, OrgaoEntity> _actionSeletor = null;

            _actionSeletor = orgaoAdministracao => new OrgaoEntity()
                                                                    {
                                                                        Id = orgaoAdministracao.TB_ORGAO_ID,
                                                                        Codigo = orgaoAdministracao.TB_ORGAO_CODIGO,
                                                                        Descricao = orgaoAdministracao.TB_ORGAO_DESCRICAO
                                                                    };


            return _actionSeletor;
        }
        //Busca UO(s) Associada(s) ao ORGAO
        public IList<OrgaoEntity> ListarUoPorOrgao(int? orgaoId)
        {
            var resultado = (from orgao in this.Db.TB_ORGAOs
                             join uo in this.Db.TB_UOs on orgao.TB_ORGAO_ID equals uo.TB_ORGAO_ID
                             where orgao.TB_ORGAO_CODIGO == orgaoId && uo.TB_UO_STATUS == true
                             orderby uo.TB_UO_CODIGO ascending 
                             select new OrgaoEntity
                             {

                                 Id = uo.TB_UO_ID,
                                 Codigo = uo.TB_UO_CODIGO,
                                 Descricao = uo.TB_UO_DESCRICAO,
                                 Ativo = Convert.ToBoolean(uo.TB_UO_STATUS)
                             }).ToList<OrgaoEntity>();
            return resultado;
        }

        public OrgaoEntity ListarCodigoOrgao(int orgaoId)
        {
            OrgaoEntity resultado = (from a in this.Db.TB_ORGAOs
                                     where a.TB_ORGAO_ID == orgaoId
                                     orderby a.TB_ORGAO_ID ascending
                                     select new OrgaoEntity
                                     {                                                                            
                                         Codigo = a.TB_ORGAO_CODIGO
                                     })
                                   .First();
            this.orgao = resultado;
            return resultado;
        }

        public IList<OrgaoEntity> ListarTodosCod()
        {
           var resultado = (from a in this.Db.TB_ORGAOs
                       
                         orderby a.TB_ORGAO_CODIGO
                         select new OrgaoEntity
                         {
                             Id = a.TB_ORGAO_ID,
                             Descricao = a.TB_ORGAO_DESCRICAO,
                             Codigo = a.TB_ORGAO_CODIGO,
                         })
                                    .ToList();
            return resultado;
        }
    }
}
