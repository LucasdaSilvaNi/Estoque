using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using System.Collections;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;
using System.Transactions;

namespace Sam.Domain.Business
{
    public partial class EstruturaOrganizacionalBusiness : BaseBusiness
    {
        #region UGE

        private UGEEntity uge = new UGEEntity();

        public UGEEntity UGE
        {
            get { return uge; }
            set { uge = value; }
        }

        public bool SalvarUGE()
        {
            this.Service<IUGEService>().Entity = this.UGE;
            this.ConsistirUGE();

            if (this.Consistido)
            {
                this.Service<IUGEService>().Salvar();
            }
            return this.Consistido;
        }

        public bool ExcluirUGE()
        {
            this.Service<IUGEService>().Entity = this.UGE;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IUGEService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public IList<UGEEntity> ListarUGE(int? UoId)
        {
            this.Service<IUGEService>().SkipRegistros = this.SkipRegistros;
            IList<UGEEntity> retorno = Service<IUGEService>().Listar(UoId);
            this.TotalRegistros = this.Service<IUGEService>().TotalRegistros();
            return retorno;
        }

        public IList<UGEEntity> ListarUGESaldoTodosCod(int subItemId, int almoxarifadoId, int ugeId)
        {
            IList<UGEEntity> retorno = Service<IUGEService>().ListarUGESaldoTodosCod(subItemId, almoxarifadoId, ugeId);
            return retorno;
        }

        public IList<UGEEntity> ListarUGE(int OrgaoId, int UoId)
        {
            this.Service<IUGEService>().SkipRegistros = this.SkipRegistros;
            IList<UGEEntity> retorno = Service<IUGEService>().Listar(OrgaoId, UoId);
            this.TotalRegistros = this.Service<IUGEService>().TotalRegistros();
            return retorno;
        }

        public IList<UGEEntity> ListarUGESaldoSubItemComSaldo(int _gestorId, int _almoxId)
        {
            return this.Service<IUGEService>().ListarUGESaldoTodosCod(_gestorId, _almoxId);
        }

        public IList<UGEEntity> ListarUGEsComSaldoParaSubitem(long _subItemCodigo, int _almoxId)
        {
            return this.Service<IUGEService>().ListarUGEsComSaldoParaSubitem(_subItemCodigo, _almoxId);
        }

        public IList<UGEEntity> ListarTodosCodPorGestor(int GestorId)
        {
            IList<UGEEntity> retorno = Service<IUGEService>().ListarTodosCodPorGestor(GestorId);
            return retorno;
        }

        public IList<UGEEntity> ListarUgesTodosCod()
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    return this.Service<IUGEService>().ListarTodosCod();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<UGEEntity> ListarUGE()
        {
            this.Service<IUGEService>().SkipRegistros = this.SkipRegistros;
            IList<UGEEntity> retorno = Service<IUGEService>().Listar();
            this.TotalRegistros = this.Service<IUGEService>().TotalRegistros();
            return retorno;
        }

        public IList<UGEEntity> ImprimirUGE(int OrgaoId, int UoId)
        {
            IList<UGEEntity> retorno = Service<IUGEService>().Imprimir(OrgaoId, UoId);
            return retorno;
        }

        public IList<TipoUGEEntity> ListarTipoUGE()
        {
            IList<TipoUGEEntity> retorno = Service<IUGEService>().ListarTipoUGE();
            return retorno;
        }

        public void ConsistirUGE()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<UGEEntity>(ref this.uge);

            if (!this.UGE.Orgao.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o Órgão.");

            if (!this.UGE.Uo.Id.HasValue)
                this.ListaErro.Add("É obrigatório informar o UO.");

            if (!this.UGE.Codigo.HasValue)
            {
                this.ListaErro.Add("É obrigatório informar o Código.");
            }
            else
            {
                if (this.UGE.Codigo == 0)
                    this.ListaErro.Add("Código da UA inválido.");
            }

            if (string.IsNullOrEmpty(this.UGE.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");

            if (this.ListaErro.Count == 0)
            {
                if (this.Service<IUGEService>().ExisteCodigoInformado())
                    this.ListaErro.Add("Código já existente.");
            }
            //Listar UA(s) Ativas Associadas a UGE          
            if (!this.ua.IndicadorAtividade && uge.Ativo == false)
            {
                IList<UGEEntity> _listar = this.ListarUaAssociadaUge(this.UGE.Codigo);
                if (_listar != null && _listar.Count > 0)
                {
                    this.ListaErro.Add("Existe(m) UA(s) ativa(s) associada(s) a essa UGE");
                }
            }
        }
        public IList<UGEEntity> ListarUaAssociadaUge(int? ugeId)
        {
            return this.Service<IUGEService>().ListarUaAssociadaUge(ugeId);
        }
    

    #endregion
}
}
