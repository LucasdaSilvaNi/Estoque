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


namespace Sam.Presenter
{
   // [DataObject(true)]
    public class ItemMaterialPresenter : CrudPresenter<IItemMaterialView>
    {
        IItemMaterialView view;

        public IItemMaterialView View
        {
            get { return view; }
            set { view = value; }
        }

        public ItemMaterialPresenter()
        {
        }

        public ItemMaterialPresenter(IItemMaterialView _view)
            : base(_view)
        {
            this.View = _view;
        }
        

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> PopularDadosClasse(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterial(_materialId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> ListarSubItemCod(int _materialId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<ItemMaterialEntity> retorno = estrutura.ListarSubItemCod(_materialId);
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> ListarItemAlmox(int _materialId, int _gestorId, int _almoxarifadoId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            //IList<ItemMaterialEntity> retorno = estrutura.ListarAlmox(_materialId, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value, Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id.Value);
            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterialTodosCod(_materialId);
            return retorno;
        }

        public bool ConsultarSiafisico(string itemMaterial) 
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            ItemMaterialEntity item = new ItemMaterialEntity();

            // procurar item de material do siafisico
            item = estrutura.RecuperarCadastroItemMaterialDoSiafisico(itemMaterial, false);
            if (estrutura.ListaErro.Count > 0)
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return false;
            }

            // informar os dados do item de material
            if (item != null || item.Codigo != 0)
            {
                this.View.SiafCodGrupo = item.Material.Classe.Grupo.Codigo.ToString().PadLeft(2, '0');
                this.View.SiafDescGrupo = item.Material.Classe.Grupo.Descricao;
                this.View.SiafCodClasse = item.Material.Classe.Codigo.ToString().PadLeft(2, '0');
                this.View.SiafDescClasse = item.Material.Classe.Descricao;
                this.View.SiafCodMaterial = item.Material.Codigo.ToString().PadLeft(8, '0');
                this.View.SiafDescMaterial = item.Material.Descricao;
                this.View.SiafCodItem = item.Codigo.ToString().PadLeft(9, '0');
                this.View.SiafDescItem = item.Descricao;
                this.View.SiafNatDesp1 = item.NatDespSiafisicoCodigo1.ToString().PadLeft(8, '0');
                this.View.SiafNatDesp2 = item.NatDespSiafisicoCodigo2.ToString().PadLeft(8, '0');
                this.View.SiafNatDesp3 = item.NatDespSiafisicoCodigo3.ToString().PadLeft(8, '0');
                this.View.SiafNatDesp4 = item.NatDespSiafisicoCodigo4.ToString().PadLeft(8, '0');
                this.View.SiafNatDesp5 = item.NatDespSiafisicoCodigo5.ToString().PadLeft(8, '0');
            }
            else
            {
                estrutura.ListaErro.Add("Item de Material não encontrado no Siafisico!");
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return false;
            }

            return true;
        }

        public IList<NaturezaDespesaEntity> PopularNaturezaDespesaTodosCodPorItemMaterial(int _itemMaterialId)
        { 
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.ItemMaterial = new ItemMaterialEntity();
            estrutura.ItemMaterial.Id = Convert.ToInt32(_itemMaterialId);
            ItemMaterialEntity item = estrutura.GetItemMaterialNaturezaDespesa();
            IList<NaturezaDespesaEntity> resultado = new List<NaturezaDespesaEntity>();
            if (item != null)
            {
                resultado = item.NaturezaDespesa;
            }

            if ( (resultado.Count == 0) || !item.IsNull() && item.Descricao.ToUpperInvariant().Contains("ITEM INATIVO"))
            {
                var iGestor_ID = this.Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;

                SubItemMaterialPresenter lObjPresenter = new SubItemMaterialPresenter();
                resultado = lObjPresenter.PopularNaturezaDespesaComSubItem(iGestor_ID);
            }

            resultado.Add(new NaturezaDespesaEntity { Id = 0, CodigoDescricao = "- Selecione -" });
            return resultado.OrderBy(a => a.Descricao).ToList();
        }


        public IList<TB_ITEM_MATERIAL> BuscarItemMaterial(int startRowIndexParameterName, string palavraChave)
        {
            Sam.Business.ItemMaterialBusiness business = new Business.ItemMaterialBusiness();
            var result = business.BuscarItemMaterial(startRowIndexParameterName, palavraChave);
            this.TotalRegistrosGrid = business.TotalRegistros;

            return result;
        }




        public IList<ItemMaterialEntity> ListarItemSaldoByAlmox(int almoxarifado)
        {
            return new CatalogoBusiness().ListarItemSaldoByAlmox(almoxarifado);
        }

        public IList<ItemMaterialEntity> ListarItemMaterialPorPalavraChave(int _tipoFiltro, string _chave, bool filtrarAlmox, bool FiltrarGestorLogado)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            int? _id = null;
            int? _codigo = null;
            string _descricao = null;

            //Filtrar o  almoxarifado Logado
            int? _almoxId = 0;
            int? _gestorId = 0;

            int perfilId = Acesso.Transacoes.Perfis[0].IdPerfil;
            int loginId = Acesso.Transacoes.Perfis[0].IdLogin;

            //Filtra os subitens do almoxarigado logado
            if (filtrarAlmox)
            {
                //O perfil de requisitante não precisa filtrar por Almoxarigado Logado
                if (!(perfilId == (int)Sam.Common.Perfil.REQUISITANTE))
                    _almoxId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Id;
                else
                    _almoxId = 0;
            }
            else
                _almoxId = 0;

            //Filtra os subitens do gestor logado
            if (FiltrarGestorLogado)
            {
                //O perfil de requisitante não precisa filtrar por Almoxarigado Logado
                if (!(perfilId == (int)Sam.Common.Perfil.REQUISITANTE))
                    _gestorId = Acesso.Transacoes.Perfis[0].AlmoxarifadoLogado.Gestor.Id.Value;                    
                else
                    _gestorId = 0;
            }
            else
                _gestorId = 0;

            switch (_tipoFiltro)
            {
                case 0:
                    _id = Common.Util.TratamentoDados.TryParseInt32(_chave);                    
                    break;
                case 1:
                    _codigo = Common.Util.TratamentoDados.TryParseInt32(_chave);
                    break;
                case 2:
                    _descricao = _chave;
                    break;
                default:
                    break;
            }

            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterialPorPalavraChaveTodosCod(_id, _codigo, _descricao, _almoxId, _gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;

            return retorno;
        }


        //[DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> PopularDadosItemMaterialBySubItem(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId, int _gestorId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterialBySubItem(_materialId,_gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }
        

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> PopularDadosItemMaterialTodosComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterialTodosCod(_materialId);
            return retorno;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<ItemMaterialEntity> PopularDadosItemMaterialComCod(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            IList<ItemMaterialEntity> retorno = estrutura.ListarItemMaterialCod(_materialId);
            return retorno;
        }
        
        public IList<ItemMaterialEntity> PopularDadosRelatorio(int _materialId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            IList<ItemMaterialEntity> retorno = estrutura.ImprimirItemMaterial(_materialId);
            return retorno;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _tipoFiltro, string _chave)
        {
            return this.TotalRegistrosGrid;
        }

        public int TotalRegistros(int startRowIndexParameterName, int maximumRowsParameterName, int _materialId, int _gestorId, int _almoxarifadoId)
        {
            return this.TotalRegistrosGrid;
        }

        public override void Load()
        {
            base.Load();
            this.View.PopularListaGrupo();
            this.View.PopularListaClasse();
            this.View.PopularListaMaterial();
            this.View.PopularGrid();
        }

        public void Gravar()
        {

            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.ItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int codigo,  material;

            int.TryParse(this.View.Codigo, out codigo);
            int.TryParse(this.View.MaterialId, out material);

            bool bResult;

            estrutura.ItemMaterial.Codigo = codigo;
            estrutura.ItemMaterial.Descricao = this.View.Descricao;
            estrutura.ItemMaterial.Material = (new MaterialEntity(material));
            estrutura.ItemMaterial.Atividade = bool.TryParse(this.View.AtividadeItemMaterialId,out bResult);
            estrutura.ItemMaterial.Atividade = bResult;

            if (estrutura.SalvarItemMaterial())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.BloqueiaSiafisico = false;
                this.View.ExibirMensagem("Registro salvo com sucesso!");
            }
            else
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }
        /// <summary>
        /// Atualizar item de material do SIAFISICO para o banco de dados
        /// Inclui-se a gravação de: grupo, classe e material, além das naturezas de despesa associadas ao item
        /// </summary>
        public void GravarSiafisico()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.ItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            //if (this.View.SiafCodItem == null || this.View.SiafCodItem == "")
            if (String.IsNullOrWhiteSpace(this.View.SiafCodItem))
            {
                estrutura.ListaErro.Add("Favor informar o Código do item Siafisico!");
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
                return;
            }

            estrutura.ItemMaterial = estrutura.RecuperarCadastroItemMaterialDoSiafisico(this.View.SiafCodItem, false);

            // default para ATIVO sobre o item de material
            if (estrutura.ItemMaterial.IsNotNull())
                estrutura.ItemMaterial.Atividade = true;

            if (estrutura.SalvarItemMaterialSiafisico())
            {
                this.View.PopularGrid();
                this.GravadoSucesso();
                this.View.BloqueiaSiafisico = false;
                this.View.ExibirMensagem("Registro salvo com sucesso!");
                this.View.LimparDadosSiafisico();
                this.View.ExibeSiafisico = false;
            }
            else
            {
                this.View.ExibirMensagem("Inconsistências encontradas, verificar mensagens!");
                this.View.ListaErros = estrutura.ListaErro;
            }
        }


        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.ItemMaterial;
            //RelatorioEntity.Nome = "rptItemMaterial.rdlc";
            //RelatorioEntity.DataSet = "dsItemMaterial";
            //RelatorioEntity.Parametros = this.View.ParametrosRelatorio;

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.ItemMaterial;
            relatorioImpressao.Nome = "rptItemMaterial.rdlc";
            relatorioImpressao.DataSet = "dsItemMaterial";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
        
        public void Excluir()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.ItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            if (estrutura.ExcluirItemMaterial())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.BloqueiaSiafisico = false;
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public bool IsAdministradorOrgaoOuSuperior()
        {
            var PerfilLogado      = this.Acesso.Transacoes.Perfis[0];
            int iTipoPerfilLogado = PerfilLogado.IdPerfil;

            return ((iTipoPerfilLogado == (int)GeralEnum.TipoPerfil.AdministradorOrgao) || (iTipoPerfilLogado == (int)GeralEnum.TipoPerfil.AdministradorGeral));
        }

        public ItemMaterialEntity ObterItemMaterial(int codigoItemMaterial)
        {
            ItemMaterialEntity objRetorno = null;

            CatalogoBusiness lObjBusiness = new CatalogoBusiness();
            objRetorno = lObjBusiness.ObterItemMaterial(codigoItemMaterial);

            return objRetorno;
        }
    }
}
