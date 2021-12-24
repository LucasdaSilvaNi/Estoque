using System;
using System.Collections.Generic;
using System.Text;



namespace Sam.Domain.Entity.Relatorios
{
    //public class rowSaldoSubitemFechamento
    //{
    //    public string AnoMesReferencia;
    //    //public string UgeConsulta;
    //    public decimal FechamentoValor;
    //    public decimal FechamentoQtde;

    //    public UGEEntity UgeConsulta;
    //    public AlmoxarifadoEntity AlmoxarifadoConsulta;
    //    public SubItemMaterialEntity SubitemMaterial;
    //    public NaturezaDespesaEntity NaturezaDespesa;
    //}

    //public class relAnaliticoBalanceteMensal
    //{
    //    string NomeDescricaoUGE;
    //    string NomeDescricaoSubitem;
    //    public rowSaldoSubitemFechamento SaldoAnterior;
    //    public rowSaldoSubitemFechamento Entradas;
    //    public rowSaldoSubitemFechamento Saidas;
    //    public rowSaldoSubitemFechamento SaldoFinal;
    //}

    [Serializable]
    public class relFechamentoMensalBase
    {
        public AlmoxarifadoEntity Almoxarifado { get; set; }
        public UGEEntity UGE { get; set; }
        public SubItemMaterialEntity SubItemMaterial { get; set; }
        public int AnoMesRef { get; set; }
        public int SituacaoFechamento { get; set; }
        public decimal? QtdeEntrada { get; set; }
        public decimal? QtdeSaida { get; set; }
        public decimal? ValorEntrada { get; set; }
        public decimal? ValorSaida { get; set; }

        public decimal? SaldoQtde { get; set; }
        public decimal? SaldoValor { get; set; }

        #region Campos Relatorio
        public string NaturezaDespesa
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && (SubItemMaterial.NaturezaDespesa != null) && (SubItemMaterial.NaturezaDespesa.Codigo != 0 && !String.IsNullOrWhiteSpace(SubItemMaterial.NaturezaDespesa.Descricao)))
                    _strDescricao = String.Format("{0} - {1}", SubItemMaterial.NaturezaDespesa.Codigo.ToString().PadLeft(8, '0'), SubItemMaterial.NaturezaDespesa.Descricao);

                return _strDescricao;
            }
        }
        public string UgeCodigo
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((UGE != null) && (UGE.Codigo != 0))
                    _strDescricao = UGE.Codigo.ToString().PadLeft(6, '0');

                return _strDescricao;
            }
        }
        public string UgeDescricao
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((UGE != null) && !String.IsNullOrWhiteSpace(UGE.Descricao))
                    _strDescricao = UGE.Descricao;

                return _strDescricao;
            }
        }
        public string UgeCodigoDescricao
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((UGE != null) && !String.IsNullOrWhiteSpace(UGE.Descricao) && (UGE.Codigo != 0))
                    _strDescricao = String.Format("{0} - {1}", UGE.Codigo.ToString().PadLeft(6, '0'), UGE.Descricao);

                return _strDescricao;
            }
        }
        public string SubItemCodigo
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && (SubItemMaterial.Codigo != 0))
                    _strDescricao = SubItemMaterial.Codigo.ToString().PadLeft(12, '0');

                return _strDescricao;
            }
        }
        public string SubItemDescricao
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && !String.IsNullOrWhiteSpace(SubItemMaterial.Descricao))
                    _strDescricao = SubItemMaterial.Descricao;

                return _strDescricao;
            }
        }
        public string SubitemCodigoDescricao 
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && !String.IsNullOrWhiteSpace(SubItemMaterial.Descricao) && (SubItemMaterial.Codigo != 0))
                    _strDescricao = String.Format("{0} - {1}", SubItemMaterial.Codigo.ToString().PadLeft(12, '0'), SubItemMaterial.Descricao);

                return _strDescricao;
            }
        }
        public string UnidadeFornecimento
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && (SubItemMaterial.UnidadeFornecimento != null) && (!String.IsNullOrWhiteSpace(SubItemMaterial.UnidadeFornecimento.Codigo)))
                    _strDescricao = SubItemMaterial.UnidadeFornecimento.Codigo;

                return _strDescricao;
            }
        }
        public string AlmoxarifadoDescricao
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((Almoxarifado != null) && (Almoxarifado.Codigo != 0 && !String.IsNullOrWhiteSpace(Almoxarifado.Descricao)))
                    _strDescricao = String.Format("{0} - {1}", Almoxarifado.Codigo.ToString().PadLeft(3, '0'), Almoxarifado.Descricao);

                return _strDescricao;
            }
        }
        public string ItemMaterialCodigo
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && (SubItemMaterial.ItemMaterial != null && SubItemMaterial.ItemMaterial.Codigo != 0))
                    _strDescricao = SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9, '0');

                return _strDescricao;
            }
        }
        public string ItemMaterialCodigoDescricao
        {
            get
            {
                string _strDescricao = string.Empty;

                if ((SubItemMaterial != null) && (SubItemMaterial.ItemMaterial != null && !String.IsNullOrWhiteSpace(SubItemMaterial.ItemMaterial.Descricao) && SubItemMaterial.ItemMaterial.Codigo != 0))
                    _strDescricao = _strDescricao = String.Format("{0} - {1}", SubItemMaterial.ItemMaterial.Codigo.ToString().PadLeft(9, '0'), SubItemMaterial.ItemMaterial.Descricao);

                return _strDescricao;
            }
        }
        #endregion Campos Relatorio
    }

    [Serializable]
    public class relAnaliticoFechamentoMensalEntity : relFechamentoMensalBase
    {
        public relAnaliticoFechamentoMensalEntity() { }

        public decimal? SaldoAnterior { get; set; }
        public decimal? SaldoAnteriorValor { get; set; }

        #region Campos Totais

        public decimal SomatoriaQtdeEntradaNaturezaDespesa;
        public decimal SomatoriaSaldoEntradaNaturezaDespesa;

        public decimal SomatoriaQtdeSaidaNaturezaDespesa;
        public decimal SomatoriaSaldoSaidaNaturezaDespesa;

        public decimal SomatoriaQtdeAnteriorNaturezaDespesa;
        public decimal SomatoriaSaldoAnteriorNaturezaDespesa;

        public decimal QtdeFinalNaturezaDespesa;
        public decimal SaldoFinalNaturezaDespesa;

        public decimal QtdeFinalMes;
        public decimal SaldoFinalMes;

        #endregion Campos Totais
    }

    [Serializable]
    public class relInventarioFechamentoMensalEntity : relFechamentoMensalBase
    {
        public relInventarioFechamentoMensalEntity() { }

        public decimal? SaldoAnterior { get; set; }        
        public decimal? SaldoAnteriorValor { get; set; }

        #region Campos Totais

        public decimal SomatoriaQtdeSubitemMaterial;
        public decimal SomatoriaValorSubitemMaterial;

        public decimal SomatoriaQtdeSaidaNaturezaDespesa;
        public decimal SomatoriaSaldoSaidaNaturezaDespesa;

        public decimal SomatoriaQtdeAnteriorNaturezaDespesa;
        public decimal SomatoriaSaldoAnteriorNaturezaDespesa;

        public decimal QtdeFinalNaturezaDespesa;
        public decimal SaldoFinalNaturezaDespesa;

        public decimal QtdeFinalMes;
        public decimal SaldoFinalMes;

        #endregion Campos Totais
    }
}
