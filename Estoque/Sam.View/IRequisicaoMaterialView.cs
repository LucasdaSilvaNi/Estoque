using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;

namespace Sam.View
{
    public interface IRequisicaoMaterialView : ICrudView
    {   
        int OrgaoId { get; set; }
        int UOId { get; set; }
        int UGEId { get; set; }
        int UAId { get; set; }
        int DivisaoId { get; set; }
        int? PTResId { get; set; }
        decimal Quantidade { get; set; }
        string Descricao { get; set; }
        string PTResDescricao { get; set; }
        string PTResAcao { get; set; }
        string PTAssociadoPTRes { get; set; }
        int? PTResCodigo { get; set; }
        string Observacao { get; set; }
        string Codigo { get; set; }
        int SubItemId { get; }
        string UnidadeFornecimentoDescricao { get; set; }
        int StatusId { get; set; }
        int AlmoxarifadoId { get; set; }
        string NumeroDocumento { get; set; }        
        bool ResetarStartRowIndex { get; set; } 

        bool BloqueiaSubItem { set; }
        bool BloqueiaDescricao { set; }
        bool BloqueiaQuantidade { set; }
        bool BloqueiaObservacoes { set; }
        bool BloqueiaInstrucoes { set; }
        
        bool BloqueiaListaOrgao { set; }
        bool BloqueiaListaUO { set; }
        bool BloqueiaListaUGE { set; }        
        bool BloqueiaListaUA { set; }
        bool BloqueiaListaDivisao { set; }

        bool VisivelEditar { set; }
        bool VisivelAdicionar { set; }
        bool MostrarPainelRequisicao { set; }
                
        bool OcultaOrgao { set; }
        bool OcultaUO { set; }
        bool OcultaUGE { set; }

        RelatorioEntity DadosRelatorio { get; set; }

        void ResetGridViewPageIndex();
        void InserirMensagemEmSessao(string chaveSessao, string msgSessao);
        void InserirMensagemCancelada(string chaveSessao, string msgSessao);
    }
}
