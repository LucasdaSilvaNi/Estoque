using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;




namespace Sam.Domain.Infrastructure
{
    public partial class EmpenhoEventoInfrastructure : BaseInfraestructure, IEmpenhoEventoService
    {
        private EmpenhoEventoEntity empenhoEvento = new EmpenhoEventoEntity();
        public EmpenhoEventoEntity Entity
        {
            get { return empenhoEvento; }
            set { empenhoEvento = value; }
        }

        public EmpenhoEventoEntity ObterEventoEmpenho(int codigoEvento)
        {
            EmpenhoEventoEntity objEntidade = null;
            IQueryable<TB_EMPENHO_EVENTO> qryConsulta = null;
            Expression<Func<TB_EMPENHO_EVENTO, bool>> expWhere;

            expWhere = (empenhoEvento => empenhoEvento.TB_EMPENHO_EVENTO_CODIGO == codigoEvento);

            qryConsulta = Db.TB_EMPENHO_EVENTOs.Where(expWhere)
                                               .AsQueryable();

            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                     .FirstOrDefault();

            return objEntidade;
        }
        public EmpenhoEventoEntity ObterEventoEmpenho(MovimentoEntity objMovimentacao)
        {
            EmpenhoEventoEntity objEntidade = null;
            IQueryable<TB_EMPENHO_EVENTO> qryConsulta = null;
            Expression<Func<TB_EMPENHO_EVENTO, TB_MOVIMENTO, bool>> expWhere;

            expWhere = ((empenhoEvento, movimentacao) => movimentacao.TB_EMPENHO_EVENTO_ID == empenhoEvento.TB_EMPENHO_EVENTO_ID);

            qryConsulta = (from empenhoEvento in Db.TB_EMPENHO_EVENTOs
                           join Movimentacoes in Db.TB_MOVIMENTOs on empenhoEvento.TB_EMPENHO_EVENTO_ID equals Movimentacoes.TB_EMPENHO_EVENTO_ID
                           where Movimentacoes.TB_MOVIMENTO_ID == objMovimentacao.Id
                           select empenhoEvento).AsQueryable();


            #region debug SQL
            var _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            #endregion

            objEntidade = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                     .FirstOrDefault();

            return objEntidade;
        }
        public IList<EmpenhoEventoEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public IList<EmpenhoEventoEntity> Listar()
        {
            IList<EmpenhoEventoEntity> lstRetorno = null;
            IQueryable<TB_EMPENHO_EVENTO> qryConsulta = null;

            qryConsulta = Db.TB_EMPENHO_EVENTOs.AsQueryable();

            //#region debug SQL
            //var _strSQL = qryConsulta.ToString();
            //Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            //#endregion

            lstRetorno = qryConsulta.Select(_instanciadorDTOEmpenhoEvento())
                                    .ToList();

            return lstRetorno;
        }

        public EmpenhoEventoEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<EmpenhoEventoEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        void ICrudBaseService<EmpenhoEventoEntity>.Excluir()
        {
            throw new NotImplementedException();
        }

        public bool Excluir()
        {
            bool blnRetorno = false;
            TB_EMPENHO_EVENTO rowTabela = this.Db.TB_EMPENHO_EVENTOs.Where(empenhoEvento => empenhoEvento.TB_EMPENHO_EVENTO_ID == this.Entity.Id).FirstOrDefault();

            if (rowTabela.IsNotNull())
            {
                this.Db.TB_EMPENHO_EVENTOs.DeleteOnSubmit(rowTabela);
                this.Db.SubmitChanges();
            }

            blnRetorno = this.Db.TB_EMPENHO_EVENTOs.Where(empenhoEvento => empenhoEvento.TB_EMPENHO_EVENTO_ID == this.Entity.Id).FirstOrDefault().IsNull();

            return blnRetorno;
        }

        public void Salvar()
        {
            TB_EMPENHO_EVENTO rowTabela = new TB_EMPENHO_EVENTO();

            if (this.Entity.Id.HasValue && this.Entity.Id.Value != 0)
                rowTabela = this.Db.TB_EMPENHO_EVENTOs.Where(a => a.TB_EMPENHO_EVENTO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_EMPENHO_EVENTOs.InsertOnSubmit(rowTabela);

            rowTabela.TB_EMPENHO_EVENTO_CODIGO = this.Entity.Codigo;
            rowTabela.TB_EMPENHO_EVENTO_DESCRICAO = this.Entity.Descricao;
            rowTabela.TB_EMPENHO_EVENTO_ANO_BASE = this.Entity.AnoBase;
            rowTabela.TB_EMPENHO_EVENTO_ATIVO = this.Entity.Ativo;

            //TODO: Ativação novos campos tabela TB_EMPENHO_EVENTO
            rowTabela.TB_EMPENHO_EVENTO_CODIGO_ESTORNO = this.Entity.CodigoEstorno;
            rowTabela.TB_TIPO_MOVIMENTO_ID = (this.Entity.TipoMovimentoAssociado.IsNotNull() ? this.Entity.TipoMovimentoAssociado.Id : (int?)null);
            rowTabela.TB_EMPENHO_EVENTO_TIPO_MATERIAL = this.Entity.TipoMaterialAssociado;

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            return (this.Db.TB_EMPENHO_EVENTOs.Where(empenhoEvento => empenhoEvento.TB_EMPENHO_EVENTO_CODIGO == this.Entity.Codigo)
                                              .CountReadUncommitted() == 1);
        }

        private Func<TB_EMPENHO_EVENTO, EmpenhoEventoEntity> _instanciadorDTOEmpenhoEvento()
        {
            Func<TB_EMPENHO_EVENTO, EmpenhoEventoEntity> _actionSeletor = null;

            _actionSeletor = (empenhoEvento => new EmpenhoEventoEntity()
            {
                Id = empenhoEvento.TB_EMPENHO_EVENTO_ID,
                Codigo = empenhoEvento.TB_EMPENHO_EVENTO_CODIGO,
                Descricao = empenhoEvento.TB_EMPENHO_EVENTO_DESCRICAO,
                AnoBase = empenhoEvento.TB_EMPENHO_EVENTO_ANO_BASE,
                Ativo = (empenhoEvento.TB_EMPENHO_EVENTO_ATIVO.IsNull() ? false : true),

                CodigoDescricao = string.Format("{0} - {1}", empenhoEvento.TB_EMPENHO_EVENTO_CODIGO, empenhoEvento.TB_EMPENHO_EVENTO_DESCRICAO),
                //TODO: Ativação novos campos tabela TB_EMPENHO_EVENTO
                CodigoEstorno = empenhoEvento.TB_EMPENHO_EVENTO_CODIGO_ESTORNO,
                TipoMovimentoAssociado = (empenhoEvento.TB_TIPO_MOVIMENTO.IsNotNull() ? (new TipoMovimentoEntity() { Id = empenhoEvento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID, Descricao = empenhoEvento.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO }) : null),
                TipoMaterialAssociado = empenhoEvento.TB_EMPENHO_EVENTO_TIPO_MATERIAL
            });

            return _actionSeletor;
        }

        public IList<string> ListarEmpenhosPorAgrupamento(int almoxarifadoId, int gestorId, List<string> listaEmpenhos, bool consumoImediato)
        {
            var listaRetorno = new List<string>();

            var listaItensPossuamEntradaEmpenho = (from movItem in Db.TB_MOVIMENTO_ITEMs
                                                   join mov in Db.TB_MOVIMENTOs on movItem.TB_MOVIMENTO_ID equals mov.TB_MOVIMENTO_ID
                                                   join tipoMov in Db.TB_TIPO_MOVIMENTOs on mov.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID
                                                   join tipoMovAg in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals tipoMovAg.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                   join almox in Db.TB_ALMOXARIFADOs on mov.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID

                                                   where
                                                        mov.TB_MOVIMENTO_ATIVO == true &&
                                                        movItem.TB_MOVIMENTO_ITEM_ATIVO == true &&
                                                        tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.Entrada &&
                                                        mov.TB_ALMOXARIFADO_ID == almoxarifadoId &&
                                                        almox.TB_GESTOR_ID == gestorId &&
                                                        listaEmpenhos.Contains(mov.TB_MOVIMENTO_EMPENHO)
                                                   select mov.TB_MOVIMENTO_EMPENHO
                                              ).Distinct().ToList();


            var listaItensPossuamEntradaConsumoImediato = (from movItem in Db.TB_MOVIMENTO_ITEMs
                                                           join mov in Db.TB_MOVIMENTOs on movItem.TB_MOVIMENTO_ID equals mov.TB_MOVIMENTO_ID
                                                           join tipoMov in Db.TB_TIPO_MOVIMENTOs on mov.TB_TIPO_MOVIMENTO_ID equals tipoMov.TB_TIPO_MOVIMENTO_ID
                                                           join tipoMovAg in Db.TB_TIPO_MOVIMENTO_AGRUPAMENTOs on tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID equals tipoMovAg.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                           join almox in Db.TB_ALMOXARIFADOs on mov.TB_ALMOXARIFADO_ID equals almox.TB_ALMOXARIFADO_ID

                                                           where
                                                                mov.TB_MOVIMENTO_ATIVO == true &&
                                                                movItem.TB_MOVIMENTO_ITEM_ATIVO == true &&
                                                                tipoMov.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)GeralEnum.TipoMovimentoAgrupamento.ConsumoImediato &&
                                                                mov.TB_ALMOXARIFADO_ID == almoxarifadoId &&
                                                                almox.TB_GESTOR_ID == gestorId &&
                                                                listaEmpenhos.Contains(mov.TB_MOVIMENTO_EMPENHO)
                                                           select mov.TB_MOVIMENTO_EMPENHO
                                             ).Distinct().ToList();

            var listaSemEntradaEmpenhoESemConsumoImediato = listaEmpenhos.Where(x => !listaItensPossuamEntradaEmpenho.Contains(x) && !listaItensPossuamEntradaConsumoImediato.Contains(x)).ToList();
            var listaComEntradaEmpenhoEComConsumoImediato = listaEmpenhos.Where(x => listaItensPossuamEntradaEmpenho.Contains(x) && listaItensPossuamEntradaConsumoImediato.Contains(x)).ToList();

            //Remove os itens que sejam consumo e que estejam na lista de Entrada de empenho
            listaItensPossuamEntradaEmpenho = listaItensPossuamEntradaEmpenho.Where(x => !listaItensPossuamEntradaConsumoImediato.Contains(x)).ToList();
            //Remove os itens que sejam entrada de empenho na lista de consumo
            listaItensPossuamEntradaConsumoImediato = listaItensPossuamEntradaConsumoImediato.Where(x => !listaItensPossuamEntradaEmpenho.Contains(x)).ToList();


            if (consumoImediato == false)
            {
                listaRetorno.AddRange(listaItensPossuamEntradaEmpenho);
                listaRetorno.AddRange(listaSemEntradaEmpenhoESemConsumoImediato);

                listaRetorno = listaRetorno.Select(x=>x).Distinct().ToList();
            }
            else
            {
                listaRetorno.AddRange(listaItensPossuamEntradaConsumoImediato);
                listaRetorno.AddRange(listaSemEntradaEmpenhoESemConsumoImediato);
                listaRetorno.AddRange(listaComEntradaEmpenhoEComConsumoImediato);

                listaRetorno = listaRetorno.Select(x => x).Distinct().ToList();
            }

            return listaRetorno;
        }
    }
}
