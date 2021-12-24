using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IMovimentoView : ICrudView
    {
        int OrgaoId { get; }
        int UgeId { get; set; }
        int UnidadeId { get; set; }
        int? FornecedorId { get; set; }
        int TipoMovimento { get; set; }
        string NumeroDocumento { get; set; }
        string AnoMesReferencia { get; set; }
        DateTime? DataDocumento { get; set; }
        DateTime? DataMovimento { get; set; }
        string FonteRecurso { get; set; }
        decimal? ValorDocumento { get; set; }
        string Observacoes { get; set; }
        string Instrucoes { get; set; }
        string Empenho { get; set; }
        int? DivisaoId { get; set; }
        int? AlmoxarifadoIdOrigem { get; set; }
        string GeradorDescricao { get; set; }
        bool ExibirListaEmpenho { get; set; }
        bool ExibirNumeroEmpenho { get; set; }
        bool BloqueiaEmpenho { set; }
        int TipoOperacao { get; set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}

