using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.Common.Util;
using TipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;



namespace Sam.Domain.Entity
{
    [Serializable]
    public class MovimentoEntity : BaseEntity
    {
        public MovimentoEntity() { }

        public MovimentoEntity(int _Id)
        { this.Id = _Id;
          this.IdTemp = _Id;
        }

        public MovimentoEntity(int _Id, AlmoxarifadoEntity almoxarifado)
        {
            this.Id = _Id;
            this.Almoxarifado = almoxarifado;
        }

        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public TipoMovimentoEntity TipoMovimento { get; set; }
        public string GeradorDescricao { get; set; }
        public string NumeroDocumento { get; set; }
        public string AnoMesReferencia { get; set; }
        public string DataDigitada { get; set; }
        public DateTime? DataDocumento { get; set; }
        public DateTime? DataMovimento { get; set; }
        public DateTime? DataOperacao { get; set; }
        public string FonteRecurso { get; set; }
        public decimal? ValorDocumento { get; set; }
        public string Observacoes { get; set; }
        public string Instrucoes { get; set; }
        public string Empenho { get; set; }
        public AlmoxarifadoEntity MovimAlmoxOrigemDestino { get; set; }
        public bool? Ativo { get; set; }
        public IList<MovimentoItemEntity> MovimentoItem { get; set; }
        public MovimentoItemEntity MovimentoItem2 { get; set; }
        public FornecedorEntity Fornecedor { get; set; }
        public DivisaoEntity Divisao { get; set; }
        public UGEEntity UGE { get; set; }
        public UAEntity UA { get; set; }
        public EmpenhoEventoEntity EmpenhoEvento { get; set; }
        public EmpenhoLicitacaoEntity EmpenhoLicitacao { get; set; }
        public string NaturezaDespesaEmpenho { get; set; }
        public int? IdLogin { get; set; }
        public int? IdLoginEstorno { get; set; }
        public int? IdTransferencia { get; set; }
        public decimal? ValorSomaItens { get; set; }
        public decimal? ValorOriginalDocumento { get; set; }
        public int QtdeItens
        {
            get { return MovimentoItem != null ? MovimentoItem.Count() : 0; }
        }

        public string AlmoxConsumo { get; set; }
        public decimal? TotalQtdeSaidasSubitem { get; set; }
        public decimal? SaldoAtualAlmox { get; set; }
        public decimal? TotalQtdeSaidasGestor { get; set; }
        public decimal? SaldoAtualGestor { get; set; }
        public string NomeRequisitante { get; set; }
        public bool? Bloquear { get; set; }
        public int? SubTipoMovimentoId { get; set; }
        public string UgeCPFCnpjDestino { get; set; }

        [Description("Campo utilizado para definição de Evento/InscricaoClassificacao a ser utilizada no lancamento de DocumentoSAM no SIAFEM")]
        public string InscricaoCE;
        
        [Description("Campo utilizado para definição de tipo de material (consumo/permanente) a ser utilizado no lancamento de DocumentoSAM no SIAFEM")]
        public TipoMaterial @TipoMaterial { get; set; }
    }
}



