using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Domain.Entity;
using LinqToExcel;
using Sam.Common.Util;


namespace Sam.Business.Importacao
{
    public class ImportacaoCargaControle
    {

        public static decimal _valorZero = 0.0000m;




        /// <summary>
        /// Faz a leitura do arquivo excel e insere na tabela Carga
        /// </summary>
        /// <param name="cargaEntity">Dados para realizar a leitura do excel</param>
        /// <param name="loginId">Login do usuário logado</param>
        /// <returns></returns>
        public virtual IQueryable<TB_CARGA> ProcessarExcel(CargaEntity cargaEntity, int loginId)
        {
            string nomeAbaExcel = "tblCarga";
            StringBuilder TB_CARGA_SEQ = new StringBuilder(); ;

            var excel = new ExcelQueryFactory(cargaEntity.CaminhoDiretorio + cargaEntity.NomeArquivo);
            excel.DatabaseEngine = LinqToExcel.Domain.DatabaseEngine.Ace;
            var carga = from c in excel.Worksheet<TB_CARGA>(nomeAbaExcel)
                        select c;

            CargaInfrastructure Infrastructure = new CargaInfrastructure();
            try
            {
                //É mais rapido carregar todos os registros na memória com IQueryable
                TB_CONTROLE controle = new TB_CONTROLE();
                controle.TB_CONTROLE_DATA_OPERACAO = DateTime.Now;
                controle.TB_CONTROLE_NOME_ARQUIVO = cargaEntity.NomeArquivo;
                controle.TB_LOGIN_ID = loginId;
                controle.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.Pendente;
                controle.TB_TIPO_CONTROLE_ID = cargaEntity.TipoArquivo;

                List<TB_CARGA> linhasParaGravar = this.ProcessarImportacaoEspecifica(carga, controle);

                foreach (var c in linhasParaGravar)
                {

                    //TB_CARGA_SEQ.Clear();
                    //TB_CARGA_SEQ.Append(c.TB_CARGA_SEQ);
                    Infrastructure.Insert(c);
                }

                Infrastructure.SaveChanges();
            }
            catch (Exception ex)
            {
                string fmtMsgErro = (TB_CARGA_SEQ.IsNotNull()) ? String.Format("{0} Seq.: {1}", ex.Message, TB_CARGA_SEQ) : String.Format("{0}", ex.Message);
                throw new Exception(fmtMsgErro, ex.InnerException);
            }

            return carga;
        }

        public List<TB_CARGA> ProcessarImportacaoEspecifica(IQueryable<TB_CARGA> linhasPlanilha, TB_CONTROLE pObjControle)
        {
            try
            {
                //Retorna o objeto controle com todos as cargas
                int tipoCargaImportacao = pObjControle.TB_TIPO_CONTROLE_ID.Value;
                //var controle = new ControleBusiness().QueryAll(a => a.TB_CONTROLE_ID == tipoCargaImportacao, LazyLoadingEnabled).FirstOrDefault();

                List<TB_CARGA> lLstRetorno = new List<TB_CARGA>();

                //Verifica qual tipo de importação irá realizar
                switch (tipoCargaImportacao)
                {

                    case (int)GeralEnum.TipoControle.SubItemMaterial:
                        lLstRetorno = new ImportacaoSubItemMaterial().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.CatalogoAlmoxarifado:
                        lLstRetorno = new ImportacaoCatalogo().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.InventarioAlmoxarifado:
                        lLstRetorno = new ImportacaoInventario().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.Divisao:
                        lLstRetorno = new ImportacaoDivisao().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.UnidadeAdm:
                        lLstRetorno = new ImportacaoUA().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.Almoxarifado:
                        lLstRetorno = new ImportacaoAlmoxarifado().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.Responsavel:
                        lLstRetorno = new ImportacaoResponsavel().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.Usuario:
                        lLstRetorno = new ImportacaoResponsavel().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    case (int)GeralEnum.TipoControle.PerfilRequisitante:
                        lLstRetorno = new ImportacaoResponsavel().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                        break;
                    default:
                        throw new Exception("Tipo de carga não foi implementado");

                }



                //if (tipoCargaImportacao == (int)GeralEnum.TipoControle.SubItemMaterial)
                //    lLstRetorno = new ImportacaoSubItemMaterial().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                //else if (tipoCargaImportacao == (int)GeralEnum.TipoControle.CatalogoAlmoxarifado)
                //    lLstRetorno = new ImportacaoCatalogo().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                //else if (tipoCargaImportacao == (int)GeralEnum.TipoControle.InventarioAlmoxarifado)
                //    lLstRetorno = new ImportacaoInventario().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                //else if (tipoCargaImportacao == (int)GeralEnum.TipoControle.Divisao)
                //    lLstRetorno = new ImportacaoDivisao().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                //else if (tipoCargaImportacao == (int)GeralEnum.TipoControle.UnidadeAdm)
                //    lLstRetorno = new ImportacaoUA().ProcessarExcel(linhasPlanilha, pObjControle, tipoCargaImportacao);
                //else
                //    throw new Exception("Tipo de carga não foi implementado");

                return lLstRetorno;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Atualiza o status do controle conforme o retorno do erro
        /// </summary>
        /// <param name="isErro">Se retornou erro</param>
        /// <param name="controleId">Id do controle</param>
        protected void AtualizaControle(bool isErro, int controleId)
        {
            //Busca o objeto atualizado para fazer o update
            var controleUpdate = new ControleBusiness().SelectOne(a => a.TB_CONTROLE_ID == controleId);

            if (isErro)
                controleUpdate.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.FinalizadaErro;
            else
                controleUpdate.TB_CONTROLE_SITUACAO_ID = (int)GeralEnum.ControleSituacao.FinalizadoSucesso;

            new ControleBusiness().Update(controleUpdate);
        }

        //Metodo Generico para o processamento do excel. Caso precise customiza-lo fazer um overrride
        public virtual List<TB_CARGA> ProcessarExcel(IQueryable<TB_CARGA> linhasPlanilha, TB_CONTROLE objetoControlador, int tipoCargaImportacao)
        {


            switch (tipoCargaImportacao)
            {

                case (int)GeralEnum.TipoControle.Almoxarifado:

                    var pl = linhasPlanilha.ToList().GroupBy(n => new { n.TB_ALMOXARIFADO_CODIGO, n.TB_UGE_CODIGO, n.TB_GESTOR_NOME_REDUZIDO },
                    (key, values) => new { Item = key, Count = values.Count() });


                    foreach (var item in pl)
                    {
                        if (item.Count > 1)
                            throw new Exception("Itens repetidos na planilha");
                    }
                    break;

                case (int)GeralEnum.TipoControle.Responsavel:
                    var plr = linhasPlanilha.ToList().GroupBy(n => new { n.TB_RESPONSAVEL_CODIGO, n.TB_GESTOR_NOME_REDUZIDO },
                   (key, values) => new { Item = key, Count = values.Count() });

                    foreach (var item in plr)
                    {
                        if (item.Count > 1)
                            throw new Exception("Itens repetidos na planilha");
                    }

                    break;

                case (int)GeralEnum.TipoControle.Divisao:
                    var pld = linhasPlanilha.ToList().GroupBy(n => new { n.TB_DIVISAO_CODIGO, n.TB_GESTOR_NOME_REDUZIDO, n.TB_UA_CODIGO },
                   (key, values) => new { Item = key, Count = values.Count() });

                    foreach (var item in pld)
                    {
                        if (item.Count > 1)
                            throw new Exception("Itens repetidos na planilha");
                    }

                    break;

                case (int)GeralEnum.TipoControle.Usuario:               
                    var plu = linhasPlanilha.ToList().GroupBy(n => new { n.TB_USUARIO_CPF },
                   (key, values) => new { Item = key, Count = values.Count() });

                    foreach (var item in plu)
                    {
                        if (item.Count > 1)
                            throw new Exception("Itens repetidos na planilha");
                    }

                    break;
                case (int)GeralEnum.TipoControle.PerfilRequisitante:
                    var plp = linhasPlanilha.ToList().GroupBy(n => new { n.TB_USUARIO_CPF, n.TB_UA_CODIGO, n.TB_DIVISAO_CODIGO },
                  (key, values) => new { Item = key, Count = values.Count() });

                    foreach (var item in plp)
                    {
                        if (item.Count > 1)
                            throw new Exception("Itens repetidos na planilha");
                    }

                    break;
                
                default:

                    break;
            }

            string numeroDocumento = String.Format("{0}{1}{2}", DateTime.Now.Hour.ToString().PadLeft(2, '0')
                , DateTime.Now.Minute.ToString().PadLeft(2, '0')
                , DateTime.Now.Second.ToString()).ToString().PadLeft(2, '0');

            List<TB_CARGA> lLstRetorno = new List<TB_CARGA>();

            StringBuilder TB_CARGA_SEQ = new StringBuilder();
            List<string> listaErros = new List<string>();

            try
            {
                IQueryable<TB_ITEM_MATERIAL> itemMaterialList;
                itemMaterialList = new ItemMaterialBusiness().QueryAll();

                IQueryable<TB_SUBITEM_MATERIAL> subItemMaterialList;
                subItemMaterialList = new SubItemMaterialBusiness().QueryAll();

                IQueryable<TB_CONTA_AUXILIAR> contaAuxList;
                contaAuxList = new ContaAuxiliarBusiness().QueryAll();

                IQueryable<TB_NATUREZA_DESPESA> naturezaList;
                naturezaList = new NaturezaDespesaBusiness().QueryAll();

                IQueryable<TB_UNIDADE_FORNECIMENTO> unidadeList;
                unidadeList = new UnidadeFornecimentoBusiness().QueryAll();

                IQueryable<TB_ALMOXARIFADO> almoxList;
                almoxList = new AlmoxarifadoBusiness().QueryAll();

                IQueryable<TB_GESTOR> gestorList;
                gestorList = new GestorBusiness().QueryAll();

                IQueryable<TB_UGE> ugeList;
                ugeList = new UgeBusiness().QueryAll();

                IQueryable<TB_UA> uaList;
                uaList = new UaBusiness().QueryAll();

                IQueryable<TB_DIVISAO> divisaoList;
                divisaoList = new DivisaoBusiness().QueryAll();

                IQueryable<TB_UF> ufList;
                ufList = new UfBusiness().QueryAll();

                IQueryable<TB_RESPONSAVEL> uRespList;
                uRespList = new ResponsavelBusiness().QueryAll();

                UsuarioBusiness usuarioList;
                usuarioList = new UsuarioBusiness();


                foreach (var c in linhasPlanilha.ToList())
                {
                    if (String.IsNullOrEmpty(c.TB_CARGA_SEQ))
                    {
                        throw new Exception("O Campo sequêncial é de preenchimento obrigatório e deve ser preenchido apenas com números em sequência ascendente.");
                    }

                    //Utilizado para mostrar na exception
                    TB_CARGA_SEQ.Clear();
                    TB_CARGA_SEQ.Append(c.TB_CARGA_SEQ);

                    //Adiciona o controle
                    c.TB_CONTROLE = objetoControlador;

                    var gestor = RetornaGestor(gestorList, c.TB_GESTOR_NOME_REDUZIDO);
                    if (gestor != null)
                    {
                        c.TB_GESTOR_ID = gestor.TB_GESTOR_ID;
                        c.TB_ORGAO_ID = gestor.TB_ORGAO_ID;

                    }


                    var uge = new TB_UGE();
                    if (!string.IsNullOrEmpty(c.TB_UGE_CODIGO))
                    {
                        uge = RetornaUnidadeGestora(ugeList, c.TB_UGE_CODIGO);
                        if (uge != null)
                            c.TB_UGE_ID = uge.TB_UGE_ID;
                    }

                    var almoxarifado = RetornaAlmoxarifado(almoxList, c.TB_ALMOXARIFADO_CODIGO, c.TB_GESTOR_ID, c.TB_UGE_ID);
                    if (almoxarifado != null)
                        c.TB_ALMOXARIFADO_ID = almoxarifado.TB_ALMOXARIFADO_ID;

                    switch (tipoCargaImportacao)
                    {

                        case (int)GeralEnum.TipoControle.SubItemMaterial:
                        case (int)GeralEnum.TipoControle.CatalogoAlmoxarifado:
                            //Carrega Código de subItem Material
                            if (!String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_CODIGO) && !contemLetras(c.TB_SUBITEM_MATERIAL_CODIGO))
                            {
                                var subItemMaterial = RetornaSubItemMaterial(subItemMaterialList, c.TB_SUBITEM_MATERIAL_CODIGO, c.TB_GESTOR_ID);
                                if (subItemMaterial != null)
                                    c.TB_SUBITEM_MATERIAL_ID = subItemMaterial.TB_SUBITEM_MATERIAL_ID;
                            }
                            else
                                throw new Exception("O Campo Código do Subitem Material é de preenchimento obrigatório e deve ser preenchido apenas com números.");


                            if (tipoCargaImportacao != (int)GeralEnum.TipoControle.CatalogoAlmoxarifado)
                            {
                                //Carrega Descrição de subitem Material
                                if (String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_DESCRICAO))
                                    throw new Exception("O Campo Descrição do Subitem Material é de preenchimento obrigatório.");
                            }
                            var contaAux = RetornaContaAuxiliar(contaAuxList, c.TB_CONTA_AUXILIAR_CODIGO);
                            if (contaAux != null)
                                c.TB_CONTA_AUXILIAR_ID = contaAux.TB_CONTA_AUXILIAR_ID;

                            var naturezaDespesa = Retornanatureza(naturezaList, c.TB_NATUREZA_DESPESA_CODIGO, c.TB_ITEM_MATERIAL_ID, c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE);
                            if (naturezaDespesa != null)
                                c.TB_NATUREZA_DESPESA_ID = naturezaDespesa.TB_NATUREZA_DESPESA_ID;



                            var unidadeFornecimento = RetornaUnidadeFornecimento(unidadeList, c.TB_UNIDADE_FORNECIMENTO_CODIGO, c.TB_GESTOR_ID);
                            if (unidadeFornecimento != null)
                                c.TB_UNIDADE_FORNECIMENTO_ID = unidadeFornecimento.TB_UNIDADE_FORNECIMENTO_ID;

                            var unidadeGestora = RetornaUnidadeGestora(ugeList, c.TB_UGE_CODIGO);
                            if (unidadeGestora != null)
                                c.TB_UGE_ID = unidadeGestora.TB_UGE_ID;

                            if ((!String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_LOTE)) && c.TB_SUBITEM_MATERIAL_LOTE.ToUpper() == "S")
                                c.TB_SUBITEM_MATERIAL_LOTE = "S";
                            else
                                c.TB_SUBITEM_MATERIAL_LOTE = "N";

                            if ((!String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE)) && c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE.ToUpper() == "S")
                                c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = "S";
                            else
                                c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = "N";

                            //Estoque Max
                            if (c.TB_SUBITEM_MATERIAL_ESTOQUE_MAX == null)
                            {
                                c.TB_SUBITEM_MATERIAL_ESTOQUE_MAX = "0";
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_ESTOQUE_MAX.Trim()))
                                    c.TB_SUBITEM_MATERIAL_ESTOQUE_MAX = "0";
                            }

                            //Estoque Min
                            if (c.TB_SUBITEM_MATERIAL_ESTOQUE_MIN == null)
                            {
                                c.TB_SUBITEM_MATERIAL_ESTOQUE_MIN = "0";
                            }
                            else
                            {
                                if (String.IsNullOrEmpty(c.TB_SUBITEM_MATERIAL_ESTOQUE_MIN.Trim()))
                                    c.TB_SUBITEM_MATERIAL_ESTOQUE_MIN = "0";
                            }

                            //Indicador de disponível

                            if (c.TB_INDICADOR_DISPONIVEL_DESCRICAO == null)
                            {
                                c.TB_INDICADOR_DISPONIVEL_DESCRICAO = "N";
                            }
                            else
                            {
                                if (c.TB_INDICADOR_DISPONIVEL_DESCRICAO.ToUpper() == "S")
                                {
                                    c.TB_INDICADOR_DISPONIVEL_DESCRICAO = "S";
                                    c.TB_INDICADOR_DISPONIVEL_ID = (int)GeralEnum.IndicadorDisponivel.Sim;
                                }
                                else if (c.TB_INDICADOR_DISPONIVEL_DESCRICAO.ToUpper() == "L")
                                {
                                    c.TB_INDICADOR_DISPONIVEL_DESCRICAO = "L";
                                    c.TB_INDICADOR_DISPONIVEL_ID = (int)GeralEnum.IndicadorDisponivel.SimAteLimite;
                                }
                                else
                                {
                                    c.TB_INDICADOR_DISPONIVEL_DESCRICAO = "N";
                                    c.TB_INDICADOR_DISPONIVEL_ID = (int)GeralEnum.IndicadorDisponivel.Nao;
                                }
                            }

                            if (!String.IsNullOrEmpty(c.TB_MOVIMENTO_ANO_MES_REFERENCIA))
                            {
                                string anoMes = c.TB_MOVIMENTO_ANO_MES_REFERENCIA.Replace("/", "").Trim().PadLeft(4, '0');

                                //Numero Documento
                                c.TB_MOVIMENTO_NUMERO_DOCUMENTO = anoMes + numeroDocumento.Trim().PadLeft(6, '0');

                                //Data
                                c.TB_MOVIMENTO_DATA_DOCUMENTO = DateTime.Now.ToString(); //Data Atual

                                if (anoMes.Length > 4)
                                {
                                    c.TB_MOVIMENTO_DATA_MOVIMENTO = String.Format("01/{0}/{1}", anoMes.Remove(0, 4), anoMes.Remove(4, 2));
                                }

                                c.TB_MOVIMENTO_DATA_OPERACAO = DateTime.Now.ToString(); //Data Atual
                            }

                            if (tipoCargaImportacao != (int)GeralEnum.TipoControle.CatalogoAlmoxarifado)
                            {
                                // carrega Código de item material
                                if (!String.IsNullOrEmpty(c.TB_ITEM_MATERIAL_CODIGO) && !contemLetras(c.TB_ITEM_MATERIAL_CODIGO))
                                {
                                    var itemMaterial = RetornaItemMaterial(itemMaterialList, c.TB_ITEM_MATERIAL_CODIGO);

                                    //if (itemMaterial.IsNull())
                                    //{
                                    //    listaErros.Add(String.Format("Linha {0} - Item Material {1} não está cadastrado na base de dados do sistema!\n\n", c.TB_CARGA_SEQ, c.TB_ITEM_MATERIAL_CODIGO));
                                    //    continue;
                                    //}
                                    if (itemMaterial != null)
                                        c.TB_ITEM_MATERIAL_ID = itemMaterial.TB_ITEM_MATERIAL_ID;
                                }
                                else
                                    throw new Exception("O Campo Código do Item Material é de preenchimento obrigatório e deve ser preenchido apenas com números.");
                            }

                            break;

                        case (int)GeralEnum.TipoControle.Divisao:


                            var ua = RetornaUA(uaList, c.TB_UA_CODIGO, c.TB_GESTOR_ID);
                            if (!string.IsNullOrEmpty(c.TB_UA_CODIGO))
                            {
                                if (ua != null)
                                    c.TB_UA_ID = ua.TB_UA_ID;
                            }
                            else
                                throw new Exception("O Campo UA é de preenchimento obrigatório e deve ser preenchido apenas com Código.");


                            if (string.IsNullOrEmpty(c.TB_DIVISAO_CODIGO))
                                throw new Exception("O Campo CÓDIGO é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_DESCRICAO))
                            //    throw new Exception("O Campo DESCRIÇÃO é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_LOGRADOURO))
                            //    throw new Exception("O Campo LOGRADOURO é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_NUMERO))
                            //    throw new Exception("O Campo NÚMERO é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_BAIRRO))
                            //    throw new Exception("O Campo BAIRRO é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_COMPLEMENTO))
                            //    throw new Exception("O Campo COMPLEMENTO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_DIVISAO_MUNICIPIO))
                                throw new Exception("O Campo MUNCIPIO é de preenchimento obrigatório.");

                            var uf = RetornaUF(ufList, c.TB_UF_SIGLA);
                            if (uf != null)
                                c.TB_UF_ID = uf.TB_UF_ID;
                            //else
                            //    throw new Exception("O Campo UF é de preenchimento obrigatório e deve ser preenchido apenas com a Sigla do estado.");


                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_CEP))
                            //    throw new Exception("O Campo CEP é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_TELEFONE))
                            //    throw new Exception("O Campo TELEFONE é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_FAX))
                            //    throw new Exception("O Campo FAX é de preenchimento obrigatório.");

                            //if (string.IsNullOrEmpty(c.TB_DIVISAO_INDICADOR_ATIVIDADE))
                            //    throw new Exception("O Campo INDICADOR ATIVIDADE é de preenchimento obrigatório.");

                            var Resp = RetornaResponsavel(uRespList, c.TB_RESPONSAVEL_CODIGO, c.TB_GESTOR_ID);
                            if (Resp != null)
                                c.TB_RESPONSAVEL_ID = Resp.TB_RESPONSAVEL_ID;


                            break;

                        case (int)GeralEnum.TipoControle.Almoxarifado:

                            if (uge == null)
                                throw new Exception("O Campo CÓDIGO da UGE é de preenchimento obrigatório.");


                            if ((!String.IsNullOrEmpty(c.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE)) && c.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE.ToUpper() == "S")
                                c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = "S";
                            else
                                c.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = "N";


                            var ufSigla = RetornaUF(ufList, c.TB_UF_SIGLA);
                            if (ufSigla != null)
                                c.TB_UF_ID = ufSigla.TB_UF_ID;
                            else
                                throw new Exception("O Campo TB_UF_SIGLA é de preenchimento obrigatório.");


                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_CODIGO))
                                throw new Exception("O Campo CÓDIGO ALMOXARIFADO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_DESCRICAO))
                                throw new Exception("O Campo DESCRIÇÃO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_LOGRADOURO))
                                throw new Exception("O Campo LOGRADOURO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_NUMERO))
                                throw new Exception("O Campo NÚMERO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_BAIRRO))
                                throw new Exception("O Campo BAIRRO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_MUNICIPIO))
                                throw new Exception("O Campo MUNCIPIO é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_CEP))
                                throw new Exception("O Campo CEP é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_MES_REF_INICIAL))
                                throw new Exception("O Campo MES REFERENCIA é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE))
                                throw new Exception("O Campo INDICADOR ATIVIDADE é de preenchimento obrigatório.");

                            break;

                        case (int)GeralEnum.TipoControle.Responsavel:

                            if (!string.IsNullOrEmpty(c.TB_RESPONSAVEL_CODIGO))
                            {
                                var RespId = RetornaResponsavel(uRespList, c.TB_RESPONSAVEL_CODIGO, c.TB_GESTOR_ID);
                                if (RespId != null)
                                {
                                    c.TB_RESPONSAVEL_ID = RespId.TB_RESPONSAVEL_ID;
                                }
                            }
                            else
                                throw new Exception("O Campo CÓDIGO RESPONSAVEL é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_RESPONSAVEL_NOME))
                                throw new Exception("O Campo NOME é de preenchimento obrigatório.");


                            break;

                        case (int)GeralEnum.TipoControle.Usuario:

                            if (!string.IsNullOrEmpty(c.TB_USUARIO_CPF))
                            {
                                var Usuario = usuarioList.ExisteCPFCadastrado(c.TB_USUARIO_CPF);
                                if (Usuario != null)
                                {
                                    c.TB_USUARIO_ID = Usuario.Id;
                                }
                            }
                            else
                                throw new Exception("O Campo CPF é de preenchimento obrigatório.");

                            if (string.IsNullOrEmpty(c.TB_USUARIO_NUM_RG))
                                c.TB_USUARIO_NUM_RG = string.Empty;

                            if (string.IsNullOrEmpty(c.TB_USUARIO_RG_UF))
                                c.TB_USUARIO_RG_UF = string.Empty;

                            if (string.IsNullOrEmpty(c.TB_USUARIO_ORGAO_EMISSOR))
                                c.TB_USUARIO_ORGAO_EMISSOR = string.Empty;



                            break;

                        case (int)GeralEnum.TipoControle.PerfilRequisitante:

                            if (!string.IsNullOrEmpty(c.TB_USUARIO_CPF))
                            {
                                var Usuario = usuarioList.ExisteCPFCadastrado(c.TB_USUARIO_CPF);
                                if (Usuario != null)
                                {
                                    c.TB_USUARIO_ID = Usuario.Id;
                                }
                            }
                            else
                                throw new Exception("O Campo CPF é de preenchimento obrigatório.");

                            if(c.TB_GESTOR_ID == null)
                                throw new Exception("Gestor não esta cadastrado");


                            if (!string.IsNullOrEmpty(c.TB_UA_CODIGO))
                            {
                                var uaP = RetornaUA(uaList, c.TB_UA_CODIGO, c.TB_GESTOR_ID);

                                if (uaP != null)
                                {
                                    c.TB_UA_ID = uaP.TB_UA_ID;
                                    c.TB_UGE_ID = uaP.TB_UGE_ID;

                                    if (!string.IsNullOrEmpty(c.TB_DIVISAO_CODIGO))
                                    {
                                        var divisaoP = RetornaDivisao(divisaoList, c.TB_DIVISAO_CODIGO, uaP.TB_UA_ID);

                                        if (divisaoP != null)
                                            c.TB_DIVISAO_ID = divisaoP.TB_DIVISAO_ID;
                                    }
                                    else
                                        throw new Exception("O Campo DIVISÃO é de preenchimento obrigatório e deve ser preenchido apenas com Código.");

                                }
                            }
                            else
                                throw new Exception("O Campo UA é de preenchimento obrigatório e deve ser preenchido apenas com Código.");





                            break;

                        default:

                            break;


                    }


                    lLstRetorno.Add(c);
                }

                if (listaErros.HasElements())
                {
                    string saidaConsolidada = string.Empty;

                    listaErros.ForEach(linhaErro => saidaConsolidada = String.Format("{0}{1}", saidaConsolidada, linhaErro));
                    throw new Exception(String.Format("Erro ao efetuar carga de subitens!\n\n{0}", saidaConsolidada));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lLstRetorno;
        }

        #region Retorna os dados das tabelas

        private TB_ITEM_MATERIAL RetornaItemMaterial(IQueryable<TB_ITEM_MATERIAL> itemMaterialList, string codigo)
        {
            try
            {
                int? itemMaterialCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    itemMaterialCodigo = TratamentoDados.TryParseInt32(codigo);
                    return itemMaterialList.Where(a => a.TB_ITEM_MATERIAL_CODIGO == itemMaterialCodigo).FirstOrDefault();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private TB_SUBITEM_MATERIAL RetornaSubItemMaterial(IQueryable<TB_SUBITEM_MATERIAL> subItemMaterialList, string codigo, int? gestorId)
        {
            try
            {
                long? subItemMaterialCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    subItemMaterialCodigo = TratamentoDados.TryParseLong(codigo);
                    return subItemMaterialList.Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == subItemMaterialCodigo && a.TB_GESTOR_ID == gestorId).FirstOrDefault();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public TB_NATUREZA_DESPESA Retornanatureza(IQueryable<TB_NATUREZA_DESPESA> naturezaList, string NaturezaDespesaCodigo, int? itemId, string indDisponivel)
        {
            try
            {
                //Se não foi encontrado o item, retorna nulo.
                if (itemId == null)
                    return null;

                int? naturezaCodigo;

                NaturezaDespesaBusiness natuzeraBusiness = new NaturezaDespesaBusiness();
                naturezaCodigo = Convert.ToInt32(NaturezaDespesaCodigo);
                var natureza = naturezaList.Where(a => a.TB_NATUREZA_DESPESA_CODIGO == naturezaCodigo).FirstOrDefault();

                return natureza;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        public TB_UNIDADE_FORNECIMENTO RetornaUnidadeFornec(IQueryable<TB_UNIDADE_FORNECIMENTO> unidFornecList, string unidadeFornecCodigo, int gestorId)
        {
            try
            {


                UnidadeFornecimentoBusiness unidadeBusiness = new UnidadeFornecimentoBusiness();
                var unidade = unidFornecList.Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO == unidadeFornecCodigo && a.TB_GESTOR_ID == gestorId).FirstOrDefault();

                return unidade;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }




        private TB_UNIDADE_FORNECIMENTO RetornaUnidadeFornecimento(IQueryable<TB_UNIDADE_FORNECIMENTO> unidadeList, string codigo, int? gestorId)
        {
            try
            {
                if (!string.IsNullOrEmpty(codigo))
                {
                    if (gestorId != null)
                    {
                        return unidadeList.Where(a => a.TB_UNIDADE_FORNECIMENTO_CODIGO.Trim() == codigo && a.TB_GESTOR_ID == gestorId).FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private TB_ALMOXARIFADO RetornaAlmoxarifado(IQueryable<TB_ALMOXARIFADO> almoxList, string codigo, int? gestorId, int? ugeId)
        {
            try
            {
                int? almoxCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {

                    almoxCodigo = TratamentoDados.TryParseInt32(codigo);
                    if (ugeId != null)
                        return almoxList.Where(a => a.TB_ALMOXARIFADO_CODIGO == almoxCodigo && a.TB_GESTOR_ID == gestorId && a.TB_UGE_ID == ugeId).FirstOrDefault();
                    else
                        return almoxList.Where(a => a.TB_ALMOXARIFADO_CODIGO == almoxCodigo && a.TB_GESTOR_ID == gestorId).FirstOrDefault();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private TB_GESTOR RetornaGestor(IQueryable<TB_GESTOR> almoxList, string nomeReduzido)
        {
            try
            {
                if (!string.IsNullOrEmpty(nomeReduzido))
                {
                    return almoxList.Where(a => a.TB_GESTOR_NOME_REDUZIDO == nomeReduzido).FirstOrDefault();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private TB_CONTA_AUXILIAR RetornaContaAuxiliar(IQueryable<TB_CONTA_AUXILIAR> contaAuxiliarList, string codigo)
        {
            try
            {
                int? contaAuxCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    contaAuxCodigo = TratamentoDados.TryParseInt32(codigo);

                    return contaAuxiliarList.Where(a => a.TB_CONTA_AUXILIAR_CODIGO == contaAuxCodigo).FirstOrDefault();
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        private TB_UGE RetornaUnidadeGestora(IQueryable<TB_UGE> ugeList, string codigo)
        {
            try
            {
                int? ugeCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    ugeCodigo = TratamentoDados.TryParseInt32(codigo);

                    //return ugeList.Where(uge => uge.TB_UGE_CODIGO == ugeCodigo).FirstOrDefault();
                    var mUge = ugeList.Where(uge => uge.TB_UGE_CODIGO == ugeCodigo).FirstOrDefault();

                    return mUge;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private TB_UA RetornaUA(IQueryable<TB_UA> uaList, string codigo, int? gestorId)
        {
            try
            {
                int? uaCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    uaCodigo = TratamentoDados.TryParseInt32(codigo);

                    var mUa = uaList.Where(uge => uge.TB_UA_CODIGO == uaCodigo && uge.TB_GESTOR_ID == gestorId).FirstOrDefault();

                    return mUa;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private TB_DIVISAO RetornaDivisao(IQueryable<TB_DIVISAO> divisaoList, string codigo, int? uaId)
        {
            try
            {
                int? divisaoCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    divisaoCodigo = TratamentoDados.TryParseInt32(codigo);

                    var mDiv = divisaoList.Where(div => div.TB_DIVISAO_CODIGO == divisaoCodigo && div.TB_UA_ID == uaId).FirstOrDefault();

                    return mDiv;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private TB_UF RetornaUF(IQueryable<TB_UF> ufList, string estado)
        {
            try
            {


                if (!string.IsNullOrEmpty(estado))
                {

                    var mUF = ufList.Where(uf => uf.TB_UF_SIGLA == estado).FirstOrDefault();

                    return mUF;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private TB_RESPONSAVEL RetornaResponsavel(IQueryable<TB_RESPONSAVEL> uRespList, string codigo, int? gestorId)
        {
            try
            {
                int? RespCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    RespCodigo = TratamentoDados.TryParseInt32(codigo);

                    var mResp = uRespList.Where(Resp => Resp.TB_RESPONSAVEL_CODIGO == RespCodigo && Resp.TB_GESTOR_ID == gestorId).FirstOrDefault();

                    return mResp;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private TB_USUARIO RetornaUsuario(IQueryable<TB_USUARIO> usuarioList, string cpf, int? gestorId)
        {
            try
            {

                if (!string.IsNullOrEmpty(cpf))
                {

                    var mResp = usuarioList.Where(Usu => Usu.TB_USUARIO_CPF == cpf && Usu.TB_GESTOR_ID == gestorId).FirstOrDefault();

                    return mResp;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        public bool contemLetras(string texto)
        {
            if (texto.Where(c => char.IsLetter(c)).Count() > 0)
                return true;
            else
                return false;
        }
    }
}
