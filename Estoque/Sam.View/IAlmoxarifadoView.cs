using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IAlmoxarifadoView : ICrudView
    {
        string OrgaoId { get; set; }
        string GestorId { get; set; }
        string EnderecoLogradouro { get; set; }
        string EnderecoNumero { get; set; }
        string EnderecoComplemento { get; set; }
        string EnderecoBairro { get; set; }
        string EnderecoMunicipio { get; set; }
        string UfId { get; set; }
        string EnderecoCep { get; set; }
        string EnderecoTelefone { get; set; }
        string EnderecoFax { get; set; }
        string Responsavel { get; set; }
        string UgeId { get; set; }
        string RefInicial { get; set; }
        string RefFaturamento { get; set; }
        string TipoAlmoxarifado { get; set; }
        string IndicadorAtividadeId { get; set; }
        
        bool BloqueiaEnderecoLogradouro { set; }
        bool BloqueiaEnderecoNumero { set; }
        bool BloqueiaEnderecoComplemento { set; }
        bool BloqueiaEnderecoBairro { set; }
        bool BloqueiaEnderecoMunicipio { set; }
        bool BloqueiaListaUf { set; }
        bool BloqueiaEnderecoCep { set; }
        bool BloqueiaEnderecoTelefone { set; }
        bool BloqueiaEnderecoFax { set; }
        bool BloqueiaResponsavel { set; }
        bool BloqueiaListaUge { set; }
        bool BloqueiaRefInicial {set;}
        bool BloqueiaListaIndicadorAtividade { set; }
        bool PermiteIgnorarCalendarioSiafemParaReabertura { get; set; }

        void PopularListaOrgao();
        void PopularListaGestor(int _orgaoId);
        void PopularListaUge();
        void PopularListaUf();
        void PopularListaIndicadorAtividade();
        void PopularListaAlmoxarifados();

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
