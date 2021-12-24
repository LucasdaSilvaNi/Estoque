using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.Infrastructure;
using Sam.Entity;



namespace Sam.Business
{
    public partial class UnidadeFornecimentoConversaoBusiness : BaseBusiness
    {
        #region Unidade Fornecimento Conversao

        private UnidadeFornecimentoConversaoEntity unidadeFornecimentoConversao = new UnidadeFornecimentoConversaoEntity();

        public UnidadeFornecimentoConversaoEntity UnidadeFornecimentoConversao
        {
            get { return unidadeFornecimentoConversao; }
            set { unidadeFornecimentoConversao = value; }
        }

        public bool SalvarUnidadeFornecimentoDeConversao()
        {
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();
            this.ConsistirUnidadeFornecimentoConversao();
            if (this.Consistido)
            {
                infraEstrutura.Insert(this.UnidadeFornecimentoConversao);
            }
            return this.Consistido;
        }

        //public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadeFornecimento(int? OrgaoId)
        //{
        //    IList<UnidadeFornecimentoConversaoEntity> retorno = this.Service<IUnidadeFornecimentoConversaoService>().Listar(OrgaoId);
        //    this.TotalRegistros = this.Service<IUnidadeFornecimentoConversaoService>().TotalRegistros();
        //    return retorno;
        //}

        public IList<UnidadeFornecimentoSiafEntity> ListarUnidadeFornecimentoSiafisico()
        {
            IList<UnidadeFornecimentoSiafEntity>  lstRetorno     = null;
            UnidadeFornecimentoSiafInfrastructure infraEstrutura = new UnidadeFornecimentoSiafInfrastructure();

            lstRetorno = infraEstrutura.ListarUnidadeFornecimentoSiafisico();

            return lstRetorno;
        }

        public bool ExcluirUnidadeFornecimentoDeConversao()
        {
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();
            if (this.Consistido)
            {
                try
                {
                    infraEstrutura.Delete(this.UnidadeFornecimentoConversao);
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(int UnidadeFornecimentoConversao_ID)
        {
            UnidadeFornecimentoConversaoEntity         objRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            objRetorno = infraEstrutura.ObterDadosUnidadeFornecimentoConversao(UnidadeFornecimentoConversao_ID);

            return objRetorno;
        }
        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(string strCodigoUnidadeConversao)
        {
            UnidadeFornecimentoConversaoEntity         objRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            objRetorno = infraEstrutura.ObterDadosUnidadeFornecimentoConversao(strCodigoUnidadeConversao);

            return objRetorno;
        }

        public UnidadeFornecimentoSiafEntity ObterUnidadeFornecimentoSiafisico(string strCodigoUnidadeConversao)
        {
            UnidadeFornecimentoSiafEntity objRetorno = null;
            UnidadeFornecimentoSiafInfrastructure infraEstrutura = new UnidadeFornecimentoSiafInfrastructure();

            objRetorno = infraEstrutura.ObterDadosUnidadeFornecimentoSiafisico(strCodigoUnidadeConversao);

            return objRetorno;
        }

        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversao()
        {
            IList<UnidadeFornecimentoConversaoEntity>  lstRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            lstRetorno = infraEstrutura.ListarUnidadesDeConversao();

            return lstRetorno;
        }
        public IList<UnidadeFornecimentoConversaoEntity> ListarUnidadesDeConversaoPorGestor(int? Gestor_ID)
        {
            IList<UnidadeFornecimentoConversaoEntity>  lstRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            lstRetorno = infraEstrutura.ListarUnidadesDeConversaoPorGestor(Gestor_ID);

            return lstRetorno;
        }

        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(int? gestorID, string strUnidadeSiafisico, int subitemMaterialId)
        {
            UnidadeFornecimentoConversaoEntity objRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            objRetorno = infraEstrutura.ObterDadosUnidadeFornecimentoConversao(gestorID, strUnidadeSiafisico, subitemMaterialId);

            return objRetorno;
        }

        public UnidadeFornecimentoConversaoEntity ObterDadosUnidadeFornecimentoConversao(string strDescricaoUnidadeSIAFISICO, int subitemMaterialId)
        {
            UnidadeFornecimentoConversaoEntity objRetorno = null;
            UnidadeFornecimentoConversaoInfrastructure infraEstrutura = new UnidadeFornecimentoConversaoInfrastructure();

            objRetorno = infraEstrutura.ObterDadosUnidadeFornecimentoConversao(strDescricaoUnidadeSIAFISICO, subitemMaterialId);

            return objRetorno;
        }

        public void ConsistirUnidadeFornecimentoConversao()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UnidadeFornecimentoConversaoEntity>(ref this.unidadeFornecimentoConversao);


            if (string.IsNullOrEmpty(this.UnidadeFornecimentoConversao.Codigo))
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.UnidadeFornecimentoConversao.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                //if (this.Service<IUnidadeFornecimentoConversaoService>().ExisteCodigoInformado())
                  //  this.ListaErro.Add("Código já existente.");
            }
        }

        #endregion
    }
}
