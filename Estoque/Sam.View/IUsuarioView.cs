using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IUsuarioView : ICrudView
    {
        string CPF { get; set; }
        bool? Ativo { get; set; }
        string NomeUsuario { get; set; }
        long? Telefone { get; set; }
        int? UsuarioIdResponsavel { get; set; }

        int? OrgaoId { get; set; }
        int? GestorId { get; set; }
        int? PerfilId { get; set; }
        int? OrgaoPdId { get; set; }
        int? GestorPdId { get; set; }

        string Email { get; set; }
        string Senha { get; set; }

        string MsgSenha { get; set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
