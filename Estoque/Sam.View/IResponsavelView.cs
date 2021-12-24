using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IResponsavelView : ICrudView
    {
        string OrgaoId { get; }
        string GestorId { get; }
        string Cargo { get; set; }
        string Endereco { get; set; }
        bool BloqueiaCargo { set; }
        bool BloqueiaEndereco { set; }
        void PopularListaOrgao();
        void PopularListaGestor(int OrgaoId);
        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
