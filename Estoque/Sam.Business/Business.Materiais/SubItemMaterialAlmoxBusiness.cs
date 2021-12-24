using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Business.Importacao;
using System.Data.Objects.DataClasses;
using Sam.Common.Util;
using Sam.Domain.Entity;


namespace Sam.Business
{
    public class SubItemMaterialAlmoxBusiness : BaseBusiness, ICrudBaseBusiness<TB_SUBITEM_MATERIAL_ALMOX>
    {

        #region Métodos Customizados

        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int maximumRows, int startRowIndex, int divisaoId)
        {
            SubItemMaterialAlmoxInfrastructure infra = new SubItemMaterialAlmoxInfrastructure();
            IList<TB_SUBITEM_MATERIAL_ALMOX> result = infra.SelectSubItemAlmoxByDivisao(maximumRows, startRowIndex, divisaoId);
            this.TotalRegistros = infra.TotalRegistros;

            return result;
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int divisaoId, string pesquisa)
        {
            Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where = a => a.TB_INDICADOR_DISPONIVEL_ID == 1 || a.TB_INDICADOR_DISPONIVEL_ID == 2;
            //&& ((pesquisa == string.Empty) || ((a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().Contains(pesquisa)))
            //|| ((pesquisa == string.Empty)) || (a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO.ToUpper().Contains(pesquisa.ToUpper())));

            SubItemMaterialAlmoxInfrastructure infra = new SubItemMaterialAlmoxInfrastructure();
            //return infra.SelectWhere(where);

            IList<TB_SUBITEM_MATERIAL_ALMOX> result = infra.SelectSubItemAlmoxByDivisao(divisaoId, pesquisa);
            return result;
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> SelectSubItemAlmoxByDivisao(int divisaoId, string pesquisa, bool possuiSaldo)
        {
            if (!possuiSaldo)
                return SelectSubItemAlmoxByDivisao(divisaoId, pesquisa);

            SubItemMaterialAlmoxInfrastructure infra = new SubItemMaterialAlmoxInfrastructure();
            IList<TB_SUBITEM_MATERIAL_ALMOX> result = infra.SelectSubItemAlmoxByDivisao(divisaoId, pesquisa, possuiSaldo);

            return result;
        }

        public IList<TB_SUBITEM_MATERIAL_ALMOX> ObterSubItensMaterialAlmoxarifadoPorDivisao(int divisaoId, string pesquisa)
        {
            SubItemMaterialAlmoxInfrastructure objInfra = new SubItemMaterialAlmoxInfrastructure();
            IList<TB_SUBITEM_MATERIAL_ALMOX> lstRetorno = objInfra.ObterSubItensMaterialAlmoxarifadoPorDivisao(divisaoId, pesquisa, null);

            return lstRetorno;
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
                SubItemMaterialAlmoxInfrastructure infraSubItemMaterialAlmox = new SubItemMaterialAlmoxInfrastructure();
                CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();
                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);
                    var subItemMaterialAlmox = MontarListaSubItens(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        //Realiza a importação dos registros
                        infraSubItemMaterialAlmox.Insert(subItemMaterialAlmox);

                        //Atualiza a carga                        
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);

                        infraSubItemMaterialAlmox.SaveChanges();
                        infraCargaErro.SaveChanges();
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

                            infraCargaErro.SaveChanges();

                            //Se ocorreu ao menos 1 erro, retorna true;
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
                //throw new Exception(ErroSistema + e.Message + String.Format("Seq: {0}", seq));
                //return true;
            }
        }

        /// <summary>
        /// Monta a Lista de SubItems e ItemSubItem a partir da tabela Carga
        /// </summary>
        /// <param name="carga"></param>
        /// <returns></returns>
        private TB_SUBITEM_MATERIAL_ALMOX MontarListaSubItens(TB_CARGA carga)
        {
            ListCargaErro.Clear();
            TB_SUBITEM_MATERIAL_ALMOX subItemMaterialAlmox = new TB_SUBITEM_MATERIAL_ALMOX();

            TB_SUBITEM_MATERIAL subItem = new ImportacaoSubItemMaterial().ValidaSubItem(carga);

            if (subItem != null)
            {
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ID = subItem.TB_SUBITEM_MATERIAL_ID;
                carga.TB_SUBITEM_MATERIAL_ID = subItem.TB_SUBITEM_MATERIAL_ID;

                //Realiza as validações básicas do Item e SubItem
                ListCargaErro = new ImportacaoCatalogo().ValidarImportacao(carga);
            }
            else
            {
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoSubItemNaoCadastradoGestor);
            }

            if (carga.TB_ALMOXARIFADO_ID != null)
                subItemMaterialAlmox.TB_ALMOXARIFADO_ID = (int)carga.TB_ALMOXARIFADO_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoAlmoxarifadoInvalido);

            if (carga.TB_INDICADOR_DISPONIVEL_ID != null)
                subItemMaterialAlmox.TB_INDICADOR_DISPONIVEL_ID = (int)carga.TB_INDICADOR_DISPONIVEL_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.IndicadorDisponivelInvalido);

            //if (carga.TB_INDICADOR_ITEM_SALDO_ZERADO_ID != null)
            //    subItemMaterialAlmox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID = (int)carga.TB_INDICADOR_ITEM_SALDO_ZERADO_ID;
            //else
            //    ListCargaErro.Add(GeralEnum.CargaErro.IndicadorDisponivelInvalido);

            int? max = TratamentoDados.TryParseInt32(carga.TB_SUBITEM_MATERIAL_ESTOQUE_MAX);
            if (max != null)
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = (int)max;
            else
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = 0;

            var mim = TratamentoDados.TryParseInt32(carga.TB_SUBITEM_MATERIAL_ESTOQUE_MIN);
            if (mim != null)
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = (int)mim;
            else
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = 0;

            if (carga.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE.ToUpper() == "S")
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = true;
            else
                subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = false;

            //Validar se estoque Maximo é maior que Minimo
            if (subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX < subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN)
                ListCargaErro.Add(GeralEnum.CargaErro.EstoqueMaxInvalido);

            return subItemMaterialAlmox;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_SUBITEM_MATERIAL_ALMOX entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public void Update(TB_SUBITEM_MATERIAL_ALMOX entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public void Delete(TB_SUBITEM_MATERIAL_ALMOX entity)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ALMOX_ID == entity.TB_SUBITEM_MATERIAL_ALMOX_ID);
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
        public void DeleteRelatedEntries(TB_SUBITEM_MATERIAL_ALMOX entity)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ALMOX_ID == entity.TB_SUBITEM_MATERIAL_ALMOX_ID);
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
        public void DeleteRelatedEntries(TB_SUBITEM_MATERIAL_ALMOX entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_SUBITEM_MATERIAL_ALMOX_ID == entity.TB_SUBITEM_MATERIAL_ALMOX_ID);
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
        public TB_SUBITEM_MATERIAL_ALMOX SelectOne(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public List<TB_SUBITEM_MATERIAL_ALMOX> SelectAll()
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public List<TB_SUBITEM_MATERIAL_ALMOX> SelectAll(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        /// Retorna uma lista de objetos encontrados na condição, ordenados e com paginação. Utilizado em Listas paginadas.
        /// </summary>
        /// <param name="where">Expressão lambda where</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL_ALMOX> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">Número máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">índice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_SUBITEM_MATERIAL_ALMOX> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, int>> sortExpression, bool desc, Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where, int startRowIndex)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public List<TB_SUBITEM_MATERIAL_ALMOX> SelectWhere(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, string>> sortExpression, bool desc, Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where, int startRowIndex)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
                infraestrutura.LazyLoadingEnabled = true;
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
        public IQueryable<TB_SUBITEM_MATERIAL_ALMOX> QueryAll()
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public IQueryable<TB_SUBITEM_MATERIAL_ALMOX> QueryAll(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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

        public IList<SubItemMaterialEntity> ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(int Almoxarifado_ID, int NaturezaDespesa_ID)
        {
            IList<SubItemMaterialEntity> lstRetorno = new List<SubItemMaterialEntity>();

            try
            {
                SubItemMaterialAlmoxInfrastructure objInfra = new SubItemMaterialAlmoxInfrastructure();
                lstRetorno = objInfra.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(Almoxarifado_ID, NaturezaDespesa_ID);

                this.TotalRegistros = objInfra.TotalRegistros;
            }
            catch (Exception excErroConsulta)
            {
                //base.GravarLogErro(e);
                //throw new Exception(e.Message, e.InnerException);

                Exception excErroParaPropagacao = new Exception("Erro ao processar consulta para grid Gerência Catálogo.", excErroConsulta);
                base.GravarLogErro(excErroParaPropagacao);
                throw excErroParaPropagacao;
            }

            return lstRetorno;
        }

        public IList<SubItemMaterialEntity> ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(int Almoxarifado_ID, int NaturezaDespesa_ID, int startRowIndex, int maximumRows)
        {
            IList<SubItemMaterialEntity> lstRetorno = new List<SubItemMaterialEntity>();

            try
            {
                SubItemMaterialAlmoxInfrastructure objInfra = new SubItemMaterialAlmoxInfrastructure();
                lstRetorno = objInfra.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(Almoxarifado_ID, NaturezaDespesa_ID, startRowIndex, maximumRows);

                this.TotalRegistros = objInfra.TotalRegistros;
            }
            catch (Exception excErroConsulta)
            {
                //base.GravarLogErro(e);
                //throw new Exception(e.Message, e.InnerException);

                Exception excErroParaPropagacao = new Exception("Erro ao processar consulta para grid Gerência Catálogo.", excErroConsulta);
                base.GravarLogErro(excErroParaPropagacao);
                throw excErroParaPropagacao;
            }

            return lstRetorno;
        }

        /// <summary>
        /// Retorna o total de registros encontrados na tabela
        /// </summary>
        /// <returns>Total de registros inteiro</returns>
        public int GetCount()
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public int GetCount(Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where)
        {
            try
            {
                SubItemMaterialAlmoxInfrastructure infraestrutura = new SubItemMaterialAlmoxInfrastructure();
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
        public void Consistir(TB_SUBITEM_MATERIAL_ALMOX entity)
        {
            try
            {
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
