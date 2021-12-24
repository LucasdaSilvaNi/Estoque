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
        #region UO

        private UOEntity uo = new UOEntity();

        public UOEntity UO
        {
            get { return uo; }
            set { uo = value; }
        }

        public bool SalvarUO()
        {
            this.Service<IUOService>().Entity = this.UO;
            this.ConsistirUO();
            if (this.Consistido)
            {
                this.Service<IUOService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<UOEntity> ListarUO(int OrgaoId)
        {
            this.Service<IUOService>().SkipRegistros = this.SkipRegistros;
            IList<UOEntity> retorno = this.Service<IUOService>().Listar(OrgaoId);
            this.TotalRegistros = this.Service<IUOService>().TotalRegistros();
            return retorno;
        }

        public IList<UOEntity> ImprimirUO(int OrgaoId)
        {
            this.Service<IUOService>().SkipRegistros = this.SkipRegistros;
            IList<UOEntity> retorno = this.Service<IUOService>().Imprimir(OrgaoId);
            this.TotalRegistros = this.Service<IUOService>().TotalRegistros();
            return retorno;
        }

        public IList<UOEntity> ListarUO()
        {
            this.Service<IUOService>().SkipRegistros = this.SkipRegistros;
            IList<UOEntity> retorno = this.Service<IUOService>().Listar();
            this.TotalRegistros = this.Service<IUOService>().TotalRegistros();
            return retorno;
        }

        public bool ExcluirUO()
        {
            this.Service<IUOService>().Entity = this.UO;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IUOService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirUO()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UOEntity>(ref this.uo);

            if (!this.UO.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (this.UO.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.UO.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUOService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
            //Listar UGE(s) Ativas Associadas a UO
            if (!this.ua.IndicadorAtividade && UO.Ativo == false)
            {
                IList<UOEntity> _listarUge = this.ListarUgePorUo(this.UO.Codigo);
                if (_listarUge != null && _listarUge.Count > 0)
                {
                    this.ListaErro.Add("Existe(m) UGE(s) ativa(s) associada(s) a essa UO");
                }
            }
        }

        public IList<UOEntity> ListarUgePorUo(int ugeId)
        {
            return this.Service<IUOService>().ListarUgePorUo(ugeId);
        }

        public UOEntity ObterUoPorCodigoUGE(int codigoUGE)
        {
            UOEntity objEntidade = null;
            var svcInfra = this.Service<IUOService>();
            objEntidade = svcInfra.ObterUoPorCodigoUGE(codigoUGE);

            return objEntidade;
        }

        #endregion
    }
}
