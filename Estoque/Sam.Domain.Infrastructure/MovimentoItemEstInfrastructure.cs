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
    public partial class MovimentoItemEstInfrastructure : BaseInfraestructure, IMovimentoItemEstService
    {

        public MovimentoItemEstEntity Entity { get; set; }

        public MovimentoItemEstEntity LerRegistroItem(int MovimentoEstornoItemId) 
        {
            MovimentoItemEstEntity resultado = (from a in this.Db.TB_MOVIMENTO_ITEM_ESTs
                                                join si in this.Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                                where a.TB_MOVIMENTO_ITEM_ESTORNO_ID == MovimentoEstornoItemId
                                                select new MovimentoItemEstEntity
                                                    {
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        MovimentoEst = new MovimentoEstEntity(a.TB_MOVIMENTO_ESTORNO_ID),
                                                        MovimentoItem = new MovimentoItemEntity(a.TB_MOVIMENTO_ITEM_ID),
                                                        Id = a.TB_MOVIMENTO_ITEM_ESTORNO_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity
                                                        {
                                                            Id = si.TB_SUBITEM_MATERIAL_ID,
                                                            Codigo = si.TB_SUBITEM_MATERIAL_CODIGO,
                                                            CodigoFormatado = si.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            Descricao = si.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                            select new ItemMaterialEntity
                                                                            {
                                                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                            }
                                                                            ).FirstOrDefault(),
                                                            NaturezaDespesa = (from n in this.Db.TB_NATUREZA_DESPESAs
                                                                               where n.TB_NATUREZA_DESPESA_ID == si.TB_NATUREZA_DESPESA_ID
                                                                               select new NaturezaDespesaEntity
                                                                               {
                                                                                   Id = n.TB_NATUREZA_DESPESA_ID,
                                                                                   Codigo = n.TB_NATUREZA_DESPESA_CODIGO,
                                                                                   Descricao = n.TB_NATUREZA_DESPESA_DESCRICAO
                                                                               }).FirstOrDefault(),
                                                            UnidadeFornecimento = (from un in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                                                   where un.TB_UNIDADE_FORNECIMENTO_ID == si.TB_UNIDADE_FORNECIMENTO_ID
                                                                                   select new UnidadeFornecimentoEntity
                                                                                   {
                                                                                       Id = un.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                       Codigo = un.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                       Descricao = un.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                   }
                                                                                    ).FirstOrDefault()
                                                        }
                                                        ,
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).FirstOrDefault();
            return resultado;
        }

        public IList<MovimentoItemEstEntity> ListarPorMovimentoTodos(MovimentoEstEntity mov)
        {
            IList<MovimentoItemEstEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEM_ESTs
                                                    join si in this.Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                                    join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                    where (m.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                                       orderby si.TB_SUBITEM_MATERIAL_DESCRICAO
                                                    select new MovimentoItemEstEntity
                                                    {
                                                        Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                        MovimentoEst = new MovimentoEstEntity(a.TB_MOVIMENTO_ESTORNO_ID),
                                                        MovimentoItem = new MovimentoItemEntity(a.TB_MOVIMENTO_ITEM_ID),
                                                        Id = a.TB_MOVIMENTO_ITEM_ESTORNO_ID,
                                                        Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                        DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                        Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                        FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                        IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                        PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                        QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                        QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                        SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                        SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                        SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                        SubItemMaterial = new SubItemMaterialEntity
                                                        {
                                                            Id = si.TB_SUBITEM_MATERIAL_ID,
                                                            Codigo = si.TB_SUBITEM_MATERIAL_CODIGO,
                                                            CodigoFormatado = si.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                            Descricao = si.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                            ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                            join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                            where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                            select new ItemMaterialEntity
                                                                            {
                                                                                Id = i.TB_ITEM_MATERIAL_ID,
                                                                                Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                            }
                                                                            ).FirstOrDefault(),
                                                            NaturezaDespesa = (from n in this.Db.TB_NATUREZA_DESPESAs
                                                                               where n.TB_NATUREZA_DESPESA_ID == si.TB_NATUREZA_DESPESA_ID 
                                                                               select new NaturezaDespesaEntity 
                                                                               { 
                                                                                    Id = n.TB_NATUREZA_DESPESA_ID,
                                                                                    Codigo = n.TB_NATUREZA_DESPESA_CODIGO,
                                                                                    Descricao = n.TB_NATUREZA_DESPESA_DESCRICAO
                                                                               }).FirstOrDefault(),
                                                            UnidadeFornecimento = (from un in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                                                   where un.TB_UNIDADE_FORNECIMENTO_ID == si.TB_UNIDADE_FORNECIMENTO_ID
                                                                                   select new UnidadeFornecimentoEntity
                                                                                   {
                                                                                       Id = un.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                       Codigo = un.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                       Descricao = un.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                   }
                                                                                    ).FirstOrDefault()
                                                        }
                                                        ,
                                                        UGE = new UGEEntity(a.TB_UGE_ID),
                                                        ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                    }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTO_ITEM_ESTs
                                   join si in this.Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                   join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                   where (m.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ITEM_ESTORNO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEstEntity> ListarPorMovimento(MovimentoEstEntity mov)
        {
            IList<MovimentoItemEstEntity> resultado = (from a in this.Db.TB_MOVIMENTO_ITEM_ESTs
                                                       join si in this.Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                                       join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                                       where (m.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                                       orderby a.TB_MOVIMENTO_ITEM_ID
                                                       select new MovimentoItemEstEntity
                                                       {
                                                           Movimento = new MovimentoEntity(a.TB_MOVIMENTO_ID),
                                                           MovimentoEst = new MovimentoEstEntity(a.TB_MOVIMENTO_ESTORNO_ID),
                                                           MovimentoItem = new MovimentoItemEntity(a.TB_MOVIMENTO_ITEM_ID),
                                                           Id = a.TB_MOVIMENTO_ITEM_ESTORNO_ID,
                                                           Ativo = a.TB_MOVIMENTO_ITEM_ATIVO,
                                                           DataVencimentoLote = a.TB_MOVIMENTO_ITEM_LOTE_DATA_VENC,
                                                           Desd = a.TB_MOVIMENTO_ITEM_DESD,
                                                           FabricanteLote = a.TB_MOVIMENTO_ITEM_LOTE_FABR,
                                                           IdentificacaoLote = a.TB_MOVIMENTO_ITEM_LOTE_IDENT,
                                                           PrecoUnit = a.TB_MOVIMENTO_ITEM_PRECO_UNIT,
                                                           QtdeLiq = a.TB_MOVIMENTO_ITEM_QTDE_LIQ,
                                                           QtdeMov = a.TB_MOVIMENTO_ITEM_QTDE_MOV,
                                                           SaldoQtde = a.TB_MOVIMENTO_ITEM_SALDO_QTDE,
                                                           SaldoQtdeLote = a.TB_MOVIMENTO_ITEM_SALDO_QTDE_LOTE,
                                                           SaldoValor = a.TB_MOVIMENTO_ITEM_SALDO_VALOR,
                                                           SubItemMaterial = new SubItemMaterialEntity
                                                           {
                                                               Id = si.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = si.TB_SUBITEM_MATERIAL_CODIGO,
                                                               CodigoFormatado = si.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                               Descricao = si.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               ItemMaterial = (from i in this.Db.TB_ITEM_MATERIALs
                                                                               join isi in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                                                   on i.TB_ITEM_MATERIAL_ID equals isi.TB_ITEM_MATERIAL_ID
                                                                               where a.TB_SUBITEM_MATERIAL_ID == isi.TB_SUBITEM_MATERIAL_ID
                                                                               select new ItemMaterialEntity
                                                                               {
                                                                                   Id = i.TB_ITEM_MATERIAL_ID,
                                                                                   Codigo = i.TB_ITEM_MATERIAL_CODIGO,
                                                                                   CodigoFormatado = i.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                                   Descricao = i.TB_ITEM_MATERIAL_DESCRICAO
                                                                               }
                                                                               ).FirstOrDefault(),
                                                               NaturezaDespesa = (from n in this.Db.TB_NATUREZA_DESPESAs
                                                                                  where n.TB_NATUREZA_DESPESA_ID == si.TB_NATUREZA_DESPESA_ID
                                                                                  select new NaturezaDespesaEntity
                                                                                  {
                                                                                      Id = n.TB_NATUREZA_DESPESA_ID,
                                                                                      Codigo = n.TB_NATUREZA_DESPESA_CODIGO,
                                                                                      Descricao = n.TB_NATUREZA_DESPESA_DESCRICAO
                                                                                  }).FirstOrDefault(),
                                                               UnidadeFornecimento = (from un in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                                                      where un.TB_UNIDADE_FORNECIMENTO_ID == si.TB_UNIDADE_FORNECIMENTO_ID
                                                                                      select new UnidadeFornecimentoEntity
                                                                                      {
                                                                                          Id = un.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                          Codigo = un.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                          Descricao = un.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                      }
                                                                                       ).FirstOrDefault()
                                                           }
                                                           ,
                                                           UGE = new UGEEntity(a.TB_UGE_ID),
                                                           ValorMov = a.TB_MOVIMENTO_ITEM_VALOR_MOV
                                                       }).ToList();

            this.totalregistros = (from a in this.Db.TB_MOVIMENTO_ITEM_ESTs
                                   join si in this.Db.TB_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                   join m in this.Db.TB_MOVIMENTOs on a.TB_MOVIMENTO_ID equals m.TB_MOVIMENTO_ID
                                   where (m.TB_MOVIMENTO_NUMERO_DOCUMENTO == mov.NumeroDocumento)
                                   select new
                                   {
                                       Id = a.TB_MOVIMENTO_ITEM_ESTORNO_ID,
                                   }).Count();
            return resultado;
        }

        public IList<MovimentoItemEstEntity> Listar()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEstEntity> ListarTodosCod()
        {
            throw new NotImplementedException();
        }

        public MovimentoItemEstEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        public IList<MovimentoItemEstEntity> Imprimir()
        {
            throw new NotImplementedException();
        }

        public void Excluir()
        {
            throw new NotImplementedException();
        }

        public void Salvar()
        {
            throw new NotImplementedException();
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
