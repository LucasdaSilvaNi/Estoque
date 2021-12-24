using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Infrastructure;
using Sam.Common.Util;
using Sam.Business.Importacao;

namespace Sam.Business
{
    public class UaBusiness : BaseBusiness, ICrudBaseBusiness<TB_UA>
    {
        #region M�todos Customizados

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
            StringBuilder sequencia = new StringBuilder();

            try
            {
                bool isErro = false;
                UaInfrastructure infraUa = new UaInfrastructure();
                CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();
                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    sequencia.Clear();
                    sequencia.Append(carga.TB_CARGA_SEQ);

                    var divisao = MontarListaUA(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        //Realiza a importa��o dos registros
                        infraUa.Insert(divisao);

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
                infraUa.SaveChanges();
                infraCargaErro.SaveChanges();
                return isErro;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
                //throw new Exception(ErroSistema + e.Message + String.Format("Seq: {0}", sequencia));
                //return true;
            }
        }

        private TB_UA MontarListaUA(TB_CARGA carga)
        {
            ListCargaErro.Clear();

            TB_UA ua = new TB_UA();

            TB_GESTOR lObjGestor = new ImportacaoDivisao().ValidaGestor(carga);
            TB_UGE lObjUGE = new ImportacaoUA().ValidaUGE(carga);
            TB_CENTRO_CUSTO lObjCentroCusto = new ImportacaoUA().ValidaCentroDeCusto(carga);
           

            //Valida Codigo divis�o da planilha e verifica se j� cadastrado.
            if (ua != null)
            {
                if (!String.IsNullOrEmpty(ua.TB_UA_CODIGO.ToString()))
                {
                    ua.TB_UA_CODIGO = Int32.Parse(carga.TB_UA_CODIGO);

                    //Verificar se a divis�o j� est� cadastrada
                    if (new ImportacaoDivisao().ValidaDivisao(carga) != null)
                    {
                        ListCargaErro.Add(GeralEnum.CargaErro.DivisaoJaCadastrada);
                    }
                }
                else
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);
            }
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);


            //string uaDescricao = carga.TB_UA_DESCRICAO;
            //if (!String.IsNullOrEmpty(uaDescricao))
            //    ua.TB_UA_DESCRICAO = uaDescricao;
            //else
            //    ua.TB_UA_DESCRICAO = string.Empty;



            ////divisao.TB_DIVISAO_CODIGO 
            //int uaVinc= Convert.ToInt32(carga.TB_UA_VINCULADA);
            //if (uaVinc > 0)
            //    ua.TB_UA_VINCULADA = uaVinc;
           


            //if (carga.TB_UA_INDICADOR_ATIVIDADE != null)
            //{
            //    if (carga.TB_DIVISAO_INDICADOR_ATIVIDADE.ToUpper() == "S")
            //        ua.TB_UA_INDICADOR_ATIVIDADE = true;
            //    else
            //        ua.TB_UA_INDICADOR_ATIVIDADE = false;
            //}
            //else
            //{
            //    ListCargaErro.Add(GeralEnum.CargaErro.IndicadorAtividadeInvalido);
            //}

          

            
            string ccCodigo = carga.TB_CENTRO_CUSTO_CODIGO;
            if (!String.IsNullOrEmpty(ccCodigo))
            {

                if (lObjCentroCusto.TB_CENTRO_CUSTO_ID > 0)
                    ua.TB_CENTRO_CUSTO_ID = lObjCentroCusto.TB_CENTRO_CUSTO_ID;
                else
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoResponsavelInvalido);
            }


            
            return ua;
        }



        #endregion

        #region M�todos Gen�ricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_UA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public void Update(TB_UA entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public void Delete(TB_UA entity)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UA_ID == entity.TB_UA_ID);
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
        public void DeleteRelatedEntries(TB_UA entity)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UA_ID == entity.TB_UA_ID);
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
        public void DeleteRelatedEntries(TB_UA entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_UA_ID == entity.TB_UA_ID);
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
        public TB_UA SelectOne(Expression<Func<TB_UA, bool>> where)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
                return infraestrutura.SelectOne(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public IList<Sam.Domain.Entity.UAEntity> SelectOne(int uaId)
        {
            try
            {
                UaInfrastructure infra = new UaInfrastructure();
                return infra.SelectById(uaId);
            }
            catch (Exception ex)
            {
                base.GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Retorna todos os registros encontrados sem filtro
        /// </summary>
        /// <returns>Lista de objetos</returns>
        public List<TB_UA> SelectAll()
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public List<TB_UA> SelectAll(Expression<Func<TB_UA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public List<TB_UA> SelectWhere(Expression<Func<TB_UA, bool>> where)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        /// Retorna uma lista de objetos encontrados na condi��o, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Express�o de ordena��o para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Express�o lambda where</param>
        /// <param name="maximumRows">Número m�ximo de registros que ir� retornar</param>
        /// <param name="startRowIndex">índice da pagina��o</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_UA> SelectWhere(Expression<Func<TB_UA, int>> sortExpression, bool desc, Expression<Func<TB_UA, bool>> where, int startRowIndex)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public List<TB_UA> SelectWhere(Expression<Func<TB_UA, string>> sortExpression, bool desc, Expression<Func<TB_UA, bool>> where, int startRowIndex)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public IQueryable<TB_UA> QueryAll()
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public IQueryable<TB_UA> QueryAll(Expression<Func<TB_UA, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public int GetCount(Expression<Func<TB_UA, bool>> where)
        {
            try
            {
                UaInfrastructure infraestrutura = new UaInfrastructure();
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
        public void Consistir(TB_UA entity)
        {
            try
            {
                if (entity.TB_UA_CODIGO == 0)
                {
                    throw new Exception("O campo C�digo � de preenchimento obritag�rio");
                }
                if (string.IsNullOrEmpty(entity.TB_UA_DESCRICAO))
                {
                    throw new Exception("O campo Descri��o � de preenchimento obritag�rio");
                }
                if (entity.TB_GESTOR_ID == 0)
                {
                    throw new Exception("O campo Gestor � de preenchimento obritag�rio");
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
