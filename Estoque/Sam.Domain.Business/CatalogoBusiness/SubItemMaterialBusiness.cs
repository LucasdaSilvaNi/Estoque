using System;
using System.Collections.Generic;
using System.Linq;
using Sam.Common.Util;
using Sam.Domain.Entity;
using Sam.ServiceInfraestructure;
using System.Transactions;


namespace Sam.Domain.Business
{
    public partial class CatalogoBusiness : BaseBusiness
    {
        #region Sub Item de Material

        private SubItemMaterialEntity subitem = new SubItemMaterialEntity();

        public SubItemMaterialEntity SubItemMaterial
        {
            get { return subitem; }
            set { subitem = value; }
        }

        public bool SalvarSubItemMaterial(string codigoSubItem, string acao)
        {
            this.Service<ISubItemMaterialService>().Entity = this.SubItemMaterial;
            this.Service<ISubItemMaterialService>().Entity.UnidadeFornecimento = this.UnidadeFornecimento;
            this.ConsistirSubItemMaterial(codigoSubItem, acao);
            if (this.Consistido)
            {
                this.Service<ISubItemMaterialService>().Salvar();
            }
            return this.Consistido;
        }


        public void SelectSubItemMaterial(int _id)
        {

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.SubItemMaterial = this.Service<ISubItemMaterialService>().Select(_id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public void SelectSubItemMaterialPorCodigo(long? _codigo, int gestorId)
        {
            if (_codigo == null)
                throw new Exception("Não foi possivel realizar uma busca de subitens por código");

            this.Service<ISubItemMaterialService>().Entity = new SubItemMaterialEntity();
            this.SubItemMaterial = this.Service<ISubItemMaterialService>().ListarSubItemMaterial(a => a.Codigo == _codigo.Value && a.Gestor.Id == gestorId).FirstOrDefault();
        }

        public SubItemMaterialEntity SelectSubItemMaterialRetorno(int _id)
        {
            return this.Service<ISubItemMaterialService>().Select(_id);
        }

        public void SelectSubItemMaterialAlmox(int _id, int _idAlmox)
        {
            this.SubItemMaterial = this.Service<ISubItemMaterialService>().SelectAlmox(_id, _idAlmox);
        }

        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado, int item)
        {
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemSaldoByAlmox(almoxarifado, item);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemSaldoByAlmox(int almoxarifado)
        {
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemSaldoByAlmox(almoxarifado);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemMaterial(int _itemId, int _gestorId, bool? IndAtividade)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
                    IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().Listar(_itemId, _gestorId, IndAtividade);
                    this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<SubItemMaterialEntity> ListarSubItem(int _subItemId, int _gestorId)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
                    IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(_subItemId, _gestorId);
                    this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        public IList<SubItemMaterialEntity> ListarSubItemPorAlmox(int _itemId, int _almoxarifadoId)
        {
            return this.Service<ISubItemMaterialService>().ListarSubItemPorAlmox(_itemId, _almoxarifadoId);
        }


        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestorId, int _almoxarifadoId)
        {
            this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(_itemId, _gestorId, _almoxarifadoId);
            this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _grupoId, int _classeId, int _materialId, int _itemId, int _gestorId, int _almoxarifadoId, int iNaturezaDespesa_ID, Int64? _SubItemCodigo, Int64? _Itemcodigo, int _indicadorId, int _saldoId)
        {
            this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(_grupoId, _classeId, _materialId, _itemId, _gestorId, _almoxarifadoId, iNaturezaDespesa_ID, _SubItemCodigo, _Itemcodigo, _indicadorId, _saldoId);
            this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public bool atualizarAlmoxSaldo(int _gestorId, int _almoxarifadoId, bool _indicadorDisponivel)
        {

            bool retorno = this.Service<ISubItemMaterialService>().atualizarAlmoxSaldo(_gestorId, _almoxarifadoId, _indicadorDisponivel);

            return retorno;
        }


        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId)
        {
            this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(_materialId, _itemId, _gestorId, _almoxarifadoId);
            this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemByAlmoxItemMaterial(int almoxarifado, int itemId, string natDespesa)
        {
            return this.Service<ISubItemMaterialService>().ListarSubItemByAlmoxItemMaterial(almoxarifado, itemId, natDespesa);
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmoxarifadoItemNatureza(int almoxarifado, int itemCodigo, string natDespesa)
        {
            return this.Service<ISubItemMaterialService>().ListarSubItemAlmoxarifadoItemNatureza(almoxarifado, itemCodigo, natDespesa);
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmoxPorMaterial(int _materialId, int _gestorId, int _almoxarifadoId)
        {
            this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
            SubItemMaterialEntity subItem = new SubItemMaterialEntity();
            subitem.ItemMaterial = new ItemMaterialEntity();
            subitem.ItemMaterial.Material = new MaterialEntity(_materialId);
            subitem.Gestor = new GestorEntity(_gestorId);
            subitem.AlmoxarifadoId = _almoxarifadoId;
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(subitem);
            this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemAlmox(int _itemId, int _gestorId)
        {
            this.Service<ISubItemMaterialService>().SkipRegistros = this.SkipRegistros;
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemAlmox(_itemId, _gestorId);
            this.TotalRegistros = this.Service<ISubItemMaterialService>().TotalRegistros();
            return retorno;
        }


        public IList<SubItemMaterialEntity> ListarSubItemMaterialTodosCod(int _itemId, int _gestorId)
        {
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarTodosCod(_itemId, _gestorId);
            return retorno;
        }

        //public IList<SubItemMaterialEntity> ListarSubItemMaterialTodosCod()
        //{
        //    this.Service<ISubItemMaterialService>().Entity = new SubItemMaterialEntity();
        //    IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarTodosCod();
        //    return retorno;
        //}

        public IList<SubItemMaterialEntity> ListarSubItemMaterial(System.Linq.Expressions.Expression<Func<SubItemMaterialEntity, bool>> where)
        {
            this.Service<ISubItemMaterialService>().Entity = new SubItemMaterialEntity();
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().Listar(where);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarSubItemMaterialTodosCodPorItemUgeAlmox(int _itemCodigo, int _ugeId, int _almoxId)
        {
            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarTodosCodPorItemUgeAlmox(_itemCodigo, _ugeId, _almoxId);
                    return retorno;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }
        }

        //public IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemId, int _ugeId, int _almoxId, int _codigoNaturezadespesa)
        public IList<SubItemMaterialEntity> ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(int _itemMaterialCodigo, int _ugeId, int _almoxId, int gestorId, int _codigoNaturezadespesa)
        {
            IList<SubItemMaterialEntity> retorno = null;

            using (TransactionScope tras = new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    //IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(_itemId, _ugeId, _almoxId, _codigoNaturezadespesa);
                    retorno = this.Service<ISubItemMaterialService>().ListarSubItemMaterialPorItemMaterialUgeAlmoxNaturezaDespesa(_itemMaterialCodigo, _ugeId, _almoxId, gestorId, _codigoNaturezadespesa);
                }
                catch (Exception excErro)
                {
                    throw new Exception(excErro.Message);
                }
                finally
                {
                    tras.Complete();
                }
            }

            return retorno;
        }

        public IList<SubItemMaterialEntity> ListarCatalogoGestor(int gestorId, int naturezaDespesaCodigo, int itemCodigo, long subitemCodigo)
        {
            IList<SubItemMaterialEntity> lstRetorno = this.Service<ISubItemMaterialService>().ListarCatalogoGestor(gestorId, naturezaDespesaCodigo, itemCodigo, subitemCodigo);
            return lstRetorno;
        }

        public IList<SubItemMaterialEntity> ImprimirSubitensMaterial(int _itemId, int _gestorId)
        {
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().Imprimir(_itemId, _gestorId);
            return retorno;
        }

        public IList<SubItemMaterialEntity> ImprimirSubitensMaterial(int _gestorId)
        {
            this.Service<ISubItemMaterialService>().Entity = new SubItemMaterialEntity();
            this.Service<ISubItemMaterialService>().Entity.Gestor = new GestorEntity(_gestorId);
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().Imprimir();
            return retorno;
        }

        public IList<SubItemMaterialEntity> ImprimirGerenciaCatalogo(int _materialId, int _itemId, int _gestorId, int _almoxarifadoId)
        {
            IList<SubItemMaterialEntity> retorno = this.Service<ISubItemMaterialService>().Imprimir(_materialId, _itemId, _gestorId, _almoxarifadoId);
            return retorno;
        }

        public bool SalvarSubItemAlmox()
        {
            this.Service<ISubItemMaterialService>().Entity = this.SubItemMaterial;
            this.Service<ISubItemMaterialService>().Entity.UnidadeFornecimento = this.UnidadeFornecimento;
            this.ConsistirSubItemAlmox();
            if (this.Consistido)
            {
                this.Service<ISubItemMaterialService>().SalvarAlmox();
            }
            return this.Consistido;
        }

        public bool ExcluirSubItemMaterial()
        {
            this.Service<ISubItemMaterialService>().Entity = this.SubItemMaterial;
            if (this.Consistido)
            {
                try
                {
                    this.Service<ISubItemMaterialService>().Excluir();
                }
                catch (Exception ex)
                {
                    TratarErro(ex);
                }
            }
            return this.Consistido;
        }

        public void ConsistirSubItemAlmox()
        {
            if (this.SubItemMaterial.EstoqueMinimo < 0)
                this.ListaErro.Add("É obrigatório informar o Estoque Mínimo.");

            if (this.SubItemMaterial.EstoqueMaximo < 0)
                this.ListaErro.Add("É obrigatório informar o Estoque Máximo.");

            if (this.SubItemMaterial.EstoqueMaximo < this.SubItemMaterial.EstoqueMinimo)
                this.ListaErro.Add("Estoque Mínimo inválido.");
        }

        public void ConsistirSubItemMaterial(string codigoSubItem, string acao)
        {
            //Tira todos os espaços dos campos do tipo string
            Sam.Common.Util.TratamentoDados.Trim<SubItemMaterialEntity>(ref this.subitem);

            if (this.SubItemMaterial.ItemMaterial.Id == null || this.SubItemMaterial.ItemMaterial.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Item.");

            if (this.SubItemMaterial.NaturezaDespesa.Id == null || this.SubItemMaterial.NaturezaDespesa.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar a Natureza da Despesa.");

            //if (this.SubItemMaterial.ContaAuxiliar.Id == null || this.SubItemMaterial.ContaAuxiliar.Id.Value < 1)
            //    this.ListaErro.Add("É obrigatório informar a Conta Auxiliar.");

            if (this.SubItemMaterial.Gestor.Id == null || this.SubItemMaterial.Gestor.Id.Value < 1)
                this.ListaErro.Add("É obrigatório informar o Gestor.");

            if (this.SubItemMaterial.Codigo < 1)
                this.ListaErro.Add("É obrigatório informar o Código.");

            if (string.IsNullOrEmpty(this.SubItemMaterial.Descricao))
                this.ListaErro.Add("É obrigatório informar a Descrição.");


            if (this.ListaErro.Count == 0)
            {

                if (!codigoSubItem.Contains(this.SubItemMaterial.Codigo.ToString()) || acao == "Novo")
                {
                    if (this.Service<ISubItemMaterialService>().ExisteCodigoInformado())
                        this.ListaErro.Add("Código já existente.");
                }
            }
        }

        public bool VerificarSubitensInativos(int almoxarifadoId)
        {
            bool blnRetorno = false;
            IList<SubItemMaterialEntity> lstRetorno = null;

            lstRetorno = this.Service<IFechamentoMensalService>().VerificarSubitensInativos(almoxarifadoId);

            if (lstRetorno.IsNotNull() && lstRetorno.Count > 0)
            {
                foreach (var subItem in lstRetorno)
                    ListaErro.Add(String.Format("Subitem {0} - {1} não está ativo no catálogo do almoxarifado.", subItem.Codigo, subItem.Descricao).ToUpperInvariant());

                ListaErro.Add("Favor regularizar cadastro dos subitens apontados.".ToUpperInvariant());
            }

            blnRetorno = (this.ListaErro.IsNotNull() && this.ListaErro.Count > 0);

            return blnRetorno;
        }

        #endregion
        public IList<SubItemMaterialEntity> ListarItensEstoque(int filtro, int idAlmoxarifado)
        {
            IList<SubItemMaterialEntity> retorno = Service<ISubItemMaterialService>().ListarItensEstoque(filtro, idAlmoxarifado);

            return retorno;
        }

    }
}
