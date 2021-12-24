using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using Sam.Domain.Entity;
using Sam.Common.Util;

namespace Sam.Domain.Business
{
    public class TipoMovimentoBusiness : BaseBusiness
    {

        private TipoMovimentoEntity tipoMovimento = new TipoMovimentoEntity();

        public TipoMovimentoEntity TipoMovimento
        {
            get { return tipoMovimento; }
            set { tipoMovimento = value; }
        }

        public IList<TipoMovimentoEntity> ListarTipoMovimentoTodosEntrada()
        {
            IList<TipoMovimentoEntity> retorno = this.Service<ITipoMovimentoService>().ListarTipoMovimentoTodosEntrada();
            return retorno;
        }

        public IList<TipoMovimentoEntity> ListarTipoMovimento(TipoMovimentoAgrupamentoEntity tipoMovimentoAgrupamento)
        {
            IList<TipoMovimentoEntity> retorno = this.Service<ITipoMovimentoService>().ListarTipoMovimento(tipoMovimentoAgrupamento);
            return retorno;
        }

        public TipoMovimentoEntity ExibirTipoMovimentoEntrada(int iTipoMovimento_ID)
        {
            TipoMovimentoEntity lObjRetorno = this.Service<ITipoMovimentoService>().ListarTipoMovimentoEntrada(iTipoMovimento_ID);
            return lObjRetorno;
        }

        public List<SubTipoMovimentoEntity> ListarSubTipoMovimento()
        {
            List<SubTipoMovimentoEntity> lObjRetorno = this.Service<ITipoMovimentoService>().ListarSubTipoMovimento();
            return lObjRetorno;
        }

        public SubTipoMovimentoEntity ListarInserirSubTipoMovimento(SubTipoMovimentoEntity objSubTipo)
        {
            SubTipoMovimentoEntity lObjRetorno = this.Service<ITipoMovimentoService>().ListarInserirSubTipoMovimento(objSubTipo);
            return lObjRetorno;
        }

       

        public List<TipoMovimentoEntity> ListarTipoMovimentoAtivoNl()
        {
            List<TipoMovimentoEntity> lObjRetorno = this.Service<ITipoMovimentoService>().ListarTipoMovimentoAtivoNl();
            return lObjRetorno;
        }
        

        public IList<TipoMovimentoEntity> RetirarTipoMovimentoEntrada(int iTipoMovimento_ID)
        {
            IList<TipoMovimentoEntity> lObjRetorno = this.Service<ITipoMovimentoService>().RetirarTipoMovimentoEntrada(iTipoMovimento_ID);
            return lObjRetorno;
        }

        public IList<TipoMovimentoEntity> Listar()
        {
            IList<TipoMovimentoEntity> lstRetorno = null;
                
            lstRetorno = this.Service<ITipoMovimentoService>().Listar();

            return lstRetorno;
        }

        public TipoMovimentoEntity Recupera(int Id)
        {
            TipoMovimentoEntity tipoMovimento;

            tipoMovimento = this.Service<ITipoMovimentoService>().Recupera(Id);

            return tipoMovimento;
        }

        public bool Salvar(TipoMovimentoEntity TipoMovimento)
        {
            this.Service<ITipoMovimentoService>().Entity = TipoMovimento;
            if (this.Consistido)
            {
                this.Service<ITipoMovimentoService>().Salvar();
            }
            return this.Consistido;
        }
    }
}
