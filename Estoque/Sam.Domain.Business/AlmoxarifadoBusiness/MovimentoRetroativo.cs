using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;

namespace Sam.Domain.Business
{
    public abstract class MovimentoRetroativo : BaseBusiness
    {
        protected MovimentoItemEntity movimento = null;
        protected IList<MovimentoItemEntity> ListaMovimento { get; set; }
        protected IList<MovimentoItemEntity> ListaMovimentoSIAFEM { get; set; }
        protected IList<MovimentoItemEntity> ListaMovimentoLote { get; set; }

        public MovimentoRetroativo(MovimentoItemEntity movimento)
        {
            //this.movimento = movimento;

            MovimentoItemEntity item = new MovimentoItemEntity();
            MovimentoEntity movimentos = new MovimentoEntity();

            item.Almoxarifado = movimento.Almoxarifado;
            UGEEntity uge = new UGEEntity(movimento.UGE.Id);
            item.UGE = uge;

            SubItemMaterialEntity subMaterial = new SubItemMaterialEntity
            {
                Id = movimento.SubItemMaterial.Id,
                Codigo = movimento.SubItemMaterial.Codigo
            };

            item.SubItemMaterial = subMaterial;
            item.UGE = uge;

            movimentos.Almoxarifado = (movimento.Almoxarifado == null ? movimento.Movimento.Almoxarifado : movimento.Almoxarifado);
            movimentos.AnoMesReferencia = movimento.Movimento.AnoMesReferencia;
            movimentos.DataMovimento = movimento.DataMovimento;
            movimentos.UGE = uge;
            item.Movimento = movimentos;
            item.Almoxarifado = movimentos.Almoxarifado;

            item.DataMovimento = movimento.DataMovimento;
            this.movimento = item;
        }

        public abstract void setListaMovimento(IMovimentoService service);
        public abstract List<MovimentoItemEntity> setListaMovimentoSIAFEM(IMovimentoService service);
        public abstract void setListaMovimentoLote(IMovimentoService service, MovimentoItemEntity movItem);
        public abstract void Recalculo(IMovimentoService service, int indice);
        public abstract Tuple<IList<MovimentoItemEntity>, bool> RecalculoSIAFEM(IMovimentoService service, int indice, MovimentoItemEntity movItem, string TipoMov);
        public abstract IList<MovimentoItemEntity> listaMovimentoItemErro { set; get; }
        public abstract IList<MovimentoItemEntity> listaPendencias { set; get; }
        public abstract bool Correcao { set; get; }
        public abstract bool gerarRelatorio { set; get; }
        public abstract void Corrigir(IMovimentoService service, int indice);
        public abstract void gerarPendencias(IMovimentoService service, IList<MovimentoItemEntity> listaPendencias, int indice);
        public abstract Boolean Estorno { get; set; }
    }
}
