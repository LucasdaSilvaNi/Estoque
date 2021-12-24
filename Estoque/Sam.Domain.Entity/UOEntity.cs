using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    [Serializable]
    public class UOEntity : BaseEntity
    {
        public UOEntity()
        {
        }

        public UOEntity(int? id)
        {
            this.Id = id;
        }

        public int Codigo { get; set; }

        public OrgaoEntity Orgao { get; set; }
        public bool? Ativo { get; set; }

       public bool UoDefault { get; set; }

        public override string CodigoDescricao
        {
            get { return string.IsNullOrEmpty(_codigoDescricao) ? base.concatenarCodigoDescricao(Codigo, Descricao, 5) : _codigoDescricao; }
            set { _codigoDescricao = value; }
        }
    }
}
