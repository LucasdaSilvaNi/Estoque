using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Sam.View;
using Sam.Domain.Entity;
using Sam.Domain.Business;
using Sam.Common.Util;

namespace Sam.Presenter
{

    public class RelacaoMaterialItemSubItemPresenter : CrudPresenter<IRelacaoMaterialItemSubItemView>
    {


        IRelacaoMaterialItemSubItemView view;

        public IRelacaoMaterialItemSubItemView View
        {
            get { return view; }
            set { view = value; }
        }


        public RelacaoMaterialItemSubItemPresenter()
        {
        }

        public RelacaoMaterialItemSubItemPresenter(IRelacaoMaterialItemSubItemView _view)
            : base(_view)
        {
            this.View = _view;
        }

        public Dictionary<string, string> SelectItem(object obj)
        {
            RelacaoMaterialItemSubItemEntity objRel = (RelacaoMaterialItemSubItemEntity)obj;


            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SelectItemMaterial(objRel.Item.Id.Value);
            ItemMaterialEntity info = estrutura.ItemMaterial;

            Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            dic.Add("codigo", info.Codigo.ToString());
            dic.Add("descricao", info.Descricao);

            return dic;
        }

        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public IList<RelacaoMaterialItemSubItemEntity> PopularDados(int startRowIndexParameterName,
int maximumRowsParameterName, int _itemId, int _subItemId, int _gestorId)
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();

            estrutura.SkipRegistros = startRowIndexParameterName;
            IList<RelacaoMaterialItemSubItemEntity> retorno = estrutura.ListarRelacaoSubItemMaterial(_itemId, _subItemId, _gestorId);
            this.TotalRegistrosGrid = estrutura.TotalRegistros;
            return retorno;
        }

        public IList<RelacaoMaterialItemSubItemEntity> PopularDadosRelatorio(int _itemId, int _subItemId, int _gestorId)
        {
            CatalogoBusiness catalogo = new CatalogoBusiness();
            //IList<ItemMaterialEntity> retorno = catalogo.ImprimirRelacaoItemSubitem(_itemId, _subItemId, _gestorId);
            IList<RelacaoMaterialItemSubItemEntity> retorno = catalogo.ImprimirRelacaoItemSubitem(_itemId, _subItemId, _gestorId);
            return retorno;
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

        public int TotalRegistros(int startRowIndexParameterName,
int maximumRowsParameterName, int _itemId, int _subItemId, int _gestorId)
        {
            return this.TotalRegistrosGrid;
        }
        public override void Load()
        {
            base.Load();
            this.View.BloqueiaGrupo = false;
            this.View.BloqueiaClasse = false;
            this.View.BloqueiaMaterial = false;
            this.View.BloqueiaItem = false;

            this.View.PopularListaOrgao();
            this.View.PopularGrid();
        }

        public override void RegistroSelecionado()
        {
            base.RegistroSelecionado();

            this.View.BloqueiaGrupo = true;
            this.View.BloqueiaMaterial = true;
            this.View.BloqueiaClasse = true;
            this.View.BloqueiaItem = true;

            CatalogoBusiness estrutura = new CatalogoBusiness();

            int id = int.MinValue;
            int.TryParse(this.View.Id, out id);

            estrutura.SelectItemMaterial(id);
            estrutura.SelectMaterial(estrutura.ItemMaterial.Material.Id.Value);
            estrutura.SelectClasse(estrutura.Material.Classe.Id.Value);

            // carregar combos...
            this.view.PopularListaGrupoEdit();
            this.view.Grupo = estrutura.Classe.Grupo.Id.Value.ToString();

            this.view.PopularListaClasseEdit();
            this.view.Classe = estrutura.Classe.Id.Value.ToString();

            this.view.PopularListaMaterialEdit();
            this.View.Material = estrutura.Material.Id.Value.ToString();

            this.view.PopularListaItemEdit();
            this.View.Item = estrutura.ItemMaterial.Id.Value.ToString();
        }

        public override void GravadoSucesso()
        {
            base.GravadoSucesso();

            CatalogoBusiness estrutura = new CatalogoBusiness();


            this.View.BloqueiaGrupo = false;
            this.View.BloqueiaClasse = false;
            this.View.BloqueiaMaterial = false;
            this.View.BloqueiaItem = false;
        }

        public override void Novo()
        {
            base.Novo();
            this.View.BloqueiaGrupo = true;
            this.View.BloqueiaClasse = true;
            this.View.BloqueiaMaterial = true;
            this.View.BloqueiaItem = true;

            this.View.PopularListaGrupoEdit();
            this.View.PopularListaClasseEdit();
            this.View.PopularListaMaterialEdit();
            this.View.PopularListaItemEdit();
        }

        public override void Cancelar()
        {
            base.Cancelar();

            this.View.BloqueiaGrupo = false;
            this.View.BloqueiaClasse = false;
            this.View.BloqueiaMaterial = false;
            this.View.BloqueiaItem = false;
            this.View.LimparPesquisaItem();
        }

        public override void ExcluidoSucesso()
        {
            base.ExcluidoSucesso();

            this.View.BloqueiaGrupo = false;
            this.View.BloqueiaClasse = false;
            this.View.BloqueiaMaterial = false;
            this.View.BloqueiaItem = false;

        }
        public void Gravar()
        {
            CatalogoBusiness estrutura = new CatalogoBusiness();
            estrutura.RelacaoSubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            int idItemSubItem, item, itemEdit, subitem, gestor;

            int.TryParse(this.View.Id, out idItemSubItem);
            int.TryParse(this.View.ItemId, out item);
            int.TryParse(this.View.ItemEditId, out itemEdit);
            int.TryParse(this.View.SubItemId, out subitem);
            int.TryParse(this.View.GestorId, out gestor);

            estrutura.RelacaoSubItemMaterial.Item = (new ItemMaterialEntity(itemEdit));
            estrutura.RelacaoSubItemMaterial.ItemEdit = (new ItemMaterialEntity(itemEdit));
            estrutura.RelacaoSubItemMaterial.SubItem = (new SubItemMaterialEntity(subitem));
            estrutura.RelacaoSubItemMaterial.Gestor = (new GestorEntity(gestor));

            if (estrutura.SalvarRelacaoSubItemMaterial())
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
            estrutura.RelacaoSubItemMaterial.Id = TratamentoDados.TryParseInt32(this.View.Id);

            SubItemMaterialPresenter classe = new SubItemMaterialPresenter();

            int idItemSubItem, item, itemEdit, subitem, gestor;

            int.TryParse(this.View.Id, out idItemSubItem);
            int.TryParse(this.View.ItemId, out item);
            int.TryParse(this.View.ItemEditId, out itemEdit);
            int.TryParse(this.View.SubItemId, out subitem);
            int.TryParse(this.View.GestorId, out gestor);

            estrutura.RelacaoSubItemMaterial.Item = (new ItemMaterialEntity(itemEdit)); //
            estrutura.RelacaoSubItemMaterial.ItemEdit = (new ItemMaterialEntity(itemEdit)); //
            estrutura.RelacaoSubItemMaterial.SubItem = (new SubItemMaterialEntity(subitem));
            estrutura.RelacaoSubItemMaterial.Gestor = (new GestorEntity(gestor));



            bool isUsado = classe.VerificaSubItemUtilizado(subitem);
            if (isUsado)
                estrutura.ListaErro.Add("Este subItem já foi movimentado, e suas relações com itens siafisicos não podem ser excluídas.");

            if (estrutura.ExcluirRelacaoSubItemMaterial())
            {
                this.View.PopularGrid();
                this.ExcluidoSucesso();
                this.View.ExibirMensagem("Registro excluído com sucesso!");
            }
            else
                this.View.ExibirMensagem("Não foi possível excluir, verifique as mensagens!");
            this.View.ListaErros = estrutura.ListaErro;
        }

        public void Imprimir()
        {
            //RelatorioEntity.Id = (int)RelatorioEnum.RelacaoItemSubitem;
            //RelatorioEntity.Nome = "rptRelacaoItemSubitem.rdlc";
            //RelatorioEntity.DataSet = "dsMaterialItemSubitem";

            RelatorioEntity relatorioImpressao = new RelatorioEntity();
            relatorioImpressao.Id = (int)RelatorioEnum.RelacaoItemSubitem;
            relatorioImpressao.Nome = "rptRelacaoItemSubitem.rdlc";
            relatorioImpressao.DataSet = "dsMaterialItemSubitem";

            relatorioImpressao.Parametros = this.View.ParametrosRelatorio;
            this.View.DadosRelatorio = relatorioImpressao;

            this.View.ExibirRelatorio();
        }
    }
}
