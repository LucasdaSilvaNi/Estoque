using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IDivisaoView : ICrudView
    {
        string OrgaoId { get; set; }
        string UAId { get; set; }
        string ResponsavelId { get; set; }
        string AlmoxarifadoId { get; set; }
        string EnderecoLogradouro { get; set; }
        string EnderecoNumero { get; set; }
        string EnderecoComplemento { get; set; }
        string EnderecoBairro { get; set; }
        string EnderecoMunicipio { get; set; }
        string UfId { get; set; }
        string EnderecoCep { get; set; }
        string EnderecoTelefone { get; set; }
        string EnderecoFax { get; set; }
        string Area { get; set; }
        string NumeroFuncionarios { get; set; }
        string IndicadorAtividadeId { get; set; }
        string UOId { get; set; }
        string UGEId { get; set; }

        bool BloqueiaEnderecoLogradouro { set; }
        bool BloqueiaEnderecoNumero { set; }
        bool BloqueiaEnderecoComplemento { set; }
        bool BloqueiaEnderecoBairro { set; }
        bool BloqueiaEnderecoMunicipio { set; }
        bool BloqueiaEnderecoCep { set; }
        bool BloqueiaEnderecoTelefone { set; }
        bool BloqueiaEnderecoFax { set; }
        bool BloqueiaListaAlmoxarifado { set; }
        bool BloqueiaListaResponsavel { set; }
        bool BloqueiaListaUA { set; }
        bool BloqueiaListaUF { set; }
        bool BloqueiaNumeroFuncionarios { set; }
        bool BloqueiaArea { set; }
        bool BloqueiaListaIndicadorAtividade { set; }

        void PopularListaOrgao();
        void PopularListaAlmoxarifado();
        void PopularListaResponsavel();
        void PopularListaUA(int OrgaoId);
        void PopularListaUF();
        void PopularListaIndicadorAtividade();
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
