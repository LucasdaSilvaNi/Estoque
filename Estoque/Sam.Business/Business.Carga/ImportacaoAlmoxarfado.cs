using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;

namespace Sam.Business.Importacao
{
    public class ImportacaoAlmoxarifado : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new AlmoxarifadoBusiness().InsertListControleImportacao(entityList);
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


            if (carga.TB_ALMOXARIFADO_ID != null)
            {
                int cod = Convert.ToInt32(carga.TB_ALMOXARIFADO_CODIGO);
                int uge = Convert.ToInt32(carga.TB_UGE_ID);
                if (new AlmoxarifadoBusiness().SelectWhere(a => a.TB_ALMOXARIFADO_CODIGO == cod
                    && a.TB_GESTOR_ID == (int)carga.TB_GESTOR_ID && a.TB_UGE_ID == uge).Count() > 0)
                {
                    listCargaErro.Add(GeralEnum.CargaErro.CodigoCadastrado);
                }
            }


                    
            //Valida Gestor
            if (carga.TB_GESTOR_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
            else if (String.IsNullOrEmpty(carga.TB_GESTOR_NOME_REDUZIDO))
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);

            //Valida UGE
            if (carga.TB_UGE_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);
            else if (String.IsNullOrEmpty(carga.TB_UGE_CODIGO))
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);

            //Valida UF
            if (carga.TB_UF_SIGLA == null)
                listCargaErro.Add(GeralEnum.CargaErro.SiglaUFInvalida);

          
            return listCargaErro;
        }

        public List<TB_CARGA> ProcessarExcel(IQueryable<TB_CARGA> linhasPlanilha, TB_CONTROLE objetoControlador)
        {
            List<TB_CARGA> lLstRetorno = new List<TB_CARGA>();

            StringBuilder TB_CARGA_SEQ = new StringBuilder();

            #region Registros Tabelas

            IQueryable<TB_ALMOXARIFADO> almoxList;
            almoxList = new AlmoxarifadoBusiness().QueryAll();

            IQueryable<TB_GESTOR> gestorList;
            gestorList = new GestorBusiness().QueryAll();

       
            IQueryable<TB_UGE> ugeList;
            ugeList = new UgeBusiness().QueryAll();


            IQueryable<TB_UF> ufList;
            ufList = new UfBusiness().QueryAll();
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

                var almoxarifado = RetornaAlmoxarifado(almoxList, c.TB_ALMOXARIFADO_CODIGO, c.TB_GESTOR_ID);
                if (almoxarifado != null)
                    c.TB_ALMOXARIFADO_ID = almoxarifado.TB_ALMOXARIFADO_ID;

                var unidadeGestora = RetornaUnidadeGestora(ugeList, c.TB_UGE_CODIGO);
                if (unidadeGestora != null)
                    c.TB_UGE_CODIGO = unidadeGestora.TB_UGE_CODIGO.ToString();

                var unidadeFederacao = RetornaUF(ufList, c.TB_UF_SIGLA);
                if (unidadeFederacao != null)
                    c.TB_UF_SIGLA = unidadeFederacao.TB_UF_SIGLA;
                #endregion Carga Divisao

                gestor                = null;
                almoxarifado          = null;
                unidadeFederacao      = null;                

                lLstRetorno.Add(c);
            }

            return lLstRetorno;
        }

        #region Retorna os dados das tabelas
        
        private TB_ALMOXARIFADO RetornaAlmoxarifado(IQueryable<TB_ALMOXARIFADO> almoxList, string codigo, int? gestorId)
        {
            try
            {
                int? almoxCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {

                    almoxCodigo = TratamentoDados.TryParseInt32(codigo);
                    return almoxList.Where(almox => almox.TB_ALMOXARIFADO_CODIGO == almoxCodigo && almox.TB_GESTOR_ID == gestorId).FirstOrDefault();
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
        private TB_UF RetornaUF(IQueryable<TB_UF> ufList, string nomeReduzido)
        {
            try
            {
                if (!string.IsNullOrEmpty(nomeReduzido))
                {
                    return ufList.Where(uf => uf.TB_UF_SIGLA == nomeReduzido).FirstOrDefault();
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
        

        
        public TB_ALMOXARIFADO ValidaAlmoxarifado(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do almoxarifado
            int? almoxCodigo = TratamentoDados.TryParseInt32(carga.TB_ALMOXARIFADO_CODIGO);

            if (almoxCodigo != null)
            {
                var lObjAlmoxarifado = new AlmoxarifadoBusiness().SelectWhere(almox => almox.TB_ALMOXARIFADO_CODIGO == almoxCodigo
                    && almox.TB_GESTOR_ID == carga.TB_GESTOR_ID).FirstOrDefault();

                if (lObjAlmoxarifado != null)
                {
                    //O almoxarifado existe para o gestor
                    return lObjAlmoxarifado;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }

        public TB_UGE ValidaUGE(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código da UGE
            int? ugeCodigo = TratamentoDados.TryParseInt32(carga.TB_UGE_CODIGO);

            if (ugeCodigo != null)
            {
                var lObjUGE = new UgeBusiness().SelectWhere(Uge => Uge.TB_UGE_ID == ugeCodigo).FirstOrDefault();

                if (lObjUGE != null)
                {
                    //O Centro de Custo existe para o gestor
                    return lObjUGE;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }

        public TB_UF ValidaUF(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do estado (UF)
            string siglaUF = carga.TB_UF_SIGLA;

            if (!String.IsNullOrEmpty(siglaUF))
            {
                var lObjUF = new UfBusiness().SelectWhere(uf => uf.TB_UF_SIGLA == siglaUF).FirstOrDefault();

                if (lObjUF != null)
                {
                    //UF existente para divisão
                    return lObjUF;
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
