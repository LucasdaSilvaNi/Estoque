using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Domain.Entity;



namespace Sam.View
{
    public interface IChamadoSuporteView : ICrudView
    {
        DateTime DataAberturaChamado { get; set; }
        DateTime? DataFechamentoChamado { get; set; }
        int OrgaoID { get; set; }
        int GestorID { get; set; }
        int UgeID { get; set; }
        int AlmoxarifadoID { get; set; }
        int DivisaoId { get; set; }
        AlmoxarifadoEntity Almoxarifado { get; }
        DivisaoEntity Divisao { get; }
        UAEntity Ua { get; }
        int NumeroChamado { get; set; }
        long CpfUsuario { get; }
        long CpfUsuarioLogado { get; }
        string NomeUsuario { get; }
        string EMailUsuario { get; set; }
        byte SistemaModulo { get; set; }
        byte StatusChamadoAtendimentoProdesp { get; set; }
        byte StatusChamadoAtendimentoUsuario { get; set; }
        string Responsavel { get; set; }
        byte FuncionalidadeSistemaID { get; set; }
        byte TipoChamado { get; set; }
        string Observacoes { get; set; }
        IList<AnexoChamadoSuporte> Anexos { get; set; }
        IList<string> ListaArquivosAnexados { get; }
        string LogHistoricoAtendimento { get; set; }
        byte AmbienteSistema { get; set; }
        string DescricaoPerfilUsuario { get; set; }
        byte PerfilUsuarioAberturaChamadoID { get; set; }

        object FileUploader { get; }
        object RelacaoArquivosAnexados { get; }
        object WebResponse { get; }
        object WebContext { get; }
        string AnexoSelecionado { get; }
        string ArquivoSelecionadoParaUpload { get; }


        void LimparCamposView();

        bool IsUploadingFile { get; set; }
        bool BloqueiaBotaoAdicionarAnexo { set; }
        bool BloqueiaBotaoDownloadAnexos { set; }
        bool BloqueiaBotaoRemoverAnexo { set; }
        bool BloqueiaEMail { set; }

        bool MostrarPainelStatusAtualizar { set; }

        bool MostrarBotaoStatusAtualizar { set; }

        void ExibirRelatorio();
        SortedList ParametrosRelatorio { get; }
        RelatorioEntity DadosRelatorio { get; set; }
    }
}
