using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Sam.Domain.Entity.ValidationAttributes
{
    [Serializable]
    public class InstanceClassAttribute : ValidationAttribute
    {
        public InstanceClassAttribute()
        {

        }

        public override bool IsValid(object value)
        {
            return this.IsValid(value as BaseEntity);
        }

        public bool IsValid(BaseEntity _entity)
        {
            bool retorno = false;

            if (_entity != null)
                retorno = _entity.Id.HasValue;

            return retorno;
        }
    }
}
