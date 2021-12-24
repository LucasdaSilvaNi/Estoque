using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class RelacaoMaterialItemSubItemEntity : BaseEntity
    {
        public ItemMaterialEntity Item { get; set; }
        public ItemMaterialEntity ItemEdit { get; set; }
        public SubItemMaterialEntity SubItem { get; set; }
        public GestorEntity Gestor { get; set; }
                
        public string DescricaoItem { get { return string.Format("{0} - {1}", Item.Codigo.ToString().PadLeft(9,'0'),Item.Descricao); } }
        public string DescricaoSubItem { get { return string.Format("{0} - {1}", SubItem.Codigo.ToString().PadLeft(12, '0'), SubItem.Descricao); } }
        public string NomeGestor { get { return Gestor.Nome; } }
    }
}
