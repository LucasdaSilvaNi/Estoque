using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface INotificacaoView : ICrudView
    {
        int ID { get; set; }
        short? PERFIL_ID { get; set; }
        string TITULO{ get; set; }
        string MENSAGEM { get; set; }
        DateTime DATA { get; set; }
        bool? ATIVO { get; set; }
    }
}
