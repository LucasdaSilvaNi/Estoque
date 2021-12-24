using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Linq.Expressions;
using Sam.Common.LambdaExpression;
using TipoPesquisa = Sam.Common.Util.GeralEnum.TipoPesquisa;
using TipoNotaSIAF = Sam.Common.Util.GeralEnum.TipoNotaSIAF;
using Sam.Domain.Infrastructure;

namespace Sam.Domain.Infrastructure
{
    public class NotaLancamentoPendenteSIAFEMInfraestructure : BaseInfraestructure, INotaLancamentoPendenteSIAFEMService
    {
        private int totalregistros
        {
            get;
            set;
        }
        public int TotalRegistros()
        { return totalregistros; }

        public NotaLancamentoPendenteSIAFEMEntity Entity { get; set; }


        void ICrudBaseService<NotaLancamentoPendenteSIAFEMEntity>.Salvar()
        { throw new NotImplementedException(); }
        void ICrudBaseService<NotaLancamentoPendenteSIAFEMEntity>.Excluir()
        { throw new NotImplementedException(); }
        public bool PodeExcluir()
        { throw new NotImplementedException(); }
        public bool ExisteCodigoInformado()
        { throw new NotImplementedException(); }
        public NotaLancamentoPendenteSIAFEMEntity LerRegistro()
        { throw new NotImplementedException(); }
        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarTodosCod()
        { throw new NotImplementedException(); }


        public NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, bool consideraTambemMovimentacaoEstornada, string anoMesRef = null)
        {
            Expression<Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, bool>> expWhere = null;
            NotaLancamentoPendenteSIAFEMEntity objRetorno = null;


            expWhere = (notaLancamentoPendente => notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DOCUMENTO_SAM == numeroDocumento
                                               && notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA == ((short)((pendenciaAtiva) ? 0 : 1))
                                               && notaLancamentoPendente.TB_ALMOXARIFADO_ID == almoxID);

            if (!consideraTambemMovimentacaoEstornada)
                expWhere = expWhere.And(notaLancamentoPendente => notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO == true);

            if (!String.IsNullOrWhiteSpace(anoMesRef))
                expWhere = expWhere.And(notaLancamentoPendente => notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA == anoMesRef);

            objRetorno = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(expWhere)
                                                                .Select(_instanciadorDTONotaLancamentoPendente())
                                                                .LastOrDefault();

            return objRetorno;
        }

        public NotaLancamentoPendenteSIAFEMEntity ObterPendenciaParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true)
        {
            NotaLancamentoPendenteSIAFEMEntity objRetorno = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaLancamentoPendente => notaLancamentoPendente.TB_MOVIMENTO_ID == movimentacaoMaterialID)
                                                                                                   .Select(_instanciadorDTONotaLancamentoPendente())
                                                                                                   .FirstOrDefault();
            return objRetorno;
        }

        public IList<NotaLancamentoPendenteSIAFEMEntity> ObterPendenciasParaMovimentacao(int movimentacaoMaterialID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstRetorno = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaLancamentoPendente => notaLancamentoPendente.TB_MOVIMENTO_ID == movimentacaoMaterialID)
                                                                                                   .Select(_instanciadorDTONotaLancamentoPendente())
                                                                                                   .ToList();
            return lstRetorno;
        }
        public NotaLancamentoPendenteSIAFEMEntity ObterNotaLancamentoPendente(int notaLancamentoPendenteID)
        {
            NotaLancamentoPendenteSIAFEMEntity objEntidade = null;

            objEntidade = ObterPendenciaPorID(notaLancamentoPendenteID);

            return objEntidade;
        }
        private NotaLancamentoPendenteSIAFEMEntity ObterPendenciaPorID(int notaLancamentoPendenteID)
        {
            NotaLancamentoPendenteSIAFEMEntity objRetorno = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaLancamentoPendente => notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ID == notaLancamentoPendenteID)
                                                                                                   .Select(_instanciadorDTONotaLancamentoPendente())
                                                                                                   .FirstOrDefault();
            return objRetorno;
        }
        private IList<NotaLancamentoPendenteSIAFEMEntity> ObterPendenciaSiafemPorAlmox(int almoxID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstPendenciasLancamentosSiafemAlmox = null;
            Expression<Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, bool>> expWherePrincipal;
            Expression<Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, bool>> expWhere;

            expWherePrincipal = (notaLancamentoPendente => notaLancamentoPendente.TB_ALMOXARIFADO_ID == almoxID
                                                        && notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA == notaLancamentoPendente.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF);


            if (pendenciasAtivas.IsNotNull() && pendenciasAtivas.Value)
            {
                expWhere = (notaLancamentoPendente => notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA == 0);
                expWherePrincipal = LambaExpressionHelper.And(expWherePrincipal, expWhere);
            }
            
                
            lstPendenciasLancamentosSiafemAlmox = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(expWherePrincipal)
                                                                                         .Select(_instanciadorDTONotaLancamentoPendente())
                                                                                         .ToList();

            return lstPendenciasLancamentosSiafemAlmox;
        }

        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarpendenciaSiafemPorAlmox(Enum tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstPendenciasNotasSiafem = null;

            switch ((TipoPesquisa)tipoPesquisa)
            {
                //case TipoPesquisa.SemFiltro:  lstPendenciasNotasSiafem = ObterTodosOsChamados(chamadosAtivos); break;
                //case TipoPesquisa.Gestor:     lstPendenciasNotasSiafem = ObterChamadosPorGestor((int)tabelaPesquisaID); break;
                //case TipoPesquisa.Orgao:      lstPendenciasNotasSiafem = ObterChamadosPorOrgao((int)tabelaPesquisaID); break;
                //case TipoPesquisa.UO:         lstPendenciasNotasSiafem = ObterChamadosPorUO((int)tabelaPesquisaID); break;
                //case TipoPesquisa.UGE:        lstPendenciasNotasSiafem = ObterChamadosPorUGE((int)tabelaPesquisaID); break;
                case TipoPesquisa.Almox: lstPendenciasNotasSiafem = ObterPendenciaSiafemPorAlmox((int)tabelaPesquisaID, pendenciasAtivas); break;
                //case TipoPesquisa.Divisao:    lstPendenciasNotasSiafem = ObterChamadosPorDivisao((int)tabelaPesquisaID); break;
                //case TipoPesquisa.Usuario:    lstPendenciasNotasSiafem = ObterChamadosPorUsuario(tabelaPesquisaID); break;
                case TipoPesquisa.ID: lstPendenciasNotasSiafem = new List<NotaLancamentoPendenteSIAFEMEntity>() { ObterPendenciaPorID((int)tabelaPesquisaID) }; break;

                case TipoPesquisa.UA:

                default: throw new Exception("Filtro de pesquisa não-implementado.");
            }

            this.totalregistros = lstPendenciasNotasSiafem.Count();
            return lstPendenciasNotasSiafem;
        }
        public bool ExistePendenciaSiafemParaMovimentacao(int movimentacaoMaterialID, bool? pendenciaAtiva = true)
        {
            NotaLancamentoPendenteSIAFEMEntity objRetorno = ObterPendenciaParaMovimentacao(movimentacaoMaterialID, pendenciaAtiva);
            return (objRetorno.IsNotNull());
        }
        public NotaLancamentoPendenteSIAFEMEntity ExistePendenciaSiafemParaMovimentacao(int almoxID, string numeroDocumento, bool pendenciaAtiva, bool consideraTambemMovimentacaoEstornada, string anoMesRef = null)
        {
            NotaLancamentoPendenteSIAFEMEntity objRetorno = ObterPendenciaParaMovimentacao(almoxID, numeroDocumento, pendenciaAtiva, consideraTambemMovimentacaoEstornada, anoMesRef);
            return objRetorno;
        }
        public bool InativarPendencia(int notaLancamentoPendenciaID)
        {
            bool blnRetorno = false;
            TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO rowTabela = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaLancamentoPendente => notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ID == notaLancamentoPendenciaID)
                                                                                                   .FirstOrDefault();

            if (rowTabela.IsNotNull())
            {
                rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA = 1;
                this.Db.SubmitChanges();
            }
            else
                throw new Exception("Registro não encontrado.");

            return blnRetorno;
        }
        public bool InativarPendenciasPorMovimentacao(int movimentacaoMaterialID)
        {
            bool blnRetorno = false;
            IList<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO> rowsTabela = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaLancamentoPendente => notaLancamentoPendente.TB_MOVIMENTO_ID == movimentacaoMaterialID
                                                                                                                                 && notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA == 0)
                                                                                                           .ToList();

            if (rowsTabela.HasElements())
            {
                foreach (var pendenciaNotaLancamento in rowsTabela)
                    pendenciaNotaLancamento.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA = 1;

                this.Db.SubmitChanges();
            }
            else
                throw new Exception("Registro não encontrado.");

            return blnRetorno;
        }

        public bool InserirRegistro(NotaLancamentoPendenteSIAFEMEntity pendenciaNotaLancamento)
        {
            TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO rowTabela = new TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO();

            if (pendenciaNotaLancamento.Id.HasValue)
                rowTabela = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(notaPendente => notaPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ID == pendenciaNotaLancamento.Id.Value).FirstOrDefault();
            else
                Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.InsertOnSubmit(rowTabela);

            

            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DATA_ENVIO_MSG_WS = pendenciaNotaLancamento.DataEnvioMsgWs;
            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DATA_REENVIO_MSG_WS = pendenciaNotaLancamento.DataReenvioMsgWs;
            rowTabela.TB_MOVIMENTO_ID = pendenciaNotaLancamento.MovimentoVinculado.Id.Value;
            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DOCUMENTO_SAM = pendenciaNotaLancamento.MovimentoVinculado.NumeroDocumento;
            rowTabela.TB_ALMOXARIFADO_ID = pendenciaNotaLancamento.MovimentoVinculado.Almoxarifado.Id.Value;
            rowTabela.TB_AUDITORIA_INTEGRACAO_ID = pendenciaNotaLancamento.AuditoriaIntegracaoVinculada.Id.Value;
            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ERRO_PROCESSAMENTO_MSG_WS = pendenciaNotaLancamento.ErroProcessamentoMsgWS;
            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA = pendenciaNotaLancamento.StatusPendencia;
            rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_TIPO_NOTA_PENDENCIA = (short)pendenciaNotaLancamento.TipoNotaSIAF;


            this.Db.SubmitChanges();
            pendenciaNotaLancamento.Id = rowTabela.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ID;

            return (pendenciaNotaLancamento.Id.HasValue);
        }
        public IList<NotaLancamentoPendenteSIAFEMEntity> Listar()
        { throw new NotImplementedException(); }
        public IList<NotaLancamentoPendenteSIAFEMEntity> Imprimir()
        { throw new NotImplementedException(); }


        private Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, NotaLancamentoPendenteSIAFEMEntity> _instanciadorDTONotaLancamentoPendente()
        {
            Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, NotaLancamentoPendenteSIAFEMEntity> _actionSeletor = null;
            IQueryable<TB_ALMOXARIFADO> qryConsulta = null;

            qryConsulta = (from almox in Db.TB_ALMOXARIFADOs select almox).AsQueryable();
            _actionSeletor = (notaLancamentoPendente => new NotaLancamentoPendenteSIAFEMEntity()
            {
                Id = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ID,
                DocumentoSAM = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DOCUMENTO_SAM,
                ErroProcessamentoMsgWS = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_ERRO_PROCESSAMENTO_MSG_WS,
                DataReenvioMsgWs = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DATA_REENVIO_MSG_WS,
                DataEnvioMsgWs = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_DATA_ENVIO_MSG_WS,
                StatusPendencia = notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA,
                AlmoxarifadoVinculado = (notaLancamentoPendente.TB_ALMOXARIFADO.IsNotNull() ? (new AlmoxarifadoEntity()
                {
                    Id = notaLancamentoPendente.TB_ALMOXARIFADO_ID,
                    Descricao = notaLancamentoPendente.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                    Codigo = notaLancamentoPendente.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO
                }) : null),
                TipoNotaSIAF = (TipoNotaSIAF)notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_TIPO_NOTA_PENDENCIA,
                AuditoriaIntegracaoVinculada = (notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.IsNotNull() ? (new AuditoriaIntegracaoEntity()
                {
                    Id = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_ID,
                    DataEnvio = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_DATA_ENVIO,
                    DataRetorno = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_DATA_RETORNO,
                    MsgEstimuloWS = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_MSG_ESTIMULO_WS,
                    MsgRetornoWS = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_MSG_RETORNO_WS,
                    NomeSistema = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_NOME_SISTEMA,
                    UsuarioSAM = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_USUARIO_SAM,
                    UsuarioSistemaExterno = notaLancamentoPendente.TB_AUDITORIA_INTEGRACAO.TB_AUDITORIA_INTEGRACAO_USUARIO_SISTEMA_EXTERNO,
                }) : null),
                MovimentoVinculado = (notaLancamentoPendente.TB_MOVIMENTO.IsNotNull() ? (new MovimentoEntity()
                {
                    Id = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ID,
                    AnoMesReferencia = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA,
                    Almoxarifado = (notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.IsNotNull() ? (new AlmoxarifadoEntity()
                    {
                        Id = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID,
                        Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO,
                        Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO,
                        MesRef = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF,
                        Uge = new UGEEntity()
                        {
                            Id = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_UGE.TB_UGE_ID,
                            Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_UGE.TB_UGE_CODIGO,
                            Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_UGE.TB_UGE_DESCRICAO
                        },
                        Gestor = new GestorEntity()
                        {
                            Id = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_ID,
                            CodigoGestao = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO,
                            Nome = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME,
                            NomeReduzido = notaLancamentoPendente.TB_MOVIMENTO.TB_ALMOXARIFADO.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO
                        }
                    }) : null),
                    UGE = (notaLancamentoPendente.TB_MOVIMENTO.TB_UGE.IsNotNull() ? (new UGEEntity()
                    {
                        Id = notaLancamentoPendente.TB_MOVIMENTO.TB_UGE.TB_UGE_ID,
                        Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_UGE.TB_UGE_CODIGO,
                        Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_UGE.TB_UGE_DESCRICAO
                    }) : null),
                    Divisao = (notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.IsNotNull() ? (new DivisaoEntity()
                    {
                        Id = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_ID,
                        Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_CODIGO,
                        Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_DIVISAO_DESCRICAO,
                        Ua = (notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.IsNotNull() ? (new UAEntity()
                        {
                            Id = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UA_ID,
                            Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UA_CODIGO,
                            Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UA_DESCRICAO,
                            Uge = (notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UGE.IsNotNull() ? (new UGEEntity()
                            {
                                Id = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UGE.TB_UGE_ID,
                                Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UGE.TB_UGE_CODIGO,
                                Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_DIVISAO.TB_UA.TB_UGE.TB_UGE_DESCRICAO,


                            }) : null),

                        }) : null),

                    }) : null),

                    NumeroDocumento = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_NUMERO_DOCUMENTO,
                    Ativo = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO,
                    DataMovimento = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_DATA_MOVIMENTO,
                    Observacoes = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_OBSERVACOES,
                    SubTipoMovimentoId = notaLancamentoPendente.TB_MOVIMENTO.TB_SUBTIPO_MOVIMENTO_ID,
                    MovimAlmoxOrigemDestino = (notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO.IsNotNull() ? (qryConsulta.Where(almoxTransf => almoxTransf.TB_ALMOXARIFADO_ID == notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO)
                                                                                                                                                                                                                                      .Select(almoxTransf => (new AlmoxarifadoEntity()
                                                                                                                                                                                                                                      {
                                                                                                                                                                                                                                          Id = almoxTransf.TB_ALMOXARIFADO_ID,
                                                                                                                                                                                                                                          Descricao = almoxTransf.TB_ALMOXARIFADO_DESCRICAO,
                                                                                                                                                                                                                                          Codigo = almoxTransf.TB_ALMOXARIFADO_CODIGO,
                                                                                                                                                                                                                                          Uge = new UGEEntity()
                                                                                                                                                                                                                                          {
                                                                                                                                                                                                                                              Id = almoxTransf.TB_UGE.TB_UGE_ID,
                                                                                                                                                                                                                                              Codigo = almoxTransf.TB_UGE.TB_UGE_CODIGO,
                                                                                                                                                                                                                                              Descricao = almoxTransf.TB_UGE.TB_UGE_DESCRICAO
                                                                                                                                                                                                                                          },
                                                                                                                                                                                                                                          Gestor = new GestorEntity()
                                                                                                                                                                                                                                          {
                                                                                                                                                                                                                                              Id = almoxTransf.TB_GESTOR.TB_GESTOR_ID,
                                                                                                                                                                                                                                              CodigoGestao = almoxTransf.TB_GESTOR.TB_GESTOR_CODIGO_GESTAO,
                                                                                                                                                                                                                                              Nome = almoxTransf.TB_GESTOR.TB_GESTOR_NOME,
                                                                                                                                                                                                                                              NomeReduzido = almoxTransf.TB_GESTOR.TB_GESTOR_NOME_REDUZIDO
                                                                                                                                                                                                                                          }
                                                                                                                                                                                                                                      }))
                                                                                                                                                                                                                                      .FirstOrDefault()) : null),
                    //Carga dos dados necessários para determinar TipoMaterial (NaturezaDespesa/Subitem)
                    MovimentoItem = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity()
                    {
                        Id = _movItem.TB_MOVIMENTO_ITEM_ID,
                        ValorMov = _movItem.TB_MOVIMENTO_ITEM_VALOR_MOV,
                        PTResCodigo = _movItem.TB_PTRES_CODIGO != null ? _movItem.TB_PTRES_CODIGO.ToString() : string.Empty,
                        //PTResAcao = _movItem.TB_PTRES_PT_ACAO != null ? _movItem.TB_PTRES_PT_ACAO.ToString() : string.Empty,
                        SubItemMaterial = new SubItemMaterialEntity()
                        {
                            Id = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                            Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                            Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,

                            NaturezaDespesa = new NaturezaDespesaEntity()
                            {
                                Id = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                Codigo = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                Descricao = _movItem.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                            },

                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                            where _movItem.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                            select new ItemMaterialEntity
                                            {
                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                            }
  ).FirstOrDefault(),
                        }
                    })
                                                                                                                                                                              .ToList(),
                    ValorDocumento = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ITEMs.Select(_movItem => new MovimentoItemEntity() { ValorMov = _movItem.TB_MOVIMENTO_ITEM_VALOR_MOV })
                                                                                                                                                                               .Sum(movItem => ((movItem.ValorMov.HasValue) ? movItem.ValorMov : 0.00m)),
                    ValorOriginalDocumento = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_VALOR_DOCUMENTO,
                    TipoMovimento = (notaLancamentoPendente.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.IsNotNull() ? (new TipoMovimentoEntity()
                    {
                        Id = notaLancamentoPendente.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_ID,
                        Descricao = notaLancamentoPendente.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_DESCRICAO,
                        Codigo = notaLancamentoPendente.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_CODIGO,
                        SubTipoMovimentoItem = (notaLancamentoPendente.TB_MOVIMENTO.TB_SUBTIPO_MOVIMENTO_ID.IsNotNull() ? (new SubTipoMovimentoEntity()
                        {
                            Id = notaLancamentoPendente.TB_MOVIMENTO.TB_SUBTIPO_MOVIMENTO_ID

                        }) : null)
                    }) : null),
                    InscricaoCE = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_CE,
                    Empenho = notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_EMPENHO
                }) : null),

                Tipo = (notaLancamentoPendente.TB_MOVIMENTO.IsNotNull() ? ((notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ATIVO.Value) ? "N" : "E") : null)
            });
            return _actionSeletor;
        }



        private IList<NotaLancamentoPendenteSIAFEMEntity> ListarpendenciaSiafemPorAlmox(int almoxID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstPendenciasLancamentosSiafemAlmox = null;
            Expression<Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, bool>> expWherePrincipal;
            Expression<Func<TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO, bool>> expWhere;
            NotaLancamentoPendenteSIAFEMEntity nota = new NotaLancamentoPendenteSIAFEMEntity();

            expWherePrincipal = (notaLancamentoPendente => notaLancamentoPendente.TB_ALMOXARIFADO_ID == almoxID
                                                         && notaLancamentoPendente.TB_MOVIMENTO.TB_MOVIMENTO_ANO_MES_REFERENCIA == notaLancamentoPendente.TB_ALMOXARIFADO.TB_ALMOXARIFADO_MES_REF);

            if (pendenciasAtivas.IsNotNull() && pendenciasAtivas.Value)
            {
                expWhere = (notaLancamentoPendente => notaLancamentoPendente.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTO_STATUS_PENDENCIA == 0);
                expWherePrincipal = LambaExpressionHelper.And(expWherePrincipal, expWhere);
            }

            lstPendenciasLancamentosSiafemAlmox = Db.TB_SIAFEM_PENDENCIA_NOTA_LANCAMENTOs.Where(expWherePrincipal)
                                                                                         .Select(_instanciadorDTONotaLancamentoPendente())
                                                                                         .ToList();
            return lstPendenciasLancamentosSiafemAlmox;
        }



        public IList<NotaLancamentoPendenteSIAFEMEntity> ListarpendenciaSiafemPorAlmoxarifados(Enum tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstPendenciasNotasSiafem = null;

            switch ((TipoPesquisa)tipoPesquisa)
            {
                //case TipoPesquisa.SemFiltro:  lstPendenciasNotasSiafem = ObterTodosOsChamados(chamadosAtivos); break;
                //case TipoPesquisa.Gestor:     lstPendenciasNotasSiafem = ObterChamadosPorGestor((int)tabelaPesquisaID); break;
                //case TipoPesquisa.Orgao:      lstPendenciasNotasSiafem = ObterChamadosPorOrgao((int)tabelaPesquisaID); break;
                //case TipoPesquisa.UO:         lstPendenciasNotasSiafem = ObterChamadosPorUO((int)tabelaPesquisaID); break;
                //case TipoPesquisa.UGE:        lstPendenciasNotasSiafem = ObterChamadosPorUGE((int)tabelaPesquisaID); break;
                case TipoPesquisa.Almox: lstPendenciasNotasSiafem = ListarpendenciaSiafemPorAlmox((int)tabelaPesquisaID, pendenciasAtivas); break;
                //case TipoPesquisa.Divisao:    lstPendenciasNotasSiafem = ObterChamadosPorDivisao((int)tabelaPesquisaID); break;
                //case TipoPesquisa.Usuario:    lstPendenciasNotasSiafem = ObterChamadosPorUsuario(tabelaPesquisaID); break;
                case TipoPesquisa.ID: lstPendenciasNotasSiafem = new List<NotaLancamentoPendenteSIAFEMEntity>() { ObterPendenciaPorID((int)tabelaPesquisaID) }; break;

                case TipoPesquisa.UA:

                default: throw new Exception("Filtro de pesquisa não-implementado.");
            }

            this.totalregistros = lstPendenciasNotasSiafem.Count();
            return lstPendenciasNotasSiafem;
        }



        public IList<NotaLancamentoPendenteSIAFEMEntity> Listarpendencia(Enum tipoPesquisa, long tabelaPesquisaID, bool? pendenciasAtivas = true)
        {
            IList<NotaLancamentoPendenteSIAFEMEntity> lstPendenciasNotasSiafem = null;

            switch ((TipoPesquisa)tipoPesquisa)
            {

                case TipoPesquisa.Almox: lstPendenciasNotasSiafem = ObterPendenciaSiafemPorAlmox((int)tabelaPesquisaID, pendenciasAtivas); break;

                case TipoPesquisa.ID: lstPendenciasNotasSiafem = new List<NotaLancamentoPendenteSIAFEMEntity>() { ObterPendenciaPorID((int)tabelaPesquisaID) }; break;

                case TipoPesquisa.UA:

                default: throw new Exception("Filtro de pesquisa não-implementado.");
            }

            this.totalregistros = lstPendenciasNotasSiafem.Count();
            return lstPendenciasNotasSiafem;
        }


    }
}
