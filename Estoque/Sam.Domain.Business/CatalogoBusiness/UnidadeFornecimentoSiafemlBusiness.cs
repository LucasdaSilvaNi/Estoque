using System.Collections.Generic;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;

namespace Sam.Domain.Business
{
    public partial class CatalogoSiafemBusiness : BaseBusiness
    {
        #region Unidade Fornecimento

        private UnidadeFornecimentoSiafemEntity unidadeFornecimento = new UnidadeFornecimentoSiafemEntity();

        public UnidadeFornecimentoSiafemEntity UnidadeFornecimento
        {
            get { return unidadeFornecimento; }
            set { unidadeFornecimento = value; }
        }

        public bool SalvarUnidadeFornecimento()
        {
            this.Service<IUnidadeFornecimentoSiafemService>().Entity = this.UnidadeFornecimento;
            this.ConsistirUnidadeFornecimento();
            if (this.Consistido)
            {
                this.Service<IUnidadeFornecimentoSiafemService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<UnidadeFornecimentoSiafemEntity> ListarUnidadeFornecimentoTodosCod()
        {

            this.Service<IUnidadeFornecimentoSiafemService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoSiafemEntity> retorno = this.Service<IUnidadeFornecimentoSiafemService>().ListarTodosCod();
            this.TotalRegistros = this.Service<IUnidadeFornecimentoSiafemService>().TotalRegistros();
            return retorno;
        }


        public IList<UnidadeFornecimentoSiafemEntity> ListarUnidadeFornecimento()
        {
            this.Service<IUnidadeFornecimentoSiafemService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoSiafemEntity> retorno = this.Service<IUnidadeFornecimentoSiafemService>().Listar();
            this.TotalRegistros = this.Service<IUnidadeFornecimentoSiafemService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoSiafemEntity> ImprimirUnidadeFornecimento(int OrgaoId, int GestorId)
        {
            
            var service = new Sam.Domain.Infrastructure.UnidadeFornecimentoSiafemInfraestructure();
            IList<UnidadeFornecimentoSiafemEntity> retorno = service.Imprimir();
            return retorno;
        }

        //public bool ExcluirUnidadeFornecimento()
        //{
        //    this.Service<IUnidadeFornecimentoSiafemService>().Entity = this.UnidadeFornecimento;
        //    if (this.Consistido)
        //    {
        //        try
        //        {
        //            this.Service<IUnidadeFornecimentoSiafemService>().Excluir();
        //        }
        //        catch (Exception ex)
        //        {
        //            TratarErro(ex);
        //        }
        //    }
        //    return this.Consistido;
        //}

        public void ConsistirUnidadeFornecimento()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UnidadeFornecimentoSiafemEntity>(ref this.unidadeFornecimento);

            if (string.IsNullOrEmpty(this.UnidadeFornecimento.Codigo))
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.UnidadeFornecimento.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUnidadeFornecimentoSiafemService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
