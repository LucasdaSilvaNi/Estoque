using System;
using System.Collections.Generic;
using Sam.Common.Util;
using statusAtendimentoChamado = Sam.Common.Enums.ChamadoSuporteEnums.StatusAtendimentoChamado;




namespace Sam.Domain.Entity
{
    [Serializable]
    public class ChamadoSuporteEntity : BaseEntity
    {
        public AlmoxarifadoEntity Almoxarifado;
        public DivisaoEntity Divisao;
        public DateTime DataAbertura;
        public DateTime? DataFechamento;
        public long CpfUsuario; //CPF
        public long CpfUsuarioLogado; //CPF
        public string NomeUsuario;
        public string EMailUsuario;
        public long? UsuarioCancelador; //CPF
        public byte SistemaModulo;
        public byte PerfilUsuarioAberturaChamado;
        public byte StatusChamadoAtendimentoProdesp;
        public byte StatusChamadoAtendimentoUsuario;
        public string Responsavel; // (Analista SAM)
        public FuncionalidadeSistemaEntity Funcionalidade;
        public byte TipoChamado;
        public string Observacoes;
        public IList<AnexoChamadoSuporte> Anexos; //(BLOB)
        public string HistoricoAtendimento; //(XML)
        public string LogHistoricoAtendimento; //(XML)
        public byte AmbienteSistema;
        public bool? Ativo;
        public DateTime DataHoraUltimaEdicao;

        public bool AguardandoUsuario
        {
            get { return (this.IsNotNull() && this.StatusChamadoAtendimentoProdesp == (byte)statusAtendimentoChamado.AguardandoRetornoUsuario); }
        }

        public bool ChamadoFinalizado
        {
            get { return (this.IsNotNull() && (this.StatusChamadoAtendimentoProdesp == (byte)statusAtendimentoChamado.Finalizado)); }
        }

    }

    [Serializable]
    public sealed class AnexoChamadoSuporte
    {
        public string NomeArquivo;

        private byte[] conteudoArquivo;
        public byte[] ConteudoArquivo
        {
            get { return conteudoArquivo; }
            set { conteudoArquivo = value; }
        }

        public decimal TamanhoMB
        {
            get { return (conteudoArquivo.Length / 1024.00m / 1024.00m); }
        }
        public decimal TamanhoKB
        {
            get { return (conteudoArquivo.Length / 1024.00m); }
        }
        public decimal TamanhoBytes
        {
            get { return conteudoArquivo.Length; }
        }

        public string ContentType;
    }
}

