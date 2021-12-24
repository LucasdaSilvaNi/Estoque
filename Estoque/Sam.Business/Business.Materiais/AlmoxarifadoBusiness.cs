using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Sam.Domain.Entity;
using Sam.Common.Util;
using Sam.Business.Importacao;
using System.Text.RegularExpressions;
using Sam.Common.Enums;
using System.Data;

namespace Sam.Business
{
    public class AlmoxarifadoBusiness : BaseBusiness, ICrudBaseBusiness<TB_ALMOXARIFADO>
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

        public IList<AlmoxarifadoEntity> ListarAlmoxarifadosPorNivelAcesso(int idGestor, List<AlmoxarifadoEntity> almoxarifadosNivelAcesso)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadosNivelAcesso(idGestor, almoxarifadosNivelAcesso);
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorGestorMovimentoPendente(int GestorId)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadoPorGestorMovimentoPendente(GestorId);
        }

        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorUge(int UgeId)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadoPorUge(UgeId);
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoPorGestor(int GestorId)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadoPorGestor(GestorId);
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoAtivoPorGestor(int GestorId)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadoAtivoPorGestor(GestorId);
        }
        public IList<TB_ALMOXARIFADO> ListarAlmoxarifadoAtivoPorGestor(int GestorId, RepositorioEnum.Repositorio Repositorio)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();
            return infra.ListarAlmoxarifadoAtivoPorGestor(GestorId, Repositorio);
        }

        public void InsertAlmoxarifadoTransacao(List<TB_ALMOXARIFADO> almoxarifadoList)
        {
            AlmoxarifadoInfrastructure infra = new AlmoxarifadoInfrastructure();

            foreach (var almox in almoxarifadoList)
            {
                infra.Context.AddToTB_ALMOXARIFADO(almox);
            }

            infra.Context.SaveChanges();
        }


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
                    AlmoxarifadoInfrastructure infraAlmoxarifado = new AlmoxarifadoInfrastructure();
                    seq.Clear();
                    seq.Append(carga.TB_CARGA_SEQ);

                    var almoxarifado = MontarListaAlmoxarifado(carga);

                    if (this.ConsistidoCargaErro)
                    {
                        carga.TB_CARGA_VALIDO = true;
                        cargaBusiness.Update(carga);

                        infraAlmoxarifado.Insert(almoxarifado);

                        infraAlmoxarifado.SaveChanges();
                        infraAlmoxarifado.Dispose();

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


        private TB_ALMOXARIFADO MontarListaAlmoxarifado(TB_CARGA carga)
        {
            ListCargaErro.Clear();

            TB_ALMOXARIFADO almox = new TB_ALMOXARIFADO();


            ListCargaErro = new ImportacaoAlmoxarifado().ValidarImportacao(carga);

            //Valida Codigo divisão da planilha e verifica se já cadastrado.
            if (almox != null)
            {
                if (!String.IsNullOrEmpty(almox.TB_ALMOXARIFADO_CODIGO.ToString()))
                {
                    almox.TB_ALMOXARIFADO_CODIGO = Int32.Parse(carga.TB_ALMOXARIFADO_CODIGO);

                }
                else if (!(almox.TB_ALMOXARIFADO_CODIGO.ToString().Length < 4 && ValidaAlfaNumerico(almox.TB_ALMOXARIFADO_CODIGO.ToString())))
                    ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);
            }
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoInvalido);

            if (carga.TB_GESTOR_ID != null)
                almox.TB_GESTOR_ID = Convert.ToInt32(carga.TB_GESTOR_ID);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);

            if (carga.TB_UGE_ID != null)
                almox.TB_UGE_ID = Convert.ToInt32(carga.TB_UGE_ID);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);


            if (carga.TB_UF_ID != null)
                almox.TB_UF_ID = Convert.ToInt32(carga.TB_UF_ID);
            else
                ListCargaErro.Add(GeralEnum.CargaErro.SiglaUFInvalida);


            almox.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = carga.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE.ToUpper() == "S" ? true : false;


            string almoxDescricao = carga.TB_ALMOXARIFADO_DESCRICAO;
            if (!String.IsNullOrEmpty(almoxDescricao))
                almox.TB_ALMOXARIFADO_DESCRICAO = almoxDescricao;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.DescricaoInvalida);


            string almoxLogradouro = carga.TB_ALMOXARIFADO_LOGRADOURO;
            if (!String.IsNullOrEmpty(almoxLogradouro))
                almox.TB_ALMOXARIFADO_LOGRADOURO = almoxLogradouro;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.LogradouroInvalido);


            string almoxNumero = carga.TB_ALMOXARIFADO_NUMERO;
            if (!String.IsNullOrEmpty(almoxNumero))
                almox.TB_ALMOXARIFADO_NUMERO = almoxNumero;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.NumeroLogradouroInvalido);

            string almoxCompl = carga.TB_ALMOXARIFADO_COMPLEMENTO;
            if (!String.IsNullOrEmpty(almoxCompl))
                almox.TB_ALMOXARIFADO_COMPLEMENTO = almoxCompl;
            else
                almox.TB_ALMOXARIFADO_COMPLEMENTO = string.Empty;


            string almoxBairro = carga.TB_ALMOXARIFADO_BAIRRO;
            if (!String.IsNullOrEmpty(almoxBairro))
                almox.TB_ALMOXARIFADO_BAIRRO = almoxBairro;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.BairroInvalido);

            string almoxMunicipio = carga.TB_ALMOXARIFADO_MUNICIPIO;
            if (!String.IsNullOrEmpty(almoxMunicipio))
                almox.TB_ALMOXARIFADO_MUNICIPIO = almoxMunicipio;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.MunicipioInvalido);



            string almoxcep = carga.TB_ALMOXARIFADO_CEP;
            if (!String.IsNullOrEmpty(almoxcep))
                almox.TB_ALMOXARIFADO_CEP = almoxcep;
            else
                ListCargaErro.Add(GeralEnum.CargaErro.AreaInvalida);


            string almoxTelefone = carga.TB_ALMOXARIFADO_TELEFONE;
            if (!String.IsNullOrEmpty(almoxTelefone))
                almox.TB_ALMOXARIFADO_TELEFONE = almoxTelefone;
            else
                almox.TB_ALMOXARIFADO_TELEFONE = string.Empty;


            string almoxFax = carga.TB_ALMOXARIFADO_FAX;
            if (!String.IsNullOrEmpty(almoxFax))
                almox.TB_ALMOXARIFADO_FAX = almoxFax;
            else
                almox.TB_ALMOXARIFADO_FAX = string.Empty;


            string almoxResp = carga.TB_ALMOXARIFADO_RESPONSAVEL;
            if (!String.IsNullOrEmpty(almoxResp))
                almox.TB_ALMOXARIFADO_RESPONSAVEL = almoxResp;
            else
                almox.TB_ALMOXARIFADO_RESPONSAVEL = string.Empty;

            string almoxMesRef = carga.TB_ALMOXARIFADO_MES_REF_INICIAL;
            if (!String.IsNullOrEmpty(almoxMesRef))
            {
                almox.TB_ALMOXARIFADO_MES_REF_INICIAL = almoxMesRef;
                almox.TB_ALMOXARIFADO_MES_REF = almoxMesRef;
            }
            else
                ListCargaErro.Add(GeralEnum.CargaErro.AnoMesRefInvalido);


            //string almoxIndAtiv = carga.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE;
            //if (!String.IsNullOrEmpty(almoxIndAtiv))
            //    almox.TB_ALMOXARIFADO_INDICADOR_ATIVIDADE = almoxIndAtiv;
            //else
            //    ListCargaErro.Add(GeralEnum.CargaErro.CodigoDivisaoInvalido);


            return almox;
        }

        #endregion

        #region Métodos Genéricos

        /// <summary>
        /// Insere um novo objeto consistido
        /// </summary>
        /// <param name="entity">Objeto</param>
        public void Insert(TB_ALMOXARIFADO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public void Update(TB_ALMOXARIFADO entity)
        {
            try
            {
                Consistir(entity);
                if (base.Consistido)
                {
                    AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public void Delete(TB_ALMOXARIFADO entity)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ALMOXARIFADO_ID == entity.TB_ALMOXARIFADO_ID);
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
        public void DeleteRelatedEntries(TB_ALMOXARIFADO entity)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ALMOXARIFADO_ID == entity.TB_ALMOXARIFADO_ID);
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
        /// Exclui os objetos relacionados com excessão a lista de ignorados
        /// </summary>
        /// <param name="entity">Objetos</param>
        /// <param name="keyListOfIgnoreEntites">Lista de tabelas ignoradas</param>
        public void DeleteRelatedEntries(TB_ALMOXARIFADO entity, ObservableCollection<string> keyListOfIgnoreEntites)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();

                //Atualiza o objeto
                entity = infraestrutura.SelectOne(a => a.TB_ALMOXARIFADO_ID == entity.TB_ALMOXARIFADO_ID);
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
        public TB_ALMOXARIFADO SelectOne(Expression<Func<TB_ALMOXARIFADO, bool>> where)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public List<TB_ALMOXARIFADO> SelectAll()
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public List<TB_ALMOXARIFADO> SelectAll(Expression<Func<TB_ALMOXARIFADO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public List<TB_ALMOXARIFADO> SelectWhere(Expression<Func<TB_ALMOXARIFADO, bool>> where)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        /// <param name="maximumRows">NÃºmero máximo de registros que irá retornar</param>
        /// <param name="startRowIndex">Ã­ndice da paginação</param>
        /// <returns>Lista de objetos</returns>
        public List<TB_ALMOXARIFADO> SelectWhere(Expression<Func<TB_ALMOXARIFADO, int>> sortExpression, bool desc, Expression<Func<TB_ALMOXARIFADO, bool>> where, int startRowIndex)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public List<TB_ALMOXARIFADO> SelectWhere(Expression<Func<TB_ALMOXARIFADO, string>> sortExpression, bool desc, Expression<Func<TB_ALMOXARIFADO, bool>> where, int startRowIndex)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public IQueryable<TB_ALMOXARIFADO> QueryAll()
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public IQueryable<TB_ALMOXARIFADO> QueryAll(Expression<Func<TB_ALMOXARIFADO, object>> sortExpression, int maximumRows, int startRowIndex)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public int GetCount(Expression<Func<TB_ALMOXARIFADO, bool>> where)
        {
            try
            {
                AlmoxarifadoInfrastructure infraestrutura = new AlmoxarifadoInfrastructure();
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
        public void Consistir(TB_ALMOXARIFADO entity)
        {
            try
            {
                if (entity.TB_ALMOXARIFADO_CODIGO == null)
                {
                    throw new Exception("O campo Código é de preenchimento obritagório");
                }
                if (string.IsNullOrEmpty(entity.TB_ALMOXARIFADO_DESCRICAO))
                {
                    throw new Exception("O campo Descrição é de preenchimento obritagório");
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
