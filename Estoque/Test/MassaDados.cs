using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sam.Infrastructure;
using Sam.Entity;
using Sam.Domain.Entity;
using Sam.Common.Util;

namespace Test
{
    public class MassaDados
    {
        public int MovimentoId { get; set; }
        public int SubItemId { get; set; }
        public int UGEID { get; set; }
        public int AlmoxId { get; set; }
        public int DivisaoId { get; set; }
        public int FornecedorId { get; set; }
        public int Iteracao { get; set; }
        public bool ComLote { get; set; }
        public GeralEnum.TipoMovimento TipoMovimento { get; set; }
        public int Qtd { get; set; }
        public decimal Valor { get; set; }
        public int MultQtd { get; set; }
        public decimal MultValor { get; set; }
        public DateTime DataMovimento { get; set; }
        public int QtdItens { get; set; }
        public string Fabricante { get; set; }
        public string Identificador { get; set; }
        public DateTime DataVencimento { get; set; }

        public MovimentoEntity GetMovimentoMassa(int iteracao)
        {
            MovimentoEntity movimento = new MovimentoEntity();
            movimento.Almoxarifado = new AlmoxarifadoEntity(AlmoxId);
            //movimento.Divisao = new DivisaoEntity(DivisaoId);
            movimento.TipoMovimento = new TipoMovimentoEntity((int)TipoMovimento);
            movimento.UGE = new UGEEntity(UGEID);
            movimento.Fornecedor = new FornecedorEntity(FornecedorId);
            //movimento.EmpenhoEvento = new EmpenhoEventoEntity();
            //movimento.EmpenhoLicitacao = new EmpenhoLicitacaoEntity();
            
            //movimento.MovimAlmoxOrigemDestino = new AlmoxarifadoEntity();
            //movimento.PTRes = new PTResEntity();
            

            //movimento.Almoxarifado.Id = 130;
            movimento.AnoMesReferencia = "201209";
            movimento.Ativo = true;
            movimento.CodigoDescricao = "cod";
            movimento.CodigoFormatado = "";
            movimento.DataDocumento = DataMovimento;
            movimento.DataMovimento = DataMovimento;
            movimento.DataOperacao = DateTime.Now;
            movimento.Empenho = "Avulsa";
            movimento.ValorDocumento = 0;
            movimento.FonteRecurso = "n";
            movimento.GeradorDescricao = "n";
            movimento.IdLogin = 50;
            movimento.Instrucoes = "n";
            movimento.NaturezaDespesaEmpenho = "n";
            //movimento.NlLiquidacao = "n";
            movimento.NumeroDocumento = movimento.AnoMesReferencia + new Random().Next(600000, 699999).ToString();
            movimento.Observacoes = "n";


            IList<MovimentoItemEntity> ListMovimentoItem = new List<MovimentoItemEntity>();
            for (int i = 1; i <= QtdItens; i++)
            {
                ListMovimentoItem.Add(GetMovimentoItemMassa(i, movimento, iteracao));                
            }

            movimento.MovimentoItem = ListMovimentoItem;
            movimento.ValorDocumento = ListMovimentoItem.Sum(a => a.ValorMov);            

            return movimento;
        }

        public MovimentoItemEntity GetMovimentoItemMassa(int seq, MovimentoEntity movimento, int iteracao)
        {
            MovimentoItemEntity movimentoItem = new MovimentoItemEntity();

            movimentoItem.UGE = new UGEEntity(UGEID);
            movimentoItem.QtdeMov = Qtd * (MultQtd);
            movimentoItem.ValorMov = Valor * (MultValor);

            //if (iteracao == 1)
            //{
            //    if (seq == 1)
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(27134);//qualquer um item
            //        movimentoItem.QtdeMov = 100;
            //        movimentoItem.ValorMov = 40;
            //    }
            //    else
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(26820);//qualquer um item
            //        movimentoItem.QtdeMov = 100;
            //        movimentoItem.ValorMov = 10;
            //    }
            //}
            //if (iteracao == 3)
            //{
            //    if (seq == 1)
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(27134);//qualquer um item
            //        movimentoItem.QtdeMov = 10;
            //        movimentoItem.ValorMov = 4;
            //    }
            //    else
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(26820);//qualquer um item
            //        movimentoItem.QtdeMov = 10;
            //        movimentoItem.ValorMov = 1;
            //    }
            //}
            //if (iteracao == 2)
            //{
            //    if (seq == 1)
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(27134);//qualquer um item
            //        movimentoItem.QtdeMov = 10;
            //        movimentoItem.ValorMov = 4;
            //    }
            //    else
            //    {
            //        movimentoItem.SubItemMaterial = new SubItemMaterialEntity(26820);//qualquer um item
            //        movimentoItem.QtdeMov = 5;
            //        movimentoItem.ValorMov = 0;
            //    }
            //}


            movimentoItem.ItemMaterial = new ItemMaterialEntity(27134);//qualquer um item
            movimentoItem.SubItemMaterial = new SubItemMaterialEntity(27134);//qualquer um item
            //movimentoItem.SubItemMaterial.ItemMaterial = new ItemMaterialEntity(27134);//qualquer um item
            

            //movimentoItem.FabricanteLote = Fabricante;
            //movimentoItem.IdentificacaoLote = Identificador;
            //movimentoItem.DataVencimentoLote = DataVencimento;

            //if (ComLote)
            //    movimentoItem.FabricanteLote = seq.ToString();

            //if (ComLote)
            //    movimentoItem.FabricanteLote = new Random().Next(1, 99999999).ToString();

            //movimentoItem.IdentificacaoLote = "LOTE B";
            movimentoItem.Ativo = true;

            //Adiciona o movimento dentro do movimento Item.
            movimentoItem.Movimento = movimento;

            return movimentoItem;
        }
    }
}
