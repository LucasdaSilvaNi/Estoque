using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Transactions;
using System.Diagnostics;
using Sam.Common.LambdaExpression;
using System.Linq.Expressions;

namespace Sam.Domain.Infrastructure
{
    public partial class PTResInfraestructure : BaseInfraestructure, IPTResService
    {
        private PTResEntity PTRes = new PTResEntity();
        
        public PTResEntity Entity
        {
            get { return PTRes; }
            set { PTRes = value; }
        }

        public bool Salvar()
        {
            throw new AccessViolationException("Tabela somente-leitura, a partir do SAM. Alterações somente via SIGEO.");
        }
        void ICrudBaseService<PTResEntity>.Salvar()
        {
            throw new AccessViolationException("Tabela somente-leitura, a partir do SAM. Alterações somente via SIGEO.");
        }

        public bool Excluir()
        {
            throw new AccessViolationException("Tabela PTREs permitido apenas acesso para leitura, por este sistema. Alterações permitidas somente via SIGEO.");
        }
        void ICrudBaseService<PTResEntity>.Excluir()
        {
            throw new AccessViolationException("Tabela somente-leitura, a partir do SAM. Alterações somente via SIGEO.");
        }

        public IList<PTResEntity> Listar()
        {
            IList<PTResEntity> resultado = (from a in this.Db.TB_PTREs
                                            orderby a.TB_PTRES_CODIGO
                                            select new PTResEntity
                                            {
                                                Id = a.TB_PTRES_ID,
                                                Codigo = a.TB_PTRES_CODIGO,
                                                Descricao = a.TB_PTRES_DESCRICAO
                                            }).Skip(this.SkipRegistros)
                                              .Take(this.RegistrosPagina)
                                              .ToList<PTResEntity>();

            this.totalregistros = (resultado.IsNotNull())? resultado.Count() : 0;

            return resultado;

        }

        public IList<PTResEntity> Imprimir()
        {
            IList<PTResEntity> resultado = (from a in this.Db.TB_PTREs
                                            orderby a.TB_PTRES_CODIGO
                                            select new PTResEntity
                                            {
                                                Id = a.TB_PTRES_ID,
                                                Codigo = a.TB_PTRES_CODIGO,
                                                Descricao = a.TB_PTRES_DESCRICAO                                            })
                                              .ToList<PTResEntity>();



            return resultado;
        }

        void PodeExcluir()
        {
            throw new AccessViolationException("Tabela somente-leitura, a partir do SAM. Alterações somente via SIGEO.");
        }
        bool ICrudBaseService<PTResEntity>.PodeExcluir() 
        {
            throw new AccessViolationException("Tabela somente-leitura, a partir do SAM. Alterações somente via SIGEO.");
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_PTREs
                .Where(a => a.TB_PTRES_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_PTRES_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_PTREs
                .Where(a => a.TB_PTRES_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }

        public PTResEntity LerRegistro()
        {
            return Listar().Where(a => a.Id == Entity.Id).FirstOrDefault();
        }

        public PTResEntity LerRegistro(string CodigoPtRes)
        {
            return Listar().Where(a => a.Codigo == Convert.ToInt32(CodigoPtRes)).FirstOrDefault();
        }

        public PTResEntity Listar(int pIntCodigoPtRes)
        {
            PTResEntity objRetorno = (from rowTabela in this.Db.TB_PTREs
                                      where rowTabela.TB_PTRES_CODIGO == pIntCodigoPtRes
                                      select new PTResEntity
                                      {
                                          Id = rowTabela.TB_PTRES_ID,
                                          Codigo = rowTabela.TB_PTRES_CODIGO,
                                          Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                          CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                          CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                          CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                          AnoDotacao = rowTabela.TB_PTRES_ANO,
                                          //Ativo = rowTabela.TB_PTRES_STATUS,
                                          ProgramaTrabalho = new ProgramaTrabalho(Int64.Parse(rowTabela.TB_PTRES_PT_CODIGO)),
                                          //}).FirstOrDefault();
                                      }).DistinctBy(ptres => new { ptres.CodigoUGE, ptres.Codigo }).FirstOrDefault();
            return objRetorno;
        }

        public IList<PTResEntity> Listar(int ugeCodigo, bool retornaListagem = true)
        {
            IQueryable<TB_PTRE> qryConsulta = null;
            IList<PTResEntity> lstRetorno = null;

            qryConsulta = (from rowTabela in this.Db.TB_PTREs
                           where rowTabela.TB_PTRES_UGE_CODIGO == ugeCodigo
                           //select rowTabela).AsQueryable();
                           //select rowTabela).DistinctBy(ptres => new { ptres.TB_PTRES_UGE_CODIGO, ptres.TB_PTRES_CODIGO }).AsQueryable();
                           select rowTabela).DistinctBy(ptres => new { ptres.TB_PTRES_UGE_CODIGO, ptres.TB_PTRES_CODIGO, ptres.TB_PTRES_PT_PROJ_ATV }).AsQueryable();

            if (Debugger.IsAttached)
            {
                var strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(parametro => strSQL = strSQL.Replace(parametro.ParameterName, string.Format("'{0}'", parametro.Value.ToString())));

                Debugger.Log(0, "", "\r\n");
                Debugger.Log(0, "debug da consulta de ptres no bd", strSQL);
            }

            lstRetorno = new List<PTResEntity>();
            qryConsulta.ToList().
                OrderBy(f=>f.TB_PTRES_CODIGO).ToList().
                ForEach(rowTabela => lstRetorno.Add(new PTResEntity {
                                                                                        Id = rowTabela.TB_PTRES_ID,
                                                                                        Codigo = rowTabela.TB_PTRES_CODIGO,
                                                                                        Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                                                                        CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                                                                        CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                                                                        CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                                                                        AnoDotacao = rowTabela.TB_PTRES_ANO,
                                                                                        CodigoPT = rowTabela.TB_PTRES_PT_CODIGO,
                                                                                        //Ativo = rowTabela.TB_PTRES_STATUS,
                                                                                        ProgramaTrabalho = new ProgramaTrabalho(Int64.Parse(rowTabela.TB_PTRES_PT_CODIGO)),
                                                                                    }));

            return lstRetorno;
        }

        public IList<PTResEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public PTResEntity ObterPTRes(int ptresID)
        {
            PTResEntity objRetorno = (from rowTabela in this.Db.TB_PTREs
                                      where rowTabela.TB_PTRES_ID == ptresID
                                      select new PTResEntity
                                      {
                                          Id = rowTabela.TB_PTRES_ID,
                                          Codigo = rowTabela.TB_PTRES_CODIGO,
                                          Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                          CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                          CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                          CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                          AnoDotacao = rowTabela.TB_PTRES_ANO,
                                          //Ativo = rowTabela.TB_PTRES_STATUS,
                                          ProgramaTrabalho = new ProgramaTrabalho(rowTabela.TB_PTRES_PT_CODIGO),
                                      }).AsQueryable()
                                      .FirstOrDefault();

            return objRetorno;
        }

        public PTResEntity ObterPTRes(int ptresCodigo, int ugeCodigo)
        {
            PTResEntity objRetorno = (from rowTabela in this.Db.TB_PTREs
                                      where rowTabela.TB_PTRES_CODIGO == ptresCodigo
                                      where rowTabela.TB_PTRES_UGE_CODIGO == ugeCodigo
                                      select new PTResEntity
                                      {
                                          Id = rowTabela.TB_PTRES_ID,
                                          Codigo = rowTabela.TB_PTRES_CODIGO,
                                          Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                          CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                          CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                          CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                          AnoDotacao = rowTabela.TB_PTRES_ANO,
                                          //Ativo = rowTabela.TB_PTRES_STATUS,
                                          ProgramaTrabalho = new ProgramaTrabalho(rowTabela.TB_PTRES_PT_CODIGO),
                                      }).AsQueryable()
                                      .FirstOrDefault();

            return objRetorno;
        }

        public IList<PTResEntity> ObterPTRes(int ptresCodigo, int ugeCodigo, int? ptresAcao = null)
        {
            IList<PTResEntity> lstRetorno = null;


            if (ptresAcao == null)
            {
                PTResEntity objPTREs = null;
                objPTREs = this.ObterPTRes(ptresCodigo, ugeCodigo);
                lstRetorno = new List<PTResEntity>();
                lstRetorno.Add(objPTREs);

                //return lstRetorno;
            }
            else
            {


                Expression<Func<TB_PTRE, bool>> expWhere = null;
                IQueryable<TB_PTRE> qryConsulta = null;


                expWhere = (rowTabela => rowTabela.TB_PTRES_CODIGO == ptresCodigo
                                      && rowTabela.TB_PTRES_UGE_CODIGO == ugeCodigo);

                if (ptresAcao.IsNotNull() && ptresAcao.GetValueOrDefault() > 0)
                {
                    string codigoAcaoPTRes = ptresAcao.ToString().PadLeft(4, '0');
                    Expression<Func<TB_PTRE, bool>> expWhereAuxiliar = (rowTabela => rowTabela.TB_PTRES_PT_PROJ_ATV == codigoAcaoPTRes);
                    expWhere = expWhere.And(expWhereAuxiliar);
                }

                if (Debugger.IsAttached)
                {
                    var strSQL = qryConsulta.ToString();
                    Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(parametro => strSQL = strSQL.Replace(parametro.ParameterName, string.Format("'{0}'", parametro.Value.ToString())));

                    Debugger.Log(0, "", "\r\n");
                    Debugger.Log(0, "debug da consulta de ptres no bd", strSQL);
                }

                lstRetorno = new List<PTResEntity>();
                qryConsulta = (from rowTabela in this.Db.TB_PTREs
                               select rowTabela).AsQueryable<TB_PTRE>();

                qryConsulta = qryConsulta.Where(expWhere).AsQueryable();
                qryConsulta.DistinctBy(ptres => new { ptres.TB_PTRES_PT_PROJ_ATV, ptres.TB_PTRES_CODIGO })
                           .OrderBy(ptres => ptres.TB_PTRES_PT_PROJ_ATV)
                           .ToList()
                           .ForEach(rowTabela => lstRetorno.Add(new PTResEntity
                           {
                               Id = rowTabela.TB_PTRES_ID,
                               Codigo = rowTabela.TB_PTRES_CODIGO,
                               Descricao = rowTabela.TB_PTRES_DESCRICAO,

                               CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                               CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                               CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                               AnoDotacao = rowTabela.TB_PTRES_ANO,
                               CodigoPT = rowTabela.TB_PTRES_PT_CODIGO,
                               ProgramaTrabalho = new ProgramaTrabalho(Int64.Parse(rowTabela.TB_PTRES_PT_CODIGO)),
                           }));

            }

            return lstRetorno;
        }

        public IList<PTResEntity> ObterPTResAcao(int ugeCodigo)
        {
            IQueryable<TB_PTRE> qryConsulta = null;
            IList<PTResEntity> lstRetorno = null;

            qryConsulta = (from rowTabela in this.Db.TB_PTREs
                           where rowTabela.TB_PTRES_UGE_CODIGO == ugeCodigo
                           where (rowTabela.TB_PTRES_PT_CODIGO != null || rowTabela.TB_PTRES_PT_CODIGO != string.Empty)
                           //select rowTabela).AsQueryable();
                           //select rowTabela).DistinctBy(ptres => new { ptres.TB_PTRES_UGE_CODIGO, ptres.TB_PTRES_CODIGO }).AsQueryable();
                           select rowTabela).DistinctBy(ptres => new { ptres.TB_PTRES_UGE_CODIGO, ptres.TB_PTRES_CODIGO, ptres.TB_PTRES_PT_PROJ_ATV }).AsQueryable();

            if (Debugger.IsAttached)
            {
                var strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(parametro => strSQL = strSQL.Replace(parametro.ParameterName, string.Format("'{0}'", parametro.Value.ToString())));

                Debugger.Log(0, "", "\r\n");
                Debugger.Log(0, "debug da consulta de ptres no bd", strSQL);
            }

            lstRetorno = new List<PTResEntity>();
            qryConsulta.DistinctBy(ptres => new { ptres.TB_PTRES_PT_PROJ_ATV, ptres.TB_PTRES_CODIGO })
                       .OrderBy(ptres => ptres.TB_PTRES_PT_PROJ_ATV)
                       .ToList()
                       .ForEach(rowTabela => lstRetorno.Add(new PTResEntity
                                                                            {
                                                                                Id = rowTabela.TB_PTRES_ID,
                                                                                Codigo = rowTabela.TB_PTRES_CODIGO,
                                                                                Descricao = rowTabela.TB_PTRES_DESCRICAO,

                                                                                CodigoGestao = rowTabela.TB_PTRES_CODIGO_GESTAO,
                                                                                CodigoUGE = rowTabela.TB_PTRES_UGE_CODIGO,
                                                                                CodigoUO = rowTabela.TB_PTRES_UO_CODIGO,
                                                                                AnoDotacao = rowTabela.TB_PTRES_ANO,
                                                                                CodigoPT = rowTabela.TB_PTRES_PT_CODIGO,
                                                                                //Ativo = rowTabela.TB_PTRES_STATUS,
                                                                                ProgramaTrabalho = new ProgramaTrabalho(Int64.Parse(rowTabela.TB_PTRES_PT_CODIGO)),
                                                                            }));
            return lstRetorno;
        }
    }
}
