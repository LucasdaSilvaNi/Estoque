using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Common.LambdaExpression;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Transactions;
using TipoMovimento = Sam.Common.Util.GeralEnum.TipoMovimento;



namespace Sam.Domain.Infrastructure
{
    public partial class EventosPagamentoInfrastructure : BaseInfraestructure, IEventosPagamentoService
    {
        private EventosPagamentoEntity empenhoEvento = new EventosPagamentoEntity();
        public EventosPagamentoEntity Entity
        {
            get { return empenhoEvento; }
            set { empenhoEvento = value; }
        }

        public EventosPagamentoEntity ObterEventoEmpenho(int codigoEvento)
        {
            EventosPagamentoEntity objEntidade = null;
            IQueryable<TB_EVENTOS_PAGAMENTO> qryConsulta = null;
            Expression<Func<TB_EVENTOS_PAGAMENTO, bool>> expWhere;

            //expWhere = (empenhoEvento => empenhoEvento.TB_EVENTOS_PAGAMENTO_CODIGO == codigoEvento);
            expWhere = (empenhoEvento => ((empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO == codigoEvento) || (empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO == codigoEvento)));

            qryConsulta = Db.TB_EVENTOS_PAGAMENTOs.Where(expWhere)
                                               .AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                     .FirstOrDefault();

            return objEntidade;
        }
        public EventosPagamentoEntity ObterEventoPagamento(MovimentoEntity objMovimentacao, bool retornaApenasSeAtivo = false)
        {
            EventosPagamentoEntity objEntidade = null;
            Expression<Func<TB_EVENTOS_PAGAMENTO, bool>> expWhere;
            Expression<Func<TB_EVENTOS_PAGAMENTO, bool>> expWhereSeAtivo;
            IQueryable<TB_EVENTOS_PAGAMENTO> qryConsulta = null;
            int tipoMovimentoID = 0;
            int tipoMaterialID = 0;
            string anoMesRef = null;



            anoMesRef = objMovimentacao.Almoxarifado.MesRef.Substring(0, 4);
            tipoMovimentoID = objMovimentacao.TipoMovimento.Id;
            tipoMaterialID = (int)objMovimentacao.TipoMaterial;

            expWhere = (empenhoEvento => ((empenhoEvento.TB_TIPO_MOVIMENTO_ID == tipoMovimentoID)
                                      &&  (empenhoEvento.TB_EVENTOS_PAGAMENTO_TIPO_MATERIAL == tipoMaterialID))
                                      &&  (empenhoEvento.TB_EVENTOS_PAGAMENTO_ANO_BASE == anoMesRef)); //Evento deverá ser do ano do MesReferencia do almox lancador

            if (retornaApenasSeAtivo)
            {
                expWhereSeAtivo = (empenhoEvento => (empenhoEvento.TB_EVENTOS_PAGAMENTO_ATIVO == retornaApenasSeAtivo));
                expWhere = expWhere.And(expWhereSeAtivo);
            }


            qryConsulta = Db.TB_EVENTOS_PAGAMENTOs.Where(expWhere)
                                                  .AsQueryable();

            #region debug SQL
            if (Constante.isSamWebDebugged)
            {
                string _strSQL = qryConsulta.ToString();
                Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            }
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                     .FirstOrDefault();

            return objEntidade;
        }



        public EventoSiafemEntity ObterEventoSiafem(MovimentoEntity objMovimentacao)
        {
            IQueryable<EventoSiafemEntity> qryConsulta = (from a in this.Db.TB_EVENTO_SIAFEMs
                                                          join b in this.Db.TB_SUBTIPO_MOVIMENTOs on a.TB_SUBTIPO_MOVIMENTO_ID equals b.TB_SUBTIPO_MOVIMENTO_ID
                                                          where (b.TB_SUBTIPO_MOVIMENTO_ATIVO == true)
                                                          where (a.TB_SUBTIPO_MOVIMENTO_ID == objMovimentacao.SubTipoMovimentoId)
                                                          where (a.TB_EVENTO_TIPO_MATERIAL == objMovimentacao.TipoMaterial.ToString())
                                                          where (a.TB_EVENTO_SIAFEM_ATIVO == true)
                                                          select new EventoSiafemEntity
                                                          {
                                                              Id = a.TB_EVENTO_SIAFEM_ID,
                                                              EventoTipoMaterial = a.TB_EVENTO_TIPO_MATERIAL,
                                                              EventoTipoEntradaSaidaReclassificacaoDepreciacao = a.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO,
                                                              EventoTipoEstoque = a.TB_EVENTO_TIPO_ESTOQUE,
                                                              EventoEstoque = a.TB_EVENTO_ESTOQUE,
                                                              EventoTipoMovimentacao = a.TB_EVENTO_TIPO_MOVIMENTACAO,
                                                              EventoTipoMovimento = a.TB_EVENTO_TIPO_MOVIMENTO,
                                                              Ativo = a.TB_EVENTO_SIAFEM_ATIVO,
                                                              DetalheAtivo = a.TB_EVENTO_SIAFEM_DETALHE_ATIVO,
                                                              SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,
                                                              EstimuloAtivo = a.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO

                                                          }).AsQueryable();

            IList<EventoSiafemEntity> resultado;

            resultado = qryConsulta.ToList<EventoSiafemEntity>();



            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            return resultado.FirstOrDefault();
        }

        public bool InativarItemEventoSiafem(int Id, string usuario)
        {
            bool retorno = false;
            try
            {
                TB_EVENTO_SIAFEM rowTabela = this.Db.TB_EVENTO_SIAFEMs.Where(empenhoEvento => empenhoEvento.TB_EVENTO_SIAFEM_ID == Id).FirstOrDefault();

                if (rowTabela.IsNotNull())
                {
                    rowTabela.TB_EVENTO_SIAFEM_ATIVO = false;
                    rowTabela.DATA_ALTERACAO = DateTime.Now;
                    rowTabela.LOGIN_ALTERACAO = usuario;
                    this.Db.SubmitChanges();

                    TB_EVENTO_SIAFEM rowTabelax = this.Db.TB_EVENTO_SIAFEMs.Where(empenhoEvento => (empenhoEvento.TB_SUBTIPO_MOVIMENTO_ID == rowTabela.TB_SUBTIPO_MOVIMENTO_ID)
                                                && (rowTabela.TB_EVENTO_SIAFEM_ATIVO == true)).FirstOrDefault();

                    if (rowTabelax.IsNull())
                    {
                        TB_SUBTIPO_MOVIMENTO sub = this.Db.TB_SUBTIPO_MOVIMENTOs.Where(subItem => (subItem.TB_SUBTIPO_MOVIMENTO_ID == rowTabela.TB_SUBTIPO_MOVIMENTO_ID)
                                                && (subItem.TB_SUBTIPO_MOVIMENTO_ATIVO == true)).FirstOrDefault();

                        if (sub.IsNotNull())
                        {
                            sub.TB_SUBTIPO_MOVIMENTO_ATIVO = false;
                            this.Db.SubmitChanges();
                        }

                    }
                    retorno = true;
                }
            }
            catch (Exception e)
            {
                retorno = false;
            }

            return retorno;


        }

        public bool AlterarItemEventoSiafem(int Id, string usuario, string txt1, string txt2, int subtipo, int subtipoOld, bool estimulo)
        {
            bool retorno = false;
            try
            {
                TB_EVENTO_SIAFEM rowTabela = this.Db.TB_EVENTO_SIAFEMs.Where(empenhoEvento => empenhoEvento.TB_EVENTO_SIAFEM_ID == Id).FirstOrDefault();

                if (rowTabela.IsNotNull())
                {
                    rowTabela.DATA_ALTERACAO = DateTime.Now;
                    rowTabela.LOGIN_ALTERACAO = usuario;
                    rowTabela.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO = txt1;
                    rowTabela.TB_EVENTO_TIPO_MOVIMENTACAO = txt2;
                    rowTabela.TB_SUBTIPO_MOVIMENTO_ID = subtipo;
                    rowTabela.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO = estimulo;

                    this.Db.SubmitChanges();

                    TB_EVENTO_SIAFEM rowTabelax = this.Db.TB_EVENTO_SIAFEMs.Where(empenhoEvento => (empenhoEvento.TB_SUBTIPO_MOVIMENTO_ID == subtipoOld)
                                                && (rowTabela.TB_EVENTO_SIAFEM_ATIVO == true)).FirstOrDefault();

                    if (rowTabelax.IsNull())
                    {
                        TB_SUBTIPO_MOVIMENTO sub = this.Db.TB_SUBTIPO_MOVIMENTOs.Where(subItem => (subItem.TB_SUBTIPO_MOVIMENTO_ID == subtipoOld)
                                                && (subItem.TB_SUBTIPO_MOVIMENTO_ATIVO == true)).FirstOrDefault();

                        if (sub.IsNotNull())
                        {
                            sub.TB_SUBTIPO_MOVIMENTO_ATIVO = false;
                            this.Db.SubmitChanges();
                        }

                    }
                    retorno = true;
                }
            }
            catch (Exception e)
            {
                retorno = false;
            }

            return retorno;


        }

        public EventoSiafemEntity SalvarSiafem(EventoSiafemEntity objSiafem)
        {

            IQueryable<EventoSiafemEntity> qryConsulta = (from a in this.Db.TB_EVENTO_SIAFEMs
                                                          where (a.TB_SUBTIPO_MOVIMENTO_ID == objSiafem.SubTipoMovimentoId)
                                                          where (a.TB_EVENTO_TIPO_MATERIAL == objSiafem.EventoTipoMaterial)
                                                          where (a.TB_EVENTO_SIAFEM_ATIVO == true)
                                                          select new EventoSiafemEntity
                                                          {
                                                              Id = a.TB_EVENTO_SIAFEM_ID,
                                                              EventoTipoMaterial = a.TB_EVENTO_TIPO_MATERIAL,
                                                              EventoTipoEntradaSaidaReclassificacaoDepreciacao = a.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO,
                                                              EventoTipoEstoque = a.TB_EVENTO_TIPO_ESTOQUE,
                                                              EventoEstoque = a.TB_EVENTO_ESTOQUE,
                                                              EventoTipoMovimentacao = a.TB_EVENTO_TIPO_MOVIMENTACAO,
                                                              EventoTipoMovimento = a.TB_EVENTO_TIPO_MOVIMENTO,
                                                              Ativo = a.TB_EVENTO_SIAFEM_ATIVO,
                                                              SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,
                                                              DetalheAtivo = a.TB_EVENTO_SIAFEM_DETALHE_ATIVO,
                                                              EstimuloAtivo = a.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO

                                                          }).AsQueryable();




            IList<EventoSiafemEntity> resultado;

            resultado = qryConsulta.ToList<EventoSiafemEntity>();



            //string _strSQL = qryConsulta.ToString();
            //Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));



            if (resultado.Count == 0)
            {
                TB_EVENTO_SIAFEM rowTabela = new TB_EVENTO_SIAFEM();

                this.Db.TB_EVENTO_SIAFEMs.InsertOnSubmit(rowTabela);

                rowTabela.DATA_INCLUSAO = objSiafem.DataInclusao;
                rowTabela.TB_EVENTO_ESTOQUE = objSiafem.EventoEstoque;
                rowTabela.TB_EVENTO_TIPO_MATERIAL = objSiafem.EventoTipoMaterial;
                rowTabela.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO = objSiafem.EventoTipoEntradaSaidaReclassificacaoDepreciacao;
                rowTabela.TB_EVENTO_TIPO_ESTOQUE = objSiafem.EventoTipoEstoque;
                rowTabela.TB_EVENTO_TIPO_MOVIMENTACAO = objSiafem.EventoTipoMovimentacao;
                rowTabela.TB_EVENTO_TIPO_MOVIMENTO = objSiafem.EventoTipoMovimento;
                rowTabela.TB_EVENTO_SIAFEM_ATIVO = objSiafem.Ativo;
                rowTabela.TB_SUBTIPO_MOVIMENTO_ID = objSiafem.SubTipoMovimentoId;
                rowTabela.LOGIN_ATIVACAO = objSiafem.LoginAtivacao;
                rowTabela.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO = objSiafem.EstimuloAtivo;

                this.Db.SubmitChanges();

                //Retorna o objeto que inseriu atualizado
                objSiafem = new EventoSiafemEntity();
                objSiafem.Id = rowTabela.TB_EVENTO_SIAFEM_ID;
                objSiafem.inseriu = true;
            }
            else
            {


                objSiafem = new EventoSiafemEntity();
                objSiafem = resultado.FirstOrDefault();
                objSiafem.inseriu = false;
            }


            return objSiafem;
        }


        public List<EventoSiafemEntity> ListarItem(int _subTipoMovId)
        {
            //IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
            IQueryable<EventoSiafemEntity> qryConsulta = (from a in this.Db.TB_EVENTO_SIAFEMs
                                                          join b in this.Db.TB_SUBTIPO_MOVIMENTOs on a.TB_SUBTIPO_MOVIMENTO_ID equals b.TB_SUBTIPO_MOVIMENTO_ID
                                                          where (b.TB_SUBTIPO_MOVIMENTO_ATIVO == true)
                                                          where (a.TB_SUBTIPO_MOVIMENTO_ID == _subTipoMovId)
                                                          where (a.TB_EVENTO_SIAFEM_ATIVO == true)
                                                          select new EventoSiafemEntity
                                                          {
                                                              Id = a.TB_EVENTO_SIAFEM_ID,
                                                              EventoTipoMaterial = a.TB_EVENTO_TIPO_MATERIAL,
                                                              EventoTipoEntradaSaidaReclassificacaoDepreciacao = a.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO,
                                                              EventoTipoEstoque = a.TB_EVENTO_TIPO_ESTOQUE,
                                                              EventoEstoque = a.TB_EVENTO_ESTOQUE,
                                                              EventoTipoMovimentacao = a.TB_EVENTO_TIPO_MOVIMENTACAO,
                                                              EventoTipoMovimento = a.TB_EVENTO_TIPO_MOVIMENTO,
                                                              Ativo = a.TB_EVENTO_SIAFEM_ATIVO,
                                                              SubTipoMovimentoId = a.TB_SUBTIPO_MOVIMENTO_ID,
                                                              EstimuloAtivo = a.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO

                                                          }).AsQueryable();

            IList<EventoSiafemEntity> resultado;

            resultado = qryConsulta.Skip(this.SkipRegistros)
                                                                 .Take(this.RegistrosPagina)
                                                                 .ToList<EventoSiafemEntity>();



            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            this.totalregistros = qryConsulta.Count();

            return resultado.ToList();

        }
        public EventosPagamentoEntity ObterEventoPagamento(Enum tipoMovimento)
        {
            EventosPagamentoEntity objEntidade = null;
            IQueryable<TB_EVENTOS_PAGAMENTO> qryConsulta = null;
            Expression<Func<TB_EVENTOS_PAGAMENTO, bool>> expWhere;


            expWhere = (eventoPagamento => eventoPagamento.TB_TIPO_MOVIMENTO_ID == (int)((TipoMovimento)tipoMovimento));
            qryConsulta = Db.TB_EVENTOS_PAGAMENTOs.Where(expWhere);

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                     .FirstOrDefault();

            return objEntidade;
        }
        public IList<EventosPagamentoEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public IList<EventosPagamentoEntity> Listar()
        {
            IList<EventosPagamentoEntity> lstRetorno = null;
            IQueryable<TB_EVENTOS_PAGAMENTO> qryConsulta = null;

            qryConsulta = Db.TB_EVENTOS_PAGAMENTOs.AsQueryable();

            //#region debug SQL
            //var _strSQL = qryConsulta.ToString();
            //Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            //#endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                    .ToList();

            return lstRetorno;
        }

        public IList<EventosPagamentoEntity> Listar(string Ano)
        {
            IList<EventosPagamentoEntity> lstRetorno = Listar();

            return (from _lista in lstRetorno
                   where _lista.AnoBase == Ano
                   select _lista).DefaultIfEmpty().ToList();
        }

        public IList<string> ListarAnoEvento()
        {
            IList<EventosPagamentoEntity> lstRetorno = Listar();

            return (from _lista in lstRetorno
                    orderby _lista.AnoBase descending
                    select _lista.AnoBase).DefaultIfEmpty().Distinct().ToList();
        }

        public EventosPagamentoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<EventosPagamentoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        void ICrudBaseService<EventosPagamentoEntity>.Excluir()
        {
            throw new NotImplementedException();
        }

        public bool Excluir()
        {
            bool blnRetorno = false;
            TB_EVENTOS_PAGAMENTO rowTabela = this.Db.TB_EVENTOS_PAGAMENTOs.Where(empenhoEvento => empenhoEvento.TB_EVENTOS_PAGAMENTO_ID == this.Entity.Id).FirstOrDefault();

            if (rowTabela.IsNotNull())
            {
                this.Db.TB_EVENTOS_PAGAMENTOs.DeleteOnSubmit(rowTabela);
                this.Db.SubmitChanges();
            }

            blnRetorno = this.Db.TB_EVENTOS_PAGAMENTOs.Where(empenhoEvento => empenhoEvento.TB_EVENTOS_PAGAMENTO_ID == this.Entity.Id).FirstOrDefault().IsNull();

            return blnRetorno;
        }

        public void Salvar()
        {
            TB_EVENTOS_PAGAMENTO rowTabela = new TB_EVENTOS_PAGAMENTO();

            if (this.Entity.Id.HasValue && this.Entity.Id.Value != 0)
                rowTabela = this.Db.TB_EVENTOS_PAGAMENTOs.Where(a => a.TB_EVENTOS_PAGAMENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_EVENTOS_PAGAMENTOs.InsertOnSubmit(rowTabela);



            rowTabela.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO         = this.Entity.PrimeiroCodigo;
            rowTabela.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO_ESTORNO = this.Entity.PrimeiroCodigoEstorno;
            rowTabela.TB_EVENTOS_PAGAMENTO_PRIMEIRA_INSCRICAO      = this.Entity.PrimeiraInscricao;
            rowTabela.TB_EVENTOS_PAGAMENTO_PRIMEIRA_CLASSIFICACAO  = this.Entity.PrimeiraClassificacao;
                 
            rowTabela.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO          = this.Entity.SegundoCodigo;
            rowTabela.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO_ESTORNO  = this.Entity.SegundoCodigoEstorno;
            rowTabela.TB_EVENTOS_PAGAMENTO_SEGUNDA_INSCRICAO       = this.Entity.SegundaInscricao;
            rowTabela.TB_EVENTOS_PAGAMENTO_SEGUNDA_CLASSIFICACAO   = this.Entity.SegundaClassificacao;

            rowTabela.TB_EVENTOS_PAGAMENTO_ANO_BASE                = this.Entity.AnoBase;
            rowTabela.TB_EVENTOS_PAGAMENTO_ATIVO                   = this.Entity.Ativo;
            rowTabela.TB_EVENTOS_PAGAMENTO_UG_FAVORECIDA           = this.Entity.UGFavorecida;

            rowTabela.TB_TIPO_MOVIMENTO_ID                         = (this.Entity.TipoMovimentoAssociado.IsNotNull() ? this.Entity.TipoMovimentoAssociado.Id : (int?)null);
			rowTabela.TB_EVENTOS_PAGAMENTO_TIPO_MATERIAL           = this.Entity.TipoMaterialAssociado;

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        //Manter retor-compatibilidade depois
        //public bool ExisteCodigoInformado()
        //{
        //    return (this.Db.TB_EMPENHO_EVENTOs.Where(empenhoEvento => (empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO == this.Entity.PrimeiroCodigo) || (empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO == this.Entity.SegundoCodigo))
        //                                      .CountReadUncommitted() == 1);
        //}
        public bool ExisteCodigoInformado() { throw new NotImplementedException();  }

        private Func<TB_EVENTOS_PAGAMENTO, EventosPagamentoEntity> _instanciadorDTOEmpenhoEvento()
        {
            Func<TB_EVENTOS_PAGAMENTO, EventosPagamentoEntity> _actionSeletor = null;

            _actionSeletor = (empenhoEvento => new EventosPagamentoEntity()
            {
                Id = empenhoEvento.TB_EVENTOS_PAGAMENTO_ID,
                PrimeiroCodigo = empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO,
                PrimeiroCodigoEstorno = empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRO_CODIGO_ESTORNO,
                PrimeiraInscricao = empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRA_INSCRICAO,
                PrimeiraClassificacao = empenhoEvento.TB_EVENTOS_PAGAMENTO_PRIMEIRA_CLASSIFICACAO,

                SegundoCodigo = empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO,
                SegundoCodigoEstorno = empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDO_CODIGO_ESTORNO,
                SegundaInscricao = empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDA_INSCRICAO,
                SegundaClassificacao = empenhoEvento.TB_EVENTOS_PAGAMENTO_SEGUNDA_CLASSIFICACAO,


                AnoBase = empenhoEvento.TB_EVENTOS_PAGAMENTO_ANO_BASE,
                //Ativo                   = (empenhoEvento.TB_EVENTOS_PAGAMENTO_ATIVO.IsNull() ? false : true),
                Ativo = (empenhoEvento.TB_EVENTOS_PAGAMENTO_ATIVO.IsNull() ? false : empenhoEvento.TB_EVENTOS_PAGAMENTO_ATIVO),
                UGFavorecida = empenhoEvento.TB_EVENTOS_PAGAMENTO_UG_FAVORECIDA,

                TipoMovimentoAssociado = (empenhoEvento.TB_TIPO_MOVIMENTO.IsNotNull() ? (new TipoMovimentoEntity() { Id = empenhoEvento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID, Descricao = empenhoEvento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO }) : null),
                TipoMaterialAssociado = empenhoEvento.TB_EVENTOS_PAGAMENTO_TIPO_MATERIAL
            });

            return _actionSeletor;
        }


        public List<EventoSiafemEntity> ListarPatrimonial()
        {
            List<EventoSiafemEntity> lstRetorno = null;
            IQueryable<TB_EVENTO_SIAFEM> qryConsulta = null;

            qryConsulta = Db.TB_EVENTO_SIAFEMs.Where(a => a.TB_EVENTO_SIAFEM_ATIVO == true).AsQueryable();

            //#region debug SQL
            //var _strSQL = qryConsulta.ToString();
            //Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            //#endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOEventoPatrimonial())
                                    .ToList();

            return lstRetorno;
        }

        private Func<TB_EVENTO_SIAFEM, EventoSiafemEntity> _instanciadorDTOEventoPatrimonial()
        {
            Func<TB_EVENTO_SIAFEM, EventoSiafemEntity> _actionSeletor = null;

            _actionSeletor = (Evento => new EventoSiafemEntity()
            {
                Id = Evento.TB_EVENTO_SIAFEM_ID,
                SubTipoMovimento = (Evento.TB_SUBTIPO_MOVIMENTO.IsNotNull() ? (new SubTipoMovimentoEntity()
                {
                    Id = Evento.TB_SUBTIPO_MOVIMENTO.TB_SUBTIPO_MOVIMENTO_ID,
                    Descricao = Evento.TB_SUBTIPO_MOVIMENTO.TB_SUBTIPO_MOVIMENTO_DESCRICAO,
                    TipoMovimento = (new TipoMovimentoEntity()
                    {
                        Id = Evento.TB_SUBTIPO_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                        Descricao = Evento.TB_SUBTIPO_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                        CodigoFormatado = Evento.TB_SUBTIPO_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_DESCRICAO
                        + " - " + Evento.TB_SUBTIPO_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO

                    })

                }) : null),
                EventoTipoMaterial = Evento.TB_EVENTO_TIPO_MATERIAL,
                EventoTipoEntradaSaidaReclassificacaoDepreciacao = Evento.TB_EVENTO_TIPO_ENTRADA_SAIDA_RECLASSIFICACAO_DEPRECIACAO,
                EventoTipoEstoque = Evento.TB_EVENTO_TIPO_ESTOQUE,
                EventoEstoque = Evento.TB_EVENTO_ESTOQUE,
                EventoTipoMovimentacao = Evento.TB_EVENTO_TIPO_MOVIMENTACAO,
                EventoTipoMovimento = Evento.TB_EVENTO_TIPO_MOVIMENTO,
                EstimuloAtivo = Evento.TB_EVENTO_SIAFEM_ESTIMULO_ATIVO

            });

            return _actionSeletor;
        }
    }
}
