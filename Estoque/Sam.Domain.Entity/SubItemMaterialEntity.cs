using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common.Util;
using TipoMaterial = Sam.Common.Util.GeralEnum.TipoMaterial;


namespace Sam.Domain.Entity
{
    [Serializable]
    public class SubItemMaterialEntity : BaseEntity
    {
        public SubItemMaterialEntity() { }

        public SubItemMaterialEntity(int? _id) { this.Id = _id; }

        public long Codigo { get; set; }

        public string Descricao { get; set; }

        public string CodigoDescricao { get; set; }

        public string CodigoBarras { get; set; }

        public NaturezaDespesaEntity NaturezaDespesa { get; set; }

        public ContaAuxiliarEntity ContaAuxiliar { get; set; }

        public List<SaldoSubItemEntity> SaldoSubItems { get; set; }

        public List<SaldoSubItemEntity> SaldoSubItemsTotal { get; set; }

        public SaldoSubItemEntity SomaSaldoLote { get; set; }

        public bool? IsLote { get; set; }

        public bool? IsDecimal { get; set; }

        public bool? IsFracionado { get; set; }

        public bool IndicadorAtividade { get; set; }

        public ItemMaterialEntity ItemMaterial { get; set; }

        public GestorEntity Gestor { get; set; }

        public UnidadeFornecimentoSiafEntity UnidadeFornecimentoSiafisico { get; set; }

        public UnidadeFornecimentoEntity UnidadeFornecimento { get; set; }
        
        public string DescricaoNaturezaDespesa
        { 
            get 
            {
                if (NaturezaDespesa != null)
                    return Convert.ToString(NaturezaDespesa.Codigo.ToString().PadLeft(8, '0'));
                else
                    return string.Empty;
            } 
        }
        public string DescricaoContaAuxiliar { get { return ContaAuxiliar != null ? Convert.ToString(ContaAuxiliar.ContaContabil) : null; } }
        public string DescricaoStatus { get { return IndicadorAtividade == false ? "Inativo" : "Ativo"; } }
        public string DescricaoUnidadeFornecimento 
        { 
            get 
            {
                if (UnidadeFornecimento != null)
                    return UnidadeFornecimento.Codigo;
                else
                    return string.Empty;
            } 
        }

        public bool IndicadorAtividadeAlmox { get; set; }
        public int IndicadorDisponivelId { get; set; }
        public string IndicadorDisponivelDescricao { get; set; }
        public bool IndicadorDisponivel { get; set; }

        public int IndicadorDisponivelIdZerado { get; set; }
        public string IndicadorDisponivelZeradoDescricao { get; set; }
        public bool IndicadorDisponivelZerado { get; set; }
        public int AlmoxarifadoId { get; set; }

        public decimal? SomaSaldoTotal { get; set; }
        public decimal? SomaSaldoValorTotal { get; set; }
        public decimal? SomaSaldoTotalDataMovimento { get; set; }
        public decimal? SomaSaldoValorTotalDataMovimento { get; set; }

        public decimal? PrecoUnitDataMovimento { get; set; }
        public decimal? PrecoUnitAtual { get; set; }

        public decimal? EstoqueMaximo { get; set; }
        public decimal? EstoqueMinimo { get; set; }
        public decimal? SaldoSubItemUnit { get; set; }
        public decimal? SaldoAtual { get; set; }
        public decimal? QtdePeriodo { get; set; }
        public decimal? SaldoReservaMat { get; set; }
        public int? NumeroMesesRelatorio { get; set; }
        public AlmoxarifadoEntity Almoxarifado { get; set; }        
        public string NomeAlmoxarifado { get { return null; } }
        public string NomeDivisao { get; set; }
        public string CodigoUnidadeFornec { get; set; }
        public string CodigoNaturezaDesp { get; set; }

        public string DescricaoUnidade 
        {
            get 
            {
                if (UnidadeFornecimento != null)
                    return UnidadeFornecimento.Codigo;
                else
                    return string.Empty;
            } 
        }

        public string ItemMaterialCodigo
        {
            get
            {
                int iTamanhoCodigo = 0;
                string strRetorno = string.Empty;

                if (ItemMaterial != null && ItemMaterial.Codigo != 0)
                {
                    iTamanhoCodigo = ItemMaterial.Codigo.ToString().Length;
                    strRetorno = ItemMaterial.Codigo.ToString().Insert(iTamanhoCodigo-1, "-");
                    strRetorno = strRetorno.PadLeft(12, '0');
                }


                return strRetorno;
            } 
        }
        /// <summary>
        /// 
        /// para 9 casas decimais sem dígito
        /// </summary>
        public string ItemMaterialCodigoFormatado
        {
            get
            {
                string strRetorno = string.Empty;

                if (ItemMaterial != null && ItemMaterial.Codigo != 0)
                {
                    strRetorno = ItemMaterial.Codigo.ToString().PadLeft(9,'0');
                }


                return strRetorno;
            }
        }

        public TipoMaterial @TipoMaterial { get; set; }
    }
}
