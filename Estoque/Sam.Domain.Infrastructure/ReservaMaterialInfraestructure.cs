using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public class ReservaMaterialInfraestructure : BaseInfraestructure, IReservaMaterialService
    {
        private ReservaMaterialEntity entity = new ReservaMaterialEntity();

        public ReservaMaterialEntity Entity
        {
            get { return this.entity; }
            set { this.entity = value; }
        }

        public IList<UGEEntity> Listar(int subItem, int almoxarifado)
        {
            IList<UGEEntity> resultado = (from a in this.Db.TB_SALDO_SUBITEMs
                                          join b in this.Db.TB_UGEs on a.TB_UGE_ID equals b.TB_UGE_ID
                                          where (a.TB_SUBITEM_MATERIAL_ID == subItem)
                                          where (a.TB_ALMOXARIFADO_ID == almoxarifado)
                                          select new UGEEntity
                                          {
                                              Codigo = a.TB_UGE_ID
                                          })
                                         .ToList();
            return resultado;
        }

        public IList<ReservaMaterialEntity> Listar(int almoxarifado)
        {

            IList<ReservaMaterialEntity> resultado = (from s  in Db.TB_SALDO_SUBITEMs
                                                      join a  in Db.TB_RESERVA_MATERIALs on new {s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID} equals new {a.TB_SUBITEM_MATERIAL_ID, a.TB_UGE_ID, a.TB_ALMOXARIFADO_ID} 
                                                      join b in Db.TB_UGEs on a.TB_UGE_ID equals b.TB_UGE_ID
                                                      join d in Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals d.TB_SUBITEM_MATERIAL_ID
                                                      join e in Db.TB_UNIDADE_FORNECIMENTOs on d.TB_UNIDADE_FORNECIMENTO_ID equals e.TB_UNIDADE_FORNECIMENTO_ID
                                                      join f in Db.TB_ITEM_SUBITEM_MATERIALs on d.TB_SUBITEM_MATERIAL_ID equals f.TB_SUBITEM_MATERIAL_ID
                                                      join g in Db.TB_ITEM_MATERIALs on f.TB_ITEM_MATERIAL_ID equals g.TB_ITEM_MATERIAL_ID
                                                      where (a.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      select new ReservaMaterialEntity
                                                      {
                                                          Data = a.TB_RESERVA_MATERIAL_DATA,
                                                          Obs = a.TB_RESERVA_MATERIAL_OBS,
                                                          Quantidade = a.TB_RESERVA_MATERIAL_QUANT.Value,
                                                          Uge = new UGEEntity { 
                                                              Id = b.TB_UGE_ID, 
                                                              Codigo = b.TB_UGE_CODIGO,
                                                              Descricao = string.Format("{0} - {1} - Saldo: {2}", s.TB_UGE.TB_UGE_ID.ToString().PadLeft(6, '0'), s.TB_UGE.TB_UGE_DESCRICAO, s.TB_SALDO_SUBITEM_SALDO_QTDE - a.TB_RESERVA_MATERIAL_QUANT)
                                                          },
                                                          SubItemMaterial = new SubItemMaterialEntity { Id = d.TB_SUBITEM_MATERIAL_ID, Codigo = d.TB_SUBITEM_MATERIAL_CODIGO, Descricao = d.TB_SUBITEM_MATERIAL_DESCRICAO },
                                                          UnidadeFornecimento = new UnidadeFornecimentoEntity { Descricao = e.TB_UNIDADE_FORNECIMENTO_DESCRICAO },
                                                          ItemMaterial = new ItemMaterialEntity { Id = g.TB_ITEM_MATERIAL_ID, Codigo = g.TB_ITEM_MATERIAL_CODIGO, Descricao = g.TB_ITEM_MATERIAL_DESCRICAO },
                                                          Id = a.TB_RESERVA_MATERIAL_ID
                                                      })
                                         .ToList<ReservaMaterialEntity>();

            this.totalregistros = resultado.Count();


            //for (int i = 0; i < resultado.Count; i++)
            //{
            //    resultado[i].SubItemMaterial.Id = resultado[i].SubItemMaterial.Id + "<font color=\"Red\">" + resultado[i].SubItemMaterial.Descricao + "</font>";
            //}

            resultado = resultado.Skip(this.SkipRegistros).Take(this.RegistrosPagina).ToList<ReservaMaterialEntity>();

            return resultado;
        }

        public IList<ReservaMaterialEntity> Imprimir(int almoxarifado)
        {
            throw new NotImplementedException();
        }

        public ReservaMaterialEntity Select(int id)
        {
            throw new NotImplementedException();
        }

        public IList<ReservaMaterialEntity> Listar()
        {
            throw new NotImplementedException();
        }

        public IList<ReservaMaterialEntity> ListarReservaPorPeriodoAlmoxSubItem(int almoxarifado, long SubItemMaterialCodigo, DateTime[] periodo) 
        {
            IList<ReservaMaterialEntity> resultado = (from a in this.Db.TB_RESERVA_MATERIALs
                                                      join b in Db.TB_UGEs on a.TB_UGE_ID equals b.TB_UGE_ID
                                                      join c in Db.TB_SUBITEM_MATERIAL_ALMOXes on a.TB_SUBITEM_MATERIAL_ID equals c.TB_SUBITEM_MATERIAL_ID
                                                      join d in Db.TB_SUBITEM_MATERIALs on c.TB_SUBITEM_MATERIAL_ID equals d.TB_SUBITEM_MATERIAL_ID
                                                      join e in Db.TB_UNIDADE_FORNECIMENTOs on d.TB_UNIDADE_FORNECIMENTO_ID equals e.TB_UNIDADE_FORNECIMENTO_ID
                                                      join f in Db.TB_ITEM_SUBITEM_MATERIALs on d.TB_SUBITEM_MATERIAL_ID equals f.TB_SUBITEM_MATERIAL_ID
                                                      join g in Db.TB_ITEM_MATERIALs on f.TB_ITEM_MATERIAL_ID equals g.TB_ITEM_MATERIAL_ID
                                                      where (a.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      where (a.TB_ALMOXARIFADO_ID == c.TB_ALMOXARIFADO_ID)
                                                      where (d.TB_SUBITEM_MATERIAL_CODIGO == SubItemMaterialCodigo)
                                                      where (a.TB_RESERVA_MATERIAL_DATA >= periodo[0] && a.TB_RESERVA_MATERIAL_DATA < periodo[1])
                                                      select new ReservaMaterialEntity
                                                      {
                                                          Data = a.TB_RESERVA_MATERIAL_DATA,
                                                          Obs = a.TB_RESERVA_MATERIAL_OBS,
                                                          Quantidade = a.TB_RESERVA_MATERIAL_QUANT.Value,
                                                          SubItemMaterialAlmox = new SubItemMaterialAlmoxEntity { Id = c.TB_SUBITEM_MATERIAL_ALMOX_ID },
                                                          Uge = new UGEEntity { Id = b.TB_UGE_ID, Descricao = b.TB_UGE_DESCRICAO },
                                                          SubItemMaterial = new SubItemMaterialEntity { Id = d.TB_SUBITEM_MATERIAL_ID, Codigo = d.TB_SUBITEM_MATERIAL_CODIGO, Descricao = d.TB_SUBITEM_MATERIAL_DESCRICAO },
                                                          UnidadeFornecimento = new UnidadeFornecimentoEntity { Descricao = e.TB_UNIDADE_FORNECIMENTO_DESCRICAO },
                                                          ItemMaterial = new ItemMaterialEntity { Id = g.TB_ITEM_MATERIAL_ID, Codigo = g.TB_ITEM_MATERIAL_CODIGO, Descricao = g.TB_ITEM_MATERIAL_DESCRICAO },
                                                          Id = a.TB_RESERVA_MATERIAL_ID
                                                      })
                             .ToList<ReservaMaterialEntity>();
            return resultado;
        }

        public IList<ReservaMaterialEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public ReservaMaterialEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<ReservaMaterialEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            TB_RESERVA_MATERIAL tbReserva = this.Db.TB_RESERVA_MATERIALs
                .Where(a => a.TB_RESERVA_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            this.Db.TB_RESERVA_MATERIALs.DeleteOnSubmit(tbReserva);
            this.Db.SubmitChanges();
        }

        public void Salvar()
        {
            TB_RESERVA_MATERIAL tbReserva = new TB_RESERVA_MATERIAL();

            if (this.Entity.Id.HasValue)
                tbReserva = this.Db.TB_RESERVA_MATERIALs.Where(a => a.TB_RESERVA_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
            else
                Db.TB_RESERVA_MATERIALs.InsertOnSubmit(tbReserva);

            tbReserva.TB_SUBITEM_MATERIAL_ID = this.Entity.SubItemMaterial.Id ?? 0;
            tbReserva.TB_UGE_ID = this.Entity.Uge.Id ?? 0;
            tbReserva.TB_ALMOXARIFADO_ID = this.Entity.Almoxarifado.Id ?? 0;

            tbReserva.TB_RESERVA_MATERIAL_DATA = DateTime.Now;
            tbReserva.TB_RESERVA_MATERIAL_OBS = this.Entity.Obs;
            tbReserva.TB_RESERVA_MATERIAL_QUANT = this.Entity.Quantidade;

            this.Db.SubmitChanges();

        }

        public bool PodeExcluir()
        {
            throw new NotImplementedException();
        }

        public bool ExisteCodigoInformado()
        {
            throw new NotImplementedException();
        }
    }
}
