using System;
using System.Collections.Generic;
using System.Transactions;
using Sam.Domain.Entity;
using Sam.Domain.Infrastructure;
using Sam.ServiceInfraestructure;




namespace Sam.Domain.Business
{
    public class FuncionalidadeSistemaBusiness : BaseBusiness
    {
        private FuncionalidadeSistemaEntity entity = new FuncionalidadeSistemaEntity();
        public FuncionalidadeSistemaEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public IList<FuncionalidadeSistemaEntity> Listar(int[] perfilIDs)
        {
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;
            using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    //var srvInfra = this.Service<IChamadoSuporteService>();
                    var srvInfra = new FuncionalidadeSistemaInfrastructure();
                    lstRetorno = srvInfra.Listar(perfilIDs);
                }
                catch (Exception excErroPesquisa)
                {
                    this.ListaErro.Add("Não foi possível obter resultados para esta solicitação.");
                }

                tsOperacao.Complete();
            }

            return lstRetorno;
        }

        public bool GravarFuncionalidade(FuncionalidadeSistemaEntity chamadoSuporte)
        {
            bool blnRetorno = false;
            FuncionalidadeSistemaInfrastructure objInfra = null;

            using (TransactionScope tsOperacao = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    objInfra = new FuncionalidadeSistemaInfrastructure();
                    objInfra.Entity = chamadoSuporte;

                    //blnRetorno = objInfra.Salvar();
                    objInfra.Salvar();
                }
                catch (Exception excErroGravacao)
                {
                    var _descErro = String.Format("Erro ao salvar chamado suporte: {0}", excErroGravacao.Message);

                    ListaErro.Add(_descErro);
                    LogErro.GravarMsgInfo("Chamado Suporte Funcionalidade Sistema", _descErro);
                }

                tsOperacao.Complete();
            }
            return blnRetorno;
        }
        public bool Salvar()
        {
            this.Service<IFuncionalidadeSistemaService>().Entity = this.Entity;
            if (this.Consistido)
            {
                this.Service<IFuncionalidadeSistemaService>().Salvar();
            }
            return this.Consistido;
        }
        public IList<FuncionalidadeSistemaEntity> Imprimir()
        {
            IList<FuncionalidadeSistemaEntity> lstRetorno = null;

            lstRetorno = this.Service<IFuncionalidadeSistemaService>().Imprimir();

            return lstRetorno;
        }

        public bool Excluir()
        {
            this.Service<IFuncionalidadeSistemaService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IFuncionalidadeSistemaService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }
        public FuncionalidadeSistemaEntity ObterChamadoSuporteFuncionalidade(int chamadoSuporteID)
        {
            FuncionalidadeSistemaEntity objEntidade = null;
            
            return objEntidade;
        }

        public int ObterNumeroFuncionalidadesSistema()
        {
            int _numRetorno = -1;
            FuncionalidadeSistemaInfrastructure objInfra;

            try
            {
                objInfra = new FuncionalidadeSistemaInfrastructure();
                _numRetorno = objInfra.Count();
            }
            catch (Exception excErroGravacao)
            {
                var _descErro = String.Format(excErroGravacao.Message);

                ListaErro.Add(_descErro);
                LogErro.GravarMsgInfo("Chamado Suporte", _descErro);
            }

            return _numRetorno;
        }
    }
}
