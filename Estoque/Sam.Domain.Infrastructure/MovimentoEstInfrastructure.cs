using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using Sam.ServiceInfraestructure;
using System.Configuration;
using System.Collections;
using Sam.Domain.Entity;


namespace Sam.Domain.Infrastructure
{
    public partial class MovimentoEstInfrastructure : BaseInfraestructure, IMovimentoEstService
    {

        public MovimentoEstEntity Entity { get; set; }



        public IList<MovimentoEstEntity> ListarTodosCod(int MovimentoId)
        {
            throw new NotImplementedException();
        }


        public IList<MovimentoEstEntity> Listar()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoEstEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public MovimentoEstEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoEstEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            TB_MOVIMENTO_EST mov = new TB_MOVIMENTO_EST();
            EntitySet<TB_MOVIMENTO_ITEM_EST> item = new EntitySet<TB_MOVIMENTO_ITEM_EST>();

            if (this.Entity.Id.HasValue)
                mov = this.Db.TB_MOVIMENTO_ESTs.Where(a => a.TB_MOVIMENTO_ESTORNO_ID == this.Entity.Id.Value).FirstOrDefault();
            else
                this.Db.TB_MOVIMENTO_ESTs.InsertOnSubmit(mov);

            foreach (MovimentoItemEntity view in this.Entity.Movimento.MovimentoItem)
            {
                TB_MOVIMENTO_ITEM_EST itemAdd = new TB_MOVIMENTO_ITEM_EST();
                itemAdd.TB_MOVIMENTO_ESTORNO_ID = 0;
                itemAdd.TB_UGE_ID = view.UGE.Id;
                itemAdd.TB_MOVIMENTO_ID = view.Movimento.Id.Value;
                itemAdd.TB_MOVIMENTO_ITEM_ID = view.Id.Value;
                itemAdd.TB_MOVIMENTO_ITEM_ATIVO = view.Ativo;
                itemAdd.TB_MOVIMENTO_ITEM_DESD = view.Desd;
                itemAdd.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC = view.DataVencimentoLote.Value.Year < 1900 ? null : view.DataVencimentoLote;
                itemAdd.TB_MOVIMENTO_ITEM_LOTE_FABR = view.FabricanteLote;
                itemAdd.TB_MOVIMENTO_ITEM_LOTE_IDENT = view.IdentificacaoLote;
                itemAdd.TB_MOVIMENTO_ITEM_PRECO_UNIT = view.PrecoUnit;
                itemAdd.TB_MOVIMENTO_ITEM_QTDE_LIQ = view.QtdeLiq;
                itemAdd.TB_MOVIMENTO_ITEM_QTDE_MOV = view.QtdeMov;
                itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE = view.SaldoQtde;
                itemAdd.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE = view.SaldoQtdeLote;
                itemAdd.TB_MOVIMENTO_ITEM_SALDO_VALOR = view.SaldoValor;
                itemAdd.TB_MOVIMENTO_ITEM_VALOR_MOV = view.ValorMov;
                itemAdd.TB_SUBITEM_MATERIAL_ID = (int)view.SubItemMaterial.Id;
                item.Add(itemAdd);
            }

            mov.TB_MOVIMENTO_ITEM_ESTs = item;
            mov.TB_ALMOXARIFADO_ID = this.Entity.Movimento.Almoxarifado.Id.Value;

            mov.TB_MOVIMENTO_ANO_MES_REFERENCIA = this.Entity.Movimento.AnoMesReferencia;
            mov.TB_MOVIMENTO_DATA_DOCUMENTO = this.Entity.Movimento.DataDocumento.Value.Year < 1900 ? new DateTime(1900, 1, 1) : this.Entity.Movimento.DataDocumento.Value;
            mov.TB_MOVIMENTO_DATA_MOVIMENTO = DateTime.Now;
            mov.TB_MOVIMENTO_EMPENHO = this.Entity.Movimento.Empenho;
            mov.TB_MOVIMENTO_FONTE_RECURSO = this.Entity.Movimento.FonteRecurso;
            mov.TB_MOVIMENTO_GERADOR_DESCRICAO  = this.Entity.GeradorDescricao;
            mov.TB_MOVIMENTO_INSTRUCOES = this.Entity.Movimento.Instrucoes;
            mov.TB_MOVIMENTO_NUMERO_DOCUMENTO = this.Entity.Movimento.NumeroDocumento;
            mov.TB_MOVIMENTO_OBSERVACOES = this.Entity.Movimento.Observacoes;
            mov.TB_MOVIMENTO_VALOR_DOCUMENTO = this.Entity.Movimento.ValorDocumento.Value;

            if (this.Entity.Movimento != null)
            {
                mov.TB_MOVIMENTO_ID = this.Entity.Movimento.Id.Value;
            }

            if (this.Entity.Movimento.UGE != null)
            {
                mov.TB_UGE_ID = this.Entity.Movimento.UGE.Id.Value;
            }

            if (this.Entity.Movimento.MovimAlmoxOrigemDestino != null)
            {
                if (this.Entity.Movimento.MovimAlmoxOrigemDestino.Id.HasValue)
                    mov.TB_MOVIMENTO_ALMOX_ID_ORIGEM_DESTINO = this.Entity.Movimento.MovimAlmoxOrigemDestino.Id;
            }

            if (this.Entity.Movimento.Divisao != null)
            {
                if(this.Entity.Movimento.Divisao.Id != null)
                    mov.TB_DIVISAO_ID = this.Entity.Movimento.Divisao.Id;
            }

            if (this.Entity.Movimento.Fornecedor != null)
            {
                if(this.Entity.Movimento.Fornecedor.Id == null)
                    mov.TB_FORNECEDOR_ID = this.Entity.Movimento.Fornecedor.Id;
            }

            mov.TB_TIPO_MOVIMENTO_ID = this.Entity.Movimento.TipoMovimento.Id;
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
