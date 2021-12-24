using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Common.Util;
using Sam.Business.Importacao;
using Sam.Domain.Business;
using Sam.Domain.Entity;
using Sam.Domain.Entity.DtoWs;


namespace Sam.Business
{
    public class SubItemMaterialBusiness : BaseBusiness, ICrudBaseBusiness<TB_SUBITEM_MATERIAL>
    {

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

        #region Métodos Customizados

        /// <summary>
        /// Realiza a busca de subitens material utilizada nas lupas.
        /// </summary>
        /// <returns></returns>
        //public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterial(int startIndex, string palavraChave, bool filtraAlmoxarifado, bool filtraGestor, bool comSaldo, TB_ALMOXARIFADO almoxarifado)
        public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterial(int startIndex, string palavraChave, bool filtraAlmoxarifado, bool filtraGestor, bool comSaldo, TB_ALMOXARIFADO almoxarifado, bool filtraNaturezasDespesaConsumoImediato, int almox =0, int gestor=0)
        {
            SubItemMaterialInfrastructure infraSubItemMaterial = new SubItemMaterialInfrastructure();
            //var result = infraSubItemMaterial.BuscaSubItemMaterial(startIndex, palavraChave, filtraAlmoxarifado, filtraGestor, comSaldo, almoxarifado);
            var result = infraSubItemMaterial.BuscaSubItemMaterial(startIndex, palavraChave, filtraAlmoxarifado, filtraGestor, comSaldo, almoxarifado, filtraNaturezasDespesaConsumoImediato, almox, gestor);
            this.TotalRegistros = infraSubItemMaterial.TotalRegistros;

            return result;
        }

        /// <summary>
        /// Insere uma lista de objeto consistido, utilizado na importação de arquivos
        /// </summary>
        /// <param name="entity">Objeto</param>
        public bool InsertListControleImportacao(TB_CONTROLE entityList)
        {
            StringBuilder seq = new StringBuilder();

            try
            {
                bool isErro = false;


                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    SubItemMaterialInfrastructure infraSubItemMaterial = new SubItemMaterialInfrastructure();
                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);

                    var subItemMaterial = MontarListaSubItens(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);

                        infraSubItemMaterial.Insert(subItemMaterial);

                        infraSubItemMaterial.SaveChanges();
                        infraSubItemMaterial.Dispose();

                    }
                    else
                    {
                        foreach (GeralEnum.CargaErro erroEnum in ListCargaErro)
                        {
                            TB_CARGA_ERRO cargaErro = new TB_CARGA_ERRO();
                            CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();

                            carga.TB_CARGA_VALIDO = false;
                            cargaBusiness.Update(carga);

                            cargaErro.TB_CARGA_ID = carga.TB_CARGA_ID;
                            cargaErro.TB_ERRO_ID = (int)erroEnum;

                            infraCargaErro.Insert(cargaErro);
                            infraCargaErro.SaveChanges();
                            infraCargaErro.Dispose();



                            isErro = true;
                        }
                    }
                }
                // Salva o contexto apenas se todos os registros foram inseridos com sucesso

                return isErro;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Insere uma lista de objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void InsertList(List<TB_SUBITEM_MATERIAL> entityList)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();

                foreach (var entity in entityList)
                {
                    Consistir(entity);
                    if (base.Consistido)
                    {
                        infraestrutura.Insert(entity);
                    }
                }
                infraestrutura.SaveChanges();
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        /// <summary>
        /// Monta a Lista de SubItems e ItemSubItem a partir da tabela Carga
        /// </summary>
        /// <param name="carga"></param>
        /// <returns></returns>
        private TB_SUBITEM_MATERIAL MontarListaSubItens(TB_CARGA carga)
        {
            IQueryable<TB_ITEM_MATERIAL> itemMaterialList;
            itemMaterialList = new ItemMaterialBusiness().QueryAll();

            IQueryable<TB_NATUREZA_DESPESA> naturezaList;
            naturezaList = new NaturezaDespesaBusiness().QueryAll();

            IQueryable<TB_UNIDADE_FORNECIMENTO> unidadeFornecList;
            unidadeFornecList = new UnidadeFornecimentoBusiness().QueryAll();

            CatalogoBusiness ct = new CatalogoBusiness();


            ListCargaErro.Clear();

            //Realiza as validações básicas do Item e SubItem
            ListCargaErro = new ImportacaoSubItemMaterial().ValidarImportacao(carga);

            TB_SUBITEM_MATERIAL_ALMOX subItemMaterialAlmox = new TB_SUBITEM_MATERIAL_ALMOX();
            TB_SUBITEM_MATERIAL subItem = new ImportacaoSubItemMaterial().ValidaSubItem(carga);

            if (subItem != null)
            {
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoCadastrado);
                return null;
            }

            TB_SUBITEM_MATERIAL subItemMaterial = new TB_SUBITEM_MATERIAL();
            TB_ITEM_SUBITEM_MATERIAL itemSubItemMaterial = new TB_ITEM_SUBITEM_MATERIAL();
            subItemMaterial.TB_ITEM_SUBITEM_MATERIAL = new EntityCollection<TB_ITEM_SUBITEM_MATERIAL>();

            //Conta Auxiliar
            //if (carga.TB_CONTA_AUXILIAR_ID != null)
            //    subItemMaterial.TB_CONTA_AUXILIAR_ID = (int)carga.TB_CONTA_AUXILIAR_ID;
            //else
            //{
            //    ListCargaErro.Add(GeralEnum.CargaErro.CodigoContaAuxliarInvalida);
            //    return null;
            //}

            //Gestor
            if (carga.TB_GESTOR_ID != null)
                subItemMaterial.TB_GESTOR_ID = (int)carga.TB_GESTOR_ID;
            else
            {
                ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
                return null;
            }
            //Item material
            if (carga.TB_ITEM_MATERIAL_ID != null)
                itemSubItemMaterial.TB_ITEM_MATERIAL_ID = (int)carga.TB_ITEM_MATERIAL_ID;
            else
            {
                ItemMaterialEntity itemMaterialRetorno = ct.RecuperarCadastroItemMaterialDoSiafisico(carga.TB_ITEM_MATERIAL_CODIGO, true);
                if (itemMaterialRetorno != null)
                {
                    itemSubItemMaterial.TB_ITEM_MATERIAL_ID = (int)itemMaterialRetorno.Id;
                    carga.TB_ITEM_MATERIAL_ID = itemSubItemMaterial.TB_ITEM_MATERIAL_ID;
                }
                else
                {
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoItemInvalido);
                    return null;
                }
            }

            //Natureza Despesa
            if (carga.TB_NATUREZA_DESPESA_ID != null)
                subItemMaterial.TB_NATUREZA_DESPESA_ID = (int)carga.TB_NATUREZA_DESPESA_ID;
            else
            {
                if (carga.TB_ITEM_MATERIAL_ID != null)
                {
                    ImportacaoCargaControle iCC = new ImportacaoCargaControle();

                    var naturezaDespesa = iCC.Retornanatureza(naturezaList, carga.TB_NATUREZA_DESPESA_CODIGO.Trim(), carga.TB_ITEM_MATERIAL_ID, carga.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE);
                    if (naturezaDespesa != null)
                    {
                        subItemMaterial.TB_NATUREZA_DESPESA_ID = (int)naturezaDespesa.TB_NATUREZA_DESPESA_ID;
                        carga.TB_NATUREZA_DESPESA_ID = subItemMaterial.TB_NATUREZA_DESPESA_ID;
                    }
                    else
                    {
                        ListCargaErro.Add(GeralEnum.CargaErro.NaturezaDespesaInvalida);
                        return null;
                    }

                }
            }

            //Verifica se há relação da ND com o Item
            ItemNaturezaDespesaInfrastructure infraItemNatureza = new ItemNaturezaDespesaInfrastructure();
            ItemMaterialBusiness business = new ItemMaterialBusiness();

            bool naturezaItem = false;
            if (carga.TB_ITEM_MATERIAL_ID != null && carga.TB_NATUREZA_DESPESA_CODIGO != null)
            {
                if (!infraItemNatureza.ExisteRelacaoItemNaturezaDespesa((int)carga.TB_ITEM_MATERIAL_ID, carga.TB_NATUREZA_DESPESA_CODIGO.Trim()))
                {
                    ItemMaterialEntity itemMaterialRetorno = ct.RecuperarCadastroItemMaterialDoSiafisico(carga.TB_ITEM_MATERIAL_CODIGO, false);
                    if (itemMaterialRetorno != null)
                    {
                        for (int i = 0; i < itemMaterialRetorno.NaturezaDespesa.Count; i++)
                        {
                            if (itemMaterialRetorno.NaturezaDespesa[i].Codigo.ToString().Trim() == carga.TB_NATUREZA_DESPESA_CODIGO.Trim())
                            {
                                infraItemNatureza.AtualizaRelacaoItemNaturezaDespesa((int)carga.TB_ITEM_MATERIAL_ID, (int)itemMaterialRetorno.NaturezaDespesa[i].Id);
                                naturezaItem = true;
                            }

                        }
                    }
                }
                else
                    naturezaItem = true;
            }

            if (!naturezaItem)
            {
                ListCargaErro.Add(GeralEnum.CargaErro.NaturezaItem);
                return null;
            }


            //Atribui SubItemMaterial
            subItemMaterial.TB_SUBITEM_MATERIAL_COD_BARRAS = string.Empty;
            subItemMaterial.TB_SUBITEM_MATERIAL_DECIMOS = false;
            subItemMaterial.TB_SUBITEM_MATERIAL_FRACIONA = false;
            subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO = (long)TratamentoDados.TryParseLong(carga.TB_SUBITEM_MATERIAL_CODIGO.Trim());

            //Descrição
            if (!String.IsNullOrEmpty(carga.TB_SUBITEM_MATERIAL_DESCRICAO))
            {

                if (carga.TB_SUBITEM_MATERIAL_DESCRICAO.Trim().Length <= 120)
                    subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO = carga.TB_SUBITEM_MATERIAL_DESCRICAO.Trim();
                else
                    ListCargaErro.Add(GeralEnum.CargaErro.DescricaoSubItemInvalida);

            }
            else
            {
                ListCargaErro.Add(GeralEnum.CargaErro.DescricaoSubItemInvalida);
                return null;
            }

            //Lote
            //if (carga.TB_SUBITEM_MATERIAL_LOTE == "S")
            //    subItemMaterial.TB_SUBITEM_MATERIAL_LOTE = true;
            //else
            //    subItemMaterial.TB_SUBITEM_MATERIAL_LOTE = false;

            //Indicador Atividade
            if (carga.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == "S")
                subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = true;
            else
                subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = false;

            //Unidade Fornecimento
            if (carga.TB_UNIDADE_FORNECIMENTO_ID != null)
                subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID = (int)carga.TB_UNIDADE_FORNECIMENTO_ID;
            else
            {

                ImportacaoCargaControle TUF = new ImportacaoCargaControle();

                var unidadeForn = TUF.RetornaUnidadeFornec(unidadeFornecList, carga.TB_UNIDADE_FORNECIMENTO_CODIGO, (int)carga.TB_GESTOR_ID);
                if (unidadeForn != null)
                {
                    subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID = (int)unidadeForn.TB_UNIDADE_FORNECIMENTO_ID;
                    carga.TB_UNIDADE_FORNECIMENTO_ID = subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID;
                }
                else
                {
                    ListCargaErro.Add(GeralEnum.CargaErro.UnidadeFornecimentoInvalida);
                    return null;
                }

            }

            //Gestor ItemSubItem
            if (carga.TB_GESTOR_ID != null)
                itemSubItemMaterial.TB_GESTOR_ID = (int)carga.TB_GESTOR_ID;
            else
            {
                ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
                return null;
            }


            //Adiciona ItemSubItem
            subItemMaterial.TB_ITEM_SUBITEM_MATERIAL.Add(itemSubItemMaterial);

            return subItemMaterial;
        }

        #endregion

        #region Métodos Genéricos
        #endregion

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_SUBITEM_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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

        public IList<TB_SUBITEM_MATERIAL> BuscaSubItemMaterialRequisicao(int startIndex, string palavraChave, bool filtraGestor, TB_ALMOXARIFADO almoxarifado, bool dispRequisicao)
        {
            SubItemMaterialInfrastructure infraSubItemMaterial = new SubItemMaterialInfrastructure();
            var result = infraSubItemMaterial.BuscaSubItemMaterialRequisicao(startIndex, palavraChave, filtraGestor, almoxarifado, dispRequisicao);
            this.TotalRegistros = infraSubItemMaterial.TotalRegistros;

            return result;
        }

        public IList<dtoWsSubitemMaterial> BuscaSubItemMaterialRequisicaoParaWs(string termoParaPesquisa, int codigoOrgao, int codigoAlmox, bool dispRequisicao = true, int numeroPaginaConsulta = 0)
        {
            if (String.IsNullOrWhiteSpace(termoParaPesquisa))
                return (new List<dtoWsSubitemMaterial>() { new dtoWsSubitemMaterial() { Codigo = 0, Descricao = "TERMO DE PESQUISA INVALIDO", NaturezaDespesa = 0, UnidadeFornecimento = null } });


            SubItemMaterialInfrastructure infraSubItemMaterial = new SubItemMaterialInfrastructure();
            var retornoParaWs = infraSubItemMaterial.BuscaSubItemMaterialRequisicaoParaWs(termoParaPesquisa, codigoOrgao, codigoAlmox, dispRequisicao, numeroPaginaConsulta);
            this.TotalRegistros = infraSubItemMaterial.TotalRegistros;

            return retornoParaWs;
        }

        #region Métodos Genéricos



        /// <summary>
        /// Atualiza o objeto consitido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Update(TB_SUBITEM_MATERIAL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        /// Exclui o objeto não relacionado
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Delete(TB_SUBITEM_MATERIAL entity)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ID == entity.TB_SUBITEM_MATERIAL_ID);
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
        public void DeleteRelatedEntries(TB_SUBITEM_MATERIAL entity)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ID == entity.TB_SUBITEM_MATERIAL_ID);
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
        /// Eclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_SUBITEM_MATERIAL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ID == entity.TB_SUBITEM_MATERIAL_ID);
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
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        public TB_SUBITEM_MATERIAL SelectOne(Expression<Func<TB_SUBITEM_MATERIAL, bool>> where)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        /// Retorna o primeiro registro encontrado na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Objeto</returns>
        public TB_SUBITEM_MATERIAL SelectOne(Expression<Func<TB_SUBITEM_MATERIAL, bool>> where, bool lazyLoadingEnabled)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
                infraestrutura.LazyLoadingEnabled = lazyLoadingEnabled;
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
        public List<TB_SUBITEM_MATERIAL> SelectAll()
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
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
        /// Retorna todos os objetos encontrados, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL> SelectAll(Expression<Func<TB_SUBITEM_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
                return infraestrutura.SelectAll(sortExpression, startRowIndex);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL, bool>> where)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
                return infraestrutura.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL, int>> sortExpression, bool desc, Expression<Func<TB_SUBITEM_MATERIAL, bool>> where, int startRowIndex)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos STRING</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL, string>> sortExpression, bool desc, Expression<Func<TB_SUBITEM_MATERIAL, bool>> where, int startRowIndex)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        public IQueryable<TB_SUBITEM_MATERIAL> QueryAll()
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        /// <param name="sortExpression">Expressão de ordenação</param>
        /// <param name="desc">Ordem decrescente</param>        
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_SUBITEM_MATERIAL> QueryAll(Expression<Func<TB_SUBITEM_MATERIAL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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
        /// Retorna o total de registros na condição
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount(Expression<Func<TB_SUBITEM_MATERIAL, bool>> where)
        {
            try
            {
                SubItemMaterialInfrastructure infraestrutura = new SubItemMaterialInfrastructure();
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

        #region Métodos Outros

        /// <summary>
        /// Valida os campos da entidade antes da persistencia
        /// </summary>
        /// <param name="entity">Objeto a ser validado</param>
        public void Consistir(TB_SUBITEM_MATERIAL entity)
        {
            try
            {
                if (entity.TB_SUBITEM_MATERIAL_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_SUBITEM_MATERIAL_DESCRICAO))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
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
    }
}

