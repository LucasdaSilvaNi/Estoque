using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface ITerceiroView : ICrudView
    {
        string OrgaoId { get; set; }
        string GestorId { get; set; }
        string EnderecoLogradouro { get; set; }
        string EnderecoNumero { get; set; }
        string EnderecoComplemento { get; set; }
        string EnderecoBairro { get; set; }
        string EnderecoCidade { get; set; }
        string UfId { get; set; }
        string EnderecoCep { get; set; }
        string EnderecoTelefone { get; set; }
        string EnderecoFax { get; set; }
       
        bool BloqueiaEnderecoLogradouro { set; }
        bool BloqueiaEnderecoNumero { set; }
        bool BloqueiaEnderecoComplemento { set; }
        bool BloqueiaEnderecoBairro { set; }
        bool BloqueiaEnderecoCidade { set; }
        bool BloqueiaEnderecoCep { set; }
        bool BloqueiaEnderecoTelefone { set; }
        bool BloqueiaEnderecoFax { set; }
        bool BloqueiaListaUF { set; }

        void PopularListaOrgao();
        void PopularListaGestor(int? OrgaoId);
        void PopularListaUF();
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
