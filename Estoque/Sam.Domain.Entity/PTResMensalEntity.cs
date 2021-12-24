using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Common.Util;


namespace Sam.Domain.Entity
{
    [Serializable]
    public class PTResMensalEntity : BaseEntity
    {
        public PTResMensalEntity()
        {
        }

        public PTResMensalEntity(int? id)
        {
            this.Id = id;
        }

        PTResEntity _ptRes;
        public PTResEntity PtRes
        {
            get { return _ptRes; }
            set { _ptRes = value; }
        }

        AlmoxarifadoEntity _almoxarifado;
        public AlmoxarifadoEntity Almoxarifado
        {
            get { return _almoxarifado; }
            set { _almoxarifado = value; }
        }

        int _anoMesRef;
        public int AnoMesRef
        {
            get { return _anoMesRef; }
            set { _anoMesRef = value; }
        }

        UGEEntity _uge;
        public UGEEntity UGE
        {
            get { return _uge; }
            set { _uge = value; }
        }

        UOEntity _uo;
        public UOEntity UO
        {
            get { return _uo; }
            set { _uo = value; }
        }

        UAEntity _ua;
        public UAEntity UA
        {
            get { return _ua; }
            set { _ua = value; }
        }

        UGEEntity _ugeAlmoxarifado;
        public UGEEntity UgeAlmoxarifado
        {
            get { return _ugeAlmoxarifado; }
            set { _ugeAlmoxarifado = value; }
        }

        decimal? _valor;
        public decimal? Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        char? _tipoLancamento;
        public char? TipoLancamento
        {
            get { return _tipoLancamento; }
            set { _tipoLancamento = value; }
        }

        string _obs;
        public string Obs
        {
            get { return _obs; }
            set { _obs = value; }
        }

        char? _flagLancamento;
        public char? FlagLancamento
        {
            get { return _flagLancamento; }
            set { _flagLancamento = value; }
        }

        string _nl;
        public string NlLancamento
        {
            get { return _nl; }
            set { _nl = value; }
        }

        string _retorno;
        public string Retorno
        {
            get { return _retorno; }
            set { _retorno = value; }
        }

        string _MensagemWS;
        public string MensagemWs
        {
            get { return _MensagemWS; }
            set { _MensagemWS = value; }
        }

        NaturezaDespesaEntity _naturezaDespesa;
        public NaturezaDespesaEntity NaturezaDespesa
        {
            get { return _naturezaDespesa; }
            set { _naturezaDespesa = value; }
        }

        GestorEntity _gestor;
        public GestorEntity Gestor
        {
            get { return _gestor; }
            set { _gestor = value; }
        }

        int _usuarioSamID;
        public PTResEntity Entity;
        public int UsuarioSamId
        {
            get { return _usuarioSamID; }
            set { _usuarioSamID = value; }
        }

        public string DocumentoRelacionado { get; set; }
        public int MovItemID { get; set; }
        public string MovimentoItemIDs { get; set; }
        public DateTime DataHoraPagamento { get; set; }
        public string Status { get; set; }
        public bool IntegraLote { get; set; }
        public int? NumeradorSequencia { get; set; }

        public int? UgeAlmoxarifado_Codigo { get; set; }
        public string UgeAlmoxarifado_Descricao { get; set; }
        public int? Uge_Codigo { get; set; }
        public string Uge_Descricao { get; set; }
        public int Uo_Codigo { get; set; }
        public string Uo_Descricao { get; set; }
        public int? Ua_Codigo { get; set; }
        public string Ua_Descricao { get; set; }
        public int NaturezaDespesa_Codigo { get; set; }
        public string NaturezaDespesa_Descricao { get; set; }
        public int? PtRes_Codigo { get; set; }

        public string NL_Liquidacao { get; set; }
        public string NL_Reclassificacao { get; set; }

        #region IComparable<PTResMensalEntity> Members
        public int CompareTo(PTResMensalEntity notaComparacao)
        {
            int retornoComparacao = 0;

            if ((this.UA.IsNotNull() && this.UGE.IsNotNull() && this.PtRes.IsNotNull() && this.Valor.HasValue) &&
                (notaComparacao.UA.IsNotNull() && notaComparacao.UGE.IsNotNull() && notaComparacao.UgeAlmoxarifado.IsNotNull() && notaComparacao.PtRes.IsNotNull() && notaComparacao.Valor.HasValue))
            {
                var apenasValorDivergente = (this.UGE.Id == notaComparacao.UGE.Id) &&
                                            (this.UgeAlmoxarifado.Id == notaComparacao.UgeAlmoxarifado.Id) &&
                                            (this.PtRes.Codigo == notaComparacao.PtRes.Codigo) &&
                                            (this.UA.Id == notaComparacao.UA.Id) &&
                                            (this.NaturezaDespesa.Codigo == notaComparacao.NaturezaDespesa.Codigo) &&
                                            (this.NlLancamento == notaComparacao.NlLancamento) &&
                                            (this.Valor != notaComparacao.Valor);

                if (apenasValorDivergente && (this.Valor < notaComparacao.Valor))
                    retornoComparacao = 1;
                else if (apenasValorDivergente && (this.Valor > notaComparacao.Valor))
                    retornoComparacao = - 1;
                else
                    retornoComparacao = 0;
            }

            return retornoComparacao;
        }
        public bool Equals(PTResMensalEntity x, PTResMensalEntity y)
        {
            bool isEqual = false;

            isEqual = (x.UGE.Codigo   == y.UGE.Codigo &&
                       x.UA.Codigo    == y.UA.Codigo &&
                       x.PtRes.Codigo == y.PtRes.Codigo &&
                       x.Valor        == y.Valor);

            return isEqual;
        }
        #endregion
    }
}
