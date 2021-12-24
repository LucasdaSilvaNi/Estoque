using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using Sam.Common.Util;
using System.Data.SqlClient;

namespace Sam.Domain.Business
{
    public class ReservaMaterialBusiness : BaseBusiness
    {

        private ReservaMaterialEntity entity = new ReservaMaterialEntity();

        public ReservaMaterialEntity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

        public bool Salvar()
        {
            this.Service<IReservaMaterialService>().Entity = this.Entity;
            this.ConsistirReserva();
            if (this.Consistido)
            {
                this.Service<IReservaMaterialService>().Salvar();
            }
            return this.Consistido;
        }

        public IList<ReservaMaterialEntity> Listar()
        {
            this.Service<IReservaMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ReservaMaterialEntity> retorno = this.Service<IReservaMaterialService>().Listar();
            this.TotalRegistros = this.Service<IReservaMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<ReservaMaterialEntity> ListarOrgaosTodosCod()
        {
            IList<ReservaMaterialEntity> retorno = this.Service<IReservaMaterialService>().ListarTodosCod();
            return retorno;
        }

        public IList<ReservaMaterialEntity> Listar(int almoxarifado)
        {
            this.Service<IReservaMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<ReservaMaterialEntity> retorno = this.Service<IReservaMaterialService>().Listar(almoxarifado);
            this.TotalRegistros = this.Service<IReservaMaterialService>().TotalRegistros();
            return retorno;
        }

        public ReservaMaterialEntity Ler()
        {
            this.Service<IReservaMaterialService>().SkipRegistros = this.SkipRegistros;
            ReservaMaterialEntity retorno = this.Service<IReservaMaterialService>().LerRegistro();

            return retorno;
        }

        public IList<ReservaMaterialEntity> ListarReservaPorPeriodoAlmoxSubItem(int almoxarifado, long subItemMaterialCodigo, DateTime[] periodo) 
        {
            return this.Service<IReservaMaterialService>().ListarReservaPorPeriodoAlmoxSubItem(almoxarifado, subItemMaterialCodigo, periodo);
        }

        public IList<ReservaMaterialEntity> Imprimir(int almoxarifado)
        {
            IList<ReservaMaterialEntity> retorno = this.Service<IReservaMaterialService>().Imprimir(almoxarifado);
            return retorno;
        }

        public bool Excluir()
        {
            this.Service<IReservaMaterialService>().Entity = this.Entity;
            if (this.Consistido)
            {
                try
                {
                    this.Service<IReservaMaterialService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirExclusao()
        {
            this.Service<IReservaMaterialService>().Entity = this.Entity;
            if (!this.Service<IReservaMaterialService>().PodeExcluir())
                this.ListaErro.Add("Não é possível excluir este item, existem registros associados a ele.");
        }

        public void ConsistirReserva()
        {

            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<ReservaMaterialEntity>(ref this.entity);

            var saldoAtual  = new SaldoSubItemBusiness().ConsultarSaldoSubItem(entity.Almoxarifado, entity.Uge, entity.SubItemMaterial).SaldoQtde.Value;
            if (this.entity.Quantidade > (saldoAtual))
            {
                this.ListaErro.Add("Saldo não suficiente. O saldo atual é de: " + saldoAtual);
            }

            if (this.Entity.Quantidade == 0)
                this.ListaErro.Add("É obrigatório informar a quantidade.");

            if (string.IsNullOrEmpty(this.Entity.Uge.Id.ToString()))
                this.ListaErro.Add("É obrigatório informar a UGE.");

            if (string.IsNullOrEmpty(this.Entity.SubItemMaterial.Id.ToString()))
                this.ListaErro.Add("É obrigatório informar o SubItem.");

        }
    }

}
