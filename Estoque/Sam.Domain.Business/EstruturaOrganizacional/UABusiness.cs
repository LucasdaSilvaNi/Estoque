using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Linq;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region UA

        private UAEntity ua = new UAEntity();

        public UAEntity Ua
        {
            get { return ua; }
            set { ua = value; }
        }

        public bool SalvarUA()
        {
            this.Service<IUAService>().Entity = this.Ua;
            this.ConsistirUA();
            if (this.Consistido)
            {
                this.Service<IUAService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<UAEntity> ListarUA()
        {
            this.Service<IUAService>().SkipRegistros = this.SkipRegistros;
            IList<UAEntity> retorno = this.Service<IUAService>().Listar();
            this.TotalRegistros = this.Service<IUAService>().TotalRegistros();
            return retorno;
        }

        public IList<UAEntity> ListarTodasUA()
        {
            IList<UAEntity> retorno = this.Service<IUAService>().ListarTodas();            
            return retorno;
        }

        public IList<UAEntity> ListarUA(int UgeId)
        {
            this.Service<IUAService>().SkipRegistros = this.SkipRegistros;
            IList<UAEntity> retorno = this.Service<IUAService>().ListarPorUGE(UgeId);
            this.TotalRegistros = this.Service<IUAService>().TotalRegistros();
            return retorno;
        }

        public IList<UAEntity> ListarUA(int UgeId, int UaCodigo)
        {
            this.Service<IUAService>().SkipRegistros = this.SkipRegistros;
            IList<UAEntity> retorno;

            if (UaCodigo == 0)
                retorno = this.Service<IUAService>().ListarPorUGE(UgeId);
            else
                retorno = this.Service<IUAService>().ListarPorUGE(UgeId, UaCodigo);

            this.TotalRegistros = this.Service<IUAService>().TotalRegistros();
            return retorno;
        }

        public IList<UAEntity> ImprimirUA(int UgeId)
        {
            IList<UAEntity> retorno = this.Service<IUAService>().Imprimir(UgeId);
            return retorno;
        }

        public IList<UAEntity> ListarUAPorOrgao(int OrgaoId)
        {
            IList<UAEntity> retorno = this.Service<IUAService>().ListarPorOrgao(OrgaoId);
            return retorno;
        }

        public bool ExcluirUA()
        {
            this.Service<IUAService>().Entity = this.Ua;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IUAService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public IList<UAEntity> ListarUasTodosCodPorOrgao(int? OrgaoId)
        {
            IList<UAEntity> retorno = this.Service<IUAService>().ListarUasTodosCod(OrgaoId);
            return retorno;
        }

        public IList<UAEntity> ListarUasTodosCod()
        {
            return this.Service<IUAService>().ListarTodosCod();
        }

        /// <summary>
        /// Listar Todas as UA's ATIVAS filtrado por Uge
        /// </summary>
        /// <param name="UgeId">Identificador da UGE</param>
        /// <returns></returns>
        public IList<UAEntity> ListarUasTodosCodAtivoPorUge(int? UgeId)
        {
            return (from _ua in this.ListarUasTodosCodPorUge(UgeId)
                    where _ua.IndicadorAtividade == true
                    select _ua).ToList();
        }

        public IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId)
        {
            return this.Service<IUAService>().ListarUasTodosCodPorUge(UgeId);
        }
        public IList<UAEntity> ListarUasTodosCodPorUge(int? UgeId, IList<DivisaoEntity> divisaoList)
        {
            return this.Service<IUAService>().ListarUasTodosCodPorUge(UgeId, divisaoList);
        }

        public IList<UAEntity> ListarUasTodosCodPorUo(int? UoId) 
        {
            return this.Service<IUAService>().ListarUasTodosCodPorUo(UoId);
        }

        public IList<UAEntity> ListarUAsPorUO(int codigoUO)
        {
            return this.Service<IUAService>().ListarUAsPorUO(codigoUO);
        }

        public IList<UAEntity> ListarUAsPorUGE(int codigoUGE)
        {
            return this.Service<IUAService>().ListarUAsPorUGE(codigoUGE);
        }

        public IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almoxarifado)
        {
            return this.Service<IUAService>().ListarUasTodosCodPorAlmoxarifado(almoxarifado);
        }

        public IList<UAEntity> ListarUasTodosCodPorAlmoxarifado(AlmoxarifadoEntity almoxarifado, bool mostraDivisaoEspecial)
        {
            return this.Service<IUAService>().ListarUasTodosCodPorAlmoxarifado(almoxarifado, mostraDivisaoEspecial);
        }

        public void ConsistirUA()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UAEntity>(ref this.ua);

            if (!this.Ua.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.Ua.Uge.Id.HasValue)
                this.ListaErro.Add("E obrigatório informar a UGE.");

            if (!this.Ua.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Código.");
            }
            else
            {
                if (this.Ua.Codigo == 0)
                    this.ListaErro.Add("Código da UA inválido.");
            }

            if (!this.Ua.Id.HasValue && this.Ua.Codigo.HasValue)
            {
                if (this.Service<IUAService>().ObterUAAtivaPorCodigo(this.Ua.Codigo.GetValueOrDefault()) != null)
                    this.ListaErro.Add("Código da UA já cadastrado.");
            }

            if (string.IsNullOrEmpty(this.Ua.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            // Consistência aplicada somente para inativar UA
            if (!this.ua.IndicadorAtividade)
            {
                IList<DivisaoEntity> _divisao = this.ObterDivisaoPorUA(this.ua.Id.GetValueOrDefault(), this.ua.Gestor.Id.GetValueOrDefault());

                if (_divisao != null && _divisao.Count > 0)
                {
                    if (_divisao.Any(d => d.IndicadorAtividade.GetValueOrDefault() == true))
                        this.ListaErro.Add("Existe(m) Divisão(ões) ativa(s) associada(s) a UA.");
                }
            }

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUAService>().ExisteCodigoInformado() && this.ua.IndicadorAtividade)
                    this.ListaErro.Add("Código para UA já existente para este Órgão.");
            }
        }

        private IList<DivisaoEntity> ObterDivisaoPorUA(int uaId, int gestorId)
        {
            return new EstruturaOrganizacionalBusiness().ListarDivisaoByUA(uaId, gestorId);
        }

        public UAEntity ObterUA(int uaID)
        {
            return this.Service<IUAService>().ObterUA(uaID);
        }

        public UAEntity ObterUAPorCodigo(int uaCodigo, int gestorId)
        {
            return this.Service<IUAService>().ObterUAPorCodigo(uaCodigo, gestorId);
        }
        #endregion
    }
}
