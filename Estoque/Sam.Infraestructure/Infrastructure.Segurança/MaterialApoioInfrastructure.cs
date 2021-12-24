using System;
using System.Collections.Generic;
using Sam.Domain.Entity;
using Sam.Entity;
using Sam.Common.Util;
using Enums = Sam.Common.Util.GeralEnum;
using System.Linq.Expressions;
using System.Linq;


namespace Sam.Infrastructure
{
    public class MaterialApoioInfrastructure : AbstractCrud<TB_MATERIAL_APOIO, SEGwebEntities>
    {
        public IList<MaterialApoioEntity> ListarRecursosPorPerfil(int? Perfil_ID)
        {
            List<MaterialApoioEntity> lstRetorno   = new List<MaterialApoioEntity>();
            List<TB_MATERIAL_APOIO>   lstRetornoBD = null;

            Expression<Func<TB_MATERIAL_APOIO, bool>> expWhere;

            expWhere = this.MontarWherePorPerfil(Perfil_ID.Value);

            //lstRetornoBD = this.SelectWhere(MaterialApoio => (MaterialApoio.TB_PERFIL_ID == Perfil_ID || MaterialApoio.TB_PERFIL_ID == null));
            lstRetornoBD = this.SelectWhere(expWhere);
            lstRetornoBD.ForEach(rowMaterialApoio => lstRetorno.Add(this.MapearDTO(rowMaterialApoio)));
            lstRetornoBD.OrderBy(MaterialApoio => new { MaterialApoio.TB_PERFIL_ID, MaterialApoio.TB_MATERIAL_APOIO_CODIGO });

            return lstRetorno;
        }
        public MaterialApoioEntity ObterDadosMaterialApoio(int MaterialApoio_ID)
        {
            MaterialApoioEntity         objRetorno       = null;
            TB_MATERIAL_APOIO           rowMaterialApoio = null;
            MaterialApoioInfrastructure infraEstrutura   = new MaterialApoioInfrastructure();

            
            rowMaterialApoio = this.SelectOne(MaterialApoio => MaterialApoio.TB_MATERIAL_APOIO_ID == MaterialApoio_ID);

            if (rowMaterialApoio != null)
                objRetorno = this.MapearDTO(rowMaterialApoio);


                return objRetorno;
        }
        public bool Salvar(MaterialApoioEntity objMaterialApoio)
        {
            TB_MATERIAL_APOIO rowMaterialApoio = this.MapearEntity(objMaterialApoio);
            this.Insert(rowMaterialApoio);

            return (this.SelectOne(MaterialApoio => MaterialApoio.TB_MATERIAL_APOIO_ID == rowMaterialApoio.TB_MATERIAL_APOIO_ID) != null); 
        }


        public Expression<Func<TB_MATERIAL_APOIO,bool>> MontarWherePorPerfil(int? Perfil_ID)
        {
            //Func<TB_MATERIAL_APOIO, bool> lFuncRetorno = null;
            Expression<Func<TB_MATERIAL_APOIO, bool>> lExpRetorno = null;

            switch (Perfil_ID)    
            {
                case (int)Enums.TipoPerfil.Requisitante:         lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.Requisitante ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == null); 
                                                                 break;

                case (int)Enums.TipoPerfil.OperadorAlmoxarifado: lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.Requisitante ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.OperadorAlmoxarifado ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == null); 
                                                                 break;



                case (int)Enums.TipoPerfil.AdministradorGestor:  lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.Requisitante ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.OperadorAlmoxarifado ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorGestor ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == null);
                                                                 break;

                case (int)Enums.TipoPerfil.AdministradorOrgao:   lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.Requisitante ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.OperadorAlmoxarifado ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorGestor ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorOrgao ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == null);
                                                                 break;

                case (int)Enums.TipoPerfil.AdministradorGeral:   lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.Requisitante ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.OperadorAlmoxarifado ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorGestor ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorGeral ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == (int)Enums.TipoPerfil.AdministradorOrgao ||
                                                                                                 MaterialApoio.TB_PERFIL_ID == null);
                                                                 break;

                default:                                         lExpRetorno = (MaterialApoio => MaterialApoio.TB_PERFIL_ID == null); break;
            }

            return lExpRetorno;
        }

        #region Mappers
        internal MaterialApoioEntity MapearDTO(TB_MATERIAL_APOIO rowMaterialApoio)
        {
            MaterialApoioEntity lObjRetorno = new MaterialApoioEntity();

            lObjRetorno.Id                 = rowMaterialApoio.TB_MATERIAL_APOIO_ID;
            lObjRetorno.Codigo             = rowMaterialApoio.TB_MATERIAL_APOIO_CODIGO;
            lObjRetorno.NomeArquivo        = rowMaterialApoio.TB_MATERIAL_APOIO_NOME_ARQUIVO;
            lObjRetorno.PathArquivo        = rowMaterialApoio.TB_MATERIAL_APOIO_PATH_ARQUIVO;
            lObjRetorno.Descricao          = rowMaterialApoio.TB_MATERIAL_APOIO_DESCRICAO;
            lObjRetorno.DescricaoDetalhada = rowMaterialApoio.TB_MATERIAL_APOIO_DESCRICAO_DETALHADA;
            lObjRetorno.TipoRecurso        = rowMaterialApoio.TB_MATERIAL_APOIO_TIPO_RECURSO;

            if (!rowMaterialApoio.TB_PERFIL_ID.IsNull())
                lObjRetorno.Perfil = new Perfil() { IdPerfil = Int16.Parse(rowMaterialApoio.TB_PERFIL_ID.Value.ToString()) };

            return lObjRetorno;
        }
        internal TB_MATERIAL_APOIO MapearEntity(MaterialApoioEntity objMaterialApoio)
        {
            TB_MATERIAL_APOIO lObjRetorno = new TB_MATERIAL_APOIO();

            lObjRetorno.TB_MATERIAL_APOIO_ID                  = objMaterialApoio.Id.Value;
            lObjRetorno.TB_MATERIAL_APOIO_CODIGO              = objMaterialApoio.Codigo;
            lObjRetorno.TB_MATERIAL_APOIO_NOME_ARQUIVO        = objMaterialApoio.NomeArquivo;
            lObjRetorno.TB_MATERIAL_APOIO_PATH_ARQUIVO        = objMaterialApoio.PathArquivo;
            lObjRetorno.TB_MATERIAL_APOIO_DESCRICAO           = objMaterialApoio.Descricao;
            lObjRetorno.TB_MATERIAL_APOIO_DESCRICAO_DETALHADA = objMaterialApoio.DescricaoDetalhada;
            lObjRetorno.TB_PERFIL_ID                          = objMaterialApoio.Perfil.IdPerfil;
            lObjRetorno.TB_MATERIAL_APOIO_TIPO_RECURSO        = objMaterialApoio.TipoRecurso;

            return lObjRetorno;
        }
        #endregion Mappers
    }
}
