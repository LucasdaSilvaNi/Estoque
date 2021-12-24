using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Common.Util;



namespace Sam.Domain.Infrastructure
{
    public partial class TipoMovimentoInfrastructure : BaseInfraestructure, ITipoMovimentoService
    {

        private TipoMovimentoEntity TipoMovimento = new TipoMovimentoEntity();

        public IList<TipoMovimentoEntity> Listar(int TipoMovimentoId)
        {
            throw new NotImplementedException();
        }

        public IList<TipoMovimentoEntity> ListarTodosCod(int TipoMovimentoId)
        {
            throw new NotImplementedException();
        }

        public TipoMovimentoEntity LerRegistro(int MovimentoId)
        {
            throw new NotImplementedException();
        }


        public TipoMovimentoEntity Entity
        {
            get { return TipoMovimento; }
            set { TipoMovimento = value; }
        }

        public IList<TipoMovimentoEntity> Listar()
        {
            IList<TipoMovimentoEntity> lstRetorno = null;
            IQueryable<TB_TIPO_MOVIMENTO> qryConsulta = null;

            qryConsulta = Db.TB_TIPO_MOVIMENTOs.AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOTipoMovimento())
                                    .ToList();

            return lstRetorno;
        }

        public IList<TipoMovimentoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public TipoMovimentoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<TipoMovimentoEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            var tbTIpoMovimento = Db.TB_TIPO_MOVIMENTOs.Where(a => a.TB_TIPO_MOVIMENTO_ID == Entity.Id).FirstOrDefault();

            tbTIpoMovimento.TB_TIPO_MOVIMENTO_ATIVO = Entity.Ativo;

            Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }

        public IList<TipoMovimentoEntity> ListarTipoMovimento(TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento)
        {
            if (tipoMovimentoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada || 
                tipoMovimentoAgrupamento.Id == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato)
            {
                IList<TipoMovimentoEntity> resultado = (from a in Db.TB_TIPO_MOVIMENTOs
                                                        where (a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoMovimentoAgrupamento.Id)
                                                        select new TipoMovimentoEntity
                                                        {
                                                            Id = a.TB_TIPO_MOVIMENTO_ID,
                                                            Codigo = a.TB_TIPO_MOVIMENTO_CODIGO,
                                                            Descricao = a.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                            AgrupamentoId = a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                            Ativo = a.TB_TIPO_MOVIMENTO_ATIVO,
                                                            SubTipoMovimento = (from item in this.Db.TB_SUBTIPO_MOVIMENTOs
                                                                                where a.TB_TIPO_MOVIMENTO_ID == item.TB_TIPO_MOVIMENTO_ID
                                                                                && item.TB_SUBTIPO_MOVIMENTO_ATIVO == true
                                                                                select new SubTipoMovimentoEntity()
                                                                                {
                                                                                    Id = item.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                    Ativo = item.TB_SUBTIPO_MOVIMENTO_ATIVO,
                                                                                    Descricao = item.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                }).ToList()
                                                        })
                                                   .ToList();

                return resultado;
            }
            else
            {
                IList<TipoMovimentoEntity> resultado = (from a in Db.TB_TIPO_MOVIMENTOs
                                                        where (a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == tipoMovimentoAgrupamento.Id)
                                                        || (a.TB_TIPO_MOVIMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimento.RequisicaoPendente)
                                                        select new TipoMovimentoEntity
                                                        {
                                                            Id = a.TB_TIPO_MOVIMENTO_ID,
                                                            Codigo = a.TB_TIPO_MOVIMENTO_CODIGO,
                                                            Descricao = a.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                            AgrupamentoId = a.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                            Ativo = a.TB_TIPO_MOVIMENTO_ATIVO,
                                                            SubTipoMovimento = (from item in this.Db.TB_SUBTIPO_MOVIMENTOs
                                                                                where a.TB_TIPO_MOVIMENTO_ID == item.TB_TIPO_MOVIMENTO_ID
                                                                                && item.TB_SUBTIPO_MOVIMENTO_ATIVO == true
                                                                                select new SubTipoMovimentoEntity()
                                                                                {
                                                                                    Id = item.TB_SUBTIPO_MOVIMENTO_ID,
                                                                                    Ativo = item.TB_SUBTIPO_MOVIMENTO_ATIVO,
                                                                                    Descricao = item.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                                                }).ToList()
                                                        })
                                                        .ToList();

                return resultado;
            }
        }

        public IList<TipoMovimentoEntity> ListarTipoMovimentoTodosEntrada()
        {
            IList<TipoMovimentoEntity> resultado = (from a in Db.TB_TIPO_MOVIMENTOs
                                                    where a.TB_TIPO_MOVIMENTO_CODIGO.ToString().Substring(0,1) == "1"
                                                    select new TipoMovimentoEntity
                                                    {
                                                        Id = a.TB_TIPO_MOVIMENTO_ID,
                                                        Codigo = a.TB_TIPO_MOVIMENTO_CODIGO,
                                                        Descricao = a.TB_TIPO_MOVIMENTO_DESCRICAO
                                                    })
                                                    .ToList();

            return resultado;
        }

        public List<TipoMovimentoEntity> ListarTipoMovimentoAtivoNl()
        {
            List<TipoMovimentoEntity> resultado = (from a in Db.TB_TIPO_MOVIMENTOs
                                                   where a.TB_TIPO_MOVIMENTO_ATIVO_NL == true
                                                   select new TipoMovimentoEntity
                                                   {
                                                       Id = a.TB_TIPO_MOVIMENTO_ID,
                                                       Codigo = a.TB_TIPO_MOVIMENTO_CODIGO,
                                                       Descricao = a.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                       CodigoFormatado = a.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO + " - " + a.TB_TIPO_MOVIMENTO_DESCRICAO
                                                    })
                                                    .ToList();

            return resultado;
        }

        public IList<TipoMovimentoEntity> ListarTipoMovimentoTodosSaida()
        {
            IList<TipoMovimentoEntity> resultado = (from a in Db.TB_TIPO_MOVIMENTOs
                                                    select new TipoMovimentoEntity
                                                    {
                                                        Id = a.TB_TIPO_MOVIMENTO_ID,
                                                        Codigo = a.TB_TIPO_MOVIMENTO_CODIGO,
                                                        Descricao = a.TB_TIPO_MOVIMENTO_DESCRICAO
                                                    })
                                                    .ToList();

            return resultado;
        }

        public TipoMovimentoEntity ListarTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID)
        {
            TipoMovimentoEntity lObjRetorno = (from TipoMovimento in Db.TB_TIPO_MOVIMENTOs
                                               where TipoMovimento.TB_TIPO_MOVIMENTO_ID == iTipoMovimentoEntrada_ID
                                               orderby TipoMovimento.TB_TIPO_MOVIMENTO_ID ascending 
                                               select new TipoMovimentoEntity
                                               {
                                                   Id = TipoMovimento.TB_TIPO_MOVIMENTO_ID,
                                                   Codigo = TipoMovimento.TB_TIPO_MOVIMENTO_CODIGO,
                                                   Descricao = TipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                   SubTipoMovimento = (from item in this.Db.TB_SUBTIPO_MOVIMENTOs
                                                                       where TipoMovimento.TB_TIPO_MOVIMENTO_ID == item.TB_TIPO_MOVIMENTO_ID
                                                                       && item.TB_SUBTIPO_MOVIMENTO_ATIVO == true
                                                                       select new SubTipoMovimentoEntity()
                                                                       {
                                                                           Id = item.TB_SUBTIPO_MOVIMENTO_ID,
                                                                           Ativo = item.TB_SUBTIPO_MOVIMENTO_ATIVO,
                                                                           Descricao = item.TB_SUBTIPO_MOVIMENTO_DESCRICAO,
                                                                           ListEventoSiafem = (from evento in this.Db.TB_EVENTO_SIAFEMs
                                                                                               where item.TB_SUBTIPO_MOVIMENTO_ID == evento.TB_SUBTIPO_MOVIMENTO_ID
                                                                                               && evento.TB_EVENTO_SIAFEM_ATIVO == true
                                                                                               select new EventoSiafemEntity()
                                                                                               {
                                                                                                   Id = evento.TB_EVENTO_SIAFEM_ID,
                                                                                                   DetalheAtivo = evento.TB_EVENTO_SIAFEM_DETALHE_ATIVO,
                                                                                                   EventoTipoMaterial = evento.TB_EVENTO_TIPO_MATERIAL
                                                                                               }).ToList()
                                                                          
                                                                      }).ToList()
                                               }).FirstOrDefault();

            return lObjRetorno;
        }


        public List<SubTipoMovimentoEntity> ListarSubTipoMovimento()
        {
            List<SubTipoMovimentoEntity> lObjRetorno = (from subTipoMovimento in Db.TB_SUBTIPO_MOVIMENTOs
                                                        where subTipoMovimento.TB_SUBTIPO_MOVIMENTO_ATIVO == true
                                                        select new SubTipoMovimentoEntity()
                                                        {
                                                            Id = subTipoMovimento.TB_SUBTIPO_MOVIMENTO_ID,
                                                            Ativo = subTipoMovimento.TB_SUBTIPO_MOVIMENTO_ATIVO,
                                                            Descricao = subTipoMovimento.TB_SUBTIPO_MOVIMENTO_DESCRICAO
                                                        }).ToList();

            return lObjRetorno;
        }


        public SubTipoMovimentoEntity ListarInserirSubTipoMovimento(SubTipoMovimentoEntity objSubTipo)
        {

            TB_SUBTIPO_MOVIMENTO rowTabela = new TB_SUBTIPO_MOVIMENTO();

            
        

            var subTipo = this.Db.TB_SUBTIPO_MOVIMENTOs.Where(a => a.TB_SUBTIPO_MOVIMENTO_DESCRICAO == objSubTipo.Descricao && a.TB_TIPO_MOVIMENTO_ID == objSubTipo.TipoMovimentoId).FirstOrDefault();

            if (subTipo == null)
            {
                this.Db.TB_SUBTIPO_MOVIMENTOs.InsertOnSubmit(rowTabela);

                rowTabela.TB_SUBTIPO_MOVIMENTO_ATIVO = true;
                rowTabela.TB_SUBTIPO_MOVIMENTO_DESCRICAO = objSubTipo.Descricao;
                rowTabela.TB_TIPO_MOVIMENTO_ID = Convert.ToInt32(objSubTipo.TipoMovimentoId);
            }
            else
            {
                rowTabela = subTipo;
                rowTabela.TB_SUBTIPO_MOVIMENTO_ATIVO = true;                
            }


            this.Db.SubmitChanges();

            //Retorna o objeto que inseriu atualizado
            objSubTipo = new SubTipoMovimentoEntity();
            objSubTipo.Id = rowTabela.TB_SUBTIPO_MOVIMENTO_ID;

            return objSubTipo;
        }



        public IList<TipoMovimentoEntity> RetirarTipoMovimentoEntrada(int iTipoMovimentoEntrada_ID)
        {
            IList<TipoMovimentoEntity> lstRetorno = (from TipoMovimento in Db.TB_TIPO_MOVIMENTOs
                                                     where TipoMovimento.TB_TIPO_MOVIMENTO_ID != iTipoMovimentoEntrada_ID
                                                     select new TipoMovimentoEntity
                                                     {
                                                         Id = TipoMovimento.TB_TIPO_MOVIMENTO_ID,
                                                         Codigo = TipoMovimento.TB_TIPO_MOVIMENTO_CODIGO,
                                                         Descricao = TipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                         AgrupamentoId = TipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                     }).ToList();

            return lstRetorno;
        }

        private Func<TB_TIPO_MOVIMENTO, TipoMovimentoEntity> _instanciadorDTOTipoMovimento()
        {
            Func<TB_TIPO_MOVIMENTO, TipoMovimentoEntity> _actionSeletor = null;

            _actionSeletor = (tipoMovimento => new TipoMovimentoEntity()
            {
                Id = tipoMovimento.TB_TIPO_MOVIMENTO_ID,
                Codigo = tipoMovimento.TB_TIPO_MOVIMENTO_CODIGO,
                Descricao = tipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO,

                CodigoDescricao = string.Format("{0} - {1}", tipoMovimento.TB_TIPO_MOVIMENTO_CODIGO, tipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO),
                TipoAgrupamento = (tipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO.IsNotNull()? (new TipoMovimentoAgrupamentoEntity() { 
                                                                                                                                     Id = tipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID, 
                                                                                                                                     Descricao = tipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                                                                                                                                  } ) : null)
                

            });

            return _actionSeletor;
        }

        public TipoMovimentoEntity Recupera(int Id)
        {
            TipoMovimentoEntity tipoMov = (from tipoMovimento in Db.TB_TIPO_MOVIMENTOs
                                           where tipoMovimento.TB_TIPO_MOVIMENTO_ID == Id
                                           select new TipoMovimentoEntity()
                                           {
                                                Id = tipoMovimento.TB_TIPO_MOVIMENTO_ID,
                                                Codigo = tipoMovimento.TB_TIPO_MOVIMENTO_CODIGO,
                                                Descricao = tipoMovimento.TB_TIPO_MOVIMENTO_DESCRICAO,
                                                Aux_ID = tipoMovimento.TB_TIPO_MOVIMENTO_AUX_ID,
                                                AgrupamentoId = tipoMovimento.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID,
                                                AtivoNL = tipoMovimento.TB_TIPO_MOVIMENTO_ATIVO_NL,
                                                Ativo = tipoMovimento.TB_TIPO_MOVIMENTO_ATIVO
                                           }).FirstOrDefault();

            return tipoMov;
        }
    }
}

