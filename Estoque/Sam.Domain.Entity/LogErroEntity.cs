using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class LogErroEntity : BaseEntity
    {
        public int QtdRegistros { get; set; }

        public int Id { get; set; }
        public string Message { get; set; }
        public string StrackTrace { get; set; }
        public DateTime? Data { get; set; }
    }
}
