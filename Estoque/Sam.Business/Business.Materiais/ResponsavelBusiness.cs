using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Common.Util;
using System.Text.RegularExpressions;
using Sam.Business.Importacao;


namespace Sam.Business
{
    public class ResponsavelBusiness : BaseBusiness, ICrudBaseBusiness<TB_RESPONSAVEL>
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
                    ResponsavelInfrastructure infraResponsavel = new ResponsavelInfrastructure();
                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);

                    var responsavel = MontarListaResponsavel(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);

                        infraResponsavel.Insert(responsavel);

                        infraResponsavel.SaveChanges();
                        infraResponsavel.Dispose();

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

        private bool ValidaAlfaNumerico(string valor)
        {
            Regex rgx = new Regex(@"^[a-zA-Z0-9]\d{2}[a-zA-Z0-9](-\d{3}){2}[A-Za-z0-9]$");

            return rgx.IsMatch(valor) ? true : false;
        }


        private TB_RESPONSAVEL MontarListaResponsavel(TB_CARGA carga)
        {
            ListCargaErro.Clear();

            TB_RESPONSAVEL resp = new TB_RESPONSAVEL();


            ListCargaErro = new ImportacaoResponsavel().ValidarImportacao(carga);

            //Valida Codigo divisão da planilha e verifica se já cadastrado.
            if (resp != null)
            {
                if (!String.IsNullOrEmpty(carga.TB_RESPONSAVEL_CODIGO.ToString()))
                {
                    resp.TB_RESPONSAVEL_CODIGO = Int64.Parse(carga.TB_RESPONSAVEL_CODIGO);
                }
                else if (!(ValidaAlfaNumerico(carga.TB_RESPONSAVEL_CODIGO.ToString())))
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);



            }
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);

            if (carga.TB_GESTOR_ID != null)
                resp.TB_GESTOR_ID = Convert.ToInt32(carga.TB_GESTOR_ID);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);



            string almoxRespN = carga.TB_RESPONSAVEL_NOME ;
            if (!String.IsNullOrEmpty(almoxRespN))
                resp.TB_RESPONSAVEL_NOME = almoxRespN;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.DescricaoInvalida);


            string almoxRespC = carga.TB_RESPONSAVEL_CARGO;
            if (!String.IsNullOrEmpty(almoxRespC))
                resp.TB_RESPONSAVEL_CARGO = almoxRespC;
            else
                resp.TB_RESPONSAVEL_CARGO = string.Empty;


            string almoxRespE = carga.TB_RESPONSAVEL_ENDERECO;
            if (!String.IsNullOrEmpty(almoxRespE))
                resp.TB_RESPONSAVEL_ENDERECO = almoxRespE;
            else
                resp.TB_RESPONSAVEL_ENDERECO = string.Empty;

            

            return resp;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_RESPONSAVEL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public void Update(TB_RESPONSAVEL entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public void Delete(TB_RESPONSAVEL entity)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_RESPONSAVEL_ID == entity.TB_RESPONSAVEL_ID);
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
        public void DeleteRelatedEntries(TB_RESPONSAVEL entity)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_RESPONSAVEL_ID == entity.TB_RESPONSAVEL_ID);
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
        public void DeleteRelatedEntries(TB_RESPONSAVEL entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_RESPONSAVEL_ID == entity.TB_RESPONSAVEL_ID);
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
        public TB_RESPONSAVEL SelectOne(Expression<Func<TB_RESPONSAVEL, bool>> where)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public List<TB_RESPONSAVEL> SelectAll()
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public List<TB_RESPONSAVEL> SelectAll(Expression<Func<TB_RESPONSAVEL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public List<TB_RESPONSAVEL> SelectWhere(Expression<Func<TB_RESPONSAVEL, bool>> where)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public List<TB_RESPONSAVEL> SelectWhere(Expression<Func<TB_RESPONSAVEL, int>> sortExpression, bool desc, Expression<Func<TB_RESPONSAVEL, bool>> where, int startRowIndex)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public List<TB_RESPONSAVEL> SelectWhere(Expression<Func<TB_RESPONSAVEL, string>> sortExpression, bool desc, Expression<Func<TB_RESPONSAVEL, bool>> where, int startRowIndex)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public IQueryable<TB_RESPONSAVEL> QueryAll()
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public IQueryable<TB_RESPONSAVEL> QueryAll(Expression<Func<TB_RESPONSAVEL, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public int GetCount(Expression<Func<TB_RESPONSAVEL, bool>> where)
        {
            try
            {
                ResponsavelInfrastructure infraestrutura = new ResponsavelInfrastructure();
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
        public void Consistir(TB_RESPONSAVEL entity)
        {
            try
            {
                if (entity.TB_RESPONSAVEL_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_RESPONSAVEL_NOME))
                {
                    throw new Exception("O campo Nome é de preenchimento obritagório");
                }
                if (entity.TB_GESTOR_ID == null)
                {
                    throw new Exception("O campo Gestor é de preenchimento obritagório");
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
