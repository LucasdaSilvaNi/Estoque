using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IConsultarSaidaView : ICrudView
    {
        int ID { get; set; }
        int ALMOXARIFADO_ID { get; }
        int TIPO_MOVIMENTO_ID { get; }
        string MOVIMENTO_NUMERO_DOCUMENTO { get; }        
        int MOVIMENTO_ANO_MES_REFERENCIA { get; }
        DateTime? MOVIMENTO_DATA_DOCUMENTO { get; }
        DateTime? MOVIMENTO_DATA_MOVIMENTO { get; }
        DateTime? MOVIMENTO_DATA_DOCUMENTO_ATE { get; }
        DateTime? MOVIMENTO_DATA_MOVIMENTO_ATE { get; }
        string EMPENHO_COD { get; }
        int? UGE_ID { get; }

        //Botões
        bool BloqueiaEditar  { set; }
        bool BloqueiaEstornar { set; }
        bool BloqueiaNotaFornecimento { set; }

        //Outros
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
        bool Estornado {get;}

    }
}
