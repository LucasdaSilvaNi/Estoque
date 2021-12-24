using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Common.Util;
using System.Data.Objects.DataClasses;
using Sam.Business.Importacao;
using Sam.Domain.Entity;
using System.Transactions;
using System.Data.Objects;
using Sam.Infrastructure.Infrastructure.Materiais;
using System.Data;


namespace Sam.Business
{
    public class MovimentoBusiness : BaseBusiness, ICrudBaseBusiness<TB_MOVIMENTO>
    {
        private readonly MovimentoRecalcularInfrastructure infraMovimentoPendente = new MovimentoRecalcularInfrastructure();

        #region M�todos Customizados

        public List<TB_MOVIMENTO> ConsultaMovimento(int startRowIndex, Expression<Func<TB_MOVIMENTO, bool>> where)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();
            infra.LazyLoadingEnabled = true;

            //Monta Sort
            Expression<Func<TB_MOVIMENTO, DateTime>> sortExpression = a => (DateTime)a.TB_MOVIMENTO_DATA_OPERACAO;


            var result = infra.SelectWhere(sortExpression, true, where, startRowIndex);

            // intera��o com outro modelo (SamModel x SegModel)
            foreach (TB_MOVIMENTO resultUsu in result)
            {
                if (resultUsu.TB_LOGIN_ID != null)
                    resultUsu.TB_USUARIO_NOME = new Sam.Business.LoginBusiness().SelectOne(a => a.TB_LOGIN_ID == resultUsu.TB_LOGIN_ID).TB_USUARIO.TB_USUARIO_NOME_USUARIO;

                if (resultUsu.TB_LOGIN_ID_ESTORNO != null)
                    resultUsu.TB_USUARIO_NOME_ESTORNO = new Sam.Business.LoginBusiness().SelectOne(a => a.TB_LOGIN_ID == resultUsu.TB_LOGIN_ID_ESTORNO).TB_USUARIO.TB_USUARIO_NOME_USUARIO;

                foreach (TB_MOVIMENTO_ITEM resultUsuItem in resultUsu.TB_MOVIMENTO_ITEM)
                {
                    if (resultUsuItem.TB_LOGIN_ID != null)
                        resultUsuItem.TB_USUARIO_NOME = new Sam.Business.LoginBusiness().SelectOne(a => a.TB_LOGIN_ID == resultUsuItem.TB_LOGIN_ID).TB_USUARIO.TB_USUARIO_NOME_USUARIO;

                    if (resultUsuItem.TB_LOGIN_ID_ESTORNO != null)
                        resultUsuItem.TB_USUARIO_NOME_ESTORNO = new Sam.Business.LoginBusiness().SelectOne(a => a.TB_LOGIN_ID == resultUsuItem.TB_LOGIN_ID_ESTORNO).TB_USUARIO.TB_USUARIO_NOME_USUARIO;
                }
            }

            // para TB_MOVIMENTO_ITEM

            this.TotalRegistros = infra.TotalRegistros;
            return result;

        }

        /// <summary>
        /// Retorna TB_MOVIMENTO e TB_MOVIMENTO_ITEM preenchido a partir do MovimentoEntity
        /// </summary>
        /// <param name="movimento">MovimentoEntity</param>
        /// <returns>TB_MOVIMENTO</returns>
        public TB_MOVIMENTO SetEntidade(MovimentoEntity movimento)
        {
            TB_MOVIMENTO tbMovimento = new TB_MOVIMENTO();

            if (movimento == null)
                throw new Exception("Erro ao carregar TB_MOVIMENTO");
            else
            {
                tbMovimento.TB_MOVIMENTO_ID = (movimento.Id != null) ? (int)movimento.Id : 0;

                if (movimento.Almoxarifado != null)
                    tbMovimento.TB_ALMOXARIFADO_ID = TratamentoDados.ParseIntNull(movimento.Almoxarifado.Id);

                if (movimento.TipoMovimento != null)
                    tbMovimento.TB_TIPO_MOVIMENTO_ID = movimento.TipoMovimento.Id;

                tbMovimento.TB_MOVIMENTO_GERADOR_DESCRICAO = movimento.GeradorDescricao;
                tbMovimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = TratamentoDados.ParseStringNull(movimento.NumeroDocumento);
                tbMovimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = TratamentoDados.ParseStringNull(movimento.AnoMesReferencia);
                tbMovimento.TB_MOVIMENTO_DATA_DOCUMENTO = TratamentoDados.ParseDateTimeNull(movimento.DataDocumento);
                tbMovimento.TB_MOVIMENTO_DATA_MOVIMENTO = TratamentoDados.ParseDateTimeNull(movimento.DataMovimento);
                tbMovimento.TB_MOVIMENTO_FONTE_RECURSO = TratamentoDados.ParseStringNull(movimento.FonteRecurso);
                tbMovimento.TB_MOVIMENTO_VALOR_DOCUMENTO = TratamentoDados.ParseDecimalNull(movimento.ValorDocumento);
                tbMovimento.TB_MOVIMENTO_OBSERVACOES = TratamentoDados.ParseStringNull(movimento.Observacoes);
                tbMovimento.TB_MOVIMENTO_INSTRUCOES = TratamentoDados.ParseStringNull(movimento.Instrucoes);
                tbMovimento.TB_MOVIMENTO_EMPENHO = TratamentoDados.ParseStringNull(movimento.Empenho);

                if (movimento.MovimAlmoxOrigemDestino != null)
                    tbMovimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = movimento.MovimAlmoxOrigemDestino.Id;

                if (movimento.Fornecedor != null)
                    tbMovimento.TB_FORNECEDOR_ID = movimento.Fornecedor.Id;

                if (movimento.Divisao != null)
                    tbMovimento.TB_DIVISAO_ID = movimento.Divisao.Id;

                if (movimento.UGE != null)
                    tbMovimento.TB_UGE_ID = movimento.UGE.Id;

                tbMovimento.TB_MOVIMENTO_ATIVO = movimento.Ativo;

                if (movimento.EmpenhoEvento != null)
                    tbMovimento.TB_EMPENHO_EVENTO_ID = movimento.EmpenhoEvento.Id;

                tbMovimento.TB_MOVIMENTO_DATA_OPERACAO = TratamentoDados.ParseDateTimeNull(movimento.DataOperacao);
                tbMovimento.TB_LOGIN_ID = movimento.IdLogin;

                if (movimento.EmpenhoLicitacao != null)
                    tbMovimento.TB_EMPENHO_LICITACAO_ID = movimento.EmpenhoLicitacao.Id;

                if (movimento.MovimentoItem != null)
                {
                    foreach (var movItem in movimento.MovimentoItem)
                    {
                        TB_MOVIMENTO_ITEM tbMovItem = new TB_MOVIMENTO_ITEM();

                        tbMovItem.TB_MOVIMENTO_ITEM_ID = TratamentoDados.ParseIntNull(movItem.Id);

                        if (movItem.Movimento != null)
                            tbMovItem.TB_MOVIMENTO_ID = TratamentoDados.ParseIntNull(movItem.Movimento.Id);

                        if (movItem.UGE != null)
                            tbMovItem.TB_UGE_ID = TratamentoDados.ParseIntNull(movItem.UGE.Id);

                        if (movItem.ItemMaterial != null)
                            tbMovItem.TB_ITEM_MATERIAL_ID = TratamentoDados.ParseIntNull(movItem.ItemMaterial.Id);

                        if (movItem.SubItemMaterial != null)
                            tbMovItem.TB_SUBITEM_MATERIAL_ID = TratamentoDados.ParseIntNull(movItem.SubItemMaterial.Id);


                        tbMovItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = movItem.DataVencimentoLote;
                        tbMovItem.TB_MOVIMENTO_ITEM_LOTE_IDENT = movItem.IdentificacaoLote;
                        tbMovItem.TB_MOVIMENTO_ITEM_LOTE_FABR = movItem.FabricanteLote;
                        //tbMovItem.TB_MOVIMENTO_ITEM_QTDE_MOV = movItem.QtdeMov.Value;
                        //tbMovItem.TB_MOVIMENTO_ITEM_QTDE_LIQ = movItem.QtdeLiq.Value;
                        tbMovItem.TB_MOVIMENTO_ITEM_PRECO_UNIT = movItem.PrecoUnit;
                        tbMovItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = movItem.SaldoValor;
                        //tbMovItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = movItem.SaldoQtde.Value;
                        //tbMovItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = movItem.SaldoQtdeLote.Value;
                        tbMovItem.TB_MOVIMENTO_ITEM_VALOR_MOV = movItem.ValorMov;
                        tbMovItem.TB_MOVIMENTO_ITEM_DESD = movItem.Desd;
                        tbMovItem.TB_MOVIMENTO_ITEM_ATIVO = movItem.Ativo;
                        tbMovItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = movItem.NL_Liquidacao;
                        tbMovItem.TB_MOVIMENTO = tbMovimento;

                        tbMovimento.TB_MOVIMENTO_ITEM.Add(tbMovItem);
                    }
                }
            }

            return tbMovimento;
        }

        /// <summary>
        /// Caso um movimento possui Itens identicos (SubIten, UGE, Almoxarifado e lote) ira agrupar os valores e quantidade
        /// </summary>
        /// <param name="tbMovimento">Movimento a ser agrupado</param>
        /// <returns>Movimento Agrupado</returns>
        public TB_MOVIMENTO AgrupaMovimentoItems(TB_MOVIMENTO tbMovimento)
        {
            if (tbMovimento == null)
                return tbMovimento;
            else
            {
                if (tbMovimento.TB_MOVIMENTO_ITEM == null)
                    return tbMovimento;
                else
                {
                    var result = (from movItem in tbMovimento.TB_MOVIMENTO_ITEM
                                  group movItem by new
                                  {
                                      movItem.TB_MOVIMENTO_ITEM_ID,
                                      movItem.TB_MOVIMENTO_ID,
                                      movItem.TB_UGE_ID,
                                      movItem.TB_ITEM_MATERIAL_ID,
                                      movItem.TB_SUBITEM_MATERIAL_ID,
                                      movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                      movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                      movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                      movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                      movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                      movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                      movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                      movItem.TB_MOVIMENTO_ITEM_DESD,
                                      movItem.TB_MOVIMENTO_ITEM_ATIVO,
                                      movItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                                      //movItem.TB_LOGIN_ID,
                                      //movItem.TB_MOVIMENTO_ITEM_DATA_OPERACAO,
                                      //movItem.TB_LOGIN_ID_ESTORNO,
                                      //movItem.TB_MOVIMENTO_ITEM_DATA_ESTORNO,
                                  } into g
                                  select new TB_MOVIMENTO_ITEM
                                  {
                                      TB_MOVIMENTO_ITEM_ID = g.Key.TB_MOVIMENTO_ITEM_ID,
                                      TB_MOVIMENTO_ID = g.Key.TB_MOVIMENTO_ID,
                                      TB_UGE_ID = g.Key.TB_UGE_ID,
                                      TB_ITEM_MATERIAL_ID = g.Key.TB_ITEM_MATERIAL_ID,
                                      TB_SUBITEM_MATERIAL_ID = g.Key.TB_SUBITEM_MATERIAL_ID,
                                      TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = g.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                      TB_MOVIMENTO_ITEM_LOTE_IDENT = g.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                      TB_MOVIMENTO_ITEM_LOTE_FABR = g.Key.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                      TB_MOVIMENTO_ITEM_PRECO_UNIT = g.Key.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                      TB_MOVIMENTO_ITEM_SALDO_VALOR = g.Key.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                      TB_MOVIMENTO_ITEM_SALDO_QTDE = g.Key.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                      TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = g.Key.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                      TB_MOVIMENTO_ITEM_DESD = g.Key.TB_MOVIMENTO_ITEM_DESD,
                                      TB_MOVIMENTO_ITEM_ATIVO = g.Key.TB_MOVIMENTO_ITEM_ATIVO,
                                      TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = g.Key.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO,
                                      TB_MOVIMENTO_ITEM_QTDE_MOV = g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV),
                                      TB_MOVIMENTO_ITEM_QTDE_LIQ = g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_LIQ),
                                      TB_MOVIMENTO_ITEM_VALOR_MOV = g.Sum(s => s.TB_MOVIMENTO_ITEM_VALOR_MOV),
                                  }).ToEntityCollection<TB_MOVIMENTO_ITEM>();

                    tbMovimento.TB_MOVIMENTO_ITEM.Clear();
                    tbMovimento.TB_MOVIMENTO_ITEM = result;
                    return tbMovimento;
                }
            }
        }

        /// <summary>
        /// Retorna o Saldo pelo Movimento Item, agrupando por Lote, Somando a quantidade, Valor e calculando o Pre�o M�dio.
        /// </summary>
        /// <param name="tbMovimento">Lista de movimentos para ser calculado o Saldo</param>
        /// <returns>Retorna agrupamento do Movimento Item</returns>
        public IList<TB_MOVIMENTO_ITEM> CalcularSaldoByMovimentoItem(IList<TB_MOVIMENTO_ITEM> tbMovimento)
        {
            if (tbMovimento == null)
                return tbMovimento;
            else
            {
                //Agrupo os SubItens por lote
                IList<TB_MOVIMENTO_ITEM> result = (from movItem in tbMovimento
                                                   group movItem by new
                                                   {
                                                       movItem.TB_UGE_ID,
                                                       movItem.TB_SUBITEM_MATERIAL_ID,
                                                       movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                       movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                       movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                       movItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID
                                                   } into g
                                                   select new TB_MOVIMENTO_ITEM
                                                   {
                                                       TB_UGE_ID = g.Key.TB_UGE_ID,
                                                       TB_SUBITEM_MATERIAL_ID = g.Key.TB_SUBITEM_MATERIAL_ID,
                                                       TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = g.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                       TB_MOVIMENTO_ITEM_LOTE_IDENT = g.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                       TB_MOVIMENTO_ITEM_LOTE_FABR = g.Key.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                       TB_MOVIMENTO_ITEM_QTDE_MOV = (g.Key.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 1) ? +g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV) : (g.Key.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 2) ? -g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV) : 0,
                                                       TB_MOVIMENTO_ITEM_QTDE_LIQ = g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_LIQ),
                                                       TB_MOVIMENTO_ITEM_VALOR_MOV = (g.Key.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 1) ? +g.Sum(s => s.TB_MOVIMENTO_ITEM_VALOR_MOV) : (g.Key.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == 2) ? -g.Sum(s => s.TB_MOVIMENTO_ITEM_VALOR_MOV) : 0,


                                                   }).ToList<TB_MOVIMENTO_ITEM>();

                //Calcular os valores Entrada-Saida e pre�o m�dio
                IList<TB_MOVIMENTO_ITEM> result2 = (from movItem in result
                                                    group movItem by new
                                                    {
                                                        movItem.TB_SUBITEM_MATERIAL_ID,
                                                        movItem.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        movItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        movItem.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        movItem.TB_MOVIMENTO_ITEM_LOTE_FABR,

                                                    } into g
                                                    select new TB_MOVIMENTO_ITEM
                                                    {
                                                        TB_SUBITEM_MATERIAL_ID = g.Key.TB_SUBITEM_MATERIAL_ID,
                                                        TB_MOVIMENTO_ITEM_PRECO_UNIT = ((g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV) > 0) ? (g.Sum(s => s.TB_MOVIMENTO_ITEM_VALOR_MOV) / g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV)) : 0),
                                                        TB_MOVIMENTO_ITEM_QTDE_MOV = g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_MOV),
                                                        TB_MOVIMENTO_ITEM_QTDE_LIQ = g.Sum(s => s.TB_MOVIMENTO_ITEM_QTDE_LIQ),
                                                        TB_MOVIMENTO_ITEM_VALOR_MOV = g.Sum(s => s.TB_MOVIMENTO_ITEM_VALOR_MOV),
                                                        TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = g.Key.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        TB_MOVIMENTO_ITEM_LOTE_IDENT = g.Key.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        TB_MOVIMENTO_ITEM_LOTE_FABR = g.Key.TB_MOVIMENTO_ITEM_LOTE_FABR,

                                                    }).ToList<TB_MOVIMENTO_ITEM>();

                return result2;

            }
        }

        /// <summary>
        /// Estornar o movimento, Movimento e Itens ser�o Desativado
        /// </summary>
        /// <param name="movimentoId">Id do movimento</param>
        /// <param name="LoginId">Login do usu�rio</param>
        public void DesativarMovimento(int movimentoId, int LoginId)
        {
            Infrastructure.MovimentoInfrastructure infra = new MovimentoInfrastructure();
            infra.LazyLoadingEnabled = true; //Retornar com os Itens

            //Retorna o objeto Movimento atualizado para ser estornado
            var tbMovimento = infra.SelectOne(a => a.TB_MOVIMENTO_ID == movimentoId);

            try
            {
                tbMovimento.TB_MOVIMENTO_ATIVO = false;
                tbMovimento.TB_LOGIN_ID_ESTORNO = LoginId;

                foreach (var movItem in tbMovimento.TB_MOVIMENTO_ITEM)
                {
                    movItem.TB_MOVIMENTO_ITEM_ATIVO = false;
                    movItem.TB_LOGIN_ID_ESTORNO = LoginId;
                    movItem.TB_MOVIMENTO_ITEM_DATA_ESTORNO = DateTime.Now;
                }

                infra.Update(tbMovimento);
                infra.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

        public List<GeralEnum.CargaErro> ListCargaErro
        {
            get { return listCargaErro; }
            set { listCargaErro = value; }
        }

        public bool ConsistidoCargaErro
        {
            get
            {
                return this.ListCargaErro.Count == 0;
            }
        }

        public bool InsertListControleImportacao(TB_CONTROLE entityList)
        {
            string sequencia = "";

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(1, 0, 0, 0)))
            {

                try
                {
                    bool isErro = false;
                    MovimentoInfrastructure infraMovimento = new MovimentoInfrastructure();
                    SaldoSubItemInfrastructure infraSaldo = new SaldoSubItemInfrastructure();
                    MovimentoItemInfrastructure infraMovimentoItem = new MovimentoItemInfrastructure();
                    CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();
                    CargaBusiness cargaBusiness = new CargaBusiness();
                    ImportacaoInventario importacaoInventario = new ImportacaoInventario();

                    // N�o pode ter saldo cadastrado para o SubItem, Almox, UGEId ou de outros lotes
                    // Calcular o pre�o m�dio
                    //Inserir o Saldo
                    //Desdobro n�o precisa gravar

                    //Calcular o pre�o medio do lote, criar Metodo....

                    foreach (var carga in entityList.TB_CARGA)
                    {
                        sequencia = carga.TB_CARGA_SEQ;

                        //Verifica se o movimento j� foi cadastrado na base
                        bool isDocCadastrado = importacaoInventario.IsDocumentoCadastrado(carga.TB_MOVIMENTO_NUMERO_DOCUMENTO);

                        var movimento = MontarMovimento(carga);
                        var saldo = MontarSaldoSubItem(carga);

                        if (isDocCadastrado)
                            listCargaErro.Add(GeralEnum.CargaErro.NumeroDocumentoCadastrado);

                        if (this.ConsistidoCargaErro)
                        {
                            //Insere o movimento e movimento item
                            infraMovimento.Insert(movimento);

                            //Verifica se existe saldo para realizar o update do pre�o medio
                            var saldoList = importacaoInventario.VerificaExisteSaldo(carga);

                            if (saldoList.Count > 0)
                            {
                                var precoUnitario = importacaoInventario.CalcularPrecoUnitarioLote(saldoList, carga);

                                if (precoUnitario != null)
                                {
                                    foreach (var saldoUpdate in saldoList)
                                    {
                                        //Atualiza o pre�o unit�rio
                                        saldoUpdate.TB_SALDO_SUBITEM_PRECO_UNIT = precoUnitario;
                                        saldo.TB_SALDO_SUBITEM_PRECO_UNIT = precoUnitario;

                                        infraSaldo.Update(saldoUpdate);
                                    }
                                }
                            }

                            //Cadastra saldo, sem o SaveChanges
                            infraSaldo.Insert(saldo);

                            //Atualiza a carga                        
                            carga.TB_CARGA_VALIDO = true;
                            cargaBusiness.Update(carga);
                        }
                        else
                        {
                            foreach (GeralEnum.CargaErro erroEnum in ListCargaErro)
                            {
                                TB_CARGA_ERRO cargaErro = new TB_CARGA_ERRO();
                                cargaErro.TB_CARGA_ID = carga.TB_CARGA_ID;
                                cargaErro.TB_ERRO_ID = (int)erroEnum;
                                infraCargaErro.Insert(cargaErro);

                                //Atualiza a carga
                                carga.TB_CARGA_VALIDO = false;
                                cargaBusiness.Update(carga);

                                //Se ocorreu ao menos 1 erro, retorna true;
                                isErro = true;
                            }
                        }
                    }
                    // Salva o contexto apenas se todos os registros foram inseridos com sucesso
                    infraMovimento.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    infraMovimentoItem.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    infraCargaErro.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
                    infraSaldo.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);

                    scope.Complete();

                    infraMovimento.AcceptAllChanges();
                    infraMovimentoItem.AcceptAllChanges();
                    infraCargaErro.AcceptAllChanges();
                    infraSaldo.AcceptAllChanges();

                    return isErro;
                }

                catch (Exception e)
                {
                    scope.Dispose();
                    throw new Exception(e.Message, e);
                    //throw new Exception(ErroSistema + e.Message + String.Format("Seq: {0}", sequencia));
                    //return true;
                }
            }

        }

        public TB_MOVIMENTO MontarMovimento(TB_CARGA carga)
        {
            listCargaErro.Clear();

            var importacaoInventario = new ImportacaoInventario();

            //Valida se a carga j� foi cadastrada no banco
            if (!new ImportacaoInventario().ValidarInventarioCadastrado(carga))
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoCadastrado);

            TB_MOVIMENTO movimento = new TB_MOVIMENTO();
            movimento.TB_MOVIMENTO_ITEM = new EntityCollection<TB_MOVIMENTO_ITEM>();

            if (carga.TB_ALMOXARIFADO_ID != null)
                movimento.TB_ALMOXARIFADO_ID = (int)carga.TB_ALMOXARIFADO_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoAlmoxarifadoInvalido);


            movimento.TB_EMPENHO_EVENTO_ID = null;
            movimento.TB_EMPENHO_LICITACAO_ID = null;

            var fornecedor = importacaoInventario.RetornaFornecedorInventarioPadrao();

            if (fornecedor != null)
                movimento.TB_FORNECEDOR_ID = fornecedor.TB_FORNECEDOR_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.FornecedorPadraoNaoCadastrado);


            if (movimento.TB_FORNECEDOR_ID == 0)
                ListCargaErro.Add(GeralEnum.CargaErro.FornecedorPadraoNaoCadastrado);

            movimento.TB_LOGIN_ID = carga.TB_CONTROLE.TB_LOGIN_ID;
            movimento.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = null;

            if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ANO_MES_REFERENCIA))
                movimento.TB_MOVIMENTO_ANO_MES_REFERENCIA = carga.TB_MOVIMENTO_ANO_MES_REFERENCIA;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.AnoMesRefInvalido);

            movimento.TB_MOVIMENTO_ATIVO = true;

            if (importacaoInventario.ValidarData(carga.TB_MOVIMENTO_DATA_DOCUMENTO))
                movimento.TB_MOVIMENTO_DATA_DOCUMENTO = Convert.ToDateTime(carga.TB_MOVIMENTO_DATA_DOCUMENTO);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.DataDocumentoInvalido);

            if (importacaoInventario.ValidarData(carga.TB_MOVIMENTO_DATA_MOVIMENTO))
                movimento.TB_MOVIMENTO_DATA_MOVIMENTO = Convert.ToDateTime(carga.TB_MOVIMENTO_DATA_MOVIMENTO);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.DataMovimentoInvalido);

            if (importacaoInventario.ValidarData(carga.TB_MOVIMENTO_DATA_OPERACAO))
                movimento.TB_MOVIMENTO_DATA_OPERACAO = Convert.ToDateTime(carga.TB_MOVIMENTO_DATA_OPERACAO);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.DataOperacao);

            movimento.TB_MOVIMENTO_EMPENHO = string.Empty;
            movimento.TB_MOVIMENTO_FONTE_RECURSO = string.Empty;
            movimento.TB_MOVIMENTO_INSTRUCOES = string.Empty;
            movimento.TB_MOVIMENTO_NUMERO_DOCUMENTO = carga.TB_MOVIMENTO_NUMERO_DOCUMENTO;
            movimento.TB_MOVIMENTO_OBSERVACOES = "Inserido automaticamente pela rotina de carga.";
            movimento.TB_MOVIMENTO_GERADOR_DESCRICAO = "Transferencia de saldo.";
            //movimento.TB_TIPO_MOVIMENTO_ID = (int)GeralEnum.TipoMovimento.AquisicaoAvulsa;
            movimento.TB_TIPO_MOVIMENTO_ID = (int)GeralEnum.TipoMovimento.EntradaAvulsa;

            //Validar Uge
            if (carga.TB_UGE_ID != null)
                movimento.TB_UGE_ID = carga.TB_UGE_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);

            movimento.TB_MOVIMENTO_VALOR_DOCUMENTO = (decimal)Sam.Common.Util.TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV).Value.truncarDuasCasas();

            movimento.TB_MOVIMENTO_ITEM.Add(MontarMovimentoItem(carga));
            return movimento;
        }

        public TB_MOVIMENTO_ITEM MontarMovimentoItem(TB_CARGA carga)
        {
            var movimentoItem = new TB_MOVIMENTO_ITEM();

            if (carga.TB_SUBITEM_MATERIAL_ID != null)
                movimentoItem.TB_SUBITEM_MATERIAL_ID = (int)carga.TB_SUBITEM_MATERIAL_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoSubitemInvalido);

            movimentoItem.TB_ITEM_MATERIAL_ID = carga.TB_ITEM_MATERIAL_ID;
            movimentoItem.TB_MOVIMENTO_ITEM_ATIVO = true;
            movimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT = new ImportacaoInventario().CalcularPrecoUnitario(carga);
            carga.TB_MOVIMENTO_ITEM_PRECO_UNIT = movimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT.ToString();
            movimentoItem.TB_MOVIMENTO_ITEM_DESD = new ImportacaoInventario().CalcularDesdobro(carga);


            //movimentoItem.TB_MOVIMENTO_ITEM_DESD = carga.TB_MOVIMENTO_ITEM_DESD;
            //movimentoItem.TB_MOVIMENTO_ITEM_EST = carga.TB_MOVIMENTO_ITEM_EST;

            //Trata Lote
            if (carga.TB_SUBITEM_MATERIAL_LOTE == "S")
            {
                //Lote Data de Vencimento
                if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC))//N�o � obrigat�rio
                {
                    if (new ImportacaoInventario().ValidarData(carga.TB_MOVIMENTO_DATA_OPERACAO))
                        movimentoItem.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = Convert.ToDateTime(carga.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC);
                    else
                        ListCargaErro.Add(GeralEnum.CargaErro.DataVencimentoInvalido);
                }

                //Lote Fabrica��o
                if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_LOTE_FABR))
                    movimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR = carga.TB_MOVIMENTO_ITEM_LOTE_FABR.Trim();
                else
                    movimentoItem.TB_MOVIMENTO_ITEM_LOTE_FABR = string.Empty;

                //Lote Identifica��o
                if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_LOTE_IDENT))
                    movimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT = carga.TB_MOVIMENTO_ITEM_LOTE_IDENT.Trim();
                else
                    movimentoItem.TB_MOVIMENTO_ITEM_LOTE_IDENT = string.Empty;
            }

            //movimentoItem.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO = carga.TB_MOVIMENTO_ITEM_NL_LIQUIDACAO;
            //movimentoItem.TB_MOVIMENTO_ITEM_PRECO_UNIT = carga.TB_MOVIMENTO_ITEM_PRECO_UNIT; // calcular
            movimentoItem.TB_MOVIMENTO_ITEM_QTDE_LIQ = null;

            //Qtd Movimento
            if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_QTDE_MOV))
            {
                movimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV = Sam.Common.Util.TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_QTDE_MOV);
                movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = movimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
            }
            else
            {
                movimentoItem.TB_MOVIMENTO_ITEM_QTDE_MOV = null;
                ListCargaErro.Add(GeralEnum.CargaErro.QtdSaldoSubItemInvalido);
            }

            //Saldo Qtd
            //if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_SALDO_QTDE))
            //    movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = Sam.Common.Util.TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_SALDO_QTDE);
            //else
            //{
            //    movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE = null;
            //    ListCargaErro.Add(GeralEnum.CargaErro.QtdSaldoSubItemInvalido);
            //}

            //Saldo Qtd Lote
            if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE))
                movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = Sam.Common.Util.TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE);
            else
            {
                movimentoItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = null;
            }

            ////Saldo Valor
            //if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_SALDO_VALOR))
            //    movimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = Sam.Common.Util.TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_SALDO_VALOR);
            //else
            //{
            //    movimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = null;
            //    ListCargaErro.Add(GeralEnum.CargaErro.ValorSaldoInvalido);
            //}

            //Valor Movimento
            if (!String.IsNullOrEmpty(carga.TB_MOVIMENTO_ITEM_VALOR_MOV))
            {
                movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV = Sam.Common.Util.TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV);
                movimentoItem.TB_MOVIMENTO_ITEM_SALDO_VALOR = movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV;

            }
            else
            {
                movimentoItem.TB_MOVIMENTO_ITEM_VALOR_MOV = null;
                ListCargaErro.Add(GeralEnum.CargaErro.ValorSaldoInvalido);
            }

            //Validar Uge MovimentoItem
            if (carga.TB_UGE_ID != null)
                movimentoItem.TB_UGE_ID = (int)carga.TB_UGE_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);

            return movimentoItem;
        }

        public TB_SALDO_SUBITEM MontarSaldoSubItem(TB_CARGA carga)
        {

            TB_SALDO_SUBITEM saldo = new TB_SALDO_SUBITEM();
            if (listCargaErro.Count > 0) //Se j� tiver algum erro, retorna.
                return saldo;

            saldo.TB_SALDO_SUBITEM_SALDO_QTDE = TratamentoDados.TryParseInt32(carga.TB_MOVIMENTO_ITEM_QTDE_MOV);
            saldo.TB_SALDO_SUBITEM_SALDO_VALOR = TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_VALOR_MOV);
            saldo.TB_SALDO_SUBITEM_LOTE_DT_VENC = TratamentoDados.TryParseDateTime(carga.TB_SALDO_SUBITEM_LOTE_DT_VENC);
            saldo.TB_SALDO_SUBITEM_PRECO_UNIT = TratamentoDados.TryParseDecimal(carga.TB_MOVIMENTO_ITEM_PRECO_UNIT);

            //TB_SALDO_SUBITEM_LOTE_IDENT
            if (!String.IsNullOrEmpty(carga.TB_SALDO_SUBITEM_LOTE_IDENT))
            {
                saldo.TB_SALDO_SUBITEM_LOTE_IDENT = carga.TB_SALDO_SUBITEM_LOTE_IDENT;
            }
            else
            {
                saldo.TB_SALDO_SUBITEM_LOTE_IDENT = null;
            }

            //TB_SALDO_SUBITEM_LOTE_FAB
            if (!String.IsNullOrEmpty(carga.TB_SALDO_SUBITEM_LOTE_FAB))
            {
                saldo.TB_SALDO_SUBITEM_LOTE_FAB = carga.TB_SALDO_SUBITEM_LOTE_FAB;
            }
            else
            {
                saldo.TB_SALDO_SUBITEM_LOTE_FAB = null;
            }

            saldo.TB_ALMOXARIFADO_ID = (int)carga.TB_ALMOXARIFADO_ID;
            saldo.TB_SUBITEM_MATERIAL_ID = (int)carga.TB_SUBITEM_MATERIAL_ID;
            saldo.TB_UGE_ID = (int)carga.TB_UGE_ID;

            return saldo;
        }

        public void ListarMovimento()
        {
            //Exemplo para sort
            Expression<Func<TB_MOVIMENTO, int>> sort = a => a.TB_MOVIMENTO_ID;

            //Exemplo para Where
            Expression<Func<TB_MOVIMENTO, bool>> where = a => a.TB_MOVIMENTO_ID == 1;

            MovimentoInfrastructure persistencia = new MovimentoInfrastructure();

            persistencia.LazyLoadingEnabled = false;
            var result = persistencia.SelectAll();

            persistencia.LazyLoadingEnabled = true;
            var result2 = persistencia.SelectAll();

            var result3 = persistencia.SelectWhere(sort, false, where, 0);


            foreach (var i in result)
            {

            }
        }

        #endregion

        #region M�todos Gen�ricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_MOVIMENTO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                    infraestrutura.Insert(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Update(TB_MOVIMENTO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                    infraestrutura.Update(entity);
                    infraestrutura.SaveChanges();
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui o objeto n�o relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Delete(TB_MOVIMENTO entity)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ID == entity.TB_MOVIMENTO_ID);
                infraestrutura.Delete(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Exclui todos os objetos relecionados
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void DeleteRelatedEntries(TB_MOVIMENTO entity)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ID == entity.TB_MOVIMENTO_ID);
                infraestrutura.DeleteRelatedEntries(entity);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Eclui os objetos relacionados com excess�o a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_MOVIMENTO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_MOVIMENTO_ID == entity.TB_MOVIMENTO_ID);
                infraestrutura.DeleteRelatedEntries(entity, keyListOfIgnoreEntites);
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Retorna o primeiro registro encontrado na condi��o
        /// </summary>
        /// <param name="where">Express�o lambda where</param>
        /// <returns>Objeto</returns>
        public TB_MOVIMENTO SelectOne(Expression<Func<TB_MOVIMENTO, bool>> where)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO> SelectAll()
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                return infraestrutura.SelectAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os objetos encontrados, ordenados e com pagina��o. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="sortExpression">Express�o de ordena��o</param>
        /// <param name="maximumRows">Número m�ximo de registros que ir� retornar</param>
        /// <param name="startRowIndex">índice da pagina��o</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO> SelectAll(Expression<Func<TB_MOVIMENTO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                var result = infraestrutura.SelectAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condi��o, ordenados e com pagina��o. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Express�o lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO> SelectWhere(Expression<Func<TB_MOVIMENTO, bool>> where)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                var result = infraestrutura.SelectWhere(where);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condi��o, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Express�o de ordena��o para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Express�o lambda where</param>
        /// <param name="maximumRows">Número m�ximo de registros que ir� retornar</param>
        /// <param name="startRowIndex">índice da pagina��o</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO> SelectWhere(Expression<Func<TB_MOVIMENTO, int>> sortExpression, bool desc, Expression<Func<TB_MOVIMENTO, bool>> where, int startRowIndex)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condi��o, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Express�o de ordena��o para campos STRING</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Express�o lambda where</param>
        /// <param name="maximumRows">Número m�ximo de registros que ir� retornar</param>
        /// <param name="startRowIndex">índice da pagina��o</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_MOVIMENTO> SelectWhere(Expression<Func<TB_MOVIMENTO, string>> sortExpression, bool desc, Expression<Func<TB_MOVIMENTO, bool>> where, int startRowIndex)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                var result = infraestrutura.SelectWhere(sortExpression, desc, where, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_MOVIMENTO> QueryAll()
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                return infraestrutura.QueryAll();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <param name="sortExpression">Express�o de ordena��o</param>
        /// <param name="desc">Ordem decrescente</param>        
        /// <param name="maximumRows">Número m�ximo de registros que ir� retornar</param>
        /// <param name="startRowIndex">índice da pagina��o</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_MOVIMENTO> QueryAll(Expression<Func<TB_MOVIMENTO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                var result = infraestrutura.QueryAll(sortExpression, startRowIndex);
                this.TotalRegistros = infraestrutura.TotalRegistros;

                return result;
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros encontrados na tabela
        /// </summary>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount()
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                return infraestrutura.GetCount();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna o total de registros na condi��o
        /// </summary>
        /// <param name="where">Express�o lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount(Expression<Func<TB_MOVIMENTO, bool>> where)
        {
            try
            {
                MovimentoInfrastructure infraestrutura = new MovimentoInfrastructure();
                return infraestrutura.GetCount(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion

        #region M�todos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(TB_MOVIMENTO entity)
        {
            try
            {
                if (entity.TB_ALMOXARIFADO_ID == null)
                {
                    throw new Exception("O campo Almoxarifado � de preenchimento obritag�rio");
                }
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        #endregion



        /// <summary>
        /// Retorna os documentos utilizados na lupa de pesquisa
        /// </summary>
        /// <param name="startIndex">Index da pagina��o</param>
        /// <param name="palavraChave">Palavra chave para a busca: Numero documento</param>
        /// <param name="movimento">Dados do movimento, Tipo, Almox, Ativo</param>
        /// <returns></returns>
        public IList<TB_MOVIMENTO> PesquisaDocumentos(int startIndex, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {

            MovimentoInfrastructure infra = new MovimentoInfrastructure();

            var result = infra.PesquisaDocumentos(startIndex, palavraChave, movimento, tipoOperacao);

            foreach (var item in result)
            {
                if (item.TB_ALMOXARIFADO != null)
                    item.TB_ALMOXARIFADO_CODIGO_DESCRICAO = String.Format("{0} - {1}", item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(4, '0'), item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO);
                item.TB_ALMOXARIFADO_DESCRICAO = String.Format("{0}", item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO);
            }

            TotalRegistros = infra.TotalRegistros;

            return result;
        }
        /// <summary>
        /// Retorna os documentos utilizados na lupa de pesquisa
        /// </summary>
        /// <param name="startIndex">Index da pagina��o</param>
        /// <param name="palavraChave">Palavra chave para a busca: Numero documento</param>
        /// <param name="movimento">Dados do movimento, Tipo, Almox, Ativo</param>
        /// <returns></returns>
        public IList<TB_MOVIMENTO> PesquisaDocumentosMesReferencia(int startIndex, string palavraChave, TB_MOVIMENTO movimento, int tipoOperacao)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();

            var result = infra.PesquisaDocumentosMesReferencia(startIndex, palavraChave, movimento, tipoOperacao);

            foreach (var item in result)
            {
                if (item.TB_ALMOXARIFADO != null)
                    item.TB_ALMOXARIFADO_CODIGO_DESCRICAO = String.Format("{0} - {1}", item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_CODIGO.ToString().PadLeft(4, '0'), item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO);
                item.TB_ALMOXARIFADO_DESCRICAO = String.Format("{0}", item.TB_ALMOXARIFADO.TB_ALMOXARIFADO_DESCRICAO);
            }

            TotalRegistros = infra.TotalRegistros;

            return result;



        }
        /// <summary>
        /// Listar todos os documentos para serem recalculados
        /// </summary>
        /// <param name="movimento"></param>
        /// <returns></returns>
        public IList<TB_MOVIMENTO_ITEM> ListarMovimentosRecalcular(TB_MOVIMENTO movimento)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();

            //Retorna todos os movimentos itens do almoxarifado
            var result = infra.ListarMovimentosRecalcular(movimento.TB_ALMOXARIFADO_ID);

            return RecalcularMovimentoItem(result);
        }

        private IList<TB_MOVIMENTO_ITEM> RecalcularMovimentoItem(IList<TB_MOVIMENTO_ITEM> movimentoItem)
        {
            //Cria uma lista com todos os subitens movimentados pelo almoxarifado
            IList<TB_SUBITEM_MATERIAL> subItens = (from m in movimentoItem
                                                   select new TB_SUBITEM_MATERIAL()
                                                   {
                                                       TB_SUBITEM_MATERIAL_ID = m.TB_SUBITEM_MATERIAL_ID
                                                   }).ToList();
            subItens = subItens.Distinct().ToList();

            foreach (var item in subItens)
            {
                foreach (var movItem in movimentoItem)
                {
                    if (movItem.TB_SUBITEM_MATERIAL_ID == item.TB_SUBITEM_MATERIAL_ID)
                    {
                        if (movItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Entrada)
                        {
                            movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_REC += movItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
                            movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR_REC += movItem.TB_MOVIMENTO_ITEM_VALOR_MOV;
                        }
                        else if (movItem.TB_MOVIMENTO.TB_TIPO_MOVIMENTO.TB_TIPO_MOVIMENTO_AGRUPAMENTO_ID == (int)Common.Util.GeralEnum.TipoMovimentoAgrupamento.Saida)
                        {
                            movItem.TB_MOVIMENTO_ITEM_SALDO_QTDE_REC -= movItem.TB_MOVIMENTO_ITEM_QTDE_MOV;
                            movItem.TB_MOVIMENTO_ITEM_SALDO_VALOR_REC -= movItem.TB_MOVIMENTO_ITEM_VALOR_MOV;
                        }
                    }
                }
            }

            return movimentoItem;
        }
        public void recalculaSubtItem(Int32? almoxarifadoId, Int64? subtItemCodigo, int gestorId)
        {
            Sam.Domain.Business.MovimentoBusiness _business = new Domain.Business.MovimentoBusiness();
            IList<MovimentoItemEntity> lista = _business.getListaItensCorrigidos(almoxarifadoId, subtItemCodigo, gestorId);
            MovimentoInfrastructure infra = new MovimentoInfrastructure();


            if (!subtItemCodigo.HasValue)
                deletaReacalculoPorAlmoxarifadoSubItem(almoxarifadoId.Value, subtItemCodigo.Value);
            else
                deletaReacalculoPorAlmoxarifado(almoxarifadoId.Value);


            foreach (MovimentoItemEntity item in lista)
            {
                infra.InserirRecalculo(item);
            }

            infra = null;
        }

        private void deletaReacalculoPorAlmoxarifado(Int32 almoxarifadoId)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();

            infra.DeletarRecalculo(almoxarifadoId, 0);

            infra = null;
        }

        private void deletaReacalculoPorAlmoxarifadoSubItem(Int32 almoxarifadoId, Int64 subItemCodigo)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();

            infra.DeletarRecalculo(almoxarifadoId, subItemCodigo);

            infra = null;
        }
        #region m�todos movimento pendente
        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> ListaDeMovimentoPendente(int startIndex, Int32? almoxarifadoId, Int64? subtItemCodigo)
        {
            var result = infraMovimentoPendente.RetornarListaDeMovimentoPendente(startIndex, almoxarifadoId, subtItemCodigo);
            TotalRegistros = infraMovimentoPendente.TotalRegistros;
            return result;

        }
        public Int32 getSubItensCount(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> lista)
        {
            var result = infraMovimentoPendente.getSubItensCount(lista);
            return result;

        }

        public IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> getListaDeMovimentoPendente()
        {
            return infraMovimentoPendente.getListaDeMovimentoPendente();
        }
        public void setListaDeMovimentoPendente(IList<Sam.Infrastructure.Infrastructure.Interface.IMovimentoPendente> ListaDeMovimentoPendente)
        {
            infraMovimentoPendente.setListaDeMovimentoPendente(ListaDeMovimentoPendente);
        }

        #endregion

        public DataSet GerarExportacaoAnalitica(int w_OrgaoId
                                                            , int r_Periodo = 0
                                                            , int r_UGECodigo = 0
                                                            , int r_UGEDescricao = 0
                                                            , int r_NaturezaDespesaCodigo = 0
                                                            , int r_NaturezaDespesaDescricao = 0
                                                            , int r_SubitemCodigo = 0
                                                            , int r_SubitemDescricao = 0
                                                            , int r_UnidadeFornecimento = 0
                                                            , int r_DataMovimento = 0
                                                            , int r_TipoMovimentoDescricao = 0
                                                            , int r_SituacaoDescricao = 0
                                                            , int r_FornecedorDescricao = 0
                                                            , int r_NumeroDoc = 0
                                                            , int r_Lote = 0
                                                            , int r_VenctoDoc = 0
                                                            , int r_QtdeDoc = 0
                                                            , int r_ValorUnitario = 0

                                                            , int r_PrecoUnitario_EMP = 0

                                                            , int r_Desdobro = 0
                                                            , int r_TotalDoc = 0
                                                            , int r_QtdeSaldo = 0
                                                            , int r_ValorSaldo = 0

                                                            , int r_UaCodigoRequisicao = 0
                                                            , int r_UaDescricaoRequisicao = 0
                                                            , int r_Divisao = 0
                                                            , int r_UGECodigoDestino = 0
                                                            , int r_UGEDescricaoDestino = 0

                                                            , int r_InformacoesComplementares = 0
                                                            , int r_NLConsumo = 0
                                                            , int r_NLLiquidacao = 0
                                                            , int r_NLLiquidacaoEstorno = 0
                                                            , string w_AnoMesRefDe = ""
                                                            , string w_AnoMesRefAte = ""
                                                            , string w_Fornecedor = ""
                                                            , string w_LoteDescricao = ""
                                                            , string w_NaturezaDespesaCodigo = ""
                                                            , string w_NumeroDoc = ""
                                                            , string w_SaldoQuantidade = ""
                                                            , string w_SaldoValor = ""
                                                            , string w_SubitemCodigo = ""
                                                            , string w_TipoMovimentoId = ""
                                                            , string w_UgeCodigo = ""
                                                            , string w_NLConsumo = ""
                                                            , string w_NLLiquidacao = ""
                                                            , string w_NLLiquidacaoEstorno = ""
                                                            , string w_UA_Id_Destino = ""
                                                            , string w_UGE_Id_Destino = "")
        {
            DataSet dsRetorno = new DataSet();
            try
            {



                MovimentoInfrastructure infra = new MovimentoInfrastructure();
                dsRetorno = infra.GerarExportacaoAnalitica(w_OrgaoId
                                                            , r_Periodo
                                                            , r_UGECodigo
                                                            , r_UGEDescricao
                                                            , r_NaturezaDespesaCodigo
                                                            , r_NaturezaDespesaDescricao
                                                            , r_SubitemCodigo
                                                            , r_SubitemDescricao
                                                            , r_UnidadeFornecimento
                                                            , r_DataMovimento
                                                            , r_TipoMovimentoDescricao
                                                            , r_SituacaoDescricao
                                                            , r_FornecedorDescricao
                                                            , r_NumeroDoc
                                                            , r_Lote
                                                            , r_VenctoDoc
                                                            , r_QtdeDoc
                                                            , r_ValorUnitario


                                                            , r_PrecoUnitario_EMP

                                                            , r_Desdobro
                                                            , r_TotalDoc
                                                            , r_QtdeSaldo
                                                            , r_ValorSaldo

                                                            , r_UaCodigoRequisicao
                                                            , r_UaDescricaoRequisicao
                                                            , r_Divisao
                                                            , r_UGECodigoDestino
                                                            , r_UGEDescricaoDestino

                                                            , r_InformacoesComplementares
                                                            , r_NLConsumo
                                                            , r_NLLiquidacao
                                                            , r_NLLiquidacaoEstorno
                                                            , w_AnoMesRefDe
                                                            , w_AnoMesRefAte
                                                            , w_Fornecedor
                                                            , w_LoteDescricao
                                                            , w_NaturezaDespesaCodigo
                                                            , w_NumeroDoc
                                                            , w_SaldoQuantidade
                                                            , w_SaldoValor
                                                            , w_SubitemCodigo
                                                            , w_TipoMovimentoId
                                                            , w_UgeCodigo
                                                            , w_NLConsumo
                                                            , w_NLLiquidacao
                                                            , w_NLLiquidacaoEstorno
                                                            , w_UA_Id_Destino
                                                            , w_UGE_Id_Destino);
                return dsRetorno;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataSet GerarExportacaoSintetico(int w_OrgaoId
                                                            , int r_UGECodigo = 0
                                                            , int r_UGEDescricao = 0
                                                            , int r_GrupoMaterial = 0
                                                            , int r_SubitemCodigo = 0
                                                            , int r_SubitemDescricao = 0
                                                            , int r_UnidadeFornecimento = 0
                                                            , int r_QtdeSaldo = 0
                                                            , int r_ValorSaldo = 0
                                                            , int r_PrecoMedio = 0
                                                            , int r_LoteDescricao = 0
                                                            , int r_LoteQtde = 0
                                                            , int r_LoteDataVencimento = 0
                                                            , int r_NaturezaDespesaCodigo = 0
                                                            , int r_NaturezaDespesaDescricao = 0
                                                            , string w_AnoMesRefDe = ""
                                                            , string w_AnoMesRefAte = ""
                                                            , string w_GrupoMaterial = ""
                                                            , string w_UgeCodigo = ""
                                                            , int w_ComSemSaldo = 0)
        {
            DataSet dsRetorno = new DataSet();
            try
            {


                MovimentoInfrastructure infra = new MovimentoInfrastructure();
                dsRetorno = infra.GerarExportacaoSintetica(w_OrgaoId
                                                            , r_UGECodigo
                                                            , r_UGEDescricao
                                                            , r_GrupoMaterial
                                                            , r_SubitemCodigo
                                                            , r_SubitemDescricao
                                                            , r_UnidadeFornecimento
                                                            , r_QtdeSaldo
                                                            , r_ValorSaldo
                                                            , r_PrecoMedio
                                                            , r_LoteDescricao
                                                            , r_LoteQtde
                                                            , r_LoteDataVencimento
                                                            , r_NaturezaDespesaCodigo
                                                            , r_NaturezaDespesaDescricao
                                                            , w_AnoMesRefDe
                                                            , w_AnoMesRefAte
                                                            , w_GrupoMaterial
                                                            , w_UgeCodigo
                                                            , w_ComSemSaldo);

                return dsRetorno;
            }
            catch (Exception)
            {

                throw;
            }
        }


        public TB_UGE RetornarMovimentoAlmoxUge(string numeroDocumento)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();
            return infra.RetornarMovimentoAlmoxUge(numeroDocumento);

        }

        public TB_MOVIMENTO RetornarMovimentoSaidaFornecimento(int divisao, string numeroDocumento)
        {
            MovimentoInfrastructure infra = new MovimentoInfrastructure();
            return infra.RetornarMovimentoSaidaFornecimento(divisao, numeroDocumento);

        }

    }
}
