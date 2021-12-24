using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Atividade Natureza
        private AtividadeNaturezaDespesaEntity atividadenatureza = new AtividadeNaturezaDespesaEntity();

        public AtividadeNaturezaDespesaEntity AtividadeNatureza
        {
            get { return atividadenatureza; }
            set { atividadenatureza = value; }
        }

        public bool SalvarAtividadeNatureza()
        {
            this.Service<IAtividadeNaturezaService>().Entity = this.AtividadeNatureza;
            this.ConsistirAtividadeNatureza();
            if (this.Consistido)
            {
                this.Service<IAtividadeNaturezaService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<AtividadeNaturezaDespesaEntity> ListarAtividadeNatureza()
        {
            this.Service<IAtividadeNaturezaService>().SkipRegistros = this.SkipRegistros;
            IList<AtividadeNaturezaDespesaEntity> retorno = this.Service<IAtividadeNaturezaService>().Listar();
            this.TotalRegistros = this.Service<IAtividadeNaturezaService>().TotalRegistros();
            return retorno;
        }

        public bool ExcluirAtividadeNatureza()
        {
            this.Service<IAtividadeNaturezaService>().Entity = this.AtividadeNatureza;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IAtividadeNaturezaService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }     

        public void ConsistirAtividadeNatureza()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<AtividadeNaturezaDespesaEntity>(ref this.atividadenatureza);

            if (string.IsNullOrEmpty(this.AtividadeNatureza.Descricao))
                this.ListaErro.Add("É obrigatório informar a Atividade.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IAtividadeNaturezaService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
