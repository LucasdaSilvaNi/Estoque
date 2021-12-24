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
        #region Unidade Fornecimento

        private UnidadeFornecimentoEntity unidadeFornecimento = new UnidadeFornecimentoEntity();

        public UnidadeFornecimentoEntity UnidadeFornecimento
        {
            get { return unidadeFornecimento; }
            set { unidadeFornecimento = value; }
        }

        public bool SalvarUnidadeFornecimento()
        {
            this.Service<IUnidadeFornecimentoService>().Entity = this.UnidadeFornecimento;
            this.ConsistirUnidadeFornecimento();
            if (this.Consistido)
            {
                this.Service<IUnidadeFornecimentoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<UnidadeFornecimentoEntity> PopularUnidFornecimentoTodosPorUge(int _ugeId)
        {
            this.Service<IUnidadeFornecimentoService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().PopularUnidFornecimentoTodosPorUge(_ugeId);
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ListarUnidadeFornecimento(int OrgaoId, int GestorId)
        {
            this.Service<IUnidadeFornecimentoService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().Listar(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ListarUnidadeFornecimento(int OrgaoId, bool noSkipResultSet)
        {
            this.Service<IUnidadeFornecimentoService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().Listar(OrgaoId, noSkipResultSet);
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ListarUnidadeFornecimentoTodosCod(int OrgaoId, int GestorId)
        {
            this.Service<IUnidadeFornecimentoService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().ListarTodosCod(OrgaoId, GestorId);
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ListarUnidadeFornecimento(int? OrgaoId)
        {
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().Listar(OrgaoId);
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ListarUnidadeFornecimento()
        {
            this.Service<IUnidadeFornecimentoService>().SkipRegistros = this.SkipRegistros;
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().Listar();
            this.TotalRegistros = this.Service<IUnidadeFornecimentoService>().TotalRegistros();
            return retorno;
        }

        public IList<UnidadeFornecimentoEntity> ImprimirUnidadeFornecimento(int OrgaoId, int GestorId)
        {
            IList<UnidadeFornecimentoEntity> retorno = this.Service<IUnidadeFornecimentoService>().Imprimir(OrgaoId, GestorId);
            return retorno;
        }

        public bool ExcluirUnidadeFornecimento()
        {
            this.Service<IUnidadeFornecimentoService>().Entity = this.UnidadeFornecimento;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IUnidadeFornecimentoService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirUnidadeFornecimento()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UnidadeFornecimentoEntity>(ref this.unidadeFornecimento);

            if (!this.UnidadeFornecimento.Gestor.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (string.IsNullOrEmpty(this.UnidadeFornecimento.Codigo))
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.UnidadeFornecimento.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUnidadeFornecimentoService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
