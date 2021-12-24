using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;
using Sam.Common.Util;
using System.Linq.Expressions;
using System.Diagnostics;
using casasDecimais = Sam.Common.Util.GeralEnum.casasDecimais;
using System.Data;
using System.Xml.Linq;

namespace Sam.Domain.Infrastructure
{
    public class NLConsumoInfrastructure : BaseInfraestructure
    {
        TB_NL_CONSUMO tbNlConsumo = new TB_NL_CONSUMO();
        TB_NL_CONSUMO_MOVIMENTO_ITEM tbNlConsumoMovimentoItem = new TB_NL_CONSUMO_MOVIMENTO_ITEM();

        private NLConsumoEntity NLConsumo = new NLConsumoEntity();
        //TODO: Criar métodos de extensão especifico para isso, para poupar banco.
        private IDictionary<string, BaseEntity> dicObjetoMemoria = new Dictionary<string, BaseEntity>(StringComparer.InvariantCultureIgnoreCase);

        public NLConsumoEntity Entity
        {
            get { return NLConsumo; }
            set { NLConsumo = value; }
        }

        public void CriarNLConsumo(DataTable dataSource)
        {
            if (this.Entity.Id.HasValue)
                tbNlConsumo = this.Db.TB_NL_CONSUMOs.Where(a => a.TB_NL_CONSUMO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_NL_CONSUMOs.InsertOnSubmit(tbNlConsumo);

            tbNlConsumo.TB_NL_CONSUMO_ID = (int)this.Entity.Id;
            tbNlConsumo.TB_NL_CONSUMO_CODIGO_PTRES = this.Entity.PtresCodigo;
            tbNlConsumo.TB_ALMOXARIFADO_ID = this.Entity.AlmoxarifadoId;
            tbNlConsumo.TB_NL_CONSUMO_ANO_MES_REF = this.Entity.NLConsumoAnoMesRef;
            tbNlConsumo.TB_UGE_ID = this.Entity.UgeId;
            tbNlConsumo.TB_UA_ID = this.Entity.UAId;
            tbNlConsumo.TB_NL_CONSUMO_VALOR = this.Entity.NLConsumoValor;
            tbNlConsumo.TB_NL_CONSUMO_TIPO_LANC = this.Entity.NLConsumoTipoLancamento;
            tbNlConsumo.TB_NL_CONSUMO_OBS = this.Entity.NLConsumoObservacao;
            tbNlConsumo.TB_NL_CONSUMO_STATUS = this.Entity.NLConsumoStatus;
            //tbNlConsumo.TB_NL_CONSUMO_ESTIMULO_WS = this.Entity.NLConsumoEstimuloWS;
            //tbNlConsumo.TB_NL_CONSUMO_RETORNO_WS = this.Entity.NLConsumoRetornoWS;
            tbNlConsumo.TB_NATUREZA_DESPESA_CODIGO = this.Entity.NaturezaDespesaCodigo;
            tbNlConsumo.TB_USUARIO_CPF = this.Entity.UsuarioCPF;
            tbNlConsumo.TB_NL_CONSUMO_DATA_PAGAMENTO = this.Entity.NLConsumoDataPagamento;

            this.Db.SubmitChanges();

        }

        public void EstornarNLConsumo()
        {
        }

        public void ListarNLConsumo()
        {
        }
    }
}
