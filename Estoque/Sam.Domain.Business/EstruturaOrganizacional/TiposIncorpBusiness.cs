using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {        
        #region TiposIncorp

        private TipoIncorpEntity tipoIncorp = new TipoIncorpEntity();
        public TipoIncorpEntity TipoIncorp
        {
            get { return tipoIncorp; }
            set { tipoIncorp = value; }
        }

        public IList<TipoIncorpEntity> ListarTiposIncorp()
        {
            this.Service<ITipoIncorpService>().SkipRegistros = this.SkipRegistros;
            IList<TipoIncorpEntity> retorno =  this.Service<ITipoIncorpService>().Listar();
            this.TotalRegistros = this.Service<ITipoIncorpService>().TotalRegistros();
            return retorno;
        }

        public IList<TipoIncorpEntity> ImprimirTiposIncorp()
        {
            IList<TipoIncorpEntity> retorno = this.Service<ITipoIncorpService>().Imprimir();
            return retorno;
        }
        
        public bool SalvarTipoIncorp()
        {
            this.Service<ITipoIncorpService>().Entity = this.TipoIncorp;
            this.ConsistirTipoIncorp();
            if (Consistido)
            {
                this.Service<ITipoIncorpService>().Salvar();
            }

            return this.Consistido;
        }

        public bool ExcluirTipoIncorp()
        {
            this.Service<ITipoIncorpService>().Entity = this.TipoIncorp;
            if (this.Consistido)
            {
                try
                {
                    this.Service<ITipoIncorpService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        private void ConsistirTipoIncorp()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<TipoIncorpEntity>(ref this.tipoIncorp);

            if (!this.TipoIncorp.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o código.");
            }

            if (string.IsNullOrEmpty(this.TipoIncorp.Descricao))
            {
                this.ListaErro.Add("É obrigatório informar a Descrição.");
            }

            if (this.TipoIncorp.CodigoTransacao.Length > 3)
            {
                this.ListaErro.Add("Campo Código Transação deve ter no máximo três caracteres!");
            }

            if (ListaErro.Count == 0)
            {
                if (this.Service<ITipoIncorpService>().ExisteCodigoInformado())
                {
                    ListaErro.Add("Código já existe.");
                }
            }
        }

        #endregion
    }
}
