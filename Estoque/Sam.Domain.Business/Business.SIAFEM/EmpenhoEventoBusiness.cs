using System;
using System.Collections;
using System.Collections.Generic;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;




namespace Sam.Domain.Business
{
    public partial class EmpenhoEventoBusiness : BaseBusiness
    {
        private EmpenhoEventoEntity entity = new EmpenhoEventoEntity();
        public EmpenhoEventoEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public bool Salvar()
        {
            this.Service<IEmpenhoEventoService>().Entity = this.Entity;
            this.ConsistirEmpenhoEvento();
            if (this.Consistido)
            {
                this.Service<IEmpenhoEventoService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<EmpenhoEventoEntity> ListarEventos()
        {
            IList<EmpenhoEventoEntity> lstRetorno = null;

            this.Service<IEmpenhoEventoService>().SkipRegistros = this.SkipRegistros;
            lstRetorno = this.Service<IEmpenhoEventoService>().Listar();
            this.TotalRegistros = this.Service<IEmpenhoEventoService>().TotalRegistros();

            return lstRetorno;
        }

        public IList<EmpenhoEventoEntity> Imprimir()
        {
            IList<EmpenhoEventoEntity> lstRetorno = null;

            lstRetorno = this.Service<IEmpenhoEventoService>().Imprimir();

            return lstRetorno;
        }

        public bool Excluir()
        {
            this.Service<IEmpenhoEventoService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IEmpenhoEventoService>().Excluir();
                }
                catch (Exception ex)
                {
                    new LogErro().GravarLogErro(ex);
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirEmpenhoEvento()
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<EmpenhoEventoEntity>(ref this.entity);

            //Fazer validação
            if (this.Entity.Codigo <= 0)
                this.ListaErro.Add("É obrigatório informar código de evento, obrigatoriamente maior que 0 (zero).");

            if (string.IsNullOrEmpty(this.Entity.Descricao.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar descrição.");

            if (string.IsNullOrEmpty(this.Entity.AnoBase.Trim().ToString()))
                this.ListaErro.Add("É obrigatório informar Ano-Base do evento.");

            if (this.Service<IEmpenhoEventoService>().ExisteCodigoInformado() && !this.Entity.Id.HasValue)
                this.ListaErro.Add("Código já existente.");

            if (!(this.Entity.TipoMaterialAssociado == 0 || this.Entity.TipoMaterialAssociado == 1))
                this.ListaErro.Add("É obrigatório escolher Tipo de Material associado.");

            if (this.Entity.TipoMovimentoAssociado.IsNull() || (this.Entity.TipoMovimentoAssociado.IsNotNull() && this.Entity.TipoMovimentoAssociado.Id == 0))
                this.ListaErro.Add("É obrigatório associar um tipo de movimentação.");
        }
    }
}
