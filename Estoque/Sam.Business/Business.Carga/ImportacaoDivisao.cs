using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Common.Util;

namespace Sam.Business.Importacao
{
    public class ImportacaoDivisao : ImportacaoCargaControle, ICarga
    {
        public bool InsertImportacao(Infrastructure.TB_CONTROLE entityList)
        {
            try
            {
                bool isErro = new DivisaoBusiness().InsertListControleImportacao(entityList);
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

            //Valida Gestor
            if (carga.TB_GESTOR_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);
            else if (String.IsNullOrEmpty(carga.TB_GESTOR_NOME_REDUZIDO))
                listCargaErro.Add(GeralEnum.CargaErro.GestorInvalido);

            //Valida UA
            if (carga.TB_UA_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUAInvalido);
            else if (String.IsNullOrEmpty(carga.TB_UA_CODIGO))
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUAInvalido);

            //Valida UGE
            if (carga.TB_UGE_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);
            else if (String.IsNullOrEmpty(carga.TB_UGE_CODIGO))
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);

            //Valida UF
            if (carga.TB_UF_SIGLA == null)
                listCargaErro.Add(GeralEnum.CargaErro.SiglaUFInvalida);

            //Valida Responsavel
            if (carga.TB_RESPONSAVEL_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.CodigoResponsavelInvalido);
            else if (String.IsNullOrEmpty(carga.TB_RESPONSAVEL_CODIGO))
                listCargaErro.Add(GeralEnum.CargaErro.CodigoResponsavelInvalido);

            //Valida Divisao
            if (carga.TB_DIVISAO_ID == null)
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);
            else if (String.IsNullOrEmpty(carga.TB_DIVISAO_CODIGO))
                listCargaErro.Add(GeralEnum.CargaErro.CodigoUGEInvalido);
            else if (String.IsNullOrEmpty(carga.TB_DIVISAO_DESCRICAO))
                listCargaErro.Add(GeralEnum.CargaErro.DescricaoInvalida);
            else if (String.IsNullOrEmpty(carga.TB_DIVISAO_LOGRADOURO))
                listCargaErro.Add(GeralEnum.CargaErro.LogradouroInvalido);
            else if (String.IsNullOrEmpty(carga.TB_DIVISAO_CEP))
                listCargaErro.Add(GeralEnum.CargaErro.CEPInvalido);
            else if (String.IsNullOrEmpty(carga.TB_DIVISAO_AREA))
                listCargaErro.Add(GeralEnum.CargaErro.AreaInvalida);

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

            IQueryable<TB_DIVISAO> divisaoList;
            divisaoList = new DivisaoBusiness().QueryAll();

            IQueryable<TB_UA> uaList;
            uaList = new UaBusiness().QueryAll();

            IQueryable<TB_UGE> ugeList;
            ugeList = new UgeBusiness().QueryAll();

            IQueryable<TB_RESPONSAVEL> responsavelList;
            responsavelList = new ResponsavelBusiness().QueryAll();

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
                var divisao = RetornaDivisao(divisaoList, c.TB_DIVISAO_CODIGO);
                if (divisao != null)
                    c.TB_DIVISAO_ID = divisao.TB_DIVISAO_ID;

                var gestor = RetornaGestor(gestorList, c.TB_GESTOR_NOME_REDUZIDO);
                if (gestor != null)
                    c.TB_GESTOR_ID = gestor.TB_GESTOR_ID;

                var almoxarifado = RetornaAlmoxarifado(almoxList, c.TB_ALMOXARIFADO_CODIGO, c.TB_GESTOR_ID);
                if (almoxarifado != null)
                    c.TB_ALMOXARIFADO_ID = almoxarifado.TB_ALMOXARIFADO_ID;

                var unidadeAdministrativa = RetornaUnidadeAdministrativa(uaList, c.TB_UA_CODIGO, c.TB_GESTOR_ID);
                if (unidadeAdministrativa != null)
                    c.TB_UA_ID = unidadeAdministrativa.TB_UA_ID;

                //var unidadeGestora = RetornaUnidadeGestora(ugeList, c.TB_UGE_CODIGO);
                //if (unidadeGestora != null)
                //    c.TB_UGE_CODIGO = unidadeGestora.TB_UGE_CODIGO.ToString();

                var responsavel = RetornaResponsavel(responsavelList, c.TB_RESPONSAVEL_CODIGO, c.TB_GESTOR_ID);
                if (responsavel != null)
                    c.TB_RESPONSAVEL_ID = responsavel.TB_RESPONSAVEL_ID;

                var unidadeFederacao = RetornaUF(ufList, c.TB_UF_SIGLA);
                if (unidadeFederacao != null)
                    c.TB_UF_SIGLA = unidadeFederacao.TB_UF_SIGLA;
                #endregion Carga Divisao

                divisao               = null;
                gestor                = null;
                almoxarifado          = null;
                unidadeAdministrativa = null;
                responsavel           = null;
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
        private TB_CONTA_AUXILIAR RetornaContaAuxiliar(IQueryable<TB_CONTA_AUXILIAR> contaAuxiliarList, string codigo)
        {
            try
            {
                int? contaAuxCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    contaAuxCodigo = TratamentoDados.TryParseInt32(codigo);

                    return contaAuxiliarList.Where(contaAux => contaAux.TB_CONTA_AUXILIAR_CODIGO == contaAuxCodigo).FirstOrDefault();
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
        private TB_DIVISAO RetornaDivisao(IQueryable<TB_DIVISAO> divisaoList, string codigo)
        {
            try
            {
                int? divisaoCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    divisaoCodigo = TratamentoDados.TryParseInt32(codigo);

                    return divisaoList.Where(a => a.TB_DIVISAO_CODIGO == divisaoCodigo).FirstOrDefault();
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
        private TB_UA RetornaUnidadeAdministrativa(IQueryable<TB_UA> uaList, string codigo, int? gestorId)
        {
            try
            {
                int? uaCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    uaCodigo = TratamentoDados.TryParseInt32(codigo);

                    if (gestorId != null)
                    {
                        return uaList.Where(ua => ua.TB_UA_CODIGO == uaCodigo && ua.TB_GESTOR_ID == gestorId).FirstOrDefault();
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
        private TB_RESPONSAVEL RetornaResponsavel(IQueryable<TB_RESPONSAVEL> responsavelList, string codigo, int? gestorId)
        {
            try
            {
                int? responsavelCodigo;

                if (!string.IsNullOrEmpty(codigo))
                {
                    responsavelCodigo = TratamentoDados.TryParseInt32(codigo);

                    return responsavelList.Where(responsavel => responsavel.TB_RESPONSAVEL_CODIGO == responsavelCodigo && responsavel.TB_GESTOR_ID == gestorId).FirstOrDefault();
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
        
        public TB_CENTRO_CUSTO ValidaCentroDeCusto(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do Centro de Custo
            int? centroCustoCodigo = TratamentoDados.TryParseInt32(carga.TB_CENTRO_CUSTO_CODIGO);

            if (centroCustoCodigo != null)
            {
                var lObjCentroCusto = new CentroCustoBusiness().SelectWhere(centroCusto => Int32.Parse(centroCusto.TB_CENTRO_CUSTO_CODIGO) == centroCustoCodigo
                    && centroCusto.TB_GESTOR_ID == carga.TB_GESTOR_ID).FirstOrDefault();

                if (lObjCentroCusto != null)
                {
                    //O Centro de Custo existe para o gestor
                    return lObjCentroCusto;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }

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

        public TB_DIVISAO ValidaDivisao(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código do Gestor
            int? divisaoCodigo = TratamentoDados.TryParseInt32(carga.TB_DIVISAO_CODIGO);

            if (divisaoCodigo != null)
            {
                var lObjDivisao = new DivisaoBusiness().SelectWhere(divisao => (divisao.TB_DIVISAO_CODIGO == divisaoCodigo
                    && divisao.TB_UA.TB_UA_ID == carga.TB_UA_ID
                    && divisao.TB_UA.TB_GESTOR_ID == carga.TB_GESTOR_ID)).FirstOrDefault();

                if (lObjDivisao != null)
                    return lObjDivisao;
                else
                    return null;
            }
            else
                return null;

        }

        public TB_UA ValidaUA(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Código da UA
            int? uaCodigo = TratamentoDados.TryParseInt32(carga.TB_UA_CODIGO);

            if (uaCodigo != null)
            {
                var lObjUa = new UaBusiness().SelectWhere(Ua => Ua.TB_UA_CODIGO == uaCodigo).FirstOrDefault();

                if (lObjUa != null)
                {
                    //A UA existe para o gestor
                    return lObjUa;
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

        public TB_RESPONSAVEL ValidaResponsavel(TB_CARGA carga)
        {
            List<GeralEnum.CargaErro> listCargaErro = new List<GeralEnum.CargaErro>();

            //Valida o Responsável
            long? responsavelCodigo = Convert.ToInt64(carga.TB_RESPONSAVEL_CODIGO);

            if (responsavelCodigo != null)
            {
                var lObjResponsavel = new ResponsavelBusiness().SelectWhere(responsavel => (responsavel.TB_RESPONSAVEL_CODIGO == responsavelCodigo)
                    && responsavel.TB_GESTOR_ID == carga.TB_GESTOR_ID).FirstOrDefault();

                if (lObjResponsavel != null)
                {
                    //O Responsável existe para o gestor
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
            {
                return null;
            }

        }

        #endregion
    }
}
