using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IFornecedorView : ICrudView
    {
        string EnderecoLogradouro { get; set; }
        string EnderecoNumero { get; set; }
        string EnderecoComplemento { get; set; }
        string EnderecoBairro { get; set; }
        string EnderecoCidade { get; set; }
        string UfId { get; set; }
        string EnderecoCep { get; set; }
        string EnderecoTelefone { get; set; }
        string EnderecoFax { get; set; }
        string Email { get; set; }
        string InformacoesComplementares { get; set; }
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }

        bool BloqueiaEnderecoLogradouro { set; }
        bool BloqueiaEnderecoNumero { set; }
        bool BloqueiaEnderecoComplemento { set; }
        bool BloqueiaEnderecoBairro { set; }
        bool BloqueiaEnderecoCidade { set; }
        bool BloqueiaEnderecoCep { set; }
        bool BloqueiaEnderecoTelefone { set; }
        bool BloqueiaEnderecoFax { set; }
        bool BloqueiaEmail { set; }
        bool BloqueiaListaUF { set; }
        bool BloqueiaInformacoesComplementares { set; }       

        void PopularListaUF();
        void ExibirRelatorio();
    }
}
