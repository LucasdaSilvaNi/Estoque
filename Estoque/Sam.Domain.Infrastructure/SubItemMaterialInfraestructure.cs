using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Linq.Expressions;
using Sam.Common.Util;
using System.Transactions;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;



namespace Sam.Domain.Infrastructure
{
    public class SubItemMaterialInfraestructure : BaseInfraestructure, ISubItemMaterialService
    {
        public int totalregistros
        {
            get;
            set;
        }

        public int TotalRegistros()
        {
            return totalregistros;
        }

        public SubItemMaterialEntity Entity { get; set; }

        public IList<SubItemMaterialEntity> Listar()
        {
            return this.Listar(int.MinValue, int.MinValue, null);
        }

        /// <summary>
        /// OK
        /// Retorna o Subitem material pelo id
        /// </summary>
        /// <param name="_id">id do Subitem</param>
        /// <returns></returns>
        public SubItemMaterialEntity Select(int _id)
        {
            IQueryable<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                           join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                           where (a.TB_SUBITEM_MATERIAL_ID == _id)
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               CodigoBarras = a.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                               //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                                               Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                               IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               IsDecimal = (bool)a.TB_SUBITEM_MATERIAL_DECIMOS,
                                                               IsFracionado = (bool)a.TB_SUBITEM_MATERIAL_FRACIONA,
                                                               // IsLote = a.TB_SUBITEM_MATERIAL_LOTE,
                                                               NaturezaDespesa = (new NaturezaDespesaEntity(a.TB_NATUREZA_DESPESA_ID)
                                                               {
                                                                   Codigo = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                   Descricao = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                               }),
                                                               ItemMaterial = (new ItemMaterialEntity(b.TB_ITEM_MATERIAL_ID)
                                                               {
                                                                   Codigo = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                   Descricao = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                               }),
                                                               UnidadeFornecimento = (from un in this.Db.TB_UNIDADE_FORNECIMENTOs
                                                                                      where un.TB_UNIDADE_FORNECIMENTO_ID == a.TB_UNIDADE_FORNECIMENTO_ID
                                                                                      select new UnidadeFornecimentoEntity
                                                                                      {
                                                                                          Id = un.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                          Codigo = un.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                                          Descricao = un.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                                      }
                                                                                       ).FirstOrDefault()
                                                           }).AsQueryable();
            return resultado.FirstOrDefault();
        }

        /// <summary>
        /// OK
        /// Retorna a lista de SubItens que contenha saldo - o reservador por almoxarifado
        /// </summary>
        /// <param name="almoxarifado">Almoxarifado id</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado)
        {
            IList<SubItemMaterialEntity> resultado = (from s in Db.TB_SALDO_SUBITEMs
                                                      join r in Db.TB_RESERVA_MATERIALs on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID } equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                                                      from r in x.DefaultIfEmpty()
                                                      join si in Db.TB_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals si.TB_SUBITEM_MATERIAL_ID
                                                      where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                                                      where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                                             && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                                             || (r.TB_RESERVA_MATERIAL_QUANT == null))
                                                      where (s.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = si.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = si.TB_SUBITEM_MATERIAL_CODIGO,
                                                          Descricao = si.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                      }).Distinct().ToList();
            return resultado;
        }

        /// <summary>
        /// OK
        /// Retorna os SubItens que contem saldo - os reservados por Almoxarifado e Item
        /// </summary>
        /// <param name="almoxarifado">Almoxarifado Id</param>
        /// <param name="item">Item Id</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado, int item)
        {

            IList<SubItemMaterialEntity> resultado = (from s in Db.TB_SALDO_SUBITEMs
                                                      join r in Db.TB_RESERVA_MATERIALs on new { s.TB_SUBITEM_MATERIAL_ID, s.TB_UGE_ID, s.TB_ALMOXARIFADO_ID } equals new { r.TB_SUBITEM_MATERIAL_ID, r.TB_UGE_ID, r.TB_ALMOXARIFADO_ID } into x
                                                      from r in x.DefaultIfEmpty()
                                                      join isi in Db.TB_ITEM_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals isi.TB_SUBITEM_MATERIAL_ID
                                                      join i in Db.TB_ITEM_MATERIALs on isi.TB_ITEM_MATERIAL_ID equals i.TB_ITEM_MATERIAL_ID
                                                      join su in Db.TB_SUBITEM_MATERIALs on isi.TB_SUBITEM_MATERIAL_ID equals su.TB_SUBITEM_MATERIAL_ID
                                                      where (s.TB_SALDO_SUBITEM_SALDO_QTDE > 0)
                                                      where ((r.TB_RESERVA_MATERIAL_QUANT != null
                                                             && s.TB_SALDO_SUBITEM_SALDO_QTDE - r.TB_RESERVA_MATERIAL_QUANT > 0)
                                                             || (r.TB_RESERVA_MATERIAL_QUANT == null))
                                                      where (s.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      where (i.TB_ITEM_MATERIAL_ID == item)
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = su.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = su.TB_SUBITEM_MATERIAL_CODIGO,
                                                          CodigoFormatado = su.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                          Descricao = su.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                      }
                                                   ).Distinct().ToList();
            return resultado;
        }

        /// <summary>
        /// OK
        /// Listar os Subitens do estoque do almoxarifado por natureza de despesa
        /// </summary>
        /// <param name="almoxarifado">Almoxarifado ID</param>
        /// <param name="itemId">Item Id</param>
        /// <param name="natDespesa">Natureza despesa ID</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemByAlmoxItemMaterial(int almoxarifado, int itemId, string natDespesa)
        {

            IList<SubItemMaterialEntity> resultado = (from s in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                      join it in Db.TB_ITEM_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals it.TB_SUBITEM_MATERIAL_ID
                                                      where (s.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      where (it.TB_ITEM_MATERIAL_ID == itemId)
                                                      where (s.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == Convert.ToInt32(natDespesa))
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                          CodigoFormatado = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                          Descricao = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          CodigoDescricao = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO + " - " + s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      }
                                                   ).Distinct().ToList();
            return resultado;
        }

        /// <summary>
        /// O Método está rápido, podem pode ocorrer lentidão com uma grande quantidade de dados
        /// Retorna o Subitem com left join com Almoxarifado
        /// </summary>
        /// <param name="_id">SubItem ID</param>
        /// <param name="_idAlmoxarifado">Almoxarifado ID</param>
        /// <returns></returns>
        public SubItemMaterialEntity SelectAlmox(int _id, int _idAlmoxarifado)
        {
            SubItemMaterialEntity resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                    join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                    into subItemDefault
                                                    from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                                    join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _idAlmoxarifado } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                                    into almoxDefault
                                                    from almox in almoxDefault.DefaultIfEmpty()
                                                    where ((itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID == _id) || (_id == 0))
                                                    where (almox.TB_ALMOXARIFADO_ID == _idAlmoxarifado || almox.TB_ALMOXARIFADO_ID == null)
                                                    select new SubItemMaterialEntity
                                                    {
                                                        Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                        Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                        Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                        //ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                                        IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                        ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial == null ? 0 : itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                                        EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                                                        EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                                                        IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                        IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                                        IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                        AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID
                                                    }).FirstOrDefault();
            return resultadoAlmox;
        }



        /// <summary>
        /// Retorna o SubItem do almoxarifado
        /// </summary>
        /// <param name="_itemId">Código Item Material</param>
        /// <param name="_almoxarifadoId">Almoxarifado ID</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemPorAlmox(int _itemId, int _almoxarifadoId)
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                      join b in Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                      where a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == _almoxarifadoId
                                                      where b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == _itemId
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                          CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                          Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR_ID)),
                                                          IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          ItemMaterial = (new ItemMaterialEntity(b.TB_ITEM_MATERIAL_ID)),
                                                          EstoqueMaximo = a.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX,
                                                          EstoqueMinimo = a.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN,
                                                          IndicadorAtividadeAlmox = a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                          IndicadorDisponivelId = a.TB_INDICADOR_DISPONIVEL_ID,
                                                          IndicadorDisponivelDescricao = a.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                          AlmoxarifadoId = a.TB_ALMOXARIFADO_ID
                                                      })
                                          .OrderBy(a => a.Codigo).ToList();
            return resultado;

        }

        /// <summary>
        /// Retorna a lista de itens utilizado na modal.
        /// Esse método é mais simples do foi desenvolvido especificamente para a tela de gerencia de catálogo
        /// </summary>
        /// <param name="Codigo">Não utilizado</param>
        /// <param name="Descricao">Não Utilizado</param>
        /// <param name="almoxarifado">Almoxarifado ID</param>
        /// <returns></returns>
        //private IEnumerable<SubItemMaterialEntity> ListarSubItemGestorPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado)
        //{
        //    var resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
        //                     join unid in this.Db.TB_UNIDADE_FORNECIMENTOs on new { a.TB_UNIDADE_FORNECIMENTO_ID } equals new { unid.TB_UNIDADE_FORNECIMENTO_ID }
        //                     where (a.TB_GESTOR_ID == almoxarifado.Gestor.Id || almoxarifado.Gestor.Id == 0)
        //                     orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
        //                     select new SubItemMaterialEntity
        //                     {
        //                         Id = a.TB_SUBITEM_MATERIAL_ID,
        //                         Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
        //                         Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
        //                         UnidadeFornecimento = new UnidadeFornecimentoEntity
        //                         {
        //                             Id = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
        //                             Codigo = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                             Descricao = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
        //                         },
        //                         ItemMaterial = (from i in Db.TB_ITEM_SUBITEM_MATERIALs
        //                                         where i.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
        //                                         select new ItemMaterialEntity
        //                                         {
        //                                             Id = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
        //                                             Codigo = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
        //                                             Descricao = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
        //                                         }).FirstOrDefault<ItemMaterialEntity>()
        //                     });

        //    return resultado;
        //}

        //private IEnumerable<SubItemMaterialEntity> ListarSubItemAlmoxPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado)
        //{
        //    var resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
        //                     join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
        //                     into almoxLeft from almox in almoxLeft.DefaultIfEmpty()
        //                     join unid in this.Db.TB_UNIDADE_FORNECIMENTOs on new { a.TB_UNIDADE_FORNECIMENTO_ID } equals new { unid.TB_UNIDADE_FORNECIMENTO_ID }
        //                     where (almox.TB_ALMOXARIFADO_ID == almoxarifado.Id || almoxarifado.Id == 0)
        //                     where (a.TB_GESTOR_ID == almoxarifado.Gestor.Id || almoxarifado.Gestor.Id == 0)
        //                     orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
        //                     select new SubItemMaterialEntity
        //                     {
        //                         Id = a.TB_SUBITEM_MATERIAL_ID,
        //                         Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
        //                         Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
        //                         IsLote = a.TB_SUBITEM_MATERIAL_LOTE,
        //                         IndicadorAtividadeAlmox = almox != null ? almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE : false,
        //                         SaldoSubItems = (from saldo in Db.TB_SALDO_SUBITEMs
        //                                          where saldo.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
        //                                          where (saldo.TB_UGE_ID == almoxarifado.Uge.Id || almoxarifado.Uge.Id == 0)
        //                                          select new SaldoSubItemEntity
        //                                          {
        //                                              Id = saldo.TB_SALDO_SUBITEM_ID,
        //                                              SaldoQtde = saldo.TB_SALDO_SUBITEM_SALDO_QTDE
        //                                          }).ToList(),
        //                         UnidadeFornecimento = new UnidadeFornecimentoEntity
        //                                          {
        //                                              Id = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
        //                                              Codigo = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
        //                                              Descricao = a.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
        //                                          },
        //                         NaturezaDespesa = new NaturezaDespesaEntity
        //                                          {
        //                                              Id = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
        //                                              Codigo = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
        //                                              Descricao = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
        //                                          },
        //                         ItemMaterial = (from i in Db.TB_ITEM_SUBITEM_MATERIALs
        //                                         where i.TB_SUBITEM_MATERIAL_ID == a.TB_SUBITEM_MATERIAL_ID
        //                                         select new ItemMaterialEntity 
        //                                         {
        //                                             Id = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
        //                                             Codigo = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
        //                                             Descricao = i.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
        //                                         }).FirstOrDefault<ItemMaterialEntity>()
        //                     });

        //    return resultado;
        //}

        //public IList<SubItemMaterialEntity> ListarSubItemPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado)
        //{
        //    //faz o tratamento dos dados
        //    if (almoxarifado == null)
        //        almoxarifado = new AlmoxarifadoEntity();

        //    if (almoxarifado.Id == null)
        //        almoxarifado.Id = 0;

        //    if (almoxarifado.Uge == null)
        //    {
        //        almoxarifado.Uge = new UGEEntity();
        //        almoxarifado.Uge.Id = 0;
        //    }

        //    if (almoxarifado.Gestor == null)
        //    {
        //        almoxarifado.Gestor = new GestorEntity();
        //        almoxarifado.Gestor.Id = 0;
        //    }

        //    //Realiza a estratégia
        //    IEnumerable<SubItemMaterialEntity> resultado;

        //    if (almoxarifado.Id == 0 && almoxarifado.Gestor.Id != 0)
        //    {
        //        resultado = ListarSubItemGestorPorPalavraChave(Codigo, Descricao, almoxarifado);
        //    }
        //    else
        //    {
        //        resultado = ListarSubItemAlmoxPorPalavraChave(Codigo, Descricao, almoxarifado);
        //    }

        //    //filtra os dados
        //    long codigoProduto = 0;
        //    long.TryParse(Descricao, out codigoProduto);

        //    //Se tiver código, então pesquisar pelo código
        //    if (codigoProduto > 0)
        //    {
        //        resultado = resultado.Where(b => b.Codigo == (codigoProduto));
        //    }
        //    //Se for por descrição, então pesquisar cada palavra chave
        //    else
        //    {
        //        List<String> palavras = Descricao.Split(' ').ToList<String>();
        //        foreach (var p in palavras)
        //        {
        //            String palavra = p;
        //            resultado = resultado.Where(b => b.Descricao.ToUpper().Contains(palavra.ToUpper()));
        //        }
        //    }

        //    return resultado.ToList();
        //}

        //public IList<SubItemMaterialEntity> ListarSubItemPorPalavraChave(long? Codigo, string Descricao, AlmoxarifadoEntity almoxarifado, bool subitemComSaldo)
        //{
        //    if (!subitemComSaldo)
        //        return ListarSubItemPorPalavraChave(Codigo, Descricao, almoxarifado);

        //    //faz o tratamento dos dados
        //    if (almoxarifado == null)
        //        almoxarifado = new AlmoxarifadoEntity() { Id = 0 };

        //    if (!almoxarifado.Id.HasValue)
        //        almoxarifado.Id = 0;

        //    if (almoxarifado.Uge == null)
        //        almoxarifado.Uge = new UGEEntity() { Id = 0 };

        //    if (almoxarifado.Gestor == null)
        //        almoxarifado.Gestor = new GestorEntity() { Id = 0 };


        //    //Realiza a estratégia
        //    IEnumerable<SubItemMaterialEntity> resultado;
        //    List<SubItemMaterialEntity> lstFiltro;

        //    if (almoxarifado.Id == 0 && almoxarifado.Gestor.Id != 0)
        //        resultado = ListarSubItemGestorPorPalavraChave(Codigo, Descricao, almoxarifado);
        //    else
        //        resultado = ListarSubItemAlmoxPorPalavraChave(Codigo, Descricao, almoxarifado);


        //    //filtra os dados
        //    long codigoProduto = 0;
        //    long.TryParse(Descricao, out codigoProduto);

        //    //Se tiver código, então pesquisar pelo código
        //    if (codigoProduto > 0)
        //    {
        //        resultado = resultado.Where(b => b.Codigo == (codigoProduto));
        //    }
        //    //Se for por descrição, então pesquisar cada palavra chave
        //    else
        //    {
        //        List<String> palavras = Descricao.Split(' ').ToList<String>();
        //        foreach (var p in palavras)
        //        {
        //            String palavra = p;
        //            resultado = resultado.Where(b => b.Descricao.ToUpper().Contains(palavra.ToUpper()));
        //        }
        //    }

        //    lstFiltro = new List<SubItemMaterialEntity>();
        //    resultado = resultado.Where(subItem => subItem.SaldoAtual > 0);

        //    resultado.ToList().ForEach(subItem => { 
        //                                            if(subItem.SaldoAtual.Value > 0) 
        //                                                lstFiltro.Add(subItem);
        //                                          });
        //    return resultado.ToList();
        //}

        /// <summary>
        /// OK
        /// Lista paginado os SubItens  por gestor e item 
        /// </summary>
        /// <param name="_itemId">Item Id</param>
        /// <param name="_gestorId">Gestor Id</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> Listar(int _itemId, int _gestorId, bool? IndAtidade)
        {
            //IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
            IQueryable<SubItemMaterialEntity> qryConsulta = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                             join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                             where (b.TB_ITEM_MATERIAL_ID == _itemId)
                                                             where (a.TB_GESTOR_ID == _gestorId)
                                                             orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
                                                             select new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL_ID,
                                                                 Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                 CodigoBarras = a.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                                 ContaAuxiliar = (new ContaAuxiliarEntity
                                                                 {
                                                                     //Id = a.TB_CONTA_AUXILIAR_ID,
                                                                     // Codigo = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                                     //  Descricao = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                                 }),
                                                                 Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                                 IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                                 IsDecimal = (bool)a.TB_SUBITEM_MATERIAL_DECIMOS,
                                                                 IsFracionado = (bool)a.TB_SUBITEM_MATERIAL_FRACIONA,
                                                                 //IsLote = a.TB_SUBITEM_MATERIAL_LOTE,
                                                                 NaturezaDespesa = new NaturezaDespesaEntity
                                                                 {
                                                                     Id = a.TB_NATUREZA_DESPESA_ID,
                                                                     Codigo = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                     Descricao = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                 },
                                                                 ItemMaterial = new ItemMaterialEntity
                                                                 {
                                                                     Id = b.TB_ITEM_MATERIAL_ID,
                                                                     Codigo = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                     Descricao = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                                 },
                                                                 UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                 {
                                                                     Id = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                     Codigo = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                     Descricao = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                 }
                                                             }).AsQueryable();

            IList<SubItemMaterialEntity> resultado;

            if (IndAtidade != null)
            {
                resultado = qryConsulta.Skip(this.SkipRegistros).Where(a => a.IndicadorAtividade == IndAtidade)
                                                                 .Take(this.RegistrosPagina)
                                                                 .ToList<SubItemMaterialEntity>();
            }
            else
            {
                resultado = qryConsulta.Skip(this.SkipRegistros)
                                                                     .Take(this.RegistrosPagina)
                                                                     .ToList<SubItemMaterialEntity>();

            }


            //this.totalregistros = (from a in this.Db.TB_SUBITEM_MATERIALs
            //                       join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
            //                       where (b.TB_ITEM_MATERIAL_ID == _itemId)
            //                       where (a.TB_GESTOR_ID == _gestorId)
            //                       select new
            //                       {
            //                           Id = a.TB_SUBITEM_MATERIAL_ID,
            //                       }).Count();

            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));

            this.totalregistros = qryConsulta.Count();

            return resultado;

        }

        /// <summary>
        /// OK
        /// Retorna a lista de SubItens por gestor
        /// </summary>
        /// <param name="_itemId">Item Id</param>
        /// <param name="_gestorId">Gestor Id</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> Imprimir(int _itemId, int _gestorId)
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                      join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                      where (b.TB_ITEM_MATERIAL_ID == _itemId && a.TB_GESTOR_ID == _gestorId)
                                                      orderby a.TB_SUBITEM_MATERIAL_DECIMOS
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                          Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          CodigoBarras = a.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                          ContaAuxiliar = (new ContaAuxiliarEntity
                                                          {
                                                              //Id = a.TB_CONTA_AUXILIAR_ID,
                                                              //   Codigo = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                              //  Descricao = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                          }),
                                                          Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          IsDecimal = (bool)a.TB_SUBITEM_MATERIAL_DECIMOS,
                                                          IsFracionado = (bool)a.TB_SUBITEM_MATERIAL_FRACIONA,
                                                          //IsLote = a.TB_SUBITEM_MATERIAL_LOTE,
                                                          NaturezaDespesa = new NaturezaDespesaEntity
                                                          {
                                                              Id = a.TB_NATUREZA_DESPESA_ID,
                                                              Codigo = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                              Descricao = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                          },
                                                          ItemMaterial = new ItemMaterialEntity
                                                          {
                                                              Id = b.TB_ITEM_MATERIAL_ID,
                                                              Codigo = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                              Descricao = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                          },
                                                          UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                          {
                                                              Id = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                          }
                                                      })
                                          .ToList<SubItemMaterialEntity>();

            return resultado;
        }

        /// <summary>
        /// OK
        /// Imprime consulta de subitens de material por gestor, ordenado por descrição
        /// </summary>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> Imprimir()
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                      join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                      where (a.TB_GESTOR_ID == this.Entity.Gestor.Id)
                                                      orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                          Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          CodigoBarras = a.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                          ContaAuxiliar = (new ContaAuxiliarEntity
                                                          {
                                                              //Id = a.TB_CONTA_AUXILIAR_ID,
                                                              //  Codigo = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                              // Descricao = a.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                          }),
                                                          Gestor = (new GestorEntity(a.TB_GESTOR_ID)),
                                                          IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          IsDecimal = (bool)a.TB_SUBITEM_MATERIAL_DECIMOS,
                                                          IsFracionado = (bool)a.TB_SUBITEM_MATERIAL_FRACIONA,
                                                          //IsLote = a.TB_SUBITEM_MATERIAL_LOTE,
                                                          NaturezaDespesa = new NaturezaDespesaEntity
                                                          {
                                                              Id = a.TB_NATUREZA_DESPESA_ID,
                                                              Codigo = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                              Descricao = a.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                          },
                                                          ItemMaterial = new ItemMaterialEntity
                                                          {
                                                              Id = b.TB_ITEM_MATERIAL_ID,
                                                              Codigo = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                              Descricao = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                          },
                                                          UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                          {
                                                              Id = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                              Codigo = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                              Descricao = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                          }
                                                      })
                                          .ToList<SubItemMaterialEntity>();

            return resultado;
        }

        /// <summary>
        /// OK
        /// Imprime consulta de subitens de material por gestor, ordenado por descrição
        /// </summary>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> Imprimir(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId)
        {
            IList<SubItemMaterialEntity> resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                           join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           into subItemDefault
                                                           from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                                           join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                                           into itemMaterialDefault
                                                           from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                                           join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on subItemMaterial equals almox.TB_SUBITEM_MATERIAL into almoxDefault
                                                           from almox in almoxDefault.DefaultIfEmpty()
                                                           where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                                           where (itemMaterial.TB_MATERIAL_ID == _materialId)
                                                           where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                                           where (almox.TB_ALMOXARIFADO_ID == _almoxarifadoId || almox.TB_ALMOXARIFADO_ID == null)
                                                           orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID descending
                                                           orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               //ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                                               IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                                               EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                               EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                               IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                               IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                                               IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                               AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID
                                                           }).ToList();
            return resultadoAlmox;
        }

        public void Excluir()
        {
            try
            {

                var resulta = (from subMat in Db.TB_SUBITEM_MATERIALs
                               join almo in Db.TB_SUBITEM_MATERIAL_ALMOXes
                               on subMat.TB_SUBITEM_MATERIAL_ID equals almo.TB_SUBITEM_MATERIAL_ID
                               where subMat.TB_SUBITEM_MATERIAL_ID == this.Entity.Id
                               // &&  almo.TB_SUBITEM_MATERIAL_ID == this.Entity.AlmoxarifadoId
                               select new
                               {
                                   subMat.TB_SUBITEM_MATERIAL_ID
                               }
                   ).ToList().Count;
                if (!(resulta > 0))
                {
                    TB_ITEM_SUBITEM_MATERIAL item
                     = this.Db.TB_ITEM_SUBITEM_MATERIALs.Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
                    this.Db.TB_ITEM_SUBITEM_MATERIALs.DeleteOnSubmit(item);
                    this.Db.SubmitChanges();

                    TB_SUBITEM_MATERIAL sub
                      = this.Db.TB_SUBITEM_MATERIALs.Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id).FirstOrDefault();
                    this.Db.TB_SUBITEM_MATERIALs.DeleteOnSubmit(sub);
                    this.Db.SubmitChanges();
                }

                //IQueryable<TB_SUBITEM_MATERIAL_ALMOX> query = (from a in Db.TB_SUBITEM_MATERIAL_ALMOXes
                //                                               where a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id &&
                //                                               a.TB_ALMOXARIFADO_ID == this.Entity.AlmoxarifadoId
                //                                                 && !(from o in Db.TB_SALDO_SUBITEMs
                //                                                      where o.TB_ALMOXARIFADO_ID == this.Entity.AlmoxarifadoId
                //                                                      select o.TB_SUBITEM_MATERIAL_ID)
                //                                                   .Contains(a.TB_SUBITEM_MATERIAL_ID)
                //                                               select a).AsQueryable();

                //var itemSubItemMaterialAlmox = query.FirstOrDefault();
                //if (itemSubItemMaterialAlmox != null)
                //{
                //    this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DeleteOnSubmit(itemSubItemMaterialAlmox);
                //    this.Db.SubmitChanges();
                //}
                //else
                //    throw new Exception("Esse subitem tem saldo, somente pode ser inativado, não pode ser excluído.");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public void Salvar()
        {
            TB_SUBITEM_MATERIAL sitem = new TB_SUBITEM_MATERIAL();

            if (this.Entity.Id.HasValue)
                sitem = this.Db.TB_SUBITEM_MATERIALs.Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id.Value).FirstOrDefault();
            else
            {
                this.Db.TB_SUBITEM_MATERIALs.InsertOnSubmit(sitem);
            }

            sitem.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
            sitem.TB_SUBITEM_MATERIAL_CODIGO = this.Entity.Codigo;
            sitem.TB_SUBITEM_MATERIAL_DESCRICAO = this.Entity.Descricao;
            sitem.TB_SUBITEM_MATERIAL_COD_BARRAS = this.Entity.CodigoBarras;
            sitem.TB_NATUREZA_DESPESA_ID = this.Entity.NaturezaDespesa.Id.Value;
            //sitem.TB_CONTA_AUXILIAR_ID = this.Entity.ContaAuxiliar.Id.Value;
            //sitem.TB_SUBITEM_MATERIAL_LOTE = (bool)this.Entity.IsLote;
            sitem.TB_SUBITEM_MATERIAL_DECIMOS = this.Entity.IsDecimal;
            sitem.TB_SUBITEM_MATERIAL_FRACIONA = this.Entity.IsFracionado;
            sitem.TB_UNIDADE_FORNECIMENTO_ID = this.Entity.UnidadeFornecimento.Id.Value;
            sitem.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE = (bool)this.Entity.IndicadorAtividade;

            if (!this.Entity.Id.HasValue)
            {
                var itemMaterial = new TB_ITEM_SUBITEM_MATERIAL();
                itemMaterial.TB_ITEM_MATERIAL_ID = this.Entity.ItemMaterial.Id.Value;
                itemMaterial.TB_GESTOR_ID = this.Entity.Gestor.Id.Value;
                sitem.TB_ITEM_SUBITEM_MATERIALs.Add(itemMaterial);
            }

            this.Db.SubmitChanges();
        }

        public void SalvarAlmox()
        {
            TB_SUBITEM_MATERIAL_ALMOX sitem = new TB_SUBITEM_MATERIAL_ALMOX();

            if (this.Entity.Id.HasValue && this.Entity.AlmoxarifadoId != 0)
            {
                sitem = this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                    .Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id.Value)
                    .Where(a => a.TB_ALMOXARIFADO_ID == this.Entity.AlmoxarifadoId)
                    .FirstOrDefault();

                if (sitem == null)
                {
                    TB_SUBITEM_MATERIAL_ALMOX item = new TB_SUBITEM_MATERIAL_ALMOX();
                    this.Db.TB_SUBITEM_MATERIAL_ALMOXes.InsertOnSubmit(item);

                    item.TB_SUBITEM_MATERIAL_ID = (int)this.Entity.Id.Value;
                    item.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = this.Entity.EstoqueMaximo;
                    item.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = this.Entity.EstoqueMinimo;

                    item.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = this.Entity.IndicadorAtividadeAlmox;
                    item.TB_INDICADOR_DISPONIVEL_ID = this.Entity.IndicadorDisponivelId;
                    item.TB_INDICADOR_ITEM_SALDO_ZERADO_ID = this.Entity.IndicadorDisponivelIdZerado;
                    item.TB_ALMOXARIFADO_ID = this.Entity.AlmoxarifadoId;
                }
                else
                {
                    sitem.TB_SUBITEM_MATERIAL_ID = (int)this.Entity.Id.Value;
                    sitem.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = this.Entity.EstoqueMaximo;
                    sitem.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = this.Entity.EstoqueMinimo;

                    sitem.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = this.Entity.IndicadorAtividadeAlmox;
                    sitem.TB_INDICADOR_DISPONIVEL_ID = this.Entity.IndicadorDisponivelId;
                    sitem.TB_INDICADOR_ITEM_SALDO_ZERADO_ID = this.Entity.IndicadorDisponivelIdZerado;
                    sitem.TB_ALMOXARIFADO_ID = this.Entity.AlmoxarifadoId;
                }
            }
            else
            {
                this.Db.TB_SUBITEM_MATERIAL_ALMOXes.InsertOnSubmit(sitem);

                sitem.TB_SUBITEM_MATERIAL_ID = (int)this.Entity.Id.Value;
                sitem.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX = this.Entity.EstoqueMaximo;
                sitem.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN = this.Entity.EstoqueMinimo;
                sitem.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE = this.Entity.IndicadorAtividadeAlmox;
                sitem.TB_INDICADOR_DISPONIVEL_ID = this.Entity.IndicadorDisponivelId;
                sitem.TB_INDICADOR_ITEM_SALDO_ZERADO_ID = this.Entity.IndicadorDisponivelIdZerado;
                sitem.TB_ALMOXARIFADO_ID = this.Entity.AlmoxarifadoId;
            }

            this.Db.SubmitChanges();
        }

        public bool PodeExcluir()
        {
            int qtd = int.MinValue;

            qtd = (from a in this.Db.TB_MOVIMENTO_ITEMs.DefaultIfEmpty()
                   join b in this.Db.TB_MOVIMENTO_ITEM_ESTs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                   where a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id
                   select new
                   {
                       Id = a.TB_MOVIMENTO_ID,
                   }).Count();

            return (qtd < 1);
        }

        public bool ExisteCodigoInformado()
        {
            bool retorno = false;
            retorno = this.Db.TB_SUBITEM_MATERIALs
                .Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == this.Entity.Codigo)
                .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
                .Count() > 0;
            //if (this.Entity.Id.HasValue)
            //{
            //    retorno = this.Db.TB_SUBITEM_MATERIALs
            //    .Where(a => a.TB_SUBITEM_MATERIAL_ID == this.Entity.Id)
            //    .Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == this.Entity.Codigo)
            //    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
            //    .Count() > 0;
            //}
            //else
            //{
            //    retorno = this.Db.TB_SUBITEM_MATERIALs
            //    .Where(a => a.TB_SUBITEM_MATERIAL_CODIGO == this.Entity.Codigo)
            //    .Where(a => a.TB_GESTOR_ID == this.Entity.Gestor.Id.Value)
            //    .Count() > 0;
            //}
            return retorno;
        }

        public SubItemMaterialEntity LerRegistro()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// OK Esse método pode retornar muitos registros
        /// Retorna todos os Subitens material, ou pode ser filtrado pelo código
        /// </summary>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarTodosCod()
        {
            IQueryable<SubItemMaterialEntity> resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                               CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                               Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                               ContaAuxiliar = new ContaAuxiliarEntity
                                                               {
                                                                   // Id = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID,
                                                                   //Codigo = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                                   // Descricao = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                               },
                                                               Gestor = new GestorEntity
                                                               {
                                                                   Id = a.TB_GESTOR_ID
                                                               },
                                                               IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               IsDecimal = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DECIMOS,
                                                               IsFracionado = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_FRACIONA,
                                                               //IsLote = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE,
                                                               NaturezaDespesa = new NaturezaDespesaEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                               },
                                                               ItemMaterial = new ItemMaterialEntity
                                                               {
                                                                   Id = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                   Codigo = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                   Descricao = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                               },
                                                               UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                               }
                                                           }
                                                     );

            if (this.Entity.Codigo != null && this.Entity.Codigo != 0)
                resultado = resultado.Where(a => a.Codigo == this.Entity.Codigo);

            if (this.Entity.ItemMaterial != null && this.Entity.ItemMaterial.Codigo != 0)
                resultado = resultado.Where(a => a.ItemMaterial.Codigo == this.Entity.ItemMaterial.Codigo);

            return resultado.ToList();
        }

        /// <summary>
        /// Lista os SubItens utilizando Lambda
        /// </summary>
        /// <param name="where">Expressao para a consulta</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemMaterial(System.Linq.Expressions.Expression<Func<SubItemMaterialEntity, bool>> where)
        {
            IQueryable<SubItemMaterialEntity> resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                               CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                               Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                               ContaAuxiliar = new ContaAuxiliarEntity
                                                               {
                                                                   //Id = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID,
                                                                   //Codigo = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                                   // Descricao = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                               },
                                                               Gestor = new GestorEntity
                                                               {
                                                                   Id = a.TB_GESTOR_ID
                                                               },
                                                               IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               IsDecimal = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DECIMOS,
                                                               IsFracionado = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_FRACIONA,
                                                               //IsLote = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE,
                                                               NaturezaDespesa = new NaturezaDespesaEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                               },
                                                               ItemMaterial = new ItemMaterialEntity
                                                               {
                                                                   Id = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                   Codigo = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                   Descricao = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                               },
                                                               UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                               }
                                                           }
                                                    ).Where(where);
            return resultado.ToList();
        }

        /// <summary>
        /// Lista os Itens por ID e Gestor
        /// </summary>
        /// <param name="_itemId">Item Id</param>
        /// <param name="_gestor_id">Gestor Id</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarTodosCod(int _itemId, int _gestor_id)
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                      where a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID == _itemId
                                                      where a.TB_GESTOR_ID == _gestor_id
                                                      orderby a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                          Descricao = string.Format("{0} - {1}", a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'), a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO),
                                                      }).ToList<SubItemMaterialEntity>();
            return resultado;
        }

        /// <summary>
        /// OK
        /// Listar Sub Itens por Item id, Gestor id e Almox Id
        /// </summary>
        /// <param name="_itemCodigo"></param>
        /// <param name="_ugeId"></param>
        /// <param name="_almoxId"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarTodosCodPorItemUgeAlmox(int _itemCodigo, int _ugeId, int _almoxId)
        {

            //IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
            IQueryable<SubItemMaterialEntity> qryConsulta = (from a in this.Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                             join b in Db.TB_ITEM_SUBITEM_MATERIALs
                                                                on a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID
                                                             where b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == _itemCodigo
                                                             where a.TB_ALMOXARIFADO.TB_ALMOXARIFADO_ID == _almoxId
                                                             //where b.TB_GESTOR_ID == gestorId
                                                             select new SubItemMaterialEntity
                                                             {
                                                                 Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                 CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                 CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                                 ContaAuxiliar = new ContaAuxiliarEntity
                                                                 {
                                                                     // Id = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID,
                                                                     //  Codigo = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                                     // Descricao = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                                 },
                                                                 Gestor = new GestorEntity
                                                                 {
                                                                     Id = b.TB_GESTOR_ID
                                                                 },
                                                                 IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                                 IndicadorAtividadeAlmox = (bool)a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                                 IsDecimal = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DECIMOS,
                                                                 IsFracionado = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_FRACIONA,
                                                                 //IsLote = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE,
                                                                 NaturezaDespesa = new NaturezaDespesaEntity
                                                                 {
                                                                     Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                     Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                     Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                                 },
                                                                 ItemMaterial = new ItemMaterialEntity
                                                                 {
                                                                     Id = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                     Codigo = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                     CodigoFormatado = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                     Descricao = b.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                                 },
                                                                 UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                                 {
                                                                     Id = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                     Codigo = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                     Descricao = b.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                                 }
                                                             }
                                                     ).AsQueryable();

            IList<SubItemMaterialEntity> resultado = qryConsulta.ToList<SubItemMaterialEntity>();

            string _strSQL = qryConsulta.ToString();
            Db.GetCommand(qryConsulta).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));


            return resultado;
        }

        /// <summary>
        /// OK
        /// </summary>
        /// <param name="_itemCodigo"></param>
        /// <param name="_ugeId"></param>
        /// <param name="_almoxId"></param>
        /// <param name="_codigoNaturezaDespesa"></param>
        /// <returns></returns>
        //public IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemCodigo, int _ugeId, int _almoxId, int _codigoNaturezaDespesa)
        public IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemCodigo, int _ugeId, int _almoxId, int gestorId, int _codigoNaturezaDespesa)
        {
            List<SubItemMaterialEntity> listaRetorno = new List<SubItemMaterialEntity>();
            // procurar o gestor da UGE
            //int gestorId = (from a in this.Db.TB_UGEs
            //                join g in this.Db.TB_GESTORs on a.TB_UGE_ID equals g.TB_UGE_ID
            //                where a.TB_UGE_ID == _ugeId select g.TB_GESTOR_ID).FirstOrDefault();

            //int gestorId = (from Almoxarifado in this.Db.TB_ALMOXARIFADOs
            //                join Gestor in this.Db.TB_GESTORs on Almoxarifado.TB_GESTOR_ID equals Gestor.TB_GESTOR_ID
            //                where Almoxarifado.TB_ALMOXARIFADO_ID == _almoxId
            //                select Gestor.TB_GESTOR_ID).FirstOrDefault();

            var _resultSet = (from subItemMaterialAlmox in Db.TB_SUBITEM_MATERIAL_ALMOXes
                              join itemSubItemMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ID equals itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID
                              join itemMaterial in Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                              join itemNaturezaDespesa in Db.TB_ITEM_NATUREZA_DESPESAs on itemMaterial.TB_ITEM_MATERIAL_ID equals itemNaturezaDespesa.TB_ITEM_MATERIAL_ID
                              join naturezaDespesa in Db.TB_NATUREZA_DESPESAs on itemNaturezaDespesa.TB_NATUREZA_DESPESA_ID equals naturezaDespesa.TB_NATUREZA_DESPESA_ID

                              where subItemMaterialAlmox.TB_ALMOXARIFADO_ID == _almoxId
                              where itemSubItemMaterial.TB_GESTOR_ID == gestorId
                              where itemMaterial.TB_ITEM_MATERIAL_CODIGO == _itemCodigo
                              where naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == _codigoNaturezaDespesa
                              where naturezaDespesa.TB_NATUREZA_DESPESA_ID == subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA_ID
                              where itemSubItemMaterial.TB_GESTOR_ID == subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_GESTOR_ID

                              where subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true //subitens ativos

                              select new SubItemMaterialEntity
                              {
                                  Id = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                  CodigoFormatado = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                  Codigo = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                  Descricao = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                  CodigoBarras = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                  Gestor = new GestorEntity(itemSubItemMaterial.TB_GESTOR_ID),
                                  IndicadorAtividade = (bool)subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                  //IsLote = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE,
                                  NaturezaDespesa = new NaturezaDespesaEntity
                                  {
                                      Id = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                      Codigo = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                      Descricao = subItemMaterialAlmox.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                  },
                                  ItemMaterial = new ItemMaterialEntity
                                  {
                                      Id = itemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                      Codigo = itemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                      CodigoFormatado = itemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                      Descricao = itemSubItemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                  },
                                  UnidadeFornecimento = new UnidadeFornecimentoEntity
                                  {
                                      Id = itemSubItemMaterial.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                      Codigo = itemSubItemMaterial.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      Descricao = itemSubItemMaterial.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                  },
                                  IndicadorAtividadeAlmox = subItemMaterialAlmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                              });

            string _strSQL = _resultSet.ToString();
            Db.GetCommand(_resultSet).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => _strSQL = _strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));


            listaRetorno = _resultSet.ToList();

            return listaRetorno;
        }

        #region Gerenciamento de Catálogo

        /// <summary>
        ///OK - O Método pode ficar lento devido a quantidade de itens
        ///Utilizado na tela de Gerenciamento de catálogo para listar todos os Itens pelo Codigo do Material
        /// </summary>
        /// <param name="subItem"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemAlmox(SubItemMaterialEntity subItem)
        {
            IEnumerable<SubItemMaterialEntity> resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                            join b in Db.TB_ALMOXARIFADOs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                                                            join c in Db.TB_SUBITEM_MATERIAL_ALMOXes on b.TB_ALMOXARIFADO_ID equals c.TB_ALMOXARIFADO_ID
                                                            where a.TB_GESTOR_ID == subItem.Gestor.Id
                                                            where a.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID == subItem.ItemMaterial.Material.Id
                                                            select new SubItemMaterialEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                // ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID)),
                                                                IndicadorAtividade = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                                ItemMaterial = (new ItemMaterialEntity(a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID)),
                                                                EstoqueMaximo = 0,
                                                                EstoqueMinimo = 0,
                                                                IndicadorAtividadeAlmox = false,
                                                                IndicadorDisponivelId = 0,
                                                                AlmoxarifadoId = 0
                                                            }).Distinct();
            if (subItem.ItemMaterial.Material.Id == 0)
            {
                resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                             join b in Db.TB_ALMOXARIFADOs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                             join c in Db.TB_SUBITEM_MATERIAL_ALMOXes on b.TB_ALMOXARIFADO_ID equals c.TB_ALMOXARIFADO_ID
                             where a.TB_GESTOR_ID == subItem.Gestor.Id
                             select new SubItemMaterialEntity
                             {
                                 Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                 Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                 Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                 //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID)),
                                 IndicadorAtividade = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                 ItemMaterial = (new ItemMaterialEntity(a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID)),
                                 EstoqueMaximo = 0,
                                 EstoqueMinimo = 0,
                                 IndicadorAtividadeAlmox = false,
                                 IndicadorDisponivelId = 0,
                                 AlmoxarifadoId = 0
                             }).Distinct();
            }

            IList<SubItemMaterialEntity> resultadoAlmox = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                           join b in this.Db.TB_ITEM_SUBITEM_MATERIALs.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID into a_b
                                                           from ab in a_b.DefaultIfEmpty()
                                                           join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID into a_almox
                                                           from aalmox in a_almox.DefaultIfEmpty()
                                                           where (ab.TB_ITEM_MATERIAL.TB_MATERIAL.TB_MATERIAL_ID == subItem.ItemMaterial.Material.Id)
                                                           where (a.TB_GESTOR_ID == subItem.Gestor.Id)
                                                           where aalmox.TB_ALMOXARIFADO_ID == subItem.AlmoxarifadoId
                                                           orderby a.TB_SUBITEM_MATERIAL_ID
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                                               IndicadorAtividade = a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               ItemMaterial = (new ItemMaterialEntity(ab.TB_ITEM_MATERIAL_ID)),
                                                               EstoqueMaximo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                               EstoqueMinimo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                               IndicadorAtividadeAlmox = aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                               IndicadorDisponivelId = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                                               IndicadorDisponivelDescricao = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                               AlmoxarifadoId = aalmox.TB_ALMOXARIFADO_ID == null ? 0 : aalmox.TB_ALMOXARIFADO_ID
                                                           }).ToList();

            if (subItem.ItemMaterial.Material.Id == 0)
            {
                resultadoAlmox = (from a in this.Db.TB_SUBITEM_MATERIALs
                                  join b in this.Db.TB_ITEM_SUBITEM_MATERIALs.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID into a_b
                                  from ab in a_b.DefaultIfEmpty()
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID into a_almox
                                  from aalmox in a_almox.DefaultIfEmpty()
                                  where (a.TB_GESTOR_ID == subItem.Gestor.Id)
                                  where aalmox.TB_ALMOXARIFADO_ID == subItem.AlmoxarifadoId
                                  orderby a.TB_SUBITEM_MATERIAL_ID
                                  select new SubItemMaterialEntity
                                  {
                                      Id = a.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                      IndicadorAtividade = a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      ItemMaterial = (new ItemMaterialEntity(ab.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      IndicadorDisponivelId = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      IndicadorDisponivelDescricao = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      AlmoxarifadoId = aalmox.TB_ALMOXARIFADO_ID == null ? 0 : aalmox.TB_ALMOXARIFADO_ID
                                  }).ToList();
            }


            this.totalregistros = resultado.Count();

            IList<SubItemMaterialEntity> result = resultado.ToList<SubItemMaterialEntity>();

            if (resultadoAlmox.Count() > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = 0; j < resultadoAlmox.Count(); j++)
                    {
                        if (result[i].Id == resultadoAlmox[j].Id)
                        {
                            result[i].EstoqueMaximo = resultadoAlmox[j].EstoqueMaximo;
                            result[i].EstoqueMinimo = resultadoAlmox[j].EstoqueMinimo;
                            result[i].IndicadorAtividadeAlmox = resultadoAlmox[j].IndicadorAtividadeAlmox;
                            result[i].IndicadorDisponivelId = resultadoAlmox[j].IndicadorDisponivelId;
                            result[i].IndicadorDisponivelDescricao = resultadoAlmox[j].IndicadorDisponivelDescricao;
                            result[i].AlmoxarifadoId = resultadoAlmox[j].AlmoxarifadoId;
                        }
                    }
                }
            }

            //return result;
            return result.ToList();
        }
        /// <summary>
        /// OK
        /// Lista os SubItens LEFT Almox e LEFT indicador disponivel</summary>
        /// <param name="_itemId">Item Id</param>
        /// <param name="_gestorId">gestor ID</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestorId)
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                      join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                      join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                                                      join indicador in this.Db.TB_INDICADOR_DISPONIVELs.DefaultIfEmpty() on almox.TB_INDICADOR_DISPONIVEL_ID equals indicador.TB_INDICADOR_DISPONIVEL_ID
                                                      where (b.TB_ITEM_MATERIAL_ID == _itemId && a.TB_GESTOR_ID == _gestorId)
                                                      orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                          Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                                          IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          ItemMaterial = (new ItemMaterialEntity(b.TB_ITEM_MATERIAL_ID)),
                                                          EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                          EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                          IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                          IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL_ID,
                                                          IndicadorDisponivelDescricao = indicador.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                          AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID,

                                                      }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<SubItemMaterialEntity>();

            this.totalregistros = (from a in this.Db.TB_SUBITEM_MATERIALs
                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                   join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                                   join indicador in this.Db.TB_INDICADOR_DISPONIVELs.DefaultIfEmpty() on almox.TB_INDICADOR_DISPONIVEL_ID equals indicador.TB_INDICADOR_DISPONIVEL_ID
                                   where (b.TB_ITEM_MATERIAL_ID == _itemId && a.TB_GESTOR_ID == _gestorId)
                                   select new
                                   {
                                       Id = a.TB_SUBITEM_MATERIAL_ID,
                                   }).Count();
            return resultado;

        }

        /// <summary>
        /// OK
        /// Lista os SubItens LEFT Almox e LEFT indicador disponivel</summary>
        /// <param name="_itemId">Item Id</param>
        /// <param name="_gestorId">gestor ID</param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItem(int _subItemId, int _gestorId)
        {
            IList<SubItemMaterialEntity> resultado = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                      join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                                      join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                                                      join indicador in this.Db.TB_INDICADOR_DISPONIVELs.DefaultIfEmpty() on almox.TB_INDICADOR_DISPONIVEL_ID equals indicador.TB_INDICADOR_DISPONIVEL_ID
                                                      where (b.TB_SUBITEM_MATERIAL_ID == _subItemId && a.TB_GESTOR_ID == _gestorId)
                                                      orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = a.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                          Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                                          IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                          ItemMaterial = (new ItemMaterialEntity(b.TB_ITEM_MATERIAL_ID)),
                                                          EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                          EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                          IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                          IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL_ID,
                                                          IndicadorDisponivelDescricao = indicador.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                          AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID,

                                                      }).Skip(this.SkipRegistros)
                                          .Take(this.RegistrosPagina)
                                          .ToList<SubItemMaterialEntity>();

            this.totalregistros = (from a in this.Db.TB_SUBITEM_MATERIALs
                                   join b in this.Db.TB_ITEM_SUBITEM_MATERIALs on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID
                                   join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                                   join indicador in this.Db.TB_INDICADOR_DISPONIVELs.DefaultIfEmpty() on almox.TB_INDICADOR_DISPONIVEL_ID equals indicador.TB_INDICADOR_DISPONIVEL_ID
                                   where (b.TB_SUBITEM_MATERIAL_ID == _subItemId && a.TB_GESTOR_ID == _gestorId)
                                   select new
                                   {
                                       Id = a.TB_SUBITEM_MATERIAL_ID,
                                   }).Count();
            return resultado;

        }
        /// <summary>
        /// OK - O Método pode ficar lento devido a quantidade de itens
        /// Utilizado para trazer os registros da tela de Gerenciamento De catálogo
        /// </summary>
        /// <param name="_itemId"></param>
        /// <param name="_gestorId"></param>
        /// <param name="_almoxarifadoId"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestorId, int _almoxarifadoId)
        {
            IEnumerable<SubItemMaterialEntity> resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                            join b in Db.TB_ALMOXARIFADOs on a.TB_GESTOR_ID equals b.TB_GESTOR_ID
                                                            //join c in Db.TB_SUBITEM_MATERIAL_ALMOXes on b.TB_ALMOXARIFADO_ID equals c.TB_ALMOXARIFADO_ID
                                                            where a.TB_GESTOR_ID == _gestorId
                                                            where a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID == _itemId
                                                            select new SubItemMaterialEntity
                                                            {
                                                                Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                                Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                                Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                                //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID)),
                                                                IndicadorAtividade = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                                ItemMaterial = (new ItemMaterialEntity(a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID)),
                                                                EstoqueMaximo = 0,
                                                                EstoqueMinimo = 0,
                                                                IndicadorAtividadeAlmox = false,
                                                                IndicadorDisponivelId = 0,
                                                                AlmoxarifadoId = 0
                                                            }).Distinct();

            IList<SubItemMaterialEntity> resultadoAlmox = (from a in this.Db.TB_SUBITEM_MATERIALs
                                                           join b in this.Db.TB_ITEM_SUBITEM_MATERIALs.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals b.TB_SUBITEM_MATERIAL_ID into a_b
                                                           from ab in a_b.DefaultIfEmpty()
                                                           join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes.DefaultIfEmpty() on a.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID into a_almox
                                                           from aalmox in a_almox.DefaultIfEmpty()
                                                           where (ab.TB_ITEM_MATERIAL_ID == _itemId)
                                                           where (a.TB_GESTOR_ID == _gestorId)
                                                           where aalmox.TB_ALMOXARIFADO_ID == _almoxarifadoId
                                                           orderby a.TB_SUBITEM_MATERIAL_DESCRICAO
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = a.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               //ContaAuxiliar = (new ContaAuxiliarEntity(a.TB_CONTA_AUXILIAR_ID)),
                                                               IndicadorAtividade = a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : a.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               ItemMaterial = (new ItemMaterialEntity(ab.TB_ITEM_MATERIAL_ID)),
                                                               EstoqueMaximo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                               EstoqueMinimo = aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : aalmox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                               IndicadorAtividadeAlmox = aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : aalmox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                               IndicadorDisponivelId = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                                               IndicadorDisponivelDescricao = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                               IndicadorDisponivel = aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(aalmox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                                               AlmoxarifadoId = aalmox.TB_ALMOXARIFADO_ID == null ? 0 : aalmox.TB_ALMOXARIFADO_ID
                                                           }).ToList();

            this.totalregistros = resultado.Count();

            IList<SubItemMaterialEntity> result = resultado.ToList<SubItemMaterialEntity>();

            if (resultadoAlmox.Count() > 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = 0; j < resultadoAlmox.Count(); j++)
                    {
                        if (result[i].Id == resultadoAlmox[j].Id)
                        {
                            result[i].EstoqueMaximo = resultadoAlmox[j].EstoqueMaximo;
                            result[i].EstoqueMinimo = resultadoAlmox[j].EstoqueMinimo;
                            result[i].IndicadorAtividadeAlmox = resultadoAlmox[j].IndicadorAtividadeAlmox;
                            result[i].IndicadorDisponivelId = resultadoAlmox[j].IndicadorDisponivelId;
                            result[i].IndicadorDisponivelDescricao = resultadoAlmox[j].IndicadorDisponivelDescricao;
                            result[i].AlmoxarifadoId = resultadoAlmox[j].AlmoxarifadoId;
                            result[i].IndicadorDisponivel = resultadoAlmox[j].IndicadorDisponivel;
                        }
                    }
                }
            }

            //return result;
            return result.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_materialId"></param>
        /// <param name="_itemId"></param>
        /// <param name="_gestorId"></param>
        /// <param name="_almoxarifadoId"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId)
        {

            IList<SubItemMaterialEntity> resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                                           join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                                           into subItemDefault
                                                           from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                                           join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                                           into itemMaterialDefault
                                                           from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                                           join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }

                                                           into almoxDefault
                                                           from almox in almoxDefault.DefaultIfEmpty()
                                                           where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                                           where (itemMaterial.TB_MATERIAL_ID == _materialId)
                                                           where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                                           orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID descending
                                                           orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                                               Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               //ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                                               IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                                               EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                                               EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                                               IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                                               IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                                               IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                                               AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID
                                                           }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();

            this.totalregistros = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                   join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                   into subItemDefault
                                   from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                   join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                   into itemMaterialDefault
                                   from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                   join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                   into almoxDefault
                                   from almox in almoxDefault.DefaultIfEmpty()
                                   where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                   where (itemMaterial.TB_MATERIAL_ID == _materialId)
                                   where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                   select new
                                   {
                                       subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                   }).Distinct().Count();
            return resultadoAlmox;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_materialId"></param>
        /// <param name="_itemId"></param>
        /// <param name="_gestorId"></param>
        /// <param name="_almoxarifadoId"></param>
        /// <returns></returns>
        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorId, int _almoxarifadoId, int _naturezaId, Int64? _SubItemcodigo, Int64? _Itemcodigo, int _indicadorId, int _saldoId)
        {
            IList<SubItemMaterialEntity> resultadoAlmox;

            if (_saldoId == -1)
            {
                resultadoAlmox = resultadoAlmoxSaldo(_grupoId, _classeId, _materialId, _itemId, _gestorId, _almoxarifadoId, _naturezaId, _SubItemcodigo, _Itemcodigo, _indicadorId, _saldoId);
            }
            else
            {
                resultadoAlmox = resultadoAlmoxSaldo(_grupoId, _classeId, _materialId, _itemId, _gestorId, _almoxarifadoId, _naturezaId, _SubItemcodigo, _Itemcodigo, _indicadorId, _saldoId);
            }




            return resultadoAlmox;


        }
        #endregion

        public bool atualizarAlmoxSaldo(int _gestorId, int _almoxarifadoId, bool _indicadorDisponivel)
        {
            int indicadorDispNao = (int)GeralEnum.IndicadorDisponivel.Nao;
            int indicadorDisp = (int)GeralEnum.IndicadorDisponivel.Sim;
            bool erro = false;

            IList<SubItemMaterialAlmoxEntity> almoxSaldo = null;
            var resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  //into subItemDefault
                                  //from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  //into itemMaterialDefault
                                  //from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  //into almoxDefault
                                  //from almox in almoxDefault.DefaultIfEmpty()
                                  join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID, a = _almoxarifadoId } equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, a = saldo.TB_ALMOXARIFADO_ID }
                                  //into materialDefault
                                  //from Material in materialDefault.DefaultIfEmpty()
                                  //join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID


                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  //where (saldo.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
                                  //where (almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == ((_indicadorDisponivel == true) ? indicadorDispNao : indicadorDisp))
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending


                                  select new SubItemMaterialAlmoxEntity
                                  {
                                      Id = almox.TB_SUBITEM_MATERIAL_ALMOX_ID

                                  }).Distinct();

            almoxSaldo = resultadoAlmox.ToList();

            TB_SUBITEM_MATERIAL_ALMOX rowSubItemAlmoxItem = null;
            EntitySet<TB_SUBITEM_MATERIAL_ALMOX> listSubItemAlmoxItem = new EntitySet<TB_SUBITEM_MATERIAL_ALMOX>();
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                try
                {

                    foreach (var item in resultadoAlmox)
                    {
                        int id = Convert.ToInt32(item.Id);
                        rowSubItemAlmoxItem = this.Db.TB_SUBITEM_MATERIAL_ALMOXes.Where(a => a.TB_SUBITEM_MATERIAL_ALMOX_ID == id).FirstOrDefault();

                        rowSubItemAlmoxItem.TB_INDICADOR_ITEM_SALDO_ZERADO_ID = ((_indicadorDisponivel == true) ? indicadorDisp : indicadorDispNao);

                        listSubItemAlmoxItem.Add(rowSubItemAlmoxItem);
                    }
                    this.Db.SubmitChanges();
                    erro = true;
                }
                catch (Exception)
                {
                    erro = false;
                    throw new Exception("Erro ao atualizar dados.");
                }

                ts.Complete();
            }

            return erro;

        }

        public IList<SubItemMaterialEntity> resultadoAlmoxSaldo(int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorId, int _almoxarifadoId, int _naturezaId, Int64? _SubItemcodigo, Int64? _Itemcodigo, int _indicadorId, int _saldoId)
        {

            List<SubItemMaterialEntity> resultadoAlmox;

            if (_SubItemcodigo != null)
            {

                #region EntityFramework
                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID 
                                  equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } 
                                  equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID 
                                  equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID
                                  equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID 
                                  equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID

                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } 
                                  equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()

                                  join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID } 
                                  equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID }
                                  into dadosSaldoSubitemMaterial
                                  from saldo in dadosSaldoSubitemMaterial.DefaultIfEmpty()

                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where (subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo)
                                  //trecho abaixo faz retornar apenas subitens com saldo e ativos
                                  //where (saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true)

                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending
                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      
                                      SaldoSubItemUnit = (saldo.TB_SALDO_SUBITEM_SALDO_QTDE != null ? saldo.TB_SALDO_SUBITEM_SALDO_QTDE.Value : 0.00m),
                                      //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                      //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),

                                      IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 0 ? false : false
                                      || almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 2 ? true : true,

                                      //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)

                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();


                #endregion
                // string conn = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                #region SqlCruConsulta
                //string conn = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;

                //using (SqlConnection openCon = new SqlConnection(conn))
                //{
                //    StringBuilder sb = new StringBuilder();
                //    sb.Append("SELECT[t1].[TB_SUBITEM_MATERIAL_CODIGO] AS[Codigo], [t1].[TB_SUBITEM_MATERIAL_DESCRICAO]");
                //    sb.Append(" AS [Descricao],");
                //    sb.Append(" (CASE");
                //    sb.Append(" WHEN([t8].[TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE]) IS NULL THEN 0");
                //    sb.Append(" ELSE CONVERT(Int, [t8].[TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE])");
                //    sb.Append(" END) AS[IndicadorAtividadeAlmox], ");
                //    sb.Append(" (CASE");
                //    sb.Append("  WHEN([t12].[TB_INDICADOR_DISPONIVEL_ID]) IS NULL THEN 0");
                //    sb.Append("  ELSE CONVERT(Int, CONVERT(Bit, [t12].[TB_INDICADOR_DISPONIVEL_ID]))");
                //    sb.Append("  END) AS[IndicadorDisponivel], ");
                //    sb.Append("  (CASE");
                //    sb.Append(" WHEN[t8].[TB_INDICADOR_ITEM_SALDO_ZERADO_ID] = 2 THEN 2");
                //    sb.Append(" WHEN[t8].[TB_INDICADOR_ITEM_SALDO_ZERADO_ID] = 0 THEN 0");
                //    sb.Append(" ELSE 2");
                //    sb.Append(" END) AS[IndicadorDisponivelZerado],");
                //    sb.Append(" (CASE");
                //    sb.Append(" WHEN[t8].[TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX]");
                //    sb.Append(" IS NULL THEN CONVERT(Decimal(33,4),0.00)");
                //    sb.Append(" ELSE CONVERT(Decimal(33,4),[t8].[TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX])");
                //    sb.Append(" END) AS[EstoqueMaximo],");
                //    sb.Append(" (CASE");
                //    sb.Append(" WHEN[t8].[TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN]");
                //    sb.Append(" IS NULL THEN CONVERT(Decimal(33,4),0.00)");
                //    sb.Append(" ELSE CONVERT(Decimal(33,4),[t8].[TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN])");
                //    sb.Append(" END) AS[EstoqueMinimo], [t10].[TB_SALDO_SUBITEM_SALDO_QTDE]");
                //    sb.Append(" AS[SaldoSubItemUnit], [t7].[TB_UNIDADE_FORNECIMENTO_CODIGO]");
                //    sb.Append(" AS[CodigoUnidadeFornec], CONVERT(NVarChar, [t6].[TB_NATUREZA_DESPESA_CODIGO]) AS[CodigoNaturezaDesp],");
                //    sb.Append(" [t1].[TB_SUBITEM_MATERIAL_ID]  AS[Id]");

                //    sb.Append(" FROM[dbo].[TB_ITEM_SUBITEM_MATERIAL]");
                //    sb.Append(" AS[t0]");
                //    sb.Append(" LEFT OUTER JOIN[dbo].[TB_SUBITEM_MATERIAL]");
                //    sb.Append(" AS[t1] ON[t0].[TB_SUBITEM_MATERIAL_ID] = [t1].[TB_SUBITEM_MATERIAL_ID]");
                //    sb.Append(" LEFT OUTER JOIN[dbo].[TB_ITEM_MATERIAL]");
                //    sb.Append(" AS[t2] ON[t0].[TB_ITEM_MATERIAL_ID] = [t2].[TB_ITEM_MATERIAL_ID]");
                //    sb.Append(" INNER JOIN[dbo].[TB_MATERIAL]");
                //    sb.Append(" AS[t3] ON[t2].[TB_MATERIAL_ID] = [t3].[TB_MATERIAL_ID]");
                //    sb.Append(" INNER JOIN[dbo].[TB_CLASSE_MATERIAL]");
                //    sb.Append(" AS[t4] ON[t3].[TB_CLASSE_MATERIAL_ID] = [t4].[TB_CLASSE_MATERIAL_ID]");
                //    sb.Append(" INNER JOIN[dbo].[TB_GRUPO_MATERIAL]");
                //    sb.Append(" AS[t5] ON[t4].[TB_GRUPO_MATERIAL_ID] = [t5].[TB_GRUPO_MATERIAL_ID]");
                //    sb.Append(" INNER JOIN[dbo].[TB_NATUREZA_DESPESA]");
                //    sb.Append(" AS[t6] ON[t1].[TB_NATUREZA_DESPESA_ID] = [t6].[TB_NATUREZA_DESPESA_ID]");
                //    sb.Append(" INNER JOIN[dbo].[TB_UNIDADE_FORNECIMENTO]");
                //    sb.Append(" AS[t7] ON[t1].[TB_UNIDADE_FORNECIMENTO_ID] = [t7].[TB_UNIDADE_FORNECIMENTO_ID]");
                //    sb.Append(" LEFT OUTER JOIN([dbo].[TB_SUBITEM_MATERIAL_ALMOX] AS [t8]");
                //    sb.Append(" INNER JOIN[dbo].[TB_SUBITEM_MATERIAL] AS[t9] ON [t9].[TB_SUBITEM_MATERIAL_ID] = [t8].[TB_SUBITEM_MATERIAL_ID])");
                //    sb.Append(" ON([t1].[TB_SUBITEM_MATERIAL_ID] = [t9].[TB_SUBITEM_MATERIAL_ID]) AND(456 = [t8].[TB_ALMOXARIFADO_ID])");
                //    sb.Append(" INNER JOIN[dbo].[TB_SALDO_SUBITEM]");
                //    sb.Append(" AS[t10] ON([t1].[TB_SUBITEM_MATERIAL_ID] = ((");
                //    sb.Append(" SELECT [t11].[TB_SUBITEM_MATERIAL_ID]");
                //    sb.Append(" FROM[dbo].[TB_SUBITEM_MATERIAL] AS [t11]");
                //    sb.Append(" WHERE [t11].[TB_SUBITEM_MATERIAL_ID] = [t10].[TB_SUBITEM_MATERIAL_ID]");
                //    sb.Append(" ))) AND(" + _almoxarifadoId + " = [t10].[TB_ALMOXARIFADO_ID])");
                //    sb.Append(" LEFT OUTER JOIN[dbo].[TB_INDICADOR_DISPONIVEL] AS[t12]");
                //    sb.Append(" ON[t12].[TB_INDICADOR_DISPONIVEL_ID] = [t8].[TB_INDICADOR_DISPONIVEL_ID]");
                //    sb.Append(" WHERE(([t1].[TB_SUBITEM_MATERIAL_CODIGO]) = " + _SubItemcodigo + ") AND([t1].[TB_GESTOR_ID] = " + _gestorId + ")");
                //    sb.Append(" ORDER BY[t8].[TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE], [t1].[TB_SUBITEM_MATERIAL_DESCRICAO]");

                //    System.Data.SqlClient.SqlDataReader dr;
                //    using (SqlCommand cuery = new SqlCommand(sb.ToString()))
                //    {
                //        cuery.Connection = openCon;
                //        openCon.Open();
                //        dr = cuery.ExecuteReader();

                //    }
                //    List<SubItemMaterialEntity> lst = new List<SubItemMaterialEntity>();
                //    while (dr.Read())
                //    {
                //        SubItemMaterialEntity item = new SubItemMaterialEntity();
                //        item.Descricao = dr["Descricao"].ToString();
                //        item.Id = Convert.ToInt32(dr["Id"]);
                //        item.Codigo = Convert.ToInt64(dr["Codigo"].ToString());
                //        item.SaldoSubItemUnit = Convert.ToDecimal(dr["SaldoSubItemUnit"].ToString());
                //        // ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                //       // item.IndicadorAtividade = Convert.ToBoolean(dr["IndicadorAtividade"].ToString() == null ? false : Convert.ToBoolean(dr["IndicadorAtividade"].ToString()));
                //        //item.ItemMaterial = Convert.ToDecimal(dr["SaldoSubItemUnit"].ToString());
                //        item.EstoqueMaximo = Convert.ToDecimal(dr["EstoqueMaximo"].ToString());
                //        item.EstoqueMinimo = Convert.ToDecimal(dr["EstoqueMinimo"].ToString());
                //        //IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                //        item.IndicadorAtividadeAlmox = Convert.ToBoolean(dr["IndicadorAtividadeAlmox"].ToString() == "0" ? false : true);//: Convert.ToBoolean(dr["IndicadorAtividadeAlmox"].ToString()));
                //        //item.IndicadorDisponivelId = Convert.ToInt32(dr["IndicadorDisponivelId"] == "0" ? false : true);// Convert.ToInt32(dr["IndicadorDisponivelId"].ToString()));
                //        //almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                //        //item.IndicadorDisponivelDescricao = dr["IndicadorDisponivelDescricao"].ToString();
                //        item.IndicadorDisponivel = Convert.ToBoolean(dr["IndicadorDisponivel"].ToString() == "0" ? false : true);// Convert.ToBoolean(dr["IndicadorDisponivel"].ToString()));
                //        //IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                //        item.IndicadorDisponivelZerado = Convert.ToBoolean(dr["IndicadorDisponivelZerado"].ToString() == "0" ? false : true);
                //        //IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 0 ? false : false

                //        //|| item.IndicadorDisponivelZerado = Convert.ToBoolean(dr["IndicadorDisponivelZerado"].ToString() == 2 ? true : true,
                //       // item.AlmoxarifadoId = Convert.ToInt32(dr["AlmoxarifadoId"].ToString());
                //        //AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                //        item.CodigoUnidadeFornec = dr["CodigoUnidadeFornec"].ToString();
                //        //CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                //        item.CodigoNaturezaDesp = dr["CodigoUnidadeFornec"].ToString();
                //        //CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)


                //        lst.Add(item);

                //    }

                //    resultadoAlmox = lst;
                //}
                #endregion



            }


            else if (_Itemcodigo != null)
            {
                #region
                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()
                                  join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID, a = _almoxarifadoId }
                                  equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, a = saldo.TB_ALMOXARIFADO_ID }

                                  //into materialDefault
                                  //from Material in materialDefault.DefaultIfEmpty()
                                  //join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID

                                  where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                  where (itemMaterial.TB_MATERIAL_ID == _materialId || _materialId == 0)
                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where (itemMaterial.TB_ITEM_MATERIAL_CODIGO == _Itemcodigo || _Itemcodigo == null)
                                  where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                  where (Material.TB_CLASSE_MATERIAL_ID == _classeId || _classeId == 0)
                                  where (Classe.TB_GRUPO_MATERIAL_ID == _grupoId || _grupoId == 0)
                                  //where (_saldoId == 1 ? saldo.TB_SALDO_SUBITEM_SALDO_QTDE > 0 : saldo.TB_SALDO_SUBITEM_SALDO_QTDE == 0)

                                  //where (Material.TB_MATERIAL_ATIVO == true)
                                  // where (Classe.TB_MATERIAL_ATIVO == true)
                                  where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID ascending
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                      //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                      //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)
                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();
                #endregion

            }
            else
            {
                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID

                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()

                                  join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID, a = _almoxarifadoId }
                                  equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, a = saldo.TB_ALMOXARIFADO_ID }

                                  where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                  where ((itemMaterial.TB_MATERIAL_ID == _materialId) || (_materialId == 0))
                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where ((Material.TB_CLASSE_MATERIAL_ID == _classeId) || (_classeId == 0))
                                  where ((Classe.TB_GRUPO_MATERIAL_ID == _grupoId) || (_grupoId == 0))
                                  //where (_saldoId == -1 ? saldo.TB_SALDO_SUBITEM_SALDO_QTDE == 39 : saldo.TB_SALDO_SUBITEM_SALDO_QTDE == 39)
                                  //where (Material.TB_MATERIAL_ATIVO == true)
                                  //where (Classe.TB_MATERIAL_ATIVO == true)
                                  where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                  where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                  //orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID ascending
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending

                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      SaldoSubItemUnit = saldo.TB_SALDO_SUBITEM_SALDO_QTDE,
                                     
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,

                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE.Equals(null) ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID.Equals(null)? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                      IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 0 ? false : false
                                      || almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 2 ? true : true,
                                      
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)
                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();

            }
            this.totalregistros = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                   join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                   into subItemDefault
                                   from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                   join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                   into itemMaterialDefault
                                   from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                   join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                   join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                   join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                   join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                   join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                   join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                   into almoxDefault
                                   from almox in almoxDefault.DefaultIfEmpty()

                                   join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID, a = _almoxarifadoId } 
                                   equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID, a = saldo.TB_ALMOXARIFADO_ID }
                                   into saudotemp
                                   from saldo in almoxDefault.DefaultIfEmpty()





                                   where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                   where ((itemMaterial.TB_MATERIAL_ID == _materialId) || (_materialId == 0))
                                   where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                   where ((subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo) || (_SubItemcodigo == null))
                                   where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                   where ((Material.TB_CLASSE_MATERIAL_ID == _classeId) || (_classeId == 0))
                                   where ((Classe.TB_GRUPO_MATERIAL_ID == _grupoId) || (_grupoId == 0))
                                   //where (_saldoId == 1 ? saldo.TB_SALDO_SUBITEM_SALDO_QTDE > 0 : saldo.TB_SALDO_SUBITEM_SALDO_QTDE == 0)
                                   where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                   //where (Material.TB_MATERIAL_ATIVO == true)
                                   orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                   orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending
                                   select new
                                   {
                                       subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                   }).Distinct().Count();



            #region PegaQuery
            IQueryable query = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                into subItemDefault
                                from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                into itemMaterialDefault
                                from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID

                                join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId }
                                equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                into almoxDefault
                                from almox in almoxDefault.DefaultIfEmpty()

                                join saldo in this.Db.TB_SALDO_SUBITEMs on new { s = subItemMaterial.TB_SUBITEM_MATERIAL_ID }
                                equals new { s = saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID }

                                where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                where (subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo)
                                where (saldo.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true)

                                orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending
                                select new SubItemMaterialEntity
                                {
                                    Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                    Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                    Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                    SaldoSubItemUnit = saldo.TB_SALDO_SUBITEM_SALDO_QTDE,
                                    //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                    //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                    //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                    EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                    EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                    IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                    //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                    //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                    IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),

                                    IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 0 ? false : false
                                    || almox.TB_INDICADOR_ITEM_SALDO_ZERADO_ID == 2 ? true : true,

                                    //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                    CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                    CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)

                                });

            ////var sql = query.ToString();
            #endregion

            return resultadoAlmox;

        }

        public IList<SubItemMaterialEntity> resultadoAlmoxSaldoERRADO(int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorId, int _almoxarifadoId, int _naturezaId, Int64? _SubItemcodigo, Int64? _Itemcodigo, int _indicadorId)
        {

            IList<SubItemMaterialEntity> resultadoAlmox;

            if (_SubItemcodigo != null)
            {
                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()
                                      //implementado 18/03
                                  join saldo in Db.TB_SALDO_SUBITEMs on new { subItemMaterial.TB_SUBITEM_MATERIAL_ID, almox.TB_ALMOXARIFADO_ID } equals new { saldo.TB_SUBITEM_MATERIAL_ID, saldo.TB_ALMOXARIFADO_ID }
                                  into saldoDefault
                                  from saldo in saldoDefault.DefaultIfEmpty()
                                      //into materialDefault
                                      //from Material in materialDefault.DefaultIfEmpty()
                                      //join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID

                                  where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                  where (itemMaterial.TB_MATERIAL_ID == _materialId || _materialId == 0)
                                  // where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where (subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo || _SubItemcodigo == null)
                                  where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                  where (Material.TB_CLASSE_MATERIAL_ID == _classeId || _classeId == 0)
                                  where (Classe.TB_GRUPO_MATERIAL_ID == _grupoId || _grupoId == 0)
                                  where (almox.TB_ALMOXARIFADO_ID == _almoxarifadoId)
                                  where (saldo.TB_ALMOXARIFADO_ID == _almoxarifadoId)
                                  //where (Material.TB_MATERIAL_ATIVO == true)
                                  // where (Classe.TB_MATERIAL_ATIVO == true)
                                  where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID descending
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                      //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                      IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID),
                                      //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)

                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();
                if (resultadoAlmox.Count == 0)
                {
                    resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                      join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                      into subItemDefault
                                      from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                      join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                      into itemMaterialDefault
                                      from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                      join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                      join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                      join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                      join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                      join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                      join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                      into almoxDefault
                                      from almox in almoxDefault.DefaultIfEmpty()

                                          //into materialDefault
                                          //from Material in materialDefault.DefaultIfEmpty()
                                          //join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID

                                      where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                      where (itemMaterial.TB_MATERIAL_ID == _materialId || _materialId == 0)
                                      where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                      where (subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo || _SubItemcodigo == null)
                                      where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                      where (Material.TB_CLASSE_MATERIAL_ID == _classeId || _classeId == 0)
                                      where (Classe.TB_GRUPO_MATERIAL_ID == _grupoId || _grupoId == 0)

                                      //where (Material.TB_MATERIAL_ATIVO == true)
                                      // where (Classe.TB_MATERIAL_ATIVO == true)
                                      where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                      orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID descending
                                      orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                      select new SubItemMaterialEntity
                                      {
                                          Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                          Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                          Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                          //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                          //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                          //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                          EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                          EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                          IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                          //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                          //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                          IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                          IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID),
                                          //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                          CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                          CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)

                                      }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();
                }
            }
            else if (_Itemcodigo != null)
            {

                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()

                                      //into materialDefault
                                      //from Material in materialDefault.DefaultIfEmpty()
                                      //join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID

                                  where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                  where (itemMaterial.TB_MATERIAL_ID == _materialId || _materialId == 0)
                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where (itemMaterial.TB_ITEM_MATERIAL_CODIGO == _Itemcodigo || _Itemcodigo == null)
                                  where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                  where (Material.TB_CLASSE_MATERIAL_ID == _classeId || _classeId == 0)
                                  where (Classe.TB_GRUPO_MATERIAL_ID == _grupoId || _grupoId == 0)
                                  //Implementado para trazer apenas os ativos
                                  // where (itemMaterial.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE==true)

                                  //where (Material.TB_MATERIAL_ATIVO == true)
                                  // where (Classe.TB_MATERIAL_ATIVO == true)
                                  where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID descending
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending

                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,
                                      //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                      //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                      IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID),
                                      //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)
                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();

                //Implementado para não trazer inativos               
                // resultadoAlmox = resultadoAlmox.GroupBy(x => x.ItemMaterialCodigo).Select(x => x.FirstOrDefault()).ToList();
                // resultadoAlmox = resultadoAlmox.Where(x => x.IndicadorAtividadeAlmox == true).Take(this.RegistrosPagina).Distinct().ToList();

            }
            else
            {
                resultadoAlmox = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
                                  join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
                                  into subItemDefault
                                  from subItemMaterial in subItemDefault.DefaultIfEmpty()
                                  join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                                  into itemMaterialDefault
                                  from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
                                  join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
                                  join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
                                  join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
                                  join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
                                  join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }

                                  into almoxDefault
                                  from almox in almoxDefault.DefaultIfEmpty()

                                  where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
                                  where ((itemMaterial.TB_MATERIAL_ID == _materialId) || (_materialId == 0))
                                  where (subItemMaterial.TB_GESTOR_ID == _gestorId)
                                  where ((Material.TB_CLASSE_MATERIAL_ID == _classeId) || (_classeId == 0))
                                  where ((Classe.TB_GRUPO_MATERIAL_ID == _grupoId) || (_grupoId == 0))

                                  //where (Material.TB_MATERIAL_ATIVO == true)
                                  //where (Classe.TB_MATERIAL_ATIVO == true)
                                  where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
                                  where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_ID ascending
                                  orderby subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO ascending
                                  orderby almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE ascending

                                  select new SubItemMaterialEntity
                                  {
                                      Id = subItemMaterial.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO,
                                      Descricao = subItemMaterial.TB_SUBITEM_MATERIAL_DESCRICAO,

                                      //  ContaAuxiliar = (new ContaAuxiliarEntity(subItemMaterial.TB_CONTA_AUXILIAR_ID)),
                                      //   IndicadorAtividade = subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == null ? false : subItemMaterial.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                      //   ItemMaterial = (new ItemMaterialEntity(itemSubItemMaterial.TB_ITEM_MATERIAL_ID)),
                                      EstoqueMaximo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX.Value,
                                      EstoqueMinimo = almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN == null ? 0 : almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN.Value,
                                      IndicadorAtividadeAlmox = almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == null ? false : almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE,
                                      //   IndicadorDisponivelId = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? 0 : almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID,
                                      //   IndicadorDisponivelDescricao = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_DESCRICAO,
                                      IndicadorDisponivel = almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID),
                                      IndicadorDisponivelZerado = almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID == null ? false : Convert.ToBoolean(almox.TB_INDICADOR_ITEM_SALDO_ZERADO.TB_INDICADOR_DISPONIVEL_ID),
                                      //   AlmoxarifadoId = almox.TB_ALMOXARIFADO_ID == null ? 0 : almox.TB_ALMOXARIFADO_ID,
                                      CodigoUnidadeFornec = UnidForn.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                      CodigoNaturezaDesp = Convert.ToString(NatDesp.TB_NATUREZA_DESPESA_CODIGO)
                                  }).Skip(this.SkipRegistros).Take(this.RegistrosPagina).Distinct().ToList();
            }

            this.totalregistros = resultadoAlmox.Count;
            //this.totalregistros = (from itemSubItemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs
            //                       join subItemMaterial in this.Db.TB_SUBITEM_MATERIALs on itemSubItemMaterial.TB_SUBITEM_MATERIAL_ID equals subItemMaterial.TB_SUBITEM_MATERIAL_ID
            //                       into subItemDefault
            //                       from subItemMaterial in subItemDefault.DefaultIfEmpty()
            //                       join itemMaterial in this.Db.TB_ITEM_MATERIALs on itemSubItemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
            //                       into itemMaterialDefault
            //                       from itemMaterial in itemMaterialDefault.DefaultIfEmpty()
            //                       join Material in this.Db.TB_MATERIALs on itemMaterial.TB_MATERIAL_ID equals Material.TB_MATERIAL_ID
            //                       join Classe in this.Db.TB_CLASSE_MATERIALs on new { s = Material.TB_CLASSE_MATERIAL_ID } equals new { s = Classe.TB_CLASSE_MATERIAL_ID }
            //                       join Grupo in this.Db.TB_GRUPO_MATERIALs on Classe.TB_GRUPO_MATERIAL_ID equals Grupo.TB_GRUPO_MATERIAL_ID
            //                       join NatDesp in this.Db.TB_NATUREZA_DESPESAs on subItemMaterial.TB_NATUREZA_DESPESA_ID equals NatDesp.TB_NATUREZA_DESPESA_ID
            //                       join UnidForn in this.Db.TB_UNIDADE_FORNECIMENTOs on subItemMaterial.TB_UNIDADE_FORNECIMENTO_ID equals UnidForn.TB_UNIDADE_FORNECIMENTO_ID
            //                       join almox in this.Db.TB_SUBITEM_MATERIAL_ALMOXes on new { s = subItemMaterial, a = _almoxarifadoId } equals new { s = almox.TB_SUBITEM_MATERIAL, a = almox.TB_ALMOXARIFADO_ID }
            //                       into almoxDefault
            //                       from almox in almoxDefault.DefaultIfEmpty()

            //                       where ((itemSubItemMaterial.TB_ITEM_MATERIAL_ID == _itemId) || (_itemId == 0))
            //                       where ((itemMaterial.TB_MATERIAL_ID == _materialId) || (_materialId == 0))
            //                      // where (subItemMaterial.TB_GESTOR_ID == _gestorId)
            //                       where ((subItemMaterial.TB_SUBITEM_MATERIAL_CODIGO == _SubItemcodigo) || (_SubItemcodigo == null))
            //                       where ((subItemMaterial.TB_NATUREZA_DESPESA_ID == _naturezaId || _naturezaId == 0))
            //                       where ((Material.TB_CLASSE_MATERIAL_ID == _classeId) || (_classeId == 0))
            //                       where ((Classe.TB_GRUPO_MATERIAL_ID == _grupoId) || (_grupoId == 0))

            //                       where (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == Convert.ToBoolean(_indicadorId) || _indicadorId == -1)
            //                       //where (Material.TB_MATERIAL_ATIVO == true)
            //                       select new
            //                       {
            //                           subItemMaterial.TB_SUBITEM_MATERIAL_ID
            //                       }).Distinct().Count();
            return resultadoAlmox;

        }

        public IList<SubItemMaterialEntity> ListarSubItensMaterialAlmoxByNatDespesaItem(int almoxarifado, int itemId, int naturezaId)
        {
            IList<SubItemMaterialEntity> resultado = (from s in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                      join it in Db.TB_ITEM_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals it.TB_SUBITEM_MATERIAL_ID
                                                      join nat in Db.TB_ITEM_NATUREZA_DESPESAs on it.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID equals nat.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID
                                                      where (s.TB_ALMOXARIFADO_ID.Equals(almoxarifado))
                                                      where (it.TB_ITEM_MATERIAL_ID.Equals(itemId))
                                                      where nat.TB_NATUREZA_DESPESA_ID.Equals(naturezaId)
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                          CodigoFormatado = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                          Descricao = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      }
                                                   ).Distinct().ToList();
            return resultado;
        }

        public IList<SubItemMaterialEntity> Listar(System.Linq.Expressions.Expression<Func<SubItemMaterialEntity, bool>> where)
        {
            IQueryable<SubItemMaterialEntity> resultado = (from a in Db.TB_ITEM_SUBITEM_MATERIALs
                                                           select new SubItemMaterialEntity
                                                           {
                                                               Id = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                               CodigoFormatado = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                               Codigo = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                               Descricao = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                               CodigoBarras = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_COD_BARRAS,
                                                               ContaAuxiliar = new ContaAuxiliarEntity
                                                               {
                                                                   // Id = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_ID,
                                                                   //   Codigo = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_CODIGO,
                                                                   //   Descricao = a.TB_SUBITEM_MATERIAL.TB_CONTA_AUXILIAR.TB_CONTA_AUXILIAR_DESCRICAO
                                                               },
                                                               Gestor = new GestorEntity
                                                               {
                                                                   Id = a.TB_GESTOR_ID
                                                               },
                                                               IndicadorAtividade = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                               IsDecimal = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DECIMOS,
                                                               IsFracionado = (bool)a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_FRACIONA,
                                                               //IsLote = a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_LOTE,
                                                               NaturezaDespesa = new NaturezaDespesaEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO
                                                               },
                                                               ItemMaterial = new ItemMaterialEntity
                                                               {
                                                                   Id = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                   Codigo = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                   CodigoFormatado = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO.ToString().PadLeft(9, '0'),
                                                                   Descricao = a.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO
                                                               },
                                                               UnidadeFornecimento = new UnidadeFornecimentoEntity
                                                               {
                                                                   Id = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                   Codigo = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO,
                                                                   Descricao = a.TB_SUBITEM_MATERIAL.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO
                                                               }
                                                           }
                                                    );

            if (this.Entity.Codigo != null && this.Entity.Codigo != 0)
                resultado = resultado.Where(a => a.Codigo == this.Entity.Codigo);

            if (this.Entity.ItemMaterial != null && this.Entity.ItemMaterial.Codigo != 0)
                resultado = resultado.Where(a => a.ItemMaterial.Codigo == this.Entity.ItemMaterial.Codigo);

            return resultado.ToList();
        }

        private void ListarCatalogoGestorDefinirFiltro(int gestorId, int naturezaDespesaCodigo, int itemCod, long subitemCod, out Expression<Func<TB_SUBITEM_MATERIAL, bool>> expWhere, out Expression<Func<TB_NATUREZA_DESPESA, bool>> expWhereNaturezaDespesa)
        {
            int itemMaterialID = 0;
            bool statusAtivo = true;

            if (itemCod > 0)
                itemMaterialID = Db.TB_ITEM_MATERIALs.FirstOrDefault(x => x.TB_ITEM_MATERIAL_ID == itemCod).TB_ITEM_MATERIAL_ID;

            if (naturezaDespesaCodigo != 0 && subitemCod != 0 && itemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                                                           && naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);

                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                            && subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO == subitemCod
                            && subitemMaterial.TB_ITEM_SUBITEM_MATERIALs.FirstOrDefault(x => x.TB_ITEM_MATERIAL_ID == itemMaterialID).TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == itemCod);
            }
            else if (naturezaDespesaCodigo != 0 && subitemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                                                           && naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);

                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                            && subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO == subitemCod);
            }
            else if (naturezaDespesaCodigo != 0 && itemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                                                           && naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);

                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                            && subitemMaterial.TB_ITEM_SUBITEM_MATERIALs.FirstOrDefault(x => x.TB_ITEM_MATERIAL_ID == itemMaterialID).TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == itemCod);
            }
            else if (subitemCod != 0 && itemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);

                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO == subitemCod
                            && subitemMaterial.TB_ITEM_SUBITEM_MATERIALs.FirstOrDefault(x => x.TB_ITEM_MATERIAL_ID == itemMaterialID).TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == itemCod);
            }
            else if (naturezaDespesaCodigo != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo
                                                           && naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);

                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == naturezaDespesaCodigo);
            }
            else if (subitemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);
                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_SUBITEM_MATERIAL_CODIGO == subitemCod);
            }
            else if (itemCod != 0)
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);
                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId
                            && subitemMaterial.TB_ITEM_SUBITEM_MATERIALs.FirstOrDefault(x => x.TB_ITEM_MATERIAL_ID == itemMaterialID).TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO == itemCod);
            }
            else
            {
                expWhereNaturezaDespesa = (naturezaDespesa => naturezaDespesa.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == statusAtivo);
                expWhere = (subitemMaterial => subitemMaterial.TB_GESTOR_ID == gestorId);
            }

        }

        public IList<SubItemMaterialEntity> ListarCatalogoGestor(int gestorId, int naturezaDespesaCodigo, int itemCod, long subitemCod)
        {
            Db.DeferredLoadingEnabled = true;
            string strSQL = null;

            IList<SubItemMaterialEntity> lstRetorno = null;
            IQueryable<TB_NATUREZA_DESPESA> qryNaturezaDespesa = null;
            IQueryable<TB_SUBITEM_MATERIAL> qrySubitemMaterial = null;
            IQueryable<TB_SUBITEM_MATERIAL> qryConsulta = null;

            Expression<Func<TB_NATUREZA_DESPESA, bool>> expWhereNaturezaDespesa = null;
            Expression<Func<TB_SUBITEM_MATERIAL, bool>> expWhere = null;

            ListarCatalogoGestorDefinirFiltro(gestorId, naturezaDespesaCodigo, itemCod, subitemCod, out expWhere, out expWhereNaturezaDespesa);


            if (expWhereNaturezaDespesa.IsNotNull())
                qryNaturezaDespesa = Db.TB_NATUREZA_DESPESAs.Where(expWhereNaturezaDespesa);

            if (expWhere.IsNotNull())
                qrySubitemMaterial = Db.TB_SUBITEM_MATERIALs.Where(expWhere);

            if (qryNaturezaDespesa.Count() != 0 || qrySubitemMaterial.Count() != 0)
            {
                qryConsulta = (from subitemMaterial in this.Db.TB_SUBITEM_MATERIALs
                               join itemSubitemMaterial in Db.TB_ITEM_SUBITEM_MATERIALs on subitemMaterial.TB_SUBITEM_MATERIAL_ID equals itemSubitemMaterial.TB_SUBITEM_MATERIAL_ID
                               join itemMaterial in Db.TB_ITEM_MATERIALs on itemSubitemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterial.TB_ITEM_MATERIAL_ID
                               select subitemMaterial).Where(expWhere)
                                                      .Distinct()
                                                      .AsQueryable<TB_SUBITEM_MATERIAL>();

                lstRetorno = new List<SubItemMaterialEntity>();
                qryConsulta.ToList().ForEach(rowTabela => lstRetorno.Add(
                                                                            new SubItemMaterialEntity
                                                                            {
                                                                                Id = rowTabela.TB_SUBITEM_MATERIAL_ID,
                                                                                Descricao = rowTabela.TB_SUBITEM_MATERIAL_DESCRICAO.Trim(),
                                                                                Codigo = rowTabela.TB_SUBITEM_MATERIAL_CODIGO,
                                                                                IndicadorAtividade = rowTabela.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE,
                                                                                ItemMaterial = (rowTabela.TB_ITEM_SUBITEM_MATERIALs.Where(itemSubitemMaterial => itemSubitemMaterial.TB_SUBITEM_MATERIAL_ID == rowTabela.TB_SUBITEM_MATERIAL_ID)
                                                                                                                                   .Select(itemMaterial => new ItemMaterialEntity()
                                                                                                                                   {
                                                                                                                                       Id = itemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID,
                                                                                                                                       Codigo = itemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO,
                                                                                                                                       Descricao = itemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_DESCRICAO,
                                                                                                                                       Atividade = itemMaterial.TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_INDICADOR_ATIVIDADE
                                                                                                                                   }).FirstOrDefault()),
                                                                                NaturezaDespesa = new NaturezaDespesaEntity()
                                                                                {
                                                                                    Id = rowTabela.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_ID,
                                                                                    Codigo = rowTabela.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO,
                                                                                    Descricao = rowTabela.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_DESCRICAO,
                                                                                },
                                                                                UnidadeFornecimento = new UnidadeFornecimentoEntity()
                                                                                {
                                                                                    Id = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_ID,
                                                                                    Codigo = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO.Trim(),
                                                                                    Descricao = rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO.Trim(),
                                                                                    CodigoDescricao = String.Format("{0} - {1}", rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO.Trim(), rowTabela.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO.Trim())
                                                                                }
                                                                            }));

                lstRetorno = lstRetorno.OrderBy(subitemMaterialOrdenado => subitemMaterialOrdenado.Descricao)
                                       .ThenBy(subitemMaterialOrdenado => subitemMaterialOrdenado.Codigo)
                                       .ToList();

                this.totalregistros = lstRetorno.Count;

                strSQL = qryConsulta.ToString();
                this.Db.GetCommand(qryConsulta as IQueryable).Parameters.Cast<System.Data.Common.DbParameter>().ToList().ForEach(Parametro => strSQL = strSQL.Replace(Parametro.ParameterName, String.Format("'{0}'", Parametro.Value.ToString())));
            }

            return lstRetorno;
        }

        public IList<int> obterSubitemMaterial_IDs__NaturezasDespesa_ConsumoImediato()
        {
            IQueryable<TB_SUBITEM_MATERIAL> qryConsulta = null;
            IList<int> lstRetorno = null;


            int[] codigosGrupoMaterialSIAFISICO = new int[] { 89, 91 };
            string[] codigosInicioNaturezaDespesa = new string[] { "3390" };
            qryConsulta =
                          //(from naturezaDespesa in this.Db.TB_NATUREZA_DESPESAs
                          (from subitemMaterial in this.Db.TB_SUBITEM_MATERIALs
                               //join relacaoItemMaterial_NaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs on naturezaDespesa.TB_NATUREZA_DESPESA_ID equals relacaoItemMaterial_NaturezaDespesa.TB_NATUREZA_DESPESA_ID
                           join relacaoItemMaterial_SubitemMaterial in this.Db.TB_ITEM_SUBITEM_MATERIALs on subitemMaterial.TB_SUBITEM_MATERIAL_ID equals relacaoItemMaterial_SubitemMaterial.TB_SUBITEM_MATERIAL_ID
                           join relacaoItemMaterial_NaturezaDespesa in this.Db.TB_ITEM_NATUREZA_DESPESAs on relacaoItemMaterial_SubitemMaterial.TB_ITEM_MATERIAL_ID equals relacaoItemMaterial_NaturezaDespesa.TB_ITEM_MATERIAL_ID
                           join itemMaterialSIAFISICO in this.Db.TB_ITEM_MATERIALs on relacaoItemMaterial_SubitemMaterial.TB_ITEM_MATERIAL_ID equals itemMaterialSIAFISICO.TB_ITEM_MATERIAL_ID
                           join materialSIAFISICO in this.Db.TB_MATERIALs on itemMaterialSIAFISICO.TB_MATERIAL_ID equals materialSIAFISICO.TB_MATERIAL_ID
                           join classeMaterialSIAFISICO in this.Db.TB_CLASSE_MATERIALs on materialSIAFISICO.TB_CLASSE_MATERIAL_ID equals classeMaterialSIAFISICO.TB_CLASSE_MATERIAL_ID
                           join grupoMaterialSIAFISICO in this.Db.TB_GRUPO_MATERIALs on classeMaterialSIAFISICO.TB_GRUPO_MATERIAL_ID equals grupoMaterialSIAFISICO.TB_GRUPO_MATERIAL_ID
                           join naturezaDespesa in this.Db.TB_NATUREZA_DESPESAs on relacaoItemMaterial_NaturezaDespesa.TB_NATUREZA_DESPESA_ID equals naturezaDespesa.TB_NATUREZA_DESPESA_ID
                           //where codigosInicioNaturezaDespesa.Any(codigoInicioNaturezaDespesa => subitemMaterial.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO.ToString().StartsWith(codigoInicioNaturezaDespesa))
                           where codigosGrupoMaterialSIAFISICO.Contains(grupoMaterialSIAFISICO.TB_GRUPO_MATERIAL_CODIGO)
                           select subitemMaterial).AsQueryable();


            lstRetorno = qryConsulta.Select(subitemMaterial => subitemMaterial.TB_SUBITEM_MATERIAL_ID).Cast<int>().ToList();
            lstRetorno = lstRetorno.Distinct().ToList();

            return lstRetorno;
        }
        /// <summary>        
        /// Retorna os itens que estão com estoque máximo/mínimo
        /// </summary>
        /// <param name="filtro">FILTRO DE BUSCA - 1: ESTOQUE MÁXIMO 2: ESTOQUE MÍNIMO</param>
        /// <param name="idAlmoxarifado">Almoxarifado ID</param>
        /// <returns></returns>
        /// 

        public IList<SubItemMaterialEntity> ListarItensEstoque(int filtro, int idAlmoxarifado)
        {
            IList<SubItemMaterialEntity> listagemItem = null;

            if (filtro == 1) ////ESTOQUE MÁXIMO
            {
                listagemItem =
                    (from saldoSub in Db.TB_SALDO_SUBITEMs
                     join almox in Db.TB_SUBITEM_MATERIAL_ALMOXes on saldoSub.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                     join subMat in Db.TB_SUBITEM_MATERIALs on saldoSub.TB_SUBITEM_MATERIAL_ID equals subMat.TB_SUBITEM_MATERIAL_ID
                     join natDesp in Db.TB_NATUREZA_DESPESAs on subMat.TB_NATUREZA_DESPESA_ID equals natDesp.TB_NATUREZA_DESPESA_ID
                     join unidForn in Db.TB_UNIDADE_FORNECIMENTOs on subMat.TB_UNIDADE_FORNECIMENTO_ID equals unidForn.TB_UNIDADE_FORNECIMENTO_ID
                     where ((almox.TB_ALMOXARIFADO_ID == idAlmoxarifado) &&
                            (almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX > 0) &&
                            (saldoSub.TB_SALDO_SUBITEM_SALDO_QTDE >= almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MAX) &&
                            (saldoSub.TB_SALDO_SUBITEM_SALDO_QTDE > 0) &&
                            (subMat.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true) &&
                            (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true) &&
                            (natDesp.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == true) && (saldoSub.TB_ALMOXARIFADO_ID == almox.TB_ALMOXARIFADO_ID))

                     select new SubItemMaterialEntity

                     {
                         Id = subMat.TB_SUBITEM_MATERIAL_ID,
                         Codigo = subMat.TB_SUBITEM_MATERIAL_CODIGO, //Codigo
                         Descricao = subMat.TB_SUBITEM_MATERIAL_DESCRICAO, //Descricao                                                                                 
                         CodigoNaturezaDesp = Convert.ToString(natDesp.TB_NATUREZA_DESPESA_CODIGO),
                         CodigoUnidadeFornec = unidForn.TB_UNIDADE_FORNECIMENTO_CODIGO
                     }).Distinct().ToList<SubItemMaterialEntity>();
            }
            if (filtro == 2) //ESTOQUE MÍNIMO
            {
                listagemItem =
                                 (from saldoSub in Db.TB_SALDO_SUBITEMs
                                  join almox in Db.TB_SUBITEM_MATERIAL_ALMOXes on saldoSub.TB_SUBITEM_MATERIAL_ID equals almox.TB_SUBITEM_MATERIAL_ID
                                  join subMat in Db.TB_SUBITEM_MATERIALs on saldoSub.TB_SUBITEM_MATERIAL_ID equals subMat.TB_SUBITEM_MATERIAL_ID
                                  join natDesp in Db.TB_NATUREZA_DESPESAs on subMat.TB_NATUREZA_DESPESA_ID equals natDesp.TB_NATUREZA_DESPESA_ID
                                  join unidForn in Db.TB_UNIDADE_FORNECIMENTOs on subMat.TB_UNIDADE_FORNECIMENTO_ID equals unidForn.TB_UNIDADE_FORNECIMENTO_ID
                                  where ((almox.TB_ALMOXARIFADO_ID == idAlmoxarifado) &&
                                       (saldoSub.TB_SALDO_SUBITEM_SALDO_QTDE > 0) &&
                                        (almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN > 0) &&
                                        (saldoSub.TB_SALDO_SUBITEM_SALDO_QTDE <= almox.TB_SUBITEM_MATERIAL_ALMOX_ESTOQUE_MIN) &&
                                        (subMat.TB_SUBITEM_MATERIAL_INDICADOR_ATIVIDADE == true) &&
                                        (almox.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true) &&
                                       (natDesp.TB_NATUREZA_DESPESA_INDICADOR_ATIVIDADE == true) && (saldoSub.TB_ALMOXARIFADO_ID == almox.TB_ALMOXARIFADO_ID))
                                  select new SubItemMaterialEntity
                                  {
                                      Id = subMat.TB_SUBITEM_MATERIAL_ID,
                                      Codigo = subMat.TB_SUBITEM_MATERIAL_CODIGO, //Codigo
                                      Descricao = subMat.TB_SUBITEM_MATERIAL_DESCRICAO, //Descricao                                                                                                           
                                      CodigoNaturezaDesp = Convert.ToString(natDesp.TB_NATUREZA_DESPESA_CODIGO),
                                      CodigoUnidadeFornec = unidForn.TB_UNIDADE_FORNECIMENTO_CODIGO
                                  }).Distinct().ToList<SubItemMaterialEntity>();
            }


            return listagemItem;
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmoxarifadoItemNatureza(int almoxarifado, int CodigoItem, string natDespesa)
        {

            IList<SubItemMaterialEntity> resultado = (from s in Db.TB_SUBITEM_MATERIAL_ALMOXes
                                                      join it in Db.TB_ITEM_SUBITEM_MATERIALs on s.TB_SUBITEM_MATERIAL_ID equals it.TB_SUBITEM_MATERIAL_ID
                                                      join item in Db.TB_ITEM_MATERIALs on it.TB_ITEM_MATERIAL_ID equals item.TB_ITEM_MATERIAL_ID
                                                      where (s.TB_ALMOXARIFADO_ID == almoxarifado)
                                                      where (item.TB_ITEM_MATERIAL_CODIGO == CodigoItem)
                                                      where (s.TB_SUBITEM_MATERIAL.TB_NATUREZA_DESPESA.TB_NATUREZA_DESPESA_CODIGO == Convert.ToInt32(natDespesa))
                                                      select new SubItemMaterialEntity
                                                      {
                                                          Id = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_ID,
                                                          Codigo = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO,
                                                          CodigoFormatado = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO.ToString().PadLeft(12, '0'),
                                                          Descricao = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO,
                                                          CodigoDescricao = s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_CODIGO + " - " + s.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO
                                                      }
                                                   ).Distinct().ToList();
            return resultado;
        }

    }
}
