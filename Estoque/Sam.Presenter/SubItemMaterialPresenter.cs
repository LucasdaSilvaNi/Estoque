using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;
using Sam.Infrastructure;
using Sam.Business;
using System.Linq.Expressions;

namespace Sam.Presenter
{

    public class SubItemMaterialPresenter : CrudPresenter<ISubItemMaterialView>
    {
        ISubItemMaterialView view;

        public ISubItemMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public int SubItemId { get; set; }


        public SubItemMaterialPresenter()
        {
        }

        public SubItemMaterialPresenter(ISubItemMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public bool VerificaSubItemUtilizado(int subItemId)
        {
            Domain.Business.SaldoSubItemBusiness business = new Domain.Business.SaldoSubItemBusiness();
            return business.VerificaSubItemUtilizado(subItemId);
        }

        public IList<OrgaoEntity> PopularListaOrgaoTodosCod()
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<OrgaoEntity> lista = estrutura.ListarOrgaosTodosCod();
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public IList<GestorEntity> PopularListaGestorTodosCod(int _orgaoId)
        {
            EstruturaOrganizacionalBusiness estrutura = new EstruturaOrganizacionalBusiness();
            IList<GestorEntity> lista = estrutura.ListarGestorTodosCod(_orgaoId);
            lista = base.FiltrarNivelAcesso(lista);
            return lista;
        }

        public SubItemMaterialEntity CarregarSubItem(long? subItemCodigo, int gestorId)
        {
            try
            {
                if (subItemCodigo == null)
                    return null;

                CatalogoBusiness cat = new CatalogoBusiness();
                cat.SelectSubItemMaterialPorCodigo((long)subItemCodigo, gestorId);
                return cat.SubItemMaterial;
            }
            catch
            {
                List<String> erro = new List<string>();
                erro.Add("Código do Subitem não encontrado");
                this.View.ListaErros = erro;
                return null;
            }
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<SubItemMaterialEntity> PopularDados(int startRowIndexParameterName, int maximumRowsParameterName, int _itemId, int _gestorID)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            //IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterial(_itemId, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterial(_itemId, _gestorID, null);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<SubItemMaterialEntity> PopularDados(int startRowIndexParameterName, int maximumRowsParameterName, int _subItemId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            int _gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            estrutura.SkipRegistros = startRowIndexParameterName;
            //IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterial(_itemId, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value);
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItem(_subItemId, _gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarCatalogoGestor(int gestorId, int? naturezaDespesaCodigo = null, int? itemCodigo = null, long? subitemCodigo = null)
        {
            CatalogoBusiness objBusiness = new CatalogoBusiness();
            IList<SubItemMaterialEntity> lstRetorno = new List<SubItemMaterialEntity>();
            int _naturezaDespesa = 0;
            int _itemCodigo = 0;
            long _subitemCodigo = 0;

            if (naturezaDespesaCodigo.HasValue)
                _naturezaDespesa = naturezaDespesaCodigo.Value;

            if (itemCodigo.HasValue)
                _itemCodigo = itemCodigo.Value;

            if (subitemCodigo.HasValue)
                _subitemCodigo = subitemCodigo.Value;

            lstRetorno = objBusiness.ListarCatalogoGestor(gestorId, _naturezaDespesa, _itemCodigo, _subitemCodigo).ToList();

            return lstRetorno;
        }

        public IList<SubItemMaterialEntity> PopularDados(int? _gestorID, int? _naturezaCodigo = null)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> lista = new List<SubItemMaterialEntity>();
            if (!_naturezaCodigo.HasValue)
            {
                lista = estrutura.ListarSubItemMaterial(a => a.Gestor.Id == _gestorID).OrderBy(a => a.Descricao).ToList();
            }
            else
            {
                lista = estrutura.ListarSubItemMaterial(a => a.Gestor.Id == _gestorID && a.NaturezaDespesa.Codigo == _naturezaCodigo).OrderBy(a => a.Descricao).ToList();
            }

            return lista;
        }




        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<IndicadorDisponivelEntity> PopularDadosIndicadorDisponivel()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<IndicadorDisponivelEntity> retorno = estrutura.ListarIndicadorDisponivelTodosCod();
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int item)
        {
            int almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemSaldoByAlmox(almoxarifado, item);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox()
        {
            int almoxarifado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemSaldoByAlmox(almoxarifado);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubitemMaterialAlmoxarifadoPorNaturezaDespesa(int iNaturezaDespesa_ID)
        {
            int iAlmoxarifado_ID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;

            SubItemMaterialAlmoxBusiness objBusiness = new SubItemMaterialAlmoxBusiness();
            IList<SubItemMaterialEntity> retorno = objBusiness.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(iAlmoxarifado_ID, iNaturezaDespesa_ID);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubitemMaterialAlmoxarifadoPorNaturezaDespesa(int iNaturezaDespesa_ID, int startRowIndex, int maximumRows)
        {
            int iAlmoxarifado_ID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;

            SubItemMaterialAlmoxBusiness objBusiness = new SubItemMaterialAlmoxBusiness();
            IList<SubItemMaterialEntity> retorno = objBusiness.ObterSubItensMaterialAlmoxarifadoPorNaturezaDespesa(iAlmoxarifado_ID, iNaturezaDespesa_ID, startRowIndex, maximumRows);

            this.TotalRegistrosGrid = objBusiness.TotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Retorna a lista de Subitems do catálogo da Divisão
        /// </summary>
        /// <returns>Lista Catálogo Almoxarifado da divisão</returns>
        public IList<TB_SUBITEM_MATERIAL_ALMOX> ListarCatalogoAlmox(int startRowIndexParameterName, int maximumRowsParameterName, int divisaoId)
        {
            if (divisaoId == 0)
                return new List<TB_SUBITEM_MATERIAL_ALMOX>();

            SubItemMaterialAlmoxBusiness business = new SubItemMaterialAlmoxBusiness();
            //TB_SUBITEM_MATERIAL_ALMOX subItemAlmox = new TB_SUBITEM_MATERIAL_ALMOX();

            //Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, string>> sort = a => a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO;



            ////Lista todos os SubItens do almoxarifado ativo

            //Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where = a =>
            //    a.TB_ALMOXARIFADO.TB_DIVISAO.Where( b => b.TB_DIVISAO_ID == divisaoId);
            //    //&& a.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID != 0;


            //Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where = a =>
            //    a.TB_ALMOXARIFADO.TB_DIVISAO.Where(b => b.TB_DIVISAO_ID == divisaoId) > 0
            //    && a.TB_INDICADOR_DISPONIVEL.TB_INDICADOR_DISPONIVEL_ID != 0;

            var result = business.SelectSubItemAlmoxByDivisao(50, startRowIndexParameterName, divisaoId);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }


        /// <summary>
        /// Retorna a lista de Subitems do catálogo da Divisão
        /// </summary>
        /// <returns>Lista Catálogo Almoxarifado da divisão</returns>
        public IList<TB_SUBITEM_MATERIAL_ALMOX> ListarCatalogoAlmox(int divisaoId, string pesquisa)
        {
            if (divisaoId == 0)
                return new List<TB_SUBITEM_MATERIAL_ALMOX>();

            SubItemMaterialAlmoxBusiness business = new SubItemMaterialAlmoxBusiness();
            var result = business.SelectSubItemAlmoxByDivisao(divisaoId, pesquisa);
            return result;
        }

        /// <summary>
        /// Retorna a lista de Subitems do catálogo da Divisão
        /// </summary>
        /// <returns>Lista Catálogo Almoxarifado da divisão</returns>
        public IList<TB_SUBITEM_MATERIAL_ALMOX> ListarCatalogoAlmox(int divisaoId, string pesquisa, bool newMethod = false)
        {
            if (divisaoId == 0)
                return new List<TB_SUBITEM_MATERIAL_ALMOX>();

            SubItemMaterialAlmoxBusiness objBusiness = new SubItemMaterialAlmoxBusiness();
            IList<TB_SUBITEM_MATERIAL_ALMOX> lstRetorno = null;

            lstRetorno = objBusiness.ObterSubItensMaterialAlmoxarifadoPorDivisao(divisaoId, pesquisa);

            return lstRetorno;
        }

        /// <summary>
        /// Retorna a lista de Subitems do catálogo do almoxarifado Logado
        /// </summary>
        /// <returns>Lista Catálogo Almoxarifado</returns>
        public List<TB_SUBITEM_MATERIAL_ALMOX> ListarCatalogoAlmox()
        {
            SubItemMaterialAlmoxBusiness business = new SubItemMaterialAlmoxBusiness();
            TB_SUBITEM_MATERIAL_ALMOX subItemAlmox = new TB_SUBITEM_MATERIAL_ALMOX();

            Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, string>> sort = a => a.TB_SUBITEM_MATERIAL.TB_SUBITEM_MATERIAL_DESCRICAO;

            //Lista todos os SubItens do almoxarifado ativo
            Expression<Func<TB_SUBITEM_MATERIAL_ALMOX, bool>> where = a =>
                a.TB_ALMOXARIFADO_ID == subItemAlmox.TB_ALMOXARIFADO_ID
                && a.TB_SUBITEM_MATERIAL_ALMOX_INDICADOR_ATIVIDADE == true;

            var result = business.SelectWhere(sort, false, where, 0);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }

        public void atualizarAlmoxSaldo(bool _indicadorDisponivel)
        {
            int _almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            int _gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            CatalogoBusiness estrutura = new CatalogoBusiness();
            bool retorno;

            retorno = estrutura.atualizarAlmoxSaldo(_gestorId, _almoxarifadoId, _indicadorDisponivel);

            if (retorno)
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");

            // return retorno;

        }

        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public IList<SubItemMaterialEntity> PopularDadosAlmoxPorMaterial(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, 
            int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, int iNaturezaDespesa_ID, int _indicadorId, int _saldoId, Int64?
            _SubItemcodigo, Int64? _Itemcodigo)
        {
            _almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            _gestorID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
        
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<SubItemMaterialEntity> retorno;

            retorno = estrutura.ListarSubItemAlmox(_grupoId, _classeId, _materialId, _itemId, _gestorID, _almoxarifadoId, iNaturezaDespesa_ID, _SubItemcodigo, _Itemcodigo, _indicadorId, _saldoId);
            //atualizarAlmoxSaldo
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<SubItemMaterialEntity> PopularDadosAlmoxPorMaterial(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int iNaturezaDespesa_ID, int _indicadorId, int _saldoId, Int64? _SubItemcodigo, Int64? _Itemcodigo)
        {
            int _almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            int _gestorID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<SubItemMaterialEntity> retorno;

            retorno = estrutura.ListarSubItemAlmox(_grupoId, _classeId, _materialId, _itemId, _gestorID, _almoxarifadoId, iNaturezaDespesa_ID, _SubItemcodigo, _Itemcodigo, _indicadorId, _saldoId);

            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }


        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<SubItemMaterialEntity> PopularDadosAlmoxPorMaterial(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId)
        {
            _almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            _gestorID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<SubItemMaterialEntity> retorno;

            retorno = estrutura.ListarSubItemAlmox(_materialId, _itemId, _gestorID, _almoxarifadoId);

            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemByAlmoxItemMaterial(int almoxarifado, int itemId, string natDespesa)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            return estrutura.ListarSubItemByAlmoxItemMaterial(almoxarifado, itemId, natDespesa);
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmoxarifadoItemNatureza(int almoxarifado, int itemCodigo, string natDespesa)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            return estrutura.ListarSubItemAlmoxarifadoItemNatureza(almoxarifado, itemCodigo, natDespesa);
        }

        public IList<SubItemMaterialEntity> PopularDadosPorAlmox(int _itemId, int _almoxarifadoId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemPorAlmox(_itemId, _almoxarifadoId);
            return retorno;
        }

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        //public IList<SubItemMaterialEntity> ListarSubItemAlmoxPorPalavraChave(int startRowIndexParameterName, int maximumRowsParameterName, string opcao, string valor, bool FiltrarAlmox, int? divisaoId, bool FiltraGestor)
        //{
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();
        //    almoxarifado.Gestor = new GestorEntity();
        //    almoxarifado.Uge = new UGEEntity();

        //    long? codigo = 0;
        //    string descricao = string.Empty;

        //    if (opcao == "1")
        //    {
        //        codigo = Common.Util.TratamentoDados.TryParseLong(valor);
        //        descricao = string.Empty;
        //    }
        //    else if (opcao == "2")
        //    {
        //        codigo = 0;
        //        descricao = ((string.IsNullOrEmpty(valor)) ? string.Empty : valor);
        //    }

        //    //
        //    if (FiltrarAlmox)
        //    {
        //        //Se for perfil requisitante, irá pegar o almoxarifado da divisão pertencente
        //        if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE)
        //        {

        //            if (divisaoId != null && divisaoId != 0)
        //            {
        //                EstruturaOrganizacionalBusiness business = new EstruturaOrganizacionalBusiness();
        //                almoxarifado = business.GetAlmoxarifadoByDivisao((int)divisaoId);
        //            }
        //            else
        //            {
        //                //se a divisão estiver nula, não retornará registros
        //                return new List<SubItemMaterialEntity>();
        //            }
        //        }
        //        else
        //        {

        //            //Uge do Perfil                    
        //            if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
        //            {
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != 0)
        //                {
        //                    almoxarifado.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
        //                }
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id != 0)
        //                {
        //                    almoxarifado.Uge.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Não irá filtrar almoxarifado, trazendo todos os SubItens
        //    }

        //    //Filtrar Gestor Logado
        //    if (FiltraGestor)
        //    {
        //        almoxarifado.Gestor.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
        //    }
        //    else
        //    {
        //        almoxarifado.Gestor.Id = 0;
        //    }

        //    CatalogoBusiness estrutura = new CatalogoBusiness();
        //    estrutura.SkipRegistros = startRowIndexParameterName;
        //    IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemAlmoxPorPalavraChave(codigo, descricao, almoxarifado);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;

        //    return retorno;
        //}

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        //public IList<SubItemMaterialEntity> ListarSubItemAlmoxPorPalavraChave(int startRowIndexParameterName, string valorBusca, bool FiltrarAlmox, bool FiltraGestor)
        //{
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();
        //    almoxarifado.Gestor = new GestorEntity();
        //    almoxarifado.Uge = new UGEEntity();

        //    if (FiltrarAlmox)
        //    {
        //        //Se for perfil requisitante, irá pegar o almoxarifado da divisão pertencente
        //        if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE)
        //        {
        //            EstruturaOrganizacionalBusiness business = new EstruturaOrganizacionalBusiness();
        //            almoxarifado = business.GetAlmoxarifadoByDivisao((int)Acesso.Transacoes.Perfis[0].Divisao.Id);
        //        }
        //        else
        //        {

        //            //Uge do Perfil                    
        //            if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
        //            {
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != 0)
        //                {
        //                    almoxarifado.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
        //                }
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id != 0)
        //                {
        //                    almoxarifado.Uge.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Não irá filtrar almoxarifado, trazendo todos os SubItens
        //    }

        //    //Filtrar Gestor Logado
        //    if (FiltraGestor)
        //    {
        //        almoxarifado.Gestor.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
        //    }
        //    else
        //    {
        //        almoxarifado.Gestor.Id = 0;
        //    }

        //    CatalogoBusiness estrutura = new CatalogoBusiness();
        //    estrutura.SkipRegistros = startRowIndexParameterName;
        //    IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemAlmoxPorPalavraChave(valorBusca, almoxarifado);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;

        //    return retorno;
        //}

        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        //public IList<SubItemMaterialEntity> ListarSubItemAlmoxPorPalavraChave(int startRowIndexParameterName, string valorBusca, bool FiltrarAlmox, bool FiltraGestor)
        //{
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();
        //    almoxarifado.Gestor = new GestorEntity();
        //    almoxarifado.Uge = new UGEEntity();

        //    if (FiltrarAlmox)
        //    {
        //        //Se for perfil requisitante, irá pegar o almoxarifado da divisão pertencente
        //        if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE)
        //        {
        //            EstruturaOrganizacionalBusiness business = new EstruturaOrganizacionalBusiness();
        //            almoxarifado = business.GetAlmoxarifadoByDivisao((int)Acesso.Transacoes.Perfis[0].Divisao.Id);
        //        }
        //        else
        //        {

        //            //Uge do Perfil                    
        //            if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
        //            {
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != 0)
        //                {
        //                    almoxarifado.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
        //                }
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id != 0)
        //                {
        //                    almoxarifado.Uge.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Não irá filtrar almoxarifado, trazendo todos os SubItens
        //    }

        //    //Filtrar Gestor Logado
        //    if (FiltraGestor)
        //    {
        //        almoxarifado.Gestor.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
        //    }
        //    else
        //    {
        //        almoxarifado.Gestor.Id = 0;
        //    }

        //    CatalogoBusiness estrutura = new CatalogoBusiness();
        //    estrutura.SkipRegistros = startRowIndexParameterName;
        //    IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemAlmoxPorPalavraChave(valorBusca, almoxarifado);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;

        //    return retorno;
        //}


        /// <summary>
        /// Busca subitens utilizada na modal de pesquisa
        /// </summary>
        /// <param name="startRowIndexParameterName">index da paginação</param>
        /// <param name="valorBusca">palavra digitada pelo usuário</param>
        /// <param name="FiltrarAlmox">Filtrar por almoxarifado</param>
        /// <param name="comSaldo">Filtrar subitens com saldo</param>
        /// <param name="FiltraGestor">Filtrar subItens do gestor logado</param>
        /// <param name="almoxarifado">parametros do almoxarifado e gestor</param>
        /// <returns></returns>

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        //public IList<TB_SUBITEM_MATERIAL> BuscarSubItemMaterial(int startRowIndexParameterName, string valorBusca, bool FiltrarAlmox, bool comSaldo, bool FiltraGestor, int? divisaoId = null, bool pesquisaRequisicao = false, bool dispRequisicao = false)
        public IList<TB_SUBITEM_MATERIAL> BuscarSubItemMaterial(int startRowIndexParameterName, string valorBusca, bool FiltrarAlmox, bool comSaldo, bool FiltraGestor,
            int? divisaoId = null, bool pesquisaRequisicao = false, bool dispRequisicao = false, 
            bool filtraNaturezasDespesaConsumoImediato = false, int almox = 0, int gestor= 0)
        {
            TB_ALMOXARIFADO almoxarifado = new TB_ALMOXARIFADO();
            almoxarifado = GetAlmoxarifadoBusca(FiltrarAlmox, FiltraGestor, divisaoId);
            IList<TB_SUBITEM_MATERIAL> result = null;
            Sam.Business.SubItemMaterialBusiness business = new Business.SubItemMaterialBusiness();
          
            almox =  Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            gestor = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            //var result = business.BuscaSubItemMaterial(startRowIndexParameterName, valorBusca, FiltrarAlmox, FiltraGestor, comSaldo, almoxarifado);           
            if (pesquisaRequisicao)
                result = business.BuscaSubItemMaterialRequisicao(startRowIndexParameterName, valorBusca, FiltraGestor, almoxarifado, dispRequisicao);
            else
                //result = business.BuscaSubItemMaterial(startRowIndexParameterName, valorBusca, FiltrarAlmox, FiltraGestor, comSaldo, almoxarifado);
                result = business.BuscaSubItemMaterial(startRowIndexParameterName, valorBusca, FiltrarAlmox, FiltraGestor, comSaldo, almoxarifado, filtraNaturezasDespesaConsumoImediato,
                    almox, gestor);

            

            TotalRegistrosGrid = business.TotalRegistros;

            foreach (var item in result)
            {
                item.TB_ITEM_MATERIAL_ID = item.TB_ITEM_SUBITEM_MATERIAL.FirstOrDefault().TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_ID;
                item.TB_ITEM_MATERIAL_CODIGO = item.TB_ITEM_SUBITEM_MATERIAL.FirstOrDefault().TB_ITEM_MATERIAL.TB_ITEM_MATERIAL_CODIGO;
                item.TB_UNIDADE_FORNECIMENTO_CODIGO_DESCRICAO = String.Format("{0}-{1}", item.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_CODIGO, item.TB_UNIDADE_FORNECIMENTO.TB_UNIDADE_FORNECIMENTO_DESCRICAO);
            }

            return result;
        }

        private TB_ALMOXARIFADO GetAlmoxarifadoBusca(bool FiltrarAlmox, bool FiltraGestor, int? divisaoLogadaId = null)
        {

            TB_ALMOXARIFADO almoxarifado = new TB_ALMOXARIFADO();
            almoxarifado.TB_GESTOR = new TB_GESTOR();
            almoxarifado.TB_UGE = new TB_UGE();

            if (FiltrarAlmox)
            {
                //Se for perfil REQUISITANTEs, irá tentar pegar o almoxarifado da divisão LOGADA, caso contrário pegará o almoxarifado da divisão PADRÃO.
                if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE || Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE_GERAL || Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.ADMINISTRADOR_GERAL)
                {
                    if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != null && divisaoLogadaId == null)
                        almoxarifado.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                    else if (divisaoLogadaId != null && divisaoLogadaId != 0)
                    {
                        //int? divisaoId = divisaoLogadaId.IsNotNull() ? divisaoLogadaId : Acesso.Transacoes.Perfis[0].DivisaoPadrao.Id;
                        int? divisaoId = divisaoLogadaId;

                        EstruturaOrganizacionalBusiness business = new EstruturaOrganizacionalBusiness();
                        int? id = business.GetAlmoxarifadoByDivisao((int)divisaoId).Id;

                        if (id != null)
                           almoxarifado.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id; 
                        else
                            throw new Exception("Não é possivel disponibilizar a pesquina para o perfil de requisitante.");

                    }
                    else
                    {
                        //se a divisão estiver nula, não retornará registros
                        throw new Exception("Não é possivel disponibilizar a pesquina para o perfil de requisitante.");
                    }
                }
                else
                {
                    //Uge do Perfil                    
                    if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
                    {
                        if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != 0)
                        {
                            almoxarifado.TB_ALMOXARIFADO_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                        }
                        if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id != 0)
                        {
                            almoxarifado.TB_UGE_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;
                        }
                    }
                }
            }
            else
            {
                //throw new Exception("A busca não pode ser executada sem filtros");
            }

            //Filtrar Gestor Logado
            //if (FiltraGestor)
            //{
                almoxarifado.TB_GESTOR_ID = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
            //}
            //else
            //{
            //    almoxarifado.TB_GESTOR_ID = 0;
            //}

            return almoxarifado;
        }


        //public IList<SubItemMaterialEntity> ListarSubItemAlmoxPorPalavraChave(int startRowIndexParameterName, int maximumRowsParameterName, string opcao, string valor, bool FiltrarAlmox, int? divisaoId, bool FiltraGestor, bool subitemComSaldo)
        //{
        //    AlmoxarifadoEntity almoxarifado = new AlmoxarifadoEntity();
        //    almoxarifado.Gestor = new GestorEntity();
        //    almoxarifado.Uge = new UGEEntity();

        //    long? codigo = 0;
        //    string descricao = string.Empty;

        //    if (opcao == "1")
        //    {
        //        codigo = Common.Util.TratamentoDados.TryParseLong(valor);
        //        descricao = string.Empty;
        //    }
        //    else if (opcao == "2")
        //    {
        //        codigo = 0;
        //        descricao = ((string.IsNullOrEmpty(valor)) ? string.Empty : valor);
        //    }

        //    //
        //    if (FiltrarAlmox)
        //    {
        //        //Se for perfil requisitante, irá pegar o almoxarifado da divisão pertencente
        //        if (Acesso.Transacoes.Perfis[0].IdPerfil == (int)Sam.Common.Perfil.REQUISITANTE)
        //        {

        //            if (divisaoId != null && divisaoId != 0)
        //            {
        //                EstruturaOrganizacionalBusiness business = new EstruturaOrganizacionalBusiness();
        //                almoxarifado = business.GetAlmoxarifadoByDivisao((int)divisaoId);
        //            }
        //            else
        //            {
        //                //se a divisão estiver nula, não retornará registros
        //                return new List<SubItemMaterialEntity>();
        //            }
        //        }
        //        else
        //        {

        //            //Uge do Perfil                    
        //            if (Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado != null)
        //            {
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id != 0)
        //                {
        //                    almoxarifado.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
        //                }
        //                if ((int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id != 0)
        //                {
        //                    almoxarifado.Uge.Id = (int)Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Uge.Id;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Não irá filtrar almoxarifado, trazendo todos os SubItens
        //    }

        //    //Filtrar Gestor Logado
        //    if (FiltraGestor)
        //    {
        //        almoxarifado.Gestor.Id = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id;
        //    }
        //    else
        //    {
        //        almoxarifado.Gestor.Id = 0;
        //    }

        //    CatalogoBusiness estrutura = new CatalogoBusiness();
        //    estrutura.SkipRegistros = startRowIndexParameterName;
        //    IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemAlmoxPorPalavraChave(codigo, descricao, almoxarifado, subitemComSaldo);
        //    this.TotalRegistrosGrid = estrutura.TotalRegistros;

        //    return retorno;
        //}

        //        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<SubItemMaterialEntity> PopularDadosSubItemComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _itemId, int _gestorID)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterialTodosCod(_itemId, _gestorID);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }

        public ItemMaterialEntity GetItemMaterialBySubItem(int subItemId)
        {
            CatalogoBusiness presenter = new CatalogoBusiness();
            return presenter.GetItemMaterialBySubItem(subItemId);
        }

        public string NaturezaSubItem(int subItemId)
        {
            CatalogoBusiness presenter = new CatalogoBusiness();
            return presenter.NaturezaSubItem(subItemId);
        }

        public ItemMaterialEntity GetItemMaterialByItem(int itemId)
        {
            CatalogoBusiness presenter = new CatalogoBusiness();
            return presenter.GetItemMaterialByItem(itemId);
        }

        public IList<SubItemMaterialEntity> PopularSubItemMaterialTodosPorItemUgeAlmox(int _itemCodigo, int _ugeId, int _almoxId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = estrutura.ListarSubItemMaterialTodosCodPorItemUgeAlmox(_itemCodigo, _ugeId, _almoxId);
            return retorno;
        }

        public IList<SubItemMaterialEntity> PopularDadosRelatorio(int _gestorId)
        {
            // imprimir filtrando o gestor logado
            _gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            CatalogoBusiness catalogo = new CatalogoBusiness();
            return catalogo.ImprimirSubitensMaterial(_gestorId);
        }

        public IList<NaturezaDespesaEntity> PopularNaturezaDespesaComSubItem(int _gestorId)
        {


            return (from a in PopularDadosRelatorioConsulta(_gestorId, null)
                    group a by new
                    {
                        Id = a.NaturezaDespesa.Id,
                        Codigo = a.NaturezaDespesa.Codigo,
                        Descricao = a.NaturezaDespesa.Descricao
                    } into g
                    select new NaturezaDespesaEntity
                    {
                        Id = g.Key.Id,
                        Codigo = g.Key.Codigo,
                        Descricao = g.Key.Descricao,
                        //CodigoDescricao = string.Format("{0} - {1}", g.Key.Descricao, g.Key.Codigo)
                        CodigoDescricao = string.Format("{0} - {1}", g.Key.Codigo, g.Key.Descricao)
                        //}).OrderBy(a=>a.Descricao).ToList();
                    }).OrderBy(a => a.Codigo).ToList();
        }

        public IList<SubItemMaterialEntity> PopularDadosRelatorioConsulta(int _gestorId, int? _naturezaId, int? _itemCodigo = 0, long? _subitemCodigo = 0)
        {
            // imprimir filtrando o gestor logado
            CatalogoBusiness catalogo = new CatalogoBusiness();

            if (_naturezaId.HasValue && _subitemCodigo != 0 && _itemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.NaturezaDespesa.Codigo == _naturezaId && a.Codigo == _subitemCodigo && a.ItemMaterial.Codigo == _itemCodigo).ToList();

            else if (_naturezaId.HasValue && _subitemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.NaturezaDespesa.Codigo == _naturezaId && a.Codigo == _subitemCodigo).ToList();

            else if (_naturezaId.HasValue && _itemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.NaturezaDespesa.Codigo == _naturezaId && a.ItemMaterial.Codigo == _itemCodigo).ToList();

            else if (_subitemCodigo != 0 && _itemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.Codigo == _subitemCodigo && a.ItemMaterial.Codigo == _itemCodigo).ToList();

            else if (_naturezaId.HasValue)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.NaturezaDespesa.Codigo == _naturezaId).ToList();

            else if (_subitemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.Codigo == _subitemCodigo).ToList();

            else if (_itemCodigo != 0)
                return catalogo.ImprimirSubitensMaterial(_gestorId).Where(a => a.ItemMaterial.Codigo == _itemCodigo).ToList();
            else
                return catalogo.ImprimirSubitensMaterial(_gestorId);
        }

        public IList<SubItemMaterialEntity> PopularDadosRelatorio(int _itemId, int _gestorId)
        {
            //_gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = catalogo.ImprimirSubitensMaterial(_itemId, _gestorId);
            return retorno;
        }

        public IList<SubItemMaterialEntity> PopularDadosRelatorioGerenciaCatalogo(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId)
        {
            //_gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            CatalogoBusiness catalogo = new CatalogoBusiness();
            IList<SubItemMaterialEntity> retorno = catalogo.ImprimirGerenciaCatalogo(_materialId, _itemId, _gestorId, _almoxarifadoId);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ImprimirConsumoAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            Sam.Domain.Business.SaldoSubItemBusiness catalogo = new Sam.Domain.Business.SaldoSubItemBusiness();
            return catalogo.ImprimirConsumoSubitemMaterialAlmox(_almoxId, dataInicial, dataFinal);
        }

        public IList<SubItemMaterialEntity> ImprimirConsumoDivisao(int? _divisaoId, int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        {
            Sam.Domain.Business.SaldoSubItemBusiness catalogo = new Sam.Domain.Business.SaldoSubItemBusiness();
            return catalogo.ImprimirConsumoSubitemMaterialDivisao(_divisaoId, _almoxId, dataInicial, dataFinal);
        }

        //public IList<SubItemMaterialEntity> ImprimirPrevisaoConsumoAlmox(int? _almoxId, DateTime? dataInicial, DateTime? dataFinal)
        //{
        //    Sam.Domain.Business.SaldoSubItemBusiness catalogo = new Sam.Domain.Business.SaldoSubItemBusiness();
        //    return catalogo.ImprimirPrevisaoConsumoSubitemMaterialAlmox(_almoxId, dataInicial, dataFinal);
        //}

        public int TotalRegistros(int startRowIndexParameterName, string palavra)
        {
            return this.TotalRegistrosGrid;
        }


        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _itemId, int _gestorID)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _itemId, int _gestorID, int _almoxarifadoId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _MaterialId, int _itemId, int _gestorID, int _almoxarifadoId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, Int64 _SubItemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, Int64 _SubItemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, Int64 _SubItemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, int iNaturezaDespesa_ID, Int64 _SubItemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, int iNaturezaDespesa_ID, Int64 _SubItemcodigo = 0, Int64 _Itemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, int iNaturezaDespesa_ID, int _indicadorId, Int64 _SubItemcodigo = 0, Int64 _Itemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, string opcao, string valor, bool comSaldo)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int divisaoId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int iNaturezaDespesa_ID, int _indicadorId, int _saldoId, Int64? _SubItemcodigo = 0, Int64? _Itemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }


        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorID, int _almoxarifadoId, int iNaturezaDespesa_ID, int _indicadorId, int _saldoId, Int64? _SubItemcodigo = 0, Int64? _Itemcodigo = 0)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistrosConsultaSubitemPorND(int iNaturezaDespesa_ID, int startRowIndex, int maximumRows)
        {
            return this.TotalRegistrosGrid;
        }

        public void Load(bool blnExecuteBaseMethodOnly)
        {
            base.Load();

            if (!blnExecuteBaseMethodOnly)
            {
                this.View.PopularListaOrgao();
                this.View.PopularListaGrupo();
            }
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaOrgao();
            this.View.PopularListaGrupo();
        }

        public void LoadConsulta()
        {
            base.Load();
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SelectSubItemMaterial(this.SubItemId);

            this.View.Id = estrutura.SubItemMaterial.Id.Value.ToString();
            this.View.Codigo = estrutura.SubItemMaterial.Codigo.ToString();
            this.View.Descricao = estrutura.SubItemMaterial.Descricao;
            this.View.CodigoBarras = estrutura.SubItemMaterial.CodigoBarras;

            this.View.PopularListaNaturezaDespeza();
            //this.View.PopularListaContaAuxiliar();
            this.View.PopularListaIndicadorAtividade();
            this.View.PopularListaUnidadeFornecimento();

            if (estrutura.SubItemMaterial.NaturezaDespesa != null)
                this.View.NaturezaDespesaId = estrutura.SubItemMaterial.NaturezaDespesa.Id.Value.ToString();

            //if (estrutura.SubItemMaterial.ContaAuxiliar != null)
            //    this.View.ContaAuxiliarId = estrutura.SubItemMaterial.ContaAuxiliar.Id.Value.ToString();
            if (estrutura.SubItemMaterial.IsDecimal != null)

                this.View.ExpandeDecimos = (bool)estrutura.SubItemMaterial.IsDecimal;

            if (estrutura.SubItemMaterial.IsFracionado != null)
                this.View.PermiteFracionamento = (bool)(estrutura.SubItemMaterial.IsFracionado);

            if (estrutura.SubItemMaterial.IndicadorAtividade != null)
                this.View.IndicadorAtividadeId = (bool)estrutura.SubItemMaterial.IndicadorAtividade;

            if (estrutura.SubItemMaterial.UnidadeFornecimento != null)
                this.View.UnidadeFornecimentoId = (int)estrutura.SubItemMaterial.UnidadeFornecimento.Id;

            this.View.BloqueiaCodBarras = true;
            this.View.BloqueiaNaturezaDespesa = true;
            //this.View.BloqueiaContaAuxiliar = true;            
            this.View.BloqueiaExpandeDecimais = true;
            this.View.BloqueiaPermiteFacionamento = true;
            this.View.BloqueiaAtividade = true;
            this.View.BloqueiaUnidadeFornecimento = true;


            //this.View.PopularListaIndicardorDisponivel();
        }

        public void RegistroSelecionadoAlmox()
        {
            var almoxLogado = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado;

            base.RegistroSelecionado();
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SelectSubItemMaterialAlmox(this.SubItemId, almoxLogado.Id.Value);

            if (estrutura.SubItemMaterial != null)
            {
                this.View.Id = estrutura.SubItemMaterial.Id.Value.ToString();
                this.View.Codigo = estrutura.SubItemMaterial.Codigo.ToString();
                this.View.Descricao = estrutura.SubItemMaterial.Descricao;
                this.View.EstoqueMaximo = estrutura.SubItemMaterial.EstoqueMaximo.Value.Truncar(almoxLogado.MesRef, true);
                this.View.EstoqueMinimo = estrutura.SubItemMaterial.EstoqueMinimo.Value.Truncar(almoxLogado.MesRef, true);

                this.View.IndicadorAtividadeAlmox = (bool)estrutura.SubItemMaterial.IndicadorAtividadeAlmox;

                this.View.IndicadorDisponivelId = estrutura.SubItemMaterial.IndicadorDisponivelId;
            }



            this.View.BloqueiaCodBarras = true;
            this.View.BloqueiaNaturezaDespesa = true;
            //this.View.BloqueiaContaAuxiliar = true;           
            this.View.BloqueiaExpandeDecimais = true;
            this.View.BloqueiaPermiteFacionamento = true;
            this.View.BloqueiaAtividade = true;
            this.View.BloqueiaUnidadeFornecimento = true;
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();
            CatalogoBusiness estrutura = new CatalogoBusiness();

            this.View.CodigoBarras = string.Empty;
            this.View.BloqueiaCodBarras = false;
            this.View.BloqueiaNaturezaDespesa = false;
            //this.View.BloqueiaContaAuxiliar = false;           
            this.View.BloqueiaExpandeDecimais = false;
            this.View.BloqueiaPermiteFacionamento = false;
            this.View.BloqueiaAtividade = false;
            this.View.BloqueiaUnidadeFornecimento = false;
        }

        public override void Novo()
        {
            base.Novo();
            this.View.CodigoBarras = string.Empty;
            this.View.BloqueiaCodBarras = true;
            this.View.BloqueiaNaturezaDespesa = true;
            //this.View.BloqueiaContaAuxiliar = true;           
            this.View.BloqueiaExpandeDecimais = true;
            this.View.BloqueiaPermiteFacionamento = true;
            this.View.BloqueiaAtividade = true;
            this.View.BloqueiaUnidadeFornecimento = true;
            this.View.PermiteFracionamento = false;
            this.View.ExpandeDecimos = false;

            this.View.PopularListaNaturezaDespeza();
            //this.View.PopularListaContaAuxiliar();
            this.View.PopularListaIndicadorAtividade();
            this.View.PopularListaUnidadeFornecimento();
        }

        public void DesativaEdicao(bool enable)
        {
            this.View.BloqueiaCodBarras = enable;
            this.View.BloqueiaNaturezaDespesa = enable;
            //this.View.BloqueiaContaAuxiliar = enable;            
            this.View.BloqueiaExpandeDecimais = enable;
            this.View.BloqueiaPermiteFacionamento = enable;
            this.View.BloqueiaAtividade = true;
            this.View.BloqueiaUnidadeFornecimento = enable;
            this.View.BloqueiaDescricao = enable;
            this.View.BloqueiaCodigo = enable;

            this.View.BloqueiaExcluir = enable;
        }

        public override void Cancelar()
        {

            base.Cancelar();
            this.View.BloqueiaCodBarras = false;
            this.View.BloqueiaNaturezaDespesa = false;
            //this.View.BloqueiaContaAuxiliar = false;
            this.View.BloqueiaExpandeDecimais = false;
            this.View.BloqueiaPermiteFacionamento = false;
            this.View.BloqueiaAtividade = false;
            this.View.BloqueiaUnidadeFornecimento = false;
            this.View.LimparPesquisaItem();
        }

        public override void ExcluidoSucesso()
        {
            base.ExcluidoSucesso();
            this.View.CodigoBarras = string.Empty;
            this.View.BloqueiaCodBarras = false;
            this.View.BloqueiaNaturezaDespesa = false;
            //this.View.BloqueiaContaAuxiliar = false;            
            this.View.BloqueiaExpandeDecimais = false;
            this.View.BloqueiaPermiteFacionamento = false;
            this.View.BloqueiaAtividade = false;
            this.View.BloqueiaUnidadeFornecimento = false;
        }

        public void Gravar(string codigoSubItem, string acao)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);
            int item, natureza, conta, gestor;
            long codigo;
            long.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.ItemId, out item);
            int.TryParse(this.View.NaturezaDespesaId, out natureza);
            //int.TryParse(this.View.ContaAuxiliarId, out conta);
            int.TryParse(this.View.GestorId, out gestor);

            estrutura.SubItemMaterial.Codigo = codigo;
            estrutura.SubItemMaterial.Descricao = this.View.Descricao;
            estrutura.SubItemMaterial.ItemMaterial = (new ItemMaterialEntity(item));
            estrutura.SubItemMaterial.IndicadorAtividade = this.View.IndicadorAtividadeId;
            estrutura.SubItemMaterial.NaturezaDespesa = (new NaturezaDespesaEntity(natureza));
            //estrutura.SubItemMaterial.ContaAuxiliar = (new ContaAuxiliarEntity(conta));
            estrutura.SubItemMaterial.Gestor = (new GestorEntity(gestor));
            estrutura.SubItemMaterial.CodigoBarras = View.CodigoBarras;
            estrutura.SubItemMaterial.IsDecimal = View.ExpandeDecimos;
            estrutura.SubItemMaterial.IsFracionado = View.PermiteFracionamento;
            estrutura.UnidadeFornecimento = (new UnidadeFornecimentoEntity(this.View.UnidadeFornecimentoId));

            if (estrutura.SalvarSubItemMaterial(codigoSubItem, acao))
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void GravarAlmox()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);
            int codigo, item, natureza, conta, gestor;
            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.ItemId, out item);
            int.TryParse(this.View.NaturezaDespesaId, out natureza);
            //int.TryParse(this.View.ContaAuxiliarId, out conta);
            int.TryParse(this.View.GestorId, out gestor);

            estrutura.SubItemMaterial.AlmoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            estrutura.SubItemMaterial.IndicadorAtividadeAlmox = this.View.IndicadorAtividadeAlmox;
            estrutura.SubItemMaterial.IndicadorDisponivelId = this.View.IndicadorDisponivelId;
            estrutura.SubItemMaterial.EstoqueMaximo = this.View.EstoqueMaximo;
            estrutura.SubItemMaterial.EstoqueMinimo = this.View.EstoqueMinimo;
            estrutura.UnidadeFornecimento = (new UnidadeFornecimentoEntity(this.View.UnidadeFornecimentoId));

            if (estrutura.SalvarSubItemAlmox())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void GravarAlmox(bool IndicadorAtividadeAlmox, int IndicadorDisponivelId, int IndicadorDisponivalZaeradoId, decimal EstoqueMinimo, decimal EstoqueMaximo)
        {
            base.RegistroSelecionado();

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);
            int codigo, item, natureza, conta, gestor;
            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.ItemId, out item);
            int.TryParse(this.View.NaturezaDespesaId, out natureza);
            //int.TryParse(this.View.ContaAuxiliarId, out conta);
            int.TryParse(this.View.GestorId, out gestor);

            estrutura.SubItemMaterial.AlmoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value;
            estrutura.SubItemMaterial.IndicadorAtividadeAlmox = IndicadorAtividadeAlmox;
            estrutura.SubItemMaterial.IndicadorDisponivelId = IndicadorDisponivelId;
            estrutura.SubItemMaterial.IndicadorDisponivelIdZerado = IndicadorDisponivalZaeradoId;
            estrutura.SubItemMaterial.EstoqueMaximo = EstoqueMaximo;
            estrutura.SubItemMaterial.EstoqueMinimo = EstoqueMinimo;
            estrutura.UnidadeFornecimento = (new UnidadeFornecimentoEntity(this.View.UnidadeFornecimentoId));

            if (estrutura.SalvarSubItemAlmox())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.SubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirSubItemMaterial())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Excluir(int Id)
        {
            //CatalogoBusiness estrutura = new CatalogoBusiness();
            //estrutura.SubItemMaterial.Id = Convert.ToInt32(Id);

            //if (estrutura.ExcluirSubItemMaterial())
            //{
            //    this.View.PopularGrid();
            //    this.ExcluidoSucesso();
            //    this.View.ExibirMensagem("Registro excluído com sucesso!");
            //}
            //else
            //    this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            //this.View.ListaErros = estrutura.ListaErro;
            try
            {
                var almoxarifadoId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                CatalogoBusiness estrutura = new CatalogoBusiness();
                estrutura.SubItemMaterial.Id = Convert.ToInt32(Id);
                estrutura.SubItemMaterial.AlmoxarifadoId = almoxarifadoId.Value;
                if (estrutura.ExcluirSubItemMaterial())
                {
                    this.View.PopularGrid();
                    this.ExcluidoSucesso();
                    this.View.ExibirMensagem("Subitem retirado do catalogo do almoxarifado com sucesso.");
                }
                else
                    this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens abaixo em vermelho!");
                this.View.ListaErros = estrutura.ListaErro;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.SubitensMaterial;
            ///            _gestorID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            //RelatorioEntity.Nome = "rptSubitensMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsSubitensMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.SubitensMaterial;
            relatorioImpressao.Nome = "rptSubitensMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSubitensMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void ImprimirConsulta()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.ConsultaSubitemMaterial;
            ////_gestorID = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;
            //RelatorioEntity.Nome = "rptConsultaSubitemMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsSubitensMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaSubitemMaterial;
            relatorioImpressao.Nome = "rptConsultaSubitemMaterial.rdlc";
            relatorioImpressao.DataSet = "dsSubitensMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public void ImprimirConsultaSubitensPorND()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.ConsultaSubitemMaterialAlmoxPorND;
            //RelatorioEntity.Nome = "rptConsultaSubitemMaterialPorND.rdlc";
            //RelatorioEntity.DataSet = "dsSubitensMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ConsultaSubitemMaterialAlmoxPorND;
            relatorioImpressao.Nome = "rptConsultaSubitemMaterialPorND.rdlc";
            relatorioImpressao.DataSet = "dsSubitensMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
        public void ImprimirGerenciaCatalogo()
        {
            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.GerenciaCatalogo;
            relatorioImpressao.Nome = "AlmoxGerenciaCatalogo.rdlc";
            relatorioImpressao.DataSet = "dsGerenciaCatalogo";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            //this.DadosRelatorio = relatorioImpressao;
            //this.View.DadosRelatorio = this.DadosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }

        public bool IsRequisitante()
        {
            var PerfilLogado = this.Acesso.Transacoes.Perfis[0];
            int iTipoPerfilLogado = PerfilLogado.IdPerfil;

            return (iTipoPerfilLogado == (int)GeralEnum.TipoPerfil.Requisitante);
        }
    }
}
