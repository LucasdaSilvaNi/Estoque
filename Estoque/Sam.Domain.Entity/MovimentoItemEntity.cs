using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common.Util;
using TipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;


namespace Sam.Domain.Entity
{
    [Serializable]
    public class MovimentoItemEntity : BaseEntity
    {
        public MovimentoItemEntity() { }

        public MovimentoItemEntity(int _Id)
        {
            this.Id = _Id;
        }

        public MovimentoEntity Movimento { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public DateTime? DataVencimentoLote { get; set; }
        public string DataVencimentoLoteFormatado { get; set; }
        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public bool ItemValidado { get; set; }

        public string Destino { get; set; }
        public string DocumentoMovimento { get; set; }
        public string DocumentoOrigem { get; set; }
        public string DocumentoDestino { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime? OperacaoSaida { get; set; }
        public DateTime? EstornoSaida { get; set; }
        public string ValorTotalDocSaida { get; set; }
        public string SomatoriaSaida { get; set; }
        public DateTime? DataEntrada { get; set; }
        public DateTime? OperacaoEntrada { get; set; }
        public DateTime? EstornoEntrada { get; set; }
        public string ValorTotalDocEntrada { get; set; }
        public string SomatoriaEntrada { get; set; }
        public string Produtor { get; set; }
        public string ItMeEpp { get; set; }

        public string IdentificacaoLote { get; set; }
        public string FabricanteLote { get; set; }
        public int IdLote { get; set; }
        public int AgrupamentoId { get; set; }

        public decimal? QtdeMov { get; set; }
        public decimal? QtdeLiq { get; set; }
        public decimal? SaldoLiq { get; set; }
        public decimal? QtdeLiqSiafisico { get; set; }
        public decimal? SaldoLiqSiafisico { get; set; }
        public decimal? SaldoQtde { get; set; }
        public decimal? SaldoQtdeLote { get; set; }
        public decimal? SaldoQtdeLastAtivo { get; set; }

        public decimal? PrecoUnit { get; set; }
        public decimal? PrecoUnitSiafem { get; set; }
        public decimal? PrecoUnitDtMov { get; set; }
        public decimal? PrecoMedio { get; set; }
        public decimal? SaldoValor { get; set; }
        public decimal? ValorMov { get; set; }
        public decimal? ValorUnit { get; set; }
        public decimal? ValorSomaItens { get; set; }
        public decimal? QtdeMedia { get; set; }
        public decimal? ValorOriginalDocumento { get; set; }
        public decimal? Desd { get; set; }
        public bool Ativo { get; set; }
        public int Posicao { get; set; }
        public ItemMaterialEntity ItemMaterial { get; set; }
        public bool Retroativo { get; set; }
        public string NL_Consumo { get; set; }
        public bool? Visualizar { get; set; }
        public string UnidadeFornecimentoSiafisicoDescricao { get; set; }
        public UnidadeFornecimentoSiafEntity UnidadeFornecimentoSiafisico
        {
            //get { return (SubItemMaterial.IsNotNull() ? SubItemMaterial.UnidadeFornecimentoSiafisico : null); }
            //set { }
            get;
            set;
        }
        public string NL_Liquidacao { get; set; }
        public string NL_LiquidacaoEstorno { get; set; }
        public string NL_Reclassificacao { get; set; }
        public string NL_ReclassificacaoEstorno { get; set; }
        public decimal? SaldoValorLastAtivo { get; set; }
        public decimal? PrecoUnitLastAtivo { get; set; }

        public int? UgeCodigo
        {
            get { return UGE != null ? UGE.Codigo : null; }
        }

        public string UgeDescricao
        {
            get { return UGE != null ? UGE.Descricao : null; }
        }

        public long? SubItemCodigo
        {
            get { return SubItemMaterial.Codigo; }
            set
            {
                SubItemMaterial = new SubItemMaterialEntity();
                SubItemMaterial.Codigo = Convert.ToInt64(value);

            }
        }

        public string SubItemCodigoFormatado
        {
            get { return SubItemMaterial.Codigo.ToString().PadLeft(12, '0'); }
            set
            {
               SubItemMaterial.Codigo = Convert.ToInt64(value);

            }
        }

        public string SubItemDescricao
        {
            get { return SubItemMaterial.Descricao; }
            set { SubItemMaterial.Descricao = value; }
        }

        public int NaturezaDespesaCodigo
        {
            get { return SubItemMaterial.NaturezaDespesa.Codigo; }
            set
            {
                SubItemMaterial.NaturezaDespesa.Codigo = value;

            }
        }

        public string NaturezaDespesaDesc
        {
            get { return SubItemMaterial.NaturezaDespesa.Descricao; }
            set {
                SubItemMaterial.NaturezaDespesa.Descricao = value; 
            
            }
        }

        public DateTime? DataMovimento
        {
            get { return Movimento.DataMovimento; }
            set { }
        }

        public int? TipoMovimentoId
        {
            get
            {
                if (Movimento.TipoMovimento != null)
                    return Movimento.TipoMovimento.Id;
                else
                    return null;
            }
        }
        public string TipoMovimentoDescricao { get; set; }

        public int? TipoMovimentoAgrup
        {
            get
            {
                if (Movimento.TipoMovimento != null && Movimento.TipoMovimento.TipoAgrupamento != null)
                    return Movimento.TipoMovimento.TipoAgrupamento.Id;
                else
                    return null;
            }
        }

        public decimal? EstoqueMinimo { get; set; }

        public string NomeFornecedor
        {
            get
            {
                return Movimento.Fornecedor != null ? Movimento.Fornecedor.Nome : null;
            }
        }

        public string NomeRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.Descricao : null; }
        }

        public string FoneRequisitante // = Divisão
        {
            get
            {
                if (Movimento.Divisao != null)
                {
                    if (string.IsNullOrEmpty(Movimento.Divisao.EnderecoTelefone))
                        return string.Empty;
                    else
                        return Movimento.Divisao != null ? String.Format("{0:(##)####-####}", long.Parse(Movimento.Divisao.EnderecoTelefone.Replace("-", "").Replace("/", ""))) : null;
                }
                else
                    return string.Empty;
            }
        }

        public string FaxRequisitante // = Divisão
        {
            get
            {
                if (Movimento.Divisao != null)
                {
                    if (string.IsNullOrEmpty(Movimento.Divisao.EnderecoFax))
                        return string.Empty;
                    else
                        return Movimento.Divisao != null ? String.Format("{0:(##)####-####}", long.Parse(Movimento.Divisao.EnderecoFax.Replace("-", "").Replace("/", ""))) : null;
                }
                else
                    return string.Empty;
            }
        }

        public string EnderecoRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoLogradouro : string.Empty; }
        }

        public string EnderecoNumeroRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoNumero : string.Empty; }
        }

        public string EnderecoComplRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoCompl : string.Empty; }
        }

        public string EnderecoMunicipioRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoMunicipio : string.Empty; }
        }

        public string DestinatarioAlmoxarifado { get; set; }

        public string TotalItens { get; set; }

        public string UfRequisitante // = Divisão
        {
            get
            {
                if (Movimento.Divisao != null)
                {
                    if (Movimento.Divisao.Uf != null)
                        return Movimento.Divisao.Uf.Sigla;
                    else
                        return string.Empty;
                }
                else
                    return string.Empty;
            }
        }

        public string CepRequisitante // = Divisão
        {
            get
            {

                if (Movimento.Divisao == null || string.IsNullOrEmpty(Movimento.Divisao.EnderecoCep))
                    return string.Empty;
                else
                {
                    return Movimento.Divisao != null ?
                        Movimento.Divisao.EnderecoCep != null && Movimento.Divisao.EnderecoCep != "" ?
                            String.Format("{0:00000-000}", int.Parse(Movimento.Divisao.EnderecoCep.Replace("-", "")))
                            : string.Empty
                        : string.Empty;
                }
            }
        }

        public string ResponsaRequisitante
        {
            get
            {
                if (Movimento.Divisao != null)
                    if (Movimento.Divisao.Responsavel != null)
                        return Movimento.Divisao.Responsavel.Descricao;

                return string.Empty;
            }
        }

        public string AnoMesReferencia
        {
            get { return Movimento.AnoMesReferencia; }
        }

        public string Documento
        {
            get //{ return Movimento.NumeroDocumento.Length == 12 ? string.Format(@"{0:0000\/00000000}", Convert.ToInt64(Movimento.NumeroDocumento)) : Movimento.NumeroDocumento; }
            {
                if (!String.IsNullOrEmpty(Movimento.NumeroDocumento))
                    return Movimento.NumeroDocumento.Length == 12 ? string.Format(@"{0:0000\/00000000}", Convert.ToString(Movimento.NumeroDocumento)) : Movimento.NumeroDocumento;
                else
                    return String.Empty;
            }
        }

        public string Empenho
        {
            get { return Movimento.Empenho; }
        }

        public string InfoComplementares
        {
            get { return Movimento.Observacoes; }
        }

        public string UnidadeFornecimento
        {
            get { return SubItemMaterial.UnidadeFornecimento != null ? SubItemMaterial.UnidadeFornecimento.Codigo : string.Empty; }
            set { }
        }

        public string UnidadeFornecimentoDes
        {
            get { return SubItemMaterial.UnidadeFornecimento != null ? SubItemMaterial.UnidadeFornecimento.Descricao : string.Empty; }
            set { }
        }

        public string AlmoxarifadoDesc
        {
            get
            {
                if (Movimento.Almoxarifado != null)

                    try
                    {
                        return string.Format("{0} - {1}", Movimento.Almoxarifado.Codigo.Value.ToString().PadLeft(3, '0'), Movimento.Almoxarifado.Descricao);
                    }
                    catch
                    {
                        return "";
                    }
                else
                    return "";
            }
            set { }
        }

        public string AlmoxOrigemDestino
        {
            get
            {
                if (Movimento.MovimAlmoxOrigemDestino != null)
                    return string.Format("{0} - {1}", Movimento.MovimAlmoxOrigemDestino.Codigo.ToString().PadLeft(3, '0'), Movimento.MovimAlmoxOrigemDestino.Descricao);
                else
                    return "";
            }

        }

        public string ItemMaterialCodigo
        {
            get
            {
                if (SubItemMaterial.ItemMaterial != null)
                    return SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9, '0');
                else
                    return string.Empty;
            }

            set
            {

                SubItemMaterial.ItemMaterial.Codigo = int.Parse(value);

            }
        }

        public string GeradorDescricaoMovimento
        {
            get { return Movimento.GeradorDescricao; }
        }

        public string ObservacoesMovimento
        {
            get { return Movimento.Observacoes; }
        }

        public string InstrucoesMovimento
        {
            get { return Movimento.Instrucoes; }
        }

        public PTResEntity PTRes { get; set; }

        public string PTResCodigo
        {
            get { return ((this.PTRes.IsNotNull() && this.PTRes.Codigo.HasValue) ? this.PTRes.Codigo.ToString() : string.Empty); }
            set
            {
                if (this.PTRes == null)
                    this.PTRes = new PTResEntity();
                if (!string.IsNullOrEmpty(value))
                    this.PTRes.Codigo = int.Parse(value);

            }
        }

        public string PTResAcao
        {
            get { return ((this.PTRes.IsNotNull() && this.PTRes.ProgramaTrabalho.IsNotNull()) ? this.PTRes.ProgramaTrabalho.ProjetoAtividade : string.Empty); }
            set
            {
            }
        }

        public TipoMaterial @TipoMaterial 
        {
            get 
            { 
                TipoMaterial tipoMaterial = TipoMaterial.Indeterminado;
                
                if(this.SubItemMaterial.IsNotNull())
                    tipoMaterial = this.SubItemMaterial.TipoMaterial;

                return tipoMaterial;
            }
        }
    }

}
