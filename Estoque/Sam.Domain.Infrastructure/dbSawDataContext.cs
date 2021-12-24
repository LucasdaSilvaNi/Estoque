using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Linq.Expressions;
using Doddle.Linq.Audit;
using System.Web;
namespace Sam.Domain.Infrastructure
{
    public partial class dbSawDataContext
     {

        /// <summary>
        /// Insere o histório de auditoria no sistema SAMWEB
        /// </summary>
        /// <param name="record"></param>
        protected override void InsertAuditRecordToDatabase(EntityAuditRecord record)
         {
             try
             {
                 TB_HISTORICO historico = new TB_HISTORICO();

                 historico.TB_HISTORICO_ACAO = (byte)record.Action;
                 historico.TB_HISTORICO_DATA = DateTime.Now;
                 historico.TB_HISTORICO_RELACIONAMENTO = record.AssociationTable;
                 historico.TB_HISTORICO_RELACIONAMENTO_ID = record.AssociationTableKey;
                 historico.TB_HISTORICO_TABELA = record.EntityTable.Replace(" ", "");
                 historico.TB_HISTORICO_TABELA_ID = record.EntityTableKey;
                 historico.TB_HISTORICO_NOMEUSUARIO = ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.Name;
                 historico.TB_HISTORICO_CPF = ((System.Web.Security.FormsIdentity)(HttpContext.Current.User.Identity)).Ticket.Name;

                 foreach (ModifiedEntityProperty av in record.ModifiedProperties)
                 {
                     TB_HISTORICO_CAMPO historicoCampo = new TB_HISTORICO_CAMPO();
                     historicoCampo.TB_HISTORICO_CAMPO_NOME = av.MemberName.Replace(" ", "");
                     historicoCampo.TB_HISTORICO_CAMPO_ANTIGO = av.OldValue;
                     historicoCampo.TB_HISTORICO_CAMPO_NOVO = av.NewValue;

                     historico.TB_HISTORICO_CAMPOs.Add(historicoCampo);
                 }
                 this.TB_HISTORICOs.InsertOnSubmit(historico);
             }
             catch (Exception e)
             {
                 new Sam.Domain.Infrastructure.LogErroInfraestructure().GravarLogErro(e);
             }
         }


        /// <summary>
        /// Define quais tabelas serão auditada
        /// </summary>
        protected override void DefaultAuditDefinitions()
         {
             this.TB_SUBITEM_MATERIALs.Audit();
             this.TB_ITEM_SUBITEM_MATERIALs.Audit();
             this.TB_ITEM_MATERIALs.Audit();
             this.TB_ITEM_NATUREZA_DESPESAs.Audit();
             this.TB_SUBITEM_MATERIAL_ALMOXes.Audit();
             this.TB_ORGAOs.Audit();
             this.TB_ALMOXARIFADOs.Audit();
             this.TB_DIVISAOs.Audit();
             this.TB_GESTORs.Audit();
             this.TB_NATUREZA_DESPESAs.Audit();
             this.TB_UAs.Audit();
             this.TB_UGEs.Audit();
             this.TB_UNIDADE_FORNECIMENTOs.Audit();
             this.TB_UOs.Audit();
             this.TB_FORNECEDORs.Audit();

             this.TB_CHAMADO_SUPORTEs.Audit();
             this.TB_CALENDARIO_FECHAMENTO_MENSALs.Audit();
             this.TB_EVENTOS_PAGAMENTOs.Audit();
             this.TB_EVENTO_SIAFEMs.Audit();
             //this.TB_MOVIMENTOs.Audit();
             //this.TB_MOVIMENTO_ITEMs.Audit();
             //this.TB_FECHAMENTOs.Audit();
         }
        
     }
}
