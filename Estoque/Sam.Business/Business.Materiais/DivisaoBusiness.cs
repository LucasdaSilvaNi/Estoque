using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Common.Util;
using Sam.Business.Importacao;
using Sam.Domain.Entity;

namespace Sam.Business
{
    public class DivisaoBusiness : BaseBusiness, ICrudBaseBusiness<TB_DIVISAO>
    {
        #region Métodos Customizados

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
                DivisaoInfrastructure infraDivisao = new DivisaoInfrastructure();
                CargaErroInfrastructure infraCargaErro = new CargaErroInfrastructure();
                CargaBusiness cargaBusiness = new CargaBusiness();

                foreach (var carga in entityList.TB_CARGA)
                {
                    sequencia.Clear();
                    sequencia.Append(carga.TB_CARGA_SEQ);

                    var divisao = MontarListaDivisao(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        //Realiza a importação dos registros
                        infraDivisao.Insert(divisao);

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
                infraDivisao.SaveChanges();
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

        private TB_DIVISAO MontarListaDivisao(TB_CARGA carga)
        {
            ListCargaErro.Clear();

            TB_DIVISAO divisao = new TB_DIVISAO();

            TB_GESTOR lObjGestor = new ImportacaoDivisao().ValidaGestor(carga);
            TB_ALMOXARIFADO lObjAlmoxarifado = new ImportacaoDivisao().ValidaAlmoxarifado(carga);
            TB_RESPONSAVEL lObjResponsavel = new TB_RESPONSAVEL();
            if (!string.IsNullOrEmpty(carga.TB_RESPONSAVEL_CODIGO))
                lObjResponsavel = new ImportacaoDivisao().ValidaResponsavel(carga);
            TB_UA lObjUA = new ImportacaoDivisao().ValidaUA(carga);
            TB_UF lObjUF = new ImportacaoDivisao().ValidaUF(carga);

            //Valida Codigo divisão da planilha e verifica se já cadastrado.
            if (divisao != null)
            {
                if (!String.IsNullOrEmpty(carga.TB_DIVISAO_CODIGO))
                {
                    divisao.TB_DIVISAO_CODIGO = Int32.Parse(carga.TB_DIVISAO_CODIGO);

                    //Verificar se a divisão já está cadastrada
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

            //divisao.TB_DIVISAO_INDICADOR_ATIVIDADE
            if (!String.IsNullOrEmpty(carga.TB_DIVISAO_INDICADOR_ATIVIDADE))
            {
                if (carga.TB_DIVISAO_INDICADOR_ATIVIDADE.ToUpper() == "S")
                    divisao.TB_DIVISAO_INDICADOR_ATIVIDADE = true;
                else
                    divisao.TB_DIVISAO_INDICADOR_ATIVIDADE = false;
            }
            else
            {
                ListCargaErro.Add(GeralEnum.CargaErro.IndicadorAtividadeInvalido);
            }

            //divisao.TB_DIVISAO_AREA
            string divisaoArea = carga.TB_DIVISAO_AREA;
            if (!String.IsNullOrEmpty(divisaoArea))
                divisao.TB_DIVISAO_AREA = Int32.Parse(divisaoArea);
            else
                divisao.TB_DIVISAO_AREA = null;


            //divisao.TB_DIVISAO_TELEFONE
            string divisaoTelefone = carga.TB_DIVISAO_TELEFONE;
            if (!String.IsNullOrEmpty(divisaoTelefone))
                divisao.TB_DIVISAO_TELEFONE = divisaoTelefone.Trim();//.Substring(0, 20);
            else
                divisao.TB_DIVISAO_TELEFONE = string.Empty;



            //divisao.TB_DIVISAO_FAX
            string divisaoFax = carga.TB_DIVISAO_FAX;
            if (!String.IsNullOrEmpty(divisaoFax))
                divisao.TB_DIVISAO_FAX = divisaoFax.Trim();//.Substring(0, 20);
            else
                divisao.TB_DIVISAO_FAX = string.Empty;


            //TB_DIVISAO_DESCRICAO
            string divisaoDescricao = carga.TB_DIVISAO_DESCRICAO;
            if (!String.IsNullOrEmpty(divisaoDescricao))
            {
                if (divisaoDescricao.Trim().Length <= 120)
                    divisao.TB_DIVISAO_DESCRICAO = divisaoDescricao.Trim();//.Substring(0, 120);
                else
                    ListCargaErro.Add(GeralEnum.CargaErro.DescricaoInvalida);
            }
            else
                divisao.TB_DIVISAO_DESCRICAO = string.Empty;


            //divisao.TB_DIVISAO_LOGRADOURO
            string divisaoLogradouro = carga.TB_DIVISAO_LOGRADOURO;
            if (!String.IsNullOrEmpty(divisaoLogradouro))
                divisao.TB_DIVISAO_LOGRADOURO = divisaoLogradouro.Trim();//.Substring(0, 120);
            else
                divisao.TB_DIVISAO_LOGRADOURO = string.Empty;



            //divisao.TB_DIVISAO_Numero
            string divisaoNumero = carga.TB_DIVISAO_NUMERO;
            if (!String.IsNullOrEmpty(divisaoNumero))
                divisao.TB_DIVISAO_NUMERO = divisaoNumero.Trim();//.Substring(0, 120);
            else
                divisao.TB_DIVISAO_NUMERO = string.Empty;


            string divisaoBairro = carga.TB_DIVISAO_BAIRRO;
            if (!String.IsNullOrEmpty(divisaoBairro))
                divisao.TB_DIVISAO_BAIRRO = divisaoBairro;
            else
                divisao.TB_DIVISAO_BAIRRO = string.Empty;


            string divisaoCompl = carga.TB_DIVISAO_COMPLEMENTO;
            if (!String.IsNullOrEmpty(divisaoCompl))
                divisao.TB_DIVISAO_COMPLEMENTO = divisaoCompl;
            else
                divisao.TB_DIVISAO_COMPLEMENTO = string.Empty;



            //divisao.TB_DIVISAO_CEP
            string divisaoCEP = carga.TB_DIVISAO_CEP;
            if (!String.IsNullOrEmpty(divisaoCEP))
            {
                //divisao.TB_DIVISAO_CEP = Int32.Parse(divisaoCEP.Trim().Substring(0, 20)).ToString();
                divisao.TB_DIVISAO_CEP = divisaoCEP.Trim().ToString();
                divisao.TB_DIVISAO_CEP = divisao.TB_DIVISAO_CEP.Replace("-", string.Empty);

                if (divisao.TB_DIVISAO_CEP.Length > 8)
                    ListCargaErro.Add(GeralEnum.CargaErro.CEPInvalido);


            }
            else
                divisao.TB_DIVISAO_CEP = string.Empty;


            //divisao.TB_DIVISAO_MUNICIPIO
            string divisaoMunicipio = carga.TB_DIVISAO_MUNICIPIO;
            if (!String.IsNullOrEmpty(divisaoMunicipio))
                divisao.TB_DIVISAO_MUNICIPIO = divisaoMunicipio.Trim();
            else
                divisao.TB_DIVISAO_MUNICIPIO = string.Empty;


            //divisao.TB_DIVISAO_CODIGO 
            string divisaoCodigo = carga.TB_DIVISAO_CODIGO;
            if (!String.IsNullOrEmpty(divisaoCodigo))
                divisao.TB_DIVISAO_CODIGO = Int32.Parse(divisaoCodigo.Trim());
            else
                divisao.TB_DIVISAO_CODIGO = 0;

            if (lObjAlmoxarifado != null)
                divisao.TB_ALMOXARIFADO_ID = lObjAlmoxarifado.TB_ALMOXARIFADO_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoAlmoxarifadoInvalido);

            if (!string.IsNullOrEmpty(carga.TB_RESPONSAVEL_CODIGO))

                if (lObjResponsavel != null)
                {
                    if (lObjResponsavel.TB_RESPONSAVEL_ID > 0)
                        divisao.TB_RESPONSAVEL_ID = lObjResponsavel.TB_RESPONSAVEL_ID;
                    else
                        ListCargaErro.Add(GeralEnum.CargaErro.CodigoResponsavelInvalido);
                }
                else
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoResponsavelInvalido);

            if (lObjUA != null)
            {
                if (lObjGestor != null)
                {
                    if (lObjUA.TB_GESTOR_ID == lObjGestor.TB_GESTOR_ID)
                    {
                        divisao.TB_UA_ID = lObjUA.TB_UA_ID;
                    }
                    else
                    {
                        ListCargaErro.Add(GeralEnum.CargaErro.UaGestor);
                    }
                }else
                    ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido); 
            }
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoUAInvalido);

            if (lObjUF != null)
                divisao.TB_UF_ID = lObjUF.TB_UF_ID;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.SiglaUFInvalida);

            return divisao;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_DIVISAO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public void Update(TB_DIVISAO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public void Delete(TB_DIVISAO entity)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_DIVISAO_ID == entity.TB_DIVISAO_ID);
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
        public void DeleteRelatedEntries(TB_DIVISAO entity)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_DIVISAO_ID == entity.TB_DIVISAO_ID);
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
        public void DeleteRelatedEntries(TB_DIVISAO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_DIVISAO_ID == entity.TB_DIVISAO_ID);
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
        public TB_DIVISAO SelectOne(Expression<Func<TB_DIVISAO, bool>> where)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public List<TB_DIVISAO> SelectAll()
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_DIVISAO> SelectAll(Expression<Func<TB_DIVISAO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public List<TB_DIVISAO> SelectWhere(Expression<Func<TB_DIVISAO, bool>> where)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
                return infraestrutura.SelectWhere(where);
            }
            catch (Exception e)
            {
                base.GravarLogErro(e);
                throw new Exception(e.Message, e.InnerException);
                throw new Exception(e.Message, e);
            }
        }

        public List<DivisaoEntity> SelectExpression(Expression<Func<TB_DIVISAO, bool>> _where)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
                return infraestrutura.SelectByExpression(_where).ToList();
            }
            catch (Exception ex)
            {
                base.GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
                throw new Exception(ex.Message, ex);
            }
        }

        public List<DivisaoEntity> SelectById(int divisaoId)
        {
            try
            {
                Expression<Func<TB_DIVISAO, bool>> _expression = _ => _.TB_DIVISAO_ID == divisaoId;

                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
                return infraestrutura.SelectByExpression(_expression).ToList();
            }
            catch (Exception ex)
            {
                base.GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
                throw new Exception(ex.Message, ex);
            }
        }

        public List<DivisaoEntity> SelectSimplesExpression(Expression<Func<TB_DIVISAO, bool>> _where)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
                return infraestrutura.SelectSimplesByExpression(_where).ToList();
            }
            catch (Exception ex)
            {
                base.GravarLogErro(ex);
                throw new Exception(ex.Message, ex.InnerException);
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Retorna uma lista de objetos encontrados na condição, ordenados e paginados
        /// </summary>
        /// <param name="sortExpression">Expressão de ordenação para campos INT</param>
        /// <param name="desc">Ordem decrescente</param>
        /// <param name="where">Expressão lambda where</param>
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_DIVISAO> SelectWhere(Expression<Func<TB_DIVISAO, int>> sortExpression, bool desc, Expression<Func<TB_DIVISAO, bool>> where, int startRowIndex)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_DIVISAO> SelectWhere(Expression<Func<TB_DIVISAO, string>> sortExpression, bool desc, Expression<Func<TB_DIVISAO, bool>> where, int startRowIndex)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public IQueryable<TB_DIVISAO> QueryAll()
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>IQueryable de objetos</returns>
        public IQueryable<TB_DIVISAO> QueryAll(Expression<Func<TB_DIVISAO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public int GetCount(Expression<Func<TB_DIVISAO, bool>> where)
        {
            try
            {
                DivisaoInfrastructure infraestrutura = new DivisaoInfrastructure();
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
        public void Consistir(TB_DIVISAO entity)
        {
            try
            {
                if (entity.TB_DIVISAO_CODIGO == 0)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_DIVISAO_DESCRICAO))
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
