using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Objects;
using Doddle.Linq.Audit;
using System.Web;

namespace Sam.Infrastructure
{
     public class SAMwebEntities : ObjectStateManager, IAuditableContext
     {
         public SAMwebEntities(MetadataWorkspace workspace)
             : base(workspace)
         {}

         public void Audit()
         {
             ObjectContext t;

             var added = this.GetObjectStateEntries(System.Data.EntityState.Added);
             foreach (ObjectStateEntry entry in added)
             {
                 AuditRecords record = new AuditRecords();

                 foreach (string propName in entry.GetModifiedProperties())
                 {
                     object propValue = entry.CurrentValues[propName];

                     //AuditValue values = new AuditValue();
                     //values.MemberName = propName;
                     //values.OldValue = entry.OriginalValues[propName].ToString();
                     //values.NewValue = entry.CurrentValues[propName].ToString();
                 }
                 //entry.CurrentValues
             }
         }


         protected override void InsertAuditRecordsToDatabase(EntityAuditRecord record)
         {
             AuditRecords audit = new AuditRecords();
             audit.Action = (byte)record.Action;
             audit.AuditDate = DateTime.Now;
             audit.AssociationTable = record.AssociationTable;
             audit.AssociationTableKey = record.AssociationTableKey;
             audit.EntityTable = record.EntityTable;
             audit.EntityTableKey = record.EntityTableKey;

             audit.UserName = HttpContext.Current.User.Identity.Name;

             foreach (ModifiedEntityProperty av in record.ModifiedProperties)
             {
                 AuditRecordFields field = new AuditRecordFields();
                 field.MemberName = av.MemberName;
                 field.OldValue = av.OldValue;
                 field.NewValue = av.NewValue;

                 audit.AuditRecordFields.Add(field);
             }

             
             //this.AuditRecords.InsertOnSubmit(audit);
         }

         /// <summary>
         /// Define your audit definitions here. 
         /// This is not mandatory:
         /// you can define audit definitions anywhere in code as long as it is before SubmitChanges()
         /// </summary>
         protected override void DefaultAuditDefinitions()
         {
             this.TB_SUBITEM_MATERIAL;
             //this.Categories.Audit().AuditAssociation(c => c.Products);
             //this.Orders.Audit().AuditAssociation(o => o.Order_Details);
         }
        
     }
}
