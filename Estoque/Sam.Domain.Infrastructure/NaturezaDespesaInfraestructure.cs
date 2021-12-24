using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;

namespace Sam.Domain.Infrastructure
{
    public class NaturezaDespesaInfraestructure : BaseInfraestructure, INaturezaDespesaService
    {
        public int totalregistros
        {
            get;
            set;
        }

        public int TotalRegistros()
        {
            return totalregistros;
        }


        public NaturezaDespesaEntity Entity { get; set; }

        public IList<NaturezaDespesaEntity> Listar()
        {
            IList<NaturezaDespesaEntity> resultado = (from a in this.Db.TB_NATUREZA_DESPESAs
                                                      orderby a.TB_NATUREZA_DESPESA_CODIGO
                                                      select new NaturezaDespesaEntity
                                                      {
                                                          Id = a.TB_NATUREZA_DESPESA_ID,
                                                          Descricao = a.TB_NATUREZA_DESPESA_DESCRICAO,
                                                          Codigo = a.TB_NATUREZA_DESPESA_CODIGO,
                                                          Natureza = a.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                                                      }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<NaturezaDespesaEntity>();

            this.totalregistros = (from a in this.Db.TB_NATUREZA_DESPESAs
                                   select new
                                   {
                                       Id = a.TB_NATUREZA_DESPESA_ID,
                                   }).Count();
            return resultado;

        }

        public void Excluir()
        {
            TB_NATUREZA_DESPESA natureza
                   = this.Db.TB_NATUREZA_DESPESAs.Where(a => a.TB_NATUREZA_DESPESA_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_NATUREZA_DESPESAs.DeleteOnSubmit(natureza);
            this.Db.SubmitChanges();
        }

        public bool ExcluirNaturezaDespesa()
        {
            bool blnStatusExclusao = false;

            try
            {
                TB_NATUREZA_DESPESA natureza = this.Db.TB_NATUREZA_DESPESAs.Where(a => a.TB_NATUREZA_DESPESA_ID == this.Entity.Id).FirstOrDefault();
                IList<TB_ITEM_NATUREZA_DESPESA> lstItemMaterialRelacionado = this.Db.TB_ITEM_NATUREZA_DESPESAs.Where(_itemMaterial => _itemMaterial.TB_NATUREZA_DESPESA_ID == this.Entity.Id).ToList();

                if (natureza.IsNotNull() && lstItemMaterialRelacionado.IsNullOrEmpty())
                {
                    this.Db.TB_NATUREZA_DESPESAs.DeleteOnSubmit(natureza);
                    this.Db.SubmitChanges();

                    blnStatusExclusao = true;
                }
            }
            catch (Exception excErro)
            {
                //Sam.Domain.Business.LogErro.GravarStackTrace();

                throw;
            }


            return blnStatusExclusao;
        }

        public void Salvar()
        {
            TB_NATUREZA_DESPESA natureza = new TB_NATUREZA_DESPESA();

            if (this.Entity.Id.HasValue)
                natureza = this.Db.TB_NATUREZA_DESPESAs.Where(a => a.TB_NATUREZA_DESPESA_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_NATUREZA_DESPESAs.InsertOnSubmit(natureza);

            natureza.TB_NATUREZA_DESPESA_CODIGO = this.Entity.Codigo;
            natureza.TB_NATUREZA_DESPESA_DESCRICAO = this.Entity.Descricao;
            natureza.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE = this.Entity.Natureza ?? false;
            this.Db.SubmitChanges();
            this.Entity.Id = natureza.TB_NATUREZA_DESPESA_ID; // retornando ID em caso de cadastro de novo item do SIAFISICO
        }

        public IList<NaturezaDespesaEntity> Imprimir()
        {
            IList<NaturezaDespesaEntity> resultado = (from a in this.Db.TB_NATUREZA_DESPESAs
                                                      orderby a.TB_NATUREZA_DESPESA_CODIGO
                                                      select new NaturezaDespesaEntity
                                                      {
                                                          Id = a.TB_NATUREZA_DESPESA_ID,
                                                          Descricao = a.TB_NATUREZA_DESPESA_DESCRICAO,
                                                          Codigo = a.TB_NATUREZA_DESPESA_CODIGO,
                                                          Natureza = a.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                                                      }).ToList<NaturezaDespesaEntity>();

            return resultado;
        }

        public bool PodeExcluir()
        {

            bool retorno = true;

            //TB_MATERIAL uo = this.Db.TB_NATUREZA_DESPESAs.Where(a => a. == this.UO.Id.Value).FirstOrDefault();
            // XUXA - implementar validação para verificar possibilidade de exclusão da grupo material
            // provavelmente ligacoes com as tabelas: classe, material, item material

            //if (UO.Count > 0)
            //    retorno = false;

            return retorno;
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            if (this.Entity.Id.HasValue)
            {
                retorno = this.Db.TB_NATUREZA_DESPESAs
                .Where(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_NATUREZA_DESPESA_ID != this.Entity.Id.Value)
                .Count() > 0;
            }
            else
            {
                retorno = this.Db.TB_NATUREZA_DESPESAs
                .Where(a => a.TB_NATUREZA_DESPESA_CODIGO == this.Entity.Codigo)
                .Count() > 0;
            }
            return retorno;
        }




        public NaturezaDespesaEntity LerRegistro()
        {
            throw new NotImplementedException();
        }


        public IList<NaturezaDespesaEntity> ListarTodosCod()
        {
            IList<NaturezaDespesaEntity> resultado = (from a in this.Db.TB_NATUREZA_DESPESAs
                                                      orderby a.TB_NATUREZA_DESPESA_DESCRICAO
                                                      select new NaturezaDespesaEntity
                                                      {
                                                          Id = a.TB_NATUREZA_DESPESA_ID,
                                                          Codigo = a.TB_NATUREZA_DESPESA_CODIGO,
                                                          Descricao = string.Format("{0} - {1}", a.TB_NATUREZA_DESPESA_CODIGO.ToString().PadLeft(8, '0'), a.TB_NATUREZA_DESPESA_DESCRICAO),
                                                          Natureza = a.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE,
                                                      })                                         
                                         .ToList<NaturezaDespesaEntity>();            
            return resultado;

        }

        public List<String> ListarNaturezaConsumoImediato()
        {
            List<NaturezaDespesaEntity> resultado = (from a in this.Db.TB_NATUREZA_DESPESAs
                             join b in this.Db.TB_ITEM_NATUREZA_DESPESAs on a.TB_NATUREZA_DESPESA_ID equals b.TB_NATUREZA_DESPESA_ID
                             join c in this.Db.TB_ITEM_MATERIALs on b.TB_ITEM_MATERIAL_ID equals c.TB_ITEM_MATERIAL_ID
                             join d in this.Db.TB_MATERIALs on c.TB_MATERIAL_ID equals d.TB_MATERIAL_ID
                             join e in this.Db.TB_CLASSE_MATERIALs on d.TB_CLASSE_MATERIAL_ID equals e.TB_CLASSE_MATERIAL_ID
                             join f in this.Db.TB_GRUPO_MATERIALs on e.TB_GRUPO_MATERIAL_ID equals f.TB_GRUPO_MATERIAL_ID
                             where (f.TB_GRUPO_MATERIAL_CODIGO == 89 || f.TB_GRUPO_MATERIAL_CODIGO == 91)
                             //where f.TB_GRUPO_MATERIAL_CODIGO == 65
                             select new NaturezaDespesaEntity
                             {
                                 
                                 // Id = a.TB_NATUREZA_DESPESA_ID,
                                 Codigo = a.TB_NATUREZA_DESPESA_CODIGO,
                                 //Descricao = string.Format("{0} - {1}", a.TB_NATUREZA_DESPESA_CODIGO.ToString().PadLeft(8, '0'), a.TB_NATUREZA_DESPESA_DESCRICAO),
                                 //Natureza = a.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE,
                                 //IdTemp = f.TB_GRUPO_MATERIAL_CODIGO,
                             })
                                         .ToList<NaturezaDespesaEntity>();
           
            var list = new List<String>();
            list = resultado.Select(x => x.Codigo.ToString()).ToList();
            var distinct = list.Distinct().ToList();

            

            return distinct;

        }

        public IList<int> ListarNaturezasDespesaConsumoImediato()
        {
            IQueryable<int> qryConsulta = null;
            IList<int> lstRetorno = null;


            int[] codigosGrupoMaterialSIAFISICO = new int[] { 89, 91 };
            qryConsulta = (from naturezaDespesa in this.Db.TB_NATUREZA_DESPESAs
                           join relacaoItemMaterial_NaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs on naturezaDespesa.TB_NATUREZA_DESPESA_ID equals relacaoItemMaterial_NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                           join itemMaterialSIAFISICO in this.Db.TB_ITEM_MATERIALs on relacaoItemMaterial_NaturezaDespesa.TB_ITEM_MATERIAL_ID equals itemMaterialSIAFISICO.TB_ITEM_MATERIAL_ID
                           join materialSIAFISICO in this.Db.TB_MATERIALs on itemMaterialSIAFISICO.TB_MATERIAL_ID equals materialSIAFISICO.TB_MATERIAL_ID
                           join classeMaterialSIAFISICO in this.Db.TB_CLASSE_MATERIALs on materialSIAFISICO.TB_CLASSE_MATERIAL_ID equals classeMaterialSIAFISICO.TB_CLASSE_MATERIAL_ID
                           join grupoMaterialSIAFISICO in this.Db.TB_GRUPO_MATERIALs on classeMaterialSIAFISICO.TB_GRUPO_MATERIAL_ID equals grupoMaterialSIAFISICO.TB_GRUPO_MATERIAL_ID
                           where codigosGrupoMaterialSIAFISICO.Contains(grupoMaterialSIAFISICO.TB_GRUPO_MATERIAL_CODIGO)
                           select naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO).AsQueryable();


            lstRetorno = qryConsulta.Distinct().ToList();

            return lstRetorno;
        }

        public NaturezaDespesaEntity ObterNaturezaDespesa(string strCodigoNaturezaDespesa)
        {
            int codigoNaturezaDespesa = Int32.Parse(strCodigoNaturezaDespesa);

           NaturezaDespesaEntity lObjRetorno = (from NaturezaDespesa in this.Db.TB_NATUREZA_DESPESAs
                                                where NaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == codigoNaturezaDespesa
                                                select new NaturezaDespesaEntity
                                                {
                                                    Id        = NaturezaDespesa.TB_NATUREZA_DESPESA_ID,
                                                    Descricao = NaturezaDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                                                    Codigo    = NaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO,
                                                    CodigoDescricao = String.Format("{0} - {1}", NaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO, NaturezaDespesa.TB_NATUREZA_DESPESA_DESCRICAO),
                                                    Natureza  = NaturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                                                }).FirstOrDefault();

           return lObjRetorno;
        }

        public NaturezaDespesaEntity ObterNaturezaDespesa(int naturezaDespesaID)
        {
            NaturezaDespesaEntity lObjRetorno = (from NaturezaDespesa in this.Db.TB_NATUREZA_DESPESAs
                                                 where NaturezaDespesa.TB_NATUREZA_DESPESA_ID == naturezaDespesaID
                                                 select new NaturezaDespesaEntity
                                                 {
                                                     Id = NaturezaDespesa.TB_NATUREZA_DESPESA_ID,
                                                     Descricao = NaturezaDespesa.TB_NATUREZA_DESPESA_DESCRICAO,
                                                     Codigo = NaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO,
                                                     CodigoDescricao = String.Format("{0} - {1}", NaturezaDespesa.TB_NATUREZA_DESPESA_CODIGO, NaturezaDespesa.TB_NATUREZA_DESPESA_DESCRICAO),
                                                     Natureza = NaturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE
                                                 }).FirstOrDefault();

            return lObjRetorno;
        }
    }
}
