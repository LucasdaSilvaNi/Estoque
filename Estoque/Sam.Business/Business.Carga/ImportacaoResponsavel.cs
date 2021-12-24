using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;

namespace Sam.Business.Importacao
{
    public class ImportacaoResponsavel : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new ResponsavelBusiness().InsertListControleImportacao(entityList);
                base.AtualizaControle(isErro, entityList.TB_CONTROLE_ID);
                return isErro;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<GeralEnum.CargaErro> ValidarImportacao(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();


            if (carga.TB_RESPONSAVEL_ID != null)
            {
                int codResp = Convert.ToInt32(carga.TB_RESPONSAVEL_CODIGO);
                if (new ResponsavelBusiness().SelectWhere(a => a.TB_RESPONSAVEL_CODIGO == codResp
                    && a.TB_GESTOR_ID == (int)carga.TB_GESTOR_ID).Count() > 0)
                {
                    listCargaErro.Add(GeralEnum.CargaErro.CodigoCadastrado);
                }
            }


                    
            //Valida Gestor
            if (carga.TB_GESTOR_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
            else if (String.IsNullOrEmpty(carga.TB_GESTOR_NOME_REDUZIDO))
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);

         

          
            return listCargaErro;
        }

        public List<TB_CARGA> ProcessarExcel(IQueryable<TB_CARGA> linhasPlanilha, TB_CONTROLE objetoControlador)
        {
            List<TB_CARGA> lLstRetorno = new List<TB_CARGA>();

            StringBuilder TB_CARGA_SEQ = new StringBuilder();

            #region Registros Tabelas

            IQueryable<TB_RESPONSAVEL> respList;
            respList = new ResponsavelBusiness().QueryAll();

            IQueryable<TB_GESTOR> gestorList;
            gestorList = new GestorBusiness().QueryAll();

       
            
            #endregion Registros Tabelas

            foreach (var c in linhasPlanilha.ToList())
            {
                if (String.IsNullOrEmpty(c.TB_CARGA_SEQ))
                    break;

                //Utilizado para mostrar na exception
                TB_CARGA_SEQ.Clear();
                TB_CARGA_SEQ.Append(c.TB_CARGA_SEQ);

                //Adiciona o controle
                c.TB_CONTROLE = objetoControlador;

                #region Carga Divisao


                var gestor = RetornaGestor(gestorList, c.TB_GESTOR_NOME_REDUZIDO);
                if (gestor != null)
                    c.TB_GESTOR_ID = gestor.TB_GESTOR_ID;

                var responsavel = RetornaResponsavel(respList, c.TB_RESPONSAVEL_CODIGO, c.TB_GESTOR_ID);
                if (responsavel != null)
                    c.TB_RESPONSAVEL_ID = responsavel.TB_RESPONSAVEL_ID;

               
                #endregion Carga Divisao

                gestor                = null;
                responsavel          = null;
              

                lLstRetorno.Add(c);
            }

            return lLstRetorno;
        }

        #region Retorna os dados das tabelas

        private TB_RESPONSAVEL RetornaResponsavel(IQueryable<TB_RESPONSAVEL> respList, string codigo, int? gestorId)
        {
            try
            {
                int? respCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {

                    respCodigo = TratamentoDados.TryParseInt32(codigo);
                    return respList.Where(resp => resp.TB_RESPONSAVEL_CODIGO == respCodigo && resp.TB_GESTOR_ID == gestorId).FirstOrDefault();
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
        private TB_GESTOR RetornaGestor(IQueryable<TB_GESTOR> gestorList, string nomeReduzido)
        {
            try
            {
                if (!string.IsNullOrEmpty(nomeReduzido))
                {
                    return gestorList.Where(gestor => gestor.TB_GESTOR_NOME_REDUZIDO == nomeReduzido).FirstOrDefault();
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

        #region Validações
        

        
        public TB_RESPONSAVEL ValidaResp(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do responsavel
            int? respCodigo = TratamentoDados.TryParseInt32(carga.TB_RESPONSAVEL_CODIGO);

            if (respCodigo != null)
            {
                var lObjResponsavel = new ResponsavelBusiness().SelectWhere(resp => resp.TB_RESPONSAVEL_CODIGO == respCodigo
                    && resp.TB_GESTOR_ID == carga.TB_GESTOR_ID).FirstOrDefault();

                if (lObjResponsavel != null)
                {
                    //O responsavel existe para o gestor
                    return lObjResponsavel;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }


        
        public TB_GESTOR ValidaGestor(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Gestor
            string nomeGestor = carga.TB_GESTOR_NOME_REDUZIDO;

            if (!String.IsNullOrEmpty(carga.TB_GESTOR_NOME_REDUZIDO))
            {
               
                    var lObjGestor = new GestorBusiness().SelectWhere(gestor => (gestor.TB_GESTOR_NOME_REDUZIDO == nomeGestor)).FirstOrDefault();

                    if (lObjGestor != null)
                        return lObjGestor;
                    else
                        return null;
                }
                else
                    return null;           

        }

        #endregion

       
    }
}
