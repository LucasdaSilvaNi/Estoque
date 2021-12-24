using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IGestorView : ICrudView
    {
        string OrgaoId { set; get; }
        string UoId { set; get; }
        string UgeId { set; get; }
        string TipoId { set; get; }
        string Nome { get; set; }
        string NomeReduzido { get; set; }
        string EnderecoLogradouro { get; set; }
        string EnderecoNumero { get; set; }
        string EnderecoComplemento { get; set; }
        string EnderecoTelefone { get; set; }
        string CodigoGestao { get; set; }
        byte[] Logotipo { get; }
        string LogotipoImgUrl {get;set;}

        bool BloqueiaNomeReduzido { set; }
        bool BloqueiaEnderecoLogradouro { set; }
        bool BloqueiaEnderecoNumero { set; }
        bool BloqueiaEnderecoComplemento { set; }
        bool BloqueiaEnderecoTelefone { set; }
        bool BloqueiaCodigoGestao { set; }
        bool BloqueiaListaUo { set; }
        bool BloqueiaListaUge { set; }
        bool BloqueiaFileUploadGestor { set; }
        bool BloqueiaTipoGestor { set; }

        void PopularListaOrgao();
        void PopularListaUo();
        void PopularListaUge();

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
