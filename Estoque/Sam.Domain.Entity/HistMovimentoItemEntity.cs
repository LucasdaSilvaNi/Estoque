using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class HistMovimentoItemEntity : BaseEntity
    {
        public HistMovimentoItemEntity() { }

        public HistMovimentoItemEntity(int _Id)
        {
            this.Id = _Id;
        }

        public int? MovimentoItemId { get; set; }
        public MovimentoEntity Movimento { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public DateTime? DataVencimentoLote { get; set; }
        public string IdentificacaoLote { get; set; }
        public string FabricanteLote { get; set; }
        public int? QtdeMov { get; set; }
        public int? QtdeLiq { get; set; }
        public decimal? PrecoUnit { get; set; }
        public decimal? SaldoValor { get; set; }
        public int? SaldoQtde { get; set; }
        public int? SaldoQtdeLote { get; set; }
        public decimal? ValorMov { get; set; }
        public decimal? Desd { get; set; }
        public bool Ativo { get; set; }
        public int Posicao { get; set; }

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
        }

        public string SubItemCodigoFormatado
        {
            get { return SubItemMaterial.Codigo.ToString().PadLeft(12,'0'); }
        }

        public string SubItemDescricao
        {
            get { return SubItemMaterial.Descricao; }
            set { SubItemMaterial.Descricao = value; }
        }
        
        public int NaturezaDespesaCodigo
        {
            get { return SubItemMaterial.NaturezaDespesa.Codigo; }
        }

        public string NaturezaDespesaDesc
        {
            get { return SubItemMaterial.NaturezaDespesa.Descricao; }
        }

        public DateTime? DataMovimento 
        {
            get { return Movimento.DataMovimento; } 
        }

        public int? TipoMovimentoAgrup
        {
            get { return Movimento.TipoMovimento != null ? Movimento.TipoMovimento.AgrupamentoId : null; }
        }

        public int EstoqueMinimo { get; set; }

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
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoLogradouro : null; }
        }

        public string EnderecoNumeroRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoNumero : null; }
        }

        public string EnderecoComplRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoCompl : null; }
        }

        public string EnderecoMunicipioRequisitante // = Divisão
        {
            get { return Movimento.Divisao != null ? Movimento.Divisao.EnderecoMunicipio : null; }
        }

        public string UfRequisitante // = Divisão
        {
            get
            { 
                if(Movimento.Divisao !=null)
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
                            : null
                        : null;
                }
            }
        }

        public string ResponsaRequisitante
        {
            get
            {
                return Movimento.Divisao != null ?
                    Movimento.Divisao.Responsavel.Descricao : null;
            }
        }

        public string AnoMesReferencia
        {
            get { return Movimento.AnoMesReferencia; }
        }

        public string Documento
        {
            get { return Movimento.NumeroDocumento.Length == 12 ? string.Format(@"{0:0000\/00000000}", Convert.ToInt64(Movimento.NumeroDocumento)) : Movimento.NumeroDocumento; }
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
            get { return SubItemMaterial.UnidadeFornecimento != null ? SubItemMaterial.UnidadeFornecimento.Codigo : null; }
            set { }
        }

        public string UnidadeFornecimentoDes
        {
            get { return SubItemMaterial.UnidadeFornecimento != null ? SubItemMaterial.UnidadeFornecimento.Descricao : null; }
            set { }
        }

        public string AlmoxarifadoDesc {
            get 
            {
                if (Movimento.Almoxarifado != null)
                    return string.Format("{0} - {1}",Movimento.Almoxarifado.Codigo.Value.ToString().PadLeft(3,'0'), Movimento.Almoxarifado.Descricao);
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
                    return string.Format("{0} - {1}", Movimento.MovimAlmoxOrigemDestino.Codigo.ToString().PadLeft(3,'0'), Movimento.MovimAlmoxOrigemDestino.Descricao);
                else
                    return "";
            }
        }

        public string ItemMaterialCodigo 
        {
            get { return SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9,'0'); }
        }

    }

}
