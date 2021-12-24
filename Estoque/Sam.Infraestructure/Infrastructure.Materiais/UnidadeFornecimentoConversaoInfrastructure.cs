using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.Entity;


namespace Sam.Infrastructure
{
    public class UnidadeFornecimentoConversaoInfrastructure : AbstractCrud<TB_UNIDADE_FORNECIMENTO_CONVERSAO, SAMwebEntities>
    {
        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversaoPorGestor(int? Gestor_ID)
        {
            List<UnidadeFornecimentoConversaoEntity> lstRetorno   = new List<UnidadeFornecimentoConversaoEntity>();
            List<TB_UNIDADE_FORNECIMENTO_CONVERSAO>  lstRetornoBD = null;

            Expression<Func<TB_UNIDADE_FORNECIMENTO_CONVERSAO, bool>> expWhere;

            expWhere = this.MontarWherePorGestor(Gestor_ID.Value);


            lstRetornoBD = this.SelectWhere(expWhere);
            lstRetornoBD.ForEach(rowUnidadeFornecimentoConversao => lstRetorno.Add(this.MapearDTO(rowUnidadeFornecimentoConversao)));
            lstRetornoBD.OrderBy(UnidadeFornecimentoConversao => new { UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO , UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID });

            return lstRetorno;
        }
        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversao()
        {
            List<UnidadeFornecimentoConversaoEntity> lstRetorno   = new List<UnidadeFornecimentoConversaoEntity>();
            List<TB_UNIDADE_FORNECIMENTO_CONVERSAO>  lstRetornoBD = null;

            Expression<Func<TB_UNIDADE_FORNECIMENTO_CONVERSAO, bool>> expWhere;

            expWhere = (UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID > 0);


            lstRetornoBD = this.SelectWhere(expWhere);
            lstRetornoBD.ForEach(rowUnidadeFornecimentoConversao => lstRetorno.Add(this.MapearDTO(rowUnidadeFornecimentoConversao)));
            lstRetornoBD.OrderBy(UnidadeFornecimentoConversao => new { UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO , UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID });

            return lstRetorno;
        }
        public UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversao(int UnidadeFornecimentoConversao_ID)
        {
            UnidadeFornecimentoConversaoEntity         objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_CONVERSAO          rowUnidadeFornecimentoConversao = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura                  = new UnidadeFornecimentoConversaoInfrastructure();

            
            rowUnidadeFornecimentoConversao = this.SelectOne(UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID == UnidadeFornecimentoConversao_ID);

            if (rowUnidadeFornecimentoConversao != null)
                objRetorno = this.MapearDTO(rowUnidadeFornecimentoConversao);


                return objRetorno;
        }
        public UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversao(string strCodigoUnidadeConversao)
        {
            UnidadeFornecimentoConversaoEntity         objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_CONVERSAO          rowUnidadeFornecimentoConversao = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura                  = new UnidadeFornecimentoConversaoInfrastructure();


            rowUnidadeFornecimentoConversao = this.SelectOne(UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO == strCodigoUnidadeConversao);

            if (rowUnidadeFornecimentoConversao != null)
                objRetorno = this.MapearDTO(rowUnidadeFornecimentoConversao);


                return objRetorno;
        }
        public UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversao(string strDescricaoUnidadeSIAFISICO, int subitemMaterialId)
        {
            UnidadeFornecimentoConversaoEntity         objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_CONVERSAO          rowUnidadeFornecimentoConversao = null;
            TB_SUBITEM_MATERIAL                        rowSubitemMaterial              = null;
            UnidadeFornecimentoConversaoInfrastructure infraUFC                        = new UnidadeFornecimentoConversaoInfrastructure();
            SubItemMaterialInfrastructure              infraSubitemMaterial            = new SubItemMaterialInfrastructure();

            this.LazyLoadingEnabled = true; 

            rowSubitemMaterial = infraSubitemMaterial.SelectOne(_subitemMaterial => _subitemMaterial.TB_SUBITEM_MATERIAL_ID == subitemMaterialId);// &&
                                                                                    //_subitemMaterial.TB_GESTOR_ID == gestorId);
            if (rowSubitemMaterial.IsNotNull())
                rowUnidadeFornecimentoConversao = this.SelectOne(unidadeConversao => unidadeConversao.TB_UNIDADE_FORNECIMENTO_SIAF.TB_UNIDADE_FORNECIMENTO_DESCRICAO == strDescricaoUnidadeSIAFISICO &&
                                                                                     unidadeConversao.TB_UNIDADE_FORNECIMENTO_SISTEMA_SAM_ID == rowSubitemMaterial.TB_UNIDADE_FORNECIMENTO_ID &&
                                                                                     unidadeConversao.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID == rowSubitemMaterial.TB_GESTOR_ID);

            if (rowUnidadeFornecimentoConversao.IsNotNull())
            {
                var unidadeSAM = new UnidadeFornecimentoEntity() { 
                                                                    Id = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID, 
                                                                    Codigo = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                    Descricao = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                                                 };

                var unidadeSiafisico = new UnidadeFornecimentoSiafEntity() { 
                                                                             Codigo = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAF.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                             Descricao = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAF.TB_UNIDADE_FORNECIMENTO_DESCRICAO 
                                                                           };

                objRetorno = new UnidadeFornecimentoConversaoEntity(unidadeSAM, unidadeSiafisico){
                                                                                                   Id = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID,
                                                                                                   Codigo = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO,
                                                                                                   Descricao = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_DESCRICAO,
                                                                                                   FatorUnitario = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_FATOR_UNITARIO
                                                                                                 };
            }

            this.LazyLoadingEnabled = false;
            return objRetorno;
        }
        public UnidadeFornecimentoConversaoEntity        ObterDadosUnidadeFornecimentoConversao(int? gestorId, string strCodigoUnidadeSIAFISICO, int unidadeFornecimentoId)
        {
            UnidadeFornecimentoConversaoEntity           objRetorno                      = null;
            TB_UNIDADE_FORNECIMENTO_CONVERSAO            rowUnidadeFornecimentoConversao = null;
            UnidadeFornecimentoConversaoInfrastructure   infraUFC                        = new UnidadeFornecimentoConversaoInfrastructure();
            SubItemMaterialInfrastructure                infraSubitemMaterial            = new SubItemMaterialInfrastructure();


            rowUnidadeFornecimentoConversao = this.SelectOne(unidadeConversao =>  unidadeConversao.TB_UNIDADE_FORNECIMENTO_SIAFISICO_CODIGO == Int32.Parse(strCodigoUnidadeSIAFISICO) &&
                                                                                  unidadeConversao.TB_UNIDADE_FORNECIMENTO_SISTEMA_SAM_ID == unidadeFornecimentoId &&
                                                                                  unidadeConversao.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID == gestorId );
           
            return objRetorno;
        }

        public bool Insert(UnidadeFornecimentoConversaoEntity objUnidadeFornecimentoConversao)
        {
            TB_UNIDADE_FORNECIMENTO_CONVERSAO rowUnidadeFornecimentoConversao = this.MapearEntity(objUnidadeFornecimentoConversao);

            if (rowUnidadeFornecimentoConversao.EntityState == System.Data.EntityState.Modified)
                this.Update(rowUnidadeFornecimentoConversao);
            else if (rowUnidadeFornecimentoConversao.EntityState == System.Data.EntityState.Detached)
                this.Insert(rowUnidadeFornecimentoConversao);

            this.SaveChanges();
            return (this.SelectOne(UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID == rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID) != null);
        }
        public bool Delete(UnidadeFornecimentoConversaoEntity objUnidadeFornecimentoConversao)
        {
            TB_UNIDADE_FORNECIMENTO_CONVERSAO rowUnidadeFornecimentoConversao = this.MapearEntity(objUnidadeFornecimentoConversao);
            this.Delete(rowUnidadeFornecimentoConversao);

            this.SaveChanges();
            return (this.SelectOne(UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID == rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID) != null);
        }

        internal Expression<Func<TB_UNIDADE_FORNECIMENTO_CONVERSAO, bool>> MontarWherePorGestor(int Gestor_ID)
        {
            Expression<Func<TB_UNIDADE_FORNECIMENTO_CONVERSAO, bool>> lExpRetorno = null;

            lExpRetorno = rowTabela => rowTabela.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID == Gestor_ID;

            return lExpRetorno;
        }

        #region Mappers
        internal UnidadeFornecimentoConversaoEntity MapearDTO(TB_UNIDADE_FORNECIMENTO_CONVERSAO rowUnidadeFornecimentoConversao)
        {
            this.LazyLoadingEnabled = true;
            rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTOReference.Load(MergeOption.PreserveChanges);
            rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAFReference.Load(MergeOption.PreserveChanges);

            var UnidadeFornecimentoSAM  = new Domain.Entity.UnidadeFornecimentoEntity()
                                        { 
                                          Id        = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                          Codigo    = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                          Descricao = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                          Gestor    = new Domain.Entity.GestorEntity(rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO.TB_GESTOR_ID),
                                        };

            var UnidadeFornecimentoSiafisico = new UnidadeFornecimentoSiafEntity()
                                             {
                                                Codigo    = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAF.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                Descricao = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAF.TB_UNIDADE_FORNECIMENTO_DESCRICAO,
                                             };
            
            UnidadeFornecimentoConversaoEntity lObjRetorno = new UnidadeFornecimentoConversaoEntity(UnidadeFornecimentoSAM, UnidadeFornecimentoSiafisico);

            lObjRetorno.Id                      = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID;
            lObjRetorno.Codigo                  = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO;
            lObjRetorno.Descricao               = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_DESCRICAO;
            lObjRetorno.SistemaSamId            = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SISTEMA_SAM_ID;
            lObjRetorno.SistemaSiafisicoCodigo  = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_SIAFISICO_CODIGO;
            lObjRetorno.FatorUnitario           = rowUnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_FATOR_UNITARIO;

            return lObjRetorno;
        }
        internal TB_UNIDADE_FORNECIMENTO_CONVERSAO MapearEntity(UnidadeFornecimentoConversaoEntity objUnidadeFornecimentoConversao)
        {
            TB_UNIDADE_FORNECIMENTO_CONVERSAO lObjRetorno = null;

            //lObjRetorno.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID                  = objUnidadeFornecimentoConversao.Id.Value;
            lObjRetorno = this.SelectOne(UnidadeFornecimentoConversao => UnidadeFornecimentoConversao.TB_UNIDADE_FORNECIMENTO_CONVERSAO_ID == objUnidadeFornecimentoConversao.Id.Value);

            if(lObjRetorno == null)
                lObjRetorno = new TB_UNIDADE_FORNECIMENTO_CONVERSAO();

            lObjRetorno.TB_UNIDADE_FORNECIMENTO_CONVERSAO_CODIGO              = objUnidadeFornecimentoConversao.Codigo;
            lObjRetorno.TB_UNIDADE_FORNECIMENTO_CONVERSAO_DESCRICAO           = objUnidadeFornecimentoConversao.Descricao;
            
            lObjRetorno.TB_UNIDADE_FORNECIMENTO_SIAFISICO_CODIGO              = objUnidadeFornecimentoConversao.SistemaSiafisicoCodigo; //TB_UNIDADE_FORNECIMENTO_SIAF
            lObjRetorno.TB_UNIDADE_FORNECIMENTO_SISTEMA_SAM_ID                = objUnidadeFornecimentoConversao.SistemaSamId; //TB_UNIDADE_FORNECIMENTO

            lObjRetorno.TB_UNIDADE_FORNECIMENTO_CONVERSAO_FATOR_UNITARIO      = objUnidadeFornecimentoConversao.FatorUnitario;


            return lObjRetorno;
        }
        #endregion Mappers
    }
}
