using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sam.Domain.Entity
{
    public class NLConsumoEntity: BaseEntity
    {
        public NLConsumoEntity()
        {
        }
        public NLConsumoEntity(int? id)
        {
            this.Id = id;
        }
        
        int  _PtresCodigo;
        public int PtresCodigo
        {
            get { return _PtresCodigo; }
            set { _PtresCodigo = value; }
        }

        int  _AlmoxarifadoId;
        public int AlmoxarifadoId
        {
            get { return _AlmoxarifadoId; }
            set { _AlmoxarifadoId = value; }
        }

        string _NLConsumoAnoMesRef;
        public string NLConsumoAnoMesRef
        {
            get { return _NLConsumoAnoMesRef; }
            set { _NLConsumoAnoMesRef = value; }
        }

        int _UgeId;
        public int UgeId
        {
            get { return _UgeId; }
            set { _UgeId = value; }
        }

        int _UAId;
        public int UAId
        {
            get { return _UAId; }
            set { _UAId = value; }
        }

        decimal _NLConsumoValor;
        public decimal NLConsumoValor
        {
            get { return _NLConsumoValor; }
            set { _NLConsumoValor = value; }
        }

        string _NLConsumoTipoLancamento;
        public string NLConsumoTipoLancamento
        {
            get { return _NLConsumoTipoLancamento; }
            set { _NLConsumoTipoLancamento = value; }
        }

        string _NLConsumoObservacao;
        public string NLConsumoObservacao
        {
            get { return _NLConsumoObservacao; }
            set { _NLConsumoObservacao = value; }
        }

        string _NLConsumoStatus;
        public string NLConsumoStatus
        {
            get { return _NLConsumoStatus; }
            set { _NLConsumoStatus = value; }
        }

        string _NLConsumoEstimuloWS;
        public string NLConsumoEstimuloWS
        {
            get { return _NLConsumoEstimuloWS; }
            set { _NLConsumoEstimuloWS = value; }
        }

        string _NLConsumoRetornoWS;
        public string NLConsumoRetornoWS
        {
            get { return _NLConsumoRetornoWS; }
            set { _NLConsumoRetornoWS = value; }
        }

        string _NaturezaDespesaCodigo;
        public string NaturezaDespesaCodigo
        {
            get { return _NaturezaDespesaCodigo; }
            set { _NaturezaDespesaCodigo = value; }
        }

        string _UsuarioCPF;
        public string UsuarioCPF
        {
            get { return _UsuarioCPF; }
            set { _UsuarioCPF = value; }
        }

        DateTime _NLConsumoDataPagamento;
        public DateTime NLConsumoDataPagamento
        {
            get { return _NLConsumoDataPagamento; }
            set { _NLConsumoDataPagamento = value; }
        }

    }
}
